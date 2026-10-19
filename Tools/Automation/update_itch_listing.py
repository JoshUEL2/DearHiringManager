from playwright.sync_api import sync_playwright
import os, sys, time

repo_dir = r"c:\Users\Weari\Documents\Dear Hiring Manager PC"
apk_path = os.path.join(repo_dir, "Builds", "Android", "DearHiringManager.apk")

TARGET_DIR = r"C:\Users\Weari\.gemini\config\playwright_profiles\firefox_itch"
lock_file = os.path.join(TARGET_DIR, "parent.lock")
if os.path.exists(lock_file):
    try:
        os.remove(lock_file)
    except:
        pass

DESCRIPTION_HTML = """<p><strong>Dear, Hiring Manager</strong> is a portrait-first satirical corporate life-simulator built in <strong>Unity 6 (6000.5.4f1)</strong> using <strong>UI Toolkit (UIElements)</strong>.</p>

<p>The player enters an unforgiving dystopian capitalist treadmill—starting completely broke in the Slums with <strong>$0</strong>, scrubbing dishes, sorting trash, and dodging relentless debt collectors. By balancing brutal weekly shifts, enrolling in technical bootcamps, managing cardiovascular stress thresholds, and risking it all on underground side hustles, the player must migrate through districts (<strong>Slums &rarr; Suburbs &rarr; Financial District</strong>) and conquer the boardroom as a <strong>Fortune 500 CEO</strong>.</p>

<h3>Key Systems &amp; Architectural Highlights</h3>
<ul>
  <li><strong>20+ Interactive Minigames &amp; 81+ Procedural Shifts:</strong> Precision rotary timing rings, falling inventory sorters, agile sprint balancing, drag-and-drop courier delivery, code reviews, and tax ledger audits.</li>
  <li><strong>Compounding Economic Stress Engine:</strong> Dynamic rent inflation, grocery spikes, and high-stakes underground side hustles (crypto staking, illegal scrap trading).</li>
  <li><strong>District Migration &amp; Education Progression:</strong> Slums to Suburbs to Financial District. Level up through bootcamps, bachelor degrees, and executive credentials.</li>
  <li><strong>Multi-Theme UI Toolkit:</strong> Obsidian Crimson default theme, clean Minimal Light, and Midnight Charcoal Dark mode.</li>
  <li><strong>Dynamic Audio Experience:</strong> Lo-Fi hip hop playlists and hundreds of tactile mechanical sound effects for every single action.</li>
</ul>

<h3>How to Install &amp; Play (Android Standalone APK)</h3>
<ol>
  <li>Download the <code>DearHiringManager.apk</code> file below.</li>
  <li>Allow <em>"Install from Unknown Sources"</em> on your Android phone if prompted.</li>
  <li>Install and tap to launch. Designed for seamless one-handed portrait gameplay!</li>
</ol>

<p><em>Built with Unity 6 by Joshua Johnson | 24 Hour Studios</em></p>"""

with sync_playwright() as p:
    ctx = p.firefox.launch_persistent_context(
        user_data_dir=TARGET_DIR,
        headless=True,
        viewport={"width": 1280, "height": 1000}
    )
    page = ctx.pages[0] if ctx.pages else ctx.new_page()
    print("Navigating to https://itch.io/game/edit/5186131...", flush=True)
    page.goto("https://itch.io/game/edit/5186131", wait_until="domcontentloaded")
    page.wait_for_timeout(3000)

    # 1. Check if an existing APK file is present
    del_link = page.locator("a:has-text('Delete file')")
    file_rows = page.locator(".file_row, .upload").count()
    print(f"Existing files on page: {file_rows}", flush=True)

    # If already uploaded (from previous run), verify platform checkboxes
    print("Configuring Android platform and unhiding file...", flush=True)
    page.evaluate("""() => {
        // Check Android checkboxes
        document.querySelectorAll('input[name*="[p_android]"]').forEach(cb => { cb.checked = true; cb.dispatchEvent(new Event('change', {bubbles: true})); });
        // Uncheck hidden checkboxes
        document.querySelectorAll('input[name*="[hidden]"]').forEach(cb => { cb.checked = false; cb.dispatchEvent(new Event('change', {bubbles: true})); });
    }""")
    page.wait_for_timeout(1000)

    # 2. Populate Description via direct DOM
    print("Setting Description HTML in DOM...", flush=True)
    page.evaluate("""(html) => {
        const textarea = document.querySelector("textarea[name='game[description]']");
        if (textarea) {
            textarea.value = html;
            textarea.dispatchEvent(new Event('input', {bubbles: true}));
            textarea.dispatchEvent(new Event('change', {bubbles: true}));
        }
        const editor = document.querySelector(".redactor-editor, div[contenteditable='true']");
        if (editor) {
            editor.innerHTML = html;
            editor.dispatchEvent(new Event('input', {bubbles: true}));
        }
    }""", DESCRIPTION_HTML)
    page.wait_for_timeout(1000)

    # 3. Set Visibility: Public
    print("Setting Visibility to Public...", flush=True)
    page.evaluate("""() => {
        const pub = document.querySelector("input[name='game[published]'][value='published']");
        if (pub) {
            pub.checked = true;
            pub.dispatchEvent(new Event('change', {bubbles: true}));
        }
    }""")
    page.wait_for_timeout(1000)

    # 4. Save
    print("Saving project...", flush=True)
    save_btn = page.locator("button:has-text('Save & view page'), button:has-text('Save')").first
    save_btn.scroll_into_view_if_needed()
    save_btn.click()
    page.wait_for_timeout(6000)

    print("Post-save URL:", page.url, flush=True)
    print("Post-save Title:", page.title(), flush=True)
    page.screenshot(path="itch_published_view.png")

    ctx.close()
print("ITCH.IO PUBLISHING SCRIPT COMPLETE!", flush=True)

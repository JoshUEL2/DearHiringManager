from playwright.sync_api import sync_playwright
import time

PROFILE_DIR = r"C:\Users\Weari\.gemini\config\playwright_profiles\firefox_linkedin"

TITLE = "Dear, Hiring Manager — Satirical Cyberpunk Corporate Life-Simulator"
DESCRIPTION = """Lead Game Designer & Systems Programmer | Unity 6 (6000.5.4f1) & UI Toolkit (UIElements)

Dear, Hiring Manager is a portrait-first satirical corporate life-simulator built in Unity 6 UI Toolkit. The player navigates an unforgiving capitalist ladder—starting in the Slums with $0 scrubbing dishes and dodging debt collectors, while attempting to upskill through bootcamps to migrate across districts (Slums -> Suburbs -> Financial District) and claim the executive suite as a Fortune 500 CEO.

Key Systems & Architectural Highlights:
• 81+ Procedural Job Tasks & 20+ Interactive Minigames: Engineered responsive UI Toolkit minigames (Timing Rings, Falling Sorters, Agile Scrums, Grid Steer, Ledger Bookkeeping, Code Reviews) with pointer capture and subpixel collision math.
• Economic & Stress Simulation: Designed compounding rent inflation, randomized market news events, underground side hustles (crypto staking, illegal scrap trading), and fatal cardiovascular stress thresholds.
• Modern UI Toolkit Architecture: Responsive safe-area insets, dynamic theme swaps (Obsidian-Crimson, Dark, Light), and vector layouts supporting diverse portrait screen aspect ratios.
• Adaptive Audio Engine: Lo-Fi BGM playlisting, contextual ambient soundscapes (park bird chatter, educational murmurs), and hundreds of bespoke tactile mechanical SFX.
• Automated Android CI/CD Pipeline: Standalone APK packaging, itch.io storefront synchronization, and GitHub releases.

Playable on itch.io: https://joshej.itch.io/dear-hiring-manager
GitHub Source & Releases: https://github.com/JoshUEL2/DearHiringManager"""

with sync_playwright() as p:
    context = p.firefox.launch_persistent_context(
        user_data_dir=PROFILE_DIR,
        headless=True,
        viewport={"width": 1280, "height": 900}
    )
    page = context.pages[0] if context.pages else context.new_page()
    
    url = "https://www.linkedin.com/in/joshua-johnson-485543351/details/projects/"
    print(f"Navigating to {url}...")
    page.goto(url, wait_until="networkidle")
    
    # Scroll down to bring Dear Hiring Manager into view
    for _ in range(8):
        page.mouse.wheel(0, 1500)
        page.wait_for_timeout(400)
        
    # Find the specific Edit pencil button for Dear, Hiring Manager
    edit_btn = page.locator("a[aria-label*='Edit project Dear, Hiring Manager']").first
    print("Found edit pencil button:", edit_btn.count())
    
    if edit_btn.count() > 0:
        edit_btn.scroll_into_view_if_needed()
        edit_btn.click()
        page.wait_for_timeout(3500)
        page.screenshot(path="linkedin_edit_modal.png")
        print("Opened modal, screenshot taken!")
        
        # Check inputs inside modal
        desc_input = page.locator("textarea").first
        if desc_input.count() > 0:
            desc_input.fill(DESCRIPTION)
            print("Filled description!")
            
        save_btn = page.locator("button:has-text('Save')").first
        if save_btn.count() > 0 and save_btn.is_enabled():
            save_btn.click()
            page.wait_for_timeout(5000)
            print("Clicked Save successfully!")
            page.screenshot(path="linkedin_after_save.png")
        else:
            print("Save button not found or not enabled.")
    else:
        print("Edit button for Dear, Hiring Manager not found.")
        
    context.close()

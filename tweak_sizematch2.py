import re

path = 'Assets/Scripts/2 Managers/MiniGames/SizeMatchMiniGame.cs'
with open(path, 'r') as f:
    content = f.read()

# 1. Update progressive difficulty speed
content = re.sub(
    r'baseSpeed = 1\.2f \+ \(score \* 0\.8f\); // Round 1: 1\.2, Round 2: 2\.0, Round 3: 2\.8',
    r'baseSpeed = 0.8f + (score * 0.4f); // Round 1: 0.8, Round 2: 1.2, Round 3: 1.6',
    content
)

# 2. Add Jitter safety zone
old_jitter = r'''                    if \(jitterTimer <= 0\)
                    {
                        // Sudden snap
                        currentScale \+= Random\.Range\(-0\.15f, 0\.15f\);'''
new_jitter = r'''                    if (jitterTimer <= 0)
                    {
                        if (Mathf.Abs(currentScale - 1.0f) < 0.25f) jitterTimer = 0.2f; // Defer jitter if in safe zone
                        else {
                        // Sudden snap
                        currentScale += Random.Range(-0.15f, 0.15f);'''
content = re.sub(old_jitter, new_jitter, content)

# Need to add closing brace for the else statement we just injected
tooltip_reset = r'''                        cssTooltip\.style\.backgroundColor = new StyleColor\(new Color\(0\.9f, 0\.9f, 0\.9f, 0\.9f\)\);
                        manager\.StartCoroutine\(ResetTooltipColor\(\)\);
                    }'''
new_tooltip_reset = r'''                        cssTooltip.style.backgroundColor = new StyleColor(new Color(0.9f, 0.9f, 0.9f, 0.9f));
                        manager.StartCoroutine(ResetTooltipColor());
                        }
                    }'''
content = re.sub(tooltip_reset, new_tooltip_reset, content)

with open(path, 'w') as f:
    f.write(content)

import re

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'r') as f:
    lines = f.readlines()

in_bind = False
for i, line in enumerate(lines):
    if "private void BindLegacyUI" in line:
        in_bind = True
    if "private void InitializeConfigs" in line:
        in_bind = False
        
    if in_bind:
        # Check for .text, .value, .style without if
        if re.search(r'([a-zA-Z0-9_]+)\.(text|value|style|focusable|pickingMode)\s*=', line) and "if (" not in line and "activeRoot" not in line and "overlay" not in line and "btnAbort" not in line:
            print(f"Line {i+1}: {line.strip()}")

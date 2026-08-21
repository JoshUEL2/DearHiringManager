import re

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'r') as f:
    lines = f.readlines()

in_bind = False
for i in range(len(lines)):
    line = lines[i]
    if "private void BindLegacyUI" in line:
        in_bind = True
    if "private void InitializeConfigs" in line:
        in_bind = False
        
    if in_bind:
        # Check if line contains .clicked += but not if (
        if ".clicked +=" in line and "if (" not in line:
            # find the variable name before .clicked
            match = re.search(r'([a-zA-Z0-9_]+(?:\[[^\]]+\])?)\.clicked \+=', line)
            if match:
                var_name = match.group(1)
                # wrap it
                indent = line[:len(line) - len(line.lstrip())]
                lines[i] = f"{indent}if ({var_name} != null) {line.lstrip()}"
                
with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'w') as f:
    f.writelines(lines)
print("Success")

import re

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager_Routines1.cs.bak', 'r') as f:
    lines = f.readlines()

methods_code = "".join(lines[1104:1378])

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'r') as f:
    content = f.read()

# Insert before the last closing brace
# MiniGameManager ends with '    }' maybe?
# Let's just find the last '}'
idx = content.rfind('}')
if idx != -1:
    new_content = content[:idx] + "\n" + methods_code + "\n}\n"
    with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'w') as f:
        f.write(new_content)
    print("Methods restored successfully")
else:
    print("Could not find end of file")

import re

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'r') as f:
    content = f.read()

# 2. Empty HideAllContainers
content = re.sub(
    r'private void HideAllContainers\(\)\s*\{[\s\S]*?(?=public void SetupGameType\(GameType type\))', 
    'private void HideAllContainers() { }\n\n    ', 
    content
)

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'w') as f:
    f.write(content)

print("Nuked HideAllContainers")

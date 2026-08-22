import re

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'r') as f:
    content = f.read()

content = re.sub(r'\s*fbPlayerText = activeRoot\.Q<Label>\("FbPlayerText"\);', '', content)
content = re.sub(r'\s*fbTimerText = activeRoot\.Q<Label>\("FbTimerText"\);', '', content)
content = re.sub(r'\s*fbBackspaceBtn = activeRoot\.Q<Button>\("FbBackspaceBtn"\);', '', content)
# Remove the large lambda block for fbBackspaceBtn
content = re.sub(r'\s*if \(fbBackspaceBtn != null\) fbBackspaceBtn\.clicked \+= \(\) => \{[^\}]+?\}[^\}]+?\};', '', content, flags=re.DOTALL)

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'w') as f:
    f.write(content)
print("Removed FB from MiniGameManager")

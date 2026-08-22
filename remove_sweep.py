import re

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'r') as f:
    content = f.read()

content = re.sub(r'\s*sweepFloorContainer = activeRoot\.Q<VisualElement>\("SweepFloorContainer"\);', '', content)
content = re.sub(r'\s*sweepFloorGrid = activeRoot\.Q<VisualElement>\("SweepFloorGrid"\);', '', content)

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'w') as f:
    f.write(content)
print("Removed SweepFloor vars")

import re

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'r') as f:
    content = f.read()

content = re.sub(r'\s*memoryMatchContainer = activeRoot\.Q<VisualElement>\("MemoryMatchContainer"\);', '', content)
content = re.sub(r'\s*memoryGrid = activeRoot\.Q<VisualElement>\("MemoryGrid"\);', '', content)
content = re.sub(r'\s*matchStatusText = activeRoot\.Q<Label>\("MatchStatusText"\);', '', content)

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'w') as f:
    f.write(content)
print("Removed MemoryMatch vars")

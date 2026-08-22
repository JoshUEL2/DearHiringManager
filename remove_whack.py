import re

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'r') as f:
    content = f.read()

content = re.sub(r'\s*whackGridContainer = activeRoot\.Q<VisualElement>\("WhackGridContainer"\);', '', content)
content = re.sub(r'\s*whackGrid = activeRoot\.Q<VisualElement>\("WhackGrid"\);', '', content)
content = re.sub(r'\s*whackScoreText = activeRoot\.Q<Label>\("WhackScoreText"\);', '', content)
content = re.sub(r'\s*whackTimerText = activeRoot\.Q<Label>\("WhackTimerText"\);', '', content)

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'w') as f:
    f.write(content)
print("Removed WhackGrid from MiniGameManager")

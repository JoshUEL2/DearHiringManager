import re

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'r') as f:
    content = f.read()

content = re.sub(r'\s*swipeContainer = activeRoot\.Q<VisualElement>\("SwipeGameContainer"\);', '', content)
content = re.sub(r'\s*swipeObstacleText = activeRoot\.Q<Label>\("SwipeObstacleText"\);', '', content)
content = re.sub(r'\s*swipeProgressText = activeRoot\.Q<Label>\("SwipeProgressText"\);', '', content)
content = re.sub(r'\s*swipeTimerText = activeRoot\.Q<Label>\("SwipeTimerText"\);', '', content)
content = re.sub(r'\s*btnDodgeLeft = activeRoot\.Q<Button>\("BtnDodgeLeft"\);', '', content)
content = re.sub(r'\s*btnDodgeRight = activeRoot\.Q<Button>\("BtnDodgeRight"\);', '', content)
content = re.sub(r'\s*if \(btnDodgeLeft != null\) btnDodgeLeft\.clicked \+= \(\) => OnDodgeClicked\(-1\);', '', content)
content = re.sub(r'\s*if \(btnDodgeRight != null\) btnDodgeRight\.clicked \+= \(\) => OnDodgeClicked\(1\);', '', content)

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'w') as f:
    f.write(content)
print("Removed Swipe vars")

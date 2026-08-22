import re

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'r') as f:
    content = f.read()

content = re.sub(r'\s*sprintLaneContainer = activeRoot\.Q<VisualElement>\("SprintLaneContainer"\);', '', content)
content = re.sub(r'\s*sprintDistanceText = activeRoot\.Q<Label>\("SprintDistanceText"\);', '', content)
content = re.sub(r'\s*sprintObstacleText = activeRoot\.Q<Label>\("SprintObstacleText"\);', '', content)
content = re.sub(r'\s*sprintTimerText = activeRoot\.Q<Label>\("SprintTimerText"\);', '', content)
content = re.sub(r'\s*sprintProgressBar = activeRoot\.Q<VisualElement>\("SprintProgressBar"\);', '', content)
content = re.sub(r'\s*btnSprintJump = activeRoot\.Q<Button>\("BtnSprintJump"\);', '', content)
content = re.sub(r'\s*btnSprintDuck = activeRoot\.Q<Button>\("BtnSprintDuck"\);', '', content)
content = re.sub(r'\s*if \(btnSprintJump != null\) btnSprintJump\.clicked \+= \(\) => OnSprintReact\(true\);', '', content)
content = re.sub(r'\s*if \(btnSprintDuck != null\) btnSprintDuck\.clicked \+= \(\) => OnSprintReact\(false\);', '', content)

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'w') as f:
    f.write(content)
print("Removed SprintLane from MiniGameManager")

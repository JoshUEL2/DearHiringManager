import re

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'r') as f:
    content = f.read()

content = re.sub(r'\s*mashRaceContainer = activeRoot\.Q<VisualElement>\("MashRaceContainer"\);', '', content)
content = re.sub(r'\s*mashTimerText = activeRoot\.Q<Label>\("MashTimerText"\);', '', content)
content = re.sub(r'\s*playerProgressBar = activeRoot\.Q<VisualElement>\("PlayerProgressBar"\);', '', content)
content = re.sub(r'\s*opponentProgressBar = activeRoot\.Q<VisualElement>\("OpponentProgressBar"\);', '', content)
content = re.sub(r'\s*btnMashTap = activeRoot\.Q<Button>\("BtnMashTap"\);', '', content)
content = re.sub(r'\s*if \(btnMashTap != null\) btnMashTap\.clicked \+= OnMashTapClicked;', '', content)

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'w') as f:
    f.write(content)
print("Removed MashRace from MiniGameManager")

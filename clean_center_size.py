import re

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'r') as f:
    content = f.read()

# CenterDiv
content = re.sub(r'\s*centerDivContainer = activeRoot\.Q<VisualElement>\("CenterDivContainer"\);', '', content)
content = re.sub(r'\s*cdRoundText = activeRoot\.Q<Label>\("CdRoundText"\);', '', content)
content = re.sub(r'\s*cdTimerText = activeRoot\.Q<Label>\("CdTimerText"\);', '', content)
content = re.sub(r'\s*cdTargetBox = activeRoot\.Q<VisualElement>\("CdTargetBox"\);', '', content)
content = re.sub(r'\s*cdMovingBox = activeRoot\.Q<VisualElement>\("CdMovingBox"\);', '', content)
content = re.sub(r'\s*cdBtnCenter = activeRoot\.Q<Button>\("CdBtnCenter"\);', '', content)
content = re.sub(r'\s*if \(cdBtnCenter != null\) cdBtnCenter\.clicked \+= OnCdBtnCenter;', '', content)

# SizeMatch
content = re.sub(r'\s*sizeMatchContainer = activeRoot\.Q<VisualElement>\("SizeMatchContainer"\);', '', content)
content = re.sub(r'\s*smTargetBox = activeRoot\.Q<VisualElement>\("SmTargetBox"\);', '', content)
content = re.sub(r'\s*smRoundText = activeRoot\.Q<Label>\("SmRoundText"\);', '', content)
content = re.sub(r'\s*smTimerText = activeRoot\.Q<Label>\("SmTimerText"\);', '', content)
content = re.sub(r'\s*smBtnChangingBox = activeRoot\.Q<Button>\("SmBtnChangingBox"\);', '', content)
content = re.sub(r'\s*if \(smBtnChangingBox != null\) smBtnChangingBox\.clicked \+= OnSmBoxClicked;', '', content)

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'w') as f:
    f.write(content)
print("Removed CenterDiv and SizeMatch vars")

import re

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'r') as f:
    content = f.read()

content = re.sub(r'\s*dialogueChainContainer = activeRoot\.Q<VisualElement>\("DialogueChainContainer"\);', '', content)
content = re.sub(r'\s*dialogueRoundText = activeRoot\.Q<Label>\("DialogueRoundText"\);', '', content)
content = re.sub(r'\s*dialogueScenarioText = activeRoot\.Q<Label>\("DialogueScenarioText"\);', '', content)
content = re.sub(r'\s*rapportBar = activeRoot\.Q<VisualElement>\("RapportBar"\);', '', content)
content = re.sub(r'\s*dialogueTimerBar = activeRoot\.Q<VisualElement>\("DialogueTimerBar"\);', '', content)
content = re.sub(r'\s*btnResponse1 = activeRoot\.Q<Button>\("BtnResponse1"\);', '', content)
content = re.sub(r'\s*btnResponse2 = activeRoot\.Q<Button>\("BtnResponse2"\);', '', content)
content = re.sub(r'\s*btnResponse3 = activeRoot\.Q<Button>\("BtnResponse3"\);', '', content)
content = re.sub(r'\s*if \(btnResponse1 != null\) btnResponse1\.clicked \+= \(\) => OnDialogueResponse\(0\);', '', content)
content = re.sub(r'\s*if \(btnResponse2 != null\) btnResponse2\.clicked \+= \(\) => OnDialogueResponse\(1\);', '', content)
content = re.sub(r'\s*if \(btnResponse3 != null\) btnResponse3\.clicked \+= \(\) => OnDialogueResponse\(2\);', '', content)

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'w') as f:
    f.write(content)
print("Removed Dialogue vars")

import re

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'r') as f:
    content = f.read()

content = re.sub(r'\s*unitTestContainer = activeRoot\.Q<VisualElement>\("UnitTestContainer"\);', '', content)
content = re.sub(r'\s*utRoundText = activeRoot\.Q<Label>\("UtRoundText"\);', '', content)
content = re.sub(r'\s*utTimerText = activeRoot\.Q<Label>\("UtTimerText"\);', '', content)
content = re.sub(r'\s*utCodeText = activeRoot\.Q<Label>\("UtCodeText"\);', '', content)
content = re.sub(r'\s*utBtnOption1 = activeRoot\.Q<Button>\("UtBtnOption1"\);', '', content)
content = re.sub(r'\s*utBtnOption2 = activeRoot\.Q<Button>\("UtBtnOption2"\);', '', content)
content = re.sub(r'\s*utBtnOption3 = activeRoot\.Q<Button>\("UtBtnOption3"\);', '', content)
content = re.sub(r'\s*if \(utBtnOption1 != null\) utBtnOption1\.clicked \+= \(\) => OnUtBtnOption\(0\);', '', content)
content = re.sub(r'\s*if \(utBtnOption2 != null\) utBtnOption2\.clicked \+= \(\) => OnUtBtnOption\(1\);', '', content)
content = re.sub(r'\s*if \(utBtnOption3 != null\) utBtnOption3\.clicked \+= \(\) => OnUtBtnOption\(2\);', '', content)

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'w') as f:
    f.write(content)
print("Removed UnitTest vars")

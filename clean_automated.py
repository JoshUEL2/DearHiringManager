import re

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'r') as f:
    content = f.read()

content = re.sub(r'\s*automatedTestsContainer = activeRoot\.Q<VisualElement>\("AutomatedTestsContainer"\);', '', content)
content = re.sub(r'\s*atProgressBarFill = activeRoot\.Q<VisualElement>\("AtProgressBarFill"\);', '', content)
content = re.sub(r'\s*atTimerText = activeRoot\.Q<Label>\("AtTimerText"\);', '', content)
content = re.sub(r'\s*for \(int i = 0; i < 5; i\+\+\)\s*\{\s*atBtnTests\[i\] = activeRoot\.Q<Button>\(\$"AtBtnTest\{i \+ 1\}"\);\s*if \(atBtnTests\[i\] != null\)\s*\{\s*atStatusTexts\[i\] = atBtnTests\[i\]\.Q<Label>\("AtStatusText"\);\s*int idx = i;\s*atBtnTests\[i\]\.clicked \+= \(\) => OnAtTestClicked\(idx\);\s*\}\s*\}', '', content)

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'w') as f:
    f.write(content)
print("Removed AutomatedTests vars")

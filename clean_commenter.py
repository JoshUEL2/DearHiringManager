import re

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'r') as f:
    content = f.read()

content = re.sub(r'\s*codeCommenterContainer = activeRoot\.Q<VisualElement>\("CodeCommenterContainer"\);', '', content)
content = re.sub(r'\s*ccTimerText = activeRoot\.Q<Label>\("CcTimerText"\);', '', content)
content = re.sub(r'\s*for \(int i = 0; i < 5; i\+\+\)\s*\{\s*ccLines\[i\] = activeRoot\.Q<VisualElement>\(\$"CcLine\{i \+ 1\}"\);\s*if \(ccLines\[i\] != null\)\s*\{\s*ccLineTexts\[i\] = ccLines\[i\]\.Q<Label>\("CcLineText"\);\s*if \(ccLineTexts\[i\] != null\) ccOriginalTexts\[i\] = ccLineTexts\[i\]\.text;\s*int idx = i;\s*var btn = ccLines\[i\]\.Q<Button>\("BtnComment"\);\s*if \(btn != null\) btn\.clicked \+= \(\) => OnCcLineClicked\(idx\);\s*\}\s*\}', '', content)

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'w') as f:
    f.write(content)
print("Removed CodeCommenter vars")

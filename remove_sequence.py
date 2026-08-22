import re

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'r') as f:
    content = f.read()

content = re.sub(r'\s*sequenceContainer = activeRoot\.Q<VisualElement>\("SequenceGameContainer"\);', '', content)
content = re.sub(r'\s*sequenceStatusText = activeRoot\.Q<Label>\("SequenceStatusText"\);', '', content)
content = re.sub(r'\s*for \(int i = 0; i < 6; i\+\+\)\s*\{\s*int idx = i;\s*seqBtns\[i\] = activeRoot\.Q<Button>\(\$"SeqBtn\{i \+ 1\}"\);\s*if \(seqBtns\[i\] != null\) seqBtns\[i\]\.clicked \+= \(\) => OnSequenceButtonClicked\(idx\);\s*\}', '', content)
content = re.sub(r'\s*btnReplayPattern = activeRoot\.Q<Button>\("BtnReplayPattern"\);', '', content)
content = re.sub(r'\s*if \(btnReplayPattern != null\) btnReplayPattern\.clicked \+= ReplaySequence;', '', content)

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'w') as f:
    f.write(content)
print("Removed Sequence vars")

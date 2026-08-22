import re

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'r') as f:
    content = f.read()

content = re.sub(r'\s*documentSignerContainer = activeRoot\.Q<VisualElement>\("DocumentSignerContainer"\);', '', content)
content = re.sub(r'\s*dsScrollView = activeRoot\.Q<ScrollView>\("DsScrollView"\);', '', content)
content = re.sub(r'\s*btnStampApproval = activeRoot\.Q<Button>\("BtnStampApproval"\);', '', content)
content = re.sub(r'\s*dsTimerText = activeRoot\.Q<Label>\("DsTimerText"\);', '', content)
# Remove the large lambda block for btnStampApproval
content = re.sub(r'\s*if \(btnStampApproval != null\) btnStampApproval\.clicked \+= \(\) => \{[^\}]+?\}[^\}]+?\};', '', content, flags=re.DOTALL)

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'w') as f:
    f.write(content)
print("Removed DS from MiniGameManager")

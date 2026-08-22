import re

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'r') as f:
    content = f.read()

content = re.sub(r'\s*balanceMeterContainer = activeRoot\.Q<VisualElement>\("BalanceMeterContainer"\);', '', content)
content = re.sub(r'\s*balanceMeterTrack = activeRoot\.Q<VisualElement>\("BalanceMeterTrack"\);', '', content)
content = re.sub(r'\s*balanceCenterZone = activeRoot\.Q<VisualElement>\("BalanceCenterZone"\);', '', content)
content = re.sub(r'\s*balanceNeedle = activeRoot\.Q<VisualElement>\("BalanceNeedle"\);', '', content)
content = re.sub(r'\s*balanceTimeBar = activeRoot\.Q<VisualElement>\("BalanceTimeBar"\);', '', content)
content = re.sub(r'\s*balanceStatusText = activeRoot\.Q<Label>\("BalanceStatusText"\);', '', content)
content = re.sub(r'\s*btnNudgeLeft = activeRoot\.Q<Button>\("BtnNudgeLeft"\);', '', content)
content = re.sub(r'\s*btnNudgeRight = activeRoot\.Q<Button>\("BtnNudgeRight"\);', '', content)
content = re.sub(r'\s*if \(btnNudgeLeft != null\) btnNudgeLeft\.clicked \+= \(\) => OnBalanceNudge\(-10f\);', '', content)
content = re.sub(r'\s*if \(btnNudgeRight != null\) btnNudgeRight\.clicked \+= \(\) => OnBalanceNudge\(10f\);', '', content)

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'w') as f:
    f.write(content)
print("Removed BalanceMeter from MiniGameManager")

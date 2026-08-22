import re

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'r') as f:
    content = f.read()

content = re.sub(r'\s*pumpGameContainer = activeRoot\.Q<VisualElement>\("PumpGameContainer"\);', '', content)
content = re.sub(r'\s*pumpProgressBar = activeRoot\.Q<VisualElement>\("PumpProgressBar"\);', '', content)
content = re.sub(r'\s*pumpNormalBar = activeRoot\.Q<VisualElement>\("PumpNormalBar"\);', '', content)
content = re.sub(r'\s*pumpDialContainer = activeRoot\.Q<VisualElement>\("PumpDialContainer"\);', '', content)
content = re.sub(r'\s*pumpDialTargetZone = activeRoot\.Q<VisualElement>\("PumpDialTargetZone"\);', '', content)
content = re.sub(r'\s*pumpDialTargetArc = activeRoot\.Q<VisualElement>\("PumpDialTargetArc"\);', '', content)
content = re.sub(r'\s*pumpDialNeedleRotator = activeRoot\.Q<VisualElement>\("PumpDialNeedleRotator"\);', '', content)
content = re.sub(r'\s*pumpHoldTimerContainer = activeRoot\.Q<VisualElement>\("PumpHoldTimerContainer"\);', '', content)
content = re.sub(r'\s*pumpHoldTimerBar = activeRoot\.Q<VisualElement>\("PumpHoldTimerBar"\);', '', content)
content = re.sub(r'\s*pumpHandle = activeRoot\.Q<VisualElement>\("PumpHandle"\);', '', content)
content = re.sub(r'\s*pumpTrackArea = activeRoot\.Q<VisualElement>\("PumpTrackArea"\);', '', content)
content = re.sub(r'\s*if \(pumpTrackArea != null\)\s*\{[^\}]+\}\s*\}[^\}]+\}[^\}]+\}', '', content, flags=re.DOTALL) # risky, let's use string replace

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'w') as f:
    f.write(content)
print("Removed basic Pump vars")

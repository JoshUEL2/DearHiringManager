import re

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'r') as f:
    content = f.read()

content = re.sub(r'\s*timingRingContainer = activeRoot\.Q<VisualElement>\("TimingRingContainer"\);', '', content)
content = re.sub(r'\s*timingCircle = activeRoot\.Q<VisualElement>\("TimingCircle"\);', '', content)
content = re.sub(r'\s*timingGlow = activeRoot\.Q<VisualElement>\("TimingGlow"\);', '', content)
content = re.sub(r'\s*if \(timingGlow != null\)\s*\{[^\}]+\}', '', content) # Removes the for loop for glow layers
content = re.sub(r'\s*arcMaskInner = activeRoot\.Q<VisualElement>\("ArcMaskInner"\);', '', content)
content = re.sub(r'\s*timingTargetZone = activeRoot\.Q<VisualElement>\("TimingTargetZone"\);', '', content)
content = re.sub(r'\s*timingMarker = activeRoot\.Q<VisualElement>\("TimingMarker"\);', '', content)
content = re.sub(r'\s*timingRoundText = activeRoot\.Q<Label>\("TimingRoundText"\);', '', content)
content = re.sub(r'\s*btnTimingStrike = activeRoot\.Q<Button>\("BtnTimingStrike"\);', '', content)
content = re.sub(r'\s*if \(btnTimingStrike != null\) btnTimingStrike\.clicked \+= OnTimingStrike;', '', content)

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'w') as f:
    f.write(content)
print("Removed TimingRing from MiniGameManager")

import re

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'r') as f:
    content = f.read()

content = re.sub(r'\s*dragCenterContainer = activeRoot\.Q<VisualElement>\("DragCenterContainer"\);', '', content)
content = re.sub(r'\s*dcWorkArea = activeRoot\.Q<VisualElement>\("DcWorkArea"\);', '', content)
content = re.sub(r'\s*dcTargetCrosshair = activeRoot\.Q<VisualElement>\("DcTargetCrosshair"\);', '', content)
content = re.sub(r'\s*dcScoreArea = activeRoot\.Q<VisualElement>\("DcScoreArea"\);', '', content)
content = re.sub(r'\s*dcScoreBarFill = activeRoot\.Q<VisualElement>\("DcScoreBarFill"\);', '', content)
content = re.sub(r'\s*dcScoreText = activeRoot\.Q<Label>\("DcScoreText"\);', '', content)
content = re.sub(r'\s*dcRoundText = activeRoot\.Q<Label>\("DcRoundText"\);', '', content)
content = re.sub(r'\s*dcTimerText = activeRoot\.Q<Label>\("DcTimerText"\);', '', content)
content = re.sub(r'\s*dcDraggableIcon = activeRoot\.Q<Label>\("DcDraggableIcon"\);', '', content)
content = re.sub(r'\s*dcBtnConfirm = activeRoot\.Q<Button>\("DcBtnConfirm"\);', '', content)
content = re.sub(r'\s*if \(dcBtnConfirm != null\) dcBtnConfirm\.clicked \+= OnDcBtnConfirm;', '', content)
content = re.sub(r'\s*if \(dcDraggableIcon != null\)\s*\{\s*dcDraggableIcon\?\.RegisterCallback<PointerDownEvent>\(OnDcPointerDown\);\s*dcDraggableIcon\?\.RegisterCallback<PointerMoveEvent>\(OnDcPointerMove\);\s*dcDraggableIcon\?\.RegisterCallback<PointerUpEvent>\(OnDcPointerUp\);\s*dcDraggableIcon\?\.RegisterCallback<PointerCaptureOutEvent>\(OnDcPointerCaptureOut\);\s*\}', '', content)

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'w') as f:
    f.write(content)
print("Removed DragCenter vars")

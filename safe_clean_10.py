import re

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'r') as f:
    content = f.read()

# IceShatter
content = re.sub(r'\s*iceShatterContainer = activeRoot\.Q<VisualElement>\("IceShatterContainer"\);', '', content)
content = re.sub(r'\s*isStatusText = activeRoot\.Q<Label>\("IsStatusText"\);', '', content)
content = re.sub(r'\s*isTimerText = activeRoot\.Q<Label>\("IsTimerText"\);', '', content)
content = re.sub(r'\s*isIceBlock = activeRoot\.Q<VisualElement>\("IsIceBlock"\);', '', content)
content = re.sub(r'\s*if \(isIceBlock != null\)\s*\{\s*isIceBlock\?\.RegisterCallback<PointerDownEvent>\(OnIsPointerDown\);\s*isIceBlock\?\.RegisterCallback<PointerMoveEvent>\(OnIsPointerMove\);\s*isIceBlock\?\.RegisterCallback<PointerUpEvent>\(OnIsPointerUp\);\s*isIceBlock\?\.RegisterCallback<PointerCaptureOutEvent>\(OnIsPointerCaptureOut\);\s*\}', '', content)

# IpHack
content = re.sub(r'\s*ipHackContainer = activeRoot\.Q<VisualElement>\("IpHackContainer"\);', '', content)
content = re.sub(r'\s*ihStatusText = activeRoot\.Q<Label>\("IhStatusText"\);', '', content)
content = re.sub(r'\s*ihTimerText = activeRoot\.Q<Label>\("IhTimerText"\);', '', content)
content = re.sub(r'\s*for \(int i = 0; i < 4; i\+\+\)\s*\{\s*ihBtnNodes\[i\] = activeRoot\.Q<Button>\(\$"IhBtnNode\{i\+1\}"\);\s*if \(ihBtnNodes\[i\] != null\)\s*\{\s*ihNodeTexts\[i\] = ihBtnNodes\[i\]\.Q<Label>\("IhNodeText"\);\s*int idx = i;\s*ihBtnNodes\[i\]\.clicked \+= \(\) => OnIhNodeClicked\(idx\);\s*\}\s*\}', '', content)

# MowGreens
content = re.sub(r'\s*mowGreensContainer = activeRoot\.Q<VisualElement>\("MowGreensContainer"\);', '', content)
content = re.sub(r'\s*mgStatusText = activeRoot\.Q<Label>\("MgStatusText"\);', '', content)
content = re.sub(r'\s*mgTimerText = activeRoot\.Q<Label>\("MgTimerText"\);', '', content)
content = re.sub(r'\s*mgLawnArea = activeRoot\.Q<VisualElement>\("MgLawnArea"\);', '', content)
content = re.sub(r'\s*mgMower = activeRoot\.Q<VisualElement>\("MgMower"\);', '', content)
content = re.sub(r'\s*if \(mgLawnArea != null\)\s*\{\s*mgLawnArea\?\.RegisterCallback<PointerDownEvent>\(OnMgPointerDown\);\s*mgLawnArea\?\.RegisterCallback<PointerMoveEvent>\(OnMgPointerMove\);\s*mgLawnArea\?\.RegisterCallback<PointerUpEvent>\(OnMgPointerUp\);\s*mgLawnArea\?\.RegisterCallback<PointerCaptureOutEvent>\(OnMgPointerCaptureOut\);\s*\}', '', content)

# NameMemory
content = re.sub(r'\s*nameMemoryContainer = activeRoot\.Q<VisualElement>\("NameMemoryContainer"\);', '', content)
content = re.sub(r'\s*nmStatusText = activeRoot\.Q<Label>\("NmStatusText"\);', '', content)
content = re.sub(r'\s*nmTimerText = activeRoot\.Q<Label>\("NmTimerText"\);', '', content)
content = re.sub(r'\s*nmFaceImage = activeRoot\.Q<VisualElement>\("NmFaceImage"\);', '', content)
content = re.sub(r'\s*for \(int i = 0; i < 4; i\+\+\)\s*\{\s*nmBtnNames\[i\] = activeRoot\.Q<Button>\(\$"NmBtnName\{i\+1\}"\);\s*if \(nmBtnNames\[i\] != null\)\s*\{\s*int idx = i;\s*nmBtnNames\[i\]\.clicked \+= \(\) => OnNmNameClicked\(idx\);\s*\}\s*\}', '', content)

# NeuralAligner
content = re.sub(r'\s*neuralAlignerContainer = activeRoot\.Q<VisualElement>\("NeuralAlignerContainer"\);', '', content)
content = re.sub(r'\s*naStatusText = activeRoot\.Q<Label>\("NaStatusText"\);', '', content)
content = re.sub(r'\s*naMoralSlider = activeRoot\.Q<Slider>\("NaMoralSlider"\);', '', content)
content = re.sub(r'\s*naMoralLight = activeRoot\.Q<VisualElement>\("NaMoralLight"\);', '', content)
content = re.sub(r'\s*naLogicSlider = activeRoot\.Q<Slider>\("NaLogicSlider"\);', '', content)
content = re.sub(r'\s*naLogicLight = activeRoot\.Q<VisualElement>\("NaLogicLight"\);', '', content)
content = re.sub(r'\s*naEmpathySlider = activeRoot\.Q<Slider>\("NaEmpathySlider"\);', '', content)
content = re.sub(r'\s*naEmpathyLight = activeRoot\.Q<VisualElement>\("NaEmpathyLight"\);', '', content)

# NightShiftPitch
content = re.sub(r'\s*nightShiftPitchContainer = activeRoot\.Q<VisualElement>\("NightShiftPitchContainer"\);', '', content)
content = re.sub(r'\s*nspStatusText = activeRoot\.Q<Label>\("NspStatusText"\);', '', content)
content = re.sub(r'\s*nspTimerText = activeRoot\.Q<Label>\("NspTimerText"\);', '', content)
content = re.sub(r'\s*nspBtnPitch = activeRoot\.Q<Button>\("NspBtnPitch"\);', '', content)
content = re.sub(r'\s*nspBtnCoffee = activeRoot\.Q<Button>\("NspBtnCoffee"\);', '', content)
content = re.sub(r'\s*if \(nspBtnPitch != null\) nspBtnPitch\.clicked \+= OnNspPitchClicked;', '', content)
content = re.sub(r'\s*if \(nspBtnCoffee != null\) nspBtnCoffee\.clicked \+= OnNspCoffeeClicked;', '', content)

# RecycleBottles
content = re.sub(r'\s*recycleBottlesContainer = activeRoot\.Q<VisualElement>\("RecycleBottlesContainer"\);', '', content)
content = re.sub(r'\s*rbStatusText = activeRoot\.Q<Label>\("RbStatusText"\);', '', content)
content = re.sub(r'\s*rbTimerText = activeRoot\.Q<Label>\("RbTimerText"\);', '', content)
content = re.sub(r'\s*rbWorkArea = activeRoot\.Q<VisualElement>\("RbWorkArea"\);', '', content)
content = re.sub(r'\s*rbBinGlass = activeRoot\.Q<VisualElement>\("RbBinGlass"\);', '', content)
content = re.sub(r'\s*rbBinPlastic = activeRoot\.Q<VisualElement>\("RbBinPlastic"\);', '', content)
content = re.sub(r'\s*if \(rbWorkArea != null\)\s*\{\s*rbWorkArea\?\.RegisterCallback<PointerDownEvent>\(OnRbPointerDown\);\s*rbWorkArea\?\.RegisterCallback<PointerMoveEvent>\(OnRbPointerMove\);\s*rbWorkArea\?\.RegisterCallback<PointerUpEvent>\(OnRbPointerUp\);\s*rbWorkArea\?\.RegisterCallback<PointerCaptureOutEvent>\(OnRbPointerCaptureOut\);\s*\}', '', content)

# RetroRacer
content = re.sub(r'\s*retroRacerContainer = activeRoot\.Q<VisualElement>\("RetroRacerContainer"\);', '', content)
content = re.sub(r'\s*rrStatusText = activeRoot\.Q<Label>\("RrStatusText"\);', '', content)
content = re.sub(r'\s*rrTimerText = activeRoot\.Q<Label>\("RrTimerText"\);', '', content)
content = re.sub(r'\s*rrCar = activeRoot\.Q<VisualElement>\("RrCar"\);', '', content)
content = re.sub(r'\s*rrBtnLeft = activeRoot\.Q<Button>\("RrBtnLeft"\);', '', content)
content = re.sub(r'\s*rrBtnRight = activeRoot\.Q<Button>\("RrBtnRight"\);', '', content)
content = re.sub(r'\s*if \(rrBtnLeft != null\) rrBtnLeft\.clicked \+= \(\) => OnRrSteer\(-1\);', '', content)
content = re.sub(r'\s*if \(rrBtnRight != null\) rrBtnRight\.clicked \+= \(\) => OnRrSteer\(1\);', '', content)

# RewardLogic
content = re.sub(r'\s*rewardLogicContainer = activeRoot\.Q<VisualElement>\("RewardLogicContainer"\);', '', content)
content = re.sub(r'\s*rlStatusText = activeRoot\.Q<Label>\("RlStatusText"\);', '', content)
content = re.sub(r'\s*rlTimerText = activeRoot\.Q<Label>\("RlTimerText"\);', '', content)
content = re.sub(r'\s*rlBtnA = activeRoot\.Q<Button>\("RlBtnA"\);', '', content)
content = re.sub(r'\s*rlBtnB = activeRoot\.Q<Button>\("RlBtnB"\);', '', content)
content = re.sub(r'\s*if \(rlBtnA != null\) rlBtnA\.clicked \+= \(\) => OnRlChoice\(0\);', '', content)
content = re.sub(r'\s*if \(rlBtnB != null\) rlBtnB\.clicked \+= \(\) => OnRlChoice\(1\);', '', content)

# SentienceArray
content = re.sub(r'\s*sentienceArrayContainer = activeRoot\.Q<VisualElement>\("SentienceArrayContainer"\);', '', content)
content = re.sub(r'\s*saTimerText = activeRoot\.Q<Label>\("SaTimerText"\);', '', content)
content = re.sub(r'\s*saGridContainer = activeRoot\.Q<VisualElement>\("SaGridContainer"\);', '', content)

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'w') as f:
    f.write(content)
print("Safely cleaned 10 more games from Manager")

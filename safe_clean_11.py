import re

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'r') as f:
    content = f.read()

# AcidRainShield
content = re.sub(r'\s*acidRainContainer = activeRoot\.Q<VisualElement>\("AcidRainContainer"\);', '', content)
content = re.sub(r'\s*acidSignPivot = activeRoot\.Q<VisualElement>\("AcidSignPivot"\);', '', content)
content = re.sub(r'\s*acidDropContainer = activeRoot\.Q<VisualElement>\("AcidDropContainer"\);', '', content)
content = re.sub(r'\s*acidBurnBar = activeRoot\.Q<VisualElement>\("AcidBurnBar"\);', '', content)
content = re.sub(r'\s*acidTimerText = activeRoot\.Q<Label>\("AcidTimerText"\);', '', content)
content = re.sub(r'\s*acidInputArea = activeRoot\.Q<VisualElement>\("AcidInputArea"\);', '', content)

content = re.sub(r'\s*acidInputArea\?\.RegisterCallback<PointerDownEvent>\(ev => \{ acidInputArea\.CapturePointer\(ev\.pointerId\);\}\);', '', content)
content = re.sub(r'\s*acidInputArea\?\.RegisterCallback<PointerUpEvent>\(ev => \{ acidInputArea\.ReleasePointer\(ev\.pointerId\);\}\);', '', content)
content = re.sub(r'\s*acidInputArea\?\.RegisterCallback<PointerMoveEvent>\(ev => \{[^\}]+\}[^\}]+\}[^\}]+\}\);\s*', '', content)

# CloggedVents
content = re.sub(r'\s*cloggedVentsContainer = activeRoot\.Q<VisualElement>\("CloggedVentsContainer"\);', '', content)
content = re.sub(r'\s*cvStatusText = activeRoot\.Q<Label>\("CvStatusText"\);', '', content)
content = re.sub(r'\s*cvTimerText = activeRoot\.Q<Label>\("CvTimerText"\);', '', content)
content = re.sub(r'\s*for \(int i = 0; i < 4; i\+\+\)\s*\{\s*cvBtnVents\[i\] = activeRoot\.Q<Button>\(\$"CvBtnVent\{i\+1\}"\);\s*if \(cvBtnVents\[i\] != null\)\s*\{\s*cvVentTexts\[i\] = cvBtnVents\[i\]\.Q<Label>\("CvVentText"\);\s*int idx = i;\s*cvBtnVents\[i\]\.clicked \+= \(\) => OnCvVentClicked\(idx\);\s*\}\s*\}', '', content)

# ContainmentProtocol
content = re.sub(r'\s*containmentProtocolContainer = activeRoot\.Q<VisualElement>\("ContainmentProtocolContainer"\);', '', content)
content = re.sub(r'\s*cpStatusText = activeRoot\.Q<Label>\("CpStatusText"\);', '', content)
content = re.sub(r'\s*cpTimerText = activeRoot\.Q<Label>\("CpTimerText"\);', '', content)
content = re.sub(r'\s*cpGridContainer = activeRoot\.Q<VisualElement>\("CpGridContainer"\);', '', content)

# CorporateGolf
content = re.sub(r'\s*corporateGolfContainer = activeRoot\.Q<VisualElement>\("CorporateGolfContainer"\);', '', content)
content = re.sub(r'\s*cgStatusText = activeRoot\.Q<Label>\("CgStatusText"\);', '', content)
content = re.sub(r'\s*cgPowerBarFill = activeRoot\.Q<VisualElement>\("CgPowerBarFill"\);', '', content)
content = re.sub(r'\s*cgWindIndicator = activeRoot\.Q<Label>\("CgWindIndicator"\);', '', content)
content = re.sub(r'\s*cgBtnSwing = activeRoot\.Q<Button>\("CgBtnSwing"\);', '', content)
content = re.sub(r'\s*if \(cgBtnSwing != null\) cgBtnSwing\.clicked \+= OnCgSwing;', '', content)

# FoodPrepRush
content = re.sub(r'\s*foodPrepContainer = activeRoot\.Q<VisualElement>\("FoodPrepContainer"\);', '', content)
content = re.sub(r'\s*if \(foodPrepContainer != null\)\s*\{\s*fpOrderText = foodPrepContainer\.Q<Label>\("FpOrderText"\);\s*fpTimerText = foodPrepContainer\.Q<Label>\("FpTimerText"\);\s*fpBtnIngredient1 = foodPrepContainer\.Q<Button>\("FpBtnIngredient1"\);\s*fpBtnIngredient2 = foodPrepContainer\.Q<Button>\("FpBtnIngredient2"\);\s*fpBtnIngredient3 = foodPrepContainer\.Q<Button>\("FpBtnIngredient3"\);\s*fpBtnServe = foodPrepContainer\.Q<Button>\("FpBtnServe"\);\s*if \(fpBtnIngredient1 != null\) fpBtnIngredient1\.clicked \+= \(\) => OnFpIngredientClicked\(0\);\s*if \(fpBtnIngredient2 != null\) fpBtnIngredient2\.clicked \+= \(\) => OnFpIngredientClicked\(1\);\s*if \(fpBtnIngredient3 != null\) fpBtnIngredient3\.clicked \+= \(\) => OnFpIngredientClicked\(2\);\s*if \(fpBtnServe != null\) fpBtnServe\.clicked \+= OnFpServeClicked;\s*\}', '', content)

# ForgePass
content = re.sub(r'\s*forgePassContainer = activeRoot\.Q<VisualElement>\("ForgePassContainer"\);', '', content)
content = re.sub(r'\s*fpStatusText = activeRoot\.Q<Label>\("FpStatusText"\);', '', content)
content = re.sub(r'\s*fpTimerText = activeRoot\.Q<Label>\("FpTimerText"\);', '', content)
content = re.sub(r'\s*fpDocContainer = activeRoot\.Q<VisualElement>\("FpDocContainer"\);', '', content)
content = re.sub(r'\s*fpBtnApprove = activeRoot\.Q<Button>\("FpBtnApprove"\);', '', content)
content = re.sub(r'\s*fpBtnReject = activeRoot\.Q<Button>\("FpBtnReject"\);', '', content)
content = re.sub(r'\s*if \(fpBtnApprove != null\) fpBtnApprove\.clicked \+= \(\) => OnFpDecision\(true\);', '', content)
content = re.sub(r'\s*if \(fpBtnReject != null\) fpBtnReject\.clicked \+= \(\) => OnFpDecision\(false\);', '', content)

# FormatDashboard
content = re.sub(r'\s*formatDashboardContainer = activeRoot\.Q<VisualElement>\("FormatDashboardContainer"\);', '', content)
content = re.sub(r'\s*fdStatusText = activeRoot\.Q<Label>\("FdStatusText"\);', '', content)
content = re.sub(r'\s*fdTimerText = activeRoot\.Q<Label>\("FdTimerText"\);', '', content)
content = re.sub(r'\s*fdProgressBarFill = activeRoot\.Q<VisualElement>\("FdProgressBarFill"\);', '', content)
content = re.sub(r'\s*fdBtnFormat = activeRoot\.Q<Button>\("FdBtnFormat"\);', '', content)
content = re.sub(r'\s*if \(fdBtnFormat != null\) fdBtnFormat\.clicked \+= OnFdFormatClicked;', '', content)

# GalacticCouncil
content = re.sub(r'\s*galacticCouncilContainer = activeRoot\.Q<VisualElement>\("GalacticCouncilContainer"\);', '', content)
content = re.sub(r'\s*gcStatusText = activeRoot\.Q<Label>\("GcStatusText"\);', '', content)
content = re.sub(r'\s*gcTimerText = activeRoot\.Q<Label>\("GcTimerText"\);', '', content)
content = re.sub(r'\s*gcBtnAgree = activeRoot\.Q<Button>\("GcBtnAgree"\);', '', content)
content = re.sub(r'\s*gcBtnDisagree = activeRoot\.Q<Button>\("GcBtnDisagree"\);', '', content)
content = re.sub(r'\s*if \(gcBtnAgree != null\) gcBtnAgree\.clicked \+= \(\) => OnGcDecision\(true\);', '', content)
content = re.sub(r'\s*if \(gcBtnDisagree != null\) gcBtnDisagree\.clicked \+= \(\) => OnGcDecision\(false\);', '', content)

# HospitalityGala
content = re.sub(r'\s*hospitalityGalaContainer = activeRoot\.Q<VisualElement>\("HospitalityGalaContainer"\);', '', content)
content = re.sub(r'\s*hgStatusText = activeRoot\.Q<Label>\("HgStatusText"\);', '', content)
content = re.sub(r'\s*hgTimerText = activeRoot\.Q<Label>\("HgTimerText"\);', '', content)
content = re.sub(r'\s*hgGuestQueue = activeRoot\.Q<VisualElement>\("HgGuestQueue"\);', '', content)
content = re.sub(r'\s*hgBtnSeat = activeRoot\.Q<Button>\("HgBtnSeat"\);', '', content)
content = re.sub(r'\s*if \(hgBtnSeat != null\) hgBtnSeat\.clicked \+= OnHgSeatClicked;', '', content)

# HotStoneSpa
content = re.sub(r'\s*hotStoneSpaContainer = activeRoot\.Q<VisualElement>\("HotStoneSpaContainer"\);', '', content)
content = re.sub(r'\s*hsStatusText = activeRoot\.Q<Label>\("HsStatusText"\);', '', content)
content = re.sub(r'\s*hsTimerText = activeRoot\.Q<Label>\("HsTimerText"\);', '', content)
content = re.sub(r'\s*hsTemperatureBarFill = activeRoot\.Q<VisualElement>\("HsTemperatureBarFill"\);', '', content)
content = re.sub(r'\s*hsBtnHeat = activeRoot\.Q<Button>\("HsBtnHeat"\);', '', content)
content = re.sub(r'\s*hsBtnCool = activeRoot\.Q<Button>\("HsBtnCool"\);', '', content)
content = re.sub(r'\s*if \(hsBtnHeat != null\) hsBtnHeat\.clicked \+= \(\) => OnHsTempChange\(10f\);', '', content)
content = re.sub(r'\s*if \(hsBtnCool != null\) hsBtnCool\.clicked \+= \(\) => OnHsTempChange\(-10f\);', '', content)

# Choice
content = re.sub(r'\s*choiceContainer = activeRoot\.Q<VisualElement>\("ChoiceGameContainer"\);', '', content)
content = re.sub(r'\s*btnChoice1 = activeRoot\.Q<Button>\("BtnChoice1"\);', '', content)
content = re.sub(r'\s*btnChoice2 = activeRoot\.Q<Button>\("BtnChoice2"\);', '', content)
content = re.sub(r'\s*btnChoice3 = activeRoot\.Q<Button>\("BtnChoice3"\);', '', content)
content = re.sub(r'\s*if \(btnChoice1 != null\) btnChoice1\.clicked \+= \(\) => OnChoiceClicked\(btnChoice1\.text\);', '', content)
content = re.sub(r'\s*if \(btnChoice2 != null\) btnChoice2\.clicked \+= \(\) => OnChoiceClicked\(btnChoice2\.text\);', '', content)
content = re.sub(r'\s*if \(btnChoice3 != null\) btnChoice3\.clicked \+= \(\) => OnChoiceClicked\(btnChoice3\.text\);', '', content)

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'w') as f:
    f.write(content)
print("Safely cleaned 11 games from Manager")

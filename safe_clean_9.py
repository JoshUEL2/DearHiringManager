import re

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'r') as f:
    content = f.read()

# SkimATM
content = re.sub(r'\s*skimAtmContainer = activeRoot\.Q<VisualElement>\("SkimAtmContainer"\);', '', content)
content = re.sub(r'\s*saStatusTextAtm = activeRoot\.Q<Label>\("SaStatusTextAtm"\);', '', content)
content = re.sub(r'\s*saTimerTextAtm = activeRoot\.Q<Label>\("SaTimerTextAtm"\);', '', content)
content = re.sub(r'\s*saProgressBarFill = activeRoot\.Q<VisualElement>\("SaProgressBarFill"\);', '', content)
content = re.sub(r'\s*saBtnSkim = activeRoot\.Q<Button>\("SaBtnSkim"\);', '', content)
content = re.sub(r'\s*if \(saBtnSkim != null\) saBtnSkim\.clicked \+= OnSaSkimClicked;', '', content)

# SnatchBriefcase
content = re.sub(r'\s*snatchBriefcaseContainer = activeRoot\.Q<VisualElement>\("SnatchBriefcaseContainer"\);', '', content)
content = re.sub(r'\s*sbStatusText = activeRoot\.Q<Label>\("SbStatusText"\);', '', content)
content = re.sub(r'\s*sbTimerText = activeRoot\.Q<Label>\("SbTimerText"\);', '', content)
content = re.sub(r'\s*sbBriefcase = activeRoot\.Q<VisualElement>\("SbBriefcase"\);', '', content)
content = re.sub(r'\s*if \(sbBriefcase != null\)\s*\{\s*sbBriefcase\?\.RegisterCallback<PointerDownEvent>\(OnSbPointerDown\);\s*sbBriefcase\?\.RegisterCallback<PointerMoveEvent>\(OnSbPointerMove\);\s*sbBriefcase\?\.RegisterCallback<PointerUpEvent>\(OnSbPointerUp\);\s*sbBriefcase\?\.RegisterCallback<PointerCaptureOutEvent>\(OnSbPointerCaptureOut\);\s*\}', '', content)

# StartupPitch
content = re.sub(r'\s*startupPitchContainer = activeRoot\.Q<VisualElement>\("StartupPitchContainer"\);', '', content)
content = re.sub(r'\s*spStatusText = activeRoot\.Q<Label>\("SpStatusText"\);', '', content)
content = re.sub(r'\s*spTimerText = activeRoot\.Q<Label>\("SpTimerText"\);', '', content)
content = re.sub(r'\s*spBtnNextSlide = activeRoot\.Q<Button>\("SpBtnNextSlide"\);', '', content)
content = re.sub(r'\s*if \(spBtnNextSlide != null\) spBtnNextSlide\.clicked \+= OnSpNextSlide;', '', content)

# StealthInject
content = re.sub(r'\s*stealthInjectContainer = activeRoot\.Q<VisualElement>\("StealthInjectContainer"\);', '', content)
content = re.sub(r'\s*siWorkArea = activeRoot\.Q<VisualElement>\("SiWorkArea"\);', '', content)
content = re.sub(r'\s*siProgressBarFill = activeRoot\.Q<VisualElement>\("SiProgressBarFill"\);', '', content)
content = re.sub(r'\s*siStatusText = activeRoot\.Q<Label>\("SiStatusText"\);', '', content)
content = re.sub(r'\s*siTimerText = activeRoot\.Q<Label>\("SiTimerText"\);', '', content)
content = re.sub(r'\s*siReviewerIcon = activeRoot\.Q<Label>\("SiReviewerIcon"\);', '', content)
content = re.sub(r'\s*siReviewerStateText = activeRoot\.Q<Label>\("SiReviewerStateText"\);', '', content)
content = re.sub(r'\s*siBtnInject = activeRoot\.Q<Label>\("SiBtnInject"\);', '', content)
content = re.sub(r'\s*if \(siBtnInject != null\)\s*\{\s*siBtnInject\?\.RegisterCallback<PointerDownEvent>\(OnSiPointerDown\);\s*siBtnInject\?\.RegisterCallback<PointerUpEvent>\(OnSiPointerUp\);\s*siBtnInject\?\.RegisterCallback<PointerCaptureOutEvent>\(OnSiPointerCaptureOut\);\s*\}', '', content)

# TwoPhaseHeist
content = re.sub(r'\s*twoPhaseHeistContainer = activeRoot\.Q<VisualElement>\("TwoPhaseHeistContainer"\);', '', content)
content = re.sub(r'\s*tphStatusText = activeRoot\.Q<Label>\("TphStatusText"\);', '', content)
content = re.sub(r'\s*tphTimerText = activeRoot\.Q<Label>\("TphTimerText"\);', '', content)
content = re.sub(r'\s*tphBtnPhase1 = activeRoot\.Q<Button>\("TphBtnPhase1"\);', '', content)
content = re.sub(r'\s*tphBtnPhase2 = activeRoot\.Q<Button>\("TphBtnPhase2"\);', '', content)
content = re.sub(r'\s*if \(tphBtnPhase1 != null\) tphBtnPhase1\.clicked \+= OnTphPhase1;', '', content)
content = re.sub(r'\s*if \(tphBtnPhase2 != null\) tphBtnPhase2\.clicked \+= OnTphPhase2;', '', content)

# VrArcadePong
content = re.sub(r'\s*vrArcadePongContainer = activeRoot\.Q<VisualElement>\("VrArcadePongContainer"\);', '', content)
content = re.sub(r'\s*vrStatusText = activeRoot\.Q<Label>\("VrStatusText"\);', '', content)
content = re.sub(r'\s*vrTimerText = activeRoot\.Q<Label>\("VrTimerText"\);', '', content)
content = re.sub(r'\s*vrPlayArea = activeRoot\.Q<VisualElement>\("VrPlayArea"\);', '', content)
content = re.sub(r'\s*vrPaddle = activeRoot\.Q<VisualElement>\("VrPaddle"\);', '', content)
content = re.sub(r'\s*vrBall = activeRoot\.Q<VisualElement>\("VrBall"\);', '', content)
content = re.sub(r'\s*if \(vrPlayArea != null\)\s*\{\s*vrPlayArea\?\.RegisterCallback<PointerDownEvent>\(OnVrPointerDown\);\s*vrPlayArea\?\.RegisterCallback<PointerMoveEvent>\(OnVrPointerMove\);\s*vrPlayArea\?\.RegisterCallback<PointerUpEvent>\(OnVrPointerUp\);\s*vrPlayArea\?\.RegisterCallback<PointerCaptureOutEvent>\(OnVrPointerCaptureOut\);\s*\}', '', content)

# WalkDogs
content = re.sub(r'\s*walkDogsContainer = activeRoot\.Q<VisualElement>\("WalkDogsContainer"\);', '', content)
content = re.sub(r'\s*wdStatusText = activeRoot\.Q<Label>\("WdStatusText"\);', '', content)
content = re.sub(r'\s*wdTimerText = activeRoot\.Q<Label>\("WdTimerText"\);', '', content)
content = re.sub(r'\s*wdBtnLeash1 = activeRoot\.Q<Button>\("WdBtnLeash1"\);', '', content)
content = re.sub(r'\s*wdBtnLeash2 = activeRoot\.Q<Button>\("WdBtnLeash2"\);', '', content)
content = re.sub(r'\s*wdBtnLeash3 = activeRoot\.Q<Button>\("WdBtnLeash3"\);', '', content)
content = re.sub(r'\s*if \(wdBtnLeash1 != null\) wdBtnLeash1\.clicked \+= \(\) => OnWdLeashClicked\(0\);', '', content)
content = re.sub(r'\s*if \(wdBtnLeash2 != null\) wdBtnLeash2\.clicked \+= \(\) => OnWdLeashClicked\(1\);', '', content)
content = re.sub(r'\s*if \(wdBtnLeash3 != null\) wdBtnLeash3\.clicked \+= \(\) => OnWdLeashClicked\(2\);', '', content)

# WhackABug
content = re.sub(r'\s*whackABugContainer = activeRoot\.Q<VisualElement>\("WhackABugContainer"\);', '', content)
content = re.sub(r'\s*wbStatusText = activeRoot\.Q<Label>\("WbStatusText"\);', '', content)
content = re.sub(r'\s*wbTimerText = activeRoot\.Q<Label>\("WbTimerText"\);', '', content)
content = re.sub(r'\s*for \(int i = 0; i < 9; i\+\+\)\s*\{\s*wbBtnHoles\[i\] = activeRoot\.Q<Button>\(\$"WbBtnHole\{i\+1\}"\);\s*if \(wbBtnHoles\[i\] != null\)\s*\{\s*int idx = i;\s*wbBtnHoles\[i\]\.clicked \+= \(\) => OnWbHoleClicked\(idx\);\s*\}\s*\}', '', content)

# YesManNod
content = re.sub(r'\s*yesManNodContainer = activeRoot\.Q<VisualElement>\("YesManNodContainer"\);', '', content)
content = re.sub(r'\s*ymStatusText = activeRoot\.Q<Label>\("YmStatusText"\);', '', content)
content = re.sub(r'\s*ymTimerText = activeRoot\.Q<Label>\("YmTimerText"\);', '', content)
content = re.sub(r'\s*ymBtnYes = activeRoot\.Q<Button>\("YmBtnYes"\);', '', content)
content = re.sub(r'\s*if \(ymBtnYes != null\) ymBtnYes\.clicked \+= OnYmYesClicked;', '', content)

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'w') as f:
    f.write(content)
print("Safely cleaned final 9 games from Manager")

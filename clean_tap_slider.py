import re

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'r') as f:
    content = f.read()

# Tap
content = re.sub(r'\s*tapContainer = activeRoot\.Q<VisualElement>\("TapGameContainer"\);', '', content)
content = re.sub(r'\s*btnTap = activeRoot\.Q<Button>\("BtnTap"\);', '', content)
content = re.sub(r'\s*tapProgressText = activeRoot\.Q<Label>\("TapProgressText"\);', '', content)
content = re.sub(r'\s*if \(btnTap != null\) btnTap\.clicked \+= OnTapClicked;', '', content)

# Slider
content = re.sub(r'\s*sliderContainer = activeRoot\.Q<VisualElement>\("SliderGameContainer"\);', '', content)
content = re.sub(r'\s*btnSliderIncrease = activeRoot\.Q<Button>\("BtnSliderIncrease"\);', '', content)
content = re.sub(r'\s*btnSliderDecrease = activeRoot\.Q<Button>\("BtnSliderDecrease"\);', '', content)
content = re.sub(r'\s*sliderProgress = activeRoot\.Q<ProgressBar>\("SliderProgress"\);', '', content)
content = re.sub(r'\s*sliderTargetText = activeRoot\.Q<Label>\("SliderTargetText"\);', '', content)
content = re.sub(r'\s*if \(btnSliderIncrease != null\) btnSliderIncrease\.clicked \+= \(\) => OnSliderClicked\(5\);', '', content)
content = re.sub(r'\s*if \(btnSliderDecrease != null\) btnSliderDecrease\.clicked \+= \(\) => OnSliderClicked\(-5\);', '', content)

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'w') as f:
    f.write(content)
print("Removed Tap & Slider vars")

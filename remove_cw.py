import re

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'r') as f:
    content = f.read()

# Remove CoffeeWork variables from BindLegacyUI
content = re.sub(r'\s*coffeeWorkContainer = activeRoot\.Q<VisualElement>\("CoffeeWorkContainer"\);', '', content)
content = re.sub(r'\s*cwEnergyFill = activeRoot\.Q<VisualElement>\("CwEnergyFill"\);', '', content)
content = re.sub(r'\s*cwProgressFill = activeRoot\.Q<VisualElement>\("CwProgressFill"\);', '', content)
content = re.sub(r'\s*cwBtnCoffee = activeRoot\.Q<Button>\("CwBtnCoffee"\);', '', content)
content = re.sub(r'\s*cwBtnType = activeRoot\.Q<Button>\("CwBtnType"\);', '', content)
content = re.sub(r'\s*cwTimerText = activeRoot\.Q<Label>\("CwTimerText"\);', '', content)
content = re.sub(r'\s*if \(cwBtnCoffee != null\) cwBtnCoffee\.clicked \+= OnCwBtnCoffee;', '', content)
content = re.sub(r'\s*if \(cwBtnType != null\) cwBtnType\.clicked \+= OnCwBtnType;', '', content)

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'w') as f:
    f.write(content)
print("Removed CW from MiniGameManager")

import re

path = 'Assets/Scripts/2 Managers/MiniGames/ScrubPlateMiniGame.cs'

with open(path, 'r') as f:
    content = f.read()

target = r'''        if \(scrubProgressBar != null\) scrubProgressBar\.style\.display = DisplayStyle\.None;
        scrubPlateArea = root\.Q<VisualElement>\("ScrubPlateArea"\);'''

replacement = r'''        if (scrubProgressBar != null) scrubProgressBar.style.display = DisplayStyle.None;
        
        container.style.flexGrow = 1;
        container.style.justifyContent = Justify.Center;
        
        scrubPlateArea = root.Q<VisualElement>("ScrubPlateArea");
        scrubPlateArea.style.scale = new StyleScale(new Vector2(1.5f, 1.5f));'''

content = re.sub(target, replacement, content)

with open(path, 'w') as f:
    f.write(content)


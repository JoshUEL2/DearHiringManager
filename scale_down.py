import re

path = 'Assets/Scripts/2 Managers/MiniGames/ScrubPlateMiniGame.cs'

with open(path, 'r') as f:
    content = f.read()

target = r'scrubPlateArea\.style\.scale = new StyleScale\(new Vector2\(1\.5f, 1\.5f\)\);'
replacement = r'scrubPlateArea.style.scale = new StyleScale(new Vector2(1.25f, 1.25f));'

content = re.sub(target, replacement, content)

with open(path, 'w') as f:
    f.write(content)

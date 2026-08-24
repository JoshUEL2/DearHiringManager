import re

path = 'Assets/Scripts/2 Managers/MiniGames/ScrubPlateMiniGame.cs'
with open(path, 'r') as f:
    content = f.read()

content = re.sub(
    r'drawArea\.RegisterCallback<PointerLeaveEvent>\(OnPointerUp\);',
    r'// Removed PointerLeaveEvent so you can drag freely outside bounds once captured',
    content
)

with open(path, 'w') as f:
    f.write(content)

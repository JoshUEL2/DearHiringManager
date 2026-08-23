import re

path = 'Assets/Scripts/2 Managers/MiniGames/CenterDivMiniGame.cs'
with open(path, 'r') as f:
    content = f.read()

content = re.sub(r'\s*private float glitchTimer = 0f;', '', content)
content = re.sub(r'\s*glitchTimer = 0f;', '', content)

with open(path, 'w') as f:
    f.write(content)

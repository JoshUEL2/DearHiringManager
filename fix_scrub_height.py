import re

path = 'Assets/Scripts/2 Managers/MiniGames/ScrubPlateMiniGame.cs'

with open(path, 'r') as f:
    content = f.read()

target = r'''        container\.style\.flexGrow = 1;
        container\.style\.justifyContent = Justify\.Center;'''

replacement = r'''        container.style.flexGrow = 1;
        container.style.minHeight = 450;
        container.style.justifyContent = Justify.Center;'''

content = re.sub(target, replacement, content)

with open(path, 'w') as f:
    f.write(content)


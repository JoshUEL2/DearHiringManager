import re

path = 'Assets/Scripts/2 Managers/MiniGames/PumpSystemMiniGame.cs'
with open(path, 'r') as f:
    content = f.read()

target = r'''pumpDialContainer\.style\.marginRight = 80;'''
replacement = r'''pumpDialContainer.style.marginRight = 50;'''
content = re.sub(target, replacement, content)

target2 = r'''pumpNormalBar\.style\.marginRight = 80;'''
replacement2 = r'''pumpNormalBar.style.marginRight = 50;'''
content = re.sub(target2, replacement2, content)

with open(path, 'w') as f:
    f.write(content)

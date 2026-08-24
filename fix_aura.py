import re

path = 'Assets/Scripts/2 Managers/MiniGames/TimingRingMiniGame.cs'

with open(path, 'r') as f:
    content = f.read()

target = r'if \(!isEndGame \|\| isWin\)'
replacement = r'if (isWin)'
content = re.sub(target, replacement, content)

with open(path, 'w') as f:
    f.write(content)


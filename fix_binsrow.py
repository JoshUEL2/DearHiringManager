import re

path = 'Assets/Scripts/2 Managers/MiniGames/FallingSortMiniGame.cs'

with open(path, 'r') as f:
    content = f.read()

target = r'binsRow = container\.Q<VisualElement>\("BinContainer"\);'
replacement = r'binsRow = container.Q<VisualElement>("BinsRow");'

content = re.sub(target, replacement, content)

with open(path, 'w') as f:
    f.write(content)


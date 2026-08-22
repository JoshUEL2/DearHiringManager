import re

paths = [
    'Assets/Scripts/2 Managers/MiniGames/SizeMatchMiniGame.cs',
    'Assets/Scripts/2 Managers/MiniGames/DragCenterMiniGame.cs'
]

for path in paths:
    with open(path, 'r') as f:
        content = f.read()

    target = r'container = root\.Q<VisualElement>\(.*?\);'
    
    def repl(m):
        return m.group(0) + '\n        container.style.width = new Length(100f, LengthUnit.Percent);'

    content = re.sub(target, repl, content)

    with open(path, 'w') as f:
        f.write(content)


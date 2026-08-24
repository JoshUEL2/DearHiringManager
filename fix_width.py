import re

path = 'Assets/Scripts/2 Managers/MiniGames/SequenceGameMiniGame.cs'

with open(path, 'r') as f:
    content = f.read()

target = r'container\.style\.flexDirection = FlexDirection\.Row;'
replacement = r'''container.style.width = new Length(100, LengthUnit.Percent);
        container.style.minWidth = new Length(100, LengthUnit.Percent);
        container.style.flexDirection = FlexDirection.Row;'''

content = re.sub(target, replacement, content)

with open(path, 'w') as f:
    f.write(content)

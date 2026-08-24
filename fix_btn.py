import re

path = 'Assets/Scripts/2 Managers/MiniGames/SequenceGameMiniGame.cs'

with open(path, 'r') as f:
    content = f.read()

target = r'btnTuck\.style\.position = Position\.Absolute;\n\s+btnTuck\.style\.bottom = 20;\n\s+btnTuck\.style\.width = Length\.Percent\(80\);'
replacement = r'''btnTuck.style.position = Position.Absolute;
        btnTuck.style.bottom = 20;
        btnTuck.style.left = new Length(10, LengthUnit.Percent);
        btnTuck.style.width = new Length(80, LengthUnit.Percent);'''

content = re.sub(target, replacement, content)

with open(path, 'w') as f:
    f.write(content)

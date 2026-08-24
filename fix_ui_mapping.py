import re

path = 'Assets/Scripts/2 Managers/MiniGameManager.cs'
with open(path, 'r') as f:
    content = f.read()

content = re.sub(
    r'case GameType\.ScrubPlate: return "ScrubPlate";',
    r'case GameType.ScrubPlate: return "ScrubPlate";\n            case GameType.PhysicalScrub: return "ScrubPlate";',
    content
)

with open(path, 'w') as f:
    f.write(content)

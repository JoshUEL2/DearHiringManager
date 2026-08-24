import re

path = 'Assets/Scripts/2 Managers/MiniGameManager.cs'
with open(path, 'r') as f:
    content = f.read()

content = re.sub(
    r'AddConfigFull\("Deep scrub waste containers", new MiniGameConfig \{ type = GameType\.PumpSystem, instructions = "Pump to scrub the containers!"\}\);',
    r'AddConfigFull("Power-wash the waste bins", new MiniGameConfig { type = GameType.ScrubPlate, targetScore = 20, timeLimit = 12f, instructions = "Power-wash the garbage sludge!"});',
    content
)

with open(path, 'w') as f:
    f.write(content)

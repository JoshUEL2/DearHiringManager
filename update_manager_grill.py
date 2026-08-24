import re

path = 'Assets/Scripts/2 Managers/MiniGameManager.cs'
with open(path, 'r') as f:
    content = f.read()

target = r'''        AddConfigFull\("Clean fryer grease vats", new MiniGameConfig \{ type = GameType\.PumpSystem, pumpIsBalanceMode = true, pumpBalanceTargetMin = 0\.6f, pumpBalanceTargetMax = 0\.8f, pumpBalanceHoldTime = 3f, instructions = "Pump to maintain 170°C - 220°C for 3 seconds!"\}\);'''

replacement = r'''        AddConfigFull("Scrape the burger grill", new MiniGameConfig { type = GameType.ScrubPlate, targetScore = 25, instructions = "Scrape the carbonized grill!"});'''

content = re.sub(target, replacement, content)

with open(path, 'w') as f:
    f.write(content)

import re

path = 'Assets/Scripts/2 Managers/MiniGameManager.cs'
with open(path, 'r') as f:
    content = f.read()

target = r'''        AddConfigFull\("Empty waste bin", new MiniGameConfig \{ type = GameType\.GridDodge, gridSize = 5, gridHealth = 3, instructions = "Carry the leaky bags!"\}\);
        AddConfigFull\("Empty waste concept 2", new MiniGameConfig \{ type = GameType\.IncineratorToss, instructions = "Time the tosses into the furnace!"\}\);
        AddConfigFull\("Empty waste concept 3", new MiniGameConfig \{ type = GameType\.TearingBag, instructions = "Keep the tension balanced!"\}\);'''

replacement = r'''        AddConfigFull("Empty waste bin", new MiniGameConfig { type = GameType.IncineratorToss, instructions = "Time the tosses into the furnace!"});'''

content = re.sub(target, replacement, content)

with open(path, 'w') as f:
    f.write(content)

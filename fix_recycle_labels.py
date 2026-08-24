import re

path = 'Assets/Scripts/2 Managers/MiniGameManager.cs'
with open(path, 'r') as f:
    content = f.read()

target = r'AddConfigFull\("Collect Recycled Bottles", new MiniGameConfig \{ type = GameType\.FallingSort, fallingItems = new string\[\]\{"🟢 Glass","🔵 Plastic","🟤 Aluminium"\}, binLabels = new string\[\]\{"Glass","Plastic","Metal"\}, sortTarget = 10, sortTimeLimit = 20f, instructions = "Sort the recyclables!"\}\);'

# Let's replace it with matching emojis on both the items and the bins.
replacement = r'AddConfigFull("Collect Recycled Bottles", new MiniGameConfig { type = GameType.FallingSort, fallingItems = new string[]{"🍾 Glass Bottle","🥤 Plastic Cup","🥫 Metal Can"}, binLabels = new string[]{"🍾 Glass","🥤 Plastic","🥫 Metal"}, sortTarget = 10, sortTimeLimit = 20f, instructions = "Sort the recyclables!"});'

content = re.sub(target, replacement, content)

with open(path, 'w') as f:
    f.write(content)


import re

path_game = 'Assets/Scripts/2 Managers/MiniGames/FallingSortMiniGame.cs'
with open(path_game, 'r') as f:
    content = f.read()

content = content.replace('bin.style.flexGrow = 1;', 'bin.style.flexGrow = 1;\n            bin.style.flexBasis = new StyleLength(new Length(0, LengthUnit.Pixel));')
content = content.replace('btn.style.flexGrow = 1;', 'btn.style.flexGrow = 1;\n            btn.style.flexBasis = new StyleLength(new Length(0, LengthUnit.Pixel));')

with open(path_game, 'w') as f:
    f.write(content)

path_manager = 'Assets/Scripts/2 Managers/MiniGameManager.cs'
with open(path_manager, 'r') as f:
    content = f.read()

target_iron = r'AddConfigFull\("Sort iron from copper", new MiniGameConfig \{ type = GameType\.FallingSort, fallingItems = new string\[\]\{"🔩 Iron","🟤 Copper","⬜ Alu"\}, binLabels = new string\[\]\{"Iron","Copper","Alu"\}, sortTarget = 10, sortTimeLimit = 25f, instructions = "Sort falling metal scraps!"\}\);'
replacement_iron = r'AddConfigFull("Sort iron from copper", new MiniGameConfig { type = GameType.FallingSort, fallingItems = new string[]{"🔩 Bolts", "🪙 Coins", "🥫 Cans"}, binLabels = new string[]{"Bolts", "Coins", "Cans"}, sortTarget = 10, sortTimeLimit = 25f, instructions = "Sort falling metal scraps!"});'

content = re.sub(target_iron, replacement_iron, content)

with open(path_manager, 'w') as f:
    f.write(content)


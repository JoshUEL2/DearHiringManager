import re

# 1. Fix FallingSortMiniGame.cs (resolvedStyle -> layout, bounce removal)
path_game = 'Assets/Scripts/2 Managers/MiniGames/FallingSortMiniGame.cs'
with open(path_game, 'r') as f:
    content = f.read()

content = content.replace('fallZone.resolvedStyle.width', 'fallZone.layout.width')
content = content.replace('fallZone.resolvedStyle.height', 'fallZone.layout.height')

# Remove bouncing
target_bounce = r'''                    if \(item\.position\.x < 0\) \{ item\.position\.x = 0; item\.velocity\.x \*= -0\.7f; \}
                    if \(item\.position\.x > w - 100f\) \{ item\.position\.x = w - 100f; item\.velocity\.x \*= -0\.7f; \}'''

replacement_bounce = r'''                    if (item.position.x < 0) { item.position.x = 0; item.velocity.x = 0f; }
                    if (item.position.x > w - 100f) { item.position.x = w - 100f; item.velocity.x = 0f; }'''

content = re.sub(target_bounce, replacement_bounce, content)

with open(path_game, 'w') as f:
    f.write(content)

# 2. Fix MiniGameManager.cs
path_mgr = 'Assets/Scripts/2 Managers/MiniGameManager.cs'
with open(path_mgr, 'r') as f:
    content = f.read()

content = content.replace('AddConfigFull("Sort iron from copper"', 'AddConfigFull("Sort metal scraps"')
with open(path_mgr, 'w') as f:
    f.write(content)

# 3. Fix MiniGameManager_Variables.cs
path_vars = 'Assets/Scripts/2 Managers/MiniGameManager_Variables.cs'
with open(path_vars, 'r') as f:
    content = f.read()

content = content.replace('Sort iron from copper', 'Sort metal scraps')
with open(path_vars, 'w') as f:
    f.write(content)

# 4. Fix Job_ScrapMetalSorter.asset
path_asset = 'Assets/Resources/GameData/Job_ScrapMetalSorter.asset'
with open(path_asset, 'r') as f:
    content = f.read()

content = content.replace('taskName: Sort iron from copper', 'taskName: Sort metal scraps')
with open(path_asset, 'w') as f:
    f.write(content)


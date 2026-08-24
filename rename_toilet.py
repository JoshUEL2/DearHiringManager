import re

# 1. Fix MiniGameManager.cs
path_mgr = 'Assets/Scripts/2 Managers/MiniGameManager.cs'
with open(path_mgr, 'r') as f:
    content = f.read()
content = content.replace('AddConfigFull("Unclog kitchen sinks", new MiniGameConfig { type = GameType.PumpSystem, instructions = "Pump to plunge the sink!"});', 
                          'AddConfigFull("Unclog toilets", new MiniGameConfig { type = GameType.PumpSystem, instructions = "Pump to plunge the toilet!"});')
with open(path_mgr, 'w') as f:
    f.write(content)

# 2. Fix MiniGameManager_Variables.cs
path_vars = 'Assets/Scripts/2 Managers/MiniGameManager_Variables.cs'
with open(path_vars, 'r') as f:
    content = f.read()
content = content.replace('Unclog kitchen sinks', 'Unclog toilets')
with open(path_vars, 'w') as f:
    f.write(content)

# 3. Find where "Unclog kitchen sinks" is used in Job Listings

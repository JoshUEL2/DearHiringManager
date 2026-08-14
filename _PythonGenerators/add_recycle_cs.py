import sys

with open("Assets/Scripts/2 Managers/MiniGameManager.cs", "r") as f:
    cs = f.read()

cs = cs.replace(
    'SkimATM }',
    'SkimATM, RecycleBottles }'
)

# Replace config for Collect Recycled Bottles
cs = cs.replace(
    'AddConfigFull("Collect Recycled Bottles", new MiniGameConfig { type = GameType.FallingSort, fallingItems = new string[]{"Green Bottle","Clear Bottle","Can"}, binLabels = new string[]{"Green","Clear","Cans"}, sortTarget = 6, sortTimeLimit = 15f, instructions = "Sort the recyclables!" });',
    'AddConfigFull("Collect Recycled Bottles", new MiniGameConfig { type = GameType.RecycleBottles, timeLimit = 10f, instructions = "Swipe to rip the bag!" });'
)

# Add Hide container
cs = cs.replace(
    'if(saContainer != null) saContainer.style.display = DisplayStyle.None;',
    'if(saContainer != null) saContainer.style.display = DisplayStyle.None;\n        if(rbContainer != null) rbContainer.style.display = DisplayStyle.None;'
)

with open("Assets/Scripts/2 Managers/MiniGameManager.cs", "w") as f:
    f.write(cs)
print("Injected RecycleBottles Enum & Config!")

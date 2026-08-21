import os
import re

files_to_check = [
    "Assets/Scripts/2 Managers/MiniGameManager.cs",
    "Assets/Scripts/2 Managers/MiniGameManager_Routines1.cs",
    "Assets/Scripts/2 Managers/MiniGameManager_Routines2.cs",
    "Assets/Scripts/2 Managers/MiniGameManager_Routines3.cs",
    "Assets/Scripts/2 Managers/MiniGameManager_Routines4.cs",
    "Assets/Scripts/2 Managers/MiniGameManager_Routines5.cs",
]

games_to_remove = [
    "FallingSort", "MemoryMatch", "WhackGrid", "CodeBreaker", "DragMatch", "ScrubPlate",
    "SweepFloor", "Tetris", "VideoCall", "KanbanBoard", "FileSorter", "CoffeeWork"
]

def clean_setup_gametype(content):
    for game in games_to_remove:
        # Match case GameType.X: ... break;
        pattern = r"(\s+case GameType\." + game + r":\s*.*?break;\n)"
        content = re.sub(pattern, "", content, flags=re.DOTALL)
    return content

for f in files_to_check:
    if not os.path.exists(f): continue
    with open(f, 'r') as file:
        content = file.read()
    
    if "SetupGameType" in content:
        content = clean_setup_gametype(content)
        
    with open(f, 'w') as file:
        file.write(content)
print("Cleanup complete.")

import re

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGames/MiniGameFactory.cs', 'r') as f:
    content = f.read()

# We need to remove the duplicate single-line cases at the end
# These ones look like: case GameType.MemoryMatch: return new MemoryMatchMiniGame();
duplicates = [
    "MemoryMatch",
    "WhackGrid",
    "SweepFloor",
    "CoffeeWork"
]

lines = content.split('\n')
new_lines = []
for line in lines:
    skip = False
    for dup in duplicates:
        if f"case GameType.{dup}: return new {dup}MiniGame();" in line:
            skip = True
    if not skip:
        new_lines.append(line)

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGames/MiniGameFactory.cs', 'w') as f:
    f.write('\n'.join(new_lines))
print("Fixed duplicate cases in factory")

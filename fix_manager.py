import re

with open('Assets/Scripts/2 Managers/MiniGameManager.cs', 'r') as f:
    lines = f.readlines()

# Find start of SetupGameType
start_idx = -1
for i, line in enumerate(lines):
    if 'public void SetupGameType(GameType type)' in line:
        start_idx = i
        break

# Find start of MarkWinReady
end_idx = -1
for i, line in enumerate(lines):
    if 'public void MarkWinReady()' in line:
        end_idx = i
        break

if start_idx != -1 and end_idx != -1:
    new_lines = lines[:start_idx] + lines[end_idx:]
    with open('Assets/Scripts/2 Managers/MiniGameManager.cs', 'w') as f:
        f.writelines(new_lines)
    print(f"Deleted lines {start_idx} to {end_idx-1}")
else:
    print("Could not find bounds")

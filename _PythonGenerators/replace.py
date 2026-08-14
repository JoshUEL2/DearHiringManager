with open("Assets/UI/MiniGame.uxml", "r") as f:
    lines = f.readlines()

with open("replacement.txt", "r") as f:
    replacement = f.read()

# Lines 771 to 828 inclusive (0-indexed 770 to 828)
new_lines = lines[:770] + [replacement + "\n"] + lines[828:]

with open("Assets/UI/MiniGame.uxml", "w") as f:
    f.writelines(new_lines)

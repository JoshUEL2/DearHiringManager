import sys

with open("Assets/Scripts/2 Managers/MiniGameManager.cs", "r") as f:
    lines = f.readlines()

start = -1
for i, line in enumerate(lines):
    if "private void OnMgPointerEnter(int idx)" in line:
        start = i; break

if start != -1:
    braces = 0; in_method = False; end = start
    for i in range(start, len(lines)):
        if '{' in lines[i]: braces += lines[i].count('{'); in_method = True
        if '}' in lines[i]: braces -= lines[i].count('}')
        if in_method and braces == 0:
            end = i; break
    lines = lines[:start] + lines[end+1:]
    print(f"Deleted OnMgPointerEnter ({start} to {end})")

with open("Assets/Scripts/2 Managers/MiniGameManager.cs", "w") as f:
    f.writelines(lines)

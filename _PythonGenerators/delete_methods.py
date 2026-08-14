import sys

def get_method_bounds(lines, name):
    start = -1
    for i, line in enumerate(lines):
        if f"private void {name}()" in line or f"private System.Collections.IEnumerator {name}()" in line:
            start = i
            break
    if start == -1: return -1, -1

    braces = 0
    in_method = False
    end = start
    for i in range(start, len(lines)):
        line = lines[i]
        if '{' in line:
            braces += line.count('{')
            in_method = True
        if '}' in line:
            braces -= line.count('}')
        if in_method and braces == 0:
            end = i
            break
    return start, end

with open("Assets/Scripts/2 Managers/MiniGameManager.cs", "r") as f:
    lines = f.readlines()

def delete_method(name):
    global lines
    start, end = get_method_bounds(lines, name)
    if start != -1:
        # Also try to remove any comment lines directly preceding it
        while start > 0 and lines[start-1].strip().startswith("//"):
            start -= 1
        lines = lines[:start] + lines[end+1:]
        print(f"Deleted {name} ({start} to {end})")

delete_method("SetupCorporateGolf")
delete_method("OnCgSwingClicked")
delete_method("CorporateGolfRoutine")

delete_method("SetupMowGreens")
delete_method("OnMgPointerEnter") # Was it OnMgPointerEnter? Wait, earlier I searched for it.
delete_method("MowGreensRoutine")

delete_method("SetupWalkDogs")
delete_method("WalkDogsRoutine")
# Wait, OnWdDogClicked takes an int idx
start = -1
for i, line in enumerate(lines):
    if "private void OnWdDogClicked(int idx)" in line:
        start = i; break
if start != -1:
    braces = 0; in_method = False; end = start
    for i in range(start, len(lines)):
        if '{' in lines[i]: braces += lines[i].count('{'); in_method = True
        if '}' in lines[i]: braces -= lines[i].count('}')
        if in_method and braces == 0:
            end = i; break
    lines = lines[:start] + lines[end+1:]
    print(f"Deleted OnWdDogClicked ({start} to {end})")

with open("Assets/Scripts/2 Managers/MiniGameManager.cs", "w") as f:
    f.writelines(lines)

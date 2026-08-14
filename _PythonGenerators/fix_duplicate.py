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

def delete_method(name):
    global lines
    start, end = get_method_bounds(lines, name)
    if start != -1:
        while start > 0 and lines[start-1].strip().startswith("//"):
            start -= 1
        lines = lines[:start] + lines[end+1:]
        print(f"Deleted {name} ({start} to {end})")

with open("Assets/Scripts/2 Managers/MiniGameManager.cs", "r") as f:
    lines = f.readlines()

delete_method("SetupRecycleBottles")
delete_method("OnRbSwipe")
delete_method("OnRbItemClicked")
delete_method("RecycleBottlesRoutine")

with open("Assets/Scripts/2 Managers/MiniGameManager.cs", "w") as f:
    f.writelines(lines)

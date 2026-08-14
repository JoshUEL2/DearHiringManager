import sys

with open("Assets/Scripts/2 Managers/MiniGameManager.cs", "r") as f:
    lines = f.readlines()

# 1. Delete old Corporate Golf (lines 7434 to 7493)
# 2. Delete old Mow Greens
# 3. Delete old Walk Dogs

# Let's find them dynamically
def find_method(lines, name):
    for i, line in enumerate(lines):
        if f"private void {name}()" in line:
            return i
    return -1

def delete_method(lines, name):
    start = find_method(lines, name)
    if start == -1: return lines
    # find the end of the routine by looking for the next private void or private System.Collections.IEnumerator
    # Or just count braces
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
    # But wait, there are helper methods (OnCgSwingClicked) and Coroutines (CorporateGolfRoutine).
    # It's better to just delete by finding the next known Game Setup, but the order is messed up.
    return lines

# Let's just use string replacement on the exact code blocks.

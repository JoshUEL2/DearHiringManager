import os
import re

# 1. Get all container names from the Q<VisualElement> calls
cs_path = "Assets/Scripts/2 Managers/MiniGameManager.cs"
with open(cs_path, "r") as f:
    cs_content = f.read()

containers = set()
# Match Q<VisualElement>("...") 
matches = re.findall(r'Q<VisualElement>\("([^"]+Container)"\)', cs_content)
for match in matches:
    containers.add(match)

print(f"Found {len(containers)} containers to extract.")

# 2. Search through all potential files
files_to_search = ["recovered_uxml.txt"]
for root, dirs, files in os.walk("_PythonGenerators"):
    for file in files:
        if file.endswith(".py") or file.endswith(".txt"):
            files_to_search.append(os.path.join(root, file))

def extract_container(text, container_name):
    target = f'name="{container_name}"'
    start_idx = text.find(target)
    if start_idx == -1:
        return None
    
    tag_start = text.rfind("<", 0, start_idx)
    depth = 0
    i = tag_start
    while i < len(text):
        if text[i:i+2] == "</":
            depth -= 1
            if depth == 0:
                end_tag = text.find(">", i)
                if end_tag != -1:
                    return text[tag_start:end_tag+1]
        elif text[i:i+2] == "/>":
            depth -= 1
            if depth == 0:
                return text[tag_start:i+2]
        elif text[i] == "<" and not text[i:i+4] == "<!--":
            tag_end = text.find(">", i)
            if tag_end != -1 and text[tag_end-1] != "/":
                depth += 1
        i += 1
    return None

os.makedirs("Assets/UI/MiniGames", exist_ok=True)
found_containers = set()

for fname in files_to_search:
    try:
        with open(fname, "r", encoding="utf-8") as f:
            content = f.read()
            content = content.replace('\\"', '"')
    except:
        continue
        
    for c in list(containers - found_containers):
        block = extract_container(content, c)
        if block:
            if block.count("<") != block.count(">"):
                print(f"Warning: Corrupted block found for {c} in {fname}")
                continue
            
            with open(f"Assets/UI/MiniGames/{c}.uxml", "w") as out:
                out.write('<?xml version="1.0" encoding="utf-8"?>\n')
                out.write('<ui:UXML xmlns:ui="UnityEngine.UIElements" xmlns:uie="UnityEditor.UIElements">\n')
                out.write(block)
                out.write('\n</ui:UXML>')
            found_containers.add(c)
            print(f"Extracted {c} from {fname}")

missing = containers - found_containers
print(f"\nMissing containers: {missing}")


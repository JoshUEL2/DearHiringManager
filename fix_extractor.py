import os
import re
import glob

# Get all actual containers
cs_path = "Assets/Scripts/2 Managers/MiniGameManager.cs"
with open(cs_path, "r") as f:
    cs_content = f.read()

containers = set(re.findall(r'Q<VisualElement>\("([^"]+Container)"\)', cs_content))

files_to_search = ["filesforremakingminigames/MiniGame.uxml"]
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
    
    # Let's use a much simpler approach: 
    # Just parse the string and keep track of actual opening/closing tags based on tag names.
    # Or just use simple bracket matching that properly ignores self-closing tags.
    depth = 0
    i = tag_start
    while i < len(text):
        if text[i:i+2] == "</":
            # closing tag
            depth -= 1
            if depth == 0:
                end_tag = text.find(">", i)
                return text[tag_start:end_tag+1]
            i = text.find(">", i)
        elif text[i:i+4] == "<!--":
            i = text.find("-->", i) + 2
        elif text[i] == "<":
            # opening tag. check if it's self closing.
            end_tag = text.find(">", i)
            if end_tag != -1:
                if text[end_tag-1] != "/":
                    depth += 1
                i = end_tag
        i += 1
    return None

found_containers = set()
for fname in files_to_search:
    try:
        with open(fname, "r", encoding="utf-8") as f:
            content = f.read().replace('\\"', '"')
    except:
        continue
        
    for c in list(containers - found_containers):
        block = extract_container(content, c)
        if block:
            with open(f"Assets/UI/MiniGames/{c}.uxml", "w") as out:
                out.write('<?xml version="1.0" encoding="utf-8"?>\n')
                out.write('<ui:UXML xmlns:ui="UnityEngine.UIElements" xmlns:uie="UnityEditor.UIElements">\n')
                out.write(block)
                out.write('\n</ui:UXML>')
            found_containers.add(c)

print(f"Extracted {len(found_containers)} containers.")

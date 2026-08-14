import re

with open("filesforremakingminigames/MiniGame.uxml", "r", encoding="utf-8") as f:
    text = f.read()

# find all names ending with Container
matches = re.findall(r'<ui:VisualElement\s+name="([A-Za-z0-9_]+Container)"', text)
print(f"Found {len(matches)} container tags in backup UXML.")

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
                return text[tag_start:end_tag+1]
            i = text.find(">", i)
        elif text[i:i+4] == "<!--":
            i = text.find("-->", i) + 2
        elif text[i] == "<":
            end_tag = text.find(">", i)
            if end_tag != -1:
                if text[end_tag-1] != "/":
                    depth += 1
                i = end_tag
        i += 1
    return None

extracted = 0
for c in set(matches):
    block = extract_container(text, c)
    if block:
        with open(f"Assets/UI/MiniGames/{c}.uxml", "w") as out:
            out.write('<?xml version="1.0" encoding="utf-8"?>\n')
            out.write('<ui:UXML xmlns:ui="UnityEngine.UIElements" xmlns:uie="UnityEditor.UIElements">\n')
            out.write(block)
            out.write('\n</ui:UXML>')
        extracted += 1

print(f"Extracted {extracted} containers successfully.")

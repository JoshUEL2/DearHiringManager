import re
import os

with open('recovered_uxml.txt', 'r', encoding='utf-8') as f:
    content = f.read()

# Pattern to find all containers
# It looks for <ui:VisualElement name="XXXContainer" ...> ... </ui:VisualElement>
# Since containers might have nested VisualElements, we can't just use a simple regex easily.
# We'll use a stack-based parser to extract balanced tags.

def extract_containers(text):
    containers = []
    # Find start of containers
    starts = [m for m in re.finditer(r'<ui:VisualElement[^>]*name="([a-zA-Z0-9]+Container)"', text)]
    
    for start_match in starts:
        start_idx = start_match.start()
        name = start_match.group(1)
        
        # We only want top-level containers. Let's skip if it's already inside one.
        
        # Now find the matching closing tag
        open_tags = 0
        idx = start_idx
        while idx < len(text):
            open_match = re.search(r'<ui:VisualElement[^>]*>', text[idx:])
            close_match = re.search(r'</ui:VisualElement>', text[idx:])
            
            if not open_match and not close_match:
                break
                
            open_idx = open_match.start() + idx if open_match else float('inf')
            close_idx = close_match.start() + idx if close_match else float('inf')
            
            # Check for self-closing tags <ui:VisualElement ... />
            if open_match and text[open_match.end() + idx - 2:open_match.end() + idx] == '/>':
                idx = open_match.end() + idx
                continue
                
            if open_idx < close_idx:
                open_tags += 1
                idx = open_match.end() + idx
            else:
                open_tags -= 1
                idx = close_match.end() + idx
                
            if open_tags == 0:
                containers.append((name, text[start_idx:idx]))
                break
                
    return containers

containers = extract_containers(content)
out_dir = "Assets/Resources/MiniGames"
os.makedirs(out_dir, exist_ok=True)

header = """<?xml version="1.0" encoding="utf-8"?>
<ui:UXML
    xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
    xmlns:ui="UnityEngine.UIElements"
    xmlns:engine="UnityEngine.UIElements"
    xmlns:editor="UnityEditor.UIElements"
    xsi:noNamespaceSchemaLocation="../../UIElementsSchema/UIElements.xsd">
"""
footer = "\n</ui:UXML>"

for name, xml in containers:
    # Remove style="display: none;" or display:none; from the container's root tag
    # so it shows up properly when dynamically loaded
    # But wait, ScrubPlateMiniGame and others expect to toggle it themselves, or we can just remove it.
    
    # Actually, keep it as is. ScrubPlateMiniGame handles it.
    file_path = os.path.join(out_dir, f"{name.replace('Container', '')}.uxml")
    with open(file_path, 'w', encoding='utf-8') as out_f:
        out_f.write(header + xml + footer)
    print(f"Extracted {name} to {file_path}")

print(f"Done extracting {len(containers)} containers.")

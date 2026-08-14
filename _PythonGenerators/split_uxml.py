import os
import re

def split_uxml():
    file_path = "Assets/UI/MiniGame.uxml"
    out_dir = "Assets/UI/MiniGames"
    if not os.path.exists(out_dir):
        os.makedirs(out_dir)

    with open(file_path, "r", encoding="utf-8") as f:
        content = f.read()

    # The file has a ScrollView that contains all the containers.
    # We will search for all elements that have name="...Container" directly inside the scroll view.
    # Actually, a simpler way is to split by "<!-- GAME TYPE"
    
    # We'll use a regex to find all containers.
    # A container starts with: <ui:VisualElement name="SomethingContainer" ...>
    # and ends with a matching </ui:VisualElement>
    
    # Because there are nested elements, we should parse it by lines and keep a stack of tags.
    
    with open(file_path, "r", encoding="utf-8") as f:
        lines = f.readlines()
        
    new_main_lines = []
    
    in_container = False
    container_lines = []
    container_name = ""
    nesting_level = 0
    
    # We need the UXML header for the extracted files
    uxml_header = '<?xml version="1.0" encoding="utf-8"?>\n<ui:UXML xmlns:ui="UnityEngine.UIElements" xmlns:uie="UnityEditor.UIElements" xsi="http://www.w3.org/2001/XMLSchema-instance" editor-extension-mode="False">\n'
    uxml_footer = '\n</ui:UXML>'
    
    extracted_templates = []
    
    for i, line in enumerate(lines):
        # Look for the start of a container if we're not in one
        if not in_container:
            # check if line starts a game container
            match = re.search(r'<ui:VisualElement[^>]*name="([A-Za-z0-9_]+Container)"', line)
            if match:
                in_container = True
                container_name = match.group(1)
                container_lines = [line]
                
                # Check for self-closing tag
                if line.strip().endswith("/>"):
                    in_container = False
                    # Write out
                    out_path = os.path.join(out_dir, f"{container_name}.uxml")
                    with open(out_path, "w", encoding="utf-8") as out_f:
                        out_f.write(uxml_header)
                        out_f.writelines(container_lines)
                        out_f.write(uxml_footer)
                    
                    extracted_templates.append(container_name)
                    new_main_lines.append(f'                <ui:Instance template="{container_name}" name="{container_name}" />\n')
                    continue
                
                # count nesting in this line
                nesting_level = line.count("<ui:VisualElement") - line.count("</ui:VisualElement")
            else:
                new_main_lines.append(line)
        else:
            container_lines.append(line)
            nesting_level += line.count("<ui:VisualElement")
            nesting_level -= line.count("</ui:VisualElement")
            
            if nesting_level == 0:
                in_container = False
                
                # Write to its own file
                out_path = os.path.join(out_dir, f"{container_name}.uxml")
                with open(out_path, "w", encoding="utf-8") as out_f:
                    out_f.write(uxml_header)
                    out_f.writelines(container_lines)
                    out_f.write(uxml_footer)
                
                extracted_templates.append(container_name)
                # Append instance to main lines
                new_main_lines.append(f'                <ui:Instance template="{container_name}" name="{container_name}" />\n')

    # Now we need to insert the Template declarations at the top of the main file.
    # Find the <ui:UXML ...> line to insert after
    insert_index = 0
    for i, line in enumerate(new_main_lines):
        if line.strip().startswith("<ui:UXML"):
            insert_index = i + 1
            break
            
    template_declarations = []
    for t_name in extracted_templates:
        template_declarations.append(f'    <ui:Template name="{t_name}" src="MiniGames/{t_name}.uxml" />\n')
        
    final_lines = new_main_lines[:insert_index] + ['\n'] + template_declarations + ['\n'] + new_main_lines[insert_index:]
    
    with open(file_path, "w", encoding="utf-8") as f:
        f.writelines(final_lines)
        
    print(f"Extracted {len(extracted_templates)} containers.")

if __name__ == "__main__":
    split_uxml()

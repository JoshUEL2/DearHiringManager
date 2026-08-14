import re

files = [
    "Assets/UI/CareersUI.uxml", 
    "Assets/UI/HustlesUI.uxml", 
    "Assets/UI/EducationUI.uxml", 
    "Assets/UI/ActivitiesUI.uxml"
]

for filepath in files:
    with open(filepath, 'r') as f:
        content = f.read()

    # 1. Remove Top Card
    # It starts with <!-- TOP CARD --> and ends with the matching </ui:VisualElement> before the next section
    # Using regex, find <!-- TOP CARD --> and the nearest </ui:VisualElement> that matches the outer container
    # Since they are exactly formatted, we can use a simpler approach:
    # Find the block between '<!-- TOP CARD -->' and the end of the top-card block
    # Actually, we can just replace everything between <!-- TOP CARD --> and either <!-- UNEMPLOYED: or <ui:VisualElement style="flex-direction: row
    
    # A reliable way is to count <ui:VisualElement> and </ui:VisualElement>
    
    lines = content.split('\n')
    new_lines = []
    
    in_top_card = False
    in_bottom_nav = False
    top_card_depth = 0
    bottom_nav_depth = 0
    
    for i, line in enumerate(lines):
        if '<!-- TOP CARD -->' in line:
            in_top_card = True
            top_card_depth = 0
            continue
            
        if '<!-- NEW BOTTOM NAV & FOOTER -->' in line:
            in_bottom_nav = True
            bottom_nav_depth = 0
            continue
            
        if in_top_card:
            if '<ui:VisualElement' in line:
                top_card_depth += 1
            if '</ui:VisualElement>' in line:
                top_card_depth -= 1
                if top_card_depth < 0:
                    in_top_card = False
            continue
            
        if in_bottom_nav:
            if '<ui:VisualElement' in line:
                bottom_nav_depth += 1
            if '</ui:VisualElement>' in line:
                bottom_nav_depth -= 1
                if bottom_nav_depth < 0:
                    in_bottom_nav = False
            continue
            
        new_lines.append(line)
        
    # Now let's inject the Close button right after <ui:VisualElement class="screen-container" ...>
    # Also change screen-container to include modal-container modal-hidden
    
    final_lines = []
    for line in new_lines:
        if '<ui:VisualElement class="screen-container"' in line:
            # Replace class="screen-container" with class="screen-container modal-container modal-hidden"
            line = line.replace('class="screen-container"', 'class="screen-container modal-container modal-hidden"')
            final_lines.append(line)
            final_lines.append('        <ui:Button name="BtnCloseModal" text="X" class="modal-close-btn" />')
        else:
            final_lines.append(line)
            
    with open(filepath, 'w') as f:
        f.write('\n'.join(final_lines))
        
print("Refactored all UXML files.")

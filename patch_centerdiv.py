import re

path = 'Assets/Scripts/2 Managers/MiniGames/CenterDivMiniGame.cs'
with open(path, 'r') as f:
    content = f.read()

# 1. Update tolerance
target_tolerance = r'float tolerance = 12f; // Increased tolerance due to high speed at center in Rounds 2 & 3'
replace_tolerance = r'float tolerance = 15f; // Forgiving 15% margin for perfect hits'
content = content.replace(target_tolerance, replace_tolerance)

# 2. Add scaling to CreateJuiceElements
target_juice = r'''        targetBox.style.left = new Length(50f, LengthUnit.Percent); // Force absolute center'''
replace_juice = r'''        targetBox.style.left = new Length(50f, LengthUnit.Percent); // Force absolute center
        
        // Scale both boxes up as requested and fix margin alignment
        targetBox.style.width = 160;
        targetBox.style.height = 160;
        targetBox.style.marginLeft = -80;
        targetBox.style.marginTop = -80;
        
        movingBox.style.width = 160;
        movingBox.style.height = 160;
        movingBox.style.marginLeft = -80;
        movingBox.style.marginTop = -80;'''
content = content.replace(target_juice, replace_juice)

# 3. Increase font size of <div class="hero"> to match new scale
target_font = r'''        boxText.style.fontSize = 14;'''
replace_font = r'''        boxText.style.fontSize = 18;'''
content = content.replace(target_font, replace_font)

# 4. Also increase the ghost box scale
target_ghost = r'''        ghost.style.width = 100;
        ghost.style.height = 100;
        ghost.style.marginTop = -50;'''
replace_ghost = r'''        ghost.style.width = 160;
        ghost.style.height = 160;
        ghost.style.marginLeft = -80;
        ghost.style.marginTop = -80;'''
content = content.replace(target_ghost, replace_ghost)

with open(path, 'w') as f:
    f.write(content)


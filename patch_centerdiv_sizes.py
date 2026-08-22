import re

path = 'Assets/Scripts/2 Managers/MiniGames/CenterDivMiniGame.cs'
with open(path, 'r') as f:
    content = f.read()

# 1. Remove center line
target_line = r'''        // Add a vertical cyan line to exact center for precision
        VisualElement centerLine = new VisualElement\(\);
        centerLine\.style\.position = Position\.Absolute;
        centerLine\.style\.left = new Length\(50f, LengthUnit\.Percent\);
        centerLine\.style\.top = 0;
        centerLine\.style\.bottom = 0;
        centerLine\.style\.width = 2;
        centerLine\.style\.marginLeft = -1;
        centerLine\.style\.backgroundColor = new StyleColor\(new Color\(0\.13f, 0\.85f, 0\.95f, 0\.4f\)\);
        targetBox\.Add\(centerLine\);'''
content = re.sub(target_line, '', content)

# 2. Resize target box to be slightly larger so it acts as a frame (fixes the narrowing visual overlap)
target_size = r'''        // Scale both boxes up as requested and fix margin alignment
        targetBox\.style\.width = 160;
        targetBox\.style\.height = 160;
        targetBox\.style\.marginLeft = -80;
        targetBox\.style\.marginTop = -80;'''

replace_size = r'''        // Scale target box larger than moving box so it acts as a clear outer frame
        targetBox.style.width = 180;
        targetBox.style.height = 180;
        targetBox.style.marginLeft = -90;
        targetBox.style.marginTop = -90;'''
content = re.sub(target_size, replace_size, content)

# 3. Increase tolerance
target_tol = r'''        float tolerance = 15f; // Forgiving 15% margin for perfect hits'''
replace_tol = r'''        float tolerance = 18f; // Even wider tolerance (18%) as requested'''
content = re.sub(target_tol, replace_tol, content)

with open(path, 'w') as f:
    f.write(content)


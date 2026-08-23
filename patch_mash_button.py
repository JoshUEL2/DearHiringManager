import re

path = 'Assets/Scripts/2 Managers/MiniGames/MashRaceMiniGame.cs'
with open(path, 'r') as f:
    content = f.read()

target = r'''        if \(btnMashTap != null\)
        \{
            btnMashTap\.clicked \+= OnMashTapClicked;
        \}'''

replacement = r'''        if (btnMashTap != null)
        {
            btnMashTap.clicked += OnMashTapClicked;
            
            // Stylize the Mash Button
            btnMashTap.style.fontSize = 48;
            btnMashTap.style.borderTopLeftRadius = 20;
            btnMashTap.style.borderTopRightRadius = 20;
            btnMashTap.style.borderBottomLeftRadius = 20;
            btnMashTap.style.borderBottomRightRadius = 20;
            btnMashTap.style.paddingTop = 40;
            btnMashTap.style.paddingBottom = 40;
        }'''
content = re.sub(target, replacement, content)

with open(path, 'w') as f:
    f.write(content)

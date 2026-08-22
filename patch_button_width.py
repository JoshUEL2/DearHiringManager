import re

path = 'Assets/Scripts/2 Managers/MiniGames/CenterDivMiniGame.cs'
with open(path, 'r') as f:
    content = f.read()

# Lock the minimum width of the button so its text doesn't collapse the layout
target = r'''        centerBtn = container.Q<Button>\("CdBtnCenter"\);'''
replace = r'''        centerBtn = container.Q<Button>("CdBtnCenter");
        // Lock the minimum width so changing text to "SUCCESS!" doesn't collapse the container width
        centerBtn.style.minWidth = 500;'''
content = re.sub(target, replace, content)

with open(path, 'w') as f:
    f.write(content)


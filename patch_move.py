import re

path = 'Assets/Scripts/2 Managers/MiniGames/DragCenterMiniGame.cs'
with open(path, 'r') as f:
    content = f.read()

target = r'''            percentX = Mathf.Clamp(percentX, 0f, 100f);
            percentY = Mathf.Clamp(percentY, 0f, 100f);


        }'''

replacement = r'''            percentX = Mathf.Clamp(percentX, 0f, 100f);
            percentY = Mathf.Clamp(percentY, 0f, 100f);
            
            draggableIcon.style.left = new Length(percentX, LengthUnit.Percent);
            draggableIcon.style.top = new Length(percentY, LengthUnit.Percent);
        }'''

content = content.replace(target, replacement)

with open(path, 'w') as f:
    f.write(content)

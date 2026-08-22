import re

path = 'Assets/Scripts/2 Managers/MiniGames/CenterDivMiniGame.cs'
with open(path, 'r') as f:
    content = f.read()

# Add width: 100% to container to prevent it from shrinking when score text changes
target = r'''        container = root.Q<VisualElement>("CenterDivContainer");'''
replacement = r'''        container = root.Q<VisualElement>("CenterDivContainer");
        container.style.width = new Length(100f, LengthUnit.Percent); // Stop it from shrinking when text changes'''
content = content.replace(target, replacement)

# Let's also do it for SizeMatch just in case
with open(path, 'w') as f:
    f.write(content)

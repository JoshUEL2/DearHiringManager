import re

path = 'Assets/Scripts/2 Managers/MiniGames/PumpSystemMiniGame.cs'
with open(path, 'r') as f:
    content = f.read()

# Dial Resize
content = re.sub(r'pumpDialContainer\.style\.width = 320;', 'pumpDialContainer.style.width = 240;', content)
content = re.sub(r'pumpDialContainer\.style\.height = 320;', 'pumpDialContainer.style.height = 240;', content)
content = re.sub(r'pumpDialContainer\.style\.borderTopLeftRadius = 160;', 'pumpDialContainer.style.borderTopLeftRadius = 120;', content)
content = re.sub(r'pumpDialContainer\.style\.borderTopRightRadius = 160;', 'pumpDialContainer.style.borderTopRightRadius = 120;', content)
content = re.sub(r'pumpDialContainer\.style\.borderBottomLeftRadius = 160;', 'pumpDialContainer.style.borderBottomLeftRadius = 120;', content)
content = re.sub(r'pumpDialContainer\.style\.borderBottomRightRadius = 160;', 'pumpDialContainer.style.borderBottomRightRadius = 120;', content)

# Needle Resize
content = re.sub(r'needle\.style\.height = 140;', 'needle.style.height = 100;', content)

# Target Zone Size (10 to 25 instead of 15 to 40)
content = re.sub(r'float zoneSizePct = Random\.Range\(15f, 40f\);', 'float zoneSizePct = Random.Range(10f, 25f);', content)

with open(path, 'w') as f:
    f.write(content)

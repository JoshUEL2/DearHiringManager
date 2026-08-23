import re

path = 'Assets/Scripts/2 Managers/MiniGames/PumpSystemMiniGame.cs'
with open(path, 'r') as f:
    content = f.read()

# Dial Resize back to 320
content = re.sub(r'pumpDialContainer\.style\.width = 240;', 'pumpDialContainer.style.width = 320;', content)
content = re.sub(r'pumpDialContainer\.style\.height = 240;', 'pumpDialContainer.style.height = 320;', content)
content = re.sub(r'pumpDialContainer\.style\.borderTopLeftRadius = 120;', 'pumpDialContainer.style.borderTopLeftRadius = 160;', content)
content = re.sub(r'pumpDialContainer\.style\.borderTopRightRadius = 120;', 'pumpDialContainer.style.borderTopRightRadius = 160;', content)
content = re.sub(r'pumpDialContainer\.style\.borderBottomLeftRadius = 120;', 'pumpDialContainer.style.borderBottomLeftRadius = 160;', content)
content = re.sub(r'pumpDialContainer\.style\.borderBottomRightRadius = 120;', 'pumpDialContainer.style.borderBottomRightRadius = 160;', content)

# Needle Resize back to 140
content = re.sub(r'needle\.style\.height = 100;', 'needle.style.height = 140;', content)

with open(path, 'w') as f:
    f.write(content)

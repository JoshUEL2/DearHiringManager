import re

path = 'Assets/Scripts/2 Managers/MiniGames/SequenceGameMiniGame.cs'

with open(path, 'r') as f:
    content = f.read()

# Fix borderRadius
def replace_border_radius(match):
    val = match.group(1)
    return f"borderTopLeftRadius = {val}; container.style.borderTopRightRadius = {val}; container.style.borderBottomLeftRadius = {val}; container.style.borderBottomRightRadius = {val}"
content = re.sub(r'container\.style\.borderRadius = ([^;]+)', lambda m: f'container.style.borderTopLeftRadius = {m.group(1)}; container.style.borderTopRightRadius = {m.group(1)}; container.style.borderBottomLeftRadius = {m.group(1)}; container.style.borderBottomRightRadius = {m.group(1)}', content)

content = re.sub(r'playerAvatar\.style\.borderRadius = ([^;]+)', lambda m: f'playerAvatar.style.borderTopLeftRadius = {m.group(1)}; playerAvatar.style.borderTopRightRadius = {m.group(1)}; playerAvatar.style.borderBottomLeftRadius = {m.group(1)}; playerAvatar.style.borderBottomRightRadius = {m.group(1)}', content)

content = re.sub(r'head\.style\.borderRadius = ([^;]+)', lambda m: f'head.style.borderTopLeftRadius = {m.group(1)}; head.style.borderTopRightRadius = {m.group(1)}; head.style.borderBottomLeftRadius = {m.group(1)}; head.style.borderBottomRightRadius = {m.group(1)}', content)

content = re.sub(r'btnTuck\.style\.borderRadius = ([^;]+)', lambda m: f'btnTuck.style.borderTopLeftRadius = {m.group(1)}; btnTuck.style.borderTopRightRadius = {m.group(1)}; btnTuck.style.borderBottomLeftRadius = {m.group(1)}; btnTuck.style.borderBottomRightRadius = {m.group(1)}', content)

# Fix borderColor
content = re.sub(r'container\.style\.borderColor = ([^;]+)', lambda m: f'container.style.borderTopColor = {m.group(1)}; container.style.borderBottomColor = {m.group(1)}; container.style.borderLeftColor = {m.group(1)}; container.style.borderRightColor = {m.group(1)}', content)

content = re.sub(r'playerAvatar\.style\.borderColor = ([^;]+)', lambda m: f'playerAvatar.style.borderTopColor = {m.group(1)}; playerAvatar.style.borderBottomColor = {m.group(1)}; playerAvatar.style.borderLeftColor = {m.group(1)}; playerAvatar.style.borderRightColor = {m.group(1)}', content)

content = re.sub(r'btnTuck\.style\.borderColor = ([^;]+)', lambda m: f'btnTuck.style.borderTopColor = {m.group(1)}; btnTuck.style.borderBottomColor = {m.group(1)}; btnTuck.style.borderLeftColor = {m.group(1)}; btnTuck.style.borderRightColor = {m.group(1)}', content)


with open(path, 'w') as f:
    f.write(content)

path = 'Assets/Scripts/2 Managers/MiniGames/DragMatchMiniGame.cs'

with open(path, 'r') as f:
    content = f.read()

target = "lbl.style.marginLeft = 80; // FIX TEXT ALIGNMENT OVER RED LINE!"
replacement = "lbl.style.paddingLeft = 80; // FIX TEXT ALIGNMENT OVER RED LINE!"

if target in content:
    content = content.replace(target, replacement)
    with open(path, 'w') as f:
        f.write(content)
else:
    print("TARGET NOT FOUND")


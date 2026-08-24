path = 'Assets/Scripts/2 Managers/MiniGames/DragMatchMiniGame.cs'

with open(path, 'r') as f:
    content = f.read()

target = """            if (isAudit) {
                rBtn.style.backgroundColor = new StyleColor(new Color(0.9f, 0.9f, 0.95f));
                rBtn.style.color = new StyleColor(Color.black);
                rBtn.style.borderTopColor = new StyleColor(Color.black); rBtn.style.borderBottomColor = new StyleColor(Color.black); rBtn.style.borderLeftColor = new StyleColor(Color.black); rBtn.style.borderRightColor = new StyleColor(Color.black);
            }"""
            
replacement = """            if (isAudit) {
                rBtn.style.backgroundColor = new StyleColor(new Color(0.9f, 0.9f, 0.95f));
                rBtn.style.color = new StyleColor(Color.black);
                rBtn.style.borderTopColor = new StyleColor(Color.black); rBtn.style.borderBottomColor = new StyleColor(Color.black); rBtn.style.borderLeftColor = new StyleColor(Color.black); rBtn.style.borderRightColor = new StyleColor(Color.black);
                rBtn.style.borderTopWidth = 2; rBtn.style.borderBottomWidth = 2; rBtn.style.borderLeftWidth = 2; rBtn.style.borderRightWidth = 2;
                rBtn.style.borderTopLeftRadius = 25; rBtn.style.borderTopRightRadius = 25; rBtn.style.borderBottomLeftRadius = 25; rBtn.style.borderBottomRightRadius = 25;
            }"""

if target in content:
    content = content.replace(target, replacement)
    with open(path, 'w') as f:
        f.write(content)
else:
    print("TARGET NOT FOUND")


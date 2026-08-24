import re

path = 'Assets/Scripts/2 Managers/MiniGames/DragMatchMiniGame.cs'

with open(path, 'r') as f:
    content = f.read()

target = r'p\.BeginPath\(\); p\.Rect\(new Rect\(0, 0, w, 20\)\); p\.Fill\(\);'
replacement = r'''p.BeginPath(); 
        p.MoveTo(new Vector2(0,0)); p.LineTo(new Vector2(w,0)); 
        p.LineTo(new Vector2(w,20)); p.LineTo(new Vector2(0,20)); 
        p.ClosePath(); p.Fill();'''

if target in content:
    content = content.replace(target, replacement)
else:
    # Try a regex if the literal string has whitespace differences
    content = re.sub(r'p\.BeginPath\(\);\s*p\.Rect\(new Rect\(0,\s*0,\s*w,\s*20\)\);\s*p\.Fill\(\);', replacement, content)

with open(path, 'w') as f:
    f.write(content)


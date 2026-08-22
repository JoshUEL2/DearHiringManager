import os

folder = 'Assets/Scripts/2 Managers/MiniGames'
for root, dirs, files in os.walk(folder):
    for f in files:
        if f.endswith('.cs'):
            path = os.path.join(root, f)
            with open(path, 'r') as file:
                content = file.read()
            if '\\"' in content:
                print(f"Fixing {f}")
                content = content.replace('\\"', '"')
                with open(path, 'w') as file:
                    file.write(content)

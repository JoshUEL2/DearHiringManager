path = 'Assets/Scripts/2 Managers/MiniGames/DragMatchMiniGame.cs'

with open(path, 'r') as f:
    content = f.read()

target = """        var emptyLbl = new Label(" ");
        emptyLbl.style.paddingBottom = 8;
        emptyLbl.style.paddingTop = 8;
        emptyLbl.style.borderBottomWidth = 1;
        emptyLbl.style.borderBottomColor = new StyleColor(new Color(0.3f, 0.6f, 0.9f, 0.5f));
        dragCheatSheetList.Add(emptyLbl);"""

replacement = """        var emptyLbl = new Label(" ");
        emptyLbl.style.paddingBottom = 8;
        emptyLbl.style.paddingTop = 8;
        dragCheatSheetList.Add(emptyLbl);"""

if target in content:
    content = content.replace(target, replacement)
    with open(path, 'w') as f:
        f.write(content)
else:
    print("TARGET NOT FOUND")


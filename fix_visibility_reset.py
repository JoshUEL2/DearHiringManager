path = 'Assets/Scripts/2 Managers/MiniGames/DragMatchMiniGame.cs'

with open(path, 'r') as f:
    content = f.read()

target = r'''        dragLeftColumn.Clear();
        dragRightColumn.Clear();'''

replacement = r'''        dragLeftColumn.style.visibility = Visibility.Visible;
        dragRightColumn.style.visibility = Visibility.Visible;
        dragLeftColumn.Clear();
        dragRightColumn.Clear();'''

content = content.replace(target, replacement)

with open(path, 'w') as f:
    f.write(content)

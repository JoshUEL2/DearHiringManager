import re

path = 'Assets/Scripts/2 Managers/MiniGames/TetrisMiniGame.cs'

with open(path, 'r') as f:
    content = f.read()

target = r'''        SpawnTetromino\(\);

        manager\.StartCoroutine\(GameRoutine\(\)\);'''

replacement = r'''        SpawnTetromino();

        tetrisContainer.schedule.Execute(() => tetrisContainer.MarkDirtyRepaint()).ExecuteLater(50); // Ensure crane cables draw after layout

        manager.StartCoroutine(GameRoutine());'''

content = re.sub(target, replacement, content)

with open(path, 'w') as f:
    f.write(content)

import re

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'r') as f:
    content = f.read()

# FallingSort
content = re.sub(r'\s*fallingSortContainer = activeRoot\.Q<VisualElement>\("FallingSortContainer"\);', '', content)
content = re.sub(r'\s*fallZone = activeRoot\.Q<VisualElement>\("FallZone"\);', '', content)
content = re.sub(r'\s*binContainer = activeRoot\.Q<VisualElement>\("BinContainer"\);', '', content)
content = re.sub(r'\s*sortScoreText = activeRoot\.Q<Label>\("SortScoreText"\);', '', content)
content = re.sub(r'\s*sortTimerText = activeRoot\.Q<Label>\("SortTimerText"\);', '', content)

# FileSorter
content = re.sub(r'\s*fileSorterContainer = activeRoot\.Q<VisualElement>\("FileSorterContainer"\);', '', content)
content = re.sub(r'\s*fileSorterDropZone = activeRoot\.Q<VisualElement>\("FileSorterDropZone"\);', '', content)
content = re.sub(r'\s*fileSorterActiveFile = activeRoot\.Q<VisualElement>\("FileSorterActiveFile"\);', '', content)
content = re.sub(r'\s*fileSorterTitle = activeRoot\.Q<Label>\("FileSorterTitle"\);', '', content)

# Tetris
content = re.sub(r'\s*tetrisContainer = activeRoot\.Q<VisualElement>\("TetrisContainer"\);', '', content)
content = re.sub(r'\s*tetrisGrid = activeRoot\.Q<VisualElement>\("TetrisGrid"\);', '', content)
content = re.sub(r'\s*tetrisScoreText = activeRoot\.Q<Label>\("TetrisScoreText"\);', '', content)
content = re.sub(r'\s*btnTetrisLeft = activeRoot\.Q<Button>\("BtnTetrisLeft"\);', '', content)
content = re.sub(r'\s*btnTetrisRight = activeRoot\.Q<Button>\("BtnTetrisRight"\);', '', content)
content = re.sub(r'\s*btnTetrisRotate = activeRoot\.Q<Button>\("BtnTetrisRotate"\);', '', content)
content = re.sub(r'\s*btnTetrisDrop = activeRoot\.Q<Button>\("BtnTetrisDrop"\);', '', content)
content = re.sub(r'\s*if \(btnTetrisLeft != null\) btnTetrisLeft\.clicked \+= \(\) => OnTetrisInput\(-1, 0, 0\);', '', content)
content = re.sub(r'\s*if \(btnTetrisRight != null\) btnTetrisRight\.clicked \+= \(\) => OnTetrisInput\(1, 0, 0\);', '', content)
content = re.sub(r'\s*if \(btnTetrisRotate != null\) btnTetrisRotate\.clicked \+= \(\) => OnTetrisInput\(0, 1, 0\);', '', content)
content = re.sub(r'\s*if \(btnTetrisDrop != null\) btnTetrisDrop\.clicked \+= \(\) => OnTetrisInput\(0, 0, 1\);', '', content)

# KanbanBoard
content = re.sub(r'\s*kanbanContainer = activeRoot\.Q<VisualElement>\("KanbanContainer"\);', '', content)
content = re.sub(r'\s*kanbanColumn1 = activeRoot\.Q<VisualElement>\("KanbanColumn1"\);', '', content)
content = re.sub(r'\s*kanbanColumn2 = activeRoot\.Q<VisualElement>\("KanbanColumn2"\);', '', content)
content = re.sub(r'\s*kanbanColumn3 = activeRoot\.Q<VisualElement>\("KanbanColumn3"\);', '', content)
content = re.sub(r'\s*kanbanScoreText = activeRoot\.Q<Label>\("KanbanScoreText"\);', '', content)

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'w') as f:
    f.write(content)
print("Cleaned up leftovers")

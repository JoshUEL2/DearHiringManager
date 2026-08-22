import re

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'r') as f:
    content = f.read()

content = re.sub(r'\s*fileSorterActiveFileText = activeRoot\.Q<Label>\("FileSorterActiveFileText"\);', '', content)
content = re.sub(r'\s*if \(fileSorterActiveFile != null\)\s*\{\s*fileSorterActiveFile\?\.RegisterCallback<PointerDownEvent>\(ev => OnFileSorterDown\(ev\)\);\s*fileSorterActiveFile\?\.RegisterCallback<PointerMoveEvent>\(ev => OnFileSorterMove\(ev\)\);\s*fileSorterActiveFile\?\.RegisterCallback<PointerUpEvent>\(ev => OnFileSorterUp\(ev\)\);\}', '', content)

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'w') as f:
    f.write(content)
print("Removed FileSorter pointers")

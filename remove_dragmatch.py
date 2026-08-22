import re

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'r') as f:
    content = f.read()

content = re.sub(r'\s*dragMatchContainer = activeRoot\.Q<VisualElement>\("DragMatchContainer"\);', '', content)
content = re.sub(r'\s*dragLeftColumn = activeRoot\.Q<VisualElement>\("DragLeftColumn"\);', '', content)
content = re.sub(r'\s*dragRightColumn = activeRoot\.Q<VisualElement>\("DragRightColumn"\);', '', content)
content = re.sub(r'\s*dragCheatSheetList = activeRoot\.Q<VisualElement>\("DragCheatSheetList"\);', '', content)
content = re.sub(r'\s*matchPairScoreText = activeRoot\.Q<Label>\("MatchPairScoreText"\);', '', content)

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'w') as f:
    f.write(content)
print("Removed DragMatch vars")

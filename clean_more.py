import re

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'r') as f:
    content = f.read()

content = re.sub(r'\s*fileSorterTimerText = activeRoot\.Q<Label>\("FileSorterTimerText"\);', '', content)
content = re.sub(r'\s*fileSorterProgressText = activeRoot\.Q<Label>\("FileSorterProgressText"\);', '', content)

content = re.sub(r'\s*codeBreakerContainer = activeRoot\.Q<VisualElement>\("CodeBreakerContainer"\);', '', content)
content = re.sub(r'\s*codeBreakerTargetText = activeRoot\.Q<Label>\("CodeBreakerTargetText"\);', '', content)
content = re.sub(r'\s*codeGuessRow = activeRoot\.Q<VisualElement>\("CodeGuessRow"\);', '', content)
content = re.sub(r'\s*codeSymbolRow = activeRoot\.Q<VisualElement>\("CodeSymbolRow"\);', '', content)
content = re.sub(r'\s*codeHistoryList = activeRoot\.Q<VisualElement>\("CodeHistoryList"\);', '', content)
content = re.sub(r'\s*codeTimerText = activeRoot\.Q<Label>\("CodeTimerText"\);', '', content)
content = re.sub(r'\s*btnCodeSubmit = activeRoot\.Q<Button>\("BtnCodeSubmit"\);', '', content)
content = re.sub(r'\s*btnCodeClear = activeRoot\.Q<Button>\("BtnCodeClear"\);', '', content)
content = re.sub(r'\s*if \(btnCodeSubmit != null\) btnCodeSubmit\.clicked \+= OnCodeSubmit;', '', content)
content = re.sub(r'\s*if \(btnCodeClear != null\) btnCodeClear\.clicked \+= OnCodeClear;', '', content)

content = re.sub(r'\s*scrubPlateContainer = activeRoot\.Q<VisualElement>\("ScrubPlateContainer"\);', '', content)
content = re.sub(r'\s*scrubProgressBar = activeRoot\.Q<ProgressBar>\("ScrubProgressBar"\);', '', content)
content = re.sub(r'\s*scrubPlateArea = activeRoot\.Q<VisualElement>\("ScrubPlateArea"\);', '', content)
content = re.sub(r'\s*scrubDirtOverlay = activeRoot\.Q<VisualElement>\("ScrubDirtOverlay"\);', '', content)
content = re.sub(r'\s*scrubSponge = activeRoot\.Q<VisualElement>\("ScrubSponge"\);', '', content)
content = re.sub(r'\s*if \(scrubPlateArea != null\)\s*\{\s*scrubPlateArea\?\.RegisterCallback<PointerDownEvent>\(ev => \{[^\}]+\}[^\}]+\}[^\}]+\}[^\}]+\}\s*\}\);', '', content)
content = re.sub(r'\s*scrubPlateArea\?\.RegisterCallback<PointerDownEvent>\(ev => \{[^\}]+\}[^\}]+\}[^\}]+\}[^\}]+\}\s*\}\);', '', content)
content = re.sub(r'\s*scrubPlateArea\?\.RegisterCallback<PointerDownEvent>\([^\)]+\);', '', content)
content = re.sub(r'\s*scrubPlateArea\?\.RegisterCallback<PointerMoveEvent>\([^\)]+\);', '', content)
content = re.sub(r'\s*scrubPlateArea\?\.RegisterCallback<PointerUpEvent>\([^\)]+\);', '', content)

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'w') as f:
    f.write(content)
print("Removed remaining vars")

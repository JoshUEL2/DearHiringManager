import re

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'r') as f:
    content = f.read()

content = re.sub(r'\s*ztContainer = activeRoot\.Q<VisualElement>\("ZenTracingContainer"\);', '', content)
content = re.sub(r'\s*if \(ztContainer != null\)\s*\{\s*ztCircle = ztContainer\.Q<VisualElement>\("ZtCircle"\);\s*ztTarget = ztContainer\.Q<VisualElement>\("ZtTarget"\);\s*ztSponge = ztContainer\.Q<VisualElement>\("ZtSponge"\);\s*ztProgressFill = ztContainer\.Q<VisualElement>\("ZtProgressFill"\);\s*ztProgressText = ztContainer\.Q<Label>\("ZtProgressText"\);\s*\}', '', content)

content = re.sub(r'\s*if \(ztContainer != null\)\s*\{\s*ztContainer\?\.RegisterCallback<PointerMoveEvent>\(ev => \{[^\}]+\}[^\}]+\}[^\}]+\}[^\}]+\}\s*\}\);', '', content)
content = re.sub(r'\s*ztContainer\?\.RegisterCallback<PointerDownEvent>\(ev => \{[^\}]+\}\s*\);', '', content)
content = re.sub(r'\s*\}', '}', content) # Fix formatting if needed

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'w') as f:
    f.write(content)
print("Removed ZT from MiniGameManager")

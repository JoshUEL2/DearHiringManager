import re

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'r') as f:
    content = f.read()

content = re.sub(r'\s*orderQueueContainer = activeRoot\.Q<VisualElement>\("OrderQueueContainer"\);', '', content)
content = re.sub(r'\s*orderItemGrid = activeRoot\.Q<VisualElement>\("OrderItemGrid"\);', '', content)
content = re.sub(r'\s*orderStatusText = activeRoot\.Q<Label>\("OrderStatusText"\);', '', content)
content = re.sub(r'\s*orderTimerText = activeRoot\.Q<Label>\("OrderTimerText"\);', '', content)

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'w') as f:
    f.write(content)
print("Removed OrderQueue vars")

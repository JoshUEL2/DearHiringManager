import re

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'r') as f:
    content = f.read()

content = re.sub(r'\s*chartAdjusterContainer = activeRoot\.Q<VisualElement>\("ChartAdjusterContainer"\);', '', content)
content = re.sub(r'\s*caCompetitorBar = activeRoot\.Q<VisualElement>\("CaCompetitorBar"\);', '', content)
content = re.sub(r'\s*caAverageBar = activeRoot\.Q<VisualElement>\("CaAverageBar"\);', '', content)
content = re.sub(r'\s*caOurBar = activeRoot\.Q<VisualElement>\("CaOurBar"\);', '', content)
content = re.sub(r'\s*caTimerText = activeRoot\.Q<Label>\("CaTimerText"\);', '', content)
content = re.sub(r'\s*caStatusText = activeRoot\.Q<Label>\("CaStatusText"\);', '', content)

content = re.sub(r'\s*var caChartArea = activeRoot\.Q<VisualElement>\("CaChartArea"\);', '', content)
content = re.sub(r'\s*if \(caChartArea != null\)\s*\{\s*Vector2 lastPos = Vector2\.zero;\s*caChartArea\?\.RegisterCallback<PointerDownEvent>\(ev => \{[^\}]+\}[^\}]+\}[^\}]+\}[^\}]+\}[^\}]+\}[^\}]+\}[^\}]+\}[^\}]+\}[^\}]+\}\s*\);\s*\}', '', content)

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'w') as f:
    f.write(content)
print("Removed ChartAdjuster vars")

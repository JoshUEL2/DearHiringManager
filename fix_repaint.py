import re

path = 'Assets/Scripts/2 Managers/MiniGames/PumpSystemMiniGame.cs'
with open(path, 'r') as f:
    content = f.read()

target = r'''                bubbleTime \+= Time\.deltaTime;
                bool isToilet = config != null && config\.instructions\.ToLower\(\)\.Contains\("toilet"\);
                if \(isToilet && pumpTrackArea != null\) \{
                    pumpTrackArea\.MarkDirtyRepaint\(\);
                \} else if \(pumpNormalBar != null\) \{
                    pumpNormalBar\.MarkDirtyRepaint\(\);
                \}'''

replacement = r'''                bubbleTime += Time.deltaTime;
                if (pumpNormalBar != null) pumpNormalBar.MarkDirtyRepaint();'''

content = re.sub(target, replacement, content)

with open(path, 'w') as f:
    f.write(content)

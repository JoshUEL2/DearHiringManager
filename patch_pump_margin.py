import re

path = 'Assets/Scripts/2 Managers/MiniGames/PumpSystemMiniGame.cs'
with open(path, 'r') as f:
    content = f.read()

target = r'''        if \(pumpTrackArea != null\)
        \{
            pumpTrackArea\.style\.width = 120;
            pumpTrackArea\.style\.borderTopLeftRadius = 60;
            pumpTrackArea\.style\.borderTopRightRadius = 60;
            pumpTrackArea\.style\.borderBottomLeftRadius = 60;
            pumpTrackArea\.style\.borderBottomRightRadius = 60;
        \}'''

replacement = r'''        if (pumpTrackArea != null)
        {
            pumpTrackArea.style.width = 120;
            pumpTrackArea.style.borderTopLeftRadius = 60;
            pumpTrackArea.style.borderTopRightRadius = 60;
            pumpTrackArea.style.borderBottomLeftRadius = 60;
            pumpTrackArea.style.borderBottomRightRadius = 60;
            pumpTrackArea.style.marginRight = 40; // Add padding so the handle doesn't clip on the popup edge!
        }'''
content = re.sub(target, replacement, content)

with open(path, 'w') as f:
    f.write(content)

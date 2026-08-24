import re

path = 'Assets/Scripts/2 Managers/MiniGames/ScrubPlateMiniGame.cs'

with open(path, 'r') as f:
    content = f.read()

target = r'scrubProgressBar = root\.Q<ProgressBar>\("ScrubProgressBar"\);'
replacement = r'''scrubProgressBar = root.Q<ProgressBar>("ScrubProgressBar");
        if (scrubProgressBar != null) scrubProgressBar.style.display = DisplayStyle.None;'''

content = re.sub(target, replacement, content)

with open(path, 'w') as f:
    f.write(content)

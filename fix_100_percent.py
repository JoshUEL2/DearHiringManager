import re

path = 'Assets/Scripts/2 Managers/MiniGames/ScrubPlateMiniGame.cs'
with open(path, 'r') as f:
    content = f.read()

content = re.sub(
    r'if \(dirtParticles\.Count < initialDirtCount \* 0\.04f\) // 96% cleaned',
    r'if (dirtParticles.Count == 0) // 100% cleaned',
    content
)

with open(path, 'w') as f:
    f.write(content)

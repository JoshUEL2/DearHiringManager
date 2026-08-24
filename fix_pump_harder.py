import re

path = 'Assets/Scripts/2 Managers/MiniGames/PumpSystemMiniGame.cs'
with open(path, 'r') as f:
    content = f.read()

target_decay = r'''                float decayFactor = 1f \+ \(pumpProgress / 100f\) \* 2f; 
                pumpProgress -= \(decayRate \* 0\.85f\) \* decayFactor \* Time\.deltaTime;'''

replacement_decay = r'''                float decayFactor = 1f + (pumpProgress / 100f) * 2f; 
                pumpProgress -= (decayRate * 1.7f) * decayFactor * Time.deltaTime;'''

content = re.sub(target_decay, replacement_decay, content)

with open(path, 'w') as f:
    f.write(content)

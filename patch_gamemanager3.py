import re

with open('Assets/Scripts/1 Data/GameManager.cs', 'r') as f:
    content = f.read()

content = content.replace(
    'if (availableJobOpenings != null) availableJobOpenings.Clear();',
    'if (rejectedJobsCooldown == null) rejectedJobsCooldown = new System.Collections.Generic.Dictionary<string, int>();\n        if (availableJobOpenings != null) availableJobOpenings.Clear();'
)

with open('Assets/Scripts/1 Data/GameManager.cs', 'w') as f:
    f.write(content)


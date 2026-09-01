import re

with open('Assets/Scripts/1 Data/GameManager.cs', 'r') as f:
    content = f.read()

content = content.replace(
    'if (availableJobOpenings != null) availableJobOpenings.Clear();',
    'if (availableJobOpenings != null) availableJobOpenings.Clear();\n        else availableJobOpenings = new System.Collections.Generic.List<JobListingSO>();'
)

with open('Assets/Scripts/1 Data/GameManager.cs', 'w') as f:
    f.write(content)


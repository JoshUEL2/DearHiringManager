import re

with open('Assets/Scripts/1 Data/GameManager.cs', 'r') as f:
    content = f.read()

content = content.replace(
    'if (_zoneJobsCache != null) _zoneJobsCache.Clear();',
    'if (_zoneJobsCache != null) _zoneJobsCache.Clear();\n        else _zoneJobsCache = new System.Collections.Generic.List<JobListingSO>();'
)

with open('Assets/Scripts/1 Data/GameManager.cs', 'w') as f:
    f.write(content)


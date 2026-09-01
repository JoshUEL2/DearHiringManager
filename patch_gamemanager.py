import re

with open("Assets/Scripts/1 Data/GameManager.cs", "r") as f:
    content = f.read()

# Add log to AdvanceWeek
old = """        if (!isGameOver)
        {
            currentData.currentWeek++;
            TickCooldowns();
            CycleJobs();
            SaveGame();
        }"""
new = """        if (!isGameOver)
        {
            currentData.currentWeek++;
            Debug.Log($"<color=#38BDF8><b>=== WEEK {currentData.currentWeek} ===</b></color>");
            TickCooldowns();
            CycleJobs();
            SaveGame();
        }"""

content = content.replace(old, new)

with open("Assets/Scripts/1 Data/GameManager.cs", "w") as f:
    f.write(content)

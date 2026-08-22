import re

path = 'Assets/Scenes/Game.unity'
with open(path, 'r') as f:
    lines = f.readlines()

vars_to_fix = [
    "miniGameDocument", "currentGame", "isPlaying", "score",
    "currentData", "isGameOver", "gameOverReason", "activeEventPopup",
    "activeNewsFeed", "availableJobOpenings", "financialHistory",
    "hasHustledThisWeek", "rentDiscountThisWeek", "jobMarketLuckBonus",
    "completedRegularTaskThisWeek", "gameDatabase"
]

new_lines = []
for line in lines:
    replaced = False
    for v in vars_to_fix:
        if line.startswith(f"  {v}:"):
            new_lines.append(line.replace(f"  {v}:", f"  _{v}:"))
            replaced = True
            break
    if not replaced:
        new_lines.append(line)

with open(path, 'w') as f:
    f.writelines(new_lines)


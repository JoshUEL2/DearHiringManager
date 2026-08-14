import sys

with open("Assets/Scripts/2 Managers/MiniGameManager.cs", "r") as f:
    cs = f.read()

old_logic = "GameManager.Instance.AddFinancialHistory(logMessage, -fine, 0.0);"
new_logic = """GameManager.Instance.activeNewsFeed = logMessage;
                GameManager.Instance.financialHistory.Add(new FinancialSnapshot(GameManager.Instance.currentData.currentWeek, GameManager.Instance.currentData.balance, 0, -fine));"""

cs = cs.replace(old_logic, new_logic)

with open("Assets/Scripts/2 Managers/MiniGameManager.cs", "w") as f:
    f.write(cs)

print("Fixed history logic")

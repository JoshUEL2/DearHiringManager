import re

with open("Assets/Scripts/2 Managers/GameUIManager.cs", "r") as f:
    content = f.read()

# Insert after: root.Query<Label>("PlayerNameText").ForEach(l => l.text = playerName);
hook1 = 'root.Query<Label>("PlayerNameText").ForEach(l => l.text = playerName);'
new_hook1 = hook1 + '\n            root.Query<Button>(className: "id-card-name-btn").ForEach(b => b.text = playerName);'
content = content.replace(hook1, new_hook1)

# Insert after: root.Query<Label>("ExpenseText").ForEach(l => l.text = $"-${(data.rent + data.food)}");
hook2 = 'root.Query<Label>("ExpenseText").ForEach(l => l.text = $"-${(data.rent + data.food)}");'
new_hook2 = hook2 + '\n            double income = GameManager.Instance.GetBaseSalary(data.jobTitle);\n            root.Query<Label>("IncomeText").ForEach(l => l.text = $"+${income:F2}");'
content = content.replace(hook2, new_hook2)

with open("Assets/Scripts/2 Managers/GameUIManager.cs", "w") as f:
    f.write(content)

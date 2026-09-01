import re

with open("Assets/Scripts/2 Managers/GameUIManager.cs", "r") as f:
    content = f.read()

bad = """        // Ensure Home Icon works globally
        root.Query<Button>("BtnOpenMigration").ForEach(btn => {
            btn.clicked -= MigrationUI.OpenPortal;
            btn.clicked += MigrationUI.OpenPortal;
        });"""

good = """        // Ensure Home Icon works globally
        root.Query<Button>("BtnOpenMigration").ForEach(btn => {
            btn.RegisterCallback<ClickEvent>(ev => MigrationUI?.OpenPortal());
        });"""

content = content.replace(bad, good)

with open("Assets/Scripts/2 Managers/GameUIManager.cs", "w") as f:
    f.write(content)

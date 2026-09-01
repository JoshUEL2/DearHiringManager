import re

with open("Assets/Scripts/2 Managers/GameUIManager.cs", "r") as f:
    content = f.read()

# Add BtnOpenMigration and BtnViewCV bindings globally
extra_bindings = """
        // Ensure CV Button works globally
        root.Query<Button>("BtnViewCV").ForEach(btn => {
            btn.clicked -= ShowCVPopup;
            btn.clicked += ShowCVPopup;
        });
        
        // Ensure Home Icon works globally
        root.Query<Button>("BtnOpenMigration").ForEach(btn => {
            btn.clicked -= MigrationUI.OpenPortal;
            btn.clicked += MigrationUI.OpenPortal;
        });
"""

content = content.replace('        root.Query<Button>("BtnOpenSettings").ForEach(btnSettings => {', 
                          extra_bindings + '        root.Query<Button>("BtnOpenSettings").ForEach(btnSettings => {')

with open("Assets/Scripts/2 Managers/GameUIManager.cs", "w") as f:
    f.write(content)

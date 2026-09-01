import re

with open("Assets/Scripts/2 Managers/GameUIManager.cs", "r") as f:
    content = f.read()

# Fix btnOpenSettings binding in Start()
# 118:        btnOpenSettings = dashboardRoot.Q<Button>("BtnOpenSettings");
# 130:        if (btnOpenSettings != null) btnOpenSettings.clicked += OpenSettings;
# We will just do `dashboardRoot.Query<Button>("BtnOpenSettings").ForEach(btn => btn.clicked += OpenSettings);`
old_start = 'btnOpenSettings = dashboardRoot.Q<Button>("BtnOpenSettings");'
content = content.replace(old_start, 'dashboardRoot.Query<Button>("BtnOpenSettings").ForEach(btn => btn.clicked += OpenSettings);')
content = content.replace('if (btnOpenSettings != null) btnOpenSettings.clicked += OpenSettings;', '')

# Fix btnSettings in InitializeSubScreen
old_init_settings = """        var btnSettings = root.Q<Button>("BtnOpenSettings");
        if (btnSettings != null) {
            btnSettings.clicked -= OpenSettings; // Prevent double firing
            btnSettings.clicked += OpenSettings;
        }"""
new_init_settings = """        root.Query<Button>("BtnOpenSettings").ForEach(btnSettings => {
            btnSettings.clicked -= OpenSettings;
            btnSettings.clicked += OpenSettings;
        });"""
content = content.replace(old_init_settings, new_init_settings)

# Fix btnViewCV in InitializeSubScreen
old_viewcv = """        var btnViewCV = root.Q<Button>("BtnViewCV");
        if (btnViewCV != null) {
            btnViewCV.clicked -= OnViewCVClicked;
            btnViewCV.clicked += OnViewCVClicked;
        }"""
new_viewcv = """        root.Query<Button>("BtnViewCV").ForEach(btnViewCV => {
            btnViewCV.clicked -= OnViewCVClicked;
            btnViewCV.clicked += OnViewCVClicked;
        });"""
content = content.replace(old_viewcv, new_viewcv)


# Let's also check if btnViewCV is bound in Start()
old_viewcv_start = 'var btnViewCV = dashboardRoot.Q<Button>("BtnViewCV");'
if old_viewcv_start in content:
    # Actually, it looks like InitializeSubScreen does the binding globally. Let's find it.
    pass
else:
    # Use regex
    content = re.sub(
        r'var btnViewCV = dashboardRoot\.Q<Button>\("BtnViewCV"\);\s*if \(btnViewCV != null\) btnViewCV\.clicked \+= OnViewCVClicked;',
        r'dashboardRoot.Query<Button>("BtnViewCV").ForEach(btn => btn.clicked += OnViewCVClicked);',
        content, flags=re.MULTILINE
    )

with open("Assets/Scripts/2 Managers/GameUIManager.cs", "w") as f:
    f.write(content)

with open("Assets/Scripts/2 Managers/GameUIManager.cs", "r") as f:
    content = f.read()

old = """        var btnViewCV = root.Q<Button>("BtnViewCV");
        if (btnViewCV != null)
        {
            btnViewCV.clicked -= ShowCVPopup;
            btnViewCV.clicked += ShowCVPopup;
        }"""
new = """        root.Query<Button>("BtnViewCV").ForEach(btnViewCV => {
            btnViewCV.clicked -= ShowCVPopup;
            btnViewCV.clicked += ShowCVPopup;
        });"""
content = content.replace(old, new)

with open("Assets/Scripts/2 Managers/GameUIManager.cs", "w") as f:
    f.write(content)

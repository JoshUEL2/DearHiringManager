import sys

with open("Assets/Scripts/2 Managers/MiniGameManager.cs", "r") as f:
    lines = f.readlines()

start = -1
end = -1
for i, line in enumerate(lines):
    if 'cgContainer = root.Q<VisualElement>("CorporateGolfContainer");' in line:
        start = i
    if 'if (cgSwingBtn != null) cgSwingBtn.clicked += () => OnCgSwingClicked();' in line:
        end = i
        break

if start != -1 and end != -1:
    new_awake = """        cgContainer = root.Q<UnityEngine.UIElements.VisualElement>("CorporateGolfContainer");
        cgPowerFill = root.Q<UnityEngine.UIElements.VisualElement>("CgPowerFill");
        cgSwingBtn = root.Q<UnityEngine.UIElements.Button>("BtnCgSwing");
        if (cgSwingBtn != null) {
            cgSwingBtn.RegisterCallback<UnityEngine.UIElements.PointerDownEvent>(ev => { cgIsSwinging = true; });
            cgSwingBtn.RegisterCallback<UnityEngine.UIElements.PointerUpEvent>(ev => OnCgSwingRelease());
            cgSwingBtn.RegisterCallback<UnityEngine.UIElements.PointerLeaveEvent>(ev => { if(cgIsSwinging) OnCgSwingRelease(); });
        }
"""
    lines = lines[:start] + [new_awake] + lines[end+1:]
    print("Successfully replaced CorporateGolf Awake bindings!")
else:
    print("Could not find CorporateGolf Awake bindings!")

with open("Assets/Scripts/2 Managers/MiniGameManager.cs", "w") as f:
    f.writelines(lines)

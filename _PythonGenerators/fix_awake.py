import sys

with open("Assets/Scripts/2 Managers/MiniGameManager.cs", "r") as f:
    lines = f.readlines()

def replace_between(start_str, end_str, new_str, lines):
    start = -1
    end = -1
    for i, line in enumerate(lines):
        if start_str in line and start == -1:
            start = i
        if end_str in line and start != -1 and end == -1:
            end = i
            break
    if start != -1 and end != -1:
        lines = lines[:start] + [new_str + "\n"] + lines[end+1:]
        return lines, True
    return lines, False

# 1. Replace mgContainer to wdContainer end
mg_wd_new = """        mgContainer = root.Q<UnityEngine.UIElements.VisualElement>("MowGreensContainer");
        mgGrid = root.Q<UnityEngine.UIElements.VisualElement>("MgGrid");
        if (mgGrid != null) {
            for (int i=0; i<25; i++) {
                var patch = new UnityEngine.UIElements.VisualElement();
                patch.style.width = new UnityEngine.UIElements.StyleLength(UnityEngine.UIElements.Length.Percent(20f));
                patch.style.height = new UnityEngine.UIElements.StyleLength(UnityEngine.UIElements.Length.Percent(20f));
                patch.style.backgroundColor = new UnityEngine.UIElements.StyleColor(new UnityEngine.Color(0.2f, 0.7f, 0.2f));
                patch.style.borderWidth = 1;
                patch.style.borderColor = new UnityEngine.UIElements.StyleColor(new UnityEngine.Color(0.1f, 0.4f, 0.1f));
                mgGrid.Add(patch);
                mgPatches.Add(patch);
            }
            mgContainer.RegisterCallback<UnityEngine.UIElements.PointerMoveEvent>(ev => OnMgPointerMove(ev));
            mgContainer.RegisterCallback<UnityEngine.UIElements.PointerDownEvent>(ev => OnMgPointerMove(ev));
        }

        wdContainer = root.Q<UnityEngine.UIElements.VisualElement>("WalkDogsContainer");
        for (int i=0; i<3; i++) {
            wdLeashes[i] = root.Q<UnityEngine.UIElements.VisualElement>($"WdLeash{i}");
            wdYankBtns[i] = root.Q<UnityEngine.UIElements.Button>($"BtnWdYank{i}");
            int idx = i;
            if (wdYankBtns[i] != null) wdYankBtns[i].clicked += () => OnWdYank(idx);
        }"""
lines, ok = replace_between("mgContainer = root.Q<VisualElement>(\"MowGreensContainer\");", "if (wdDogs[i] != null) wdDogs[i].clicked += () => OnWdDogClicked(dIdx);\n        }", mg_wd_new, lines)

cg_new = """        cgContainer = root.Q<UnityEngine.UIElements.VisualElement>("CorporateGolfContainer");
        cgPowerFill = root.Q<UnityEngine.UIElements.VisualElement>("CgPowerFill");
        cgSwingBtn = root.Q<UnityEngine.UIElements.Button>("BtnCgSwing");
        if (cgSwingBtn != null) {
            cgSwingBtn.RegisterCallback<UnityEngine.UIElements.PointerDownEvent>(ev => { cgIsSwinging = true; });
            cgSwingBtn.RegisterCallback<UnityEngine.UIElements.PointerUpEvent>(ev => OnCgSwingRelease());
            cgSwingBtn.RegisterCallback<UnityEngine.UIElements.PointerLeaveEvent>(ev => { if(cgIsSwinging) OnCgSwingRelease(); });
        }"""
lines, ok = replace_between("cgContainer = root.Q<VisualElement>(\"CorporateGolfContainer\");", "if (cgSwingBtn != null) cgSwingBtn.clicked += () => OnCgSwingClicked();", cg_new, lines)


# Inject RecycleBottles after fpContainer logic
rb_awake = """
        rbContainer = root.Q<UnityEngine.UIElements.VisualElement>("RecycleBottlesContainer");
        rbBag = root.Q<UnityEngine.UIElements.VisualElement>("RbBag");
        rbItemsContainer = root.Q<UnityEngine.UIElements.VisualElement>("RbItemsContainer");
        rbInstructionText = root.Q<UnityEngine.UIElements.Label>("RbInstructionText");
        
        rbItems[0] = root.Q<UnityEngine.UIElements.Button>("BtnRbItem0");
        rbItems[1] = root.Q<UnityEngine.UIElements.Button>("BtnRbItem1");
        rbItems[2] = root.Q<UnityEngine.UIElements.Button>("BtnRbItem2");
        rbItems[3] = root.Q<UnityEngine.UIElements.Button>("BtnRbTrash0");
        rbItems[4] = root.Q<UnityEngine.UIElements.Button>("BtnRbTrash1");
        rbItems[5] = root.Q<UnityEngine.UIElements.Button>("BtnRbTrash2");
        
        for (int i=0; i<6; i++) {
            int idx = i;
            if (rbItems[i] != null) rbItems[i].clicked += () => OnRbItemClicked(idx);
        }
        
        if (rbBag != null) {
            rbBag.RegisterCallback<UnityEngine.UIElements.PointerDownEvent>(ev => OnRbSwipe());
        }
"""
for i, line in enumerate(lines):
    if "fpStampBtn.clicked += () => OnFpStampClicked();" in line:
        lines.insert(i + 1, rb_awake)
        break

with open("Assets/Scripts/2 Managers/MiniGameManager.cs", "w") as f:
    f.writelines(lines)

import sys

with open("Assets/Scripts/2 Managers/MiniGameManager.cs", "r") as f:
    cs = f.read()

old_awake = """        cgContainer = root.Q<VisualElement>("CorporateGolfContainer");
        cgMarker = root.Q<VisualElement>("CgMarker");
        cgSwingBtn = root.Q<Button>("BtnCgSwing");
        if (cgSwingBtn != null) cgSwingBtn.clicked += () => OnCgSwingClicked();

        mgContainer = root.Q<VisualElement>("MowGreensContainer");
        for (int i=0; i<16; i++) {
            mgSquares[i] = root.Q<VisualElement>($"MgSq{i}");
        }

        wdContainer = root.Q<VisualElement>("WalkDogsContainer");
        for (int i=0; i<3; i++) {
            wdDogs[i] = root.Q<VisualElement>($"WdDog{i}");
            int idx = i;
            if (wdDogs[i] != null) {
                // Not buttons, so we register callbacks
                wdDogs[i].RegisterCallback<UnityEngine.UIElements.PointerDownEvent>(ev => OnWdDogClicked(idx));
            }
        }"""
new_awake = """        cgContainer = root.Q<VisualElement>("CorporateGolfContainer");
        cgPowerFill = root.Q<VisualElement>("CgPowerFill");
        cgSwingBtn = root.Q<Button>("BtnCgSwing");
        if (cgSwingBtn != null) {
            cgSwingBtn.RegisterCallback<UnityEngine.UIElements.PointerDownEvent>(ev => { cgIsSwinging = true; });
            cgSwingBtn.RegisterCallback<UnityEngine.UIElements.PointerUpEvent>(ev => OnCgSwingRelease());
            cgSwingBtn.RegisterCallback<UnityEngine.UIElements.PointerLeaveEvent>(ev => { if(cgIsSwinging) OnCgSwingRelease(); });
        }

        mgContainer = root.Q<VisualElement>("MowGreensContainer");
        mgGrid = root.Q<VisualElement>("MgGrid");
        if (mgGrid != null) {
            // Generate 25 patches dynamically in UXML
            for (int i=0; i<25; i++) {
                var patch = new UnityEngine.UIElements.VisualElement();
                patch.style.width = new UnityEngine.UIElements.StyleLength(UnityEngine.UIElements.Length.Percent(20f));
                patch.style.height = new UnityEngine.UIElements.StyleLength(UnityEngine.UIElements.Length.Percent(20f));
                patch.style.backgroundColor = new UnityEngine.UIElements.StyleColor(new UnityEngine.Color(0.2f, 0.7f, 0.2f)); // Tall grass
                patch.style.borderWidth = 1;
                patch.style.borderColor = new UnityEngine.UIElements.StyleColor(new UnityEngine.Color(0.1f, 0.4f, 0.1f));
                mgGrid.Add(patch);
                mgPatches.Add(patch);
            }
            // PointerMove on container
            mgContainer.RegisterCallback<UnityEngine.UIElements.PointerMoveEvent>(ev => OnMgPointerMove(ev));
            mgContainer.RegisterCallback<UnityEngine.UIElements.PointerDownEvent>(ev => OnMgPointerMove(ev));
        }

        wdContainer = root.Q<VisualElement>("WalkDogsContainer");
        for (int i=0; i<3; i++) {
            wdLeashes[i] = root.Q<VisualElement>($"WdLeash{i}");
            wdYankBtns[i] = root.Q<Button>($"BtnWdYank{i}");
            int idx = i;
            if (wdYankBtns[i] != null) wdYankBtns[i].clicked += () => OnWdYank(idx);
        }

        rbContainer = root.Q<VisualElement>("RecycleBottlesContainer");
        rbBag = root.Q<VisualElement>("RbBag");
        rbItemsContainer = root.Q<VisualElement>("RbItemsContainer");
        rbInstructionText = root.Q<Label>("RbInstructionText");
        
        rbItems[0] = root.Q<Button>("BtnRbItem0");
        rbItems[1] = root.Q<Button>("BtnRbItem1");
        rbItems[2] = root.Q<Button>("BtnRbItem2");
        rbItems[3] = root.Q<Button>("BtnRbTrash0");
        rbItems[4] = root.Q<Button>("BtnRbTrash1");
        rbItems[5] = root.Q<Button>("BtnRbTrash2");
        
        for (int i=0; i<6; i++) {
            int idx = i;
            if (rbItems[i] != null) rbItems[i].clicked += () => OnRbItemClicked(idx);
        }
        
        if (rbContainer != null) {
            // Register swipe on the bag/container
            rbBag.RegisterCallback<UnityEngine.UIElements.PointerDownEvent>(ev => OnRbSwipe());
        }"""
cs = cs.replace(old_awake, new_awake)

with open("Assets/Scripts/2 Managers/MiniGameManager.cs", "w") as f:
    f.write(cs)
print("Updated awake!")

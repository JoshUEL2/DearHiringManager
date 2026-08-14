import sys

with open("Assets/Scripts/2 Managers/MiniGameManager.cs", "r") as f:
    lines = f.readlines()

start = -1
end = -1
for i, line in enumerate(lines):
    if 'mgContainer = root.Q<VisualElement>("MowGreensContainer");' in line:
        start = i
    if 'if (wdDogs[i] != null) wdDogs[i].clicked += () => OnWdDogClicked(dIdx);' in line:
        end = i + 1 # include the closing brace on the next line
        break

if start != -1 and end != -1:
    new_awake = """        mgContainer = root.Q<UnityEngine.UIElements.VisualElement>("MowGreensContainer");
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
        }
"""
    lines = lines[:start] + [new_awake] + lines[end+1:]
    print("Successfully replaced MowGreens and WalkDogs Awake bindings!")
else:
    print("Could not find MowGreens and WalkDogs Awake bindings!")

with open("Assets/Scripts/2 Managers/MiniGameManager.cs", "w") as f:
    f.writelines(lines)

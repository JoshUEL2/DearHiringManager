import sys

with open("Assets/Scripts/2 Managers/MiniGameManager.cs", "r") as f:
    cs = f.read()

hide_anchor = "if(vrArcadeContainer != null) vrArcadeContainer.style.display = DisplayStyle.None;"
hide_inject = """if(vrArcadeContainer != null) vrArcadeContainer.style.display = DisplayStyle.None;
        if(hotStoneContainer != null) hotStoneContainer.style.display = DisplayStyle.None;
        if(hgContainer != null) hgContainer.style.display = DisplayStyle.None;
        if(spContainer != null) spContainer.style.display = DisplayStyle.None;
        if(cgContainer != null) cgContainer.style.display = DisplayStyle.None;
        if(mgContainer != null) mgContainer.style.display = DisplayStyle.None;
        if(wdContainer != null) wdContainer.style.display = DisplayStyle.None;
        if(fpContainer != null) fpContainer.style.display = DisplayStyle.None;
        if(sbContainer != null) sbContainer.style.display = DisplayStyle.None;
        if(saContainer != null) saContainer.style.display = DisplayStyle.None;"""

cs = cs.replace(hide_anchor, hide_inject)

with open("Assets/Scripts/2 Managers/MiniGameManager.cs", "w") as f:
    f.write(cs)

print("Fixed HideAllContainers!")

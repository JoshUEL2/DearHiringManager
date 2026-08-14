import sys

with open("Assets/Scripts/2 Managers/MiniGameManager.cs", "r") as f:
    cs = f.read()

switch_anchor = """            case GameType.WalkDogs:
                if (wdContainer != null) wdContainer.style.display = DisplayStyle.Flex;
                SetupWalkDogs();
                break;"""
switch_inject = """            case GameType.WalkDogs:
                if (wdContainer != null) wdContainer.style.display = DisplayStyle.Flex;
                SetupWalkDogs();
                break;
            case GameType.RecycleBottles:
                if (rbContainer != null) rbContainer.style.display = DisplayStyle.Flex;
                SetupRecycleBottles();
                break;"""
cs = cs.replace(switch_anchor, switch_inject)

with open("Assets/Scripts/2 Managers/MiniGameManager.cs", "w") as f:
    f.write(cs)

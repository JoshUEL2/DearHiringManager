import sys

with open("Assets/Scripts/2 Managers/MiniGameManager.cs", "r") as f:
    lines = f.readlines()

# 1. Replace variables for Mow Greens, Walk Dogs, Recycle Bottles
start_vars = -1
end_vars = -1
for i, line in enumerate(lines):
    if "private UnityEngine.UIElements.VisualElement mgContainer;" in line:
        start_vars = i - 1 # include the comment
    if "private int wdSelectedDog = -1;" in line:
        end_vars = i
        break

if start_vars != -1 and end_vars != -1:
    new_vars = """    // Mow Greens
    private UnityEngine.UIElements.VisualElement mgContainer;
    private UnityEngine.UIElements.VisualElement mgGrid;
    private System.Collections.Generic.List<UnityEngine.UIElements.VisualElement> mgPatches = new System.Collections.Generic.List<UnityEngine.UIElements.VisualElement>();
    private bool[] mgCut = new bool[25];
    private int mgCutCount = 0;

    // Walk Dogs
    private UnityEngine.UIElements.VisualElement wdContainer;
    private UnityEngine.UIElements.VisualElement[] wdLeashes = new UnityEngine.UIElements.VisualElement[3];
    private float[] wdTension = new float[3];
    private UnityEngine.UIElements.Button[] wdYankBtns = new UnityEngine.UIElements.Button[3];

    // Recycle Bottles
    private UnityEngine.UIElements.VisualElement rbContainer;
    private UnityEngine.UIElements.VisualElement rbBag;
    private UnityEngine.UIElements.VisualElement rbItemsContainer;
    private UnityEngine.UIElements.Label rbInstructionText;
    private UnityEngine.UIElements.Button[] rbItems = new UnityEngine.UIElements.Button[6];
    private int rbSwipes = 0;
    private int rbCollected = 0;
    private bool rbBagOpen = false;
"""
    lines[start_vars:end_vars+1] = [new_vars]

# Write back to file to make it easier to process next steps if line numbers change
with open("Assets/Scripts/2 Managers/MiniGameManager.cs", "w") as f:
    f.writelines(lines)

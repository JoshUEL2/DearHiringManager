import sys

with open("Assets/Scripts/2 Managers/MiniGameManager.cs", "r") as f:
    cs = f.read()

# 1. Variables
old_vars = """    // Mow Greens
    private UnityEngine.UIElements.VisualElement mgContainer;
    private UnityEngine.UIElements.VisualElement[] mgSquares = new UnityEngine.UIElements.VisualElement[16];
    private bool[] mgCut = new bool[16];
    private int mgSelected = 0;

    // Walk Dogs
    private UnityEngine.UIElements.VisualElement wdContainer;
    private UnityEngine.UIElements.VisualElement[] wdDogs = new UnityEngine.UIElements.VisualElement[3];
    private int[] wdColors = new int[3];
    private int wdSelectedDog = -1;"""
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
    private bool rbBagOpen = false;"""
cs = cs.replace(old_vars, new_vars)

old_cg_vars = """    // Corporate Golf Sim
    private UnityEngine.UIElements.VisualElement cgContainer;
    private UnityEngine.UIElements.VisualElement cgMarker;
    private UnityEngine.UIElements.Button cgSwingBtn;
    private float cgMarkerPos = 0f;
    private float cgMarkerDir = 1f;"""
new_cg_vars = """    // Corporate Golf Sim
    private UnityEngine.UIElements.VisualElement cgContainer;
    private UnityEngine.UIElements.VisualElement cgPowerFill;
    private UnityEngine.UIElements.Button cgSwingBtn;
    private float cgPowerLevel = 0f;
    private float cgPowerDir = 1f;
    private bool cgIsSwinging = false;"""
cs = cs.replace(old_cg_vars, new_cg_vars)

with open("Assets/Scripts/2 Managers/MiniGameManager.cs", "w") as f:
    f.write(cs)
print("Updated vars!")

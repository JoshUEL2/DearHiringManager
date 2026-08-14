with open("Assets/Scripts/2 Managers/GameUIManager.cs", "r") as f:
    lines = f.readlines()

start = -1
end = -1
for i, line in enumerate(lines):
    if "private void SwitchTab(VisualElement targetRoot)" in line:
        start = i
    if start != -1 and "RefreshDashboardUI(" in line and i > start:
        end = i + 1
        break

if start != -1 and end != -1:
    new_code = """    private Coroutine activeModalCoroutine;

    private void SwitchTab(VisualElement targetRoot)
    {
        if (GameManager.Instance.currentData.jailWeeksRemaining > 0) return;

        if (targetRoot == dashboardRoot)
        {
            CloseActiveModal();
            return;
        }

        if (targetRoot.style.display == DisplayStyle.Flex) return; // Already open

        // If another modal is already open, instantly hide it to swap
        HideAllTabs(); 
        dashboardRoot.style.display = DisplayStyle.Flex;
        
        // Hide bottom nav
        var bottomNav = dashboardRoot.Q<VisualElement>(className: "bottom-nav-container");
        if (bottomNav != null)
        {
            bottomNav.EnableInClassList("nav-hidden", true);
        }

        // Show Modal
        targetRoot.style.display = DisplayStyle.Flex;
        targetRoot.pickingMode = PickingMode.Position;
        activeOverlay = targetRoot;
        
        // Remove modal-hidden class to trigger slide up
        if (activeModalCoroutine != null) StopCoroutine(activeModalCoroutine);
        activeModalCoroutine = StartCoroutine(AnimateModalEntrance(targetRoot));

        // Refresh tab controllers via their static instances
        if (CareerUI.Instance != null) CareerUI.Instance.RefreshView();
        if (HustlesUI.Instance != null) HustlesUI.Instance.PopulateHustlesBoard();
        if (EducationUI.Instance != null) EducationUI.Instance.RefreshView();
        if (ActivitiesUI.Instance != null) ActivitiesUI.Instance.RefreshView();
    }

    private System.Collections.IEnumerator AnimateModalEntrance(VisualElement root)
    {
        yield return null; // Wait 1 frame
        root.EnableInClassList("modal-hidden", false);
    }

    private void CloseActiveModal()
    {
        if (activeOverlay != null && activeOverlay != dashboardRoot)
        {
            activeOverlay.EnableInClassList("modal-hidden", true);
            
            var bottomNav = dashboardRoot.Q<VisualElement>(className: "bottom-nav-container");
            if (bottomNav != null)
            {
                bottomNav.EnableInClassList("nav-hidden", false);
            }
            
            if (activeModalCoroutine != null) StopCoroutine(activeModalCoroutine);
            activeModalCoroutine = StartCoroutine(HideModalAfterAnimation(activeOverlay));
            activeOverlay = null;
        }
    }

    private System.Collections.IEnumerator HideModalAfterAnimation(VisualElement root)
    {
        yield return new WaitForSeconds(0.25f);
        root.style.display = DisplayStyle.None;
        root.pickingMode = PickingMode.Ignore;
    }
"""
    lines[start:end] = [new_code + "\n"]
    
    with open("Assets/Scripts/2 Managers/GameUIManager.cs", "w") as f:
        f.write("".join(lines))
    print("Replaced SwitchTab block!")
else:
    print(f"Could not find block. Start: {start}, End: {end}")

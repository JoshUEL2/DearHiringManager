import re

with open('/Users/josh/.gemini/antigravity/brain/df5a1394-1942-41b4-9af1-cd17e7a8c488/scratch/GameUIManager.cs', 'r') as f:
    content = f.read()

# 1. Remove SetupNavBar and SetupTopCard for non-dashboard roots
content = re.sub(r'SetupNavBar\(careersRoot\);\s*SetupTopCard\(careersRoot\);', '', content)
content = re.sub(r'SetupNavBar\(hustlesRoot\);\s*SetupTopCard\(hustlesRoot\);', '', content)
content = re.sub(r'SetupNavBar\(educationRoot\);\s*SetupTopCard\(educationRoot\);', '', content)
content = re.sub(r'SetupNavBar\(activitiesRoot\);\s*SetupTopCard\(activitiesRoot\);', '', content)

# 2. Add BtnCloseModal binding in Start
close_bindings = """
        // Bind Modal Close buttons
        careersRoot.Q<Button>("BtnCloseModal")?.RegisterCallback<ClickEvent>(ev => CloseActiveModal());
        hustlesRoot.Q<Button>("BtnCloseModal")?.RegisterCallback<ClickEvent>(ev => CloseActiveModal());
        educationRoot.Q<Button>("BtnCloseModal")?.RegisterCallback<ClickEvent>(ev => CloseActiveModal());
        activitiesRoot.Q<Button>("BtnCloseModal")?.RegisterCallback<ClickEvent>(ev => CloseActiveModal());

        HideAllTabs();
"""
content = content.replace("HideAllTabs();", close_bindings, 1)

# 3. Rewrite HideAllTabs
old_hide_all = """    private void HideAllTabs()
    {
        var roots = new[] { creationRoot, dashboardRoot, careersRoot, hustlesRoot, educationRoot, activitiesRoot, jailRoot };
        foreach (var r in roots)
        {
            if (r != null)
            {
                r.style.display = DisplayStyle.None;
                r.pickingMode = PickingMode.Ignore; // CRITICAL: prevent hidden tabs from blocking clicks
            }
        }
    }"""
    
new_hide_all = """    private void HideAllTabs()
    {
        var roots = new[] { creationRoot, careersRoot, hustlesRoot, educationRoot, activitiesRoot, jailRoot };
        foreach (var r in roots)
        {
            if (r != null)
            {
                r.style.display = DisplayStyle.None;
                r.pickingMode = PickingMode.Ignore; 
            }
        }
        
        if (dashboardRoot != null) {
            dashboardRoot.style.display = DisplayStyle.Flex;
            dashboardRoot.pickingMode = PickingMode.Position;
        }
    }"""
content = content.replace(old_hide_all, new_hide_all)

# 4. Rewrite CheckJailStatus tab check
old_any_tab_active = """            var roots = new[] { dashboardRoot, careersRoot, hustlesRoot, educationRoot, activitiesRoot };
            foreach (var r in roots)
            {
                if (r != null && r.style.display == DisplayStyle.Flex)
                {
                    anyTabActive = true;
                    break;
                }
            }"""
new_any_tab_active = """            var roots = new[] { careersRoot, hustlesRoot, educationRoot, activitiesRoot };
            foreach (var r in roots)
            {
                if (r != null && r.style.display == DisplayStyle.Flex)
                {
                    anyTabActive = true;
                    break;
                }
            }
            if (dashboardRoot != null && dashboardRoot.style.display == DisplayStyle.Flex) anyTabActive = true;"""
content = content.replace(old_any_tab_active, new_any_tab_active)

# 5. Rewrite SwitchTab & Add CloseActiveModal
old_switch_tab = """    private void SwitchTab(VisualElement targetRoot)
    {
        if (GameManager.Instance.currentData.jailWeeksRemaining > 0) return;

        if (targetRoot == dashboardRoot || (targetRoot != dashboardRoot && targetRoot.style.display == DisplayStyle.Flex))
        {
            HideAllTabs();
            dashboardRoot.style.display = DisplayStyle.Flex;
            dashboardRoot.pickingMode = PickingMode.Position;
            activeOverlay = null;
            RefreshDashboardUI(GameManager.Instance.currentData.playerName);
            return;
        }

        HideAllTabs();
        targetRoot.style.display = DisplayStyle.Flex;
        targetRoot.pickingMode = PickingMode.Position;
        activeOverlay = targetRoot;

        // Refresh tab controllers via their static instances
        if (CareerUI.Instance != null) CareerUI.Instance.RefreshView();
        if (HustlesUI.Instance != null) HustlesUI.Instance.RefreshView();
        if (EducationUI.Instance != null) EducationUI.Instance.RefreshView();
        if (ActivitiesUI.Instance != null) ActivitiesUI.Instance.RefreshView();

        RefreshDashboardUI(GameManager.Instance.currentData.playerName);
    }"""
    
new_switch_tab = """    private Coroutine activeModalCoroutine;

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
        // Needs a tiny delay for CSS transition to trigger from display:none to display:flex
        if (activeModalCoroutine != null) StopCoroutine(activeModalCoroutine);
        activeModalCoroutine = StartCoroutine(AnimateModalEntrance(targetRoot));

        // Refresh tab controllers via their static instances
        if (CareerUI.Instance != null) CareerUI.Instance.RefreshView();
        if (HustlesUI.Instance != null) HustlesUI.Instance.RefreshView();
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
    }"""
content = content.replace(old_switch_tab, new_switch_tab)

# 6. Update RefreshDashboardUI
old_refresh = """    public void RefreshDashboardUI(string pName)
    {
        var data = GameManager.Instance.currentData;
        var roots = new[] { dashboardRoot, careersRoot, hustlesRoot, educationRoot, activitiesRoot };
        
        float stressPct = data.stress * 100f;
        float averageJobLuck = (GameManager.Instance.GetEffectiveLuck("manual") + GameManager.Instance.GetEffectiveLuck("hospitality") + GameManager.Instance.GetEffectiveLuck("tech") + GameManager.Instance.GetEffectiveLuck("corp")) / 4.0f;
        float luckPct = averageJobLuck * 100f; 
        float perfPct = data.promotionProgress * 100f;
        Color stressColor = stressPct >= 80 ? Color.red : (stressPct >= 50 ? Color.yellow : Color.green);

        foreach (var root in roots)
        {
            if (root == null) continue;
            var pText = root.Q<Label>("PlayerNameText");
            if (pText != null) pText.text = pName;

            var cashText = root.Q<Label>("CashText");
            if (cashText != null) cashText.text = $"${data.cash:N2}";

            var expensesText = root.Q<Label>("ExpenseText");
            if (expensesText != null) expensesText.text = $"-${data.weeklyExpenses:N2}";
            
            var jText = root.Q<Label>("JobText");
            if (jText != null) jText.text = string.IsNullOrEmpty(data.currentJobId) ? "Unemployed" : GameManager.Instance.GetJobName(data.currentJobId);

            var dText = root.Q<Label>("DistrictText");
            if (dText != null) dText.text = data.currentDistrict;

            var sText = root.Q<Label>("SubTitleText");
            if (sText != null) sText.text = $"• Week {data.weekNumber}";
            
            var stressLabel = root.Q<Label>("StressVal");
            if (stressLabel != null) stressLabel.text = $"{stressPct:F0}%";
            
            var stressBar = root.Q<VisualElement>("StressFill");
            if (stressBar != null) {
                stressBar.style.width = Length.Percent(stressPct);
                stressBar.style.backgroundColor = stressColor;
            }

            var luckLabel = root.Q<Label>("LuckVal");
            if (luckLabel != null) luckLabel.text = $"{luckPct:F0}%";
            
            var luckBar = root.Q<VisualElement>("LuckFill");
            if (luckBar != null) luckBar.style.width = Length.Percent(luckPct);
            
            var perfLabel = root.Q<Label>("PerfVal");
            if (perfLabel != null) perfLabel.text = $"{perfPct:F0}%";
            
            var perfBar = root.Q<VisualElement>("PerfFill");
            if (perfBar != null) perfBar.style.width = Length.Percent(perfPct);
        }
    }"""
    
new_refresh = """    public void RefreshDashboardUI(string pName)
    {
        var data = GameManager.Instance.currentData;
        // Only dashboardRoot has the Top Card now.
        var roots = new[] { dashboardRoot };
        
        float stressPct = data.stress * 100f;
        float averageJobLuck = (GameManager.Instance.GetEffectiveLuck("manual") + GameManager.Instance.GetEffectiveLuck("hospitality") + GameManager.Instance.GetEffectiveLuck("tech") + GameManager.Instance.GetEffectiveLuck("corp")) / 4.0f;
        float luckPct = averageJobLuck * 100f; 
        float perfPct = data.promotionProgress * 100f;
        Color stressColor = stressPct >= 80 ? Color.red : (stressPct >= 50 ? Color.yellow : Color.green);

        foreach (var root in roots)
        {
            if (root == null) continue;
            var pText = root.Q<Label>("PlayerNameText");
            if (pText != null) pText.text = pName;

            var cashText = root.Q<Label>("CashText");
            if (cashText != null) cashText.text = $"${data.cash:N2}";

            var expensesText = root.Q<Label>("ExpenseText");
            if (expensesText != null) expensesText.text = $"-${data.weeklyExpenses:N2}";
            
            var jText = root.Q<Label>("JobText");
            if (jText != null) jText.text = string.IsNullOrEmpty(data.currentJobId) ? "Unemployed" : GameManager.Instance.GetJobName(data.currentJobId);

            var dText = root.Q<Label>("DistrictText");
            if (dText != null) dText.text = data.currentDistrict;

            var sText = root.Q<Label>("SubTitleText");
            if (sText != null) sText.text = $"• Week {data.weekNumber}";
            
            var stressLabel = root.Q<Label>("StressVal");
            if (stressLabel != null) stressLabel.text = $"{stressPct:F0}%";
            
            var stressBar = root.Q<VisualElement>("StressFill");
            if (stressBar != null) {
                stressBar.style.width = Length.Percent(stressPct);
                stressBar.style.backgroundColor = stressColor;
            }

            var luckLabel = root.Q<Label>("LuckVal");
            if (luckLabel != null) luckLabel.text = $"{luckPct:F0}%";
            
            var luckBar = root.Q<VisualElement>("LuckFill");
            if (luckBar != null) luckBar.style.width = Length.Percent(luckPct);
            
            var perfLabel = root.Q<Label>("PerfVal");
            if (perfLabel != null) perfLabel.text = $"{perfPct:F0}%";
            
            var perfBar = root.Q<VisualElement>("PerfFill");
            if (perfBar != null) perfBar.style.width = Length.Percent(perfPct);
        }
    }"""
content = content.replace(old_refresh, new_refresh)

with open('/Users/josh/.gemini/antigravity/brain/df5a1394-1942-41b4-9af1-cd17e7a8c488/scratch/GameUIManager.cs', 'w') as f:
    f.write(content)

print("GameUIManager.cs refactored.")

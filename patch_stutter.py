import re

with open('Assets/Scripts/2 Managers/GameUIManager.cs', 'r') as f:
    content = f.read()

# Replace the heavy refresh logic in AnimateOpenMenu
old_refresh = '''        if (CareerUI.Instance != null) CareerUI.Instance.RefreshView();
        if (HustlesUI.Instance != null) HustlesUI.Instance.PopulateHustlesBoard();
        if (EducationUI.Instance != null) EducationUI.Instance.RefreshView();
        RefreshDashboardUI(GameManager.Instance.currentData.playerName);'''

new_refresh = '''        // Only refresh the tab we are actually opening to prevent stutter
        if (targetRoot == careersRoot && CareerUI.Instance != null) CareerUI.Instance.RefreshView();
        else if (targetRoot == hustlesRoot && HustlesUI.Instance != null) HustlesUI.Instance.PopulateHustlesBoard();
        else if (targetRoot == educationRoot && EducationUI.Instance != null) EducationUI.Instance.RefreshView();
        else if (targetRoot == dashboardRoot) RefreshDashboardUI(GameManager.Instance.currentData.playerName);
        // ActivitiesUI manages its own Population in Initialize, but we can refresh here if needed later'''

content = content.replace(old_refresh, new_refresh)

with open('Assets/Scripts/2 Managers/GameUIManager.cs', 'w') as f:
    f.write(content)

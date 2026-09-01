import re

with open("Assets/Scripts/2 Managers/GameUIManager.cs", "r") as f:
    content = f.read()

# Add PlayerPrefs HUDStyle loading and toggling.
# In OpenSettings, btnToggleHUD needs binding.
# Actually, GameUIManager's Start() binds Settings.
# Let's find: `btnWipeHistory = dashboardRoot.Q<Button>("BtnWipeHistory");`
bind_str = 'btnWipeHistory = dashboardRoot.Q<Button>("BtnWipeHistory");'
bind_new = bind_str + '\n        var btnToggleHUD = dashboardRoot.Q<Button>("BtnToggleHUD");\n        if (btnToggleHUD != null) { btnToggleHUD.clicked -= OnToggleHUD; btnToggleHUD.clicked += OnToggleHUD; }'
content = content.replace(bind_str, bind_new)

hud_funcs = """
    private void OnToggleHUD()
    {
        int style = UnityEngine.PlayerPrefs.GetInt("HUDStyle", 0);
        style = (style + 1) % 4;
        UnityEngine.PlayerPrefs.SetInt("HUDStyle", style);
        UnityEngine.PlayerPrefs.Save();
        
        if (GameManager.Instance != null && GameManager.Instance.currentData != null)
        {
            RefreshDashboardUI(GameManager.Instance.currentData.playerName);
        }
        
        var btnToggleHUD = dashboardRoot.Q<Button>("BtnToggleHUD");
        if (btnToggleHUD != null)
        {
            string[] names = new[] { "ID Card", "Smartphone", "Ledger", "Digital Ticker" };
            btnToggleHUD.text = $"HUD Style: {names[style]}";
        }
    }
"""

content = content.replace("private void OpenSettings()", hud_funcs + "\n    private void OpenSettings()")

# Add extra label updates inside RefreshDashboardUI
extra_labels = """
            root.Query<Label>("PlayerInitials").ForEach(l => l.text = !string.IsNullOrEmpty(playerName) ? playerName.Substring(0, 1).ToUpper() : "X");
            root.Query<Label>("IDCardWeek").ForEach(l => l.text = $"VALID: WK {data.currentWeek}");
            root.Query<Label>("PhoneTime").ForEach(l => l.text = $"WEEK {data.currentWeek}");
            root.Query<Label>("DistrictSignal").ForEach(l => l.text = $"📶 {data.zone.ToString().ToUpper()} - 3G");
            root.Query<Label>("LedgerDate").ForEach(l => l.text = $"DATE: WEEK {data.currentWeek}");
            root.Query<Label>("TickerWeek").ForEach(l => l.text = $"WK:{data.currentWeek:D2}");

            // Apply active HUD
            int hudStyle = UnityEngine.PlayerPrefs.GetInt("HUDStyle", 0);
            var idCard = root.Q<VisualElement>("HUD-IDCard");
            var phone = root.Q<VisualElement>("HUD-Phone");
            var ledger = root.Q<VisualElement>("HUD-Ledger");
            var ticker = root.Q<VisualElement>("HUD-Ticker");
            
            if (idCard != null) idCard.style.display = (hudStyle == 0) ? DisplayStyle.Flex : DisplayStyle.None;
            if (phone != null) phone.style.display = (hudStyle == 1) ? DisplayStyle.Flex : DisplayStyle.None;
            if (ledger != null) ledger.style.display = (hudStyle == 2) ? DisplayStyle.Flex : DisplayStyle.None;
            if (ticker != null) ticker.style.display = (hudStyle == 3) ? DisplayStyle.Flex : DisplayStyle.None;
"""

# Insert right after `root.Query<Label>("DistrictText").ForEach(l => l.text = data.zone.ToString());`
hook = 'root.Query<Label>("DistrictText").ForEach(l => l.text = data.zone.ToString());'
content = content.replace(hook, hook + extra_labels)

with open("Assets/Scripts/2 Managers/GameUIManager.cs", "w") as f:
    f.write(content)


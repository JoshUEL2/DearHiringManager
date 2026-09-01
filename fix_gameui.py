import re

with open("Assets/Scripts/2 Managers/GameUIManager.cs", "r") as f:
    content = f.read()

# 1. Remove BtnToggleHUD bindings
content = content.replace('var btnToggleHUD = dashboardRoot.Q<Button>("BtnToggleHUD");\n        if (btnToggleHUD != null) { btnToggleHUD.clicked -= OnToggleHUD; btnToggleHUD.clicked += OnToggleHUD; }', '')

# 2. Remove OnToggleHUD method
pattern_method = re.compile(r'    private void OnToggleHUD\(\)\s*\{.*?    \}\s*(?=    private void OpenSettings\(\))', re.DOTALL)
content = pattern_method.sub('', content)

# 3. Remove HUD swapping logic in RefreshDashboardUI
hud_swap_logic = r"""            // Apply active HUD
            int hudStyle = UnityEngine\.PlayerPrefs\.GetInt\("HUDStyle", 0\);
            var idCard = root\.Q<VisualElement>\("HUD-IDCard"\);
            var phone = root\.Q<VisualElement>\("HUD-Phone"\);
            var ledger = root\.Q<VisualElement>\("HUD-Ledger"\);
            var ticker = root\.Q<VisualElement>\("HUD-Ticker"\);
            
            if \(idCard != null\) idCard\.style\.display = \(hudStyle == 0\) \? DisplayStyle\.Flex : DisplayStyle\.None;
            if \(phone != null\) phone\.style\.display = \(hudStyle == 1\) \? DisplayStyle\.Flex : DisplayStyle\.None;
            if \(ledger != null\) ledger\.style\.display = \(hudStyle == 2\) \? DisplayStyle\.Flex : DisplayStyle\.None;
            if \(ticker != null\) ticker\.style\.display = \(hudStyle == 3\) \? DisplayStyle\.Flex : DisplayStyle\.None;"""
content = re.sub(hud_swap_logic, '', content)

with open("Assets/Scripts/2 Managers/GameUIManager.cs", "w") as f:
    f.write(content)

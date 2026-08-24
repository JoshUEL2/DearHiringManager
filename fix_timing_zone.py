import re

path = 'Assets/Scripts/2 Managers/MiniGames/TimingRingMiniGame.cs'

with open(path, 'r') as f:
    content = f.read()

# 1. Update zone sizes
target_zone = r'float currentZoneDegrees = Mathf\.Max\(10f, 45f - \(timingCurrentRound \* 5f\)\);'
replacement_zone = r'float currentZoneDegrees = Mathf.Max(30f, 75f - (timingCurrentRound * 6f));'

content = re.sub(target_zone, replacement_zone, content)

# 2. Update sign text alignment
target_text = r'''        var signText = new Label\("SALE ➡️"\);
        signText\.style\.fontSize = 65;
        signText\.style\.color = new StyleColor\(new Color\(0\.9f, 0\.15f, 0\.15f\)\); // Bold Red
        signText\.style\.unityFontStyleAndWeight = FontStyle\.Bold;
        signText\.style\.unityTextAlign = TextAnchor\.MiddleCenter;
        signBody\.Add\(signText\);'''

replacement_text = r'''        var signText = new Label("SALE ➡️");
        signText.style.fontSize = 65;
        signText.style.color = new StyleColor(new Color(0.9f, 0.15f, 0.15f)); // Bold Red
        signText.style.unityFontStyleAndWeight = FontStyle.Bold;
        signText.style.flexGrow = 1;
        signText.style.unityTextAlign = TextAnchor.MiddleRight;
        signText.style.paddingRight = 20;
        signBody.Add(signText);'''

content = re.sub(target_text, replacement_text, content)

with open(path, 'w') as f:
    f.write(content)


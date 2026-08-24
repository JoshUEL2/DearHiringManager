import re

path = 'Assets/Scripts/2 Managers/MiniGames/TimingRingMiniGame.cs'

with open(path, 'r') as f:
    content = f.read()

# 1. Restore the text alignment to MiddleCenter
target_text = r'''        var signText = new Label\("SALE ➡️"\);
        signText\.style\.fontSize = 65;
        signText\.style\.color = new StyleColor\(new Color\(0\.9f, 0\.15f, 0\.15f\)\); // Bold Red
        signText\.style\.unityFontStyleAndWeight = FontStyle\.Bold;
        signText\.style\.flexGrow = 1;
        signText\.style\.unityTextAlign = TextAnchor\.MiddleRight;
        signText\.style\.paddingRight = 20;
        signBody\.Add\(signText\);'''

replacement_text = r'''        var signText = new Label("SALE ➡️");
        signText.style.fontSize = 65;
        signText.style.color = new StyleColor(new Color(0.9f, 0.15f, 0.15f)); // Bold Red
        signText.style.unityFontStyleAndWeight = FontStyle.Bold;
        signText.style.unityTextAlign = TextAnchor.MiddleCenter;
        signBody.Add(signText);'''

content = re.sub(target_text, replacement_text, content)

# 2. Fix the 90 degree offset in hit detection
target_calc = r'float diff = Mathf\.Abs\(Mathf\.DeltaAngle\(timingMarkerAngle, timingTargetAngle\)\);'
replacement_calc = r'float diff = Mathf.Abs(Mathf.DeltaAngle(timingMarkerAngle + 90f, timingTargetAngle));'

content = re.sub(target_calc, replacement_calc, content)

with open(path, 'w') as f:
    f.write(content)


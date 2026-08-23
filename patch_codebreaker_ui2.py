import re

path = 'Assets/Scripts/2 Managers/MiniGames/CodeBreakerMiniGame.cs'
with open(path, 'r') as f:
    content = f.read()

target = r'''    private void StyleTerminalButton\(Button btn, bool isGuessSlot\)
    \{
        btn\.style\.width = isGuessSlot \? 70 : 60;
        btn\.style\.height = isGuessSlot \? 70 : 60;
        btn\.style\.fontSize = isGuessSlot \? 40 : 30;
        
        btn\.style\.marginTop = 5; btn\.style\.marginBottom = 5; btn\.style\.marginLeft = 5; btn\.style\.marginRight = 5;
        btn\.style\.backgroundColor = new StyleColor\(new Color\(0\.05f, 0\.1f, 0\.05f\)\);
        btn\.style\.color = new StyleColor\(new Color\(0\.2f, 0\.9f, 0\.2f\)\); // Neon green text
        btn\.style\.borderTopWidth = 1; btn\.style\.borderLeftWidth = 1; btn\.style\.borderBottomWidth = 1; btn\.style\.borderRightWidth = 1;
        btn\.style\.borderTopColor = new StyleColor\(new Color\(0\.1f, 0\.5f, 0\.1f\)\);
        btn\.style\.borderBottomColor = new StyleColor\(new Color\(0\.1f, 0\.5f, 0\.1f\)\);
        btn\.style\.borderLeftColor = new StyleColor\(new Color\(0\.1f, 0\.5f, 0\.1f\)\);
        btn\.style\.borderRightColor = new StyleColor\(new Color\(0\.1f, 0\.5f, 0\.1f\)\);
    \}
    
    private void StyleActionButtons\(\)
    \{
        btnCodeSubmit\.style\.backgroundColor = new StyleColor\(new Color\(0\.1f, 0\.6f, 0\.1f\)\);
        btnCodeSubmit\.style\.color = new StyleColor\(Color\.black\);
        btnCodeSubmit\.style\.unityFontStyleAndWeight = FontStyle\.Bold;
        
        btnCodeClear\.style\.backgroundColor = new StyleColor\(new Color\(0\.6f, 0\.1f, 0\.1f\)\);
        btnCodeClear\.style\.color = new StyleColor\(Color\.white\);
        btnCodeClear\.style\.unityFontStyleAndWeight = FontStyle\.Bold;
    \}'''

replacement = r'''    private void StyleTerminalButton(Button btn, bool isGuessSlot)
    {
        btn.style.width = isGuessSlot ? 70 : 60;
        btn.style.height = isGuessSlot ? 70 : 60;
        btn.style.fontSize = isGuessSlot ? 40 : 30;
        
        btn.style.marginTop = 5; btn.style.marginBottom = 5; btn.style.marginLeft = 5; btn.style.marginRight = 5;
        
        // Clean Modern Wordle Style
        btn.style.backgroundColor = new StyleColor(new Color(0.12f, 0.16f, 0.23f)); // Slate Blue
        btn.style.color = new StyleColor(Color.white);
        btn.style.unityFontStyleAndWeight = FontStyle.Bold;
        
        btn.style.borderTopLeftRadius = 8;
        btn.style.borderTopRightRadius = 8;
        btn.style.borderBottomLeftRadius = 8;
        btn.style.borderBottomRightRadius = 8;
        
        btn.style.borderTopWidth = 2; btn.style.borderLeftWidth = 2; btn.style.borderBottomWidth = 2; btn.style.borderRightWidth = 2;
        StyleColor borderColor = new StyleColor(new Color(0.2f, 0.25f, 0.34f)); // Light Slate border
        btn.style.borderTopColor = borderColor;
        btn.style.borderBottomColor = borderColor;
        btn.style.borderLeftColor = borderColor;
        btn.style.borderRightColor = borderColor;
    }
    
    private void StyleActionButtons()
    {
        // Restore to default UXML styling by clearing inline styles
        btnCodeSubmit.style.backgroundColor = new StyleColor(StyleKeyword.Null);
        btnCodeSubmit.style.color = new StyleColor(StyleKeyword.Null);
        
        btnCodeClear.style.backgroundColor = new StyleColor(StyleKeyword.Null);
        btnCodeClear.style.color = new StyleColor(StyleKeyword.Null);
    }'''

content = re.sub(target, replacement, content)

with open(path, 'w') as f:
    f.write(content)

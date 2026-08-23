import re

path = 'Assets/Scripts/2 Managers/MiniGames/PumpSystemMiniGame.cs'
with open(path, 'r') as f:
    content = f.read()

target = r'''    private IEnumerator ShowWinFeedbackAndClose\(\)
    \{
        // Flash entire container green and wait
        StyleColor originalColor = container\.style\.backgroundColor;
        container\.style\.backgroundColor = new StyleColor\(new Color\(0\.18f, 0\.8f, 0\.44f, 0\.5f\)\);
        
        var successLabel = new Label\("PERFECT TEMP!"\);
        successLabel\.style\.position = Position\.Absolute;
        successLabel\.style\.top = Length\.Percent\(50\);
        successLabel\.style\.width = Length\.Percent\(100\);
        successLabel\.style\.unityTextAlign = TextAnchor\.MiddleCenter;
        successLabel\.style\.fontSize = 48;
        successLabel\.style\.color = Color\.white;
        successLabel\.style\.unityFontStyleAndWeight = FontStyle\.Bold;
        container\.Add\(successLabel\);

        yield return new WaitForSeconds\(0\.6f\);
        
        container\.style\.backgroundColor = originalColor;
        container\.Remove\(successLabel\);
        manager\.MarkWinReady\(\);
    \}'''

replacement = r'''    private IEnumerator ShowWinFeedbackAndClose()
    {
        Label stampText = new Label("PERFECT TEMP!");
        stampText.style.position = Position.Absolute;
        stampText.style.color = new StyleColor(new Color(0.18f, 0.8f, 0.44f)); // Green
        stampText.style.fontSize = 72;
        stampText.style.unityFontStyleAndWeight = FontStyle.Bold;
        stampText.style.unityTextAlign = TextAnchor.MiddleCenter;
        
        VisualElement overlay = new VisualElement();
        overlay.style.position = Position.Absolute;
        overlay.style.width = new Length(100, LengthUnit.Percent);
        overlay.style.height = new Length(100, LengthUnit.Percent);
        overlay.style.backgroundColor = new StyleColor(new Color(0f, 0f, 0f, 0.6f)); // Darken background
        
        stampText.style.top = new Length(50, LengthUnit.Percent);
        stampText.style.left = new Length(50, LengthUnit.Percent);
        stampText.style.translate = new Translate(new Length(-50, LengthUnit.Percent), new Length(-50, LengthUnit.Percent));
        
        overlay.Add(stampText);
        container.Add(overlay);
        
        float life = 0.8f;
        float elapsed = 0f;
        while(elapsed < life)
        {
            elapsed += Time.deltaTime;
            float p = elapsed / life;
            
            float scale = Mathf.Lerp(2.5f, 1f, p * 10f);
            if (scale < 1f) scale = 1f;
            stampText.style.scale = new StyleScale(new Vector2(scale, scale));
            
            if (p > 0.8f) {
                overlay.style.opacity = 1f - ((p - 0.8f) * 5f);
            }
            
            yield return null;
        }
        
        if (container.Contains(overlay)) container.Remove(overlay);
        manager.MarkWinReady();
    }'''
content = re.sub(target, replacement, content)

with open(path, 'w') as f:
    f.write(content)

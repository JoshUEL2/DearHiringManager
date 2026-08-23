import re

path = 'Assets/Scripts/2 Managers/MiniGames/DialogueChainMiniGame.cs'
with open(path, 'r') as f:
    content = f.read()

target = r'''    private IEnumerator AnimateFloatingText\(Button parentBtn, string text, Color c\)
    \{
        Label floatingText = new Label\(text\);
        floatingText\.style\.position = Position\.Absolute;
        floatingText\.style\.color = new StyleColor\(c\);
        floatingText\.style\.fontSize = 24;
        floatingText\.style\.unityFontStyleAndWeight = FontStyle\.Bold;
        floatingText\.style\.top = -10;
        floatingText\.style\.alignSelf = Align\.Center;
        
        parentBtn\.Add\(floatingText\);
        
        float life = 0\.5f;
        float elapsed = 0f;
        while\(elapsed < life && isPlaying\)
        \{
            elapsed \+= Time\.deltaTime;
            float p = elapsed / life;
            
            floatingText\.style\.top = -10f - \(p \* 30f\);
            floatingText\.style\.opacity = 1f - p;
            yield return null;
        \}
        
        if \(parentBtn\.Contains\(floatingText\)\) parentBtn\.Remove\(floatingText\);
    \}'''

replacement = r'''    private IEnumerator AnimateFloatingText(Button parentBtn, string text, Color c)
    {
        Label floatingText = new Label(text);
        floatingText.style.position = Position.Absolute;
        floatingText.style.color = new StyleColor(c);
        floatingText.style.fontSize = 24;
        floatingText.style.unityFontStyleAndWeight = FontStyle.Bold;
        
        // Add to the main container instead of the button so we don't break the button's internal flex layout!
        container.Add(floatingText);
        
        // Position it over the button
        float startY = parentBtn.worldBound.y - container.worldBound.y;
        float startX = parentBtn.worldBound.x - container.worldBound.x + (parentBtn.worldBound.width / 2f) - 50f;
        
        floatingText.style.top = startY;
        floatingText.style.left = startX;
        
        float life = 0.5f;
        float elapsed = 0f;
        while(elapsed < life && isPlaying)
        {
            elapsed += Time.deltaTime;
            float p = elapsed / life;
            
            floatingText.style.top = startY - (p * 30f);
            floatingText.style.opacity = 1f - p;
            yield return null;
        }
        
        if (container.Contains(floatingText)) container.Remove(floatingText);
    }'''

content = re.sub(target, replacement, content)

with open(path, 'w') as f:
    f.write(content)

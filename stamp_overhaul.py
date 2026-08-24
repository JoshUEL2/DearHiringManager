import re

files = [
    'Assets/Scripts/2 Managers/MiniGames/DialogueChainMiniGame.cs',
    'Assets/Scripts/2 Managers/MiniGames/PumpSystemMiniGame.cs',
    'Assets/Scripts/2 Managers/MiniGames/MashRaceMiniGame.cs'
]

for path in files:
    with open(path, 'r') as f:
        content = f.read()

    # We need to replace the entire stamp visual creation block
    # For DialogueChainMiniGame:
    if 'ignoredText' in content:
        target1 = r'''        // Text Outline for legibility
        ignoredText\.style\.unityTextOutlineColor = Color\.black;
        ignoredText\.style\.unityTextOutlineWidth = 4f;
        
        // Center text in container
        ignoredText\.style\.top = new Length\(50, LengthUnit\.Percent\);
        ignoredText\.style\.left = new Length\(50, LengthUnit\.Percent\);
        ignoredText\.style\.translate = new Translate\(new Length\(-50, LengthUnit\.Percent\), new Length\(-50, LengthUnit\.Percent\)\);
        
        manager\.miniGameDocument\.rootVisualElement\.Add\(ignoredText\);
        
        // Pop in animation
        float life = 0\.8f;
        float elapsed = 0f;
        while\(elapsed < life && isPlaying\)
        \{
            elapsed \+= Time\.deltaTime;
            float p = elapsed / life;
            
            // Starts big, slams down to size 1
            float scale = Mathf\.Lerp\(2\.5f, 1f, p \* 10f\); // Fast snap
            if \(scale < 1f\) scale = 1f;
            ignoredText\.style\.scale = new StyleScale\(new Vector2\(scale, scale\)\);
            
            // Fade out at the very end
            if \(p > 0\.8f\) \{
                ignoredText\.style\.opacity = 1f - \(\(p - 0\.8f\) \* 5f\);
            \}
            
            yield return null;
        \}
        
        if \(manager\.miniGameDocument\.rootVisualElement\.Contains\(ignoredText\)\) manager\.miniGameDocument\.rootVisualElement\.Remove\(ignoredText\);'''

        replacement1 = r'''        ignoredText.style.fontSize = 110;
        ignoredText.style.letterSpacing = 10;
        ignoredText.style.rotate = new StyleRotate(new Rotate(new Angle(-12f, AngleUnit.Degree)));
        ignoredText.style.textShadow = new StyleTextShadow(new TextShadow { color = new Color(0,0,0,0.8f), offset = new Vector2(12f, 15f), blurRadius = 15f });
        
        VisualElement backgroundFade = new VisualElement();
        backgroundFade.style.position = Position.Absolute;
        backgroundFade.style.width = new Length(100, LengthUnit.Percent);
        backgroundFade.style.height = new Length(100, LengthUnit.Percent);
        backgroundFade.style.backgroundColor = new StyleColor(new Color(0f, 0f, 0f, 0.7f));
        backgroundFade.style.opacity = 0f;
        
        ignoredText.style.top = new Length(50, LengthUnit.Percent);
        ignoredText.style.left = new Length(50, LengthUnit.Percent);
        ignoredText.style.translate = new Translate(new Length(-50, LengthUnit.Percent), new Length(-50, LengthUnit.Percent));
        
        manager.miniGameDocument.rootVisualElement.Add(backgroundFade);
        manager.miniGameDocument.rootVisualElement.Add(ignoredText);
        
        float life = 0.8f;
        float elapsed = 0f;
        while(elapsed < life && isPlaying)
        {
            elapsed += Time.deltaTime;
            float p = elapsed / life;
            
            float scale = Mathf.Lerp(3.5f, 1f, p * 12f); 
            if (scale < 1f) scale = 1f;
            ignoredText.style.scale = new StyleScale(new Vector2(scale, scale));
            
            backgroundFade.style.opacity = Mathf.Lerp(0f, 1f, p * 8f);
            
            if (p > 0.8f) {
                float fadeOut = 1f - ((p - 0.8f) * 5f);
                ignoredText.style.opacity = fadeOut;
                backgroundFade.style.opacity = fadeOut;
            }
            
            yield return null;
        }
        
        if (manager.miniGameDocument.rootVisualElement.Contains(ignoredText)) manager.miniGameDocument.rootVisualElement.Remove(ignoredText);
        if (manager.miniGameDocument.rootVisualElement.Contains(backgroundFade)) manager.miniGameDocument.rootVisualElement.Remove(backgroundFade);'''
        
        content = re.sub(target1, replacement1, content)
        # Also remove stroke in case it was there before:
        content = re.sub(r'ignoredText\.style\.unityTextOutlineColor = [^\n]+\n', '', content)
        content = re.sub(r'ignoredText\.style\.unityTextOutlineWidth = [^\n]+\n', '', content)


    if 'stampText' in content:
        target2 = r'''        stampText\.style\.unityTextOutlineColor = Color\.black;
        stampText\.style\.unityTextOutlineWidth = 4f;
        
        stampText\.style\.top = new Length\(50, LengthUnit\.Percent\);
        stampText\.style\.left = new Length\(50, LengthUnit\.Percent\);
        stampText\.style\.translate = new Translate\(new Length\(-50, LengthUnit\.Percent\), new Length\(-50, LengthUnit\.Percent\)\);
        
        manager\.miniGameDocument\.rootVisualElement\.Add\(stampText\);
        
        float life = 0\.8f;
        float elapsed = 0f;
        while\(elapsed < life\)
        \{
            elapsed \+= Time\.deltaTime;
            float p = elapsed / life;
            
            float scale = Mathf\.Lerp\(2\.5f, 1f, p \* 10f\);
            if \(scale < 1f\) scale = 1f;
            stampText\.style\.scale = new StyleScale\(new Vector2\(scale, scale\)\);
            
            if \(p > 0\.8f\) \{
                stampText\.style\.opacity = 1f - \(\(p - 0\.8f\) \* 5f\);
            \}
            
            yield return null;
        \}
        
        if \(manager\.miniGameDocument\.rootVisualElement\.Contains\(stampText\)\) manager\.miniGameDocument\.rootVisualElement\.Remove\(stampText\);'''

        replacement2 = r'''        stampText.style.fontSize = 110;
        stampText.style.letterSpacing = 10;
        stampText.style.rotate = new StyleRotate(new Rotate(new Angle(-12f, AngleUnit.Degree)));
        stampText.style.textShadow = new StyleTextShadow(new TextShadow { color = new Color(0,0,0,0.8f), offset = new Vector2(12f, 15f), blurRadius = 15f });
        
        VisualElement backgroundFade = new VisualElement();
        backgroundFade.style.position = Position.Absolute;
        backgroundFade.style.width = new Length(100, LengthUnit.Percent);
        backgroundFade.style.height = new Length(100, LengthUnit.Percent);
        backgroundFade.style.backgroundColor = new StyleColor(new Color(0f, 0f, 0f, 0.7f));
        backgroundFade.style.opacity = 0f;
        
        stampText.style.top = new Length(50, LengthUnit.Percent);
        stampText.style.left = new Length(50, LengthUnit.Percent);
        stampText.style.translate = new Translate(new Length(-50, LengthUnit.Percent), new Length(-50, LengthUnit.Percent));
        
        manager.miniGameDocument.rootVisualElement.Add(backgroundFade);
        manager.miniGameDocument.rootVisualElement.Add(stampText);
        
        float life = 0.8f;
        float elapsed = 0f;
        while(elapsed < life)
        {
            elapsed += Time.deltaTime;
            float p = elapsed / life;
            
            float scale = Mathf.Lerp(3.5f, 1f, p * 12f);
            if (scale < 1f) scale = 1f;
            stampText.style.scale = new StyleScale(new Vector2(scale, scale));
            
            backgroundFade.style.opacity = Mathf.Lerp(0f, 1f, p * 8f);
            
            if (p > 0.8f) {
                float fadeOut = 1f - ((p - 0.8f) * 5f);
                stampText.style.opacity = fadeOut;
                backgroundFade.style.opacity = fadeOut;
            }
            
            yield return null;
        }
        
        if (manager.miniGameDocument.rootVisualElement.Contains(stampText)) manager.miniGameDocument.rootVisualElement.Remove(stampText);
        if (manager.miniGameDocument.rootVisualElement.Contains(backgroundFade)) manager.miniGameDocument.rootVisualElement.Remove(backgroundFade);'''

        content = re.sub(target2, replacement2, content)
        content = re.sub(r'stampText\.style\.unityTextOutlineColor = [^\n]+\n', '', content)
        content = re.sub(r'stampText\.style\.unityTextOutlineWidth = [^\n]+\n', '', content)

    with open(path, 'w') as f:
        f.write(content)


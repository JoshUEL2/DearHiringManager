import re

files = [
    'Assets/Scripts/2 Managers/MiniGames/DialogueChainMiniGame.cs',
    'Assets/Scripts/2 Managers/MiniGames/PumpSystemMiniGame.cs',
    'Assets/Scripts/2 Managers/MiniGames/MashRaceMiniGame.cs'
]

for path in files:
    with open(path, 'r') as f:
        content = f.read()

    # DialogueChainMiniGame.cs uses ignoredText, others use stampText
    # Let's standardize by just replacing the overlay logic for both
    
    # 1. DialogueChain
    if 'ignoredText' in content:
        target1 = r'''        // Darken background
        VisualElement overlay = new VisualElement\(\);
        overlay\.style\.position = Position\.Absolute;
        overlay\.style\.width = new Length\(100, LengthUnit\.Percent\);
        overlay\.style\.height = new Length\(100, LengthUnit\.Percent\);
        overlay\.style\.backgroundColor = new StyleColor\(new Color\(0f, 0f, 0f, 0\.5f\)\);
        
        // Center text in container
        ignoredText\.style\.top = new Length\(50, LengthUnit\.Percent\);
        ignoredText\.style\.left = new Length\(50, LengthUnit\.Percent\);
        ignoredText\.style\.translate = new Translate\(new Length\(-50, LengthUnit\.Percent\), new Length\(-50, LengthUnit\.Percent\)\);
        
        overlay\.Add\(ignoredText\);
        rootContainer\.Add\(overlay\);'''
        
        replacement1 = r'''        // Text Outline for legibility
        ignoredText.style.unityTextOutlineColor = Color.black;
        ignoredText.style.unityTextOutlineWidth = 4f;
        
        // Center text in container
        ignoredText.style.top = new Length(50, LengthUnit.Percent);
        ignoredText.style.left = new Length(50, LengthUnit.Percent);
        ignoredText.style.translate = new Translate(new Length(-50, LengthUnit.Percent), new Length(-50, LengthUnit.Percent));
        
        rootContainer.Add(ignoredText);'''
        
        content = re.sub(target1, replacement1, content)
        
        content = re.sub(r'overlay\.style\.opacity = 1f - \(\(p - 0\.8f\) \* 5f\);', 'ignoredText.style.opacity = 1f - ((p - 0.8f) * 5f);', content)
        content = re.sub(r'if \(rootContainer\.Contains\(overlay\)\) rootContainer\.Remove\(overlay\);', 'if (rootContainer.Contains(ignoredText)) rootContainer.Remove(ignoredText);', content)

    # 2. MashRace & PumpSystem
    if 'stampText' in content:
        target2 = r'''        VisualElement overlay = new VisualElement\(\);
        overlay\.style\.position = Position\.Absolute;
        overlay\.style\.width = new Length\(100, LengthUnit\.Percent\);
        overlay\.style\.height = new Length\(100, LengthUnit\.Percent\);
        overlay\.style\.backgroundColor = new StyleColor\(new Color\(0f, 0f, 0f, 0\.6f\)\); // Darken background
        
        stampText\.style\.top = new Length\(50, LengthUnit\.Percent\);
        stampText\.style\.left = new Length\(50, LengthUnit\.Percent\);
        stampText\.style\.translate = new Translate\(new Length\(-50, LengthUnit\.Percent\), new Length\(-50, LengthUnit\.Percent\)\);
        
        overlay\.Add\(stampText\);
        rootContainer\.Add\(overlay\);'''
        
        replacement2 = r'''        stampText.style.unityTextOutlineColor = Color.black;
        stampText.style.unityTextOutlineWidth = 4f;
        
        stampText.style.top = new Length(50, LengthUnit.Percent);
        stampText.style.left = new Length(50, LengthUnit.Percent);
        stampText.style.translate = new Translate(new Length(-50, LengthUnit.Percent), new Length(-50, LengthUnit.Percent));
        
        rootContainer.Add(stampText);'''
        
        target2_b = r'''        VisualElement overlay = new VisualElement\(\);
        overlay\.style\.position = Position\.Absolute;
        overlay\.style\.width = new Length\(100, LengthUnit\.Percent\);
        overlay\.style\.height = new Length\(100, LengthUnit\.Percent\);
        overlay\.style\.backgroundColor = new StyleColor\(new Color\(0f, 0f, 0f, 0\.6f\)\);
        
        stampText\.style\.top = new Length\(50, LengthUnit\.Percent\);
        stampText\.style\.left = new Length\(50, LengthUnit\.Percent\);
        stampText\.style\.translate = new Translate\(new Length\(-50, LengthUnit\.Percent\), new Length\(-50, LengthUnit\.Percent\)\);
        
        overlay\.Add\(stampText\);
        rootContainer\.Add\(overlay\);'''
        
        content = re.sub(target2, replacement2, content)
        content = re.sub(target2_b, replacement2, content)
        
        content = re.sub(r'overlay\.style\.opacity = 1f - \(\(p - 0\.8f\) \* 5f\);', 'stampText.style.opacity = 1f - ((p - 0.8f) * 5f);', content)
        content = re.sub(r'if \(rootContainer\.Contains\(overlay\)\) rootContainer\.Remove\(overlay\);', 'if (rootContainer.Contains(stampText)) rootContainer.Remove(stampText);', content)

    with open(path, 'w') as f:
        f.write(content)


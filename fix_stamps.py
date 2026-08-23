import re

files = [
    'Assets/Scripts/2 Managers/MiniGames/DialogueChainMiniGame.cs',
    'Assets/Scripts/2 Managers/MiniGames/PumpSystemMiniGame.cs',
    'Assets/Scripts/2 Managers/MiniGames/MashRaceMiniGame.cs'
]

for path in files:
    with open(path, 'r') as f:
        content = f.read()
        
    # Step 1: Add private VisualElement rootContainer;
    if 'private VisualElement rootContainer;' not in content:
        content = re.sub(r'private VisualElement container;', 'private VisualElement container;\n    private VisualElement rootContainer;', content)
        
    # Step 2: In Initialize, set rootContainer = root;
    if 'rootContainer = root;' not in content:
        content = re.sub(r'container = root\.Q<VisualElement>', 'rootContainer = root;\n        container = root.Q<VisualElement>', content)
        
    # Step 3: Change container.Add(overlay) to rootContainer.Add(overlay)
    content = re.sub(r'container\.Add\(overlay\);', 'rootContainer.Add(overlay);', content)
    content = re.sub(r'if \(container\.Contains\(overlay\)\) container\.Remove\(overlay\);', 'if (rootContainer.Contains(overlay)) rootContainer.Remove(overlay);', content)
    
    with open(path, 'w') as f:
        f.write(content)
        

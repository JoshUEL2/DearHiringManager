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
    
    if 'ignoredText' in content:
        content = re.sub(r'rootContainer\.Add\(ignoredText\);', 'manager.miniGameDocument.rootVisualElement.Add(ignoredText);', content)
        content = re.sub(r'if \(rootContainer\.Contains\(ignoredText\)\) rootContainer\.Remove\(ignoredText\);', 'if (manager.miniGameDocument.rootVisualElement.Contains(ignoredText)) manager.miniGameDocument.rootVisualElement.Remove(ignoredText);', content)

    if 'stampText' in content:
        content = re.sub(r'rootContainer\.Add\(stampText\);', 'manager.miniGameDocument.rootVisualElement.Add(stampText);', content)
        content = re.sub(r'if \(rootContainer\.Contains\(stampText\)\) rootContainer\.Remove\(stampText\);', 'if (manager.miniGameDocument.rootVisualElement.Contains(stampText)) manager.miniGameDocument.rootVisualElement.Remove(stampText);', content)

    with open(path, 'w') as f:
        f.write(content)


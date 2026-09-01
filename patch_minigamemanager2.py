import re

with open('Assets/Scripts/2 Managers/MiniGameManager.cs', 'r') as f:
    content = f.read()

content = re.sub(
    r'private void FinishGame\(\)\s*\{',
    r'''private async void FinishGame()
    {
        if (overlay.style.display == DisplayStyle.None) return;
        overlay.style.opacity = 0;
        var card = overlay.Q<VisualElement>("MiniGameCard");
        if (card != null) card.style.scale = new StyleScale(new Scale(new Vector3(1, 0, 1)));
        await Task.Delay(300);
''',
    content
)

with open('Assets/Scripts/2 Managers/MiniGameManager.cs', 'w') as f:
    f.write(content)


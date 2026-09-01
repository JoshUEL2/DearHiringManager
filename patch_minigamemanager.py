import re

with open('Assets/Scripts/2 Managers/MiniGameManager.cs', 'r') as f:
    content = f.read()

# Add async System.Threading.Tasks
if 'using System.Threading.Tasks;' not in content:
    content = 'using System.Threading.Tasks;\n' + content

# Patch LaunchMiniGame to open with animation
content = re.sub(
    r'overlay\.style\.display = DisplayStyle\.Flex;\s*overlay\.pickingMode = PickingMode\.Position;',
    r'''overlay.style.display = DisplayStyle.Flex;
        overlay.pickingMode = PickingMode.Position;
        overlay.schedule.Execute(() => {
            overlay.style.opacity = 1;
            var card = overlay.Q<VisualElement>("MiniGameCard");
            if (card != null) card.style.scale = new StyleScale(new Scale(new Vector3(1, 1, 1)));
        }).StartingIn(10);''',
    content
)

# Patch AbortGame to close with animation
content = re.sub(
    r'public void AbortGame\(\)\s*\{',
    r'''public async void AbortGame()
    {
        if (overlay.style.display == DisplayStyle.None) return;
        overlay.style.opacity = 0;
        var card = overlay.Q<VisualElement>("MiniGameCard");
        if (card != null) card.style.scale = new StyleScale(new Scale(new Vector3(1, 0, 1)));
        await Task.Delay(300);
''',
    content
)

# Patch the other Close method (FinishWin? line 565)
content = re.sub(
    r'public void FinishWin\(\)\s*\{',
    r'''public async void FinishWin()
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


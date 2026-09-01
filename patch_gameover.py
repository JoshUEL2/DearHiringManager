import re

# Update UXML
with open('Assets/UI/GameOver.uxml', 'r') as f:
    content = f.read()

content = re.sub(
    r'<ui:VisualElement name="GameOverOverlay".*?>\n\s*<ui:VisualElement name="GameOverCard" class="popup-card".*?>',
    r'<ui:VisualElement name="GameOverOverlay" style="position: absolute; left: 0; top: 0; right: 0; bottom: 0; background-color: rgba(15, 23, 42, 0.95); justify-content: center; align-items: center; display: none; opacity: 0; transition-property: opacity; transition-duration: 0.2s;">\n        <ui:VisualElement name="GameOverCard" class="popup-card" style="align-items: center; border-color: #E74C3C; border-width: 4px; scale: 1 0; transition-property: scale; transition-duration: 0.3s; transition-timing-function: ease-out-back;">',
    content, flags=re.DOTALL
)

with open('Assets/UI/GameOver.uxml', 'w') as f:
    f.write(content)

# Update CS
with open('Assets/Scripts/3 UI/GameOverUI.cs', 'r') as f:
    content = f.read()

content = content.replace(
    'gameOverOverlay.style.display = DisplayStyle.Flex;',
    '''gameOverOverlay.style.display = DisplayStyle.Flex;
        gameOverOverlay.style.opacity = 0f;
        var card = gameOverOverlay.Q<VisualElement>("GameOverCard");
        if (card != null) card.style.scale = new UnityEngine.UIElements.StyleScale(new UnityEngine.UIElements.Scale(new UnityEngine.Vector3(1, 0, 1)));
        gameOverOverlay.schedule.Execute(() => {
            gameOverOverlay.style.opacity = 1f;
            if (card != null) card.style.scale = new UnityEngine.UIElements.StyleScale(new UnityEngine.UIElements.Scale(new UnityEngine.Vector3(1, 1, 1)));
        }).StartingIn(50);'''
)

with open('Assets/Scripts/3 UI/GameOverUI.cs', 'w') as f:
    f.write(content)


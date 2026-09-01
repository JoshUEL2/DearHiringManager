import re

# Update UXML
with open('Assets/UI/MiniGame.uxml', 'r') as f:
    content = f.read()

content = re.sub(
    r'<ui:VisualElement name="Overlay".*?>\n\s*<!-- Terminal Window -->\n\s*<ui:VisualElement class="popup-card".*?>',
    r'<ui:VisualElement name="Overlay" style="position: absolute; left: 0; top: 0; right: 0; bottom: 0; background-color: rgba(0, 0, 0, 0.85); justify-content: center; align-items: center; display: none; opacity: 0; transition-property: opacity; transition-duration: 0.2s;">\n        <!-- Terminal Window -->\n        <ui:VisualElement name="MiniGameCard" class="popup-card" style="padding: 20px; max-height: 95%; margin-top: -20px; position: relative; scale: 1 0; transition-property: scale; transition-duration: 0.3s; transition-timing-function: ease-out-back;">',
    content, flags=re.DOTALL
)

with open('Assets/UI/MiniGame.uxml', 'w') as f:
    f.write(content)

# Update CS
with open('Assets/Scripts/2 Managers/MiniGameManager.cs', 'r') as f:
    content = f.read()

content = content.replace(
    'overlay.style.display = UnityEngine.UIElements.DisplayStyle.Flex;',
    '''overlay.style.display = UnityEngine.UIElements.DisplayStyle.Flex;
        overlay.style.opacity = 0f;
        var card = overlay.Q<UnityEngine.UIElements.VisualElement>("MiniGameCard");
        if (card != null) card.style.scale = new UnityEngine.UIElements.StyleScale(new UnityEngine.UIElements.Scale(new UnityEngine.Vector3(1, 0, 1)));
        overlay.schedule.Execute(() => {
            overlay.style.opacity = 1f;
            if (card != null) card.style.scale = new UnityEngine.UIElements.StyleScale(new UnityEngine.UIElements.Scale(new UnityEngine.Vector3(1, 1, 1)));
        }).StartingIn(50);'''
)

content = content.replace(
    'overlay.style.display = UnityEngine.UIElements.DisplayStyle.None;',
    '''var card = overlay.Q<UnityEngine.UIElements.VisualElement>("MiniGameCard");
            if (card != null) card.style.scale = new UnityEngine.UIElements.StyleScale(new UnityEngine.UIElements.Scale(new UnityEngine.Vector3(1, 0, 1)));
            overlay.style.opacity = 0f;
            overlay.schedule.Execute(() => {
                overlay.style.display = UnityEngine.UIElements.DisplayStyle.None;
            }).StartingIn(300);'''
)

with open('Assets/Scripts/2 Managers/MiniGameManager.cs', 'w') as f:
    f.write(content)


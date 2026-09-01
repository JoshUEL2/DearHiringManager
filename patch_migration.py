import re

# Update UXML
with open('Assets/UI/MigrationUI.uxml', 'r') as f:
    content = f.read()

content = re.sub(
    r'<ui:VisualElement name="MigrationOverlay".*?>\n\s*<ui:VisualElement class="popup-card".*?>',
    r'<ui:VisualElement name="MigrationOverlay" style="position: absolute; left: 0; top: 0; right: 0; bottom: 0; background-color: rgba(0, 0, 0, 0.85); justify-content: center; align-items: center; display: none; opacity: 0; transition-property: opacity; transition-duration: 0.2s;">\n        <ui:VisualElement name="MigrationCard" class="popup-card" style="width: 95%; max-height: 80%; scale: 1 0; transition-property: scale; transition-duration: 0.3s; transition-timing-function: ease-out-back;">',
    content, flags=re.DOTALL
)

with open('Assets/UI/MigrationUI.uxml', 'w') as f:
    f.write(content)

# Update CS
with open('Assets/Scripts/3 UI/MigrationUI.cs', 'r') as f:
    content = f.read()

content = content.replace(
    'migrationOverlay.style.display = DisplayStyle.Flex;',
    '''migrationOverlay.style.display = DisplayStyle.Flex;
        migrationOverlay.style.opacity = 0f;
        var card = migrationOverlay.Q<VisualElement>("MigrationCard");
        if (card != null) card.style.scale = new UnityEngine.UIElements.StyleScale(new UnityEngine.UIElements.Scale(new UnityEngine.Vector3(1, 0, 1)));
        migrationOverlay.schedule.Execute(() => {
            migrationOverlay.style.opacity = 1f;
            if (card != null) card.style.scale = new UnityEngine.UIElements.StyleScale(new UnityEngine.UIElements.Scale(new UnityEngine.Vector3(1, 1, 1)));
        }).StartingIn(50);'''
)

content = content.replace(
    'migrationOverlay.style.display = DisplayStyle.None;',
    '''var card = migrationOverlay.Q<VisualElement>("MigrationCard");
        if (card != null) card.style.scale = new UnityEngine.UIElements.StyleScale(new UnityEngine.UIElements.Scale(new UnityEngine.Vector3(1, 0, 1)));
        migrationOverlay.style.opacity = 0f;
        migrationOverlay.schedule.Execute(() => {
            migrationOverlay.style.display = DisplayStyle.None;
        }).StartingIn(300);'''
)

with open('Assets/Scripts/3 UI/MigrationUI.cs', 'w') as f:
    f.write(content)


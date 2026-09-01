import re

# Update UXML
with open('Assets/UI/EventPopup.uxml', 'r') as f:
    content = f.read()

content = re.sub(
    r'<ui:VisualElement name="EventOverlay".*?>\n\s*<ui:VisualElement class="popup-card">',
    r'<ui:VisualElement name="EventOverlay" style="position: absolute; top: 0; left: 0; right: 0; bottom: 0; background-color: rgba(15, 23, 42, 0.95); justify-content: center; align-items: center; display: none; opacity: 0; transition-property: opacity; transition-duration: 0.2s;">\n        <ui:VisualElement name="EventCard" class="popup-card" style="scale: 1 0; transition-property: scale; transition-duration: 0.3s; transition-timing-function: ease-out-back;">',
    content, flags=re.DOTALL
)

with open('Assets/UI/EventPopup.uxml', 'w') as f:
    f.write(content)

# Update CS
with open('Assets/Scripts/3 UI/EventPopupUI.cs', 'r') as f:
    content = f.read()

content = content.replace(
    'eventOverlay.style.display = DisplayStyle.Flex;',
    '''eventOverlay.style.display = DisplayStyle.Flex;
        eventOverlay.style.opacity = 0f;
        var card = eventOverlay.Q<VisualElement>("EventCard");
        if (card != null) card.style.scale = new UnityEngine.UIElements.StyleScale(new UnityEngine.UIElements.Scale(new UnityEngine.Vector3(1, 0, 1)));
        eventOverlay.schedule.Execute(() => {
            eventOverlay.style.opacity = 1f;
            if (card != null) card.style.scale = new UnityEngine.UIElements.StyleScale(new UnityEngine.UIElements.Scale(new UnityEngine.Vector3(1, 1, 1)));
        }).StartingIn(50);'''
)

content = content.replace(
    'eventOverlay.style.display = DisplayStyle.None;',
    '''var card = eventOverlay.Q<VisualElement>("EventCard");
        if (card != null) card.style.scale = new UnityEngine.UIElements.StyleScale(new UnityEngine.UIElements.Scale(new UnityEngine.Vector3(1, 0, 1)));
        eventOverlay.style.opacity = 0f;
        eventOverlay.schedule.Execute(() => {
            eventOverlay.style.display = DisplayStyle.None;
        }).StartingIn(300);'''
)

with open('Assets/Scripts/3 UI/EventPopupUI.cs', 'w') as f:
    f.write(content)


import re

# Update UXML
with open('Assets/UI/FinancialGraph.uxml', 'r') as f:
    content = f.read()

content = re.sub(
    r'<ui:VisualElement name="GraphOverlay".*?>\n\s*<ui:VisualElement class="popup-card".*?>',
    r'<ui:VisualElement name="GraphOverlay" style="position: absolute; left: 0; top: 0; right: 0; bottom: 0; background-color: rgba(0, 0, 0, 0.85); justify-content: center; align-items: center; display: none; opacity: 0; transition-property: opacity; transition-duration: 0.2s;">\n        <ui:VisualElement name="GraphCard" class="popup-card" style="width: 95%; scale: 1 0; transition-property: scale; transition-duration: 0.3s; transition-timing-function: ease-out-back;">',
    content, flags=re.DOTALL
)

with open('Assets/UI/FinancialGraph.uxml', 'w') as f:
    f.write(content)

# Update CS
with open('Assets/Scripts/3 UI/FinancialGraphUI.cs', 'r') as f:
    content = f.read()

content = content.replace(
    'graphOverlay.style.display = DisplayStyle.Flex;',
    '''graphOverlay.style.display = DisplayStyle.Flex;
        graphOverlay.style.opacity = 0f;
        var card = graphOverlay.Q<VisualElement>("GraphCard");
        if (card != null) card.style.scale = new UnityEngine.UIElements.StyleScale(new UnityEngine.UIElements.Scale(new UnityEngine.Vector3(1, 0, 1)));
        graphOverlay.schedule.Execute(() => {
            graphOverlay.style.opacity = 1f;
            if (card != null) card.style.scale = new UnityEngine.UIElements.StyleScale(new UnityEngine.UIElements.Scale(new UnityEngine.Vector3(1, 1, 1)));
        }).StartingIn(50);'''
)

content = content.replace(
    'graphOverlay.style.display = DisplayStyle.None;',
    '''var card = graphOverlay.Q<VisualElement>("GraphCard");
        if (card != null) card.style.scale = new UnityEngine.UIElements.StyleScale(new UnityEngine.UIElements.Scale(new UnityEngine.Vector3(1, 0, 1)));
        graphOverlay.style.opacity = 0f;
        graphOverlay.schedule.Execute(() => {
            graphOverlay.style.display = DisplayStyle.None;
        }).StartingIn(300);'''
)

with open('Assets/Scripts/3 UI/FinancialGraphUI.cs', 'w') as f:
    f.write(content)


import re

with open('Assets/Scripts/2 Managers/GameUIManager.cs', 'r') as f:
    content = f.read()

# Animate the dynamic popup
content = content.replace(
    'var card = new VisualElement();',
    '''var card = new VisualElement();
        card.style.transitionProperty = new StyleList<StylePropertyName>(new List<StylePropertyName> { new StylePropertyName("scale") });
        card.style.transitionDuration = new StyleList<TimeValue>(new List<TimeValue> { new TimeValue(0.3f) });
        card.style.transitionTimingFunction = new StyleList<EasingFunction>(new List<EasingFunction> { new EasingFunction(EasingMode.EaseOutBack) });
        card.style.scale = new StyleScale(new Scale(new Vector3(1, 0, 1)));
        
        overlay.style.transitionProperty = new StyleList<StylePropertyName>(new List<StylePropertyName> { new StylePropertyName("opacity") });
        overlay.style.transitionDuration = new StyleList<TimeValue>(new List<TimeValue> { new TimeValue(0.2f) });
        overlay.style.opacity = 0f;'''
)

content = content.replace(
    'overlay.RemoveFromHierarchy(); \n            onAcknowledge?.Invoke();',
    '''card.style.scale = new StyleScale(new Scale(new Vector3(1, 0, 1)));
            overlay.style.opacity = 0f;
            overlay.schedule.Execute(() => {
                overlay.RemoveFromHierarchy(); 
                onAcknowledge?.Invoke();
            }).StartingIn(300);'''
)

content = content.replace(
    'activeRoot.Add(overlay);',
    '''activeRoot.Add(overlay);
        overlay.schedule.Execute(() => {
            overlay.style.opacity = 1f;
            card.style.scale = new StyleScale(new Scale(new Vector3(1, 1, 1)));
        }).StartingIn(50);'''
)

with open('Assets/Scripts/2 Managers/GameUIManager.cs', 'w') as f:
    f.write(content)


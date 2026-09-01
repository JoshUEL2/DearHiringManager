import re

with open('Assets/Scripts/2 Managers/GameUIManager.cs', 'r') as f:
    content = f.read()

content = content.replace(
    'var panel = new VisualElement();',
    '''var panel = new VisualElement();
        panel.style.transitionProperty = new StyleList<StylePropertyName>(new List<StylePropertyName> { new StylePropertyName("scale") });
        panel.style.transitionDuration = new StyleList<TimeValue>(new List<TimeValue> { new TimeValue(0.3f) });
        panel.style.transitionTimingFunction = new StyleList<EasingFunction>(new List<EasingFunction> { new EasingFunction(EasingMode.EaseOutBack) });
        panel.style.scale = new StyleScale(new Scale(new Vector3(1, 0, 1)));
        
        overlay.style.transitionProperty = new StyleList<StylePropertyName>(new List<StylePropertyName> { new StylePropertyName("opacity") });
        overlay.style.transitionDuration = new StyleList<TimeValue>(new List<TimeValue> { new TimeValue(0.2f) });
        overlay.style.opacity = 0f;'''
)

content = content.replace(
    'dismissBtn.clicked += () => activeRoot.Remove(overlay);',
    '''dismissBtn.clicked += () => {
            panel.style.scale = new StyleScale(new Scale(new Vector3(1, 0, 1)));
            overlay.style.opacity = 0f;
            overlay.schedule.Execute(() => activeRoot.Remove(overlay)).StartingIn(300);
        };'''
)

with open('Assets/Scripts/2 Managers/GameUIManager.cs', 'w') as f:
    f.write(content)


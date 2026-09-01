import re

with open('Assets/Scripts/2 Managers/GameUIManager.cs', 'r') as f:
    content = f.read()

content = content.replace(
    '''paper.Add(closeBtn);
        
        overlay.Add(paper);
        activeRoot.Add(overlay);
        overlay.schedule.Execute(() => {
            overlay.style.opacity = 1f;
            card.style.scale = new StyleScale(new Scale(new Vector3(1, 1, 1)));
        }).StartingIn(50);''',
    '''paper.Add(closeBtn);
        
        paper.style.transitionProperty = new StyleList<StylePropertyName>(new List<StylePropertyName> { new StylePropertyName("scale") });
        paper.style.transitionDuration = new StyleList<TimeValue>(new List<TimeValue> { new TimeValue(0.3f) });
        paper.style.transitionTimingFunction = new StyleList<EasingFunction>(new List<EasingFunction> { new EasingFunction(EasingMode.EaseOutBack) });
        paper.style.scale = new StyleScale(new Scale(new Vector3(1, 0, 1)));
        
        overlay.style.transitionProperty = new StyleList<StylePropertyName>(new List<StylePropertyName> { new StylePropertyName("opacity") });
        overlay.style.transitionDuration = new StyleList<TimeValue>(new List<TimeValue> { new TimeValue(0.2f) });
        overlay.style.opacity = 0f;
        
        overlay.Add(paper);
        activeRoot.Add(overlay);
        overlay.schedule.Execute(() => {
            overlay.style.opacity = 1f;
            paper.style.scale = new StyleScale(new Scale(new Vector3(1, 1, 1)));
        }).StartingIn(50);'''
)

content = content.replace(
    '''closeBtn.clicked += () => {
            activeRoot.Remove(overlay);
        };''',
    '''closeBtn.clicked += () => {
            paper.style.scale = new StyleScale(new Scale(new Vector3(1, 0, 1)));
            overlay.style.opacity = 0f;
            overlay.schedule.Execute(() => activeRoot.Remove(overlay)).StartingIn(300);
        };'''
)

with open('Assets/Scripts/2 Managers/GameUIManager.cs', 'w') as f:
    f.write(content)


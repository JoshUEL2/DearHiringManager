import re

with open('Assets/Scripts/2 Managers/GameUIManager.cs', 'r') as f:
    content = f.read()

# Settings Open
content = content.replace(
    'settingsOverlay.style.display = DisplayStyle.Flex;\n        settingsOverlay.schedule.Execute(() => settingsOverlay.style.opacity = 1f).StartingIn(50);',
    '''settingsOverlay.style.display = DisplayStyle.Flex;
        settingsOverlay.style.opacity = 0f;
        var card = settingsOverlay.Q<VisualElement>("SettingsCard");
        if (card != null) card.style.scale = new UnityEngine.UIElements.StyleScale(new UnityEngine.UIElements.Scale(new UnityEngine.Vector3(1, 0, 1)));
        settingsOverlay.schedule.Execute(() => {
            settingsOverlay.style.opacity = 1f;
            if (card != null) card.style.scale = new UnityEngine.UIElements.StyleScale(new UnityEngine.UIElements.Scale(new UnityEngine.Vector3(1, 1, 1)));
        }).StartingIn(50);'''
)

# Settings Close
content = content.replace(
    'settingsOverlay.style.opacity = 0f;\n        settingsOverlay.schedule.Execute(() => settingsOverlay.style.display = DisplayStyle.None).StartingIn(300);',
    '''var card = settingsOverlay.Q<VisualElement>("SettingsCard");
        if (card != null) card.style.scale = new UnityEngine.UIElements.StyleScale(new UnityEngine.UIElements.Scale(new UnityEngine.Vector3(1, 0, 1)));
        settingsOverlay.style.opacity = 0f;
        settingsOverlay.schedule.Execute(() => settingsOverlay.style.display = DisplayStyle.None).StartingIn(300);'''
)

# Debug Menu Open
content = content.replace(
    'debugOverlay.style.display = DisplayStyle.Flex;\n                debugOverlay.BringToFront();',
    '''debugOverlay.style.display = DisplayStyle.Flex;
                debugOverlay.style.opacity = 0f;
                var card = debugOverlay.Q<VisualElement>("DebugGamesCard");
                if (card != null) card.style.scale = new UnityEngine.UIElements.StyleScale(new UnityEngine.UIElements.Scale(new UnityEngine.Vector3(1, 0, 1)));
                debugOverlay.schedule.Execute(() => {
                    debugOverlay.style.opacity = 1f;
                    if (card != null) card.style.scale = new UnityEngine.UIElements.StyleScale(new UnityEngine.UIElements.Scale(new UnityEngine.Vector3(1, 1, 1)));
                }).StartingIn(50);
                debugOverlay.BringToFront();'''
)

# Debug Menu Close
content = content.replace(
    'btnCloseDebug.clicked += () => debugOverlay.style.display = DisplayStyle.None;',
    '''btnCloseDebug.clicked += () => {
                    var card = debugOverlay.Q<VisualElement>("DebugGamesCard");
                    if (card != null) card.style.scale = new UnityEngine.UIElements.StyleScale(new UnityEngine.UIElements.Scale(new UnityEngine.Vector3(1, 0, 1)));
                    debugOverlay.style.opacity = 0f;
                    debugOverlay.schedule.Execute(() => debugOverlay.style.display = DisplayStyle.None).StartingIn(300);
                };'''
)

content = content.replace(
    'debugOverlay.style.display = DisplayStyle.None;',
    '''var card2 = debugOverlay.Q<VisualElement>("DebugGamesCard");
                            if (card2 != null) card2.style.scale = new UnityEngine.UIElements.StyleScale(new UnityEngine.UIElements.Scale(new UnityEngine.Vector3(1, 0, 1)));
                            debugOverlay.style.opacity = 0f;
                            debugOverlay.schedule.Execute(() => debugOverlay.style.display = DisplayStyle.None).StartingIn(300);'''
)


with open('Assets/Scripts/2 Managers/GameUIManager.cs', 'w') as f:
    f.write(content)


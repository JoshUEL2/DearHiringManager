import re

with open('Assets/Scripts/2 Managers/GameUIManager.cs', 'r') as f:
    content = f.read()

# Fix ShowLuckStatsPopup (uses 'panel' instead of 'card')
content = content.replace(
    '''panel.Add(dismissBtn);
        overlay.Add(panel);
        activeRoot.Add(overlay);
        overlay.schedule.Execute(() => {
            overlay.style.opacity = 1f;
            card.style.scale = new StyleScale(new Scale(new Vector3(1, 1, 1)));
        }).StartingIn(50);''',
    '''panel.Add(dismissBtn);
        overlay.Add(panel);
        activeRoot.Add(overlay);
        overlay.schedule.Execute(() => {
            overlay.style.opacity = 1f;
            panel.style.scale = new StyleScale(new Scale(new Vector3(1, 1, 1)));
        }).StartingIn(50);'''
)

# Fix ShowNotification (uses 'toast' instead of 'card')
content = content.replace(
    '''toast.Add(msgLbl);
        overlay.Add(toast);

        activeRoot.Add(overlay);
        overlay.schedule.Execute(() => {
            overlay.style.opacity = 1f;
            card.style.scale = new StyleScale(new Scale(new Vector3(1, 1, 1)));
        }).StartingIn(50);''',
    '''toast.Add(msgLbl);
        overlay.Add(toast);

        activeRoot.Add(overlay);
        overlay.schedule.Execute(() => {
            overlay.style.opacity = 1f;
            toast.style.scale = new StyleScale(new Scale(new Vector3(1, 1, 1)));
        }).StartingIn(50);'''
)

with open('Assets/Scripts/2 Managers/GameUIManager.cs', 'w') as f:
    f.write(content)


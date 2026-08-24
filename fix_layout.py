import re

path = 'Assets/Scripts/2 Managers/MiniGames/SequenceGameMiniGame.cs'

with open(path, 'r') as f:
    content = f.read()

target = r'''        qteContainer = root\.Q<VisualElement>\("SequenceGameContainer"\);
        if \(qteContainer != null\) qteContainer\.Clear\(\);
        
        qteContainer\.style\.position = Position\.Relative;
        qteContainer\.style\.width = new Length\(100, LengthUnit\.Percent\);
        qteContainer\.style\.height = 400;'''

replacement = r'''        var old = root.Q<VisualElement>("SequenceGameContainer");
        if (old != null) old.style.display = DisplayStyle.None;
        
        qteContainer = new VisualElement();
        qteContainer.style.position = Position.Relative;
        qteContainer.style.width = 380; // Explicit pixels to prevent shrink-wrap!
        qteContainer.style.height = 400; 
        qteContainer.style.alignSelf = Align.Center; // Center it horizontally
        root.Add(qteContainer);'''

content = re.sub(target, replacement, content)

with open(path, 'w') as f:
    f.write(content)


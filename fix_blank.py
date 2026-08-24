import re

path = 'Assets/Scripts/2 Managers/MiniGames/SequenceGameMiniGame.cs'

with open(path, 'r') as f:
    content = f.read()

target = r'''        // Hide the original UXML container to avoid shrink issues
        var oldContainer = root\.Q<VisualElement>\("SequenceGameContainer"\);
        if \(oldContainer != null\) oldContainer\.style\.display = DisplayStyle\.None;

        // Build a brand new absolute container that forcefully fills the minigame area
        qteContainer = new VisualElement\(\);
        qteContainer\.style\.position = Position\.Absolute;
        qteContainer\.style\.left = 0; qteContainer\.style\.right = 0; qteContainer\.style\.top = 0; qteContainer\.style\.bottom = 0;
        qteContainer\.style\.minHeight = 400; // Guarantee minimum height'''

replacement = r'''        // Reuse the original container but fix its width/height so it doesn't collapse!
        qteContainer = root.Q<VisualElement>("SequenceGameContainer");
        if (qteContainer != null) qteContainer.Clear();
        
        qteContainer.style.position = Position.Relative;
        qteContainer.style.width = new Length(100, LengthUnit.Percent);
        qteContainer.style.height = 400; // Force fixed height!'''

content = re.sub(target, replacement, content)

# Remove the line `root.Add(qteContainer);` because it's already in root!
content = re.sub(r'\s*root\.Add\(qteContainer\);', '', content)

with open(path, 'w') as f:
    f.write(content)


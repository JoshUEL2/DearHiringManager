import re

path = 'Assets/Scripts/2 Managers/MiniGames/DragCenterMiniGame.cs'
with open(path, 'r') as f:
    content = f.read()

# 1. Remove xGuide and yGuide from CreateJuiceElements
content = re.sub(r'\s*// Magnetic Grid Guides.*?workArea\.Add\(yGuide\);', '', content, flags=re.DOTALL)

# 2. In ResetRound, remove xGuide and yGuide hiding
content = re.sub(r'\s*xGuide\.style\.display = DisplayStyle\.None;', '', content)
content = re.sub(r'\s*yGuide\.style\.display = DisplayStyle\.None;', '', content)

# 3. In OnPointerDown, remove cssTooltip show
content = re.sub(r'\s*cssTooltip\.style\.display = DisplayStyle\.Flex;', '', content)

# 4. In OnPointerMove, remove snap logic and tooltip updates
snap_logic = r'''            // Snapping guides logic.*?cssTooltip\.style\.top = new Length\(percentY - 15f, LengthUnit\.Percent\); // Float above icon'''
content = re.sub(snap_logic, '', content, flags=re.DOTALL)

# 5. In EndDrag, remove xGuide and yGuide hiding
# Note: cssTooltip hiding remains so it resets between chances
content = re.sub(r'\s*xGuide\.style\.display = DisplayStyle\.None;', '', content)
content = re.sub(r'\s*yGuide\.style\.display = DisplayStyle\.None;', '', content)

with open(path, 'w') as f:
    f.write(content)


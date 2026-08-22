import re

path = 'Assets/Scripts/2 Managers/MiniGames/DragCenterMiniGame.cs'
with open(path, 'r') as f:
    content = f.read()

# SUCCESS
success_target = r'''            cssTooltip.text = "align-items: center;";
            cssTooltip.style.display = DisplayStyle.Flex;'''
success_replace = r'''            cssTooltip.style.left = draggableIcon.style.left;
            cssTooltip.style.top = new Length(draggableIcon.style.top.value.value - 15f, LengthUnit.Percent);
            cssTooltip.text = "align-items: center;";
            cssTooltip.style.display = DisplayStyle.Flex;'''
content = content.replace(success_target, success_replace)

# FAILURE
fail_target = r'''            cssTooltip.text = "FATAL: CSS BROKEN";
            cssTooltip.style.display = DisplayStyle.Flex;'''
fail_replace = r'''            cssTooltip.style.left = new Length(0f, LengthUnit.Percent);
            cssTooltip.style.top = new Length(20f, LengthUnit.Percent);
            cssTooltip.text = "FATAL: CSS BROKEN";
            cssTooltip.style.display = DisplayStyle.Flex;'''
content = content.replace(fail_target, fail_replace)

with open(path, 'w') as f:
    f.write(content)

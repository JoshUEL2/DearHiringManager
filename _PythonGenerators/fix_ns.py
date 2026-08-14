import sys

with open("Assets/UI/MiniGame.uxml", "r") as f:
    uxml = f.read()

# Remove the old timer from the top
old_timer = """<!-- Timer Header -->
                    <ui:VisualElement style="position: absolute; top: 10px; right: 10px; background-color: #1F2937; padding: 5px 10px; border-radius: 5px; z-index: 10;">
                        <ui:Label name="NsTimerText" text="15.0s" style="color: white; font-size: 16px; -unity-font-style: bold;" />
                    </ui:VisualElement>
                    """
uxml = uxml.replace(old_timer, "")

# Change eyelid to 0.9
uxml = uxml.replace("background-color: #000000;", "background-color: rgba(0, 0, 0, 0.9);")

# Update coffee button container with timer
old_footer = """                    <!-- Coffee Button -->
                    <ui:VisualElement style="width: 100%; padding: 10px; background-color: #CBD5E1;">
                        <ui:Button name="NsCoffeeBtn" text="☕ SLAP FACE / DRINK COFFEE" style="height: 60px; font-size: 20px; background-color: #374151; color: white; border-radius: 10px; -unity-font-style: bold; border-width: 0;" />
                    </ui:VisualElement>"""

new_footer = """                    <!-- Coffee Button & Timer -->
                    <ui:VisualElement style="width: 100%; padding: 10px; background-color: #CBD5E1; flex-direction: row; align-items: center;">
                        <ui:Button name="NsCoffeeBtn" text="☕ DRINK COFFEE" style="flex-grow: 1; height: 60px; font-size: 20px; background-color: #374151; color: white; border-radius: 10px; -unity-font-style: bold; border-width: 0; margin-right: 10px;" />
                        <ui:VisualElement style="background-color: #1F2937; height: 60px; justify-content: center; padding: 0 15px; border-radius: 10px;">
                            <ui:Label name="NsTimerText" text="15.0s" style="color: white; font-size: 20px; -unity-font-style: bold;" />
                        </ui:VisualElement>
                    </ui:VisualElement>"""
uxml = uxml.replace(old_footer, new_footer)

with open("Assets/UI/MiniGame.uxml", "w") as f:
    f.write(uxml)

print("Applied fixes")

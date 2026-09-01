import re

# Update UXML
with open('Assets/UI/LuckStatsUI.uxml', 'r') as f:
    content = f.read()

content = re.sub(
    r'<ui:VisualElement name="LuckPopupOverlay".*?>\n\s*<ui:VisualElement class="popup-panel".*?>',
    r'<ui:VisualElement name="LuckPopupOverlay" class="popup-overlay" style="display: none; position: absolute; left: 0; top: 0; right: 0; bottom: 0; background-color: rgba(0, 0, 0, 0.85); align-items: center; justify-content: center; opacity: 0; transition-property: opacity; transition-duration: 0.2s;">\n        <ui:VisualElement name="LuckPopupCard" class="popup-panel" style="background-color: #FFFFFF; border-radius: 40px; width: 90%; max-width: 900px; padding: 60px; scale: 1 0; transition-property: scale; transition-duration: 0.3s; transition-timing-function: ease-out-back;">',
    content, flags=re.DOTALL
)

with open('Assets/UI/LuckStatsUI.uxml', 'w') as f:
    f.write(content)


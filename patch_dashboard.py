import re

with open('Assets/UI/DashboardUI.uxml', 'r') as f:
    content = f.read()

# Settings Overlay
content = re.sub(
    r'<ui:VisualElement name="SettingsOverlay".*?>\n\s*<ui:VisualElement class="card"',
    r'<ui:VisualElement name="SettingsOverlay" style="position: absolute; top: 0; left: 0; right: 0; bottom: 0; background-color: rgba(15, 23, 42, 0.98); justify-content: center; align-items: center; display: none; opacity: 0; transition-property: opacity; transition-duration: 0.2s;">\n        <ui:VisualElement name="SettingsCard" class="card" style="width: 90%; background-color: #0F172A; border-color: #38BDF8; border-width: 2px; border-radius: 20px; padding: 25px; scale: 1 0; transition-property: scale; transition-duration: 0.3s; transition-timing-function: ease-out-back;"',
    content, flags=re.DOTALL
)

# Debug Games Overlay
content = re.sub(
    r'<ui:VisualElement name="DebugGamesOverlay".*?>\n\s*<ui:VisualElement class="card"',
    r'<ui:VisualElement name="DebugGamesOverlay" style="display: none; position: absolute; top: 0; left: 0; right: 0; bottom: 0; background-color: rgba(15,23,42,0.95); padding: 40px; justify-content: center; align-items: center; opacity: 0; transition-property: opacity; transition-duration: 0.2s;">\n        <ui:VisualElement name="DebugGamesCard" class="card" style="width: 100%; height: 90%; background-color: #1E293B; border-color: #E74C3C; border-width: 2px; border-radius: 20px; padding: 25px; scale: 1 0; transition-property: scale; transition-duration: 0.3s; transition-timing-function: ease-out-back;"',
    content, flags=re.DOTALL
)

with open('Assets/UI/DashboardUI.uxml', 'w') as f:
    f.write(content)


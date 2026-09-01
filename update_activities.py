import re

with open('Assets/UI/ActivitiesUI.uxml', 'r') as f:
    uxml = f.read()

# Replace the inner contents of WellnessContainer with an empty named ScrollView
uxml = re.sub(
    r'<ui:VisualElement name="WellnessContainer".*?>.*?</ui:VisualElement>',
    '<ui:VisualElement name="WellnessContainer" style="flex-grow: 1; display: flex;">\n                <ui:ScrollView name="WellnessScrollView" style="flex-grow: 1; width: 100%; padding-bottom: 40px;" />\n            </ui:VisualElement>',
    uxml, flags=re.DOTALL
)

# Replace the inner contents of MixersContainer with an empty named ScrollView
uxml = re.sub(
    r'<ui:VisualElement name="MixersContainer".*?>.*?</ui:VisualElement>',
    '<ui:VisualElement name="MixersContainer" style="flex-grow: 1; display: none;">\n                <ui:ScrollView name="MixersScrollView" style="flex-grow: 1; width: 100%; padding-bottom: 40px;" />\n            </ui:VisualElement>',
    uxml, flags=re.DOTALL
)

with open('Assets/UI/ActivitiesUI.uxml', 'w') as f:
    f.write(uxml)

with open('Assets/UI/ActivitiesUI.uxml', 'r') as f:
    uxml = f.read()

import re

# We can replace everything between <ui:VisualElement name="WellnessContainer" and the MixersContainer start
# Wait, it's easier to just do string manipulation.
start_wellness = uxml.find('<ui:VisualElement name="WellnessContainer"')
end_wellness = uxml.find('<ui:VisualElement name="MixersContainer"')
start_mixers = end_wellness
end_mixers = uxml.find('</ui:VisualElement>', uxml.find('</ui:ScrollView>', start_mixers)) + len('</ui:VisualElement>')

new_wellness = '<ui:VisualElement name="WellnessContainer" style="flex-grow: 1; display: flex;">\n                <ui:ScrollView name="WellnessScrollView" style="flex-grow: 1; width: 100%; padding-bottom: 40px;" />\n            </ui:VisualElement>\n\n            '
new_mixers = '<ui:VisualElement name="MixersContainer" style="flex-grow: 1; display: none;">\n                <ui:ScrollView name="MixersScrollView" style="flex-grow: 1; width: 100%; padding-bottom: 40px;" />\n            </ui:VisualElement>'

uxml = uxml[:start_wellness] + new_wellness + new_mixers + uxml[end_mixers:]

with open('Assets/UI/ActivitiesUI.uxml', 'w') as f:
    f.write(uxml)

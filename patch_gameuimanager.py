import re

with open("Assets/Scripts/2 Managers/GameUIManager.cs", "r") as f:
    content = f.read()

# We need to change:
# var pName = root.Q<Label>("PlayerNameText");
# if (pName != null) pName.text = playerName;
# to:
# root.Query<Label>("PlayerNameText").ForEach(l => l.text = playerName);

replacements = [
    (r'var pName = root\.Q<Label>\("PlayerNameText"\);\s*if \(pName != null\) pName\.text = playerName;',
     r'root.Query<Label>("PlayerNameText").ForEach(l => l.text = playerName);'),
     
    (r'var sTitle = root\.Q<Label>\("SubTitleText"\);\s*if \(sTitle != null\) sTitle\.text = (.*?);',
     r'root.Query<Label>("SubTitleText").ForEach(l => l.text = \1);'),
     
    (r'var cText = root\.Q<Label>\("CashText"\);\s*if \(cText != null\) cText\.text = (.*?);',
     r'root.Query<Label>("CashText").ForEach(l => l.text = \1);'),
     
    (r'var eText = root\.Q<Label>\("ExpenseText"\);\s*if \(eText != null\) eText\.text = (.*?);',
     r'root.Query<Label>("ExpenseText").ForEach(l => l.text = \1);'),
     
    (r'var jText = root\.Q<Label>\("JobText"\);\s*if \(jText != null\) jText\.text = (.*?);',
     r'root.Query<Label>("JobText").ForEach(l => l.text = \1);'),
     
    (r'var dText = root\.Q<Label>\("DistrictText"\);\s*if \(dText != null\) dText\.text = (.*?);',
     r'root.Query<Label>("DistrictText").ForEach(l => l.text = \1);')
]

for old, new in replacements:
    content = re.sub(old, new, content, flags=re.MULTILINE)

# I also need to update the top card background logic!
# The script currently does:
# var topCard = root.Q<VisualElement>(className: "top-card");
# In my new UXML, the outer container is "top-card-root" (or wait, I kept it as "top-card-root"? Let me check).
# Yes, <ui:VisualElement class="card top-card-root" style="margin-bottom: 20px;">
# BUT wait! My python replacement script earlier replaced 'top-card' with 'top-card-root'!
# So I should change 'className: "top-card"' to 'className: "top-card-root"'.
content = content.replace('className: "top-card"', 'className: "top-card-root"')

with open("Assets/Scripts/2 Managers/GameUIManager.cs", "w") as f:
    f.write(content)

with open('Assets/Scripts/1 Data/GameDataModels.cs', 'r') as f:
    content = f.read()
content = content.replace('Game started on the gritty streets of the Slums!', 'You are officially unemployed and broke.')
with open('Assets/Scripts/1 Data/GameDataModels.cs', 'w') as f:
    f.write(content)

with open('Assets/Scripts/2 Managers/GameUIManager.cs', 'r') as f:
    content = f.read()
content = content.replace('Game started on the gritty streets of the Slums!', 'You are officially unemployed and broke.')
with open('Assets/Scripts/2 Managers/GameUIManager.cs', 'w') as f:
    f.write(content)

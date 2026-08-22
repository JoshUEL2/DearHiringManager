import re

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'r') as f:
    content = f.read()

games_to_clean = [
    "AcidRainShield", "CloggedVents", "ContainmentProtocol", 
    "CorporateGolf", "FoodPrepRush", "ForgePass", 
    "FormatDashboard", "GalacticCouncil", "HospitalityGala", 
    "HotStoneSpa", "Choice"
]

for game in games_to_clean:
    # Remove GetUxmlName case
    content = re.sub(rf'\s*case GameType\.{game}: return "{game}.*?";', '', content)
    # Remove HideAllContainers if(container)
    content = re.sub(rf'\s*if\(.*?\) .*?\.style\.display = DisplayStyle\.None;', '', content)
    # Remove SetupGameType case
    content = re.sub(rf'\s*case GameType\.{game}:[\s\S]*?break;', '', content)

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'w') as f:
    f.write(content)
print("Cleaned 11 games from Manager switch blocks")

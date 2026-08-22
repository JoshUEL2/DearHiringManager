import re

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGames/MiniGameFactory.cs', 'r') as f:
    content = f.read()

games_to_add = [
    "IceShatter", "IpHack", "MowGreens", 
    "NameMemory", "NeuralAligner", "NightShiftPitch", 
    "RecycleBottles", "RetroRacer", "RewardLogic", 
    "SentienceArray"
]

injection = ""
for game in games_to_add:
    injection += f"            case GameType.{game}:\n                return new {game}MiniGame();\n"

# Inject before CodeBreaker
content = re.sub(r'case GameType\.CodeBreaker:\s*return new CodeBreakerMiniGame\(\);', injection + '            case GameType.CodeBreaker: return new CodeBreakerMiniGame();', content)

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGames/MiniGameFactory.cs', 'w') as f:
    f.write(content)
print("Hooked 10 games to Factory")

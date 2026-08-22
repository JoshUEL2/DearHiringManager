import re

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGames/MiniGameFactory.cs', 'r') as f:
    content = f.read()

games_to_add = [
    "SkimATM", "SnatchBriefcase", "StartupPitch", 
    "StealthInject", "TwoPhaseHeist", "VrArcadePong", 
    "WalkDogs", "WhackABug", "YesManNod"
]

injection = ""
for game in games_to_add:
    injection += f"            case GameType.{game}:\n                return new {game}MiniGame();\n"

content = re.sub(r'case GameType\.CodeBreaker:\s*return new CodeBreakerMiniGame\(\);', injection + '            case GameType.CodeBreaker: return new CodeBreakerMiniGame();', content)

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGames/MiniGameFactory.cs', 'w') as f:
    f.write(content)
print("Hooked final 9 games to Factory")

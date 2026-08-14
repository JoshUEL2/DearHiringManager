import sys

with open("Assets/Scripts/2 Managers/MiniGameManager.cs", "r") as f:
    cs = f.read()

# Change it back
cs = cs.replace(
    '{ "Corporate Office Heist", "I downloaded the proprietary blueprints before the security drones arrived." }',
    '{ "Corporate Espionage Heist", "I downloaded the proprietary blueprints before the security drones arrived." }'
)

cs = cs.replace(
    'AddConfigFull("Corporate Office Heist", new MiniGameConfig { type = GameType.TwoPhaseHeist, phase1Type = GameType.CodeBreaker, codeSymbols = new string[]{"0","1","2","3","4","5","6","7","8","9"}, codeLength = 4, maxAttempts = 6, sprintObstacles = 10, sprintSpeed = 1.2f, phase2Type = GameType.SprintLane, instructions = "Phase 1: Bypass! Phase 2: Escape!" });',
    'AddConfigFull("Corporate Espionage Heist", new MiniGameConfig { type = GameType.TwoPhaseHeist, phase1Type = GameType.CodeBreaker, codeSymbols = new string[]{"0","1","2","3","4","5","6","7","8","9"}, codeLength = 4, maxAttempts = 6, sprintObstacles = 10, sprintSpeed = 1.2f, phase2Type = GameType.SprintLane, instructions = "Phase 1: Bypass! Phase 2: Escape!" });'
)

with open("Assets/Scripts/2 Managers/MiniGameManager.cs", "w") as f:
    f.write(cs)
print("Reverted to Corporate Espionage Heist")

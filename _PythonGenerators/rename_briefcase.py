import sys

with open("Assets/Scripts/3 UI/HustlesUI.cs", "r") as f:
    cs = f.read()

cs = cs.replace(
    'availableHustles.Add(new SideHustleItem("Snatch Luxury Briefcase", "Swipe briefcases from distracted executive suits.", 650.0, 28, 2));',
    'availableHustles.Add(new SideHustleItem("Crack Luxury Briefcase", "Pick the combo locks on stolen executive briefcases.", 650.0, 28, 2));'
)

with open("Assets/Scripts/3 UI/HustlesUI.cs", "w") as f:
    f.write(cs)

with open("Assets/Scripts/2 Managers/MiniGameManager.cs", "r") as f:
    mg = f.read()

mg = mg.replace(
    'AddConfigFull("Snatch Luxury Briefcase", new MiniGameConfig { type = GameType.SnatchBriefcase, timeLimit = 15f, instructions = "Crack the lock!" });',
    'AddConfigFull("Crack Luxury Briefcase", new MiniGameConfig { type = GameType.SnatchBriefcase, timeLimit = 15f, instructions = "Crack the lock!" });'
)

mg = mg.replace(
    '{ "Snatch Luxury Briefcase", "I grabbed a shiny briefcase from a distracted suit and ran." },',
    '{ "Crack Luxury Briefcase", "I cracked the biometric locks on a stolen luxury briefcase." },'
)

mg = mg.replace(
    'else if (currentGame.name == "Snatch Luxury Briefcase") jailWeeks = 2;',
    'else if (currentGame.name == "Crack Luxury Briefcase") jailWeeks = 2;'
)

with open("Assets/Scripts/2 Managers/MiniGameManager.cs", "w") as f:
    f.write(mg)

print("Renamed briefcase!")

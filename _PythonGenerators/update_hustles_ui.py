import sys

with open("Assets/Scripts/3 UI/HustlesUI.cs", "r") as f:
    cs = f.read()

cs = cs.replace(
    'availableHustles.Add(new SideHustleItem("Mow Corporate Astroturf", "Trim plastic lawns in corporate micro-complexes.", 22.0, 0, 0));',
    'availableHustles.Add(new SideHustleItem("Mow Executive Golf Greens", "Trim putting greens for elite executives.", 22.0, 0, 0));'
)

cs = cs.replace(
    'availableHustles.Add(new SideHustleItem("Walk Robotic Pedigree Dogs", "Escort high-maintenance mechanical pups for lazy elites.", 45.0, 0, 0));',
    'availableHustles.Add(new SideHustleItem("Walk Purebred Pedigree Dogs", "Wrangle incredibly spoiled and aggressive purebreds.", 45.0, 0, 0));'
)

cs = cs.replace(
    'availableHustles.Add(new SideHustleItem("ATM Autonomous Hijack", "Inject firmware exploits into Slum currency terminals.", 1600.0, 48, 4));',
    'availableHustles.Add(new SideHustleItem("Skim ATM Card Data", "Install discreet card skimmers on Slum ATMs.", 1600.0, 48, 4));'
)

with open("Assets/Scripts/3 UI/HustlesUI.cs", "w") as f:
    f.write(cs)

print("Updated HustlesUI.cs names")

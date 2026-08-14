import sys

with open("Assets/Scripts/2 Managers/GameUIManager.cs", "r") as f:
    cs = f.read()

cs = cs.replace('"Mow Corporate Astroturf"', '"Mow Executive Golf Greens"')
cs = cs.replace('"Walk Robotic Pedigree Dogs"', '"Walk Purebred Pedigree Dogs"')

with open("Assets/Scripts/2 Managers/GameUIManager.cs", "w") as f:
    f.write(cs)
print("Fixed debug names!")

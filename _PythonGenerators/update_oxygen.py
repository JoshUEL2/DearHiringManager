import sys

with open("Assets/Scripts/2 Managers/MiniGameManager.cs", "r") as f:
    cs = f.read()

# 1. Update Enum
old_enum = "YesManNod, VrArcadePong }"
new_enum = "YesManNod, VrArcadePong, HotStoneSpa }"
cs = cs.replace(old_enum, new_enum)

# 2. Update Lore
old_lore = '{ "Premium Oxygen Bar", "You cleared your lungs with synthetic air at the Premium Oxygen Bar." },'
new_lore = '{ "Volcanic Hot Stone Spa", "The soothing warmth of the volcanic stones melted away the stress of the corporate grind." },'
cs = cs.replace(old_lore, new_lore)

# 3. Update Config
old_config = 'AddConfigFull("Premium Oxygen Bar", new MiniGameConfig { type = GameType.TimingRing, timingRounds = 3, timingZoneWidth = 30f, instructions = "Breathe in deeply!" });'
new_config = 'AddConfigFull("Volcanic Hot Stone Spa", new MiniGameConfig { type = GameType.HotStoneSpa, timeLimit = 15f, instructions = "Place the hot stones along the spine to relax." });'
cs = cs.replace(old_config, new_config)

with open("Assets/Scripts/2 Managers/MiniGameManager.cs", "w") as f:
    f.write(cs)

print("Updated Oxygen to Hot Stone")

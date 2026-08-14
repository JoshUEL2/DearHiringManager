import sys
import re

with open("Assets/Scripts/2 Managers/MiniGameManager.cs", "r") as f:
    cs = f.read()

# Remove old SetupCorporateGolf, OnCgSwingClicked, CorporateGolfRoutine
cs = re.sub(r'private void SetupCorporateGolf\(\)\s*\{.*?\}\s*\}\s*\}', '', cs, flags=re.DOTALL) # wait, that regex is dangerous.

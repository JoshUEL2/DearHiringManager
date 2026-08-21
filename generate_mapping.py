import re

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'r') as f:
    mm = f.read()
with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager_Routines1.cs', 'r') as f:
    mr1 = f.read()
    
# Extract variable name to string mapping from root.Q
var_to_string = {}
for match in re.finditer(r'([a-zA-Z0-9_]+)\s*=\s*root\.Q<VisualElement>\("([^"]+Container)"\);', mm):
    var_to_string[match.group(1)] = match.group(2)
for match in re.finditer(r'([a-zA-Z0-9_]+)\s*=\s*root\.Q<VisualElement>\("([^"]+Container)"\);', mr1):
    var_to_string[match.group(1)] = match.group(2)
    
# Add some that might be missing just in case
var_to_string['fpContainer'] = 'ForgePassContainer'
var_to_string['snatchBriefcaseContainer'] = 'SnatchBriefcaseContainer'
var_to_string['saContainer'] = 'SkimATMContainer'
var_to_string['ipHackContainer'] = 'IpHackContainer'
var_to_string['rbContainer'] = 'RecycleBottlesContainer'
var_to_string['cvContainer'] = 'CloggedVentsContainer'
var_to_string['wbContainer'] = 'WhackABugContainer'
var_to_string['ztContainer'] = 'ZenTracingContainer'
var_to_string['isContainer'] = 'IceShatterContainer'
var_to_string['nmContainer'] = 'NameMemoryContainer'
var_to_string['rrContainer'] = 'RetroRacerContainer'
var_to_string['tsContainer'] = 'TicketSwipeContainer'
var_to_string['csContainer'] = 'ConstructionSiteContainer'

# Extract GameType to variable from switch cases
type_to_var = {}
# Find case GameType.X: followed by [var].style.display = DisplayStyle.Flex;
cases = re.finditer(r'case GameType\.([a-zA-Z0-9_]+):\s*(?://.*?\n)*\s*(?:if\s*\([^)]+\)\s*)?([a-zA-Z0-9_]+)\.style\.display\s*=\s*DisplayStyle\.Flex;', mm + mr1)
for match in cases:
    type_to_var[match.group(1)] = match.group(2)

print("private string GetUxmlName(GameType type)")
print("{")
print("    switch(type)")
print("    {")
for t, v in type_to_var.items():
    if v in var_to_string:
        # We strip "Container" from the name because my extract_all.py or the UI/MiniGames files have it
        # Actually wait, the files in UI/MiniGames have EXACTLY the name of the container, e.g. "AcidRainContainer.uxml"
        # So we should return the exact string!
        val = var_to_string[v].replace('Container', '')
        print(f'        case GameType.{t}: return "{val}";')
print('        default: return type.ToString();')
print("    }")
print("}")


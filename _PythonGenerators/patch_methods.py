import re

target = "/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs"
with open(target, 'r') as f:
    content = f.read()

methods_to_remove = [
    "SetupFormatDashboard", "FormatDashboardRoutine",
    "SetupZenTracing", "ZenTracingRoutine",
    "SetupIceShatter", "IceShatterRoutine",
    "SetupNameMemory", "NameMemoryRoutine",
    "SetupCorporateGolf", "CorporateGolfRoutine",
    "SetupMowGreens", "MowGreensRoutine",
    "SetupWalkDogs", "WalkDogsRoutine",
    "SetupRetroRacer", "RetroRacerRoutine",
    "SetupSkimATM", "SkimATMRoutine",
    "SetupTwoPhaseHeist", "TwoPhaseHeistRoutine"
]

def find_method_bounds(text, start_index):
    # Find the opening brace of the method
    open_brace = text.find('{', start_index)
    if open_brace == -1: return -1, -1
    
    # Simple brace matching
    brace_count = 0
    in_string = False
    in_char = False
    in_line_comment = False
    in_block_comment = False
    
    for i in range(open_brace, len(text)):
        if in_line_comment:
            if text[i] == '\n': in_line_comment = False
            continue
        if in_block_comment:
            if text[i:i+2] == '*/': in_block_comment = False
            continue
        if in_string:
            if text[i] == '\\': continue # skip escaped char
            if text[i] == '"' and text[i-1] != '\\': in_string = False
            continue
        if in_char:
            if text[i] == '\\': continue
            if text[i] == "'" and text[i-1] != '\\': in_char = False
            continue
            
        if text[i:i+2] == '//': in_line_comment = True
        elif text[i:i+2] == '/*': in_block_comment = True
        elif text[i] == '"': in_string = True
        elif text[i] == "'": in_char = True
        elif text[i] == '{': brace_count += 1
        elif text[i] == '}': 
            brace_count -= 1
            if brace_count == 0:
                return start_index, i + 1
    return -1, -1

# Find and remove methods
new_content = content
for method in methods_to_remove:
    # Use regex to find the start of the method declaration
    pattern = r'(private\s+(?:void|IEnumerator|System\.Collections\.IEnumerator)\s+' + method + r'\s*\([^)]*\)\s*)'
    match = re.search(pattern, new_content)
    if match:
        start, end = find_method_bounds(new_content, match.start())
        if start != -1 and end != -1:
            print(f"Removing {method} from {start} to {end}")
            new_content = new_content[:start] + new_content[end:]
        else:
            print(f"Failed to find bounds for {method}")
    else:
        print(f"Method {method} not found")

with open(target, 'w') as f:
    f.write(new_content)

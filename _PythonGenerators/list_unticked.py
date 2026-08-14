import re

with open("Assets/Scripts/2 Managers/MiniGameManager.cs", "r") as f:
    mg_cs = f.read()

# Extract all keys added via AddConfigFull
configs = re.findall(r'AddConfigFull\("([^"]+)"', mg_cs)

with open("Assets/Scripts/2 Managers/GameUIManager.cs", "r") as f:
    gu_cs = f.read()

# Extract the big isCompleted boolean expression
m = re.search(r'bool isCompleted = (.*?);', gu_cs)
if m:
    is_completed_expr = m.group(1)
    
    # We can just check if kvp.Key == "Name" is in there
    unticked = []
    for c in configs:
        if f'kvp.Key == "{c}"' not in is_completed_expr:
            # Maybe the type is checked? 
            # We'd need to know the type for c.
            # Let's just find the type for c
            type_m = re.search(r'AddConfigFull\("' + re.escape(c) + r'".*?type = GameType\.(\w+)', mg_cs)
            if type_m:
                gtype = type_m.group(1)
                if f'kvp.Value.type == GameType.{gtype}' not in is_completed_expr:
                    unticked.append(c)
            else:
                print(f"Could not find type for {c}")

    print("UNTICKED MINIGAMES:")
    for u in unticked:
        print(f"- {u}")
else:
    print("Could not find isCompleted expression")

import re

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'r') as f:
    content = f.read()

# We want to extract the code from line 842 to 945
# Actually let's just find the block of UI bindings in Start().
# It starts around `videoCallContainer = root.Q<...`
# and ends right before `// Footer` or `btnAbort = root.Q<Button>("BtnAbortMinigame");`

match = re.search(r'(\s*// Video Call\s*videoCallContainer = root\.Q<VisualElement>\("VideoCallContainer"\);.*?)(\s*// Footer\s*btnAbort = root\.Q<Button>\("BtnAbortMinigame"\);)', content, re.DOTALL)

if match:
    binding_code = match.group(1)
    
    # We replace `root.Q` with `activeRoot.Q`
    binding_code = binding_code.replace("root.Q", "activeRoot.Q")
    
    # We remove this code from Start()
    new_content = content[:match.start(1)] + match.group(2) + content[match.end(2):]
    
    # Now we insert a new method BindLegacyUI(VisualElement activeRoot)
    method_code = "\n    private void BindLegacyUI(VisualElement activeRoot)\n    {\n" + binding_code + "    }\n"
    
    # Let's insert it before `private void InitializeConfigs()`
    init_match = re.search(r'\s*private void InitializeConfigs\(\)', new_content)
    if init_match:
        new_content = new_content[:init_match.start()] + method_code + new_content[init_match.start():]
        
    with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'w') as f:
        f.write(new_content)
    print("Success")
else:
    print("Failed to find binding block")

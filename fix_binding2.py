import re

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'r') as f:
    content = f.read()

# The start of bindings is `// Containers\n tapContainer = root.Q...`
# And it ends where we already extracted it, which is right before `BindLegacyUI` that I just inserted.
# Actually, let's just find `tapContainer = root.Q` up to the end of Start() or before `BindLegacyUI`.

match = re.search(r'(\s*// Containers\s*tapContainer = root\.Q<VisualElement>\("TapGameContainer"\);.*?)(?=\s*private void BindLegacyUI)', content, re.DOTALL)

if match:
    binding_code = match.group(1)
    
    # Wait, the rest of Start is btnAbort binding. We should only extract the UI container bindings.
    # Let's find exactly what's left in Start() related to `root.Q`.
    # Let's get everything from `// Containers` up to `// Footer`. Wait, `// Footer` is still in Start.
    
    match2 = re.search(r'(\s*// Containers\s*tapContainer = root\.Q<VisualElement>\("TapGameContainer"\);.*?)(\s*// Footer\s*btnAbort = root\.Q<Button>\("BtnAbortMinigame"\);)', content, re.DOTALL)
    
    if match2:
        extracted = match2.group(1)
        extracted = extracted.replace("root.Q", "activeRoot.Q")
        
        # Remove it from Start
        new_content = content[:match2.start(1)] + match2.group(2) + content[match2.end(2):]
        
        # Now prepend `extracted` to the INSIDE of `BindLegacyUI`
        # Find `private void BindLegacyUI(VisualElement activeRoot)\n    {\n`
        bind_match = re.search(r'private void BindLegacyUI\(VisualElement activeRoot\)\s*\{\s*', new_content)
        if bind_match:
            insert_pos = bind_match.end()
            new_content = new_content[:insert_pos] + extracted + new_content[insert_pos:]
            
            with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'w') as f:
                f.write(new_content)
            print("Success")
        else:
            print("Failed to find BindLegacyUI")
    else:
        print("Failed to find block in Start")
else:
    print("Failed")

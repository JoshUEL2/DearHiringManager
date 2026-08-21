import re

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'r') as f:
    content = f.read()

# We want to replace `<variable>.RegisterCallback` with `if (<variable> != null) <variable>.RegisterCallback`
# BUT ONLY inside `BindLegacyUI`.

match = re.search(r'(private void BindLegacyUI.*?)(?=\s*private void InitializeConfigs)', content, re.DOTALL)
if match:
    bind_code = match.group(1)
    
    # regex to find un-null-checked `.RegisterCallback`
    # E.g. `scrubPlateArea.RegisterCallback<`
    # We want to wrap it or just use `?` operator.
    # Wait, `.RegisterCallback` is a method. We can use `?.`
    # `scrubPlateArea?.RegisterCallback<...`
    # Let's just replace `([a-zA-Z0-9_]+)\.RegisterCallback` with `\1?.RegisterCallback`
    # EXCEPT for `activeRoot`, which is guaranteed non-null.
    
    # Let's do it carefully.
    def replacer(m):
        var = m.group(1)
        if var == 'activeRoot':
            return m.group(0) # unchanged
        return f"{var}?.RegisterCallback"
        
    new_bind = re.sub(r'([a-zA-Z0-9_]+(?:\[[^\]]+\])?)\.RegisterCallback', replacer, bind_code)
    
    # There are also other method calls or property accesses on potentially null objects.
    # e.g., `acidInputArea.CapturePointer(ev.pointerId);`
    # `acidInputArea.ReleasePointer`
    # `dcDraggableIcon.style`
    # `saNodes[i].RegisterCallback`
    # Wait, the easiest and SAFEST way is to just wrap the whole `BindLegacyUI` assignments inside null checks?
    # No, they are assignments! `variable = activeRoot.Q<...>` is fine. It returns null if not found.
    # The problem is ONLY when we chain methods on them.
    # Let's look at all `.RegisterCallback` first, which is 99% of the issue.
    
    new_content = content[:match.start(1)] + new_bind + content[match.end(1):]
    with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs', 'w') as f:
        f.write(new_content)
    print("Success")
else:
    print("Failed")

import re

target = "/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs"
with open(target, 'r') as f:
    content = f.read()

def get_class_body(filename):
    with open(filename, 'r') as f:
        text = f.read()
    # Find first '{' after 'class'
    class_idx = text.find('class ')
    if class_idx == -1: return ""
    open_brace = text.find('{', class_idx)
    if open_brace == -1: return ""
    
    # We want everything after open_brace and before the last '}'
    last_brace = text.rfind('}')
    if last_brace == -1: return ""
    
    return text[open_brace+1:last_brace]

b2_body = get_class_body("/Users/josh/.gemini/antigravity/brain/2ca3ffab-02f7-4937-a529-201afa766a14/scratch/BatchB2.cs")
c_body = get_class_body("/Users/josh/.gemini/antigravity/brain/6271e99e-5379-4a85-884c-097a0edb4b0c/scratch/BatchC.cs")

# Find the last brace in MiniGameManager.cs
last_brace_target = content.rfind('}')
if last_brace_target != -1:
    new_content = content[:last_brace_target] + "\n" + b2_body + "\n" + c_body + "\n" + content[last_brace_target:]
    with open(target, 'w') as f:
        f.write(new_content)
    print("Appended methods successfully.")
else:
    print("Could not find the end of MiniGameManager.cs")

import re

with open("/Users/josh/.gemini/antigravity/brain/ac9e6910-eefd-4b71-b807-d14a517efdef/task.md", "r") as f:
    text = f.read()

# check off the modularization task
text = text.replace("[/] Phase 2: UXML Refactoring (Modularization)", "[x] Phase 2: UXML Refactoring (Modularization)")

with open("/Users/josh/.gemini/antigravity/brain/ac9e6910-eefd-4b71-b807-d14a517efdef/task.md", "w") as f:
    f.write(text)


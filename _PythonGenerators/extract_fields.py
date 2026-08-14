import re

def extract_fields(filename):
    with open(filename, "r") as f:
        content = f.read()
    
    fields = []
    # match something like 'private float rrDistanceTimer;'
    for line in content.split('\n'):
        line = line.strip()
        if line.startswith('private') and not line.startswith('private void') and not line.startswith('private IEnumerator') and not line.startswith('private System.Collections'):
            fields.append(line)
    return fields

b2 = extract_fields("/Users/josh/.gemini/antigravity/brain/2ca3ffab-02f7-4937-a529-201afa766a14/scratch/BatchB2.cs")
c = extract_fields("/Users/josh/.gemini/antigravity/brain/6271e99e-5379-4a85-884c-097a0edb4b0c/scratch/BatchC.cs")

all_fields = b2 + c

with open("missing_fields.txt", "w") as f:
    for field in all_fields:
        f.write("    " + field + "\n")

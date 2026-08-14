import sys

with open("Assets/Scripts/2 Managers/MiniGameManager.cs", "r") as f:
    cs = f.read()

bad_loop = """        while (isPlaying)
        {
            timer -= Time.deltaTime;
            
            if (vrArcadeContainer != null) {
                float w = vrArcadeContainer.resolvedStyle.width;
                float h = vrArcadeContainer.resolvedStyle.height;
                if (!float.IsNaN(w) && w > 0) containerW = w;
                if (!float.IsNaN(h) && h > 0) containerH = h;
            }"""

good_loop = """        while (isPlaying)
        {
            timer -= Time.deltaTime;"""

cs = cs.replace(bad_loop, good_loop)

# Now specifically add the size update into VrArcadeRoutine only.
# Find the specific spot in VrArcadeRoutine:
vr_target = """            // Move Ball
            vrBallPos += vrBallVel * Time.deltaTime;"""
vr_replacement = """            if (vrArcadeContainer != null) {
                float w = vrArcadeContainer.resolvedStyle.width;
                float h = vrArcadeContainer.resolvedStyle.height;
                if (!float.IsNaN(w) && w > 0) containerW = w;
                if (!float.IsNaN(h) && h > 0) containerH = h;
            }

            // Move Ball
            vrBallPos += vrBallVel * Time.deltaTime;"""

cs = cs.replace(vr_target, vr_replacement)

with open("Assets/Scripts/2 Managers/MiniGameManager.cs", "w") as f:
    f.write(cs)

print("Fixed the mess")

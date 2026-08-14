import sys

# 1. Update UXML to make text more visible
with open("Assets/UI/MiniGame.uxml", "r") as f:
    uxml = f.read()

uxml = uxml.replace('opacity: 0.2;', 'opacity: 0.5;')
with open("Assets/UI/MiniGame.uxml", "w") as f:
    f.write(uxml)

# 2. Update C#
with open("Assets/Scripts/2 Managers/MiniGameManager.cs", "r") as f:
    cs = f.read()

# Remove the static containerW assignment outside the loop
old_w_h = """        // Assuming container is roughly 400x400 based on standard setup
        float containerW = 400f;
        float containerH = 400f;
        if (vrArcadeContainer != null) {
            float w = vrArcadeContainer.resolvedStyle.width;
            float h = vrArcadeContainer.resolvedStyle.height;
            if (w > 0) containerW = w;
            if (h > 0) containerH = h;
        }"""
cs = cs.replace(old_w_h, "        float containerW = 400f;\n        float containerH = 400f;")

# Put it inside the loop
old_loop_start = """        while (isPlaying)
        {
            timer -= Time.deltaTime;"""

new_loop_start = """        while (isPlaying)
        {
            timer -= Time.deltaTime;
            
            if (vrArcadeContainer != null) {
                float w = vrArcadeContainer.resolvedStyle.width;
                float h = vrArcadeContainer.resolvedStyle.height;
                if (!float.IsNaN(w) && w > 0) containerW = w;
                if (!float.IsNaN(h) && h > 0) containerH = h;
            }"""
cs = cs.replace(old_loop_start, new_loop_start)

# Slow down the ball slightly for mobile playability
cs = cs.replace('vrBallVel = new Vector2(Random.value > 0.5f ? 250f : -250f, 300f);', 'vrBallVel = new Vector2(Random.value > 0.5f ? 150f : -150f, 200f);')

with open("Assets/Scripts/2 Managers/MiniGameManager.cs", "w") as f:
    f.write(cs)

print("Fixed VrArcadePong")

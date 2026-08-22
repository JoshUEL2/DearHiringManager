import re

path = 'Assets/Scripts/2 Managers/MiniGames/CenterDivMiniGame.cs'
with open(path, 'r') as f:
    content = f.read()

# 1. Fix the width of the score text so it physically cannot shrink the container
target_score_init = r'''        scoreText = container.Q<Label>\("CdRoundText"\);'''
replace_score_init = r'''        scoreText = container.Q<Label>("CdRoundText");
        scoreText.style.width = 200; // Fixed width to completely prevent layout narrowing when text changes'''
content = re.sub(target_score_init, replace_score_init, content)

# 2. Remove the glitch mechanics from Round 3 and replace with pure speed
target_round3 = r'''                else
                \{
                    // Round 3: Sine Wave \+ Glitch \(Rubber banding\)
                    float frequency = 3\.5f; // Faster
                    
                    glitchTimer -= dt;
                    if \(glitchTimer <= 0\)
                    \{
                        // Sudden teleport backward in time
                        timeAccumulator -= Random\.Range\(0\.1f, 0\.3f\);
                        glitchTimer = Random\.Range\(0\.4f, 0\.8f\);
                        
                        // Flash red to indicate glitch
                        flashOverlay\.style\.backgroundColor = new StyleColor\(new Color\(0\.9f, 0\.1f, 0\.1f, 0\.2f\)\);
                        manager\.StartCoroutine\(ResetFlash\(\)\);
                    \}
                    
                    movingBoxPos = 50f \+ \(-Mathf\.Cos\(timeAccumulator \* frequency\) \* 50f\);
                \}'''

replace_round3 = r'''                else
                {
                    // Round 3: Pure Speed Easing (No glitching)
                    float frequency = 4.0f; // Extremely fast sine wave
                    movingBoxPos = 50f + (-Mathf.Cos(timeAccumulator * frequency) * 50f);
                }'''
content = re.sub(target_round3, replace_round3, content)

with open(path, 'w') as f:
    f.write(content)


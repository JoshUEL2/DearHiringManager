import re

path = 'Assets/Scripts/2 Managers/MiniGames/SizeMatchMiniGame.cs'
with open(path, 'r') as f:
    content = f.read()

# 1. Update tolerance in OnBoxClicked
content = re.sub(
    r'float tolerance = 0\.12f;.*?// Slightly more forgiving due to high speed',
    r'float tolerance = 0.15f; // Forgiving 15% margin as requested',
    content
)

# 2. Update HandleFailure to instant fail instead of time penalty
fail_target = r'''        // Penalty calculation
        timeRemaining -= 1.5f;'''
fail_replacement = r'''        // Instant fail on miss
        floatingPenaltyText.text = "FATAL ERROR!";'''
content = content.replace(fail_target, fail_replacement)

# Remove the resume logic in HandleFailure and replace with EndGame(false)
resume_target = r'''        yield return new WaitForSeconds\(0\.5f\);
        
        // Resume
        changingBox\.style\.backgroundColor = new StyleColor\(new Color\(0\.96f, 0\.62f, 0\.04f, 0\.8f\)\);
        cssTooltip\.style\.color = new StyleColor\(new Color\(0\.96f, 0\.62f, 0\.04f\)\);
        isPaused = false;'''
resume_replacement = r'''        yield return new WaitForSeconds(1.0f);
        manager.EndGame(false);'''
content = re.sub(resume_target, resume_replacement, content, flags=re.DOTALL)

# 3. Update the progressive difficulty speed and jitter safety zone
speed_target = r'''                baseSpeed = 1\.2f \+ \(score \* 0\.8f\); // Round 1: 1\.2, Round 2: 2\.0, Round 3: 2\.8
                float currentSpeed = baseSpeed;

                // Jitter logic \(simulating CSS breakpoint snapping\)
                if \(score >= 1\) // Only jitter on Round 2 and 3
                {
                    jitterTimer -= dt;
                    if \(jitterTimer <= 0\)
                    {
                        // Sudden snap
                        currentScale \+= Random\.Range\(-0\.15f, 0\.15f\);
                        jitterTimer = Random\.Range\(0\.2f, 0\.6f - \(score \* 0\.1f\)\); 
                        
                        // Flash tooltip to simulate a layout recalculation
                        cssTooltip\.style\.backgroundColor = new StyleColor\(new Color\(0\.9f, 0\.9f, 0\.9f, 0\.9f\)\);
                        manager\.StartCoroutine\(ResetTooltipColor\(\)\);
                    }
                }'''

speed_replacement = r'''                // Progressive Difficulty (Balanced)
                baseSpeed = 0.8f + (score * 0.4f); // Round 1: 0.8, Round 2: 1.2, Round 3: 1.6
                float currentSpeed = baseSpeed;

                // Jitter logic (simulating CSS breakpoint snapping)
                if (score >= 1) // Only jitter on Round 2 and 3
                {
                    jitterTimer -= dt;
                    if (jitterTimer <= 0)
                    {
                        // ONLY jitter if we are outside the "safe zone" so we don't rob the player of a perfect hit
                        if (Mathf.Abs(currentScale - 1.0f) > 0.35f)
                        {
                            // Sudden snap
                            currentScale += Random.Range(-0.15f, 0.15f);
                            jitterTimer = Random.Range(0.2f, 0.6f - (score * 0.1f)); 
                            
                            // Flash tooltip to simulate a layout recalculation
                            cssTooltip.style.backgroundColor = new StyleColor(new Color(0.9f, 0.9f, 0.9f, 0.9f));
                            manager.StartCoroutine(ResetTooltipColor());
                        }
                    }
                }'''
content = content.replace(speed_target, speed_replacement)

with open(path, 'w') as f:
    f.write(content)


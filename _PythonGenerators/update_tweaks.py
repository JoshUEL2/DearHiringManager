import sys

with open("Assets/Scripts/2 Managers/MiniGameManager.cs", "r") as f:
    cs = f.read()

# 1. Update variables
vars_old = """    // Forge Pass
    private UnityEngine.UIElements.VisualElement fpContainer;
    private UnityEngine.UIElements.VisualElement fpMovingPass;
    private UnityEngine.UIElements.Button fpStampBtn;
    private float fpPassPos = -200f;"""

vars_new = """    // Forge Pass
    private UnityEngine.UIElements.VisualElement fpContainer;
    private UnityEngine.UIElements.VisualElement fpMovingPass;
    private UnityEngine.UIElements.Button fpStampBtn;
    private float fpPassPos = -200f;
    private int fpAttempts = 0;
    private float fpSpeed = 600f;"""
cs = cs.replace(vars_old, vars_new)


# 2. Update ForgePass Setup
fp_setup_old = """    private void SetupForgePass()
    {
        timer = currentConfig.timeLimit;
        fpPassPos = -200f;
        if (fpMovingPass != null) fpMovingPass.style.left = fpPassPos;
        activeGameRoutine = StartCoroutine(ForgePassRoutine());
    }"""
fp_setup_new = """    private void SetupForgePass()
    {
        timer = currentConfig.timeLimit;
        fpAttempts = 0;
        fpSpeed = 600f;
        fpPassPos = -200f;
        if (fpStampBtn != null) fpStampBtn.text = "STAMP NOW! (3 LIVES)";
        if (fpMovingPass != null) fpMovingPass.style.left = fpPassPos;
        activeGameRoutine = StartCoroutine(ForgePassRoutine());
    }"""
cs = cs.replace(fp_setup_old, fp_setup_new)

# 3. Update ForgePass Stamp Logic
fp_stamp_old = """    private void OnFpStampClicked()
    {
        if (!isPlaying) return;
        
        // Stamp zone is at center (left: 50% = approx 200px if container is 400px wide, but we use percentages for responsive).
        // Let's rely on world space centers.
        
        UnityEngine.Vector2 passCenter = fpMovingPass.worldBound.center;
        UnityEngine.UIElements.VisualElement stampZone = fpContainer.Q<UnityEngine.UIElements.VisualElement>("FpStampZone");
        UnityEngine.Vector2 stampCenter = stampZone.worldBound.center;
        
        float dist = UnityEngine.Vector2.Distance(passCenter, stampCenter);
        if (dist < 30f) {
            // Success!
            fpMovingPass.style.backgroundColor = new UnityEngine.UIElements.StyleColor(new UnityEngine.Color(0.18f, 0.8f, 0.44f));
            MarkWinReady();
        } else {
            // Fail! (Illegal, will trigger prison)
            fpMovingPass.style.backgroundColor = new UnityEngine.UIElements.StyleColor(new UnityEngine.Color(0.9f, 0.3f, 0.23f));
            AbortGame();
        }
    }"""
fp_stamp_new = """    private void OnFpStampClicked()
    {
        if (!isPlaying) return;
        
        UnityEngine.Vector2 passCenter = fpMovingPass.worldBound.center;
        UnityEngine.UIElements.VisualElement stampZone = fpContainer.Q<UnityEngine.UIElements.VisualElement>("FpStampZone");
        UnityEngine.Vector2 stampCenter = stampZone.worldBound.center;
        
        float dist = UnityEngine.Vector2.Distance(passCenter, stampCenter);
        if (dist < 30f) {
            fpMovingPass.style.backgroundColor = new UnityEngine.UIElements.StyleColor(new UnityEngine.Color(0.18f, 0.8f, 0.44f));
            MarkWinReady();
        } else {
            fpAttempts++;
            if (fpAttempts >= 3) {
                fpMovingPass.style.backgroundColor = new UnityEngine.UIElements.StyleColor(new UnityEngine.Color(0.9f, 0.3f, 0.23f));
                AbortGame();
            } else {
                fpSpeed += 300f; // Speed up!
                fpPassPos = -200f;
                fpStampBtn.text = $"STAMP NOW! ({3 - fpAttempts} LIVES)";
            }
        }
    }"""
cs = cs.replace(fp_stamp_old, fp_stamp_new)

# 4. Update ForgePass Routine
fp_routine_old = """    private System.Collections.IEnumerator ForgePassRoutine()
    {
        isPlaying = true;
        float speed = 600f; // fast!
        
        while (isPlaying)
        {
            timer -= UnityEngine.Time.deltaTime;
            
            // Use resolvedStyle.width of container? Wait, easier to use raw pixels.
            // Move left to right.
            fpPassPos += speed * UnityEngine.Time.deltaTime;
            if (fpPassPos > 600f) {
                // Passed the screen entirely. Fail.
                AbortGame();
                yield break;
            }
            
            if (fpMovingPass != null) {
                fpMovingPass.style.left = fpPassPos;
            }

            if (timer <= 0f) {
                AbortGame();
                yield break;
            }
            yield return null;
        }
    }"""
fp_routine_new = """    private System.Collections.IEnumerator ForgePassRoutine()
    {
        isPlaying = true;
        while (isPlaying)
        {
            timer -= UnityEngine.Time.deltaTime;
            
            fpPassPos += fpSpeed * UnityEngine.Time.deltaTime;
            if (fpPassPos > 600f) {
                fpAttempts++;
                if (fpAttempts >= 3) {
                    AbortGame();
                    yield break;
                } else {
                    fpSpeed += 300f; // Speed up!
                    fpPassPos = -200f;
                    if (fpStampBtn != null) fpStampBtn.text = $"STAMP NOW! ({3 - fpAttempts} LIVES)";
                }
            }
            
            if (fpMovingPass != null) {
                fpMovingPass.style.left = fpPassPos;
            }

            if (timer <= 0f) {
                AbortGame();
                yield break;
            }
            yield return null;
        }
    }"""
cs = cs.replace(fp_routine_old, fp_routine_new)

# 5. Update Briefcase (Crack) Routine
sb_routine_old = """    private System.Collections.IEnumerator SnatchBriefcaseRoutine()
    {
        isPlaying = true;
        while (isPlaying)
        {
            timer -= UnityEngine.Time.deltaTime;
            sbSpinTimer += UnityEngine.Time.deltaTime;
            
            if (sbSpinTimer > 0.05f) {
                sbSpinTimer = 0f;
                // Spin active dials
                for (int i = sbCurrentDial; i < 3; i++) {
                    if (sbDials[i] != null) {
                        int currentNum = int.Parse(sbDials[i].text);
                        sbDials[i].text = ((currentNum + 1) % 10).ToString();
                    }
                }
            }

            if (timer <= 0f) {
                AbortGame();
                yield break;
            }
            yield return null;
        }
    }"""
sb_routine_new = """    private System.Collections.IEnumerator SnatchBriefcaseRoutine()
    {
        isPlaying = true;
        while (isPlaying)
        {
            timer -= UnityEngine.Time.deltaTime;
            sbSpinTimer += UnityEngine.Time.deltaTime;
            
            // Speed starts at 0.1 (half of old 0.05). Gets faster with each lock cracked.
            float currentSpeed = 0.1f - (sbCurrentDial * 0.035f); 
            if (currentSpeed < 0.02f) currentSpeed = 0.02f;
            
            if (sbSpinTimer > currentSpeed) {
                sbSpinTimer = 0f;
                for (int i = sbCurrentDial; i < 3; i++) {
                    if (sbDials[i] != null) {
                        int currentNum = int.Parse(sbDials[i].text);
                        sbDials[i].text = ((currentNum + 1) % 10).ToString();
                    }
                }
            }

            if (timer <= 0f) {
                AbortGame();
                yield break;
            }
            yield return null;
        }
    }"""
cs = cs.replace(sb_routine_old, sb_routine_new)

with open("Assets/Scripts/2 Managers/MiniGameManager.cs", "w") as f:
    f.write(cs)
print("Updated tweaks!")

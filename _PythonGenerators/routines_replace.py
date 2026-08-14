import sys
import re

with open("Assets/Scripts/2 Managers/MiniGameManager.cs", "r") as f:
    cs = f.read()

# Replace Golf Routine
old_cg = r'// ==========================================\n\s*// CORPORATE GOLF SIM \(Reverse Timing\)\n\s*// ==========================================.*?// ==========================================\n\s*// MOW CORPORATE ASTROTURF \(Grid trace\)'
new_cg = """// ==========================================
    // CORPORATE GOLF SIM (Swing Power)
    // ==========================================
    private void SetupCorporateGolf()
    {
        timer = currentConfig.timeLimit;
        cgPowerLevel = 0f;
        cgPowerDir = 1f;
        cgIsSwinging = false;
        
        if (cgPowerFill != null) cgPowerFill.style.height = UnityEngine.UIElements.Length.Percent(0);
        
        activeGameRoutine = StartCoroutine(CorporateGolfRoutine());
    }

    private void OnCgSwingRelease()
    {
        if (!isPlaying || !cgIsSwinging) return;
        cgIsSwinging = false;
        
        // Check power level (0-100)
        // Ego zone (win) is 30% to 60%.
        if (cgPowerLevel >= 30f && cgPowerLevel <= 60f) {
            MarkWinReady();
        } else if (cgPowerLevel > 60f) {
            // Hole in one - fail
            AbortGame();
        } else {
            // Too weak - fail
            AbortGame();
        }
    }

    private System.Collections.IEnumerator CorporateGolfRoutine()
    {
        isPlaying = true;
        float powerSpeed = 150f; // fill per second
        
        while (isPlaying)
        {
            timer -= UnityEngine.Time.deltaTime;
            
            if (cgIsSwinging) {
                cgPowerLevel += cgPowerDir * powerSpeed * UnityEngine.Time.deltaTime;
                if (cgPowerLevel > 100f) {
                    cgPowerLevel = 100f;
                    cgPowerDir = -1f;
                } else if (cgPowerLevel < 0f) {
                    cgPowerLevel = 0f;
                    cgPowerDir = 1f;
                }
                
                if (cgPowerFill != null) cgPowerFill.style.height = UnityEngine.UIElements.Length.Percent(cgPowerLevel);
            }

            if (timer <= 0f) {
                AbortGame();
                yield break;
            }
            yield return null;
        }
    }

    // ==========================================
    // MOW CORPORATE ASTROTURF (Freeform Drag)"""
cs = re.sub(old_cg, new_cg, cs, flags=re.DOTALL)

# Replace Mow Greens Routine
old_mg = r'// ==========================================\n\s*// MOW CORPORATE ASTROTURF \(Freeform Drag\)\n\s*// ==========================================.*?// ==========================================\n\s*// WALK ROBOTIC DOGS \(Sort puzzle\)'
new_mg = """// ==========================================
    // MOW CORPORATE ASTROTURF (Freeform Drag)
    // ==========================================
    private void SetupMowGreens()
    {
        timer = currentConfig.timeLimit;
        mgCutCount = 0;
        
        for (int i=0; i<25; i++) {
            mgCut[i] = false;
            if (i < mgPatches.Count) mgPatches[i].style.backgroundColor = new UnityEngine.UIElements.StyleColor(new UnityEngine.Color(0.2f, 0.7f, 0.2f));
        }
        
        activeGameRoutine = StartCoroutine(MowGreensRoutine());
    }

    private void OnMgPointerMove(UnityEngine.UIElements.EventBase ev)
    {
        if (!isPlaying) return;
        
        // Support both PointerDown and PointerMove
        UnityEngine.Vector2 localPos = UnityEngine.Vector2.zero;
        if (ev is UnityEngine.UIElements.PointerMoveEvent pme) {
            // Must hold left click down
            if (pme.pressedButtons != 1 && pme.pointerType != "touch") return;
            localPos = pme.localPosition;
        } else if (ev is UnityEngine.UIElements.PointerDownEvent pde) {
            localPos = pde.localPosition;
        } else {
            return;
        }
        
        for (int i=0; i<25; i++) {
            if (!mgCut[i] && i < mgPatches.Count) {
                // If localPos is inside the patch bounds
                if (mgPatches[i].layout.Contains(localPos)) {
                    mgCut[i] = true;
                    mgCutCount++;
                    mgPatches[i].style.backgroundColor = new UnityEngine.UIElements.StyleColor(new UnityEngine.Color(0.5f, 0.8f, 0.5f)); // short grass
                }
            }
        }
        
        if (mgCutCount >= 23) { // 90%+
            MarkWinReady();
        }
    }

    private System.Collections.IEnumerator MowGreensRoutine()
    {
        isPlaying = true;
        while (isPlaying)
        {
            timer -= UnityEngine.Time.deltaTime;
            if (timer <= 0f) {
                AbortGame();
                yield break;
            }
            yield return null;
        }
    }

    // ==========================================
    // WALK ROBOTIC DOGS (Tension)"""
cs = re.sub(old_mg, new_mg, cs, flags=re.DOTALL)

# Replace Walk Dogs Routine
old_wd = r'// ==========================================\n\s*// WALK ROBOTIC DOGS \(Tension\)\n\s*// ==========================================.*?// ==========================================\n\s*// FORGE PASS \(ILLEGAL\)'
new_wd = """// ==========================================
    // WALK ROBOTIC DOGS (Tension Management)
    // ==========================================
    private void SetupWalkDogs()
    {
        timer = currentConfig.timeLimit;
        
        for (int i=0; i<3; i++) {
            wdTension[i] = 0f;
            if (wdLeashes[i] != null) wdLeashes[i].style.height = UnityEngine.UIElements.Length.Percent(0);
        }
        
        activeGameRoutine = StartCoroutine(WalkDogsRoutine());
    }

    private void OnWdYank(int idx)
    {
        if (!isPlaying) return;
        wdTension[idx] -= 30f;
        if (wdTension[idx] < 0f) wdTension[idx] = 0f;
    }

    private System.Collections.IEnumerator WalkDogsRoutine()
    {
        isPlaying = true;
        float pullSpeed = 40f; // tension per sec
        
        while (isPlaying)
        {
            timer -= UnityEngine.Time.deltaTime;
            
            for (int i=0; i<3; i++) {
                wdTension[i] += pullSpeed * UnityEngine.Time.deltaTime;
                
                // Add some random surges
                if (UnityEngine.Random.value < 0.02f) wdTension[i] += 10f;
                
                if (wdTension[i] > 100f) {
                    wdTension[i] = 100f;
                    // Leash snapped!
                    AbortGame();
                    yield break;
                }
                
                if (wdLeashes[i] != null) wdLeashes[i].style.height = UnityEngine.UIElements.Length.Percent(wdTension[i]);
            }

            if (timer <= 0f) {
                MarkWinReady(); // Survived!
                yield break;
            }
            yield return null;
        }
    }

    // ==========================================
    // FORGE PASS (ILLEGAL)"""
cs = re.sub(old_wd, new_wd, cs, flags=re.DOTALL)

# Inject Recycle Bottles Routine right before FORGE PASS
rb_logic = """// ==========================================
    // RECYCLE BOTTLES (Bag Tear)
    // ==========================================
    private void SetupRecycleBottles()
    {
        timer = currentConfig.timeLimit;
        rbSwipes = 0;
        rbCollected = 0;
        rbBagOpen = false;
        
        if (rbBag != null) {
            rbBag.style.display = DisplayStyle.Flex;
            rbBag.style.backgroundColor = new UnityEngine.UIElements.StyleColor(new UnityEngine.Color(0.06f, 0.09f, 0.15f)); // Dark
        }
        if (rbItemsContainer != null) rbItemsContainer.style.display = DisplayStyle.None;
        if (rbInstructionText != null) rbInstructionText.text = "RIP OPEN THE BAG! (SWIPE 3 TIMES)";
        
        for (int i=0; i<6; i++) {
            if (rbItems[i] != null) rbItems[i].style.display = DisplayStyle.Flex;
        }
        
        activeGameRoutine = StartCoroutine(RecycleBottlesRoutine());
    }

    private void OnRbSwipe()
    {
        if (!isPlaying || rbBagOpen) return;
        
        rbSwipes++;
        // Visual feedback for tear
        if (rbBag != null) {
            if (rbSwipes == 1) rbBag.style.backgroundColor = new UnityEngine.UIElements.StyleColor(new UnityEngine.Color(0.15f, 0.2f, 0.3f));
            if (rbSwipes == 2) rbBag.style.backgroundColor = new UnityEngine.UIElements.StyleColor(new UnityEngine.Color(0.2f, 0.3f, 0.4f));
        }
        
        if (rbSwipes >= 3) {
            rbBagOpen = true;
            if (rbBag != null) rbBag.style.display = DisplayStyle.None;
            if (rbItemsContainer != null) rbItemsContainer.style.display = DisplayStyle.Flex;
            if (rbInstructionText != null) rbInstructionText.text = "COLLECT ALL 3 BOTTLES!";
        }
    }

    private void OnRbItemClicked(int idx)
    {
        if (!isPlaying || !rbBagOpen) return;
        
        if (rbItems[idx].style.display == DisplayStyle.None) return;
        
        if (idx < 3) { // 0, 1, 2 are bottles
            rbItems[idx].style.display = DisplayStyle.None;
            rbCollected++;
            if (rbCollected >= 3) {
                MarkWinReady();
            }
        } else {
            // Tapped trash!
            AbortGame();
        }
    }

    private System.Collections.IEnumerator RecycleBottlesRoutine()
    {
        isPlaying = true;
        while (isPlaying)
        {
            timer -= UnityEngine.Time.deltaTime;
            if (timer <= 0f) {
                AbortGame();
                yield break;
            }
            yield return null;
        }
    }

    """
cs = cs.replace("// ==========================================\n    // FORGE PASS (ILLEGAL)", rb_logic + "// ==========================================\n    // FORGE PASS (ILLEGAL)")

with open("Assets/Scripts/2 Managers/MiniGameManager.cs", "w") as f:
    f.write(cs)
print("Updated Routines!")

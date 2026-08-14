import sys

new_routines = """
    // ==========================================
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
        
        if (cgPowerLevel >= 30f && cgPowerLevel <= 60f) {
            MarkWinReady();
        } else if (cgPowerLevel > 60f) {
            AbortGame();
        } else {
            AbortGame();
        }
    }

    private System.Collections.IEnumerator CorporateGolfRoutine()
    {
        isPlaying = true;
        float powerSpeed = 150f;
        
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
        
        UnityEngine.Vector2 localPos = UnityEngine.Vector2.zero;
        if (ev is UnityEngine.UIElements.PointerMoveEvent pme) {
            if (pme.pressedButtons != 1 && pme.pointerType != "touch") return;
            localPos = pme.localPosition;
        } else if (ev is UnityEngine.UIElements.PointerDownEvent pde) {
            localPos = pde.localPosition;
        } else {
            return;
        }
        
        for (int i=0; i<25; i++) {
            if (!mgCut[i] && i < mgPatches.Count) {
                if (mgPatches[i].layout.Contains(localPos)) {
                    mgCut[i] = true;
                    mgCutCount++;
                    mgPatches[i].style.backgroundColor = new UnityEngine.UIElements.StyleColor(new UnityEngine.Color(0.5f, 0.8f, 0.5f));
                }
            }
        }
        
        if (mgCutCount >= 23) {
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
        float pullSpeed = 40f;
        
        while (isPlaying)
        {
            timer -= UnityEngine.Time.deltaTime;
            
            for (int i=0; i<3; i++) {
                wdTension[i] += pullSpeed * UnityEngine.Time.deltaTime;
                
                if (UnityEngine.Random.value < 0.02f) wdTension[i] += 10f;
                
                if (wdTension[i] > 100f) {
                    wdTension[i] = 100f;
                    AbortGame();
                    yield break;
                }
                
                if (wdLeashes[i] != null) wdLeashes[i].style.height = UnityEngine.UIElements.Length.Percent(wdTension[i]);
            }

            if (timer <= 0f) {
                MarkWinReady();
                yield break;
            }
            yield return null;
        }
    }

    // ==========================================
    // RECYCLE BOTTLES (Bag Tear)
    // ==========================================
    private void SetupRecycleBottles()
    {
        timer = currentConfig.timeLimit;
        rbSwipes = 0;
        rbCollected = 0;
        rbBagOpen = false;
        
        if (rbBag != null) {
            rbBag.style.display = UnityEngine.UIElements.DisplayStyle.Flex;
            rbBag.style.backgroundColor = new UnityEngine.UIElements.StyleColor(new UnityEngine.Color(0.06f, 0.09f, 0.15f));
        }
        if (rbItemsContainer != null) rbItemsContainer.style.display = UnityEngine.UIElements.DisplayStyle.None;
        if (rbInstructionText != null) rbInstructionText.text = "RIP OPEN THE BAG! (SWIPE 3 TIMES)";
        
        for (int i=0; i<6; i++) {
            if (rbItems[i] != null) rbItems[i].style.display = UnityEngine.UIElements.DisplayStyle.Flex;
        }
        
        activeGameRoutine = StartCoroutine(RecycleBottlesRoutine());
    }

    private void OnRbSwipe()
    {
        if (!isPlaying || rbBagOpen) return;
        
        rbSwipes++;
        if (rbBag != null) {
            if (rbSwipes == 1) rbBag.style.backgroundColor = new UnityEngine.UIElements.StyleColor(new UnityEngine.Color(0.15f, 0.2f, 0.3f));
            if (rbSwipes == 2) rbBag.style.backgroundColor = new UnityEngine.UIElements.StyleColor(new UnityEngine.Color(0.2f, 0.3f, 0.4f));
        }
        
        if (rbSwipes >= 3) {
            rbBagOpen = true;
            if (rbBag != null) rbBag.style.display = UnityEngine.UIElements.DisplayStyle.None;
            if (rbItemsContainer != null) rbItemsContainer.style.display = UnityEngine.UIElements.DisplayStyle.Flex;
            if (rbInstructionText != null) rbInstructionText.text = "COLLECT ALL 3 BOTTLES!";
        }
    }

    private void OnRbItemClicked(int idx)
    {
        if (!isPlaying || !rbBagOpen) return;
        
        if (rbItems[idx].style.display == UnityEngine.UIElements.DisplayStyle.None) return;
        
        if (idx < 3) {
            rbItems[idx].style.display = UnityEngine.UIElements.DisplayStyle.None;
            rbCollected++;
            if (rbCollected >= 3) {
                MarkWinReady();
            }
        } else {
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

with open("Assets/Scripts/2 Managers/MiniGameManager.cs", "r") as f:
    lines = f.readlines()

# Find the last closing brace in the file
last_brace_idx = -1
for i in range(len(lines)-1, -1, -1):
    if '}' in lines[i]:
        last_brace_idx = i
        break

if last_brace_idx != -1:
    lines.insert(last_brace_idx, new_routines + "\n")

with open("Assets/Scripts/2 Managers/MiniGameManager.cs", "w") as f:
    f.writelines(lines)

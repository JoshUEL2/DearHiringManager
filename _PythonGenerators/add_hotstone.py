import sys

with open("Assets/Scripts/2 Managers/MiniGameManager.cs", "r") as f:
    cs = f.read()

# 1. Add Variables
vars_anchor = "// Yes Man Nod"
vars_inject = """    // Volcanic Hot Stone Spa
    private UnityEngine.UIElements.VisualElement hotStoneContainer;
    private UnityEngine.UIElements.VisualElement[] hsSlots = new UnityEngine.UIElements.VisualElement[5];
    private UnityEngine.UIElements.VisualElement[] hsStones = new UnityEngine.UIElements.VisualElement[5];
    private UnityEngine.UIElements.Label hsTimerText;
    private bool[] hsStonePlaced = new bool[5];
    private int hsActiveStoneIndex = -1;
    private UnityEngine.Vector2 hsPointerStartPos;
    private UnityEngine.Vector2 hsStoneStartPos;

    // Yes Man Nod"""
cs = cs.replace(vars_anchor, vars_inject)

# 2. Add Awake logic
awake_anchor = 'ymContainer = root.Q<VisualElement>("YesManContainer");'
awake_inject = """ymContainer = root.Q<VisualElement>("YesManContainer");
        
        hotStoneContainer = root.Q<VisualElement>("HotStoneContainer");
        hsTimerText = root.Q<Label>("HsTimerText");
        for (int i = 0; i < 5; i++) {
            hsSlots[i] = root.Q<VisualElement>($"HsSlot{i}");
            hsStones[i] = root.Q<VisualElement>($"HsStone{i}");
            int stoneIdx = i; // capture
            if (hsStones[i] != null) {
                hsStones[i].RegisterCallback<UnityEngine.UIElements.PointerDownEvent>(ev => OnHsPointerDown(ev, stoneIdx));
                hsStones[i].RegisterCallback<UnityEngine.UIElements.PointerMoveEvent>(ev => OnHsPointerMove(ev, stoneIdx));
                hsStones[i].RegisterCallback<UnityEngine.UIElements.PointerUpEvent>(ev => OnHsPointerUp(ev, stoneIdx));
                hsStones[i].RegisterCallback<UnityEngine.UIElements.PointerCaptureOutEvent>(ev => OnHsPointerUp(ev, stoneIdx));
            }
        }"""
cs = cs.replace(awake_anchor, awake_inject)

# 3. Add HideAllContainers logic
hide_anchor = "if (ymContainer != null) ymContainer.style.display = DisplayStyle.None;"
hide_inject = """if (ymContainer != null) ymContainer.style.display = DisplayStyle.None;
        if (hotStoneContainer != null) hotStoneContainer.style.display = DisplayStyle.None;"""
cs = cs.replace(hide_anchor, hide_inject)

# 4. Add SetupGameType logic
setup_anchor = """            case GameType.VrArcadePong:
                if (vrArcadeContainer != null) vrArcadeContainer.style.display = DisplayStyle.Flex;
                SetupVrArcadePong();
                break;"""
setup_inject = """            case GameType.VrArcadePong:
                if (vrArcadeContainer != null) vrArcadeContainer.style.display = DisplayStyle.Flex;
                SetupVrArcadePong();
                break;
            case GameType.HotStoneSpa:
                if (hotStoneContainer != null) hotStoneContainer.style.display = DisplayStyle.Flex;
                SetupHotStoneSpa();
                break;"""
cs = cs.replace(setup_anchor, setup_inject)

# 5. Add Methods
methods_anchor = "private void SetupVrArcadePong()"
methods_inject = """
    private void OnHsPointerDown(UnityEngine.UIElements.PointerDownEvent ev, int stoneIdx)
    {
        if (!isPlaying || hsStonePlaced[stoneIdx]) return;
        hsActiveStoneIndex = stoneIdx;
        hsPointerStartPos = ev.position;
        float currentLeft = hsStones[stoneIdx].resolvedStyle.left;
        float currentTop = hsStones[stoneIdx].resolvedStyle.top;
        if (float.IsNaN(currentLeft)) currentLeft = 0;
        if (float.IsNaN(currentTop)) currentTop = 0;
        
        hsStoneStartPos = new UnityEngine.Vector2(currentLeft, currentTop);
        hsStones[stoneIdx].CapturePointer(ev.pointerId);
    }

    private void OnHsPointerMove(UnityEngine.UIElements.PointerMoveEvent ev, int stoneIdx)
    {
        if (!isPlaying || hsActiveStoneIndex != stoneIdx || hsStonePlaced[stoneIdx]) return;
        
        UnityEngine.Vector2 delta = (UnityEngine.Vector2)ev.position - hsPointerStartPos;
        float newLeft = hsStoneStartPos.x + delta.x;
        float newTop = hsStoneStartPos.y + delta.y;
        
        hsStones[stoneIdx].style.left = newLeft;
        hsStones[stoneIdx].style.top = newTop;
        hsStones[stoneIdx].style.bottom = UnityEngine.UIElements.StyleKeyword.Auto;
        hsStones[stoneIdx].style.right = UnityEngine.UIElements.StyleKeyword.Auto;
    }

    private void OnHsPointerUp(UnityEngine.UIElements.EventBase ev, int stoneIdx)
    {
        if (hsActiveStoneIndex != stoneIdx) return;
        hsActiveStoneIndex = -1;
        
        if (ev is UnityEngine.UIElements.IPointerEvent pointerEvent) {
            hsStones[stoneIdx].ReleasePointer(pointerEvent.pointerId);
        }
        
        if (hsStonePlaced[stoneIdx]) return;

        UnityEngine.Vector2 sCenter = hsStones[stoneIdx].worldBound.center;
        
        for (int i = 0; i < 5; i++) {
            if (hsSlots[i] == null) continue;
            
            UnityEngine.Vector2 targetCenter = hsSlots[i].worldBound.center;
            
            if (UnityEngine.Vector2.Distance(sCenter, targetCenter) < 50f) {
                hsStonePlaced[stoneIdx] = true;
                hsStones[stoneIdx].style.backgroundColor = new UnityEngine.UIElements.StyleColor(new UnityEngine.Color(0.82f, 0.33f, 0f)); // #D35400
                // Center the stone roughly in the slot
                hsStones[stoneIdx].style.left = hsStones[stoneIdx].resolvedStyle.left + (targetCenter.x - sCenter.x);
                hsStones[stoneIdx].style.top = hsStones[stoneIdx].resolvedStyle.top + (targetCenter.y - sCenter.y);
                
                CheckHotStoneWin();
                return;
            }
        }
    }

    private void CheckHotStoneWin()
    {
        bool allPlaced = true;
        for (int i = 0; i < 5; i++) {
            if (!hsStonePlaced[i]) {
                allPlaced = false;
                break;
            }
        }
        if (allPlaced) {
            MarkWinReady();
        }
    }

    private void SetupHotStoneSpa()
    {
        timer = currentConfig.timeLimit;
        hsActiveStoneIndex = -1;
        
        for (int i = 0; i < 5; i++) {
            hsStonePlaced[i] = false;
            if (hsStones[i] != null) {
                hsStones[i].style.backgroundColor = new UnityEngine.UIElements.StyleColor(new UnityEngine.Color(0.18f, 0.31f, 0.31f)); // #2F4F4F
                hsStones[i].style.left = UnityEngine.UIElements.StyleKeyword.Auto;
                hsStones[i].style.top = UnityEngine.UIElements.StyleKeyword.Auto;
                hsStones[i].style.right = UnityEngine.UIElements.StyleKeyword.Auto;
                
                hsStones[i].style.bottom = 20f + UnityEngine.Random.Range(-10f, 10f);
                float lx = 40f + (i * 60f) + UnityEngine.Random.Range(-10f, 10f);
                hsStones[i].style.left = lx;
            }
        }

        activeGameRoutine = StartCoroutine(HotStoneRoutine());
    }

    private System.Collections.IEnumerator HotStoneRoutine()
    {
        isPlaying = true;
        while (isPlaying)
        {
            timer -= UnityEngine.Time.deltaTime;
            if (hsTimerText != null) hsTimerText.text = $"{UnityEngine.Mathf.Max(0, timer):F1}s";
            
            if (timer <= 0f) {
                AbortGame();
                yield break;
            }
            yield return null;
        }
    }

    private void SetupVrArcadePong()"""
cs = cs.replace(methods_anchor, methods_inject)

with open("Assets/Scripts/2 Managers/MiniGameManager.cs", "w") as f:
    f.write(cs)

print("Injected Hot Stone Logic")

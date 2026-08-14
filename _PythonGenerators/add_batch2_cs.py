import sys

with open("Assets/Scripts/2 Managers/MiniGameManager.cs", "r") as f:
    cs = f.read()

# 1. Update Enum
old_enum = "CorporateGolf }"
new_enum = "CorporateGolf, MowGreens, WalkDogs, ForgePass }"
cs = cs.replace(old_enum, new_enum)

# 2. Update Configs
cs = cs.replace(
    'AddConfigFull("Mow Corporate Astroturf", new MiniGameConfig { type = GameType.Swipe, instructions = "Mow the astroturf! (Swipe left/right)" });',
    'AddConfigFull("Mow Executive Golf Greens", new MiniGameConfig { type = GameType.MowGreens, timeLimit = 15f, instructions = "Trace the grid!" });'
)
cs = cs.replace(
    'AddConfigFull("Walk Robotic Pedigree Dogs", new MiniGameConfig { type = GameType.Sequence, sequenceLength = 4, instructions = "Follow the leash pattern!" });',
    'AddConfigFull("Walk Purebred Pedigree Dogs", new MiniGameConfig { type = GameType.WalkDogs, timeLimit = 15f, instructions = "Untangle by matching colors!" });'
)
cs = cs.replace(
    'AddConfigFull("Forge Transit Passes", new MiniGameConfig { type = GameType.CodeBreaker, codeSymbols = new string[]{"0","1","2","3","4","5","6","7","8","9"}, codeLength = 4, maxAttempts = 5, instructions = "Forge the ID code!" });',
    'AddConfigFull("Forge Transit Passes", new MiniGameConfig { type = GameType.ForgePass, timeLimit = 10f, instructions = "Stamp exactly in the center!" });'
)

# 2.5 Update Lore
cs = cs.replace(
    '{ "Mow Corporate Astroturf", "I mowed the indoor corporate astroturf without damaging the server racks." },',
    '{ "Mow Executive Golf Greens", "I mowed the executive golf greens exactly to the CEO\'s neurotic standards." },'
)
cs = cs.replace(
    '{ "Walk Robotic Pedigree Dogs", "I untangled the robotic dogs of a mid-level manager." },',
    '{ "Walk Purebred Pedigree Dogs", "I painstakingly untangled the leashes of three aggressive purebreds." },'
)

# 3. Add Variables
vars_anchor = "// Hospitality Gala"
vars_inject = """    // Mow Greens
    private UnityEngine.UIElements.VisualElement mgContainer;
    private UnityEngine.UIElements.VisualElement[] mgSquares = new UnityEngine.UIElements.VisualElement[16];
    private bool[] mgMowed = new bool[16];
    private int mgMowedCount = 0;

    // Walk Dogs
    private UnityEngine.UIElements.VisualElement wdContainer;
    private UnityEngine.UIElements.Button[] wdDogs = new UnityEngine.UIElements.Button[3];
    private UnityEngine.Color[] wdTargetColors = { new UnityEngine.Color(0.93f, 0.26f, 0.26f), new UnityEngine.Color(0.23f, 0.5f, 0.96f), new UnityEngine.Color(0.06f, 0.72f, 0.5f) };
    private UnityEngine.Color[] wdCurrentColors = new UnityEngine.Color[3];
    private int wdSelectedDog = -1;

    // Forge Pass
    private UnityEngine.UIElements.VisualElement fpContainer;
    private UnityEngine.UIElements.VisualElement fpMovingPass;
    private UnityEngine.UIElements.Button fpStampBtn;
    private float fpPassPos = -200f;

    // Hospitality Gala"""
cs = cs.replace(vars_anchor, vars_inject)

# 4. Awake Logic
awake_anchor = 'hgContainer = root.Q<VisualElement>("HospitalityGalaContainer");'
awake_inject = """mgContainer = root.Q<VisualElement>("MowGreensContainer");
        for (int i=0; i<16; i++) {
            mgSquares[i] = root.Q<VisualElement>($"MgSq{i}");
            int sqIdx = i; // capture
            if (mgSquares[i] != null) {
                mgSquares[i].RegisterCallback<UnityEngine.UIElements.PointerEnterEvent>(ev => OnMgPointerEnter(sqIdx));
            }
        }

        wdContainer = root.Q<VisualElement>("WalkDogsContainer");
        for (int i=0; i<3; i++) {
            wdDogs[i] = root.Q<Button>($"BtnWdDog{i}");
            int dIdx = i; // capture
            if (wdDogs[i] != null) wdDogs[i].clicked += () => OnWdDogClicked(dIdx);
        }

        fpContainer = root.Q<VisualElement>("ForgePassContainer");
        fpMovingPass = root.Q<VisualElement>("FpMovingPass");
        fpStampBtn = root.Q<Button>("BtnFpStamp");
        if (fpStampBtn != null) fpStampBtn.clicked += () => OnFpStampClicked();

        hgContainer = root.Q<VisualElement>("HospitalityGalaContainer");"""
cs = cs.replace(awake_anchor, awake_inject)

# 5. Hide Containers
hide_anchor = "if (hgContainer != null) hgContainer.style.display = DisplayStyle.None;"
hide_inject = """if (mgContainer != null) mgContainer.style.display = DisplayStyle.None;
        if (wdContainer != null) wdContainer.style.display = DisplayStyle.None;
        if (fpContainer != null) fpContainer.style.display = DisplayStyle.None;
        if (hgContainer != null) hgContainer.style.display = DisplayStyle.None;"""
cs = cs.replace(hide_anchor, hide_inject)

# 6. Setup Switches
setup_anchor = """            case GameType.HospitalityGala:
                if (hgContainer != null) hgContainer.style.display = DisplayStyle.Flex;
                SetupHospitalityGala();
                break;"""
setup_inject = """            case GameType.MowGreens:
                if (mgContainer != null) mgContainer.style.display = DisplayStyle.Flex;
                SetupMowGreens();
                break;
            case GameType.WalkDogs:
                if (wdContainer != null) wdContainer.style.display = DisplayStyle.Flex;
                SetupWalkDogs();
                break;
            case GameType.ForgePass:
                if (fpContainer != null) fpContainer.style.display = DisplayStyle.Flex;
                SetupForgePass();
                break;
            case GameType.HospitalityGala:
                if (hgContainer != null) hgContainer.style.display = DisplayStyle.Flex;
                SetupHospitalityGala();
                break;"""
cs = cs.replace(setup_anchor, setup_inject)

# 7. Add Prison Logic to AbortGame
abort_anchor = """    private void AbortGame()
    {
        isPlaying = false;
        if (activeGameRoutine != null) StopCoroutine(activeGameRoutine);

        overlay.style.display = DisplayStyle.None;"""
abort_inject = """    private void AbortGame()
    {
        isPlaying = false;
        if (activeGameRoutine != null) StopCoroutine(activeGameRoutine);

        overlay.style.display = DisplayStyle.None;

        if (currentGame != null && currentGame.isIllegal)
        {
            UnityEngine.Debug.Log("Failed illegal minigame! Straight to jail.");
            var data = GameManager.Instance.currentData;
            float fine = (float)(data.balance * 0.5); 
            data.balance -= fine;
            data.jailWeeksRemaining = currentGame.jailSentence;
            string logMessage = $"[SYSTEM] {gameLore[currentGame.name]} BUT I WAS CAUGHT! Fined ${fine:F0} and jailed.";
            GameManager.Instance.AddFinancialHistory(logMessage, -fine, 0.0);
            
            uiManager?.CheckJailStatus();
            uiManagerForDashboard?.CheckJailStatus();
            return; // Skip normal UI refresh, jail UI is open
        }
"""
cs = cs.replace(abort_anchor, abort_inject)

# 8. Add Methods
methods_anchor = "private void SetupHospitalityGala()"
methods_inject = """
    // ==========================================
    // MOW GREENS
    // ==========================================
    private void SetupMowGreens()
    {
        timer = currentConfig.timeLimit;
        mgMowedCount = 0;
        for(int i=0; i<16; i++) {
            mgMowed[i] = false;
            if(mgSquares[i] != null) mgSquares[i].style.backgroundColor = new UnityEngine.UIElements.StyleColor(new UnityEngine.Color(0.13f, 0.77f, 0.36f)); // #22C55E
        }
        activeGameRoutine = StartCoroutine(MowGreensRoutine());
    }

    private void OnMgPointerEnter(int idx)
    {
        if (!isPlaying) return;
        
        // Require mouse down? In UI Toolkit, dragging over triggers Enter if pointer is captured or just down.
        // For simplicity, we just trigger it if they hover it while playing.
        
        if (mgMowed[idx]) {
            // Already mowed! Fail.
            AbortGame();
            return;
        }

        mgMowed[idx] = true;
        mgMowedCount++;
        mgSquares[idx].style.backgroundColor = new UnityEngine.UIElements.StyleColor(new UnityEngine.Color(0.08f, 0.39f, 0.2f)); // #166534
        
        if (mgMowedCount >= 16) {
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
    // WALK DOGS
    // ==========================================
    private void SetupWalkDogs()
    {
        timer = currentConfig.timeLimit;
        wdSelectedDog = -1;
        
        // Randomize current colors (shuffle)
        System.Collections.Generic.List<UnityEngine.Color> shuffled = new System.Collections.Generic.List<UnityEngine.Color>(wdTargetColors);
        int n = shuffled.Count;  
        while (n > 1) {  
            n--;  
            int k = UnityEngine.Random.Range(0, n + 1);  
            UnityEngine.Color value = shuffled[k];  
            shuffled[k] = shuffled[n];  
            shuffled[n] = value;  
        }

        for (int i=0; i<3; i++) {
            wdCurrentColors[i] = shuffled[i];
            if (wdDogs[i] != null) {
                wdDogs[i].style.backgroundColor = new UnityEngine.UIElements.StyleColor(wdCurrentColors[i]);
                wdDogs[i].style.borderWidth = 0; // reset selection border
            }
        }
        activeGameRoutine = StartCoroutine(WalkDogsRoutine());
    }

    private void OnWdDogClicked(int idx)
    {
        if (!isPlaying) return;
        
        if (wdSelectedDog == -1) {
            // Select it
            wdSelectedDog = idx;
            wdDogs[idx].style.borderWidth = 5;
            wdDogs[idx].style.borderColor = UnityEngine.Color.white;
        } else {
            // Swap colors
            UnityEngine.Color temp = wdCurrentColors[idx];
            wdCurrentColors[idx] = wdCurrentColors[wdSelectedDog];
            wdCurrentColors[wdSelectedDog] = temp;
            
            // Apply
            wdDogs[idx].style.backgroundColor = new UnityEngine.UIElements.StyleColor(wdCurrentColors[idx]);
            wdDogs[wdSelectedDog].style.backgroundColor = new UnityEngine.UIElements.StyleColor(wdCurrentColors[wdSelectedDog]);
            
            // Deselect
            wdDogs[wdSelectedDog].style.borderWidth = 0;
            wdSelectedDog = -1;
            
            // Check win
            bool win = true;
            for(int i=0; i<3; i++) {
                if (wdCurrentColors[i] != wdTargetColors[i]) win = false;
            }
            if (win) {
                MarkWinReady();
            }
        }
    }

    private System.Collections.IEnumerator WalkDogsRoutine()
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
    // FORGE PASS (ILLEGAL)
    // ==========================================
    private void SetupForgePass()
    {
        timer = currentConfig.timeLimit;
        fpPassPos = -200f;
        if (fpMovingPass != null) fpMovingPass.style.left = fpPassPos;
        activeGameRoutine = StartCoroutine(ForgePassRoutine());
    }

    private void OnFpStampClicked()
    {
        if (!isPlaying) return;
        
        // Stamp zone is at center (left: 50% = approx 200px if container is 400px wide, but we use percentages for responsive).
        // Let's rely on world space centers.
        
        UnityEngine.Vector2 passCenter = fpMovingPass.worldBound.center;
        UnityEngine.UIElements.VisualElement stampZone = root.Q<UnityEngine.UIElements.VisualElement>("FpStampZone");
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
    }

    private System.Collections.IEnumerator ForgePassRoutine()
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
    }

    private void SetupHospitalityGala()"""
cs = cs.replace(methods_anchor, methods_inject)

with open("Assets/Scripts/2 Managers/MiniGameManager.cs", "w") as f:
    f.write(cs)

print("Injected Batch 2 C#")

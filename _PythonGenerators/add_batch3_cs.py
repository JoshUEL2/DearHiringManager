import sys

with open("Assets/Scripts/2 Managers/MiniGameManager.cs", "r") as f:
    cs = f.read()

# 1. Enum
old_enum = "ForgePass }"
new_enum = "ForgePass, SnatchBriefcase, SkimATM }"
cs = cs.replace(old_enum, new_enum)

# 2. Add phase3Type to config
config_anchor = "public GameType phase2Type;"
config_inject = """public GameType phase2Type;
    public GameType phase3Type;"""
cs = cs.replace(config_anchor, config_inject)

# 3. Update Configs
cs = cs.replace(
    'AddConfigFull("Snatch Luxury Briefcase", new MiniGameConfig { type = GameType.TimingRing, timingRounds = 3, timingZoneWidth = 15f, instructions = "Time your snatch!" });',
    'AddConfigFull("Snatch Luxury Briefcase", new MiniGameConfig { type = GameType.SnatchBriefcase, timeLimit = 15f, instructions = "Crack the lock!" });'
)
cs = cs.replace(
    'AddConfigFull("ATM Autonomous Hijack", new MiniGameConfig { type = GameType.CodeBreaker, codeSymbols = new string[]{"0","1","2","3","4","5","6","7","8","9","A","B","C","D","E","F"}, codeLength = 4, maxAttempts = 6, instructions = "Inject the ATM payload!" });',
    'AddConfigFull("Skim ATM Card Data", new MiniGameConfig { type = GameType.SkimATM, timeLimit = 10f, instructions = "Connect the skimmer wires!" });'
)
cs = cs.replace(
    'AddConfigFull("Corporate Espionage Heist", new MiniGameConfig { type = GameType.TwoPhaseHeist, phase1Type = GameType.CodeBreaker, codeSymbols = new string[]{"0","1","2","3","4","5","6","7","8","9"}, codeLength = 4, maxAttempts = 6, sprintObstacles = 10, sprintSpeed = 1.2f, phase2Type = GameType.SprintLane, instructions = "Phase 1: Bypass! Phase 2: Escape!" });',
    'AddConfigFull("Corporate Espionage Heist", new MiniGameConfig { type = GameType.TwoPhaseHeist, phase1Type = GameType.SnatchBriefcase, phase2Type = GameType.CodeBreaker, phase3Type = GameType.SprintLane, codeSymbols = new string[]{"0","1","2","3","4","5","6","7","8","9"}, codeLength = 4, maxAttempts = 6, sprintObstacles = 10, sprintSpeed = 1.2f, instructions = "Phase 1: Break In! Phase 2: Hack! Phase 3: Escape!" });'
)

# 4. Add Variables
vars_anchor = "// Forge Pass"
vars_inject = """    // Snatch Briefcase
    private UnityEngine.UIElements.VisualElement sbContainer;
    private UnityEngine.UIElements.Label[] sbDials = new UnityEngine.UIElements.Label[3];
    private UnityEngine.UIElements.Label sbTargetText;
    private UnityEngine.UIElements.Button sbStopBtn;
    private int[] sbTargetNums = new int[3];
    private int sbCurrentDial = 0;
    private float sbSpinTimer = 0f;

    // Skim ATM
    private UnityEngine.UIElements.VisualElement saContainer;
    private UnityEngine.UIElements.Button[] saLeftNodes = new UnityEngine.UIElements.Button[4];
    private UnityEngine.UIElements.Button[] saRightNodes = new UnityEngine.UIElements.Button[4];
    private int[] saLeftColors = new int[4];
    private int[] saRightColors = new int[4];
    private bool[] saLeftConnected = new bool[4];
    private bool[] saRightConnected = new bool[4];
    private int saSelectedLeft = -1;
    private UnityEngine.Color[] saColors = { UnityEngine.Color.red, UnityEngine.Color.blue, UnityEngine.Color.green, UnityEngine.Color.yellow };

    // Forge Pass"""
cs = cs.replace(vars_anchor, vars_inject)

# 5. Awake Logic
awake_anchor = 'fpContainer = root.Q<VisualElement>("ForgePassContainer");'
awake_inject = """sbContainer = root.Q<VisualElement>("SnatchBriefcaseContainer");
        for(int i=0; i<3; i++) sbDials[i] = root.Q<Label>($"SbDial{i}");
        sbTargetText = root.Q<Label>("SbTargetText");
        sbStopBtn = root.Q<Button>("BtnSbStop");
        if(sbStopBtn != null) sbStopBtn.clicked += () => OnSbStopClicked();

        saContainer = root.Q<VisualElement>("SkimATMContainer");
        for(int i=0; i<4; i++) {
            saLeftNodes[i] = root.Q<Button>($"BtnSaL{i}");
            saRightNodes[i] = root.Q<Button>($"BtnSaR{i}");
            int nIdx = i; // capture
            if(saLeftNodes[i] != null) saLeftNodes[i].clicked += () => OnSaLeftClicked(nIdx);
            if(saRightNodes[i] != null) saRightNodes[i].clicked += () => OnSaRightClicked(nIdx);
        }

        fpContainer = root.Q<VisualElement>("ForgePassContainer");"""
cs = cs.replace(awake_anchor, awake_inject)

# 6. Hide Containers
hide_anchor = "if (fpContainer != null) fpContainer.style.display = DisplayStyle.None;"
hide_inject = """if (sbContainer != null) sbContainer.style.display = DisplayStyle.None;
        if (saContainer != null) saContainer.style.display = DisplayStyle.None;
        if (fpContainer != null) fpContainer.style.display = DisplayStyle.None;"""
cs = cs.replace(hide_anchor, hide_inject)

# 7. Setup Switches
setup_anchor = """            case GameType.ForgePass:
                if (fpContainer != null) fpContainer.style.display = DisplayStyle.Flex;
                SetupForgePass();
                break;"""
setup_inject = """            case GameType.ForgePass:
                if (fpContainer != null) fpContainer.style.display = DisplayStyle.Flex;
                SetupForgePass();
                break;
            case GameType.SnatchBriefcase:
                if (sbContainer != null) sbContainer.style.display = DisplayStyle.Flex;
                SetupSnatchBriefcase();
                break;
            case GameType.SkimATM:
                if (saContainer != null) saContainer.style.display = DisplayStyle.Flex;
                SetupSkimATM();
                break;"""
cs = cs.replace(setup_anchor, setup_inject)

# 8. MarkWinReady TwoPhaseHeist modification
markwin_anchor = """        if (currentConfig.type == GameType.TwoPhaseHeist && currentPhase == 1)
        {
            currentPhase = 2;
            if (instructionsText != null) instructionsText.text = "Phase 2: ESCAPE!";
            SetupGameType(currentConfig.phase2Type);
            return;
        }"""
markwin_inject = """        if (currentConfig.type == GameType.TwoPhaseHeist)
        {
            if (currentPhase == 1) {
                currentPhase = 2;
                if (instructionsText != null) instructionsText.text = "Phase 2: HACK!";
                SetupGameType(currentConfig.phase2Type);
                return;
            } else if (currentPhase == 2 && currentConfig.phase3Type != 0) {
                currentPhase = 3;
                if (instructionsText != null) instructionsText.text = "Phase 3: ESCAPE!";
                SetupGameType(currentConfig.phase3Type);
                return;
            }
        }"""
cs = cs.replace(markwin_anchor, markwin_inject)

# 9. Add Methods
methods_anchor = "private void SetupForgePass()"
methods_inject = """
    // ==========================================
    // SNATCH BRIEFCASE (Dial Spin)
    // ==========================================
    private void SetupSnatchBriefcase()
    {
        timer = currentConfig.timeLimit;
        sbCurrentDial = 0;
        sbSpinTimer = 0f;
        if (sbStopBtn != null) sbStopBtn.text = "STOP DIAL 1";
        
        for (int i=0; i<3; i++) {
            sbTargetNums[i] = UnityEngine.Random.Range(0, 10);
            if (sbDials[i] != null) sbDials[i].style.color = UnityEngine.Color.black;
        }
        if (sbTargetText != null) sbTargetText.text = $"TARGET: {sbTargetNums[0]} - {sbTargetNums[1]} - {sbTargetNums[2]}";
        
        activeGameRoutine = StartCoroutine(SnatchBriefcaseRoutine());
    }

    private void OnSbStopClicked()
    {
        if (!isPlaying || sbCurrentDial >= 3) return;
        
        int currentNum = int.Parse(sbDials[sbCurrentDial].text);
        if (currentNum == sbTargetNums[sbCurrentDial]) {
            sbDials[sbCurrentDial].style.color = new UnityEngine.Color(0.18f, 0.8f, 0.44f); // Green
            sbCurrentDial++;
            if (sbCurrentDial >= 3) {
                sbStopBtn.text = "UNLOCKED!";
                MarkWinReady();
            } else {
                sbStopBtn.text = $"STOP DIAL {sbCurrentDial + 1}";
            }
        } else {
            sbDials[sbCurrentDial].style.color = new UnityEngine.Color(0.9f, 0.3f, 0.23f); // Red
            sbStopBtn.text = "WRONG PIN!";
            AbortGame();
        }
    }

    private System.Collections.IEnumerator SnatchBriefcaseRoutine()
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
    }

    // ==========================================
    // SKIM ATM (Wire Connect)
    // ==========================================
    private void SetupSkimATM()
    {
        timer = currentConfig.timeLimit;
        saSelectedLeft = -1;
        
        // Shuffle colors 0-3 for left
        System.Collections.Generic.List<int> leftList = new System.Collections.Generic.List<int>{0, 1, 2, 3};
        System.Collections.Generic.List<int> rightList = new System.Collections.Generic.List<int>{0, 1, 2, 3};
        
        for (int i=0; i<4; i++) {
            int lIdx = UnityEngine.Random.Range(0, leftList.Count);
            saLeftColors[i] = leftList[lIdx];
            leftList.RemoveAt(lIdx);
            
            int rIdx = UnityEngine.Random.Range(0, rightList.Count);
            saRightColors[i] = rightList[rIdx];
            rightList.RemoveAt(rIdx);
            
            saLeftConnected[i] = false;
            saRightConnected[i] = false;
            
            if (saLeftNodes[i] != null) {
                saLeftNodes[i].style.backgroundColor = saColors[saLeftColors[i]];
                saLeftNodes[i].style.opacity = 1f;
                saLeftNodes[i].style.borderWidth = 4;
            }
            if (saRightNodes[i] != null) {
                saRightNodes[i].style.backgroundColor = saColors[saRightColors[i]];
                saRightNodes[i].style.opacity = 1f;
            }
        }
        
        activeGameRoutine = StartCoroutine(SkimATMRoutine());
    }

    private void OnSaLeftClicked(int idx)
    {
        if (!isPlaying || saLeftConnected[idx]) return;
        
        // Reset old selection border
        if (saSelectedLeft != -1) saLeftNodes[saSelectedLeft].style.borderColor = UnityEngine.Color.white;
        
        saSelectedLeft = idx;
        saLeftNodes[idx].style.borderColor = UnityEngine.Color.yellow;
    }

    private void OnSaRightClicked(int idx)
    {
        if (!isPlaying || saRightConnected[idx] || saSelectedLeft == -1) return;
        
        if (saLeftColors[saSelectedLeft] == saRightColors[idx]) {
            // Match!
            saLeftConnected[saSelectedLeft] = true;
            saRightConnected[idx] = true;
            
            saLeftNodes[saSelectedLeft].style.opacity = 0.3f;
            saRightNodes[idx].style.opacity = 0.3f;
            saLeftNodes[saSelectedLeft].style.borderColor = UnityEngine.Color.white;
            
            saSelectedLeft = -1;
            
            // Check win
            bool win = true;
            for(int i=0; i<4; i++) {
                if (!saLeftConnected[i]) win = false;
            }
            if (win) MarkWinReady();
        } else {
            // Wrong connection! Fail
            AbortGame();
        }
    }

    private System.Collections.IEnumerator SkimATMRoutine()
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

    private void SetupForgePass()"""
cs = cs.replace(methods_anchor, methods_inject)

with open("Assets/Scripts/2 Managers/MiniGameManager.cs", "w") as f:
    f.write(cs)

print("Injected Batch 3 C#")

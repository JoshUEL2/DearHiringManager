import sys

with open("Assets/Scripts/2 Managers/MiniGameManager.cs", "r") as f:
    cs = f.read()

# 1. Update Enum
old_enum = "HotStoneSpa }"
new_enum = "HotStoneSpa, HospitalityGala, StartupPitch, CorporateGolf }"
cs = cs.replace(old_enum, new_enum)

# 2. Update Lore
cs = cs.replace(
    '{ "Underground Tech Meetup", "You traded USB drives and rumors at the Underground Tech Meetup." },',
    '{ "Startup Pitch Meetup", "You aggressively pitched your disruptive synergies at the Startup Meetup." },'
)

# 3. Update Configs
cs = cs.replace(
    'AddConfigFull("Hospitality Gala", new MiniGameConfig { type = GameType.Choice, choices = new string[]{"Small Talk", "Talk Business", "Drink Champagne"}, correctChoice = "Small Talk", instructions = "Mingle elegantly." });',
    'AddConfigFull("Hospitality Gala", new MiniGameConfig { type = GameType.HospitalityGala, timeLimit = 10f, instructions = "Balance your champagne!" });'
)
cs = cs.replace(
    'AddConfigFull("Underground Tech Meetup", new MiniGameConfig { type = GameType.CodeBreaker, codeSymbols = new string[]{"0","1","2","3","4","5","6","7","8","9","A","B","C","D","E","F"}, codeLength = 4, maxAttempts = 8, instructions = "Decrypt to mingle!" });',
    'AddConfigFull("Startup Pitch Meetup", new MiniGameConfig { type = GameType.StartupPitch, timeLimit = 15f, instructions = "Pitch the perfect buzzwords!" });'
)
cs = cs.replace(
    'AddConfigFull("Corporate Golf Sim", new MiniGameConfig { type = GameType.TimingRing, timingRounds = 3, timingZoneWidth = 15f, instructions = "Swing precisely!" });',
    'AddConfigFull("Corporate Golf Sim", new MiniGameConfig { type = GameType.CorporateGolf, timeLimit = 10f, instructions = "Miss on purpose!" });'
)

# 4. Add Variables
vars_anchor = "// Volcanic Hot Stone Spa"
vars_inject = """    // Hospitality Gala
    private UnityEngine.UIElements.VisualElement hgContainer;
    private UnityEngine.UIElements.VisualElement hgIndicator;
    private UnityEngine.UIElements.Label hgTimerText;
    private float hgBalance = 0.5f;
    private float hgVelocity = 0f;
    
    // Startup Pitch Meetup
    private UnityEngine.UIElements.VisualElement spContainer;
    private UnityEngine.UIElements.Label[] spSlots = new UnityEngine.UIElements.Label[3];
    private UnityEngine.UIElements.Button spStopBtn;
    private int spCurrentReel = 0;
    private float spSpinTimer = 0f;
    private string[] spGoodWords = { "SYNERGY", "AI", "CLOUD", "WEB3", "SAAS", "AGILE" };
    private string[] spBadWords = { "FRAUD", "BANKRUPT", "SCAM", "PONZI", "LAWSUIT" };

    // Corporate Golf Sim
    private UnityEngine.UIElements.VisualElement cgContainer;
    private UnityEngine.UIElements.VisualElement cgMarker;
    private UnityEngine.UIElements.Button cgSwingBtn;
    private float cgMarkerPos = 0f;
    private float cgMarkerDir = 1f;

    // Volcanic Hot Stone Spa"""
cs = cs.replace(vars_anchor, vars_inject)

# 5. Awake Logic
awake_anchor = 'hotStoneContainer = root.Q<VisualElement>("HotStoneContainer");'
awake_inject = """hgContainer = root.Q<VisualElement>("HospitalityGalaContainer");
        hgIndicator = root.Q<VisualElement>("HgIndicator");
        hgTimerText = root.Q<Label>("HgTimerText");
        if (hgContainer != null) {
            hgContainer.RegisterCallback<UnityEngine.UIElements.PointerMoveEvent>(ev => {
                if (activeGameRoutine != null && currentConfig.type == GameType.HospitalityGala) {
                    float w = hgContainer.resolvedStyle.width;
                    if (w > 0) {
                        float normalizedX = UnityEngine.Mathf.Clamp01(ev.localPosition.x / w);
                        hgVelocity = (normalizedX - 0.5f) * 2f; // -1 to 1 based on touch pos
                    }
                }
            });
        }

        spContainer = root.Q<VisualElement>("StartupPitchContainer");
        for (int i=0; i<3; i++) spSlots[i] = root.Q<Label>($"SpSlot{i}");
        spStopBtn = root.Q<Button>("BtnSpStop");
        if (spStopBtn != null) spStopBtn.clicked += () => OnSpStopClicked();

        cgContainer = root.Q<VisualElement>("CorporateGolfContainer");
        cgMarker = root.Q<VisualElement>("CgMarker");
        cgSwingBtn = root.Q<Button>("BtnCgSwing");
        if (cgSwingBtn != null) cgSwingBtn.clicked += () => OnCgSwingClicked();

        hotStoneContainer = root.Q<VisualElement>("HotStoneContainer");"""
cs = cs.replace(awake_anchor, awake_inject)

# 6. Hide Containers
hide_anchor = "if (hotStoneContainer != null) hotStoneContainer.style.display = DisplayStyle.None;"
hide_inject = """if (hotStoneContainer != null) hotStoneContainer.style.display = DisplayStyle.None;
        if (hgContainer != null) hgContainer.style.display = DisplayStyle.None;
        if (spContainer != null) spContainer.style.display = DisplayStyle.None;
        if (cgContainer != null) cgContainer.style.display = DisplayStyle.None;"""
cs = cs.replace(hide_anchor, hide_inject)

# 7. Setup Switches
setup_anchor = """            case GameType.HotStoneSpa:
                if (hotStoneContainer != null) hotStoneContainer.style.display = DisplayStyle.Flex;
                SetupHotStoneSpa();
                break;"""
setup_inject = """            case GameType.HotStoneSpa:
                if (hotStoneContainer != null) hotStoneContainer.style.display = DisplayStyle.Flex;
                SetupHotStoneSpa();
                break;
            case GameType.HospitalityGala:
                if (hgContainer != null) hgContainer.style.display = DisplayStyle.Flex;
                SetupHospitalityGala();
                break;
            case GameType.StartupPitch:
                if (spContainer != null) spContainer.style.display = DisplayStyle.Flex;
                SetupStartupPitch();
                break;
            case GameType.CorporateGolf:
                if (cgContainer != null) cgContainer.style.display = DisplayStyle.Flex;
                SetupCorporateGolf();
                break;"""
cs = cs.replace(setup_anchor, setup_inject)

# 8. Add Methods
methods_anchor = "private void SetupHotStoneSpa()"
methods_inject = """
    // ==========================================
    // HOSPITALITY GALA
    // ==========================================
    private void SetupHospitalityGala()
    {
        timer = currentConfig.timeLimit;
        hgBalance = 0.5f;
        hgVelocity = 0f;
        activeGameRoutine = StartCoroutine(HospitalityGalaRoutine());
    }

    private System.Collections.IEnumerator HospitalityGalaRoutine()
    {
        isPlaying = true;
        float randomDrift = 0f;
        while (isPlaying)
        {
            timer -= UnityEngine.Time.deltaTime;
            if (hgTimerText != null) hgTimerText.text = $"{UnityEngine.Mathf.Max(0, timer):F1}s";
            
            // Apply drift
            randomDrift += UnityEngine.Random.Range(-2f, 2f) * UnityEngine.Time.deltaTime;
            randomDrift = UnityEngine.Mathf.Clamp(randomDrift, -0.5f, 0.5f);
            
            hgBalance += (hgVelocity + randomDrift) * UnityEngine.Time.deltaTime * 1.5f;
            
            if (hgIndicator != null) {
                hgIndicator.style.left = new UnityEngine.UIElements.StyleLength(new UnityEngine.UIElements.Length(hgBalance * 100f, UnityEngine.UIElements.LengthUnit.Percent));
            }

            if (hgBalance < 0f || hgBalance > 1f) {
                if (hgTimerText != null) hgTimerText.text = "SPILLED!";
                yield return new UnityEngine.WaitForSeconds(1f);
                AbortGame();
                yield break;
            }

            if (timer <= 0f) {
                MarkWinReady();
                yield break;
            }
            yield return null;
        }
    }

    // ==========================================
    // STARTUP PITCH MEETUP
    // ==========================================
    private void SetupStartupPitch()
    {
        timer = currentConfig.timeLimit;
        spCurrentReel = 0;
        spSpinTimer = 0f;
        if (spStopBtn != null) spStopBtn.text = "STOP REEL 1";
        for (int i=0; i<3; i++) {
            if (spSlots[i] != null) {
                spSlots[i].text = spGoodWords[UnityEngine.Random.Range(0, spGoodWords.Length)];
                spSlots[i].style.color = UnityEngine.Color.white;
            }
        }
        activeGameRoutine = StartCoroutine(StartupPitchRoutine());
    }

    private void OnSpStopClicked()
    {
        if (!isPlaying || spCurrentReel >= 3) return;
        
        string word = spSlots[spCurrentReel].text;
        bool isGood = System.Array.Exists(spGoodWords, w => w == word);
        
        if (isGood) {
            spSlots[spCurrentReel].style.color = new UnityEngine.Color(0.18f, 0.8f, 0.44f); // Green
            spCurrentReel++;
            if (spCurrentReel >= 3) {
                spStopBtn.text = "FUNDED!";
                MarkWinReady();
            } else {
                spStopBtn.text = $"STOP REEL {spCurrentReel + 1}";
            }
        } else {
            spSlots[spCurrentReel].style.color = new UnityEngine.Color(0.9f, 0.29f, 0.23f); // Red
            spStopBtn.text = "FAILED PITCH";
            AbortGame();
        }
    }

    private System.Collections.IEnumerator StartupPitchRoutine()
    {
        isPlaying = true;
        while (isPlaying)
        {
            timer -= UnityEngine.Time.deltaTime;
            spSpinTimer += UnityEngine.Time.deltaTime;
            
            if (spSpinTimer > 0.1f) {
                spSpinTimer = 0f;
                // Spin all unstopped reels
                for (int i = spCurrentReel; i < 3; i++) {
                    if (spSlots[i] != null) {
                        bool useGood = UnityEngine.Random.value > 0.3f;
                        spSlots[i].text = useGood ? 
                            spGoodWords[UnityEngine.Random.Range(0, spGoodWords.Length)] : 
                            spBadWords[UnityEngine.Random.Range(0, spBadWords.Length)];
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
    // CORPORATE GOLF SIM
    // ==========================================
    private void SetupCorporateGolf()
    {
        timer = currentConfig.timeLimit;
        cgMarkerPos = 0f;
        cgMarkerDir = 1f;
        if (cgSwingBtn != null) cgSwingBtn.text = "SWING!";
        activeGameRoutine = StartCoroutine(CorporateGolfRoutine());
    }

    private void OnCgSwingClicked()
    {
        if (!isPlaying) return;
        
        // Marker goes 0 to 1
        // Green zone is 0.45 to 0.55
        // Yellow zone is 0.35 to 0.65
        
        if (cgMarkerPos >= 0.45f && cgMarkerPos <= 0.55f) {
            // Hole in one - FAIL
            cgSwingBtn.text = "HOLE IN ONE! CEO ANGRY!";
            AbortGame();
        } else if (cgMarkerPos >= 0.35f && cgMarkerPos <= 0.65f) {
            // Close miss - WIN
            cgSwingBtn.text = "CLOSE MISS! CEO WINS!";
            MarkWinReady();
        } else {
            // Too far - FAIL
            cgSwingBtn.text = "TERRIBLE SHOT! FIRED!";
            AbortGame();
        }
    }

    private System.Collections.IEnumerator CorporateGolfRoutine()
    {
        isPlaying = true;
        float speed = 2.0f;
        while (isPlaying)
        {
            timer -= UnityEngine.Time.deltaTime;
            
            cgMarkerPos += speed * cgMarkerDir * UnityEngine.Time.deltaTime;
            if (cgMarkerPos > 1f) {
                cgMarkerPos = 1f;
                cgMarkerDir = -1f;
            } else if (cgMarkerPos < 0f) {
                cgMarkerPos = 0f;
                cgMarkerDir = 1f;
            }

            if (cgMarker != null) {
                cgMarker.style.left = new UnityEngine.UIElements.StyleLength(new UnityEngine.UIElements.Length(cgMarkerPos * 100f, UnityEngine.UIElements.LengthUnit.Percent));
            }

            if (timer <= 0f) {
                AbortGame();
                yield break;
            }
            yield return null;
        }
    }

    private void SetupHotStoneSpa()"""
cs = cs.replace(methods_anchor, methods_inject)

with open("Assets/Scripts/2 Managers/MiniGameManager.cs", "w") as f:
    f.write(cs)

print("Injected Batch 1 C#")

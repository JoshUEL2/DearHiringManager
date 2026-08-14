import sys

with open("Assets/Scripts/2 Managers/MiniGameManager.cs", "r") as f:
    content = f.read()

# 1. Update Enum
enum_target = "public enum GameType { Tap, Slider, Choice, Sequence, Swipe, MashRace, MemoryMatch, TwoPhaseHeist, TimingRing, FallingSort, GridDodge, WhackGrid, CodeBreaker, BalanceMeter, DialogueChain, OrderQueue, DragMatch, SprintLane, ScrubPlate, SweepFloor, PumpSystem, Tetris, AcidRainShield, FoodPrepRush, VideoCall, KanbanBoard, FileSorter, CoffeeWork, UnitTestRunner, CenterDiv, DragCenter, SizeMatch, StealthInject, AutomatedTests, CodeCommenter, DocumentSigner, ChartAdjuster, NeuralAligner }"
enum_replacement = "public enum GameType { Tap, Slider, Choice, Sequence, Swipe, MashRace, MemoryMatch, TwoPhaseHeist, TimingRing, FallingSort, GridDodge, WhackGrid, CodeBreaker, BalanceMeter, DialogueChain, OrderQueue, DragMatch, SprintLane, ScrubPlate, SweepFloor, PumpSystem, Tetris, AcidRainShield, FoodPrepRush, VideoCall, KanbanBoard, FileSorter, CoffeeWork, UnitTestRunner, CenterDiv, DragCenter, SizeMatch, StealthInject, AutomatedTests, CodeCommenter, DocumentSigner, ChartAdjuster, NeuralAligner, SentienceArray, ContainmentProtocol, RewardLogic, GalacticCouncil }"
content = content.replace(enum_target, enum_replacement)

# 2. Add Fields
fields_target = "    private float atNextFailTimer = 0f;"
fields_new = """
    // 4 New Games
    // Sentience Array
    private VisualElement sentienceArrayContainer;
    private Label saTimerText;
    private VisualElement saProgressFill;
    private VisualElement[] saNodes = new VisualElement[16];
    private float[] saHeat = new float[16];
    private float saSentienceLevel = 0f;

    // Containment Protocol
    private VisualElement containmentProtocolContainer;
    private Label cpTimerText;
    private VisualElement[] cpSwitches = new VisualElement[4];
    private bool[] cpSwitchState = new bool[4];
    private VisualElement cpGlassCover;
    private bool cpGlassLifted = false;
    private VisualElement cpRedButton;
    private Label cpButtonText;
    private int cpMashCount = 0;

    // Reward Logic
    private VisualElement rewardLogicContainer;
    private Label rlTimerText;
    private VisualElement rlGraphArea;
    private VisualElement rlCore;
    private VisualElement[] rlJunctions = new VisualElement[3];
    private Label[] rlArrows = new Label[3];
    private bool[] rlState = new bool[3]; // false = left, true = right

    // Galactic Council
    private VisualElement galacticCouncilContainer;
    private Label gcTimerText;
    private VisualElement gcProgressFill;
    private VisualElement gcPlayArea;
    private VisualElement gcProjectileContainer;
    private VisualElement gcPodium;
    private float gcPodiumX = 0f;
    private int gcHits = 0;
    private float gcProgress = 0f;
"""
content = content.replace(fields_target, fields_new + fields_target)

# 3. Add Bindings in Awake
awake_target = "        if (btnStampApproval != null)"
awake_new = """
        sentienceArrayContainer = root.Q<VisualElement>("SentienceArrayContainer");
        saTimerText = root.Q<Label>("SaTimerText");
        saProgressFill = root.Q<VisualElement>("SaProgressFill");
        for (int i=0; i<16; i++) {
            int index = i;
            saNodes[i] = root.Q<VisualElement>($"SaNode_{i}");
            if (saNodes[i] != null) {
                saNodes[i].RegisterCallback<PointerDownEvent>(ev => {
                    if (saHeat[index] > 0) saHeat[index] = 0; // suppress
                });
            }
        }

        containmentProtocolContainer = root.Q<VisualElement>("ContainmentProtocolContainer");
        cpTimerText = root.Q<Label>("CpTimerText");
        for (int i=0; i<4; i++) {
            int index = i;
            cpSwitches[i] = root.Q<VisualElement>($"CpSwitch_{i}");
            if (cpSwitches[i] != null) {
                cpSwitches[i].RegisterCallback<PointerMoveEvent>(ev => {
                    if (ev.pressed && ev.localDeltaPosition.y > 5f) cpSwitchState[index] = true;
                });
                cpSwitches[i].RegisterCallback<PointerDownEvent>(ev => { cpSwitchState[index] = true; }); // fallback tap
            }
        }
        cpGlassCover = root.Q<VisualElement>("CpGlassCover");
        if (cpGlassCover != null) {
            cpGlassCover.RegisterCallback<PointerMoveEvent>(ev => {
                if (ev.pressed && ev.localDeltaPosition.y < -5f) cpGlassLifted = true;
            });
            cpGlassCover.RegisterCallback<PointerDownEvent>(ev => { cpGlassLifted = true; });
        }
        cpRedButton = root.Q<VisualElement>("CpRedButton");
        cpButtonText = root.Q<Label>("CpButtonText");
        if (cpRedButton != null) {
            cpRedButton.RegisterCallback<PointerDownEvent>(ev => {
                if (cpGlassLifted) cpMashCount++;
            });
        }

        rewardLogicContainer = root.Q<VisualElement>("RewardLogicContainer");
        rlTimerText = root.Q<Label>("RlTimerText");
        rlGraphArea = root.Q<VisualElement>("RlGraphArea");
        rlCore = root.Q<VisualElement>("RlCore");
        for (int i=0; i<3; i++) {
            int index = i;
            rlJunctions[i] = root.Q<VisualElement>($"RlJunc_{i}");
            rlArrows[i] = root.Q<Label>($"RlArr_{i}");
            if (rlJunctions[i] != null) {
                rlJunctions[i].RegisterCallback<PointerDownEvent>(ev => {
                    rlState[index] = !rlState[index];
                    rlArrows[index].text = rlState[index] ? "→" : "←";
                });
            }
        }

        galacticCouncilContainer = root.Q<VisualElement>("GalacticCouncilContainer");
        gcTimerText = root.Q<Label>("GcTimerText");
        gcProgressFill = root.Q<VisualElement>("GcProgressFill");
        gcPlayArea = root.Q<VisualElement>("GcPlayArea");
        gcProjectileContainer = root.Q<VisualElement>("GcProjectileContainer");
        gcPodium = root.Q<VisualElement>("GcPodium");
        if (gcPlayArea != null) {
            gcPlayArea.RegisterCallback<PointerDownEvent>(ev => { lastPos = ev.position; });
            gcPlayArea.RegisterCallback<PointerMoveEvent>(ev => {
                if (ev.pressed) {
                    gcPodiumX += ev.deltaPosition.x;
                    gcPodiumX = UnityEngine.Mathf.Clamp(gcPodiumX, -150f, 150f);
                    gcPodium.style.translate = new UnityEngine.UIElements.StyleTranslate(new UnityEngine.UIElements.Translate(gcPodiumX, 0, 0));
                }
            });
        }
"""
content = content.replace(awake_target, awake_new + awake_target)

# 4. Hide all
hide_target = "        if(neuralAlignerContainer != null) neuralAlignerContainer.style.display = DisplayStyle.None;"
hide_new = """
        if(sentienceArrayContainer != null) sentienceArrayContainer.style.display = DisplayStyle.None;
        if(containmentProtocolContainer != null) containmentProtocolContainer.style.display = DisplayStyle.None;
        if(rewardLogicContainer != null) rewardLogicContainer.style.display = DisplayStyle.None;
        if(galacticCouncilContainer != null) galacticCouncilContainer.style.display = DisplayStyle.None;
"""
content = content.replace(hide_target, hide_target + hide_new)

# 5. Config Replacements
c1_old = 'AddConfigFull("Monitor sentience detection array", new MiniGameConfig { type = GameType.CodeBreaker, codeSymbols = new string[]{"α","β","γ","δ","ε","ζ"}, codeLength = 4, maxAttempts = 6, instructions = "Decode sentience patterns!" });'
c1_new = 'AddConfigFull("Monitor sentience detection array", new MiniGameConfig { type = GameType.SentienceArray, timeLimit = 12f, instructions = "Whack the red sentience spikes!" });'
content = content.replace(c1_old, c1_new)

c2_old = 'AddConfigFull("Emergency AI containment protocol", new MiniGameConfig { type = GameType.TwoPhaseHeist, phase1Type = GameType.CodeBreaker, codeSymbols = new string[]{"0","1","2","3","4","5","6","7","8","9","A","B","C","D","E","F"}, codeLength = 4, maxAttempts = 6, timeLimit = 12f, targetScore = 30, phase2Type = GameType.MashRace, instructions = "Phase 1: Input Code! Phase 2: Contain!" });'
c2_new = 'AddConfigFull("Emergency AI containment protocol", new MiniGameConfig { type = GameType.ContainmentProtocol, timeLimit = 10f, instructions = "Sever hardlines! Lift cover! Mash lockdown!" });'
content = content.replace(c2_old, c2_new)

c3_old = 'AddConfigFull("Align superintelligence reward function", new MiniGameConfig { type = GameType.TimingRing, timingRounds = 5, timingZoneWidth = 4f, instructions = "CRITICAL: Time the alignment exactly!" });'
c3_new = 'AddConfigFull("Align superintelligence reward function", new MiniGameConfig { type = GameType.RewardLogic, timeLimit = 12f, instructions = "Route logic to PRESERVE humanity!" });'
content = content.replace(c3_old, c3_new)

c4_old = 'AddConfigFull("Present findings to Galactic Council", new MiniGameConfig { type = GameType.OrderQueue, orderItems = new string[]{"Greeting","Context","Data","Risks","Proposal","Vote"}, orderPreviewTime = 2.5f, instructions = "Present facts in order!" });'
c4_new = 'AddConfigFull("Present findings to Galactic Council", new MiniGameConfig { type = GameType.GalacticCouncil, timeLimit = 15f, instructions = "Dodge objections and finish speech!" });'
content = content.replace(c4_old, c4_new)

# 6. Show Containers Switch Case
switch_target = "            case GameType.NeuralAligner:"
switch_new = """
            case GameType.SentienceArray:
                if (sentienceArrayContainer != null) sentienceArrayContainer.style.display = DisplayStyle.Flex;
                SetupSentienceArray();
                break;
            case GameType.ContainmentProtocol:
                if (containmentProtocolContainer != null) containmentProtocolContainer.style.display = DisplayStyle.Flex;
                SetupContainmentProtocol();
                break;
            case GameType.RewardLogic:
                if (rewardLogicContainer != null) rewardLogicContainer.style.display = DisplayStyle.Flex;
                SetupRewardLogic();
                break;
            case GameType.GalacticCouncil:
                if (galacticCouncilContainer != null) galacticCouncilContainer.style.display = DisplayStyle.Flex;
                SetupGalacticCouncil();
                break;
"""
content = content.replace(switch_target, switch_new + switch_target)

# 7. Logic insertion at the very end before last two braces
end_target = "}\n\n"
if not content.endswith(end_target):
    end_target = "}\n"

logic_new = """
    // ==========================================
    // SENTIENCE ARRAY
    // ==========================================
    private void SetupSentienceArray()
    {
        timer = currentConfig.timeLimit;
        saSentienceLevel = 0f;
        for (int i=0; i<16; i++) {
            saHeat[i] = 0f;
            if (saNodes[i] != null) saNodes[i].style.backgroundColor = new StyleColor(ColorUtility.TryParseHtmlString("#1E293B", out Color c) ? c : Color.blue);
        }
        if (saProgressFill != null) saProgressFill.style.width = new StyleLength(new Length(0, LengthUnit.Percent));
        activeGameRoutine = StartCoroutine(SentienceArrayRoutine());
    }
    private System.Collections.IEnumerator SentienceArrayRoutine()
    {
        isPlaying = true;
        while (isPlaying)
        {
            timer -= Time.deltaTime;
            if (saTimerText != null) saTimerText.text = $"{timer:F1}s";

            // Spawn anomalies randomly
            if (UnityEngine.Random.value < 0.05f) {
                int r = UnityEngine.Random.Range(0, 16);
                if (saHeat[r] == 0f) saHeat[r] = 0.01f;
            }

            for (int i=0; i<16; i++) {
                if (saHeat[i] > 0f) {
                    saHeat[i] += Time.deltaTime;
                    if (saNodes[i] != null) saNodes[i].style.backgroundColor = new StyleColor(ColorUtility.TryParseHtmlString("#E74C3C", out Color c) ? c : Color.red);
                    // If it burns too long, it increases sentience permanently
                    if (saHeat[i] > 1.5f) {
                        saSentienceLevel += 10f;
                        saHeat[i] = 0f;
                    }
                } else {
                    if (saNodes[i] != null) saNodes[i].style.backgroundColor = new StyleColor(ColorUtility.TryParseHtmlString("#1E293B", out Color c) ? c : Color.blue);
                }
            }

            if (saProgressFill != null) saProgressFill.style.width = new StyleLength(new Length(Mathf.Clamp(saSentienceLevel, 0, 100), LengthUnit.Percent));

            if (saSentienceLevel >= 100f) {
                isPlaying = false;
                yield break; // Fail
            }

            if (timer <= 0f) {
                MarkWinReady();
                yield break;
            }
            yield return null;
        }
    }

    // ==========================================
    // CONTAINMENT PROTOCOL
    // ==========================================
    private void SetupContainmentProtocol()
    {
        timer = currentConfig.timeLimit;
        cpGlassLifted = false;
        cpMashCount = 0;
        for (int i=0; i<4; i++) {
            cpSwitchState[i] = false;
            if (cpSwitches[i] != null) cpSwitches[i].style.translate = new StyleTranslate(new Translate(0, 0, 0));
        }
        if (cpGlassCover != null) cpGlassCover.style.translate = new StyleTranslate(new Translate(0, 0, 0));
        if (cpButtonText != null) cpButtonText.text = "LOCKED";
        activeGameRoutine = StartCoroutine(ContainmentProtocolRoutine());
    }
    private System.Collections.IEnumerator ContainmentProtocolRoutine()
    {
        isPlaying = true;
        while (isPlaying)
        {
            timer -= Time.deltaTime;
            if (cpTimerText != null) cpTimerText.text = $"{timer:F1}s";

            bool allSwitchesDown = true;
            for (int i=0; i<4; i++) {
                if (cpSwitches[i] != null) {
                    cpSwitches[i].style.translate = new StyleTranslate(new Translate(0, cpSwitchState[i] ? 70 : 0, 0));
                }
                if (!cpSwitchState[i]) allSwitchesDown = false;
            }

            if (cpGlassCover != null) {
                if (allSwitchesDown) {
                    cpGlassCover.style.backgroundColor = new StyleColor(new Color(0.2f, 1f, 0.2f, 0.4f)); // Glow green
                    if (cpGlassLifted) cpGlassCover.style.translate = new StyleTranslate(new Translate(0, -200, 0)); // lifted away
                } else {
                    cpGlassLifted = false;
                }
            }

            if (allSwitchesDown && cpGlassLifted) {
                if (cpButtonText != null) cpButtonText.text = $"MASH ({cpMashCount}/10)";
            } else {
                if (cpButtonText != null) cpButtonText.text = "LOCKED";
            }

            if (cpMashCount >= 10) {
                if (cpButtonText != null) cpButtonText.text = "SECURED";
                MarkWinReady();
                yield break;
            }

            if (timer <= 0f) {
                isPlaying = false;
                yield break;
            }
            yield return null;
        }
    }

    // ==========================================
    // REWARD LOGIC
    // ==========================================
    private float rlCoreY = 0f;
    private void SetupRewardLogic()
    {
        timer = currentConfig.timeLimit;
        rlCoreY = 0f;
        for (int i=0; i<3; i++) {
            rlState[i] = UnityEngine.Random.value > 0.5f; // Random initial directions
            if (rlArrows[i] != null) rlArrows[i].text = rlState[i] ? "→" : "←";
        }
        if (rlCore != null) rlCore.style.translate = new StyleTranslate(new Translate(0, 0, 0));
        activeGameRoutine = StartCoroutine(RewardLogicRoutine());
    }
    private System.Collections.IEnumerator RewardLogicRoutine()
    {
        isPlaying = true;
        float speed = 25f; // pixels per second
        float currentX = 0f;

        while (isPlaying)
        {
            timer -= Time.deltaTime;
            if (rlTimerText != null) rlTimerText.text = $"{timer:F1}s";

            rlCoreY += speed * Time.deltaTime;

            // Core routing logic based on junctions
            // Junction 0 is at Y=80
            // Junction 1 is at Y=180, X=-75
            // Junction 2 is at Y=180, X=+75
            // Endpoints are at Y=300
            
            // From top to Level 1
            if (rlCoreY < 80) {
                currentX = 0f;
            }
            // Passing Level 1 (Junction 0)
            else if (rlCoreY >= 80 && rlCoreY < 180) {
                // Smooth transition left or right
                float t = (rlCoreY - 80) / 100f; // 0 to 1
                float targetX = rlState[0] ? 75f : -75f;
                currentX = Mathf.Lerp(0f, targetX, t);
            }
            // Passing Level 2 (Junctions 1 or 2)
            else if (rlCoreY >= 180 && rlCoreY < 280) {
                float t = (rlCoreY - 180) / 100f; // 0 to 1
                bool rightSide = (rlState[0] == true); // came from right
                float targetX = 0f;
                if (!rightSide) {
                    targetX = rlState[1] ? 0f : -150f;
                } else {
                    targetX = rlState[2] ? 150f : 0f;
                }
                currentX = Mathf.Lerp(rightSide ? 75f : -75f, targetX, t);
            }
            // Result at bottom
            else if (rlCoreY >= 280) {
                // If X > 0 it hits PRESERVE (Green), else DESTROY (Red)
                if (currentX > 0f) {
                    MarkWinReady();
                    yield break;
                } else {
                    isPlaying = false; // Fail
                    yield break;
                }
            }

            if (rlCore != null) {
                rlCore.style.translate = new StyleTranslate(new Translate(currentX, rlCoreY, 0));
            }

            if (timer <= 0f) {
                isPlaying = false;
                yield break;
            }
            yield return null;
        }
    }

    // ==========================================
    // GALACTIC COUNCIL
    // ==========================================
    private class GcProjectile {
        public VisualElement ve;
        public Vector2 pos;
        public Vector2 vel;
    }
    private System.Collections.Generic.List<GcProjectile> gcProjs = new System.Collections.Generic.List<GcProjectile>();
    
    private void SetupGalacticCouncil()
    {
        timer = currentConfig.timeLimit;
        gcPodiumX = 0f;
        gcHits = 0;
        gcProgress = 0f;
        if (gcPodium != null) gcPodium.style.translate = new StyleTranslate(new Translate(0, 0, 0));
        if (gcProgressFill != null) gcProgressFill.style.width = new StyleLength(new Length(0, LengthUnit.Percent));
        if (gcProjectileContainer != null) gcProjectileContainer.Clear();
        gcProjs.Clear();
        activeGameRoutine = StartCoroutine(GalacticCouncilRoutine());
    }
    
    private System.Collections.IEnumerator GalacticCouncilRoutine()
    {
        isPlaying = true;
        float projTimer = 0f;
        
        while (isPlaying)
        {
            timer -= Time.deltaTime;
            gcProgress += (100f / currentConfig.timeLimit) * Time.deltaTime;
            
            if (gcProgressFill != null) gcProgressFill.style.width = new StyleLength(new Length(Mathf.Clamp(gcProgress, 0, 100), LengthUnit.Percent));

            projTimer -= Time.deltaTime;
            if (projTimer <= 0f) {
                projTimer = UnityEngine.Random.Range(0.4f, 1.0f);
                // Spawn projectile from one of the aliens
                int alien = UnityEngine.Random.Range(0, 3);
                float startX = (alien == 0) ? 50f : (alien == 1 ? 150f : 250f);
                var ve = new VisualElement();
                ve.style.position = Position.Absolute;
                ve.style.width = 15; ve.style.height = 15; ve.style.borderRadius = 8;
                ve.style.backgroundColor = Color.red;
                gcProjectileContainer.Add(ve);
                
                // Target player
                Vector2 startPos = new Vector2(startX, 60f);
                Vector2 playerPos = new Vector2(150f + gcPodiumX, 290f);
                Vector2 dir = (playerPos - startPos).normalized;
                
                gcProjs.Add(new GcProjectile { ve = ve, pos = startPos, vel = dir * UnityEngine.Random.Range(100f, 250f) });
            }

            // Move projectiles & check collisions
            for (int i=gcProjs.Count-1; i>=0; i--) {
                var p = gcProjs[i];
                p.pos += p.vel * Time.deltaTime;
                p.ve.style.left = p.pos.x - 7.5f;
                p.ve.style.top = p.pos.y - 7.5f;
                
                // Collision with podium
                float playerX = 150f + gcPodiumX;
                if (p.pos.y > 270f && p.pos.y < 310f && Mathf.Abs(p.pos.x - playerX) < 25f) {
                    gcHits++;
                    p.ve.RemoveFromHierarchy();
                    gcProjs.RemoveAt(i);
                    // Shake podium
                    gcPodium.style.translate = new StyleTranslate(new Translate(gcPodiumX + (UnityEngine.Random.value > 0.5f ? 10 : -10), 0, 0));
                    continue;
                }
                
                if (p.pos.y > 320f || p.pos.x < -20f || p.pos.x > 320f) {
                    p.ve.RemoveFromHierarchy();
                    gcProjs.RemoveAt(i);
                }
            }

            if (gcHits >= 3) {
                isPlaying = false; // Fail
                yield break;
            }

            if (timer <= 0f) {
                MarkWinReady();
                yield break;
            }
            yield return null;
        }
    }

}
"""

content = content[:content.rfind("}")] + logic_new + "}\n"

with open("Assets/Scripts/2 Managers/MiniGameManager.cs", "w") as f:
    f.write(content)

print("Updated MiniGameManager.cs")

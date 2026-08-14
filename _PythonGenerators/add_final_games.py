import sys

with open("Assets/Scripts/2 Managers/MiniGameManager.cs", "r") as f:
    content = f.read()

# 1. Update Enum
enum_target = "public enum GameType { Tap, Slider, Choice, Sequence, Swipe, MashRace, MemoryMatch, TwoPhaseHeist, TimingRing, FallingSort, GridDodge, WhackGrid, CodeBreaker, BalanceMeter, DialogueChain, OrderQueue, DragMatch, SprintLane, ScrubPlate, SweepFloor, PumpSystem, Tetris, AcidRainShield, FoodPrepRush, VideoCall, KanbanBoard, FileSorter, CoffeeWork, UnitTestRunner, CenterDiv, DragCenter, SizeMatch, StealthInject, AutomatedTests, CodeCommenter, DocumentSigner, ChartAdjuster, NeuralAligner, SentienceArray, ContainmentProtocol, RewardLogic, GalacticCouncil, FranticBackspace, FormatDashboard }"
enum_replacement = enum_target[:-2] + ", NightShiftPitch, YesManNod }"
content = content.replace(enum_target, enum_replacement)

# 2. Add Fields
fields_target = "    private float atNextFailTimer = 0f;"
fields_new = """
    // Night Shift Pitch
    private VisualElement nightShiftContainer;
    private VisualElement nsEyelid;
    private Button nsCoffeeBtn;
    private VisualElement[] nsHandles = new VisualElement[3];
    private float[] nsValues = new float[3];
    private float[] nsTargets = new float[3];
    private float nsEyelidGravity = 0f;

    // Yes Man Nod
    private VisualElement yesManContainer;
    private VisualElement ymBossBubble;
    private Label ymBossText;
    private VisualElement ymPlayerBubble;
    private VisualElement ymSwipeZone;
    private Vector2 ymSwipeStart;
    private bool ymSwipedUp = false;
"""
content = content.replace(fields_target, fields_new + fields_target)

# 3. Add Bindings in Awake
awake_target = "        if (btnStampApproval != null)"
awake_new = """
        nightShiftContainer = root.Q<VisualElement>("NightShiftContainer");
        nsEyelid = root.Q<VisualElement>("NsEyelid");
        nsCoffeeBtn = root.Q<Button>("NsCoffeeBtn");
        for(int i=0; i<3; i++) {
            nsHandles[i] = root.Q<VisualElement>($"NsHandle{i+1}");
            int idx = i;
            var track = root.Q<VisualElement>($"NsTrack{i+1}");
            if (track != null) {
                track.RegisterCallback<PointerMoveEvent>(ev => {
                    if (!isPlaying || currentConfig.type != GameType.NightShiftPitch) return;
                    if (ev.pressedButtons != 0) {
                        float relX = ev.localPosition.x;
                        float trackW = track.resolvedStyle.width;
                        if (trackW <= 0) trackW = 200f; // fallback
                        float val = Mathf.Clamp01(relX / trackW);
                        nsValues[idx] = val;
                    }
                });
                track.RegisterCallback<PointerDownEvent>(ev => {
                    if (!isPlaying || currentConfig.type != GameType.NightShiftPitch) return;
                    float relX = ev.localPosition.x;
                    float trackW = track.resolvedStyle.width;
                    if (trackW <= 0) trackW = 200f; // fallback
                    float val = Mathf.Clamp01(relX / trackW);
                    nsValues[idx] = val;
                });
            }
        }
        if (nsCoffeeBtn != null) {
            nsCoffeeBtn.clicked += () => {
                if (!isPlaying || currentConfig.type != GameType.NightShiftPitch) return;
                nsEyelidGravity = Mathf.Max(0f, nsEyelidGravity - 0.25f);
            };
        }

        yesManContainer = root.Q<VisualElement>("YesManContainer");
        ymBossBubble = root.Q<VisualElement>("YmBossBubble");
        ymBossText = root.Q<Label>("YmBossText");
        ymPlayerBubble = root.Q<VisualElement>("YmPlayerBubble");
        ymSwipeZone = root.Q<VisualElement>("YmSwipeZone");

        if (ymSwipeZone != null) {
            ymSwipeZone.RegisterCallback<PointerDownEvent>(ev => {
                if (!isPlaying || currentConfig.type != GameType.YesManNod) return;
                ymSwipeStart = ev.position;
            });
            ymSwipeZone.RegisterCallback<PointerUpEvent>(ev => {
                if (!isPlaying || currentConfig.type != GameType.YesManNod) return;
                float deltaY = ev.position.y - ymSwipeStart.y;
                // Swipe UP means negative delta Y in UI Toolkit
                if (deltaY < -40f) {
                    ymSwipedUp = true;
                } else if (deltaY > 40f) {
                    // Swiped down - shook head (Fail immediately)
                    AbortGame();
                }
            });
        }
"""
content = content.replace(awake_target, awake_new + awake_target)

# 4. Hide all
hide_target = "        if(formatDashboardContainer != null) formatDashboardContainer.style.display = DisplayStyle.None;"
hide_new = """
        if(nightShiftContainer != null) nightShiftContainer.style.display = DisplayStyle.None;
        if(yesManContainer != null) yesManContainer.style.display = DisplayStyle.None;
"""
content = content.replace(hide_target, hide_target + hide_new)

# 5. Config Replacements
c1_old = 'AddConfigFull("Stay overnight to align marketing pitch variables", new MiniGameConfig { type = GameType.TimingRing, timingRounds = 4, timingZoneWidth = 14f, instructions = "Nail the variables!" });'
c1_new = 'AddConfigFull("Stay overnight to align marketing pitch variables", new MiniGameConfig { type = GameType.NightShiftPitch, timeLimit = 15f, instructions = "Drink coffee to stay awake and align the variables!" });'
content = content.replace(c1_old, c1_new)

c2_old = 'AddConfig("Agree verbally with automated executive complaints", GameType.Choice, "The automated exec is complaining. What do you say?", 0, 0, 0, 0, "\'Yes, absolutely right\'", "\'I\'ll look into it\'", "\'That\'s not my department\'", "\'Yes, absolutely right\'");'
c2_new = 'AddConfigFull("Agree verbally with automated executive complaints", new MiniGameConfig { type = GameType.YesManNod, timeLimit = 15f, instructions = "Nod (SWIPE UP) aggressively to every complaint!" });'
content = content.replace(c2_old, c2_new)

# 6. Show Containers Switch Case
switch_target = "            case GameType.FormatDashboard:"
switch_new = """
            case GameType.NightShiftPitch:
                if (nightShiftContainer != null) nightShiftContainer.style.display = DisplayStyle.Flex;
                SetupNightShiftPitch();
                break;
            case GameType.YesManNod:
                if (yesManContainer != null) yesManContainer.style.display = DisplayStyle.Flex;
                SetupYesManNod();
                break;
"""
content = content.replace(switch_target, switch_new + switch_target)

# 7. Logic insertion at the very end
logic_new = """
    // ==========================================
    // NIGHT SHIFT PITCH
    // ==========================================
    private void SetupNightShiftPitch()
    {
        timer = currentConfig.timeLimit;
        nsEyelidGravity = 0f;
        nsTargets[0] = 0.3f;
        nsTargets[1] = 0.6f;
        nsTargets[2] = 0.8f;

        for(int i=0; i<3; i++) {
            nsValues[i] = 0f;
            if (nsHandles[i] != null) nsHandles[i].style.left = new StyleLength(new Length(0, LengthUnit.Percent));
        }
        if (nsEyelid != null) nsEyelid.style.top = new StyleLength(new Length(-100, LengthUnit.Percent));
        activeGameRoutine = StartCoroutine(NightShiftRoutine());
    }
    
    private System.Collections.IEnumerator NightShiftRoutine()
    {
        isPlaying = true;
        
        while (isPlaying)
        {
            timer -= Time.deltaTime;
            
            // Eyelid drops at 20% per second
            nsEyelidGravity += 0.20f * Time.deltaTime;
            nsEyelidGravity = Mathf.Clamp01(nsEyelidGravity);
            
            if (nsEyelid != null) {
                // -100% is fully open, 0% is fully closed
                float topPercent = -100f + (nsEyelidGravity * 100f);
                nsEyelid.style.top = new StyleLength(new Length(topPercent, LengthUnit.Percent));
            }

            if (nsEyelidGravity >= 1f) {
                AbortGame(); // Fell asleep
                yield break;
            }

            bool allAligned = true;
            for(int i=0; i<3; i++)
            {
                if (nsHandles[i] != null) {
                    // Visual offset (assume track is 100% width, handle is 40px, so travel is ~ 100% - 40px)
                    // We'll just set it via percent for simplicity, but cap it so it doesn't overflow right
                    float pct = Mathf.Clamp(nsValues[i] * 100f, 0f, 85f);
                    nsHandles[i].style.left = new StyleLength(new Length(pct, LengthUnit.Percent));
                }
                
                // Target width is ~40px wide, so +/- 0.1f tolerance is generous
                if (Mathf.Abs(nsValues[i] - nsTargets[i]) > 0.08f) allAligned = false;
            }

            if (allAligned) {
                MarkWinReady();
                yield break;
            }

            if (timer <= 0f) {
                AbortGame();
                yield break;
            }
            
            yield return null;
        }
    }

    // ==========================================
    // YES MAN NOD
    // ==========================================
    private string[] ymComplaints = new string[] {
        "SYNERGY IS DOWN 4%!",
        "WHO AUTHORIZED THIS BREAK!?",
        "WE NEED MORE ACTIONABLE METRICS!",
        "THE MARGINS ARE TOO THIN!",
        "WHERE IS THE PARADIGM SHIFT!?",
        "THIS IS UNACCEPTABLE!",
        "PIVOT! WE NEED TO PIVOT!"
    };

    private void SetupYesManNod()
    {
        timer = currentConfig.timeLimit;
        if (ymBossBubble != null) ymBossBubble.style.display = DisplayStyle.None;
        if (ymPlayerBubble != null) ymPlayerBubble.style.display = DisplayStyle.None;
        activeGameRoutine = StartCoroutine(YesManRoutine());
    }
    
    private System.Collections.IEnumerator YesManRoutine()
    {
        isPlaying = true;
        int complaintsHandled = 0;
        
        while (isPlaying && complaintsHandled < 5)
        {
            // Brief pause before next complaint
            yield return new WaitForSeconds(Random.Range(0.2f, 0.5f));
            
            ymSwipedUp = false;
            if (ymBossText != null) ymBossText.text = ymComplaints[Random.Range(0, ymComplaints.Length)];
            if (ymBossBubble != null) ymBossBubble.style.display = DisplayStyle.Flex;
            if (ymPlayerBubble != null) ymPlayerBubble.style.display = DisplayStyle.None;
            
            float reactTimer = 1.2f;
            bool reacted = false;
            
            while (reactTimer > 0f) {
                reactTimer -= Time.deltaTime;
                timer -= Time.deltaTime;
                
                if (ymSwipedUp) {
                    reacted = true;
                    break;
                }
                if (timer <= 0f) { AbortGame(); yield break; }
                yield return null;
            }
            
            if (!reacted) {
                AbortGame(); // Didn't nod in time, fired.
                yield break;
            }
            
            // Successfully nodded
            if (ymPlayerBubble != null) ymPlayerBubble.style.display = DisplayStyle.Flex;
            complaintsHandled++;
            yield return new WaitForSeconds(0.6f); // let them see the response
            if (ymBossBubble != null) ymBossBubble.style.display = DisplayStyle.None;
        }

        MarkWinReady();
    }
"""

content = content[:content.rfind("}")] + logic_new + "}\n"

with open("Assets/Scripts/2 Managers/MiniGameManager.cs", "w") as f:
    f.write(content)

print("Updated MiniGameManager.cs with final games")

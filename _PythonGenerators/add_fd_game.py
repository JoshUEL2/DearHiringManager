import sys

with open("Assets/Scripts/2 Managers/MiniGameManager.cs", "r") as f:
    content = f.read()

# 1. Update Enum
enum_target = "public enum GameType { Tap, Slider, Choice, Sequence, Swipe, MashRace, MemoryMatch, TwoPhaseHeist, TimingRing, FallingSort, GridDodge, WhackGrid, CodeBreaker, BalanceMeter, DialogueChain, OrderQueue, DragMatch, SprintLane, ScrubPlate, SweepFloor, PumpSystem, Tetris, AcidRainShield, FoodPrepRush, VideoCall, KanbanBoard, FileSorter, CoffeeWork, UnitTestRunner, CenterDiv, DragCenter, SizeMatch, StealthInject, AutomatedTests, CodeCommenter, DocumentSigner, ChartAdjuster, NeuralAligner, SentienceArray, ContainmentProtocol, RewardLogic, GalacticCouncil, FranticBackspace }"
enum_replacement = enum_target[:-2] + ", FormatDashboard }"
content = content.replace(enum_target, enum_replacement)

# 2. Add Fields
fields_target = "    private float atNextFailTimer = 0f;"
fields_new = """
    // Format Dashboard
    private VisualElement formatDashboardContainer;
    private Label fdTimerText;
    private VisualElement fdProgressFill;
    private VisualElement[] fdBars = new VisualElement[4];
    private VisualElement[] fdHandles = new VisualElement[3];
    private float[] fdValues = new float[3];
    private float fdHoldTimer = 0f;
"""
content = content.replace(fields_target, fields_new + fields_target)

# 3. Add Bindings in Awake
awake_target = "        if (btnStampApproval != null)"
awake_new = """
        formatDashboardContainer = root.Q<VisualElement>("FormatDashboardContainer");
        fdTimerText = root.Q<Label>("FdTimerText");
        fdProgressFill = root.Q<VisualElement>("FdProgressFill");
        for(int i=0; i<4; i++) fdBars[i] = root.Q<VisualElement>($"FdBar{i+1}");
        for(int i=0; i<3; i++) {
            fdHandles[i] = root.Q<VisualElement>($"FdHandle{i+1}");
            int idx = i;
            var track = root.Q<VisualElement>($"FdTrack{i+1}");
            if (track != null) {
                track.RegisterCallback<PointerMoveEvent>(ev => {
                    if (!isPlaying || currentConfig.type != GameType.FormatDashboard) return;
                    if (ev.pressedButtons != 0) {
                        float relY = ev.localPosition.y;
                        float trackHeight = track.resolvedStyle.height;
                        if (trackHeight <= 0) trackHeight = 200f; // fallback
                        // y=0 is top (100%), y=trackHeight is bottom (0%)
                        float val = 1f - Mathf.Clamp01(relY / trackHeight);
                        // Make sure we only allow dragging UP, or just set it
                        fdValues[idx] = val;
                    }
                });
                track.RegisterCallback<PointerDownEvent>(ev => {
                    if (!isPlaying || currentConfig.type != GameType.FormatDashboard) return;
                    float relY = ev.localPosition.y;
                    float trackHeight = track.resolvedStyle.height;
                    if (trackHeight <= 0) trackHeight = 200f; // fallback
                    float val = 1f - Mathf.Clamp01(relY / trackHeight);
                    fdValues[idx] = val;
                });
            }
        }
"""
content = content.replace(awake_target, awake_new + awake_target)

# 4. Hide all
hide_target = "        if(franticBackspaceContainer != null) franticBackspaceContainer.style.display = DisplayStyle.None;"
hide_new = """
        if(formatDashboardContainer != null) formatDashboardContainer.style.display = DisplayStyle.None;
"""
content = content.replace(hide_target, hide_target + hide_new)

# 5. Config Replacements
c1_old = 'AddConfig("Format summary dashboards", GameType.Slider, "Adjust the chart scale — too zoomed in hides trends, too zoomed out hides details!", 0, 0, 42f, 58f);'
c1_new = 'AddConfigFull("Format summary dashboards", new MiniGameConfig { type = GameType.FormatDashboard, timeLimit = 15f, instructions = "Make it POP! Swipe all 3 sliders up and HOLD them in the green!" });'
content = content.replace(c1_old, c1_new)

# 6. Show Containers Switch Case
switch_target = "            case GameType.FranticBackspace:"
switch_new = """
            case GameType.FormatDashboard:
                if (formatDashboardContainer != null) formatDashboardContainer.style.display = DisplayStyle.Flex;
                SetupFormatDashboard();
                break;
"""
content = content.replace(switch_target, switch_new + switch_target)

# 7. Logic insertion at the very end
logic_new = """
    // ==========================================
    // FORMAT DASHBOARD (MAKE IT POP)
    // ==========================================
    private void SetupFormatDashboard()
    {
        timer = currentConfig.timeLimit;
        fdHoldTimer = 0f;
        for(int i=0; i<3; i++) {
            fdValues[i] = 0f;
            if (fdHandles[i] != null) fdHandles[i].style.translate = new StyleTranslate(new Translate(0, 0, 0));
        }
        if (fdProgressFill != null) fdProgressFill.style.width = new StyleLength(new Length(0, LengthUnit.Percent));
        if (fdTimerText != null) fdTimerText.text = $"{timer:F1}s";
        activeGameRoutine = StartCoroutine(FormatDashboardRoutine());
    }
    
    private System.Collections.IEnumerator FormatDashboardRoutine()
    {
        isPlaying = true;
        float gravity = 0.40f; // Drops 40% per second
        
        while (isPlaying)
        {
            timer -= Time.deltaTime;
            
            if (fdTimerText != null) {
                fdTimerText.text = $"{timer:F1}s";
                if (timer <= 3.0f) fdTimerText.style.color = Color.red;
                else fdTimerText.style.color = new StyleColor(ColorUtility.TryParseHtmlString("#9CA3AF", out Color c) ? c : Color.gray);
            }

            bool allGreen = true;

            for(int i=0; i<3; i++)
            {
                // Apply gravity
                fdValues[i] -= gravity * Time.deltaTime;
                fdValues[i] = Mathf.Clamp01(fdValues[i]);

                if (fdValues[i] < 0.85f) allGreen = false;

                // Visual update Handle
                if (fdHandles[i] != null) {
                    float yOffset = (1f - fdValues[i]) * 170f; // 200 track - 30 handle = 170 travel
                    fdHandles[i].style.translate = new StyleTranslate(new Translate(0, yOffset, 0));
                    
                    if (fdValues[i] >= 0.85f) fdHandles[i].style.backgroundColor = new StyleColor(ColorUtility.TryParseHtmlString("#10B981", out Color g) ? g : Color.green);
                    else fdHandles[i].style.backgroundColor = new StyleColor(i == 0 ? ColorUtility.TryParseHtmlString("#3B82F6", out Color b) ? b : Color.blue : i == 1 ? ColorUtility.TryParseHtmlString("#F59E0B", out Color y) ? y : Color.yellow : ColorUtility.TryParseHtmlString("#EF4444", out Color r) ? r : Color.red);
                }
            }

            // Dummy visual chaos
            float avg = (fdValues[0] + fdValues[1] + fdValues[2]) / 3f;
            for(int i=0; i<4; i++) {
                if (fdBars[i] != null) {
                    float randomJitter = (allGreen ? 0f : Random.Range(-0.1f, 0.1f)) * avg;
                    float baseH = 0.3f + (0.2f * i);
                    fdBars[i].style.height = new StyleLength(new Length(Mathf.Clamp01(baseH + randomJitter) * 100f, LengthUnit.Percent));
                }
            }

            if (allGreen) {
                fdHoldTimer += Time.deltaTime;
            } else {
                fdHoldTimer = Mathf.Max(0, fdHoldTimer - Time.deltaTime * 2f); // lose progress fast if dropped
            }

            float progressPct = Mathf.Clamp01(fdHoldTimer / 2.0f);
            if (fdProgressFill != null) fdProgressFill.style.width = new StyleLength(new Length(progressPct * 100f, LengthUnit.Percent));

            if (progressPct >= 1f) {
                if (fdTimerText != null) {
                    fdTimerText.text = "POPPIN!";
                    fdTimerText.style.color = new StyleColor(ColorUtility.TryParseHtmlString("#10B981", out Color g) ? g : Color.green);
                }
                yield return new WaitForSeconds(1.0f);
                MarkWinReady();
                yield break;
            }

            if (timer <= 0f) {
                if (fdTimerText != null) fdTimerText.text = "FAILED!";
                yield return new WaitForSeconds(1.5f);
                AbortGame(); // Fail
                yield break;
            }
            
            yield return null;
        }
    }
"""

content = content[:content.rfind("}")] + logic_new + "}\n"

with open("Assets/Scripts/2 Managers/MiniGameManager.cs", "w") as f:
    f.write(content)

print("Updated MiniGameManager.cs with FormatDashboard")

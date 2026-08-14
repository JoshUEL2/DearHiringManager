import sys

with open("Assets/Scripts/2 Managers/MiniGameManager.cs", "r") as f:
    content = f.read()

# 1. Update Enum
enum_target = "public enum GameType { Tap, Slider, Choice, Sequence, Swipe, MashRace, MemoryMatch, TwoPhaseHeist, TimingRing, FallingSort, GridDodge, WhackGrid, CodeBreaker, BalanceMeter, DialogueChain, OrderQueue, DragMatch, SprintLane, ScrubPlate, SweepFloor, PumpSystem, Tetris, AcidRainShield, FoodPrepRush, VideoCall, KanbanBoard, FileSorter, CoffeeWork, UnitTestRunner, CenterDiv, DragCenter, SizeMatch, StealthInject, AutomatedTests, CodeCommenter, DocumentSigner, ChartAdjuster, NeuralAligner, SentienceArray, ContainmentProtocol, RewardLogic, GalacticCouncil }"
enum_replacement = "public enum GameType { Tap, Slider, Choice, Sequence, Swipe, MashRace, MemoryMatch, TwoPhaseHeist, TimingRing, FallingSort, GridDodge, WhackGrid, CodeBreaker, BalanceMeter, DialogueChain, OrderQueue, DragMatch, SprintLane, ScrubPlate, SweepFloor, PumpSystem, Tetris, AcidRainShield, FoodPrepRush, VideoCall, KanbanBoard, FileSorter, CoffeeWork, UnitTestRunner, CenterDiv, DragCenter, SizeMatch, StealthInject, AutomatedTests, CodeCommenter, DocumentSigner, ChartAdjuster, NeuralAligner, SentienceArray, ContainmentProtocol, RewardLogic, GalacticCouncil, FranticBackspace }"
content = content.replace(enum_target, enum_replacement)

# 2. Add Fields
fields_target = "    private float atNextFailTimer = 0f;"
fields_new = """
    // Frantic Backspace
    private VisualElement franticBackspaceContainer;
    private Label fbTimerText;
    private Label fbPlayerText;
    private Button fbBackspaceBtn;
    private string fbOriginalAngryMessage = "Are you out of your mind? I am absolutely not doing your job for you on my weekend!";
    private string fbCurrentMessage = "";
"""
content = content.replace(fields_target, fields_new + fields_target)

# 3. Add Bindings in Awake
awake_target = "        if (btnStampApproval != null)"
awake_new = """
        franticBackspaceContainer = root.Q<VisualElement>("FranticBackspaceContainer");
        fbTimerText = root.Q<Label>("FbTimerText");
        fbPlayerText = root.Q<Label>("FbPlayerText");
        fbBackspaceBtn = root.Q<Button>("FbBackspaceBtn");
        
        if (fbBackspaceBtn != null) {
            fbBackspaceBtn.RegisterCallback<PointerDownEvent>(ev => {
                if (!isPlaying || currentConfig.type != GameType.FranticBackspace) return;
                
                // Delete up to 5 characters per tap to make it frantic but possible
                if (fbCurrentMessage.Length > 0) {
                    int removeCount = UnityEngine.Mathf.Min(5, fbCurrentMessage.Length);
                    fbCurrentMessage = fbCurrentMessage.Substring(0, fbCurrentMessage.Length - removeCount);
                    if (fbPlayerText != null) fbPlayerText.text = fbCurrentMessage + "|";
                }
            });
        }
"""
content = content.replace(awake_target, awake_new + awake_target)

# 4. Hide all
hide_target = "        if(galacticCouncilContainer != null) galacticCouncilContainer.style.display = DisplayStyle.None;"
hide_new = """
        if(franticBackspaceContainer != null) franticBackspaceContainer.style.display = DisplayStyle.None;
"""
content = content.replace(hide_target, hide_target + hide_new)

# 5. Config Replacements
c1_old = 'AddConfig("Respond to supervisor", GameType.Choice, "Your supervisor has sent an urgent message. How do you respond?", 0, 0, 0, 0, "Reply immediately", "Wait an hour", "Forward to colleague", "Reply immediately");'
c1_new = 'AddConfigFull("Respond to supervisor", new MiniGameConfig { type = GameType.FranticBackspace, timeLimit = 6f, instructions = "FRANTIC BACKSPACE! DELETE YOUR HONEST THOUGHTS!" });'
content = content.replace(c1_old, c1_new)

# 6. Show Containers Switch Case
switch_target = "            case GameType.SentienceArray:"
switch_new = """
            case GameType.FranticBackspace:
                if (franticBackspaceContainer != null) franticBackspaceContainer.style.display = DisplayStyle.Flex;
                SetupFranticBackspace();
                break;
"""
content = content.replace(switch_target, switch_new + switch_target)

# 7. Logic insertion at the very end
logic_new = """
    // ==========================================
    // FRANTIC BACKSPACE
    // ==========================================
    private void SetupFranticBackspace()
    {
        timer = currentConfig.timeLimit;
        fbCurrentMessage = fbOriginalAngryMessage;
        if (fbPlayerText != null) fbPlayerText.text = fbCurrentMessage + "|";
        if (fbTimerText != null) fbTimerText.text = $"AUTO-SEND: {timer:F1}s";
        activeGameRoutine = StartCoroutine(FranticBackspaceRoutine());
    }
    
    private System.Collections.IEnumerator FranticBackspaceRoutine()
    {
        isPlaying = true;
        
        while (isPlaying)
        {
            timer -= Time.deltaTime;
            
            if (fbTimerText != null) {
                fbTimerText.text = $"AUTO-SEND: {timer:F1}s";
                if (timer <= 2.0f) fbTimerText.style.color = Color.red;
                else fbTimerText.style.color = new StyleColor(ColorUtility.TryParseHtmlString("#DC2626", out Color c) ? c : Color.red);
            }

            if (fbCurrentMessage.Length == 0) {
                if (fbPlayerText != null) fbPlayerText.text = "Sounds great! 👍";
                if (fbTimerText != null) {
                    fbTimerText.text = "SENT!";
                    fbTimerText.style.color = new StyleColor(ColorUtility.TryParseHtmlString("#166534", out Color g) ? g : Color.green);
                }
                MarkWinReady();
                yield break;
            }

            if (timer <= 0f) {
                if (fbTimerText != null) fbTimerText.text = "SENT (UH OH)";
                isPlaying = false; // Fail
                yield break;
            }
            yield return null;
        }
    }
"""

content = content[:content.rfind("}")] + logic_new + "}\n"

with open("Assets/Scripts/2 Managers/MiniGameManager.cs", "w") as f:
    f.write(content)

print("Updated MiniGameManager.cs with FranticBackspace")

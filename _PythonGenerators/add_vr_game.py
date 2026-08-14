import sys

with open("Assets/Scripts/2 Managers/MiniGameManager.cs", "r") as f:
    content = f.read()

# 1. Update Enum
enum_target = "public enum GameType { Tap, Slider, Choice, Sequence, Swipe, MashRace, MemoryMatch, TwoPhaseHeist, TimingRing, FallingSort, GridDodge, WhackGrid, CodeBreaker, BalanceMeter, DialogueChain, OrderQueue, DragMatch, SprintLane, ScrubPlate, SweepFloor, PumpSystem, Tetris, AcidRainShield, FoodPrepRush, VideoCall, KanbanBoard, FileSorter, CoffeeWork, UnitTestRunner, CenterDiv, DragCenter, SizeMatch, StealthInject, AutomatedTests, CodeCommenter, DocumentSigner, ChartAdjuster, NeuralAligner, SentienceArray, ContainmentProtocol, RewardLogic, GalacticCouncil, FranticBackspace, FormatDashboard, NightShiftPitch, YesManNod }"
enum_replacement = enum_target[:-2] + ", VrArcadePong }"
content = content.replace(enum_target, enum_replacement)

# 2. Add Fields
fields_target = "    private bool ymSwipedUp = false;"
fields_new = """    // VR Arcade Pong
    private VisualElement vrArcadeContainer;
    private VisualElement vrBall;
    private VisualElement vrPaddle;
    private VisualElement vrTouchZone;
    private Label vrScoreText;
    private Vector2 vrBallPos;
    private Vector2 vrBallVel;
    private float vrPaddleX;
    private int vrBounces;
"""
content = content.replace(fields_target, fields_target + "\n" + fields_new)

# 3. Add Bindings in Awake
awake_target = "        yesManContainer = root.Q<VisualElement>(\"YesManContainer\");"
awake_new = """        vrArcadeContainer = root.Q<VisualElement>("VrArcadeContainer");
        vrBall = root.Q<VisualElement>("VrBall");
        vrPaddle = root.Q<VisualElement>("VrPaddle");
        vrScoreText = root.Q<Label>("VrScoreText");
        vrTouchZone = root.Q<VisualElement>("VrTouchZone");

        if (vrTouchZone != null) {
            vrTouchZone.RegisterCallback<PointerMoveEvent>(ev => {
                if (!isPlaying || currentConfig.type != GameType.VrArcadePong) return;
                if (ev.pressedButtons != 0) {
                    vrPaddleX = ev.localPosition.x;
                }
            });
            vrTouchZone.RegisterCallback<PointerDownEvent>(ev => {
                if (!isPlaying || currentConfig.type != GameType.VrArcadePong) return;
                vrPaddleX = ev.localPosition.x;
            });
        }
"""
content = content.replace(awake_target, awake_new + "\n" + awake_target)

# 4. Hide all
hide_target = "        if(yesManContainer != null) yesManContainer.style.display = DisplayStyle.None;"
hide_new = "        if(vrArcadeContainer != null) vrArcadeContainer.style.display = DisplayStyle.None;\n"
content = content.replace(hide_target, hide_target + "\n" + hide_new)

# 5. Config Replacements
c1_old = 'AddConfigFull("Capsule Arcade VR", new MiniGameConfig { type = GameType.OrderQueue, orderItems = new string[]{"⬆","⬇","◀","▶"}, orderPreviewTime = 3f, instructions = "Play the arcade game!" });'
c1_new = 'AddConfigFull("Capsule Arcade VR", new MiniGameConfig { type = GameType.VrArcadePong, timeLimit = 15f, instructions = "Keep the ball alive! (Bounce 5 times)" });'
content = content.replace(c1_old, c1_new)


# 6. Show Containers Switch Case
switch_target = "            case GameType.NightShiftPitch:"
switch_new = """            case GameType.VrArcadePong:
                if (vrArcadeContainer != null) vrArcadeContainer.style.display = DisplayStyle.Flex;
                SetupVrArcadePong();
                break;
"""
content = content.replace(switch_target, switch_new + switch_target)

# 7. Logic insertion at the very end
logic_new = """
    // ==========================================
    // VR ARCADE PONG
    // ==========================================
    private void SetupVrArcadePong()
    {
        timer = currentConfig.timeLimit;
        vrBounces = 0;
        vrBallPos = new Vector2(200f, 50f);
        vrBallVel = new Vector2(Random.value > 0.5f ? 250f : -250f, 300f);
        vrPaddleX = 200f;
        
        if (vrScoreText != null) vrScoreText.text = $"BOUNCES: 0 / 5";
        if (vrBall != null) {
            vrBall.style.left = new StyleLength(new Length(vrBallPos.x, LengthUnit.Pixel));
            vrBall.style.top = new StyleLength(new Length(vrBallPos.y, LengthUnit.Pixel));
        }

        activeGameRoutine = StartCoroutine(VrArcadeRoutine());
    }
    
    private System.Collections.IEnumerator VrArcadeRoutine()
    {
        isPlaying = true;
        
        // Assuming container is roughly 400x400 based on standard setup
        float containerW = 400f;
        float containerH = 400f;
        if (vrArcadeContainer != null) {
            float w = vrArcadeContainer.resolvedStyle.width;
            float h = vrArcadeContainer.resolvedStyle.height;
            if (w > 0) containerW = w;
            if (h > 0) containerH = h;
        }

        float paddleWidth = 100f;
        float paddleHeight = 20f;
        float paddleBottomOffset = 10f;
        float ballSize = 20f;

        while (isPlaying)
        {
            timer -= Time.deltaTime;
            
            // Move Ball
            vrBallPos += vrBallVel * Time.deltaTime;

            // Constrain Paddle (centered on vrPaddleX)
            float paddleLeft = Mathf.Clamp(vrPaddleX - (paddleWidth / 2f), 0, containerW - paddleWidth);
            if (vrPaddle != null) vrPaddle.style.left = new StyleLength(new Length(paddleLeft, LengthUnit.Pixel));

            // Wall Collisions
            if (vrBallPos.x <= 0f) {
                vrBallPos.x = 0f;
                vrBallVel.x *= -1f;
            } else if (vrBallPos.x >= containerW - ballSize) {
                vrBallPos.x = containerW - ballSize;
                vrBallVel.x *= -1f;
            }

            if (vrBallPos.y <= 0f) {
                vrBallPos.y = 0f;
                vrBallVel.y *= -1f;
            }

            // Paddle Collision (paddle is at containerH - paddleBottomOffset - paddleHeight)
            float paddleTopY = containerH - paddleBottomOffset - paddleHeight;
            
            if (vrBallVel.y > 0 && vrBallPos.y + ballSize >= paddleTopY && vrBallPos.y < paddleTopY + paddleHeight) {
                // Check X bounds
                if (vrBallPos.x + ballSize >= paddleLeft && vrBallPos.x <= paddleLeft + paddleWidth) {
                    // Bounce!
                    vrBallPos.y = paddleTopY - ballSize;
                    vrBallVel.y *= -1f; // go up
                    
                    // Add some english based on where it hit the paddle
                    float hitFactor = (vrBallPos.x + (ballSize/2f)) - (paddleLeft + (paddleWidth/2f));
                    vrBallVel.x = hitFactor * 5f; // apply horizontal speed based on hit offset
                    
                    // Speed up slightly
                    vrBallVel *= 1.1f;
                    
                    vrBounces++;
                    if (vrScoreText != null) vrScoreText.text = $"BOUNCES: {vrBounces} / 5";

                    if (vrBounces >= 5) {
                        MarkWinReady();
                        yield break;
                    }
                }
            }

            // Floor Collision (Miss)
            if (vrBallPos.y > containerH) {
                if (vrScoreText != null) vrScoreText.text = "FAILED!";
                yield return new WaitForSeconds(1.0f);
                AbortGame();
                yield break;
            }

            // Update visuals
            if (vrBall != null) {
                vrBall.style.left = new StyleLength(new Length(vrBallPos.x, LengthUnit.Pixel));
                vrBall.style.top = new StyleLength(new Length(vrBallPos.y, LengthUnit.Pixel));
            }

            if (timer <= 0f) {
                AbortGame();
                yield break;
            }
            
            yield return null;
        }
    }
"""

content = content[:content.rfind("}")] + logic_new + "}\n"

with open("Assets/Scripts/2 Managers/MiniGameManager.cs", "w") as f:
    f.write(content)

print("Updated MiniGameManager.cs with VrArcadePong")

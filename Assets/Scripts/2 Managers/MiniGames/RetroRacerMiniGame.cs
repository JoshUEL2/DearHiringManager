using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class RetroRacerMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement rootElement;

    private VisualElement rrContainer;
    private VisualElement rrPlayer;
    private Label rrDistanceLabel;
    private VisualElement[] rrFoodPips = new VisualElement[3];
    private Label rrTimeLabel;
    private VisualElement[] rrRoadLines = new VisualElement[8];
    private VisualElement[] rrObstacleElements = new VisualElement[8];
    private bool[] rrObstacleActive = new bool[8];
    private Vector2[] rrObstaclePos = new Vector2[8];
    private int rrBlocks = 0;
    private float rrDistanceTimer = 0f;
    private bool rrIsDragging = false;
    private float rrTargetLeft = 50f;
    private float timer;
    private int foodHealth = 3;
    private float crashInvulnTimer = 0f;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        this.rootElement = root;

        rrContainer = root.Q<VisualElement>("RetroRacerContainer");
        if (rrContainer != null)
        {
            rrPlayer = rrContainer.Q<VisualElement>("RrPlayer");
            rrDistanceLabel = rrContainer.Q<Label>("RrDistanceLabel");
            rrTimeLabel = rrContainer.Q<Label>("RrTimeLabel");

            for (int i = 0; i < 3; i++)
            {
                rrFoodPips[i] = rrContainer.Q<VisualElement>($"RrFoodPip{i}");
            }

            for (int i = 0; i < 8; i++)
            {
                rrRoadLines[i] = rrContainer.Q<VisualElement>($"RrLine{i}");
                rrObstacleElements[i] = rrContainer.Q<VisualElement>($"RrObs{i}");
            }

            rrContainer.RegisterCallback<PointerDownEvent>(OnRrPointerDown);
            rrContainer.RegisterCallback<PointerMoveEvent>(OnRrPointerMove);
            rrContainer.RegisterCallback<PointerUpEvent>(OnRrPointerUp);
            rrContainer.RegisterCallback<PointerCancelEvent>(OnRrPointerUp);
            rrContainer.RegisterCallback<PointerCaptureOutEvent>(ev => rrIsDragging = false);
        }
    }

    public void StartGame()
    {
        if (rrContainer != null) rrContainer.style.display = DisplayStyle.Flex;
        
        timer = config.timeLimit > 0 ? config.timeLimit : 20f;
        rrBlocks = 0;
        rrDistanceTimer = 0f;
        rrTargetLeft = 50f;
        foodHealth = 3;
        crashInvulnTimer = 0f;
        
        if (rrPlayer != null)
        {
            rrPlayer.style.left = new Length(50f, LengthUnit.Percent);
            rrPlayer.style.rotate = new Rotate(Angle.Degrees(0f));
            rrPlayer.style.opacity = 1f;
            rrPlayer.style.unityBackgroundImageTintColor = Color.white;
        }

        if (rrDistanceLabel != null) rrDistanceLabel.text = "DISTANCE: 0";
        if (rrTimeLabel != null) rrTimeLabel.text = $"TIME: {Mathf.CeilToInt(timer)}";
        UpdateFoodDisplay();
        
        for (int i = 0; i < 8; i++) {
            rrObstacleActive[i] = false;
            if (rrObstacleElements[i] != null) rrObstacleElements[i].style.display = DisplayStyle.None;
        }

        manager.StartCoroutine(RetroRacerRoutine());
    }

    public void AbortGame()
    {
        if (rrContainer != null) rrContainer.style.display = DisplayStyle.None;
        rrIsDragging = false;
    }

    private void UpdateFoodDisplay()
    {
        for (int i = 0; i < 3; i++)
        {
            if (rrFoodPips[i] != null)
            {
                if (i < foodHealth)
                {
                    rrFoodPips[i].style.opacity = 1f;
                    rrFoodPips[i].style.unityBackgroundImageTintColor = Color.white;
                }
                else
                {
                    rrFoodPips[i].style.opacity = 0.25f;
                    rrFoodPips[i].style.unityBackgroundImageTintColor = new Color(0.4f, 0.4f, 0.4f, 0.5f);
                }
            }
        }
    }

    private void OnRrPointerDown(PointerDownEvent ev)
    {
        if (!manager.isPlaying) return;
        rrIsDragging = true;
        if (rrContainer != null) rrContainer.CapturePointer(ev.pointerId);
        UpdateRrPlayerPos(ev.position.x);
    }

    private void OnRrPointerMove(PointerMoveEvent ev)
    {
        if (!rrIsDragging || !manager.isPlaying) return;
        UpdateRrPlayerPos(ev.position.x);
    }

    private void OnRrPointerUp(EventBase ev)
    {
        rrIsDragging = false;
        if (rrContainer == null) return;

        if (ev is PointerUpEvent pue && rrContainer.HasPointerCapture(pue.pointerId))
        {
            rrContainer.ReleasePointer(pue.pointerId);
        }
        else if (ev is PointerCancelEvent pce && rrContainer.HasPointerCapture(pce.pointerId))
        {
            rrContainer.ReleasePointer(pce.pointerId);
        }
    }

    private void UpdateRrPlayerPos(float screenMouseX)
    {
        if (rrContainer == null) return;
        float containerX = rrContainer.worldBound.xMin;
        float width = rrContainer.worldBound.width;
        if (width <= 0f) return;

        float relativeX = screenMouseX - containerX;
        float pct = (relativeX / width) * 100f;
        // Restrict steering safely within visible road bounds (15% to 85%)
        rrTargetLeft = Mathf.Clamp(pct, 15f, 85f);
    }

    private IEnumerator RetroRacerRoutine()
    {
        float spawnTimer = 0.5f;
        float[] laneTargets = { 18f, 50f, 82f };
        
        float[] lineProgress = new float[8];
        for(int i = 0; i < 8; i++) lineProgress[i] = (float)i / 8f;
        
        float speed = 0.55f;

        while (manager.isPlaying)
        {
            timer -= Time.deltaTime;
            if (crashInvulnTimer > 0f)
            {
                crashInvulnTimer -= Time.deltaTime;
                if (rrPlayer != null)
                {
                    // Rapid flicker during recovery
                    float pulse = Mathf.PingPong(Time.time * 20f, 1f);
                    rrPlayer.style.opacity = pulse > 0.4f ? 1f : 0.3f;
                }
            }
            else if (rrPlayer != null && rrPlayer.style.opacity.value != 1f)
            {
                rrPlayer.style.opacity = 1f;
            }

            if (timer <= 0f)
            {
                AudioManager.Instance?.PlayVictoryFanfare();
                MiniGameFeedback.PlaySuccess(rrPlayer, "DELIVERED!", rrContainer);
                manager.MarkWinReady();
                yield break;
            }
            if (rrTimeLabel != null) rrTimeLabel.text = $"TIME: {Mathf.CeilToInt(timer)}";

            speed += Time.deltaTime * 0.04f;
            
            rrDistanceTimer += Time.deltaTime * speed;
            if (rrDistanceTimer > 0.1f)
            {
                rrDistanceTimer = 0f;
                rrBlocks++;
                if (rrDistanceLabel != null) rrDistanceLabel.text = $"DISTANCE: {rrBlocks}";
            }

            // Animate 3D road strips
            for (int i = 0; i < 8; i++) {
                lineProgress[i] += Time.deltaTime * speed * 2f;
                if (lineProgress[i] > 1f) lineProgress[i] -= 1f;
                
                if (rrRoadLines[i] != null) {
                    float y = Mathf.Pow(lineProgress[i], 2f);
                    rrRoadLines[i].style.top = new Length(y * 100f, LengthUnit.Percent);
                    
                    float w = Mathf.Lerp(10f, 800f, y);
                    rrRoadLines[i].style.width = w;
                    rrRoadLines[i].style.left = new Length(50f, LengthUnit.Percent);
                    rrRoadLines[i].style.marginLeft = -w / 2f;
                    
                    float h = Mathf.Lerp(1f, 15f, y);
                    rrRoadLines[i].style.height = h;
                    
                    rrRoadLines[i].style.backgroundColor = new StyleColor(new Color(1f, 1f, 1f, y));
                }
            }

            // Keyboard steering support (A/D or Arrows)
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            {
                rrTargetLeft = Mathf.Clamp(rrTargetLeft - Time.deltaTime * 75f, 15f, 85f);
            }
            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            {
                rrTargetLeft = Mathf.Clamp(rrTargetLeft + Time.deltaTime * 75f, 15f, 85f);
            }

            // Position and tilt player
            if (rrPlayer != null)
            {
                float currentLeft = rrPlayer.style.left.value.value;
                float newLeft = Mathf.Lerp(currentLeft, rrTargetLeft, Time.deltaTime * 25f);
                rrPlayer.style.left = new Length(newLeft, LengthUnit.Percent);
                
                float steerDelta = rrTargetLeft - currentLeft;
                float tilt = Mathf.Clamp(steerDelta * 2.8f, -22f, 22f);
                rrPlayer.style.rotate = new Rotate(Angle.Degrees(tilt));
            }

            // Spawn incoming obstacles
            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0f)
            {
                for (int i = 0; i < 8; i++) {
                    if (!rrObstacleActive[i]) {
                        rrObstacleActive[i] = true;
                        int obsLane = Random.Range(0, 3);
                        rrObstaclePos[i] = new Vector2(obsLane, 0f);
                        if (rrObstacleElements[i] != null) {
                            rrObstacleElements[i].style.display = DisplayStyle.Flex;
                            float r = Random.value;
                            rrObstacleElements[i].RemoveFromClassList("mg-icon-car");
                            rrObstacleElements[i].RemoveFromClassList("mg-icon-cone");
                            if (r < 0.6f)
                                rrObstacleElements[i].AddToClassList("mg-icon-car");
                            else
                                rrObstacleElements[i].AddToClassList("mg-icon-cone");
                        }
                        break;
                    }
                }
                spawnTimer = Random.Range(0.6f, 1.1f) / speed;
            }

            // Update obstacles and check collisions
            float playerX = rrPlayer != null ? rrPlayer.style.left.value.value : 50f;

            for (int i = 0; i < 8; i++)
            {
                if (!rrObstacleActive[i]) continue;

                Vector2 pos = rrObstaclePos[i];
                pos.y += Time.deltaTime * speed * 0.85f;
                rrObstaclePos[i] = pos;

                float screenY = Mathf.Pow(pos.y, 2f);
                float laneTargetX = laneTargets[(int)pos.x];
                float currentX = Mathf.Lerp(50f, laneTargetX, screenY);

                // Accurate, tactile collision detection:
                // Obstacle is in player's vertical contact zone (screenY 65% to 95%)
                if (pos.y >= 0.78f && pos.y <= 0.98f && crashInvulnTimer <= 0f)
                {
                    float xDiff = Mathf.Abs(playerX - currentX);
                    if (xDiff < 15f)
                    {
                        // Direct collision!
                        rrObstacleActive[i] = false;
                        if (rrObstacleElements[i] != null) rrObstacleElements[i].style.display = DisplayStyle.None;

                        foodHealth--;
                        UpdateFoodDisplay();
                        crashInvulnTimer = 1.0f; // 1s grace period

                        AudioManager.Instance?.PlayBuzzer();
                        HapticManager.HeavyImpact();
                        MiniGameFeedback.PlayError(rrPlayer, "CRASH!", rrContainer);
                        MiniGameFeedback.Shake(rrContainer, 16f, 250);

                        if (rrPlayer != null)
                        {
                            rrPlayer.style.unityBackgroundImageTintColor = Color.red;
                            rrPlayer.schedule.Execute(() => {
                                if (rrPlayer != null) rrPlayer.style.unityBackgroundImageTintColor = Color.white;
                            }).StartingIn(350);
                        }

                        if (foodHealth <= 0)
                        {
                            AudioManager.Instance?.PlayMinigameFail();
                            MiniGameFeedback.PlayError(rrPlayer, "ORDER DESTROYED!", rrContainer);
                            manager.StartCoroutine(CrashSequenceAndAbort());
                            yield break;
                        }
                    }
                }

                if (pos.y >= 1f)
                {
                    rrObstacleActive[i] = false;
                    if (rrObstacleElements[i] != null) rrObstacleElements[i].style.display = DisplayStyle.None;
                }
                else
                {
                    if (rrObstacleElements[i] != null) {
                        rrObstacleElements[i].style.left = new Length(currentX, LengthUnit.Percent);
                        rrObstacleElements[i].style.top = new Length(screenY * 100f, LengthUnit.Percent);
                        
                        float scale = Mathf.Lerp(0.3f, 2.5f, screenY);
                        rrObstacleElements[i].style.scale = new StyleScale(new Scale(new Vector3(scale, scale, 1f)));
                    }
                }
            }
            
            yield return null;
        }
    }

    private IEnumerator CrashSequenceAndAbort()
    {
        yield return new WaitForSeconds(0.4f);
        manager.AbortGame();
    }
}

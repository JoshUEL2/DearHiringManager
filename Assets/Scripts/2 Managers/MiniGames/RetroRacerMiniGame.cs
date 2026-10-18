using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class RetroRacerMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement rootElement;

    private VisualElement rrContainer;
    private Label rrPlayer;
    private Label rrDistanceLabel;
    private Label rrTimeLabel;
    private Button btnRrLeft, btnRrRight;
    private VisualElement[] rrRoadLines = new VisualElement[8];
    private Label[] rrObstacleElements = new Label[8];
    private bool[] rrObstacleActive = new bool[8];
    private Vector2[] rrObstaclePos = new Vector2[8];
    private int rrBlocks = 0;
    private float rrDistanceTimer = 0f;
    private bool rrIsDragging = false;
    private float rrTargetLeft = 50f;
    private float timer;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        this.rootElement = root;

        rrContainer = root.Q<VisualElement>("RetroRacerContainer");
        if (rrContainer != null)
        {
            rrPlayer = rrContainer.Q<Label>("RrPlayer");
            rrDistanceLabel = rrContainer.Q<Label>("RrDistanceLabel");
            rrTimeLabel = rrContainer.Q<Label>("RrTimeLabel");
            btnRrLeft = rrContainer.Q<Button>("BtnRrLeft");
            btnRrRight = rrContainer.Q<Button>("BtnRrRight");

            for (int i = 0; i < 8; i++)
            {
                rrRoadLines[i] = rrContainer.Q<VisualElement>($"RrLine{i}");
                rrObstacleElements[i] = rrContainer.Q<Label>($"RrObs{i}");
            }

            rrContainer.RegisterCallback<PointerDownEvent>(OnRrPointerDown);
            rrContainer.RegisterCallback<PointerMoveEvent>(OnRrPointerMove);
            rrContainer.RegisterCallback<PointerUpEvent>(ev => rrIsDragging = false);
            rrContainer.RegisterCallback<PointerOutEvent>(ev => rrIsDragging = false);
        }
    }

    public void StartGame()
    {
        if (rrContainer != null) rrContainer.style.display = DisplayStyle.Flex;
        
        timer = config.timeLimit > 0 ? config.timeLimit : 20f;
        rrBlocks = 0;
        rrDistanceTimer = 0f;
        rrTargetLeft = 50f;
        
        if (rrDistanceLabel != null) rrDistanceLabel.text = "DISTANCE: 0";
        if (rrTimeLabel != null) rrTimeLabel.text = $"TIME: {Mathf.CeilToInt(timer)}";
        
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

    private void OnRrPointerDown(PointerDownEvent ev)
    {
        if (!manager.isPlaying) return;
        rrIsDragging = true;
        UpdateRrPlayerPos(ev.localPosition.x);
    }

    private void OnRrPointerMove(PointerMoveEvent ev)
    {
        if (!rrIsDragging || !manager.isPlaying) return;
        UpdateRrPlayerPos(ev.localPosition.x);
    }

    private void UpdateRrPlayerPos(float mouseX)
    {
        if (rrContainer == null) return;
        float width = rrContainer.worldBound.width;
        if (width <= 0f) return;
        float pct = (mouseX / width) * 100f;
        rrTargetLeft = Mathf.Clamp(pct, 15f, 85f);
    }

    private IEnumerator RetroRacerRoutine()
    {
        float spawnTimer = 0f;
        float[] laneTargets = { 15f, 50f, 85f };
        
        float[] lineProgress = new float[8];
        for(int i=0; i<8; i++) lineProgress[i] = (float)i / 8f;
        
        float speed = 0.5f;

        while (manager.isPlaying)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                AudioManager.Instance?.PlayVictoryFanfare();
                manager.MarkWinReady();
                yield break;
            }
            if (rrTimeLabel != null) rrTimeLabel.text = $"TIME: {Mathf.CeilToInt(timer)}";

            speed += Time.deltaTime * 0.05f;
            
            rrDistanceTimer += Time.deltaTime * speed;
            if (rrDistanceTimer > 0.1f)
            {
                rrDistanceTimer = 0f;
                rrBlocks++;
                if (rrDistanceLabel != null) rrDistanceLabel.text = $"DISTANCE: {rrBlocks}";
            }

            for (int i = 0; i < 8; i++) {
                lineProgress[i] += Time.deltaTime * speed * 2f;
                if (lineProgress[i] > 1f) lineProgress[i] -= 1f;
                
                if (rrRoadLines[i] != null) {
                    float y = Mathf.Pow(lineProgress[i], 2f);
                    rrRoadLines[i].style.top = new Length(y * 100f, LengthUnit.Percent);
                    
                    float w = Mathf.Lerp(10f, 800f, y);
                    rrRoadLines[i].style.width = w;
                    rrRoadLines[i].style.left = new Length(50f, LengthUnit.Percent);
                    rrRoadLines[i].style.marginLeft = -w/2f;
                    
                    float h = Mathf.Lerp(1f, 15f, y);
                    rrRoadLines[i].style.height = h;
                    
                    rrRoadLines[i].style.backgroundColor = new StyleColor(new Color(1f, 1f, 1f, y));
                }
            }

            if (rrPlayer != null)
            {
                float currentLeft = rrPlayer.style.left.value.value;
                float newLeft = Mathf.Lerp(currentLeft, rrTargetLeft, Time.deltaTime * 15f);
                rrPlayer.style.left = new Length(newLeft, LengthUnit.Percent);
            }

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
                            rrObstacleElements[i].text = Random.value > 0.5f ? "🚕" : (Random.value > 0.5f ? "🚧" : "🛢️");
                        }
                        break;
                    }
                }
                spawnTimer = Random.Range(0.4f, 0.8f) / speed;
            }

            for (int i = 0; i < 8; i++)
            {
                if (!rrObstacleActive[i]) continue;

                Vector2 pos = rrObstaclePos[i];
                pos.y += Time.deltaTime * speed * 0.8f;
                rrObstaclePos[i] = pos;

                if (pos.y >= 0.9f && pos.y <= 1.0f && rrPlayer != null)
                {
                    float laneTargetX = laneTargets[(int)pos.x];
                    float playerX = rrPlayer.style.left.value.value;
                    if (Mathf.Abs(playerX - laneTargetX) < 12f)
                    {
                        AudioManager.Instance?.PlayBuzzer();
                        AudioManager.Instance?.PlayMinigameFail();
                        manager.AbortGame();
                        yield break;
                    }
                }

                if (pos.y >= 1f)
                {
                    rrObstacleActive[i] = false;
                    if (rrObstacleElements[i] != null) rrObstacleElements[i].style.display = DisplayStyle.None;
                }
                else
                {
                    float screenY = Mathf.Pow(pos.y, 2f);
                    
                    float laneTargetX = laneTargets[(int)pos.x];
                    float currentX = Mathf.Lerp(50f, laneTargetX, screenY);
                    
                    if (rrObstacleElements[i] != null) {
                        rrObstacleElements[i].style.left = new Length(currentX, LengthUnit.Percent);
                        rrObstacleElements[i].style.top = new Length(screenY * 100f, LengthUnit.Percent);
                        
                        float scale = Mathf.Lerp(0.1f, 3.5f, screenY);
                        rrObstacleElements[i].style.scale = new StyleScale(new Scale(new Vector3(scale, scale, 1f)));
                    }
                }
            }
            
            yield return null;
        }
    }
}

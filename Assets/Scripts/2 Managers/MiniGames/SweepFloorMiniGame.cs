using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;

public class SweepFloorMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement container;
    private VisualElement drawArea;
    private Label toolLabel;

    private float[] sweepTileTimers = new float[25];
    private bool isPlaying = false;
    private int playerGridX = 2;
    private int playerGridY = 2;
    private Vector2 mopVisualPos = new Vector2(200, 200);
    private Vector2 swipeStart;
    private bool isSwiping = false;
    
    private enum Theme { Kitchen, Grease }
    private Theme currentTheme;

    // For visual juice
    private struct Dust {
        public Vector2 pos;
        public float offsetTime;
    }
    private List<Dust>[] tileDusts = new List<Dust>[25];

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        
        container = root.Q<VisualElement>("SweepFloorContainer");
        if (container != null)
        {
            container.Clear();
            container.style.flexGrow = 1;
            container.style.minHeight = 450;
            container.style.alignItems = Align.Center;
            container.style.justifyContent = Justify.Center;
            
            drawArea = new VisualElement();
            drawArea.style.width = 400;
            drawArea.style.height = 400;
            drawArea.style.overflow = Overflow.Hidden;
            drawArea.style.borderTopLeftRadius = 15;
            drawArea.style.borderTopRightRadius = 15;
            drawArea.style.borderBottomLeftRadius = 15;
            drawArea.style.borderBottomRightRadius = 15;
            drawArea.generateVisualContent += OnDrawGame;
            container.Add(drawArea);

            toolLabel = new Label();
            toolLabel.style.position = Position.Absolute;
            toolLabel.style.fontSize = 60;
            toolLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            drawArea.Add(toolLabel);

            drawArea.focusable = true;
            drawArea.RegisterCallback<PointerDownEvent>(ev => {
                if (!isPlaying) return;
                isSwiping = true;
                drawArea.CapturePointer(ev.pointerId);
                swipeStart = ev.position;
            });
            drawArea.RegisterCallback<PointerMoveEvent>(ev => {
                // Ignore continuous dragging
            });
            drawArea.RegisterCallback<PointerUpEvent>(ev => {
                if (!isPlaying || !isSwiping) return;
                isSwiping = false;
                if (drawArea.HasPointerCapture(ev.pointerId)) drawArea.ReleasePointer(ev.pointerId);
                
                Vector2 swipeDelta = (Vector2)ev.position - swipeStart;
                if (swipeDelta.magnitude > 20f)
                {
                    if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
                    {
                        if (swipeDelta.x > 0) MovePlayer(1, 0);
                        else MovePlayer(-1, 0);
                    }
                    else
                    {
                        if (swipeDelta.y > 0) MovePlayer(0, 1);
                        else MovePlayer(0, -1);
                    }
                }
            });
            drawArea.RegisterCallback<NavigationMoveEvent>(ev => {
                // STOP UI TOOLKIT FROM STEALING FOCUS WITH ARROW KEYS!
                
                ev.StopPropagation();
            });
            drawArea.RegisterCallback<KeyDownEvent>(ev => {
                if (!isPlaying) return;
                bool handled = false;
                if (ev.keyCode == KeyCode.UpArrow || ev.keyCode == KeyCode.W) { MovePlayer(0, -1); handled = true; }
                if (ev.keyCode == KeyCode.DownArrow || ev.keyCode == KeyCode.S) { MovePlayer(0, 1); handled = true; }
                if (ev.keyCode == KeyCode.LeftArrow || ev.keyCode == KeyCode.A) { MovePlayer(-1, 0); handled = true; }
                if (ev.keyCode == KeyCode.RightArrow || ev.keyCode == KeyCode.D) { MovePlayer(1, 0); handled = true; }
                
                if (handled) {
                    
                    ev.StopPropagation();
                }
            });
        }
    }

    public void StartGame()
    {
        isPlaying = true;
        container.style.display = DisplayStyle.Flex;
        SetupSweepFloor();
        if (drawArea != null) drawArea.Focus(); // Ensure keyboard focus
        manager.StartCoroutine(SweepFloorRoutine());
    }

    public void AbortGame()
    {
        isPlaying = false;
        container.style.display = DisplayStyle.None;
    }

    private void SetupSweepFloor()
    {
        currentTheme = Theme.Kitchen;
        toolLabel.text = "🧹";
        if (manager.currentGame != null && manager.currentGame.name.Contains("grease"))
        {
            currentTheme = Theme.Grease;
            toolLabel.text = "🪣";
        }
        
        playerGridX = 2;
        playerGridY = 2;
        mopVisualPos = new Vector2(200, 200);
        isSwiping = false;

        Random.InitState(123);
        for (int i = 0; i < 25; i++)
        {
            sweepTileTimers[i] = 0f; // Start completely dirty
            
            tileDusts[i] = new List<Dust>();
            int cx = i % 5;
            int cy = i / 5;
            for (int d = 0; d < 8; d++)
            {
                tileDusts[i].Add(new Dust {
                    pos = new Vector2(cx * 80 + Random.Range(10, 70), cy * 80 + Random.Range(10, 70)),
                    offsetTime = Random.Range(0f, 100f)
                });
            }
        }
    }

    private void MovePlayer(int dx, int dy)
    {
        playerGridX = Mathf.Clamp(playerGridX + dx, 0, 4);
        playerGridY = Mathf.Clamp(playerGridY + dy, 0, 4);
        sweepTileTimers[playerGridY * 5 + playerGridX] = 15f;
        AudioManager.Instance?.PlayScrubPlate();
        HapticManager.LightImpact();
    }

    private void OnDrawGame(MeshGenerationContext ctx)
    {
        var p = ctx.painter2D;
        float w = ctx.visualElement.layout.width;
        float h = ctx.visualElement.layout.height;
        if (w == 0 || h == 0) return;

        // Draw Base Tiles
        for (int y = 0; y < 5; y++)
        {
            for (int x = 0; x < 5; x++)
            {
                Rect tileRect = new Rect(x * 80, y * 80, 80, 80);
                
                if (currentTheme == Theme.Grease)
                {
                    // Checkerboard Black & White Kitchen Floor
                    if ((x + y) % 2 == 0) p.fillColor = new Color(0.9f, 0.9f, 0.9f);
                    else p.fillColor = new Color(0.15f, 0.15f, 0.15f);
                }
                else
                {
                    // Terracotta Ceramic Floor
                    p.fillColor = new Color(0.7f, 0.4f, 0.25f);
                }
                
                p.BeginPath(); p.MoveTo(tileRect.min); 
                p.LineTo(new Vector2(tileRect.xMax, tileRect.yMin));
                p.LineTo(tileRect.max);
                p.LineTo(new Vector2(tileRect.xMin, tileRect.yMax));
                p.Fill();
                
                // Grout / Border
                p.strokeColor = new Color(0.1f, 0.1f, 0.1f, 0.3f);
                p.lineWidth = 2f;
                p.Stroke();
            }
        }

        // Draw Dirt Overlays
        for (int i = 0; i < 25; i++)
        {
            float dirtiness = 1f - Mathf.Clamp01(sweepTileTimers[i] / 15f);
            if (dirtiness <= 0) continue; // Perfectly clean
            
            // Draw dust bunnies or grease blobs
            foreach (var dust in tileDusts[i])
            {
                float pulse = Mathf.Sin(Time.time * 2f + dust.offsetTime) * 2f;
                float radius = (15f + pulse) * dirtiness;
                
                if (currentTheme == Theme.Grease)
                {
                    p.fillColor = new Color(0.6f, 0.5f, 0.1f, 0.7f * dirtiness); // Yellow/Brown grease
                }
                else
                {
                    p.fillColor = new Color(0.3f, 0.3f, 0.3f, 0.8f * dirtiness); // Grey dust
                }
                
                p.BeginPath();
                p.Arc(dust.pos, radius, 0, 360);
                p.Fill();
            }
        }
    }

    private IEnumerator SweepFloorRoutine()
    {
        while (isPlaying && manager.isPlaying)
        {
            Vector2 targetPos = new Vector2(playerGridX * 80 + 40, playerGridY * 80 + 40);
            mopVisualPos = Vector2.Lerp(mopVisualPos, targetPos, Time.deltaTime * 20f);
            
            toolLabel.style.left = mopVisualPos.x - 30f;
            toolLabel.style.top = mopVisualPos.y - 30f;

            bool allClean = true;
            for (int i = 0; i < 25; i++)
            {
                if (sweepTileTimers[i] > 0)
                {
                    sweepTileTimers[i] -= Time.deltaTime;
                    if (sweepTileTimers[i] <= 0) sweepTileTimers[i] = 0;
                }
                if (sweepTileTimers[i] <= 0)
                {
                    allClean = false;
                }
            }
            
            if (drawArea != null) drawArea.MarkDirtyRepaint();

            if (allClean)
            {
                isPlaying = false;
                manager.MarkWinReady();
            }

            yield return null;
        }
    }
}

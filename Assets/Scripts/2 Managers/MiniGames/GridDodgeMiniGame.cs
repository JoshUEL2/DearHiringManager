using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;

public class GridDodgeMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;

    private VisualElement container;
    private VisualElement gridBoard;
    private Label healthText;
    private VisualElement damageOverlay;
    
    private VisualElement playerNode;
    private Label playerIcon;

    private int playerX;
    private int playerY;
    private int health;
    private HashSet<(int, int)> safePath = new HashSet<(int, int)>();
    private List<(int, int)> pathList = new List<(int, int)>();

    private Vector2 swipeStartPos;
    private bool isSwiping = false;

    // Sliding Mechanics
    private Vector2 slideDir = Vector2.zero;
    private float slideProgress = 0f;
    private float animTime = 0f;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;

        container = root.Q<VisualElement>("GridDodgeContainer");
        gridBoard = container.Q<VisualElement>("GridDodgeGrid");
        healthText = container.Q<Label>("GridHealthText");
        damageOverlay = container.Q<VisualElement>("DamageOverlay");

        gridBoard.Clear();
        gridBoard.Add(damageOverlay); 
        
        gridBoard.generateVisualContent += OnDrawFactoryFloor;

        GenerateGrid();
    }

    private void GenerateGrid()
    {
        safePath.Clear();
        pathList.Clear();
        var visited = new HashSet<(int, int)>();
        
        GeneratePathDFS(0, 0, config.gridSize - 1, config.gridSize - 1, visited, pathList);
        
        foreach (var p in pathList)
        {
            safePath.Add(p);
        }

        // Add floating player node
        playerNode = new VisualElement();
        playerNode.AddToClassList("grid-dodge-player");
        playerNode.style.width = Length.Percent(100f / config.gridSize);
        playerNode.style.height = Length.Percent(100f / config.gridSize);
        playerNode.style.transitionProperty = new StyleList<StylePropertyName>(new List<StylePropertyName>()); // Disable CSS transitions so we can lerp manually
        
        playerIcon = new Label();
        playerIcon.AddToClassList("grid-dodge-player-icon");
        SetPlayerIcon();
        playerNode.Add(playerIcon);
        
        gridBoard.Add(playerNode);
    }

    private bool GeneratePathDFS(int cx, int cy, int tx, int ty, HashSet<(int, int)> visited, List<(int, int)> path)
    {
        if (cx < 0 || cy < 0 || cx > tx || cy > ty) return false;
        if (visited.Contains((cx, cy))) return false;

        int visitedNeighbors = 0;
        if (visited.Contains((cx + 1, cy))) visitedNeighbors++;
        if (visited.Contains((cx - 1, cy))) visitedNeighbors++;
        if (visited.Contains((cx, cy + 1))) visitedNeighbors++;
        if (visited.Contains((cx, cy - 1))) visitedNeighbors++;
        
        if (visitedNeighbors > 1) return false;

        visited.Add((cx, cy));
        path.Add((cx, cy));

        if (cx == tx && cy == ty) return true;

        var dirs = new List<(int, int)> { (1, 0), (0, 1), (-1, 0), (0, -1) };
        for (int i = 0; i < dirs.Count; i++)
        {
            int temp = Random.Range(i, dirs.Count);
            var d = dirs[i];
            dirs[i] = dirs[temp];
            dirs[temp] = d;
        }

        foreach (var dir in dirs)
        {
            if (GeneratePathDFS(cx + dir.Item1, cy + dir.Item2, tx, ty, visited, path))
            {
                return true;
            }
        }

        path.RemoveAt(path.Count - 1);
        visited.Remove((cx, cy));
        return false;
    }

    private void SetPlayerIcon()
    {
        bool isTrash = config.instructions.ToLower().Contains("leaky");
        bool isPark = config.instructions.ToLower().Contains("peaceful");
        
        if (isTrash) playerIcon.text = "🗑️";
        else if (isPark) playerIcon.text = "🚶";
        else playerIcon.text = "🏗️"; 
        
        playerIcon.style.backgroundColor = new StyleColor(Color.clear);
        playerIcon.style.fontSize = 45;
    }

    private void OnDrawFactoryFloor(MeshGenerationContext ctx)
    {
        var p = ctx.painter2D;
        float w = ctx.visualElement.layout.width;
        float h = ctx.visualElement.layout.height;
        if (w == 0) return;

        bool isTrash = config.instructions.ToLower().Contains("leaky");
        bool isPark = config.instructions.ToLower().Contains("peaceful");
        
        Color hazardColor = isTrash ? new Color(0.2f, 0.8f, 0.1f, 0.85f) : (isPark ? new Color(0.2f, 0.5f, 0.9f, 0.85f) : new Color(0.9f, 0.3f, 0.1f, 0.85f));
        Color walkColor = isPark ? new Color(0.4f, 0.7f, 0.3f) : new Color(0.45f, 0.45f, 0.5f);
        Color floorColor = isPark ? new Color(0.15f, 0.45f, 0.15f) : new Color(0.1f, 0.12f, 0.15f);

        // Base Floor
        p.fillColor = floorColor;
        p.BeginPath(); p.MoveTo(new Vector2(0,0)); p.LineTo(new Vector2(w,0)); p.LineTo(new Vector2(w,h)); p.LineTo(new Vector2(0,h)); p.Fill();
        
        float cellSize = w / config.gridSize;

        // Hazards (Pits)
        for (int y = 0; y < config.gridSize; y++) {
            for (int x = 0; x < config.gridSize; x++) {
                if (!safePath.Contains((x, y)) && !(x == config.gridSize - 1 && y == config.gridSize - 1)) {
                    float cx = x * cellSize + cellSize * 0.5f;
                    float cy = y * cellSize + cellSize * 0.5f;
                    
                    p.fillColor = new Color(hazardColor.r * 0.5f, hazardColor.g * 0.5f, hazardColor.b * 0.5f);
                    p.BeginPath(); p.Arc(new Vector2(cx, cy), cellSize * 0.48f, 0, 360); p.Fill();
                    
                    p.fillColor = hazardColor;
                    p.BeginPath();
                    float pulse = Mathf.Sin(animTime * 4f + x * 2f + y * 2f) * 4f;
                    p.Arc(new Vector2(cx, cy), cellSize * 0.35f + pulse, 0, 360);
                    p.Fill();
                }
            }
        }
        
        // Catwalk
        if (pathList.Count > 0)
        {
            p.lineCap = LineCap.Round;
            p.lineJoin = LineJoin.Round;
            
            p.strokeColor = new Color(0.05f, 0.05f, 0.05f);
            p.lineWidth = cellSize * 0.75f;
            p.BeginPath();
            for (int i = 0; i < pathList.Count; i++) {
                Vector2 pos = new Vector2(pathList[i].Item1 * cellSize + cellSize * 0.5f, pathList[i].Item2 * cellSize + cellSize * 0.5f);
                if (i == 0) p.MoveTo(pos);
                else p.LineTo(pos);
            }
            p.Stroke();
            
            p.strokeColor = walkColor;
            p.lineWidth = cellSize * 0.55f;
            p.BeginPath();
            for (int i = 0; i < pathList.Count; i++) {
                Vector2 pos = new Vector2(pathList[i].Item1 * cellSize + cellSize * 0.5f, pathList[i].Item2 * cellSize + cellSize * 0.5f);
                if (i == 0) p.MoveTo(pos);
                else p.LineTo(pos);
            }
            p.Stroke();
        }

        // Goal
        float gx = (config.gridSize - 1) * cellSize;
        float gy = (config.gridSize - 1) * cellSize;
        p.fillColor = new Color(0.15f, 0.15f, 0.15f);
        p.BeginPath(); p.MoveTo(new Vector2(gx + 5, gy + 5)); p.LineTo(new Vector2(gx + cellSize - 5, gy + 5));
        p.LineTo(new Vector2(gx + cellSize - 5, gy + cellSize - 5)); p.LineTo(new Vector2(gx + 5, gy + cellSize - 5)); p.Fill();
        
        p.fillColor = isTrash ? new Color(0.9f, 0.3f, 0.1f) : new Color(0.9f, 0.8f, 0.1f);
        p.BeginPath(); p.Arc(new Vector2(gx + cellSize * 0.5f, gy + cellSize * 0.5f), cellSize * 0.25f + Mathf.Sin(animTime * 10f) * 2f, 0, 360); p.Fill();
    }

    public void StartGame()
    {
        playerX = 0;
        playerY = 0;
        slideDir = Vector2.zero;
        slideProgress = 0f;
        health = config.gridHealth;
        UpdateUI();

        container.style.display = DisplayStyle.Flex;
        
        var root = container.panel.visualTree;
        root.RegisterCallback<KeyDownEvent>(OnKeyDown);
        root.RegisterCallback<PointerDownEvent>(OnPointerDown);
        root.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        root.RegisterCallback<PointerUpEvent>(OnPointerUp);
        root.focusable = true;
        root.Focus();

        manager.StartCoroutine(GridDodgeRoutine());
    }

    public void AbortGame()
    {
        container.style.display = DisplayStyle.None;
        
        var root = container.panel?.visualTree;
        if (root != null)
        {
            root.UnregisterCallback<KeyDownEvent>(OnKeyDown);
            root.UnregisterCallback<PointerDownEvent>(OnPointerDown);
            root.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
            root.UnregisterCallback<PointerUpEvent>(OnPointerUp);
        }
    }

    private IEnumerator GridDodgeRoutine()
    {
        while (manager != null && manager.isPlaying)
        {
            animTime += Time.deltaTime;
            
            if (slideDir != Vector2.zero)
            {
                slideProgress += Time.deltaTime * 6.5f; 
                if (slideProgress >= 1f)
                {
                    playerX += (int)slideDir.x;
                    playerY += (int)slideDir.y;
                    slideProgress -= 1f;

                    if (!safePath.Contains((playerX, playerY)))
                    {
                        TakeDamage();
                        playerX = 0; playerY = 0; 
                        slideDir = Vector2.zero; slideProgress = 0f;
                    }
                    
                    if (playerX == config.gridSize - 1 && playerY == config.gridSize - 1)
                    {
                        slideDir = Vector2.zero; slideProgress = 0f;
                        manager.MarkWinReady();
                    }

                    int nx = playerX + (int)slideDir.x;
                    int ny = playerY + (int)slideDir.y;
                    if (nx < 0 || nx >= config.gridSize || ny < 0 || ny >= config.gridSize)
                    {
                        slideDir = Vector2.zero; slideProgress = 0f;
                    }
                }
            }

            if (gridBoard != null) gridBoard.MarkDirtyRepaint();
            UpdateUI();
            
            yield return null;
        }
    }

    private void UpdateUI()
    {
        healthText.text = $"❤️ {health}";
        
        float cellSizePct = 100f / config.gridSize;
        float currentX = playerX + (slideDir.x * slideProgress);
        float currentY = playerY + (slideDir.y * slideProgress);

        playerNode.style.left = Length.Percent(currentX * cellSizePct);
        playerNode.style.top = Length.Percent(currentY * cellSizePct);

        if (slideDir != Vector2.zero) {
            float rotAngle = slideDir.x != 0 ? slideDir.x * 15f : slideDir.y * 15f;
            playerIcon.style.rotate = new StyleRotate(new Rotate(new Angle(rotAngle, AngleUnit.Degree)));
        } else {
            playerIcon.style.rotate = new StyleRotate(new Rotate(new Angle(0, AngleUnit.Degree)));
        }
    }

    private void HandleInput(int dx, int dy)
    {
        if (!manager.isPlaying) return;
        
        int nx = playerX + dx;
        int ny = playerY + dy;
        if (nx >= 0 && nx < config.gridSize && ny >= 0 && ny < config.gridSize)
        {
            if (slideDir == Vector2.zero) slideProgress = 0f;
            slideDir = new Vector2(dx, dy);
        }
    }

    private void TakeDamage()
    {
        health--;
        
        if (damageOverlay != null)
        {
            damageOverlay.style.backgroundColor = new StyleColor(new Color(0.86f, 0.15f, 0.15f, 0.5f));
            manager.StartCoroutine(ResetDamageFlash());
        }

        if (health <= 0)
        {
            manager.AbortGame();
        }
    }

    private IEnumerator ResetDamageFlash()
    {
        yield return new WaitForSeconds(0.15f);
        if (damageOverlay != null)
        {
            damageOverlay.style.backgroundColor = new StyleColor(new Color(0.86f, 0.15f, 0.15f, 0f));
        }
    }

    private void OnKeyDown(KeyDownEvent ev)
    {
        if (ev.keyCode == KeyCode.W || ev.keyCode == KeyCode.UpArrow) HandleInput(0, -1);
        else if (ev.keyCode == KeyCode.S || ev.keyCode == KeyCode.DownArrow) HandleInput(0, 1);
        else if (ev.keyCode == KeyCode.A || ev.keyCode == KeyCode.LeftArrow) HandleInput(-1, 0);
        else if (ev.keyCode == KeyCode.D || ev.keyCode == KeyCode.RightArrow) HandleInput(1, 0);
    }

    private void OnPointerDown(PointerDownEvent ev)
    {
        swipeStartPos = ev.position;
        isSwiping = true;
    }

    private void OnPointerMove(PointerMoveEvent ev)
    {
        if (!isSwiping) return;

        Vector2 delta = (Vector2)ev.position - swipeStartPos;
        if (delta.magnitude > 30f)
        {
            int dx = 0, dy = 0;
            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y)) dx = delta.x > 0 ? 1 : -1;
            else dy = delta.y > 0 ? 1 : -1;
            
            HandleInput(dx, dy);
            swipeStartPos = ev.position;
        }
    }

    private void OnPointerUp(PointerUpEvent ev)
    {
        isSwiping = false;
    }
}

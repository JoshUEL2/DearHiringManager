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

    private Vector2 swipeStartPos;
    private bool isSwiping = false;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;

        container = root.Q<VisualElement>("GridDodgeContainer");
        gridBoard = container.Q<VisualElement>("GridDodgeGrid");
        healthText = container.Q<Label>("GridHealthText");
        damageOverlay = container.Q<VisualElement>("DamageOverlay");

        // Clean up previous elements just in case
        gridBoard.Clear();
        gridBoard.Add(damageOverlay); // Ensure overlay stays on top

        GenerateGrid();
    }

    private void GenerateGrid()
    {
        safePath.Clear();
        var visited = new HashSet<(int, int)>();
        var path = new List<(int, int)>();
        
        GeneratePathDFS(0, 0, config.gridSize - 1, config.gridSize - 1, visited, path);
        
        foreach (var p in path)
        {
            safePath.Add(p);
        }

        float cellSizePct = 100f / config.gridSize;

        // Build visual tiles
        for (int y = 0; y < config.gridSize; y++)
        {
            var row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.height = Length.Percent(cellSizePct);
            row.style.width = Length.Percent(100f);

            for (int x = 0; x < config.gridSize; x++)
            {
                var cell = new VisualElement();
                cell.style.width = Length.Percent(cellSizePct);
                cell.style.height = Length.Percent(100f);
                
                if (x == config.gridSize - 1 && y == config.gridSize - 1)
                {
                    cell.AddToClassList("grid-dodge-cell");
                    cell.AddToClassList("grid-dodge-cell-goal");
                    var goalIcon = new Label("⛲");
                    goalIcon.style.fontSize = 24;
                    goalIcon.style.unityTextAlign = TextAnchor.MiddleCenter;
                    goalIcon.style.position = Position.Absolute;
                    goalIcon.style.left = 0; goalIcon.style.right = 0; goalIcon.style.top = 0; goalIcon.style.bottom = 0;
                    cell.Add(goalIcon);
                }
                else if (safePath.Contains((x, y)))
                {
                    cell.AddToClassList("grid-dodge-cell");
                    cell.AddToClassList("grid-dodge-cell-safe");
                }
                else
                {
                    cell.AddToClassList("grid-dodge-cell");
                    cell.AddToClassList("grid-dodge-cell-hazard");
                }

                row.Add(cell);
            }
            gridBoard.Add(row);
        }

        // Add floating player node
        playerNode = new VisualElement();
        playerNode.AddToClassList("grid-dodge-player");
        playerNode.style.width = Length.Percent(cellSizePct);
        playerNode.style.height = Length.Percent(cellSizePct);
        
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

        // Prevent 2x2 clusters of paths by ensuring we don't step adjacent to already path'd cells (other than our parent)
        // A simple heuristic: if more than 1 neighbor is visited, it's touching itself!
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
        visited.Remove((cx, cy)); // Backtrack completely
        return false;
    }

    private void SetPlayerIcon()
    {
        string gameName = manager.GetComponent<GameManager>()?.currentData?.jobTitle ?? "";
        // Just use a generic logic to set icon if we don't have direct access to currentGame name
        playerIcon.text = "🚶‍♂️"; // Default
    }

    public void StartGame()
    {
        playerX = 0;
        playerY = 0;
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

    private void UpdateUI()
    {
        healthText.text = $"❤️ {health}";
        
        float cellSizePct = 100f / config.gridSize;
        playerNode.style.left = Length.Percent(playerX * cellSizePct);
        playerNode.style.top = Length.Percent(playerY * cellSizePct);
    }

    private void HandleInput(int dx, int dy)
    {
        if (!manager.isPlaying) return;

        int nx = playerX + dx;
        int ny = playerY + dy;

        if (nx >= 0 && nx < config.gridSize && ny >= 0 && ny < config.gridSize)
        {
            playerX = nx;
            playerY = ny;

            if (!safePath.Contains((nx, ny)))
            {
                TakeDamage();
            }

            UpdateUI();

            if (playerX == config.gridSize - 1 && playerY == config.gridSize - 1 && manager.isPlaying)
            {
                manager.MarkWinReady();
            }
        }
    }

    private void TakeDamage()
    {
        health--;
        
        // Visual flash
        if (damageOverlay != null)
        {
            damageOverlay.style.backgroundColor = new StyleColor(new Color(0.86f, 0.15f, 0.15f, 0.5f));
            manager.StartCoroutine(ResetDamageFlash());
        }

        if (health <= 0)
        {
            // MiniGameManager legacy code handles fail via HP=0 check inside the manager, but wait!
            // We extracted it. If health hits 0, we need to fail the game.
            // Currently, the manager only handles WIN (MarkWinReady).
            // So if health hits 0, we can just abort/fail.
            manager.isPlaying = false;
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
        if (delta.magnitude > 40f) // Threshold
        {
            int dx = 0, dy = 0;
            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y)) dx = delta.x > 0 ? 1 : -1;
            else dy = delta.y > 0 ? 1 : -1;
            
            HandleInput(dx, dy);
            
            isSwiping = false; // Reset so they must lift finger for next move
        }
    }

    private void OnPointerUp(PointerUpEvent ev)
    {
        isSwiping = false;
    }
}

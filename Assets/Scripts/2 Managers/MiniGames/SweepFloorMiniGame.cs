using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class SweepFloorMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement rootElement;
    private VisualElement container;
    private VisualElement grid;

    private float[] tileTimers;
    private VisualElement[] tiles;
    private int playerX, playerY;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        this.rootElement = root;
        container = root.Q<VisualElement>("SweepFloorContainer");
        grid = root.Q<VisualElement>("SweepFloorGrid");
        
        rootElement.RegisterCallback<KeyDownEvent>(OnKeyDown);
    }

    public void StartGame()
    {
        container.style.display = DisplayStyle.Flex;
        
        grid.Clear();
        tileTimers = new float[25];
        tiles = new VisualElement[25];
        playerX = 0;
        playerY = 0;
        
        for (int y = 0; y < 5; y++)
        {
            var row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.flexGrow = 1;
            for (int x = 0; x < 5; x++)
            {
                var cell = new VisualElement();
                cell.style.flexGrow = 1;
                cell.style.borderTopWidth = 2; cell.style.borderLeftWidth = 2; cell.style.borderRightWidth = 2; cell.style.borderBottomWidth = 2;
                cell.style.borderTopColor = new StyleColor(new Color(0.2f, 0.1f, 0.05f));
                cell.style.borderBottomColor = new StyleColor(new Color(0.2f, 0.1f, 0.05f));
                cell.style.borderLeftColor = new StyleColor(new Color(0.2f, 0.1f, 0.05f));
                cell.style.borderRightColor = new StyleColor(new Color(0.2f, 0.1f, 0.05f));
                cell.style.justifyContent = Justify.Center;
                cell.style.alignItems = Align.Center;
                cell.style.paddingTop = 0; cell.style.paddingBottom = 0; cell.style.paddingLeft = 0; cell.style.paddingRight = 0;
                cell.style.marginTop = 0; cell.style.marginBottom = 0; cell.style.marginLeft = 0; cell.style.marginRight = 0;
                
                var icon = new Label("");
                icon.style.position = Position.Absolute;
                icon.style.left = 0; icon.style.right = 0; icon.style.top = 0; icon.style.bottom = 0;
                icon.style.fontSize = 55;
                icon.style.paddingTop = 0; icon.style.paddingBottom = 0; icon.style.paddingLeft = 0; icon.style.paddingRight = 0;
                icon.style.marginTop = 0; icon.style.marginBottom = 0; icon.style.marginLeft = 0; icon.style.marginRight = 0;
                icon.style.unityTextAlign = TextAnchor.MiddleCenter;
                icon.name = "Icon";
                cell.Add(icon);
                
                int cx = x; int cy = y;
                cell.RegisterCallback<PointerDownEvent>(ev => {
                    if (!manager.isPlaying) return;
                    playerX = cx; playerY = cy;
                });
                cell.RegisterCallback<PointerEnterEvent>(ev => {
                    if (!manager.isPlaying) return;
                    if ((ev.pressedButtons & 1) != 0) { // left click or touch hold
                        playerX = cx; playerY = cy;
                    }
                });
                
                row.Add(cell);
                tiles[y * 5 + x] = cell;
            }
            grid.Add(row);
        }

        manager.StartCoroutine(GameRoutine());
    }

    public void AbortGame()
    {
        container.style.display = DisplayStyle.None;
        rootElement.UnregisterCallback<KeyDownEvent>(OnKeyDown);
    }

    private void OnKeyDown(KeyDownEvent ev)
    {
        if (!manager.isPlaying) return;
        
        int dx = 0, dy = 0;
        if (ev.keyCode == KeyCode.UpArrow || ev.keyCode == KeyCode.W) dy = -1;
        else if (ev.keyCode == KeyCode.DownArrow || ev.keyCode == KeyCode.S) dy = 1;
        else if (ev.keyCode == KeyCode.LeftArrow || ev.keyCode == KeyCode.A) dx = -1;
        else if (ev.keyCode == KeyCode.RightArrow || ev.keyCode == KeyCode.D) dx = 1;
        
        if (dx == 0 && dy == 0) return;
        
        ev.StopPropagation();

        int nx = playerX + dx;
        int ny = playerY + dy;
        if (nx >= 0 && nx < 5 && ny >= 0 && ny < 5)
        {
            playerX = nx;
            playerY = ny;
            tileTimers[ny * 5 + nx] = 15f; // Instantly clean
            Render();
        }
    }

    private void Render()
    {
        for (int i = 0; i < 25; i++)
        {
            var cell = tiles[i];
            var icon = cell.Q<Label>("Icon");
            
            if (tileTimers[i] > 0)
            {
                // Clean: light brown
                cell.style.backgroundColor = new StyleColor(new Color(0.76f, 0.6f, 0.42f));
            }
            else
            {
                // Dirty: dark brown
                cell.style.backgroundColor = new StyleColor(new Color(0.35f, 0.22f, 0.12f));
            }
            
            int cy = i / 5;
            int cx = i % 5;
            if (cx == playerX && cy == playerY)
            {
                if (config.instructions != null && config.instructions.Contains("Mop")) icon.text = "🪣";
                else if (config.instructions != null && config.instructions.Contains("conveyor")) icon.text = "🌪️";
                else icon.text = "🧹";
            }
            else
            {
                icon.text = "";
            }
        }
    }

    private IEnumerator GameRoutine()
    {
        tileTimers[playerY * 5 + playerX] = 15f;
        Render();

        while (manager.isPlaying)
        {
            bool allClean = true;
            for (int i = 0; i < 25; i++)
            {
                if (tileTimers[i] > 0)
                {
                    tileTimers[i] -= Time.deltaTime;
                    if (tileTimers[i] <= 0) tileTimers[i] = 0;
                }
                if (tileTimers[i] <= 0)
                {
                    allClean = false;
                }
            }

            tileTimers[playerY * 5 + playerX] = 15f;
            Render();

            if (allClean)
            {
                manager.MarkWinReady();
            }

            yield return null;
        }
    }
}

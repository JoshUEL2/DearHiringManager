using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class SweepFloorMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement container;

    private VisualElement sweepFloorGrid;
    private float[] sweepTileTimers = new float[25];
    private VisualElement[] sweepTiles = new VisualElement[25];
    private int sweepPlayerX = 0;
    private int sweepPlayerY = 0;
    private bool isPlaying = false;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        
        container = root.Q<VisualElement>("SweepFloorContainer");
        sweepFloorGrid = container.Q<VisualElement>("SweepFloorGrid");
    }

    public void StartGame()
    {
        isPlaying = true;
        SetupSweepFloor();
        container.style.display = DisplayStyle.Flex;
        if (manager.miniGameDocument != null) manager.miniGameDocument.rootVisualElement.Focus();
        manager.StartCoroutine(SweepFloorRoutine());
    }

    public void AbortGame()
    {
        isPlaying = false;
        container.style.display = DisplayStyle.None;
    }

    private void SetupSweepFloor()
    {
        if (sweepFloorGrid == null) return;
        sweepFloorGrid.Clear();
        sweepTileTimers = new float[25];
        sweepTiles = new VisualElement[25];
        sweepPlayerX = 0;
        sweepPlayerY = 0;
        
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
                    if (!isPlaying || !manager.isPlaying) return;
                    sweepPlayerX = cx; sweepPlayerY = cy;
                });
                cell.RegisterCallback<PointerEnterEvent>(ev => {
                    if (!isPlaying || !manager.isPlaying) return;
                    if ((ev.pressedButtons & 1) != 0) {
                        sweepPlayerX = cx; sweepPlayerY = cy;
                    }
                });
                
                row.Add(cell);
                sweepTiles[y * 5 + x] = cell;
            }
            sweepFloorGrid.Add(row);
        }
    }

    private void RenderSweepFloor()
    {
        for (int i = 0; i < 25; i++)
        {
            var cell = sweepTiles[i];
            var icon = cell.Q<Label>("Icon");
            
            if (sweepTileTimers[i] > 0)
            {
                cell.style.backgroundColor = new StyleColor(new Color(0.76f, 0.6f, 0.42f));
            }
            else
            {
                cell.style.backgroundColor = new StyleColor(new Color(0.35f, 0.22f, 0.12f));
            }
            
            int cy = i / 5;
            int cx = i % 5;
            if (cx == sweepPlayerX && cy == sweepPlayerY)
            {
                if (manager.currentGame != null && manager.currentGame.name == "Mop grease spills") icon.text = "🪣";
                else if (manager.currentGame != null && manager.currentGame.name == "Clear conveyor dust filters") icon.text = "🌪️";
                else icon.text = "🧹";
            }
            else
            {
                icon.text = "";
            }
        }
    }

    private IEnumerator SweepFloorRoutine()
    {
        sweepTileTimers[sweepPlayerY * 5 + sweepPlayerX] = 15f;
        RenderSweepFloor();

        while (isPlaying && manager.isPlaying)
        {
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

            sweepTileTimers[sweepPlayerY * 5 + sweepPlayerX] = 15f;
            RenderSweepFloor();

            if (allClean)
            {
                isPlaying = false;
                manager.MarkWinReady();
            }

            yield return null;
        }
    }
}

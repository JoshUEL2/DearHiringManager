using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;

public class MowGreensMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement rootElement;
    
    private VisualElement mgContainer;
    private VisualElement mgGrid;
    private Label mgTimeLabel;
    private Label mgProgressLabel;
    private VisualElement mgTractor;
    
    private int mgTilesCut = 0;
    private bool[] mgCutStatus;
    private bool[] mgIsObstacle;
    private bool mgIsDragging = false;
    private float timer;
    private Coroutine activeRoutine;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        this.rootElement = root;
        
        mgContainer = root.Q<VisualElement>("MowGreensContainer");
        mgGrid = root.Q<VisualElement>("MgGrid");
        mgTimeLabel = root.Q<Label>("MgTimeText");
        mgProgressLabel = root.Q<Label>("MgProgressLabel");
        mgTractor = root.Q<VisualElement>("MgTractor");
        
        if (mgGrid != null)
        {
            mgGrid.UnregisterCallback<PointerDownEvent>(OnPointerDown);
            mgGrid.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
            mgGrid.UnregisterCallback<PointerUpEvent>(OnPointerUp);
            mgGrid.UnregisterCallback<PointerLeaveEvent>(OnPointerLeave);

            mgGrid.RegisterCallback<PointerDownEvent>(OnPointerDown);
            mgGrid.RegisterCallback<PointerMoveEvent>(OnPointerMove);
            mgGrid.RegisterCallback<PointerUpEvent>(OnPointerUp);
            mgGrid.RegisterCallback<PointerLeaveEvent>(OnPointerLeave);
        }
    }

    private void OnPointerDown(PointerDownEvent ev)
    {
        mgIsDragging = true;
        mgGrid.CapturePointer(ev.pointerId);
        Vector2 localPos = mgGrid.WorldToLocal(ev.position);
        if (mgTractor != null) {
            mgTractor.style.display = DisplayStyle.Flex;
            mgTractor.style.left = localPos.x;
            mgTractor.style.top = localPos.y;
        }
        HandleMowInteraction(localPos);
    }

    private void OnPointerMove(PointerMoveEvent ev)
    {
        if (mgIsDragging && manager.isPlaying) {
            Vector2 localPos = mgGrid.WorldToLocal(ev.position);
            if (mgTractor != null) {
                mgTractor.style.left = localPos.x;
                mgTractor.style.top = localPos.y;
            }
            HandleMowInteraction(localPos);
        }
    }

    private void OnPointerUp(PointerUpEvent ev)
    {
        mgIsDragging = false;
        if (mgTractor != null) mgTractor.style.display = DisplayStyle.None;
        mgGrid.ReleasePointer(ev.pointerId);
    }

    private void OnPointerLeave(PointerLeaveEvent ev)
    {
    }

    public void StartGame()
    {
        mgContainer.style.display = DisplayStyle.Flex;
        
        timer = config.timeLimit > 0 ? config.timeLimit : 15f;
        mgTilesCut = 0;
        mgCutStatus = new bool[36];
        mgIsObstacle = new bool[36];
        mgIsDragging = false;

        if (mgTimeLabel != null) mgTimeLabel.text = $"TIME: {timer:F1}s";
        if (mgProgressLabel != null) mgProgressLabel.text = "MOWED: 0/31";

        if (mgGrid != null)
        {
            var children = new List<VisualElement>(mgGrid.Children());
            foreach (var child in children)
            {
                if (child != mgTractor) mgGrid.Remove(child);
            }
            
            List<int> obstacleIndices = new List<int>();
            while (obstacleIndices.Count < 5)
            {
                int rand = Random.Range(0, 36);
                if (!obstacleIndices.Contains(rand))
                {
                    obstacleIndices.Add(rand);
                    mgIsObstacle[rand] = true;
                }
            }

            for (int i = 0; i < 36; i++)
            {
                VisualElement tile = new VisualElement();
                tile.style.width = 75;
                tile.style.height = 75;
                tile.style.borderTopWidth = 2;
                tile.style.borderBottomWidth = 2;
                tile.style.borderLeftWidth = 2;
                tile.style.borderRightWidth = 2;
                tile.style.borderTopColor = new StyleColor(new Color(0.1f, 0.4f, 0.2f)); 
                tile.style.borderBottomColor = new StyleColor(new Color(0.1f, 0.4f, 0.2f)); 
                tile.style.borderLeftColor = new StyleColor(new Color(0.1f, 0.4f, 0.2f)); 
                tile.style.borderRightColor = new StyleColor(new Color(0.1f, 0.4f, 0.2f));
                tile.style.position = Position.Relative;
                
                VisualElement icon = new VisualElement();
                icon.AddToClassList("mg-icon");

                if (mgIsObstacle[i])
                {
                    tile.style.backgroundColor = new StyleColor(new Color(0.08f, 0.5f, 0.3f));
                    icon.AddToClassList(Random.value > 0.5f ? "mg-icon-coffee" : "mg-icon-paper");
                    icon.style.width = 44;
                    icon.style.height = 44;
                }
                else
                {
                    tile.style.backgroundColor = new StyleColor(new Color(0.08f, 0.6f, 0.3f));
                    icon.AddToClassList("mg-icon-grass");
                    icon.style.width = 42;
                    icon.style.height = 42;
                }

                icon.style.position = Position.Absolute;
                icon.style.top = Length.Percent(50);
                icon.style.left = Length.Percent(50);
                icon.style.translate = new StyleTranslate(new Translate(new Length(-50, LengthUnit.Percent), new Length(-50, LengthUnit.Percent), 0));
                tile.Add(icon);

                mgGrid.Insert(mgGrid.childCount - 1, tile);
            }
        }
        
        activeRoutine = manager.StartCoroutine(GameRoutine());
    }

    public void AbortGame()
    {
        mgContainer.style.display = DisplayStyle.None;
        mgIsDragging = false;
        if (mgTractor != null) mgTractor.style.display = DisplayStyle.None;
        if (activeRoutine != null)
        {
            manager.StopCoroutine(activeRoutine);
            activeRoutine = null;
        }
    }

    private void HandleMowInteraction(Vector2 localPos)
    {
        if (mgGrid == null || !manager.isPlaying) return;
        
        float adjustedX = localPos.x - 4f;
        float adjustedY = localPos.y - 4f;
        
        int col = (int)(adjustedX / 75f);
        int row = (int)(adjustedY / 75f);
        
        if (col >= 0 && col < 6 && row >= 0 && row < 6)
        {
            int index = row * 6 + col;
            
            if (mgIsObstacle[index])
            {
                mgGrid.ElementAt(index).style.backgroundColor = new StyleColor(Color.red);
                AudioManager.Instance?.PlayBuzzer();
                HapticManager.HeavyImpact();
                manager.EndGame(false);
            }
            else if (!mgCutStatus[index])
            {
                mgCutStatus[index] = true;
                AudioManager.Instance?.PlayLawnMower();
                HapticManager.SelectionChanged();
                VisualElement tile = mgGrid.ElementAt(index);
                tile.style.backgroundColor = new StyleColor(new Color(0.13f, 0.77f, 0.36f));
                tile.Clear();
                
                mgTilesCut++;
                if (mgProgressLabel != null) mgProgressLabel.text = $"MOWED: {mgTilesCut}/31";
                
                if (mgTilesCut >= 31)
                {
                    manager.EndGame(true);
                }
            }
        }
    }

    private IEnumerator GameRoutine()
    {
        while (manager.isPlaying)
        {
            timer -= Time.deltaTime;
            if (mgTimeLabel != null) mgTimeLabel.text = $"TIME: {Mathf.Max(0, timer):F1}s";
            
            if (timer <= 0f)
            {
                manager.EndGame(false);
                yield break;
            }
            yield return null;
        }
    }
}

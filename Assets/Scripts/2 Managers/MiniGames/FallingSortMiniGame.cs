using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;

public class FallingSortMiniGame : IMiniGame
{
    private class FallingItemData
    {
        public VisualElement element;
        public Vector2 position;
        public Vector2 velocity;
        public int typeId;
        public bool isDragging;
    }

    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement container;

    private VisualElement fallZone;
    private VisualElement binContainer;
    private Label sortScoreText;
    private Label sortTimerText;

    private int sortScore = 0;
    private float timer = 30f;
    private bool isPlaying = false;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        
        container = root.Q<VisualElement>("FallingSortContainer");
        fallZone = container.Q<VisualElement>("FallZone");
        binContainer = container.Q<VisualElement>("BinContainer");
        sortScoreText = container.Q<Label>("SortScoreText");
        sortTimerText = container.Q<Label>("SortTimerText");
    }

    public void StartGame()
    {
        isPlaying = true;
        sortScore = 0;
        timer = config.sortTimeLimit > 0 ? config.sortTimeLimit : 30f;
        
        if (sortScoreText != null) sortScoreText.text = $"Sorted: 0 / {config.sortTarget}";
        
        if (binContainer != null && config.binLabels != null)
        {
            binContainer.Clear();
            foreach (var label in config.binLabels)
            {
                var lbl = new Label(label);
                lbl.style.flexGrow = 1;
                lbl.style.unityTextAlign = TextAnchor.MiddleCenter;
                lbl.style.fontSize = 24;
                lbl.style.borderTopWidth = 2; lbl.style.borderLeftWidth = 2; lbl.style.borderRightWidth = 2;
                lbl.style.borderTopColor = Color.white; lbl.style.borderLeftColor = Color.white; lbl.style.borderRightColor = Color.white;
                binContainer.Add(lbl);
            }
        }
        
        if (fallZone != null) fallZone.Clear();

        container.style.display = DisplayStyle.Flex;
        manager.StartCoroutine(FallingSortRoutine());
    }

    public void AbortGame()
    {
        isPlaying = false;
        container.style.display = DisplayStyle.None;
    }

    private IEnumerator FallingSortRoutine()
    {
        List<FallingItemData> activeItems = new List<FallingItemData>();
        float spawnTimer = 0f;
        int itemsSpawned = 0;
        int totalToDrop = 10;
        float gravity = 600f; 

        while (isPlaying && manager.isPlaying && sortScore < config.sortTarget && timer > 0)
        {
            if (sortScoreText != null) sortScoreText.text = $"Sorted: {sortScore} / {config.sortTarget}";
            if (sortTimerText != null) sortTimerText.text = $"Time: {Mathf.Ceil(timer)}s";
            timer -= Time.deltaTime;

            if (itemsSpawned < totalToDrop && spawnTimer <= 0f)
            {
                int typeIdx = Random.Range(0, config.fallingItems.Length);
                string text = config.fallingItems[typeIdx];
                string emojiText = text.Contains(" ") ? text.Split(' ')[0] : text;
                
                var label = new Label(emojiText);
                label.style.position = Position.Absolute;
                label.style.fontSize = 50;
                
                float startX = Random.Range(50f, Mathf.Max(60f, fallZone.resolvedStyle.width - 100f));
                
                FallingItemData itemData = new FallingItemData {
                    element = label,
                    position = new Vector2(startX, -50),
                    velocity = new Vector2(Random.Range(-50f, 50f), 0),
                    typeId = typeIdx
                };
                
                label.RegisterCallback<PointerDownEvent>(ev => {
                    if (!isPlaying || !manager.isPlaying) return;
                    itemData.isDragging = true;
                    itemData.velocity = Vector2.zero;
                    label.CapturePointer(ev.pointerId);
                    ev.StopPropagation();
                });
                
                label.RegisterCallback<PointerMoveEvent>(ev => {
                    if (itemData.isDragging && label.HasPointerCapture(ev.pointerId)) {
                        itemData.position += (Vector2)ev.deltaPosition;
                        itemData.velocity = (Vector2)ev.deltaPosition / Time.deltaTime; 
                    }
                });
                
                label.RegisterCallback<PointerUpEvent>(ev => {
                    if (itemData.isDragging) {
                        itemData.isDragging = false;
                        label.ReleasePointer(ev.pointerId);
                    }
                });

                if (fallZone != null) fallZone.Add(label);
                activeItems.Add(itemData);
                itemsSpawned++;
                spawnTimer = 2.0f; 
            }
            if (spawnTimer > 0f) spawnTimer -= Time.deltaTime;

            float w = fallZone != null ? fallZone.resolvedStyle.width : 500f;
            float h = fallZone != null ? fallZone.resolvedStyle.height : 500f;

            for (int i = activeItems.Count - 1; i >= 0; i--)
            {
                var item = activeItems[i];
                if (!item.isDragging)
                {
                    item.velocity.y += gravity * Time.deltaTime;
                    item.position += item.velocity * Time.deltaTime;
                    
                    if (item.position.x < 0) { item.position.x = 0; item.velocity.x = 0; }
                    if (item.position.x > w - 80f) { item.position.x = w - 80f; item.velocity.x = 0; }
                    if (item.position.y > h - 40f) {
                        item.position.y = h - 40f;
                        item.velocity.y = 0;
                        item.velocity.x = 0;
                    }
                }
                
                item.element.style.left = item.position.x;
                item.element.style.top = item.position.y;
                
                if (item.position.y >= h - 60f && !item.isDragging) 
                {
                    float binWidth = w / config.binLabels.Length;
                    int binIdx = Mathf.FloorToInt((item.position.x + 40f) / binWidth);
                    binIdx = Mathf.Clamp(binIdx, 0, config.binLabels.Length - 1);
                    
                    if (binIdx == item.typeId)
                    {
                        sortScore++;
                    }
                    
                    if (fallZone != null) fallZone.Remove(item.element);
                    activeItems.RemoveAt(i);
                    
                    if (sortScore >= config.sortTarget)
                    {
                        isPlaying = false;
                        manager.MarkWinReady();
                    }
                }
            }

            if (itemsSpawned >= totalToDrop && activeItems.Count == 0)
            {
                if (sortScore >= config.sortTarget || (manager.currentGame != null && !manager.currentGame.canFail))
                {
                    isPlaying = false;
                    manager.MarkWinReady();
                }
                else 
                { 
                    isPlaying = false; 
                    manager.AbortGame(); 
                    yield break; 
                }
            }

            if (timer <= 0 && manager.currentGame != null && manager.currentGame.canFail)
            {
                isPlaying = false;
                manager.AbortGame();
                yield break;
            }
            else if (timer <= 0 && manager.currentGame != null && !manager.currentGame.canFail && sortScore < config.sortTarget)
            {
                timer = 20f; 
            }
            yield return null;
        }
    }
}

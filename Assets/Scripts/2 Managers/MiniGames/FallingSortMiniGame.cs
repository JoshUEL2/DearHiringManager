using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;

public class FallingSortMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement container;

    private VisualElement fallZone;
    private VisualElement binsRow;
    private Label sortScoreText;
    private Label sortTimerText;

    private int sortScore = 0;
    private float timer = 0f;

    private class FallingItemData
    {
        public VisualElement element;
        public Vector2 position;
        public Vector2 velocity;
        public bool isDragging;
        public int typeId;
    }

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        
        container = root.Q<VisualElement>("FallingSortContainer");
        fallZone = root.Q<VisualElement>("FallZone");
        binsRow = root.Q<VisualElement>("BinsRow");
        sortScoreText = root.Q<Label>("SortScoreText");
        sortTimerText = root.Q<Label>("SortTimerText");
    }

    public void StartGame()
    {
        sortScore = 0;
        timer = config.timeLimit > 0 ? config.timeLimit : 15f;
        container.style.display = DisplayStyle.Flex;

        SetupFallingSort();
        manager.StartCoroutine(FallingSortRoutine());
    }

    public void AbortGame()
    {
        container.style.display = DisplayStyle.None;
    }

    private void SetupFallingSort()
    {
        fallZone.Clear();
        binsRow.Clear();
        
        Color[] binColors = new Color[] {
            new Color(0.9f, 0.3f, 0.3f), // Red
            new Color(0.3f, 0.8f, 0.3f), // Green
            new Color(0.9f, 0.8f, 0.2f), // Yellow
        };
        
        for (int i = 0; i < config.binLabels.Length; i++)
        {
            var bin = new VisualElement();
            bin.style.flexGrow = 1;
            bin.style.backgroundColor = new StyleColor(binColors[i % binColors.Length]);
            bin.style.borderLeftWidth = 4;
            bin.style.borderRightWidth = 4;
            bin.style.borderTopWidth = 0;
            bin.style.borderBottomWidth = 4;
            bin.style.borderLeftColor = new StyleColor(Color.black);
            bin.style.borderRightColor = new StyleColor(Color.black);
            bin.style.borderBottomColor = new StyleColor(Color.black);
            
            // Bin Shape
            bin.style.borderBottomLeftRadius = 20;
            bin.style.borderBottomRightRadius = 20;
            bin.style.borderTopLeftRadius = 5;
            bin.style.borderTopRightRadius = 5;
            
            bin.style.marginRight = 10;
            bin.style.marginLeft = 10;
            bin.style.justifyContent = Justify.Center;
            bin.style.alignItems = Align.Center;
            
            // Extract just the emoji if present, else use full text
            string fullText = config.fallingItems[i];
            string emojiText = fullText;
            if (fullText.Contains(" ")) emojiText = fullText.Split(' ')[0];
            
            var label = new Label(emojiText); 
            label.style.fontSize = 50;
            label.style.color = Color.white;
            bin.Add(label);
            
            binsRow.Add(bin);
        }
    }

    private IEnumerator FallingSortRoutine()
    {
        List<FallingItemData> activeItems = new List<FallingItemData>();
        float spawnTimer = 0f;
        int itemsSpawned = 0;
        int totalToDrop = 10;
        float gravity = 600f; // Slower falling

        while (manager.isPlaying && sortScore < config.sortTarget && timer > 0)
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
                
                float startX = Random.Range(50f, fallZone.resolvedStyle.width - 100f);
                
                FallingItemData itemData = new FallingItemData {
                    element = label,
                    position = new Vector2(startX, -50),
                    velocity = new Vector2(Random.Range(-50f, 50f), 0),
                    typeId = typeIdx
                };
                
                label.RegisterCallback<PointerDownEvent>(ev => {
                    if (!manager.isPlaying) return;
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

                fallZone.Add(label);
                activeItems.Add(itemData);
                itemsSpawned++;
                spawnTimer = 2.0f; // 2 second gap
            }
            if (spawnTimer > 0f) spawnTimer -= Time.deltaTime;

            float w = fallZone.resolvedStyle.width;
            float h = fallZone.resolvedStyle.height;

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
                    
                    // Whether wrong or right, destroy the item when it falls in. No bouncing.
                    fallZone.Remove(item.element);
                    activeItems.RemoveAt(i);
                    
                    if (sortScore >= config.sortTarget) manager.MarkWinReady();
                }
            }

            // If we've dropped all 10 items and the box is empty, the game ends.
            if (itemsSpawned >= totalToDrop && activeItems.Count == 0)
            {
                if (sortScore >= config.sortTarget || !config.canFail) manager.MarkWinReady();
                else { manager.AbortGame(); yield break; }
            }

            if (timer <= 0 && config.canFail)
            {
                manager.AbortGame();
                yield break;
            }
            else if (timer <= 0 && !config.canFail && sortScore < config.sortTarget)
            {
                timer = 20f; 
            }
            yield return null;
        }
    }
}

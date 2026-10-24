using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class FallingSortMiniGame : IMiniGame
{
    private class FallingItemData
    {
        public VisualElement element;
        public Vector2 position;
        public Vector2 velocity;
        public int typeId;
        public bool isDragging;
        public float rotation;
    }

    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement container;

    // Button Sort Fields (Condiments)
    private VisualElement fallZone;
    private VisualElement binsRow;
    private Label sortScoreText;
    private Label sortTimerText;
    private int currentItemType;
    private Label currentItemLabel;
    private bool isAnimating;

    // Shared
    private int sortScore = 0;
    private float timer = 30f;
    private bool isPlaying = false;
    private bool isButtonSort = false;

    // Bucket Drop Fields
    private List<FallingItemData> activeItems = new List<FallingItemData>();

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        
        container = root.Q<VisualElement>("FallingSortContainer");
        fallZone = container.Q<VisualElement>("FallZone");
        binsRow = container.Q<VisualElement>("BinsRow");
        sortScoreText = container.Q<Label>("SortScoreText");
        sortTimerText = container.Q<Label>("SortTimerText");
    }

    public void StartGame()
    {
        isPlaying = true;
        sortScore = 0;
        timer = config.sortTimeLimit > 0 ? config.sortTimeLimit : 30f;
        isButtonSort = config.instructions.ToLower().Contains("condiment");

        if (sortScoreText != null) sortScoreText.text = $"Sorted: 0 / {config.sortTarget}";

        container.style.minWidth = 600; // Force popup to expand for comfortable sorting!
        container.style.minHeight = 560;

        if (isButtonSort)
        {
            SetupButtonSort();
        }
        else
        {
            SetupBucketDrop();
        }

        container.style.display = DisplayStyle.Flex;
    }

    public void AbortGame()
    {
        isPlaying = false;
        container.style.display = DisplayStyle.None;
    }

    // ==========================================
    // BUCKET DROP LOGIC (Industrial Overhaul)
    // ==========================================
    private void SetupBucketDrop()
    {
        binsRow.Clear();
        fallZone.Clear();
        activeItems.Clear();

        // Industrial Theme
        container.style.backgroundColor = new StyleColor(new Color(0.12f, 0.12f, 0.14f));
        fallZone.style.alignItems = Align.Auto;
        fallZone.style.justifyContent = Justify.FlexStart;

        fallZone.generateVisualContent -= OnDrawIndustrialBucketBg;
        fallZone.generateVisualContent += OnDrawIndustrialBucketBg;

        // Draw Bins
        Color[] binColors = new Color[] { 
            new Color(0.9f, 0.7f, 0.2f), // Yellow
            new Color(0.8f, 0.3f, 0.2f), // Red
            new Color(0.3f, 0.7f, 0.8f), // Cyan
            new Color(0.4f, 0.8f, 0.3f)  // Green
        };

        for (int i = 0; i < config.binLabels.Length; i++)
        {
            var bin = new VisualElement();
            bin.style.flexGrow = 1;
            bin.style.flexBasis = new StyleLength(new Length(0, LengthUnit.Pixel));
            bin.style.height = 120;
            bin.style.marginRight = 10;
            bin.style.marginLeft = 10;
            
            Color c = binColors[i % binColors.Length];
            string lblLower = config.binLabels[i].ToLower();
            if (lblLower.Contains("iron")) c = new Color(0.65f, 0.75f, 0.85f); // Steel/Iron Cyan-Gray
            else if (lblLower.Contains("copper")) c = new Color(0.92f, 0.48f, 0.22f); // Copper Bronze/Orange

            bin.generateVisualContent += (ctx) => {
                var p = ctx.painter2D;
                float w = ctx.visualElement.layout.width;
                float h = ctx.visualElement.layout.height;

                if (float.IsNaN(w) || float.IsNaN(h) || w <= 0 || h <= 0) return;

                // Back rim
                p.fillColor = new Color(c.r*0.4f, c.g*0.4f, c.b*0.4f);
                p.BeginPath(); p.MoveTo(new Vector2(10, 20)); p.LineTo(new Vector2(w-10, 20)); p.LineTo(new Vector2(w-20, h)); p.LineTo(new Vector2(20, h)); p.Fill();
                
                // Dark interior
                p.fillColor = new Color(0.05f, 0.05f, 0.05f);
                p.BeginPath(); p.MoveTo(new Vector2(10, 20)); p.LineTo(new Vector2(w-10, 20)); p.LineTo(new Vector2(w-30, 60)); p.LineTo(new Vector2(30, 60)); p.Fill();

                // Front Rim Glowing
                p.strokeColor = c;
                p.lineWidth = 10f;
                p.BeginPath(); p.MoveTo(new Vector2(0, 0)); p.LineTo(new Vector2(w, 0)); p.LineTo(new Vector2(w-20, h)); p.LineTo(new Vector2(20, h)); p.ClosePath(); p.Stroke();

                // Rivets
                p.fillColor = new Color(0.2f, 0.2f, 0.2f);
                p.BeginPath(); p.Arc(new Vector2(20, h-20), 4, 0, 360); p.Fill();
                p.BeginPath(); p.Arc(new Vector2(w-20, h-20), 4, 0, 360); p.Fill();
            };

            var label = new Label(config.binLabels[i]);
            label.pickingMode = PickingMode.Ignore;
            label.style.unityTextAlign = TextAnchor.MiddleCenter;
            label.style.fontSize = 26;
            label.style.unityFontStyleAndWeight = FontStyle.Bold;
            label.style.color = Color.white;
            label.style.marginTop = 40;
            bin.Add(label);

            binsRow.Add(bin);
        }

        manager.StartCoroutine(BucketDropRoutine());
        manager.StartCoroutine(TimerRoutine());
    }

    private void OnDrawIndustrialBucketBg(MeshGenerationContext ctx)
    {
        var p = ctx.painter2D;
        float w = ctx.visualElement.layout.width;
        float h = ctx.visualElement.layout.height;

        if (float.IsNaN(w) || float.IsNaN(h) || w <= 0 || h <= 0) return;

        p.strokeColor = new Color(0.15f, 0.15f, 0.15f);
        p.lineWidth = 3f;
        for (float x = 0; x < w; x += 40) { p.BeginPath(); p.MoveTo(new Vector2(x, 0)); p.LineTo(new Vector2(x, h)); p.Stroke(); }
        for (float y = 0; y < h; y += 40) { p.BeginPath(); p.MoveTo(new Vector2(0, y)); p.LineTo(new Vector2(w, y)); p.Stroke(); }

        p.fillColor = new Color(0.9f, 0.7f, 0.1f, 0.2f);
        for(float y = -100; y < h; y += 80) {
            p.BeginPath(); p.MoveTo(new Vector2(0, y)); p.LineTo(new Vector2(20, y)); p.LineTo(new Vector2(20, y+40)); p.LineTo(new Vector2(0, y+40)); p.Fill();
            p.BeginPath(); p.MoveTo(new Vector2(w-20, y)); p.LineTo(new Vector2(w, y)); p.LineTo(new Vector2(w, y+40)); p.LineTo(new Vector2(w-20, y+40)); p.Fill();
        }
    }

    private IEnumerator BucketDropRoutine()
    {
        float spawnTimer = 0f;
        int itemsSpawned = 0;
        float gravity = 1200f; 

        while (isPlaying && manager.isPlaying)
        {
            if (sortScore >= config.sortTarget)
            {
                isPlaying = false;
                AudioManager.Instance?.PlayVictoryFanfare();
                manager.MarkWinReady();
                yield break;
            }

            if (itemsSpawned < config.sortTarget + 10 && spawnTimer <= 0f) 
            {
                int typeIdx = Random.Range(0, config.fallingItems.Length);
                string text = config.fallingItems[typeIdx];
                string emojiText = text.Contains(" ") ? text.Split(' ')[0] : text;
                
                var crate = new VisualElement();
                crate.pickingMode = PickingMode.Position;
                crate.style.position = Position.Absolute;
                crate.style.width = 100; crate.style.height = 100;
                crate.style.backgroundColor = new StyleColor(new Color(0.15f, 0.15f, 0.18f)); 
                crate.style.borderTopWidth = 6; crate.style.borderBottomWidth = 6; crate.style.borderLeftWidth = 6; crate.style.borderRightWidth = 6;
                crate.style.borderTopColor = new StyleColor(new Color(0.4f, 0.4f, 0.4f));
                crate.style.borderBottomColor = new StyleColor(new Color(0.05f, 0.05f, 0.05f));
                crate.style.borderLeftColor = new StyleColor(new Color(0.2f, 0.2f, 0.2f));
                crate.style.borderRightColor = new StyleColor(new Color(0.2f, 0.2f, 0.2f));
                crate.style.justifyContent = Justify.Center;
                crate.style.alignItems = Align.Center;

                var label = new Label(emojiText);
                label.pickingMode = PickingMode.Ignore;
                label.style.fontSize = 65;
                crate.Add(label);
                
                float zoneW = (fallZone != null && !float.IsNaN(fallZone.layout.width) && fallZone.layout.width > 120f) 
                    ? fallZone.layout.width : 500f;
                float startX = Random.Range(50f, Mathf.Max(60f, zoneW - 120f));
                
                FallingItemData itemData = new FallingItemData {
                    element = crate,
                    position = new Vector2(startX, -100),
                    velocity = new Vector2(Random.Range(-150f, 150f), Random.Range(-50f, 50f)),
                    typeId = typeIdx,
                    rotation = Random.Range(-30f, 30f)
                };
                
                crate.RegisterCallback<PointerDownEvent>(ev => {
                    if (!isPlaying) return;
                    itemData.isDragging = true;
                    itemData.velocity = Vector2.zero;
                    crate.CapturePointer(ev.pointerId);
                    crate.style.scale = new StyleScale(new Vector2(1.15f, 1.15f));
                    ev.StopPropagation();
                });
                
                crate.RegisterCallback<PointerMoveEvent>(ev => {
                    if (itemData.isDragging && crate.HasPointerCapture(ev.pointerId)) {
                        float dt = Mathf.Max(Time.deltaTime, 0.001f);
                        itemData.position += (Vector2)ev.deltaPosition;
                        itemData.velocity = (Vector2)ev.deltaPosition / dt;
                    }
                });
                
                crate.RegisterCallback<PointerUpEvent>(ev => {
                    if (itemData.isDragging) {
                        itemData.isDragging = false;
                        crate.ReleasePointer(ev.pointerId);
                        crate.style.scale = new StyleScale(new Vector2(1f, 1f));
                    }
                });

                crate.RegisterCallback<PointerCaptureOutEvent>(ev => {
                    if (itemData.isDragging) {
                        itemData.isDragging = false;
                        crate.style.scale = new StyleScale(new Vector2(1f, 1f));
                    }
                });

                if (fallZone != null) fallZone.Add(crate);
                activeItems.Add(itemData);
                itemsSpawned++;
                spawnTimer = Mathf.Max(0.8f, 2.0f - (itemsSpawned * 0.1f)); 
            }
            if (spawnTimer > 0f) spawnTimer -= Time.deltaTime;

            float w = (fallZone != null && !float.IsNaN(fallZone.layout.width) && fallZone.layout.width > 100f) 
                ? fallZone.layout.width : 500f;
            float h = (fallZone != null && !float.IsNaN(fallZone.layout.height) && fallZone.layout.height > 100f) 
                ? fallZone.layout.height : 400f;

            for (int i = activeItems.Count - 1; i >= 0; i--)
            {
                var item = activeItems[i];
                if (!item.isDragging)
                {
                    item.velocity.y += gravity * Time.deltaTime;
                    item.position += item.velocity * Time.deltaTime;
                    
                    if (item.position.x < 0) { item.position.x = 0; item.velocity.x = 0f; }
                    if (item.position.x > w - 100f) { item.position.x = w - 100f; item.velocity.x = 0f; }
                    
                    item.rotation += item.velocity.x * 0.3f * Time.deltaTime;
                }
                else
                {
                    item.rotation = Mathf.Lerp(item.rotation, -item.velocity.x * 0.05f, Time.deltaTime * 15f);
                }
                
                if (float.IsNaN(item.position.x) || float.IsInfinity(item.position.x)) item.position.x = w * 0.5f;
                if (float.IsNaN(item.position.y) || float.IsInfinity(item.position.y)) item.position.y = 0f;

                item.element.style.left = item.position.x;
                item.element.style.top = item.position.y;
                item.element.style.rotate = new StyleRotate(new Rotate(Angle.Degrees(item.rotation)));
                
                if (item.position.y >= h - 50f && !item.isDragging) 
                {
                    float binWidth = w / Mathf.Max(1, config.binLabels.Length);
                    int binIdx = Mathf.FloorToInt((item.position.x + 50f) / binWidth);
                    binIdx = Mathf.Clamp(binIdx, 0, config.binLabels.Length - 1);
                    
                    bool isCorrect = (binIdx == item.typeId);
                    if (!isCorrect && binIdx >= 0 && binIdx < config.binLabels.Length && item.typeId >= 0 && item.typeId < config.fallingItems.Length)
                    {
                        string binClean = config.binLabels[binIdx].ToLower().Replace("bin", "").Trim();
                        string itemClean = config.fallingItems[item.typeId].ToLower();
                        if (itemClean.Contains(binClean) || binClean.Contains(itemClean))
                        {
                            isCorrect = true;
                        }
                    }

                    if (isCorrect)
                    {
                        sortScore++;
                        if (sortScoreText != null) sortScoreText.text = $"Sorted: {sortScore} / {config.sortTarget}";
                        manager.StartCoroutine(BinFlash(binIdx, true));
                    }
                    else
                    {
                        if (manager.currentGame != null && manager.currentGame.canFail) sortScore = Mathf.Max(0, sortScore - 1);
                        if (sortScoreText != null) sortScoreText.text = $"Sorted: {sortScore} / {config.sortTarget}";
                        manager.StartCoroutine(BinFlash(binIdx, false));
                    }
                    
                    if (fallZone != null) fallZone.Remove(item.element);
                    activeItems.RemoveAt(i);
                }
            }
            yield return null;
        }
    }

    private IEnumerator BinFlash(int index, bool correct)
    {
        if (index < 0 || index >= binsRow.childCount) yield break;
        var bin = binsRow.ElementAt(index);
        
        if (correct)
        {
            AudioManager.Instance?.PlayScorePop();
            HapticManager.LightImpact();
        }
        else
        {
            AudioManager.Instance?.PlayBuzzer();
            HapticManager.MediumImpact();
        }

        float elapsed = 0f;
        while(elapsed < 0.2f) {
            elapsed += Time.deltaTime;
            float p = elapsed / 0.2f;
            float s = 1f + Mathf.Sin(p * Mathf.PI) * 0.15f;
            bin.style.scale = new StyleScale(new Vector2(s, s));
            
            if (p < 0.5f) bin.style.translate = new StyleTranslate(new Translate(0, 10, 0)); // Crunch down
            else bin.style.translate = new StyleTranslate(new Translate(0, 0, 0));
            
            yield return null;
        }
        
        bin.style.scale = new StyleScale(new Vector2(1f, 1f));
        bin.style.translate = new StyleTranslate(new Translate(0, 0, 0));
    }


    // ==========================================
    // BUTTON SORT LOGIC (Condiments Tinder Swipe)
    // ==========================================
    private void SetupButtonSort()
    {
        container.style.backgroundColor = new StyleColor(new Color(0.08f, 0.1f, 0.14f));
        fallZone.generateVisualContent -= OnDrawIndustrialBucketBg;

        Color[] binColors = new Color[] { new Color(0.9f, 0.7f, 0.2f), new Color(0.8f, 0.3f, 0.2f), new Color(0.2f, 0.7f, 0.4f), new Color(0.2f, 0.5f, 0.8f) };
        
        binsRow.Clear();
        for (int i = 0; i < config.binLabels.Length; i++)
        {
            Button btn = new Button();
            btn.text = config.binLabels[i];
            btn.style.flexGrow = 1;
            btn.style.flexBasis = new StyleLength(new Length(0, LengthUnit.Pixel));
            btn.style.marginRight = 10;
            btn.style.marginLeft = 10;
            btn.style.fontSize = 24;
            btn.style.unityFontStyleAndWeight = FontStyle.Bold;
            btn.style.color = Color.white;
            btn.style.backgroundColor = new StyleColor(binColors[i % binColors.Length]);
            btn.style.borderTopWidth = 2; btn.style.borderLeftWidth = 2; btn.style.borderRightWidth = 2; btn.style.borderBottomWidth = 2;
            btn.style.borderTopColor = Color.white; btn.style.borderLeftColor = Color.white; btn.style.borderRightColor = Color.white; btn.style.borderBottomColor = Color.white;
            btn.style.borderTopLeftRadius = 10; btn.style.borderTopRightRadius = 10; btn.style.borderBottomLeftRadius = 10; btn.style.borderBottomRightRadius = 10;
            
            int binIndex = i;
            btn.clicked += () => OnBinClickedButtonSort(binIndex, btn);
            binsRow.Add(btn);
        }
        
        fallZone.Clear();
        fallZone.style.alignItems = Align.Center;
        fallZone.style.justifyContent = Justify.Center;

        manager.StartCoroutine(TimerRoutine());
        manager.StartCoroutine(SpawnNextItemButtonSort());
    }

    private void OnBinClickedButtonSort(int binIndex, Button btn)
    {
        if (!isPlaying || isAnimating || currentItemLabel == null) return;
        
        btn.style.scale = new StyleScale(new Vector2(0.9f, 0.9f));
        btn.schedule.Execute(() => { if (btn != null) btn.style.scale = new StyleScale(new Vector2(1f, 1f)); }).StartingIn(100);

        bool isCorrect = (binIndex == currentItemType);
        if (!isCorrect && binIndex >= 0 && binIndex < config.binLabels.Length && currentItemType >= 0 && currentItemType < config.fallingItems.Length)
        {
            string binClean = config.binLabels[binIndex].ToLower().Replace("bin", "").Trim();
            string itemClean = config.fallingItems[currentItemType].ToLower();
            if (itemClean.Contains(binClean) || binClean.Contains(itemClean))
            {
                isCorrect = true;
            }
        }

        if (isCorrect)
        {
            sortScore++;
            if (sortScoreText != null) sortScoreText.text = $"Sorted: {sortScore} / {config.sortTarget}";
            manager.StartCoroutine(FlingItemRoutineButtonSort(btn));
        }
        else
        {
            if (manager.currentGame != null && manager.currentGame.canFail) {
                sortScore = Mathf.Max(0, sortScore - 1);
                if (sortScoreText != null) sortScoreText.text = $"Sorted: {sortScore} / {config.sortTarget}";
            }
            manager.StartCoroutine(ErrorShakeRoutineButtonSort(btn));
        }
    }

    private IEnumerator SpawnNextItemButtonSort()
    {
        isAnimating = true;
        if (sortScore >= config.sortTarget) { 
            isPlaying = false; 
            AudioManager.Instance?.PlayVictoryFanfare();
            manager.MarkWinReady(); 
            yield break; 
        }
        
        currentItemType = Random.Range(0, config.fallingItems.Length);
        string text = config.fallingItems[currentItemType];
        string emojiText = text.Contains(" ") ? text.Split(' ')[0] : text;
        
        currentItemLabel = new Label(emojiText);
        currentItemLabel.style.fontSize = 120;
        currentItemLabel.style.position = Position.Absolute;
        currentItemLabel.style.translate = new Translate(0, -300, 0);
        currentItemLabel.style.scale = new StyleScale(new Vector2(0f, 0f));
        
        if (fallZone != null) fallZone.Add(currentItemLabel);
        
        float duration = 0.25f;
        float elapsed = 0f;
        while(elapsed < duration && isPlaying)
        {
            elapsed += Time.deltaTime;
            float p = elapsed / duration;
            float s = 1f + Mathf.Sin(p * Mathf.PI) * 0.3f;
            currentItemLabel.style.scale = new StyleScale(new Vector2(s, s));
            float y = Mathf.Lerp(-300f, 0f, p);
            currentItemLabel.style.translate = new Translate(0, y, 0);
            yield return null;
        }
        
        if (currentItemLabel != null) { currentItemLabel.style.scale = new StyleScale(new Vector2(1f, 1f)); currentItemLabel.style.translate = new Translate(0, 0, 0); }
        isAnimating = false;
    }
    
    private IEnumerator FlingItemRoutineButtonSort(Button targetBtn)
    {
        isAnimating = true;
        AudioManager.Instance?.PlayScorePop();
        HapticManager.LightImpact();
        Label movingItem = currentItemLabel;
        currentItemLabel = null;
        
        StyleColor originalColor = targetBtn.style.backgroundColor;
        targetBtn.style.backgroundColor = new StyleColor(Color.white);
        targetBtn.schedule.Execute(() => { if (targetBtn != null) targetBtn.style.backgroundColor = originalColor; }).StartingIn(150);
        
        float targetX = 0f;
        if (targetBtn.worldBound.center.x < container.worldBound.center.x - 50) targetX = -300f;
        else if (targetBtn.worldBound.center.x > container.worldBound.center.x + 50) targetX = 300f;
        
        float duration = 0.2f;
        float elapsed = 0f;
        while(elapsed < duration && isPlaying)
        {
            elapsed += Time.deltaTime;
            float p = elapsed / duration;
            float x = Mathf.Lerp(0f, targetX, p);
            float y = Mathf.Lerp(0f, 300f, p);
            movingItem.style.translate = new Translate(x, y, 0);
            movingItem.style.rotate = new Rotate(new Angle(Mathf.Lerp(0f, 360f, p), AngleUnit.Degree));
            movingItem.style.scale = new StyleScale(new Vector2(1f - p, 1f - p));
            yield return null;
        }
        
        if (fallZone != null && fallZone.Contains(movingItem)) fallZone.Remove(movingItem);
        if (isPlaying) manager.StartCoroutine(SpawnNextItemButtonSort());
    }
    
    private IEnumerator ErrorShakeRoutineButtonSort(Button targetBtn)
    {
        isAnimating = true;
        AudioManager.Instance?.PlayBuzzer();
        HapticManager.MediumImpact();
        StyleColor originalColor = targetBtn.style.backgroundColor;
        targetBtn.style.backgroundColor = new StyleColor(new Color(0.9f, 0.2f, 0.2f));
        
        float duration = 0.4f;
        float elapsed = 0f;
        while(elapsed < duration && isPlaying)
        {
            elapsed += Time.deltaTime;
            if (currentItemLabel != null) currentItemLabel.style.translate = new Translate(Mathf.Sin(elapsed * 40f) * 10f, 0, 0);
            yield return null;
        }
        
        if (currentItemLabel != null) currentItemLabel.style.translate = new Translate(0, 0, 0);
        if (targetBtn != null) targetBtn.style.backgroundColor = originalColor;
        isAnimating = false;
    }

    // ==========================================
    // SHARED
    // ==========================================
    private IEnumerator TimerRoutine()
    {
        while (isPlaying && manager.isPlaying)
        {
            if (sortTimerText != null) sortTimerText.text = $"Time: {Mathf.Ceil(timer)}s";
            timer -= Time.deltaTime;
            
            if (timer <= 0)
            {
                if (manager.currentGame != null && manager.currentGame.canFail)
                {
                    isPlaying = false;
                    AudioManager.Instance?.PlayMinigameFail();
                    manager.AbortGame();
                    yield break;
                }
                else if (sortScore < config.sortTarget)
                {
                    timer = 20f; 
                }
            }
            yield return null;
        }
    }
}

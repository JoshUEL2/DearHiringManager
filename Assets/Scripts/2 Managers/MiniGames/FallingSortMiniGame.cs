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
    private float timer = 30f;
    private bool isPlaying = false;
    
    private Label currentItemLabel;
    private int currentItemType = -1;
    private bool isAnimating = false;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        
        container = root.Q<VisualElement>("FallingSortContainer");
        fallZone = container.Q<VisualElement>("FallZone");
        
        // Correctly target BinsRow
        binsRow = container.Q<VisualElement>("BinsRow");
        
        sortScoreText = container.Q<Label>("SortScoreText");
        sortTimerText = container.Q<Label>("SortTimerText");
    }

    public void StartGame()
    {
        isPlaying = true;
        sortScore = 0;
        timer = config.sortTimeLimit > 0 ? config.sortTimeLimit : 30f;
        isAnimating = false;
        
        if (sortScoreText != null) sortScoreText.text = $"Sorted: 0 / {config.sortTarget}";
        
        if (binsRow != null && config.binLabels != null)
        {
            binsRow.Clear();
            Color[] binColors = new Color[] {
                new Color(0.94f, 0.76f, 0.05f), // Mustard Yellow
                new Color(0.9f, 0.2f, 0.2f),    // Ketchup Red
                new Color(0.18f, 0.8f, 0.44f),  // Relish Green
                new Color(0.6f, 0.2f, 0.8f)     // Fallback
            };

            for (int i = 0; i < config.binLabels.Length; i++)
            {
                Button btn = new Button();
                btn.text = config.binLabels[i];
                btn.style.flexGrow = 1;
                btn.style.marginRight = 10;
                btn.style.marginLeft = 10;
                btn.style.fontSize = 24;
                btn.style.unityFontStyleAndWeight = FontStyle.Bold;
                btn.style.color = Color.white;
                btn.style.backgroundColor = new StyleColor(binColors[i % binColors.Length]);
                btn.style.borderTopWidth = 2; btn.style.borderLeftWidth = 2; btn.style.borderRightWidth = 2; btn.style.borderBottomWidth = 2;
                btn.style.borderTopColor = Color.white; btn.style.borderLeftColor = Color.white; btn.style.borderRightColor = Color.white; btn.style.borderBottomColor = Color.white;
                btn.style.borderTopLeftRadius = 10; btn.style.borderTopRightRadius = 10; btn.style.borderBottomLeftRadius = 10; btn.style.borderBottomRightRadius = 10;
                
                int binIndex = i; // capture for closure
                btn.clicked += () => OnBinClicked(binIndex, btn);
                
                binsRow.Add(btn);
            }
        }
        
        if (fallZone != null) 
        {
            fallZone.Clear();
            // Center the items in the fall zone
            fallZone.style.alignItems = Align.Center;
            fallZone.style.justifyContent = Justify.Center;
        }

        container.style.display = DisplayStyle.Flex;
        manager.StartCoroutine(TimerRoutine());
        manager.StartCoroutine(SpawnNextItem());
    }

    public void AbortGame()
    {
        isPlaying = false;
        container.style.display = DisplayStyle.None;
    }
    
    private void OnBinClicked(int binIndex, Button btn)
    {
        if (!isPlaying || isAnimating || currentItemLabel == null) return;
        
        // Physical button press effect
        btn.style.scale = new StyleScale(new Vector2(0.9f, 0.9f));
        btn.schedule.Execute(() => {
            if (btn != null) btn.style.scale = new StyleScale(new Vector2(1f, 1f));
        }).StartingIn(100);

        if (binIndex == currentItemType)
        {
            sortScore++;
            if (sortScoreText != null) sortScoreText.text = $"Sorted: {sortScore} / {config.sortTarget}";
            manager.StartCoroutine(FlingItemRoutine(btn));
        }
        else
        {
            if (manager.currentGame != null && manager.currentGame.canFail)
            {
                sortScore = Mathf.Max(0, sortScore - 1);
                if (sortScoreText != null) sortScoreText.text = $"Sorted: {sortScore} / {config.sortTarget}";
            }
            manager.StartCoroutine(ErrorShakeRoutine(btn));
        }
    }
    
    private IEnumerator SpawnNextItem()
    {
        isAnimating = true;
        
        if (sortScore >= config.sortTarget)
        {
            isPlaying = false;
            manager.MarkWinReady();
            yield break;
        }
        
        currentItemType = Random.Range(0, config.fallingItems.Length);
        string text = config.fallingItems[currentItemType];
        string emojiText = text.Contains(" ") ? text.Split(' ')[0] : text; // Just grab the emoji!
        
        currentItemLabel = new Label(emojiText);
        currentItemLabel.style.fontSize = 120;
        currentItemLabel.style.position = Position.Absolute;
        
        // Start high up, scale 0
        currentItemLabel.style.translate = new Translate(0, -200, 0);
        currentItemLabel.style.scale = new StyleScale(new Vector2(0f, 0f));
        
        if (fallZone != null) fallZone.Add(currentItemLabel);
        
        // Slam down animation
        float duration = 0.25f;
        float elapsed = 0f;
        while(elapsed < duration && isPlaying)
        {
            elapsed += Time.deltaTime;
            float p = elapsed / duration;
            
            // Ease out back
            float s = 1f + Mathf.Sin(p * Mathf.PI) * 0.3f;
            currentItemLabel.style.scale = new StyleScale(new Vector2(s, s));
            
            float y = Mathf.Lerp(-200f, 0f, p);
            currentItemLabel.style.translate = new Translate(0, y, 0);
            
            yield return null;
        }
        
        if (currentItemLabel != null)
        {
            currentItemLabel.style.scale = new StyleScale(new Vector2(1f, 1f));
            currentItemLabel.style.translate = new Translate(0, 0, 0);
        }
        
        isAnimating = false;
    }
    
    private IEnumerator FlingItemRoutine(Button targetBtn)
    {
        isAnimating = true;
        Label movingItem = currentItemLabel;
        currentItemLabel = null; // Clear it instantly so we can't click again
        
        // Flash button white
        StyleColor originalColor = targetBtn.style.backgroundColor;
        targetBtn.style.backgroundColor = new StyleColor(Color.white);
        targetBtn.schedule.Execute(() => {
            if (targetBtn != null) targetBtn.style.backgroundColor = originalColor;
        }).StartingIn(150);
        
        // Figure out horizontal direction based on bin index
        float targetX = 0f;
        if (targetBtn.worldBound.center.x < container.worldBound.center.x - 50) targetX = -300f; // Left bin
        else if (targetBtn.worldBound.center.x > container.worldBound.center.x + 50) targetX = 300f; // Right bin
        
        float duration = 0.2f;
        float elapsed = 0f;
        while(elapsed < duration && isPlaying)
        {
            elapsed += Time.deltaTime;
            float p = elapsed / duration;
            
            // Fling down and towards the bin, spinning wildly
            float x = Mathf.Lerp(0f, targetX, p);
            float y = Mathf.Lerp(0f, 300f, p);
            movingItem.style.translate = new Translate(x, y, 0);
            
            // Rotate string hack
            float rot = Mathf.Lerp(0f, 360f, p);
            movingItem.style.rotate = new Rotate(new Angle(rot, AngleUnit.Degree));
            
            movingItem.style.scale = new StyleScale(new Vector2(1f - p, 1f - p)); // Shrink as it falls in
            
            yield return null;
        }
        
        if (fallZone != null && fallZone.Contains(movingItem)) fallZone.Remove(movingItem);
        
        if (isPlaying) manager.StartCoroutine(SpawnNextItem());
    }
    
    private IEnumerator ErrorShakeRoutine(Button targetBtn)
    {
        isAnimating = true; // Lock input
        
        // Flash button red
        StyleColor originalColor = targetBtn.style.backgroundColor;
        targetBtn.style.backgroundColor = new StyleColor(new Color(0.9f, 0.2f, 0.2f));
        
        // Screen shake
        float duration = 0.4f;
        float elapsed = 0f;
        while(elapsed < duration && isPlaying)
        {
            elapsed += Time.deltaTime;
            float xOffset = Mathf.Sin(elapsed * 40f) * 10f; // Violent shake
            
            if (currentItemLabel != null)
                currentItemLabel.style.translate = new Translate(xOffset, 0, 0);
                
            yield return null;
        }
        
        if (currentItemLabel != null)
            currentItemLabel.style.translate = new Translate(0, 0, 0);
            
        if (targetBtn != null) 
            targetBtn.style.backgroundColor = originalColor;
            
        isAnimating = false; // Unlock input to let them try again!
    }

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

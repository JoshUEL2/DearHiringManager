using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class OrderQueueMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement container;

    private VisualElement orderItemGrid;
    private Label orderStatusText;
    private Label orderTimerText;

    private List<string> orderCorrectSequence = new List<string>();
    private int orderInputIndex = 0;
    
    // Dance minigame visuals
    private VisualElement danceStage;
    private Label dancerAvatar;
    private Coroutine discoRoutine;
    private bool orderIsPreview = false;
    private bool isPlaying = false;
    private float timer = 0f;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        
        container = root.Q<VisualElement>("OrderQueueContainer");
        orderItemGrid = container.Q<VisualElement>("OrderItemGrid");
        
        // Setup Dance Stage
        if (manager.currentGame != null && manager.currentGame.name == "Dance with sign for tourists")
        {
            danceStage = new VisualElement();
            danceStage.style.height = 180;
            danceStage.style.width = Length.Percent(100);
            danceStage.style.alignItems = Align.Center;
            danceStage.style.justifyContent = Justify.Center;
            danceStage.style.marginBottom = 20;
            danceStage.style.backgroundColor = new StyleColor(new Color(0.1f, 0.05f, 0.2f));
            danceStage.style.borderTopLeftRadius = 20; danceStage.style.borderTopRightRadius = 20;
            danceStage.style.borderBottomLeftRadius = 20; danceStage.style.borderBottomRightRadius = 20;
            danceStage.style.overflow = Overflow.Hidden;
            
            VisualElement discoBall = new VisualElement();
            discoBall.style.position = Position.Absolute;
            discoBall.style.top = 10;
            discoBall.style.width = 44;
            discoBall.style.height = 44;
            discoBall.style.backgroundColor = new StyleColor(new Color(0.9f, 0.95f, 1f));
            discoBall.style.borderTopLeftRadius = 22; discoBall.style.borderTopRightRadius = 22;
            discoBall.style.borderBottomLeftRadius = 22; discoBall.style.borderBottomRightRadius = 22;
            danceStage.Add(discoBall);
            
            dancerAvatar = new Label();
            dancerAvatar.AddToClassList("mg-icon");
            dancerAvatar.AddToClassList("mg-icon-person");
            dancerAvatar.style.width = 72;
            dancerAvatar.style.height = 72;
            dancerAvatar.style.position = Position.Absolute;
            dancerAvatar.style.top = 70;
            danceStage.Add(dancerAvatar);
            
            // Insert dance stage before the grid
            int gridIndex = container.IndexOf(orderItemGrid);
            if (gridIndex >= 0) container.Insert(gridIndex, danceStage);
            else container.Insert(0, danceStage);
        }
        orderStatusText = container.Q<Label>("OrderStatusText");
        orderTimerText = container.Q<Label>("OrderTimerText");
    }

    public void StartGame()
    {
        isPlaying = true;
        timer = config.timeLimit > 0 ? config.timeLimit : 30f;
        container.style.display = DisplayStyle.Flex;
        
        if (manager.currentGame != null)
        {
            if (manager.currentGame.name == "Train new hire on POS system")
            {
                container.style.backgroundColor = new StyleColor(new Color(0.05f, 0.12f, 0.05f));
                container.style.borderTopWidth = 15; container.style.borderBottomWidth = 15; container.style.borderLeftWidth = 15; container.style.borderRightWidth = 15;
                container.style.borderTopColor = new StyleColor(new Color(0.85f, 0.85f, 0.8f));
                container.style.borderBottomColor = new StyleColor(new Color(0.85f, 0.85f, 0.8f));
                container.style.borderLeftColor = new StyleColor(new Color(0.85f, 0.85f, 0.8f));
                container.style.borderRightColor = new StyleColor(new Color(0.85f, 0.85f, 0.8f));
                container.style.borderTopLeftRadius = 20; container.style.borderTopRightRadius = 20; container.style.borderBottomLeftRadius = 20; container.style.borderBottomRightRadius = 20;
                container.style.paddingTop = 20; container.style.paddingBottom = 20;
            }
            else if (manager.currentGame.name == "Dance with sign for tourists")
            {
                container.style.backgroundColor = new StyleColor(new Color(0.15f, 0.05f, 0.25f)); // Deep purple
                container.style.borderTopWidth = 6; container.style.borderBottomWidth = 6; container.style.borderLeftWidth = 6; container.style.borderRightWidth = 6;
                container.style.borderTopColor = new StyleColor(new Color(1f, 0f, 0.8f)); // Neon pink
                container.style.borderBottomColor = new StyleColor(new Color(0f, 1f, 0.8f)); // Neon cyan
                container.style.borderLeftColor = new StyleColor(new Color(1f, 0f, 0.8f));
                container.style.borderRightColor = new StyleColor(new Color(0f, 1f, 0.8f));
                container.style.borderTopLeftRadius = 30; container.style.borderTopRightRadius = 30; container.style.borderBottomLeftRadius = 30; container.style.borderBottomRightRadius = 30;
                
                if (discoRoutine != null) manager.StopCoroutine(discoRoutine);
                discoRoutine = manager.StartCoroutine(DiscoRoutine());
            }
            else if (manager.currentGame.name == "Verify hotfix deployment")
            {
                Color ghBg = ColorUtility.TryParseHtmlString("#0d1117", out Color c1) ? c1 : new Color(0.05f, 0.07f, 0.09f);
                Color ghBorder = ColorUtility.TryParseHtmlString("#30363d", out Color c2) ? c2 : new Color(0.18f, 0.21f, 0.23f);
                container.style.backgroundColor = new StyleColor(ghBg);
                container.style.borderTopWidth = 2; container.style.borderBottomWidth = 2; container.style.borderLeftWidth = 2; container.style.borderRightWidth = 2;
                container.style.borderTopColor = new StyleColor(ghBorder); container.style.borderBottomColor = new StyleColor(ghBorder); container.style.borderLeftColor = new StyleColor(ghBorder); container.style.borderRightColor = new StyleColor(ghBorder);
                container.style.borderTopLeftRadius = 10; container.style.borderTopRightRadius = 10; container.style.borderBottomLeftRadius = 10; container.style.borderBottomRightRadius = 10;
                container.style.paddingTop = 20; container.style.paddingBottom = 20;
            }
            else
            {
                container.style.backgroundColor = new StyleColor(Color.clear);
            }
        }

        manager.StartCoroutine(OrderQueueRoutine());
        if (config.timeLimit > 0) manager.StartCoroutine(TimerRoutine());
    }

    public void AbortGame()
    {
        isPlaying = false;
        if (discoRoutine != null) { manager.StopCoroutine(discoRoutine); discoRoutine = null; }
        container.style.display = DisplayStyle.None;
    }

    private IEnumerator TimerRoutine()
    {
        while (isPlaying && manager.isPlaying)
        {
            if (!orderIsPreview)
            {
                timer -= Time.deltaTime;
                if (orderTimerText != null) orderTimerText.text = $"Time: {Mathf.Ceil(timer)}s";
                if (timer <= 0)
                {
                    isPlaying = false;
                    if (manager.currentGame != null && manager.currentGame.canFail)
                    {
                        AudioManager.Instance?.PlayMinigameFail();
                        manager.AbortGame();
                    }
                    else
                    {
                        AudioManager.Instance?.PlayVictoryFanfare();
                        manager.MarkWinReady();
                    }
                    yield break;
                }
            }
            yield return null;
        }
    }

    private IEnumerator OrderQueueRoutine()
    {
        if (config.orderItems == null) yield break;
        orderCorrectSequence = config.orderItems.ToList();
        orderInputIndex = 0;
        orderIsPreview = true;
        
        if (orderStatusText != null) orderStatusText.text = "Memorize the order!";
        if (orderItemGrid != null) orderItemGrid.Clear();
        
        foreach (var item in orderCorrectSequence)
        {
            var btn = CreateOrderButton(item, true);
            if (orderItemGrid != null) orderItemGrid.Add(btn);
        }
        
        if (manager.currentGame.name == "Dance with sign for tourists")
        {
            float timePerMove = config.orderPreviewTime / orderCorrectSequence.Count;
            foreach (var item in orderCorrectSequence)
            {
                if (!isPlaying) break;
                manager.StartCoroutine(DanceMoveAnimation(item));
                yield return new WaitForSeconds(timePerMove);
            }
        }
        else
        {
            yield return new WaitForSeconds(config.orderPreviewTime);
        }
        if (!isPlaying || !manager.isPlaying) yield break;
        
        orderIsPreview = false;
        if (orderStatusText != null) orderStatusText.text = "Select in order!";
        if (orderItemGrid != null) orderItemGrid.Clear();
        
        var scrambled = orderCorrectSequence.OrderBy(x => Random.value).ToList();
        foreach (var item in scrambled)
        {
            var btn = CreateOrderButton(item, false);
            string capturedItem = item;
            btn.clicked += () => OnOrderItemClicked(capturedItem, btn);
            if (orderItemGrid != null) orderItemGrid.Add(btn);
        }
    }

    private Button CreateOrderButton(string item, bool isPreview)
    {
        var btn = new Button { text = item };
        btn.AddToClassList("popup-button");
        btn.style.width = new StyleLength(StyleKeyword.Auto);
        btn.style.paddingLeft = 20; btn.style.paddingRight = 20; btn.style.marginRight = 10; btn.style.marginBottom = 10; btn.style.fontSize = 24;
        
        if (manager.currentGame != null && manager.currentGame.name == "Train new hire on POS system")
        {
            btn.style.backgroundColor = new StyleColor(isPreview ? new Color(0.05f, 0.25f, 0.05f) : new Color(0.02f, 0.15f, 0.02f));
            btn.style.color = new StyleColor(new Color(0.2f, 1f, 0.2f));
            btn.style.borderTopColor = new StyleColor(new Color(0.2f, 1f, 0.2f));
            btn.style.borderBottomColor = new StyleColor(new Color(0.2f, 1f, 0.2f));
            btn.style.borderLeftColor = new StyleColor(new Color(0.2f, 1f, 0.2f));
            btn.style.borderRightColor = new StyleColor(new Color(0.2f, 1f, 0.2f));
            btn.style.borderTopWidth = 2; btn.style.borderBottomWidth = 2; btn.style.borderLeftWidth = 2; btn.style.borderRightWidth = 2;
        }
        else if (manager.currentGame != null && manager.currentGame.name == "Dance with sign for tourists")
        {
            btn.style.backgroundColor = new StyleColor(isPreview ? new Color(0.2f, 0.05f, 0.3f) : new Color(0.1f, 0.05f, 0.2f));
            btn.style.color = new StyleColor(Color.white);
            btn.style.borderTopWidth = 3; btn.style.borderBottomWidth = 3; btn.style.borderLeftWidth = 3; btn.style.borderRightWidth = 3;
            btn.style.borderTopColor = new StyleColor(new Color(0f, 1f, 0.8f)); // Cyan
            btn.style.borderBottomColor = new StyleColor(new Color(1f, 0f, 0.8f)); // Pink
            btn.style.borderLeftColor = new StyleColor(new Color(0f, 1f, 0.8f));
            btn.style.borderRightColor = new StyleColor(new Color(1f, 0f, 0.8f));
            btn.style.borderTopLeftRadius = 10; btn.style.borderTopRightRadius = 10; btn.style.borderBottomLeftRadius = 10; btn.style.borderBottomRightRadius = 10;
        }
        else if (manager.currentGame != null && manager.currentGame.name == "Verify hotfix deployment")
        {
            Color ghBtnBg = ColorUtility.TryParseHtmlString("#21262d", out Color c1) ? c1 : new Color(0.13f, 0.15f, 0.17f);
            Color ghBtnText = ColorUtility.TryParseHtmlString("#c9d1d9", out Color c2) ? c2 : Color.white;
            Color ghBtnBorder = ColorUtility.TryParseHtmlString("#30363d", out Color c3) ? c3 : new Color(0.18f, 0.21f, 0.23f);

            btn.style.backgroundColor = new StyleColor(ghBtnBg);
            btn.style.color = new StyleColor(ghBtnText);
            btn.style.borderTopColor = new StyleColor(ghBtnBorder);
            btn.style.borderBottomColor = new StyleColor(ghBtnBorder);
            btn.style.borderLeftColor = new StyleColor(ghBtnBorder);
            btn.style.borderRightColor = new StyleColor(ghBtnBorder);
            btn.style.borderTopWidth = 1; btn.style.borderBottomWidth = 1; btn.style.borderLeftWidth = 1; btn.style.borderRightWidth = 1;
            btn.style.borderTopLeftRadius = 15; btn.style.borderTopRightRadius = 15; btn.style.borderBottomLeftRadius = 15; btn.style.borderBottomRightRadius = 15;
        }
        else
        {
            btn.style.backgroundColor = new StyleColor(isPreview ? new Color(0.22f, 0.74f, 0.97f) : new Color(0.15f, 0.15f, 0.15f)); 
            btn.style.color = new StyleColor(Color.white);
        }
        return btn;
    }

    private void OnOrderItemClicked(string item, Button btn)
    {
        if (!isPlaying || !manager.isPlaying || orderIsPreview) return;
        
        if (item == orderCorrectSequence[orderInputIndex])
        {
            MiniGameFeedback.PlaySuccess(btn, "STEP OK!", container);
            btn.style.backgroundColor = new StyleColor(Color.green);
            if (manager.currentGame.name == "Dance with sign for tourists")
            {
                manager.StartCoroutine(DanceMoveAnimation(item));
            }
            btn.SetEnabled(false);
            orderInputIndex++;
            if (orderInputIndex >= orderCorrectSequence.Count)
            {
                isPlaying = false;
                AudioManager.Instance?.PlayVictoryFanfare();
                manager.MarkWinReady();
            }
        }
        else
        {
            MiniGameFeedback.PlayError(btn, "WRONG STEP!", container);
            if (manager.currentGame != null && manager.currentGame.canFail)
            {
                manager.StartCoroutine(OrderQueueRoutine()); 
            }
            else
            {
                btn.style.backgroundColor = new StyleColor(Color.red);
            }
        }
    }

    private IEnumerator DiscoRoutine()
    {
        Color[] colors = new Color[] {
            new Color(0.2f, 0.05f, 0.3f), // Purple
            new Color(0.1f, 0.2f, 0.3f),  // Blue
            new Color(0.3f, 0.05f, 0.1f), // Red
            new Color(0.1f, 0.3f, 0.1f)   // Green
        };
        int idx = 0;
        while (isPlaying && manager.isPlaying)
        {
            if (danceStage != null)
            {
                danceStage.style.backgroundColor = new StyleColor(colors[idx]);
            }
            idx = (idx + 1) % colors.Length;
            yield return new WaitForSeconds(0.4f);
        }
    }

    private IEnumerator DanceMoveAnimation(string move)
    {
        if (dancerAvatar == null) yield break;
        
        float duration = 0.3f;
        float elapsed = 0f;
        
        Vector2 startPos = new Vector2(0, 70); // translate X, Y
        
        while (elapsed < duration && isPlaying)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            if (move == "Left")
            {
                dancerAvatar.style.translate = new Translate(new Length(Mathf.Lerp(0, -60, Mathf.Sin(t * Mathf.PI)), LengthUnit.Pixel), 0);
            }
            else if (move == "Right")
            {
                dancerAvatar.style.translate = new Translate(new Length(Mathf.Lerp(0, 60, Mathf.Sin(t * Mathf.PI)), LengthUnit.Pixel), 0);
            }
            else if (move == "Spin")
            {
                dancerAvatar.style.rotate = new StyleRotate(new Rotate(new Angle(Mathf.Lerp(0, 360, t), AngleUnit.Degree)));
            }
            else if (move == "Pose")
            {
                float scale = Mathf.Lerp(1f, 1.5f, Mathf.Sin(t * Mathf.PI));
                dancerAvatar.style.scale = new StyleScale(new Vector2(scale, scale));
            }
            else if (move == "Wave")
            {
                dancerAvatar.style.rotate = new StyleRotate(new Rotate(new Angle(Mathf.Lerp(0, 45, Mathf.Sin(t * Mathf.PI * 2)), AngleUnit.Degree)));
            }
            else if (move == "Dip")
            {
                dancerAvatar.style.translate = new Translate(0, new Length(Mathf.Lerp(0, 40, Mathf.Sin(t * Mathf.PI)), LengthUnit.Pixel));
            }
            
            yield return null;
        }
        
        dancerAvatar.style.translate = new Translate(0, 0);
        dancerAvatar.style.rotate = new StyleRotate(new Rotate(new Angle(0, AngleUnit.Degree)));
        dancerAvatar.style.scale = new StyleScale(new Vector2(1, 1));
    }
}
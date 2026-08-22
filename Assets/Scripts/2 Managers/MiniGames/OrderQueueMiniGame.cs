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
    private bool orderIsPreview = false;
    private bool isPlaying = false;
    private float timer = 0f;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        
        container = root.Q<VisualElement>("OrderQueueContainer");
        orderItemGrid = container.Q<VisualElement>("OrderItemGrid");
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
                    if (manager.currentGame != null && manager.currentGame.canFail) manager.AbortGame();
                    else manager.MarkWinReady();
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
        
        yield return new WaitForSeconds(config.orderPreviewTime);
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
            btn.style.backgroundColor = new StyleColor(Color.green);
            btn.SetEnabled(false);
            orderInputIndex++;
            if (orderInputIndex >= orderCorrectSequence.Count)
            {
                isPlaying = false;
                manager.MarkWinReady();
            }
        }
        else
        {
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
}

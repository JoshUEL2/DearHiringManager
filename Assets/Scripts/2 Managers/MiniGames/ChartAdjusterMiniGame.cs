using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class ChartAdjusterMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement container;

    private VisualElement caCompetitorBar;
    private VisualElement caAverageBar;
    private VisualElement caOurBar;
    private Label caTimerText;
    private Label caStatusText;
    private VisualElement caChartArea;

    private float timer = 10f;
    private float caCompVal = 80f;
    private float caOurVal = 20f;
    private bool isPlaying = false;
    private Vector2 lastPos = Vector2.zero;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        
        container = root.Q<VisualElement>("ChartAdjusterContainer");
        caCompetitorBar = container.Q<VisualElement>("CaCompetitorBar");
        caAverageBar = container.Q<VisualElement>("CaAverageBar");
        caOurBar = container.Q<VisualElement>("CaOurBar");
        caTimerText = container.Q<Label>("CaTimerText");
        caStatusText = container.Q<Label>("CaStatusText");
        caChartArea = container.Q<VisualElement>("CaChartArea");

        if (caChartArea != null)
        {
            caChartArea.RegisterCallback<PointerDownEvent>(ev => {
                if (!isPlaying || !manager.isPlaying) return;
                lastPos = ev.position;
                caChartArea.CapturePointer(ev.pointerId);
                ev.StopPropagation();
            });

            caChartArea.RegisterCallback<PointerUpEvent>(ev => {
                if (caChartArea.HasPointerCapture(ev.pointerId))
                {
                    caChartArea.ReleasePointer(ev.pointerId);
                }
            });

            caChartArea.RegisterCallback<PointerMoveEvent>(ev => {
                if (!isPlaying || !manager.isPlaying) return;
                
                if (caChartArea.HasPointerCapture(ev.pointerId))
                {
                    float deltaY = ev.position.y - lastPos.y;
                    float x = ev.localPosition.x;
                    float width = caChartArea.layout.width;

                    if (x < width * 0.33f)
                    {
                        caCompVal -= deltaY * 0.2f;
                        caCompVal = Mathf.Clamp(caCompVal, 5f, 95f);
                        if (caCompetitorBar != null) caCompetitorBar.style.height = new Length(caCompVal, LengthUnit.Percent);
                    }
                    else if (x > width * 0.66f)
                    {
                        caOurVal -= deltaY * 0.2f;
                        caOurVal = Mathf.Clamp(caOurVal, 5f, 95f);
                        if (caOurBar != null) caOurBar.style.height = new Length(caOurVal, LengthUnit.Percent);
                    }
                    
                    lastPos = ev.position;
                }
            });
        }
    }

    public void StartGame()
    {
        isPlaying = true;
        timer = config.timeLimit > 0 ? config.timeLimit : 10f;
        container.style.display = DisplayStyle.Flex;

        if (caTimerText != null) caTimerText.text = $"{timer:F1}s";
        if (caStatusText != null)
        {
            caStatusText.text = "Q3 REVENUE PROJECTIONS";
            caStatusText.style.color = new StyleColor(ColorUtility.TryParseHtmlString("#1E293B", out Color c1) ? c1 : Color.black);
        }
        
        caCompVal = 80f;
        caOurVal = 20f;
        
        if (caCompetitorBar != null) caCompetitorBar.style.height = new Length(caCompVal, LengthUnit.Percent);
        if (caAverageBar != null) caAverageBar.style.height = new Length(40f, LengthUnit.Percent);
        if (caOurBar != null) caOurBar.style.height = new Length(caOurVal, LengthUnit.Percent);

        manager.StartCoroutine(ChartAdjusterRoutine());
    }

    public void AbortGame()
    {
        isPlaying = false;
        container.style.display = DisplayStyle.None;
    }

    private IEnumerator ChartAdjusterRoutine()
    {
        while (isPlaying && manager.isPlaying)
        {
            timer -= Time.deltaTime;
            
            if (caTimerText != null)
            {
                caTimerText.text = $"{Mathf.Max(0, timer):F1}s";
            }
            
            if (caOurVal >= 80f && caCompVal <= 20f)
            {
                if (caTimerText != null) caTimerText.text = "BOARD IMPRESSED!";
                if (caStatusText != null)
                {
                    caStatusText.text = "GROWTH VERIFIED";
                    caStatusText.style.color = new StyleColor(Color.green);
                }
                isPlaying = false;
                MiniGameFeedback.PlaySuccess(caOurBar, "STOCKS SOAR!", container);
                AudioManager.Instance?.PlayVictoryFanfare();
                manager.MarkWinReady();
                yield break;
            }

            if (timer <= 0f)
            {
                if (manager.currentGame != null && manager.currentGame.canFail)
                {
                    if (caTimerText != null) caTimerText.text = "BOARD ANGRY!";
                    isPlaying = false;
                    MiniGameFeedback.PlayError(container, "BOARD ANGRY!", container);
                    AudioManager.Instance?.PlayMinigameFail();
                    manager.AbortGame();
                }
                else
                {
                    isPlaying = false;
                    AudioManager.Instance?.PlayVictoryFanfare();
                    manager.MarkWinReady();
                }
                yield break;
            }

            yield return null;
        }
    }
}

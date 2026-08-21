using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class DragCenterMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement container;

    private Label scoreText;
    private Label timeText;
    private VisualElement workArea;
    private VisualElement draggableIcon;
    private Button confirmBtn;
    private VisualElement targetCrosshair;
    private VisualElement scoreArea;
    private Label scoreValueText;

    private int score = 0;
    private float timeRemaining = 15f;
    private bool isDragging = false;
    private bool isScoring = false;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        container = root.Q<VisualElement>("DragCenterContainer");

        scoreText = container.Q<Label>("DcScoreText");
        timeText = container.Q<Label>("DcTimeText");
        workArea = container.Q<VisualElement>("DcWorkArea");
        draggableIcon = container.Q<VisualElement>("DcDraggableIcon");
        confirmBtn = container.Q<Button>("DcConfirmBtn");
        targetCrosshair = container.Q<VisualElement>("DcTargetCrosshair");
        scoreArea = container.Q<VisualElement>("DcScoreArea");
        scoreValueText = container.Q<Label>("DcScoreValueText");

        draggableIcon.RegisterCallback<PointerDownEvent>(OnPointerDown);
        draggableIcon.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        draggableIcon.RegisterCallback<PointerUpEvent>(OnPointerUp);
        draggableIcon.RegisterCallback<PointerCaptureOutEvent>(OnPointerCaptureOut);
        
        confirmBtn.clicked += OnConfirmClicked;
    }

    public void StartGame()
    {
        score = 0;
        timeRemaining = config.timeLimit > 0 ? config.timeLimit : 15f;
        container.style.display = DisplayStyle.Flex;

        UpdateScoreText();
        ResetRound();

        manager.StartCoroutine(GameRoutine());
    }

    public void AbortGame()
    {
        container.style.display = DisplayStyle.None;
    }

    private void ResetRound()
    {
        isScoring = false;
        confirmBtn.style.display = DisplayStyle.Flex;
        scoreArea.style.display = DisplayStyle.None;
        targetCrosshair.style.display = DisplayStyle.None;
        
        draggableIcon.style.left = new Length(Random.Range(10f, 90f), LengthUnit.Percent);
        draggableIcon.style.top = new Length(Random.Range(10f, 90f), LengthUnit.Percent);
    }

    private void OnPointerDown(PointerDownEvent e)
    {
        if (!manager.isPlaying || isScoring) return;
        isDragging = true;
        draggableIcon.CapturePointer(e.pointerId);
        e.StopPropagation();
    }

    private void OnPointerMove(PointerMoveEvent e)
    {
        if (!isDragging || !manager.isPlaying) return;
        
        Vector2 localPos = workArea.WorldToLocal(e.position);
        float width = workArea.resolvedStyle.width;
        float height = workArea.resolvedStyle.height;
        
        if (width > 0 && height > 0)
        {
            float percentX = (localPos.x / width) * 100f;
            float percentY = (localPos.y / height) * 100f;
            
            draggableIcon.style.left = new Length(Mathf.Clamp(percentX, 0f, 100f), LengthUnit.Percent);
            draggableIcon.style.top = new Length(Mathf.Clamp(percentY, 0f, 100f), LengthUnit.Percent);
        }
        e.StopPropagation();
    }

    private void OnPointerUp(PointerUpEvent e)
    {
        if (!isDragging) return;
        isDragging = false;
        if (draggableIcon.HasPointerCapture(e.pointerId))
        {
            draggableIcon.ReleasePointer(e.pointerId);
        }
        e.StopPropagation();
    }

    private void OnPointerCaptureOut(PointerCaptureOutEvent e)
    {
        isDragging = false;
    }

    private void OnConfirmClicked()
    {
        if (!manager.isPlaying || isScoring) return;
        manager.StartCoroutine(ScoreRoutine());
    }

    private IEnumerator ScoreRoutine()
    {
        isScoring = true;
        confirmBtn.style.display = DisplayStyle.None;
        
        targetCrosshair.style.display = DisplayStyle.Flex;
        
        float iconX = draggableIcon.style.left.value.value;
        float iconY = draggableIcon.style.top.value.value;
        
        float distX = Mathf.Abs(50f - iconX);
        float distY = Mathf.Abs(50f - iconY);
        float maxDist = Mathf.Sqrt(50f*50f + 50f*50f); 
        float actualDist = Mathf.Sqrt(distX*distX + distY*distY);
        
        float matchPercent = Mathf.Clamp01(1f - (actualDist / maxDist)) * 100f;
        // Make it easier: if within 8%, consider it perfect
        if (actualDist < 8f) matchPercent = 100f; 
        
        scoreArea.style.display = DisplayStyle.Flex;
        scoreValueText.text = $"{matchPercent:F0}% MATCH";
        
        if (matchPercent >= 95f)
        {
            scoreValueText.style.color = new StyleColor(new Color(0.13f, 0.77f, 0.36f)); // Green
            score++;
            UpdateScoreText();
        }
        else
        {
            scoreValueText.style.color = new StyleColor(new Color(0.9f, 0.1f, 0.1f)); // Red
            timeRemaining -= 2f;
        }
        
        yield return new WaitForSeconds(1f);
        
        if (score >= 3)
        {
            manager.MarkWinReady();
        }
        else
        {
            ResetRound();
        }
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = $"CENTERED: {score} / 3";
        }
    }

    private IEnumerator GameRoutine()
    {
        while (manager.isPlaying)
        {
            if (!isScoring)
            {
                timeRemaining -= Time.deltaTime;
                if (timeText != null) timeText.text = $"TIME: {Mathf.Max(0, timeRemaining):F1}s";

                if (timeRemaining <= 0)
                {
                    manager.AbortGame();
                    yield break;
                }
            }
            yield return null;
        }
    }
}

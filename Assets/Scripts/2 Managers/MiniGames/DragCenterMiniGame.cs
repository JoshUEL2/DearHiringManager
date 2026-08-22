using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class DragCenterMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement container;

    private Label scoreText;   // DcRoundText (round counter)
    private Label timeText;    // DcTimerText
    private VisualElement workArea;
    private Label draggableIcon; // Label with emoji
    private Button confirmBtn;   // DcBtnConfirm
    private VisualElement targetCrosshair;
    private VisualElement scoreArea;
    private Label scoreValueText;  // DcScoreText (accuracy %)
    private VisualElement scoreBarFill; // DcScoreBarFill

    private int chances = 3;
    private float timeRemaining = 15f;
    private bool isDragging = false;
    private bool isScoring = false;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        container = root.Q<VisualElement>("DragCenterContainer");

        // Use the ORIGINAL element names from the UXML
        scoreText = container.Q<Label>("DcRoundText");
        timeText = container.Q<Label>("DcTimerText");
        workArea = container.Q<VisualElement>("DcWorkArea");
        draggableIcon = container.Q<Label>("DcDraggableIcon");
        confirmBtn = container.Q<Button>("DcBtnConfirm");
        targetCrosshair = container.Q<VisualElement>("DcTargetCrosshair");
        scoreArea = container.Q<VisualElement>("DcScoreArea");
        scoreValueText = container.Q<Label>("DcScoreText");
        scoreBarFill = container.Q<VisualElement>("DcScoreBarFill");

        draggableIcon.RegisterCallback<PointerDownEvent>(OnPointerDown);
        draggableIcon.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        draggableIcon.RegisterCallback<PointerUpEvent>(OnPointerUp);
        draggableIcon.RegisterCallback<PointerCaptureOutEvent>(OnPointerCaptureOut);
        
        confirmBtn.clicked += OnConfirmClicked;
    }

    public void StartGame()
    {
        chances = 3;
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
        
        scoreArea.style.display = DisplayStyle.Flex;
        if (scoreValueText != null) scoreValueText.text = $"Accuracy: {matchPercent:F0}%";
        
        // Animate the score bar
        if (scoreBarFill != null) scoreBarFill.style.width = new Length(matchPercent, LengthUnit.Percent);
        
        if (matchPercent >= 90f) // Use legacy 90% threshold
        {
            if (scoreValueText != null) scoreValueText.style.color = new StyleColor(new Color(0.13f, 0.77f, 0.36f)); // Green
            yield return new WaitForSeconds(1f);
            manager.MarkWinReady(); // Win immediately!
        }
        else
        {
            if (scoreValueText != null) scoreValueText.style.color = new StyleColor(new Color(0.9f, 0.1f, 0.1f)); // Red
            chances--;
            UpdateScoreText();
            yield return new WaitForSeconds(1f);
            
            if (chances <= 0)
            {
                manager.AbortGame(); // Out of chances, fail game
            }
            else
            {
                ResetRound();
            }
        }
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Chances left: {chances}";
        }
    }

    private IEnumerator GameRoutine()
    {
        while (manager.isPlaying)
        {
            if (!isScoring)
            {
                timeRemaining -= Time.deltaTime;
                if (timeText != null) timeText.text = $"{Mathf.Max(0, timeRemaining):F1}s";

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

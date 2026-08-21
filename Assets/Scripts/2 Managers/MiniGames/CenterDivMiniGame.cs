using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class CenterDivMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement container;

    private Label scoreText;
    private Label timeText;
    private VisualElement targetBox;
    private VisualElement movingBox;
    private Button centerBtn;
    private VisualElement flashOverlay;

    private int score = 0;
    private float timeRemaining = 15f;
    private float currentSpeed = 300f;
    private float movingBoxPos = 0f;
    private float movingBoxDir = 1f;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        container = root.Q<VisualElement>("CenterDivContainer");

        scoreText = container.Q<Label>("CdScoreText");
        timeText = container.Q<Label>("CdTimeText");
        targetBox = container.Q<VisualElement>("CdTargetBox");
        movingBox = container.Q<VisualElement>("CdMovingBox");
        centerBtn = container.Q<Button>("CdCenterBtn");
        flashOverlay = container.Q<VisualElement>("CdFlashOverlay");

        centerBtn.clicked += OnCenterClicked;
    }

    public void StartGame()
    {
        score = 0;
        timeRemaining = config.timeLimit > 0 ? config.timeLimit : 15f;
        container.style.display = DisplayStyle.Flex;

        UpdateScoreText();
        SetNextTarget();

        manager.StartCoroutine(GameRoutine());
    }

    public void AbortGame()
    {
        container.style.display = DisplayStyle.None;
        centerBtn.clicked -= OnCenterClicked;
    }

    private void SetNextTarget()
    {
        float targetLeft = Random.Range(20f, 80f);
        targetBox.style.left = new Length(targetLeft, LengthUnit.Percent);
        
        currentSpeed = Random.Range(config.centerDivSpeedMin > 0 ? config.centerDivSpeedMin : 300f, 
                                    config.centerDivSpeedMax > 0 ? config.centerDivSpeedMax : 700f);
        movingBoxPos = Random.value > 0.5f ? 0f : 100f;
        movingBoxDir = movingBoxPos == 0f ? 1f : -1f;
        
        movingBox.style.backgroundColor = new StyleColor(new Color(0.13f, 0.84f, 0.96f)); // Cyan
    }

    private void OnCenterClicked()
    {
        if (!manager.isPlaying) return;

        float targetLeft = targetBox.style.left.value.value; 
        float dist = Mathf.Abs(targetLeft - movingBoxPos);
        float tolerance = 15f; 

        if (dist <= tolerance)
        {
            score++;
            UpdateScoreText();
            manager.StartCoroutine(FlashEffect(true));

            if (score >= config.centerDivTargets)
            {
                movingBox.style.backgroundColor = new StyleColor(new Color(0.13f, 0.77f, 0.36f)); // Green
                manager.MarkWinReady();
            }
            else
            {
                SetNextTarget();
            }
        }
        else
        {
            manager.StartCoroutine(FlashEffect(false));
            timeRemaining -= 2f;
        }
    }

    private IEnumerator FlashEffect(bool success)
    {
        flashOverlay.style.opacity = 1f;
        flashOverlay.style.backgroundColor = success ? new StyleColor(Color.white) : new StyleColor(new Color(0.9f, 0.1f, 0.1f, 0.5f));
        yield return new WaitForSeconds(0.1f);
        flashOverlay.style.opacity = 0f;
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = $"CENTERED: {score} / {config.centerDivTargets}";
        }
    }

    private IEnumerator GameRoutine()
    {
        while (manager.isPlaying)
        {
            float dt = Time.deltaTime;
            timeRemaining -= dt;
            if (timeText != null) timeText.text = $"TIME: {Mathf.Max(0, timeRemaining):F1}s";

            if (timeRemaining <= 0)
            {
                manager.AbortGame();
                yield break;
            }

            movingBoxPos += currentSpeed * movingBoxDir * dt * 0.05f; 
            if (movingBoxPos > 100f)
            {
                movingBoxPos = 100f;
                movingBoxDir = -1f;
            }
            else if (movingBoxPos < 0f)
            {
                movingBoxPos = 0f;
                movingBoxDir = 1f;
            }

            if (movingBox != null)
            {
                movingBox.style.left = new Length(movingBoxPos, LengthUnit.Percent);
            }

            yield return null;
        }
    }
}

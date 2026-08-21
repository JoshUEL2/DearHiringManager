using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class SizeMatchMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement container;

    private Label scoreText;
    private Label timeText;
    private VisualElement targetBox;
    private VisualElement changingBox;
    private VisualElement flashOverlay;

    private int score = 0;
    private float timeRemaining = 15f;
    private float currentScale = 0.2f;
    private float scaleDir = 1f;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        container = root.Q<VisualElement>("SizeMatchContainer");

        scoreText = container.Q<Label>("SmScoreText");
        timeText = container.Q<Label>("SmTimeText");
        targetBox = container.Q<VisualElement>("SmTargetBox");
        changingBox = container.Q<VisualElement>("SmChangingBox");
        flashOverlay = container.Q<VisualElement>("SmFlashOverlay");

        changingBox.RegisterCallback<PointerDownEvent>(OnBoxClicked);
    }

    public void StartGame()
    {
        score = 0;
        timeRemaining = config.timeLimit > 0 ? config.timeLimit : 15f;
        currentScale = 0.2f;
        scaleDir = 1f;
        container.style.display = DisplayStyle.Flex;
        changingBox.style.backgroundColor = new StyleColor(new Color(0.96f, 0.84f, 0.13f)); // Yellow

        UpdateScoreText();
        manager.StartCoroutine(GameRoutine());
    }

    public void AbortGame()
    {
        container.style.display = DisplayStyle.None;
        changingBox.UnregisterCallback<PointerDownEvent>(OnBoxClicked);
    }

    private void OnBoxClicked(PointerDownEvent e)
    {
        if (!manager.isPlaying) return;

        float diff = Mathf.Abs(currentScale - 1.0f);
        float tolerance = 0.15f; // Slightly forgiving

        if (diff <= tolerance)
        {
            score++;
            UpdateScoreText();
            manager.StartCoroutine(FlashEffect(true));

            if (score >= 3)
            {
                changingBox.style.backgroundColor = new StyleColor(new Color(0.13f, 0.77f, 0.36f)); // Green
                manager.MarkWinReady();
            }
        }
        else
        {
            manager.StartCoroutine(FlashEffect(false));
            timeRemaining -= 2f;
        }
        
        e.StopPropagation();
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
            scoreText.text = $"MATCHED: {score} / 3";
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

            float scaleSpeed = 1.2f; // Speed of pulsing
            currentScale += scaleSpeed * scaleDir * dt;
            
            if (currentScale >= 1.5f)
            {
                currentScale = 1.5f;
                scaleDir = -1f;
            }
            else if (currentScale <= 0.2f)
            {
                currentScale = 0.2f;
                scaleDir = 1f;
            }

            if (changingBox != null)
            {
                changingBox.style.scale = new StyleScale(new Scale(new Vector2(currentScale, currentScale)));
            }

            yield return null;
        }
    }
}

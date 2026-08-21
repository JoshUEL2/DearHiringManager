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
    private Button changingBox; // Button so clicks work at all sizes
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

        // Use the ORIGINAL element names from the UXML
        scoreText = container.Q<Label>("SmRoundText");
        timeText = container.Q<Label>("SmTimerText");
        targetBox = container.Q<VisualElement>("SmTargetBox");
        changingBox = container.Q<Button>("SmBtnChangingBox");
        flashOverlay = container.Q<VisualElement>("SmFlashOverlay");

        changingBox.clicked += OnBoxClicked;
    }

    public void StartGame()
    {
        score = 0;
        timeRemaining = config.timeLimit > 0 ? config.timeLimit : 15f;
        currentScale = 0.2f;
        scaleDir = 1f;
        container.style.display = DisplayStyle.Flex;
        changingBox.style.backgroundColor = new StyleColor(new Color(0.96f, 0.62f, 0.04f)); // Orange

        UpdateScoreText();
        manager.StartCoroutine(GameRoutine());
    }

    public void AbortGame()
    {
        container.style.display = DisplayStyle.None;
        changingBox.clicked -= OnBoxClicked;
    }

    private void OnBoxClicked()
    {
        if (!manager.isPlaying) return;

        float diff = Mathf.Abs(currentScale - 1.0f);
        float tolerance = 0.1f; // Match legacy tolerance

        if (diff <= tolerance)
        {
            score++;
            UpdateScoreText();

            // Flash green
            changingBox.style.backgroundColor = new StyleColor(new Color(0.13f, 0.77f, 0.36f));
            changingBox.schedule.Execute(() => {
                if (changingBox != null) changingBox.style.backgroundColor = new StyleColor(new Color(0.96f, 0.62f, 0.04f));
            }).StartingIn(200);

            if (score >= 3)
            {
                manager.MarkWinReady();
            }
            else
            {
                // Reset scale for next round
                currentScale = 0.2f;
                scaleDir = 1f;
            }
        }
        else
        {
            // Flash red
            changingBox.style.backgroundColor = new StyleColor(new Color(0.93f, 0.26f, 0.22f));
            changingBox.schedule.Execute(() => {
                if (changingBox != null) changingBox.style.backgroundColor = new StyleColor(new Color(0.96f, 0.62f, 0.04f));
            }).StartingIn(200);

            timeRemaining -= 1.5f;
        }
    }

    private IEnumerator FlashEffect(bool success)
    {
        if (flashOverlay == null) yield break;
        flashOverlay.style.opacity = 1f;
        flashOverlay.style.backgroundColor = success ? new StyleColor(Color.white) : new StyleColor(new Color(0.9f, 0.1f, 0.1f, 0.5f));
        yield return new WaitForSeconds(0.1f);
        flashOverlay.style.opacity = 0f;
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Matched: {score}/3";
        }
    }

    private IEnumerator GameRoutine()
    {
        while (manager.isPlaying)
        {
            float dt = Time.deltaTime;
            timeRemaining -= dt;
            if (timeText != null) timeText.text = $"{Mathf.Max(0, timeRemaining):F1}s";

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
                changingBox.style.width = new Length(200f * currentScale, LengthUnit.Pixel);
                changingBox.style.height = new Length(150f * currentScale, LengthUnit.Pixel);
                
                // Update text to TAP when in tolerance range
                if (Mathf.Abs(currentScale - 1.0f) <= 0.1f)
                {
                    changingBox.text = "TAP";
                }
                else
                {
                    changingBox.text = "this";
                }
            }

            yield return null;
        }
    }
}

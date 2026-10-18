using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;

public class UnitTestRunnerMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement container;

    private Label utRoundText;
    private Label utTimerText;
    private Label utCodeText;
    private Button utBtnOption1;
    private Button utBtnOption2;
    private Button utBtnOption3;
    private VisualElement codeWindow;

    // Juice Elements
    private VisualElement flashOverlay;
    private Label floatingPenaltyText;
    
    private int utCurrentRound = 0;
    private float timer = 30f;
    private bool isPlaying = false;
    private bool isTransitioning = false; // Prevents spam clicking during animations

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        
        container = root.Q<VisualElement>("UnitTestContainer");
        container.style.width = new Length(100f, LengthUnit.Percent); // Stop shrinking

        utRoundText = container.Q<Label>("UtRoundText");
        utRoundText.style.width = 200; // Fixed width to prevent layout collapse
        
        utTimerText = container.Q<Label>("UtTimerText");
        utCodeText = container.Q<Label>("UtCodeText");
        utCodeText.enableRichText = true; // Ensure color tags work
        
        utBtnOption1 = container.Q<Button>("UtBtnOption1");
        utBtnOption2 = container.Q<Button>("UtBtnOption2");
        utBtnOption3 = container.Q<Button>("UtBtnOption3");
        
        codeWindow = utCodeText.parent; // The dark box containing the code

        if (utBtnOption1 != null) utBtnOption1.clicked += () => OnUtBtnOption(0, utBtnOption1);
        if (utBtnOption2 != null) utBtnOption2.clicked += () => OnUtBtnOption(1, utBtnOption2);
        if (utBtnOption3 != null) utBtnOption3.clicked += () => OnUtBtnOption(2, utBtnOption3);

        CreateJuiceElements();
    }

    private void CreateJuiceElements()
    {
        // Style Code Window to look like an IDE
        codeWindow.style.backgroundColor = new StyleColor(new Color(0.04f, 0.06f, 0.1f)); 
        codeWindow.style.borderLeftWidth = 6;
        codeWindow.style.borderLeftColor = new StyleColor(new Color(0.22f, 0.74f, 0.97f)); // Tech Blue gutter
        
        // Flash Overlay for Code Window
        flashOverlay = new VisualElement();
        flashOverlay.style.position = Position.Absolute;
        flashOverlay.style.left = 0;
        flashOverlay.style.right = 0;
        flashOverlay.style.top = 0;
        flashOverlay.style.bottom = 0;
        flashOverlay.style.backgroundColor = new StyleColor(new Color(1f, 1f, 1f, 0f));
        flashOverlay.style.borderTopLeftRadius = 10;
        flashOverlay.style.borderTopRightRadius = 10;
        flashOverlay.style.borderBottomLeftRadius = 10;
        flashOverlay.style.borderBottomRightRadius = 10;
        codeWindow.Add(flashOverlay);

        // Floating Penalty Text
        floatingPenaltyText = new Label("-2.0s");
        floatingPenaltyText.style.position = Position.Absolute;
        floatingPenaltyText.style.color = new StyleColor(new Color(0.93f, 0.26f, 0.22f)); // Red
        floatingPenaltyText.style.fontSize = 28;
        floatingPenaltyText.style.unityFontStyleAndWeight = FontStyle.Bold;
        floatingPenaltyText.style.opacity = 0f;
        floatingPenaltyText.style.right = 10;
        floatingPenaltyText.style.top = 10;
        codeWindow.Add(floatingPenaltyText); 
        
        // Style buttons baseline
        ResetButtonStyles();
    }

    private void ResetButtonStyles()
    {
        StyleColor defaultBg = new StyleColor(new Color(0.12f, 0.16f, 0.23f));
        StyleColor defaultBorder = new StyleColor(new Color(0.28f, 0.33f, 0.41f));
        
        utBtnOption1.style.backgroundColor = defaultBg;
        utBtnOption2.style.backgroundColor = defaultBg;
        utBtnOption3.style.backgroundColor = defaultBg;
        
        utBtnOption1.style.borderTopColor = defaultBorder;
        utBtnOption2.style.borderTopColor = defaultBorder;
        utBtnOption3.style.borderTopColor = defaultBorder;
        
        utBtnOption1.style.borderBottomColor = defaultBorder;
        utBtnOption2.style.borderBottomColor = defaultBorder;
        utBtnOption3.style.borderBottomColor = defaultBorder;
        
        utBtnOption1.style.borderLeftColor = defaultBorder;
        utBtnOption2.style.borderLeftColor = defaultBorder;
        utBtnOption3.style.borderLeftColor = defaultBorder;
        
        utBtnOption1.style.borderRightColor = defaultBorder;
        utBtnOption2.style.borderRightColor = defaultBorder;
        utBtnOption3.style.borderRightColor = defaultBorder;
        
        utBtnOption1.style.scale = new StyleScale(new Vector2(1f, 1f));
        utBtnOption2.style.scale = new StyleScale(new Vector2(1f, 1f));
        utBtnOption3.style.scale = new StyleScale(new Vector2(1f, 1f));
    }

    public void StartGame()
    {
        isPlaying = true;
        isTransitioning = false;
        utCurrentRound = 0;
        timer = config.timeLimit > 0 ? config.timeLimit : 30f;
        container.style.display = DisplayStyle.Flex;
        
        ResetButtonStyles();
        LoadNextTest();
        manager.StartCoroutine(UnitTestRoutine());
    }

    public void AbortGame()
    {
        isPlaying = false;
        container.style.display = DisplayStyle.None;
    }

    private void LoadNextTest()
    {
        if (utCurrentRound >= config.unitTestRounds)
        {
            isPlaying = false;
            AudioManager.Instance?.PlayVictoryFanfare();
            manager.MarkWinReady();
            return;
        }

        if (utRoundText != null) utRoundText.text = $"Test: {utCurrentRound + 1}/{config.unitTestRounds}";
        
        string[] options = config.unitTestOptions[utCurrentRound];
        if (utBtnOption1 != null) utBtnOption1.text = options.Length > 0 ? options[0] : "";
        if (utBtnOption2 != null) utBtnOption2.text = options.Length > 1 ? options[1] : "";
        if (utBtnOption3 != null) utBtnOption3.text = options.Length > 2 ? options[2] : "";

        string targetText = config.unitTestQuestions[utCurrentRound];
        AudioManager.Instance?.PlayTypewriter();
        manager.StartCoroutine(TypewriterEffect(targetText));
    }

    private IEnumerator TypewriterEffect(string fullText)
    {
        utCodeText.text = "_";
        int length = fullText.Length;
        float delay = 0.15f / Mathf.Max(1, length); // Type out over 0.15 seconds total

        for (int i = 1; i <= length; i++)
        {
            if (!isPlaying) yield break;
            // Append blinking cursor
            string cursor = (i % 2 == 0) ? "_" : ""; 
            if (i == length) cursor = ""; // No cursor at the exact end so it looks clean
            
            utCodeText.text = fullText.Substring(0, i) + cursor;
            yield return new WaitForSeconds(delay);
        }
        utCodeText.text = fullText; // Ensure exact match
    }

    private void OnUtBtnOption(int idx, Button btn)
    {
        if (!isPlaying || !manager.isPlaying || isTransitioning) return;

        // Button physically depresses
        btn.style.scale = new StyleScale(new Vector2(0.95f, 0.95f));
        btn.schedule.Execute(() => {
            if (btn != null) btn.style.scale = new StyleScale(new Vector2(1f, 1f));
        }).StartingIn(100);

        if (idx == config.unitTestCorrectIndices[utCurrentRound])
        {
            manager.StartCoroutine(HandleSuccess(btn));
        }
        else
        {
            manager.StartCoroutine(HandleFailure(btn));
        }
    }

    private IEnumerator HandleSuccess(Button btn)
    {
        isTransitioning = true;
        AudioManager.Instance?.PlayScorePop();
        HapticManager.LightImpact();
        MiniGameFeedback.Punch(btn, 1.18f);

        // Success Juice
        StyleColor greenColor = new StyleColor(new Color(0.13f, 0.77f, 0.36f));
        btn.style.backgroundColor = greenColor;
        btn.style.borderTopColor = greenColor;
        btn.style.borderBottomColor = greenColor;
        btn.style.borderLeftColor = greenColor;
        btn.style.borderRightColor = greenColor;
        
        flashOverlay.style.backgroundColor = new StyleColor(new Color(0.13f, 0.77f, 0.36f, 0.2f));

        // Replace the "?" in the code with the answer
        string currentText = config.unitTestQuestions[utCurrentRound];
        string answer = btn.text;
        utCodeText.text = currentText.Replace("?", $"<color=#22C55E>{answer}</color>"); // Green answer

        yield return new WaitForSeconds(0.1f);
        flashOverlay.style.backgroundColor = new StyleColor(new Color(1f, 1f, 1f, 0f));
        
        yield return new WaitForSeconds(0.2f); // Brief cinematic pause

        utCurrentRound++;
        ResetButtonStyles();
        isTransitioning = false;
        LoadNextTest();
    }

    private IEnumerator HandleFailure(Button btn)
    {
        // Failure Juice (Do NOT set isTransitioning, allow them to keep trying immediately)
        AudioManager.Instance?.PlayBuzzer();
        HapticManager.MediumImpact();
        StyleColor redColor = new StyleColor(new Color(0.93f, 0.26f, 0.22f));
        btn.style.backgroundColor = redColor;
        
        flashOverlay.style.backgroundColor = new StyleColor(new Color(0.93f, 0.26f, 0.22f, 0.3f));
        
        timer -= 2f; // Penalty
        
        // Float penalty text up
        manager.StartCoroutine(AnimatePenaltyText());

        // Screen Shake the Code Window
        float shakeAmt = 10f;
        codeWindow.style.left = shakeAmt;
        yield return new WaitForSeconds(0.05f);
        codeWindow.style.left = -shakeAmt;
        yield return new WaitForSeconds(0.05f);
        codeWindow.style.left = 0f;

        flashOverlay.style.backgroundColor = new StyleColor(new Color(1f, 1f, 1f, 0f));
        
        yield return new WaitForSeconds(0.15f);
        
        // Reset button color so they can try again
        StyleColor defaultBg = new StyleColor(new Color(0.12f, 0.16f, 0.23f));
        btn.style.backgroundColor = defaultBg;
    }

    private IEnumerator AnimatePenaltyText()
    {
        floatingPenaltyText.style.opacity = 1f;
        floatingPenaltyText.style.top = 10;
        
        float life = 0.5f;
        float elapsed = 0f;
        while(elapsed < life && isPlaying)
        {
            elapsed += Time.deltaTime;
            float p = elapsed / life;
            
            // Float up and fade out
            floatingPenaltyText.style.top = 10f - (p * 30f);
            floatingPenaltyText.style.opacity = 1f - p;
            
            // Pulse timer red
            if (utTimerText != null) {
                utTimerText.style.color = new StyleColor(Color.Lerp(Color.red, new Color(0.97f, 0.44f, 0.44f), p)); // F87171 is roughly 0.97, 0.44, 0.44
            }
            
            yield return null;
        }
        floatingPenaltyText.style.opacity = 0f;
    }

    private IEnumerator UnitTestRoutine()
    {
        while (isPlaying && manager.isPlaying)
        {
            timer -= Time.deltaTime;
            if (utTimerText != null) utTimerText.text = $"{Mathf.Max(0, timer):F1}s";

            if (timer <= 0f)
            {
                if (manager.currentGame != null && manager.currentGame.canFail)
                {
                    if (utTimerText != null) utTimerText.text = "FAILED!";
                    isPlaying = false;
                    AudioManager.Instance?.PlayMinigameFail();
                    manager.AbortGame();
                    yield break;
                }
                else
                {
                    timer = 30f;
                }
            }
            yield return null;
        }
    }
}

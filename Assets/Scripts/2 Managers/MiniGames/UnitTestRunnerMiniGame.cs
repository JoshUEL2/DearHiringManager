using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

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

    private int utCurrentRound = 0;
    private float timer = 30f;
    private bool isPlaying = false;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        
        container = root.Q<VisualElement>("UnitTestContainer");
        utRoundText = container.Q<Label>("UtRoundText");
        utTimerText = container.Q<Label>("UtTimerText");
        utCodeText = container.Q<Label>("UtCodeText");
        utBtnOption1 = container.Q<Button>("UtBtnOption1");
        utBtnOption2 = container.Q<Button>("UtBtnOption2");
        utBtnOption3 = container.Q<Button>("UtBtnOption3");

        if (utBtnOption1 != null) utBtnOption1.clicked += () => OnUtBtnOption(0);
        if (utBtnOption2 != null) utBtnOption2.clicked += () => OnUtBtnOption(1);
        if (utBtnOption3 != null) utBtnOption3.clicked += () => OnUtBtnOption(2);
    }

    public void StartGame()
    {
        isPlaying = true;
        utCurrentRound = 0;
        timer = config.timeLimit > 0 ? config.timeLimit : 30f;
        container.style.display = DisplayStyle.Flex;
        
        UpdateUtUI();
        manager.StartCoroutine(UnitTestRoutine());
    }

    public void AbortGame()
    {
        isPlaying = false;
        container.style.display = DisplayStyle.None;
    }

    private void UpdateUtUI()
    {
        if (utCurrentRound >= config.unitTestRounds)
        {
            isPlaying = false;
            manager.MarkWinReady();
            return;
        }

        if (utRoundText != null) utRoundText.text = $"Test: {utCurrentRound + 1}/{config.unitTestRounds}";
        if (utCodeText != null) utCodeText.text = config.unitTestQuestions[utCurrentRound];
        
        string[] options = config.unitTestOptions[utCurrentRound];
        if (utBtnOption1 != null) utBtnOption1.text = options.Length > 0 ? options[0] : "";
        if (utBtnOption2 != null) utBtnOption2.text = options.Length > 1 ? options[1] : "";
        if (utBtnOption3 != null) utBtnOption3.text = options.Length > 2 ? options[2] : "";
    }

    private void OnUtBtnOption(int idx)
    {
        if (!isPlaying || !manager.isPlaying) return;
        if (utCurrentRound >= config.unitTestRounds) return;

        if (idx == config.unitTestCorrectIndices[utCurrentRound])
        {
            utCurrentRound++;
            UpdateUtUI();
        }
        else
        {
            Button clickedBtn = idx == 0 ? utBtnOption1 : (idx == 1 ? utBtnOption2 : utBtnOption3);
            if (clickedBtn != null)
            {
                var originalColor = clickedBtn.style.backgroundColor;
                clickedBtn.style.backgroundColor = new StyleColor(Color.red);
                clickedBtn.schedule.Execute(() => { 
                    if (clickedBtn != null) clickedBtn.style.backgroundColor = originalColor; 
                }).StartingIn(200);
            }
            timer -= 2f;
        }
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

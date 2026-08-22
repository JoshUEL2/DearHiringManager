using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class FoodPrepRushMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement rootElement;

    private VisualElement container;
    private Label prepTimerText;
    private Label prepScoreText;
    private VisualElement[] prepBars = new VisualElement[4];
    private Label[] prepStatus = new Label[4];
    private Button[] prepBtns = new Button[4];

    private float[] prepProgress = new float[4];
    private float[] prepSpeeds = new float[4];
    private float[] prepFreezes = new float[4];
    
    private int score = 0;
    private float timer = 0f;
    
    private System.Action[] clickActions = new System.Action[4];

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        this.rootElement = root;

        container = root.Q<VisualElement>("FoodPrepContainer");
        if (container != null)
        {
            prepTimerText = container.Q<Label>("PrepTimerText");
            prepScoreText = container.Q<Label>("PrepScoreText");

            for (int i = 0; i < 4; i++)
            {
                prepBars[i] = container.Q<VisualElement>($"PrepBar{i}");
                prepStatus[i] = container.Q<Label>($"StationStatus{i}");
                prepBtns[i] = container.Q<Button>($"BtnStation{i}");
                
                int index = i;
                clickActions[i] = () => OnFoodPrepStationTapped(index);
            }
        }
    }

    public void StartGame()
    {
        if (container != null)
        {
            container.style.display = DisplayStyle.Flex;
            for (int i = 0; i < 4; i++)
            {
                if (prepBtns[i] != null)
                {
                    prepBtns[i].clicked += clickActions[i];
                }
            }
        }

        timer = config.timeLimit;
        score = 0;

        for (int i = 0; i < 4; i++)
        {
            prepSpeeds[i] = Random.Range(30f, 60f);
            prepProgress[i] = 0f;
            prepFreezes[i] = 0f;
            if (prepStatus[i] != null) prepStatus[i].text = "";
            if (prepBars[i] != null) prepBars[i].style.height = new Length(0, LengthUnit.Percent);
        }

        if (prepScoreText != null) prepScoreText.text = $"Prepped: {score} / {config.targetScore}";
        if (prepTimerText != null) prepTimerText.text = $"{timer:0.0}s";

        manager.StartCoroutine(FoodPrepRoutine());
    }

    public void AbortGame()
    {
        if (container != null)
        {
            container.style.display = DisplayStyle.None;
            for (int i = 0; i < 4; i++)
            {
                if (prepBtns[i] != null)
                {
                    prepBtns[i].clicked -= clickActions[i];
                }
            }
        }
    }

    private void OnFoodPrepStationTapped(int index)
    {
        if (!manager.isPlaying) return;
        if (prepFreezes[index] > 0f) return; // Station is frozen

        float p = prepProgress[index];
        if (p < 70f)
        {
            if (prepStatus[index] != null) prepStatus[index].text = "RAW!";
            prepFreezes[index] = 1.0f;
            if (manager.currentGame.canFail) score = Mathf.Max(0, score - 1);
        }
        else if (p >= 70f && p <= 95f)
        {
            if (prepStatus[index] != null) prepStatus[index].text = "PERFECT!";
            prepFreezes[index] = 0.5f;
            score++;
            if (score >= config.targetScore)
            {
                if (prepTimerText != null) prepTimerText.text = "YOU WIN!";
                manager.MarkWinReady();
            }
        }
        else
        {
            if (prepStatus[index] != null) prepStatus[index].text = (index == 2) ? "SPILLED!" : "BURNED!";
            prepFreezes[index] = 2.0f;
            if (manager.currentGame.canFail) score = Mathf.Max(0, score - 1);
        }
        
        if (prepScoreText != null) prepScoreText.text = $"Prepped: {score} / {config.targetScore}";
    }

    private IEnumerator FoodPrepRoutine()
    {
        while (manager.isPlaying && score < config.targetScore && timer > 0f)
        {
            timer -= Time.deltaTime;
            if (prepTimerText != null) prepTimerText.text = $"{timer:0.0}s";

            for (int i = 0; i < 4; i++)
            {
                if (prepFreezes[i] > 0f)
                {
                    prepFreezes[i] -= Time.deltaTime;
                    if (prepFreezes[i] <= 0f)
                    {
                        prepFreezes[i] = 0f;
                        if (prepStatus[i] != null) prepStatus[i].text = "";
                        prepProgress[i] = 0f;
                        prepSpeeds[i] = Random.Range(30f, 60f);
                    }
                }
                else
                {
                    prepProgress[i] += prepSpeeds[i] * Time.deltaTime;
                    if (prepProgress[i] >= 100f)
                    {
                        prepProgress[i] = 100f;
                        if (prepStatus[i] != null) prepStatus[i].text = (i == 2) ? "SPILLED!" : "BURNED!";
                        prepFreezes[i] = 2.0f;
                        if (manager.currentGame.canFail) score = Mathf.Max(0, score - 1);
                        if (prepScoreText != null) prepScoreText.text = $"Prepped: {score} / {config.targetScore}";
                    }
                }
                if (prepBars[i] != null) prepBars[i].style.height = new Length(prepProgress[i], LengthUnit.Percent);
            }

            if (timer <= 0f)
            {
                if (manager.currentGame.canFail)
                {
                    if (prepTimerText != null) prepTimerText.text = "TIME UP!";
                    manager.AbortGame();
                    yield break;
                }
                else
                {
                    manager.MarkWinReady();
                }
            }

            yield return null;
        }
    }
}

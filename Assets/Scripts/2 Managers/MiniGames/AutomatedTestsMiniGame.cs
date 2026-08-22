using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;

public class AutomatedTestsMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement container;

    private VisualElement atProgressBarFill;
    private Label atTimerText;
    private Button[] atBtnTests = new Button[5];
    private Label[] atStatusTexts = new Label[5];

    private float timer = 15f;
    private float atSuiteProgress = 0f;
    private float atNextFailTimer = 0f;
    private bool[] atIsFailing = new bool[5];
    private float[] atFuses = new float[5];
    
    private bool isPlaying = false;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        
        container = root.Q<VisualElement>("AutomatedTestsContainer");
        atProgressBarFill = container.Q<VisualElement>("AtProgressBarFill");
        atTimerText = container.Q<Label>("AtTimerText");

        for (int i = 0; i < 5; i++)
        {
            atBtnTests[i] = container.Q<Button>($"AtBtnTest{i + 1}");
            if (atBtnTests[i] != null)
            {
                atStatusTexts[i] = atBtnTests[i].Q<Label>("AtStatusText");
                int idx = i;
                atBtnTests[i].clicked += () => OnAtTestClicked(idx);
            }
        }
    }

    public void StartGame()
    {
        isPlaying = true;
        timer = config.timeLimit > 0 ? config.timeLimit : 15f;
        atSuiteProgress = 0f;
        atNextFailTimer = Random.Range(1.5f, 3.0f);
        
        container.style.display = DisplayStyle.Flex;

        for (int i = 0; i < 5; i++)
        {
            atIsFailing[i] = false;
            atFuses[i] = 0f;
            UpdateAtTestUI(i);
        }

        if (atProgressBarFill != null) atProgressBarFill.style.width = new Length(0f, LengthUnit.Percent);
        if (atTimerText != null) atTimerText.text = $"{timer:F1}s";

        manager.StartCoroutine(AutomatedTestsRoutine());
    }

    public void AbortGame()
    {
        isPlaying = false;
        container.style.display = DisplayStyle.None;
    }

    private void UpdateAtTestUI(int index)
    {
        if (atBtnTests[index] == null || atStatusTexts[index] == null) return;

        if (atIsFailing[index])
        {
            atStatusTexts[index].text = $"FAILING! ({atFuses[index]:F1}s)";
            atStatusTexts[index].style.color = new StyleColor(new Color(0.93f, 0.26f, 0.22f)); 
            atBtnTests[index].style.backgroundColor = new StyleColor(new Color(0.4f, 0.1f, 0.1f)); 
        }
        else
        {
            atStatusTexts[index].text = "PASSING";
            atStatusTexts[index].style.color = new StyleColor(new Color(0.13f, 0.77f, 0.36f)); 
            atBtnTests[index].style.backgroundColor = new StyleColor(new Color(0.12f, 0.16f, 0.23f)); 
        }
    }

    private void OnAtTestClicked(int index)
    {
        if (!isPlaying || !manager.isPlaying) return;

        if (atIsFailing[index])
        {
            atIsFailing[index] = false;
            UpdateAtTestUI(index);
        }
    }

    private IEnumerator AutomatedTestsRoutine()
    {
        while (isPlaying && manager.isPlaying)
        {
            timer -= Time.deltaTime;
            
            atSuiteProgress = 100f * (1f - (timer / (config.timeLimit > 0 ? config.timeLimit : 15f)));
            if (atProgressBarFill != null) atProgressBarFill.style.width = new Length(Mathf.Clamp(atSuiteProgress, 0f, 100f), LengthUnit.Percent);
            if (atTimerText != null) atTimerText.text = $"{Mathf.Max(0, timer):F1}s";

            atNextFailTimer -= Time.deltaTime;
            if (atNextFailTimer <= 0f)
            {
                List<int> available = new List<int>();
                for (int i = 0; i < 5; i++) if (!atIsFailing[i]) available.Add(i);
                
                if (available.Count > 0)
                {
                    int toFail = available[Random.Range(0, available.Count)];
                    atIsFailing[toFail] = true;
                    atFuses[toFail] = 3.0f; 
                    UpdateAtTestUI(toFail);
                }
                atNextFailTimer = Random.Range(1.0f, 2.5f);
            }

            for (int i = 0; i < 5; i++)
            {
                if (atIsFailing[i])
                {
                    atFuses[i] -= Time.deltaTime;
                    UpdateAtTestUI(i); 

                    if (atFuses[i] <= 0f)
                    {
                        if (atTimerText != null) atTimerText.text = "SUITE CRASHED!";
                        
                        if (manager.currentGame != null && manager.currentGame.canFail)
                        {
                            isPlaying = false;
                            manager.AbortGame();
                            yield break;
                        }
                        else
                        {
                            timer -= 5f; 
                            atIsFailing[i] = false;
                            UpdateAtTestUI(i);
                        }
                    }
                }
            }

            if (timer <= 0f)
            {
                bool anyFailing = false;
                for (int i = 0; i < 5; i++) if (atIsFailing[i]) anyFailing = true;

                if (anyFailing && manager.currentGame != null && manager.currentGame.canFail)
                {
                    if (atTimerText != null) atTimerText.text = "FAILED (Flaky test open)";
                    isPlaying = false;
                    manager.AbortGame();
                    yield break;
                }
                else
                {
                    if (atTimerText != null) atTimerText.text = "SUITE PASSED!";
                    isPlaying = false;
                    manager.MarkWinReady();
                    yield break;
                }
            }

            yield return null;
        }
    }
}

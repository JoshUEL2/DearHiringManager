using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;

public class CodeCommenterMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement container;

    private Label ccTimerText;
    private VisualElement[] ccLines = new VisualElement[5];
    private Label[] ccLineTexts = new Label[5];
    
    private bool[] ccLineIsFlaky = new bool[5];
    private bool[] ccLineCommented = new bool[5];
    private float[] ccLineTimers = new float[5];
    private string[] ccOriginalTexts = new string[5];

    private float timer = 10f;
    private bool isPlaying = false;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        
        container = root.Q<VisualElement>("CodeCommenterContainer");
        ccTimerText = container.Q<Label>("CcTimerText");

        for (int i = 0; i < 5; i++)
        {
            ccLines[i] = container.Q<VisualElement>($"CcLine{i + 1}");
            if (ccLines[i] != null)
            {
                ccLineTexts[i] = ccLines[i].Q<Label>($"CcLineText{i}");
                if (ccLineTexts[i] != null) ccOriginalTexts[i] = ccLineTexts[i].text;
                
                int idx = i;
                var btn = ccLines[i] as Button;
                if (btn != null) btn.clicked += () => OnCcLineClicked(idx);
            }
        }
    }

    public void StartGame()
    {
        isPlaying = true;
        timer = config.timeLimit > 0 ? config.timeLimit : 10f;
        container.style.display = DisplayStyle.Flex;
        
        if (ccTimerText != null) ccTimerText.text = $"Time: {timer:F1}s";
        
        for (int i = 0; i < 5; i++)
        {
            ccLineIsFlaky[i] = false;
            ccLineCommented[i] = false;
            ccLineTimers[i] = 0f;
            UpdateCcLineUI(i);
        }

        manager.StartCoroutine(CodeCommenterRoutine());
    }

    public void AbortGame()
    {
        isPlaying = false;
        container.style.display = DisplayStyle.None;
    }

    private void UpdateCcLineUI(int index)
    {
        if (ccLines[index] == null || ccLineTexts[index] == null) return;
        
        if (ccLineCommented[index])
        {
            ccLines[index].style.backgroundColor = new StyleColor(Color.clear);
            ccLineTexts[index].text = $"// {ccOriginalTexts[index]}";
            ccLineTexts[index].style.color = new StyleColor(ColorUtility.TryParseHtmlString("#8b949e", out Color c1) ? c1 : Color.gray);
            ccLines[index].style.translate = new Translate(0, 0, 0);
        }
        else if (ccLineIsFlaky[index])
        {
            ccLines[index].style.backgroundColor = new StyleColor(ColorUtility.TryParseHtmlString("#e74c3c", out Color c2) ? c2 : Color.red);
            ccLineTexts[index].text = ccOriginalTexts[index];
            ccLineTexts[index].style.color = new StyleColor(Color.white);
        }
        else
        {
            ccLines[index].style.backgroundColor = new StyleColor(Color.clear);
            ccLineTexts[index].text = ccOriginalTexts[index];
            ccLineTexts[index].style.color = new StyleColor(ColorUtility.TryParseHtmlString("#c9d1d9", out Color c3) ? c3 : Color.white);
            ccLines[index].style.translate = new Translate(0, 0, 0);
        }
    }

    private void OnCcLineClicked(int index)
    {
        if (!isPlaying || !manager.isPlaying) return;
        if (ccLineIsFlaky[index] && !ccLineCommented[index])
        {
            ccLineIsFlaky[index] = false;
            ccLineCommented[index] = true;
            UpdateCcLineUI(index);
        }
        else if (!ccLineIsFlaky[index] && !ccLineCommented[index])
        {
            if (manager.currentGame != null && manager.currentGame.canFail)
            {
                if (ccTimerText != null) ccTimerText.text = "BUILD FAILED!";
                isPlaying = false;
                manager.AbortGame();
            }
        }
    }

    private IEnumerator CodeCommenterRoutine()
    {
        float nextFlakeTimer = Random.Range(1.0f, 2.0f);

        while (isPlaying && manager.isPlaying)
        {
            timer -= Time.deltaTime;
            if (ccTimerText != null) ccTimerText.text = $"Time: {timer:F1}s";

            nextFlakeTimer -= Time.deltaTime;
            if (nextFlakeTimer <= 0f)
            {
                List<int> available = new List<int>();
                for(int i=0; i<5; i++) if (!ccLineIsFlaky[i] && !ccLineCommented[i]) available.Add(i);
                
                if (available.Count > 0)
                {
                    int flakeIdx = available[Random.Range(0, available.Count)];
                    ccLineIsFlaky[flakeIdx] = true;
                    ccLineTimers[flakeIdx] = 1.5f; 
                    UpdateCcLineUI(flakeIdx);
                }
                
                nextFlakeTimer = Random.Range(0.8f, 1.8f);
            }

            bool failed = false;
            for (int i = 0; i < 5; i++)
            {
                if (ccLineIsFlaky[i] && !ccLineCommented[i])
                {
                    ccLineTimers[i] -= Time.deltaTime;
                    if (ccLines[i] != null)
                    {
                        ccLines[i].style.translate = new Translate(Random.Range(-5f, 5f), Random.Range(-5f, 5f), 0);
                    }

                    if (ccLineTimers[i] <= 0f)
                    {
                        failed = true;
                    }
                }
            }

            if (failed && manager.currentGame != null && manager.currentGame.canFail)
            {
                if (ccTimerText != null) ccTimerText.text = "BUILD FAILED!";
                isPlaying = false;
                manager.AbortGame();
                yield break;
            }

            if (timer <= 0f)
            {
                if (ccTimerText != null) ccTimerText.text = "SUITE PASSED!";
                for(int i=0; i<5; i++) if (ccLines[i] != null) ccLines[i].style.translate = new Translate(0, 0, 0);
                isPlaying = false;
                manager.MarkWinReady();
                yield break;
            }

            yield return null;
        }
    }
}

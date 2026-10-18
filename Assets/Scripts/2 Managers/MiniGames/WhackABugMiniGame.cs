using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;

public class WhackABugMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement rootElement;

    // Extracted from MiniGameManager_Variables.cs
    private VisualElement wbContainer, wbGrid;
    private List<Button> wbBugs = new List<Button>();
    private float[] wbBugTimers;
    private bool[] wbBugIsRed;
    private int wbScore = 0;

    private float timer;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        this.rootElement = root;
        
        wbContainer = root.Q<VisualElement>("WhackABugContainer");
    }

    public void StartGame()
    {
        wbContainer.style.display = DisplayStyle.Flex;
        
        if (wbContainer == null) return;
        wbContainer.Clear();
        wbBugs.Clear();
        wbScore = 0;

        Label header = new Label("PASSING: 9/9   Fixes: 0/15   Time: 20.0");
        header.name = "HeaderLabel";
        header.style.fontSize = 24;
        header.style.color = Color.white;
        header.style.unityTextAlign = TextAnchor.MiddleCenter;
        header.style.marginBottom = 20;
        wbContainer.Add(header);

        wbGrid = new VisualElement();
        wbGrid.style.flexDirection = FlexDirection.Row;
        wbGrid.style.flexWrap = Wrap.Wrap;
        wbGrid.style.justifyContent = Justify.Center;
        wbGrid.style.width = 360;
        wbGrid.style.alignSelf = Align.Center;
        wbContainer.Add(wbGrid);

        string[] modules = { "Login", "Cart", "Profile", "API", "Auth", "DB", "Search", "Payment", "Settings" };
        wbBugIsRed = new bool[9];
        wbBugTimers = new float[9];

        for (int i = 0; i < 9; i++)
        {
            int index = i;
            Button btn = new Button();
            btn.text = modules[i] + "\n✅ PASS";
            btn.style.fontSize = 18;
            btn.style.width = 100;
            btn.style.height = 100;
            btn.style.marginTop = 10; btn.style.marginBottom = 10; btn.style.marginLeft = 10; btn.style.marginRight = 10;
            btn.style.backgroundColor = Color.green;
            btn.style.color = Color.black;
            
            btn.clicked += () => {
                if (!manager.isPlaying) return;
                if (!wbBugIsRed[index])
                {
                    MiniGameFeedback.PlayTap(btn);
                    return;
                }
                wbBugIsRed[index] = false;
                btn.text = modules[index] + "\n✅ PASS";
                btn.style.backgroundColor = Color.green;
                wbScore++;
                
                MiniGameFeedback.PlaySuccess(btn, "+1 FIX!", wbContainer);
                
                if (wbScore >= 15)
                {
                    AudioManager.Instance?.PlayVictoryFanfare();
                    manager.EndGame(true);
                }
            };
            
            wbBugs.Add(btn);
            wbGrid.Add(btn);
        }

        timer = config.timeLimit > 0 ? config.timeLimit : 20f;
        manager.StartCoroutine(WhackABugRoutine());
    }

    public void AbortGame()
    {
        if (wbContainer != null)
        {
            wbContainer.style.display = DisplayStyle.None;
        }
    }

    private IEnumerator WhackABugRoutine()
    {
        Label header = wbContainer.Q<Label>("HeaderLabel");
        string[] modules = { "Login", "Cart", "Profile", "API", "Auth", "DB", "Search", "Payment", "Settings" };
        
        while (manager.isPlaying)
        {
            timer -= Time.deltaTime;
            
            int redCount = 0;
            for (int i = 0; i < 9; i++)
            {
                if (wbBugIsRed[i])
                {
                    redCount++;
                    float pulse = (Mathf.Sin(Time.time * 15f) + 1f) * 0.5f;
                    wbBugs[i].style.backgroundColor = Color.Lerp(Color.red, new Color(0.5f, 0f, 0f), pulse);
                }
                else
                {
                    wbBugTimers[i] -= Time.deltaTime;
                    if (wbBugTimers[i] <= 0f)
                    {
                        if (UnityEngine.Random.value < 0.3f * Time.deltaTime)
                        {
                            wbBugIsRed[i] = true;
                            wbBugs[i].text = modules[i] + "\n❌ FAIL";
                            wbBugs[i].style.backgroundColor = Color.red;
                        }
                    }
                }
            }
            
            if (header != null)
                header.text = $"PASSING: {9 - redCount}/9   Fixes: {wbScore}/15   Time: {timer:0.0}";

            if (redCount >= 4)
            {
                AudioManager.Instance?.PlayMinigameFail();
                manager.EndGame(false);
                yield break;
            }

            if (timer <= 0f)
            {
                bool won = wbScore >= 15;
                if (won) AudioManager.Instance?.PlayVictoryFanfare();
                else AudioManager.Instance?.PlayMinigameFail();
                manager.EndGame(won);
                yield break;
            }
            
            yield return null;
        }
    }
}

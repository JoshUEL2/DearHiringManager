using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;

public class IpHackMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement container;

    private VisualElement gridContainer;
    private Label timerText;
    private Label[] targetLabels = new Label[4];
    
    private int currentProgress = 0;
    private float shuffleTimer = 5f;
    private float timer = 0f;
    
    private List<int> targets = new List<int>();
    private List<Button> gridButtons = new List<Button>();

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;

        container = root.Q<VisualElement>("IpHackContainer");
        gridContainer = root.Q<VisualElement>("IpHackGridContainer");
        timerText = root.Q<Label>("IpHackTimerText");
        
        if (container != null)
        {
            for (int i = 0; i < 4; i++)
            {
                targetLabels[i] = container.Q<Label>($"Target{i}");
            }
        }
        
        // Initialize Grid Buttons just once to reuse them
        if (gridContainer != null)
        {
            gridContainer.Clear();
            gridButtons.Clear();

            for (int i = 0; i < 50; i++)
            {
                Button btn = new Button();
                btn.style.width = new Length(9, LengthUnit.Percent); 
                btn.style.height = 45;
                btn.style.marginTop = 2;
                btn.style.marginBottom = 2;
                btn.style.marginLeft = 2;
                btn.style.marginRight = 2;
                btn.style.fontSize = 28;
                btn.style.backgroundColor = new StyleColor(Color.clear);
                btn.style.color = new StyleColor(new Color(0.8f, 0.8f, 0.8f));
                btn.style.unityFontStyleAndWeight = FontStyle.Bold;
                btn.style.borderTopWidth = 0;
                btn.style.borderBottomWidth = 0;
                btn.style.borderLeftWidth = 0;
                btn.style.borderRightWidth = 0;

                btn.clicked += () => OnNumberClicked(btn);
                
                gridButtons.Add(btn);
                gridContainer.Add(btn);
            }
        }
    }

    public void StartGame()
    {
        if (container != null) container.style.display = DisplayStyle.Flex;
        
        timer = config.timeLimit > 0 ? config.timeLimit : 30f;
        currentProgress = 0;
        shuffleTimer = 5f;

        targets.Clear();
        while (targets.Count < 4)
        {
            int rnd = Random.Range(10, 100);
            if (!targets.Contains(rnd))
            {
                targets.Add(rnd);
            }
        }

        for (int i = 0; i < 4; i++)
        {
            if (targetLabels[i] != null)
            {
                targetLabels[i].text = targets[i].ToString("D2");
                targetLabels[i].style.color = new StyleColor(new Color(0.93f, 0.26f, 0.26f)); // Red
            }
        }

        ShuffleGrid();
        manager.StartCoroutine(GameRoutine());
    }

    public void AbortGame()
    {
        if (container != null) container.style.display = DisplayStyle.None;
    }

    private void ShuffleGrid()
    {
        if (gridButtons.Count == 0) return;

        List<int> availableNumbers = new List<int>();

        for (int i = currentProgress; i < 4; i++)
        {
            availableNumbers.Add(targets[i]);
        }

        while (availableNumbers.Count < 50)
        {
            int rnd = Random.Range(10, 100);
            bool isRemainingTarget = false;
            for (int i = currentProgress; i < 4; i++)
            {
                if (rnd == targets[i])
                {
                    isRemainingTarget = true;
                    break;
                }
            }

            if (!isRemainingTarget)
            {
                availableNumbers.Add(rnd);
            }
        }

        for (int i = 0; i < availableNumbers.Count; i++)
        {
            int r = Random.Range(i, availableNumbers.Count);
            int temp = availableNumbers[i];
            availableNumbers[i] = availableNumbers[r];
            availableNumbers[r] = temp;
        }

        for (int i = 0; i < 50; i++)
        {
            Button btn = gridButtons[i];
            btn.text = availableNumbers[i].ToString("D2");
            btn.userData = availableNumbers[i];
            btn.enabledSelf = true;
            btn.style.backgroundColor = new StyleColor(Color.clear);
            btn.style.color = new StyleColor(new Color(0.8f, 0.8f, 0.8f));
        }
    }

    private void OnNumberClicked(Button btn)
    {
        if (!manager.isPlaying || btn.userData == null) return;

        int val = (int)btn.userData;

        if (val == targets[currentProgress])
        {
            AudioManager.Instance?.PlayIPHackFlurry();
            if (targetLabels[currentProgress] != null)
            {
                targetLabels[currentProgress].style.color = new StyleColor(new Color(0.06f, 0.72f, 0.5f)); // Green
            }
            
            currentProgress++;
            btn.style.color = new StyleColor(new Color(0.06f, 0.72f, 0.5f)); // Green text temporarily
            btn.enabledSelf = false;

            if (currentProgress >= 4)
            {
                manager.MarkWinReady();
            }
        }
        else
        {
            timer -= 5f;
            AudioManager.Instance?.PlayBuzzer();
            btn.style.backgroundColor = new StyleColor(new Color(0.93f, 0.26f, 0.26f, 0.4f)); // Red background flash
            btn.style.color = new StyleColor(Color.white);
            btn.enabledSelf = false;
        }
    }

    private IEnumerator GameRoutine()
    {
        while (manager.isPlaying)
        {
            timer -= Time.deltaTime;
            shuffleTimer -= Time.deltaTime;

            if (timerText != null)
            {
                int mins = Mathf.FloorToInt(Mathf.Max(0, timer) / 60F);
                int secs = Mathf.FloorToInt(Mathf.Max(0, timer) - mins * 60);
                int ms = Mathf.FloorToInt((Mathf.Max(0, timer) * 1000) % 1000);
                timerText.text = string.Format("{0:00}:{1:00}:{2:000}", mins, secs, ms);
            }

            if (timer <= 0)
            {
                manager.AbortGame();
                yield break;
            }
            else if (shuffleTimer <= 0)
            {
                shuffleTimer = 5f;
                ShuffleGrid();
            }

            yield return null;
        }
    }
}

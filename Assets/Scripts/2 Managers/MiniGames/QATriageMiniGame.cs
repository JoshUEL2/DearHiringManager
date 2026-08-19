using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;

public class QATriageMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private VisualElement container;
    private MiniGameConfig currentConfig;

    private Label titleText;
    private Label scoreText;
    private Label timeText;
    private VisualElement paper;
    private Label bugLogText;
    private Label sevInk;
    private Label catInk;

    private Button[] sevBtns = new Button[3];
    private Button[] catBtns = new Button[3];
    private Button submitBtn;

    private int currentLogIndex = 0;
    private int score = 0;
    private float timeRemaining = 20f;
    private int targetScore = 5;

    // State
    private int selectedSev = -1; // 0=Minor, 1=Major, 2=Critical
    private int selectedCat = -1; // 0=UI, 1=Network, 2=Logic
    private bool isAnimating = false;

    // Simple truth mapping for logs
    // Index 0: "NullReferenceException in Login()" -> Logic(2), Critical(2)
    // Index 1: "Timeout 504 on API" -> Network(1), Major(1) or Critical(2) - let's say Major
    // Index 2: "Button overlaps text" -> UI(0), Minor(0)
    // Index 3: "Infinite loop on checkout" -> Logic(2), Critical(2)
    // Index 4: "Database connection lost" -> Network(1), Critical(2)
    private struct BugTruth {
        public int cat;
        public int sev;
    }
    
    // We will just dynamically assign truth based on keywords if we want it robust, 
    // or hardcode it since the array is fixed in MiniGameManager.cs.
    private Dictionary<string, BugTruth> truthMap = new Dictionary<string, BugTruth>()
    {
        { "NullReferenceException in Login()", new BugTruth { cat=2, sev=2 } },
        { "Timeout 504 on API", new BugTruth { cat=1, sev=1 } },
        { "Button overlaps text", new BugTruth { cat=0, sev=0 } },
        { "Infinite loop on checkout", new BugTruth { cat=2, sev=2 } },
        { "Database connection lost", new BugTruth { cat=1, sev=2 } }
    };

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        currentConfig = config;
        container = root.Q<VisualElement>("QATriageContainer");

        titleText = container.Q<Label>("QaTitle");
        scoreText = container.Q<Label>("QaScoreText");
        timeText = container.Q<Label>("QaTimeText");
        
        paper = container.Q<VisualElement>("QaPaper");
        bugLogText = container.Q<Label>("QaBugLog");
        sevInk = container.Q<Label>("QaSevInk");
        catInk = container.Q<Label>("QaCatInk");

        for (int i = 0; i < 3; i++)
        {
            int idx = i;
            sevBtns[i] = container.Q<Button>($"QaBtnSev{i}");
            if (sevBtns[i] != null) sevBtns[i].clicked += () => OnSevClicked(idx);
            
            catBtns[i] = container.Q<Button>($"QaBtnCat{i}");
            if (catBtns[i] != null) catBtns[i].clicked += () => OnCatClicked(idx);
        }

        submitBtn = container.Q<Button>("QaBtnSubmit");
        if (submitBtn != null) submitBtn.clicked += OnSubmitClicked;
    }

    public void StartGame()
    {
        score = 0;
        timeRemaining = currentConfig.timeLimit > 0 ? currentConfig.timeLimit : 15f;
        targetScore = currentConfig.targetScore > 0 ? currentConfig.targetScore : 5;
        currentLogIndex = 0;
        isAnimating = false;

        UpdateScoreText();
        LoadNextBug(immediate: true);

        container.style.display = DisplayStyle.Flex;
        manager.StartCoroutine(GameRoutine());
    }

    public void AbortGame()
    {
        container.style.display = DisplayStyle.None;
    }

    private void OnSevClicked(int index)
    {
        if (isAnimating || !manager.isPlaying) return;
        selectedSev = index;
        ApplyInk(sevInk, currentConfig.qaSeverities[index], Random.Range(-10f, 10f));
    }

    private void OnCatClicked(int index)
    {
        if (isAnimating || !manager.isPlaying) return;
        selectedCat = index;
        ApplyInk(catInk, currentConfig.qaCategories[index], Random.Range(-10f, 10f));
    }

    private void ApplyInk(Label inkLabel, string text, float rotation)
    {
        inkLabel.text = text;
        inkLabel.style.rotate = new StyleRotate(new Rotate(new Angle(rotation, AngleUnit.Degree)));
        inkLabel.RemoveFromClassList("qa-ink-visible");
        
        // Small delay to re-trigger transition
        manager.StartCoroutine(DelayAddVisible(inkLabel));
    }

    private IEnumerator DelayAddVisible(Label inkLabel)
    {
        yield return null;
        inkLabel.AddToClassList("qa-ink-visible");
    }

    private void OnSubmitClicked()
    {
        if (isAnimating || !manager.isPlaying) return;
        
        // Check if both stamps applied
        if (selectedSev == -1 || selectedCat == -1) return;

        // Check correctness
        string log = bugLogText.text;
        bool correct = true;
        if (truthMap.TryGetValue(log, out BugTruth truth))
        {
            if (truth.sev != selectedSev || truth.cat != selectedCat)
            {
                correct = false;
            }
        }
        else
        {
            // Fallback: if not in map, just accept it for gameplay sake
            correct = true;
        }

        if (correct)
        {
            score++;
            UpdateScoreText();
            if (score >= targetScore)
            {
                manager.MarkWinReady();
                return;
            }
        }
        else
        {
            // Penalize time?
            timeRemaining -= 2f;
            // Maybe apply a big REJECTED stamp visually? For now, we just proceed.
        }

        manager.StartCoroutine(SlidePaperRoutine());
    }

    private IEnumerator SlidePaperRoutine()
    {
        isAnimating = true;
        
        // Slide out
        paper.RemoveFromClassList("qa-paper-slide-in");
        paper.AddToClassList("qa-paper-slide-out");
        
        yield return new WaitForSeconds(0.3f);
        
        // Reset state
        paper.RemoveFromClassList("qa-paper-slide-out");
        paper.AddToClassList("qa-paper-slide-hidden");
        
        LoadNextBug(immediate: false);
        
        yield return null; // Wait 1 frame for hidden class to apply without transition
        
        // Slide in
        paper.RemoveFromClassList("qa-paper-slide-hidden");
        paper.AddToClassList("qa-paper-slide-in");
        
        yield return new WaitForSeconds(0.3f);
        
        isAnimating = false;
    }

    private void LoadNextBug(bool immediate)
    {
        selectedSev = -1;
        selectedCat = -1;
        sevInk.RemoveFromClassList("qa-ink-visible");
        catInk.RemoveFromClassList("qa-ink-visible");

        if (currentConfig.qaBugLogs != null && currentConfig.qaBugLogs.Length > 0)
        {
            currentLogIndex = (currentLogIndex + 1) % currentConfig.qaBugLogs.Length;
            bugLogText.text = currentConfig.qaBugLogs[currentLogIndex];
        }
        else
        {
            bugLogText.text = "Error: Something broke!";
        }

        if (immediate)
        {
            paper.RemoveFromClassList("qa-paper-slide-out");
            paper.RemoveFromClassList("qa-paper-slide-hidden");
            paper.AddToClassList("qa-paper-slide-in");
        }
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = $"FILED: {score} / {targetScore}";
        }
    }

    private IEnumerator GameRoutine()
    {
        while (manager.isPlaying)
        {
            timeRemaining -= Time.deltaTime;
            if (timeText != null)
            {
                timeText.text = $"TIME: {Mathf.Max(0, timeRemaining):F1}s";
            }

            if (timeRemaining <= 0)
            {
                manager.AbortGame();
                yield break;
            }
            yield return null;
        }
    }
}

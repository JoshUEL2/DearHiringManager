using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;

public class GitMergeMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig currentConfig;
    private VisualElement container;

    private Label scoreText;
    private Label timeText;
    private Label objectiveText;
    private Label currentCodeText;
    private Label incomingCodeText;
    
    private VisualElement conflictArea;
    private Button currentBtn;
    private Button incomingBtn;

    private int score = 0;
    private float timeRemaining = 15f;
    private int targetScore = 5;
    private bool isAnimating = false;

    private struct ConflictData
    {
        public string objective;
        public string correctCode;
        public string wrongCode;
    }

    private List<ConflictData> conflicts = new List<ConflictData>()
    {
        new ConflictData { objective = "Implement Dark Mode", correctCode = "theme = 'dark';\nrender();", wrongCode = "theme = 'light';\nrender();" },
        new ConflictData { objective = "Fix Null Crash", correctCode = "if (user != null)\n  user.Login();", wrongCode = "user.Login(); // Yolo" },
        new ConflictData { objective = "Optimize Loop", correctCode = "for(int i=0; i<len; i++)", wrongCode = "while(true) { i++; }" },
        new ConflictData { objective = "Enable Multiplayer", correctCode = "Network.Connect();", wrongCode = "LocalSolo.Start();" },
        new ConflictData { objective = "Remove Debug Logs", correctCode = "// Cleaned up", wrongCode = "print('HERE 1');\nprint('TEST');" },
        new ConflictData { objective = "Add Health UI", correctCode = "ui.ShowHealthBar(true);", wrongCode = "ui.HideAll();" },
        new ConflictData { objective = "Fix Typo in Welcome", correctCode = "Welcome to the game!", wrongCode = "Wlecome ot the gmae!" }
    };

    private bool correctIsCurrent = false;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        currentConfig = config;
        container = root.Q<VisualElement>("GitMergeContainer");

        scoreText = container.Q<Label>("GmScoreText");
        timeText = container.Q<Label>("GmTimeText");
        objectiveText = container.Q<Label>("GmObjectiveText");
        currentCodeText = container.Q<Label>("GmCurrentCodeText");
        incomingCodeText = container.Q<Label>("GmIncomingCodeText");
        
        conflictArea = container.Q<VisualElement>("GmConflictArea");
        currentBtn = container.Q<Button>("GmCurrentBtn");
        incomingBtn = container.Q<Button>("GmIncomingBtn");

        currentBtn.clicked += () => OnBlockTapped(isCurrent: true);
        incomingBtn.clicked += () => OnBlockTapped(isCurrent: false);
    }

    public void StartGame()
    {
        score = 0;
        timeRemaining = currentConfig.timeLimit > 0 ? currentConfig.timeLimit : 15f;
        targetScore = currentConfig.targetScore > 0 ? currentConfig.targetScore : 5;
        isAnimating = false;

        UpdateScoreText();
        LoadNextConflict(immediate: true);

        container.style.display = DisplayStyle.Flex;
        manager.StartCoroutine(GameRoutine());
    }

    public void AbortGame()
    {
        container.style.display = DisplayStyle.None;
    }

    private void LoadNextConflict(bool immediate)
    {
        var conflict = conflicts[Random.Range(0, conflicts.Count)];
        objectiveText.text = conflict.objective;

        correctIsCurrent = Random.value > 0.5f;

        if (correctIsCurrent)
        {
            currentCodeText.text = conflict.correctCode;
            incomingCodeText.text = conflict.wrongCode;
        }
        else
        {
            currentCodeText.text = conflict.wrongCode;
            incomingCodeText.text = conflict.correctCode;
        }

        // Reset animations
        conflictArea.RemoveFromClassList("gm-merge-anim");
        currentBtn.RemoveFromClassList("gm-block-merged");
        incomingBtn.RemoveFromClassList("gm-block-merged");
        
        currentBtn.style.display = DisplayStyle.Flex;
        incomingBtn.style.display = DisplayStyle.Flex;
        
        if (immediate)
        {
            conflictArea.RemoveFromClassList("gm-slide-in");
            manager.StartCoroutine(DelaySlideIn());
        }
        else
        {
            conflictArea.AddToClassList("gm-slide-in");
        }
    }

    private IEnumerator DelaySlideIn()
    {
        yield return null;
        conflictArea.AddToClassList("gm-slide-in");
    }

    private void OnBlockTapped(bool isCurrent)
    {
        if (isAnimating || !manager.isPlaying) return;

        bool correct = (isCurrent == correctIsCurrent);

        if (correct)
        {
            score++;
            UpdateScoreText();
            manager.StartCoroutine(MergeAnimation(isCurrent));
        }
        else
        {
            timeRemaining -= 2f;
            // Visual error shake could be added here
            Button wrongBtn = isCurrent ? currentBtn : incomingBtn;
            manager.StartCoroutine(ErrorShake(wrongBtn));
        }
    }

    private WaitForSeconds _wait03f = new WaitForSeconds(0.3f);
    private WaitForSeconds _wait02f = new WaitForSeconds(0.2f);

    private IEnumerator ErrorShake(Button btn)
    {
        isAnimating = true;
        btn.AddToClassList("gm-error-shake");
        yield return _wait03f;
        btn.RemoveFromClassList("gm-error-shake");
        isAnimating = false;
    }

    private IEnumerator MergeAnimation(bool isCurrentAccepted)
    {
        isAnimating = true;

        Button accepted = isCurrentAccepted ? currentBtn : incomingBtn;
        Button rejected = isCurrentAccepted ? incomingBtn : currentBtn;

        rejected.style.display = DisplayStyle.None;
        accepted.AddToClassList("gm-block-merged");

        yield return _wait02f;
        
        conflictArea.RemoveFromClassList("gm-slide-in");
        
        yield return _wait02f;

        if (score >= targetScore)
        {
            manager.MarkWinReady();
        }
        else
        {
            LoadNextConflict(immediate: false);
            isAnimating = false;
        }
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = $"RESOLVED: {score} / {targetScore}";
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

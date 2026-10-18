using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;

public class QATriageMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private VisualElement container;
    private MiniGameConfig currentConfig;

    private Label scoreText;
    private Label timeText;
    
    private VisualElement[] folders = new VisualElement[3];
    private VisualElement paper;
    private Label bugLogText;
    
    private int currentLogIndex = 0;
    private int score = 0;
    private float timeRemaining = 20f;
    private int targetScore = 5;

    private bool isDragging = false;
    private Vector2 paperStartPos;
    private Vector2 dragStartPointerPos;

    // Truth mapping: index maps to category folder (0=UI, 1=Network, 2=Logic)
    private Dictionary<string, int> truthMap = new Dictionary<string, int>()
    {
        { "NullReferenceException in Login()", 2 }, // Logic
        { "Timeout 504 on API", 1 }, // Network
        { "Button overlaps text", 0 }, // UI
        { "Infinite loop on checkout", 2 }, // Logic
        { "Database connection lost", 1 } // Network
    };

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        currentConfig = config;
        container = root.Q<VisualElement>("QATriageContainer");

        scoreText = container.Q<Label>("QaScoreText");
        timeText = container.Q<Label>("QaTimeText");
        
        folders[0] = container.Q<VisualElement>("QaFolder0");
        folders[1] = container.Q<VisualElement>("QaFolder1");
        folders[2] = container.Q<VisualElement>("QaFolder2");

        paper = container.Q<VisualElement>("QaPaper");
        bugLogText = container.Q<Label>("QaBugLog");

        paper.RegisterCallback<PointerDownEvent>(OnPointerDown);
        paper.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        paper.RegisterCallback<PointerUpEvent>(OnPointerUp);
        paper.RegisterCallback<PointerCaptureOutEvent>(OnPointerCaptureOut);
    }

    public void StartGame()
    {
        score = 0;
        timeRemaining = currentConfig.timeLimit > 0 ? currentConfig.timeLimit : 15f;
        targetScore = currentConfig.targetScore > 0 ? currentConfig.targetScore : 5;
        currentLogIndex = 0;
        isDragging = false;
        
        currentTranslate = Vector2.zero;
        paper.style.translate = new StyleTranslate(new Translate(0, 0, 0));
        paper.RemoveFromClassList("qa-paper-hovering-folder");

        UpdateScoreText();
        LoadNextBug();

        container.style.display = DisplayStyle.Flex;
        manager.StartCoroutine(GameRoutine());
    }

    public void AbortGame()
    {
        container.style.display = DisplayStyle.None;
    }

    private Vector2 currentTranslate = Vector2.zero;
    
    private Vector2 lastPointerPos;
    
    private void OnPointerDown(PointerDownEvent e)
    {
        if (!manager.isPlaying) return;
        isDragging = true;
        paper.CapturePointer(e.pointerId);
        dragStartPointerPos = e.position;
        lastPointerPos = e.position;
        paperStartPos = currentTranslate;
        
        e.StopPropagation();
    }

    private void OnPointerMove(PointerMoveEvent e)
    {
        if (!isDragging || !manager.isPlaying) return;
        
        lastPointerPos = e.position;
        Vector2 delta = (Vector2)e.position - dragStartPointerPos;
        currentTranslate = paperStartPos + delta;
        
        paper.style.translate = new StyleTranslate(new Translate(currentTranslate.x, currentTranslate.y, 0));
        
        // Highlight folders if hovering
        bool isHoveringFolder = false;
        for (int i = 0; i < 3; i++)
        {
            if (folders[i].worldBound.Contains(e.position))
            {
                folders[i].AddToClassList("qa-folder-hover");
                isHoveringFolder = true;
            }
            else
            {
                folders[i].RemoveFromClassList("qa-folder-hover");
            }
        }
        
        if (isHoveringFolder)
        {
            paper.AddToClassList("qa-paper-hovering-folder");
        }
        else
        {
            paper.RemoveFromClassList("qa-paper-hovering-folder");
        }
        
        e.StopPropagation();
    }

    private void OnPointerUp(PointerUpEvent e)
    {
        if (!isDragging) return;
        isDragging = false;
        if (paper.HasPointerCapture(e.pointerId))
        {
            paper.ReleasePointer(e.pointerId);
        }
        
        CheckDrop(lastPointerPos);
        e.StopPropagation();
    }

    private void OnPointerCaptureOut(PointerCaptureOutEvent e)
    {
        if (!isDragging) return;
        isDragging = false;
        CheckDrop(lastPointerPos);
    }

    private void CheckDrop(Vector2 pointerPos)
    {
        int droppedFolderIndex = -1;
        
        paper.RemoveFromClassList("qa-paper-hovering-folder");
        
        for (int i = 0; i < 3; i++)
        {
            folders[i].RemoveFromClassList("qa-folder-hover");
            if (folders[i].worldBound.Contains(pointerPos))
            {
                droppedFolderIndex = i;
            }
        }
        
        if (droppedFolderIndex != -1)
        {
            string log = bugLogText.text;
            int correctFolder = -1;
            if (truthMap.TryGetValue(log, out int folder))
            {
                correctFolder = folder;
            }
            else
            {
                correctFolder = 0; // Fallback
            }
            
            if (droppedFolderIndex == correctFolder)
            {
                score++;
                UpdateScoreText();
                MiniGameFeedback.PlaySuccess(folders[droppedFolderIndex], "TRIAGED!", container);
                if (score >= targetScore)
                {
                    manager.MarkWinReady();
                    return;
                }
                LoadNextBug();
            }
            else
            {
                // Wrong folder, penalize time slightly
                timeRemaining = Mathf.Max(0, timeRemaining - 2f);
                MiniGameFeedback.PlayError(folders[droppedFolderIndex], "WRONG QUEUE! -2s", container);
            }
        }
        
        // Reset position
        currentTranslate = Vector2.zero;
        paper.style.translate = new StyleTranslate(new Translate(0, 0, 0));
    }

    private void LoadNextBug()
    {
        if (currentConfig.qaBugLogs != null && currentConfig.qaBugLogs.Length > 0)
        {
            currentLogIndex = (currentLogIndex + 1) % currentConfig.qaBugLogs.Length;
            bugLogText.text = currentConfig.qaBugLogs[currentLogIndex];
        }
        else
        {
            bugLogText.text = "Error: Something broke!";
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

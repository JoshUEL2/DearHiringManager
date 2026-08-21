using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class FileSorterMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement container;

    private Label fileSorterTimerText;
    private Label fileSorterProgressText;
    private VisualElement fileSorterActiveFile;
    private Label fileSorterActiveFileText;
    
    private Vector2 fileSorterPointerStart;
    private bool isDraggingFileSorter = false;
    private int fileSorterSortedCount = 0;
    private string fileSorterTargetDirection = "";

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        
        container = root.Q<VisualElement>("FileSorterContainer");
        fileSorterTimerText = container.Q<Label>("FileSorterTimerText");
        fileSorterProgressText = container.Q<Label>("FileSorterProgressText");
        fileSorterActiveFile = container.Q<VisualElement>("FileSorterActiveFile");
        fileSorterActiveFileText = container.Q<Label>("FileSorterActiveFileText");
        
        if (fileSorterActiveFile != null)
        {
            fileSorterActiveFile.RegisterCallback<PointerDownEvent>(OnFileSorterDown);
            fileSorterActiveFile.RegisterCallback<PointerMoveEvent>(OnFileSorterMove);
            fileSorterActiveFile.RegisterCallback<PointerUpEvent>(OnFileSorterUp);
        }
    }

    public void StartGame()
    {
        container.style.display = DisplayStyle.Flex;
        
        fileSorterSortedCount = 0;
        if (fileSorterActiveFile != null) fileSorterActiveFile.style.display = DisplayStyle.Flex;
        if (fileSorterProgressText != null) fileSorterProgressText.text = $"0 / {config.fileSorterTarget}";
        if (fileSorterTimerText != null) fileSorterTimerText.text = $"{config.timeLimit:F1}s";
        
        NextFileSorterItem();
        
        manager.StartCoroutine(FileSorterRoutine());
    }

    public void AbortGame()
    {
        container.style.display = DisplayStyle.None;
    }
    
    private void NextFileSorterItem()
    {
        if (fileSorterActiveFile != null)
        {
            fileSorterActiveFile.style.translate = new Translate(0, 0, 0);
        }
        
        string fileName = "";
        if (config.fileSorterItems != null && config.fileSorterItems.Length > 0)
        {
            fileName = config.fileSorterItems[Random.Range(0, config.fileSorterItems.Length)];
        }
        else
        {
            fileName = $"Doc_{Random.Range(100, 999)}.txt";
        }
        
        string[] dirs = new string[]{"Up", "Down", "Left", "Right"};
        fileSorterTargetDirection = dirs[Random.Range(0, 4)];
        
        string folderName = "";
        switch (fileSorterTargetDirection)
        {
            case "Up": folderName = "FINANCE"; break;
            case "Down": folderName = "HR"; break;
            case "Left": folderName = "MARKETING"; break;
            case "Right": folderName = "IT"; break;
        }
        
        if (fileSorterActiveFileText != null) 
        {
            fileSorterActiveFileText.text = fileName + $"\n\n\n<b>To: {folderName}</b>";
        }
    }
    
    private void OnFileSorterDown(PointerDownEvent evt)
    {
        if (!manager.isPlaying || isDraggingFileSorter) return;
        isDraggingFileSorter = true;
        fileSorterPointerStart = evt.position;
        fileSorterActiveFile.CapturePointer(evt.pointerId);
    }
    
    private void OnFileSorterMove(PointerMoveEvent evt)
    {
        if (!manager.isPlaying || !isDraggingFileSorter || !fileSorterActiveFile.HasPointerCapture(evt.pointerId)) return;
        
        Vector2 delta = (Vector2)evt.position - fileSorterPointerStart;
        fileSorterActiveFile.style.translate = new Translate(delta.x, delta.y, 0);
    }
    
    private void OnFileSorterUp(PointerUpEvent evt)
    {
        if (!manager.isPlaying || !isDraggingFileSorter || !fileSorterActiveFile.HasPointerCapture(evt.pointerId)) return;
        
        fileSorterActiveFile.ReleasePointer(evt.pointerId);
        isDraggingFileSorter = false;
        
        Vector2 delta = (Vector2)evt.position - fileSorterPointerStart;
        
        if (delta.magnitude < 50f)
        {
            fileSorterActiveFile.style.translate = new Translate(0, 0, 0);
            return;
        }
        
        string swipedDir = "";
        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
        {
            swipedDir = delta.x > 0 ? "Right" : "Left";
        }
        else
        {
            swipedDir = delta.y > 0 ? "Down" : "Up";
        }
        
        if (swipedDir == fileSorterTargetDirection)
        {
            manager.StartCoroutine(FileSorterAnimateOut(swipedDir));
        }
        else
        {
            manager.StartCoroutine(FileSorterShakeError());
        }
    }
    
    private IEnumerator FileSorterAnimateOut(string dir)
    {
        Vector2 currentTrans = new Vector2(fileSorterActiveFile.resolvedStyle.translate.x, fileSorterActiveFile.resolvedStyle.translate.y);
        Vector2 targetTrans = currentTrans;
        
        if (dir == "Up") targetTrans.y -= 500f;
        if (dir == "Down") targetTrans.y += 500f;
        if (dir == "Left") targetTrans.x -= 500f;
        if (dir == "Right") targetTrans.x += 500f;
        
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * 10f;
            fileSorterActiveFile.style.translate = new Translate(Mathf.Lerp(currentTrans.x, targetTrans.x, t), Mathf.Lerp(currentTrans.y, targetTrans.y, t), 0);
            yield return null;
        }
        
        fileSorterSortedCount++;
        if (fileSorterProgressText != null) fileSorterProgressText.text = $"{fileSorterSortedCount} / {config.fileSorterTarget}";
        
        if (fileSorterSortedCount >= config.fileSorterTarget)
        {
            fileSorterActiveFile.style.display = DisplayStyle.None;
            manager.MarkWinReady();
        }
        else
        {
            NextFileSorterItem();
        }
    }
    
    private IEnumerator FileSorterShakeError()
    {
        fileSorterActiveFile.style.backgroundColor = new StyleColor(new Color(0.9f, 0.2f, 0.2f));
        
        float duration = 0.3f;
        float elapsed = 0f;
        float magnitude = 15f;
        
        Vector2 baseTrans = new Vector2(fileSorterActiveFile.resolvedStyle.translate.x, fileSorterActiveFile.resolvedStyle.translate.y);
        
        while (elapsed < duration && manager.isPlaying)
        {
            elapsed += Time.deltaTime;
            float xOffset = Mathf.Sin(elapsed * 40f) * magnitude;
            fileSorterActiveFile.style.translate = new Translate(baseTrans.x + xOffset, baseTrans.y, 0);
            yield return null;
        }
        
        fileSorterActiveFile.style.translate = new Translate(0, 0, 0);
        fileSorterActiveFile.style.backgroundColor = new StyleColor(Color.white);
    }
    
    private IEnumerator FileSorterRoutine()
    {
        float timer = config.timeLimit;
        
        while (manager.isPlaying)
        {
            timer -= Time.deltaTime;
            if (fileSorterTimerText != null) fileSorterTimerText.text = $"{Mathf.Max(0, timer):F1}s";
            
            if (timer <= 0f)
            {
                manager.AbortGame();
                yield break;
            }
            yield return null;
        }
    }
}

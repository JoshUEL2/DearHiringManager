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
    
    private VisualElement folderUp;
    private VisualElement folderDown;
    private VisualElement folderLeft;
    private VisualElement folderRight;
    
    private Vector2 fileSorterPointerStart;
    private Vector2 fileSorterDragStartPos;
    private bool isDraggingFileSorter = false;
    private int fileSorterSortedCount = 0;
    private string fileSorterTargetDirection = "";

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        
        container = root.Q<VisualElement>("FileSorterContainer");
        // Layout fix to stretch across popup window
        container.style.alignSelf = Align.Stretch; 
        
        fileSorterTimerText = container.Q<Label>("FileSorterTimerText");
        fileSorterProgressText = container.Q<Label>("FileSorterProgressText");
        
        fileSorterActiveFile = container.Q<VisualElement>("FileSorterActiveFile");
        fileSorterActiveFileText = container.Q<Label>("FileSorterActiveFileText");
        fileSorterActiveFileText.enableRichText = true; // Make sure rich text works for formatting
        
        // Find folders by looking for their child labels
        folderUp = container.Q<Label>("FileSorterFolderUp").parent;
        folderDown = container.Q<Label>("FileSorterFolderDown").parent;
        folderLeft = container.Q<Label>("FileSorterFolderLeft").parent;
        folderRight = container.Q<Label>("FileSorterFolderRight").parent;
        
        ApplyClassicBevels();
        
        if (fileSorterActiveFile != null)
        {
            fileSorterActiveFile.RegisterCallback<PointerDownEvent>(OnFileSorterDown);
            fileSorterActiveFile.RegisterCallback<PointerMoveEvent>(OnFileSorterMove);
            fileSorterActiveFile.RegisterCallback<PointerUpEvent>(OnFileSorterUp);
        }
    }

    private void ApplyClassicBevels()
    {
        // Apply Win95 Bevels to folders
        VisualElement[] folders = { folderUp, folderDown, folderLeft, folderRight };
        foreach (var f in folders)
        {
            if (f == null) continue;
            f.style.borderTopWidth = 3;
            f.style.borderLeftWidth = 3;
            f.style.borderBottomWidth = 3;
            f.style.borderRightWidth = 3;
            
            f.style.borderTopColor = new StyleColor(new Color(1f, 0.95f, 0.8f)); // Bright highlight
            f.style.borderLeftColor = new StyleColor(new Color(1f, 0.95f, 0.8f));
            f.style.borderBottomColor = new StyleColor(new Color(0.6f, 0.5f, 0.2f)); // Dark shadow
            f.style.borderRightColor = new StyleColor(new Color(0.6f, 0.5f, 0.2f));
        }

        // Apply Win95 Bevels to File
        if (fileSorterActiveFile != null)
        {
            fileSorterActiveFile.style.borderTopWidth = 3;
            fileSorterActiveFile.style.borderLeftWidth = 3;
            fileSorterActiveFile.style.borderBottomWidth = 3;
            fileSorterActiveFile.style.borderRightWidth = 3;
            
            fileSorterActiveFile.style.borderTopColor = new StyleColor(Color.white);
            fileSorterActiveFile.style.borderLeftColor = new StyleColor(Color.white);
            fileSorterActiveFile.style.borderBottomColor = new StyleColor(new Color(0.4f, 0.4f, 0.4f));
            fileSorterActiveFile.style.borderRightColor = new StyleColor(new Color(0.4f, 0.4f, 0.4f));
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
            fileSorterActiveFile.style.scale = new StyleScale(new Vector2(1f, 1f));
            fileSorterActiveFile.style.rotate = new StyleRotate(new Rotate(new Angle(0f, AngleUnit.Degree)));
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
            // Distinct formatting for the target
            fileSorterActiveFileText.text = $"<b>{fileName}</b>\n\n\nTo: <color=#000080><b>{folderName}</b></color>";
        }
    }
    
    private void OnFileSorterDown(PointerDownEvent evt)
    {
        if (!manager.isPlaying || isDraggingFileSorter) return;
        isDraggingFileSorter = true;
        
        fileSorterPointerStart = evt.position;
        fileSorterDragStartPos = new Vector2(
            fileSorterActiveFile.resolvedStyle.translate.x, 
            fileSorterActiveFile.resolvedStyle.translate.y
        );
        
        // Lift Physics
        fileSorterActiveFile.style.scale = new StyleScale(new Vector2(1.15f, 1.15f));
        
        fileSorterActiveFile.CapturePointer(evt.pointerId);
    }
    
    private void OnFileSorterMove(PointerMoveEvent evt)
    {
        if (!manager.isPlaying || !isDraggingFileSorter || !fileSorterActiveFile.HasPointerCapture(evt.pointerId)) return;
        
        Vector2 delta = (Vector2)evt.position - fileSorterPointerStart;
        
        fileSorterActiveFile.style.translate = new Translate(fileSorterDragStartPos.x + delta.x, fileSorterDragStartPos.y + delta.y, 0);
        
        // Dynamic Tilt Physics based on drag velocity
        float tilt = Mathf.Clamp(delta.x * 0.05f, -15f, 15f);
        fileSorterActiveFile.style.rotate = new StyleRotate(new Rotate(new Angle(tilt, AngleUnit.Degree)));
    }
    
    private void OnFileSorterUp(PointerUpEvent evt)
    {
        if (!manager.isPlaying || !isDraggingFileSorter || !fileSorterActiveFile.HasPointerCapture(evt.pointerId)) return;
        
        fileSorterActiveFile.ReleasePointer(evt.pointerId);
        isDraggingFileSorter = false;
        
        Vector2 delta = (Vector2)evt.position - fileSorterPointerStart;
        
        // Reset tilt and scale unless animating out
        fileSorterActiveFile.style.rotate = new StyleRotate(new Rotate(new Angle(0f, AngleUnit.Degree)));
        
        if (delta.magnitude < 80f)
        {
            // Dropped too close to center, snap back
            manager.StartCoroutine(FileSorterRubberBand());
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
    
    private IEnumerator FileSorterRubberBand()
    {
        // Smoothly return to center
        Vector2 currentTrans = new Vector2(fileSorterActiveFile.resolvedStyle.translate.x, fileSorterActiveFile.resolvedStyle.translate.y);
        
        float t = 0;
        while (t < 1f && !isDraggingFileSorter)
        {
            t += Time.deltaTime * 15f; // Fast snap
            fileSorterActiveFile.style.translate = new Translate(Mathf.Lerp(currentTrans.x, 0, t), Mathf.Lerp(currentTrans.y, 0, t), 0);
            fileSorterActiveFile.style.scale = new StyleScale(new Vector2(Mathf.Lerp(1.15f, 1f, t), Mathf.Lerp(1.15f, 1f, t)));
            yield return null;
        }
    }
    
    private IEnumerator FileSorterAnimateOut(string dir)
    {
        Vector2 currentTrans = new Vector2(fileSorterActiveFile.resolvedStyle.translate.x, fileSorterActiveFile.resolvedStyle.translate.y);
        
        // Calculate Target relative to center based on folder positions
        Vector2 targetTrans = Vector2.zero;
        VisualElement targetFolder = null;
        
        if (dir == "Up") { targetTrans = new Vector2(0, -250f); targetFolder = folderUp; }
        if (dir == "Down") { targetTrans = new Vector2(0, 250f); targetFolder = folderDown; }
        if (dir == "Left") { targetTrans = new Vector2(-250f, 0); targetFolder = folderLeft; }
        if (dir == "Right") { targetTrans = new Vector2(250f, 0); targetFolder = folderRight; }
        
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * 12f; // Very fast swallow
            float ease = t * t * t; // Ease in cubic
            
            // Move toward folder and shrink to 0!
            fileSorterActiveFile.style.translate = new Translate(Mathf.Lerp(currentTrans.x, targetTrans.x, ease), Mathf.Lerp(currentTrans.y, targetTrans.y, ease), 0);
            fileSorterActiveFile.style.scale = new StyleScale(new Vector2(Mathf.Lerp(1.15f, 0f, ease), Mathf.Lerp(1.15f, 0f, ease)));
            yield return null;
        }
        
        // Folder catch bounce juice
        if (targetFolder != null)
        {
            manager.StartCoroutine(FolderBounceRoutine(targetFolder));
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
    
    private IEnumerator FolderBounceRoutine(VisualElement folder)
    {
        // Use hardcoded manila color because reading .style.backgroundColor returns transparent if it was set via UXML
        StyleColor originalBg = new StyleColor(new Color(0.902f, 0.808f, 0.494f)); 
        folder.style.backgroundColor = new StyleColor(new Color(1f, 0.9f, 0.2f)); // Flash bright yellow
        
        float duration = 0.2f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float p = elapsed / duration;
            float scale = 1f + (Mathf.Sin(p * Mathf.PI) * 0.15f); // Pop out
            folder.style.scale = new StyleScale(new Vector2(scale, scale));
            yield return null;
        }
        
        folder.style.scale = new StyleScale(new Vector2(1f, 1f));
        folder.style.backgroundColor = originalBg; // Restore manila color
    }
    
    private IEnumerator FileSorterShakeError()
    {
        // Use hardcoded white because reading .style.backgroundColor returns transparent if it was set via UXML
        StyleColor originalBg = new StyleColor(Color.white);
        fileSorterActiveFile.style.backgroundColor = new StyleColor(new Color(0.9f, 0.2f, 0.2f));
        
        float duration = 0.3f;
        float elapsed = 0f;
        float magnitude = 20f;
        
        Vector2 baseTrans = new Vector2(fileSorterActiveFile.resolvedStyle.translate.x, fileSorterActiveFile.resolvedStyle.translate.y);
        
        while (elapsed < duration && manager.isPlaying)
        {
            elapsed += Time.deltaTime;
            float xOffset = Mathf.Sin(elapsed * 40f) * magnitude;
            fileSorterActiveFile.style.translate = new Translate(baseTrans.x + xOffset, baseTrans.y, 0);
            yield return null;
        }
        
        fileSorterActiveFile.style.backgroundColor = originalBg;
        
        // Rubber band back
        manager.StartCoroutine(FileSorterRubberBand());
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

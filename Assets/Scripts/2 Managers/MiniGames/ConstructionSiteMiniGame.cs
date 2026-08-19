using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class ConstructionSiteMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;

    private VisualElement container;
    private Label timeText;
    private Label statusText;
    private VisualElement wall;
    private VisualElement scanner;
    private VisualElement thermalLayer;
    private VisualElement hotspot;
    private VisualElement progressFill;
    private VisualElement radarSweep;

    private float timeRemaining = 15f;
    private float analysisTime = 0f;
    private float requiredAnalysis = 3f;
    private float radarAngle = 0f;

    // Draggable state
    private bool isDragging = false;
    private Vector2 dragStartPos;
    private Vector2 scannerStartPos;
    private Vector2 scannerPos;
    private Vector2 hotspotPos;

    // Dimensions
    private const float WallWidth = 700f;
    private const float WallHeight = 400f;
    private const float ScannerSize = 220f;
    private const float HotspotSize = 120f;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;

        container = root.Q<VisualElement>("ConstructionSiteContainer");
        timeText = container.Q<Label>("CsTimeText");
        statusText = container.Q<Label>("CsStatusText");
        
        wall = container.Q<VisualElement>("CsWall");
        scanner = container.Q<VisualElement>("CsScanner");
        thermalLayer = container.Q<VisualElement>("CsThermalLayer");
        hotspot = container.Q<VisualElement>("CsHotspot");
        progressFill = container.Q<VisualElement>("CsProgressFill");
        radarSweep = container.Q<VisualElement>("CsRadarSweep");

        // Dragging events (register on both so you can click anywhere to scan)
        wall.RegisterCallback<PointerDownEvent>(OnPointerDown);
        wall.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        wall.RegisterCallback<PointerUpEvent>(OnPointerUp);
        wall.RegisterCallback<PointerCaptureOutEvent>(OnPointerCaptureOut);

        scanner.RegisterCallback<PointerDownEvent>(OnPointerDown);
        scanner.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        scanner.RegisterCallback<PointerUpEvent>(OnPointerUp);
        scanner.RegisterCallback<PointerCaptureOutEvent>(OnPointerCaptureOut);
    }

    private void OnPointerDown(PointerDownEvent e)
    {
        isDragging = true;
        
        // Snap scanner to pointer immediately
        Vector2 localPos = wall.WorldToLocal(e.position);
        scannerPos = localPos - new Vector2(ScannerSize / 2f, ScannerSize / 2f);
        scannerPos.x = Mathf.Clamp(scannerPos.x, 0, WallWidth - ScannerSize);
        scannerPos.y = Mathf.Clamp(scannerPos.y, 0, WallHeight - ScannerSize);
        UpdateScannerTransform();

        dragStartPos = (Vector2)e.position;
        scannerStartPos = scannerPos;
        
        var target = e.target as VisualElement;
        if (target != null)
        {
            target.CapturePointer(e.pointerId);
        }
    }

    private void OnPointerMove(PointerMoveEvent e)
    {
        if (!isDragging) return;
        
        Vector2 delta = (Vector2)e.position - dragStartPos;
        scannerPos = scannerStartPos + delta;
        
        // Clamp scanner to wall bounds
        scannerPos.x = Mathf.Clamp(scannerPos.x, 0, WallWidth - ScannerSize);
        scannerPos.y = Mathf.Clamp(scannerPos.y, 0, WallHeight - ScannerSize);

        UpdateScannerTransform();
    }

    private void OnPointerUp(PointerUpEvent e)
    {
        isDragging = false;
        var target = e.target as VisualElement;
        if (target != null && target.HasPointerCapture(e.pointerId))
        {
            target.ReleasePointer(e.pointerId);
        }
    }
    
    private void OnPointerCaptureOut(PointerCaptureOutEvent e)
    {
        isDragging = false;
    }

    private void UpdateScannerTransform()
    {
        // Move scanner
        scanner.style.left = scannerPos.x;
        scanner.style.top = scannerPos.y;

        // Inverse translate thermal layer to keep it stationary relative to the wall
        thermalLayer.style.left = -scannerPos.x;
        thermalLayer.style.top = -scannerPos.y;
    }

    public void StartGame()
    {
        timeRemaining = config.timeLimit > 0 ? config.timeLimit : 15f;
        analysisTime = 0f;
        isDragging = false;
        
        // Randomize Hotspot (ensure it's fully within bounds)
        hotspotPos = new Vector2(
            Random.Range(0f, WallWidth - HotspotSize),
            Random.Range(0f, WallHeight - HotspotSize)
        );
        hotspot.style.left = hotspotPos.x;
        hotspot.style.top = hotspotPos.y;

        // Start Scanner in center
        scannerPos = new Vector2((WallWidth - ScannerSize)/2f, (WallHeight - ScannerSize)/2f);
        UpdateScannerTransform();

        container.style.display = DisplayStyle.Flex;
        progressFill.style.width = new Length(0, LengthUnit.Percent);
        statusText.text = "SEARCHING...";
        statusText.style.color = new StyleColor(new Color(0.22f, 0.74f, 0.97f)); // #38BDF8

        manager.StartCoroutine(GameRoutine());
    }

    public void AbortGame()
    {
        container.style.display = DisplayStyle.None;
    }

    private IEnumerator GameRoutine()
    {
        while (manager.isPlaying)
        {
            float dt = Time.deltaTime;
            timeRemaining -= dt;
            
            if (radarSweep != null)
            {
                radarAngle += dt * 360f; // 1 full rotation per second
                radarSweep.style.rotate = new Rotate(new Angle(radarAngle, AngleUnit.Degree));
            }
            
            // Check Proximity
            Vector2 scannerCenter = scannerPos + new Vector2(ScannerSize/2f, ScannerSize/2f);
            Vector2 hotspotCenter = hotspotPos + new Vector2(HotspotSize/2f, HotspotSize/2f);
            
            float distance = Vector2.Distance(scannerCenter, hotspotCenter);
            float maxScanDistance = 80f; // Must be fairly precise

            if (distance < maxScanDistance)
            {
                analysisTime += dt;
                statusText.text = "ANALYZING...";
                statusText.style.color = new StyleColor(new Color(0.98f, 0.8f, 0.08f)); // Yellow
            }
            else
            {
                // Lose progress if you move away? Let's just pause progress, or slightly decay.
                analysisTime -= dt * 0.5f; 
                if (analysisTime < 0) analysisTime = 0;
                
                statusText.text = "SEARCHING...";
                statusText.style.color = new StyleColor(new Color(0.22f, 0.74f, 0.97f)); // Blue
            }

            // Update UI
            timeText.text = $"TIME: {Mathf.Max(0, timeRemaining):F1}s";
            
            float progressPercent = (analysisTime / requiredAnalysis) * 100f;
            progressFill.style.width = new Length(Mathf.Clamp(progressPercent, 0, 100), LengthUnit.Percent);

            if (analysisTime >= requiredAnalysis)
            {
                statusText.text = "FAULT DETECTED!";
                statusText.style.color = new StyleColor(new Color(0.13f, 0.77f, 0.36f)); // Green
                manager.MarkWinReady();
            }
            else if (timeRemaining <= 0)
            {
                manager.AbortGame(); // Time's up! Fail.
                yield break;
            }

            yield return null;
        }
    }
}

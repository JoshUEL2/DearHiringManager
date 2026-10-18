using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;

public class DragCenterMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement container;

    private Label scoreText;
    private Label timeText;
    private VisualElement workArea;
    private Label draggableIcon;
    private Button confirmBtn;
    private VisualElement targetCrosshair;
    private VisualElement scoreArea;
    private Label scoreValueText;
    private VisualElement scoreBarFill;

    // Dynamically added juice elements
    private Label cssTooltip;
    private VisualElement scanLine;
    private VisualElement flashOverlay;
    
    // Effects tracking
    private float ghostTimer = 0f;
    private List<VisualElement> activeParticles = new List<VisualElement>();

    private int chances = 3;
    private float timeRemaining = 15f;
    private bool isDragging = false;
    private bool isScoring = false;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        container = root.Q<VisualElement>("DragCenterContainer");
        container.style.width = new Length(100f, LengthUnit.Percent);

        scoreText = container.Q<Label>("DcRoundText");
        timeText = container.Q<Label>("DcTimerText");
        workArea = container.Q<VisualElement>("DcWorkArea");
        draggableIcon = container.Q<Label>("DcDraggableIcon");
        confirmBtn = container.Q<Button>("DcBtnConfirm");
        targetCrosshair = container.Q<VisualElement>("DcTargetCrosshair");
        scoreArea = container.Q<VisualElement>("DcScoreArea");
        scoreValueText = container.Q<Label>("DcScoreText");
        scoreBarFill = container.Q<VisualElement>("DcScoreBarFill");

        draggableIcon.RegisterCallback<PointerDownEvent>(OnPointerDown);
        draggableIcon.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        draggableIcon.RegisterCallback<PointerUpEvent>(OnPointerUp);
        draggableIcon.RegisterCallback<PointerCaptureOutEvent>(OnPointerCaptureOut);
        
        confirmBtn.clicked += OnConfirmClicked;

        CreateJuiceElements();
    }

    private void CreateJuiceElements()
    {
        // Darken the work area to make neon effects pop
        workArea.style.backgroundColor = new StyleColor(new Color(0.05f, 0.07f, 0.12f));

        // CSS Tooltip
        cssTooltip = new Label();
        cssTooltip.style.position = Position.Absolute;
        cssTooltip.style.backgroundColor = new StyleColor(new Color(0.1f, 0.1f, 0.15f, 0.9f));
        cssTooltip.style.color = new StyleColor(new Color(0.22f, 0.74f, 0.97f)); // Bright blue
        cssTooltip.style.fontSize = 20;
        cssTooltip.style.paddingLeft = 8;
        cssTooltip.style.paddingRight = 8;
        cssTooltip.style.paddingTop = 4;
        cssTooltip.style.paddingBottom = 4;
        cssTooltip.style.borderBottomLeftRadius = 5;
        cssTooltip.style.borderBottomRightRadius = 5;
        cssTooltip.style.borderTopLeftRadius = 5;
        cssTooltip.style.borderTopRightRadius = 5;
        cssTooltip.style.borderTopWidth = 1;
        cssTooltip.style.borderBottomWidth = 1;
        cssTooltip.style.borderLeftWidth = 1;
        cssTooltip.style.borderRightWidth = 1;
        cssTooltip.style.borderTopColor = new StyleColor(new Color(0.22f, 0.74f, 0.97f));
        cssTooltip.style.borderBottomColor = new StyleColor(new Color(0.22f, 0.74f, 0.97f));
        cssTooltip.style.borderLeftColor = new StyleColor(new Color(0.22f, 0.74f, 0.97f));
        cssTooltip.style.borderRightColor = new StyleColor(new Color(0.22f, 0.74f, 0.97f));
        cssTooltip.style.display = DisplayStyle.None;
        workArea.Add(cssTooltip);

        // Compiler Scan Line
        scanLine = new VisualElement();
        scanLine.style.position = Position.Absolute;
        scanLine.style.left = 0;
        scanLine.style.right = 0;
        scanLine.style.top = 0;
        scanLine.style.height = 10;
        scanLine.style.backgroundColor = new StyleColor(new Color(0.22f, 0.74f, 0.97f, 0.8f));
        
        // Add a glow shadow to scan line
        VisualElement scanGlow = new VisualElement();
        scanGlow.style.position = Position.Absolute;
        scanGlow.style.left = 0;
        scanGlow.style.right = 0;
        scanGlow.style.top = -10;
        scanGlow.style.height = 30;
        scanGlow.style.backgroundColor = new StyleColor(new Color(0.22f, 0.74f, 0.97f, 0.3f));
        scanLine.Add(scanGlow);
        
        scanLine.style.display = DisplayStyle.None;
        workArea.Add(scanLine);

        // Flash Overlay
        flashOverlay = new VisualElement();
        flashOverlay.style.position = Position.Absolute;
        flashOverlay.style.left = 0;
        flashOverlay.style.right = 0;
        flashOverlay.style.top = 0;
        flashOverlay.style.bottom = 0;
        flashOverlay.style.backgroundColor = new StyleColor(new Color(1f, 1f, 1f, 0f));
        workArea.Add(flashOverlay);

        draggableIcon.BringToFront();
    }

    public void StartGame()
    {
        chances = 3;
        timeRemaining = config.timeLimit > 0 ? config.timeLimit : 15f;
        container.style.display = DisplayStyle.Flex;

        UpdateScoreText();
        ResetRound();

        manager.StartCoroutine(GameRoutine());
    }

    public void AbortGame()
    {
        container.style.display = DisplayStyle.None;
        ClearParticles();
    }

    private void ResetRound()
    {
        isScoring = false;
        confirmBtn.style.display = DisplayStyle.Flex;
        scoreArea.style.display = DisplayStyle.None;
        targetCrosshair.style.display = DisplayStyle.None;
        cssTooltip.style.display = DisplayStyle.None;
        
        // Update to the "X" and neon color
        draggableIcon.text = "✖";
        draggableIcon.style.color = new StyleColor(new Color(0.9f, 0.9f, 0.9f));
        
        draggableIcon.style.left = new Length(Random.Range(10f, 90f), LengthUnit.Percent);
        draggableIcon.style.top = new Length(Random.Range(10f, 90f), LengthUnit.Percent);
        
        ClearParticles();
    }

    private void OnPointerDown(PointerDownEvent e)
    {
        if (!manager.isPlaying || isScoring) return;
        isDragging = true;
        draggableIcon.CapturePointer(e.pointerId);
        
        draggableIcon.style.scale = new StyleScale(new Vector2(1.3f, 1.3f));
        draggableIcon.style.color = new StyleColor(new Color(0.22f, 0.74f, 0.97f)); // Glow blue on touch
        
        e.StopPropagation();
    }

    private void OnPointerMove(PointerMoveEvent e)
    {
        if (!isDragging || !manager.isPlaying) return;
        
        Vector2 localPos = workArea.WorldToLocal(e.position);
        float width = workArea.resolvedStyle.width;
        float height = workArea.resolvedStyle.height;
        
        if (width > 0 && height > 0)
        {
            float percentX = (localPos.x / width) * 100f;
            float percentY = ((localPos.y - 40f) / height) * 100f; 
            
            percentX = Mathf.Clamp(percentX, 0f, 100f);
            percentY = Mathf.Clamp(percentY, 0f, 100f);
            
            draggableIcon.style.left = new Length(percentX, LengthUnit.Percent);
            draggableIcon.style.top = new Length(percentY, LengthUnit.Percent);

            // Ghost Trail Effect
            ghostTimer += Time.deltaTime;
            if (ghostTimer > 0.05f)
            {
                ghostTimer = 0f;
                manager.StartCoroutine(SpawnGhostTrail(percentX, percentY));
            }
        }
        e.StopPropagation();
    }

    private IEnumerator SpawnGhostTrail(float x, float y)
    {
        Label ghost = new Label("✖");
        ghost.style.position = Position.Absolute;
        ghost.style.left = new Length(x, LengthUnit.Percent);
        ghost.style.top = new Length(y, LengthUnit.Percent);
        ghost.style.fontSize = 60;
        ghost.style.marginLeft = -30;
        ghost.style.marginTop = -30;
        ghost.style.width = 60;
        ghost.style.height = 60;
        ghost.style.unityTextAlign = TextAnchor.MiddleCenter;
        ghost.style.color = new StyleColor(new Color(0.22f, 0.74f, 0.97f, 0.4f));
        
        workArea.Add(ghost);
        activeParticles.Add(ghost);
        ghost.SendToBack();

        float life = 0.4f;
        float elapsed = 0f;
        while(elapsed < life && manager.isPlaying)
        {
            elapsed += Time.deltaTime;
            float p = 1f - (elapsed / life);
            ghost.style.scale = new StyleScale(new Vector2(p, p));
            ghost.style.opacity = p;
            yield return null;
        }

        if (workArea.Contains(ghost)) workArea.Remove(ghost);
        activeParticles.Remove(ghost);
    }

    private void OnPointerUp(PointerUpEvent e)
    {
        if (!isDragging) return;
        EndDrag(e.pointerId);
        e.StopPropagation();
    }

    private void OnPointerCaptureOut(PointerCaptureOutEvent e)
    {
        EndDrag(-1);
    }

    private void EndDrag(int pointerId)
    {
        isDragging = false;
        draggableIcon.style.scale = new StyleScale(new Vector2(1f, 1f));
        draggableIcon.style.color = new StyleColor(new Color(0.9f, 0.9f, 0.9f));

        if (pointerId != -1 && draggableIcon.HasPointerCapture(pointerId))
        {
            draggableIcon.ReleasePointer(pointerId);
        }
    }

    private void OnConfirmClicked()
    {
        if (!manager.isPlaying || isScoring) return;
        manager.StartCoroutine(CompilerScanRoutine());
    }

    private IEnumerator CompilerScanRoutine()
    {
        isScoring = true;
        confirmBtn.style.display = DisplayStyle.None;
        
        // 1. Double Scanner sweep
        scanLine.style.display = DisplayStyle.Flex;
        float scanTime = 0.4f;
        
        // Down
        float elapsed = 0f;
        while(elapsed < scanTime)
        {
            elapsed += Time.deltaTime;
            scanLine.style.top = new Length((elapsed / scanTime) * 100f, LengthUnit.Percent);
            yield return null;
        }
        // Up
        elapsed = 0f;
        while(elapsed < scanTime)
        {
            elapsed += Time.deltaTime;
            scanLine.style.top = new Length(100f - ((elapsed / scanTime) * 100f), LengthUnit.Percent);
            yield return null;
        }
        scanLine.style.display = DisplayStyle.None;

        // 2. Evaluate accuracy
        targetCrosshair.style.display = DisplayStyle.Flex;
        
        float iconX = draggableIcon.style.left.value.value;
        float iconY = draggableIcon.style.top.value.value;
        
        float distX = Mathf.Abs(50f - iconX);
        float distY = Mathf.Abs(50f - iconY);
        float maxDist = Mathf.Sqrt(50f*50f + 50f*50f); 
        float actualDist = Mathf.Sqrt(distX*distX + distY*distY);
        
        float matchPercent = Mathf.Clamp01(1f - (actualDist / maxDist)) * 100f;
        
        scoreArea.style.display = DisplayStyle.Flex;
        if (scoreValueText != null) scoreValueText.text = $"Compilation Match: {matchPercent:F1}%";
        if (scoreBarFill != null) scoreBarFill.style.width = new Length(matchPercent, LengthUnit.Percent);
        
        if (matchPercent >= 90f)
        {
            // SUCCESS JUICE
            AudioManager.Instance?.PlayCenterDivSnap();
            Color winCol = new Color(0.13f, 0.77f, 0.36f);
            if (scoreValueText != null) scoreValueText.style.color = new StyleColor(winCol);
            draggableIcon.text = "✓";
            draggableIcon.style.color = new StyleColor(winCol);
            
            flashOverlay.style.backgroundColor = new StyleColor(new Color(0.13f, 0.77f, 0.36f, 0.4f));
            manager.StartCoroutine(ExplodeParticles(iconX, iconY, winCol));
            
            yield return new WaitForSeconds(0.1f);
            flashOverlay.style.backgroundColor = new StyleColor(new Color(1f, 1f, 1f, 0f));
            
            cssTooltip.style.left = draggableIcon.style.left;
            cssTooltip.style.top = new Length(draggableIcon.style.top.value.value - 15f, LengthUnit.Percent);
            cssTooltip.text = "align-items: center;";
            cssTooltip.style.display = DisplayStyle.Flex;
            cssTooltip.style.color = new StyleColor(winCol);
            cssTooltip.style.borderTopColor = new StyleColor(winCol);

            yield return new WaitForSeconds(1.5f);
            manager.EndGame(true);
        }
        else
        {
            // FAILURE JUICE
            AudioManager.Instance?.PlayBuzzer();
            Color failCol = new Color(0.9f, 0.1f, 0.1f);
            if (scoreValueText != null) scoreValueText.style.color = new StyleColor(failCol);
            draggableIcon.style.color = new StyleColor(failCol);
            
            flashOverlay.style.backgroundColor = new StyleColor(new Color(0.9f, 0.1f, 0.1f, 0.4f));
            manager.StartCoroutine(ExplodeParticles(iconX, iconY, failCol));
            
            yield return new WaitForSeconds(0.1f);
            flashOverlay.style.backgroundColor = new StyleColor(new Color(1f, 1f, 1f, 0f));

            draggableIcon.style.left = new Length(0f, LengthUnit.Percent);
            draggableIcon.style.top = new Length(0f, LengthUnit.Percent);
            
            cssTooltip.style.left = new Length(0f, LengthUnit.Percent);
            cssTooltip.style.top = new Length(15f, LengthUnit.Percent);
            cssTooltip.text = "FATAL: CSS BROKEN";
            cssTooltip.style.display = DisplayStyle.Flex;
            cssTooltip.style.color = new StyleColor(failCol);
            cssTooltip.style.borderTopColor = new StyleColor(failCol);
            
            // Screen Shake
            container.style.left = 15f;
            container.style.top = 10f;
            yield return new WaitForSeconds(0.05f);
            container.style.left = -15f;
            container.style.top = -10f;
            yield return new WaitForSeconds(0.05f);
            container.style.left = 0f;
            container.style.top = 0f;

            chances--;
            UpdateScoreText();
            yield return new WaitForSeconds(1.5f);
            
            if (chances <= 0)
            {
                manager.EndGame(false);
            }
            else
            {
                ResetRound();
            }
        }
    }

    private IEnumerator ExplodeParticles(float startX, float startY, Color color)
    {
        int count = 15;
        List<VisualElement> parts = new List<VisualElement>();
        List<Vector2> vels = new List<Vector2>();

        for (int i = 0; i < count; i++)
        {
            VisualElement p = new VisualElement();
            p.style.position = Position.Absolute;
            p.style.width = Random.Range(4, 10);
            p.style.height = p.style.width;
            p.style.backgroundColor = new StyleColor(color);
            p.style.left = new Length(startX, LengthUnit.Percent);
            p.style.top = new Length(startY, LengthUnit.Percent);
            
            workArea.Add(p);
            parts.Add(p);
            activeParticles.Add(p);

            float angle = Random.Range(0f, Mathf.PI * 2f);
            float speed = Random.Range(20f, 60f); // Percent per second
            vels.Add(new Vector2(Mathf.Cos(angle) * speed, Mathf.Sin(angle) * speed));
        }

        float life = 0.6f;
        float elapsed = 0f;
        while(elapsed < life && manager.isPlaying)
        {
            elapsed += Time.deltaTime;
            for (int i = 0; i < parts.Count; i++)
            {
                var p = parts[i];
                if (p == null) continue;
                
                float nx = p.style.left.value.value + (vels[i].x * Time.deltaTime);
                float ny = p.style.top.value.value + (vels[i].y * Time.deltaTime);
                
                p.style.left = new Length(nx, LengthUnit.Percent);
                p.style.top = new Length(ny, LengthUnit.Percent);
                p.style.opacity = 1f - (elapsed / life);
            }
            yield return null;
        }

        foreach(var p in parts)
        {
            if (workArea.Contains(p)) workArea.Remove(p);
            activeParticles.Remove(p);
        }
    }

    private void ClearParticles()
    {
        foreach (var p in activeParticles)
        {
            if (p != null && workArea.Contains(p))
            {
                workArea.Remove(p);
            }
        }
        activeParticles.Clear();
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Attempts left: {chances}";
        }
    }

    private IEnumerator GameRoutine()
    {
        while (manager.isPlaying)
        {
            if (!isScoring)
            {
                timeRemaining -= Time.deltaTime;
                if (timeText != null) timeText.text = $"{Mathf.Max(0, timeRemaining):F1}s";

                if (timeRemaining <= 0)
                {
                    manager.EndGame(false);
                    yield break;
                }
            }
            yield return null;
        }
    }
}

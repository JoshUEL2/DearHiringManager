using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;

public class SizeMatchMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement container;

    private Label scoreText;
    private Label timeText;
    private VisualElement workArea;
    private VisualElement targetBox;
    private Button changingBox;
    
    // Juice Elements
    private VisualElement flashOverlay;
    private Label cssTooltip;
    private Label floatingPenaltyText;
    
    private int score = 0;
    private float timeRemaining = 15f;
    private float currentScale = 0.2f;
    private float scaleDir = 1f;
    
    // Gameplay state
    private bool isPaused = false;
    private float baseSpeed = 1.2f;
    private float jitterTimer = 0f;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        container = root.Q<VisualElement>("SizeMatchContainer");
        container.style.width = new Length(100f, LengthUnit.Percent);

        scoreText = container.Q<Label>("SmRoundText");
        timeText = container.Q<Label>("SmTimerText");
        workArea = container.Q<VisualElement>("SmWorkArea");
        targetBox = container.Q<VisualElement>("SmTargetBox");
        changingBox = container.Q<Button>("SmBtnChangingBox");
        
        // Remove old UXML overlay if it exists, we create our own
        var oldOverlay = container.Q<VisualElement>("SmFlashOverlay") ?? new VisualElement();
        if (oldOverlay != null) oldOverlay.style.display = DisplayStyle.None;

        // Register on workArea so ANY tap counts, not just tapping the moving box!
        workArea.RegisterCallback<PointerDownEvent>(OnWorkAreaTapped);
        
        CreateJuiceElements();
    }

    private void CreateJuiceElements()
    {
        // Style Work Area to DevTools Dark Theme
        workArea.style.backgroundColor = new StyleColor(new Color(0.06f, 0.09f, 0.16f)); // Deep dark slate
        
        // Style Target Box (The Container)
        targetBox.style.backgroundColor = new StyleColor(new Color(1f, 1f, 1f, 0.05f));
        targetBox.style.borderTopColor = new StyleColor(new Color(0.58f, 0.64f, 0.72f, 0.5f));
        targetBox.style.borderBottomColor = targetBox.style.borderTopColor;
        targetBox.style.borderLeftColor = targetBox.style.borderTopColor;
        targetBox.style.borderRightColor = targetBox.style.borderTopColor;
        // In UI Toolkit, dashed borders aren't natively supported easily, so we use faint colors

        // Flash Overlay
        flashOverlay = new VisualElement();
        flashOverlay.style.position = Position.Absolute;
        flashOverlay.style.left = 0;
        flashOverlay.style.right = 0;
        flashOverlay.style.top = 0;
        flashOverlay.style.bottom = 0;
        flashOverlay.style.backgroundColor = new StyleColor(new Color(1f, 1f, 1f, 0f));
        workArea.Add(flashOverlay);

        // CSS Tooltip (Follows the changing box)
        cssTooltip = new Label();
        cssTooltip.style.position = Position.Absolute;
        cssTooltip.style.backgroundColor = new StyleColor(new Color(0.1f, 0.1f, 0.15f, 0.9f));
        cssTooltip.style.color = new StyleColor(new Color(0.96f, 0.62f, 0.04f)); // Orange
        cssTooltip.style.fontSize = 16;
        cssTooltip.style.paddingLeft = 6;
        cssTooltip.style.paddingRight = 6;
        cssTooltip.style.paddingTop = 2;
        cssTooltip.style.paddingBottom = 2;
        cssTooltip.style.borderTopLeftRadius = 4;
        cssTooltip.style.borderTopRightRadius = 4;
        cssTooltip.style.borderBottomLeftRadius = 4;
        cssTooltip.style.borderBottomRightRadius = 4;
        workArea.Add(cssTooltip);

        // Floating Penalty Text
        floatingPenaltyText = new Label("-1.5s");
        floatingPenaltyText.style.position = Position.Absolute;
        floatingPenaltyText.style.color = new StyleColor(new Color(0.93f, 0.26f, 0.22f)); // Red
        floatingPenaltyText.style.fontSize = 32;
        floatingPenaltyText.style.unityFontStyleAndWeight = FontStyle.Bold;
        floatingPenaltyText.style.opacity = 0f;
        floatingPenaltyText.style.right = 20;
        floatingPenaltyText.style.top = 20;
        container.Add(floatingPenaltyText); // Add to container so it overlaps header
    }

    public void StartGame()
    {
        score = 0;
        timeRemaining = config.timeLimit > 0 ? config.timeLimit : 15f;
        currentScale = 0.2f;
        scaleDir = 1f;
        isPaused = false;
        
        container.style.display = DisplayStyle.Flex;
        changingBox.style.backgroundColor = new StyleColor(new Color(0.96f, 0.62f, 0.04f, 0.8f)); // Semi-transparent neon orange
        changingBox.style.color = new StyleColor(Color.white);
        changingBox.text = "<div id=\"this\">";

        UpdateScoreText();
        manager.StartCoroutine(GameRoutine());
    }

    public void AbortGame()
    {
        container.style.display = DisplayStyle.None;
        workArea.UnregisterCallback<PointerDownEvent>(OnWorkAreaTapped);
    }

    private void OnWorkAreaTapped(PointerDownEvent e)
    {
        if (!manager.isPlaying || isPaused) return;

        float diff = Mathf.Abs(currentScale - 1.0f);
        float tolerance = 0.15f; // Forgiving 15% margin as requested

        if (diff <= tolerance)
        {
            manager.StartCoroutine(HandleSuccess());
        }
        else
        {
            manager.StartCoroutine(HandleFailure());
        }
    }

    private IEnumerator HandleSuccess()
    {
        isPaused = true;
        score++;
        UpdateScoreText();

        // Snap to exact perfection
        currentScale = 1.0f;
        UpdateBoxVisuals(true);

        // Success Juice
        AudioManager.Instance?.PlayCenterDivSnap();
        AudioManager.Instance?.PlayScorePop();
        changingBox.style.backgroundColor = new StyleColor(new Color(0.13f, 0.77f, 0.36f, 0.9f));
        changingBox.text = "🔒 100%";
        cssTooltip.style.color = new StyleColor(new Color(0.13f, 0.77f, 0.36f));
        cssTooltip.text = "box-sizing: border-box;";
        
        flashOverlay.style.backgroundColor = new StyleColor(new Color(0.13f, 0.77f, 0.36f, 0.3f));
        yield return new WaitForSeconds(0.1f);
        flashOverlay.style.backgroundColor = new StyleColor(new Color(1f, 1f, 1f, 0f));

        if (score >= 3)
        {
            AudioManager.Instance?.PlayVictoryFanfare();
            yield return new WaitForSeconds(0.8f);
            manager.EndGame(true);
        }
        else
        {
            yield return new WaitForSeconds(0.6f);
            
            // Reset for next round
            currentScale = 0.2f;
            scaleDir = 1f;
            changingBox.style.backgroundColor = new StyleColor(new Color(0.96f, 0.62f, 0.04f, 0.8f));
            changingBox.text = "<div id=\"this\">";
            cssTooltip.style.color = new StyleColor(new Color(0.96f, 0.62f, 0.04f));
            isPaused = false;
        }
    }

    private IEnumerator HandleFailure()
    {
        isPaused = true;
        AudioManager.Instance?.PlayBuzzer();
        
        // Instant fail on miss
        floatingPenaltyText.text = "FATAL ERROR!";
        
        // Failure Juice
        changingBox.style.backgroundColor = new StyleColor(new Color(0.93f, 0.26f, 0.22f, 0.9f));
        cssTooltip.style.color = new StyleColor(new Color(0.93f, 0.26f, 0.22f));
        cssTooltip.text = "Exception: Overflow!";
        
        flashOverlay.style.backgroundColor = new StyleColor(new Color(0.93f, 0.26f, 0.22f, 0.4f));
        
        // Floating Penalty Text Animation
        manager.StartCoroutine(AnimatePenaltyText());

        // Screen Shake
        workArea.style.left = 15f;
        workArea.style.top = 10f;
        yield return new WaitForSeconds(0.05f);
        workArea.style.left = -15f;
        workArea.style.top = -10f;
        yield return new WaitForSeconds(0.05f);
        workArea.style.left = 0f;
        workArea.style.top = 0f;

        flashOverlay.style.backgroundColor = new StyleColor(new Color(1f, 1f, 1f, 0f));
        
        yield return new WaitForSeconds(1.0f);
        AudioManager.Instance?.PlayMinigameFail();
        manager.EndGame(false);
    }

    private IEnumerator AnimatePenaltyText()
    {
        floatingPenaltyText.style.opacity = 1f;
        floatingPenaltyText.style.top = 10;
        
        float life = 0.8f;
        float elapsed = 0f;
        while(elapsed < life && manager.isPlaying)
        {
            elapsed += Time.deltaTime;
            float p = elapsed / life;
            
            // Float up and fade out
            floatingPenaltyText.style.top = 10f - (p * 40f);
            floatingPenaltyText.style.opacity = 1f - p;
            yield return null;
        }
        floatingPenaltyText.style.opacity = 0f;
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Matched: {score}/3";
        }
    }

    private void UpdateBoxVisuals(bool isPerfect)
    {
        if (changingBox == null) return;

        changingBox.style.width = new Length(200f * currentScale, LengthUnit.Pixel);
        changingBox.style.height = new Length(150f * currentScale, LengthUnit.Pixel);
        
        // Update Tooltip position and text
        if (!isPaused)
        {
            float w = 200f * currentScale;
            float h = 150f * currentScale;
            
            // Generate chaotic CSS string
            string css = "";
            if (currentScale > 1.05f) css = $"width: calc({(currentScale*100):F0}% + {Random.Range(10, 50)}px); overflow: visible !important;";
            else if (currentScale < 0.95f) css = $"width: {(currentScale*100):F0}%; height: {(currentScale*100):F0}%; margin: auto;";
            else css = "width: 100%; height: 100%; /* almost... */";
            
            cssTooltip.text = css;
        }
        
        // Tooltip follows the bottom-right corner of the box roughly
        cssTooltip.style.left = new Length(50f + (currentScale * 15f), LengthUnit.Percent);
        cssTooltip.style.top = new Length(50f + (currentScale * 25f), LengthUnit.Percent);
    }

    private IEnumerator GameRoutine()
    {
        while (manager.isPlaying)
        {
            float dt = Time.deltaTime;
            
            // Timer logic runs even if paused
            timeRemaining -= dt;
            if (timeText != null) timeText.text = $"{Mathf.Max(0, timeRemaining):F1}s";

            if (timeRemaining <= 0)
            {
                manager.EndGame(false);
                yield break;
            }

            if (!isPaused)
            {
                // Progressive Difficulty
                baseSpeed = 0.8f + (score * 0.4f); // Round 1: 0.8, Round 2: 1.2, Round 3: 1.6
                float currentSpeed = baseSpeed;

                // Jitter logic (simulating CSS breakpoint snapping)
                if (score >= 1) // Only jitter on Round 2 and 3
                {
                    jitterTimer -= dt;
                    if (jitterTimer <= 0)
                    {
                        if (Mathf.Abs(currentScale - 1.0f) < 0.25f) jitterTimer = 0.2f; // Defer jitter if in safe zone
                        else {
                        // Sudden snap
                        currentScale += Random.Range(-0.15f, 0.15f);
                        jitterTimer = Random.Range(0.2f, 0.6f - (score * 0.1f)); 
                        
                        // Flash tooltip to simulate a layout recalculation
                        cssTooltip.style.backgroundColor = new StyleColor(new Color(0.9f, 0.9f, 0.9f, 0.9f));
                        manager.StartCoroutine(ResetTooltipColor());
                        }
                    }
                }

                currentScale += currentSpeed * scaleDir * dt;
                
                if (currentScale >= 1.6f)
                {
                    currentScale = 1.6f;
                    scaleDir = -1f;
                }
                else if (currentScale <= 0.3f)
                {
                    currentScale = 0.3f;
                    scaleDir = 1f;
                }

                UpdateBoxVisuals(false);
            }

            yield return null;
        }
    }

    private IEnumerator ResetTooltipColor()
    {
        yield return new WaitForSeconds(0.05f);
        if (cssTooltip != null) cssTooltip.style.backgroundColor = new StyleColor(new Color(0.1f, 0.1f, 0.15f, 0.9f));
    }
}

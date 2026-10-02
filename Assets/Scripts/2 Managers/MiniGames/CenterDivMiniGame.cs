using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;

public class CenterDivMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement container;

    private Label scoreText;
    private Label timeText;
    private VisualElement workArea;
    private VisualElement targetBox;
    private VisualElement movingBox;
    private Button centerBtn;
    
    // Juice Elements
    private VisualElement flashOverlay;
    private Label cssTooltip;
    private Label floatingPenaltyText;
    
    // Effects tracking
    private float ghostTimer = 0f;
    private List<VisualElement> activeParticles = new List<VisualElement>();
    
    private int score = 0;
    private float timeRemaining = 15f;
    
    // Gameplay state
    private bool isPaused = false;
    private float timeAccumulator = 0f;
    private float movingBoxPos = 0f; // 0 to 100

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        container = root.Q<VisualElement>("CenterDivContainer");
        container.style.width = new Length(100f, LengthUnit.Percent); // Stop it from shrinking when text changes

        scoreText = container.Q<Label>("CdRoundText");
        scoreText.style.width = 200; // Fixed width to completely prevent layout narrowing when text changes
        timeText = container.Q<Label>("CdTimerText");
        
        // Target elements from UXML
        targetBox = container.Q<VisualElement>("CdTargetBox");
        movingBox = container.Q<VisualElement>("CdMovingBox");
        centerBtn = container.Q<Button>("CdBtnCenter");
        // Lock the minimum width so changing text to "SUCCESS!" doesn't collapse the container width
        centerBtn.style.minWidth = 500;
        
        // workArea is the parent of targetBox
        workArea = targetBox.parent;
        
        // Remove old UXML overlay if it exists
        var oldOverlay = container.Q<VisualElement>("CdFlashOverlay") ?? new VisualElement();
        if (oldOverlay != null) oldOverlay.style.display = DisplayStyle.None;

        centerBtn.clicked += OnCenterClicked;
        
        CreateJuiceElements();
    }

    private void CreateJuiceElements()
    {
        // Style Work Area to DevTools Dark Theme
        workArea.style.backgroundColor = new StyleColor(new Color(0.06f, 0.09f, 0.16f)); 
        
        // Style Target Box (The Rulers)
        targetBox.style.backgroundColor = new StyleColor(new Color(1f, 1f, 1f, 0f));
        targetBox.style.borderTopColor = new StyleColor(new Color(0.13f, 0.85f, 0.95f, 0.5f)); // Cyan
        targetBox.style.borderBottomColor = targetBox.style.borderTopColor;
        targetBox.style.borderLeftColor = targetBox.style.borderTopColor;
        targetBox.style.borderRightColor = targetBox.style.borderTopColor;
        targetBox.style.left = new Length(50f, LengthUnit.Percent); // Force absolute center
        
        // Scale target box larger than moving box so it acts as a clear outer frame
        targetBox.style.width = 180;
        targetBox.style.height = 180;
        targetBox.style.marginLeft = -90;
        targetBox.style.marginTop = -90;
        
        movingBox.style.width = 160;
        movingBox.style.height = 160;
        movingBox.style.marginLeft = -80;
        movingBox.style.marginTop = -80;
        


        // Flash Overlay
        flashOverlay = new VisualElement();
        flashOverlay.style.position = Position.Absolute;
        flashOverlay.style.left = 0;
        flashOverlay.style.right = 0;
        flashOverlay.style.top = 0;
        flashOverlay.style.bottom = 0;
        flashOverlay.style.backgroundColor = new StyleColor(new Color(1f, 1f, 1f, 0f));
        workArea.Add(flashOverlay);

        // CSS Tooltip
        cssTooltip = new Label();
        cssTooltip.style.position = Position.Absolute;
        cssTooltip.style.backgroundColor = new StyleColor(new Color(0.1f, 0.1f, 0.15f, 0.9f));
        cssTooltip.style.color = new StyleColor(new Color(0.13f, 0.77f, 0.36f)); // Green
        cssTooltip.style.fontSize = 18;
        cssTooltip.style.paddingLeft = 8;
        cssTooltip.style.paddingRight = 8;
        cssTooltip.style.paddingTop = 4;
        cssTooltip.style.paddingBottom = 4;
        cssTooltip.style.borderTopLeftRadius = 4;
        cssTooltip.style.borderTopRightRadius = 4;
        cssTooltip.style.borderBottomLeftRadius = 4;
        cssTooltip.style.borderBottomRightRadius = 4;
        cssTooltip.style.left = new Length(50f, LengthUnit.Percent);
        cssTooltip.style.top = new Length(15f, LengthUnit.Percent);
        cssTooltip.style.display = DisplayStyle.None;
        workArea.Add(cssTooltip);

        // Make moving box look like HTML element
        Label boxText = new Label("<div class=\"hero\">");
        boxText.style.color = new StyleColor(Color.white);
        boxText.style.unityTextAlign = TextAnchor.MiddleCenter;
        boxText.style.fontSize = 18;
        boxText.style.width = new Length(100f, LengthUnit.Percent);
        boxText.style.height = new Length(100f, LengthUnit.Percent);
        movingBox.Add(boxText);
        
        movingBox.BringToFront();
    }

    public void StartGame()
    {
        score = 0;
        timeRemaining = config.timeLimit > 0 ? config.timeLimit : 15f;
        timeAccumulator = 0f;
        isPaused = false;
        
        container.style.display = DisplayStyle.Flex;
        
        // Reset Visuals
        movingBox.style.backgroundColor = new StyleColor(new Color(0.96f, 0.62f, 0.04f)); // Orange
        centerBtn.style.backgroundColor = new StyleColor(new Color(0.22f, 0.74f, 0.97f)); // Normal Blue
        centerBtn.text = "[ margin: auto; ]";
        cssTooltip.style.display = DisplayStyle.None;
        
        ClearParticles();
        UpdateScoreText();

        manager.StartCoroutine(GameRoutine());
    }

    public void AbortGame()
    {
        container.style.display = DisplayStyle.None;
        ClearParticles();
    }

    private void OnCenterClicked()
    {
        if (!manager.isPlaying || isPaused) return;

        // Button physically depresses
        centerBtn.style.scale = new StyleScale(new Vector2(0.95f, 0.95f));
        centerBtn.schedule.Execute(() => {
            if (centerBtn != null) centerBtn.style.scale = new StyleScale(new Vector2(1f, 1f));
        }).StartingIn(100);

        // Distance from exact center (50%)
        float dist = Mathf.Abs(50f - movingBoxPos);
        float tolerance = 18f; // Even wider tolerance (18%) as requested

        if (dist <= tolerance)
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

        // Snap to absolute perfection
        movingBoxPos = 50f;
        movingBox.style.left = new Length(50f, LengthUnit.Percent);

        // Success Juice
        movingBox.style.backgroundColor = new StyleColor(new Color(0.13f, 0.77f, 0.36f)); // Green
        centerBtn.style.backgroundColor = new StyleColor(new Color(0.13f, 0.77f, 0.36f));
        centerBtn.text = "SUCCESS!";
        
        cssTooltip.text = "margin: 0 auto;";
        cssTooltip.style.color = new StyleColor(new Color(0.13f, 0.77f, 0.36f));
        cssTooltip.style.display = DisplayStyle.Flex;
        
        flashOverlay.style.backgroundColor = new StyleColor(new Color(0.13f, 0.77f, 0.36f, 0.3f));
        yield return new WaitForSeconds(0.1f);
        flashOverlay.style.backgroundColor = new StyleColor(new Color(1f, 1f, 1f, 0f));

        if (score >= 3)
        {
            yield return new WaitForSeconds(0.8f);
            manager.EndGame(true);
        }
        else
        {
            yield return new WaitForSeconds(0.6f);
            
            // Reset for next round
            movingBox.style.backgroundColor = new StyleColor(new Color(0.96f, 0.62f, 0.04f));
            centerBtn.style.backgroundColor = new StyleColor(new Color(0.22f, 0.74f, 0.97f));
            centerBtn.text = "[ margin: auto; ]";
            cssTooltip.style.display = DisplayStyle.None;
            timeAccumulator = 0f; // Reset position to edge
            isPaused = false;
        }
    }

    private IEnumerator HandleFailure()
    {
        isPaused = true;
        
        // Failure Juice
        movingBox.style.backgroundColor = new StyleColor(new Color(0.93f, 0.26f, 0.22f)); // Red
        centerBtn.style.backgroundColor = new StyleColor(new Color(0.93f, 0.26f, 0.22f));
        centerBtn.text = "FLOAT COLLAPSED!";
        
        cssTooltip.text = "Exception: Layout Broken!";
        cssTooltip.style.color = new StyleColor(new Color(0.93f, 0.26f, 0.22f));
        cssTooltip.style.display = DisplayStyle.Flex;
        
        flashOverlay.style.backgroundColor = new StyleColor(new Color(0.93f, 0.26f, 0.22f, 0.4f));

        // Ricochet animation - fly off screen
        float direction = movingBoxPos < 50f ? -1f : 1f;
        float failSpeed = 200f;
        
        // Screen Shake
        workArea.style.left = 15f;
        workArea.style.top = 10f;
        centerBtn.style.left = 10f;
        
        float shakeT = 0f;
        while(shakeT < 0.1f) {
            shakeT += Time.deltaTime;
            movingBoxPos += failSpeed * direction * Time.deltaTime;
            movingBox.style.left = new Length(movingBoxPos, LengthUnit.Percent);
            yield return null;
        }
        
        workArea.style.left = -15f;
        workArea.style.top = -10f;
        centerBtn.style.left = -10f;
        
        while(shakeT < 0.2f) {
            shakeT += Time.deltaTime;
            movingBoxPos += failSpeed * direction * Time.deltaTime;
            movingBox.style.left = new Length(movingBoxPos, LengthUnit.Percent);
            yield return null;
        }

        workArea.style.left = 0f;
        workArea.style.top = 0f;
        centerBtn.style.left = 0f;
        flashOverlay.style.backgroundColor = new StyleColor(new Color(1f, 1f, 1f, 0f));
        
        yield return new WaitForSeconds(0.8f);
        manager.EndGame(false);
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Centered: {score}/3";
        }
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
                timeAccumulator += dt;
                
                // Physics based on Round
                if (score == 0)
                {
                    // Round 1: Linear PingPong
                    float speed = 120f; // Percent per second
                    movingBoxPos = Mathf.PingPong(timeAccumulator * speed, 100f);
                }
                else if (score == 1)
                {
                    // Round 2: Sine Wave (Easing - Slow at edges, blazing fast in center)
                    float frequency = 2.5f; 
                    // -Mathf.Cos goes from -1 to 1. Multiply by 50 and add 50 to get 0 to 100.
                    movingBoxPos = 50f + (-Mathf.Cos(timeAccumulator * frequency) * 50f);
                }
                else
                {
                    // Round 3: Pure Speed Easing (No glitching)
                    float frequency = 4.0f; // Extremely fast sine wave
                    movingBoxPos = 50f + (-Mathf.Cos(timeAccumulator * frequency) * 50f);
                }

                // Ensure it stays in bounds mathematically
                movingBoxPos = Mathf.Clamp(movingBoxPos, 0f, 100f);
                movingBox.style.left = new Length(movingBoxPos, LengthUnit.Percent);

                // Ghost Trail Effect
                ghostTimer += dt;
                if (ghostTimer > 0.05f)
                {
                    ghostTimer = 0f;
                    manager.StartCoroutine(SpawnGhostTrail(movingBoxPos));
                }
            }

            yield return null;
        }
    }

    private IEnumerator ResetFlash()
    {
        yield return new WaitForSeconds(0.05f);
        if (flashOverlay != null && !isPaused) flashOverlay.style.backgroundColor = new StyleColor(new Color(1f, 1f, 1f, 0f));
    }

    private IEnumerator SpawnGhostTrail(float posX)
    {
        VisualElement ghost = new VisualElement();
        ghost.style.position = Position.Absolute;
        ghost.style.left = new Length(posX, LengthUnit.Percent);
        ghost.style.top = new Length(50f, LengthUnit.Percent);
        ghost.style.width = 160;
        ghost.style.height = 160;
        ghost.style.marginLeft = -80;
        ghost.style.marginTop = -80;
        ghost.style.backgroundColor = new StyleColor(new Color(0.96f, 0.62f, 0.04f, 0.3f)); // Faint orange
        ghost.style.borderTopLeftRadius = 8;
        ghost.style.borderTopRightRadius = 8;
        ghost.style.borderBottomLeftRadius = 8;
        ghost.style.borderBottomRightRadius = 8;
        
        workArea.Add(ghost);
        activeParticles.Add(ghost);
        ghost.SendToBack();

        float life = 0.3f;
        float elapsed = 0f;
        while(elapsed < life && manager.isPlaying && !isPaused)
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
}

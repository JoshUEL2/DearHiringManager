using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class ConstructionSiteMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;

    private VisualElement container;
    private Label inspectionText;
    private Label timeText;
    private Label windLabel;
    private VisualElement windFill;
    private VisualElement balanceZone;
    private VisualElement playerMarker;
    private VisualElement dangerFlash;
    private Button btnLeft;
    private Button btnRight;

    private float inspectionTime = 0f;
    private float timeRemaining = 15f;
    private float requiredInspectionTime = 10f;

    // Physics
    private float playerPos = 50f; // 0 to 100
    private float playerVelocity = 0f;
    private float zonePos = 35f; // left edge of zone (width is 30%)
    private float zoneWidth = 30f;
    private float zoneVelocity = 0f;
    
    // Wind
    private float currentWind = 0f;
    private float targetWind = 0f;
    private float windChangeTimer = 0f;

    private bool leftPressed = false;
    private bool rightPressed = false;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;

        container = root.Q<VisualElement>("ConstructionSiteContainer");
        inspectionText = container.Q<Label>("CsInspectionText");
        timeText = container.Q<Label>("CsTimeText");
        windLabel = container.Q<Label>("CsWindLabel");
        windFill = container.Q<VisualElement>("CsWindFill");
        balanceZone = container.Q<VisualElement>("CsBalanceZone");
        playerMarker = container.Q<VisualElement>("CsPlayerMarker");
        dangerFlash = container.Q<VisualElement>("CsDangerFlash");

        btnLeft = container.Q<Button>("BtnCsLeft");
        btnRight = container.Q<Button>("BtnCsRight");

        // Input bindings for mobile/click
        btnLeft.RegisterCallback<PointerDownEvent>(e => leftPressed = true);
        btnLeft.RegisterCallback<PointerUpEvent>(e => leftPressed = false);
        btnLeft.RegisterCallback<PointerLeaveEvent>(e => leftPressed = false);
        
        btnRight.RegisterCallback<PointerDownEvent>(e => rightPressed = true);
        btnRight.RegisterCallback<PointerUpEvent>(e => rightPressed = false);
        btnRight.RegisterCallback<PointerLeaveEvent>(e => rightPressed = false);
    }

    public void StartGame()
    {
        inspectionTime = 0f;
        timeRemaining = config.timeLimit > 0 ? config.timeLimit : 20f;
        
        playerPos = 50f;
        playerVelocity = 0f;
        
        zoneWidth = 30f;
        zonePos = 35f; // Middle
        
        currentWind = 0f;
        targetWind = 0f;
        windChangeTimer = 2f;

        leftPressed = false;
        rightPressed = false;

        container.style.display = DisplayStyle.Flex;
        dangerFlash.style.backgroundColor = new StyleColor(new Color(0.86f, 0.15f, 0.15f, 0f));

        UpdateUI();
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

            // Wind Logic
            windChangeTimer -= dt;
            if (windChangeTimer <= 0)
            {
                windChangeTimer = Random.Range(1.5f, 3.5f);
                targetWind = Random.Range(-30f, 30f); // -30 left, +30 right
            }
            // Smoothly interpolate wind
            currentWind = Mathf.MoveTowards(currentWind, targetWind, 15f * dt);

            // Player Physics
            float acceleration = currentWind; // Wind pushes player
            
            if (leftPressed) acceleration -= 60f; // Player moves left
            if (rightPressed) acceleration += 60f; // Player moves right

            // Apply friction
            playerVelocity = Mathf.MoveTowards(playerVelocity, 0, 15f * dt);
            
            playerVelocity += acceleration * dt;
            // Cap velocity
            playerVelocity = Mathf.Clamp(playerVelocity, -40f, 40f);
            
            playerPos += playerVelocity * dt;

            // Zone Movement Logic (slowly drifts)
            float distanceToPlayer = playerPos - (zonePos + zoneWidth / 2f);
            zoneVelocity = Mathf.MoveTowards(zoneVelocity, distanceToPlayer * 0.2f, 5f * dt);
            zoneVelocity = Mathf.Clamp(zoneVelocity, -10f, 10f);
            zonePos += zoneVelocity * dt;
            
            // Constrain zone
            zonePos = Mathf.Clamp(zonePos, 5f, 95f - zoneWidth);

            // Check if player is off the edge (Fail)
            if (playerPos < 0f || playerPos > 100f)
            {
                UpdateUI();
                manager.isPlaying = false; // Player fell off!
                yield break;
            }

            // Check Inspection Zone
            bool inZone = (playerPos >= zonePos && playerPos <= zonePos + zoneWidth);
            if (inZone)
            {
                inspectionTime += dt;
                balanceZone.RemoveFromClassList("cs-zone-danger");
            }
            else
            {
                balanceZone.AddToClassList("cs-zone-danger");
            }

            UpdateUI();

            // Win / Lose Checks
            if (inspectionTime >= requiredInspectionTime)
            {
                manager.MarkWinReady();
            }
            else if (timeRemaining <= 0)
            {
                timeRemaining = 0;
                manager.isPlaying = false; // Fail due to time
                yield break;
            }

            yield return null;
        }
    }

    private void UpdateUI()
    {
        inspectionText.text = $"INSPECTION: {inspectionTime:F1}s / {requiredInspectionTime:F1}s";
        timeText.text = $"TIME LEFT: {timeRemaining:F1}s";

        // Update player position
        playerMarker.style.left = new Length(Mathf.Clamp(playerPos, 0, 100), LengthUnit.Percent);
        
        // Update zone position
        balanceZone.style.left = new Length(zonePos, LengthUnit.Percent);

        // Update Wind UI
        string windDir = currentWind > 5f ? "➡️" : (currentWind < -5f ? "⬅️" : " ");
        string intensity = Mathf.Abs(currentWind) > 20f ? "HIGH" : (Mathf.Abs(currentWind) > 10f ? "MODERATE" : "CALM");
        windLabel.text = $"WIND: {intensity} {windDir}";

        // Wind bar
        float windPercent = (currentWind + 30f) / 60f * 100f; // Map -30..30 to 0..100
        windFill.style.width = new Length(windPercent, LengthUnit.Percent);
        if (currentWind > 0)
        {
            windFill.style.left = new Length(50, LengthUnit.Percent);
            windFill.style.width = new Length(Mathf.Abs(currentWind) / 30f * 50f, LengthUnit.Percent);
        }
        else
        {
            windFill.style.left = new Length(50f - (Mathf.Abs(currentWind) / 30f * 50f), LengthUnit.Percent);
            windFill.style.width = new Length(Mathf.Abs(currentWind) / 30f * 50f, LengthUnit.Percent);
        }

        // Danger Flash if close to edge
        if (playerPos < 10f || playerPos > 90f)
        {
            float alpha = 1f - (Mathf.Min(playerPos, 100f - playerPos) / 10f);
            dangerFlash.style.backgroundColor = new StyleColor(new Color(0.86f, 0.15f, 0.15f, alpha * 0.3f));
        }
        else
        {
            dangerFlash.style.backgroundColor = new StyleColor(new Color(0.86f, 0.15f, 0.15f, 0f));
        }
    }
}

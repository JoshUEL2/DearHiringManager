using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class TimingRingMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement container;

    private VisualElement timingCircle;
    private VisualElement timingGlow;
    private VisualElement[] glowLayers = new VisualElement[6];
    private VisualElement timingTargetZone;
    private VisualElement timingMarker;
    private VisualElement arcMaskInner;
    private Label timingRoundText;
    private Button btnTimingStrike;

    private float timingTargetAngle = 0f;
    private float timingMarkerAngle = 0f;
    private int timingSpinDir = 1;
    private int timingCurrentRound = 0;
    private bool timingMarkerMoving = false;
    private float timingRingRotSpeed = 300f;
    private bool isPlaying = false;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        
        container = root.Q<VisualElement>("TimingRingContainer");
        timingCircle = container.Q<VisualElement>("TimingCircle");
        timingGlow = container.Q<VisualElement>("TimingGlow");
        
        if (timingGlow != null)
        {
            for (int i = 0; i < 6; i++)
            {
                glowLayers[i] = timingGlow.Q<VisualElement>($"GlowLayer{i + 1}");
            }
        }
        
        arcMaskInner = container.Q<VisualElement>("ArcMaskInner");
        timingTargetZone = container.Q<VisualElement>("TimingTargetZone");
        timingMarker = container.Q<VisualElement>("TimingMarker");
        timingRoundText = container.Q<Label>("TimingRoundText");
        btnTimingStrike = container.Q<Button>("BtnTimingStrike");
        
        if (btnTimingStrike != null) btnTimingStrike.clicked += OnTimingStrike;
        
        // Visual Overhaul: The Traffic Circle
        timingCircle.style.borderTopColor = new StyleColor(new Color(0.15f, 0.15f, 0.18f));
        timingCircle.style.borderBottomColor = new StyleColor(new Color(0.15f, 0.15f, 0.18f));
        timingCircle.style.borderLeftColor = new StyleColor(new Color(0.15f, 0.15f, 0.18f));
        timingCircle.style.borderRightColor = new StyleColor(new Color(0.15f, 0.15f, 0.18f));
        timingCircle.style.borderTopWidth = 24; timingCircle.style.borderBottomWidth = 24;
        timingCircle.style.borderLeftWidth = 24; timingCircle.style.borderRightWidth = 24;
        timingCircle.style.backgroundColor = new StyleColor(new Color(0.08f, 0.08f, 0.1f));

        // Visual Overhaul: The Sign Spinner's Sign
        timingMarker.Clear();
        var signBody = new VisualElement();
        signBody.style.width = 300;
        signBody.style.height = 110;
        signBody.style.backgroundColor = new StyleColor(new Color(1f, 0.85f, 0.1f)); // Bright Yellow
        signBody.style.borderTopWidth = 6; signBody.style.borderBottomWidth = 6;
        signBody.style.borderLeftWidth = 6; signBody.style.borderRightWidth = 6;
        signBody.style.borderTopColor = new StyleColor(new Color(0.1f, 0.1f, 0.1f));
        signBody.style.borderBottomColor = new StyleColor(new Color(0.1f, 0.1f, 0.1f));
        signBody.style.borderLeftColor = new StyleColor(new Color(0.1f, 0.1f, 0.1f));
        signBody.style.borderRightColor = new StyleColor(new Color(0.1f, 0.1f, 0.1f));
        signBody.style.borderTopLeftRadius = 20; signBody.style.borderTopRightRadius = 20;
        signBody.style.borderBottomLeftRadius = 20; signBody.style.borderBottomRightRadius = 20;
        signBody.style.justifyContent = Justify.Center;
        
        var signText = new Label("SALE ➡️");
        signText.style.fontSize = 65;
        signText.style.color = new StyleColor(new Color(0.9f, 0.15f, 0.15f)); // Bold Red
        signText.style.unityFontStyleAndWeight = FontStyle.Bold;
        signText.style.unityTextAlign = TextAnchor.MiddleCenter;
        signBody.Add(signText);
        
        timingMarker.Add(signBody);
    }

    public void StartGame()
    {
        isPlaying = true;
        timingTargetAngle = Random.Range(0f, 360f);
        timingMarkerAngle = 0f;
        timingSpinDir = 1;
        timingCurrentRound = 0;

        container.style.display = DisplayStyle.Flex;
        manager.StartCoroutine(TimingRingRoutine());
    }

    public void AbortGame()
    {
        isPlaying = false;
        container.style.display = DisplayStyle.None;
    }

    private IEnumerator TimingRingRoutine()
    {
        while (isPlaying && manager.isPlaying && timingCurrentRound < config.timingRounds)
        {
            if (timingRoundText != null) timingRoundText.text = $"Round {timingCurrentRound + 1} / {config.timingRounds}";
            
            float currentZoneDegrees = Mathf.Max(30f, 75f - (timingCurrentRound * 6f)); 
            
            if (arcMaskInner != null)
                arcMaskInner.style.rotate = new Rotate(Angle.Degrees(90f - currentZoneDegrees));

            if (timingTargetZone != null)
            {
                float centerOffset = 90f - currentZoneDegrees / 2f;
                timingTargetZone.style.rotate = new Rotate(Angle.Degrees(timingTargetAngle - centerOffset));
            }

            timingMarkerMoving = true;
            
            while (timingMarkerMoving && isPlaying && manager.isPlaying)
            {
                timingMarkerAngle += timingRingRotSpeed * timingSpinDir * Time.deltaTime;
                timingMarkerAngle %= 360f;
                if (timingMarkerAngle < 0f) timingMarkerAngle += 360f;
                
                if (timingMarker != null)
                    timingMarker.style.rotate = new Rotate(Angle.Degrees(timingMarkerAngle));
                    
                yield return null;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void OnTimingStrike()
    {
        if (!isPlaying || !manager.isPlaying || !timingMarkerMoving) return;
        
        float currentZoneDegrees = Mathf.Max(30f, 75f - (timingCurrentRound * 6f)); 
        float diff = Mathf.Abs(Mathf.DeltaAngle(timingMarkerAngle + 90f, timingTargetAngle));
        
        if (diff <= currentZoneDegrees / 2f)
        {
            timingMarkerMoving = false; // Stop briefly on hit for juice
            timingCurrentRound++;
            MiniGameFeedback.PlaySuccess(timingMarker, "PERFECT!", container);
            
            if (timingCurrentRound >= config.timingRounds)
            {
                isPlaying = false;
                manager.StartCoroutine(HitAuraRoutine(new Color(0.18f, 0.8f, 0.44f), true, true));
            }
            else
            {
                timingSpinDir *= -1;
                timingTargetAngle = Random.Range(0f, 360f);
                manager.StartCoroutine(HitAuraRoutine(new Color(0.18f, 0.8f, 0.44f), false, false));
            }
        }
        else
        {
            // MISS! We DO NOT stop the marker. It keeps spinning!
            MiniGameFeedback.PlayError(timingCircle, "MISS!", container);
            manager.StartCoroutine(HitAuraRoutine(new Color(0.9f, 0.16f, 0.16f), false, false)); 
        }
    }

    private IEnumerator HitAuraRoutine(Color glowColor, bool isEndGame, bool isWin)
    {
        if (timingGlow != null && glowLayers != null)
        {
            timingGlow.style.opacity = 1f;
            // Greatly expanded glow widths!
            float[] maxBorderWidths = { 6f, 12f, 20f, 32f, 48f, 70f }; 
            
            // Apply the new color with decaying opacity
            for(int i = 0; i < 6; i++)
            {
                if (glowLayers[i] == null) continue;
                float op = 1f - (i * 0.16f); // Decaying opacity outwards
                Color layerColor = new Color(glowColor.r, glowColor.g, glowColor.b, op);
                glowLayers[i].style.borderTopColor = new StyleColor(layerColor);
                glowLayers[i].style.borderBottomColor = new StyleColor(layerColor);
                glowLayers[i].style.borderLeftColor = new StyleColor(layerColor);
                glowLayers[i].style.borderRightColor = new StyleColor(layerColor);
            }
            
            // Arrow rapid spin effect on success, violent shake on fail
            float startAngle = timingMarkerAngle;
            
            float t = 0;
            float duration = 0.15f;
            while(t < duration)
            {
                t += Time.deltaTime;
                float progress = t / duration;
                
                // Explode the glow outwards
                for(int i = 0; i < 6; i++)
                {
                    if (glowLayers[i] == null) continue;
                    float currentWidth = Mathf.Lerp(0, maxBorderWidths[i], Mathf.Pow(progress, 0.5f)); // Fast pop!
                    glowLayers[i].style.borderTopWidth = currentWidth;
                    glowLayers[i].style.borderBottomWidth = currentWidth;
                    glowLayers[i].style.borderLeftWidth = currentWidth;
                    glowLayers[i].style.borderRightWidth = currentWidth;
                }
                
                // Visual Juice
                if (isWin) 
                {
                    // Success: Spin 360 rapidly!
                    if (timingMarker != null) timingMarker.style.rotate = new Rotate(Angle.Degrees(startAngle + (360f * progress * timingSpinDir)));
                }
                else
                {
                    // Fail: Shake!
                    if (timingCircle != null) timingCircle.style.translate = new Translate(new Length(Random.Range(-10,10), LengthUnit.Pixel), new Length(Random.Range(-10,10), LengthUnit.Pixel));
                }
                
                yield return null;
            }
            
            if (timingMarker != null && (!isEndGame || isWin)) timingMarker.style.rotate = new Rotate(Angle.Degrees(startAngle));
            if (timingCircle != null) timingCircle.style.translate = new Translate(0,0);
            
            if (!isEndGame && isWin) 
            {
                // Resume spinning ONLY on success (on fail it never stopped!)
                timingMarkerMoving = true;
            }
            
            t = 0;
            float fadeOut = isEndGame ? 0.8f : 0.4f; 
            while(t < fadeOut)
            {
                t += Time.deltaTime;
                float opacity = Mathf.Lerp(1f, 0f, t / fadeOut);
                timingGlow.style.opacity = opacity;
                yield return null;
            }
            
            timingGlow.style.opacity = 0f;
            
            if (isEndGame)
            {
                if (isWin) manager.MarkWinReady();
                else if (manager.currentGame.canFail) manager.AbortGame();
            }
        }
        else
        {
            // Fallback if UI is missing
            if (isEndGame)
            {
                if (isWin) manager.MarkWinReady();
                else if (manager.currentGame.canFail) manager.AbortGame();
            }
        }
    }
}

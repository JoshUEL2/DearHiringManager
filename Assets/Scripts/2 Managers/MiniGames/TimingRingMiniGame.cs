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
            
            float currentZoneDegrees = Mathf.Max(10f, 45f - (timingCurrentRound * 5f)); 
            
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
        timingMarkerMoving = false;
        
        float currentZoneDegrees = Mathf.Max(10f, 45f - (timingCurrentRound * 5f)); 
        float diff = Mathf.Abs(Mathf.DeltaAngle(timingMarkerAngle, timingTargetAngle));
        
        if (diff <= currentZoneDegrees / 2f)
        {
            timingCurrentRound++;
            manager.StartCoroutine(HitAuraRoutine());
            if (timingCurrentRound >= config.timingRounds)
            {
                isPlaying = false;
                manager.MarkWinReady();
            }
            else
            {
                timingSpinDir *= -1;
                timingTargetAngle = Random.Range(0f, 360f);
            }
        }
        else
        {
            if (manager.currentGame != null && manager.currentGame.canFail) 
            { 
                isPlaying = false;
                manager.AbortGame();
            }
        }
    }

    private IEnumerator HitAuraRoutine()
    {
        if (timingGlow != null && glowLayers != null)
        {
            timingGlow.style.opacity = 1f;
            float[] maxBorderWidths = { 2f, 4f, 7f, 10f, 14f, 20f }; 
            
            float t = 0;
            while(t < 0.1f)
            {
                t += Time.deltaTime;
                float progress = t / 0.1f;
                
                for(int i = 0; i < 6; i++)
                {
                    if (glowLayers[i] == null) continue;
                    float currentWidth = Mathf.Lerp(0, maxBorderWidths[i], progress);
                    glowLayers[i].style.borderTopWidth = currentWidth;
                    glowLayers[i].style.borderBottomWidth = currentWidth;
                    glowLayers[i].style.borderLeftWidth = currentWidth;
                    glowLayers[i].style.borderRightWidth = currentWidth;
                }
                yield return null;
            }
            
            for(int i = 0; i < 6; i++)
            {
                if (glowLayers[i] == null) continue;
                glowLayers[i].style.borderTopWidth = maxBorderWidths[i];
                glowLayers[i].style.borderBottomWidth = maxBorderWidths[i];
                glowLayers[i].style.borderLeftWidth = maxBorderWidths[i];
                glowLayers[i].style.borderRightWidth = maxBorderWidths[i];
            }
            
            t = 0;
            while(t < 0.4f)
            {
                t += Time.deltaTime;
                float opacity = Mathf.Lerp(1f, 0f, t / 0.4f);
                timingGlow.style.opacity = opacity;
                yield return null;
            }
            
            timingGlow.style.opacity = 0f;
        }
    }
}

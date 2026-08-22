using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class PumpSystemMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement container;

    private VisualElement pumpProgressBar;
    private VisualElement pumpNormalBar;
    private VisualElement pumpDialContainer;
    private VisualElement pumpDialTargetZone;
    private VisualElement pumpDialTargetArc;
    private VisualElement pumpDialNeedleRotator;
    private VisualElement pumpHoldTimerContainer;
    private VisualElement pumpHoldTimerBar;
    private VisualElement pumpHandle;
    private VisualElement pumpTrackArea;

    private float pumpProgress = 0f;
    private float pumpHoldTimer = 0f;
    private float pumpLastY = 0f;
    private bool isPlaying = false;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        
        container = root.Q<VisualElement>("PumpGameContainer");
        pumpProgressBar = container.Q<VisualElement>("PumpProgressBar");
        pumpNormalBar = container.Q<VisualElement>("PumpNormalBar");
        pumpDialContainer = container.Q<VisualElement>("PumpDialContainer");
        pumpDialTargetZone = container.Q<VisualElement>("PumpDialTargetZone");
        pumpDialTargetArc = container.Q<VisualElement>("PumpDialTargetArc");
        pumpDialNeedleRotator = container.Q<VisualElement>("PumpDialNeedleRotator");
        pumpHoldTimerContainer = container.Q<VisualElement>("PumpHoldTimerContainer");
        pumpHoldTimerBar = container.Q<VisualElement>("PumpHoldTimerBar");
        pumpHandle = container.Q<VisualElement>("PumpHandle");
        pumpTrackArea = container.Q<VisualElement>("PumpTrackArea");

        if (pumpTrackArea != null)
        {
            pumpTrackArea.RegisterCallback<PointerDownEvent>(ev => {
                if (!isPlaying || !manager.isPlaying) return;
                pumpLastY = ev.localPosition.y;
                pumpTrackArea.CapturePointer(ev.pointerId);
            });
            pumpTrackArea.RegisterCallback<PointerUpEvent>(ev => {
                if (pumpTrackArea.HasPointerCapture(ev.pointerId)) pumpTrackArea.ReleasePointer(ev.pointerId);
            });
            pumpTrackArea.RegisterCallback<PointerMoveEvent>(ev => {
                if (!isPlaying || !manager.isPlaying) return;
                if (pumpTrackArea.HasPointerCapture(ev.pointerId))
                {
                    float dy = ev.localPosition.y - pumpLastY;
                    pumpLastY = ev.localPosition.y;

                    if (dy < 0) 
                    {
                        pumpProgress += -dy * 0.3f; 
                    }

                    float hY = Mathf.Clamp(ev.localPosition.y, 0, pumpTrackArea.resolvedStyle.height - 40f);
                    if (pumpHandle != null) pumpHandle.style.top = hY;
                }
            });
        }
    }

    public void StartGame()
    {
        isPlaying = true;
        pumpProgress = 0f;
        pumpHoldTimer = 0f;
        
        if (pumpProgressBar != null) pumpProgressBar.style.height = Length.Percent(0f);
        if (pumpHandle != null) pumpHandle.style.top = 0f;

        if (config.pumpIsBalanceMode)
        {
            if (pumpDialContainer != null) pumpDialContainer.style.display = DisplayStyle.Flex;
            if (pumpNormalBar != null) pumpNormalBar.style.display = DisplayStyle.None;
            if (pumpHoldTimerContainer != null) pumpHoldTimerContainer.style.display = DisplayStyle.Flex;
            if (pumpHoldTimerBar != null) pumpHoldTimerBar.style.width = Length.Percent(0f);

            float totalArc = 240f; 
            float arcWidth = (config.pumpBalanceTargetMax - config.pumpBalanceTargetMin) * totalArc;
            float startAngle = -120f + (config.pumpBalanceTargetMin * totalArc);

            if (pumpDialTargetZone != null) pumpDialTargetZone.style.rotate = new StyleRotate(new Rotate(new Angle(startAngle + arcWidth / 2f)));
            if (pumpDialTargetArc != null) pumpDialTargetArc.style.rotate = new StyleRotate(new Rotate(new Angle(90f - (arcWidth / 2f))));
        }
        else
        {
            if (pumpDialContainer != null) pumpDialContainer.style.display = DisplayStyle.None;
            if (pumpNormalBar != null) pumpNormalBar.style.display = DisplayStyle.Flex;
            if (pumpHoldTimerContainer != null) pumpHoldTimerContainer.style.display = DisplayStyle.None;
        }

        container.style.display = DisplayStyle.Flex;
        manager.StartCoroutine(PumpSystemRoutine());
    }

    public void AbortGame()
    {
        isPlaying = false;
        container.style.display = DisplayStyle.None;
    }

    private IEnumerator PumpSystemRoutine()
    {
        float decayRate = 15f; 
        
        while (isPlaying && manager.isPlaying)
        {
            if (config.pumpIsBalanceMode)
            {
                pumpProgress -= decayRate * Time.deltaTime;
                pumpProgress = Mathf.Clamp(pumpProgress, 0f, 100f);
                
                float needleAngle = -120f + ((pumpProgress / 100f) * 240f);
                if (pumpDialNeedleRotator != null) pumpDialNeedleRotator.style.rotate = new StyleRotate(new Rotate(new Angle(needleAngle)));

                float p = pumpProgress / 100f;
                if (p >= config.pumpBalanceTargetMin && p <= config.pumpBalanceTargetMax)
                {
                    pumpHoldTimer += Time.deltaTime;
                }
                else
                {
                    pumpHoldTimer -= Time.deltaTime * 0.5f; 
                    pumpHoldTimer = Mathf.Max(0f, pumpHoldTimer);
                }

                if (pumpHoldTimerBar != null) pumpHoldTimerBar.style.width = Length.Percent((pumpHoldTimer / config.pumpBalanceHoldTime) * 100f);

                if (pumpHoldTimer >= config.pumpBalanceHoldTime)
                {
                    if (pumpHoldTimerBar != null) pumpHoldTimerBar.style.width = Length.Percent(100f);
                    isPlaying = false;
                    manager.MarkWinReady();
                    yield break;
                }
            }
            else
            {
                float decayFactor = 1f + (pumpProgress / 100f) * 4f; 
                
                pumpProgress -= decayRate * decayFactor * Time.deltaTime;
                pumpProgress = Mathf.Clamp(pumpProgress, 0f, 100f);
                
                if (pumpProgressBar != null) pumpProgressBar.style.height = Length.Percent(pumpProgress);

                if (pumpProgress >= 100f)
                {
                    if (pumpProgressBar != null) pumpProgressBar.style.height = Length.Percent(100f);
                    isPlaying = false;
                    manager.MarkWinReady();
                    yield break;
                }
            }
            yield return null;
        }
    }
}

path = 'Assets/Scripts/2 Managers/MiniGames/PumpSystemMiniGame.cs'

code = '''using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class PumpSystemMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement container;

    private VisualElement pumpNormalBar;
    private VisualElement pumpDialContainer;
    private VisualElement pumpDialNeedleRotator;
    private VisualElement pumpHoldTimerContainer;
    private VisualElement pumpHoldTimerBar;
    private VisualElement pumpHandle;
    private VisualElement pumpTrackArea;

    private class GaugeElement : VisualElement
    {
        public float minAngle = 150f;
        public float maxAngle = 390f;
        public Color arcColor = Color.green;
        public float arcThickness = 16f;

        public GaugeElement()
        {
            generateVisualContent += OnGenerateVisualContent;
        }

        private void OnGenerateVisualContent(MeshGenerationContext ctx)
        {
            var painter = ctx.painter2D;
            if (painter == null) return;
            
            float radius = (Mathf.Min(layout.width, layout.height) / 2f) - (arcThickness / 2f);
            Vector2 center = new Vector2(layout.width / 2f, layout.height / 2f);
            
            painter.strokeColor = arcColor;
            painter.lineWidth = arcThickness;
            painter.lineCap = LineCap.Round;
            
            painter.BeginPath();
            painter.Arc(center, radius, minAngle, maxAngle, ArcDirection.Clockwise);
            painter.Stroke();
        }
    }
    private GaugeElement gaugeElement;

    private float pumpProgress = 0f;
    private float pumpHoldTimer = 0f;
    private float pumpLastY = 0f;
    private bool isPlaying = false;
    
    private float dynamicTargetMin;
    private float dynamicTargetMax;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        
        container = root.Q<VisualElement>("PumpGameContainer");
        pumpNormalBar = container.Q<VisualElement>("PumpNormalBar");
        pumpDialContainer = container.Q<VisualElement>("PumpDialContainer");
        pumpHoldTimerContainer = container.Q<VisualElement>("PumpHoldTimerContainer");
        pumpHoldTimerBar = container.Q<VisualElement>("PumpHoldTimerBar");
        pumpHandle = container.Q<VisualElement>("PumpHandle");
        pumpTrackArea = container.Q<VisualElement>("PumpTrackArea");
        pumpDialNeedleRotator = container.Q<VisualElement>("PumpDialNeedleRotator");

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

                    // User requested pump to only increase when moving DOWN!
                    if (dy > 0) 
                    {
                        pumpProgress += dy * 0.4f; // Give it a nice kick
                    }

                    float hY = Mathf.Clamp(ev.localPosition.y, 0, pumpTrackArea.resolvedStyle.height - 40f);
                    if (pumpHandle != null) pumpHandle.style.top = hY;
                }
            });
        }
        
        // Hide unused elements
        if (pumpNormalBar != null) pumpNormalBar.style.display = DisplayStyle.None;
        
        // Inject vector gauge into Dial Container
        if (pumpDialContainer != null)
        {
            pumpDialContainer.style.display = DisplayStyle.Flex;
            
            // Hide the old hacky target zone
            var oldTargetZone = container.Q<VisualElement>("PumpDialTargetZone");
            if (oldTargetZone != null) oldTargetZone.style.display = DisplayStyle.None;
            
            if (gaugeElement == null)
            {
                gaugeElement = new GaugeElement();
                gaugeElement.style.position = Position.Absolute;
                gaugeElement.style.width = Length.Percent(100);
                gaugeElement.style.height = Length.Percent(100);
                // Insert behind the needle (index 0 usually)
                pumpDialContainer.Insert(0, gaugeElement);
            }
        }
    }

    public void StartGame()
    {
        isPlaying = true;
        pumpProgress = 0f;
        pumpHoldTimer = 0f;
        
        if (pumpHandle != null) pumpHandle.style.top = 0f;

        if (pumpHoldTimerContainer != null) pumpHoldTimerContainer.style.display = DisplayStyle.Flex;
        if (pumpHoldTimerBar != null) pumpHoldTimerBar.style.width = Length.Percent(0f);

        // RANDOMIZE TARGET ZONE
        // Convert to percentage (0 to 100)
        float zoneSizePct = Random.Range(15f, 40f); 
        float startPct = Random.Range(10f, 95f - zoneSizePct);
        
        dynamicTargetMin = startPct;
        dynamicTargetMax = startPct + zoneSizePct;

        // Apply to Gauge Vector
        if (gaugeElement != null)
        {
            gaugeElement.minAngle = 150f + (startPct / 100f * 240f);
            gaugeElement.maxAngle = 150f + ((startPct + zoneSizePct) / 100f * 240f);
            gaugeElement.arcColor = new Color(0.18f, 0.8f, 0.44f); // Green
            gaugeElement.MarkDirtyRepaint();
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
        float decayRate = 35f; 
        
        while (isPlaying && manager.isPlaying)
        {
            pumpProgress -= decayRate * Time.deltaTime;
            pumpProgress = Mathf.Clamp(pumpProgress, 0f, 100f);
            
            // Rotate Needle
            if (pumpDialNeedleRotator != null)
            {
                // Smooth needle lerp
                float targetAngle = -120f + ((pumpProgress / 100f) * 240f);
                float currentAngle = pumpDialNeedleRotator.style.rotate.value.angle.value;
                float newAngle = Mathf.Lerp(currentAngle, targetAngle, Time.deltaTime * 20f);
                pumpDialNeedleRotator.style.rotate = new StyleRotate(new Rotate(new Angle(newAngle, AngleUnit.Degree)));
            }

            // Check if inside target zone
            if (pumpProgress >= dynamicTargetMin && pumpProgress <= dynamicTargetMax)
            {
                pumpHoldTimer += Time.deltaTime;
                
                // Visual Juice: Pulse gauge color
                if (gaugeElement != null)
                {
                    float pulse = 0.6f + (Mathf.Sin(Time.time * 20f) * 0.4f);
                    gaugeElement.arcColor = new Color(0.18f, 0.8f * pulse, 0.44f * pulse, 1f);
                    gaugeElement.MarkDirtyRepaint();
                }
            }
            else
            {
                pumpHoldTimer -= Time.deltaTime * 0.8f; 
                pumpHoldTimer = Mathf.Max(0f, pumpHoldTimer);
                
                if (gaugeElement != null)
                {
                    gaugeElement.arcColor = new Color(0.18f, 0.8f, 0.44f, 1f); // Reset color
                    gaugeElement.MarkDirtyRepaint();
                }
            }

            if (pumpHoldTimerBar != null) pumpHoldTimerBar.style.width = Length.Percent((pumpHoldTimer / config.pumpBalanceHoldTime) * 100f);

            if (pumpHoldTimer >= config.pumpBalanceHoldTime)
            {
                if (pumpHoldTimerBar != null) pumpHoldTimerBar.style.width = Length.Percent(100f);
                if (gaugeElement != null) { gaugeElement.arcColor = new Color(0.18f, 0.8f, 0.44f, 1f); gaugeElement.MarkDirtyRepaint(); }
                isPlaying = false;
                manager.MarkWinReady();
                yield break;
            }
            yield return null;
        }
    }
}
'''

with open(path, 'w') as f:
    f.write(code)

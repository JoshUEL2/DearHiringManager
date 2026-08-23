using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class PumpSystemMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement container;
    private VisualElement rootContainer;

    private VisualElement pumpNormalBar;
    private VisualElement pumpProgressBar;
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
        
        rootContainer = root;
        container = root.Q<VisualElement>("PumpGameContainer");
        pumpNormalBar = container.Q<VisualElement>("PumpNormalBar");
        pumpProgressBar = container.Q<VisualElement>("PumpProgressBar");
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

                    // Pumping direction depends on the game mode!
                    if (config.pumpIsBalanceMode)
                    {
                        if (dy > 0) pumpProgress += dy * 0.4f; // Downward stroke
                    }
                    else
                    {
                        if (dy < 0) pumpProgress += -dy * 0.4f; // Upward stroke
                    }

                    float hY = Mathf.Clamp(ev.localPosition.y, 0, pumpTrackArea.resolvedStyle.height - 40f);
                    if (pumpHandle != null) pumpHandle.style.top = hY;
                }
            });
        }
        
        // Hide the old hacky target zone
        var oldTargetZone = container.Q<VisualElement>("PumpDialTargetZone");
        if (oldTargetZone != null) oldTargetZone.style.display = DisplayStyle.None;
            
        // Scale everything up massively as requested
        if (pumpDialContainer != null)
        {
            pumpDialContainer.style.width = 320;
            pumpDialContainer.style.height = 320;
            pumpDialContainer.style.borderTopLeftRadius = 160;
            pumpDialContainer.style.borderTopRightRadius = 160;
            pumpDialContainer.style.borderBottomLeftRadius = 160;
            pumpDialContainer.style.borderBottomRightRadius = 160;
            pumpDialContainer.style.marginRight = 50;
            
            if (pumpDialNeedleRotator != null && pumpDialNeedleRotator.childCount > 0)
            {
                var needle = pumpDialNeedleRotator.ElementAt(0);
                needle.style.height = 140;
                needle.style.top = 20;
                needle.style.width = 8;
            }
            
            var oilLabel = pumpDialContainer.Q<Label>();
            if (oilLabel != null)
            {
                oilLabel.style.fontSize = 20;
                oilLabel.style.bottom = 45;
            }
            
            if (gaugeElement == null)
            {
                gaugeElement = new GaugeElement();
                gaugeElement.style.position = Position.Absolute;
                gaugeElement.style.width = Length.Percent(100);
                gaugeElement.style.height = Length.Percent(100);
                pumpDialContainer.Insert(0, gaugeElement);
            }
        }
        
        if (pumpNormalBar != null)
        {
            pumpNormalBar.style.width = 100;
            pumpNormalBar.style.marginRight = 50;
            pumpNormalBar.style.borderTopLeftRadius = 15;
            pumpNormalBar.style.borderTopRightRadius = 15;
            pumpNormalBar.style.borderBottomLeftRadius = 15;
            pumpNormalBar.style.borderBottomRightRadius = 15;
        }

        if (pumpTrackArea != null)
        {
            pumpTrackArea.style.width = 120;
            pumpTrackArea.style.borderTopLeftRadius = 60;
            pumpTrackArea.style.borderTopRightRadius = 60;
            pumpTrackArea.style.borderBottomLeftRadius = 60;
            pumpTrackArea.style.borderBottomRightRadius = 60;
            pumpTrackArea.style.marginRight = 40; // Add padding so the handle doesn't clip on the popup edge!
        }
        
        if (pumpHandle != null)
        {
            pumpHandle.style.width = 160;
            pumpHandle.style.height = 60;
            pumpHandle.style.borderTopLeftRadius = 15;
            pumpHandle.style.borderTopRightRadius = 15;
            pumpHandle.style.borderBottomLeftRadius = 15;
            pumpHandle.style.borderBottomRightRadius = 15;
        }
        
        if (pumpTrackArea != null)
        {
            var shaft = pumpTrackArea.Q<VisualElement>("PumpShaft");
            if (shaft != null) shaft.style.width = 30;
        }
    }

    public void StartGame()
    {
        isPlaying = true;
        pumpProgress = 0f;
        pumpHoldTimer = 0f;
        
        if (pumpHandle != null) pumpHandle.style.top = 0f;

        if (config.pumpIsBalanceMode)
        {
            if (pumpDialContainer != null) pumpDialContainer.style.display = DisplayStyle.Flex;
            if (pumpNormalBar != null) pumpNormalBar.style.display = DisplayStyle.None;
            if (pumpHoldTimerContainer != null) pumpHoldTimerContainer.style.display = DisplayStyle.Flex;
            if (pumpHoldTimerBar != null) pumpHoldTimerBar.style.width = Length.Percent(0f);

            // RANDOMIZE TARGET ZONE
            float zoneSizePct = Random.Range(10f, 25f); 
            float startPct = Random.Range(10f, 95f - zoneSizePct);
            
            dynamicTargetMin = startPct;
            dynamicTargetMax = startPct + zoneSizePct;

            if (gaugeElement != null)
            {
                gaugeElement.minAngle = 150f + (startPct / 100f * 240f);
                gaugeElement.maxAngle = 150f + ((startPct + zoneSizePct) / 100f * 240f);
                gaugeElement.arcColor = new Color(0.18f, 0.8f, 0.44f); 
                gaugeElement.MarkDirtyRepaint();
            }
        }
        else
        {
            if (pumpDialContainer != null) pumpDialContainer.style.display = DisplayStyle.None;
            if (pumpNormalBar != null) pumpNormalBar.style.display = DisplayStyle.Flex;
            if (pumpHoldTimerContainer != null) pumpHoldTimerContainer.style.display = DisplayStyle.None;
            if (pumpProgressBar != null) pumpProgressBar.style.height = Length.Percent(0f);
        }

        container.style.display = DisplayStyle.Flex;
        manager.StartCoroutine(PumpSystemRoutine());
    }

    public void AbortGame()
    {
        isPlaying = false;
        container.style.display = DisplayStyle.None;
    }

    private IEnumerator ShowWinFeedbackAndClose()
    {
        Label stampText = new Label("PERFECT TEMP!");
        stampText.style.position = Position.Absolute;
        stampText.style.color = new StyleColor(new Color(0.18f, 0.8f, 0.44f)); // Green
        stampText.style.fontSize = 72;
        stampText.style.unityFontStyleAndWeight = FontStyle.Bold;
        stampText.style.unityTextAlign = TextAnchor.MiddleCenter;
        
        stampText.style.unityTextOutlineColor = Color.black;
        stampText.style.unityTextOutlineWidth = 4f;
        
        stampText.style.top = new Length(50, LengthUnit.Percent);
        stampText.style.left = new Length(50, LengthUnit.Percent);
        stampText.style.translate = new Translate(new Length(-50, LengthUnit.Percent), new Length(-50, LengthUnit.Percent));
        
        manager.miniGameDocument.rootVisualElement.Add(stampText);
        
        float life = 0.8f;
        float elapsed = 0f;
        while(elapsed < life)
        {
            elapsed += Time.deltaTime;
            float p = elapsed / life;
            
            float scale = Mathf.Lerp(2.5f, 1f, p * 10f);
            if (scale < 1f) scale = 1f;
            stampText.style.scale = new StyleScale(new Vector2(scale, scale));
            
            if (p > 0.8f) {
                stampText.style.opacity = 1f - ((p - 0.8f) * 5f);
            }
            
            yield return null;
        }
        
        if (manager.miniGameDocument.rootVisualElement.Contains(stampText)) manager.miniGameDocument.rootVisualElement.Remove(stampText);
        manager.MarkWinReady();
    }

    private IEnumerator PumpSystemRoutine()
    {
        float decayRate = 35f; 
        
        while (isPlaying && manager.isPlaying)
        {
            if (config.pumpIsBalanceMode)
            {
                pumpProgress -= decayRate * Time.deltaTime;
                pumpProgress = Mathf.Clamp(pumpProgress, 0f, 100f);
                
                if (pumpDialNeedleRotator != null)
                {
                    float targetAngle = -120f + ((pumpProgress / 100f) * 240f);
                    float currentAngle = pumpDialNeedleRotator.style.rotate.value.angle.value;
                    float newAngle = Mathf.Lerp(currentAngle, targetAngle, Time.deltaTime * 20f);
                    pumpDialNeedleRotator.style.rotate = new StyleRotate(new Rotate(new Angle(newAngle, AngleUnit.Degree)));
                }

                if (pumpProgress >= dynamicTargetMin && pumpProgress <= dynamicTargetMax)
                {
                    pumpHoldTimer += Time.deltaTime;
                    
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
                        gaugeElement.arcColor = new Color(0.18f, 0.8f, 0.44f, 1f); 
                        gaugeElement.MarkDirtyRepaint();
                    }
                }

                if (pumpHoldTimerBar != null) pumpHoldTimerBar.style.width = Length.Percent((pumpHoldTimer / config.pumpBalanceHoldTime) * 100f);

                if (pumpHoldTimer >= config.pumpBalanceHoldTime)
                {
                    if (pumpHoldTimerBar != null) pumpHoldTimerBar.style.width = Length.Percent(100f);
                    if (gaugeElement != null) { gaugeElement.arcColor = new Color(0.18f, 0.8f, 0.44f, 1f); gaugeElement.MarkDirtyRepaint(); }
                    isPlaying = false;
                    manager.StartCoroutine(ShowWinFeedbackAndClose());
                    yield break;
                }
            }
            else
            {
                // Normal Mode (e.g., Pump Up a tyre or something)
                float decayFactor = 1f + (pumpProgress / 100f) * 2f; 
                pumpProgress -= (decayRate * 0.5f) * decayFactor * Time.deltaTime;
                pumpProgress = Mathf.Clamp(pumpProgress, 0f, 100f);
                
                if (pumpProgressBar != null) pumpProgressBar.style.height = Length.Percent(pumpProgress);

                if (pumpProgress >= 100f)
                {
                    if (pumpProgressBar != null) pumpProgressBar.style.height = Length.Percent(100f);
                    isPlaying = false;
                    manager.StartCoroutine(ShowWinFeedbackAndClose());
                    yield break;
                }
            }
            yield return null;
        }
    }
}

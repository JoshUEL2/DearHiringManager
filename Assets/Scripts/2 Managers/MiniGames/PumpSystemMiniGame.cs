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
    
    private float bubbleTime = 0f;
    private float dynamicTargetMin;
    private float dynamicTargetMax;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        bool isToilet = config != null && config.instructions.ToLower().Contains("toilet");
        
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
                        if (dy > 0) pumpProgress += dy * 0.15f; // Downward stroke
                    }
                    else
                    {
                        if (dy > 0) pumpProgress += dy * 0.08f; // Downward stroke for normal too!
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
        
        if (pumpNormalBar != null) {
            pumpNormalBar.style.width = isToilet ? 350 : 160;
            pumpNormalBar.style.backgroundColor = isToilet ? new StyleColor(Color.clear) : new StyleColor(new Color(0.05f, 0.1f, 0.05f, 0.8f));
            if (isToilet) {
                pumpNormalBar.style.borderTopWidth = 0; pumpNormalBar.style.borderBottomWidth = 0;
                pumpNormalBar.style.borderLeftWidth = 0; pumpNormalBar.style.borderRightWidth = 0;
            } else {
                pumpNormalBar.style.borderTopWidth = 6; pumpNormalBar.style.borderBottomWidth = 6;
                pumpNormalBar.style.borderLeftWidth = 6; pumpNormalBar.style.borderRightWidth = 6;
            }
            pumpNormalBar.style.borderTopColor = new StyleColor(new Color(0.4f, 0.4f, 0.4f));
            pumpNormalBar.style.borderBottomColor = new StyleColor(new Color(0.4f, 0.4f, 0.4f));
            pumpNormalBar.style.borderLeftColor = new StyleColor(new Color(0.4f, 0.4f, 0.4f));
            pumpNormalBar.style.borderRightColor = new StyleColor(new Color(0.4f, 0.4f, 0.4f));
            
            pumpNormalBar.generateVisualContent -= OnDrawGreaseVat;
            pumpNormalBar.generateVisualContent += OnDrawGreaseVat;
            
            if (pumpProgressBar != null) {
                pumpProgressBar.style.display = DisplayStyle.None;
            }
        }

        if (pumpTrackArea != null)
        {
            pumpTrackArea.style.borderTopWidth = 6; pumpTrackArea.style.borderBottomWidth = 6;
            pumpTrackArea.style.borderLeftWidth = 6; pumpTrackArea.style.borderRightWidth = 6;
            pumpTrackArea.style.borderTopColor = new StyleColor(new Color(0.4f, 0.4f, 0.4f));
            pumpTrackArea.style.borderBottomColor = new StyleColor(new Color(0.4f, 0.4f, 0.4f));
            pumpTrackArea.style.borderLeftColor = new StyleColor(new Color(0.4f, 0.4f, 0.4f));
            pumpTrackArea.style.borderRightColor = new StyleColor(new Color(0.4f, 0.4f, 0.4f));
            
            pumpTrackArea.generateVisualContent -= OnDrawPumpTrack;
            pumpTrackArea.generateVisualContent += OnDrawPumpTrack;

            var shaft = pumpTrackArea.Q<VisualElement>("PumpShaft");
            if (shaft != null) {
                shaft.style.width = isToilet ? 16 : 30;
                shaft.style.backgroundColor = isToilet ? new StyleColor(new Color(0.7f, 0.5f, 0.3f)) : new StyleColor(new Color(0.6f, 0.6f, 0.6f));
                shaft.style.left = StyleKeyword.Null;
                shaft.style.translate = StyleKeyword.Null;
            }
            if (pumpHandle != null) {
                if (isToilet) {
                    pumpHandle.style.backgroundColor = new StyleColor(new Color(0.8f, 0.2f, 0.2f));
                    pumpHandle.style.borderTopWidth = 0; pumpHandle.style.borderBottomWidth = 0;
                    pumpHandle.style.borderLeftWidth = 0; pumpHandle.style.borderRightWidth = 0;
                    pumpHandle.style.borderTopLeftRadius = 25;
                    pumpHandle.style.borderTopRightRadius = 25;
                    pumpHandle.style.borderBottomLeftRadius = 5;
                    pumpHandle.style.borderBottomRightRadius = 5;
                    pumpHandle.style.height = 40;
                    pumpHandle.style.width = 70;
                    pumpHandle.style.left = StyleKeyword.Null;
                    pumpHandle.style.translate = StyleKeyword.Null;
                } else {
                    pumpHandle.style.backgroundColor = new StyleColor(new Color(0.9f, 0.2f, 0.2f));
                    pumpHandle.style.borderTopWidth = 4; pumpHandle.style.borderBottomWidth = 4;
                    pumpHandle.style.borderLeftWidth = 4; pumpHandle.style.borderRightWidth = 4;
                    pumpHandle.style.borderTopColor = new StyleColor(new Color(1f, 0.4f, 0.4f));
                    pumpHandle.style.borderBottomColor = new StyleColor(new Color(0.5f, 0.1f, 0.1f));
                }
            }
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
            
            bool isToilet = config.instructions.ToLower().Contains("toilet");
            if (pumpNormalBar != null) pumpNormalBar.style.display = DisplayStyle.Flex;
            
            if (pumpHoldTimerContainer != null) pumpHoldTimerContainer.style.display = DisplayStyle.None;
            if (pumpProgressBar != null) pumpProgressBar.style.height = Length.Percent(0f);
            

        }

        container.style.display = DisplayStyle.Flex;
        manager.StartCoroutine(PumpSystemRoutine());
    }

    private void OnDrawGreaseVat(MeshGenerationContext ctx)
    {
        var p = ctx.painter2D;
        float w = ctx.visualElement.layout.width;
        float h = ctx.visualElement.layout.height;
        if (w == 0 || h == 0) return;

        bool isToilet = config != null && config.instructions.ToLower().Contains("toilet");

        if (isToilet)
        {
            // TOILET BODY SHADOW
            p.fillColor = new Color(0.1f, 0.1f, 0.1f, 0.2f);
            p.BeginPath(); 
            p.MoveTo(new Vector2(35, 25)); p.LineTo(new Vector2(115, 25));
            p.LineTo(new Vector2(115, 155)); p.LineTo(new Vector2(325, 155)); 
            p.BezierCurveTo(new Vector2(325, 185), new Vector2(335, 225), new Vector2(285, 265));
            p.BezierCurveTo(new Vector2(255, 285), new Vector2(225, 305), new Vector2(225, 355)); 
            p.LineTo(new Vector2(105, 355)); p.LineTo(new Vector2(105, 325));
            p.BezierCurveTo(new Vector2(105, 265), new Vector2(65, 265), new Vector2(65, 205));
            p.BezierCurveTo(new Vector2(65, 185), new Vector2(115, 185), new Vector2(115, 155));
            p.LineTo(new Vector2(35, 155));
            p.Fill();

            // TOILET BODY (White Porcelain)
            p.fillColor = new Color(0.95f, 0.95f, 0.95f);
            p.BeginPath(); p.MoveTo(new Vector2(30, 20)); p.LineTo(new Vector2(110, 20));
            p.LineTo(new Vector2(110, 150)); p.LineTo(new Vector2(30, 150)); p.Fill();
            
            p.BeginPath(); 
            p.MoveTo(new Vector2(110, 150)); p.LineTo(new Vector2(320, 150)); 
            p.BezierCurveTo(new Vector2(320, 180), new Vector2(330, 220), new Vector2(280, 260)); 
            p.BezierCurveTo(new Vector2(250, 280), new Vector2(220, 300), new Vector2(220, 350)); 
            p.LineTo(new Vector2(100, 350)); p.LineTo(new Vector2(100, 320)); 
            p.BezierCurveTo(new Vector2(100, 260), new Vector2(60, 260), new Vector2(60, 200)); 
            p.BezierCurveTo(new Vector2(60, 180), new Vector2(110, 180), new Vector2(110, 150)); 
            p.Fill();

            // TOILET BODY SHADING
            p.fillColor = new Color(0.85f, 0.85f, 0.85f);
            p.BeginPath(); p.MoveTo(new Vector2(90, 345)); p.LineTo(new Vector2(230, 345));
            p.LineTo(new Vector2(235, 355)); p.LineTo(new Vector2(85, 355)); p.Fill();
            
            p.fillColor = new Color(0.85f, 0.85f, 0.85f);
            p.BeginPath();
            p.MoveTo(new Vector2(100, 350)); p.LineTo(new Vector2(100, 320));
            p.BezierCurveTo(new Vector2(100, 260), new Vector2(60, 260), new Vector2(60, 200)); 
            p.BezierCurveTo(new Vector2(60, 180), new Vector2(110, 180), new Vector2(110, 150)); 
            p.LineTo(new Vector2(130, 150)); 
            p.BezierCurveTo(new Vector2(130, 180), new Vector2(80, 180), new Vector2(80, 200));
            p.BezierCurveTo(new Vector2(80, 260), new Vector2(120, 260), new Vector2(120, 320));
            p.LineTo(new Vector2(120, 350));
            p.Fill();

            // INNER BOWL CUTOUT
            p.fillColor = new Color(0.7f, 0.7f, 0.7f);
            p.BeginPath(); 
            p.MoveTo(new Vector2(130, 150)); p.LineTo(new Vector2(300, 150));
            p.BezierCurveTo(new Vector2(300, 220), new Vector2(250, 270), new Vector2(180, 280)); 
            p.BezierCurveTo(new Vector2(140, 260), new Vector2(130, 200), new Vector2(130, 150));
            p.Fill();
            
            p.fillColor = new Color(0.6f, 0.6f, 0.6f);
            p.BeginPath(); 
            p.MoveTo(new Vector2(150, 180)); p.LineTo(new Vector2(280, 180));
            p.BezierCurveTo(new Vector2(280, 220), new Vector2(240, 260), new Vector2(180, 280)); 
            p.BezierCurveTo(new Vector2(160, 260), new Vector2(150, 220), new Vector2(150, 180));
            p.Fill();

            // U-BEND PIPE (3 layers for 3D cross-section effect)
            System.Action<float> DrawPipePath = (float thickness) => {
                p.lineWidth = thickness;
                p.lineCap = LineCap.Round;
                p.lineJoin = LineJoin.Round;
                p.BeginPath();
                p.MoveTo(new Vector2(180, 280)); 
                p.BezierCurveTo(new Vector2(180, 350), new Vector2(60, 350), new Vector2(60, 280)); 
                p.BezierCurveTo(new Vector2(60, 220), new Vector2(120, 220), new Vector2(120, 280)); 
                p.LineTo(new Vector2(120, 350)); 
                p.Stroke();
            };
            
            p.strokeColor = new Color(0.85f, 0.85f, 0.85f);
            DrawPipePath(65f); // Outer wall
            p.strokeColor = new Color(0.3f, 0.3f, 0.3f);
            DrawPipePath(45f); // Inner shadow
            p.strokeColor = new Color(0.1f, 0.1f, 0.15f);
            DrawPipePath(35f); // Dark hollow void

            // DETAILS (Seat, Lid, Flusher)
            p.fillColor = new Color(1f, 1f, 1f);
            p.BeginPath(); p.MoveTo(new Vector2(25, 15)); p.LineTo(new Vector2(115, 15)); 
            p.LineTo(new Vector2(115, 25)); p.LineTo(new Vector2(25, 25)); p.Fill();
            p.fillColor = new Color(0.8f, 0.8f, 0.8f);
            p.BeginPath(); p.MoveTo(new Vector2(25, 25)); p.LineTo(new Vector2(115, 25)); 
            p.LineTo(new Vector2(115, 30)); p.LineTo(new Vector2(25, 30)); p.Fill();

            p.fillColor = new Color(0.7f, 0.7f, 0.75f);
            p.BeginPath(); p.Arc(new Vector2(95, 45), 6, 0, 360); p.Fill();
            p.BeginPath(); p.MoveTo(new Vector2(95, 43)); p.LineTo(new Vector2(105, 43)); 
            p.LineTo(new Vector2(105, 47)); p.LineTo(new Vector2(95, 47)); p.Fill();
            
            p.fillColor = new Color(0.95f, 0.95f, 0.95f);
            p.BeginPath(); p.MoveTo(new Vector2(110, 140)); p.LineTo(new Vector2(325, 140)); 
            p.LineTo(new Vector2(325, 150)); p.LineTo(new Vector2(110, 150)); p.Fill();
            p.fillColor = new Color(0.8f, 0.8f, 0.8f);
            p.BeginPath(); p.MoveTo(new Vector2(110, 148)); p.LineTo(new Vector2(325, 148)); 
            p.LineTo(new Vector2(325, 150)); p.LineTo(new Vector2(110, 150)); p.Fill();

            // CUBIC BEZIER LOGIC FOR PERFECT CLOG TRACKING
            Vector2 CubicBezier(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t) {
                float u = 1 - t;
                float uu = u * u;
                float uuu = uu * u;
                float tt = t * t;
                float ttt = tt * t;
                return uuu * p0 + 3 * uu * t * p1 + 3 * u * tt * p2 + ttt * p3;
            }

            Vector2 GetPipePos(float t) {
                if (t < 0.4f) { 
                    return CubicBezier(new Vector2(180, 280), new Vector2(180, 350), new Vector2(60, 350), new Vector2(60, 280), t / 0.4f);
                } else if (t < 0.8f) { 
                    return CubicBezier(new Vector2(60, 280), new Vector2(60, 220), new Vector2(120, 220), new Vector2(120, 280), (t - 0.4f) / 0.4f);
                } else { 
                    return Vector2.Lerp(new Vector2(120, 280), new Vector2(120, 350), (t - 0.8f) / 0.2f);
                }
            }

            float clogHealth = 1f - (pumpProgress / 100f);
            if (clogHealth > 0)
            {
                float pipeT = pumpProgress / 100f;
                Vector2 clogStart = GetPipePos(Mathf.Clamp01(pipeT - 0.05f));
                Vector2 clogEnd = GetPipePos(Mathf.Clamp01(pipeT + 0.05f));

                p.strokeColor = new Color(0.4f, 0.5f, 0.3f, 0.8f);
                p.lineWidth = 35f;
                p.lineCap = LineCap.Round;
                p.BeginPath();
                p.MoveTo(new Vector2(180, 280));
                for (float pt = 0; pt <= pipeT - 0.05f; pt += 0.05f) {
                    p.LineTo(GetPipePos(pt));
                }
                p.LineTo(clogStart);
                p.Stroke();

                p.strokeColor = new Color(0.4f, 0.25f, 0.1f);
                p.lineWidth = 36f;
                p.BeginPath(); p.MoveTo(clogStart); p.LineTo(clogEnd); p.Stroke();

                p.fillColor = new Color(0.4f, 0.5f, 0.3f, 0.8f); 
                float waterTop = 170 + (90 * (pumpProgress / 100f)); 
                float slosh = Mathf.Sin(bubbleTime * 25f) * 12f * (pumpProgress / 100f);
                
                float wT = Mathf.Clamp01((waterTop - 150f) / 130f);
                Vector2 lPt = CubicBezier(new Vector2(130, 150), new Vector2(130, 200), new Vector2(140, 260), new Vector2(180, 280), wT);
                Vector2 rPt = CubicBezier(new Vector2(300, 150), new Vector2(300, 220), new Vector2(250, 270), new Vector2(180, 280), wT);
                
                p.BeginPath();
                p.MoveTo(new Vector2(lPt.x, lPt.y + slosh));
                p.LineTo(new Vector2(rPt.x, rPt.y - slosh));
                for (float t = wT; t <= 1f; t += 0.05f) {
                    p.LineTo(CubicBezier(new Vector2(300, 150), new Vector2(300, 220), new Vector2(250, 270), new Vector2(180, 280), t));
                }
                for (float t = 1f; t >= wT; t -= 0.05f) {
                    p.LineTo(CubicBezier(new Vector2(130, 150), new Vector2(130, 200), new Vector2(140, 260), new Vector2(180, 280), t));
                }
                p.LineTo(new Vector2(lPt.x, lPt.y + slosh));
                p.Fill();

                p.fillColor = new Color(0.35f, 0.2f, 0.05f);
                Random.State oldState = Random.state;
                Random.InitState(42);
                for(int i=0; i<15; i++) {
                    float bx = Random.Range(140f, 270f);
                    float by = Random.Range(waterTop + 10f, 260f);
                    by += Mathf.Sin(bubbleTime * 15f + bx) * 10f;
                    float bSize = Random.Range(6f, 15f);
                    p.BeginPath(); p.Arc(new Vector2(bx, by), bSize, 0, 360); p.Fill();
                }
                Random.state = oldState;
            }
            else
            {
                p.fillColor = new Color(0.2f, 0.6f, 0.9f, 0.8f);
                float fT = Mathf.Clamp01((260f - 150f) / 130f);
                Vector2 flPt = CubicBezier(new Vector2(130, 150), new Vector2(130, 200), new Vector2(140, 260), new Vector2(180, 280), fT);
                Vector2 frPt = CubicBezier(new Vector2(300, 150), new Vector2(300, 220), new Vector2(250, 270), new Vector2(180, 280), fT);
                
                p.BeginPath();
                p.MoveTo(flPt);
                p.LineTo(frPt);
                for (float t = fT; t <= 1f; t += 0.05f) {
                    p.LineTo(CubicBezier(new Vector2(300, 150), new Vector2(300, 220), new Vector2(250, 270), new Vector2(180, 280), t));
                }
                for (float t = 1f; t >= fT; t -= 0.05f) {
                    p.LineTo(CubicBezier(new Vector2(130, 150), new Vector2(130, 200), new Vector2(140, 260), new Vector2(180, 280), t));
                }
                p.Fill();
                
                p.strokeColor = new Color(0.2f, 0.6f, 0.9f, 0.8f);
                p.lineWidth = 35f;
                p.lineCap = LineCap.Round;
                p.lineJoin = LineJoin.Round;
                p.BeginPath();
                p.MoveTo(new Vector2(180, 280)); 
                p.BezierCurveTo(new Vector2(180, 350), new Vector2(60, 350), new Vector2(60, 280)); 
                p.BezierCurveTo(new Vector2(60, 220), new Vector2(120, 220), new Vector2(120, 280)); 
                p.LineTo(new Vector2(120, 350)); 
                p.Stroke();
                
                p.fillColor = Color.white;
                for(int i=0; i<30; i++) {
                    float t = (Time.time * 2.5f + i * 0.1f) % 1f;
                    Vector2 pos = GetPipePos(t);
                    p.BeginPath(); p.Arc(pos, 5f, 0, 360); p.Fill();
                }
            }
        }
        else
        {
            p.fillColor = new Color(0.05f, 0.1f, 0.05f, 0.8f);
            p.BeginPath(); p.MoveTo(new Vector2(0, 0)); p.LineTo(new Vector2(w, 0)); p.LineTo(new Vector2(w, h)); p.LineTo(new Vector2(0, h)); p.Fill();
            
            float greaseTopY = h * (pumpProgress / 100f);
            
            if (greaseTopY < h)
            {
                p.fillColor = new Color(0.35f, 0.25f, 0.1f);
                
                p.BeginPath();
                p.MoveTo(new Vector2(0, h));
                p.LineTo(new Vector2(w, h));
                p.LineTo(new Vector2(w, greaseTopY));
                
                int segments = 10;
                float segWidth = w / segments;
                for (int i = segments - 1; i >= 0; i--)
                {
                    float px = i * segWidth;
                    float py = greaseTopY + Mathf.Sin(bubbleTime * 4f + px * 0.1f) * 12f;
                    if (i == 0) p.LineTo(new Vector2(0, py));
                    else p.LineTo(new Vector2(px, py));
                }
                p.Fill();

                p.fillColor = new Color(0.45f, 0.35f, 0.15f, 0.6f);
                Random.State oldState = Random.state;
                Random.InitState(42);
                for(int i=0; i<20; i++) {
                    float bx = Random.Range(10f, w - 10f);
                    float bSpeed = Random.Range(30f, 80f);
                    float bubbleRange = h - greaseTopY;
                    if (bubbleRange > 5f) {
                        float by = h - ((bubbleTime * bSpeed + Random.Range(0, h)) % bubbleRange);
                        float bSize = Random.Range(4f, 15f);
                        p.BeginPath(); p.Arc(new Vector2(bx, by), bSize, 0, 360); p.Fill();
                    }
                }
                Random.state = oldState;
            }

            p.strokeColor = new Color(1f, 1f, 1f, 0.2f);
            p.lineWidth = 2f;
            for(float y = h * 0.2f; y < h; y += h * 0.2f) {
                p.BeginPath(); p.MoveTo(new Vector2(0, y)); p.LineTo(new Vector2(25, y)); p.Stroke();
                p.BeginPath(); p.MoveTo(new Vector2(w, y)); p.LineTo(new Vector2(w - 25, y)); p.Stroke();
            }
            
            p.fillColor = new Color(1f, 1f, 1f, 0.1f);
            p.BeginPath(); p.MoveTo(new Vector2(5, 5)); p.LineTo(new Vector2(20, 5)); p.LineTo(new Vector2(20, h-5)); p.LineTo(new Vector2(5, h-5)); p.Fill();
        }
    }

    private void OnDrawPumpTrack(MeshGenerationContext ctx)
    {
        var p = ctx.painter2D;
        float w = ctx.visualElement.layout.width;
        float h = ctx.visualElement.layout.height;
        if (w == 0 || h == 0) return;

        p.fillColor = new Color(0.12f, 0.14f, 0.16f);
        p.BeginPath(); p.MoveTo(new Vector2(0, 0)); p.LineTo(new Vector2(w, 0)); p.LineTo(new Vector2(w, h)); p.LineTo(new Vector2(0, h)); p.Fill();

        p.fillColor = new Color(0.9f, 0.7f, 0.1f, 0.3f);
        for(float y = -50; y < h; y += 40) {
            p.BeginPath(); p.MoveTo(new Vector2(0, y)); p.LineTo(new Vector2(15, y)); p.LineTo(new Vector2(15, y+20)); p.LineTo(new Vector2(0, y+20)); p.Fill();
            p.BeginPath(); p.MoveTo(new Vector2(w-15, y)); p.LineTo(new Vector2(w, y)); p.LineTo(new Vector2(w, y+20)); p.LineTo(new Vector2(w-15, y+20)); p.Fill();
        }
        
        p.fillColor = new Color(0f, 0f, 0f, 0.5f);
        p.BeginPath(); p.MoveTo(new Vector2(0, 0)); p.LineTo(new Vector2(10, 0)); p.LineTo(new Vector2(10, h)); p.LineTo(new Vector2(0, h)); p.Fill();
    }

    public void AbortGame()
    {
        isPlaying = false;
        container.style.display = DisplayStyle.None;
    }

    private IEnumerator WinAnimationRoutine()
    {
        while (!isPlaying && manager.isPlaying)
        {
            bubbleTime += Time.deltaTime;
            if (pumpNormalBar != null) pumpNormalBar.MarkDirtyRepaint();
            yield return null;
        }
    }

    private IEnumerator ShowWinFeedbackAndClose()
    {
        bool isToilet = config != null && config.instructions.ToLower().Contains("toilet");
        if (isToilet) {
            yield return new WaitForSeconds(1.0f);
        }
        string stampString = config.pumpIsBalanceMode ? "PERFECT TEMP!" : "CLEARED!";
        Label stampText = new Label(stampString);
        stampText.style.position = Position.Absolute;
        stampText.style.color = new StyleColor(new Color(0.18f, 0.8f, 0.44f)); // Green
        stampText.style.fontSize = 72;
        stampText.style.unityFontStyleAndWeight = FontStyle.Bold;
        stampText.style.unityTextAlign = TextAnchor.MiddleCenter;
        
        stampText.style.fontSize = 110;
        stampText.style.letterSpacing = 10;
        stampText.style.rotate = new StyleRotate(new Rotate(new Angle(-12f, AngleUnit.Degree)));
        stampText.style.textShadow = new StyleTextShadow(new TextShadow { color = new Color(0,0,0,0.8f), offset = new Vector2(12f, 15f), blurRadius = 15f });
        
        VisualElement backgroundFade = new VisualElement();
        backgroundFade.style.position = Position.Absolute;
        backgroundFade.style.width = new Length(100, LengthUnit.Percent);
        backgroundFade.style.height = new Length(100, LengthUnit.Percent);
        backgroundFade.style.backgroundColor = new StyleColor(new Color(0f, 0f, 0f, 0.7f));
        backgroundFade.style.opacity = 0f;
        
        stampText.style.top = new Length(50, LengthUnit.Percent);
        stampText.style.left = new Length(50, LengthUnit.Percent);
        stampText.style.translate = new Translate(new Length(-50, LengthUnit.Percent), new Length(-50, LengthUnit.Percent));
        
        manager.miniGameDocument.rootVisualElement.Add(backgroundFade);
        manager.miniGameDocument.rootVisualElement.Add(stampText);
        
        float life = 0.8f;
        float elapsed = 0f;
        while(elapsed < life)
        {
            elapsed += Time.deltaTime;
            float p = elapsed / life;
            
            float scale = Mathf.Lerp(3.5f, 1f, p * 12f);
            if (scale < 1f) scale = 1f;
            stampText.style.scale = new StyleScale(new Vector2(scale, scale));
            
            backgroundFade.style.opacity = Mathf.Lerp(0f, 1f, p * 8f);
            
            if (p > 0.8f) {
                float fadeOut = 1f - ((p - 0.8f) * 5f);
                stampText.style.opacity = fadeOut;
                backgroundFade.style.opacity = fadeOut;
            }
            
            yield return null;
        }
        
        if (manager.miniGameDocument.rootVisualElement.Contains(stampText)) manager.miniGameDocument.rootVisualElement.Remove(stampText);
        if (manager.miniGameDocument.rootVisualElement.Contains(backgroundFade)) manager.miniGameDocument.rootVisualElement.Remove(backgroundFade);
        AudioManager.Instance?.PlayVictoryFanfare();
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
                bubbleTime += Time.deltaTime;
                if (pumpNormalBar != null) pumpNormalBar.MarkDirtyRepaint();
                
                // Normal Mode (e.g., Pump Up a tyre or something)
                float decayFactor = 1f + (pumpProgress / 100f) * 2f; 
                pumpProgress -= (decayRate * 1.7f) * decayFactor * Time.deltaTime;
                pumpProgress = Mathf.Clamp(pumpProgress, 0f, 100f);
                
                if (pumpProgressBar != null) pumpProgressBar.style.height = Length.Percent(pumpProgress);

                if (pumpProgress >= 100f)
                {
                    if (pumpProgressBar != null) pumpProgressBar.style.height = Length.Percent(100f);
                    isPlaying = false;
                    manager.StartCoroutine(WinAnimationRoutine());
                    manager.StartCoroutine(ShowWinFeedbackAndClose());
                    yield break;
                }
            }
            yield return null;
        }
    }
}

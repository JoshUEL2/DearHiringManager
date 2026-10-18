using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;

public class CloggedVentsMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement container;
    private VisualElement cvGrid;
    private Label timerLabel;

    private List<VisualElement> cvVents = new List<VisualElement>();
    private float[] cvDustLevels;
    private bool[] cvIsClogged;
    private float[] fanAngles;
    private float[] blastTimers;
    private float timer;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        container = root.Q<VisualElement>("CloggedVentsContainer");
    }

    public void StartGame()
    {
        if (container == null) return;
        container.style.display = DisplayStyle.Flex;
        container.Clear();
        cvVents.Clear();

        int count = config.ventCount > 0 ? config.ventCount : 6;
        cvDustLevels = new float[count];
        cvIsClogged = new bool[count];
        fanAngles = new float[count];
        blastTimers = new float[count];

        // Container Theme
        container.style.backgroundColor = new StyleColor(new Color(0.1f, 0.11f, 0.12f));
        container.style.borderTopWidth = 6; container.style.borderBottomWidth = 6; container.style.borderLeftWidth = 6; container.style.borderRightWidth = 6;
        container.style.borderTopColor = new StyleColor(new Color(0.2f, 0.2f, 0.25f));
        container.style.borderBottomColor = new StyleColor(new Color(0.05f, 0.05f, 0.08f));
        container.style.borderLeftColor = new StyleColor(new Color(0.15f, 0.15f, 0.18f));
        container.style.borderRightColor = new StyleColor(new Color(0.15f, 0.15f, 0.18f));
        container.style.overflow = Overflow.Hidden;

        container.generateVisualContent -= OnDrawBackground;
        container.generateVisualContent += OnDrawBackground;

        // Header Panel
        var header = new VisualElement();
        header.style.flexDirection = FlexDirection.Row;
        header.style.justifyContent = Justify.SpaceBetween;
        header.style.alignItems = Align.Center;
        header.style.paddingTop = 15; header.style.paddingBottom = 15;
        header.style.paddingLeft = 20; header.style.paddingRight = 20;
        header.style.backgroundColor = new StyleColor(new Color(0.05f, 0.05f, 0.05f, 0.8f));
        header.style.borderBottomWidth = 4;
        header.style.borderBottomColor = new StyleColor(new Color(0.9f, 0.7f, 0.1f));

        var title = new Label("FILTER EXHAUST MAIN");
        title.style.color = new StyleColor(new Color(0.6f, 0.6f, 0.6f));
        title.style.fontSize = 22;
        title.style.unityFontStyleAndWeight = FontStyle.Bold;
        header.Add(title);

        timerLabel = new Label("00.00");
        timerLabel.style.fontSize = 40;
        timerLabel.style.color = new StyleColor(new Color(1f, 0.2f, 0.2f));
        timerLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        header.Add(timerLabel);
        
        container.Add(header);

        // Vents Grid
        cvGrid = new VisualElement();
        cvGrid.style.flexDirection = FlexDirection.Row;
        cvGrid.style.flexWrap = Wrap.Wrap;
        cvGrid.style.justifyContent = Justify.Center;
        cvGrid.style.alignItems = Align.Center;
        cvGrid.style.flexGrow = 1;
        cvGrid.style.paddingTop = 20;
        cvGrid.style.paddingBottom = 20;
        container.Add(cvGrid);

        for (int i = 0; i < count; i++)
        {
            VisualElement vent = new VisualElement();
            vent.style.width = 150;
            vent.style.height = 150;
            vent.style.marginTop = 15; vent.style.marginBottom = 15;
            vent.style.marginLeft = 15; vent.style.marginRight = 15;
            vent.userData = i; 
            
            int index = i;
            vent.RegisterCallback<PointerDownEvent>(ev => {
                if (!manager.isPlaying || cvIsClogged[index]) return;
                MiniGameFeedback.PlaySuccess(vent, "BLAST!", container);
                cvDustLevels[index] = 0f;
                blastTimers[index] = 1f;
                manager.StartCoroutine(ShakeVent(vent));
            });

            vent.generateVisualContent += OnDrawFan;
            
            cvVents.Add(vent);
            cvGrid.Add(vent);
        }

        timer = config.timeLimit > 0 ? config.timeLimit : 15f;
        manager.StartCoroutine(GameRoutine());
    }

    public void AbortGame()
    {
        if (container != null) container.style.display = DisplayStyle.None;
    }

    private IEnumerator GameRoutine()
    {
        while (manager.isPlaying)
        {
            timer -= Time.deltaTime;
            if (timerLabel != null) timerLabel.text = Mathf.Max(timer, 0f).ToString("0.00");

            if (timer <= 0f)
            {
                manager.EndGame(true);
                yield break;
            }
            
            int cloggedCount = 0;
            for (int i = 0; i < cvDustLevels.Length; i++)
            {
                if (blastTimers[i] > 0f) {
                    blastTimers[i] -= Time.deltaTime * 3f;
                }

                if (cvIsClogged[i])
                {
                    cloggedCount++;
                }
                else
                {
                    // Update rotation
                    float speed = 1200f * (1f - (cvDustLevels[i] / 100f));
                    fanAngles[i] += speed * Time.deltaTime;
                    
                    // Add dust
                    cvDustLevels[i] += Random.Range(4f, 16f) * Time.deltaTime;
                    
                    if (cvDustLevels[i] >= 100f)
                    {
                        cvDustLevels[i] = 100f;
                        cvIsClogged[i] = true;
                        cloggedCount++;
                        MiniGameFeedback.PlayError(cvVents[i], "CLOGGED!", container);
                        manager.StartCoroutine(DeathSpark(cvVents[i]));
                    }
                }
                
                // Force redraw for animation
                if (cvVents[i] != null) cvVents[i].MarkDirtyRepaint();
            }

            if (cloggedCount >= 2)
            {
                manager.EndGame(false);
                yield break;
            }
            yield return null;
        }
    }

    private void OnDrawBackground(MeshGenerationContext ctx)
    {
        var p = ctx.painter2D;
        float w = ctx.visualElement.layout.width;
        float h = ctx.visualElement.layout.height;

        p.fillColor = new Color(0.1f, 0.11f, 0.12f);
        p.BeginPath(); p.MoveTo(new Vector2(0,0)); p.LineTo(new Vector2(w,0)); p.LineTo(new Vector2(w,h)); p.LineTo(new Vector2(0,h)); p.Fill();
        
        p.fillColor = new Color(0.9f, 0.8f, 0.1f, 0.08f);
        for(float y = -100; y < h; y += 40) {
            p.BeginPath();
            p.MoveTo(new Vector2(0, y)); p.LineTo(new Vector2(30, y));
            p.LineTo(new Vector2(30, y+20)); p.LineTo(new Vector2(0, y+20));
            p.Fill();
            p.BeginPath();
            p.MoveTo(new Vector2(w-30, y)); p.LineTo(new Vector2(w, y));
            p.LineTo(new Vector2(w, y+20)); p.LineTo(new Vector2(w-30, y+20));
            p.Fill();
        }
        
        p.fillColor = new Color(0.08f, 0.08f, 0.08f);
        p.BeginPath(); p.MoveTo(new Vector2(0, h/2 - 70)); p.LineTo(new Vector2(w, h/2 - 70)); p.LineTo(new Vector2(w, h/2 + 70)); p.LineTo(new Vector2(0, h/2 + 70)); p.Fill();
        
        p.strokeColor = new Color(0.05f, 0.05f, 0.05f);
        p.lineWidth = 10f;
        p.BeginPath(); p.MoveTo(new Vector2(0, h/2 - 70)); p.LineTo(new Vector2(w, h/2 - 70)); p.Stroke();
        p.BeginPath(); p.MoveTo(new Vector2(0, h/2 + 70)); p.LineTo(new Vector2(w, h/2 + 70)); p.Stroke();
    }

    private void OnDrawFan(MeshGenerationContext ctx)
    {
        int i = (int)ctx.visualElement.userData;
        var p = ctx.painter2D;
        float w = ctx.visualElement.layout.width;
        float h = ctx.visualElement.layout.height;
        float cx = w/2f; float cy = h/2f;
        float radius = (w/2f) - 15f;
        
        p.fillColor = new Color(0.12f, 0.12f, 0.14f);
        p.BeginPath(); p.MoveTo(new Vector2(0,0)); p.LineTo(new Vector2(w,0)); p.LineTo(new Vector2(w,h)); p.LineTo(new Vector2(0,h)); p.Fill();
        
        p.strokeColor = new Color(0.2f, 0.2f, 0.2f);
        p.lineWidth = 2f;
        p.BeginPath(); p.MoveTo(new Vector2(0,0)); p.LineTo(new Vector2(w,0)); p.LineTo(new Vector2(w,h)); p.LineTo(new Vector2(0,h)); p.ClosePath(); p.Stroke();

        p.fillColor = new Color(0.02f, 0.02f, 0.02f);
        p.BeginPath(); p.Arc(new Vector2(cx, cy), radius, 0, 360); p.Fill();
        
        Color ledColor = Color.green;
        if (cvIsClogged[i]) ledColor = new Color(0.2f, 0.2f, 0.2f);
        else if (cvDustLevels[i] > 85f) ledColor = Color.red;
        else if (cvDustLevels[i] > 60f) ledColor = new Color(1f, 0.5f, 0f);
        else if (cvDustLevels[i] > 30f) ledColor = Color.yellow;
        
        if (!cvIsClogged[i] && cvDustLevels[i] > 75f) {
            float pulse = (Mathf.Sin(Time.time * 20f) + 1f) * 0.5f;
            ledColor = Color.Lerp(new Color(0.5f, 0f, 0f), Color.red, pulse);
        }

        p.strokeColor = ledColor;
        p.lineWidth = 4f;
        p.BeginPath(); p.Arc(new Vector2(cx, cy), radius + 5f, 0, 360); p.Stroke();

        float currentAngle = fanAngles[i];
        p.fillColor = new Color(0.5f, 0.5f, 0.55f); 
        
        for(int b = 0; b < 5; b++) {
            float angleRad = (currentAngle + b * 72f) * Mathf.Deg2Rad;
            float widthRadRoot = 15f * Mathf.Deg2Rad;
            float widthRadTip = 35f * Mathf.Deg2Rad;
            
            Vector2 p0 = new Vector2(cx + Mathf.Cos(angleRad - widthRadRoot) * 10f, cy + Mathf.Sin(angleRad - widthRadRoot) * 10f);
            Vector2 p1 = new Vector2(cx + Mathf.Cos(angleRad + widthRadRoot) * 10f, cy + Mathf.Sin(angleRad + widthRadRoot) * 10f);
            Vector2 p2 = new Vector2(cx + Mathf.Cos(angleRad + widthRadTip) * (radius-2f), cy + Mathf.Sin(angleRad + widthRadTip) * (radius-2f));
            Vector2 p3 = new Vector2(cx + Mathf.Cos(angleRad - widthRadTip) * (radius-2f), cy + Mathf.Sin(angleRad - widthRadTip) * (radius-2f));
            
            p.BeginPath();
            p.MoveTo(p0); p.LineTo(p1); p.LineTo(p2); p.LineTo(p3); p.ClosePath();
            p.Fill();
        }

        p.fillColor = new Color(0.2f, 0.2f, 0.2f);
        p.BeginPath(); p.Arc(new Vector2(cx, cy), 18f, 0, 360); p.Fill();
        p.fillColor = ledColor;
        p.BeginPath(); p.Arc(new Vector2(cx, cy), 6f, 0, 360); p.Fill();

        float dustNormalized = cvDustLevels[i] / 100f;
        if (cvIsClogged[i]) dustNormalized = 1f;
        
        if (dustNormalized > 0.01f) {
            p.fillColor = new Color(0.35f, 0.25f, 0.15f, dustNormalized * 0.95f);
            p.BeginPath(); p.Arc(new Vector2(cx, cy), radius, 0, 360); p.Fill();
        }

        if (blastTimers[i] > 0f) {
            float blastNorm = 1f - blastTimers[i]; 
            float blastRad = Mathf.Lerp(15f, radius + 25f, blastNorm * blastNorm); 
            float blastAlpha = blastTimers[i]; 
            
            p.strokeColor = new Color(0.8f, 1f, 1f, blastAlpha); 
            p.lineWidth = 15f * blastAlpha;
            p.BeginPath(); p.Arc(new Vector2(cx, cy), blastRad, 0, 360); p.Stroke();
            
            p.fillColor = new Color(0.8f, 1f, 1f, blastAlpha * 0.4f);
            p.BeginPath(); p.Arc(new Vector2(cx, cy), blastRad, 0, 360); p.Fill();
        }
    }

    private IEnumerator ShakeVent(VisualElement vent)
    {
        float elapsed = 0f;
        while(elapsed < 0.15f) {
            elapsed += Time.deltaTime;
            float x = Random.Range(-8f, 8f);
            float y = Random.Range(-8f, 8f);
            vent.style.translate = new StyleTranslate(new Translate(x, y, 0));
            yield return null;
        }
        vent.style.translate = new StyleTranslate(new Translate(0, 0, 0));
    }

    private IEnumerator DeathSpark(VisualElement vent)
    {
        float elapsed = 0f;
        while(elapsed < 0.3f) {
            elapsed += Time.deltaTime;
            float x = Random.Range(-15f, 15f);
            float y = Random.Range(-15f, 15f);
            vent.style.translate = new StyleTranslate(new Translate(x, y, 0));
            yield return null;
        }
        vent.style.translate = new StyleTranslate(new Translate(0, 0, 0));
    }
}

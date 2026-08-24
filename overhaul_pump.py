import re

path = 'Assets/Scripts/2 Managers/MiniGames/PumpSystemMiniGame.cs'
with open(path, 'r') as f:
    content = f.read()

# 1. Add bubbleTime field
content = content.replace('private float dynamicTargetMin;', 'private float bubbleTime = 0f;\n    private float dynamicTargetMin;')

# 2. Update Initialize for Normal Bar visual replacement
target_init = r'''        if \(pumpTrackArea != null\)
        \{
            var shaft = pumpTrackArea\.Q<VisualElement>\("PumpShaft"\);
            if \(shaft != null\) shaft\.style\.width = 30;
        \}'''

replacement_init = r'''        if (pumpNormalBar != null) {
            pumpNormalBar.style.width = 160;
            pumpNormalBar.style.borderTopWidth = 6; pumpNormalBar.style.borderBottomWidth = 6;
            pumpNormalBar.style.borderLeftWidth = 6; pumpNormalBar.style.borderRightWidth = 6;
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
                shaft.style.width = 30;
                shaft.style.backgroundColor = new StyleColor(new Color(0.6f, 0.6f, 0.6f));
            }
            if (pumpHandle != null) {
                pumpHandle.style.backgroundColor = new StyleColor(new Color(0.9f, 0.2f, 0.2f));
                pumpHandle.style.borderTopWidth = 4; pumpHandle.style.borderBottomWidth = 4;
                pumpHandle.style.borderLeftWidth = 4; pumpHandle.style.borderRightWidth = 4;
                pumpHandle.style.borderTopColor = new StyleColor(new Color(1f, 0.4f, 0.4f));
                pumpHandle.style.borderBottomColor = new StyleColor(new Color(0.5f, 0.1f, 0.1f));
            }
        }'''

content = re.sub(target_init, replacement_init, content)

# 3. Add Painter Methods
target_painter = r'''    public void AbortGame\(\)
    \{'''

replacement_painter = r'''    private void OnDrawGreaseVat(MeshGenerationContext ctx)
    {
        var p = ctx.painter2D;
        float w = ctx.visualElement.layout.width;
        float h = ctx.visualElement.layout.height;
        if (w == 0 || h == 0) return;

        p.fillColor = new Color(0.05f, 0.1f, 0.05f, 0.8f);
        p.BeginPath(); p.MoveTo(new Vector2(0, 0)); p.LineTo(new Vector2(w, 0)); p.LineTo(new Vector2(w, h)); p.LineTo(new Vector2(0, h)); p.Fill();
        
        float greaseHeight = h * (1f - (pumpProgress / 100f));
        
        if (greaseHeight > 0)
        {
            p.fillColor = new Color(0.35f, 0.25f, 0.1f);
            
            p.BeginPath();
            p.MoveTo(new Vector2(0, h));
            p.LineTo(new Vector2(w, h));
            p.LineTo(new Vector2(w, greaseHeight));
            
            int segments = 10;
            float segWidth = w / segments;
            for (int i = segments - 1; i >= 0; i--)
            {
                float px = i * segWidth;
                float py = greaseHeight + Mathf.Sin(bubbleTime * 4f + px * 0.1f) * 12f;
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
                float by = h - ((bubbleTime * bSpeed + Random.Range(0, h)) % h);
                if (by > greaseHeight) {
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
    {'''

content = re.sub(target_painter, replacement_painter, content)

# 4. Stamp text change
target_stamp = r'''    private IEnumerator ShowWinFeedbackAndClose\(\)
    \{
        Label stampText = new Label\("PERFECT TEMP!"\);'''

replacement_stamp = r'''    private IEnumerator ShowWinFeedbackAndClose()
    {
        string stampString = config.pumpIsBalanceMode ? "PERFECT TEMP!" : "CLEARED!";
        Label stampText = new Label(stampString);'''

content = re.sub(target_stamp, replacement_stamp, content)

# 5. Timer repaint logic
target_routine = r'''            else
            \{
                // Normal Mode \(e\.g\., Pump Up a tyre or something\)'''

replacement_routine = r'''            else
            {
                if (pumpNormalBar != null) {
                    bubbleTime += Time.deltaTime;
                    pumpNormalBar.MarkDirtyRepaint();
                }
                
                // Normal Mode (e.g., Pump Up a tyre or something)'''

content = re.sub(target_routine, replacement_routine, content)

with open(path, 'w') as f:
    f.write(content)


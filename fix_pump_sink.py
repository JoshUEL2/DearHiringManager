import re

path = 'Assets/Scripts/2 Managers/MiniGames/PumpSystemMiniGame.cs'
with open(path, 'r') as f:
    content = f.read()

target_init = r'''            var shaft = pumpTrackArea\.Q<VisualElement>\("PumpShaft"\);
            if \(shaft != null\) \{
                shaft\.style\.width = 30;
                shaft\.style\.backgroundColor = new StyleColor\(new Color\(0\.6f, 0\.6f, 0\.6f\)\);
            \}'''

replacement_init = r'''            bool isSink = config != null && config.instructions.ToLower().Contains("sink");
            var shaft = pumpTrackArea.Q<VisualElement>("PumpShaft");
            if (shaft != null) {
                shaft.style.width = isSink ? 16 : 30;
                shaft.style.backgroundColor = isSink ? new StyleColor(new Color(0.7f, 0.5f, 0.3f)) : new StyleColor(new Color(0.6f, 0.6f, 0.6f));
            }'''

content = re.sub(target_init, replacement_init, content)

target_grease_draw = r'''    private void OnDrawGreaseVat\(MeshGenerationContext ctx\)
    \{
        var p = ctx\.painter2D;
        float w = ctx\.visualElement\.layout\.width;
        float h = ctx\.visualElement\.layout\.height;
        if \(w == 0 \|\| h == 0\) return;

        p\.fillColor = new Color\(0\.05f, 0\.1f, 0\.05f, 0\.8f\);
        p\.BeginPath\(\); p\.MoveTo\(new Vector2\(0, 0\)\); p\.LineTo\(new Vector2\(w, 0\)\); p\.LineTo\(new Vector2\(w, h\)\); p\.LineTo\(new Vector2\(0, h\)\); p\.Fill\(\);
        
        float greaseTopY = h \* \(pumpProgress / 100f\);
        
        if \(greaseTopY < h\)
        \{
            p\.fillColor = new Color\(0\.35f, 0\.25f, 0\.1f\);
            
            p\.BeginPath\(\);
            p\.MoveTo\(new Vector2\(0, h\)\);
            p\.LineTo\(new Vector2\(w, h\)\);
            p\.LineTo\(new Vector2\(w, greaseTopY\)\);
            
            int segments = 10;
            float segWidth = w / segments;
            for \(int i = segments - 1; i >= 0; i--\)
            \{
                float px = i \* segWidth;
                float py = greaseTopY \+ Mathf\.Sin\(bubbleTime \* 4f \+ px \* 0\.1f\) \* 12f;
                if \(i == 0\) p\.LineTo\(new Vector2\(0, py\)\);
                else p\.LineTo\(new Vector2\(px, py\)\);
            \}
            p\.Fill\(\);

            p\.fillColor = new Color\(0\.45f, 0\.35f, 0\.15f, 0\.6f\);
            Random\.State oldState = Random\.state;
            Random\.InitState\(42\);
            for\(int i=0; i<20; i\+\+\) \{
                float bx = Random\.Range\(10f, w - 10f\);
                float bSpeed = Random\.Range\(30f, 80f\);
                float bubbleRange = h - greaseTopY;
                if \(bubbleRange > 5f\) \{
                    float by = h - \(\(bubbleTime \* bSpeed \+ Random\.Range\(0, h\)\) % bubbleRange\);
                    float bSize = Random\.Range\(4f, 15f\);
                    p\.BeginPath\(\); p\.Arc\(new Vector2\(bx, by\), bSize, 0, 360\); p\.Fill\(\);
                \}
            \}
            Random\.state = oldState;
        \}

        p\.strokeColor = new Color\(1f, 1f, 1f, 0\.2f\);
        p\.lineWidth = 2f;
        for\(float y = h \* 0\.2f; y < h; y \+= h \* 0\.2f\) \{
            p\.BeginPath\(\); p\.MoveTo\(new Vector2\(0, y\)\); p\.LineTo\(new Vector2\(25, y\)\); p\.Stroke\(\);
            p\.BeginPath\(\); p\.MoveTo\(new Vector2\(w, y\)\); p\.LineTo\(new Vector2\(w - 25, y\)\); p\.Stroke\(\);
        \}
        
        p\.fillColor = new Color\(1f, 1f, 1f, 0\.1f\);
        p\.BeginPath\(\); p\.MoveTo\(new Vector2\(5, 5\)\); p\.LineTo\(new Vector2\(20, 5\)\); p\.LineTo\(new Vector2\(20, h-5\)\); p\.LineTo\(new Vector2\(5, h-5\)\); p\.Fill\(\);
    \}'''

replacement_grease_draw = r'''    private void OnDrawGreaseVat(MeshGenerationContext ctx)
    {
        var p = ctx.painter2D;
        float w = ctx.visualElement.layout.width;
        float h = ctx.visualElement.layout.height;
        if (w == 0 || h == 0) return;

        bool isSink = config != null && config.instructions.ToLower().Contains("sink");

        if (isSink)
        {
            p.fillColor = new Color(0.75f, 0.78f, 0.8f);
            p.BeginPath(); p.MoveTo(new Vector2(0, 0)); p.LineTo(new Vector2(w, 0)); p.LineTo(new Vector2(w, h)); p.LineTo(new Vector2(0, h)); p.Fill();
            
            p.fillColor = new Color(0.55f, 0.6f, 0.65f);
            p.BeginPath(); p.MoveTo(new Vector2(15, 15)); p.LineTo(new Vector2(w-15, 15)); p.LineTo(new Vector2(w-35, h-25)); p.LineTo(new Vector2(35, h-25)); p.Fill();

            p.fillColor = new Color(0.1f, 0.1f, 0.15f);
            p.BeginPath(); p.Arc(new Vector2(w/2, h - 35), 25, 0, 360); p.Fill();
            
            p.strokeColor = new Color(0.3f, 0.3f, 0.35f);
            p.lineWidth = 2f;
            p.BeginPath(); p.Arc(new Vector2(w/2, h - 35), 25, 0, 360); p.Stroke();
        }
        else
        {
            p.fillColor = new Color(0.05f, 0.1f, 0.05f, 0.8f);
            p.BeginPath(); p.MoveTo(new Vector2(0, 0)); p.LineTo(new Vector2(w, 0)); p.LineTo(new Vector2(w, h)); p.LineTo(new Vector2(0, h)); p.Fill();
        }
        
        float greaseTopY = h * (pumpProgress / 100f);
        
        if (greaseTopY < h)
        {
            if (isSink)
            {
                p.fillColor = new Color(0.3f, 0.4f, 0.35f, 0.85f);
            }
            else
            {
                p.fillColor = new Color(0.35f, 0.25f, 0.1f);
            }
            
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

            p.fillColor = isSink ? new Color(0.4f, 0.5f, 0.3f, 0.7f) : new Color(0.45f, 0.35f, 0.15f, 0.6f);
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

        if (!isSink)
        {
            p.strokeColor = new Color(1f, 1f, 1f, 0.2f);
            p.lineWidth = 2f;
            for(float y = h * 0.2f; y < h; y += h * 0.2f) {
                p.BeginPath(); p.MoveTo(new Vector2(0, y)); p.LineTo(new Vector2(25, y)); p.Stroke();
                p.BeginPath(); p.MoveTo(new Vector2(w, y)); p.LineTo(new Vector2(w - 25, y)); p.Stroke();
            }
            
            p.fillColor = new Color(1f, 1f, 1f, 0.1f);
            p.BeginPath(); p.MoveTo(new Vector2(5, 5)); p.LineTo(new Vector2(20, 5)); p.LineTo(new Vector2(20, h-5)); p.LineTo(new Vector2(5, h-5)); p.Fill();
        }
    }'''

content = re.sub(target_grease_draw, replacement_grease_draw, content)

target_track_draw = r'''    private void OnDrawPumpTrack\(MeshGenerationContext ctx\)
    \{
        var p = ctx\.painter2D;
        float w = ctx\.visualElement\.layout\.width;
        float h = ctx\.visualElement\.layout\.height;
        if \(w == 0 \|\| h == 0\) return;

        p\.fillColor = new Color\(0\.12f, 0\.14f, 0\.16f\);
        p\.BeginPath\(\); p\.MoveTo\(new Vector2\(0, 0\)\); p\.LineTo\(new Vector2\(w, 0\)\); p\.LineTo\(new Vector2\(w, h\)\); p\.LineTo\(new Vector2\(0, h\)\); p\.Fill\(\);

        p\.fillColor = new Color\(0\.9f, 0\.7f, 0\.1f, 0\.3f\);
        for\(float y = -50; y < h; y \+= 40\) \{
            p\.BeginPath\(\); p\.MoveTo\(new Vector2\(0, y\)\); p\.LineTo\(new Vector2\(15, y\)\); p\.LineTo\(new Vector2\(15, y\+20\)\); p\.LineTo\(new Vector2\(0, y\+20\)\); p\.Fill\(\);
            p\.BeginPath\(\); p\.MoveTo\(new Vector2\(w-15, y\)\); p\.LineTo\(new Vector2\(w, y\)\); p\.LineTo\(new Vector2\(w, y\+20\)\); p\.LineTo\(new Vector2\(w-15, y\+20\)\); p\.Fill\(\);
        \}
        
        p\.fillColor = new Color\(0f, 0f, 0f, 0\.5f\);
        p\.BeginPath\(\); p\.MoveTo\(new Vector2\(0, 0\)\); p\.LineTo\(new Vector2\(10, 0\)\); p\.LineTo\(new Vector2\(10, h\)\); p\.LineTo\(new Vector2\(0, h\)\); p\.Fill\(\);
    \}'''

replacement_track_draw = r'''    private void OnDrawPumpTrack(MeshGenerationContext ctx)
    {
        var p = ctx.painter2D;
        float w = ctx.visualElement.layout.width;
        float h = ctx.visualElement.layout.height;
        if (w == 0 || h == 0) return;

        bool isSink = config != null && config.instructions.ToLower().Contains("sink");

        if (isSink)
        {
            p.fillColor = new Color(0.9f, 0.9f, 0.9f);
            p.BeginPath(); p.MoveTo(new Vector2(0, 0)); p.LineTo(new Vector2(w, 0)); p.LineTo(new Vector2(w, h)); p.LineTo(new Vector2(0, h)); p.Fill();

            p.strokeColor = new Color(0.8f, 0.8f, 0.8f);
            p.lineWidth = 2f;
            for(float y = 0; y < h; y += 30) {
                p.BeginPath(); p.MoveTo(new Vector2(0, y)); p.LineTo(new Vector2(w, y)); p.Stroke();
            }
            for(float x = 0; x < w; x += 40) {
                p.BeginPath(); p.MoveTo(new Vector2(x, 0)); p.LineTo(new Vector2(x, h)); p.Stroke();
            }
        }
        else
        {
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
    }'''

content = re.sub(target_track_draw, replacement_track_draw, content)

with open(path, 'w') as f:
    f.write(content)

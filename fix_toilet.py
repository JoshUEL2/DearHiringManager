import re

path = 'Assets/Scripts/2 Managers/MiniGames/PumpSystemMiniGame.cs'
with open(path, 'r') as f:
    content = f.read()

# Replace config checks to check for "toilet"
content = content.replace('.Contains("sink")', '.Contains("toilet")')
content = content.replace('isSink', 'isToilet')

target_start = r'''        else
        \{
            if \(pumpDialContainer != null\) pumpDialContainer\.style\.display = DisplayStyle\.None;
            if \(pumpNormalBar != null\) pumpNormalBar\.style\.display = DisplayStyle\.Flex;
            if \(pumpHoldTimerContainer != null\) pumpHoldTimerContainer\.style\.display = DisplayStyle\.None;
            if \(pumpProgressBar != null\) pumpProgressBar\.style\.height = Length\.Percent\(0f\);
        \}'''

replacement_start = r'''        else
        {
            if (pumpDialContainer != null) pumpDialContainer.style.display = DisplayStyle.None;
            
            bool isToilet = config.instructions.ToLower().Contains("toilet");
            if (pumpNormalBar != null) pumpNormalBar.style.display = isToilet ? DisplayStyle.None : DisplayStyle.Flex;
            
            if (pumpHoldTimerContainer != null) pumpHoldTimerContainer.style.display = DisplayStyle.None;
            if (pumpProgressBar != null) pumpProgressBar.style.height = Length.Percent(0f);
            
            if (pumpTrackArea != null && isToilet)
            {
                pumpTrackArea.style.width = 280;
                pumpTrackArea.style.backgroundColor = new StyleColor(Color.clear);
                pumpTrackArea.style.borderTopWidth = 0; pumpTrackArea.style.borderBottomWidth = 0;
                pumpTrackArea.style.borderLeftWidth = 0; pumpTrackArea.style.borderRightWidth = 0;
            }
        }'''
content = re.sub(target_start, replacement_start, content)

target_init_shaft = r'''            var shaft = pumpTrackArea\.Q<VisualElement>\("PumpShaft"\);
            if \(shaft != null\) \{
                shaft\.style\.width = isToilet \? 16 : 30;
                shaft\.style\.backgroundColor = isToilet \? new StyleColor\(new Color\(0\.7f, 0\.5f, 0\.3f\)\) : new StyleColor\(new Color\(0\.6f, 0\.6f, 0\.6f\)\);
            \}'''
replacement_init_shaft = r'''            var shaft = pumpTrackArea.Q<VisualElement>("PumpShaft");
            if (shaft != null) {
                shaft.style.width = isToilet ? 16 : 30;
                shaft.style.backgroundColor = isToilet ? new StyleColor(new Color(0.7f, 0.5f, 0.3f)) : new StyleColor(new Color(0.6f, 0.6f, 0.6f));
                if (isToilet) {
                    shaft.style.left = 150;
                    shaft.style.translate = new Translate(new Length(-50, LengthUnit.Percent), 0);
                }
            }'''
content = re.sub(target_init_shaft, replacement_init_shaft, content)

target_init_handle = r'''            if \(pumpHandle != null\) \{
                if \(isToilet\) \{
                    pumpHandle\.style\.backgroundColor = new StyleColor\(new Color\(0\.8f, 0\.2f, 0\.2f\)\);
                    pumpHandle\.style\.borderTopWidth = 0; pumpHandle\.style\.borderBottomWidth = 0;
                    pumpHandle\.style\.borderLeftWidth = 0; pumpHandle\.style\.borderRightWidth = 0;
                    pumpHandle\.style\.borderTopLeftRadius = 25;
                    pumpHandle\.style\.borderTopRightRadius = 25;
                    pumpHandle\.style\.borderBottomLeftRadius = 5;
                    pumpHandle\.style\.borderBottomRightRadius = 5;
                    pumpHandle\.style\.height = 40;
                \} else \{'''
replacement_init_handle = r'''            if (pumpHandle != null) {
                if (isToilet) {
                    pumpHandle.style.backgroundColor = new StyleColor(new Color(0.8f, 0.2f, 0.2f));
                    pumpHandle.style.borderTopWidth = 0; pumpHandle.style.borderBottomWidth = 0;
                    pumpHandle.style.borderLeftWidth = 0; pumpHandle.style.borderRightWidth = 0;
                    pumpHandle.style.borderTopLeftRadius = 25;
                    pumpHandle.style.borderTopRightRadius = 25;
                    pumpHandle.style.borderBottomLeftRadius = 5;
                    pumpHandle.style.borderBottomRightRadius = 5;
                    pumpHandle.style.height = 40;
                    pumpHandle.style.width = 100;
                    pumpHandle.style.left = 150;
                    pumpHandle.style.translate = new Translate(new Length(-50, LengthUnit.Percent), 0);
                } else {'''
content = re.sub(target_init_handle, replacement_init_handle, content)

target_track_draw = r'''        if \(isToilet\)
        \{
            p\.fillColor = new Color\(0\.9f, 0\.9f, 0\.9f\);
            p\.BeginPath\(\); p\.MoveTo\(new Vector2\(0, 0\)\); p\.LineTo\(new Vector2\(w, 0\)\); p\.LineTo\(new Vector2\(w, h\)\); p\.LineTo\(new Vector2\(0, h\)\); p\.Fill\(\);

            p\.strokeColor = new Color\(0\.8f, 0\.8f, 0\.8f\);
            p\.lineWidth = 2f;
            for\(float y = 0; y < h; y \+= 30\) \{
                p\.BeginPath\(\); p\.MoveTo\(new Vector2\(0, y\)\); p\.LineTo\(new Vector2\(w, y\)\); p\.Stroke\(\);
            \}
            for\(float x = 0; x < w; x \+= 40\) \{
                p\.BeginPath\(\); p\.MoveTo\(new Vector2\(x, 0\)\); p\.LineTo\(new Vector2\(x, h\)\); p\.Stroke\(\);
            \}
        \}
        else'''

replacement_track_draw = r'''        if (isToilet)
        {
            // Toilet Tank
            p.fillColor = new Color(0.95f, 0.95f, 0.95f);
            p.BeginPath(); p.MoveTo(new Vector2(20, 20)); p.LineTo(new Vector2(80, 20));
            p.LineTo(new Vector2(80, 150)); p.LineTo(new Vector2(20, 150)); p.Fill();
            
            // Toilet Bowl
            p.BeginPath(); p.MoveTo(new Vector2(80, 120)); p.LineTo(new Vector2(220, 120));
            p.BezierCurveTo(new Vector2(220, 250), new Vector2(160, 250), new Vector2(160, 320));
            p.LineTo(new Vector2(100, 320));
            p.BezierCurveTo(new Vector2(100, 250), new Vector2(80, 200), new Vector2(80, 120));
            p.Fill();

            // Inner Bowl
            p.fillColor = new Color(0.85f, 0.85f, 0.85f);
            p.BeginPath(); p.MoveTo(new Vector2(100, 120)); p.LineTo(new Vector2(200, 120));
            p.BezierCurveTo(new Vector2(200, 230), new Vector2(150, 230), new Vector2(140, 280));
            p.LineTo(new Vector2(120, 280));
            p.BezierCurveTo(new Vector2(110, 230), new Vector2(100, 200), new Vector2(100, 120));
            p.Fill();
            
            // U-Bend Pipe
            p.fillColor = new Color(0.15f, 0.15f, 0.15f);
            p.lineWidth = 20f;
            p.lineCap = LineCap.Round;
            p.lineJoin = LineJoin.Round;
            p.strokeColor = new Color(0.1f, 0.1f, 0.1f);
            
            p.BeginPath();
            p.MoveTo(new Vector2(130, 280));
            p.BezierCurveTo(new Vector2(130, 340), new Vector2(190, 340), new Vector2(190, 270));
            p.BezierCurveTo(new Vector2(190, 240), new Vector2(220, 240), new Vector2(220, 280));
            p.LineTo(new Vector2(220, 350));
            p.Stroke();
            
            float clogHealth = 1f - (pumpProgress / 100f);
            if (clogHealth > 0)
            {
                // Clog Blockage
                p.strokeColor = new Color(0.4f, 0.25f, 0.1f); 
                p.lineWidth = 18f;
                float clogWidth = 10f + (clogHealth * 35f);
                p.BeginPath();
                p.MoveTo(new Vector2(160 - clogWidth/2, 315));
                p.LineTo(new Vector2(160 + clogWidth/2, 315));
                p.Stroke();
                
                // Backed up water
                p.fillColor = new Color(0.4f, 0.5f, 0.3f, 0.8f);
                float waterTop = 140 + (80 * (1f - clogHealth)); 
                float slosh = Mathf.Sin(bubbleTime * 20f) * 15f * (pumpProgress / 100f);
                
                p.BeginPath();
                p.MoveTo(new Vector2(100, waterTop + slosh));
                p.LineTo(new Vector2(200, waterTop - slosh));
                p.BezierCurveTo(new Vector2(200, 230), new Vector2(150, 230), new Vector2(140, 280));
                p.LineTo(new Vector2(120, 280));
                p.BezierCurveTo(new Vector2(110, 230), new Vector2(100, 200), new Vector2(100, waterTop + slosh));
                p.Fill();

                // Floating chunks
                p.fillColor = new Color(0.35f, 0.2f, 0.05f);
                Random.State oldState = Random.state;
                Random.InitState(42);
                for(int i=0; i<15; i++) {
                    float bx = Random.Range(110f, 190f);
                    float by = Random.Range(waterTop, 250f);
                    by += Mathf.Sin(bubbleTime * 15f + bx) * 10f;
                    float bSize = Random.Range(5f, 12f);
                    p.BeginPath(); p.Arc(new Vector2(bx, by), bSize, 0, 360); p.Fill();
                }
                Random.state = oldState;
            }
            else
            {
                // MASSIVE FLUID FLUSH
                p.fillColor = new Color(0.2f, 0.6f, 0.9f, 0.9f);
                p.BeginPath();
                p.MoveTo(new Vector2(100, 180)); p.LineTo(new Vector2(200, 180));
                p.BezierCurveTo(new Vector2(200, 230), new Vector2(150, 230), new Vector2(140, 280));
                p.LineTo(new Vector2(120, 280));
                p.BezierCurveTo(new Vector2(110, 230), new Vector2(100, 200), new Vector2(100, 180));
                p.Fill();
                
                p.strokeColor = new Color(0.2f, 0.6f, 0.9f, 0.9f);
                p.lineWidth = 16f;
                p.BeginPath();
                p.MoveTo(new Vector2(130, 280));
                p.BezierCurveTo(new Vector2(130, 340), new Vector2(190, 340), new Vector2(190, 270));
                p.BezierCurveTo(new Vector2(190, 240), new Vector2(220, 240), new Vector2(220, 280));
                p.LineTo(new Vector2(220, 350));
                p.Stroke();
                
                // Bubbles rushing down
                p.fillColor = Color.white;
                for(int i=0; i<30; i++) {
                    float t = (Time.time * 2f + i * 0.1f) % 1f;
                    Vector2 pos = Vector2.Lerp(new Vector2(130, 280), new Vector2(160, 340), t);
                    if (t > 0.5f) pos = Vector2.Lerp(new Vector2(160, 340), new Vector2(220, 350), (t-0.5f)*2f);
                    p.BeginPath(); p.Arc(pos, 3f, 0, 360); p.Fill();
                }
            }

            p.strokeColor = new Color(0.7f, 0.7f, 0.7f);
            p.lineWidth = 2f;
            p.BeginPath(); p.MoveTo(new Vector2(80, 115)); p.LineTo(new Vector2(230, 115)); p.Stroke();
            p.BeginPath(); p.MoveTo(new Vector2(15, 20)); p.LineTo(new Vector2(85, 20)); p.Stroke();
            p.fillColor = new Color(0.8f, 0.8f, 0.8f);
            p.BeginPath(); p.Arc(new Vector2(70, 40), 5, 0, 360); p.Fill();
        }
        else'''

content = re.sub(target_track_draw, replacement_track_draw, content)


target_routine_paint = r'''                if \(pumpNormalBar != null\) \{
                    bubbleTime \+= Time\.deltaTime;
                    pumpNormalBar\.MarkDirtyRepaint\(\);
                \}'''

replacement_routine_paint = r'''                bubbleTime += Time.deltaTime;
                bool isToilet = config != null && config.instructions.ToLower().Contains("toilet");
                if (isToilet && pumpTrackArea != null) {
                    pumpTrackArea.MarkDirtyRepaint();
                } else if (pumpNormalBar != null) {
                    pumpNormalBar.MarkDirtyRepaint();
                }'''

content = re.sub(target_routine_paint, replacement_routine_paint, content)

with open(path, 'w') as f:
    f.write(content)


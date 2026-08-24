import re

path = 'Assets/Scripts/2 Managers/MiniGames/PumpSystemMiniGame.cs'
with open(path, 'r') as f:
    content = f.read()

# 1. Revert StartGame overrides
target_start = r'''            if \(pumpTrackArea != null && isToilet\)
            \{
                pumpTrackArea\.style\.width = 280;
                pumpTrackArea\.style\.backgroundColor = new StyleColor\(Color\.clear\);
                pumpTrackArea\.style\.borderTopWidth = 0; pumpTrackArea\.style\.borderBottomWidth = 0;
                pumpTrackArea\.style\.borderLeftWidth = 0; pumpTrackArea\.style\.borderRightWidth = 0;
            \}'''
content = re.sub(target_start, '', content)

target_start_bar = r'''if \(pumpNormalBar != null\) pumpNormalBar\.style\.display = isToilet \? DisplayStyle\.None : DisplayStyle\.Flex;'''
replacement_start_bar = r'''if (pumpNormalBar != null) pumpNormalBar.style.display = DisplayStyle.Flex;'''
content = re.sub(target_start_bar, replacement_start_bar, content)


# 2. Revert Initialize overrides for Shaft/Handle but keep Plunger aesthetic for the handle!
target_init_shaft = r'''            if \(shaft != null\) \{
                shaft\.style\.width = isToilet \? 16 : 30;
                shaft\.style\.backgroundColor = isToilet \? new StyleColor\(new Color\(0\.7f, 0\.5f, 0\.3f\)\) : new StyleColor\(new Color\(0\.6f, 0\.6f, 0\.6f\)\);
                if \(isToilet\) \{
                    shaft\.style\.left = 150;
                    shaft\.style\.translate = new Translate\(new Length\(-50, LengthUnit\.Percent\), 0\);
                \}
            \}'''
replacement_init_shaft = r'''            if (shaft != null) {
                shaft.style.width = isToilet ? 16 : 30;
                shaft.style.backgroundColor = isToilet ? new StyleColor(new Color(0.7f, 0.5f, 0.3f)) : new StyleColor(new Color(0.6f, 0.6f, 0.6f));
                shaft.style.left = StyleKeyword.Null;
                shaft.style.translate = StyleKeyword.Null;
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
                    pumpHandle\.style\.width = 100;
                    pumpHandle\.style\.left = 150;
                    pumpHandle\.style\.translate = new Translate\(new Length\(-50, LengthUnit\.Percent\), 0\);
                \} else \{
                    pumpHandle\.style\.backgroundColor = new StyleColor\(new Color\(0\.9f, 0\.2f, 0\.2f\)\);
                    pumpHandle\.style\.borderTopWidth = 4; pumpHandle\.style\.borderBottomWidth = 4;
                    pumpHandle\.style\.borderLeftWidth = 4; pumpHandle\.style\.borderRightWidth = 4;
                    pumpHandle\.style\.borderTopColor = new StyleColor\(new Color\(1f, 0\.4f, 0\.4f\)\);
                    pumpHandle\.style\.borderBottomColor = new StyleColor\(new Color\(0\.5f, 0\.1f, 0\.1f\)\);
                \}
            \}'''
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
            }'''
content = re.sub(target_init_handle, replacement_init_handle, content)

target_normalbar_width = r'''pumpNormalBar\.style\.width = 160;'''
replacement_normalbar_width = r'''pumpNormalBar.style.width = isToilet ? 210 : 160;
            pumpNormalBar.style.backgroundColor = isToilet ? new StyleColor(Color.clear) : new StyleColor(new Color(0.05f, 0.1f, 0.05f, 0.8f));
            if (isToilet) {
                pumpNormalBar.style.borderTopWidth = 0; pumpNormalBar.style.borderBottomWidth = 0;
                pumpNormalBar.style.borderLeftWidth = 0; pumpNormalBar.style.borderRightWidth = 0;
            }'''
content = re.sub(target_normalbar_width, replacement_normalbar_width, content)


# 3. Restore original PumpTrack drawing
target_track_draw = r'''    private void OnDrawPumpTrack\(MeshGenerationContext ctx\)
    \{
        var p = ctx\.painter2D;
        float w = ctx\.visualElement\.layout\.width;
        float h = ctx\.visualElement\.layout\.height;
        if \(w == 0 \|\| h == 0\) return;

        bool isToilet = config != null && config\.instructions\.ToLower\(\)\.Contains\("toilet"\);

        if \(isToilet\)
        \{
.*?
        \}
        else
        \{
            p\.fillColor = new Color\(0\.12f, 0\.14f, 0\.16f\);
            p\.BeginPath\(\); p\.MoveTo\(new Vector2\(0, 0\)\); p\.LineTo\(new Vector2\(w, 0\)\); p\.LineTo\(new Vector2\(w, h\)\); p\.LineTo\(new Vector2\(0, h\)\); p\.Fill\(\);

            p\.fillColor = new Color\(0\.9f, 0\.7f, 0\.1f, 0\.3f\);
            for\(float y = -50; y < h; y \+= 40\) \{
                p\.BeginPath\(\); p\.MoveTo\(new Vector2\(0, y\)\); p\.LineTo\(new Vector2\(15, y\)\); p\.LineTo\(new Vector2\(15, y\+20\)\); p\.LineTo\(new Vector2\(0, y\+20\)\); p\.Fill\(\);
                p\.BeginPath\(\); p\.MoveTo\(new Vector2\(w-15, y\)\); p\.LineTo\(new Vector2\(w, y\)\); p\.LineTo\(new Vector2\(w, y\+20\)\); p\.LineTo\(new Vector2\(w-15, y\+20\)\); p\.Fill\(\);
            \}
            
            p\.fillColor = new Color\(0f, 0f, 0f, 0\.5f\);
            p\.BeginPath\(\); p\.MoveTo\(new Vector2\(0, 0\)\); p\.LineTo\(new Vector2\(10, 0\)\); p\.LineTo\(new Vector2\(10, h\)\); p\.LineTo\(new Vector2\(0, h\)\); p\.Fill\(\);
        \}
    \}'''

replacement_track_draw = r'''    private void OnDrawPumpTrack(MeshGenerationContext ctx)
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
    }'''
content = re.sub(target_track_draw, replacement_track_draw, content, flags=re.DOTALL)

# 4. Inject Toilet logic into OnDrawGreaseVat
target_grease_draw = r'''    private void OnDrawGreaseVat\(MeshGenerationContext ctx\)
    \{
        var p = ctx\.painter2D;
        float w = ctx\.visualElement\.layout\.width;
        float h = ctx\.visualElement\.layout\.height;
        if \(w == 0 \|\| h == 0\) return;

        bool isToilet = config != null && config\.instructions\.ToLower\(\)\.Contains\("toilet"\);

        if \(isToilet\)
        \{
.*?
        \}
        else
        \{
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
        \}
    \}'''

replacement_grease_draw = r'''    private void OnDrawGreaseVat(MeshGenerationContext ctx)
    {
        var p = ctx.painter2D;
        float w = ctx.visualElement.layout.width;
        float h = ctx.visualElement.layout.height;
        if (w == 0 || h == 0) return;

        bool isToilet = config != null && config.instructions.ToLower().Contains("toilet");

        if (isToilet)
        {
            // Toilet Body (Light Gray)
            p.fillColor = new Color(0.9f, 0.9f, 0.9f);
            
            // Tank
            p.BeginPath(); p.MoveTo(new Vector2(10, 20)); p.LineTo(new Vector2(60, 20));
            p.LineTo(new Vector2(60, 150)); p.LineTo(new Vector2(10, 150)); p.Fill();
            
            // Bowl and Base
            p.BeginPath(); 
            p.MoveTo(new Vector2(60, 150)); 
            p.LineTo(new Vector2(190, 150)); 
            p.LineTo(new Vector2(190, 170)); 
            p.BezierCurveTo(new Vector2(190, 250), new Vector2(150, 300), new Vector2(130, 330));
            p.LineTo(new Vector2(160, 350)); 
            p.LineTo(new Vector2(40, 350));  
            p.LineTo(new Vector2(40, 250));  
            p.LineTo(new Vector2(60, 150));
            p.Fill();

            // Inner Bowl Cutout
            p.fillColor = new Color(0.8f, 0.8f, 0.8f);
            p.BeginPath(); 
            p.MoveTo(new Vector2(70, 150)); 
            p.LineTo(new Vector2(180, 150));
            p.BezierCurveTo(new Vector2(180, 250), new Vector2(130, 250), new Vector2(110, 280)); 
            p.BezierCurveTo(new Vector2(90, 230), new Vector2(70, 200), new Vector2(70, 150));
            p.Fill();

            // U-Bend Back Cutout (Empty pipe)
            p.strokeColor = new Color(0.6f, 0.6f, 0.6f);
            p.lineWidth = 25f;
            p.lineCap = LineCap.Round;
            p.lineJoin = LineJoin.Round;
            p.BeginPath();
            p.MoveTo(new Vector2(110, 280)); 
            p.BezierCurveTo(new Vector2(110, 350), new Vector2(40, 350), new Vector2(40, 280)); 
            p.BezierCurveTo(new Vector2(40, 220), new Vector2(80, 220), new Vector2(80, 280)); 
            p.LineTo(new Vector2(80, 350)); 
            p.Stroke();
            
            // Details
            p.strokeColor = new Color(0.7f, 0.7f, 0.7f);
            p.lineWidth = 2f;
            p.BeginPath(); p.MoveTo(new Vector2(15, 20)); p.LineTo(new Vector2(65, 20)); p.Stroke(); // Lid
            p.BeginPath(); p.MoveTo(new Vector2(55, 145)); p.LineTo(new Vector2(195, 145)); p.Stroke(); // Seat
            
            float clogHealth = 1f - (pumpProgress / 100f);
            if (clogHealth > 0)
            {
                // Clog
                p.strokeColor = new Color(0.4f, 0.25f, 0.1f);
                p.lineWidth = 25f;
                float clogLen = clogHealth * 20f; 
                p.BeginPath();
                p.MoveTo(new Vector2(110, 280 - clogLen));
                p.LineTo(new Vector2(110, 280 + clogLen));
                p.Stroke();

                // Bowl Water
                p.fillColor = new Color(0.4f, 0.5f, 0.3f, 0.8f); 
                float waterTop = 170 + (40 * (1f - clogHealth)); 
                float slosh = Mathf.Sin(bubbleTime * 20f) * 10f * (pumpProgress / 100f);
                
                p.BeginPath();
                p.MoveTo(new Vector2(75, waterTop + slosh));
                p.LineTo(new Vector2(175, waterTop - slosh));
                p.BezierCurveTo(new Vector2(175, 250), new Vector2(130, 250), new Vector2(110, 280 - clogLen)); 
                p.LineTo(new Vector2(90, 230)); 
                p.BezierCurveTo(new Vector2(80, 210), new Vector2(75, 200), new Vector2(75, waterTop + slosh));
                p.Fill();

                // Floating chunks
                p.fillColor = new Color(0.35f, 0.2f, 0.05f);
                Random.State oldState = Random.state;
                Random.InitState(42);
                for(int i=0; i<12; i++) {
                    float bx = Random.Range(85f, 165f);
                    float by = Random.Range(waterTop + 10f, 240f);
                    by += Mathf.Sin(bubbleTime * 15f + bx) * 10f;
                    float bSize = Random.Range(5f, 10f);
                    p.BeginPath(); p.Arc(new Vector2(bx, by), bSize, 0, 360); p.Fill();
                }
                Random.state = oldState;
            }
            else
            {
                // Flushing animation!
                p.fillColor = new Color(0.2f, 0.6f, 0.9f, 0.8f);
                p.BeginPath();
                p.MoveTo(new Vector2(90, 240));
                p.LineTo(new Vector2(130, 240));
                p.BezierCurveTo(new Vector2(130, 260), new Vector2(120, 270), new Vector2(110, 280));
                p.BezierCurveTo(new Vector2(100, 270), new Vector2(90, 260), new Vector2(90, 240));
                p.Fill();
                
                p.strokeColor = new Color(0.2f, 0.6f, 0.9f, 0.8f);
                p.lineWidth = 18f;
                p.BeginPath();
                p.MoveTo(new Vector2(110, 280)); 
                p.BezierCurveTo(new Vector2(110, 350), new Vector2(40, 350), new Vector2(40, 280)); 
                p.BezierCurveTo(new Vector2(40, 220), new Vector2(80, 220), new Vector2(80, 280)); 
                p.LineTo(new Vector2(80, 350)); 
                p.Stroke();
                
                p.fillColor = Color.white;
                for(int i=0; i<25; i++) {
                    float t = (Time.time * 2f + i * 0.1f) % 1f;
                    Vector2 pos;
                    if (t < 0.4f) { 
                        float nt = t / 0.4f;
                        pos = Vector2.Lerp(new Vector2(110, 280), new Vector2(40, 280), nt); 
                        pos.y += Mathf.Sin(nt * Mathf.PI) * 50f; 
                    } else if (t < 0.8f) { 
                        float nt = (t - 0.4f) / 0.4f;
                        pos = Vector2.Lerp(new Vector2(40, 280), new Vector2(80, 280), nt);
                        pos.y -= Mathf.Sin(nt * Mathf.PI) * 50f; 
                    } else { 
                        float nt = (t - 0.8f) / 0.2f;
                        pos = Vector2.Lerp(new Vector2(80, 280), new Vector2(80, 350), nt);
                    }
                    p.BeginPath(); p.Arc(pos, 3f, 0, 360); p.Fill();
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
    }'''
content = re.sub(target_grease_draw, replacement_grease_draw, content, flags=re.DOTALL)

with open(path, 'w') as f:
    f.write(content)


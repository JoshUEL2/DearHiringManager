import re

path = 'Assets/Scripts/2 Managers/MiniGames/PumpSystemMiniGame.cs'
with open(path, 'r') as f:
    content = f.read()

target_init_handle = r'''            if \(pumpHandle != null\) \{
                pumpHandle\.style\.backgroundColor = new StyleColor\(new Color\(0\.9f, 0\.2f, 0\.2f\)\);
                pumpHandle\.style\.borderTopWidth = 4; pumpHandle\.style\.borderBottomWidth = 4;
                pumpHandle\.style\.borderLeftWidth = 4; pumpHandle\.style\.borderRightWidth = 4;
                pumpHandle\.style\.borderTopColor = new StyleColor\(new Color\(1f, 0\.4f, 0\.4f\)\);
                pumpHandle\.style\.borderBottomColor = new StyleColor\(new Color\(0\.5f, 0\.1f, 0\.1f\)\);
            \}'''

replacement_init_handle = r'''            if (pumpHandle != null) {
                if (isSink) {
                    pumpHandle.style.backgroundColor = new StyleColor(new Color(0.8f, 0.2f, 0.2f));
                    pumpHandle.style.borderTopWidth = 0; pumpHandle.style.borderBottomWidth = 0;
                    pumpHandle.style.borderLeftWidth = 0; pumpHandle.style.borderRightWidth = 0;
                    pumpHandle.style.borderTopLeftRadius = 25;
                    pumpHandle.style.borderTopRightRadius = 25;
                    pumpHandle.style.borderBottomLeftRadius = 5;
                    pumpHandle.style.borderBottomRightRadius = 5;
                    pumpHandle.style.height = 40;
                } else {
                    pumpHandle.style.backgroundColor = new StyleColor(new Color(0.9f, 0.2f, 0.2f));
                    pumpHandle.style.borderTopWidth = 4; pumpHandle.style.borderBottomWidth = 4;
                    pumpHandle.style.borderLeftWidth = 4; pumpHandle.style.borderRightWidth = 4;
                    pumpHandle.style.borderTopColor = new StyleColor(new Color(1f, 0.4f, 0.4f));
                    pumpHandle.style.borderBottomColor = new StyleColor(new Color(0.5f, 0.1f, 0.1f));
                }
            }'''

content = re.sub(target_init_handle, replacement_init_handle, content)


target_grease_draw = r'''        if \(isSink\)
        \{
            p\.fillColor = new Color\(0\.75f, 0\.78f, 0\.8f\);
            p\.BeginPath\(\); p\.MoveTo\(new Vector2\(0, 0\)\); p\.LineTo\(new Vector2\(w, 0\)\); p\.LineTo\(new Vector2\(w, h\)\); p\.LineTo\(new Vector2\(0, h\)\); p\.Fill\(\);
            
            p\.fillColor = new Color\(0\.55f, 0\.6f, 0\.65f\);
            p\.BeginPath\(\); p\.MoveTo\(new Vector2\(15, 15\)\); p\.LineTo\(new Vector2\(w-15, 15\)\); p\.LineTo\(new Vector2\(w-35, h-25\)\); p\.LineTo\(new Vector2\(35, h-25\)\); p\.Fill\(\);

            p\.fillColor = new Color\(0\.1f, 0\.1f, 0\.15f\);
            p\.BeginPath\(\); p\.Arc\(new Vector2\(w/2, h - 35\), 25, 0, 360\); p\.Fill\(\);
            
            p\.strokeColor = new Color\(0\.3f, 0\.3f, 0\.35f\);
            p\.lineWidth = 2f;
            p\.BeginPath\(\); p\.Arc\(new Vector2\(w/2, h - 35\), 25, 0, 360\); p\.Stroke\(\);
        \}
        else
        \{
            p\.fillColor = new Color\(0\.05f, 0\.1f, 0\.05f, 0\.8f\);
            p\.BeginPath\(\); p\.MoveTo\(new Vector2\(0, 0\)\); p\.LineTo\(new Vector2\(w, 0\)\); p\.LineTo\(new Vector2\(w, h\)\); p\.LineTo\(new Vector2\(0, h\)\); p\.Fill\(\);
        \}
        
        float greaseTopY = h \* \(pumpProgress / 100f\);
        
        if \(greaseTopY < h\)
        \{
            if \(isSink\)
            \{
                p\.fillColor = new Color\(0\.3f, 0\.4f, 0\.35f, 0\.85f\);
            \}
            else
            \{
                p\.fillColor = new Color\(0\.35f, 0\.25f, 0\.1f\);
            \}
            
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

            p\.fillColor = isSink \? new Color\(0\.4f, 0\.5f, 0\.3f, 0\.7f\) : new Color\(0\.45f, 0\.35f, 0\.15f, 0\.6f\);
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

        if \(!isSink\)
        \{
            p\.strokeColor = new Color\(1f, 1f, 1f, 0\.2f\);
            p\.lineWidth = 2f;
            for\(float y = h \* 0\.2f; y < h; y \+= h \* 0\.2f\) \{
                p\.BeginPath\(\); p\.MoveTo\(new Vector2\(0, y\)\); p\.LineTo\(new Vector2\(25, y\)\); p\.Stroke\(\);
                p\.BeginPath\(\); p\.MoveTo\(new Vector2\(w, y\)\); p\.LineTo\(new Vector2\(w - 25, y\)\); p\.Stroke\(\);
            \}
            
            p\.fillColor = new Color\(1f, 1f, 1f, 0\.1f\);
            p\.BeginPath\(\); p\.MoveTo\(new Vector2\(5, 5\)\); p\.LineTo\(new Vector2\(20, 5\)\); p\.LineTo\(new Vector2\(20, h-5\)\); p\.LineTo\(new Vector2\(5, h-5\)\); p\.Fill\(\);
        \}'''

replacement_grease_draw = r'''        if (isSink)
        {
            // Tile Wall Background
            p.fillColor = new Color(0.9f, 0.9f, 0.9f);
            p.BeginPath(); p.MoveTo(new Vector2(0, 0)); p.LineTo(new Vector2(w, 0)); p.LineTo(new Vector2(w, h)); p.LineTo(new Vector2(0, h)); p.Fill();
            
            // Pipe Exterior (PVC)
            p.fillColor = new Color(0.8f, 0.8f, 0.85f);
            p.BeginPath();
            p.MoveTo(new Vector2(10, 0)); p.LineTo(new Vector2(10, h-30));
            p.LineTo(new Vector2(25, h-10)); p.LineTo(new Vector2(w-25, h-10));
            p.LineTo(new Vector2(w-10, h-30)); p.LineTo(new Vector2(w-10, 0));
            p.LineTo(new Vector2(w-70, 0)); p.LineTo(new Vector2(w-70, h-70));
            p.LineTo(new Vector2(70, h-70)); p.LineTo(new Vector2(70, 0));
            p.Fill();

            // Pipe Interior
            p.fillColor = new Color(0.15f, 0.15f, 0.15f);
            p.BeginPath();
            p.MoveTo(new Vector2(20, 0)); p.LineTo(new Vector2(20, h-40));
            p.LineTo(new Vector2(30, h-20)); p.LineTo(new Vector2(w-30, h-20));
            p.LineTo(new Vector2(w-20, h-40)); p.LineTo(new Vector2(w-20, 0));
            p.LineTo(new Vector2(w-60, 0)); p.LineTo(new Vector2(w-60, h-60));
            p.LineTo(new Vector2(60, h-60)); p.LineTo(new Vector2(60, 0));
            p.Fill();

            float clogHealth = 1f - (pumpProgress / 100f);
            if (clogHealth > 0)
            {
                // Draw Clog
                p.fillColor = new Color(0.4f, 0.25f, 0.1f); 
                float clogWidth = 40f + (clogHealth * 35f); 
                float cLeft = (w/2) - (clogWidth/2);
                float cRight = (w/2) + (clogWidth/2);
                p.BeginPath();
                p.MoveTo(new Vector2(cLeft, h-60)); p.LineTo(new Vector2(cRight, h-60));
                p.LineTo(new Vector2(cRight, h-20)); p.LineTo(new Vector2(cLeft, h-20));
                p.Fill();
                
                // Backed up water
                p.fillColor = new Color(0.4f, 0.5f, 0.4f, 0.8f);
                float slosh = Mathf.Sin(bubbleTime * 20f) * 15f * (pumpProgress / 100f);
                p.BeginPath();
                p.MoveTo(new Vector2(20, 40 + slosh)); p.LineTo(new Vector2(60, 40 - slosh));
                p.LineTo(new Vector2(60, h-60)); p.LineTo(new Vector2(cLeft, h-60));
                p.LineTo(new Vector2(cLeft, h-20)); p.LineTo(new Vector2(30, h-20));
                p.LineTo(new Vector2(20, h-40));
                p.Fill();
            }
            else
            {
                // Flowing clean water
                p.fillColor = new Color(0.2f, 0.6f, 0.9f, 0.8f);
                p.BeginPath();
                p.MoveTo(new Vector2(20, 0)); p.LineTo(new Vector2(20, h-40));
                p.LineTo(new Vector2(30, h-20)); p.LineTo(new Vector2(w-30, h-20));
                p.LineTo(new Vector2(w-20, h-40)); p.LineTo(new Vector2(w-20, 0));
                p.LineTo(new Vector2(w-60, 0)); p.LineTo(new Vector2(w-60, h-60));
                p.LineTo(new Vector2(60, h-60)); p.LineTo(new Vector2(60, 0));
                p.Fill();
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
        }'''

content = re.sub(target_grease_draw, replacement_grease_draw, content)

with open(path, 'w') as f:
    f.write(content)


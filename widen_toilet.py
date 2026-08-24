import re

path = 'Assets/Scripts/2 Managers/MiniGames/PumpSystemMiniGame.cs'
with open(path, 'r') as f:
    content = f.read()

target_width = r'''pumpNormalBar\.style\.width = isToilet \? 210 : 160;'''
replacement_width = r'''pumpNormalBar.style.width = isToilet ? 350 : 160;'''
content = re.sub(target_width, replacement_width, content)

target_toilet_draw = r'''        if \(isToilet\)
        \{
            // Toilet Body \(Light Gray\)
            p\.fillColor = new Color\(0\.9f, 0\.9f, 0\.9f\);
.*?
            \}
        \}
        else'''

replacement_toilet_draw = r'''        if (isToilet)
        {
            // Background Pipe (Massive U-Bend)
            p.strokeColor = new Color(0.5f, 0.5f, 0.5f);
            p.lineWidth = 45f;
            p.lineCap = LineCap.Round;
            p.lineJoin = LineJoin.Round;
            p.BeginPath();
            p.MoveTo(new Vector2(180, 280)); 
            p.BezierCurveTo(new Vector2(180, 350), new Vector2(60, 350), new Vector2(60, 280)); 
            p.BezierCurveTo(new Vector2(60, 220), new Vector2(120, 220), new Vector2(120, 280)); 
            p.LineTo(new Vector2(120, 350)); 
            p.Stroke();
            
            // Toilet Body
            p.fillColor = new Color(0.92f, 0.92f, 0.92f);
            // Tank
            p.BeginPath(); p.MoveTo(new Vector2(20, 20)); p.LineTo(new Vector2(100, 20));
            p.LineTo(new Vector2(100, 150)); p.LineTo(new Vector2(20, 150)); p.Fill();
            // Bowl
            p.BeginPath(); 
            p.MoveTo(new Vector2(100, 150)); p.LineTo(new Vector2(310, 150)); 
            p.LineTo(new Vector2(310, 180)); 
            p.BezierCurveTo(new Vector2(310, 280), new Vector2(250, 320), new Vector2(220, 350));
            p.LineTo(new Vector2(70, 350)); p.LineTo(new Vector2(70, 250)); p.LineTo(new Vector2(100, 150));
            p.Fill();

            // Inner Bowl
            p.fillColor = new Color(0.82f, 0.82f, 0.82f);
            p.BeginPath(); 
            p.MoveTo(new Vector2(120, 150)); p.LineTo(new Vector2(290, 150));
            p.BezierCurveTo(new Vector2(290, 260), new Vector2(230, 270), new Vector2(180, 280)); 
            p.BezierCurveTo(new Vector2(140, 240), new Vector2(120, 210), new Vector2(120, 150));
            p.Fill();

            // Details
            p.strokeColor = new Color(0.7f, 0.7f, 0.7f);
            p.lineWidth = 2f;
            p.BeginPath(); p.MoveTo(new Vector2(25, 20)); p.LineTo(new Vector2(95, 20)); p.Stroke(); 
            p.BeginPath(); p.MoveTo(new Vector2(110, 145)); p.LineTo(new Vector2(300, 145)); p.Stroke();

            Vector2 GetPipePos(float t) {
                if (t < 0.4f) { 
                    float nt = t / 0.4f;
                    Vector2 pos = Vector2.Lerp(new Vector2(180, 280), new Vector2(60, 280), nt); 
                    pos.y += Mathf.Sin(nt * Mathf.PI) * 70f; 
                    return pos;
                } else if (t < 0.8f) { 
                    float nt = (t - 0.4f) / 0.4f;
                    Vector2 pos = Vector2.Lerp(new Vector2(60, 280), new Vector2(120, 280), nt);
                    pos.y -= Mathf.Sin(nt * Mathf.PI) * 60f; 
                    return pos;
                } else { 
                    float nt = (t - 0.8f) / 0.2f;
                    return Vector2.Lerp(new Vector2(120, 280), new Vector2(120, 350), nt);
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
                p.lineWidth = 40f;
                p.BeginPath(); p.MoveTo(clogStart); p.LineTo(clogEnd); p.Stroke();

                p.fillColor = new Color(0.4f, 0.5f, 0.3f, 0.8f); 
                float waterTop = 170 + (90 * (pumpProgress / 100f)); 
                float slosh = Mathf.Sin(bubbleTime * 25f) * 12f * (pumpProgress / 100f);
                
                p.BeginPath();
                p.MoveTo(new Vector2(125, waterTop + slosh));
                p.LineTo(new Vector2(285, waterTop - slosh));
                p.BezierCurveTo(new Vector2(285, 260), new Vector2(230, 270), new Vector2(180, 280)); 
                p.BezierCurveTo(new Vector2(140, 240), new Vector2(125, 210), new Vector2(125, waterTop + slosh));
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
                p.BeginPath();
                p.MoveTo(new Vector2(145, 260)); p.LineTo(new Vector2(215, 260));
                p.BezierCurveTo(new Vector2(215, 270), new Vector2(200, 280), new Vector2(180, 280));
                p.BezierCurveTo(new Vector2(160, 280), new Vector2(150, 270), new Vector2(145, 260));
                p.Fill();
                
                p.strokeColor = new Color(0.2f, 0.6f, 0.9f, 0.8f);
                p.lineWidth = 35f;
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
        else'''

content = re.sub(target_toilet_draw, replacement_toilet_draw, content, flags=re.DOTALL)

with open(path, 'w') as f:
    f.write(content)

import re

path = 'Assets/Scripts/2 Managers/MiniGames/PumpSystemMiniGame.cs'
with open(path, 'r') as f:
    content = f.read()

target = r'''        if \(isToilet\)
        \{
            // Background Pipe \(Massive U-Bend\)
            p\.strokeColor = new Color\(0\.5f, 0\.5f, 0\.5f\);
.*?
                    p\.BeginPath\(\); p\.Arc\(pos, 5f, 0, 360\); p\.Fill\(\);
                \}
            \}
        \}
        else'''

replacement = r'''        if (isToilet)
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
        else'''

content = re.sub(target, replacement, content, flags=re.DOTALL)

with open(path, 'w') as f:
    f.write(content)


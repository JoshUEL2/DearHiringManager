import re

path = 'Assets/Scripts/2 Managers/MiniGames/PumpSystemMiniGame.cs'
with open(path, 'r') as f:
    content = f.read()

target_draw = r'''            float clogHealth = 1f - \(pumpProgress / 100f\);
            if \(clogHealth > 0\)
            \{
                // Clog
                p\.strokeColor = new Color\(0\.4f, 0\.25f, 0\.1f\);
                p\.lineWidth = 25f;
                float clogLen = clogHealth \* 20f; 
                p\.BeginPath\(\);
                p\.MoveTo\(new Vector2\(110, 280 - clogLen\)\);
                p\.LineTo\(new Vector2\(110, 280 \+ clogLen\)\);
                p\.Stroke\(\);

                // Bowl Water
                p\.fillColor = new Color\(0\.4f, 0\.5f, 0\.3f, 0\.8f\); 
                float waterTop = 170 \+ \(40 \* \(1f - clogHealth\)\); 
                float slosh = Mathf\.Sin\(bubbleTime \* 20f\) \* 10f \* \(pumpProgress / 100f\);
                
                p\.BeginPath\(\);
                p\.MoveTo\(new Vector2\(75, waterTop \+ slosh\)\);
                p\.LineTo\(new Vector2\(175, waterTop - slosh\)\);
                p\.BezierCurveTo\(new Vector2\(175, 250\), new Vector2\(130, 250\), new Vector2\(110, 280 - clogLen\)\); 
                p\.LineTo\(new Vector2\(90, 230\)\); 
                p\.BezierCurveTo\(new Vector2\(80, 210\), new Vector2\(75, 200\), new Vector2\(75, waterTop \+ slosh\)\);
                p\.Fill\(\);

                // Floating chunks
                p\.fillColor = new Color\(0\.35f, 0\.2f, 0\.05f\);
                Random\.State oldState = Random\.state;
                Random\.InitState\(42\);
                for\(int i=0; i<12; i\+\+\) \{
                    float bx = Random\.Range\(85f, 165f\);
                    float by = Random\.Range\(waterTop \+ 10f, 240f\);
                    by \+= Mathf\.Sin\(bubbleTime \* 15f \+ bx\) \* 10f;
                    float bSize = Random\.Range\(5f, 10f\);
                    p\.BeginPath\(\); p\.Arc\(new Vector2\(bx, by\), bSize, 0, 360\); p\.Fill\(\);
                \}
                Random\.state = oldState;
            \}
            else
            \{
                // Flushing animation!
                p\.fillColor = new Color\(0\.2f, 0\.6f, 0\.9f, 0\.8f\);
                p\.BeginPath\(\);
                p\.MoveTo\(new Vector2\(90, 240\)\);
                p\.LineTo\(new Vector2\(130, 240\)\);
                p\.BezierCurveTo\(new Vector2\(130, 260\), new Vector2\(120, 270\), new Vector2\(110, 280\)\);
                p\.BezierCurveTo\(new Vector2\(100, 270\), new Vector2\(90, 260\), new Vector2\(90, 240\)\);
                p\.Fill\(\);
                
                p\.strokeColor = new Color\(0\.2f, 0\.6f, 0\.9f, 0\.8f\);
                p\.lineWidth = 18f;
                p\.BeginPath\(\);
                p\.MoveTo\(new Vector2\(110, 280\)\); 
                p\.BezierCurveTo\(new Vector2\(110, 350\), new Vector2\(40, 350\), new Vector2\(40, 280\)\); 
                p\.BezierCurveTo\(new Vector2\(40, 220\), new Vector2\(80, 220\), new Vector2\(80, 280\)\); 
                p\.LineTo\(new Vector2\(80, 350\)\); 
                p\.Stroke\(\);
                
                p\.fillColor = Color\.white;
                for\(int i=0; i<25; i\+\+\) \{
                    float t = \(Time\.time \* 2f \+ i \* 0\.1f\) % 1f;
                    Vector2 pos;
                    if \(t < 0\.4f\) \{ 
                        float nt = t / 0\.4f;
                        pos = Vector2\.Lerp\(new Vector2\(110, 280\), new Vector2\(40, 280\), nt\); 
                        pos\.y \+= Mathf\.Sin\(nt \* Mathf\.PI\) \* 50f; 
                    \} else if \(t < 0\.8f\) \{ 
                        float nt = \(t - 0\.4f\) / 0\.4f;
                        pos = Vector2\.Lerp\(new Vector2\(40, 280\), new Vector2\(80, 280\), nt\);
                        pos\.y -= Mathf\.Sin\(nt \* Mathf\.PI\) \* 50f; 
                    \} else \{ 
                        float nt = \(t - 0\.8f\) / 0\.2f;
                        pos = Vector2\.Lerp\(new Vector2\(80, 280\), new Vector2\(80, 350\), nt\);
                    \}
                    p\.BeginPath\(\); p\.Arc\(pos, 3f, 0, 360\); p\.Fill\(\);
                \}
            \}'''

replacement_draw = r'''            Vector2 GetPipePos(float t) {
                if (t < 0.4f) { 
                    float nt = t / 0.4f;
                    Vector2 pos = Vector2.Lerp(new Vector2(110, 280), new Vector2(40, 280), nt); 
                    pos.y += Mathf.Sin(nt * Mathf.PI) * 50f; 
                    return pos;
                } else if (t < 0.8f) { 
                    float nt = (t - 0.4f) / 0.4f;
                    Vector2 pos = Vector2.Lerp(new Vector2(40, 280), new Vector2(80, 280), nt);
                    pos.y -= Mathf.Sin(nt * Mathf.PI) * 50f; 
                    return pos;
                } else { 
                    float nt = (t - 0.8f) / 0.2f;
                    return Vector2.Lerp(new Vector2(80, 280), new Vector2(80, 350), nt);
                }
            }

            float clogHealth = 1f - (pumpProgress / 100f);
            if (clogHealth > 0)
            {
                float pipeT = pumpProgress / 100f;
                Vector2 clogStart = GetPipePos(Mathf.Clamp01(pipeT - 0.05f));
                Vector2 clogEnd = GetPipePos(Mathf.Clamp01(pipeT + 0.05f));

                // Backed up water IN THE PIPE (trails the clog)
                p.strokeColor = new Color(0.4f, 0.5f, 0.3f, 0.8f);
                p.lineWidth = 18f;
                p.lineCap = LineCap.Round;
                p.BeginPath();
                p.MoveTo(new Vector2(110, 280));
                for (float pt = 0; pt <= pipeT - 0.05f; pt += 0.05f) {
                    p.LineTo(GetPipePos(pt));
                }
                p.LineTo(clogStart);
                p.Stroke();

                // Clog
                p.strokeColor = new Color(0.4f, 0.25f, 0.1f);
                p.lineWidth = 22f;
                p.BeginPath();
                p.MoveTo(clogStart);
                p.LineTo(clogEnd);
                p.Stroke();

                // Bowl Water (Physically drops as it drains!)
                p.fillColor = new Color(0.4f, 0.5f, 0.3f, 0.8f); 
                float waterTop = 170 + (70 * (pumpProgress / 100f)); 
                float slosh = Mathf.Sin(bubbleTime * 25f) * 12f * (pumpProgress / 100f);
                
                p.BeginPath();
                p.MoveTo(new Vector2(75, waterTop + slosh));
                p.LineTo(new Vector2(175, waterTop - slosh));
                p.BezierCurveTo(new Vector2(175, 250), new Vector2(130, 250), new Vector2(110, 280)); 
                p.LineTo(new Vector2(90, 230)); 
                p.BezierCurveTo(new Vector2(80, 210), new Vector2(75, 200), new Vector2(75, waterTop + slosh));
                p.Fill();

                // Floating chunks in bowl
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
                // Flushing animation! Water level in bowl sits at base (240)
                p.fillColor = new Color(0.2f, 0.6f, 0.9f, 0.8f);
                p.BeginPath();
                p.MoveTo(new Vector2(90, 240));
                p.LineTo(new Vector2(130, 240));
                p.BezierCurveTo(new Vector2(130, 260), new Vector2(120, 270), new Vector2(110, 280));
                p.BezierCurveTo(new Vector2(100, 270), new Vector2(90, 260), new Vector2(90, 240));
                p.Fill();
                
                // Clear water surging through pipe
                p.strokeColor = new Color(0.2f, 0.6f, 0.9f, 0.8f);
                p.lineWidth = 18f;
                p.BeginPath();
                p.MoveTo(new Vector2(110, 280)); 
                p.BezierCurveTo(new Vector2(110, 350), new Vector2(40, 350), new Vector2(40, 280)); 
                p.BezierCurveTo(new Vector2(40, 220), new Vector2(80, 220), new Vector2(80, 280)); 
                p.LineTo(new Vector2(80, 350)); 
                p.Stroke();
                
                p.fillColor = Color.white;
                for(int i=0; i<30; i++) {
                    float t = (Time.time * 2.5f + i * 0.1f) % 1f;
                    Vector2 pos = GetPipePos(t);
                    p.BeginPath(); p.Arc(pos, 3f, 0, 360); p.Fill();
                }
            }'''
content = re.sub(target_draw, replacement_draw, content)

target_win_close = r'''    private IEnumerator ShowWinFeedbackAndClose\(\)
    \{
        string stampString = config\.pumpIsBalanceMode \? "PERFECT TEMP!" : "CLEARED!";'''

replacement_win_close = r'''    private IEnumerator ShowWinFeedbackAndClose()
    {
        bool isToilet = config != null && config.instructions.ToLower().Contains("toilet");
        if (isToilet) {
            yield return new WaitForSeconds(1.0f);
        }
        string stampString = config.pumpIsBalanceMode ? "PERFECT TEMP!" : "CLEARED!";'''
content = re.sub(target_win_close, replacement_win_close, content)

with open(path, 'w') as f:
    f.write(content)

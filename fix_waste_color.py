import re

path = 'Assets/Scripts/2 Managers/MiniGames/PumpSystemMiniGame.cs'
with open(path, 'r') as f:
    content = f.read()

draw_target = r'''            if \(!config\.pumpIsBalanceMode\)
            \{
                float levelY = h - \(currentPumpLevel \* h\);
                p\.fillColor = new Color\(0\.35f, 0\.25f, 0\.1f\);
                p\.BeginPath\(\); p\.MoveTo\(new Vector2\(0, levelY\)\); p\.LineTo\(new Vector2\(w, levelY\)\); p\.LineTo\(new Vector2\(w, h\)\); p\.LineTo\(new Vector2\(0, h\)\); p\.Fill\(\);

                // Bubbles / Scum
                Random\.InitState\((?:42|123)\);
                for \(int i = 0; i < 8; i\+\+\)
                \{
                    float bx = Random\.Range\(10f, w - 10f\);
                    float by = levelY \+ Random\.Range\(0f, 40f\);
                    float r = Random\.Range\(5f, 15f\) \+ Mathf\.Sin\(Time\.time \* 5f \+ i\) \* 2f;
                    if \(by < h\) \{
                        p\.fillColor = new Color\(0\.45f, 0\.35f, 0\.15f, 0\.6f\);
                        p\.BeginPath\(\); p\.Arc\(new Vector2\(bx, by\), r, 0, 360\); p\.Fill\(\);
                    \}
                \}
            \}'''

draw_replacement = r'''            if (!config.pumpIsBalanceMode)
            {
                float levelY = h - (currentPumpLevel * h);
                
                Color baseWasteColor = new Color(0.35f, 0.25f, 0.1f);
                Color scumWasteColor = new Color(0.45f, 0.35f, 0.15f, 0.6f);
                
                if (manager != null && manager.currentGame != null) {
                    string n = manager.currentGame.name.ToLower();
                    if (n.Contains("waste")) {
                        // Toxic green waste
                        baseWasteColor = new Color(0.2f, 0.8f, 0.1f);
                        scumWasteColor = new Color(0.4f, 0.9f, 0.2f, 0.6f);
                    } else if (n.Contains("oil") || n.Contains("grease")) {
                        // Thick black sludge
                        baseWasteColor = new Color(0.08f, 0.08f, 0.08f);
                        scumWasteColor = new Color(0.15f, 0.15f, 0.15f, 0.6f);
                    }
                }

                p.fillColor = baseWasteColor;
                p.BeginPath(); p.MoveTo(new Vector2(0, levelY)); p.LineTo(new Vector2(w, levelY)); p.LineTo(new Vector2(w, h)); p.LineTo(new Vector2(0, h)); p.Fill();

                // Bubbles / Scum
                Random.InitState(42);
                for (int i = 0; i < 8; i++)
                {
                    float bx = Random.Range(10f, w - 10f);
                    float by = levelY + Random.Range(0f, 40f);
                    float r = Random.Range(5f, 15f) + Mathf.Sin(Time.time * 5f + i) * 2f;
                    if (by < h) {
                        p.fillColor = scumWasteColor;
                        p.BeginPath(); p.Arc(new Vector2(bx, by), r, 0, 360); p.Fill();
                    }
                }
            }'''

content = re.sub(draw_target, draw_replacement, content)

with open(path, 'w') as f:
    f.write(content)

import re

path = 'Assets/Scripts/2 Managers/MiniGames/PumpSystemMiniGame.cs'
with open(path, 'r') as f:
    content = f.read()

target_flange = r'''            p\.fillColor = new Color\(0\.85f, 0\.85f, 0\.85f\);
            p\.BeginPath\(\);
            p\.MoveTo\(new Vector2\(100, 350\)\); p\.LineTo\(new Vector2\(100, 320\)\);'''

replacement_flange = r'''            p.fillColor = new Color(0.85f, 0.85f, 0.85f);
            p.BeginPath(); p.MoveTo(new Vector2(90, 345)); p.LineTo(new Vector2(230, 345));
            p.LineTo(new Vector2(235, 355)); p.LineTo(new Vector2(85, 355)); p.Fill();
            
            p.fillColor = new Color(0.85f, 0.85f, 0.85f);
            p.BeginPath();
            p.MoveTo(new Vector2(100, 350)); p.LineTo(new Vector2(100, 320));'''

content = re.sub(target_flange, replacement_flange, content)

target_water = r'''                p\.fillColor = new Color\(0\.4f, 0\.5f, 0\.3f, 0\.8f\); 
                float waterTop = 170 \+ \(90 \* \(pumpProgress / 100f\)\); 
                float slosh = Mathf\.Sin\(bubbleTime \* 25f\) \* 12f \* \(pumpProgress / 100f\);
                
                p\.BeginPath\(\);
                p\.MoveTo\(new Vector2\(125, waterTop \+ slosh\)\);
                p\.LineTo\(new Vector2\(285, waterTop - slosh\)\);
                p\.BezierCurveTo\(new Vector2\(285, 260\), new Vector2\(230, 270\), new Vector2\(180, 280\)\); 
                p\.BezierCurveTo\(new Vector2\(140, 240\), new Vector2\(125, 210\), new Vector2\(125, waterTop \+ slosh\)\);
                p\.Fill\(\);'''

replacement_water = r'''                p.fillColor = new Color(0.4f, 0.5f, 0.3f, 0.8f); 
                float waterTop = 170 + (90 * (pumpProgress / 100f)); 
                float slosh = Mathf.Sin(bubbleTime * 25f) * 12f * (pumpProgress / 100f);
                
                float wT = Mathf.Clamp01((waterTop - 150f) / 130f);
                Vector2 lPt = CubicBezier(new Vector2(130, 150), new Vector2(130, 200), new Vector2(140, 260), new Vector2(180, 280), wT);
                Vector2 rPt = CubicBezier(new Vector2(300, 150), new Vector2(300, 220), new Vector2(250, 270), new Vector2(180, 280), wT);
                
                p.BeginPath();
                p.MoveTo(new Vector2(lPt.x, lPt.y + slosh));
                p.LineTo(new Vector2(rPt.x, rPt.y - slosh));
                for (float t = wT; t <= 1f; t += 0.05f) {
                    p.LineTo(CubicBezier(new Vector2(300, 150), new Vector2(300, 220), new Vector2(250, 270), new Vector2(180, 280), t));
                }
                for (float t = 1f; t >= wT; t -= 0.05f) {
                    p.LineTo(CubicBezier(new Vector2(130, 150), new Vector2(130, 200), new Vector2(140, 260), new Vector2(180, 280), t));
                }
                p.LineTo(new Vector2(lPt.x, lPt.y + slosh));
                p.Fill();'''

content = re.sub(target_water, replacement_water, content)

target_flush = r'''                p\.fillColor = new Color\(0\.2f, 0\.6f, 0\.9f, 0\.8f\);
                p\.BeginPath\(\);
                p\.MoveTo\(new Vector2\(145, 260\)\); p\.LineTo\(new Vector2\(215, 260\)\);
                p\.BezierCurveTo\(new Vector2\(215, 270\), new Vector2\(200, 280\), new Vector2\(180, 280\)\);
                p\.BezierCurveTo\(new Vector2\(160, 280\), new Vector2\(150, 270\), new Vector2\(145, 260\)\);
                p\.Fill\(\);'''

replacement_flush = r'''                p.fillColor = new Color(0.2f, 0.6f, 0.9f, 0.8f);
                float fT = Mathf.Clamp01((260f - 150f) / 130f);
                Vector2 flPt = CubicBezier(new Vector2(130, 150), new Vector2(130, 200), new Vector2(140, 260), new Vector2(180, 280), fT);
                Vector2 frPt = CubicBezier(new Vector2(300, 150), new Vector2(300, 220), new Vector2(250, 270), new Vector2(180, 280), fT);
                
                p.BeginPath();
                p.MoveTo(flPt);
                p.LineTo(frPt);
                for (float t = fT; t <= 1f; t += 0.05f) {
                    p.LineTo(CubicBezier(new Vector2(300, 150), new Vector2(300, 220), new Vector2(250, 270), new Vector2(180, 280), t));
                }
                for (float t = 1f; t >= fT; t -= 0.05f) {
                    p.LineTo(CubicBezier(new Vector2(130, 150), new Vector2(130, 200), new Vector2(140, 260), new Vector2(180, 280), t));
                }
                p.Fill();'''

content = re.sub(target_flush, replacement_flush, content)

with open(path, 'w') as f:
    f.write(content)

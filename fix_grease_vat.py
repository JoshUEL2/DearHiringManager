import re

path = 'Assets/Scripts/2 Managers/MiniGames/PumpSystemMiniGame.cs'
with open(path, 'r') as f:
    content = f.read()

target = r'''        float greaseHeight = h \* \(1f - \(pumpProgress / 100f\)\);
        
        if \(greaseHeight > 0\)
        \{
            p\.fillColor = new Color\(0\.35f, 0\.25f, 0\.1f\);
            
            p\.BeginPath\(\);
            p\.MoveTo\(new Vector2\(0, h\)\);
            p\.LineTo\(new Vector2\(w, h\)\);
            p\.LineTo\(new Vector2\(w, greaseHeight\)\);
            
            int segments = 10;
            float segWidth = w / segments;
            for \(int i = segments - 1; i >= 0; i--\)
            \{
                float px = i \* segWidth;
                float py = greaseHeight \+ Mathf\.Sin\(bubbleTime \* 4f \+ px \* 0\.1f\) \* 12f;
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
                float by = h - \(\(bubbleTime \* bSpeed \+ Random\.Range\(0, h\)\) % h\);
                if \(by > greaseHeight\) \{
                    float bSize = Random\.Range\(4f, 15f\);
                    p\.BeginPath\(\); p\.Arc\(new Vector2\(bx, by\), bSize, 0, 360\); p\.Fill\(\);
                \}
            \}
            Random\.state = oldState;
        \}'''

replacement = r'''        float greaseTopY = h * (pumpProgress / 100f);
        
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
        }'''

content = re.sub(target, replacement, content)

with open(path, 'w') as f:
    f.write(content)

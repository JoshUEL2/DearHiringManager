import re

path = 'Assets/Scripts/2 Managers/MiniGames/ScrubPlateMiniGame.cs'
with open(path, 'r') as f:
    content = f.read()

# Update dirt particles
dirt_target = r'''        if \(currentTheme == ScrubTheme\.WasteContainer\)
        \{
            // Thick sludge inside a circular bin
            for \(int i = 0; i < 600; i\+\+\) \{
                Vector2 pos = new Vector2\(200, 200\) \+ Random\.insideUnitCircle \* 170f;
                dirtParticles\.Add\(pos\);
                dirtSizes\.Add\(Random\.Range\(15f, 35f\)\);
            \}
        \}'''

dirt_replacement = r'''        if (currentTheme == ScrubTheme.WasteContainer)
        {
            // Sludge plastered over the front face of the wheelie bin
            for (int i = 0; i < 500; i++) {
                Vector2 pos = new Vector2(Random.Range(85, 315), Random.Range(85, 325));
                dirtParticles.Add(pos);
                dirtSizes.Add(Random.Range(10f, 28f));
            }
        }'''
content = re.sub(dirt_target, dirt_replacement, content)

# Update drawing logic
draw_target = r'''        if \(currentTheme == ScrubTheme\.WasteContainer\)
        \{
            // Dark concrete background
            p\.fillColor = new Color\(0\.15f, 0\.15f, 0\.15f\);
            p\.BeginPath\(\); p\.MoveTo\(new Vector2\(0,0\)\); p\.LineTo\(new Vector2\(w,0\)\); p\.LineTo\(new Vector2\(w,h\)\); p\.LineTo\(new Vector2\(0,h\)\); p\.Fill\(\);
            
            // Deep green plastic bin rim
            p\.fillColor = new Color\(0\.1f, 0\.4f, 0\.1f\);
            p\.BeginPath\(\); p\.Arc\(new Vector2\(200, 200\), 190, 0, 360\); p\.Fill\(\);
            
            // Inner deep bin darkness
            p\.fillColor = new Color\(0\.05f, 0\.25f, 0\.05f\);
            p\.BeginPath\(\); p\.Arc\(new Vector2\(200, 200\), 175, 0, 360\); p\.Fill\(\);
        \}'''

draw_replacement = r'''        if (currentTheme == ScrubTheme.WasteContainer)
        {
            // Concrete Wall
            p.fillColor = new Color(0.2f, 0.2f, 0.2f); 
            p.BeginPath(); p.MoveTo(new Vector2(0,0)); p.LineTo(new Vector2(w,0)); p.LineTo(new Vector2(w,250)); p.LineTo(new Vector2(0,250)); p.Fill();
            
            // Concrete Floor
            p.fillColor = new Color(0.3f, 0.3f, 0.3f); 
            p.BeginPath(); p.MoveTo(new Vector2(0,250)); p.LineTo(new Vector2(w,250)); p.LineTo(new Vector2(w,h)); p.LineTo(new Vector2(0,h)); p.Fill();
            
            // Dirt in corners of wall
            p.fillColor = new Color(0.15f, 0.15f, 0.15f);
            p.BeginPath(); p.MoveTo(new Vector2(0,250)); p.LineTo(new Vector2(w,250)); p.LineTo(new Vector2(w,h)); p.LineTo(new Vector2(0,h)); p.Fill();
            
            // Wheels (Black thick tires)
            p.fillColor = new Color(0.05f, 0.05f, 0.05f);
            p.BeginPath(); p.Arc(new Vector2(100, 340), 30, 0, 360); p.Fill();
            p.BeginPath(); p.Arc(new Vector2(300, 340), 30, 0, 360); p.Fill();
            
            // Wheel axles/hubs
            p.fillColor = new Color(0.6f, 0.6f, 0.6f);
            p.BeginPath(); p.Arc(new Vector2(100, 340), 10, 0, 360); p.Fill();
            p.BeginPath(); p.Arc(new Vector2(300, 340), 10, 0, 360); p.Fill();

            // The Lid (Open, leaning back)
            p.fillColor = new Color(0.1f, 0.3f, 0.15f);
            p.BeginPath(); p.MoveTo(new Vector2(100, 20)); p.LineTo(new Vector2(300, 20)); 
            p.LineTo(new Vector2(320, 80)); p.LineTo(new Vector2(80, 80)); p.Fill();

            // Bin Body
            p.fillColor = new Color(0.15f, 0.5f, 0.2f); // Bright Wheelie Bin Green
            p.BeginPath(); p.MoveTo(new Vector2(80, 80)); p.LineTo(new Vector2(320, 80)); 
            p.LineTo(new Vector2(310, 330)); p.LineTo(new Vector2(90, 330)); p.Fill();
            
            // Bin Shadow / Bevels
            p.fillColor = new Color(0.1f, 0.4f, 0.15f); 
            p.BeginPath(); p.MoveTo(new Vector2(80, 80)); p.LineTo(new Vector2(110, 80)); 
            p.LineTo(new Vector2(120, 330)); p.LineTo(new Vector2(90, 330)); p.Fill();
            
            // Plastic Ribbing
            p.strokeColor = new Color(0.1f, 0.35f, 0.15f);
            p.lineWidth = 15f;
            for(int rx = 150; rx <= 250; rx += 50) {
                p.BeginPath(); p.MoveTo(new Vector2(rx, 110)); p.LineTo(new Vector2(rx, 300)); p.Stroke();
            }
            
            // Lip at top
            p.fillColor = new Color(0.12f, 0.4f, 0.18f);
            p.BeginPath(); p.MoveTo(new Vector2(75, 80)); p.LineTo(new Vector2(325, 80)); 
            p.LineTo(new Vector2(325, 100)); p.LineTo(new Vector2(75, 100)); p.Fill();
        }'''
content = re.sub(draw_target, draw_replacement, content)

with open(path, 'w') as f:
    f.write(content)

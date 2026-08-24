import re

path = 'Assets/Scripts/2 Managers/MiniGames/ScrubPlateMiniGame.cs'
with open(path, 'r') as f:
    content = f.read()

# 1. Add WasteContainer to enum
enum_target = r'''    private enum ScrubTheme \{ Plate, Microwave, Arrow, Grill \}'''
enum_replacement = r'''    private enum ScrubTheme { Plate, Microwave, Arrow, Grill, WasteContainer }'''
content = re.sub(enum_target, enum_replacement, content)

# 2. Setup theme selection
setup_target = r'''        if \(gameName == "Scour microwave grease"\) \{ currentTheme = ScrubTheme\.Microwave; scraperLabel\.text = "🧽"; \}
        else if \(gameName == "Wipe smog off the neon arrow"\) \{ currentTheme = ScrubTheme\.Arrow; scraperLabel\.text = "🧻"; \}
        else if \(gameName\.Contains\("grill"\)\) \{ currentTheme = ScrubTheme\.Grill; scraperLabel\.text = "🔪"; \} 
        else \{ currentTheme = ScrubTheme\.Plate; scraperLabel\.text = "🧽"; \}'''

setup_replacement = r'''        if (gameName == "Scour microwave grease") { currentTheme = ScrubTheme.Microwave; scraperLabel.text = "🧽"; }
        else if (gameName == "Wipe smog off the neon arrow") { currentTheme = ScrubTheme.Arrow; scraperLabel.text = "🧻"; }
        else if (gameName.Contains("grill")) { currentTheme = ScrubTheme.Grill; scraperLabel.text = "🔪"; } 
        else if (gameName.Contains("waste")) { currentTheme = ScrubTheme.WasteContainer; scraperLabel.text = "🚿"; }
        else { currentTheme = ScrubTheme.Plate; scraperLabel.text = "🧽"; }'''
content = re.sub(setup_target, setup_replacement, content)

# 3. Dirt particle setup
dirt_target = r'''        if \(currentTheme == ScrubTheme\.Grill\)
        \{
            // Dense rectangular grid for massive solid carbonized grease layer
            for \(int x = 10; x < 390; x \+= 12\) \{'''

dirt_replacement = r'''        if (currentTheme == ScrubTheme.WasteContainer)
        {
            // Thick sludge inside a circular bin
            for (int i = 0; i < 600; i++) {
                Vector2 pos = new Vector2(200, 200) + Random.insideUnitCircle * 170f;
                dirtParticles.Add(pos);
                dirtSizes.Add(Random.Range(15f, 35f));
            }
        }
        else if (currentTheme == ScrubTheme.Grill)
        {
            // Dense rectangular grid for massive solid carbonized grease layer
            for (int x = 10; x < 390; x += 12) {'''
content = re.sub(dirt_target, dirt_replacement, content)

# 4. Scrub radius & Water particles
radius_target = r'''        float scrubRadius = currentTheme == ScrubTheme\.Grill \? 40f : 35f;'''
radius_replacement = r'''        float scrubRadius = currentTheme == ScrubTheme.Grill ? 40f : (currentTheme == ScrubTheme.WasteContainer ? 50f : 35f);'''
content = re.sub(radius_target, radius_replacement, content)

sparks_target = r'''            if \(currentTheme == ScrubTheme\.Grill\)
            \{
                for \(int j = 0; j < 3; j\+\+\) \{
                    sparks\.Add\(new Spark \{
                        pos = pos \+ new Vector2\(Random\.Range\(-15f, 15f\), Random\.Range\(-15f, 15f\)\),
                        vel = new Vector2\(Random\.Range\(-300f, 300f\), Random\.Range\(-400f, -100f\)\),
                        life = 1f
                    \}\);
                \}
            \}
            drawArea\.MarkDirtyRepaint\(\);'''

sparks_replacement = r'''            if (currentTheme == ScrubTheme.Grill)
            {
                for (int j = 0; j < 3; j++) {
                    sparks.Add(new Spark {
                        pos = pos + new Vector2(Random.Range(-15f, 15f), Random.Range(-15f, 15f)),
                        vel = new Vector2(Random.Range(-300f, 300f), Random.Range(-400f, -100f)),
                        life = 1f
                    });
                }
            }
            else if (currentTheme == ScrubTheme.WasteContainer)
            {
                // Spraying water!
                for (int j = 0; j < 4; j++) {
                    sparks.Add(new Spark {
                        pos = pos + new Vector2(Random.Range(-25f, 25f), Random.Range(-25f, 25f)),
                        vel = new Vector2(Random.Range(-500f, 500f), Random.Range(-500f, 500f)), // blasts in all directions
                        life = 0.8f
                    });
                }
            }
            drawArea.MarkDirtyRepaint();'''
content = re.sub(sparks_target, sparks_replacement, content)

# 5. Base drawing
drawbase_target = r'''        // Draw Base Object
        if \(currentTheme == ScrubTheme\.Grill\)'''
drawbase_replacement = r'''        // Draw Base Object
        if (currentTheme == ScrubTheme.WasteContainer)
        {
            // Dark concrete background
            p.fillColor = new Color(0.15f, 0.15f, 0.15f);
            p.BeginPath(); p.MoveTo(new Vector2(0,0)); p.LineTo(new Vector2(w,0)); p.LineTo(new Vector2(w,h)); p.LineTo(new Vector2(0,h)); p.Fill();
            
            // Deep green plastic bin rim
            p.fillColor = new Color(0.1f, 0.4f, 0.1f);
            p.BeginPath(); p.Arc(new Vector2(200, 200), 190, 0, 360); p.Fill();
            
            // Inner deep bin darkness
            p.fillColor = new Color(0.05f, 0.25f, 0.05f);
            p.BeginPath(); p.Arc(new Vector2(200, 200), 175, 0, 360); p.Fill();
        }
        else if (currentTheme == ScrubTheme.Grill)'''
content = re.sub(drawbase_target, drawbase_replacement, content)

# 6. Dirt color
dirtcolor_target = r'''        // Draw Dirt Particles
        Color dirtColor = Color\.black;
        if \(currentTheme == ScrubTheme\.Plate\) dirtColor = new Color\(0\.4f, 0\.2f, 0\.1f\);
        else if \(currentTheme == ScrubTheme\.Arrow\) dirtColor = new Color\(0\.4f, 0\.4f, 0\.4f, 0\.8f\);
        else if \(currentTheme == ScrubTheme\.Microwave\) dirtColor = new Color\(0\.8f, 0\.7f, 0\.2f, 0\.8f\);
        else dirtColor = new Color\(0\.08f, 0\.08f, 0\.08f\); // Pitch black carbon'''
dirtcolor_replacement = r'''        // Draw Dirt Particles
        Color dirtColor = Color.black;
        if (currentTheme == ScrubTheme.Plate) dirtColor = new Color(0.4f, 0.2f, 0.1f);
        else if (currentTheme == ScrubTheme.Arrow) dirtColor = new Color(0.4f, 0.4f, 0.4f, 0.8f);
        else if (currentTheme == ScrubTheme.Microwave) dirtColor = new Color(0.8f, 0.7f, 0.2f, 0.8f);
        else if (currentTheme == ScrubTheme.WasteContainer) dirtColor = new Color(0.35f, 0.25f, 0.1f); // Gross brown sludge
        else dirtColor = new Color(0.08f, 0.08f, 0.08f); // Pitch black carbon'''
content = re.sub(dirtcolor_target, dirtcolor_replacement, content)

# 7. Draw Sparks/Water
drawspark_target = r'''        // Draw Sparks
        if \(currentTheme == ScrubTheme\.Grill\)
        \{
            foreach \(var s in sparks\)
            \{
                p\.fillColor = new Color\(1f, 0\.6f, 0f, s\.life\);
                p\.BeginPath\(\); p\.Arc\(s\.pos, 4f \* s\.life, 0, 360\); p\.Fill\(\);
            \}
        \}'''
drawspark_replacement = r'''        // Draw Sparks
        if (currentTheme == ScrubTheme.Grill)
        {
            foreach (var s in sparks)
            {
                p.fillColor = new Color(1f, 0.6f, 0f, s.life);
                p.BeginPath(); p.Arc(s.pos, 4f * s.life, 0, 360); p.Fill();
            }
        }
        else if (currentTheme == ScrubTheme.WasteContainer)
        {
            foreach (var s in sparks)
            {
                p.fillColor = new Color(0.2f, 0.7f, 1f, s.life); // High pressure water
                p.BeginPath(); p.Arc(s.pos, 5f * s.life, 0, 360); p.Fill();
            }
        }'''
content = re.sub(drawspark_target, drawspark_replacement, content)


with open(path, 'w') as f:
    f.write(content)

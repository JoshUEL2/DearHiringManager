path = 'Assets/Scripts/2 Managers/MiniGames/DragMatchMiniGame.cs'

with open(path, 'r') as f:
    content = f.read()

# 1. Remove the old blue lines on Labels
target_label = """            lbl.style.paddingBottom = 8;
            lbl.style.paddingTop = 8;
            lbl.style.paddingLeft = 80; // FIX TEXT ALIGNMENT OVER RED LINE!
            lbl.style.borderBottomWidth = 1;
            lbl.style.borderBottomColor = new StyleColor(new Color(0.3f, 0.6f, 0.9f, 0.5f)); """

replacement_label = """            lbl.style.paddingBottom = 8;
            lbl.style.paddingTop = 8;
            lbl.style.paddingLeft = 80;
            // Removed border bottom, handled by global paper lines now!"""

if target_label in content:
    content = content.replace(target_label, replacement_label)
else:
    print("TARGET LABEL NOT FOUND")

# 2. Add generateVisualContent registration
target_setup = """        if (isAudit)
        {
            container.style.backgroundColor = new StyleColor(new Color(0.98f, 0.96f, 0.9f)); // Paper"""

replacement_setup = """        container.generateVisualContent -= OnGeneratePaper;
        if (isAudit)
        {
            container.generateVisualContent += OnGeneratePaper;
            container.style.backgroundColor = new StyleColor(new Color(0.98f, 0.96f, 0.9f)); // Paper"""

if target_setup in content:
    content = content.replace(target_setup, replacement_setup)
else:
    print("TARGET SETUP NOT FOUND")

# 3. Add shadow to left buttons
target_lBtn = """                lBtn.style.borderTopLeftRadius = 25; lBtn.style.borderTopRightRadius = 25; lBtn.style.borderBottomLeftRadius = 25; lBtn.style.borderBottomRightRadius = 25; // Smooth round bubbles
            }"""

replacement_lBtn = """                lBtn.style.borderTopLeftRadius = 25; lBtn.style.borderTopRightRadius = 25; lBtn.style.borderBottomLeftRadius = 25; lBtn.style.borderBottomRightRadius = 25; // Smooth round bubbles
                // Drop shadow / pop effect
                lBtn.style.borderBottomWidth = 6; 
                lBtn.style.borderRightWidth = 4;
            }"""

if target_lBtn in content:
    content = content.replace(target_lBtn, replacement_lBtn)
else:
    print("TARGET LBTN NOT FOUND")

# 4. Add shadow to right buttons
target_rBtn = """                rBtn.style.borderTopWidth = 2; rBtn.style.borderBottomWidth = 2; rBtn.style.borderLeftWidth = 2; rBtn.style.borderRightWidth = 2;
                rBtn.style.borderTopLeftRadius = 25; rBtn.style.borderTopRightRadius = 25; rBtn.style.borderBottomLeftRadius = 25; rBtn.style.borderBottomRightRadius = 25;
            }"""

replacement_rBtn = """                rBtn.style.borderTopWidth = 2; rBtn.style.borderBottomWidth = 6; rBtn.style.borderLeftWidth = 2; rBtn.style.borderRightWidth = 4;
                rBtn.style.borderTopLeftRadius = 25; rBtn.style.borderTopRightRadius = 25; rBtn.style.borderBottomLeftRadius = 25; rBtn.style.borderBottomRightRadius = 25;
            }"""

if target_rBtn in content:
    content = content.replace(target_rBtn, replacement_rBtn)
else:
    print("TARGET RBTN NOT FOUND")

# 5. Hide the red line in DragCheatSheet
target_cheatsheet = """            if (cheatSheet != null) {
                cheatSheet.style.backgroundColor = new StyleColor(Color.clear);
                cheatSheet.style.borderTopWidth = 0; cheatSheet.style.borderBottomWidth = 0; cheatSheet.style.borderLeftWidth = 0; cheatSheet.style.borderRightWidth = 0;
            }"""

replacement_cheatsheet = """            if (cheatSheet != null) {
                cheatSheet.style.backgroundColor = new StyleColor(Color.clear);
                cheatSheet.style.borderTopWidth = 0; cheatSheet.style.borderBottomWidth = 0; cheatSheet.style.borderLeftWidth = 0; cheatSheet.style.borderRightWidth = 0;
                // Hide old absolute red line
                var firstChild = cheatSheet.Children().FirstOrDefault();
                if (firstChild != null && firstChild.style.position == Position.Absolute) firstChild.style.display = DisplayStyle.None;
            }"""

if target_cheatsheet in content:
    content = content.replace(target_cheatsheet, replacement_cheatsheet)
else:
    print("TARGET CHEATSHEET NOT FOUND")

# 6. Add OnGeneratePaper method at the end
target_end = """    private System.Collections.IEnumerator RevertColorRoutine(Button btn, Color color)
    {
        yield return new WaitForSeconds(0.3f);
        if (btn != null && btn.enabledSelf) btn.style.backgroundColor = new StyleColor(color);
    }
}"""

replacement_end = """    private System.Collections.IEnumerator RevertColorRoutine(Button btn, Color color)
    {
        yield return new WaitForSeconds(0.3f);
        if (btn != null && btn.enabledSelf) btn.style.backgroundColor = new StyleColor(color);
    }

    private void OnGeneratePaper(MeshGenerationContext ctx)
    {
        var paint2D = ctx.painter2D;
        float w = container.layout.width;
        float h = container.layout.height;

        // Draw red margin line
        paint2D.strokeColor = new Color(0.9f, 0.3f, 0.3f, 0.8f);
        paint2D.lineWidth = 2f;
        paint2D.BeginPath();
        paint2D.MoveTo(new Vector2(60, 0));
        paint2D.LineTo(new Vector2(60, h));
        paint2D.Stroke();

        // Draw blue ruled lines
        paint2D.strokeColor = new Color(0.3f, 0.6f, 0.9f, 0.4f);
        paint2D.lineWidth = 1f;
        for(float y = 40; y < h; y += 38)
        {
            paint2D.BeginPath();
            paint2D.MoveTo(new Vector2(0, y));
            paint2D.LineTo(new Vector2(w, y));
            paint2D.Stroke();
        }
    }
}"""

if target_end in content:
    content = content.replace(target_end, replacement_end)
else:
    print("TARGET END NOT FOUND")

with open(path, 'w') as f:
    f.write(content)

import re

path = 'Assets/Scripts/2 Managers/MiniGames/IncineratorTossMiniGame.cs'
with open(path, 'r') as f:
    content = f.read()

# Add bagLabel
content = re.sub(r'private Label uiText;', r'private Label uiText;\n    private Label bagLabel;', content)

init_target = r'''        drawArea\.generateVisualContent \+= OnDrawGame;
        container\.Add\(drawArea\);
    \}'''
init_replacement = r'''        drawArea.generateVisualContent += OnDrawGame;
        container.Add(drawArea);

        bagLabel = new Label("🗑️");
        bagLabel.style.position = Position.Absolute;
        bagLabel.style.fontSize = 50;
        bagLabel.style.color = Color.white;
        bagLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
        drawArea.Add(bagLabel);
    }'''
content = re.sub(init_target, init_replacement, content)

# Set health = 1
start_target = r'''    public void StartGame\(\)
    \{
        bagsTossed = 0;
        health = 3;'''
start_replacement = r'''    public void StartGame()
    {
        bagsTossed = 0;
        health = 1;'''
content = re.sub(start_target, start_replacement, content)

# Update bagLabel position in GameRoutine
routine_target = r'''            if \(drawArea != null\) drawArea\.MarkDirtyRepaint\(\);
            yield return null;
        \}
    \}'''
routine_replacement = r'''            if (!isBagFlying || explosionTimer <= 0) {
                bagLabel.style.display = DisplayStyle.Flex;
                bagLabel.style.left = bagPos.x - 25f;
                bagLabel.style.top = bagPos.y - 25f;
                if (isBagFlying) {
                    bagLabel.style.scale = new StyleScale(new Scale(new Vector2(0.6f, 0.6f)));
                } else {
                    bagLabel.style.scale = new StyleScale(new Scale(new Vector2(1f, 1f)));
                }
            } else {
                bagLabel.style.display = DisplayStyle.None;
            }

            if (drawArea != null) drawArea.MarkDirtyRepaint();
            yield return null;
        }
    }'''
content = re.sub(routine_target, routine_replacement, content)

# Fix furnace bottom lip to hide fire
lip_target = r'''        p\.BeginPath\(\); p\.MoveTo\(new Vector2\(0, furnaceBottom\)\); p\.LineTo\(new Vector2\(w, furnaceBottom\)\); p\.LineTo\(new Vector2\(w, 130\)\); p\.LineTo\(new Vector2\(0, 130\)\); p\.Fill\(\);
        
        p\.fillColor = new Color\(0\.08f, 0\.08f, 0\.1f\);
        for\(float x = 20; x <= w; x \+= 40\) \{
            p\.BeginPath\(\); p\.Arc\(new Vector2\(x, 8\), 4, 0, 360\); p\.Fill\(\);
            p\.BeginPath\(\); p\.Arc\(new Vector2\(x, 122\), 4, 0, 360\); p\.Fill\(\);
        \}'''
lip_replacement = r'''        p.BeginPath(); p.MoveTo(new Vector2(0, furnaceBottom)); p.LineTo(new Vector2(w, furnaceBottom)); p.LineTo(new Vector2(w, 160)); p.LineTo(new Vector2(0, 160)); p.Fill();
        
        p.fillColor = new Color(0.08f, 0.08f, 0.1f);
        for(float x = 20; x <= w; x += 40) {
            p.BeginPath(); p.Arc(new Vector2(x, 8), 4, 0, 360); p.Fill();
            p.BeginPath(); p.Arc(new Vector2(x, 122), 4, 0, 360); p.Fill();
            p.BeginPath(); p.Arc(new Vector2(x, 152), 4, 0, 360); p.Fill();
        }'''
content = re.sub(lip_target, lip_replacement, content)

# Remove the drawn bag entirely (since we use the Label)
bag_target = r'''        // 5\. THE TRASH BAG
.*?
        \}
        
        if \(flashTimer > 0\)'''
bag_replacement = r'''        // 5. TRASH BAG IS NOW A LABEL
        
        if (flashTimer > 0)'''
content = re.sub(bag_target, bag_replacement, content, flags=re.DOTALL)

with open(path, 'w') as f:
    f.write(content)


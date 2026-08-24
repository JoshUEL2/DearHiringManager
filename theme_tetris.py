import re

path = 'Assets/Scripts/2 Managers/MiniGames/TetrisMiniGame.cs'

with open(path, 'r') as f:
    content = f.read()

target_start = r'''    public void StartGame\(\)
    \{
        tetrisContainer\.style\.display = DisplayStyle\.Flex;

        tetrisGrid\.Clear\(\);'''

replacement_start = r'''    public void StartGame()
    {
        tetrisContainer.style.display = DisplayStyle.Flex;

        // --- INDUSTRIAL THEME INJECTION ---
        tetrisContainer.style.backgroundColor = new StyleColor(new Color(0.12f, 0.13f, 0.15f));
        tetrisContainer.generateVisualContent -= OnDrawIndustrialBackground;
        tetrisContainer.generateVisualContent += OnDrawIndustrialBackground;

        tetrisGrid.style.width = 324;
        tetrisGrid.style.height = 474;
        tetrisGrid.style.backgroundColor = new StyleColor(new Color(0.05f, 0.05f, 0.05f, 0.8f));
        tetrisGrid.style.borderTopWidth = 12; tetrisGrid.style.borderBottomWidth = 12; tetrisGrid.style.borderLeftWidth = 12; tetrisGrid.style.borderRightWidth = 12;
        tetrisGrid.style.borderTopColor = new StyleColor(new Color(0.8f, 0.6f, 0.1f)); 
        tetrisGrid.style.borderBottomColor = new StyleColor(new Color(0.8f, 0.6f, 0.1f));
        tetrisGrid.style.borderLeftColor = new StyleColor(new Color(0.8f, 0.6f, 0.1f));
        tetrisGrid.style.borderRightColor = new StyleColor(new Color(0.8f, 0.6f, 0.1f));
        tetrisGrid.generateVisualContent -= OnDrawGridBackground;
        tetrisGrid.generateVisualContent += OnDrawGridBackground;
        
        if (tetrisScoreText != null) {
            tetrisScoreText.style.color = new StyleColor(new Color(1f, 0.8f, 0.1f));
            tetrisScoreText.style.backgroundColor = new StyleColor(new Color(0.1f, 0.1f, 0.1f));
            tetrisScoreText.style.paddingTop = 10; tetrisScoreText.style.paddingBottom = 10; tetrisScoreText.style.paddingLeft = 20; tetrisScoreText.style.paddingRight = 20;
            tetrisScoreText.style.borderTopWidth = 4; tetrisScoreText.style.borderBottomWidth = 4; tetrisScoreText.style.borderLeftWidth = 4; tetrisScoreText.style.borderRightWidth = 4;
            tetrisScoreText.style.borderTopColor = new StyleColor(new Color(0.3f, 0.3f, 0.3f));
            tetrisScoreText.style.borderBottomColor = new StyleColor(new Color(0.3f, 0.3f, 0.3f));
            tetrisScoreText.style.borderLeftColor = new StyleColor(new Color(0.3f, 0.3f, 0.3f));
            tetrisScoreText.style.borderRightColor = new StyleColor(new Color(0.3f, 0.3f, 0.3f));
        }

        if (btnTetrisLeft != null && btnTetrisLeft.parent != null) {
            btnTetrisLeft.parent.style.flexWrap = Wrap.Wrap;
            btnTetrisLeft.parent.style.width = 330;
            btnTetrisLeft.parent.style.justifyContent = Justify.SpaceBetween;
        }

        void StyleButton(Button btn, string txt, Color bg, Color textC) {
            if (btn == null) return;
            btn.style.display = DisplayStyle.Flex;
            btn.text = txt;
            btn.style.backgroundColor = new StyleColor(bg);
            btn.style.color = new StyleColor(textC);
            btn.style.width = 160;
            btn.style.height = 70;
            btn.style.fontSize = 24;
            btn.style.unityFontStyleAndWeight = FontStyle.Bold;
            btn.style.borderBottomWidth = 8;
            btn.style.marginBottom = 10;
            btn.style.borderTopLeftRadius = 10; btn.style.borderTopRightRadius = 10; btn.style.borderBottomLeftRadius = 10; btn.style.borderBottomRightRadius = 10;
            btn.style.marginLeft = 0; btn.style.marginRight = 0;
        }

        StyleButton(btnTetrisLeft, "◄ MOVE", new Color(0.8f, 0.2f, 0.2f), Color.white);
        StyleButton(btnTetrisRight, "MOVE ►", new Color(0.8f, 0.2f, 0.2f), Color.white);
        StyleButton(btnTetrisRotate, "ROTATE", new Color(0.2f, 0.6f, 0.9f), Color.white);
        StyleButton(btnTetrisDrop, "DROP ▼", new Color(0.8f, 0.8f, 0.2f), Color.black);
        // --- END THEME INJECTION ---

        tetrisGrid.Clear();'''

content = re.sub(target_start, replacement_start, content)

target_score = r'''        if \(tetrisScoreText != null\)
            tetrisScoreText\.text = \$"Stacked: 0 / 10";'''

replacement_score = r'''        if (tetrisScoreText != null)
            tetrisScoreText.text = $"STEEL BEAMS: 0 / 10";'''

content = re.sub(target_score, replacement_score, content)

target_score_update = r'''        if \(tetrisScoreText != null\)
            tetrisScoreText\.text = \$"Stacked: \{tetrisScore\} / 10";'''

replacement_score_update = r'''        if (tetrisScoreText != null)
            tetrisScoreText.text = $"STEEL BEAMS: {tetrisScore} / 10";'''

content = re.sub(target_score_update, replacement_score_update, content)


target_end = r'''    private IEnumerator GameRoutine\(\)
    \{
        tetrisTickTimer = 0f;'''

replacement_end = r'''    private void OnDrawIndustrialBackground(MeshGenerationContext ctx)
    {
        var p = ctx.painter2D;
        float w = ctx.visualElement.layout.width;
        float h = ctx.visualElement.layout.height;

        p.fillColor = new Color(0.12f, 0.12f, 0.14f);
        p.BeginPath(); p.MoveTo(new Vector2(0,0)); p.LineTo(new Vector2(w,0)); p.LineTo(new Vector2(w,h)); p.LineTo(new Vector2(0,h)); p.Fill();
        
        p.fillColor = new Color(0.9f, 0.7f, 0.1f, 0.5f);
        for(float y = -100; y < h; y += 40) {
            p.BeginPath(); p.MoveTo(new Vector2(0, y)); p.LineTo(new Vector2(30, y)); p.LineTo(new Vector2(30, y+20)); p.LineTo(new Vector2(0, y+20)); p.Fill();
            p.BeginPath(); p.MoveTo(new Vector2(w-30, y)); p.LineTo(new Vector2(w, y)); p.LineTo(new Vector2(w, y+20)); p.LineTo(new Vector2(w-30, y+20)); p.Fill();
        }

        if (tetrisGrid != null && tetrisGrid.layout.width > 0) {
            float gridY = tetrisGrid.layout.yMin;
            float gridX1 = tetrisGrid.layout.xMin;
            float gridX2 = tetrisGrid.layout.xMax;
            
            p.strokeColor = new Color(0.5f, 0.5f, 0.5f);
            p.lineWidth = 6f;
            
            p.BeginPath(); p.MoveTo(new Vector2(gridX1 + 20, 0)); p.LineTo(new Vector2(gridX1 + 20, gridY)); p.Stroke();
            p.BeginPath(); p.MoveTo(new Vector2(gridX2 - 20, 0)); p.LineTo(new Vector2(gridX2 - 20, gridY)); p.Stroke();
            
            p.fillColor = new Color(0.8f, 0.2f, 0.2f);
            p.BeginPath(); p.Arc(new Vector2(gridX1 + 20, gridY), 12f, 0, 360); p.Fill();
            p.BeginPath(); p.Arc(new Vector2(gridX2 - 20, gridY), 12f, 0, 360); p.Fill();
        }
    }

    private void OnDrawGridBackground(MeshGenerationContext ctx)
    {
        var p = ctx.painter2D;
        float w = ctx.visualElement.layout.width;
        float h = ctx.visualElement.layout.height;

        p.strokeColor = new Color(0.1f, 0.1f, 0.1f, 0.5f);
        p.lineWidth = 2f;
        for (float x = 0; x < w; x += 30) {
            p.BeginPath(); p.MoveTo(new Vector2(x, 0)); p.LineTo(new Vector2(x, h)); p.Stroke();
        }
        for (float y = 0; y < h; y += 30) {
            p.BeginPath(); p.MoveTo(new Vector2(0, y)); p.LineTo(new Vector2(w, y)); p.Stroke();
        }
        
        p.fillColor = new Color(0f, 0f, 0f, 0.4f);
        p.BeginPath(); p.MoveTo(new Vector2(0,0)); p.LineTo(new Vector2(w,0)); p.LineTo(new Vector2(w,20)); p.LineTo(new Vector2(0,20)); p.Fill();
        p.BeginPath(); p.MoveTo(new Vector2(0,0)); p.LineTo(new Vector2(20,0)); p.LineTo(new Vector2(20,h)); p.LineTo(new Vector2(0,h)); p.Fill();
    }

    private IEnumerator GameRoutine()
    {
        tetrisTickTimer = 0f;'''

content = re.sub(target_end, replacement_end, content)

with open(path, 'w') as f:
    f.write(content)


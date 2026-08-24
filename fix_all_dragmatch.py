import re

path = 'Assets/Scripts/2 Managers/MiniGames/DragMatchMiniGame.cs'

with open(path, 'r') as f:
    content = f.read()

# We will completely replace the SetupDragMatch, OnDragRightClicked, and OnGeneratePaper methods.

target_all = r'''    private void SetupDragMatch\(\).*?    \}

    private void OnGeneratePaper\(MeshGenerationContext ctx\)
    \{.*?    \}
\}'''

replacement_all = r'''    private void SetupDragMatch()
    {
        dragLeftColumn.Clear();
        dragRightColumn.Clear();
        dragCheatSheetList.Clear();
        dragLeftButtons.Clear();
        dragRightButtons.Clear();
        dragCorrectMap.Clear();
        dragMatchCount = 0;
        dragSelectedLeft = -1;
        
        bool isAudit = manager.currentGame != null && manager.currentGame.name == "Audit sewage drainage lines";
        
        container.generateVisualContent -= OnGenerateVisuals;
        container.generateVisualContent += OnGenerateVisuals;
        
        if (isAudit)
        {
            container.style.backgroundColor = new StyleColor(new Color(0.98f, 0.96f, 0.9f)); // Paper
            container.style.borderTopColor = new StyleColor(new Color(0.3f, 0.3f, 0.3f));
            container.style.borderBottomColor = new StyleColor(new Color(0.3f, 0.3f, 0.3f));
            container.style.borderLeftColor = new StyleColor(new Color(0.3f, 0.3f, 0.3f));
            container.style.borderRightColor = new StyleColor(new Color(0.3f, 0.3f, 0.3f));
            var bolt = dragLeftColumn.parent.Q<Label>();
            if (bolt != null) { bolt.text = "✏️"; bolt.style.color = new StyleColor(Color.black); }
        }
        else
        {
            container.style.backgroundColor = new StyleColor(new Color(0.08f, 0.15f, 0.3f)); // Blueprint blue
            container.style.borderTopColor = new StyleColor(new Color(0.2f, 0.6f, 1f));
            container.style.borderBottomColor = new StyleColor(new Color(0.2f, 0.6f, 1f));
            container.style.borderLeftColor = new StyleColor(new Color(0.2f, 0.6f, 1f));
            container.style.borderRightColor = new StyleColor(new Color(0.2f, 0.6f, 1f));
            var bolt = dragLeftColumn.parent.Q<Label>();
            if (bolt != null) { bolt.text = "⚡"; bolt.style.color = new StyleColor(new Color(0.2f, 0.8f, 1f)); }
        }
        
        container.style.borderTopWidth = 4; container.style.borderBottomWidth = 4; container.style.borderLeftWidth = 4; container.style.borderRightWidth = 4;
        
        dragLeftColumn.parent.style.backgroundColor = new StyleColor(Color.clear);
        dragLeftColumn.parent.style.borderTopWidth = 0; dragLeftColumn.parent.style.borderBottomWidth = 0;
        dragLeftColumn.parent.style.borderLeftWidth = 0; dragLeftColumn.parent.style.borderRightWidth = 0;
        
        var cheatSheet = container.Q<VisualElement>("DragCheatSheet");
        if (cheatSheet != null) {
            cheatSheet.style.backgroundColor = new StyleColor(Color.clear);
            cheatSheet.style.borderTopWidth = 0; cheatSheet.style.borderBottomWidth = 0; cheatSheet.style.borderLeftWidth = 0; cheatSheet.style.borderRightWidth = 0;
            // Hide old absolute red line
            var firstChild = cheatSheet.Children().FirstOrDefault();
            if (firstChild != null && firstChild.style.position == Position.Absolute) firstChild.style.display = DisplayStyle.None;
            
            // Adjust cheat sheet title text
            var titleLbl = cheatSheet.Q<Label>();
            if (titleLbl != null)
            {
                if (isAudit) { titleLbl.style.color = new StyleColor(Color.black); titleLbl.style.borderBottomColor = new StyleColor(Color.black); }
                else { titleLbl.style.color = new StyleColor(new Color(0.2f, 0.8f, 1f)); titleLbl.style.borderBottomColor = new StyleColor(new Color(0.2f, 0.8f, 1f)); }
            }
        }
        
        var lefts = config.matchLeft.ToList();
        var rights = config.matchRight.ToList();
        var rightIndices = Enumerable.Range(0, rights.Count).OrderBy(x => Random.value).ToList();
        
        Color textColor = isAudit ? new Color(0.1f, 0.1f, 0.1f) : new Color(0.6f, 0.9f, 1f);
        Color btnBg = isAudit ? new Color(0.9f, 0.9f, 0.95f) : new Color(0.05f, 0.1f, 0.2f);
        Color btnBorder = isAudit ? Color.black : new Color(0.2f, 0.6f, 1f);
        Color btnSel = isAudit ? new Color(0.4f, 0.6f, 1f) : new Color(0.2f, 0.8f, 1f);
        
        for (int i=0; i<lefts.Count; i++)
        {
            var lbl = new Label($"{lefts[i]} = {rights[rightIndices[i]]}");
            lbl.style.fontSize = 22;
            lbl.style.color = new StyleColor(textColor); 
            lbl.style.whiteSpace = WhiteSpace.NoWrap;
            lbl.style.marginBottom = 10;
            lbl.style.paddingBottom = 8;
            lbl.style.paddingTop = 8;
            lbl.style.paddingLeft = 80;
            lbl.style.unityTextAlign = TextAnchor.MiddleLeft;
            dragCheatSheetList.Add(lbl);
            
            int idx = i;
            var lBtn = new Button { text = lefts[i] };
            lBtn.AddToClassList("popup-button");
            lBtn.style.paddingTop = 15; lBtn.style.paddingBottom = 15; lBtn.style.width = new Length(100, LengthUnit.Percent); lBtn.style.fontSize = 20;
            
            lBtn.style.color = new StyleColor(textColor); 
            lBtn.style.borderTopColor = new StyleColor(btnBorder); lBtn.style.borderBottomColor = new StyleColor(btnBorder); 
            lBtn.style.borderLeftColor = new StyleColor(btnBorder); lBtn.style.borderRightColor = new StyleColor(btnBorder); 
            lBtn.style.borderTopWidth = 2; lBtn.style.borderBottomWidth = 6; lBtn.style.borderLeftWidth = 2; lBtn.style.borderRightWidth = 4;
            lBtn.style.borderTopLeftRadius = 25; lBtn.style.borderTopRightRadius = 25; lBtn.style.borderBottomLeftRadius = 25; lBtn.style.borderBottomRightRadius = 25;
            lBtn.style.backgroundColor = new StyleColor(btnBg);
            
            lBtn.clicked += () => { 
                if (!lBtn.enabledSelf) return;
                dragSelectedLeft = idx; 
                for(int j=0; j<dragLeftButtons.Count; j++) {
                    if (dragLeftButtons[j].enabledSelf) {
                        dragLeftButtons[j].style.backgroundColor = new StyleColor(btnBg);
                        dragLeftButtons[j].style.color = new StyleColor(textColor);
                    }
                }
                lBtn.style.backgroundColor = new StyleColor(btnSel); 
                lBtn.style.color = new StyleColor(Color.black);
            };
            dragLeftButtons.Add(lBtn);
            dragLeftColumn.Add(lBtn);
            
            int rIdx = rightIndices[i];
            dragCorrectMap[i] = rIdx;
        }
        
        var emptyLbl = new Label(" ");
        emptyLbl.style.paddingBottom = 8;
        emptyLbl.style.paddingTop = 8;
        dragCheatSheetList.Add(emptyLbl);
        
        for (int i=0; i<rights.Count; i++)
        {
            int idx = i;
            var rBtn = new Button { text = rights[idx] };
            rBtn.AddToClassList("popup-button");
            rBtn.style.paddingTop = 15; rBtn.style.paddingBottom = 15; rBtn.style.width = new Length(100, LengthUnit.Percent); rBtn.style.fontSize = 20;
            
            rBtn.style.backgroundColor = new StyleColor(btnBg);
            rBtn.style.color = new StyleColor(textColor);
            rBtn.style.borderTopColor = new StyleColor(btnBorder); rBtn.style.borderBottomColor = new StyleColor(btnBorder); 
            rBtn.style.borderLeftColor = new StyleColor(btnBorder); rBtn.style.borderRightColor = new StyleColor(btnBorder);
            rBtn.style.borderTopWidth = 2; rBtn.style.borderBottomWidth = 6; rBtn.style.borderLeftWidth = 2; rBtn.style.borderRightWidth = 4;
            rBtn.style.borderTopLeftRadius = 25; rBtn.style.borderTopRightRadius = 25; rBtn.style.borderBottomLeftRadius = 25; rBtn.style.borderBottomRightRadius = 25;
            
            rBtn.clicked += () => OnDragRightClicked(idx, rBtn);
            dragRightButtons.Add(rBtn);
            dragRightColumn.Add(rBtn);
        }
    }

    private void OnDragRightClicked(int rIdx, Button rBtn)
    {
        if (!manager.isPlaying || dragSelectedLeft == -1) return;
        
        bool isAudit = manager.currentGame != null && manager.currentGame.name == "Audit sewage drainage lines";
        Color matchedColor = isAudit ? new Color(0.2f, 0.8f, 0.2f) : new Color(0.2f, 1f, 0.2f);
        Color lDef = isAudit ? new Color(0.9f, 0.9f, 0.95f) : new Color(0.05f, 0.1f, 0.2f);
        Color textColor = isAudit ? new Color(0.1f, 0.1f, 0.1f) : new Color(0.6f, 0.9f, 1f);
        Color errorColor = isAudit ? new Color(0.9f, 0.4f, 0.4f) : Color.red;
        
        if (dragCorrectMap[dragSelectedLeft] == rIdx)
        {
            dragLeftButtons[dragSelectedLeft].style.backgroundColor = new StyleColor(matchedColor);
            dragLeftButtons[dragSelectedLeft].style.color = new StyleColor(Color.black);
            dragLeftButtons[dragSelectedLeft].SetEnabled(false);
            rBtn.style.backgroundColor = new StyleColor(matchedColor);
            rBtn.style.color = new StyleColor(Color.black);
            rBtn.SetEnabled(false);
            dragMatchCount++;
            if (dragMatchCount >= config.matchLeft.Length) manager.MarkWinReady();
        }
        else
        {
            var btnToRevert = dragLeftButtons[dragSelectedLeft];
            btnToRevert.style.backgroundColor = new StyleColor(errorColor);
            manager.StartCoroutine(RevertColorRoutine(btnToRevert, lDef, textColor));
            if (manager.currentGame != null && manager.currentGame.canFail) manager.AbortGame();
        }
        dragSelectedLeft = -1;
    }

    private System.Collections.IEnumerator RevertColorRoutine(Button btn, Color bg, Color text)
    {
        yield return new WaitForSeconds(0.3f);
        if (btn != null && btn.enabledSelf) {
            btn.style.backgroundColor = new StyleColor(bg);
            btn.style.color = new StyleColor(text);
        }
    }

    private void OnGenerateVisuals(MeshGenerationContext ctx)
    {
        bool isAudit = manager.currentGame != null && manager.currentGame.name == "Audit sewage drainage lines";
        var paint2D = ctx.painter2D;
        float w = container.layout.width;
        float h = container.layout.height;

        if (isAudit)
        {
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
        else
        {
            // Blueprint Technical Grid
            paint2D.strokeColor = new Color(0.2f, 0.6f, 1f, 0.2f);
            
            // Thin grid lines
            paint2D.lineWidth = 1f;
            for(float x = 0; x < w; x += 20) { paint2D.BeginPath(); paint2D.MoveTo(new Vector2(x, 0)); paint2D.LineTo(new Vector2(x, h)); paint2D.Stroke(); }
            for(float y = 0; y < h; y += 20) { paint2D.BeginPath(); paint2D.MoveTo(new Vector2(0, y)); paint2D.LineTo(new Vector2(w, y)); paint2D.Stroke(); }
            
            // Thick grid lines
            paint2D.strokeColor = new Color(0.2f, 0.6f, 1f, 0.4f);
            paint2D.lineWidth = 2f;
            for(float x = 0; x < w; x += 100) { paint2D.BeginPath(); paint2D.MoveTo(new Vector2(x, 0)); paint2D.LineTo(new Vector2(x, h)); paint2D.Stroke(); }
            for(float y = 0; y < h; y += 100) { paint2D.BeginPath(); paint2D.MoveTo(new Vector2(0, y)); paint2D.LineTo(new Vector2(w, y)); paint2D.Stroke(); }
        }
    }
}'''

content = re.sub(target_all, replacement_all, content, flags=re.DOTALL)

with open(path, 'w') as f:
    f.write(content)


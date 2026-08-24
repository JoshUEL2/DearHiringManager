path = 'Assets/Scripts/2 Managers/MiniGames/DragMatchMiniGame.cs'

with open(path, 'r') as f:
    content = f.read()

# Replace the text alignment and left button logic
target1 = """        for (int i=0; i<lefts.Count; i++)
        {
            var lbl = new Label($"{lefts[i]} = {rights[rightIndices[i]]}");
            lbl.style.fontSize = 22;
            lbl.style.color = new StyleColor(new Color(0.1f, 0.1f, 0.1f)); // Dark grey/black ink
            lbl.style.whiteSpace = WhiteSpace.NoWrap;
            lbl.style.marginBottom = 10;
            lbl.style.paddingBottom = 8;
            lbl.style.paddingTop = 8;
            lbl.style.borderBottomWidth = 1;
            lbl.style.borderBottomColor = new StyleColor(new Color(0.3f, 0.6f, 0.9f, 0.5f)); // Light blue ruler line
            lbl.style.unityTextAlign = TextAnchor.MiddleLeft;
            dragCheatSheetList.Add(lbl);
            
            int idx = i;
            var lBtn = new Button { text = lefts[i] };
            lBtn.AddToClassList("popup-button");
            lBtn.style.paddingTop = 15; lBtn.style.paddingBottom = 15; lBtn.style.width = new Length(100, LengthUnit.Percent); lBtn.style.fontSize = 20;
            lBtn.clicked += () => { 
                dragSelectedLeft = idx; 
                foreach(var b in dragLeftButtons) b.style.backgroundColor = new StyleColor(Color.gray); 
                lBtn.style.backgroundColor = new StyleColor(Color.yellow); 
            };
            dragLeftButtons.Add(lBtn);
            dragLeftColumn.Add(lBtn);
            
            int rIdx = rightIndices[i];
            dragCorrectMap[i] = rIdx;
        }"""

replacement1 = """        for (int i=0; i<lefts.Count; i++)
        {
            var lbl = new Label($"{lefts[i]} = {rights[rightIndices[i]]}");
            lbl.style.fontSize = 22;
            lbl.style.color = new StyleColor(new Color(0.1f, 0.1f, 0.1f)); 
            lbl.style.whiteSpace = WhiteSpace.NoWrap;
            lbl.style.marginBottom = 10;
            lbl.style.paddingBottom = 8;
            lbl.style.paddingTop = 8;
            lbl.style.marginLeft = 80; // FIX TEXT ALIGNMENT OVER RED LINE!
            lbl.style.borderBottomWidth = 1;
            lbl.style.borderBottomColor = new StyleColor(new Color(0.3f, 0.6f, 0.9f, 0.5f)); 
            lbl.style.unityTextAlign = TextAnchor.MiddleLeft;
            dragCheatSheetList.Add(lbl);
            
            int idx = i;
            var lBtn = new Button { text = lefts[i] };
            lBtn.AddToClassList("popup-button");
            lBtn.style.paddingTop = 15; lBtn.style.paddingBottom = 15; lBtn.style.width = new Length(100, LengthUnit.Percent); lBtn.style.fontSize = 20;
            
            Color lDef = isAudit ? new Color(0.9f, 0.9f, 0.95f) : new Color(0.3f, 0.3f, 0.3f);
            Color lSel = isAudit ? new Color(0.4f, 0.6f, 1f) : Color.yellow;
            
            if (isAudit) { 
                lBtn.style.color = new StyleColor(Color.black); 
                lBtn.style.borderTopColor = new StyleColor(Color.black); 
                lBtn.style.borderBottomColor = new StyleColor(Color.black); 
                lBtn.style.borderLeftColor = new StyleColor(Color.black); 
                lBtn.style.borderRightColor = new StyleColor(Color.black); 
                lBtn.style.borderTopWidth = 2; lBtn.style.borderBottomWidth = 2; lBtn.style.borderLeftWidth = 2; lBtn.style.borderRightWidth = 2;
                lBtn.style.borderTopLeftRadius = 25; lBtn.style.borderTopRightRadius = 25; lBtn.style.borderBottomLeftRadius = 25; lBtn.style.borderBottomRightRadius = 25; // Smooth round bubbles
            }
            lBtn.style.backgroundColor = new StyleColor(lDef);
            
            lBtn.clicked += () => { 
                if (!lBtn.enabledSelf) return;
                dragSelectedLeft = idx; 
                for(int j=0; j<dragLeftButtons.Count; j++) {
                    if (dragLeftButtons[j].enabledSelf) dragLeftButtons[j].style.backgroundColor = new StyleColor(lDef);
                }
                lBtn.style.backgroundColor = new StyleColor(lSel); 
            };
            dragLeftButtons.Add(lBtn);
            dragLeftColumn.Add(lBtn);
            
            int rIdx = rightIndices[i];
            dragCorrectMap[i] = rIdx;
        }"""

if target1 in content:
    content = content.replace(target1, replacement1)
else:
    print("TARGET 1 NOT FOUND")

# Replace Right buttons
target2 = """        for (int i=0; i<rights.Count; i++)
        {
            int idx = i;
            var rBtn = new Button { text = rights[idx] };
            rBtn.AddToClassList("popup-button");
            rBtn.style.paddingTop = 15; rBtn.style.paddingBottom = 15; rBtn.style.width = new Length(100, LengthUnit.Percent); rBtn.style.fontSize = 20;
            rBtn.clicked += () => OnDragRightClicked(idx, rBtn);
            dragRightButtons.Add(rBtn);
            dragRightColumn.Add(rBtn);
        }"""

replacement2 = """        for (int i=0; i<rights.Count; i++)
        {
            int idx = i;
            var rBtn = new Button { text = rights[idx] };
            rBtn.AddToClassList("popup-button");
            rBtn.style.paddingTop = 15; rBtn.style.paddingBottom = 15; rBtn.style.width = new Length(100, LengthUnit.Percent); rBtn.style.fontSize = 20;
            
            if (isAudit) {
                rBtn.style.backgroundColor = new StyleColor(new Color(0.9f, 0.9f, 0.95f));
                rBtn.style.color = new StyleColor(Color.black);
                rBtn.style.borderTopColor = new StyleColor(Color.black); rBtn.style.borderBottomColor = new StyleColor(Color.black); rBtn.style.borderLeftColor = new StyleColor(Color.black); rBtn.style.borderRightColor = new StyleColor(Color.black);
                rBtn.style.borderTopWidth = 2; rBtn.style.borderBottomWidth = 2; rBtn.style.borderLeftWidth = 2; rBtn.style.borderRightWidth = 2;
                rBtn.style.borderTopLeftRadius = 25; rBtn.style.borderTopRightRadius = 25; rBtn.style.borderBottomLeftRadius = 25; rBtn.style.borderBottomRightRadius = 25;
            }
            
            rBtn.clicked += () => OnDragRightClicked(idx, rBtn);
            dragRightButtons.Add(rBtn);
            dragRightColumn.Add(rBtn);
        }"""

if target2 in content:
    content = content.replace(target2, replacement2)
else:
    print("TARGET 2 NOT FOUND")
    
# Replace OnDragRightClicked
target3 = """    private void OnDragRightClicked(int rIdx, Button rBtn)
    {
        if (!manager.isPlaying || dragSelectedLeft == -1) return;
        
        if (dragCorrectMap[dragSelectedLeft] == rIdx)
        {
            dragLeftButtons[dragSelectedLeft].style.backgroundColor = new StyleColor(Color.green);
            dragLeftButtons[dragSelectedLeft].SetEnabled(false);
            rBtn.style.backgroundColor = new StyleColor(Color.green);
            rBtn.SetEnabled(false);
            dragMatchCount++;
            if (dragMatchCount >= config.matchLeft.Length) manager.MarkWinReady();
        }
        else
        {
            dragLeftButtons[dragSelectedLeft].style.backgroundColor = new StyleColor(Color.gray);
        }
        dragSelectedLeft = -1;
    }"""

replacement3 = """    private void OnDragRightClicked(int rIdx, Button rBtn)
    {
        if (!manager.isPlaying || dragSelectedLeft == -1) return;
        
        bool isAudit = manager.currentGame != null && manager.currentGame.name == "Audit sewage drainage lines";
        Color matchedColor = isAudit ? new Color(0.2f, 0.8f, 0.2f) : Color.green;
        Color lDef = isAudit ? new Color(0.9f, 0.9f, 0.95f) : new Color(0.3f, 0.3f, 0.3f);
        Color errorColor = isAudit ? new Color(0.9f, 0.4f, 0.4f) : Color.red;
        
        if (dragCorrectMap[dragSelectedLeft] == rIdx)
        {
            dragLeftButtons[dragSelectedLeft].style.backgroundColor = new StyleColor(matchedColor);
            dragLeftButtons[dragSelectedLeft].SetEnabled(false);
            rBtn.style.backgroundColor = new StyleColor(matchedColor);
            rBtn.SetEnabled(false);
            dragMatchCount++;
            if (dragMatchCount >= config.matchLeft.Length) manager.MarkWinReady();
        }
        else
        {
            // Flash red on error, then revert to default (not permanent grey!)
            var btnToRevert = dragLeftButtons[dragSelectedLeft];
            btnToRevert.style.backgroundColor = new StyleColor(errorColor);
            manager.StartCoroutine(RevertColorRoutine(btnToRevert, lDef));
            if (manager.currentGame != null && manager.currentGame.canFail) manager.AbortGame();
        }
        dragSelectedLeft = -1;
    }

    private System.Collections.IEnumerator RevertColorRoutine(Button btn, Color color)
    {
        yield return new WaitForSeconds(0.3f);
        if (btn != null && btn.enabledSelf) btn.style.backgroundColor = new StyleColor(color);
    }"""

if target3 in content:
    content = content.replace(target3, replacement3)
else:
    print("TARGET 3 NOT FOUND")

with open(path, 'w') as f:
    f.write(content)

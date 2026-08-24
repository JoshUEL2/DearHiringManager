import re

path = 'Assets/Scripts/2 Managers/MiniGames/DragMatchMiniGame.cs'

with open(path, 'r') as f:
    content = f.read()

# 1. Update StartGame / SetupDragMatch for the Audit aesthetic
target_setup = r'''    private void SetupDragMatch\(\)
    \{
        dragLeftColumn\.Clear\(\);
        dragRightColumn\.Clear\(\);
        dragCheatSheetList\.Clear\(\);
        dragLeftButtons\.Clear\(\);
        dragRightButtons\.Clear\(\);
        dragCorrectMap\.Clear\(\);
        dragMatchCount = 0;
        dragSelectedLeft = -1;
        
        var lefts = config\.matchLeft\.ToList\(\);
        var rights = config\.matchRight\.ToList\(\);
        var rightIndices = Enumerable\.Range\(0, rights\.Count\)\.OrderBy\(x => Random\.value\)\.ToList\(\);'''

replacement_setup = r'''    private void SetupDragMatch()
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
        
        // Apply Audit Paper Overhaul
        if (isAudit)
        {
            container.style.backgroundColor = new StyleColor(new Color(0.98f, 0.96f, 0.9f)); // Paper
            container.style.borderTopWidth = 4; container.style.borderBottomWidth = 4; container.style.borderLeftWidth = 4; container.style.borderRightWidth = 4;
            container.style.borderTopColor = new StyleColor(new Color(0.3f, 0.3f, 0.3f));
            container.style.borderBottomColor = new StyleColor(new Color(0.3f, 0.3f, 0.3f));
            container.style.borderLeftColor = new StyleColor(new Color(0.3f, 0.3f, 0.3f));
            container.style.borderRightColor = new StyleColor(new Color(0.3f, 0.3f, 0.3f));
            
            dragLeftColumn.parent.style.backgroundColor = new StyleColor(Color.clear);
            dragLeftColumn.parent.style.borderTopWidth = 0; dragLeftColumn.parent.style.borderBottomWidth = 0;
            dragLeftColumn.parent.style.borderLeftWidth = 0; dragLeftColumn.parent.style.borderRightWidth = 0;
            
            var bolt = dragLeftColumn.parent.Q<Label>();
            if (bolt != null) { bolt.text = "✏️"; bolt.style.color = new StyleColor(Color.black); }
            
            var cheatSheet = container.Q<VisualElement>("DragCheatSheet");
            if (cheatSheet != null) {
                cheatSheet.style.backgroundColor = new StyleColor(Color.clear);
                cheatSheet.style.borderTopWidth = 0; cheatSheet.style.borderBottomWidth = 0; cheatSheet.style.borderLeftWidth = 0; cheatSheet.style.borderRightWidth = 0;
            }
        }
        else
        {
            container.style.backgroundColor = new StyleColor(Color.clear);
            dragLeftColumn.parent.style.backgroundColor = new StyleColor(new Color(0.12f, 0.16f, 0.23f));
            dragLeftColumn.parent.style.borderTopWidth = 6; dragLeftColumn.parent.style.borderBottomWidth = 6;
            dragLeftColumn.parent.style.borderLeftWidth = 6; dragLeftColumn.parent.style.borderRightWidth = 6;
        }
        
        var lefts = config.matchLeft.ToList();
        var rights = config.matchRight.ToList();
        var rightIndices = Enumerable.Range(0, rights.Count).OrderBy(x => Random.value).ToList();'''

content = re.sub(target_setup, replacement_setup, content)

# 2. Fix the padding on the notepad and the button generation
target_loop1 = r'''        for \(int i=0; i<lefts\.Count; i\+\+\)
        \{
            var lbl = new Label\($"\{lefts\[i\]\} = \{rights\[rightIndices\[i\]\]\}"\);
            lbl\.style\.fontSize = 22;
            lbl\.style\.color = new StyleColor\(new Color\(0\.1f, 0\.1f, 0\.1f\)\); // Dark grey/black ink
            lbl\.style\.whiteSpace = WhiteSpace\.NoWrap;
            lbl\.style\.marginBottom = 10;
            lbl\.style\.paddingBottom = 8;
            lbl\.style\.paddingTop = 8;
            lbl\.style\.borderBottomWidth = 1;
            lbl\.style\.borderBottomColor = new StyleColor\(new Color\(0\.3f, 0\.6f, 0\.9f, 0\.5f\)\); // Light blue ruler line
            lbl\.style\.unityTextAlign = TextAnchor\.MiddleLeft;
            dragCheatSheetList\.Add\(lbl\);
            
            int idx = i;
            var lBtn = new Button \{ text = lefts\[i\] \};
            lBtn\.AddToClassList\("popup-button"\);
            lBtn\.style\.paddingTop = 15; lBtn\.style\.paddingBottom = 15; lBtn\.style\.width = new Length\(100, LengthUnit\.Percent\); lBtn\.style\.fontSize = 20;
            lBtn\.clicked \+= \(\) => \{ 
                dragSelectedLeft = idx; 
                foreach\(var b in dragLeftButtons\) b\.style\.backgroundColor = new StyleColor\(Color\.gray\); 
                lBtn\.style\.backgroundColor = new StyleColor\(Color\.yellow\); 
            \};
            dragLeftButtons\.Add\(lBtn\);
            dragLeftColumn\.Add\(lBtn\);
            
            int rIdx = rightIndices\[i\];
            dragCorrectMap\[i\] = rIdx;
        \}'''

replacement_loop1 = r'''        for (int i=0; i<lefts.Count; i++)
        {
            var lbl = new Label($"{lefts[i]} = {rights[rightIndices[i]]}");
            lbl.style.fontSize = 22;
            lbl.style.color = new StyleColor(new Color(0.1f, 0.1f, 0.1f)); 
            lbl.style.whiteSpace = WhiteSpace.NoWrap;
            lbl.style.marginBottom = 10;
            lbl.style.paddingBottom = 8;
            lbl.style.paddingTop = 8;
            lbl.style.paddingLeft = 80; // FIX TEXT ALIGNMENT OVER RED LINE!
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
            if (isAudit) { lBtn.style.color = new StyleColor(Color.black); lBtn.style.borderTopColor = new StyleColor(Color.black); lBtn.style.borderBottomColor = new StyleColor(Color.black); lBtn.style.borderLeftColor = new StyleColor(Color.black); lBtn.style.borderRightColor = new StyleColor(Color.black); }
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
        }'''

content = re.sub(target_loop1, replacement_loop1, content)


# 3. Fix Right buttons loop
target_loop2 = r'''        for \(int i=0; i<rights\.Count; i\+\+\)
        \{
            int idx = i;
            var rBtn = new Button \{ text = rights\[idx\] \};
            rBtn\.AddToClassList\("popup-button"\);
            rBtn\.style\.paddingTop = 15; rBtn\.style\.paddingBottom = 15; rBtn\.style\.width = new Length\(100, LengthUnit\.Percent\); rBtn\.style\.fontSize = 20;
            rBtn\.clicked \+= \(\) => OnDragRightClicked\(idx, rBtn\);
            dragRightButtons\.Add\(rBtn\);
            dragRightColumn\.Add\(rBtn\);
        \}'''

replacement_loop2 = r'''        for (int i=0; i<rights.Count; i++)
        {
            int idx = i;
            var rBtn = new Button { text = rights[idx] };
            rBtn.AddToClassList("popup-button");
            rBtn.style.paddingTop = 15; rBtn.style.paddingBottom = 15; rBtn.style.width = new Length(100, LengthUnit.Percent); rBtn.style.fontSize = 20;
            
            if (isAudit) {
                rBtn.style.backgroundColor = new StyleColor(new Color(0.9f, 0.9f, 0.95f));
                rBtn.style.color = new StyleColor(Color.black);
                rBtn.style.borderTopColor = new StyleColor(Color.black); rBtn.style.borderBottomColor = new StyleColor(Color.black); rBtn.style.borderLeftColor = new StyleColor(Color.black); rBtn.style.borderRightColor = new StyleColor(Color.black);
            }
            
            rBtn.clicked += () => OnDragRightClicked(idx, rBtn);
            dragRightButtons.Add(rBtn);
            dragRightColumn.Add(rBtn);
        }'''

content = re.sub(target_loop2, replacement_loop2, content)

# 4. Fix OnDragRightClicked (prevent it from grey-ing out things incorrectly)
target_click = r'''    private void OnDragRightClicked\(int rIdx, Button rBtn\)
    \{
        if \(!manager\.isPlaying \|\| dragSelectedLeft == -1\) return;
        
        if \(dragCorrectMap\[dragSelectedLeft\] == rIdx\)
        \{
            dragLeftButtons\[dragSelectedLeft\]\.style\.backgroundColor = new StyleColor\(Color\.green\);
            dragLeftButtons\[dragSelectedLeft\]\.SetEnabled\(false\);
            rBtn\.style\.backgroundColor = new StyleColor\(Color\.green\);
            rBtn\.SetEnabled\(false\);
            dragMatchCount\+\+;
            if \(dragMatchCount >= config\.matchLeft\.Length\) manager\.MarkWinReady\(\);
        \}
        else
        \{
            dragLeftButtons\[dragSelectedLeft\]\.style\.backgroundColor = new StyleColor\(Color\.gray\);
        \}
        dragSelectedLeft = -1;
    \}'''

replacement_click = r'''    private void OnDragRightClicked(int rIdx, Button rBtn)
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
    }'''

content = re.sub(target_click, replacement_click, content)

with open(path, 'w') as f:
    f.write(content)


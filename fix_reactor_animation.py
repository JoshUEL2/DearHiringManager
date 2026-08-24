import re

path = 'Assets/Scripts/2 Managers/MiniGames/DragMatchMiniGame.cs'

with open(path, 'r') as f:
    content = f.read()

# 1. Add overflow:hidden to generatorPanel
target_panel = r'''            generatorPanel\.style\.borderLeftColor = new StyleColor\(new Color\(0\.2f, 0\.2f, 0\.25f\)\);
            generatorPanel\.style\.borderRightColor = new StyleColor\(new Color\(0\.2f, 0\.2f, 0\.25f\)\);
            generatorPanel\.style\.borderTopLeftRadius = 15; generatorPanel\.style\.borderTopRightRadius = 15;'''

replacement_panel = r'''            generatorPanel.style.borderLeftColor = new StyleColor(new Color(0.2f, 0.2f, 0.25f));
            generatorPanel.style.borderRightColor = new StyleColor(new Color(0.2f, 0.2f, 0.25f));
            generatorPanel.style.borderTopLeftRadius = 15; generatorPanel.style.borderTopRightRadius = 15;
            generatorPanel.style.overflow = Overflow.Hidden;'''

content = re.sub(target_panel, replacement_panel, content)

# 2. Intercept Win condition
target_win = r'''            dragMatchCount\+\+;
            if \(dragMatchCount >= config\.matchLeft\.Length\) manager\.MarkWinReady\(\);'''

replacement_win = r'''            dragMatchCount++;
            if (dragMatchCount >= config.matchLeft.Length) {
                if (!isAudit) manager.StartCoroutine(ReactorWinAnimation());
                else manager.MarkWinReady();
            }'''

content = re.sub(target_win, replacement_win, content)

# 3. Add ReactorWinAnimation before AbortGame
target_abort = r'''    public void AbortGame\(\)
    \{
        if \(sparkCoroutine != null\)'''

replacement_abort = r'''    private System.Collections.IEnumerator ReactorWinAnimation()
    {
        if (sparkCoroutine != null) { manager.StopCoroutine(sparkCoroutine); sparkCoroutine = null; }
        
        var panel = dragLeftColumn.parent;
        if (panel == null) { manager.MarkWinReady(); yield break; }
        
        dragLeftColumn.style.display = DisplayStyle.None;
        dragRightColumn.style.display = DisplayStyle.None;
        var bolt = panel.Q<Label>();
        if (bolt != null) bolt.style.display = DisplayStyle.None;
        
        var frontPanel = new VisualElement();
        frontPanel.style.position = Position.Absolute;
        frontPanel.style.left = 0; frontPanel.style.right = 0;
        frontPanel.style.top = 0; frontPanel.style.bottom = 0;
        frontPanel.style.backgroundColor = new StyleColor(new Color(0.2f, 0.2f, 0.25f)); // Matches casing
        
        var sealText = new Label("SEALED");
        sealText.style.color = new StyleColor(new Color(0.1f, 0.9f, 0.2f));
        sealText.style.fontSize = 50;
        sealText.style.unityFontStyleAndWeight = FontStyle.Bold;
        sealText.style.position = Position.Absolute;
        sealText.style.left = new Length(50, LengthUnit.Percent);
        sealText.style.top = new Length(50, LengthUnit.Percent);
        sealText.style.translate = new StyleTranslate(new Translate(new Length(-50, LengthUnit.Percent), new Length(-50, LengthUnit.Percent), 0));
        frontPanel.Add(sealText);
        
        frontPanel.style.translate = new StyleTranslate(new Translate(0, new Length(-110, LengthUnit.Percent), 0));
        panel.Add(frontPanel);
        
        float elapsed = 0f;
        float duration = 0.4f;
        while(elapsed < duration)
        {
            elapsed += UnityEngine.Time.deltaTime;
            float t = elapsed / duration;
            float ease = 1f - UnityEngine.Mathf.Pow(1f - t, 3f);
            frontPanel.style.translate = new StyleTranslate(new Translate(0, new Length(-110f * (1f - ease), LengthUnit.Percent), 0));
            yield return null;
        }
        frontPanel.style.translate = new StyleTranslate(new Translate(0, 0, 0));
        
        // Screws locking in
        for(int i=0; i<4; i++) {
            var screw = new VisualElement();
            screw.style.position = Position.Absolute;
            screw.style.width = 16; screw.style.height = 16;
            screw.style.backgroundColor = new StyleColor(new Color(0.7f, 0.7f, 0.7f));
            screw.style.borderTopLeftRadius = 8; screw.style.borderTopRightRadius = 8; screw.style.borderBottomLeftRadius = 8; screw.style.borderBottomRightRadius = 8;
            
            if (i==0) { screw.style.left = 10; screw.style.top = 10; }
            if (i==1) { screw.style.right = 10; screw.style.top = 10; }
            if (i==2) { screw.style.left = 10; screw.style.bottom = 10; }
            if (i==3) { screw.style.right = 10; screw.style.bottom = 10; }
            
            frontPanel.Add(screw);
            
            screw.style.scale = new StyleScale(new Scale(UnityEngine.Vector3.zero));
            float sElapsed = 0f;
            while(sElapsed < 0.1f) {
                sElapsed += UnityEngine.Time.deltaTime;
                screw.style.scale = new StyleScale(new Scale(new UnityEngine.Vector3(sElapsed*10f, sElapsed*10f, 1f)));
                yield return null;
            }
            screw.style.scale = new StyleScale(new Scale(UnityEngine.Vector3.one));
        }
        
        yield return new UnityEngine.WaitForSeconds(0.6f);
        manager.MarkWinReady();
    }

    public void AbortGame()
    {
        if (sparkCoroutine != null)'''

content = re.sub(target_abort, replacement_abort, content)

with open(path, 'w') as f:
    f.write(content)


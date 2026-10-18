using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;

public class DragMatchMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement container;

    private VisualElement dragLeftColumn;
    private VisualElement dragRightColumn;
    private VisualElement dragCheatSheetList;
    private Label matchPairScoreText;

    private int dragSelectedLeft = -1;
    private List<Button> dragLeftButtons = new List<Button>();
    private List<Button> dragRightButtons = new List<Button>();
    private int dragMatchCount = 0;
    private Dictionary<int,int> dragCorrectMap = new Dictionary<int,int>();
    private Coroutine sparkCoroutine;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;

        container = root.Q<VisualElement>("DragMatchContainer");
        dragLeftColumn = root.Q<VisualElement>("DragLeftColumn");
        dragRightColumn = root.Q<VisualElement>("DragRightColumn");
        dragCheatSheetList = root.Q<VisualElement>("DragCheatSheetList");
        matchPairScoreText = root.Q<Label>("MatchPairScoreText");
    }

    public void StartGame()
    {
        container.style.display = DisplayStyle.Flex;
        SetupDragMatch();
    }

    private System.Collections.IEnumerator ReactorWinAnimation()
    {
        if (sparkCoroutine != null) { manager.StopCoroutine(sparkCoroutine); sparkCoroutine = null; }
        
        var panel = dragLeftColumn.parent;
        if (panel == null) { manager.MarkWinReady(); yield break; }
        
        // Hide elements without collapsing layout size!
        dragLeftColumn.style.visibility = Visibility.Hidden;
        dragRightColumn.style.visibility = Visibility.Hidden;
        var bolt = panel.Q<Label>();
        if (bolt != null) bolt.style.visibility = Visibility.Hidden;
        
        var frontPanel = new VisualElement();
        frontPanel.style.position = Position.Absolute;
        frontPanel.style.left = 0; frontPanel.style.right = 0;
        frontPanel.style.top = 0; frontPanel.style.bottom = 0;
        frontPanel.style.backgroundColor = new StyleColor(new Color(0.2f, 0.2f, 0.25f)); // Matches casing
        
        // Trapdoor swing animation (top hinged)
        frontPanel.style.transformOrigin = new StyleTransformOrigin(new TransformOrigin(Length.Percent(50), Length.Percent(0)));
        frontPanel.style.rotate = new StyleRotate(new Rotate(Angle.Degrees(-90))); // Open upwards
        
        panel.Add(frontPanel);
        
        float elapsed = 0f;
        float duration = 0.4f;
        while(elapsed < duration)
        {
            elapsed += UnityEngine.Time.deltaTime;
            float t = elapsed / duration;
            // Accelerating swing down (gravity)
            float angle = UnityEngine.Mathf.Lerp(-90f, 0f, t * t * t);
            frontPanel.style.rotate = new StyleRotate(new Rotate(Angle.Degrees(angle)));
            yield return null;
        }
        frontPanel.style.rotate = new StyleRotate(new Rotate(Angle.Degrees(0)));
        
        // Clang impact shake
        var sv = manager.miniGameDocument.rootVisualElement.Q<ScrollView>();
        if (sv != null) {
            float sElapsed = 0f;
            while(sElapsed < 0.15f) {
                sElapsed += UnityEngine.Time.deltaTime;
                float shakeX = UnityEngine.Random.Range(-15f, 15f) * (1f - (sElapsed/0.15f));
                float shakeY = UnityEngine.Random.Range(-15f, 15f) * (1f - (sElapsed/0.15f));
                sv.style.translate = new StyleTranslate(new Translate(shakeX, shakeY, 0));
                yield return null;
            }
            sv.style.translate = new StyleTranslate(new Translate(0, 0, 0));
        }
        
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
        if (sparkCoroutine != null) { manager.StopCoroutine(sparkCoroutine); sparkCoroutine = null; }
        container.style.display = DisplayStyle.None;
    }
    
    private System.Collections.IEnumerator SparkRoutine(Label bolt)
    {
        while (manager.isPlaying && container.style.display != DisplayStyle.None)
        {
            yield return new UnityEngine.WaitForSeconds(UnityEngine.Random.Range(1.5f, 4f));
            if (bolt == null) break;
            
            // Spawn physical sparks
            for(int i=0; i<8; i++) {
                manager.StartCoroutine(AnimateParticle());
            }
            
            // Multi-spark flash
            bolt.style.color = new StyleColor(UnityEngine.Color.white);
            bolt.style.scale = new StyleScale(new Scale(new UnityEngine.Vector3(1.3f, 1.3f, 1f)));
            yield return new UnityEngine.WaitForSeconds(0.05f);
            bolt.style.color = new StyleColor(new UnityEngine.Color(1f, 0.8f, 0.1f));
            bolt.style.scale = new StyleScale(new Scale(UnityEngine.Vector3.one));
            yield return new UnityEngine.WaitForSeconds(0.1f);
            
            bolt.style.color = new StyleColor(UnityEngine.Color.white);
            bolt.style.scale = new StyleScale(new Scale(new UnityEngine.Vector3(1.5f, 1.5f, 1f)));
            yield return new UnityEngine.WaitForSeconds(0.05f);
            bolt.style.color = new StyleColor(new UnityEngine.Color(1f, 0.8f, 0.1f));
            bolt.style.scale = new StyleScale(new Scale(UnityEngine.Vector3.one));
        }
    }

    private void SetupDragMatch()
    {
        var sv = manager.miniGameDocument.rootVisualElement.Q<ScrollView>();
        if (sv != null)
        {
            sv.style.minHeight = 850;
            sv.style.maxHeight = 2500;
        }
        
        dragLeftColumn.style.visibility = Visibility.Visible;
        dragRightColumn.style.visibility = Visibility.Visible;
        dragLeftColumn.Clear();
        dragRightColumn.Clear();
        dragCheatSheetList.Clear();
        dragLeftButtons.Clear();
        dragRightButtons.Clear();
        dragCorrectMap.Clear();
        dragMatchCount = 0;
        dragSelectedLeft = -1;
        
        bool isAudit = manager.currentGame != null && manager.currentGame.name == "Audit sewage drainage lines";
        
        var cheatSheet = container.Q<VisualElement>("DragCheatSheet");
        var generatorPanel = dragLeftColumn.parent;
        
        // Remove old callbacks to prevent duplicates
        container.generateVisualContent -= OnGenerateFullPaper;
        generatorPanel.generateVisualContent -= OnGenerateGeneratorPanel;
        if (cheatSheet != null) cheatSheet.generateVisualContent -= OnGenerateCheatSheetPaper;
        
        if (isAudit)
        {
            container.generateVisualContent += OnGenerateFullPaper;
            container.style.backgroundColor = new StyleColor(new Color(0.98f, 0.96f, 0.9f)); 
            container.style.borderTopColor = new StyleColor(new Color(0.3f, 0.3f, 0.3f));
            container.style.borderBottomColor = new StyleColor(new Color(0.3f, 0.3f, 0.3f));
            container.style.borderLeftColor = new StyleColor(new Color(0.3f, 0.3f, 0.3f));
            container.style.borderRightColor = new StyleColor(new Color(0.3f, 0.3f, 0.3f));
            
            generatorPanel.style.backgroundColor = new StyleColor(Color.clear);
            generatorPanel.style.borderTopWidth = 0; generatorPanel.style.borderBottomWidth = 0;
            generatorPanel.style.borderLeftWidth = 0; generatorPanel.style.borderRightWidth = 0;
            
            var bolt = generatorPanel.Q<Label>();
            if (bolt != null) { bolt.text = "✏️"; bolt.style.color = new StyleColor(Color.black); }
        }
        else
        {
            // Generator Top Half!
            container.style.backgroundColor = new StyleColor(Color.clear);
            container.style.borderTopColor = new StyleColor(Color.clear);
            
            generatorPanel.style.backgroundColor = new StyleColor(new Color(0.08f, 0.1f, 0.12f)); // Dark internals
            generatorPanel.style.borderTopWidth = 20; generatorPanel.style.borderBottomWidth = 20;
            generatorPanel.style.borderLeftWidth = 20; generatorPanel.style.borderRightWidth = 20;
            generatorPanel.style.borderTopColor = new StyleColor(new Color(0.25f, 0.25f, 0.3f)); // Thick grey casing
            generatorPanel.style.borderBottomColor = new StyleColor(new Color(0.15f, 0.15f, 0.2f));
            generatorPanel.style.borderLeftColor = new StyleColor(new Color(0.2f, 0.2f, 0.25f));
            generatorPanel.style.borderRightColor = new StyleColor(new Color(0.2f, 0.2f, 0.25f));
            generatorPanel.style.borderTopLeftRadius = 15; generatorPanel.style.borderTopRightRadius = 15;
            generatorPanel.style.overflow = Overflow.Hidden;
            
            generatorPanel.generateVisualContent += OnGenerateGeneratorPanel;
            
            var bolt = generatorPanel.Q<Label>();
            if (bolt != null) { bolt.text = "⚡"; bolt.style.color = new StyleColor(new Color(1f, 0.8f, 0.1f)); bolt.style.fontSize = 60; }
            
            if (sparkCoroutine != null) manager.StopCoroutine(sparkCoroutine);
            sparkCoroutine = manager.StartCoroutine(SparkRoutine(bolt));
        }
        
        if (cheatSheet != null) {
            cheatSheet.style.backgroundColor = new StyleColor(Color.clear);
            cheatSheet.style.borderTopWidth = 0; cheatSheet.style.borderBottomWidth = 0; cheatSheet.style.borderLeftWidth = 0; cheatSheet.style.borderRightWidth = 0;
            var firstChild = cheatSheet.Children().FirstOrDefault();
            if (firstChild != null && firstChild.style.position == Position.Absolute) firstChild.style.display = DisplayStyle.None;
            
            var titleLbl = cheatSheet.Q<Label>();
            if (titleLbl != null) { titleLbl.style.color = new StyleColor(Color.black); titleLbl.style.borderBottomColor = new StyleColor(Color.black); }
            
            if (!isAudit)
            {
                cheatSheet.generateVisualContent += OnGenerateCheatSheetPaper;
                cheatSheet.style.backgroundColor = new StyleColor(new Color(0.98f, 0.96f, 0.9f)); // Paper background just for the bottom
                
                // Add High Voltage Stamp
                var warningText = new Label("⚠️ HIGH VOLTAGE ⚠️");
                warningText.style.position = Position.Absolute;
                warningText.style.bottom = 35;
                warningText.style.right = 0;
                warningText.style.color = new StyleColor(new Color(0.9f, 0.2f, 0.2f, 0.4f)); 
                warningText.style.fontSize = 32;
                warningText.style.unityFontStyleAndWeight = FontStyle.Bold;
                warningText.style.rotate = new StyleRotate(new Rotate(Angle.Degrees(-15)));
                cheatSheet.Add(warningText);
            }
        }
        
        var lefts = config.matchLeft.ToList();
        var rights = config.matchRight.ToList();
        var rightIndices = Enumerable.Range(0, rights.Count).OrderBy(x => Random.value).ToList();
        
        Color textColor = isAudit ? new Color(0.1f, 0.1f, 0.1f) : new Color(0.9f, 0.9f, 0.9f);
        Color btnBg = isAudit ? new Color(0.9f, 0.9f, 0.95f) : new Color(0.15f, 0.15f, 0.2f);
        Color btnBorder = isAudit ? Color.black : new Color(0.1f, 0.1f, 0.1f);
        Color btnSel = isAudit ? new Color(0.4f, 0.6f, 1f) : new Color(0.9f, 0.8f, 0.1f); // Yellow selection for reactor
        
        for (int i=0; i<lefts.Count; i++)
        {
            var lbl = new Label($"{lefts[i]} = {rights[rightIndices[i]]}");
            lbl.style.fontSize = 22;
            lbl.style.color = new StyleColor(new Color(0.1f, 0.1f, 0.1f)); 
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
            lBtn.style.borderTopLeftRadius = 15; lBtn.style.borderTopRightRadius = 15; lBtn.style.borderBottomLeftRadius = 15; lBtn.style.borderBottomRightRadius = 15;
            lBtn.style.backgroundColor = new StyleColor(btnBg);
            
            lBtn.clicked += () => { 
                if (!lBtn.enabledSelf) return;
                MiniGameFeedback.PlayTap(lBtn);
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
            rBtn.style.borderTopLeftRadius = 15; rBtn.style.borderTopRightRadius = 15; rBtn.style.borderBottomLeftRadius = 15; rBtn.style.borderBottomRightRadius = 15;
            
            rBtn.clicked += () => OnDragRightClicked(idx, rBtn);
            dragRightButtons.Add(rBtn);
            dragRightColumn.Add(rBtn);
        }
    }

    private void OnDragRightClicked(int rIdx, Button rBtn)
    {
        if (!manager.isPlaying || dragSelectedLeft == -1) return;
        
        bool isAudit = manager.currentGame != null && manager.currentGame.name == "Audit sewage drainage lines";
        Color matchedColor = isAudit ? new Color(0.2f, 0.8f, 0.2f) : new Color(0.1f, 0.8f, 0.2f);
        Color lDef = isAudit ? new Color(0.9f, 0.9f, 0.95f) : new Color(0.15f, 0.15f, 0.2f);
        Color textColor = isAudit ? new Color(0.1f, 0.1f, 0.1f) : new Color(0.9f, 0.9f, 0.9f);
        Color errorColor = isAudit ? new Color(0.9f, 0.4f, 0.4f) : new Color(0.9f, 0.1f, 0.1f);
        
        if (dragCorrectMap[dragSelectedLeft] == rIdx)
        {
            MiniGameFeedback.PlaySuccess(rBtn, "CONNECTED!", container);
            MiniGameFeedback.Punch(dragLeftButtons[dragSelectedLeft], 1.2f);
            dragLeftButtons[dragSelectedLeft].style.backgroundColor = new StyleColor(matchedColor);
            dragLeftButtons[dragSelectedLeft].style.color = new StyleColor(Color.black);
            dragLeftButtons[dragSelectedLeft].SetEnabled(false);
            rBtn.style.backgroundColor = new StyleColor(matchedColor);
            rBtn.style.color = new StyleColor(Color.black);
            rBtn.SetEnabled(false);
            dragMatchCount++;
            if (dragMatchCount >= config.matchLeft.Length) {
                if (!isAudit) manager.StartCoroutine(ReactorWinAnimation());
                else manager.MarkWinReady();
            }
        }
        else
        {
            MiniGameFeedback.PlayError(rBtn, "MISMATCH", container);
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

    private void OnGenerateFullPaper(MeshGenerationContext ctx)
    {
        var paint2D = ctx.painter2D;
        float w = container.layout.width;
        float h = container.layout.height;

        paint2D.strokeColor = new Color(0.9f, 0.3f, 0.3f, 0.8f);
        paint2D.lineWidth = 2f;
        paint2D.BeginPath(); paint2D.MoveTo(new Vector2(60, 0)); paint2D.LineTo(new Vector2(60, h)); paint2D.Stroke();

        paint2D.strokeColor = new Color(0.3f, 0.6f, 0.9f, 0.4f);
        paint2D.lineWidth = 1f;
        for(float y = 40; y < h; y += 38)
        {
            paint2D.BeginPath(); paint2D.MoveTo(new Vector2(0, y)); paint2D.LineTo(new Vector2(w, y)); paint2D.Stroke();
        }
    }
    
    private void OnGenerateCheatSheetPaper(MeshGenerationContext ctx)
    {
        var paint2D = ctx.painter2D;
        float w = ctx.visualElement.layout.width;
        float h = ctx.visualElement.layout.height;

        paint2D.strokeColor = new Color(0.9f, 0.3f, 0.3f, 0.8f);
        paint2D.lineWidth = 2f;
        paint2D.BeginPath(); paint2D.MoveTo(new Vector2(60, 0)); paint2D.LineTo(new Vector2(60, h)); paint2D.Stroke();

        paint2D.strokeColor = new Color(0.3f, 0.6f, 0.9f, 0.4f);
        paint2D.lineWidth = 1f;
        for(float y = 40; y < h; y += 38)
        {
            paint2D.BeginPath(); paint2D.MoveTo(new Vector2(0, y)); paint2D.LineTo(new Vector2(w, y)); paint2D.Stroke();
        }
    }
    
    private void OnGenerateGeneratorPanel(MeshGenerationContext ctx)
    {
        var p = ctx.painter2D;
        float w = ctx.visualElement.layout.width;
        float h = ctx.visualElement.layout.height;
        
        // Hazard Stripes at Top
        p.fillColor = new Color(0.9f, 0.8f, 0.1f); // Yellow base
        p.BeginPath(); 
        p.MoveTo(new Vector2(0,0)); p.LineTo(new Vector2(w,0)); 
        p.LineTo(new Vector2(w,20)); p.LineTo(new Vector2(0,20)); 
        p.ClosePath(); p.Fill();
        p.fillColor = new Color(0.1f, 0.1f, 0.1f); // Black stripes
        for(float x = -20; x < w; x += 40) {
            p.BeginPath();
            p.MoveTo(new Vector2(x, 0)); p.LineTo(new Vector2(x+20, 0));
            p.LineTo(new Vector2(x+5, 20)); p.LineTo(new Vector2(x-15, 20));
            p.Fill();
        }
        
        // Internal Exposed Wires
        p.strokeColor = new Color(0.8f, 0.2f, 0.2f, 0.4f); // Red wire
        p.lineWidth = 6f;
        p.BeginPath(); p.MoveTo(new Vector2(0, 50)); p.BezierCurveTo(new Vector2(w/3, 100), new Vector2(2*w/3, 20), new Vector2(w, 80)); p.Stroke();
        
        p.strokeColor = new Color(0.2f, 0.6f, 0.9f, 0.4f); // Blue wire
        p.lineWidth = 8f;
        p.BeginPath(); p.MoveTo(new Vector2(0, h-40)); p.BezierCurveTo(new Vector2(w/3, h-100), new Vector2(2*w/3, h), new Vector2(w, h-60)); p.Stroke();
        
        // Casing Screws
        p.fillColor = new Color(0.7f, 0.7f, 0.7f);
        p.BeginPath(); p.Arc(new Vector2(20, h-20), 6, 0, 360); p.Fill();
        p.BeginPath(); p.Arc(new Vector2(w-20, h-20), 6, 0, 360); p.Fill();
    }

    private System.Collections.IEnumerator AnimateParticle()
    {
        var panel = dragLeftColumn.parent;
        if (panel == null) yield break;
        
        var particle = new VisualElement();
        particle.style.position = Position.Absolute;
        particle.style.width = UnityEngine.Random.Range(6, 14);
        particle.style.height = particle.style.width;
        particle.style.backgroundColor = new StyleColor(UnityEngine.Random.value > 0.5f ? UnityEngine.Color.white : UnityEngine.Color.yellow);
        particle.style.borderTopLeftRadius = 10; particle.style.borderTopRightRadius = 10; particle.style.borderBottomLeftRadius = 10; particle.style.borderBottomRightRadius = 10;
        
        particle.style.left = new Length(50, LengthUnit.Percent);
        particle.style.top = new Length(50, LengthUnit.Percent);
        
        panel.Add(particle);
        
        float angle = UnityEngine.Random.Range(0f, 360f) * UnityEngine.Mathf.Deg2Rad;
        float speed = UnityEngine.Random.Range(300f, 700f);
        UnityEngine.Vector2 velocity = new UnityEngine.Vector2(UnityEngine.Mathf.Cos(angle), UnityEngine.Mathf.Sin(angle)) * speed;
        
        float lifeTime = UnityEngine.Random.Range(0.2f, 0.5f);
        float elapsed = 0f;
        
        float x = 0;
        float y = 0;
        
        while(elapsed < lifeTime)
        {
            if (particle == null || panel == null) break;
            float dt = UnityEngine.Time.deltaTime;
            elapsed += dt;
            
            x += velocity.x * dt;
            y += velocity.y * dt;
            velocity.y += 1200f * dt; // Gravity
            
            particle.style.translate = new StyleTranslate(new Translate(new Length(x, LengthUnit.Pixel), new Length(y, LengthUnit.Pixel), 0));
            particle.style.opacity = 1f - (elapsed / lifeTime);
            
            yield return null;
        }
        
        if (particle != null && particle.parent != null) particle.RemoveFromHierarchy();
    }
}

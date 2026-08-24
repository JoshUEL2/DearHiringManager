import re

path = 'Assets/Scripts/2 Managers/MiniGames/OrderQueueMiniGame.cs'

with open(path, 'r') as f:
    content = f.read()

# 1. Add Dance Stage variables
target_vars = r'private int orderInputIndex = 0;'
replacement_vars = r'''private int orderInputIndex = 0;
    
    // Dance minigame visuals
    private VisualElement danceStage;
    private Label dancerAvatar;
    private Coroutine discoRoutine;'''

content = re.sub(target_vars, replacement_vars, content)

# 2. Setup Dance Stage in Initialize
target_init = r'orderItemGrid = container\.Q<VisualElement>\("OrderItemGrid"\);'
replacement_init = r'''orderItemGrid = container.Q<VisualElement>("OrderItemGrid");
        
        // Setup Dance Stage
        if (manager.currentGame != null && manager.currentGame.name == "Dance with sign for tourists")
        {
            danceStage = new VisualElement();
            danceStage.style.height = 180;
            danceStage.style.width = Length.Percent(100);
            danceStage.style.alignItems = Align.Center;
            danceStage.style.justifyContent = Justify.Center;
            danceStage.style.marginBottom = 20;
            danceStage.style.backgroundColor = new StyleColor(new Color(0.1f, 0.05f, 0.2f));
            danceStage.style.borderTopLeftRadius = 20; danceStage.style.borderTopRightRadius = 20;
            danceStage.style.borderBottomLeftRadius = 20; danceStage.style.borderBottomRightRadius = 20;
            danceStage.style.overflow = Overflow.Hidden;
            
            Label discoBall = new Label("🪩");
            discoBall.style.position = Position.Absolute;
            discoBall.style.top = 10;
            discoBall.style.fontSize = 40;
            danceStage.Add(discoBall);
            
            dancerAvatar = new Label("🕺");
            dancerAvatar.style.fontSize = 80;
            dancerAvatar.style.position = Position.Absolute;
            dancerAvatar.style.top = 70;
            danceStage.Add(dancerAvatar);
            
            // Insert dance stage before the grid
            int gridIndex = container.IndexOf(orderItemGrid);
            if (gridIndex >= 0) container.Insert(gridIndex, danceStage);
            else container.Insert(0, danceStage);
        }'''

content = re.sub(target_init, replacement_init, content)

# 3. Add styling in StartGame
target_start = r'else if \(manager\.currentGame\.name == "Verify hotfix deployment"\)'
replacement_start = r'''else if (manager.currentGame.name == "Dance with sign for tourists")
            {
                container.style.backgroundColor = new StyleColor(new Color(0.15f, 0.05f, 0.25f)); // Deep purple
                container.style.borderTopWidth = 6; container.style.borderBottomWidth = 6; container.style.borderLeftWidth = 6; container.style.borderRightWidth = 6;
                container.style.borderTopColor = new StyleColor(new Color(1f, 0f, 0.8f)); // Neon pink
                container.style.borderBottomColor = new StyleColor(new Color(0f, 1f, 0.8f)); // Neon cyan
                container.style.borderLeftColor = new StyleColor(new Color(1f, 0f, 0.8f));
                container.style.borderRightColor = new StyleColor(new Color(0f, 1f, 0.8f));
                container.style.borderTopLeftRadius = 30; container.style.borderTopRightRadius = 30; container.style.borderBottomLeftRadius = 30; container.style.borderBottomRightRadius = 30;
                
                if (discoRoutine != null) manager.StopCoroutine(discoRoutine);
                discoRoutine = manager.StartCoroutine(DiscoRoutine());
            }
            else if (manager.currentGame.name == "Verify hotfix deployment")'''

content = re.sub(target_start, replacement_start, content)

# 4. Neon styling for Dance buttons
target_btn = r'else if \(manager\.currentGame != null && manager\.currentGame\.name == "Verify hotfix deployment"\)'
replacement_btn = r'''else if (manager.currentGame != null && manager.currentGame.name == "Dance with sign for tourists")
        {
            btn.style.backgroundColor = new StyleColor(isPreview ? new Color(0.2f, 0.05f, 0.3f) : new Color(0.1f, 0.05f, 0.2f));
            btn.style.color = new StyleColor(Color.white);
            btn.style.borderTopWidth = 3; btn.style.borderBottomWidth = 3; btn.style.borderLeftWidth = 3; btn.style.borderRightWidth = 3;
            btn.style.borderTopColor = new StyleColor(new Color(0f, 1f, 0.8f)); // Cyan
            btn.style.borderBottomColor = new StyleColor(new Color(1f, 0f, 0.8f)); // Pink
            btn.style.borderLeftColor = new StyleColor(new Color(0f, 1f, 0.8f));
            btn.style.borderRightColor = new StyleColor(new Color(1f, 0f, 0.8f));
            btn.style.borderTopLeftRadius = 10; btn.style.borderTopRightRadius = 10; btn.style.borderBottomLeftRadius = 10; btn.style.borderBottomRightRadius = 10;
        }
        else if (manager.currentGame != null && manager.currentGame.name == "Verify hotfix deployment")'''

content = re.sub(target_btn, replacement_btn, content)

# 5. Handle Dance Animation on Click
target_click = r'btn\.style\.backgroundColor = new StyleColor\(Color\.green\);'
replacement_click = r'''btn.style.backgroundColor = new StyleColor(Color.green);
            if (manager.currentGame.name == "Dance with sign for tourists")
            {
                manager.StartCoroutine(DanceMoveAnimation(item));
            }'''

content = re.sub(target_click, replacement_click, content)

# 6. Stop coroutines in AbortGame exactly
target_abort = r'''    public void AbortGame\(\)
    \{
        isPlaying = false;
        container\.style\.display = DisplayStyle\.None;
    \}'''
replacement_abort = r'''    public void AbortGame()
    {
        isPlaying = false;
        if (discoRoutine != null) { manager.StopCoroutine(discoRoutine); discoRoutine = null; }
        container.style.display = DisplayStyle.None;
    }'''

content = re.sub(target_abort, replacement_abort, content)


# 7. Add DanceMoveAnimation and DiscoRoutine
target_end = r'}\s*$'
replacement_end = r'''
    private IEnumerator DiscoRoutine()
    {
        Color[] colors = new Color[] {
            new Color(0.2f, 0.05f, 0.3f), // Purple
            new Color(0.1f, 0.2f, 0.3f),  // Blue
            new Color(0.3f, 0.05f, 0.1f), // Red
            new Color(0.1f, 0.3f, 0.1f)   // Green
        };
        int idx = 0;
        while (isPlaying && manager.isPlaying)
        {
            if (danceStage != null)
            {
                danceStage.style.backgroundColor = new StyleColor(colors[idx]);
            }
            idx = (idx + 1) % colors.Length;
            yield return new WaitForSeconds(0.4f);
        }
    }

    private IEnumerator DanceMoveAnimation(string move)
    {
        if (dancerAvatar == null) yield break;
        
        float duration = 0.3f;
        float elapsed = 0f;
        
        Vector2 startPos = new Vector2(0, 70); // translate X, Y
        
        while (elapsed < duration && isPlaying)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            if (move == "Left")
            {
                dancerAvatar.style.translate = new Translate(new Length(Mathf.Lerp(0, -60, Mathf.Sin(t * Mathf.PI)), LengthUnit.Pixel), 0);
            }
            else if (move == "Right")
            {
                dancerAvatar.style.translate = new Translate(new Length(Mathf.Lerp(0, 60, Mathf.Sin(t * Mathf.PI)), LengthUnit.Pixel), 0);
            }
            else if (move == "Spin")
            {
                dancerAvatar.style.rotate = new StyleRotate(new Rotate(new Angle(Mathf.Lerp(0, 360, t), AngleUnit.Degree)));
            }
            else if (move == "Pose")
            {
                float scale = Mathf.Lerp(1f, 1.5f, Mathf.Sin(t * Mathf.PI));
                dancerAvatar.style.scale = new StyleScale(new Vector2(scale, scale));
            }
            else if (move == "Wave")
            {
                dancerAvatar.style.rotate = new StyleRotate(new Rotate(new Angle(Mathf.Lerp(0, 45, Mathf.Sin(t * Mathf.PI * 2)), AngleUnit.Degree)));
            }
            else if (move == "Dip")
            {
                dancerAvatar.style.translate = new Translate(0, new Length(Mathf.Lerp(0, 40, Mathf.Sin(t * Mathf.PI)), LengthUnit.Pixel));
            }
            
            yield return null;
        }
        
        dancerAvatar.style.translate = new Translate(0, 0);
        dancerAvatar.style.rotate = new StyleRotate(new Rotate(new Angle(0, AngleUnit.Degree)));
        dancerAvatar.style.scale = new StyleScale(new Vector2(1, 1));
    }
}'''

content = re.sub(target_end, replacement_end, content)

with open(path, 'w') as f:
    f.write(content)


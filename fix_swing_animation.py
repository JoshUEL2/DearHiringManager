import re

path = 'Assets/Scripts/2 Managers/MiniGames/DragMatchMiniGame.cs'

with open(path, 'r') as f:
    content = f.read()

target = r'''    private System\.Collections\.IEnumerator ReactorWinAnimation\(\)
    \{
        if \(sparkCoroutine != null\) \{ manager\.StopCoroutine\(sparkCoroutine\); sparkCoroutine = null; \}
        
        var panel = dragLeftColumn\.parent;
        if \(panel == null\) \{ manager\.MarkWinReady\(\); yield break; \}
        
        dragLeftColumn\.style\.display = DisplayStyle\.None;
        dragRightColumn\.style\.display = DisplayStyle\.None;
        var bolt = panel\.Q<Label>\(\);
        if \(bolt != null\) bolt\.style\.display = DisplayStyle\.None;
        
        var frontPanel = new VisualElement\(\);
        frontPanel\.style\.position = Position\.Absolute;
        frontPanel\.style\.left = 0; frontPanel\.style\.right = 0;
        frontPanel\.style\.top = 0; frontPanel\.style\.bottom = 0;
        frontPanel\.style\.backgroundColor = new StyleColor\(new Color\(0\.2f, 0\.2f, 0\.25f\)\); // Matches casing
        
        var sealText = new Label\("SEALED"\);.*?frontPanel\.Add\(sealText\);
        
        frontPanel\.style\.translate = new StyleTranslate\(new Translate\(0, new Length\(-110, LengthUnit\.Percent\), 0\)\);
        panel\.Add\(frontPanel\);
        
        float elapsed = 0f;
        float duration = 0\.4f;
        while\(elapsed < duration\)
        \{
            elapsed \+= UnityEngine\.Time\.deltaTime;
            float t = elapsed / duration;
            float ease = 1f - UnityEngine\.Mathf\.Pow\(1f - t, 3f\);
            frontPanel\.style\.translate = new StyleTranslate\(new Translate\(0, new Length\(-110f \* \(1f - ease\), LengthUnit\.Percent\), 0\)\);
            yield return null;
        \}
        frontPanel\.style\.translate = new StyleTranslate\(new Translate\(0, 0, 0\)\);'''

replacement = r'''    private System.Collections.IEnumerator ReactorWinAnimation()
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
        }'''

content = re.sub(target, replacement, content, flags=re.DOTALL)

with open(path, 'w') as f:
    f.write(content)


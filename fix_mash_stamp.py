import re

path = 'Assets/Scripts/2 Managers/MiniGames/MashRaceMiniGame.cs'
with open(path, 'r') as f:
    content = f.read()

target1 = r'''        if \(score >= config\.targetScore\)
        \{
            isPlaying = false;
            float pct = Mathf\.Clamp01\(playerMashProgress / config\.targetScore\) \* 100f;
            if \(playerProgressBar != null\) playerProgressBar\.style\.width = new Length\(pct, LengthUnit\.Percent\);
            manager\.MarkWinReady\(\);
        \}'''

replacement1 = r'''        if (score >= config.targetScore)
        {
            isPlaying = false;
            float pct = Mathf.Clamp01(playerMashProgress / config.targetScore) * 100f;
            if (playerProgressBar != null) playerProgressBar.style.width = new Length(pct, LengthUnit.Percent);
            
            string winText = (manager.currentGame != null && manager.currentGame.name == "Stress test server load") ? "SERVER OVERLOADED!" : "PULVERISED!";
            manager.StartCoroutine(StampEffectRoutine(winText, new Color(0.18f, 0.8f, 0.44f), true));
        }'''
content = re.sub(target1, replacement1, content)

target2 = r'''            if \(opponentProgress >= config\.targetScore\)
            \{
                if \(mashTimerText != null\) mashTimerText\.text = "OPPONENT WINS!";
                isPlaying = false;
                manager\.AbortGame\(\);
                yield break;
            \}'''

replacement2 = r'''            if (opponentProgress >= config.targetScore)
            {
                if (mashTimerText != null) mashTimerText.text = "OPPONENT WINS!";
                isPlaying = false;
                
                string loseText = (manager.currentGame != null && manager.currentGame.name == "Stress test server load") ? "TEST FAILED!" : "CRUSHED!";
                manager.StartCoroutine(StampEffectRoutine(loseText, new Color(0.93f, 0.26f, 0.22f), false));
                
                yield break;
            }'''
content = re.sub(target2, replacement2, content)


target3 = r'''    private IEnumerator PunchEffectRoutine\(VisualElement bar, Color originalColor, bool isPlayer\)
    \{'''
replacement3 = r'''    private IEnumerator StampEffectRoutine(string text, Color color, bool isWin)
    {
        Label stampText = new Label(text);
        stampText.style.position = Position.Absolute;
        stampText.style.color = new StyleColor(color);
        stampText.style.fontSize = 72;
        stampText.style.unityFontStyleAndWeight = FontStyle.Bold;
        stampText.style.unityTextAlign = TextAnchor.MiddleCenter;
        
        VisualElement overlay = new VisualElement();
        overlay.style.position = Position.Absolute;
        overlay.style.width = new Length(100, LengthUnit.Percent);
        overlay.style.height = new Length(100, LengthUnit.Percent);
        overlay.style.backgroundColor = new StyleColor(new Color(0f, 0f, 0f, 0.6f));
        
        stampText.style.top = new Length(50, LengthUnit.Percent);
        stampText.style.left = new Length(50, LengthUnit.Percent);
        stampText.style.translate = new Translate(new Length(-50, LengthUnit.Percent), new Length(-50, LengthUnit.Percent));
        
        overlay.Add(stampText);
        container.Add(overlay);
        
        float life = 0.8f;
        float elapsed = 0f;
        while(elapsed < life)
        {
            elapsed += Time.deltaTime;
            float p = elapsed / life;
            
            float scale = Mathf.Lerp(2.5f, 1f, p * 10f);
            if (scale < 1f) scale = 1f;
            stampText.style.scale = new StyleScale(new Vector2(scale, scale));
            
            if (p > 0.8f) {
                overlay.style.opacity = 1f - ((p - 0.8f) * 5f);
            }
            
            yield return null;
        }
        
        if (container.Contains(overlay)) container.Remove(overlay);
        
        if (isWin) manager.MarkWinReady();
        else manager.AbortGame();
    }

    private IEnumerator PunchEffectRoutine(VisualElement bar, Color originalColor, bool isPlayer)
    {'''
content = re.sub(target3, replacement3, content)

with open(path, 'w') as f:
    f.write(content)

import re

path = 'Assets/Scripts/2 Managers/MiniGames/MashRaceMiniGame.cs'
with open(path, 'r') as f:
    content = f.read()

target1 = r'''    private void OnMashTapClicked\(\)
    \{
        if \(!isPlaying \|\| !manager\.isPlaying\) return;

        playerMashProgress \+= 1f;
        score = Mathf\.FloorToInt\(playerMashProgress\);
        
        if \(score >= config\.targetScore\)
        \{
            isPlaying = false;
            float pct = Mathf\.Clamp01\(playerMashProgress / config\.targetScore\) \* 100f;
            if \(playerProgressBar != null\) playerProgressBar\.style\.width = new Length\(pct, LengthUnit\.Percent\);
            manager\.MarkWinReady\(\);
        \}
    \}'''

replacement1 = r'''    private void OnMashTapClicked()
    {
        if (!isPlaying || !manager.isPlaying) return;

        playerMashProgress += 1f;
        score = Mathf.FloorToInt(playerMashProgress);
        
        // Physical Button Press
        if (btnMashTap != null)
        {
            btnMashTap.style.scale = new StyleScale(new Vector2(0.9f, 0.9f));
            btnMashTap.schedule.Execute(() => {
                if (btnMashTap != null) btnMashTap.style.scale = new StyleScale(new Vector2(1f, 1f));
            }).StartingIn(50);
        }
        
        // Simulate a "Punch" - Screen shake and flash Player Bar
        manager.StartCoroutine(PunchEffectRoutine(playerProgressBar, new Color(0.18f, 0.8f, 0.44f), true));
        
        if (score >= config.targetScore)
        {
            isPlaying = false;
            float pct = Mathf.Clamp01(playerMashProgress / config.targetScore) * 100f;
            if (playerProgressBar != null) playerProgressBar.style.width = new Length(pct, LengthUnit.Percent);
            manager.MarkWinReady();
        }
    }
    
    private IEnumerator PunchEffectRoutine(VisualElement bar, Color originalColor, bool isPlayer)
    {
        if (bar != null) bar.style.backgroundColor = new StyleColor(Color.white); // Flash White!
        
        float shakeAmt = isPlayer ? 10f : -10f; // Player punch pushes right, opponent pushes left
        container.style.translate = new Translate(shakeAmt, Random.Range(-5f, 5f), 0);
        
        yield return new WaitForSeconds(0.05f);
        
        container.style.translate = new Translate(0, 0, 0);
        if (bar != null) bar.style.backgroundColor = new StyleColor(originalColor);
    }'''
content = re.sub(target1, replacement1, content)

target2 = r'''            if \(nextAiTapTimer <= 0f\)
            \{
                opponentProgress \+= 1f;
                
                if \(Random\.value < 0\.15f\)
                    nextAiTapTimer = Random\.Range\(0\.3f, 0\.6f\);
                else
                    nextAiTapTimer = Random\.Range\(0\.05f, 0\.2f\);
            \}'''

replacement2 = r'''            if (nextAiTapTimer <= 0f)
            {
                opponentProgress += 1f;
                
                // Opponent hits back!
                manager.StartCoroutine(PunchEffectRoutine(opponentProgressBar, new Color(0.9f, 0.2f, 0.2f), false));
                
                if (Random.value < 0.15f)
                    nextAiTapTimer = Random.Range(0.3f, 0.6f);
                else
                    nextAiTapTimer = Random.Range(0.05f, 0.2f);
            }'''
content = re.sub(target2, replacement2, content)

with open(path, 'w') as f:
    f.write(content)

import re

path = 'Assets/Scripts/2 Managers/MiniGames/DialogueChainMiniGame.cs'
with open(path, 'r') as f:
    content = f.read()

target1 = r'''    private IEnumerator HandleFeedback\(Button btn, bool isCorrect\)
    \{
        isTransitioning = true; // Freeze timer and input
        
        if \(isCorrect\)
        \{
            targetRapportValue \+= 0\.25f;
            
            StyleColor greenColor = new StyleColor\(new Color\(0\.13f, 0\.77f, 0\.36f\)\);
            btn\.style\.backgroundColor = greenColor;
            btn\.style\.borderTopColor = greenColor;
            btn\.style\.borderBottomColor = greenColor;
            btn\.style\.borderLeftColor = greenColor;
            btn\.style\.borderRightColor = greenColor;
            
            manager\.StartCoroutine\(AnimateFloatingText\(btn, "\+ Rapport", new Color\(0\.13f, 0\.77f, 0\.36f\)\)\);
        \}
        else
        \{
            targetRapportValue -= 0\.15f;
            
            StyleColor redColor = new StyleColor\(new Color\(0\.93f, 0\.26f, 0\.22f\)\);
            btn\.style\.backgroundColor = redColor;
            btn\.style\.borderTopColor = redColor;
            btn\.style\.borderBottomColor = redColor;
            btn\.style\.borderLeftColor = redColor;
            btn\.style\.borderRightColor = redColor;
            
            manager\.StartCoroutine\(AnimateFloatingText\(btn, "- Rapport", new Color\(0\.93f, 0\.26f, 0\.22f\)\)\);
            
            // Screen Shake on failure
            float shakeAmt = 8f;
            container\.style\.left = shakeAmt;
            yield return new WaitForSeconds\(0\.05f\);
            container\.style\.left = -shakeAmt;
            yield return new WaitForSeconds\(0\.05f\);
            container\.style\.left = 0f;
        \}
        
        targetRapportValue = Mathf\.Clamp01\(targetRapportValue\);
        
        yield return new WaitForSeconds\(0\.3f\); // Cinematic freeze frame before next round
        
        dialogueRound\+\+;
        ResetButtonStyles\(\);
        isTransitioning = false;
        LoadNextRound\(\);
    \}'''

replacement1 = r'''    private IEnumerator HandleFeedback(Button btn, bool isCorrect)
    {
        isTransitioning = true; // Freeze timer and input
        
        if (isCorrect)
        {
            targetRapportValue += 0.25f;
            
            if (btn != null)
            {
                StyleColor greenColor = new StyleColor(new Color(0.13f, 0.77f, 0.36f));
                btn.style.backgroundColor = greenColor;
                btn.style.borderTopColor = greenColor;
                btn.style.borderBottomColor = greenColor;
                btn.style.borderLeftColor = greenColor;
                btn.style.borderRightColor = greenColor;
                
                manager.StartCoroutine(AnimateFloatingText(btn, "+ Rapport", new Color(0.13f, 0.77f, 0.36f)));
            }
        }
        else
        {
            targetRapportValue -= 0.15f;
            
            if (btn != null)
            {
                StyleColor redColor = new StyleColor(new Color(0.93f, 0.26f, 0.22f));
                btn.style.backgroundColor = redColor;
                btn.style.borderTopColor = redColor;
                btn.style.borderBottomColor = redColor;
                btn.style.borderLeftColor = redColor;
                btn.style.borderRightColor = redColor;
                
                manager.StartCoroutine(AnimateFloatingText(btn, "- Rapport", new Color(0.93f, 0.26f, 0.22f)));
            }
            else
            {
                // Timeout / Ignored
                manager.StartCoroutine(AnimateIgnoredEffect());
            }
            
            // Screen Shake on failure
            float shakeAmt = 8f;
            container.style.left = shakeAmt;
            yield return new WaitForSeconds(0.05f);
            container.style.left = -shakeAmt;
            yield return new WaitForSeconds(0.05f);
            container.style.left = 0f;
        }
        
        targetRapportValue = Mathf.Clamp01(targetRapportValue);
        
        // Wait longer if it was an IGNORED popup so they can see it
        yield return new WaitForSeconds(btn == null ? 0.8f : 0.3f);
        
        dialogueRound++;
        ResetButtonStyles();
        isTransitioning = false;
        LoadNextRound();
    }
    
    private IEnumerator AnimateIgnoredEffect()
    {
        Label ignoredText = new Label("IGNORED");
        ignoredText.style.position = Position.Absolute;
        ignoredText.style.color = new StyleColor(new Color(0.93f, 0.26f, 0.22f)); // Red
        ignoredText.style.fontSize = 72;
        ignoredText.style.unityFontStyleAndWeight = FontStyle.Bold;
        ignoredText.style.unityTextAlign = TextAnchor.MiddleCenter;
        
        // Darken background
        VisualElement overlay = new VisualElement();
        overlay.style.position = Position.Absolute;
        overlay.style.width = new Length(100, LengthUnit.Percent);
        overlay.style.height = new Length(100, LengthUnit.Percent);
        overlay.style.backgroundColor = new StyleColor(new Color(0f, 0f, 0f, 0.5f));
        
        // Center text in container
        ignoredText.style.top = new Length(50, LengthUnit.Percent);
        ignoredText.style.left = new Length(50, LengthUnit.Percent);
        ignoredText.style.translate = new Translate(new Length(-50, LengthUnit.Percent), new Length(-50, LengthUnit.Percent));
        
        overlay.Add(ignoredText);
        container.Add(overlay);
        
        // Pop in animation
        float life = 0.8f;
        float elapsed = 0f;
        while(elapsed < life && isPlaying)
        {
            elapsed += Time.deltaTime;
            float p = elapsed / life;
            
            // Starts big, slams down to size 1
            float scale = Mathf.Lerp(2.5f, 1f, p * 10f); // Fast snap
            if (scale < 1f) scale = 1f;
            ignoredText.style.scale = new StyleScale(new Vector2(scale, scale));
            
            // Fade out at the very end
            if (p > 0.8f) {
                overlay.style.opacity = 1f - ((p - 0.8f) * 5f);
            }
            
            yield return null;
        }
        
        if (container.Contains(overlay)) container.Remove(overlay);
    }'''
content = re.sub(target1, replacement1, content)

target2 = r'''                        // Time out failure
                        manager\.StartCoroutine\(HandleFeedback\(btnResponse1, false\)\); // Pass random button for visual fail'''

replacement2 = r'''                        // Time out failure
                        manager.StartCoroutine(HandleFeedback(null, false)); // Pass null to trigger IGNORED popup'''
content = re.sub(target2, replacement2, content)

with open(path, 'w') as f:
    f.write(content)

import re

path = 'Assets/Scripts/2 Managers/MiniGames/TimingRingMiniGame.cs'

with open(path, 'r') as f:
    content = f.read()

# 1. Modify OnTimingStrike
target_strike = r'''    private void OnTimingStrike\(\)
    \{
        if \(!isPlaying \|\| !manager\.isPlaying \|\| !timingMarkerMoving\) return;
        timingMarkerMoving = false;
        
        float currentZoneDegrees = Mathf\.Max\(10f, 45f - \(timingCurrentRound \* 5f\)\); 
        float diff = Mathf\.Abs\(Mathf\.DeltaAngle\(timingMarkerAngle, timingTargetAngle\)\);
        
        if \(diff <= currentZoneDegrees / 2f\)
        \{
            timingCurrentRound\+\+;
            manager\.StartCoroutine\(HitAuraRoutine\(\)\);
            if \(timingCurrentRound >= config\.timingRounds\)
            \{
                isPlaying = false;
                manager\.MarkWinReady\(\);
            \}
            else
            \{
                timingSpinDir \*= -1;
                timingTargetAngle = Random\.Range\(0f, 360f\);
            \}
        \}
        else
        \{
            if \(manager\.currentGame != null && manager\.currentGame\.canFail\) 
            \{ 
                isPlaying = false;
                manager\.AbortGame\(\);
            \}
        \}
    \}'''
    
replacement_strike = r'''    private void OnTimingStrike()
    {
        if (!isPlaying || !manager.isPlaying || !timingMarkerMoving) return;
        timingMarkerMoving = false;
        
        float currentZoneDegrees = Mathf.Max(10f, 45f - (timingCurrentRound * 5f)); 
        float diff = Mathf.Abs(Mathf.DeltaAngle(timingMarkerAngle, timingTargetAngle));
        
        if (diff <= currentZoneDegrees / 2f)
        {
            timingCurrentRound++;
            
            if (timingCurrentRound >= config.timingRounds)
            {
                isPlaying = false;
                manager.StartCoroutine(HitAuraRoutine(new Color(0.18f, 0.8f, 0.44f), true, true));
            }
            else
            {
                timingSpinDir *= -1;
                timingTargetAngle = Random.Range(0f, 360f);
                manager.StartCoroutine(HitAuraRoutine(new Color(0.18f, 0.8f, 0.44f), false, false));
            }
        }
        else
        {
            isPlaying = false;
            manager.StartCoroutine(HitAuraRoutine(new Color(0.9f, 0.16f, 0.16f), true, false));
        }
    }'''

content = re.sub(target_strike, replacement_strike, content)

# 2. Modify HitAuraRoutine
target_aura = r'''    private IEnumerator HitAuraRoutine\(\)
    \{
        if \(timingGlow != null && glowLayers != null\)
        \{
            timingGlow\.style\.opacity = 1f;
            float\[\] maxBorderWidths = \{ 2f, 4f, 7f, 10f, 14f, 20f \}; 
            
            float t = 0;
            while\(t < 0\.1f\)
            \{
                t \+= Time\.deltaTime;
                float progress = t / 0\.1f;
                
                for\(int i = 0; i < 6; i\+\+\)
                \{
                    if \(glowLayers\[i\] == null\) continue;
                    float currentWidth = Mathf\.Lerp\(0, maxBorderWidths\[i\], progress\);
                    glowLayers\[i\]\.style\.borderTopWidth = currentWidth;
                    glowLayers\[i\]\.style\.borderBottomWidth = currentWidth;
                    glowLayers\[i\]\.style\.borderLeftWidth = currentWidth;
                    glowLayers\[i\]\.style\.borderRightWidth = currentWidth;
                \}
                yield return null;
            \}
            
            for\(int i = 0; i < 6; i\+\+\)
            \{
                if \(glowLayers\[i\] == null\) continue;
                glowLayers\[i\]\.style\.borderTopWidth = maxBorderWidths\[i\];
                glowLayers\[i\]\.style\.borderBottomWidth = maxBorderWidths\[i\];
                glowLayers\[i\]\.style\.borderLeftWidth = maxBorderWidths\[i\];
                glowLayers\[i\]\.style\.borderRightWidth = maxBorderWidths\[i\];
            \}
            
            t = 0;
            while\(t < 0\.4f\)
            \{
                t \+= Time\.deltaTime;
                float opacity = Mathf\.Lerp\(1f, 0f, t / 0\.4f\);
                timingGlow\.style\.opacity = opacity;
                yield return null;
            \}
            
            timingGlow\.style\.opacity = 0f;
        \}
    \}'''
    
replacement_aura = r'''    private IEnumerator HitAuraRoutine(Color glowColor, bool isEndGame, bool isWin)
    {
        if (timingGlow != null && glowLayers != null)
        {
            timingGlow.style.opacity = 1f;
            // Greatly expanded glow widths!
            float[] maxBorderWidths = { 6f, 12f, 20f, 32f, 48f, 70f }; 
            
            // Apply the new color with decaying opacity
            for(int i = 0; i < 6; i++)
            {
                if (glowLayers[i] == null) continue;
                float op = 1f - (i * 0.16f); // Decaying opacity outwards
                Color layerColor = new Color(glowColor.r, glowColor.g, glowColor.b, op);
                glowLayers[i].style.borderTopColor = new StyleColor(layerColor);
                glowLayers[i].style.borderBottomColor = new StyleColor(layerColor);
                glowLayers[i].style.borderLeftColor = new StyleColor(layerColor);
                glowLayers[i].style.borderRightColor = new StyleColor(layerColor);
            }
            
            // Arrow rapid spin effect on success, violent shake on fail
            float startAngle = timingMarkerAngle;
            
            float t = 0;
            float duration = 0.15f;
            while(t < duration)
            {
                t += Time.deltaTime;
                float progress = t / duration;
                
                // Explode the glow outwards
                for(int i = 0; i < 6; i++)
                {
                    if (glowLayers[i] == null) continue;
                    float currentWidth = Mathf.Lerp(0, maxBorderWidths[i], Mathf.Pow(progress, 0.5f)); // Fast pop!
                    glowLayers[i].style.borderTopWidth = currentWidth;
                    glowLayers[i].style.borderBottomWidth = currentWidth;
                    glowLayers[i].style.borderLeftWidth = currentWidth;
                    glowLayers[i].style.borderRightWidth = currentWidth;
                }
                
                // Visual Juice
                if (!isEndGame || isWin) 
                {
                    // Success: Spin 360 rapidly!
                    if (timingMarker != null) timingMarker.style.rotate = new Rotate(Angle.Degrees(startAngle + (360f * progress * timingSpinDir)));
                }
                else
                {
                    // Fail: Shake!
                    if (timingCircle != null) timingCircle.style.translate = new Translate(new Length(Random.Range(-10,10), LengthUnit.Pixel), new Length(Random.Range(-10,10), LengthUnit.Pixel));
                }
                
                yield return null;
            }
            
            if (timingMarker != null && (!isEndGame || isWin)) timingMarker.style.rotate = new Rotate(Angle.Degrees(startAngle));
            if (timingCircle != null) timingCircle.style.translate = new Translate(0,0);
            
            t = 0;
            float fadeOut = isEndGame ? 0.8f : 0.4f; // Slower fade if it's the end of the game
            while(t < fadeOut)
            {
                t += Time.deltaTime;
                float opacity = Mathf.Lerp(1f, 0f, t / fadeOut);
                timingGlow.style.opacity = opacity;
                yield return null;
            }
            
            timingGlow.style.opacity = 0f;
            
            if (isEndGame)
            {
                if (isWin) manager.MarkWinReady();
                else if (manager.currentGame.canFail) manager.AbortGame();
            }
        }
        else
        {
            // Fallback if UI is missing
            if (isEndGame)
            {
                if (isWin) manager.MarkWinReady();
                else if (manager.currentGame.canFail) manager.AbortGame();
            }
        }
    }'''

content = re.sub(target_aura, replacement_aura, content)

with open(path, 'w') as f:
    f.write(content)


import re

path = 'Assets/Scripts/2 Managers/MiniGames/TimingRingMiniGame.cs'

with open(path, 'r') as f:
    content = f.read()

# 1. Modify Initialize to add visual overhaul for Marker and Circle
target_init = r'''        if \(btnTimingStrike != null\) btnTimingStrike\.clicked \+= OnTimingStrike;
    }'''

replacement_init = r'''        if (btnTimingStrike != null) btnTimingStrike.clicked += OnTimingStrike;
        
        // Visual Overhaul: The Traffic Circle
        timingCircle.style.borderTopColor = new StyleColor(new Color(0.15f, 0.15f, 0.18f));
        timingCircle.style.borderBottomColor = new StyleColor(new Color(0.15f, 0.15f, 0.18f));
        timingCircle.style.borderLeftColor = new StyleColor(new Color(0.15f, 0.15f, 0.18f));
        timingCircle.style.borderRightColor = new StyleColor(new Color(0.15f, 0.15f, 0.18f));
        timingCircle.style.borderTopWidth = 24; timingCircle.style.borderBottomWidth = 24;
        timingCircle.style.borderLeftWidth = 24; timingCircle.style.borderRightWidth = 24;
        timingCircle.style.backgroundColor = new StyleColor(new Color(0.08f, 0.08f, 0.1f));

        // Visual Overhaul: The Sign Spinner's Sign
        timingMarker.Clear();
        var signBody = new VisualElement();
        signBody.style.width = 300;
        signBody.style.height = 110;
        signBody.style.backgroundColor = new StyleColor(new Color(1f, 0.85f, 0.1f)); // Bright Yellow
        signBody.style.borderTopWidth = 6; signBody.style.borderBottomWidth = 6;
        signBody.style.borderLeftWidth = 6; signBody.style.borderRightWidth = 6;
        signBody.style.borderTopColor = new StyleColor(new Color(0.1f, 0.1f, 0.1f));
        signBody.style.borderBottomColor = new StyleColor(new Color(0.1f, 0.1f, 0.1f));
        signBody.style.borderLeftColor = new StyleColor(new Color(0.1f, 0.1f, 0.1f));
        signBody.style.borderRightColor = new StyleColor(new Color(0.1f, 0.1f, 0.1f));
        signBody.style.borderTopLeftRadius = 20; signBody.style.borderTopRightRadius = 20;
        signBody.style.borderBottomLeftRadius = 20; signBody.style.borderBottomRightRadius = 20;
        signBody.style.justifyContent = Justify.Center;
        
        var signText = new Label("SALE ➡️");
        signText.style.fontSize = 65;
        signText.style.color = new StyleColor(new Color(0.9f, 0.15f, 0.15f)); // Bold Red
        signText.style.unityFontStyleAndWeight = FontStyle.Bold;
        signText.style.unityTextAlign = TextAnchor.MiddleCenter;
        signBody.Add(signText);
        
        timingMarker.Add(signBody);
    }'''

content = re.sub(target_init, replacement_init, content)

# 2. Modify OnTimingStrike to NOT stop marker moving on miss!
target_strike = r'''    private void OnTimingStrike\(\)
    \{
        if \(!isPlaying \|\| !manager\.isPlaying \|\| !timingMarkerMoving\) return;
        timingMarkerMoving = false;
        
        float currentZoneDegrees = Mathf\.Max\(10f, 45f - \(timingCurrentRound \* 5f\)\); 
        float diff = Mathf\.Abs\(Mathf\.DeltaAngle\(timingMarkerAngle, timingTargetAngle\)\);
        
        if \(diff <= currentZoneDegrees / 2f\)
        \{
            timingCurrentRound\+\+;
            
            if \(timingCurrentRound >= config\.timingRounds\)
            \{
                isPlaying = false;
                manager\.StartCoroutine\(HitAuraRoutine\(new Color\(0\.18f, 0\.8f, 0\.44f\), true, true\)\);
            \}
            else
            \{
                timingSpinDir \*= -1;
                timingTargetAngle = Random\.Range\(0f, 360f\);
                manager\.StartCoroutine\(HitAuraRoutine\(new Color\(0\.18f, 0\.8f, 0\.44f\), false, false\)\);
            \}
        \}
        else
        \{
            isPlaying = false;
            manager\.StartCoroutine\(HitAuraRoutine\(new Color\(0\.9f, 0\.16f, 0\.16f\), true, false\)\);
        \}
    \}'''

replacement_strike = r'''    private void OnTimingStrike()
    {
        if (!isPlaying || !manager.isPlaying || !timingMarkerMoving) return;
        
        float currentZoneDegrees = Mathf.Max(10f, 45f - (timingCurrentRound * 5f)); 
        float diff = Mathf.Abs(Mathf.DeltaAngle(timingMarkerAngle, timingTargetAngle));
        
        if (diff <= currentZoneDegrees / 2f)
        {
            timingMarkerMoving = false; // Stop briefly on hit for juice
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
            // MISS! We DO NOT stop the marker. It keeps spinning!
            manager.StartCoroutine(HitAuraRoutine(new Color(0.9f, 0.16f, 0.16f), false, false)); 
        }
    }'''

content = re.sub(target_strike, replacement_strike, content)


# 3. Modify HitAuraRoutine to resume spinning if not endgame!
target_aura = r'''            if \(timingMarker != null && \(!isEndGame \|\| isWin\)\) timingMarker\.style\.rotate = new Rotate\(Angle\.Degrees\(startAngle\)\);
            if \(timingCircle != null\) timingCircle\.style\.translate = new Translate\(0,0\);
            
            t = 0;
            float fadeOut = isEndGame \? 0\.8f : 0\.4f; // Slower fade if it's the end of the game
            while\(t < fadeOut\)
            \{
                t \+= Time\.deltaTime;
                float opacity = Mathf\.Lerp\(1f, 0f, t / fadeOut\);
                timingGlow\.style\.opacity = opacity;
                yield return null;
            \}
            
            timingGlow\.style\.opacity = 0f;
            
            if \(isEndGame\)
            \{
                if \(isWin\) manager\.MarkWinReady\(\);
                else if \(manager\.currentGame\.canFail\) manager\.AbortGame\(\);
            \}
        \}'''

replacement_aura = r'''            if (timingMarker != null && (!isEndGame || isWin)) timingMarker.style.rotate = new Rotate(Angle.Degrees(startAngle));
            if (timingCircle != null) timingCircle.style.translate = new Translate(0,0);
            
            if (!isEndGame && isWin) 
            {
                // Resume spinning ONLY on success (on fail it never stopped!)
                timingMarkerMoving = true;
            }
            
            t = 0;
            float fadeOut = isEndGame ? 0.8f : 0.4f; 
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
        }'''

content = re.sub(target_aura, replacement_aura, content)

with open(path, 'w') as f:
    f.write(content)


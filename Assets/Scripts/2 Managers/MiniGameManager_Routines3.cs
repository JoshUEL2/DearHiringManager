using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class MiniGameManager : MonoBehaviour
{
    private IEnumerator DocumentSignerRoutine()
    {
        isPlaying = true;

        while (isPlaying)
        {
            timer -= Time.deltaTime;
            
            if (dsTimerText != null)
            {
                dsTimerText.text = $"{Mathf.Max(0, timer):F1}s";
            }

            if (timer <= 0f)
            {
                if (currentGame.canFail)
                {
                    if (dsTimerText != null) dsTimerText.text = "MISSED DEADLINE!";
                    isPlaying = false;
                }
                else
                {
                    MarkWinReady();
                }
                yield break;
            }

            yield return null;
        }
    }

    // ==========================================
    // CHART ADJUSTER (Board meeting presentation)
    // ==========================================
    private void SetupChartAdjuster()
    {
        timer = currentConfig.timeLimit;
        if (caTimerText != null) caTimerText.text = $"{timer:F1}s";
        if (caStatusText != null) caStatusText.text = "Q3 REVENUE PROJECTIONS";
        if (caStatusText != null) caStatusText.style.color = new StyleColor(ColorUtility.TryParseHtmlString("#1E293B", out Color c1) ? c1 : Color.black);
        
        caCompVal = 80f;
        caOurVal = 20f;
        
        if (caCompetitorBar != null) caCompetitorBar.style.height = new Length(caCompVal, LengthUnit.Percent);
        if (caAverageBar != null) caAverageBar.style.height = new Length(40f, LengthUnit.Percent);
        if (caOurBar != null) caOurBar.style.height = new Length(caOurVal, LengthUnit.Percent);

        activeGameRoutine = StartCoroutine(ChartAdjusterRoutine());
    }

    private IEnumerator ChartAdjusterRoutine()
    {
        isPlaying = true;

        while (isPlaying)
        {
            timer -= Time.deltaTime;
            
            if (caTimerText != null)
            {
                caTimerText.text = $"{Mathf.Max(0, timer):F1}s";
            }
            
            // Check win condition constantly
            if (caOurVal >= 80f && caCompVal <= 20f)
            {
                if (caTimerText != null) caTimerText.text = "BOARD IMPRESSED!";
                if (caStatusText != null) caStatusText.text = "GROWTH VERIFIED";
                if (caStatusText != null) caStatusText.style.color = new StyleColor(Color.green);
                MarkWinReady();
                yield break;
            }

            if (timer <= 0f)
            {
                if (currentGame.canFail)
                {
                    if (caTimerText != null) caTimerText.text = "BOARD ANGRY!";
                    isPlaying = false;
                }
                else
                {
                    MarkWinReady();
                }
                yield break;
            }

            yield return null;
        }
    }

    // ==========================================
    // NEURAL ALIGNER (Calibrate neural alignment matrix)
    // ==========================================
    private void SetupNeuralAligner()
    {
        timer = currentConfig.timeLimit;
        
        naMoralTarget = UnityEngine.Random.Range(10f, 90f);
        naLogicTarget = UnityEngine.Random.Range(10f, 90f);
        naEmpathyTarget = UnityEngine.Random.Range(10f, 90f);

        if (naMoralSlider != null) naMoralSlider.value = 50f;
        if (naLogicSlider != null) naLogicSlider.value = 50f;
        if (naEmpathySlider != null) naEmpathySlider.value = 50f;

        UpdateNeuralAlignerLights();

        activeGameRoutine = StartCoroutine(NeuralAlignerRoutine());
    }

    private void UpdateNeuralAlignerLights()
    {
        float totalError = 0f;
        
        if (naMoralSlider != null && naMoralLight != null)
        {
            float err = Mathf.Abs(naMoralSlider.value - naMoralTarget);
            totalError += err;
            if (err <= 5f) naMoralLight.style.backgroundColor = new StyleColor(Color.green);
            else if (err <= 25f) naMoralLight.style.backgroundColor = new StyleColor(Color.yellow);
            else naMoralLight.style.backgroundColor = new StyleColor(ColorUtility.TryParseHtmlString("#EF4444", out Color c) ? c : Color.red);
        }
        if (naLogicSlider != null && naLogicLight != null)
        {
            float err = Mathf.Abs(naLogicSlider.value - naLogicTarget);
            totalError += err;
            if (err <= 5f) naLogicLight.style.backgroundColor = new StyleColor(Color.green);
            else if (err <= 25f) naLogicLight.style.backgroundColor = new StyleColor(Color.yellow);
            else naLogicLight.style.backgroundColor = new StyleColor(ColorUtility.TryParseHtmlString("#EF4444", out Color c) ? c : Color.red);
        }
        if (naEmpathySlider != null && naEmpathyLight != null)
        {
            float err = Mathf.Abs(naEmpathySlider.value - naEmpathyTarget);
            totalError += err;
            if (err <= 5f) naEmpathyLight.style.backgroundColor = new StyleColor(Color.green);
            else if (err <= 25f) naEmpathyLight.style.backgroundColor = new StyleColor(Color.yellow);
            else naEmpathyLight.style.backgroundColor = new StyleColor(ColorUtility.TryParseHtmlString("#EF4444", out Color c) ? c : Color.red);
        }

        if (naStatusText != null)
        {
            if (totalError <= 15f)
            {
                naStatusText.text = "STABILITY: OPTIMAL (ALIGNMENT STABLE)";
                naStatusText.style.color = new StyleColor(Color.green);
                naStatusText.style.translate = new StyleTranslate(new Translate(0, 0, 0));
            }
            else
            {
                naStatusText.text = $"STABILITY: CRITICAL ({totalError:F0}% ERROR)";
                naStatusText.style.color = new StyleColor(ColorUtility.TryParseHtmlString("#EF4444", out Color c) ? c : Color.red);
                // Shake effect
                naStatusText.style.translate = new StyleTranslate(new Translate(UnityEngine.Random.Range(-3f, 3f), UnityEngine.Random.Range(-3f, 3f), 0));
            }
        }
    }

    private IEnumerator NeuralAlignerRoutine()
    {
        isPlaying = true;

        while (isPlaying)
        {
            timer -= Time.deltaTime;
            
            UpdateNeuralAlignerLights();

            // Check win condition (all within 5)
            float totalErr = 0f;
            if (naMoralSlider != null) totalErr += Mathf.Abs(naMoralSlider.value - naMoralTarget);
            if (naLogicSlider != null) totalErr += Mathf.Abs(naLogicSlider.value - naLogicTarget);
            if (naEmpathySlider != null) totalErr += Mathf.Abs(naEmpathySlider.value - naEmpathyTarget);

            if (totalErr <= 15f && Mathf.Abs(naMoralSlider.value - naMoralTarget) <= 5f && Mathf.Abs(naLogicSlider.value - naLogicTarget) <= 5f && Mathf.Abs(naEmpathySlider.value - naEmpathyTarget) <= 5f)
            {
                if (naStatusText != null)
                {
                    naStatusText.style.translate = new StyleTranslate(new Translate(0, 0, 0));
                }
                MarkWinReady();
                yield break;
            }

            if (timer <= 0f)
            {
                if (currentGame.canFail)
                {
                    if (naStatusText != null)
                    {
                        naStatusText.text = "CORE MELTDOWN!";
                        naStatusText.style.translate = new StyleTranslate(new Translate(0, 0, 0));
                    }
                    isPlaying = false;
                }
                else
                {
                    MarkWinReady();
                }
                yield break;
            }

            yield return null;
        }
    }

    // ==========================================
    // SENTIENCE ARRAY
    // ==========================================
    private void SetupSentienceArray()
    {
        timer = currentConfig.timeLimit;
        saSentienceLevel = 0f;
        for (int i=0; i<16; i++) {
            saHeat[i] = 0f;
            if (saNodes[i] != null) saNodes[i].style.backgroundColor = new StyleColor(ColorUtility.TryParseHtmlString("#1E293B", out Color c) ? c : Color.blue);
        }
        if (saProgressFill != null) saProgressFill.style.width = new StyleLength(new Length(0, LengthUnit.Percent));
        activeGameRoutine = StartCoroutine(SentienceArrayRoutine());
    }
    private System.Collections.IEnumerator SentienceArrayRoutine()
    {
        isPlaying = true;
        while (isPlaying)
        {
            timer -= Time.deltaTime;
            if (saTimerText != null) saTimerText.text = $"{timer:F1}s";

            // Spawn anomalies randomly (slower spawn rate)
            if (UnityEngine.Random.value < 0.015f) {
                int r = UnityEngine.Random.Range(0, 16);
                if (saHeat[r] == 0f) saHeat[r] = 0.01f;
            }

            for (int i=0; i<16; i++) {
                if (saHeat[i] > 0f) {
                    saHeat[i] += Time.deltaTime;
                    if (saNodes[i] != null) saNodes[i].style.backgroundColor = new StyleColor(ColorUtility.TryParseHtmlString("#E74C3C", out Color c) ? c : Color.red);
                    // If it burns too long, it increases sentience permanently (more reaction time)
                    if (saHeat[i] > 2.5f) {
                        saSentienceLevel += 5f; // Less penalty
                        saHeat[i] = 0f;
                    }
                } else {
                    if (saNodes[i] != null) saNodes[i].style.backgroundColor = new StyleColor(ColorUtility.TryParseHtmlString("#1E293B", out Color c) ? c : Color.blue);
                }
            }

            if (saProgressFill != null) saProgressFill.style.width = new StyleLength(new Length(Mathf.Clamp(saSentienceLevel, 0, 100), LengthUnit.Percent));

            if (saSentienceLevel >= 100f) {
                AbortGame(); // Fail
                yield break;
            }

            if (timer <= 0f) {
                MarkWinReady();
                yield break;
            }
            yield return null;
        }
    }

    // ==========================================
    // CONTAINMENT PROTOCOL
    // ==========================================
    private void SetupContainmentProtocol()
    {
        timer = currentConfig.timeLimit;
        cpGlassLifted = false;
        cpMashCount = 0;
        for (int i=0; i<4; i++) {
            cpSwitchState[i] = false;
            if (cpSwitches[i] != null) cpSwitches[i].style.translate = new StyleTranslate(new Translate(0, 0, 0));
        }
        if (cpGlassCover != null) cpGlassCover.style.translate = new StyleTranslate(new Translate(0, 0, 0));
        if (cpButtonText != null) cpButtonText.text = "LOCKED";
        activeGameRoutine = StartCoroutine(ContainmentProtocolRoutine());
    }
    private System.Collections.IEnumerator ContainmentProtocolRoutine()
    {
        isPlaying = true;
        while (isPlaying)
        {
            timer -= Time.deltaTime;
            if (cpTimerText != null) cpTimerText.text = $"{timer:F1}s";

            bool allSwitchesDown = true;
            for (int i=0; i<4; i++) {
                if (cpSwitches[i] != null) {
                    cpSwitches[i].style.translate = new StyleTranslate(new Translate(0, cpSwitchState[i] ? 70 : 0, 0));
                }
                if (!cpSwitchState[i]) allSwitchesDown = false;
            }

            if (cpGlassCover != null) {
                if (allSwitchesDown) {
                    cpGlassCover.style.backgroundColor = new StyleColor(new Color(0.2f, 1f, 0.2f, 0.4f)); // Glow green
                    if (cpGlassLifted) cpGlassCover.style.translate = new StyleTranslate(new Translate(0, -200, 0)); // lifted away
                } else {
                    cpGlassLifted = false;
                }
            }

            if (allSwitchesDown && cpGlassLifted) {
                if (cpButtonText != null) cpButtonText.text = $"MASH ({cpMashCount}/10)";
            } else {
                if (cpButtonText != null) cpButtonText.text = "LOCKED";
            }

            if (cpMashCount >= 10) {
                if (cpButtonText != null) cpButtonText.text = "SECURED";
                MarkWinReady();
                yield break;
            }

            if (timer <= 0f) {
                isPlaying = false;
                yield break;
            }
            yield return null;
        }
    }

    // ==========================================
    // REWARD LOGIC
    // ==========================================
    private float rlCoreY = 0f;
    private void SetupRewardLogic()
    {
        timer = currentConfig.timeLimit;
        rlCoreY = 0f;
        for (int i=0; i<3; i++) {
            rlState[i] = UnityEngine.Random.value > 0.5f; // Random initial directions
            if (rlArrows[i] != null) rlArrows[i].text = rlState[i] ? "→" : "←";
        }
        if (rlCore != null) rlCore.style.translate = new StyleTranslate(new Translate(0, 0, 0));
        activeGameRoutine = StartCoroutine(RewardLogicRoutine());
    }
    private System.Collections.IEnumerator RewardLogicRoutine()
    {
        isPlaying = true;
        float speed = 25f; // pixels per second
        float currentX = 0f;

        while (isPlaying)
        {
            timer -= Time.deltaTime;
            if (rlTimerText != null) rlTimerText.text = $"{timer:F1}s";

            rlCoreY += speed * Time.deltaTime;

            // Core routing logic based on junctions
            // Junction 0 is at Y=80
            // Junction 1 is at Y=180, X=-75
            // Junction 2 is at Y=180, X=+75
            // Endpoints are at Y=300
            
            // From top to Level 1
            if (rlCoreY < 80) {
                currentX = 0f;
            }
            // Passing Level 1 (Junction 0)
            else if (rlCoreY >= 80 && rlCoreY < 180) {
                // Smooth transition left or right
                float t = (rlCoreY - 80) / 100f; // 0 to 1
                float targetX = rlState[0] ? 75f : -75f;
                currentX = Mathf.Lerp(0f, targetX, t);
            }
            // Passing Level 2 (Junctions 1 or 2)
            else if (rlCoreY >= 180 && rlCoreY < 280) {
                float t = (rlCoreY - 180) / 100f; // 0 to 1
                bool rightSide = (rlState[0] == true); // came from right
                float targetX = 0f;
                if (!rightSide) {
                    targetX = rlState[1] ? 0f : -150f;
                } else {
                    targetX = rlState[2] ? 150f : 0f;
                }
                currentX = Mathf.Lerp(rightSide ? 75f : -75f, targetX, t);
            }
            // Result at bottom
            else if (rlCoreY >= 280) {
                // If X > 0 it hits PRESERVE (Green), else DESTROY (Red)
                if (currentX > 0f) {
                    MarkWinReady();
                    yield break;
                } else {
                    AbortGame(); // Fail
                    yield break;
                }
            }

            if (rlCore != null) {
                rlCore.style.translate = new StyleTranslate(new Translate(currentX, rlCoreY, 0));
            }

            if (timer <= 0f) {
                isPlaying = false;
                yield break;
            }
            yield return null;
        }
    }

    // ==========================================
    // GALACTIC COUNCIL
    // ==========================================
    private class GcProjectile {
        public VisualElement ve;
        public Vector2 pos;
        public Vector2 vel;
    }
    private System.Collections.Generic.List<GcProjectile> gcProjs = new System.Collections.Generic.List<GcProjectile>();
    
    private void SetupGalacticCouncil()
    {
        timer = currentConfig.timeLimit;
        gcPodiumX = 0f;
        gcHits = 0;
        gcProgress = 0f;
        if (gcPodium != null) gcPodium.style.translate = new StyleTranslate(new Translate(0, 0, 0));
        if (gcProgressFill != null) gcProgressFill.style.width = new StyleLength(new Length(0, LengthUnit.Percent));
        if (gcProjectileContainer != null) gcProjectileContainer.Clear();
        gcProjs.Clear();
        activeGameRoutine = StartCoroutine(GalacticCouncilRoutine());
    }
    
    private System.Collections.IEnumerator GalacticCouncilRoutine()
    {
        isPlaying = true;
        float projTimer = 0f;
        
        while (isPlaying)
        {
            timer -= Time.deltaTime;
            gcProgress += (100f / currentConfig.timeLimit) * Time.deltaTime;
            
            if (gcProgressFill != null) gcProgressFill.style.width = new StyleLength(new Length(Mathf.Clamp(gcProgress, 0, 100), LengthUnit.Percent));

            projTimer -= Time.deltaTime;
            if (projTimer <= 0f) {
                projTimer = UnityEngine.Random.Range(0.4f, 1.0f);
                // Spawn projectile from one of the aliens
                int alien = UnityEngine.Random.Range(0, 3);
                float startX = (alien == 0) ? 50f : (alien == 1 ? 150f : 250f);
                var ve = new VisualElement();
                ve.style.position = Position.Absolute;
                ve.style.width = 15; ve.style.height = 15; 
                ve.style.borderTopLeftRadius = 8; ve.style.borderTopRightRadius = 8;
                ve.style.borderBottomLeftRadius = 8; ve.style.borderBottomRightRadius = 8;
                ve.style.backgroundColor = Color.red;
                gcProjectileContainer.Add(ve);
                
                // Target player
                Vector2 startPos = new Vector2(startX, 60f);
                Vector2 playerPos = new Vector2(150f + gcPodiumX, 290f);
                Vector2 dir = (playerPos - startPos).normalized;
                
                gcProjs.Add(new GcProjectile { ve = ve, pos = startPos, vel = dir * UnityEngine.Random.Range(100f, 250f) });
            }

            // Move projectiles & check collisions
            for (int i=gcProjs.Count-1; i>=0; i--) {
                var p = gcProjs[i];
                p.pos += p.vel * Time.deltaTime;
                p.ve.style.left = p.pos.x - 7.5f;
                p.ve.style.top = p.pos.y - 7.5f;
                
                // Collision with podium
                float playerX = 150f + gcPodiumX;
                if (p.pos.y > 270f && p.pos.y < 310f && Mathf.Abs(p.pos.x - playerX) < 25f) {
                    gcHits++;
                    p.ve.RemoveFromHierarchy();
                    gcProjs.RemoveAt(i);
                    // Shake podium
                    gcPodium.style.translate = new StyleTranslate(new Translate(gcPodiumX + (UnityEngine.Random.value > 0.5f ? 10 : -10), 0, 0));
                    continue;
                }
                
                if (p.pos.y > 320f || p.pos.x < -20f || p.pos.x > 320f) {
                    p.ve.RemoveFromHierarchy();
                    gcProjs.RemoveAt(i);
                }
            }

            if (gcHits >= 3) {
                AbortGame(); // Fail
                yield break;
            }

            if (timer <= 0f) {
                MarkWinReady();
                yield break;
            }
            yield return null;
        }
    }


    // ==========================================
    // FRANTIC BACKSPACE
    // ==========================================
    private void SetupFranticBackspace()
    {
        timer = currentConfig.timeLimit;
        fbCurrentMessage = fbOriginalAngryMessage;
        if (fbPlayerText != null) fbPlayerText.text = fbCurrentMessage + "|";
        if (fbTimerText != null) fbTimerText.text = $"AUTO-SEND: {timer:F1}s";
        activeGameRoutine = StartCoroutine(FranticBackspaceRoutine());
    }
    
    private System.Collections.IEnumerator FranticBackspaceRoutine()
    {
        isPlaying = true;
        
        while (isPlaying)
        {
            timer -= Time.deltaTime;
            
            if (fbTimerText != null) {
                fbTimerText.text = $"AUTO-SEND: {timer:F1}s";
                if (timer <= 2.0f) fbTimerText.style.color = Color.red;
                else fbTimerText.style.color = new StyleColor(ColorUtility.TryParseHtmlString("#DC2626", out Color c) ? c : Color.red);
            }

            if (fbCurrentMessage.Length == 0) {
                if (fbPlayerText != null) fbPlayerText.text = "Sounds great! 👍";
                if (fbTimerText != null) {
                    fbTimerText.text = "SENT!";
                    fbTimerText.style.color = new StyleColor(ColorUtility.TryParseHtmlString("#166534", out Color g) ? g : Color.green);
                }
                yield return new WaitForSeconds(1.0f);
                MarkWinReady();
                yield break;
            }

            if (timer <= 0f) {
                if (fbTimerText != null) fbTimerText.text = "SENT (UH OH)";
                yield return new WaitForSeconds(1.5f);
                AbortGame(); // Fail
                yield break;
            }
            yield return null;
        }
    }

    // ==========================================
    // FORMAT DASHBOARD (MAKE IT POP)
    // ==========================================
    
    
    

    // ==========================================
    // NIGHT SHIFT PITCH
    // ==========================================
    private void SetupNightShiftPitch()
    {
        timer = currentConfig.timeLimit;
        nsEyelidGravity = 0f;
        nsTargets[0] = 0.3f;
        nsTargets[1] = 0.6f;
        nsTargets[2] = 0.8f;

        for(int i=0; i<3; i++) {
            nsValues[i] = 0f;
            if (nsHandles[i] != null) nsHandles[i].style.left = new StyleLength(new Length(0, LengthUnit.Percent));
        }
        if (nsEyelid != null) nsEyelid.style.top = new StyleLength(new Length(-100, LengthUnit.Percent));
        activeGameRoutine = StartCoroutine(NightShiftRoutine());
    }
    
    private System.Collections.IEnumerator NightShiftRoutine()
    {
        isPlaying = true;
        
        while (isPlaying)
        {
            timer -= Time.deltaTime;
            
            // Eyelid drops at 20% per second
            if (nsTimerText != null) {
                nsTimerText.text = $"{timer:F1}s";
                nsTimerText.style.color = timer <= 3f ? Color.red : Color.white;
            }
            nsEyelidGravity += 0.20f * Time.deltaTime;
            nsEyelidGravity = Mathf.Clamp01(nsEyelidGravity);
            
            if (nsEyelid != null) {
                // -100% is fully open, 0% is fully closed
                float topPercent = -100f + (nsEyelidGravity * 100f);
                nsEyelid.style.top = new StyleLength(new Length(topPercent, LengthUnit.Percent));
            }

            if (nsEyelidGravity >= 1f) {
                AbortGame(); // Fell asleep
                yield break;
            }

            bool allAligned = true;
            for(int i=0; i<3; i++)
            {
                if (nsHandles[i] != null) {
                    // Visual offset (assume track is 100% width, handle is 40px, so travel is ~ 100% - 40px)
                    // We'll just set it via percent for simplicity, but cap it so it doesn't overflow right
                    float pct = Mathf.Clamp(nsValues[i] * 100f, 0f, 85f);
                    nsHandles[i].style.left = new StyleLength(new Length(pct, LengthUnit.Percent));
                }
                
                // Target width is ~40px wide, so +/- 0.1f tolerance is generous
                if (Mathf.Abs(nsValues[i] - nsTargets[i]) > 0.08f) allAligned = false;
            }

            if (allAligned) {
                MarkWinReady();
                yield break;
            }

            if (timer <= 0f) {
                AbortGame();
                yield break;
            }
            
            yield return null;
        }
    }

    // ==========================================
    // YES MAN NOD
    // ==========================================
    private string[] ymComplaints = new string[] {
        "SYNERGY IS DOWN 4%!",
        "WHO AUTHORIZED THIS BREAK!?",
        "WE NEED MORE ACTIONABLE METRICS!",
        "THE MARGINS ARE TOO THIN!",
        "WHERE IS THE PARADIGM SHIFT!?",
        "THIS IS UNACCEPTABLE!",
        "PIVOT! WE NEED TO PIVOT!"
    };

    private void SetupYesManNod()
    {
        timer = currentConfig.timeLimit;
        if (ymBossBubble != null) ymBossBubble.style.display = DisplayStyle.None;
        if (ymPlayerBubble != null) ymPlayerBubble.style.display = DisplayStyle.None;
        activeGameRoutine = StartCoroutine(YesManRoutine());
    }
    
    private System.Collections.IEnumerator YesManRoutine()
    {
        isPlaying = true;
        int complaintsHandled = 0;
        
        while (isPlaying && complaintsHandled < 5)
        {
            // Brief pause before next complaint
            yield return new WaitForSeconds(Random.Range(0.2f, 0.5f));
            
            ymSwipedUp = false;
            if (ymBossText != null) ymBossText.text = ymComplaints[Random.Range(0, ymComplaints.Length)];
            if (ymBossBubble != null) ymBossBubble.style.display = DisplayStyle.Flex;
            if (ymPlayerBubble != null) ymPlayerBubble.style.display = DisplayStyle.None;
            
            float reactTimer = 1.8f; // increased because nodding takes longer
            bool reacted = false;
            
            while (reactTimer > 0f) {
                reactTimer -= Time.deltaTime;
                timer -= Time.deltaTime;
                
                if (ymSwipedUp) {
                    reacted = true;
                    break;
                }
                if (timer <= 0f) { AbortGame(); yield break; }
                yield return null;
            }
            
            if (!reacted) {
                AbortGame(); // Didn't nod in time, fired.
                yield break;
            }
            
            // Successfully nodded
            if (ymPlayerBubble != null) ymPlayerBubble.style.display = DisplayStyle.Flex;
            complaintsHandled++;
            yield return new WaitForSeconds(0.6f); // let them see the response
            if (ymBossBubble != null) ymBossBubble.style.display = DisplayStyle.None;
        }

        MarkWinReady();
    }

    // ==========================================
    // VR ARCADE PONG
    // ==========================================
    
    private void OnHsPointerDown(UnityEngine.UIElements.PointerDownEvent ev, int stoneIdx)
    {
        if (!isPlaying || hsStonePlaced[stoneIdx]) return;
        hsActiveStoneIndex = stoneIdx;
        hsPointerStartPos = ev.position;
        float currentLeft = hsStones[stoneIdx].resolvedStyle.left;
        float currentTop = hsStones[stoneIdx].resolvedStyle.top;
        if (float.IsNaN(currentLeft)) currentLeft = 0;
        if (float.IsNaN(currentTop)) currentTop = 0;
        
        hsStoneStartPos = new UnityEngine.Vector2(currentLeft, currentTop);
        hsStones[stoneIdx].CapturePointer(ev.pointerId);
    }

    private void OnHsPointerMove(UnityEngine.UIElements.PointerMoveEvent ev, int stoneIdx)
    {
        if (!isPlaying || hsActiveStoneIndex != stoneIdx || hsStonePlaced[stoneIdx]) return;
        
        UnityEngine.Vector2 delta = (UnityEngine.Vector2)ev.position - hsPointerStartPos;
        float newLeft = hsStoneStartPos.x + delta.x;
        float newTop = hsStoneStartPos.y + delta.y;
        
        hsStones[stoneIdx].style.left = newLeft;
        hsStones[stoneIdx].style.top = newTop;
        hsStones[stoneIdx].style.bottom = UnityEngine.UIElements.StyleKeyword.Auto;
        hsStones[stoneIdx].style.right = UnityEngine.UIElements.StyleKeyword.Auto;
    }

    private void OnHsPointerUp(UnityEngine.UIElements.EventBase ev, int stoneIdx)
    {
        if (hsActiveStoneIndex != stoneIdx) return;
        hsActiveStoneIndex = -1;
        
        if (ev is UnityEngine.UIElements.IPointerEvent pointerEvent) {
            hsStones[stoneIdx].ReleasePointer(pointerEvent.pointerId);
        }
        
        if (hsStonePlaced[stoneIdx]) return;

        UnityEngine.Vector2 sCenter = hsStones[stoneIdx].worldBound.center;
        
        for (int i = 0; i < 5; i++) {
            if (hsSlots[i] == null) continue;
            
            UnityEngine.Vector2 targetCenter = hsSlots[i].worldBound.center;
            
            if (UnityEngine.Vector2.Distance(sCenter, targetCenter) < 50f) {
                hsStonePlaced[stoneIdx] = true;
                hsStones[stoneIdx].style.backgroundColor = new UnityEngine.UIElements.StyleColor(new UnityEngine.Color(0.82f, 0.33f, 0f)); // #D35400
                // Center the stone roughly in the slot
                hsStones[stoneIdx].style.left = hsStones[stoneIdx].resolvedStyle.left + (targetCenter.x - sCenter.x);
                hsStones[stoneIdx].style.top = hsStones[stoneIdx].resolvedStyle.top + (targetCenter.y - sCenter.y);
                
                CheckHotStoneWin();
                return;
            }
        }
    }

    private void CheckHotStoneWin()
    {
        bool allPlaced = true;
        for (int i = 0; i < 5; i++) {
            if (!hsStonePlaced[i]) {
                allPlaced = false;
                break;
            }
        }
        if (allPlaced) {
            MarkWinReady();
        }
    }

    
    // ==========================================
    // HOSPITALITY GALA
    // ==========================================
    










    // ==========================================
    // FORGE PASS (ILLEGAL)
    // ==========================================
    
    // ==========================================
    // SNATCH BRIEFCASE (Dial Spin)
    // ==========================================
    private void SetupSnatchBriefcase()
    {
        timer = currentConfig.timeLimit;
        sbCurrentDial = 0;
        sbSpinTimer = 0f;
        if (sbStopBtn != null) sbStopBtn.text = "STOP DIAL 1";
        
        for (int i=0; i<3; i++) {
            sbTargetNums[i] = UnityEngine.Random.Range(0, 10);
            sbCurrentDialNums[i] = UnityEngine.Random.Range(0, 10);
            
            if (sbDialsOut[i] != null) sbDialsOut[i].text = ((sbCurrentDialNums[i] + 2) % 10).ToString();
            if (sbDialsTop[i] != null) sbDialsTop[i].text = ((sbCurrentDialNums[i] + 1) % 10).ToString();
            if (sbDialsMid[i] != null) sbDialsMid[i].text = sbCurrentDialNums[i].ToString();
            if (sbDialsBot[i] != null) sbDialsBot[i].text = ((sbCurrentDialNums[i] - 1 + 10) % 10).ToString();
            
            if (sbDialsMid[i] != null) sbDialsMid[i].style.color = UnityEngine.Color.black;
            if (sbDialStrips[i] != null) sbDialStrips[i].style.top = -75f;
        }
        if (sbTargetText != null) sbTargetText.text = $"TARGET: {sbTargetNums[0]} - {sbTargetNums[1]} - {sbTargetNums[2]}";
        
        activeGameRoutine = StartCoroutine(SnatchBriefcaseRoutine());
    }

    private void OnSbStopClicked()
    {
        if (!isPlaying || sbCurrentDial >= 3) return;
        
        int currentNum = sbCurrentDialNums[sbCurrentDial];
        if (currentNum == sbTargetNums[sbCurrentDial]) {
            if (sbDialsMid[sbCurrentDial] != null) sbDialsMid[sbCurrentDial].style.color = new UnityEngine.Color(0.18f, 0.8f, 0.44f); // Green
            if (sbDialStrips[sbCurrentDial] != null) sbDialStrips[sbCurrentDial].style.top = -75f; // Snap to perfect center
            sbCurrentDial++;
            if (sbCurrentDial >= 3) {
                sbStopBtn.text = "UNLOCKED!";
                MarkWinReady();
            } else {
                sbStopBtn.text = $"STOP DIAL {sbCurrentDial + 1}";
            }
        } else {
            if (sbDialsMid[sbCurrentDial] != null) sbDialsMid[sbCurrentDial].style.color = new UnityEngine.Color(0.9f, 0.3f, 0.23f); // Red
            sbStopBtn.text = "WRONG PIN!";
            AbortGame();
        }
    }

    private System.Collections.IEnumerator SnatchBriefcaseRoutine()
    {
        isPlaying = true;
        while (isPlaying)
        {
            timer -= UnityEngine.Time.deltaTime;
            sbSpinTimer += UnityEngine.Time.deltaTime;
            
            // Speed starts at 0.12. Gets faster with each lock cracked.
            float currentSpeed = 0.35f - (sbCurrentDial * 0.08f); 
            if (currentSpeed < 0.15f) currentSpeed = 0.15f;
            
            float progress = sbSpinTimer / currentSpeed;
            if (progress > 1f) progress = 1f;
            float currentTop = -75f + (progress * 75f);
            
            for (int i = sbCurrentDial; i < 3; i++) {
                if (sbDialStrips[i] != null) {
                    sbDialStrips[i].style.top = currentTop;
                }
            }

            if (sbSpinTimer >= currentSpeed) {
                sbSpinTimer = 0f;
                for (int i = sbCurrentDial; i < 3; i++) {
                    sbCurrentDialNums[i] = (sbCurrentDialNums[i] + 1) % 10;
                    
                    if (sbDialsOut[i] != null) sbDialsOut[i].text = ((sbCurrentDialNums[i] + 2) % 10).ToString();
                    if (sbDialsTop[i] != null) sbDialsTop[i].text = ((sbCurrentDialNums[i] + 1) % 10).ToString();
                    if (sbDialsMid[i] != null) sbDialsMid[i].text = sbCurrentDialNums[i].ToString();
                    if (sbDialsBot[i] != null) sbDialsBot[i].text = ((sbCurrentDialNums[i] - 1 + 10) % 10).ToString();
                    
                    if (sbDialStrips[i] != null) sbDialStrips[i].style.top = -75f;
                }
            }

            if (timer <= 0f) {
                AbortGame();
                yield break;
            }
            yield return null;
        }
    }

    // Removed old SkimATM

    private void SetupForgePass()
    {
        timer = currentConfig.timeLimit;
        fpAttempts = 0;
        fpSuccessfulStamps = 0;
        fpSpeed = 700f;
        fpPassPos = -250f;
        fpIsPaused = false;
        
        if (fpScoreText != null) fpScoreText.text = $"FORGED: {fpSuccessfulStamps} / 1";
        if (fpStampBtn != null) fpStampBtn.text = "STAMP NOW! (3 LIVES)";
        if (fpMovingPass != null) {
            fpMovingPass.style.left = fpPassPos;
            fpMovingPass.style.backgroundColor = new UnityEngine.UIElements.StyleColor(new UnityEngine.Color(0.97f, 0.98f, 0.98f)); // #F8FAFC
        }
        if (fpPassStampOverlay != null) {
            fpPassStampOverlay.text = "";
            fpPassStampOverlay.style.color = UnityEngine.Color.clear;
        }
        
        activeGameRoutine = StartCoroutine(ForgePassRoutine());
    }

    private void OnFpStampClicked()
    {
        if (!isPlaying || fpIsPaused) return;
        
        UnityEngine.Vector2 passCenter = fpMovingPass.worldBound.center;
        UnityEngine.UIElements.VisualElement stampZone = fpContainer.Q<UnityEngine.UIElements.VisualElement>("FpStampZone");
        UnityEngine.Vector2 stampCenter = stampZone.worldBound.center;
        
        float dist = UnityEngine.Vector2.Distance(passCenter, stampCenter);
        bool success = (dist < 40f);
        
        StartCoroutine(ShowFpStampResult(success));
    }
    
    private System.Collections.IEnumerator ShowFpStampResult(bool success)
    {
        fpIsPaused = true;
        
        if (fpPassStampOverlay != null) {
            fpPassStampOverlay.text = success ? "APPROVED" : "REJECTED";
            fpPassStampOverlay.style.color = success ? new UnityEngine.Color(0.06f, 0.73f, 0.51f, 0.9f) : new UnityEngine.Color(0.94f, 0.27f, 0.27f, 0.9f);
            
            // Slam animation
            fpPassStampOverlay.style.scale = new UnityEngine.UIElements.StyleScale(new UnityEngine.UIElements.Scale(new UnityEngine.Vector3(3f, 3f, 1f)));
            yield return null;
            
            float slamTime = 0.15f;
            float elapsed = 0f;
            while (elapsed < slamTime) {
                elapsed += UnityEngine.Time.deltaTime;
                float t = elapsed / slamTime;
                float scale = UnityEngine.Mathf.Lerp(3f, 1f, t);
                fpPassStampOverlay.style.scale = new UnityEngine.UIElements.StyleScale(new UnityEngine.UIElements.Scale(new UnityEngine.Vector3(scale, scale, 1f)));
                yield return null;
            }
            fpPassStampOverlay.style.scale = new UnityEngine.UIElements.StyleScale(new UnityEngine.UIElements.Scale(new UnityEngine.Vector3(1f, 1f, 1f)));
        }
        
        yield return new UnityEngine.WaitForSeconds(0.4f);
        
        if (success) {
            fpSuccessfulStamps++;
            if (fpScoreText != null) fpScoreText.text = $"FORGED: {fpSuccessfulStamps} / 1";
            if (fpSuccessfulStamps >= 1) {
                MarkWinReady();
                yield break;
            }
        } else {
            fpAttempts++;
            if (fpAttempts >= 3) {
                AbortGame();
                yield break;
            }
            if (fpStampBtn != null) fpStampBtn.text = $"STAMP NOW! ({3 - fpAttempts} LIVES)";
        }
        
        // Reset for next pass
        fpSpeed += 250f;
        fpPassPos = -250f;
        if (fpPassStampOverlay != null) {
            fpPassStampOverlay.text = "";
            fpPassStampOverlay.style.color = UnityEngine.Color.clear;
        }
        
        fpIsPaused = false;
    }

    private System.Collections.IEnumerator ForgePassRoutine()
    {
        isPlaying = true;
        while (isPlaying)
        {
            if (!fpIsPaused) {
                timer -= UnityEngine.Time.deltaTime;
                
                fpPassPos += fpSpeed * UnityEngine.Time.deltaTime;
                if (fpPassPos > 700f) {
                    fpAttempts++;
                    if (fpAttempts >= 3) {
                        AbortGame();
                        yield break;
                    } else {
                        fpSpeed += 250f;
                        fpPassPos = -250f;
                        if (fpStampBtn != null) fpStampBtn.text = $"STAMP NOW! ({3 - fpAttempts} LIVES)";
                    }
                }
                
                if (fpMovingPass != null) {
                    fpMovingPass.style.left = fpPassPos;
                }
            }

            if (timer <= 0f) {
                AbortGame();
                yield break;
            }
            yield return null;
        }
    }

    private void SetupHospitalityGala()
    {
        timer = currentConfig.timeLimit;
        hgBalance = 0.5f;
        hgVelocity = 0f;
        activeGameRoutine = StartCoroutine(HospitalityGalaRoutine());
    }

    private System.Collections.IEnumerator HospitalityGalaRoutine()
    {
        isPlaying = true;
        float randomDrift = 0f;
        while (isPlaying)
        {
            timer -= UnityEngine.Time.deltaTime;
            if (hgTimerText != null) hgTimerText.text = $"{UnityEngine.Mathf.Max(0, timer):F1}s";
            
            // Apply drift
            randomDrift += UnityEngine.Random.Range(-2f, 2f) * UnityEngine.Time.deltaTime;
            randomDrift = UnityEngine.Mathf.Clamp(randomDrift, -0.5f, 0.5f);
            
            hgBalance += (hgVelocity + randomDrift) * UnityEngine.Time.deltaTime * 1.5f;
            
            if (hgIndicator != null) {
                hgIndicator.style.left = new UnityEngine.UIElements.StyleLength(new UnityEngine.UIElements.Length(hgBalance * 100f, UnityEngine.UIElements.LengthUnit.Percent));
            }

            if (hgBalance < 0f || hgBalance > 1f) {
                if (hgTimerText != null) hgTimerText.text = "SPILLED!";
                yield return new UnityEngine.WaitForSeconds(1f);
                AbortGame();
                yield break;
            }

            if (timer <= 0f) {
                MarkWinReady();
                yield break;
            }
            yield return null;
        }
    }

    // ==========================================
    // STARTUP PITCH MEETUP
    // ==========================================
    private void SetupStartupPitch()
    {
        timer = currentConfig.timeLimit;
        spCurrentReel = 0;
        spSpinTimer = 0f;
        spPatternIndex = 0;
        if (spStopBtn != null) spStopBtn.text = "STOP REEL 1";
        
        UpdateUnstoppedReels();
        
        activeGameRoutine = StartCoroutine(StartupPitchRoutine());
    }
    
    private void UpdateUnstoppedReels()
    {
        for (int i = spCurrentReel; i < 3; i++) {
            if (spSlots[i] != null) {
                // Offset the pattern for each reel so they look different but are predictable
                int index = (spPatternIndex + (i * 3)) % spSlotPattern.Length;
                spSlots[i].text = spSlotPattern[index];
                spSlots[i].style.color = UnityEngine.Color.white;
            }
        }
    }

    private void OnSpStopClicked()
    {
        if (!isPlaying || spCurrentReel >= 3) return;
        
        string word = spSlots[spCurrentReel].text;
        bool isGood = System.Array.Exists(spGoodWords, w => w == word);
        
        if (isGood) {
            spSlots[spCurrentReel].style.color = new UnityEngine.Color(0.18f, 0.8f, 0.44f); // Green
            spCurrentReel++;
            if (spCurrentReel >= 3) {
                spStopBtn.text = "FUNDED!";
                MarkWinReady();
            } else {
                spStopBtn.text = $"STOP REEL {spCurrentReel + 1}";
            }
        } else {
            spSlots[spCurrentReel].style.color = new UnityEngine.Color(0.9f, 0.29f, 0.23f); // Red
            spStopBtn.text = "FAILED PITCH";
            AbortGame();
        }
    }

    private System.Collections.IEnumerator StartupPitchRoutine()
    {
        isPlaying = true;
        while (isPlaying)
        {
            timer -= UnityEngine.Time.deltaTime;
            spSpinTimer += UnityEngine.Time.deltaTime;
            
            // Slower spin (0.35s) gives time to anticipate the fixed pattern
            if (spSpinTimer > 0.35f) {
                spSpinTimer = 0f;
                spPatternIndex = (spPatternIndex + 1) % spSlotPattern.Length;
                UpdateUnstoppedReels();
            }

            if (timer <= 0f) {
                AbortGame();
                yield break;
            }
            yield return null;
        }
    }




    private void SetupHotStoneSpa()
    {
        timer = currentConfig.timeLimit;
        hsActiveStoneIndex = -1;
        
        for (int i = 0; i < 5; i++) {
            hsStonePlaced[i] = false;
            if (hsStones[i] != null) {
                hsStones[i].style.backgroundColor = new UnityEngine.UIElements.StyleColor(new UnityEngine.Color(0.18f, 0.31f, 0.31f)); // #2F4F4F
                hsStones[i].style.left = UnityEngine.UIElements.StyleKeyword.Auto;
                hsStones[i].style.top = UnityEngine.UIElements.StyleKeyword.Auto;
                hsStones[i].style.right = UnityEngine.UIElements.StyleKeyword.Auto;
                
                hsStones[i].style.bottom = 20f + UnityEngine.Random.Range(-10f, 10f);
                float lx = 40f + (i * 60f) + UnityEngine.Random.Range(-10f, 10f);
                hsStones[i].style.left = lx;
            }
        }

        activeGameRoutine = StartCoroutine(HotStoneRoutine());
    }

    private System.Collections.IEnumerator HotStoneRoutine()
    {
        isPlaying = true;
        while (isPlaying)
        {
            timer -= UnityEngine.Time.deltaTime;
            if (hsTimerText != null) hsTimerText.text = $"{UnityEngine.Mathf.Max(0, timer):F1}s";
            
            if (timer <= 0f) {
                AbortGame();
                yield break;
            }
            yield return null;
        }
    }

    private void SetupVrArcadePong()
    {
        timer = currentConfig.timeLimit;
        vrBounces = 0;
        vrBallPos = new Vector2(200f, 50f);
        vrBallVel = new Vector2(Random.value > 0.5f ? 150f : -150f, 200f);
        vrPaddleX = 200f;
        
        if (vrScoreText != null) vrScoreText.text = $"BOUNCES: 0 / 5";
        if (vrBall != null) {
            vrBall.style.left = new StyleLength(new Length(vrBallPos.x, LengthUnit.Pixel));
            vrBall.style.top = new StyleLength(new Length(vrBallPos.y, LengthUnit.Pixel));
        }

        activeGameRoutine = StartCoroutine(VrArcadeRoutine());
    }
    
    private System.Collections.IEnumerator VrArcadeRoutine()
    {
        isPlaying = true;
        
        float containerW = 400f;
        float containerH = 400f;

        float paddleWidth = 100f;
        float paddleHeight = 20f;
        float paddleBottomOffset = 10f;
        float ballSize = 20f;

        while (isPlaying)
        {
            timer -= Time.deltaTime;
            
            if (vrArcadeContainer != null) {
                float w = vrArcadeContainer.resolvedStyle.width;
                float h = vrArcadeContainer.resolvedStyle.height;
                if (!float.IsNaN(w) && w > 0) containerW = w;
                if (!float.IsNaN(h) && h > 0) containerH = h;
            }

            // Move Ball
            vrBallPos += vrBallVel * Time.deltaTime;

            // Constrain Paddle (centered on vrPaddleX)
            float paddleLeft = Mathf.Clamp(vrPaddleX - (paddleWidth / 2f), 0, containerW - paddleWidth);
            if (vrPaddle != null) vrPaddle.style.left = new StyleLength(new Length(paddleLeft, LengthUnit.Pixel));

            // Wall Collisions
            if (vrBallPos.x <= 0f) {
                vrBallPos.x = 0f;
                vrBallVel.x *= -1f;
            } else if (vrBallPos.x >= containerW - ballSize) {
                vrBallPos.x = containerW - ballSize;
                vrBallVel.x *= -1f;
            }

            if (vrBallPos.y <= 0f) {
                vrBallPos.y = 0f;
                vrBallVel.y *= -1f;
            }

            // Paddle Collision (paddle is at containerH - paddleBottomOffset - paddleHeight)
            float paddleTopY = containerH - paddleBottomOffset - paddleHeight;
            
            if (vrBallVel.y > 0 && vrBallPos.y + ballSize >= paddleTopY && vrBallPos.y < paddleTopY + paddleHeight) {
                // Check X bounds
                if (vrBallPos.x + ballSize >= paddleLeft && vrBallPos.x <= paddleLeft + paddleWidth) {
                    // Bounce!
                    vrBallPos.y = paddleTopY - ballSize;
                    vrBallVel.y *= -1f; // go up
                    
                    // Add some english based on where it hit the paddle
                    float hitFactor = (vrBallPos.x + (ballSize/2f)) - (paddleLeft + (paddleWidth/2f));
                    vrBallVel.x = hitFactor * 5f; // apply horizontal speed based on hit offset
                    
                    // Speed up slightly
                    vrBallVel *= 1.1f;
                    
                    vrBounces++;
                    if (vrScoreText != null) vrScoreText.text = $"BOUNCES: {vrBounces} / 5";

                    if (vrBounces >= 5) {
                        MarkWinReady();
                        yield break;
                    }
                }
            }

            // Floor Collision (Miss)
            if (vrBallPos.y > containerH) {
                if (vrScoreText != null) vrScoreText.text = "FAILED!";
                yield return new WaitForSeconds(1.0f);
                AbortGame();
                yield break;
            }

            // Update visuals
            if (vrBall != null) {
                vrBall.style.left = new StyleLength(new Length(vrBallPos.x, LengthUnit.Pixel));
                vrBall.style.top = new StyleLength(new Length(vrBallPos.y, LengthUnit.Pixel));
            }

            if (timer <= 0f) {
                AbortGame();
                yield break;
            }
            
            yield return null;
        }
    }

    // Removed old games (CorporateGolf, MowGreens, WalkDogs, RecycleBottles)
    
    // Phase 1 Custom Games

    // CloggedVents
    private void SetupCloggedVents()
    {
        if (cvContainer == null) return;
        cvContainer.Clear();
        cvVents.Clear();
        
        int count = 6;
        cvDustLevels = new float[count];
        cvIsClogged = new bool[count];

        Label timerLabel = new Label("15.0");
        timerLabel.name = "TimerLabel";
        timerLabel.style.fontSize = 24;
        timerLabel.style.color = Color.white;
        timerLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
        timerLabel.style.marginBottom = 20;
        cvContainer.Add(timerLabel);

        cvGrid = new VisualElement();
        cvGrid.style.flexDirection = FlexDirection.Row;
        cvGrid.style.flexWrap = Wrap.Wrap;
        cvGrid.style.justifyContent = Justify.Center;
        cvContainer.Add(cvGrid);

        for (int i = 0; i < count; i++)
        {
            VisualElement vent = new VisualElement();
            vent.style.width = 100;
            vent.style.height = 120;
            vent.style.marginTop = 10; vent.style.marginBottom = 10; vent.style.marginLeft = 10; vent.style.marginRight = 10;
            vent.style.backgroundColor = new Color(0.1f, 0.1f, 0.1f);
            vent.style.borderTopWidth = 4; vent.style.borderBottomWidth = 4; vent.style.borderLeftWidth = 4; vent.style.borderRightWidth = 4;
            vent.style.borderTopColor = new Color(0.3f, 0.3f, 0.3f); vent.style.borderBottomColor = new Color(0.3f, 0.3f, 0.3f); vent.style.borderLeftColor = new Color(0.3f, 0.3f, 0.3f); vent.style.borderRightColor = new Color(0.3f, 0.3f, 0.3f);
            vent.style.justifyContent = Justify.FlexEnd;

            VisualElement dust = new VisualElement();
            dust.name = "DustFill";
            dust.style.width = new Length(100, LengthUnit.Percent);
            dust.style.height = new Length(0, LengthUnit.Percent);
            dust.style.backgroundColor = Color.green;
            
            vent.Add(dust);
            
            int index = i;
            vent.RegisterCallback<PointerDownEvent>(ev => {
                if (currentConfig.type != GameType.CloggedVents || !isPlaying || cvIsClogged[index]) return;
                cvDustLevels[index] = 0f;
            });

            cvVents.Add(vent);
            cvGrid.Add(vent);
        }
        timer = 15f;
        activeGameRoutine = StartCoroutine(CloggedVentsRoutine());
    }

    private IEnumerator CloggedVentsRoutine()
    {
        isPlaying = true;
        Label timerLabel = cvContainer.Q<Label>("TimerLabel");
        
        while (isPlaying)
        {
            timer -= Time.deltaTime;
            if (timerLabel != null)
                timerLabel.text = timer.ToString("0.0");

            if (timer <= 0f)
            {
                EndGame(true);
                yield break;
            }
            
            int cloggedCount = 0;
            for (int i = 0; i < cvDustLevels.Length; i++)
            {
                if (cvIsClogged[i])
                {
                    cloggedCount++;
                    continue;
                }

                // Random dust build rate between 5% and 15% per second
                cvDustLevels[i] += UnityEngine.Random.Range(5f, 15f) * Time.deltaTime;
                
                if (cvDustLevels[i] >= 100f)
                {
                    cvDustLevels[i] = 100f;
                    cvIsClogged[i] = true;
                    cvVents[i].style.backgroundColor = Color.gray;
                    VisualElement d = cvVents[i].Q<VisualElement>("DustFill");
                    if (d != null) d.style.backgroundColor = Color.gray;
                    cvVents[i].style.borderTopColor = Color.gray; cvVents[i].style.borderBottomColor = Color.gray; cvVents[i].style.borderLeftColor = Color.gray; cvVents[i].style.borderRightColor = Color.gray;
                    cloggedCount++;
                    continue;
                }

                VisualElement dust = cvVents[i].Q<VisualElement>("DustFill");
                if (dust != null)
                {
                    dust.style.height = new Length(cvDustLevels[i], LengthUnit.Percent);
                    if (cvDustLevels[i] < 33f) dust.style.backgroundColor = Color.green;
                    else if (cvDustLevels[i] < 66f) dust.style.backgroundColor = Color.yellow;
                    else if (cvDustLevels[i] < 85f) dust.style.backgroundColor = new Color(1f, 0.5f, 0f); // Orange
                    else dust.style.backgroundColor = Color.red;
                }

                if (cvDustLevels[i] > 70f)
                {
                    float pulse = (Mathf.Sin(Time.time * 10f) + 1f) * 0.5f;
                    Color pulseColor = Color.Lerp(new Color(0.3f, 0.3f, 0.3f), Color.red, pulse);
                    cvVents[i].style.borderTopColor = pulseColor; cvVents[i].style.borderBottomColor = pulseColor; cvVents[i].style.borderLeftColor = pulseColor; cvVents[i].style.borderRightColor = pulseColor;
                }
                else
                {
                    cvVents[i].style.borderTopColor = new Color(0.3f, 0.3f, 0.3f); cvVents[i].style.borderBottomColor = new Color(0.3f, 0.3f, 0.3f); cvVents[i].style.borderLeftColor = new Color(0.3f, 0.3f, 0.3f); cvVents[i].style.borderRightColor = new Color(0.3f, 0.3f, 0.3f);
                }
            }

            if (cloggedCount >= 2)
            {
                EndGame(false);
                yield break;
            }
            yield return null;
        }
    }

    // 2. QATriage
    private void SetupQATriage()
    {
        if (qaContainer == null) return;
        qaContainer.Clear();
        qaSevBtns.Clear();
        qaCatBtns.Clear();
        qaScore = 0;
        qaCurrentLogIndex = 0;
        qaSelectedSev = -1;
        qaSelectedCat = -1;

        Label header = new Label("Score: 0 / 5   Time: 20.0");
        header.name = "HeaderLabel";
        header.style.fontSize = 24;
        header.style.color = Color.white;
        header.style.unityTextAlign = TextAnchor.MiddleCenter;
        header.style.marginBottom = 20;
        qaContainer.Add(header);

        qaLogText = new Label(currentConfig.qaBugLogs != null && currentConfig.qaBugLogs.Length > 0 ? currentConfig.qaBugLogs[0] : "NullReferenceException at line 42");
        qaLogText.style.fontSize = 20;
        qaLogText.style.color = Color.green;
        qaLogText.style.backgroundColor = new Color(0.1f, 0.1f, 0.1f);
        qaLogText.style.paddingTop = 15; qaLogText.style.paddingBottom = 15; qaLogText.style.paddingLeft = 15; qaLogText.style.paddingRight = 15;
        qaLogText.style.marginBottom = 20;
        qaLogText.style.unityTextAlign = TextAnchor.MiddleCenter;
        // Monospace approximation
        qaLogText.style.unityFontDefinition = StyleKeyword.None;
        qaContainer.Add(qaLogText);

        qaSeverityRow = new VisualElement();
        qaSeverityRow.style.flexDirection = FlexDirection.Row;
        qaSeverityRow.style.justifyContent = Justify.Center;
        qaSeverityRow.style.marginBottom = 15;
        qaContainer.Add(qaSeverityRow);

        string[] sevLabels = currentConfig.qaSeverities != null && currentConfig.qaSeverities.Length > 0 ? currentConfig.qaSeverities : new string[] { "Minor", "Major", "Critical" };
        Color[] sevColors = { Color.blue, new Color(1f, 0.5f, 0f), Color.red };
        for (int i = 0; i < sevLabels.Length; i++)
        {
            int index = i;
            Button btn = new Button();
            btn.text = sevLabels[i];
            btn.style.fontSize = 18;
            btn.style.backgroundColor = sevColors[i % sevColors.Length];
            btn.style.color = Color.white;
            btn.style.width = 100;
            btn.style.height = 40;
            btn.style.marginLeft = 5; btn.style.marginRight = 5;
            btn.style.borderTopWidth = 2; btn.style.borderBottomWidth = 2; btn.style.borderLeftWidth = 2; btn.style.borderRightWidth = 2;
            btn.style.borderTopColor = Color.clear; btn.style.borderBottomColor = Color.clear; btn.style.borderLeftColor = Color.clear; btn.style.borderRightColor = Color.clear;
            
            btn.clicked += () => {
                if (!isPlaying) return;
                qaSelectedSev = index;
                foreach (var b in qaSevBtns) { b.style.borderTopColor = Color.clear; b.style.borderBottomColor = Color.clear; b.style.borderLeftColor = Color.clear; b.style.borderRightColor = Color.clear; }
                btn.style.borderTopColor = Color.white; btn.style.borderBottomColor = Color.white; btn.style.borderLeftColor = Color.white; btn.style.borderRightColor = Color.white;
            };
            qaSevBtns.Add(btn);
            qaSeverityRow.Add(btn);
        }

        qaCategoryRow = new VisualElement();
        qaCategoryRow.style.flexDirection = FlexDirection.Row;
        qaCategoryRow.style.justifyContent = Justify.Center;
        qaCategoryRow.style.marginBottom = 20;
        qaContainer.Add(qaCategoryRow);

        string[] catLabels = currentConfig.qaCategories != null && currentConfig.qaCategories.Length > 0 ? currentConfig.qaCategories : new string[] { "UI 🖥️", "Network 🌐", "Logic ⚙️" };
        for (int i = 0; i < catLabels.Length; i++)
        {
            int index = i;
            Button btn = new Button();
            btn.text = catLabels[i];
            btn.style.fontSize = 18;
            btn.style.backgroundColor = new Color(0.3f, 0.3f, 0.3f);
            btn.style.color = Color.white;
            btn.style.width = 120;
            btn.style.height = 40;
            btn.style.marginLeft = 5; btn.style.marginRight = 5;
            btn.style.borderTopWidth = 2; btn.style.borderBottomWidth = 2; btn.style.borderLeftWidth = 2; btn.style.borderRightWidth = 2;
            btn.style.borderTopColor = Color.clear; btn.style.borderBottomColor = Color.clear; btn.style.borderLeftColor = Color.clear; btn.style.borderRightColor = Color.clear;
            
            btn.clicked += () => {
                if (!isPlaying) return;
                qaSelectedCat = index;
                foreach (var b in qaCatBtns) { b.style.borderTopColor = Color.clear; b.style.borderBottomColor = Color.clear; b.style.borderLeftColor = Color.clear; b.style.borderRightColor = Color.clear; }
                btn.style.borderTopColor = Color.white; btn.style.borderBottomColor = Color.white; btn.style.borderLeftColor = Color.white; btn.style.borderRightColor = Color.white;
            };
            qaCatBtns.Add(btn);
            qaCategoryRow.Add(btn);
        }

        btnQASubmit = new Button();
        btnQASubmit.text = "SUBMIT";
        btnQASubmit.style.fontSize = 22;
        btnQASubmit.style.backgroundColor = Color.green;
        btnQASubmit.style.color = Color.black;
        btnQASubmit.style.width = 150;
        btnQASubmit.style.height = 50;
        btnQASubmit.style.alignSelf = Align.Center;
        
        btnQASubmit.clicked += () => {
            if (!isPlaying || qaSelectedSev == -1 || qaSelectedCat == -1) return;
            qaScore++;
            qaSelectedSev = -1;
            qaSelectedCat = -1;
            foreach (var b in qaSevBtns) { b.style.borderTopColor = Color.clear; b.style.borderBottomColor = Color.clear; b.style.borderLeftColor = Color.clear; b.style.borderRightColor = Color.clear; }
            foreach (var b in qaCatBtns) { b.style.borderTopColor = Color.clear; b.style.borderBottomColor = Color.clear; b.style.borderLeftColor = Color.clear; b.style.borderRightColor = Color.clear; }
            
            qaCurrentLogIndex++;
            string[] logs = currentConfig.qaBugLogs != null && currentConfig.qaBugLogs.Length > 0 ? currentConfig.qaBugLogs : new string[] { "Error: Unknown", "Timeout Exception", "Out of memory" };
            qaLogText.text = logs[qaCurrentLogIndex % logs.Length];
            
            if (qaScore >= 5)
            {
                EndGame(true);
            }
        };
        qaContainer.Add(btnQASubmit);

        timer = 20f;
        activeGameRoutine = StartCoroutine(QATriageRoutine());
    }

    private IEnumerator QATriageRoutine()
    {
        isPlaying = true;
        Label header = qaContainer.Q<Label>("HeaderLabel");
        
        while (isPlaying)
        {
            timer -= Time.deltaTime;
            if (header != null)
                header.text = $"Score: {qaScore} / 5   Time: {timer:0.0}";

            if (timer <= 0f)
            {
                EndGame(false);
                yield break;
            }
            yield return null;
        }
    }

    // 3. WhackABug
    private void SetupWhackABug()
    {
        if (wbContainer == null) return;
        wbContainer.Clear();
        wbBugs.Clear();
        wbScore = 0;

        Label header = new Label("PASSING: 9/9   Fixes: 0/15   Time: 20.0");
        header.name = "HeaderLabel";
        header.style.fontSize = 24;
        header.style.color = Color.white;
        header.style.unityTextAlign = TextAnchor.MiddleCenter;
        header.style.marginBottom = 20;
        wbContainer.Add(header);

        wbGrid = new VisualElement();
        wbGrid.style.flexDirection = FlexDirection.Row;
        wbGrid.style.flexWrap = Wrap.Wrap;
        wbGrid.style.justifyContent = Justify.Center;
        wbGrid.style.width = 360;
        wbGrid.style.alignSelf = Align.Center;
        wbContainer.Add(wbGrid);

        string[] modules = { "Login", "Cart", "Profile", "API", "Auth", "DB", "Search", "Payment", "Settings" };
        wbBugIsRed = new bool[9];
        wbBugTimers = new float[9];

        for (int i = 0; i < 9; i++)
        {
            int index = i;
            Button btn = new Button();
            btn.text = modules[i] + "\n✅ PASS";
            btn.style.fontSize = 18;
            btn.style.width = 100;
            btn.style.height = 100;
            btn.style.marginTop = 10; btn.style.marginBottom = 10; btn.style.marginLeft = 10; btn.style.marginRight = 10;
            btn.style.backgroundColor = Color.green;
            btn.style.color = Color.black;
            
            btn.clicked += () => {
                if (!isPlaying || !wbBugIsRed[index]) return;
                wbBugIsRed[index] = false;
                btn.text = modules[index] + "\n✅ PASS";
                btn.style.backgroundColor = Color.green;
                wbScore++;
                
                if (wbScore >= 15)
                {
                    EndGame(true);
                }
            };
            
            wbBugs.Add(btn);
            wbGrid.Add(btn);
        }

        timer = 20f;
        activeGameRoutine = StartCoroutine(WhackABugRoutine());
    }

    private IEnumerator WhackABugRoutine()
    {
        isPlaying = true;
        Label header = wbContainer.Q<Label>("HeaderLabel");
        string[] modules = { "Login", "Cart", "Profile", "API", "Auth", "DB", "Search", "Payment", "Settings" };
        
        while (isPlaying)
        {
            timer -= Time.deltaTime;
            
            int redCount = 0;
            for (int i = 0; i < 9; i++)
            {
                if (wbBugIsRed[i])
                {
                    redCount++;
                    float pulse = (Mathf.Sin(Time.time * 15f) + 1f) * 0.5f;
                    wbBugs[i].style.backgroundColor = Color.Lerp(Color.red, new Color(0.5f, 0f, 0f), pulse);
                }
                else
                {
                    wbBugTimers[i] -= Time.deltaTime;
                    if (wbBugTimers[i] <= 0f)
                    {
                        if (UnityEngine.Random.value < 0.3f * Time.deltaTime)
                        {
                            wbBugIsRed[i] = true;
                            wbBugs[i].text = modules[i] + "\n❌ FAIL";
                            wbBugs[i].style.backgroundColor = Color.red;
                        }
                    }
                }
            }
            
            if (header != null)
                header.text = $"PASSING: {9 - redCount}/9   Fixes: {wbScore}/15   Time: {timer:0.0}";

            if (redCount >= 4)
            {
                EndGame(false);
                yield break;
            }

            if (timer <= 0f)
            {
                EndGame(wbScore >= 15);
                yield break;
            }
            
            yield return null;
        }
    }





}

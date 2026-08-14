using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class MiniGameManager : MonoBehaviour
{
    private IEnumerator ConstructionSiteRoutine()
    {
        isPlaying = true;
        Label header = csContainer.Q<Label>("HeaderLabel");
        Label wind = csContainer.Q<Label>("WindLabel");
        
        float nextDriftChange = 2f;
        csDrift = 15f; // start drifting right
        
        bool leftPressed = false;
        bool rightPressed = false;
        
        btnCsLeft.RegisterCallback<PointerDownEvent>(e => leftPressed = true);
        btnCsLeft.RegisterCallback<PointerUpEvent>(e => leftPressed = false);
        btnCsLeft.RegisterCallback<PointerLeaveEvent>(e => leftPressed = false);
        
        btnCsRight.RegisterCallback<PointerDownEvent>(e => rightPressed = true);
        btnCsRight.RegisterCallback<PointerUpEvent>(e => rightPressed = false);
        btnCsRight.RegisterCallback<PointerLeaveEvent>(e => rightPressed = false);

        while (isPlaying)
        {
            timer -= Time.deltaTime;
            nextDriftChange -= Time.deltaTime;
            
            if (nextDriftChange <= 0f)
            {
                nextDriftChange = UnityEngine.Random.Range(2f, 3f);
                csDrift = UnityEngine.Random.Range(10f, 25f) * (UnityEngine.Random.value > 0.5f ? 1f : -1f);
                if (wind != null)
                    wind.text = csDrift > 0 ? "WIND: ➡️" : "WIND: ⬅️";
            }
            
            float move = csDrift * Time.deltaTime;
            if (leftPressed) move -= 30f * Time.deltaTime;
            if (rightPressed) move += 30f * Time.deltaTime;
            
            csBalancePos += move;
            if (csPlayerMarker != null)
            {
                // Subtract half width (approx 2.5%) for centering
                csPlayerMarker.style.left = new Length(Mathf.Clamp(csBalancePos - 2.5f, 0f, 95f), LengthUnit.Percent);
            }
            
            if (csBalancePos < 0f || csBalancePos > 100f)
            {
                EndGame(false); // fell off
                yield break;
            }
            
            // Check if in green zone (35% to 65%)
            if (csBalancePos >= 35f && csBalancePos <= 65f)
            {
                csTimeInZone += Time.deltaTime;
                csBalanceZone.style.backgroundColor = Color.green;
            }
            else
            {
                csBalanceZone.style.backgroundColor = new Color(0f, 0.5f, 0f);
            }
            
            if (header != null)
                header.text = $"Inspection: {csTimeInZone:0.0} / 10.0   Time: {timer:0.0}";
                
            if (csTimeInZone >= 10f)
            {
                EndGame(true);
                yield break;
            }
            
            if (timer <= 0f)
            {
                EndGame(false);
                yield break;
            }
            
            yield return null;
        }
    }

    // ZenTracing
    

    

    // IceShatter
    

    

    // NameMemory
    

    

    // RetroRacer
    

    

    // Phase 3 & 4 Custom Games
    // MowGreens
    

    

    

    

    

    

    

    

    

    
    private void EndGame(bool win)
    {
        if (win)
            MarkWinReady();
        else
            AbortGame();
    }


    // --- 6. Format Dashboard ---
    private void SetupFormatDashboard()
    {
        timer = currentConfig.timeLimit;
        fdHoldTimer = 0f;
        
        formatDashboardContainer.Clear();
        formatDashboardContainer.style.flexDirection = FlexDirection.Column;
        formatDashboardContainer.style.alignItems = Align.Center;
        formatDashboardContainer.style.justifyContent = Justify.FlexStart;
        formatDashboardContainer.style.paddingTop = 20;

        Label title = new Label("Format Summary Dashboards");
        title.style.fontSize = 24;
        title.style.color = Color.white;
        title.style.marginBottom = 10;
        formatDashboardContainer.Add(title);

        fdTimerText = new Label($"{timer:F1}s");
        fdTimerText.style.fontSize = 32;
        fdTimerText.style.color = Color.white;
        fdTimerText.style.marginBottom = 20;
        formatDashboardContainer.Add(fdTimerText);

        VisualElement slidersRow = new VisualElement();
        slidersRow.style.flexDirection = FlexDirection.Row;
        slidersRow.style.justifyContent = Justify.Center;
        slidersRow.style.alignItems = Align.Center;
        slidersRow.style.width = new Length(100, LengthUnit.Percent);
        slidersRow.style.height = 300;
        
        string[] labels = { "Revenue", "Growth", "Retention" };
        Color[] colors = { Color.blue, Color.yellow, Color.red };

        for(int i = 0; i < 3; i++)
        {
            fdValues[i] = 1f;

            VisualElement sliderCol = new VisualElement();
            sliderCol.style.alignItems = Align.Center;
            sliderCol.style.marginRight = 20;
            sliderCol.style.marginLeft = 20;

            Label slLabel = new Label(labels[i]);
            slLabel.style.fontSize = 20;
            slLabel.style.color = Color.white;
            slLabel.style.marginBottom = 10;
            sliderCol.Add(slLabel);

            VisualElement track = new VisualElement();
            track.style.width = 40;
            track.style.height = 200;
            track.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f);
            track.style.borderTopLeftRadius = 20;
            track.style.borderTopRightRadius = 20;
            track.style.borderBottomLeftRadius = 20;
            track.style.borderBottomRightRadius = 20;
            track.style.overflow = Overflow.Hidden;

            // Green zone (85% to 100%) -> Top 15%
            VisualElement targetZone = new VisualElement();
            targetZone.style.position = Position.Absolute;
            targetZone.style.top = 0;
            targetZone.style.width = new Length(100, LengthUnit.Percent);
            targetZone.style.height = new Length(15, LengthUnit.Percent);
            targetZone.style.backgroundColor = new Color(0f, 1f, 0f, 0.3f);
            track.Add(targetZone);

            fdHandles[i] = new VisualElement();
            fdHandles[i].style.width = 40;
            fdHandles[i].style.height = 40;
            fdHandles[i].style.backgroundColor = colors[i];
            fdHandles[i].style.borderTopLeftRadius = 20;
            fdHandles[i].style.borderTopRightRadius = 20;
            fdHandles[i].style.borderBottomLeftRadius = 20;
            fdHandles[i].style.borderBottomRightRadius = 20;
            fdHandles[i].style.position = Position.Absolute;
            fdHandles[i].style.top = 0;
            track.Add(fdHandles[i]);

            // Interaction
            int index = i;
            track.RegisterCallback<PointerMoveEvent>(evt => {
                if(evt.pressedButtons != 0)
                {
                    float localY = evt.localPosition.y;
                    float pct = 1f - Mathf.Clamp01(localY / 200f);
                    fdValues[index] = pct;
                }
            });
            track.RegisterCallback<PointerDownEvent>(evt => {
                float localY = evt.localPosition.y;
                float pct = 1f - Mathf.Clamp01(localY / 200f);
                fdValues[index] = pct;
            });

            sliderCol.Add(track);
            slidersRow.Add(sliderCol);
        }

        formatDashboardContainer.Add(slidersRow);

        VisualElement progressContainer = new VisualElement();
        progressContainer.style.width = 300;
        progressContainer.style.height = 30;
        progressContainer.style.backgroundColor = Color.black;
        progressContainer.style.marginTop = 20;
        progressContainer.style.borderTopWidth = 2;
        progressContainer.style.borderBottomWidth = 2;
        progressContainer.style.borderLeftWidth = 2;
        progressContainer.style.borderRightWidth = 2;
        progressContainer.style.borderTopColor = Color.white;
        progressContainer.style.borderBottomColor = Color.white;
        progressContainer.style.borderLeftColor = Color.white;
        progressContainer.style.borderRightColor = Color.white;

        fdProgressFill = new VisualElement();
        fdProgressFill.style.height = new Length(100, LengthUnit.Percent);
        fdProgressFill.style.width = new Length(0, LengthUnit.Percent);
        fdProgressFill.style.backgroundColor = Color.green;
        progressContainer.Add(fdProgressFill);

        Label holdLabel = new Label("HOLD STEADY!");
        holdLabel.style.position = Position.Absolute;
        holdLabel.style.width = new Length(100, LengthUnit.Percent);
        holdLabel.style.height = new Length(100, LengthUnit.Percent);
        holdLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
        holdLabel.style.fontSize = 20;
        holdLabel.style.color = Color.white;
        progressContainer.Add(holdLabel);

        formatDashboardContainer.Add(progressContainer);

        // Chart bars (decorative)
        VisualElement chartArea = new VisualElement();
        chartArea.style.flexDirection = FlexDirection.Row;
        chartArea.style.height = 100;
        chartArea.style.marginTop = 20;
        chartArea.style.alignItems = Align.FlexEnd;
        chartArea.style.justifyContent = Justify.Center;
        for(int i = 0; i < 4; i++)
        {
            fdBars[i] = new VisualElement();
            fdBars[i].style.width = 30;
            fdBars[i].style.backgroundColor = Color.gray;
            fdBars[i].style.marginLeft = 5;
            fdBars[i].style.marginRight = 5;
            chartArea.Add(fdBars[i]);
        }
        formatDashboardContainer.Add(chartArea);

        activeGameRoutine = StartCoroutine(FormatDashboardRoutine());
    }
    
    private System.Collections.IEnumerator FormatDashboardRoutine()
    {
        isPlaying = true;
        float gravity = 0.40f; 
        
        while (isPlaying)
        {
            timer -= Time.deltaTime;
            
            if (fdTimerText != null) {
                fdTimerText.text = $"{timer:F1}s";
                if (timer <= 3.0f) fdTimerText.style.color = Color.red;
                else fdTimerText.style.color = Color.white;
            }

            bool allGreen = true;

            for(int i = 0; i < 3; i++)
            {
                fdValues[i] -= gravity * Time.deltaTime;
                fdValues[i] = Mathf.Clamp01(fdValues[i]);

                if (fdValues[i] < 0.85f) allGreen = false;

                if (fdHandles[i] != null) {
                    float yOffset = (1f - fdValues[i]) * 160f;
                    fdHandles[i].style.translate = new StyleTranslate(new Translate(0, yOffset, 0));
                    
                    if (fdValues[i] >= 0.85f) fdHandles[i].style.backgroundColor = Color.green;
                    else {
                        if (i == 0) fdHandles[i].style.backgroundColor = Color.blue;
                        else if (i == 1) fdHandles[i].style.backgroundColor = Color.yellow;
                        else fdHandles[i].style.backgroundColor = Color.red;
                    }
                }
            }

            float avg = (fdValues[0] + fdValues[1] + fdValues[2]) / 3f;
            for(int i = 0; i < 4; i++) {
                if (fdBars[i] != null) {
                    float randomJitter = (allGreen ? 0f : Random.Range(-0.1f, 0.1f)) * avg;
                    float baseH = 0.3f + (0.2f * i);
                    fdBars[i].style.height = new StyleLength(new Length(Mathf.Clamp01(baseH + randomJitter) * 100f, LengthUnit.Percent));
                    fdBars[i].style.backgroundColor = allGreen ? Color.green : Color.gray;
                }
            }

            if (allGreen) {
                fdHoldTimer += Time.deltaTime;
            } else {
                fdHoldTimer = Mathf.Max(0, fdHoldTimer - Time.deltaTime * 2f);
            }

            float progressPct = Mathf.Clamp01(fdHoldTimer / 2.0f); // 2 seconds to win
            if (fdProgressFill != null) fdProgressFill.style.width = new StyleLength(new Length(progressPct * 100f, LengthUnit.Percent));

            if (progressPct >= 1f) {
                if (fdTimerText != null) {
                    fdTimerText.text = "POPPIN!";
                    fdTimerText.style.color = Color.green;
                }
                yield return new WaitForSeconds(1.0f);
                EndGame(true);
                yield break;
            }

            if (timer <= 0f) {
                if (fdTimerText != null) fdTimerText.text = "FAILED!";
                yield return new WaitForSeconds(1.5f);
                EndGame(false);
                yield break;
            }
            
            yield return null;
        }
    }

    // --- 7. ZenTracing ---
    private Label ztProgressText;
    private VisualElement ztProgressFill;

    private void SetupZenTracing()
    {
        ztAngle = 0f;
        ztTargetAngle = 0f;
        ztScore = 0f;
        timer = currentConfig.timeLimit > 0 ? currentConfig.timeLimit : 15f;
        
        ztContainer.Clear();
        ztContainer.style.alignItems = Align.Center;
        ztContainer.style.justifyContent = Justify.Center;
        ztContainer.style.backgroundColor = new Color(0.9f, 0.85f, 0.8f); // Warm grey/beige

        Label title = new Label("Suburban Sensory Mud Spa");
        title.style.fontSize = 24;
        title.style.color = new Color(0.4f, 0.2f, 0f); // Brown
        title.style.position = Position.Absolute;
        title.style.top = 20;
        ztContainer.Add(title);

        ztCircle = new VisualElement();
        ztCircle.style.width = 300;
        ztCircle.style.height = 300;
        ztCircle.style.borderTopWidth = 10;
        ztCircle.style.borderBottomWidth = 10;
        ztCircle.style.borderLeftWidth = 10;
        ztCircle.style.borderRightWidth = 10;
        ztCircle.style.borderTopColor = new Color(0.6f, 0.4f, 0.2f); // Warm brown
        ztCircle.style.borderBottomColor = new Color(0.6f, 0.4f, 0.2f);
        ztCircle.style.borderLeftColor = new Color(0.6f, 0.4f, 0.2f);
        ztCircle.style.borderRightColor = new Color(0.6f, 0.4f, 0.2f);
        ztCircle.style.borderTopLeftRadius = 150;
        ztCircle.style.borderTopRightRadius = 150;
        ztCircle.style.borderBottomLeftRadius = 150;
        ztCircle.style.borderBottomRightRadius = 150;
        ztCircle.style.alignItems = Align.Center;
        ztCircle.style.justifyContent = Justify.Center;
        ztContainer.Add(ztCircle);

        ztTarget = new VisualElement();
        ztTarget.style.width = 40;
        ztTarget.style.height = 40;
        ztTarget.style.backgroundColor = new Color(1f, 0.75f, 0f); // Glowing amber
        ztTarget.style.borderTopLeftRadius = 20;
        ztTarget.style.borderTopRightRadius = 20;
        ztTarget.style.borderBottomLeftRadius = 20;
        ztTarget.style.borderBottomRightRadius = 20;
        ztTarget.style.position = Position.Absolute;
        ztCircle.Add(ztTarget);

        ztSponge = new VisualElement();
        ztSponge.style.width = 60;
        ztSponge.style.height = 60;
        ztSponge.style.backgroundColor = new Color(0.4f, 0.2f, 0f); // Mud brown
        ztSponge.style.borderTopLeftRadius = 30;
        ztSponge.style.borderTopRightRadius = 30;
        ztSponge.style.borderBottomLeftRadius = 30;
        ztSponge.style.borderBottomRightRadius = 30;
        ztSponge.style.position = Position.Absolute;
        ztCircle.Add(ztSponge);

        ztContainer.RegisterCallback<PointerMoveEvent>(evt => {
            if (evt.pressedButtons != 0)
            {
                Vector2 localPos = evt.localPosition;
                Vector2 center = new Vector2(ztContainer.layout.width / 2f, ztContainer.layout.height / 2f);
                Vector2 dir = localPos - center;
                ztAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            }
        });
        ztContainer.RegisterCallback<PointerDownEvent>(evt => {
            Vector2 localPos = evt.localPosition;
            Vector2 center = new Vector2(ztContainer.layout.width / 2f, ztContainer.layout.height / 2f);
            Vector2 dir = localPos - center;
            ztAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        });

        VisualElement progressContainer = new VisualElement();
        progressContainer.style.width = 300;
        progressContainer.style.height = 30;
        progressContainer.style.backgroundColor = Color.black;
        progressContainer.style.marginTop = 40;
        progressContainer.style.position = Position.Absolute;
        progressContainer.style.bottom = 40;
        ztContainer.Add(progressContainer);

        ztProgressFill = new VisualElement();
        ztProgressFill.style.height = new Length(100, LengthUnit.Percent);
        ztProgressFill.style.width = new Length(0, LengthUnit.Percent);
        ztProgressFill.style.backgroundColor = Color.green;
        progressContainer.Add(ztProgressFill);

        ztProgressText = new Label("Relaxation: 0%");
        ztProgressText.style.position = Position.Absolute;
        ztProgressText.style.width = new Length(100, LengthUnit.Percent);
        ztProgressText.style.height = new Length(100, LengthUnit.Percent);
        ztProgressText.style.unityTextAlign = TextAnchor.MiddleCenter;
        ztProgressText.style.fontSize = 20;
        ztProgressText.style.color = Color.white;
        progressContainer.Add(ztProgressText);

        activeGameRoutine = StartCoroutine(ZenTracingRoutine());
    }

    private IEnumerator ZenTracingRoutine()
    {
        isPlaying = true;
        float totalTime = timer;
        while (isPlaying)
        {
            timer -= Time.deltaTime;
            
            if (timer <= 0f)
            {
                EndGame(ztScore >= totalTime * 0.7f); // 70% accuracy needed
                yield break;
            }

            ztTargetAngle += 90f * Time.deltaTime; // Move target
            if (ztTargetAngle >= 360f) ztTargetAngle -= 360f;

            if (ztTarget != null)
            {
                float rad = ztTargetAngle * Mathf.Deg2Rad;
                ztTarget.style.left = new Length(50f + Mathf.Cos(rad) * 45f, LengthUnit.Percent);
                ztTarget.style.top = new Length(50f + Mathf.Sin(rad) * 45f, LengthUnit.Percent);
                ztTarget.style.translate = new StyleTranslate(new Translate(-20, -20, 0));
            }
            if (ztSponge != null)
            {
                float rad = ztAngle * Mathf.Deg2Rad;
                ztSponge.style.left = new Length(50f + Mathf.Cos(rad) * 45f, LengthUnit.Percent);
                ztSponge.style.top = new Length(50f + Mathf.Sin(rad) * 45f, LengthUnit.Percent);
                ztSponge.style.translate = new StyleTranslate(new Translate(-30, -30, 0));
            }

            float diff = Mathf.Abs(Mathf.DeltaAngle(ztAngle, ztTargetAngle));
            if (diff < 30f)
            {
                ztScore += Time.deltaTime;
                if (ztSponge != null) ztSponge.style.backgroundColor = new Color(0.6f, 0.8f, 0.2f); // Greenish brown
            }
            else
            {
                if (ztSponge != null) ztSponge.style.backgroundColor = new Color(0.8f, 0.2f, 0.2f); // Reddish brown
            }

            float relaxPct = Mathf.Clamp01(ztScore / (totalTime * 0.7f));
            ztProgressFill.style.width = new StyleLength(new Length(relaxPct * 100f, LengthUnit.Percent));
            ztProgressText.text = $"Relaxation: {Mathf.FloorToInt(relaxPct * 100f)}%";

            yield return null;
        }
    }

    // --- 8. IceShatter ---
    private VisualElement isTempFill;

    private void SetupIceShatter()
    {
        isBodyTemp = 100f;
        timer = currentConfig.timeLimit > 0 ? currentConfig.timeLimit : 15f;
        
        isContainer.Clear();
        isNodes.Clear();

        isContainer.style.backgroundColor = new Color(0.8f, 0.9f, 1f); // icy blue background
        
        isFrostOverlay = new VisualElement();
        isFrostOverlay.style.position = Position.Absolute;
        isFrostOverlay.style.width = new Length(100, LengthUnit.Percent);
        isFrostOverlay.style.height = new Length(100, LengthUnit.Percent);
        isFrostOverlay.style.backgroundColor = new Color(1f, 1f, 1f, 0f);
        isContainer.Add(isFrostOverlay);

        Label title = new Label("Luxury Cryo Therapy");
        title.style.fontSize = 24;
        title.style.color = Color.blue;
        title.style.position = Position.Absolute;
        title.style.top = 20;
        title.style.width = new Length(100, LengthUnit.Percent);
        title.style.unityTextAlign = TextAnchor.UpperCenter;
        isContainer.Add(title);

        VisualElement tempBar = new VisualElement();
        tempBar.style.width = 300;
        tempBar.style.height = 30;
        tempBar.style.backgroundColor = Color.black;
        tempBar.style.position = Position.Absolute;
        tempBar.style.bottom = 40;
        tempBar.style.alignSelf = Align.Center;
        isContainer.Add(tempBar);

        isTempFill = new VisualElement();
        isTempFill.style.width = new Length(100, LengthUnit.Percent);
        isTempFill.style.height = new Length(100, LengthUnit.Percent);
        isTempFill.style.backgroundColor = Color.green;
        tempBar.Add(isTempFill);

        Label tempLabel = new Label("Body Temp");
        tempLabel.style.position = Position.Absolute;
        tempLabel.style.width = new Length(100, LengthUnit.Percent);
        tempLabel.style.height = new Length(100, LengthUnit.Percent);
        tempLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
        tempLabel.style.fontSize = 20;
        tempLabel.style.color = Color.white;
        tempBar.Add(tempLabel);

        activeGameRoutine = StartCoroutine(IceShatterRoutine());
    }

    private IEnumerator IceShatterRoutine()
    {
        isPlaying = true;
        float spawnTimer = 0f;
        while (isPlaying)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                EndGame(true);
                yield break;
            }

            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0f)
            {
                Button node = new Button();
                node.style.width = 70;
                node.style.height = 70;
                node.style.position = Position.Absolute;
                node.style.left = new Length(Random.Range(10f, 85f), LengthUnit.Percent);
                node.style.top = new Length(Random.Range(15f, 80f), LengthUnit.Percent);
                node.style.backgroundColor = new Color(0.2f, 0.8f, 1f, 0.9f);
                node.style.borderTopLeftRadius = 35; node.style.borderTopRightRadius = 35; node.style.borderBottomLeftRadius = 35; node.style.borderBottomRightRadius = 35;
                node.style.borderTopWidth = 3; node.style.borderBottomWidth = 3; node.style.borderLeftWidth = 3; node.style.borderRightWidth = 3;
                node.style.borderTopColor = Color.white; node.style.borderBottomColor = Color.white; node.style.borderLeftColor = Color.white; node.style.borderRightColor = Color.white;
                
                node.clicked += () => {
                    if (!isPlaying) return;
                    isBodyTemp = Mathf.Clamp(isBodyTemp + 8f, 0f, 100f);
                    
                    // visual shatter
                    node.style.backgroundColor = Color.white;
                    node.style.scale = new StyleScale(new Scale(new Vector3(0.5f, 0.5f, 1f)));
                    isNodes.Remove(node);
                    
                    // delayed remove
                    isContainer.schedule.Execute(() => {
                        if (isContainer.Contains(node)) isContainer.Remove(node);
                    }).StartingIn(100);
                };

                isNodes.Add(node);
                isContainer.Add(node);
                
                // Faster as game progresses
                float minTime = Mathf.Lerp(1.5f, 0.3f, 1f - (timer / 15f));
                float maxTime = Mathf.Lerp(2.0f, 0.6f, 1f - (timer / 15f));
                spawnTimer = Random.Range(minTime, maxTime);
            }

            isBodyTemp -= (3f + isNodes.Count * 3f) * Time.deltaTime;
            
            if (isFrostOverlay != null)
            {
                float freezePercent = 1f - (isBodyTemp / 100f);
                isFrostOverlay.style.backgroundColor = new Color(1f, 1f, 1f, freezePercent * 0.8f);
            }

            if (isTempFill != null)
            {
                isTempFill.style.width = new Length(isBodyTemp, LengthUnit.Percent);
                if (isBodyTemp > 60f) isTempFill.style.backgroundColor = Color.green;
                else if (isBodyTemp > 30f) isTempFill.style.backgroundColor = Color.yellow;
                else isTempFill.style.backgroundColor = Color.red;
            }

            if (isBodyTemp <= 0f)
            {
                EndGame(false);
                yield break;
            }

            yield return null;
        }
    }

    // --- 9. NameMemory ---
    private int nmRoundsPlayed = 0;
    private int nmCorrectAnswers = 0;

    private void SetupNameMemory()
    {
        nmRoundsPlayed = 0;
        nmCorrectAnswers = 0;
        
        nmContainer.Clear();
        nmContainer.style.alignItems = Align.Center;
        nmContainer.style.justifyContent = Justify.Center;

        nmQuestionText = new Label();
        nmQuestionText.style.fontSize = 24;
        nmQuestionText.style.color = Color.white;
        nmQuestionText.style.marginBottom = 20;
        nmContainer.Add(nmQuestionText);

        nmVipArea = new VisualElement();
        nmVipArea.style.flexDirection = FlexDirection.Row;
        nmVipArea.style.justifyContent = Justify.Center;
        nmVipArea.style.marginBottom = 20;
        nmContainer.Add(nmVipArea);

        nmAnswerRow = new VisualElement();
        nmAnswerRow.style.flexDirection = FlexDirection.Row;
        nmAnswerRow.style.justifyContent = Justify.Center;
        nmAnswerRow.style.flexWrap = Wrap.Wrap;
        nmContainer.Add(nmAnswerRow);

        activeGameRoutine = StartCoroutine(NameMemoryRoutineSequence());
    }

    private IEnumerator NameMemoryRoutineSequence()
    {
        isPlaying = true;
        
        while (isPlaying && nmRoundsPlayed < 3)
        {
            yield return StartCoroutine(PlayNameMemoryRound());
            if (!isPlaying) yield break; // game ended inside round
            nmRoundsPlayed++;
        }

        if (isPlaying)
        {
            EndGame(nmCorrectAnswers >= 2);
        }
    }

    private IEnumerator PlayNameMemoryRound()
    {
        nmVipArea.Clear();
        nmVips.Clear();
        nmAnswerRow.Clear();
        nmAnswers.Clear();
        nmIsAnsweringPhase = false;

        List<string> selectedVips = new List<string>();
        List<string> pool = new List<string>(currentConfig.vipNames);
        for (int i = 0; i < 4; i++) // Show 4 cards
        {
            if (pool.Count == 0) break;
            int idx = Random.Range(0, pool.Count);
            selectedVips.Add(pool[idx]);
            pool.RemoveAt(idx);
        }

        nmCorrectIndex = Random.Range(0, selectedVips.Count);
        string questionTarget = selectedVips[nmCorrectIndex].Split(' ')[2]; 
        
        nmQuestionText.text = "Memorize the VIPs!";
        
        for (int i = 0; i < selectedVips.Count; i++)
        {
            string dept = selectedVips[i].Split(' ')[2];
            Color bgColor = Color.gray;
            if (dept == "Tech") bgColor = Color.blue;
            else if (dept == "Sales") bgColor = Color.green;
            else if (dept == "HR") bgColor = new Color(0.5f, 0f, 0.5f); // purple
            else if (dept == "Legal") bgColor = new Color(1f, 0.6f, 0f); // orange

            VisualElement vip = new VisualElement();
            vip.style.width = 120;
            vip.style.height = 160;
            vip.style.backgroundColor = bgColor;
            vip.style.borderTopLeftRadius = 10; vip.style.borderTopRightRadius = 10; vip.style.borderBottomLeftRadius = 10; vip.style.borderBottomRightRadius = 10;
            vip.style.justifyContent = Justify.Center;
            vip.style.alignItems = Align.Center;
            vip.style.marginRight = 10;
            vip.style.marginLeft = 10;

            Label nameLbl = new Label(selectedVips[i].Split(' ')[0]);
            nameLbl.style.fontSize = 24;
            nameLbl.style.color = Color.white;
            nameLbl.style.unityFontStyleAndWeight = FontStyle.Bold;
            vip.Add(nameLbl);

            Label deptLbl = new Label(dept);
            deptLbl.style.fontSize = 20;
            deptLbl.style.color = Color.white;
            vip.Add(deptLbl);

            nmVips.Add(vip);
            nmVipArea.Add(vip);
        }

        float memTimer = 5f;
        while (memTimer > 0 && isPlaying)
        {
            memTimer -= Time.deltaTime;
            nmQuestionText.text = $"Memorize the VIPs! {memTimer:F1}s";
            yield return null;
        }
        if (!isPlaying) yield break;

        nmIsAnsweringPhase = true;
        nmQuestionText.text = $"Who was from {questionTarget}?";
        
        foreach (var vip in nmVips) vip.style.display = DisplayStyle.None;
        
        // Shuffle answer indices
        List<int> ansIndices = new List<int>();
        for (int i = 0; i < selectedVips.Count; i++) ansIndices.Add(i);
        for (int i = 0; i < ansIndices.Count; i++)
        {
            int temp = ansIndices[i];
            int rnd = Random.Range(i, ansIndices.Count);
            ansIndices[i] = ansIndices[rnd];
            ansIndices[rnd] = temp;
        }

        bool answered = false;

        foreach (int idx in ansIndices)
        {
            Button ansBtn = new Button();
            ansBtn.text = selectedVips[idx].Split(' ')[0];
            ansBtn.style.fontSize = 22;
            ansBtn.style.width = 150;
            ansBtn.style.height = 60;
            ansBtn.style.marginRight = 10;
            ansBtn.style.marginLeft = 10;
            ansBtn.style.marginTop = 10;
            ansBtn.style.backgroundColor = Color.gray;
            ansBtn.style.color = Color.white;
            
            int capturedIdx = idx;
            ansBtn.clicked += () => {
                if (!isPlaying || !nmIsAnsweringPhase || answered) return;
                answered = true;
                if (capturedIdx == nmCorrectIndex)
                {
                    ansBtn.style.backgroundColor = Color.green;
                    nmCorrectAnswers++;
                }
                else
                {
                    ansBtn.style.backgroundColor = Color.red;
                }
            };
            nmAnswers.Add(ansBtn);
            nmAnswerRow.Add(ansBtn);
        }

        float ansTimer = 10f;
        while (ansTimer > 0 && isPlaying && !answered)
        {
            ansTimer -= Time.deltaTime;
            yield return null;
        }
        
        if (!isPlaying) yield break;
        if (!answered) 
        {
            // Time out on question = wrong
        }
        
        // Brief pause to show result
        yield return new WaitForSeconds(1f);
    }

    // --- 10. CorporateGolf ---
    private int cgRoundsPlayed = 0;
    private int cgPerfectRounds = 0;
    private Label cgResultLabel;
    
    private void SetupCorporateGolf()
    {
        cgRoundsPlayed = 0;
        cgPerfectRounds = 0;
        
        cgContainer.Clear();
        cgContainer.style.alignItems = Align.Center;
        cgContainer.style.justifyContent = Justify.Center;
        
        Label title = new Label("Corporate Golf Sim");
        title.style.fontSize = 26;
        title.style.color = Color.white;
        title.style.marginBottom = 20;
        cgContainer.Add(title);
        
        VisualElement playArea = new VisualElement();
        playArea.style.flexDirection = FlexDirection.Row;
        playArea.style.alignItems = Align.Center;
        cgContainer.Add(playArea);
        
        VisualElement bar = new VisualElement();
        bar.style.width = 60;
        bar.style.height = 400;
        bar.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f);
        bar.style.borderTopLeftRadius = 10; bar.style.borderTopRightRadius = 10; bar.style.borderBottomLeftRadius = 10; bar.style.borderBottomRightRadius = 10;
        bar.style.marginRight = 40;
        bar.style.overflow = Overflow.Hidden;
        playArea.Add(bar);
        
        cgSnapZone = new VisualElement();
        cgSnapZone.style.width = new Length(100, LengthUnit.Percent);
        cgSnapZone.style.height = new Length(20, LengthUnit.Percent); // 20% size
        cgSnapZone.style.position = Position.Absolute;
        cgSnapZone.style.bottom = new Length(70, LengthUnit.Percent); // 70-90% range
        cgSnapZone.style.backgroundColor = Color.green;
        bar.Add(cgSnapZone);
        
        cgMarker = new VisualElement();
        cgMarker.style.width = new Length(100, LengthUnit.Percent);
        cgMarker.style.height = 10;
        cgMarker.style.backgroundColor = Color.white;
        cgMarker.style.position = Position.Absolute;
        cgMarker.style.bottom = 0;
        bar.Add(cgMarker);
        
        VisualElement controls = new VisualElement();
        controls.style.alignItems = Align.Center;
        playArea.Add(controls);

        cgResultLabel = new Label("Get ready!");
        cgResultLabel.style.fontSize = 24;
        cgResultLabel.style.color = Color.yellow;
        cgResultLabel.style.marginBottom = 20;
        controls.Add(cgResultLabel);

        cgBtnSwing = new Button();
        cgBtnSwing.text = "SWING!";
        cgBtnSwing.style.fontSize = 24;
        cgBtnSwing.style.width = 150;
        cgBtnSwing.style.height = 60;
        cgBtnSwing.style.backgroundColor = Color.blue;
        cgBtnSwing.style.color = Color.white;
        cgBtnSwing.clicked += () => {
            if (!isPlaying || cgClicks > 0) return;
            cgClicks = 1; // Mark as swung
        };
        controls.Add(cgBtnSwing);
        
        activeGameRoutine = StartCoroutine(CorporateGolfRoutineSequence());
    }

    private IEnumerator CorporateGolfRoutineSequence()
    {
        isPlaying = true;
        
        while (isPlaying && cgRoundsPlayed < 3)
        {
            yield return StartCoroutine(CorporateGolfRound());
            if (!isPlaying) yield break;
            cgRoundsPlayed++;
        }

        if (isPlaying)
        {
            EndGame(cgPerfectRounds >= 2);
        }
    }

    private IEnumerator CorporateGolfRound()
    {
        cgClicks = 0;
        cgPower = 0f;
        cgMovingUp = true;
        cgMarker.style.bottom = 0;
        cgBtnSwing.style.backgroundColor = Color.blue;
        cgResultLabel.text = $"Round {cgRoundsPlayed + 1}/3";

        float speed = 1.2f + (cgRoundsPlayed * 0.3f); // gets faster

        while (isPlaying && cgClicks == 0)
        {
            if (cgMovingUp)
            {
                cgPower += Time.deltaTime * speed;
                if (cgPower >= 1f) { cgPower = 1f; cgMovingUp = false; }
            }
            else
            {
                cgPower -= Time.deltaTime * speed;
                if (cgPower <= 0f) { cgPower = 0f; cgMovingUp = true; }
            }
            
            cgMarker.style.bottom = new Length(cgPower * 100f, LengthUnit.Percent);
            yield return null;
        }

        if (!isPlaying) yield break;

        // Swung
        if (cgPower >= 0.70f && cgPower <= 0.90f)
        {
            cgResultLabel.text = "PERFECT!";
            cgResultLabel.style.color = Color.green;
            cgBtnSwing.style.backgroundColor = Color.green;
            cgPerfectRounds++;
        }
        else if (cgPower >= 0.60f && cgPower <= 1.00f)
        {
            cgResultLabel.text = "GOOD";
            cgResultLabel.style.color = Color.yellow;
            cgBtnSwing.style.backgroundColor = Color.yellow;
        }
        else
        {
            cgResultLabel.text = "SHANKED!";
            cgResultLabel.style.color = Color.red;
            cgBtnSwing.style.backgroundColor = Color.red;
        }

        yield return new WaitForSeconds(1.5f);
    }


    // ==========================================
    // 11. MowGreens (Mow Corporate Astroturf)
    // ==========================================
    private Label mgProgressLabel;

    private void SetupMowGreens()
    {
        timer = currentConfig.timeLimit > 0 ? currentConfig.timeLimit : 15f;
        mgTilesCut = 0;
        mgCutStatus = new bool[25];
        
        if (mgContainer != null)
        {
            mgContainer.Clear();

            mgProgressLabel = new Label("MOWED: 0/25");
            mgProgressLabel.style.color = new StyleColor(Color.white);
            mgProgressLabel.style.fontSize = 24;
            mgProgressLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            mgProgressLabel.style.marginBottom = 10;
            mgContainer.Add(mgProgressLabel);

            mgGrid = new VisualElement();
            mgGrid.style.flexDirection = FlexDirection.Row;
            mgGrid.style.flexWrap = Wrap.Wrap;
            mgGrid.style.width = 300;
            mgGrid.style.height = 300;
            mgGrid.style.alignSelf = Align.Center;
            mgContainer.Add(mgGrid);
            
            for (int i = 0; i < 25; i++)
            {
                VisualElement tile = new VisualElement();
                tile.style.width = 58;
                tile.style.height = 58;
                tile.style.marginTop = 1;
                tile.style.marginBottom = 1;
                tile.style.marginLeft = 1;
                tile.style.marginRight = 1;
                tile.style.backgroundColor = new StyleColor(Color.green);
                int idx = i;
                
                System.Action cutGrass = () => {
                    if (isPlaying && !mgCutStatus[idx])
                    {
                        mgCutStatus[idx] = true;
                        tile.style.backgroundColor = new StyleColor(new Color(0.4f, 0.25f, 0.1f)); // Brown
                        tile.style.borderTopColor = new StyleColor(new Color(0.3f, 0.2f, 0.05f)); // Subtle stripe
                        tile.style.borderTopWidth = 5;
                        mgTilesCut++;
                        mgProgressLabel.text = $"MOWED: {mgTilesCut}/25";
                        if (mgTilesCut >= 25) EndGame(true);
                    }
                };

                tile.RegisterCallback<PointerEnterEvent>(ev => {
                    if ((ev.pressedButtons & 1) != 0) cutGrass();
                });
                tile.RegisterCallback<ClickEvent>(ev => {
                    cutGrass();
                });
                mgGrid.Add(tile);
            }
        }
        activeGameRoutine = StartCoroutine(MowGreensRoutine());
    }

    private IEnumerator MowGreensRoutine()
    {
        isPlaying = true;
        while (isPlaying)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                EndGame(false);
                yield break;
            }
            yield return null;
        }
    }

    // ==========================================
    // 12. WalkDogs (Walk Purebred Pedigree Dogs)
    // ==========================================
    private void SetupWalkDogs()
    {
        timer = currentConfig.timeLimit > 0 ? currentConfig.timeLimit : 15f;
        
        if (wdContainer != null)
        {
            wdContainer.Clear();
            VisualElement row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.justifyContent = Justify.SpaceAround;
            row.style.width = new Length(100, LengthUnit.Percent);
            row.style.height = 300;
            
            string[] dogNames = { "🐕 Rex", "🐩 Bella", "🐕🦺 Max" };

            for (int i = 0; i < 3; i++)
            {
                VisualElement col = new VisualElement();
                col.style.width = 80;
                col.style.height = 300;
                col.style.flexDirection = FlexDirection.Column;
                col.style.justifyContent = Justify.FlexEnd;
                col.style.alignItems = Align.Center;

                Label nameLbl = new Label(dogNames[i]);
                nameLbl.style.color = new StyleColor(Color.white);
                nameLbl.style.fontSize = 18;
                nameLbl.style.marginBottom = 10;
                
                VisualElement bg = new VisualElement();
                bg.style.width = 40;
                bg.style.height = 150;
                bg.style.backgroundColor = new StyleColor(new Color(0.2f, 0.2f, 0.2f));
                bg.style.justifyContent = Justify.FlexEnd;
                bg.style.marginBottom = 10;
                
                VisualElement bar = new VisualElement();
                bar.style.width = new Length(100, LengthUnit.Percent);
                bar.style.height = new Length(50, LengthUnit.Percent);
                bar.style.backgroundColor = new StyleColor(Color.green);
                bg.Add(bar);
                
                Button yank = new Button();
                yank.text = "YANK";
                yank.style.height = 40;
                yank.style.width = 70;
                yank.style.fontSize = 18;
                
                int idx = i;
                wdTensions[i] = 0.5f; // Start at 50%
                wdBars[i] = bar;
                wdBtns[i] = yank;
                
                yank.RegisterCallback<PointerDownEvent>(ev => { 
                    if(isPlaying) 
                    {
                        wdTensions[idx] -= 0.30f; 
                        if (wdTensions[idx] < 0.05f) wdTensions[idx] = 0.05f; // 5% floor
                    }
                });
                
                col.Add(nameLbl);
                col.Add(bg);
                col.Add(yank);
                row.Add(col);
            }
            wdContainer.Add(row);
        }
        activeGameRoutine = StartCoroutine(WalkDogsRoutine());
    }

    private IEnumerator WalkDogsRoutine()
    {
        isPlaying = true;
        while (isPlaying)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                EndGame(true); // Survived
                yield break;
            }
            
            for (int i = 0; i < 3; i++)
            {
                wdTensions[i] += UnityEngine.Random.Range(0.08f, 0.18f) * Time.deltaTime;
                
                if (wdBars[i] != null)
                {
                    wdBars[i].style.height = new Length(wdTensions[i] * 100f, LengthUnit.Percent);
                    if (wdTensions[i] < 0.5f) wdBars[i].style.backgroundColor = new StyleColor(Color.green);
                    else if (wdTensions[i] < 0.8f) wdBars[i].style.backgroundColor = new StyleColor(Color.yellow);
                    else wdBars[i].style.backgroundColor = new StyleColor(Color.red);
                }
                
                if (wdTensions[i] >= 1f)
                {
                    EndGame(false); // Leash snaps
                    yield break;
                }
            }
            yield return null;
        }
    }

    // ==========================================
    // 13. RetroRacer (Deliver Fast Food Order)
    // ==========================================

    private void SetupRetroRacer()
    {
        timer = currentConfig.timeLimit > 0 ? currentConfig.timeLimit : 20f;
        rrBlocks = 0;
        rrDistanceTimer = 0f;
        rrCurrentLane = 1;
        
        if (rrDistanceLabel != null) rrDistanceLabel.text = "DISTANCE: 0";
        if (rrTimeLabel != null) rrTimeLabel.text = $"TIME: {UnityEngine.Mathf.CeilToInt(timer)}";
        
        for (int i = 0; i < 8; i++) {
            rrObstacleActive[i] = false;
            if (rrObstacleElements[i] != null) rrObstacleElements[i].style.display = UnityEngine.UIElements.DisplayStyle.None;
        }

        activeGameRoutine = StartCoroutine(RetroRacerRoutine());
    }

    private System.Collections.IEnumerator RetroRacerRoutine()
    {
        isPlaying = true;
        float spawnTimer = 0f;
        float[] laneTargets = { 15f, 50f, 85f }; // left, center, right percentages at bottom of screen
        
        float[] lineProgress = new float[8];
        for(int i=0; i<8; i++) lineProgress[i] = (float)i / 8f; // Staggered progress
        
        float speed = 0.5f;

        while (isPlaying)
        {
            timer -= UnityEngine.Time.deltaTime;
            if (timer <= 0f)
            {
                MarkWinReady();
                yield break;
            }
            if (rrTimeLabel != null) rrTimeLabel.text = $"TIME: {UnityEngine.Mathf.CeilToInt(timer)}";

            speed += UnityEngine.Time.deltaTime * 0.05f; // Accelerate over time
            
            rrDistanceTimer += UnityEngine.Time.deltaTime * speed;
            if (rrDistanceTimer > 0.1f)
            {
                rrDistanceTimer = 0f;
                rrBlocks++;
                if (rrDistanceLabel != null) rrDistanceLabel.text = $"DISTANCE: {rrBlocks}";
            }

            // Animate Road Lines (Perspective V-shape)
            for (int i = 0; i < 8; i++) {
                lineProgress[i] += UnityEngine.Time.deltaTime * speed * 2f;
                if (lineProgress[i] > 1f) lineProgress[i] -= 1f;
                
                if (rrRoadLines[i] != null) {
                    float y = UnityEngine.Mathf.Pow(lineProgress[i], 2f); // Non-linear depth
                    rrRoadLines[i].style.top = new UnityEngine.UIElements.Length(y * 100f, UnityEngine.UIElements.LengthUnit.Percent);
                    
                    float w = UnityEngine.Mathf.Lerp(10f, 800f, y);
                    rrRoadLines[i].style.width = w;
                    rrRoadLines[i].style.left = new UnityEngine.UIElements.Length(50f, UnityEngine.UIElements.LengthUnit.Percent);
                    rrRoadLines[i].style.marginLeft = -w/2f;
                    
                    float h = UnityEngine.Mathf.Lerp(1f, 15f, y);
                    rrRoadLines[i].style.height = h;
                    
                    // Color fade in distance
                    rrRoadLines[i].style.backgroundColor = new UnityEngine.UIElements.StyleColor(new UnityEngine.Color(1f, 1f, 1f, y));
                }
            }

            // Smoothly move player to current lane (Player is always at bottom)
            if (rrPlayer != null)
            {
                float targetLeft = laneTargets[rrCurrentLane];
                float currentLeft = rrPlayer.style.left.value.value;
                float newLeft = UnityEngine.Mathf.Lerp(currentLeft, targetLeft, UnityEngine.Time.deltaTime * 10f);
                rrPlayer.style.left = new UnityEngine.UIElements.Length(newLeft, UnityEngine.UIElements.LengthUnit.Percent);
            }

            // Spawn Obstacles
            spawnTimer -= UnityEngine.Time.deltaTime;
            if (spawnTimer <= 0f)
            {
                for (int i = 0; i < 8; i++) {
                    if (!rrObstacleActive[i]) {
                        rrObstacleActive[i] = true;
                        int obsLane = UnityEngine.Random.Range(0, 3);
                        rrObstaclePos[i] = new UnityEngine.Vector2(obsLane, 0f);
                        if (rrObstacleElements[i] != null) {
                            rrObstacleElements[i].style.display = UnityEngine.UIElements.DisplayStyle.Flex;
                            rrObstacleElements[i].text = UnityEngine.Random.value > 0.5f ? "🚕" : (UnityEngine.Random.value > 0.5f ? "🚧" : "🛢️");
                        }
                        break;
                    }
                }
                spawnTimer = UnityEngine.Random.Range(0.4f, 0.8f) / speed;
            }

            // Move Obstacles
            for (int i = 0; i < 8; i++)
            {
                if (!rrObstacleActive[i]) continue;

                UnityEngine.Vector2 pos = rrObstaclePos[i];
                pos.y += UnityEngine.Time.deltaTime * speed * 0.8f; // Linear progress
                rrObstaclePos[i] = pos;

                if (pos.y >= 1f)
                {
                    // Collision Check
                    if ((int)pos.x == rrCurrentLane)
                    {
                        AbortGame(); // Hit obstacle
                        yield break;
                    }

                    rrObstacleActive[i] = false;
                    if (rrObstacleElements[i] != null) rrObstacleElements[i].style.display = UnityEngine.UIElements.DisplayStyle.None;
                }
                else
                {
                    // 3D Projection
                    float screenY = UnityEngine.Mathf.Pow(pos.y, 2f); // Non-linear depth mapping
                    
                    float laneTargetX = laneTargets[(int)pos.x];
                    float currentX = UnityEngine.Mathf.Lerp(50f, laneTargetX, screenY);
                    
                    if (rrObstacleElements[i] != null) {
                        rrObstacleElements[i].style.left = new UnityEngine.UIElements.Length(currentX, UnityEngine.UIElements.LengthUnit.Percent);
                        rrObstacleElements[i].style.top = new UnityEngine.UIElements.Length(screenY * 100f, UnityEngine.UIElements.LengthUnit.Percent);
                        
                        float scale = UnityEngine.Mathf.Lerp(0.1f, 3.5f, screenY);
                        rrObstacleElements[i].style.scale = new UnityEngine.UIElements.StyleScale(new UnityEngine.UIElements.Scale(new UnityEngine.Vector3(scale, scale, 1f)));
                    }
                }
            }
            
            yield return null;
        }
    }

    // ==========================================
    // 14. SkimATM (Skim ATM Card Data)
    // ==========================================
    private Label saTimerLabel;
    private Label saConnectionLabel;
    
    private void SetupSkimATM()
    {
        timer = currentConfig.timeLimit > 0 ? currentConfig.timeLimit : 10f;
        saConnections = 0;
        saActiveNode = null;
        
        if (saContainer != null)
        {
            saTimerLabel = saContainer.Q<Label>("SaTimerText");
            saConnectionLabel = saContainer.Q<Label>("SaConnectionText");

            if (saTimerLabel != null) saTimerLabel.text = $"TIME: {timer:F1}s";
            if (saConnectionLabel != null) saConnectionLabel.text = "LINKS: 0/4";

            VisualElement leftCol = saContainer.Q<VisualElement>("SaLeftPanel");
            VisualElement rightCol = saContainer.Q<VisualElement>("SaRightPanel");

            if (leftCol != null) leftCol.Clear();
            if (rightCol != null) rightCol.Clear();
            
            saLeftNodes.Clear();
            saRightNodes.Clear();
            
            Color[] colors = { Color.red, Color.blue, Color.green, Color.yellow };
            Color[] rightColors = { Color.red, Color.blue, Color.green, Color.yellow };
            
            // Shuffle rightColors
            for(int i = 0; i < rightColors.Length; i++) {
                int r = UnityEngine.Random.Range(i, rightColors.Length);
                Color temp = rightColors[i];
                rightColors[i] = rightColors[r];
                rightColors[r] = temp;
            }
            
            for (int i = 0; i < 4; i++)
            {
                Button lBtn = new Button();
                lBtn.style.width = 50; lBtn.style.height = 50; 
                lBtn.style.marginTop = 10; lBtn.style.marginBottom = 10; lBtn.style.marginLeft = 10; lBtn.style.marginRight = 10;
                lBtn.style.backgroundColor = new StyleColor(colors[i]);
                lBtn.style.borderTopWidth = 3; lBtn.style.borderBottomWidth = 3; lBtn.style.borderLeftWidth = 3; lBtn.style.borderRightWidth = 3;
                lBtn.style.borderTopColor = new StyleColor(Color.clear); lBtn.style.borderBottomColor = new StyleColor(Color.clear); 
                lBtn.style.borderLeftColor = new StyleColor(Color.clear); lBtn.style.borderRightColor = new StyleColor(Color.clear);
                lBtn.style.borderTopLeftRadius = 8; lBtn.style.borderTopRightRadius = 8; lBtn.style.borderBottomLeftRadius = 8; lBtn.style.borderBottomRightRadius = 8;
                
                lBtn.clicked += () => { 
                    if(isPlaying && lBtn.enabledSelf) 
                    {
                        if (saActiveNode != null) saActiveNode.style.borderTopColor = new StyleColor(Color.clear);
                        saActiveNode = lBtn; 
                        saActiveNode.style.borderTopColor = new StyleColor(Color.white);
                        saActiveNode.style.borderBottomColor = new StyleColor(Color.white);
                        saActiveNode.style.borderLeftColor = new StyleColor(Color.white);
                        saActiveNode.style.borderRightColor = new StyleColor(Color.white);
                    }
                };
                saLeftNodes.Add(lBtn);
                if (leftCol != null) leftCol.Add(lBtn);
                
                Button rBtn = new Button();
                rBtn.style.width = 50; rBtn.style.height = 50; 
                rBtn.style.marginTop = 10; rBtn.style.marginBottom = 10; rBtn.style.marginLeft = 10; rBtn.style.marginRight = 10;
                rBtn.style.backgroundColor = new StyleColor(rightColors[i]);
                rBtn.style.borderTopWidth = 3; rBtn.style.borderBottomWidth = 3; rBtn.style.borderLeftWidth = 3; rBtn.style.borderRightWidth = 3;
                rBtn.style.borderTopLeftRadius = 8; rBtn.style.borderTopRightRadius = 8; rBtn.style.borderBottomLeftRadius = 8; rBtn.style.borderBottomRightRadius = 8;
                
                rBtn.clicked += () => {
                    if (isPlaying && saActiveNode != null && rBtn.enabledSelf)
                    {
                        if (saActiveNode.style.backgroundColor.value == rBtn.style.backgroundColor.value)
                        {
                            saActiveNode.enabledSelf = false;
                            rBtn.enabledSelf = false;
                            
                            saActiveNode.style.backgroundColor = new StyleColor(Color.gray);
                            saActiveNode.text = "✓";
                            saActiveNode.style.color = new StyleColor(Color.white);
                            saActiveNode.style.fontSize = 20;
                            
                            rBtn.style.backgroundColor = new StyleColor(Color.gray);
                            rBtn.text = "✓";
                            rBtn.style.color = new StyleColor(Color.white);
                            rBtn.style.fontSize = 20;

                            saConnections++;
                            if (saConnectionLabel != null) saConnectionLabel.text = $"LINKS: {saConnections}/4";
                            
                            saActiveNode.style.borderTopColor = new StyleColor(Color.clear);
                            saActiveNode.style.borderBottomColor = new StyleColor(Color.clear);
                            saActiveNode.style.borderLeftColor = new StyleColor(Color.clear);
                            saActiveNode.style.borderRightColor = new StyleColor(Color.clear);
                            saActiveNode = null;
                            
                            if (saConnections >= 4) EndGame(true);
                        }
                        else
                        {
                            // Penalty
                            timer -= 2f;
                            saActiveNode.style.borderTopColor = new StyleColor(Color.clear);
                            saActiveNode.style.borderBottomColor = new StyleColor(Color.clear);
                            saActiveNode.style.borderLeftColor = new StyleColor(Color.clear);
                            saActiveNode.style.borderRightColor = new StyleColor(Color.clear);
                            saActiveNode = null;
                        }
                    }
                };
                saRightNodes.Add(rBtn);
                if (rightCol != null) rightCol.Add(rBtn);
            }
        }
        activeGameRoutine = StartCoroutine(SkimATMRoutine());
    }

    private IEnumerator SkimATMRoutine()
    {
        isPlaying = true;
        while (isPlaying)
        {
            timer -= Time.deltaTime;
            if (saTimerLabel != null) saTimerLabel.text = $"TIME: {Mathf.Max(0, timer):F1}s";
            
            if (timer <= 0f)
            {
                EndGame(false);
                yield break;
            }
            yield return null;
        }
    }

    // ==========================================
    // 15. TwoPhaseHeist (Corporate Espionage Heist)
    // ==========================================
    private void SetupTwoPhaseHeist()
    {
        if (tphContainer != null)
        {
            tphContainer.Clear();
            Label lbl = new Label("HEIST INITIATED\nPHASE 1: BREAK IN");
            lbl.style.color = new StyleColor(Color.white);
            lbl.style.fontSize = 24;
            lbl.style.unityTextAlign = TextAnchor.MiddleCenter;
            tphContainer.Add(lbl);
        }
        
        activeGameRoutine = StartCoroutine(TwoPhaseHeistFlashRoutine());
    }

    private IEnumerator TwoPhaseHeistFlashRoutine()
    {
        isPlaying = true;
        float flashTimer = 1.5f;
        while (isPlaying)
        {
            flashTimer -= Time.deltaTime;
            if (flashTimer <= 0f)
            {
                isPlaying = false;
                HideAllContainers();
                SetupGameType(currentConfig.phase1Type);
                yield break;
            }
            yield return null;
        }
    }

}

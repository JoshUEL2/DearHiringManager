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

    private void SetupZenTracing()
    {
        ztAngle = 0f;
        ztTargetAngle = 0f;
        ztScore = 0f;
        timer = currentConfig.timeLimit > 0 ? currentConfig.timeLimit : 15f;
        
        ztProgressFill.style.width = new Length(0, LengthUnit.Percent);
        ztProgressText.text = "RELAXATION: 0%";

        ztContainer.RegisterCallback<PointerMoveEvent>(evt => {
            if (evt.pressedButtons != 0)
            {
                Vector2 localPos = evt.localPosition;
                // Offset calculation relative to the ZtCircle element inside ZtContainer
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
                ztTarget.style.translate = new StyleTranslate(new Translate(-22, -22, 0));
            }
            if (ztSponge != null)
            {
                float rad = ztAngle * Mathf.Deg2Rad;
                ztSponge.style.left = new Length(50f + Mathf.Cos(rad) * 45f, LengthUnit.Percent);
                ztSponge.style.top = new Length(50f + Mathf.Sin(rad) * 45f, LengthUnit.Percent);
                ztSponge.style.translate = new StyleTranslate(new Translate(-28, -28, 0));
            }

            float diff = Mathf.Abs(Mathf.DeltaAngle(ztAngle, ztTargetAngle));
            if (diff < 30f)
            {
                ztScore += Time.deltaTime;
                if (ztSponge != null) ztSponge.style.backgroundColor = new Color(0.2f, 0.8f, 0.2f, 0.8f); // Greenish success
            }
            else
            {
                if (ztSponge != null) ztSponge.style.backgroundColor = new Color(0.9f, 0.2f, 0.2f, 0.8f); // Reddish fail
            }

            float relaxPct = Mathf.Clamp01(ztScore / (totalTime * 0.7f));
            ztProgressFill.style.width = new StyleLength(new Length(relaxPct * 100f, LengthUnit.Percent));
            ztProgressText.text = $"Relaxation: {Mathf.FloorToInt(relaxPct * 100f)}%";

            yield return null;
        }
    }

    // --- 8. IceShatter ---

    private void SetupIceShatter()
    {
        isBodyTemp = 100f;
        timer = currentConfig.timeLimit > 0 ? currentConfig.timeLimit : 15f;
        
        isNodes.Clear();
        isNodeArea.Clear();

        if (isFrostOverlay != null)
            isFrostOverlay.style.backgroundColor = new Color(1f, 1f, 1f, 0f);

        activeGameRoutine = StartCoroutine(IceShatterRoutine());
    }

    private IEnumerator IceShatterRoutine()
    {
        isPlaying = true;
        float spawnTimer = 0f;
        while (isPlaying)
        {
            timer -= Time.deltaTime;
            
            if (isTimerText != null)
                isTimerText.text = $"TIME: {Mathf.Max(0, timer):F1}s";
                
            if (timer <= 0f)
            {
                EndGame(true);
                yield break;
            }

            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0f)
            {
                int spawnCount = Random.Range(2, 4);
                for (int i = 0; i < spawnCount; i++)
                {
                    Button node = new Button();
                    node.style.width = 60;
                    node.style.height = 60;
                    node.style.position = Position.Absolute;
                    node.style.left = new Length(Random.Range(10f, 85f), LengthUnit.Percent);
                    node.style.top = new Length(Random.Range(15f, 75f), LengthUnit.Percent);
                    
                    // High-tech frosted valve look
                    node.style.backgroundColor = new Color(0.2f, 0.8f, 1f, 0.9f);
                    node.style.borderTopLeftRadius = 30; node.style.borderTopRightRadius = 30; node.style.borderBottomLeftRadius = 30; node.style.borderBottomRightRadius = 30;
                    node.style.borderTopWidth = 4; node.style.borderBottomWidth = 4; node.style.borderLeftWidth = 4; node.style.borderRightWidth = 4;
                    node.style.borderTopColor = new Color(0.8f, 0.9f, 1f); 
                    node.style.borderBottomColor = new Color(0.8f, 0.9f, 1f);
                    node.style.borderLeftColor = new Color(0.8f, 0.9f, 1f);
                    node.style.borderRightColor = new Color(0.8f, 0.9f, 1f);
                    
                    node.text = "❄️";
                    node.style.fontSize = 24;
                    node.style.color = Color.white;
                    node.style.unityTextAlign = TextAnchor.MiddleCenter;
                    
                    node.clicked += () => {
                        if (!isPlaying) return;
                        isBodyTemp = Mathf.Clamp(isBodyTemp + 12f, 0f, 100f);
                        
                        // visual shatter
                        node.style.backgroundColor = Color.white;
                        node.text = ""; // clear emoji
                        node.style.scale = new StyleScale(new Scale(new Vector3(0.3f, 0.3f, 1f)));
                        isNodes.Remove(node);
                        
                        // delayed remove
                        isNodeArea.schedule.Execute(() => {
                            if (isNodeArea.Contains(node)) isNodeArea.Remove(node);
                        }).StartingIn(100);
                    };

                    isNodes.Add(node);
                    isNodeArea.Add(node);
                }
                
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
        
        nmTimeLabel = nmContainer.Q<Label>("NmTimeText");
        nmTableText = nmContainer.Q<Label>("NmTableText");
        nmTableTitle = nmContainer.Q<Label>("NmTableTitle");

        for (int i = 0; i < 4; i++)
        {
            nmSeats[i] = nmContainer.Q<VisualElement>($"NmSeat{i}");
            nmPlates[i] = nmContainer.Q<VisualElement>($"NmPlate{i}");
            nmCards[i] = nmContainer.Q<Label>($"NmCard{i}");
            nmEmojis[i] = nmContainer.Q<Label>($"NmEmoji{i}");
            
            // Clear past callbacks
            nmSeats[i].UnregisterCallback<PointerDownEvent>(OnSeatClicked);
            nmSeats[i].RegisterCallback<PointerDownEvent>(OnSeatClicked);
        }

        activeGameRoutine = StartCoroutine(NameMemoryRoutineSequence());
    }

    private void OnSeatClicked(PointerDownEvent ev)
    {
        if (!isPlaying || !nmIsAnsweringPhase) return;
        VisualElement seat = ev.currentTarget as VisualElement;
        int seatIndex = -1;
        if (seat.name == "NmSeat0") seatIndex = 0;
        else if (seat.name == "NmSeat1") seatIndex = 1;
        else if (seat.name == "NmSeat2") seatIndex = 2;
        else if (seat.name == "NmSeat3") seatIndex = 3;
        
        if (seatIndex == -1) return;
        
        nmIsAnsweringPhase = false; // block further clicks

        if (seatIndex == nmCorrectIndex)
        {
            nmPlates[seatIndex].style.backgroundColor = new Color(0.18f, 0.8f, 0.44f); // Green
            nmCorrectAnswers++;
        }
        else
        {
            nmPlates[seatIndex].style.backgroundColor = new Color(0.9f, 0.29f, 0.23f); // Red
            if (nmCorrectIndex >= 0)
            {
                nmPlates[nmCorrectIndex].style.backgroundColor = new Color(0.18f, 0.8f, 0.44f);
            }
            AbortGame(); // Strict fail condition!
        }
    }

    private IEnumerator NameMemoryRoutineSequence()
    {
        isPlaying = true;
        
        List<int> availableTableNums = new List<int>();
        for (int i = 1; i <= 10; i++) availableTableNums.Add(i);
        
        // Shuffle table numbers
        for (int i = 0; i < availableTableNums.Count; i++)
        {
            int temp = availableTableNums[i];
            int rnd = Random.Range(i, availableTableNums.Count);
            availableTableNums[i] = availableTableNums[rnd];
            availableTableNums[rnd] = temp;
        }
        
        while (isPlaying && nmRoundsPlayed < 3)
        {
            if (nmTableTitle != null) nmTableTitle.text = $"TABLE {availableTableNums[nmRoundsPlayed]}";
            
            yield return StartCoroutine(PlayNameMemoryRound());
            if (!isPlaying) yield break; // game ended inside round
            nmRoundsPlayed++;
        }

        if (isPlaying)
        {
            EndGame(nmCorrectAnswers == 3);
        }
    }

    private IEnumerator PlayNameMemoryRound()
    {
        nmIsAnsweringPhase = false;
        
        // Reset plate colors
        for (int i = 0; i < 4; i++)
        {
            nmPlates[i].style.backgroundColor = new Color(0.95f, 0.96f, 0.96f); // #F3F4F6
        }

        List<string> selectedVips = new List<string>();
        List<string> pool = new List<string>(currentConfig.vipNames);
        
        for (int i = 0; i < 4; i++) // Show 4 seats
        {
            if (pool.Count == 0) break;
            int idx = Random.Range(0, pool.Count);
            selectedVips.Add(pool[idx]);
            pool.RemoveAt(idx);
            
            // Set UI with proper formatting "Sarah from Tech" -> "Sarah (Tech)"
            string formattedName = selectedVips[i];
            if (formattedName.Contains(" from ")) {
                formattedName = formattedName.Replace(" from ", " (") + ")";
            }
            
            nmCards[i].text = formattedName;
            nmCards[i].style.display = DisplayStyle.Flex; // Show namecards
        }

        nmCorrectIndex = Random.Range(0, selectedVips.Count);
        string questionTarget = selectedVips[nmCorrectIndex]; 
        if (questionTarget.Contains(" from ")) {
            questionTarget = questionTarget.Replace(" from ", " (") + ")";
        }
        
        nmTableText.text = "MEMORIZE SEATS!";
        
        float memTimer = 4f;
        while (memTimer > 0 && isPlaying)
        {
            memTimer -= Time.deltaTime;
            nmTimeLabel.text = $"TIME: {memTimer:F1}s";
            yield return null;
        }
        if (!isPlaying) yield break;

        // Hide names and ask question
        for (int i = 0; i < 4; i++)
        {
            nmCards[i].style.display = DisplayStyle.None;
        }
        
        nmTableText.text = $"Where is\n{questionTarget}?";
        nmIsAnsweringPhase = true;

        // Wait for player to answer
        float answerTimer = 5f;
        while (answerTimer > 0 && nmIsAnsweringPhase && isPlaying)
        {
            answerTimer -= Time.deltaTime;
            nmTimeLabel.text = $"TIME: {answerTimer:F1}s";
            yield return null;
        }
        
        if (!isPlaying) yield break;

        // If they ran out of time
        if (nmIsAnsweringPhase)
        {
            nmIsAnsweringPhase = false;
            nmPlates[nmCorrectIndex].style.backgroundColor = new Color(0.18f, 0.8f, 0.44f);
        }

        // Brief pause to show result
        yield return new WaitForSeconds(1.5f);
    }

    // --- 10. CorporateGolf ---
    private int cgRoundsPlayed = 0;
    private int cgPerfectRounds = 0;
    private Label cgResultLabel;
    
    private void SetupCorporateGolf()
    {
        timer = currentConfig.timeLimit > 0 ? currentConfig.timeLimit : 15f;
        cgIsDragging = false;
        cgBallInMotion = false;
        cgBallVel = Vector2.zero;
        
        if (cgContainer != null)
        {
            cgTimeLabel = cgContainer.Q<Label>("CgTimeText");
            cgPlayArea = cgContainer.Q<VisualElement>("CgPlayArea");
            cgBall = cgContainer.Q<VisualElement>("CgBall");
            cgHole = cgContainer.Q<VisualElement>("CgHole");
            cgSandTrap = cgContainer.Q<VisualElement>("CgSandTrap");
            cgAimLine = cgContainer.Q<VisualElement>("CgAimLine");
            cgCeo = cgContainer.Q<Label>("CgCeo");
            
            if (cgTimeLabel != null) cgTimeLabel.text = $"TIME: {timer:F1}s";
            if (cgCeo != null) cgCeo.text = "👨‍💼"; // default watching
            
            if (cgPlayArea != null && cgBall != null)
            {
                // Force absolute pixel position so it's easy to move
                cgBallPos = new Vector2(178, 380); // Roughly bottom center for 400x480 play area
                cgBall.style.left = cgBallPos.x;
                cgBall.style.top = cgBallPos.y;
                cgBall.style.bottom = new StyleLength(StyleKeyword.Null);
                cgBall.style.translate = new StyleTranslate(StyleKeyword.Null);
                
                cgPlayArea.RegisterCallback<PointerDownEvent>(ev => {
                    if (cgBallInMotion || !isPlaying) return;
                    cgIsDragging = true;
                    cgPlayArea.CapturePointer(ev.pointerId);
                    cgDragStartPos = cgPlayArea.WorldToLocal(ev.position);
                    if (cgAimLine != null) cgAimLine.style.display = DisplayStyle.Flex;
                    UpdateAimLine(cgDragStartPos, cgDragStartPos);
                });
                
                cgPlayArea.RegisterCallback<PointerMoveEvent>(ev => {
                    if (cgIsDragging && isPlaying) {
                        UpdateAimLine(cgDragStartPos, cgPlayArea.WorldToLocal(ev.position));
                    }
                });
                
                cgPlayArea.RegisterCallback<PointerUpEvent>(ev => {
                    if (!cgIsDragging) return;
                    cgIsDragging = false;
                    cgPlayArea.ReleasePointer(ev.pointerId);
                    if (cgAimLine != null) cgAimLine.style.display = DisplayStyle.None;
                    
                    Vector2 dragEndPos = cgPlayArea.WorldToLocal(ev.position);
                    Vector2 dragVector = cgDragStartPos - dragEndPos; // Pulling back shoots forward
                    
                    if (dragVector.sqrMagnitude > 400f) {
                        // Shoot!
                        cgBallVel = dragVector * 4f; // velocity multiplier
                        cgBallInMotion = true;
                    }
                });
            }
        }
        activeGameRoutine = StartCoroutine(CorporateGolfRoutine());
    }
    
    private void UpdateAimLine(Vector2 startPos, Vector2 currentPos)
    {
        if (cgAimLine == null || cgBall == null) return;
        
        Vector2 dragVector = startPos - currentPos; 
        float distance = dragVector.magnitude;
        if (distance > 150f) { dragVector = dragVector.normalized * 150f; distance = 150f; }
        
        // Draw line from ball center
        Vector2 ballCenter = cgBallPos + new Vector2(12f, 12f); // ball is 24x24
        
        cgAimLine.style.left = ballCenter.x - 4f; // 8px wide, so center is -4f
        cgAimLine.style.top = ballCenter.y;
        cgAimLine.style.height = distance;
        
        float angle = Mathf.Atan2(dragVector.y, dragVector.x) * Mathf.Rad2Deg;
        cgAimLine.style.rotate = new Rotate(new Angle(angle - 90f, AngleUnit.Degree));

        // Color gradient based on power (0 to 150)
        float t = distance / 150f;
        Color color;
        if (t < 0.5f) {
            color = Color.Lerp(Color.yellow, new Color(1f, 0.5f, 0f), t * 2f); // Yellow to Orange
        } else {
            color = Color.Lerp(new Color(1f, 0.5f, 0f), Color.red, (t - 0.5f) * 2f); // Orange to Red
        }
        
        cgAimLine.style.backgroundColor = color;
        
        Label arrow = cgAimLine.Q<Label>("CgAimArrow");
        if (arrow != null) {
            arrow.style.color = color;
        }
    }

    private IEnumerator CorporateGolfRoutine()
    {
        isPlaying = true;
        
        while (isPlaying)
        {
            timer -= Time.deltaTime;
            if (cgTimeLabel != null) cgTimeLabel.text = $"TIME: {Mathf.Max(0, timer):F1}s";
            
            if (timer <= 0f)
            {
                if (cgCeo != null) cgCeo.text = "😡";
                EndGame(false);
                yield break;
            }
            
            if (cgBallInMotion)
            {
                // Move ball
                cgBallPos += cgBallVel * Time.deltaTime;
                cgBall.style.left = cgBallPos.x;
                cgBall.style.top = cgBallPos.y;
                
                // Friction
                cgBallVel = Vector2.MoveTowards(cgBallVel, Vector2.zero, 500f * Time.deltaTime);
                
                // Wall bounds (bounce) - Play area is roughly 380x440 considering borders
                if (cgBallPos.x < 0) { cgBallPos.x = 0; cgBallVel.x *= -0.7f; }
                if (cgBallPos.x > 356) { cgBallPos.x = 356; cgBallVel.x *= -0.7f; } 
                if (cgBallPos.y < 0) { cgBallPos.y = 0; cgBallVel.y *= -0.7f; }
                if (cgBallPos.y > 416) { cgBallPos.y = 416; cgBallVel.y *= -0.7f; } 
                
                if (cgBallVel.sqrMagnitude < 5f)
                {
                    cgBallInMotion = false;
                    cgBallVel = Vector2.zero;
                    
                    // Stopped, check collision
                    Rect ballRect = new Rect(cgBall.worldBound.x, cgBall.worldBound.y, cgBall.worldBound.width, cgBall.worldBound.height);
                    Rect holeRect = new Rect(cgHole.worldBound.x, cgHole.worldBound.y, cgHole.worldBound.width, cgHole.worldBound.height);
                    Rect sandRect = new Rect(cgSandTrap.worldBound.x, cgSandTrap.worldBound.y, cgSandTrap.worldBound.width, cgSandTrap.worldBound.height);
                    
                    if (ballRect.Overlaps(holeRect))
                    {
                        if (cgCeo != null) cgCeo.text = "😡";
                        yield return new WaitForSeconds(1f);
                        EndGame(false); // Beat the CEO, fired!
                        yield break;
                    }
                    else if (ballRect.Overlaps(sandRect))
                    {
                        if (cgCeo != null) cgCeo.text = "🤑";
                        yield return new WaitForSeconds(1f);
                        EndGame(true); // Let CEO win, promoted!
                        yield break;
                    }
                }
            }
            
            yield return null;
        }
    }


    // ==========================================
    // 11. MowGreens (Mow Corporate Astroturf)
    // ==========================================

    private void SetupMowGreens()
    {
        timer = currentConfig.timeLimit > 0 ? currentConfig.timeLimit : 15f;
        mgTilesCut = 0;
        mgCutStatus = new bool[36];
        mgIsObstacle = new bool[36];
        mgIsDragging = false;
        
        if (mgContainer != null)
        {
            mgTimeLabel = mgContainer.Q<Label>("MgTimeText");
            mgProgressLabel = mgContainer.Q<Label>("MgProgressLabel");
            mgGrid = mgContainer.Q<VisualElement>("MgGrid");
            mgTractor = mgContainer.Q<Label>("MgTractor");
            
            if (mgTimeLabel != null) mgTimeLabel.text = $"TIME: {timer:F1}s";
            if (mgProgressLabel != null) mgProgressLabel.text = "MOWED: 0/31";

            if (mgGrid != null)
            {
                // Clear grid tiles (but don't remove MgTractor!)
                var children = new System.Collections.Generic.List<VisualElement>(mgGrid.Children());
                foreach (var child in children)
                {
                    if (child != mgTractor) mgGrid.Remove(child);
                }
                
                // Randomly place 5 obstacles
                System.Collections.Generic.List<int> obstacleIndices = new System.Collections.Generic.List<int>();
                while (obstacleIndices.Count < 5)
                {
                    int rand = UnityEngine.Random.Range(0, 36);
                    if (!obstacleIndices.Contains(rand))
                    {
                        obstacleIndices.Add(rand);
                        mgIsObstacle[rand] = true;
                    }
                }

                for (int i = 0; i < 36; i++)
                {
                    VisualElement tile = new VisualElement();
                    tile.style.width = 60;
                    tile.style.height = 60;
                    tile.style.borderTopWidth = 2;
                    tile.style.borderBottomWidth = 2;
                    tile.style.borderLeftWidth = 2;
                    tile.style.borderRightWidth = 2;
                    tile.style.borderTopColor = new StyleColor(new Color(0.1f, 0.4f, 0.2f)); 
                    tile.style.borderBottomColor = new StyleColor(new Color(0.1f, 0.4f, 0.2f)); 
                    tile.style.borderLeftColor = new StyleColor(new Color(0.1f, 0.4f, 0.2f)); 
                    tile.style.borderRightColor = new StyleColor(new Color(0.1f, 0.4f, 0.2f));
                    
                    if (mgIsObstacle[i])
                    {
                        tile.style.backgroundColor = new StyleColor(new Color(0.08f, 0.5f, 0.3f)); // Dark grass
                        Label icon = new Label(UnityEngine.Random.value > 0.5f ? "☕" : "📄");
                        icon.style.fontSize = 32;
                        icon.style.unityTextAlign = TextAnchor.MiddleCenter;
                        icon.style.position = Position.Absolute;
                        icon.style.top = 0; icon.style.bottom = 0; icon.style.left = 0; icon.style.right = 0;
                        tile.Add(icon);
                    }
                    else
                    {
                        tile.style.backgroundColor = new StyleColor(new Color(0.08f, 0.6f, 0.3f)); // Uncut grass
                        Label icon = new Label("🌿");
                        icon.style.fontSize = 24;
                        icon.style.unityTextAlign = TextAnchor.MiddleCenter;
                        icon.style.position = Position.Absolute;
                        icon.style.top = 0; icon.style.bottom = 0; icon.style.left = 0; icon.style.right = 0;
                        tile.Add(icon);
                    }

                    // Insert before the tractor so the tractor stays on top
                    mgGrid.Insert(mgGrid.childCount - 1, tile);
                }
                
                // Register drag events on the grid itself
                mgGrid.RegisterCallback<PointerDownEvent>(ev => {
                    mgIsDragging = true;
                    mgGrid.CapturePointer(ev.pointerId);
                    Vector2 localPos = mgGrid.WorldToLocal(ev.position);
                    if (mgTractor != null) {
                        mgTractor.style.display = DisplayStyle.Flex;
                        mgTractor.style.left = localPos.x;
                        mgTractor.style.top = localPos.y;
                    }
                    HandleMowInteraction(localPos);
                });
                
                mgGrid.RegisterCallback<PointerMoveEvent>(ev => {
                    if (mgIsDragging && isPlaying) {
                        Vector2 localPos = mgGrid.WorldToLocal(ev.position);
                        if (mgTractor != null) {
                            mgTractor.style.left = localPos.x;
                            mgTractor.style.top = localPos.y;
                        }
                        HandleMowInteraction(localPos);
                    }
                });
                
                mgGrid.RegisterCallback<PointerUpEvent>(ev => {
                    mgIsDragging = false;
                    if (mgTractor != null) mgTractor.style.display = DisplayStyle.None;
                    mgGrid.ReleasePointer(ev.pointerId);
                });
                
                mgGrid.RegisterCallback<PointerLeaveEvent>(ev => {
                    // Optional: stop dragging if left grid completely
                    // mgIsDragging = false; 
                });
            }
        }
        activeGameRoutine = StartCoroutine(MowGreensRoutine());
    }

    private void HandleMowInteraction(Vector2 localPos)
    {
        if (mgGrid == null || !isPlaying) return;
        
        // Grid is 360x360, each tile is 60x60. MgGrid has 4px borders, so content starts at 4,4
        float adjustedX = localPos.x - 4f;
        float adjustedY = localPos.y - 4f;
        
        int col = (int)(adjustedX / 60f);
        int row = (int)(adjustedY / 60f);
        
        if (col >= 0 && col < 6 && row >= 0 && row < 6)
        {
            int index = row * 6 + col;
            
            if (mgIsObstacle[index])
            {
                // Hit obstacle!
                mgGrid.ElementAt(index).style.backgroundColor = new StyleColor(Color.red);
                EndGame(false);
            }
            else if (!mgCutStatus[index])
            {
                // Mow grass!
                mgCutStatus[index] = true;
                VisualElement tile = mgGrid.ElementAt(index);
                tile.style.backgroundColor = new StyleColor(new Color(0.13f, 0.77f, 0.36f)); // Bright cut grass
                tile.Clear(); // Remove 🌿 emoji
                
                mgTilesCut++;
                if (mgProgressLabel != null) mgProgressLabel.text = $"MOWED: {mgTilesCut}/31";
                
                if (mgTilesCut >= 31)
                {
                    EndGame(true);
                }
            }
        }
    }

    private IEnumerator MowGreensRoutine()
    {
        isPlaying = true;
        while (isPlaying)
        {
            timer -= Time.deltaTime;
            if (mgTimeLabel != null) mgTimeLabel.text = $"TIME: {Mathf.Max(0, timer):F1}s";
            
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
            wdTimeLabel = wdContainer.Q<UnityEngine.UIElements.Label>("WdTimeText");
            wdPlayArea = wdContainer.Q<UnityEngine.UIElements.VisualElement>("WdPlayArea");
            
            if (wdTimeLabel != null) wdTimeLabel.text = $"TIME: {timer:F1}s";
            
            if (wdPlayArea != null) {
                // Clear old dogs/leashes, preserving WdHuman (first child usually, but let's be careful)
                var children = new System.Collections.Generic.List<UnityEngine.UIElements.VisualElement>(wdPlayArea.Children());
                foreach(var c in children) {
                    if (c.name != "WdHuman") wdPlayArea.Remove(c);
                }
                
                string[] dogEmojis = { "🐕", "🐩", "🦮", "🐕‍🦺", "🐕" };
                
                for (int i = 0; i < 5; i++)
                {
                    // Create Leash
                    UnityEngine.UIElements.VisualElement leash = new UnityEngine.UIElements.VisualElement();
                    leash.style.position = UnityEngine.UIElements.Position.Absolute;
                    leash.style.height = 4;
                    leash.style.backgroundColor = new UnityEngine.UIElements.StyleColor(new UnityEngine.Color(0.6f, 0.3f, 0.1f)); // Brown leash
                    leash.style.transformOrigin = new UnityEngine.UIElements.TransformOrigin(new UnityEngine.UIElements.Length(0, UnityEngine.UIElements.LengthUnit.Percent), new UnityEngine.UIElements.Length(50, UnityEngine.UIElements.LengthUnit.Percent));
                    leash.style.left = new UnityEngine.UIElements.Length(50, UnityEngine.UIElements.LengthUnit.Percent);
                    leash.style.top = new UnityEngine.UIElements.Length(50, UnityEngine.UIElements.LengthUnit.Percent);
                    leash.pickingMode = UnityEngine.UIElements.PickingMode.Ignore;
                    wdPlayArea.Add(leash);
                    wdLeashElements[i] = leash;
                    
                    // Create Dog
                    UnityEngine.UIElements.Label dog = new UnityEngine.UIElements.Label(dogEmojis[i]);
                    dog.style.position = UnityEngine.UIElements.Position.Absolute;
                    dog.style.fontSize = 40;
                    dog.style.width = 50;
                    dog.style.height = 50;
                    dog.style.unityTextAlign = UnityEngine.TextAnchor.MiddleCenter;
                    
                    // Random starting stats
                    wdDogAngles[i] = UnityEngine.Random.Range(0f, 360f);
                    wdDogDistances[i] = UnityEngine.Random.Range(40f, 80f); // start further away
                    wdDogSpeeds[i] = UnityEngine.Random.Range(30f, 50f); // double speed
                    wdDogYankTimers[i] = 0f;
                    
                    int idx = i;
                    dog.RegisterCallback<UnityEngine.UIElements.PointerDownEvent>(ev => {
                        if (isPlaying) {
                            wdDogYankTimers[idx] = 1.0f; // Set yank timer to 1 second
                        }
                    });
                    
                    wdPlayArea.Add(dog);
                    wdDogElements[i] = dog;
                }
            }
        }
        activeGameRoutine = StartCoroutine(WalkDogsRoutine());
    }

    private System.Collections.IEnumerator WalkDogsRoutine()
    {
        isPlaying = true;
        
        while (isPlaying)
        {
            timer -= UnityEngine.Time.deltaTime;
            if (wdTimeLabel != null) wdTimeLabel.text = $"TIME: {UnityEngine.Mathf.Max(0, timer):F1}s";
            
            if (timer <= 0f)
            {
                EndGame(true); // Survived
                yield break;
            }
            
            if (wdPlayArea != null) {
                float containerWidth = wdPlayArea.worldBound.width;
                float containerHeight = wdPlayArea.worldBound.height;
                
                // If layout hasn't occurred yet, skip processing this frame
                if (containerWidth <= 0 || containerHeight <= 0) {
                    yield return null;
                    continue;
                }
                
                float maxAllowedDistance = (UnityEngine.Mathf.Min(containerWidth, containerHeight) / 2f) - 30f; // Snaps if it reaches edges
                
                for (int i = 0; i < 5; i++)
                {
                    if (wdDogYankTimers[i] > 0f) {
                        // Smoothly yank back over 1 second
                        wdDogYankTimers[i] -= UnityEngine.Time.deltaTime;
                        wdDogDistances[i] -= 80f * UnityEngine.Time.deltaTime; // Yankee pull strength
                        wdDogDistances[i] = UnityEngine.Mathf.Max(10f, wdDogDistances[i]); // Min distance floor
                    } else {
                        // Move dog outward
                        wdDogDistances[i] += wdDogSpeeds[i] * UnityEngine.Time.deltaTime;
                    }
                    
                    // Add some erratic wandering to angle
                    wdDogAngles[i] += UnityEngine.Random.Range(-15f, 15f) * UnityEngine.Time.deltaTime;
                    
                    float rad = wdDogAngles[i] * UnityEngine.Mathf.Deg2Rad;
                    float x = UnityEngine.Mathf.Cos(rad) * wdDogDistances[i];
                    float y = UnityEngine.Mathf.Sin(rad) * wdDogDistances[i];
                    
                    // Center offset (container coords)
                    float centerX = containerWidth / 2f;
                    float centerY = containerHeight / 2f;
                    
                    if (wdDogElements[i] != null) {
                        wdDogElements[i].style.left = centerX + x - 25f; // offset half dog width
                        wdDogElements[i].style.top = centerY + y - 25f;
                        
                        // Flip sprite horizontally if running left
                        if (x < 0) {
                            wdDogElements[i].style.scale = new UnityEngine.UIElements.StyleScale(new UnityEngine.UIElements.Scale(new UnityEngine.Vector3(-1f, 1f, 1f)));
                        } else {
                            wdDogElements[i].style.scale = new UnityEngine.UIElements.StyleScale(new UnityEngine.UIElements.Scale(new UnityEngine.Vector3(1f, 1f, 1f)));
                        }
                    }
                    
                    // Box boundary checks (instead of pure radius, so they can reach the corners)
                    float edgeDistX = (containerWidth / 2f) - 30f;
                    float edgeDistY = (containerHeight / 2f) - 30f;
                    bool isNearEdge = UnityEngine.Mathf.Abs(x) > edgeDistX * 0.8f || UnityEngine.Mathf.Abs(y) > edgeDistY * 0.8f;
                    bool isOut = UnityEngine.Mathf.Abs(x) > edgeDistX || UnityEngine.Mathf.Abs(y) > edgeDistY;
                    
                    if (wdLeashElements[i] != null) {
                        wdLeashElements[i].style.width = wdDogDistances[i];
                        wdLeashElements[i].style.rotate = new UnityEngine.UIElements.Rotate(new UnityEngine.UIElements.Angle(wdDogAngles[i], UnityEngine.UIElements.AngleUnit.Degree));
                        
                        // Change leash color if getting tense
                        if (isNearEdge) {
                            wdLeashElements[i].style.backgroundColor = new UnityEngine.UIElements.StyleColor(UnityEngine.Color.red);
                        } else {
                            wdLeashElements[i].style.backgroundColor = new UnityEngine.UIElements.StyleColor(new UnityEngine.Color(0.6f, 0.3f, 0.1f));
                        }
                    }
                    
                    // Check snap condition
                    if (isOut)
                    {
                        if (wdLeashElements[i] != null) wdLeashElements[i].style.display = UnityEngine.UIElements.DisplayStyle.None;
                        EndGame(false); // Leash snaps
                        yield break;
                    }
                }
            }
            
            yield return null;
        }
    }

    // ==========================================
    // 13. RetroRacer (Deliver Fast Food Order)
    // ==========================================

    private void OnRrPointerDown(UnityEngine.UIElements.PointerDownEvent ev) {
        if (!isPlaying || currentConfig == null || currentConfig.type != GameType.RetroRacer) return;
        rrIsDragging = true;
        UpdateRrPlayerPos(ev.localPosition.x);
    }

    private void OnRrPointerMove(UnityEngine.UIElements.PointerMoveEvent ev) {
        if (!rrIsDragging || !isPlaying || currentConfig == null || currentConfig.type != GameType.RetroRacer) return;
        UpdateRrPlayerPos(ev.localPosition.x);
    }

    private void UpdateRrPlayerPos(float mouseX) {
        if (rrContainer == null) return;
        float width = rrContainer.worldBound.width;
        if (width <= 0f) return;
        float pct = (mouseX / width) * 100f;
        rrTargetLeft = UnityEngine.Mathf.Clamp(pct, 15f, 85f);
    }

    private void SetupRetroRacer()
    {
        timer = currentConfig.timeLimit > 0 ? currentConfig.timeLimit : 20f;
        rrBlocks = 0;
        rrDistanceTimer = 0f;
        rrTargetLeft = 50f;
        
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

            // Smoothly move player to target position
            if (rrPlayer != null)
            {
                float currentLeft = rrPlayer.style.left.value.value;
                float newLeft = UnityEngine.Mathf.Lerp(currentLeft, rrTargetLeft, UnityEngine.Time.deltaTime * 15f);
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

                if (pos.y >= 0.9f && pos.y <= 1.0f && rrPlayer != null)
                {
                    // Dynamic Collision Check
                    float laneTargetX = laneTargets[(int)pos.x];
                    float playerX = rrPlayer.style.left.value.value;
                    if (UnityEngine.Mathf.Abs(playerX - laneTargetX) < 12f)
                    {
                        AbortGame(); // Hit obstacle
                        yield break;
                    }
                }

                if (pos.y >= 1f)
                {
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

    private UnityEngine.UIElements.Label saTimerLabel;
    private UnityEngine.UIElements.Label saConnectionLabel;

    private void SetupSkimATM()
    {
        timer = currentConfig.timeLimit > 0 ? currentConfig.timeLimit : 10f;
        saConnections = 0;
        saIsDraggingWire = false;
        saActiveWire = null;
        saStartNode = null;
        
        if (saContainer != null)
        {
            saTimerLabel = saContainer.Q<UnityEngine.UIElements.Label>("SaTimerText");
            saConnectionLabel = saContainer.Q<UnityEngine.UIElements.Label>("SaConnectionText");
            if (saTimerLabel != null) saTimerLabel.text = $"TIME: {timer:F1}s";
            if (saConnectionLabel != null) saConnectionLabel.text = "LINKS: 0/4";

            UnityEngine.UIElements.VisualElement leftCol = saContainer.Q<UnityEngine.UIElements.VisualElement>("SaLeftPanel");
            UnityEngine.UIElements.VisualElement rightCol = saContainer.Q<UnityEngine.UIElements.VisualElement>("SaRightPanel");

            if (leftCol != null) leftCol.Clear();
            if (rightCol != null) rightCol.Clear();
            if (saWireContainer != null) {
                saWireContainer.Clear();
                // Ensure wire container receives events across its whole area
                saWireContainer.pickingMode = UnityEngine.UIElements.PickingMode.Position;
                // Clean up previous event listeners just in case
                saWireContainer.UnregisterCallback<UnityEngine.UIElements.PointerMoveEvent>(OnSaPointerMove);
                saWireContainer.UnregisterCallback<UnityEngine.UIElements.PointerUpEvent>(OnSaPointerUp);
                saWireContainer.UnregisterCallback<UnityEngine.UIElements.PointerLeaveEvent>(OnSaPointerUp);
                
                saWireContainer.RegisterCallback<UnityEngine.UIElements.PointerMoveEvent>(OnSaPointerMove);
                saWireContainer.RegisterCallback<UnityEngine.UIElements.PointerUpEvent>(OnSaPointerUp);
                saWireContainer.RegisterCallback<UnityEngine.UIElements.PointerLeaveEvent>(OnSaPointerUp);
            }
            
            saLeftNodes.Clear();
            saRightNodes.Clear();
            
            UnityEngine.Color[] colors = { UnityEngine.Color.red, UnityEngine.Color.blue, UnityEngine.Color.green, UnityEngine.Color.yellow };
            UnityEngine.Color[] rightColors = { UnityEngine.Color.red, UnityEngine.Color.blue, UnityEngine.Color.green, UnityEngine.Color.yellow };
            
            for(int i = 0; i < rightColors.Length; i++) {
                int r = UnityEngine.Random.Range(i, rightColors.Length);
                UnityEngine.Color temp = rightColors[i];
                rightColors[i] = rightColors[r];
                rightColors[r] = temp;
            }
            
            for (int i = 0; i < 4; i++)
            {
                UnityEngine.UIElements.VisualElement lBtn = new UnityEngine.UIElements.VisualElement();
                lBtn.style.width = 40; lBtn.style.height = 40; 
                lBtn.style.marginTop = 15; lBtn.style.marginBottom = 15;
                lBtn.style.backgroundColor = new UnityEngine.UIElements.StyleColor(colors[i]);
                lBtn.style.borderTopLeftRadius = 20; lBtn.style.borderTopRightRadius = 20; lBtn.style.borderBottomLeftRadius = 20; lBtn.style.borderBottomRightRadius = 20;
                
                lBtn.RegisterCallback<UnityEngine.UIElements.PointerDownEvent>(ev => OnSaNodePointerDown(ev, lBtn));
                saLeftNodes.Add(lBtn);
                if (leftCol != null) leftCol.Add(lBtn);
                
                UnityEngine.UIElements.VisualElement rBtn = new UnityEngine.UIElements.VisualElement();
                rBtn.style.width = 40; rBtn.style.height = 40; 
                rBtn.style.marginTop = 15; rBtn.style.marginBottom = 15;
                rBtn.style.backgroundColor = new UnityEngine.UIElements.StyleColor(rightColors[i]);
                rBtn.style.borderTopLeftRadius = 20; rBtn.style.borderTopRightRadius = 20; rBtn.style.borderBottomLeftRadius = 20; rBtn.style.borderBottomRightRadius = 20;
                
                saRightNodes.Add(rBtn);
                if (rightCol != null) rightCol.Add(rBtn);
            }
        }

        activeGameRoutine = StartCoroutine(SkimATMRoutine());
    }

    private void OnSaNodePointerDown(UnityEngine.UIElements.PointerDownEvent ev, UnityEngine.UIElements.VisualElement node) {
        if (!isPlaying || currentConfig?.type != GameType.SkimATM || !node.enabledSelf || saWireContainer == null) return;
        saIsDraggingWire = true;
        saStartNode = node;
        
        saActiveWire = new UnityEngine.UIElements.VisualElement();
        saActiveWire.style.position = UnityEngine.UIElements.Position.Absolute;
        saActiveWire.style.height = 8; // Wire thickness
        saActiveWire.style.backgroundColor = node.style.backgroundColor;
        saActiveWire.style.borderTopLeftRadius = 4;
        saActiveWire.style.borderBottomLeftRadius = 4;
        saActiveWire.style.transformOrigin = new UnityEngine.UIElements.TransformOrigin(new UnityEngine.UIElements.Length(0, UnityEngine.UIElements.LengthUnit.Percent), new UnityEngine.UIElements.Length(50, UnityEngine.UIElements.LengthUnit.Percent));
        saActiveWire.pickingMode = UnityEngine.UIElements.PickingMode.Ignore; // Crucial so it doesn't block PointerUp
        
        // Calculate center of start node relative to saWireContainer
        UnityEngine.Vector2 nodeCenter = node.worldBound.center;
        UnityEngine.Vector2 localCenter = saWireContainer.WorldToLocal(nodeCenter);
        
        saActiveWire.style.left = localCenter.x;
        saActiveWire.style.top = localCenter.y - 4; // offset by half height
        
        saWireContainer.Add(saActiveWire);
        
        // Ensure the container captures the pointer so we can drag outside the initial element
        saWireContainer.CapturePointer(ev.pointerId);
    }
    
    private void OnSaPointerMove(UnityEngine.UIElements.PointerMoveEvent ev) {
        if (!saIsDraggingWire || saActiveWire == null || saStartNode == null || saWireContainer == null) return;
        
        UnityEngine.Vector2 startPos = saWireContainer.WorldToLocal(saStartNode.worldBound.center);
        UnityEngine.Vector2 mousePos = saWireContainer.WorldToLocal(ev.position);
        
        UnityEngine.Vector2 dir = mousePos - startPos;
        float distance = dir.magnitude;
        float angle = UnityEngine.Mathf.Atan2(dir.y, dir.x) * UnityEngine.Mathf.Rad2Deg;
        
        saActiveWire.style.width = distance;
        saActiveWire.style.rotate = new UnityEngine.UIElements.Rotate(new UnityEngine.UIElements.Angle(angle, UnityEngine.UIElements.AngleUnit.Degree));
    }
    
    private void OnSaPointerUp(UnityEngine.UIElements.EventBase ev) {
        if (!saIsDraggingWire || saWireContainer == null) return;
        saIsDraggingWire = false;
        
        UnityEngine.UIElements.IPointerEvent pointerEv = ev as UnityEngine.UIElements.IPointerEvent;
        if (pointerEv != null && saWireContainer.HasPointerCapture(pointerEv.pointerId)) {
            saWireContainer.ReleasePointer(pointerEv.pointerId);
        }
        
        if (saActiveWire == null || saStartNode == null) return;
        
        if (pointerEv == null) return;
        
        UnityEngine.Vector2 mousePos = pointerEv.position;
        UnityEngine.UIElements.VisualElement targetNode = null;
        
        foreach (var rBtn in saRightNodes) {
            if (rBtn.enabledSelf && rBtn.worldBound.Contains(mousePos)) {
                targetNode = rBtn;
                break;
            }
        }
        
        if (targetNode != null && targetNode.style.backgroundColor.value == saStartNode.style.backgroundColor.value) {
            // Success snap
            UnityEngine.Vector2 startPos = saWireContainer.WorldToLocal(saStartNode.worldBound.center);
            UnityEngine.Vector2 targetPos = saWireContainer.WorldToLocal(targetNode.worldBound.center);
            UnityEngine.Vector2 dir = targetPos - startPos;
            
            saActiveWire.style.width = dir.magnitude;
            saActiveWire.style.rotate = new UnityEngine.UIElements.Rotate(new UnityEngine.UIElements.Angle(UnityEngine.Mathf.Atan2(dir.y, dir.x) * UnityEngine.Mathf.Rad2Deg, UnityEngine.UIElements.AngleUnit.Degree));
            
            saStartNode.enabledSelf = false;
            targetNode.enabledSelf = false;
            
            saConnections++;
            if (saConnectionLabel != null) saConnectionLabel.text = $"LINKS: {saConnections}/4";
            
            if (saConnections >= 4) EndGame(true);
        } else {
            // Snap back / Destroy
            if (saWireContainer.Contains(saActiveWire)) {
                saWireContainer.Remove(saActiveWire);
            }
            if (targetNode != null && targetNode.style.backgroundColor.value != saStartNode.style.backgroundColor.value) {
                // Penalty for connecting wrong colors
                timer -= 2f;
            }
        }
        
        saActiveWire = null;
        saStartNode = null;
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

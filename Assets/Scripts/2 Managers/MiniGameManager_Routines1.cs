using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class MiniGameManager : MonoBehaviour
{
    private IEnumerator FallingSortRoutine()
    {
        List<FallingItemData> activeItems = new List<FallingItemData>();
        float spawnTimer = 0f;
        int itemsSpawned = 0;
        int totalToDrop = 10;
        float gravity = 600f; // Slower falling

        while (isPlaying && sortScore < currentConfig.sortTarget && timer > 0)
        {
            sortScoreText.text = $"Sorted: {sortScore} / {currentConfig.sortTarget}";
            sortTimerText.text = $"Time: {Mathf.Ceil(timer)}s";
            timer -= Time.deltaTime;

            if (itemsSpawned < totalToDrop && spawnTimer <= 0f)
            {
                int typeIdx = Random.Range(0, currentConfig.fallingItems.Length);
                string text = currentConfig.fallingItems[typeIdx];
                string emojiText = text.Contains(" ") ? text.Split(' ')[0] : text;
                
                var label = new Label(emojiText);
                label.style.position = Position.Absolute;
                label.style.fontSize = 50;
                
                float startX = Random.Range(50f, fallZone.resolvedStyle.width - 100f);
                
                FallingItemData itemData = new FallingItemData {
                    element = label,
                    position = new Vector2(startX, -50),
                    velocity = new Vector2(Random.Range(-50f, 50f), 0),
                    typeId = typeIdx
                };
                
                label.RegisterCallback<PointerDownEvent>(ev => {
                    if (!isPlaying) return;
                    itemData.isDragging = true;
                    itemData.velocity = Vector2.zero;
                    label.CapturePointer(ev.pointerId);
                    ev.StopPropagation();
                });
                
                label.RegisterCallback<PointerMoveEvent>(ev => {
                    if (itemData.isDragging && label.HasPointerCapture(ev.pointerId)) {
                        itemData.position += (Vector2)ev.deltaPosition;
                        itemData.velocity = (Vector2)ev.deltaPosition / Time.deltaTime; 
                    }
                });
                
                label.RegisterCallback<PointerUpEvent>(ev => {
                    if (itemData.isDragging) {
                        itemData.isDragging = false;
                        label.ReleasePointer(ev.pointerId);
                    }
                });

                fallZone.Add(label);
                activeItems.Add(itemData);
                itemsSpawned++;
                spawnTimer = 2.0f; // 2 second gap
            }
            if (spawnTimer > 0f) spawnTimer -= Time.deltaTime;

            float w = fallZone.resolvedStyle.width;
            float h = fallZone.resolvedStyle.height;

            for (int i = activeItems.Count - 1; i >= 0; i--)
            {
                var item = activeItems[i];
                if (!item.isDragging)
                {
                    item.velocity.y += gravity * Time.deltaTime;
                    item.position += item.velocity * Time.deltaTime;
                    
                    if (item.position.x < 0) { item.position.x = 0; item.velocity.x = 0; }
                    if (item.position.x > w - 80f) { item.position.x = w - 80f; item.velocity.x = 0; }
                    if (item.position.y > h - 40f) {
                        item.position.y = h - 40f;
                        item.velocity.y = 0;
                        item.velocity.x = 0;
                    }
                }
                
                item.element.style.left = item.position.x;
                item.element.style.top = item.position.y;
                
                if (item.position.y >= h - 60f && !item.isDragging) 
                {
                    float binWidth = w / currentConfig.binLabels.Length;
                    int binIdx = Mathf.FloorToInt((item.position.x + 40f) / binWidth);
                    binIdx = Mathf.Clamp(binIdx, 0, currentConfig.binLabels.Length - 1);
                    
                    if (binIdx == item.typeId)
                    {
                        sortScore++;
                    }
                    
                    // Whether wrong or right, destroy the item when it falls in. No bouncing.
                    fallZone.Remove(item.element);
                    activeItems.RemoveAt(i);
                    
                    if (sortScore >= currentConfig.sortTarget) MarkWinReady();
                }
            }

            // If we've dropped all 10 items and the box is empty, the game ends.
            if (itemsSpawned >= totalToDrop && activeItems.Count == 0)
            {
                if (sortScore >= currentConfig.sortTarget || !currentGame.canFail) MarkWinReady();
                else { isPlaying = false; yield break; }
            }

            if (timer <= 0 && currentGame.canFail)
            {
                isPlaying = false;
                yield break;
            }
            else if (timer <= 0 && !currentGame.canFail && sortScore < currentConfig.sortTarget)
            {
                timer = 20f; 
            }
            yield return null;
        }
    }

    private void OnTapClicked()
    {
        if (!isPlaying || currentConfig.type != GameType.Tap) return;
        score++;
        tapProgressText.text = $"Progress: {score} / {currentConfig.targetScore}";
        if (score >= currentConfig.targetScore) MarkWinReady();
    }

    private void UpdateSliderText(float val)
    {
        if (currentConfig.type == GameType.Slider)
        {
            sliderTargetText.text = $"Current: {val:F0}% (Target: {currentConfig.sliderMin}% - {currentConfig.sliderMax}%)";
        }
    }

    private void OnSliderSubmitted()
    {
        if (!isPlaying || currentConfig.type != GameType.Slider) return;
        float val = gameSlider.value;
        if (val >= currentConfig.sliderMin && val <= currentConfig.sliderMax)
        {
            MarkWinReady();
        }
    }

    private void OnChoiceClicked(string choice)
    {
        if (!isPlaying || currentConfig.type != GameType.Choice) return;
        if (choice == currentConfig.correctChoice)
        {
            MarkWinReady();
        }
    }

    private void GenerateSequence(int length)
    {
        currentSequence.Clear();
        int activeButtons = Mathf.Min(6, length + 2);
        for (int i = 0; i < 6; i++)
        {
            if (i < activeButtons)
            {
                seqBtns[i].style.display = DisplayStyle.Flex;
                seqBtns[i].text = $"[{i + 1}]";
            }
            else
            {
                seqBtns[i].style.display = DisplayStyle.None;
            }
        }

        for (int i = 0; i < length; i++)
        {
            currentSequence.Add(Random.Range(0, activeButtons));
        }
    }

    private IEnumerator PlaySequenceRoutine()
    {
        isShowingSequence = true;
        sequenceInputIndex = 0;
        sequenceStatusText.text = "Watch the pattern...";
        
        foreach (var btn in seqBtns) btn.style.backgroundColor = new StyleColor(new Color(0.12f, 0.16f, 0.23f));

        yield return new WaitForSeconds(1f);

        foreach (int index in currentSequence)
        {
            seqBtns[index].style.backgroundColor = new StyleColor(new Color(0.22f, 0.74f, 0.97f));
            yield return new WaitForSeconds(0.4f);
            seqBtns[index].style.backgroundColor = new StyleColor(new Color(0.12f, 0.16f, 0.23f));
            yield return new WaitForSeconds(0.2f);
        }

        sequenceStatusText.text = "Your Turn!";
        isShowingSequence = false;
    }

    private void ReplaySequence()
    {
        if (isPlaying && !isShowingSequence)
        {
            if (activeGameRoutine != null) StopCoroutine(activeGameRoutine);
            activeGameRoutine = StartCoroutine(PlaySequenceRoutine());
        }
    }

    private void OnSequenceButtonClicked(int index)
    {
        if (!isPlaying || isShowingSequence) return;
        if (currentConfig.type != GameType.Sequence && currentConfig.type != GameType.TwoPhaseHeist) return;

        if (currentSequence[sequenceInputIndex] == index)
        {
            sequenceInputIndex++;
            seqBtns[index].style.backgroundColor = new StyleColor(new Color(0.18f, 0.8f, 0.44f));
            StartCoroutine(ResetButtonColor(seqBtns[index]));

            if (sequenceInputIndex >= currentSequence.Count)
            {
                sequenceStatusText.text = "Pattern Complete!";
                MarkWinReady();
            }
        }
        else
        {
            if (currentGame.canFail)
            {
                sequenceStatusText.text = "Pattern Failed!";
                seqBtns[index].style.backgroundColor = new StyleColor(new Color(0.9f, 0.3f, 0.23f));
                StartCoroutine(ResetButtonColor(seqBtns[index]));
                ReplaySequence();
            }
            else
            {
                StartCoroutine(ResetButtonColor(seqBtns[index], new Color(0.9f, 0.3f, 0.23f)));
            }
        }
    }

    private IEnumerator ResetButtonColor(Button btn, Color? tempColor = null)
    {
        if (tempColor.HasValue) btn.style.backgroundColor = new StyleColor(tempColor.Value);
        yield return new WaitForSeconds(0.2f);
        btn.style.backgroundColor = new StyleColor(new Color(0.12f, 0.16f, 0.23f));
    }

    private IEnumerator SwipeGameRoutine()
    {
        while (isPlaying && timer > 0 && score < currentConfig.targetScore)
        {
            expectedDodgeDir = Random.value > 0.5f ? 1 : -1;
            swipeObstacleText.text = expectedDodgeDir == -1 ? "◀ THREAT FROM LEFT!" : "THREAT FROM RIGHT! ▶";
            
            float reactTime = 1.5f;
            float elapsed = 0;

            while (elapsed < reactTime)
            {
                if (!isPlaying) yield break;
                elapsed += Time.deltaTime;
                timer -= Time.deltaTime;
                swipeTimerText.text = $"Time: {Mathf.Ceil(timer)}s";

                if (expectedDodgeDir == 0)
                {
                    score++;
                    swipeProgressText.text = $"Dodged: {score} / {currentConfig.targetScore}";
                    break;
                }
                yield return null;
            }

            expectedDodgeDir = 0;
            swipeObstacleText.text = "Get Ready...";
            yield return new WaitForSeconds(0.5f);
        }

        if (score >= currentConfig.targetScore || (!currentGame.canFail && timer <= 0))
        {
            swipeObstacleText.text = "SURVIVED!";
            MarkWinReady();
        }
    }

    private void OnDodgeClicked(int dir)
    {
        if (!isPlaying || currentConfig.type != GameType.Swipe) return;
        if (expectedDodgeDir == dir)
        {
            expectedDodgeDir = 0;
        }
    }

    private IEnumerator MashRaceRoutine()
    {
        mashRaceContainer.style.translate = new Translate(0, 0, 0);
        mashTimerText.text = "MASH!";
        
        float nextAiTapTimer = Random.Range(0.1f, 0.3f);
        float decayRate = (currentConfig.targetScore / currentConfig.timeLimit) * 0.65f; // Constant downward force

        while (isPlaying && score < currentConfig.targetScore)
        {
            // Constant Gravity / Drain for both
            playerMashProgress -= decayRate * Time.deltaTime;
            if (playerMashProgress < 0) playerMashProgress = 0;
            
            opponentProgress -= decayRate * Time.deltaTime;
            if (opponentProgress < 0) opponentProgress = 0;

            // AI Mashing Logic (discrete clicks with random micro-pauses)
            nextAiTapTimer -= Time.deltaTime;
            if (nextAiTapTimer <= 0f)
            {
                opponentProgress += 1f; // AI clicks!
                
                // Randomize next click delay (faster base speed than player to be competitive)
                // 15% chance for a slightly longer pause (mistake)
                if (Random.value < 0.15f)
                {
                    nextAiTapTimer = Random.Range(0.3f, 0.6f);
                }
                else
                {
                    nextAiTapTimer = Random.Range(0.05f, 0.2f);
                }
            }
            
            score = Mathf.FloorToInt(playerMashProgress);
            
            float pct = Mathf.Clamp01(playerMashProgress / currentConfig.targetScore) * 100f;
            playerProgressBar.style.width = new Length(pct, LengthUnit.Percent);
            
            float oppPct = Mathf.Clamp01(opponentProgress / currentConfig.targetScore) * 100f;
            opponentProgressBar.style.width = new Length(oppPct, LengthUnit.Percent);

            if (currentGame.name == "Stress test server load")
            {
                if (pct > 50f || oppPct > 50f)
                {
                    float intensity = Mathf.Max(pct, oppPct) / 100f * 5f;
                    mashRaceContainer.style.translate = new Translate(Random.Range(-intensity, intensity), Random.Range(-intensity, intensity), 0);
                }
                else
                {
                    mashRaceContainer.style.translate = new Translate(0, 0, 0);
                }
            }


            if (opponentProgress >= currentConfig.targetScore)
            {
                if (currentGame.canFail)
                {
                    mashTimerText.text = "OPPONENT WINS!";
                    isPlaying = false;
                    yield break;
                }
                else
                {
                    opponentProgress = currentConfig.targetScore; // Pin at max
                }
            }

            yield return null;
        }
    }

    private void OnMashTapClicked()
    {
        if (!isPlaying) return;
        if (currentConfig.type != GameType.MashRace && currentPhase != 2) return;

        playerMashProgress += 1f;
        score = Mathf.FloorToInt(playerMashProgress);
        
        float pct = Mathf.Clamp01(playerMashProgress / currentConfig.targetScore) * 100f;
        playerProgressBar.style.width = new Length(pct, LengthUnit.Percent);

        if (playerMashProgress >= currentConfig.targetScore)
        {
            mashTimerText.text = "YOU WIN!";
            MarkWinReady();
        }
    }

    private void SetupMemoryMatch(int pairs)
    {
        memoryGrid.Clear();
        memoryCards.Clear();
        memoryValues.Clear();
        score = 0;
        matchStatusText.text = $"Pairs Found: 0 / {pairs}";

        List<string> icons = new List<string> { "📈", "💼", "🏢", "📊", "💰", "☕", "📱", "📝" };
        for (int i = 0; i < pairs; i++)
        {
            memoryValues.Add(icons[i % icons.Count]);
            memoryValues.Add(icons[i % icons.Count]);
        }
        
        for (int i = 0; i < memoryValues.Count; i++)
        {
            string temp = memoryValues[i];
            int randomIndex = Random.Range(i, memoryValues.Count);
            memoryValues[i] = memoryValues[randomIndex];
            memoryValues[randomIndex] = temp;
        }

        for (int i = 0; i < memoryValues.Count; i++)
        {
            int index = i;
            var btn = new Button { text = "?" };
            btn.AddToClassList("popup-button");
            btn.AddToClassList("memory-card");
            btn.clicked += () => OnMemoryCardClicked(index);
            memoryGrid.Add(btn);
            memoryCards.Add(btn);
        }
    }

    private void OnMemoryCardClicked(int index)
    {
        if (!isPlaying || isCheckingMatch || memoryCards[index].text != "?") return;

        memoryCards[index].text = memoryValues[index];
        memoryCards[index].RemoveFromClassList("memory-card");
        memoryCards[index].AddToClassList("memory-card-matched");

        if (flippedIndex1 == -1)
        {
            flippedIndex1 = index;
        }
        else
        {
            flippedIndex2 = index;
            StartCoroutine(CheckMemoryMatch());
        }
    }

    private IEnumerator CheckMemoryMatch()
    {
        isCheckingMatch = true;
        yield return new WaitForSeconds(0.6f);

        if (memoryValues[flippedIndex1] == memoryValues[flippedIndex2])
        {
            score++;
            matchStatusText.text = $"Pairs Found: {score} / {currentConfig.pairs}";
            if (score >= currentConfig.pairs) MarkWinReady();
        }
        else
        {
            memoryCards[flippedIndex1].text = "?";
            memoryCards[flippedIndex1].RemoveFromClassList("memory-card-matched");
            memoryCards[flippedIndex1].AddToClassList("memory-card");
            memoryCards[flippedIndex2].text = "?";
            memoryCards[flippedIndex2].RemoveFromClassList("memory-card-matched");
            memoryCards[flippedIndex2].AddToClassList("memory-card");
        }

        flippedIndex1 = -1;
        flippedIndex2 = -1;
        isCheckingMatch = false;
    }

    private IEnumerator TimingRingRoutine()
    {
        while (isPlaying && timingCurrentRound < currentConfig.timingRounds)
        {
            timingRoundText.text = $"Round {timingCurrentRound + 1} / {currentConfig.timingRounds}";
            
            float currentZoneDegrees = Mathf.Max(10f, 45f - (timingCurrentRound * 5f)); 
            
            if (arcMaskInner != null)
                arcMaskInner.style.rotate = new Rotate(Angle.Degrees(90f - currentZoneDegrees));

            if (timingTargetZone != null)
            {
                float centerOffset = 90f - currentZoneDegrees / 2f;
                timingTargetZone.style.rotate = new Rotate(Angle.Degrees(timingTargetAngle - centerOffset));
            }

            timingMarkerMoving = true;
            
            while (timingMarkerMoving && isPlaying)
            {
                timingMarkerAngle += timingRingRotSpeed * timingSpinDir * Time.deltaTime;
                timingMarkerAngle %= 360f;
                if (timingMarkerAngle < 0f) timingMarkerAngle += 360f;
                
                if (timingMarker != null)
                    timingMarker.style.rotate = new Rotate(Angle.Degrees(timingMarkerAngle));
                    
                yield return null;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
    
    private void OnTimingStrike()
    {
        if (!isPlaying || currentConfig.type != GameType.TimingRing || !timingMarkerMoving) return;
        timingMarkerMoving = false;
        
        float currentZoneDegrees = Mathf.Max(10f, 45f - (timingCurrentRound * 5f)); 
        
        float diff = Mathf.Abs(Mathf.DeltaAngle(timingMarkerAngle, timingTargetAngle));
        
        if (diff <= currentZoneDegrees / 2f)
        {
            timingCurrentRound++;
            StartCoroutine(HitAuraRoutine());
            if (timingCurrentRound >= currentConfig.timingRounds)
            {
                MarkWinReady();
            }
            else
            {
                timingSpinDir *= -1; // Reverse direction
                timingTargetAngle = Random.Range(0f, 360f); // New target
            }
        }
        else
        {
            if (currentGame.canFail) { /* fail state */ }
        }
    }

    private IEnumerator HitAuraRoutine()
    {
        if (timingGlow != null && glowLayers != null)
        {
            timingGlow.style.opacity = 1f;
            
            float[] maxBorderWidths = { 2f, 4f, 7f, 10f, 14f, 20f }; 
            
            float t = 0;
            while(t < 0.1f)
            {
                t += Time.deltaTime;
                float progress = t / 0.1f;
                
                for(int i = 0; i < 6; i++)
                {
                    if (glowLayers[i] == null) continue;
                    float b = Mathf.Lerp(0f, maxBorderWidths[i] * 0.7f, progress);
                    float w = 360f + (b * 2f);
                    glowLayers[i].style.borderTopWidth = b;
                    glowLayers[i].style.borderBottomWidth = b;
                    glowLayers[i].style.borderLeftWidth = b;
                    glowLayers[i].style.borderRightWidth = b;
                    glowLayers[i].style.width = w;
                    glowLayers[i].style.height = w;
                    glowLayers[i].style.borderTopLeftRadius = w/2f;
                    glowLayers[i].style.borderTopRightRadius = w/2f;
                    glowLayers[i].style.borderBottomLeftRadius = w/2f;
                    glowLayers[i].style.borderBottomRightRadius = w/2f;
                }
                yield return null;
            }
            
            t = 0;
            while(t < 0.35f)
            {
                t += Time.deltaTime;
                float progress = t / 0.35f;
                float opacity = Mathf.Lerp(1f, 0f, progress);
                timingGlow.style.opacity = opacity;
                
                for(int i = 0; i < 6; i++)
                {
                    if (glowLayers[i] == null) continue;
                    float b = Mathf.Lerp(maxBorderWidths[i] * 0.7f, maxBorderWidths[i], progress);
                    float w = 360f + (b * 2f);
                    glowLayers[i].style.borderTopWidth = b;
                    glowLayers[i].style.borderBottomWidth = b;
                    glowLayers[i].style.borderLeftWidth = b;
                    glowLayers[i].style.borderRightWidth = b;
                    glowLayers[i].style.width = w;
                    glowLayers[i].style.height = w;
                    glowLayers[i].style.borderTopLeftRadius = w/2f;
                    glowLayers[i].style.borderTopRightRadius = w/2f;
                    glowLayers[i].style.borderBottomLeftRadius = w/2f;
                    glowLayers[i].style.borderBottomRightRadius = w/2f;
                }
                yield return null;
            }
            
            timingGlow.style.opacity = 0f;
            for(int i = 0; i < 6; i++)
            {
                if (glowLayers[i] == null) continue;
                glowLayers[i].style.borderTopWidth = 0f;
                glowLayers[i].style.borderBottomWidth = 0f;
                glowLayers[i].style.borderLeftWidth = 0f;
                glowLayers[i].style.borderRightWidth = 0f;
                glowLayers[i].style.width = 360f;
                glowLayers[i].style.height = 360f;
                glowLayers[i].style.borderTopLeftRadius = 180f;
                glowLayers[i].style.borderTopRightRadius = 180f;
                glowLayers[i].style.borderBottomLeftRadius = 180f;
                glowLayers[i].style.borderBottomRightRadius = 180f;
            }
        }
    }

    private void SetupGridDodge()
    {
        gridDodgeGrid.Clear();
        gridCells = new Button[currentConfig.gridSize, currentConfig.gridSize];
        gridPlayerX = 0;
        gridPlayerY = 0;
        gridHP = currentConfig.gridHealth;
        gridPathCells.Clear();
        
        int cx = 0; int cy = 0;
        gridPathCells.Add((cx, cy));
        while (cx < currentConfig.gridSize - 1 || cy < currentConfig.gridSize - 1)
        {
            if (cx == currentConfig.gridSize - 1) cy++;
            else if (cy == currentConfig.gridSize - 1) cx++;
            else { if (Random.value > 0.5f) cx++; else cy++; }
            gridPathCells.Add((cx, cy));
        }

        for (int y = 0; y < currentConfig.gridSize; y++)
        {
            var row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.flexGrow = 1;
            for (int x = 0; x < currentConfig.gridSize; x++)
            {
                var btn = new Button();
                btn.style.flexGrow = 1;
                btn.style.marginTop = 1; btn.style.marginBottom = 1; btn.style.marginLeft = 1; btn.style.marginRight = 1;
                btn.style.fontSize = 32;
                btn.style.color = Color.white;
                btn.style.unityFontStyleAndWeight = FontStyle.Bold;
                
                // Remove default button border styling for a clean flat look
                btn.style.borderTopWidth = 0; btn.style.borderBottomWidth = 0; btn.style.borderLeftWidth = 0; btn.style.borderRightWidth = 0;
                btn.style.borderTopLeftRadius = 4; btn.style.borderTopRightRadius = 4; btn.style.borderBottomLeftRadius = 4; btn.style.borderBottomRightRadius = 4;
                
                gridCells[x,y] = btn;
                row.Add(btn);
            }
            gridDodgeGrid.Add(row);
        }
        UpdateGridDodgeUI();
        activeGameRoutine = StartCoroutine(GridDodgeRoutine());
    }

    private void UpdateGridDodgeUI()
    {
        gridHealthText.text = $"HP: {gridHP}";
        for (int x=0; x<currentConfig.gridSize; x++)
        {
            for (int y=0; y<currentConfig.gridSize; y++)
            {
                var btn = gridCells[x,y];
                btn.text = "";
                
                if (!gridPathCells.Contains((x,y))) 
                    btn.style.backgroundColor = new StyleColor(new Color(0.08f, 0.32f, 0.17f)); // Dark green hedge (#14532D)
                else 
                    btn.style.backgroundColor = new StyleColor(new Color(0.83f, 0.76f, 0.65f)); // Dirt path (#D4C3A6)
                
                if (x == currentConfig.gridSize - 1 && y == currentConfig.gridSize - 1)
                {
                    btn.text = "⛲"; // Fountain goal
                    btn.style.backgroundColor = new StyleColor(new Color(0.38f, 0.65f, 0.98f)); // Cyan water pool
                }

                if (x == gridPlayerX && y == gridPlayerY)
                {
                    if (currentGame.name == "Empty waste bin") btn.text = "🗑️";
                    else if (currentGame.name == "Walk in the Park") btn.text = "🚶‍♂️";
                    else if (currentGame.name == "Carry tungsten girders") btn.text = "🏗️";
                    else btn.text = "P";
                    
                    btn.style.backgroundColor = new StyleColor(new Color(0.95f, 0.95f, 0.95f));
                    btn.style.color = Color.black;
                }
            }
        }
    }

    private IEnumerator GridDodgeRoutine()
    {
        sharedInputDx = 0; sharedInputDy = 0;
        while (isPlaying && currentConfig.type == GameType.GridDodge)
        {
            int dx = sharedInputDx, dy = sharedInputDy;
            sharedInputDx = 0; sharedInputDy = 0;

            if (dx != 0 || dy != 0)
            {
                int nx = gridPlayerX + dx;
                int ny = gridPlayerY + dy;

                if (nx >= 0 && nx < currentConfig.gridSize && ny >= 0 && ny < currentConfig.gridSize)
                {
                    if (gridPathCells.Contains((nx, ny)))
                    {
                        gridPlayerX = nx;
                        gridPlayerY = ny;
                        UpdateGridDodgeUI();

                        if (gridPlayerX == currentConfig.gridSize - 1 && gridPlayerY == currentConfig.gridSize - 1)
                        {
                            MarkWinReady();
                        }
                    }
                    else
                    {
                        gridHP--;
                        UpdateGridDodgeUI();
                        if (gridHP <= 0 && currentGame.canFail) { isPlaying = false; }
                        else if (gridHP <= 0 && !currentGame.canFail) gridHP = 1; 
                    }
                }
            }
            yield return null;
        }
    }

    private void SetupWhackGrid()
    {
        whackGrid.Clear();
        whackCells = new Button[9];
        for (int i=0; i<9; i++)
        {
            var btn = new Button();
            btn.style.width = new Length(30, LengthUnit.Percent);
            btn.style.height = new Length(30, LengthUnit.Percent);
            btn.style.marginTop = 2; btn.style.marginBottom = 2; btn.style.marginLeft = 2; btn.style.marginRight = 2;
            btn.style.fontSize = 32;
            int idx = i;
            btn.clicked += () => OnWhackCellClicked(idx);
            whackGrid.Add(btn);
            whackCells[i] = btn;
            ResetWhackBtn(btn);
        }
    }

    private void ResetWhackBtn(Button btn)
    {
        btn.style.backgroundColor = new StyleColor(Color.gray);
        btn.text = "";
    }

    private IEnumerator WhackGridRoutine()
    {
        while(isPlaying && timer > 0 && whackScore < currentConfig.whackTarget)
        {
            timer -= Time.deltaTime;
            whackTimerText.text = $"Time: {Mathf.Ceil(timer)}s";
            
            if (activeWhackCell == -1)
            {
                activeWhackCell = Random.Range(0, 9);
                isDecoy = Random.value > 0.7f;
                var btn = whackCells[activeWhackCell];
                btn.style.backgroundColor = new StyleColor(isDecoy ? Color.red : Color.green);
                btn.text = "!";
                yield return new WaitForSeconds(Random.Range(0.8f, 1.2f));
                if (activeWhackCell != -1)
                {
                    ResetWhackBtn(whackCells[activeWhackCell]);
                    activeWhackCell = -1;
                }
            }
            yield return null;
        }
        if (whackScore >= currentConfig.whackTarget || (!currentGame.canFail && timer <= 0)) MarkWinReady();
    }

    private void OnWhackCellClicked(int idx)
    {
        if (!isPlaying || currentConfig.type != GameType.WhackGrid) return;
        if (idx == activeWhackCell)
        {
            if (isDecoy) whackScore = Mathf.Max(0, whackScore - 1);
            else whackScore++;
            
            whackScoreText.text = $"Score: {whackScore} / {currentConfig.whackTarget}";
            ResetWhackBtn(whackCells[activeWhackCell]);
            activeWhackCell = -1;
            
            if (whackScore >= currentConfig.whackTarget) MarkWinReady();
        }
    }

    private void SetupCodeBreaker()
    {
        codeGuessRow.Clear();
        codeSymbolRow.Clear();
        codeHistoryList.Clear();
        codeHistoryList.style.display = DisplayStyle.None;
        currentGuess.Clear();
        codeSlotButtons.Clear();
        codeAttemptsRemaining = currentConfig.maxAttempts;
        codeAttemptsText.text = $"Attempts: {codeAttemptsRemaining}";
        
        secretCode = new string[currentConfig.codeLength];
        for(int i=0; i<currentConfig.codeLength; i++)
            secretCode[i] = currentConfig.codeSymbols[Random.Range(0, currentConfig.codeSymbols.Length)];
            
        for(int i=0; i<currentConfig.codeLength; i++)
        {
            var btn = new Button { text = "_" };
            btn.style.width = 80; btn.style.height = 80;
            btn.style.fontSize = 36;
            btn.style.marginTop = 5; btn.style.marginBottom = 5; btn.style.marginLeft = 5; btn.style.marginRight = 5;
            codeGuessRow.Add(btn);
            codeSlotButtons.Add(btn);
        }
        
        foreach(var sym in currentConfig.codeSymbols)
        {
            var btn = new Button { text = sym };
            btn.style.width = 80; btn.style.height = 80;
            btn.style.fontSize = 36;
            btn.style.marginTop = 5; btn.style.marginBottom = 5; btn.style.marginLeft = 5; btn.style.marginRight = 5;
            btn.clicked += () => {
                if (currentGuess.Count < currentConfig.codeLength) {
                    currentGuess.Add(sym);
                    UpdateCodeSlots();
                }
            };
            codeSymbolRow.Add(btn);
        }
    }

    private void UpdateCodeSlots()
    {
        for (int i=0; i<currentConfig.codeLength; i++)
        {
            codeSlotButtons[i].text = i < currentGuess.Count ? currentGuess[i] : "_";
        }
    }

    private void OnCodeClear()
    {
        currentGuess.Clear();
        UpdateCodeSlots();
    }

    private void OnCodeSubmit()
    {
        if (!isPlaying || currentGuess.Count < currentConfig.codeLength) return;
        
        var histRow = new VisualElement();
        histRow.style.flexDirection = FlexDirection.Row;
        histRow.style.justifyContent = Justify.Center;
        histRow.style.marginBottom = 10;
        int correctPos = 0;
        
        for (int i=0; i<currentConfig.codeLength; i++)
        {
            var lbl = new Label(currentGuess[i]);
            lbl.style.width = 50; lbl.style.height = 50;
            lbl.style.marginTop = 5; lbl.style.marginBottom = 5; lbl.style.marginLeft = 5; lbl.style.marginRight = 5;
            lbl.style.fontSize = 32;
            lbl.style.unityTextAlign = TextAnchor.MiddleCenter;
            lbl.style.color = Color.white;
            lbl.style.borderTopLeftRadius = 8;
            lbl.style.borderTopRightRadius = 8;
            lbl.style.borderBottomLeftRadius = 8;
            lbl.style.borderBottomRightRadius = 8;
            lbl.style.unityFontStyleAndWeight = FontStyle.Bold;

            if (currentGuess[i] == secretCode[i]) {
                lbl.style.backgroundColor = new StyleColor(new Color(0.18f, 0.8f, 0.44f)); // Green
                correctPos++;
            }
            else if (secretCode.Contains(currentGuess[i])) {
                lbl.style.backgroundColor = new StyleColor(new Color(0.94f, 0.76f, 0.05f)); // Yellow
            }
            else {
                lbl.style.backgroundColor = new StyleColor(new Color(0.4f, 0.4f, 0.4f)); // Gray
            }
            histRow.Add(lbl);
        }
        
        codeHistoryList.style.display = DisplayStyle.Flex;
        codeHistoryList.Add(histRow);
        
        if (correctPos == currentConfig.codeLength) { MarkWinReady(); return; }
        
        codeAttemptsRemaining--;
        codeAttemptsText.text = $"Attempts: {codeAttemptsRemaining}";
        currentGuess.Clear();
        UpdateCodeSlots();
        
        if (codeAttemptsRemaining <= 0)
        {
            if (currentGame.canFail) { isPlaying = false; }
            else { SetupCodeBreaker(); } 
        }
    }

    private IEnumerator BalanceMeterRoutine()
    {
        while (isPlaying)
        {
            balanceDrift += Random.Range(-2f, 2f) * Time.deltaTime * currentConfig.balanceDriftSpeed;
            balanceDrift = Mathf.Clamp(balanceDrift, -10f, 10f);
            balanceValue += balanceDrift * Time.deltaTime;
            balanceValue = Mathf.Clamp(balanceValue, 0f, 100f);
            
            if (balanceNeedle != null) balanceNeedle.style.left = new Length(balanceValue, LengthUnit.Percent);
            
            if (balanceValue >= 35f && balanceValue <= 65f)
            {
                balanceTimeInZone += Time.deltaTime;
                float pct = Mathf.Clamp01(balanceTimeInZone / currentConfig.balanceHoldTime) * 100f;
                if (balanceTimeBar != null) balanceTimeBar.style.width = new Length(pct, LengthUnit.Percent);
                if (balanceTimeInZone >= currentConfig.balanceHoldTime) MarkWinReady();
            }
            yield return null;
        }
    }

    private void OnBalanceNudge(float amount)
    {
        if (!isPlaying || currentConfig.type != GameType.BalanceMeter) return;
        balanceValue += amount;
        balanceDrift *= 0.5f; 
    }

    private void UpdateDialogueUI()
    {
        if (dialogueRound >= currentConfig.dialogueScenarios.Length)
        {
            if (rapportValue >= currentConfig.rapportThreshold || !currentGame.canFail) MarkWinReady();
            else { isPlaying = false; } 
            return;
        }
        
        dialogueTimerMax = Mathf.Max(1.0f, 3f - (dialogueRound * 0.5f));
        dialogueTimer = dialogueTimerMax;
        
        dialogueRoundText.text = $"Round {dialogueRound + 1} / {currentConfig.dialogueScenarios.Length}";
        dialogueScenarioText.text = currentConfig.dialogueScenarios[dialogueRound];
        btnResponse1.text = currentConfig.dialogueOptions[dialogueRound][0];
        btnResponse2.text = currentConfig.dialogueOptions[dialogueRound][1];
        btnResponse3.text = currentConfig.dialogueOptions[dialogueRound][2];
        if (rapportBar != null) rapportBar.style.width = new Length(rapportValue * 100f, LengthUnit.Percent);
    }

    private void OnDialogueResponse(int idx)
    {
        if (!isPlaying || currentConfig.type != GameType.DialogueChain) return;
        if (idx == currentConfig.dialogueCorrectIndices[dialogueRound]) rapportValue += 0.25f;
        else rapportValue -= 0.15f;
        rapportValue = Mathf.Clamp01(rapportValue);
        dialogueRound++;
        UpdateDialogueUI();
    }
    
    private IEnumerator DialogueChainRoutine()
    {
        while (isPlaying && dialogueRound < currentConfig.dialogueScenarios.Length)
        {
            dialogueTimer -= Time.deltaTime;
            
            if (dialogueTimerBar != null)
            {
                float pct = Mathf.Clamp01(dialogueTimer / dialogueTimerMax);
                dialogueTimerBar.style.width = new Length(pct * 100f, LengthUnit.Percent);
                dialogueTimerBar.style.backgroundColor = pct > 0.5f ? new StyleColor(new Color(0.13f, 0.77f, 0.36f)) : (pct > 0.25f ? new StyleColor(new Color(0.96f, 0.62f, 0.04f)) : new StyleColor(Color.red));
            }
            
            if (dialogueTimer <= 0f)
            {
                if (currentGame.canFail)
                {
                    isPlaying = false;
                    yield break;
                }
                else
                {
                    // If it's a sandbox, just skip the round and penalize
                    rapportValue -= 0.15f;
                    rapportValue = Mathf.Clamp01(rapportValue);
                    dialogueRound++;
                    UpdateDialogueUI();
                }
            }
            yield return null;
        }
    }

    private IEnumerator OrderQueueRoutine()
    {
        orderCorrectSequence = currentConfig.orderItems.ToList();
        orderInputIndex = 0;
        orderIsPreview = true;
        
        orderStatusText.text = "Memorize the order!";
        orderItemGrid.Clear();
        foreach (var item in orderCorrectSequence)
        {
            var btn = new Button { text = item };
            btn.AddToClassList("popup-button");
            btn.style.width = new StyleLength(StyleKeyword.Auto);
            btn.style.paddingLeft = 20; btn.style.paddingRight = 20; btn.style.marginRight = 10; btn.style.marginBottom = 10; btn.style.fontSize = 24;
            
            if (currentGame.name == "Train new hire on POS system")
            {
                btn.style.backgroundColor = new StyleColor(new Color(0.05f, 0.25f, 0.05f));
                btn.style.color = new StyleColor(new Color(0.2f, 1f, 0.2f));
                btn.style.borderTopColor = new StyleColor(new Color(0.2f, 1f, 0.2f));
                btn.style.borderBottomColor = new StyleColor(new Color(0.2f, 1f, 0.2f));
                btn.style.borderLeftColor = new StyleColor(new Color(0.2f, 1f, 0.2f));
                btn.style.borderRightColor = new StyleColor(new Color(0.2f, 1f, 0.2f));
                btn.style.borderTopWidth = 2; btn.style.borderBottomWidth = 2; btn.style.borderLeftWidth = 2; btn.style.borderRightWidth = 2;
            }
            else if (currentGame.name == "Verify hotfix deployment")
            {
                Color ghBtnBg = ColorUtility.TryParseHtmlString("#21262d", out Color c1) ? c1 : new Color(0.13f, 0.15f, 0.17f);
                Color ghBtnText = ColorUtility.TryParseHtmlString("#c9d1d9", out Color c2) ? c2 : Color.white;
                Color ghBtnBorder = ColorUtility.TryParseHtmlString("#30363d", out Color c3) ? c3 : new Color(0.18f, 0.21f, 0.23f);

                btn.style.backgroundColor = new StyleColor(ghBtnBg);
                btn.style.color = new StyleColor(ghBtnText);
                btn.style.borderTopColor = new StyleColor(ghBtnBorder);
                btn.style.borderBottomColor = new StyleColor(ghBtnBorder);
                btn.style.borderLeftColor = new StyleColor(ghBtnBorder);
                btn.style.borderRightColor = new StyleColor(ghBtnBorder);
                btn.style.borderTopWidth = 1; btn.style.borderBottomWidth = 1; btn.style.borderLeftWidth = 1; btn.style.borderRightWidth = 1;
                btn.style.borderTopLeftRadius = 15; btn.style.borderTopRightRadius = 15; btn.style.borderBottomLeftRadius = 15; btn.style.borderBottomRightRadius = 15;
            }
            else
            {
                btn.style.backgroundColor = new StyleColor(new Color(0.22f, 0.74f, 0.97f)); // Better default blue
                btn.style.color = new StyleColor(Color.white);
            }
            orderItemGrid.Add(btn);
        }
        
        yield return new WaitForSeconds(currentConfig.orderPreviewTime);
        
        orderIsPreview = false;
        orderStatusText.text = "Select in order!";
        orderItemGrid.Clear();
        
        var scrambled = orderCorrectSequence.OrderBy(x => Random.value).ToList();
        foreach (var item in scrambled)
        {
            var btn = new Button { text = item };
            btn.AddToClassList("popup-button");
            btn.style.width = new StyleLength(StyleKeyword.Auto);
            btn.style.paddingLeft = 20; btn.style.paddingRight = 20; btn.style.marginRight = 10; btn.style.marginBottom = 10; btn.style.fontSize = 24;
            
            if (currentGame.name == "Train new hire on POS system")
            {
                btn.style.backgroundColor = new StyleColor(new Color(0.02f, 0.15f, 0.02f));
                btn.style.color = new StyleColor(new Color(0.2f, 1f, 0.2f));
                btn.style.borderTopColor = new StyleColor(new Color(0.2f, 1f, 0.2f));
                btn.style.borderBottomColor = new StyleColor(new Color(0.2f, 1f, 0.2f));
                btn.style.borderLeftColor = new StyleColor(new Color(0.2f, 1f, 0.2f));
                btn.style.borderRightColor = new StyleColor(new Color(0.2f, 1f, 0.2f));
                btn.style.borderTopWidth = 2; btn.style.borderBottomWidth = 2; btn.style.borderLeftWidth = 2; btn.style.borderRightWidth = 2;
            }
            else if (currentGame.name == "Verify hotfix deployment")
            {
                Color ghBtnBg = ColorUtility.TryParseHtmlString("#21262d", out Color c1) ? c1 : new Color(0.13f, 0.15f, 0.17f);
                Color ghBtnText = ColorUtility.TryParseHtmlString("#c9d1d9", out Color c2) ? c2 : Color.white;
                Color ghBtnBorder = ColorUtility.TryParseHtmlString("#30363d", out Color c3) ? c3 : new Color(0.18f, 0.21f, 0.23f);

                btn.style.backgroundColor = new StyleColor(ghBtnBg);
                btn.style.color = new StyleColor(ghBtnText);
                btn.style.borderTopColor = new StyleColor(ghBtnBorder);
                btn.style.borderBottomColor = new StyleColor(ghBtnBorder);
                btn.style.borderLeftColor = new StyleColor(ghBtnBorder);
                btn.style.borderRightColor = new StyleColor(ghBtnBorder);
                btn.style.borderTopWidth = 1; btn.style.borderBottomWidth = 1; btn.style.borderLeftWidth = 1; btn.style.borderRightWidth = 1;
                btn.style.borderTopLeftRadius = 15; btn.style.borderTopRightRadius = 15; btn.style.borderBottomLeftRadius = 15; btn.style.borderBottomRightRadius = 15;
            }
            else
            {
                btn.style.color = new StyleColor(Color.white);
            }
            
            btn.clicked += () => OnOrderItemClicked(item, btn);
            orderItemGrid.Add(btn);
        }
    }

    private void OnOrderItemClicked(string item, Button btn)
    {
        if (!isPlaying || orderIsPreview || currentConfig.type != GameType.OrderQueue) return;
        
        if (item == orderCorrectSequence[orderInputIndex])
        {
            btn.style.backgroundColor = new StyleColor(Color.green);
            btn.SetEnabled(false);
            orderInputIndex++;
            if (orderInputIndex >= orderCorrectSequence.Count) MarkWinReady();
        }
        else
        {
            if (currentGame.canFail) { activeGameRoutine = StartCoroutine(OrderQueueRoutine()); }
            else { btn.style.backgroundColor = new StyleColor(Color.red); }
        }
    }

    private void SetupDragMatch()
    {
        dragLeftColumn.Clear();
        dragRightColumn.Clear();
        dragCheatSheetList.Clear();
        dragLeftButtons.Clear();
        dragRightButtons.Clear();
        dragCorrectMap.Clear();
        dragMatchCount = 0;
        dragSelectedLeft = -1;
        
        var lefts = currentConfig.matchLeft.ToList();
        var rights = currentConfig.matchRight.ToList();
        var rightIndices = Enumerable.Range(0, rights.Count).OrderBy(x => Random.value).ToList();
        
        for (int i=0; i<lefts.Count; i++)
        {
            var lbl = new Label($"{lefts[i]} = {rights[rightIndices[i]]}");
            lbl.style.fontSize = 22;
            lbl.style.color = new StyleColor(new Color(0.1f, 0.1f, 0.1f)); // Dark grey/black ink
            lbl.style.whiteSpace = WhiteSpace.NoWrap;
            lbl.style.marginBottom = 10;
            lbl.style.paddingBottom = 8;
            lbl.style.paddingTop = 8;
            lbl.style.borderBottomWidth = 1;
            lbl.style.borderBottomColor = new StyleColor(new Color(0.3f, 0.6f, 0.9f, 0.5f)); // Light blue ruler line
            lbl.style.unityTextAlign = TextAnchor.MiddleLeft;
            dragCheatSheetList.Add(lbl);
            
            int idx = i;
            var lBtn = new Button { text = lefts[i] };
            lBtn.AddToClassList("popup-button");
            lBtn.style.paddingTop = 15; lBtn.style.paddingBottom = 15; lBtn.style.width = new Length(100, LengthUnit.Percent); lBtn.style.fontSize = 20;
            lBtn.clicked += () => { dragSelectedLeft = idx; foreach(var b in dragLeftButtons) b.style.backgroundColor = new StyleColor(Color.gray); lBtn.style.backgroundColor = new StyleColor(Color.yellow); };
            dragLeftButtons.Add(lBtn);
            dragLeftColumn.Add(lBtn);
            
            int rIdx = rightIndices[i];
            dragCorrectMap[i] = rIdx;
        }
        
        // Empty bottom line for notepad aesthetic
        var emptyLbl = new Label(" ");
        emptyLbl.style.paddingBottom = 8;
        emptyLbl.style.paddingTop = 8;
        emptyLbl.style.borderBottomWidth = 1;
        emptyLbl.style.borderBottomColor = new StyleColor(new Color(0.3f, 0.6f, 0.9f, 0.5f));
        dragCheatSheetList.Add(emptyLbl);
        
        for (int i=0; i<rights.Count; i++)
        {
            int idx = i;
            var rBtn = new Button { text = rights[idx] };
            rBtn.AddToClassList("popup-button");
            rBtn.style.paddingTop = 15; rBtn.style.paddingBottom = 15; rBtn.style.width = new Length(100, LengthUnit.Percent); rBtn.style.fontSize = 20;
            rBtn.clicked += () => OnDragRightClicked(idx, rBtn);
            dragRightButtons.Add(rBtn);
            dragRightColumn.Add(rBtn);
        }
    }

    private void OnDragRightClicked(int rIdx, Button rBtn)
    {
        if (!isPlaying || dragSelectedLeft == -1 || currentConfig.type != GameType.DragMatch) return;
        
        if (dragCorrectMap[dragSelectedLeft] == rIdx)
        {
            dragLeftButtons[dragSelectedLeft].style.backgroundColor = new StyleColor(Color.green);
            dragLeftButtons[dragSelectedLeft].SetEnabled(false);
            rBtn.style.backgroundColor = new StyleColor(Color.green);
            rBtn.SetEnabled(false);
            dragMatchCount++;
            if (dragMatchCount >= currentConfig.matchLeft.Length) MarkWinReady();
        }
        else
        {
            dragLeftButtons[dragSelectedLeft].style.backgroundColor = new StyleColor(Color.gray);
        }
        dragSelectedLeft = -1;
    }

    private IEnumerator SprintLaneRoutine()
    {
        while (isPlaying && sprintDistance < currentConfig.sprintObstacles)
        {
            sprintDistanceText.text = $"Distance: {sprintDistance} / {currentConfig.sprintObstacles}";
            sprintObstacleIsHigh = Random.value > 0.5f;
            sprintObstacleText.text = sprintObstacleIsHigh ? "HIGH OBSTACLE! (Duck)" : "LOW OBSTACLE! (Jump)";
            sprintWaitingForInput = true;
            
            float t = currentConfig.sprintSpeed;
            while (t > 0 && sprintWaitingForInput && isPlaying)
            {
                t -= Time.deltaTime;
                if (sprintTimerText != null) sprintTimerText.text = $"React: {t:F1}s";
                yield return null;
            }
            
            if (sprintWaitingForInput && currentGame.canFail) { isPlaying = false; yield break; }
            sprintWaitingForInput = false;
            yield return new WaitForSeconds(0.2f);
        }
        if (sprintDistance >= currentConfig.sprintObstacles) MarkWinReady();
    }

    private void OnSprintReact(bool jump)
    {
        if (!isPlaying || currentConfig.type != GameType.SprintLane || !sprintWaitingForInput) return;
        
        bool success = (jump && !sprintObstacleIsHigh) || (!jump && sprintObstacleIsHigh);
        if (success)
        {
            sprintDistance++;
            float pct = Mathf.Clamp01((float)sprintDistance / currentConfig.sprintObstacles) * 100f;
            if (sprintProgressBar != null) sprintProgressBar.style.width = new Length(pct, LengthUnit.Percent);
            sprintWaitingForInput = false;
        }
        else if (currentGame.canFail)
        {
            isPlaying = false;
        }
    }

    private void MarkWinReady()
    {
        if (currentConfig.type == GameType.TwoPhaseHeist)
        {
            if (currentPhase == 1) {
                currentPhase = 2;
                if (instructionsText != null) instructionsText.text = "Phase 2: HACK!";
                SetupGameType(currentConfig.phase2Type);
                return;
            } else if (currentPhase == 2 && currentConfig.phase3Type != 0) {
                currentPhase = 3;
                if (instructionsText != null) instructionsText.text = "Phase 3: ESCAPE!";
                SetupGameType(currentConfig.phase3Type);
                return;
            }
        }

        isPlaying = false;
        hasWon = true;
        if (activeGameRoutine != null) StopCoroutine(activeGameRoutine);

        FinishGame();
    }

    private void AbortGame()
    {
        isPlaying = false;
        if (activeGameRoutine != null) StopCoroutine(activeGameRoutine);

        overlay.style.display = DisplayStyle.None;

        if (currentGame != null && currentGame.category == "side_hustle")
        {
            int jailWeeks = 0;
            if (currentGame.name == "Forge Transit Passes") jailWeeks = 1;
            else if (currentGame.name == "Crack Luxury Briefcase") jailWeeks = 2;
            else if (currentGame.name == "Skim ATM Card Data") jailWeeks = 4;
            else if (currentGame.name == "Corporate Espionage Heist") jailWeeks = 7;
            
            if (jailWeeks > 0)
            {
                UnityEngine.Debug.Log("Failed illegal minigame! Straight to jail.");
                var gData = GameManager.Instance.currentData;
                float fine = (float)(gData.balance * 0.5); 
                gData.balance -= fine;
                gData.jailWeeksRemaining = jailWeeks;
                string logMessage = $"[SYSTEM] Failed {currentGame.name} AND WAS CAUGHT! Fined ${fine:F0} and jailed.";
                GameManager.Instance.activeNewsFeed = logMessage;
                GameManager.Instance.financialHistory.Add(new FinancialSnapshot(gData.currentWeek, gData.balance, 0, -fine));
                
                var gUIManager = FindAnyObjectByType<GameUIManager>();
                if (gUIManager != null) gUIManager.CheckJailStatus();
                return; 
            }
        }

        overlay.pickingMode = PickingMode.Ignore;
        var data = GameManager.Instance.currentData;

        if (currentGame.category == "side_hustle")
        {
            var hustle = HustlesUI.availableHustles.Find(h => h.name == currentGame.name);
            if (hustle != null && hustle.riskPercent > 0)
            {
                data.jailWeeksRemaining = hustle.jailSentence;
                data.jobTitle = "Unemployed";
                data.zone = "Slums";
                double fine = data.balance * 0.20;
                data.balance -= fine;
            }
            else
            {
                data.stress = Mathf.Clamp(data.stress + 0.15f, 0f, 1f);
            }
        }
        else
        {
            if (currentGame.canFail) data.stress = Mathf.Clamp(data.stress + 0.10f, 0f, 1f);
        }

        GameManager.Instance.SaveGame();
        var uiManager = FindAnyObjectByType<GameUIManager>();
        uiManager?.CheckJailStatus();
    }

    private void FinishGame()
    {
        if (!hasWon) return;

        isPlaying = false;
        overlay.style.display = DisplayStyle.None;
        overlay.pickingMode = PickingMode.Ignore;
        var data = GameManager.Instance.currentData;

        if (currentGame.category == "side_hustle")
        {
            var hustle = HustlesUI.availableHustles.Find(h => h.name == currentGame.name);
            string logMessage = taskLogMessages.ContainsKey(currentGame.name) ? taskLogMessages[currentGame.name] : $"I completed {currentGame.name}.";
            
            if (hustle != null && hustle.riskPercent > 0)
            {
                int roll = Random.Range(0, 100);
                if (roll < hustle.riskPercent)
                {
                    data.jailWeeksRemaining = hustle.jailSentence;
                    data.jobTitle = "Unemployed";
                    data.zone = "Slums";
                    double fine = data.balance * 0.20;
                    data.balance -= fine;
                    data.historyLogsJson += $@"

[SYSTEM] {logMessage} BUT I WAS CAUGHT! Fined ${fine:F0} and jailed.";
                }
                else
                {
                    data.balance += currentGame.reward;
                    data.historyLogsJson += $@"

[SYSTEM] {logMessage} Got away with ${currentGame.reward}.";
                }
            }
            else
            {
                data.balance += currentGame.reward;
                data.stress = Mathf.Clamp(data.stress + 0.10f, 0f, 1f);
                data.historyLogsJson += $@"

[SYSTEM] {logMessage} Earned ${currentGame.reward}.";
            }
            
            var uiManager = FindAnyObjectByType<GameUIManager>();
            uiManager?.ShowPopup(new RandomEvent { 
                title = "HUSTLE COMPLETED", 
                description = $@"You finished: {currentGame.name}.

Check the dashboard for results!" 
            }, () => { 
                GameManager.Instance.CheckHeartAttack(() => {
                    HustlesUI.Instance?.PopulateHustlesBoard(); 
                    uiManager?.SwitchToDashboard();
                });
            });
        }
        else if (currentGame.category == "wellness")
        {
            data.balance -= currentGame.reward;
            data.stress = Mathf.Clamp(data.stress - currentGame.stressCost, 0f, 1f);
            
            string logMessage = taskLogMessages.ContainsKey(currentGame.name) ? taskLogMessages[currentGame.name] : $"You completed {currentGame.name}.";
            data.historyLogsJson += $@"

[SYSTEM] {logMessage} -{(currentGame.stressCost * 100):F0}% stress";
            
            var uiManager = FindAnyObjectByType<GameUIManager>();
            uiManager?.ShowPopup(new RandomEvent { 
                title = "WELLNESS PURCHASED", 
                description = $@"You successfully completed: {currentGame.name}.

<color=#2ECC71>Your stress index has dropped significantly.</color>

<color=#E74C3C>Cost: ${currentGame.reward}</color>" 
            }, () => { 
                GameManager.Instance.CheckHeartAttack(() => {
                    uiManager?.SwitchToDashboard();
                });
            });
        }
        else if (currentGame.category == "mixer")
        {
            data.balance -= currentGame.reward;
            if (currentGame.name == "Industrial Dive Bar") data.luckManualService = Mathf.Clamp(data.luckManualService + currentGame.stressCost, 0f, 1f);
            else if (currentGame.name == "Hospitality Gala") data.luckHospitality = Mathf.Clamp(data.luckHospitality + currentGame.stressCost, 0f, 1f);
            else if (currentGame.name == "Underground Tech Meetup") data.luckTechnology = Mathf.Clamp(data.luckTechnology + currentGame.stressCost, 0f, 1f);
            else if (currentGame.name == "Corporate Golf Sim") data.luckCorporate = Mathf.Clamp(data.luckCorporate + currentGame.stressCost, 0f, 1f);

            string logMessage = taskLogMessages.ContainsKey(currentGame.name) ? taskLogMessages[currentGame.name] : $"You completed {currentGame.name}.";
            data.historyLogsJson += $@"

[SYSTEM] {logMessage} +{(currentGame.stressCost * 100):F0}% luck";
            
            var uiManager = FindAnyObjectByType<GameUIManager>();
            uiManager?.ShowPopup(new RandomEvent { 
                title = "NETWORK MIXER ATTENDED", 
                description = $@"You successfully networked at {currentGame.name}.

<color=#2ECC71>Your promotion luck in this sector has increased!</color>

<color=#E74C3C>Cost: ${currentGame.reward}</color>" 
            }, () => { 
                GameManager.Instance.CheckHeartAttack(() => {
                    uiManager?.SwitchToDashboard();
                });
            });
        }
        else if (currentGame.category == "work_regular")
        {
            data.completedTasksThisWeek.Add(currentGame.name);
            data.stress = Mathf.Clamp(data.stress + currentGame.stressCost, 0f, 1f);

            if (data.completedTasksThisWeek.Count >= 3)
            {
                GameManager.Instance.completedRegularTaskThisWeek = true;
            }

            string logMessage = taskLogMessages.ContainsKey(currentGame.name) ? taskLogMessages[currentGame.name] : $"You completed {currentGame.name}.";
            data.historyLogsJson += $@"

[SYSTEM] {logMessage} +{(currentGame.stressCost * 100):F0}% stress";
            
            var uiManager = FindAnyObjectByType<GameUIManager>();
            uiManager?.ShowPopup(new RandomEvent { 
                title = "TASK COMPLETED", 
                description = $@"You finished: {currentGame.name}.

<color=#E74C3C>Stress increased by {(currentGame.stressCost * 100):F0}%.</color>

Weekly Tasks Done: {data.completedTasksThisWeek.Count}/3" 
            }, () => { 
                GameManager.Instance.CheckHeartAttack(() => {
                    CareerUI.Instance.RefreshView(); 
                });
            });
        }
        else if (currentGame.category == "work_extra")
        {
            PlayerPrefs.SetInt("completedExtraTaskThisWeek", 1);
            data.completedTasksThisWeek.Add(currentGame.name);
            data.stress = Mathf.Clamp(data.stress + currentGame.stressCost, 0f, 1f);
            data.promotionProgress = Mathf.Clamp(data.promotionProgress + currentGame.readinessGain, 0f, 1f);
            
            string logMessage = taskLogMessages.ContainsKey(currentGame.name) ? taskLogMessages[currentGame.name] : $"You completed {currentGame.name}.";
            data.historyLogsJson += $@"

[SYSTEM] {logMessage} Promotion Readiness +{(currentGame.readinessGain * 100):F0}%";
            
            var uiManager = FindAnyObjectByType<GameUIManager>();
            uiManager?.ShowPopup(new RandomEvent { 
                title = "EXTRA INITIATIVE TAKEN", 
                description = $@"You went above and beyond completing {currentGame.name}.

<color=#2ECC71>Performance Readiness increased!</color>
<color=#E74C3C>Stress increased by {(currentGame.stressCost * 100):F0}%.</color>" 
            }, () => { 
                GameManager.Instance.CheckHeartAttack(() => {
                    CareerUI.Instance.RefreshView(); 
                });
            });
        }
        else
        {
            data.stress = Mathf.Clamp(data.stress + currentGame.stressCost, 0f, 1f);
            if (currentGame.reward > 0) data.balance += currentGame.reward;
        }

        GameManager.Instance.SaveGame();
        var uiManagerForDashboard = FindAnyObjectByType<GameUIManager>();
        uiManagerForDashboard?.CheckJailStatus();
    }

    private void SetupScrubPlate()
    {
        scrubProgress = 0f;
        scrubProgressBar.value = 0f;
        scrubProgressBar.highValue = currentConfig.targetScore;
        scrubDirtOverlay.style.opacity = 1f;
        scrubSponge.style.left = 110f;
        scrubSponge.style.top = 120f;
        lastScrubPos = Vector2.zero;

        var plateIcon = scrubPlateArea.Q<Label>("ScrubTargetIcon");
        if (plateIcon != null && currentGame != null)
        {
            if (currentGame.name == "Scour microwave grease") plateIcon.text = "📺";
            else if (currentGame.name == "Deep clean grease traps") plateIcon.text = "🛢️";
            else if (currentGame.name == "Wipe smog off the neon arrow") plateIcon.text = "➡️";
            else plateIcon.text = "🍽️";
        }
        
        var spongeLabel = scrubPlateArea.Q<Label>("ScrubSponge");
        if (spongeLabel != null && currentGame != null)
        {
            if (currentGame.name == "Wipe smog off the neon arrow") spongeLabel.text = "🧻";
            else spongeLabel.text = "🧽";
        }
    }

    private void SetupSweepFloor()
    {
        sweepFloorGrid.Clear();
        sweepTileTimers = new float[25];
        sweepTiles = new VisualElement[25];
        sweepPlayerX = 0;
        sweepPlayerY = 0;
        
        for (int y = 0; y < 5; y++)
        {
            var row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.flexGrow = 1;
            for (int x = 0; x < 5; x++)
            {
                var cell = new VisualElement();
                cell.style.flexGrow = 1;
                cell.style.borderTopWidth = 2; cell.style.borderLeftWidth = 2; cell.style.borderRightWidth = 2; cell.style.borderBottomWidth = 2;
                cell.style.borderTopColor = new StyleColor(new Color(0.2f, 0.1f, 0.05f));
                cell.style.borderBottomColor = new StyleColor(new Color(0.2f, 0.1f, 0.05f));
                cell.style.borderLeftColor = new StyleColor(new Color(0.2f, 0.1f, 0.05f));
                cell.style.borderRightColor = new StyleColor(new Color(0.2f, 0.1f, 0.05f));
                cell.style.justifyContent = Justify.Center;
                cell.style.alignItems = Align.Center;
                cell.style.paddingTop = 0; cell.style.paddingBottom = 0; cell.style.paddingLeft = 0; cell.style.paddingRight = 0;
                cell.style.marginTop = 0; cell.style.marginBottom = 0; cell.style.marginLeft = 0; cell.style.marginRight = 0;
                
                var icon = new Label("");
                icon.style.position = Position.Absolute;
                icon.style.left = 0; icon.style.right = 0; icon.style.top = 0; icon.style.bottom = 0;
                icon.style.fontSize = 55;
                icon.style.paddingTop = 0; icon.style.paddingBottom = 0; icon.style.paddingLeft = 0; icon.style.paddingRight = 0;
                icon.style.marginTop = 0; icon.style.marginBottom = 0; icon.style.marginLeft = 0; icon.style.marginRight = 0;
                icon.style.unityTextAlign = TextAnchor.MiddleCenter;
                icon.name = "Icon";
                cell.Add(icon);
                
                int cx = x; int cy = y;
                cell.RegisterCallback<PointerDownEvent>(ev => {
                    if (!isPlaying || currentConfig == null || currentConfig.type != GameType.SweepFloor) return;
                    sweepPlayerX = cx; sweepPlayerY = cy;
                });
                cell.RegisterCallback<PointerEnterEvent>(ev => {
                    if (!isPlaying || currentConfig == null || currentConfig.type != GameType.SweepFloor) return;
                    if ((ev.pressedButtons & 1) != 0) { // left click or touch hold
                        sweepPlayerX = cx; sweepPlayerY = cy;
                    }
                });
                
                row.Add(cell);
                sweepTiles[y * 5 + x] = cell;
            }
            sweepFloorGrid.Add(row);
        }
    }

    private void RenderSweepFloor()
    {
        for (int i = 0; i < 25; i++)
        {
            var cell = sweepTiles[i];
            var icon = cell.Q<Label>("Icon");
            
            if (sweepTileTimers[i] > 0)
            {
                // Clean: light brown
                cell.style.backgroundColor = new StyleColor(new Color(0.76f, 0.6f, 0.42f));
            }
            else
            {
                // Dirty: dark brown
                cell.style.backgroundColor = new StyleColor(new Color(0.35f, 0.22f, 0.12f));
            }
            
            int cy = i / 5;
            int cx = i % 5;
            if (cx == sweepPlayerX && cy == sweepPlayerY)
            {
                if (currentGame != null && currentGame.name == "Mop grease spills") icon.text = "🪣";
                else if (currentGame != null && currentGame.name == "Clear conveyor dust filters") icon.text = "🌪️";
                else icon.text = "🧹";
            }
            else
            {
                icon.text = "";
            }
        }
    }

    private IEnumerator SweepFloorRoutine()
    {
        sweepTileTimers[sweepPlayerY * 5 + sweepPlayerX] = 15f;
        RenderSweepFloor();

        while (isPlaying && currentConfig.type == GameType.SweepFloor)
        {
            // Decay timers
            bool allClean = true;
            for (int i = 0; i < 25; i++)
            {
                if (sweepTileTimers[i] > 0)
                {
                    sweepTileTimers[i] -= Time.deltaTime;
                    if (sweepTileTimers[i] <= 0) sweepTileTimers[i] = 0;
                }
                if (sweepTileTimers[i] <= 0)
                {
                    allClean = false;
                }
            }

            // Movement logic is now instantly handled inside KeyDownEvent and PointerEvent for max speed!

            // Always update timer for current position
            sweepTileTimers[sweepPlayerY * 5 + sweepPlayerX] = 15f;
            RenderSweepFloor();

            if (allClean)
            {
                MarkWinReady();
            }

            yield return null;
        }
    }

    private IEnumerator ScrubPlateRoutine()
    {
        while (isPlaying && currentConfig.type == GameType.ScrubPlate)
        {
            yield return null;
        }
    }

    private IEnumerator PumpSystemRoutine()
    {
        while (isPlaying && currentConfig.type == GameType.PumpSystem)
        {
            if (currentConfig.pumpIsBalanceMode)
            {
                float decayRate = 15f; 
                pumpProgress -= decayRate * Time.deltaTime;
                pumpProgress = Mathf.Clamp(pumpProgress, 0f, 100f);
                
                float needleAngle = -120f + ((pumpProgress / 100f) * 240f);
                pumpDialNeedleRotator.style.rotate = new StyleRotate(new Rotate(new Angle(needleAngle)));
                
                float p = pumpProgress / 100f;
                if (p >= currentConfig.pumpBalanceTargetMin && p <= currentConfig.pumpBalanceTargetMax)
                {
                    pumpHoldTimer += Time.deltaTime;
                }
                else
                {
                    pumpHoldTimer -= Time.deltaTime * 0.5f; // Drain slowly when outside
                    pumpHoldTimer = Mathf.Max(0f, pumpHoldTimer);
                }
                
                pumpHoldTimerBar.style.width = Length.Percent((pumpHoldTimer / currentConfig.pumpBalanceHoldTime) * 100f);
                
                if (pumpHoldTimer >= currentConfig.pumpBalanceHoldTime)
                {
                    pumpHoldTimerBar.style.width = Length.Percent(100f);
                    MarkWinReady();
                }
            }
            else
            {
                float decayFactor = 1f + (pumpProgress / 100f) * 4f; 
                float decayRate = 18f; 
                
                pumpProgress -= decayRate * decayFactor * Time.deltaTime;
                pumpProgress = Mathf.Clamp(pumpProgress, 0f, 100f);
                
                pumpProgressBar.style.height = Length.Percent(pumpProgress);
                
                if (pumpProgress >= 100f)
                {
                    pumpProgressBar.style.height = Length.Percent(100f);
                    MarkWinReady();
                }
            }
            
            yield return null;
        }
    }

    // ==========================================
    // TETRIS ENGINE
    // ==========================================
    private int[,] tetrisBoard = new int[10, 15]; // 0=empty, 1=filled
    private int currentTetrisX, currentTetrisY, currentTetrisType, currentTetrisRot;
    private int tetrisScore = 0;
    private float tetrisTickTimer = 0f;
    private float tetrisTickRate = 0.2f;

    private readonly int[,,,] tetrominoes = {
        // I
        { { {0,0}, {1,0}, {2,0}, {3,0} }, { {1,-1}, {1,0}, {1,1}, {1,2} }, { {0,0}, {1,0}, {2,0}, {3,0} }, { {1,-1}, {1,0}, {1,1}, {1,2} } },
        // J
        { { {0,0}, {0,1}, {1,1}, {2,1} }, { {1,0}, {2,0}, {1,1}, {1,2} }, { {0,0}, {1,0}, {2,0}, {2,1} }, { {1,0}, {1,1}, {0,2}, {1,2} } },
        // L
        { { {2,0}, {0,1}, {1,1}, {2,1} }, { {1,0}, {1,1}, {1,2}, {2,2} }, { {0,0}, {1,0}, {2,0}, {0,1} }, { {0,0}, {1,0}, {1,1}, {1,2} } },
        // O
        { { {0,0}, {1,0}, {0,1}, {1,1} }, { {0,0}, {1,0}, {0,1}, {1,1} }, { {0,0}, {1,0}, {0,1}, {1,1} }, { {0,0}, {1,0}, {0,1}, {1,1} } },
        // S
        { { {1,0}, {2,0}, {0,1}, {1,1} }, { {1,0}, {1,1}, {2,1}, {2,2} }, { {1,0}, {2,0}, {0,1}, {1,1} }, { {1,0}, {1,1}, {2,1}, {2,2} } },
        // T
        { { {1,0}, {0,1}, {1,1}, {2,1} }, { {1,0}, {1,1}, {2,1}, {1,2} }, { {0,0}, {1,0}, {2,0}, {1,1} }, { {1,0}, {0,1}, {1,1}, {1,2} } },
        // Z
        { { {0,0}, {1,0}, {1,1}, {2,1} }, { {2,0}, {1,1}, {2,1}, {1,2} }, { {0,0}, {1,0}, {1,1}, {2,1} }, { {2,0}, {1,1}, {2,1}, {1,2} } }
    };

    private void SetupTetris()
    {
        tetrisGrid.Clear();
        for(int x=0; x<10; x++)
            for(int y=0; y<15; y++)
                tetrisBoard[x,y] = 0;
        
        tetrisScore = 0;
        tetrisScoreText.text = $"Stacked: 0 / 10";
        SpawnTetromino();
    }

    private void SpawnTetromino()
    {
        currentTetrisType = Random.Range(0, 7);
        currentTetrisRot = 0;
        currentTetrisX = 3;
        currentTetrisY = 0;
        
        if (CheckTetrisCollision(currentTetrisX, currentTetrisY, currentTetrisRot))
        {
            AbortGame(); // Topped out
        }
        else
        {
            DrawTetris();
        }
    }

    private bool CheckTetrisCollision(int x, int y, int rot)
    {
        for (int i=0; i<4; i++)
        {
            int bx = x + tetrominoes[currentTetrisType, rot, i, 0];
            int by = y + tetrominoes[currentTetrisType, rot, i, 1];
            
            if (bx < 0 || bx >= 10 || by >= 15) return true;
            if (by >= 0 && tetrisBoard[bx, by] != 0) return true;
        }
        return false;
    }

    private void LockTetromino()
    {
        for (int i=0; i<4; i++)
        {
            int bx = currentTetrisX + tetrominoes[currentTetrisType, currentTetrisRot, i, 0];
            int by = currentTetrisY + tetrominoes[currentTetrisType, currentTetrisRot, i, 1];
            if (bx >= 0 && bx < 10 && by >= 0 && by < 15)
                tetrisBoard[bx, by] = 1;
        }
        
        tetrisScore++;
        tetrisScoreText.text = $"Stacked: {tetrisScore} / 10";
        if (tetrisScore >= 10)
        {
            MarkWinReady();
            DrawTetris();
        }
        else
        {
            SpawnTetromino();
        }
    }

    private void DrawTetris()
    {
        tetrisGrid.Clear();
        
        for(int x=0; x<10; x++)
        {
            for(int y=0; y<15; y++)
            {
                if (tetrisBoard[x,y] != 0)
                    tetrisGrid.Add(CreateTetrisBlock(x, y));
            }
        }
        
        for (int i=0; i<4; i++)
        {
            int bx = currentTetrisX + tetrominoes[currentTetrisType, currentTetrisRot, i, 0];
            int by = currentTetrisY + tetrominoes[currentTetrisType, currentTetrisRot, i, 1];
            if (by >= 0 && by < 15)
                tetrisGrid.Add(CreateTetrisBlock(bx, by));
        }
    }

    private VisualElement CreateTetrisBlock(int x, int y)
    {
        var block = new VisualElement();
        block.style.width = 30;
        block.style.height = 30;
        block.style.position = Position.Absolute;
        block.style.left = x * 30;
        block.style.top = y * 30;
        block.style.backgroundColor = new StyleColor(new Color(0.15f, 0.15f, 0.15f));
        block.style.borderLeftWidth = 2; block.style.borderRightWidth = 2; block.style.borderTopWidth = 2; block.style.borderBottomWidth = 2;
        block.style.borderLeftColor = new StyleColor(new Color(0.8f, 0.1f, 0.1f));
        block.style.borderRightColor = new StyleColor(new Color(0.8f, 0.1f, 0.1f));
        block.style.borderTopColor = new StyleColor(new Color(0.8f, 0.1f, 0.1f));
        block.style.borderBottomColor = new StyleColor(new Color(0.8f, 0.1f, 0.1f));
        block.style.alignItems = Align.Center;
        block.style.justifyContent = Justify.Center;
        
        var xLabel = new Label("X");
        xLabel.style.color = new StyleColor(new Color(0.8f, 0.1f, 0.1f));
        xLabel.style.fontSize = 28;
        xLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        xLabel.style.position = Position.Absolute;
        xLabel.style.left = 0; xLabel.style.right = 0; xLabel.style.top = 0; xLabel.style.bottom = 0;
        xLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
        block.Add(xLabel);
        
        return block;
    }

    private void OnTetrisInput(int dx, int dy, int drot)
    {
        if (!isPlaying || currentConfig.type != GameType.Tetris) return;
        
        int nextX = currentTetrisX + dx;
        int nextY = currentTetrisY + dy;
        int nextRot = (currentTetrisRot + drot) % 4;
        
        if (!CheckTetrisCollision(nextX, nextY, nextRot))
        {
            currentTetrisX = nextX;
            currentTetrisY = nextY;
            currentTetrisRot = nextRot;
            DrawTetris();
        }
        else if (dy == 1) // Collision while moving down = lock in place
        {
            LockTetromino();
        }
    }

}

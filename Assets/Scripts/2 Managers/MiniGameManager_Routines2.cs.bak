using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class MiniGameManager : MonoBehaviour
{
    private IEnumerator TetrisRoutine()
    {
        tetrisTickTimer = 0f;
        while (isPlaying && currentConfig.type == GameType.Tetris)
        {
            tetrisTickTimer += Time.deltaTime;
            
            if (tetrisTickTimer >= tetrisTickRate)
            {
                tetrisTickTimer = 0f;
                OnTetrisInput(0, 1, 0); // Move down
            }
            yield return null;
        }
    }

    private IEnumerator AcidRainRoutine()
    {
        float duration = 15f;
        float elapsed = 0f;

        while (elapsed < duration && acidBurn < 100f)
        {
            elapsed += Time.deltaTime;
            float timeRemaining = duration - elapsed;
            if (timeRemaining < 0f) timeRemaining = 0f;
            acidTimerText.text = timeRemaining.ToString("0.0") + "s";

            // Update wind
            acidWindTimer += Time.deltaTime;
            if (acidWindTimer > 2f)
            {
                acidWindTimer = 0f;
                acidWindTarget = Random.Range(-50f, 50f);
            }
            acidWindAngle = Mathf.Lerp(acidWindAngle, acidWindTarget, Time.deltaTime * 1.5f);

            // Spawn drops
            acidDropTimer += Time.deltaTime;
            if (acidDropTimer >= 0.25f)
            {
                acidDropTimer = 0f;
                SpawnAcidDrop();
            }

            // Update drops
            for (int i = activeAcidDrops.Count - 1; i >= 0; i--)
            {
                var drop = activeAcidDrops[i];
                drop.radius -= drop.speed * Time.deltaTime;

                float angleRad = drop.angle * Mathf.Deg2Rad;
                float x = drop.radius * Mathf.Sin(angleRad);
                float y = drop.radius * Mathf.Cos(angleRad);

                drop.element.style.left = Length.Percent(50f);
                drop.element.style.translate = new StyleTranslate(new Translate(new Length(x, LengthUnit.Pixel), new Length(-y, LengthUnit.Pixel)));

                // Sign is placed at radius 150
                if (drop.radius <= 165f && drop.radius >= 135f)
                {
                    float diff = Mathf.Abs(Mathf.DeltaAngle(drop.angle, acidSignAngle));
                    if (diff <= 35f)
                    {
                        // Blocked
                        drop.element.RemoveFromHierarchy();
                        activeAcidDrops.RemoveAt(i);
                        continue;
                    }
                }

                if (drop.radius <= 10f)
                {
                    drop.element.RemoveFromHierarchy();
                    activeAcidDrops.RemoveAt(i);
                    
                    acidBurn += 8f;
                    if (acidBurn > 100f) acidBurn = 100f;
                    
                    acidBurnBar.style.width = Length.Percent(acidBurn);
                    if (acidBurn > 60f) acidBurnBar.style.backgroundColor = new StyleColor(new Color(0.9f, 0.3f, 0.23f)); // Red
                    else if (acidBurn > 30f) acidBurnBar.style.backgroundColor = new StyleColor(new Color(0.94f, 0.76f, 0.05f)); // Yellow
                }
            }

            yield return null;
        }

        foreach (var drop in activeAcidDrops)
        {
            drop.element.RemoveFromHierarchy();
        }
        activeAcidDrops.Clear();

        if (acidBurn < 100f)
        {
            MarkWinReady();
        }
        else
        {
            if (currentGame.canFail) {
                AbortGame(); 
            } else {
                MarkWinReady(); 
            }
        }
    }

    private void SpawnAcidDrop()
    {
        var dropEl = new VisualElement();
        dropEl.style.position = Position.Absolute;
        dropEl.style.width = 12;
        dropEl.style.height = 12;
        dropEl.style.backgroundColor = new StyleColor(new Color(0.18f, 0.8f, 0.44f));
        dropEl.style.borderTopLeftRadius = 6;
        dropEl.style.borderTopRightRadius = 6;
        dropEl.style.borderBottomLeftRadius = 6;
        dropEl.style.borderBottomRightRadius = 6;
        dropEl.style.bottom = 0; 
        
        acidDropContainer.Add(dropEl);

        float spawnAngle = acidWindAngle + Random.Range(-15f, 15f);
        
        activeAcidDrops.Add(new AcidDrop {
            element = dropEl,
            radius = 450f,
            angle = spawnAngle,
            speed = Random.Range(200f, 300f)
        });
    }

    private IEnumerator FoodPrepRoutine()
    {
        for (int i = 0; i < 4; i++) prepSpeeds[i] = Random.Range(30f, 60f);

        while (isPlaying && score < currentConfig.targetScore && timer > 0f)
        {
            timer -= Time.deltaTime;
            prepTimerText.text = $"{timer:0.0}s";

            for (int i = 0; i < 4; i++)
            {
                if (prepFreezes[i] > 0f)
                {
                    prepFreezes[i] -= Time.deltaTime;
                    if (prepFreezes[i] <= 0f)
                    {
                        prepFreezes[i] = 0f;
                        prepStatus[i].text = "";
                        prepProgress[i] = 0f;
                        prepSpeeds[i] = Random.Range(30f, 60f);
                    }
                }
                else
                {
                    prepProgress[i] += prepSpeeds[i] * Time.deltaTime;
                    if (prepProgress[i] >= 100f)
                    {
                        prepProgress[i] = 100f;
                        prepStatus[i].text = (i == 2) ? "SPILLED!" : "BURNED!";
                        prepFreezes[i] = 2.0f;
                        if (currentGame.canFail) score = Mathf.Max(0, score - 1);
                        prepScoreText.text = $"Prepped: {score} / {currentConfig.targetScore}";
                    }
                }
                prepBars[i].style.height = new Length(prepProgress[i], LengthUnit.Percent);
            }

            if (timer <= 0f)
            {
                if (currentGame.canFail)
                {
                    prepTimerText.text = "TIME UP!";
                    isPlaying = false;
                    yield break;
                }
                else
                {
                    MarkWinReady();
                }
            }

            yield return null;
        }
    }

    private void OnFoodPrepStationTapped(int index)
    {
        if (!isPlaying || currentConfig.type != GameType.FoodPrepRush) return;
        if (prepFreezes[index] > 0f) return; // Station is frozen

        float p = prepProgress[index];
        if (p < 70f)
        {
            prepStatus[index].text = "RAW!";
            prepFreezes[index] = 1.0f;
            if (currentGame.canFail) score = Mathf.Max(0, score - 1);
        }
        else if (p >= 70f && p <= 95f)
        {
            prepStatus[index].text = "PERFECT!";
            prepFreezes[index] = 0.5f;
            score++;
            if (score >= currentConfig.targetScore)
            {
                prepTimerText.text = "YOU WIN!";
                MarkWinReady();
            }
        }
        else
        {
            prepStatus[index].text = (index == 2) ? "SPILLED!" : "BURNED!";
            prepFreezes[index] = 2.0f;
            if (currentGame.canFail) score = Mathf.Max(0, score - 1);
        }
        
        prepScoreText.text = $"Prepped: {score} / {currentConfig.targetScore}";
    }

    private void SetupVideoCall()
    {
        vcCurrentPhase = 1;
        vcWiggleProgress = 0f;
        vcWiggleReady = false;
        vcWiggleWaitTimer = Random.Range(1.0f, 2.5f);
        
        vcPhase1.style.display = DisplayStyle.Flex;
        vcPhase2.style.display = DisplayStyle.None;
        vcPhase3.style.display = DisplayStyle.None;
        
        vcScrollView.scrollOffset = Vector2.zero;
        vcWiggleProgressBar.style.width = new Length(0, LengthUnit.Percent);
        vcWigglePromptText.text = "SCREEN ASLEEP...";
        vcWigglePromptText.style.color = new StyleColor(new Color(0.5f, 0.55f, 0.55f)); // #7F8C8D
        
        vcMousePos = new Vector2(115f, 120f);
        if (vcMouseGraphic != null)
        {
            vcMouseGraphic.style.left = vcMousePos.x;
            vcMouseGraphic.style.top = vcMousePos.y;
        }
        
        activeGameRoutine = StartCoroutine(VideoCallRoutine());
    }

    private IEnumerator VideoCallRoutine()
    {
        float timer = currentConfig.timeLimit;
        isPlaying = true;
        
        while (isPlaying)
        {
            timer -= Time.deltaTime;
            vcTimerText.text = $"{Mathf.Max(0, timer):F1}s";
            
            if (vcCurrentPhase == 3)
            {
                if (!vcWiggleReady)
                {
                    vcWiggleWaitTimer -= Time.deltaTime;
                    if (vcWiggleWaitTimer <= 0f)
                    {
                        vcWiggleReady = true;
                        vcWigglePromptText.text = "SWIPE TO WAKE!";
                        vcWigglePromptText.style.color = new StyleColor(new Color(0.18f, 0.8f, 0.44f)); // Green
                    }
                }
                else
                {
                    // Wiggle decay if not wiggling enough
                    vcWiggleProgress = Mathf.Max(0f, vcWiggleProgress - (200f * Time.deltaTime));
                    vcWiggleProgressBar.style.width = new Length(Mathf.Min(100f, vcWiggleProgress / 20f), LengthUnit.Percent); // 2000 total to win
                }
            }

            if (timer <= 0f)
            {
                if (currentGame.canFail)
                {
                    vcTimerText.text = "TIME UP!";
                    isPlaying = false;
                    yield break;
                }
                else
                {
                    MarkWinReady();
                }
            }
            yield return null;
        }
    }

    private void OnVcAcknowledgeTapped()
    {
        if (!isPlaying || vcCurrentPhase != 1) return;
        
        // Advance to phase 2
        vcCurrentPhase = 2;
        vcPhase1.style.display = DisplayStyle.None;
        vcPhase2.style.display = DisplayStyle.Flex;
    }

    private void OnVcRaiseHandTapped()
    {
        if (!isPlaying || vcCurrentPhase != 2) return;
        
        // Advance to phase 3
        vcCurrentPhase = 3;
        vcPhase2.style.display = DisplayStyle.None;
        vcPhase3.style.display = DisplayStyle.Flex;
    }

    private void OnVcWiggleMove(PointerMoveEvent evt)
    {
        if (!isPlaying || vcCurrentPhase != 3 || !vcWiggleReady) return;
        if (evt.pressedButtons != 0)
        {
            // Move mouse graphic
            vcMousePos.x = Mathf.Clamp(vcMousePos.x + evt.deltaPosition.x, 0f, 230f); // 300 - 70
            vcMousePos.y = Mathf.Clamp(vcMousePos.y + evt.deltaPosition.y, 0f, 240f); // 350 - 110
            if (vcMouseGraphic != null)
            {
                vcMouseGraphic.style.left = vcMousePos.x;
                vcMouseGraphic.style.top = vcMousePos.y;
            }

            float moveDist = Mathf.Abs(evt.deltaPosition.x) + Mathf.Abs(evt.deltaPosition.y);
            if (moveDist > 5f)
            {
                vcWiggleProgress += moveDist * 2f;
                float percent = Mathf.Min(100f, vcWiggleProgress / 30f); // 3000px total
                vcWiggleProgressBar.style.width = new Length(percent, LengthUnit.Percent);
                
                if (percent >= 100f)
                {
                    vcWigglePromptText.text = "ONLINE!";
                    MarkWinReady();
                }
            }
        }
    }

    private void SetupKanbanBoard()
    {
        kanbanColTodo.Clear();
        kanbanColProgress.Clear();
        kanbanColDone.Clear();
        kanbanTickets.Clear();
        kanbanTicketsDone = 0;
        
        string[] ticketPrefixes = new string[]{"BUG-", "DEV-", "UX-", "OPS-", "QA-"};

        for (int i = 0; i < currentConfig.kanbanTicketCount; i++)
        {
            var ticket = new VisualElement();
            ticket.style.width = new Length(90, LengthUnit.Percent);
            ticket.style.height = 40;
            ticket.style.backgroundColor = new StyleColor(new Color(0.95f, 0.85f, 0.4f)); // Yellow sticky
            ticket.style.marginBottom = 10;
            ticket.style.borderTopLeftRadius = 5;
            ticket.style.borderTopRightRadius = 5;
            ticket.style.borderBottomLeftRadius = 5;
            ticket.style.borderBottomRightRadius = 5;
            ticket.style.borderTopWidth = 1;
            ticket.style.borderBottomWidth = 1;
            ticket.style.borderLeftWidth = 1;
            ticket.style.borderRightWidth = 1;
            ticket.style.borderTopColor = new StyleColor(new Color(0.7f, 0.6f, 0.2f));
            ticket.style.borderBottomColor = new StyleColor(new Color(0.7f, 0.6f, 0.2f));
            ticket.style.borderLeftColor = new StyleColor(new Color(0.7f, 0.6f, 0.2f));
            ticket.style.borderRightColor = new StyleColor(new Color(0.7f, 0.6f, 0.2f));
            ticket.style.justifyContent = Justify.Center;
            ticket.style.alignItems = Align.Center;
            ticket.style.position = Position.Relative;
            
            var label = new Label($"{ticketPrefixes[Random.Range(0, ticketPrefixes.Length)]}{Random.Range(100, 999)}");
            label.style.color = Color.black;
            label.style.fontSize = 18;
            label.style.unityFontStyleAndWeight = FontStyle.Bold;
            ticket.Add(label);

            ticket.RegisterCallback<PointerDownEvent>(evt => OnKanbanTicketDown(evt, ticket));
            ticket.RegisterCallback<PointerMoveEvent>(evt => OnKanbanTicketMove(evt, ticket));
            ticket.RegisterCallback<PointerUpEvent>(evt => OnKanbanTicketUp(evt, ticket));

            kanbanColTodo.Add(ticket);
            kanbanTickets.Add(ticket);
        }

        activeGameRoutine = StartCoroutine(KanbanBoardRoutine());
    }

    private void OnKanbanTicketDown(PointerDownEvent evt, VisualElement ticket)
    {
        if (!isPlaying || draggedTicket != null) return;
        if (kanbanColDone.Contains(ticket)) return; // Already done

        draggedTicket = ticket;
        draggedTicketOriginalIndex = kanbanColTodo.Contains(ticket) ? kanbanColTodo.IndexOf(ticket) : kanbanColProgress.IndexOf(ticket);
        
        Vector2 localPos = new Vector2(
            ticket.worldBound.xMin - kanbanContainer.worldBound.xMin,
            ticket.worldBound.yMin - kanbanContainer.worldBound.yMin
        );
        
        float currentWidth = ticket.layout.width;

        ticket.parent.Remove(ticket);
        kanbanContainer.Add(ticket);
        
        ticket.style.position = Position.Absolute;
        ticket.style.left = localPos.x;
        ticket.style.top = localPos.y;
        ticket.style.width = currentWidth;
        
        ticket.style.opacity = 0.8f;
        pointerStartPos = evt.position;
        dragStartPos = localPos;
        ticket.CapturePointer(evt.pointerId);
    }

    private void OnKanbanTicketMove(PointerMoveEvent evt, VisualElement ticket)
    {
        if (!isPlaying || draggedTicket != ticket) return;
        if (!ticket.HasPointerCapture(evt.pointerId)) return;
        
        Vector2 delta = (Vector2)evt.position - pointerStartPos;
        ticket.style.left = dragStartPos.x + delta.x;
        ticket.style.top = dragStartPos.y + delta.y;
    }

    private void OnKanbanTicketUp(PointerUpEvent evt, VisualElement ticket)
    {
        if (!isPlaying || draggedTicket != ticket) return;
        if (!ticket.HasPointerCapture(evt.pointerId)) return;
        
        ticket.ReleasePointer(evt.pointerId);
        draggedTicket = null;
        ticket.style.opacity = 1f;
        ticket.style.width = new Length(90, LengthUnit.Percent);

        Rect ticketRect = ticket.worldBound;
        Rect progressRect = kanbanColProgress.worldBound;
        Rect doneRect = kanbanColDone.worldBound;

        if (ticketRect.Overlaps(doneRect))
        {
            if (ticket.ClassListContains("in-progress"))
            {
                // Valid move to DONE
                ticket.style.position = Position.Relative;
                ticket.style.left = 0;
                ticket.style.top = 0;
                ticket.style.backgroundColor = new StyleColor(new Color(0.2f, 0.8f, 0.4f)); 
                if (!ticket.Q<Label>().text.StartsWith("✅")) 
                    ticket.Q<Label>().text = "✅ " + ticket.Q<Label>().text;
                
                if (ticket.parent != null) ticket.parent.Remove(ticket);
                kanbanColDone.Add(ticket);
                
                kanbanTicketsDone++;
                if (kanbanTicketsDone >= currentConfig.kanbanTicketCount) MarkWinReady();
            }
            else
            {
                // Tried to skip IN PROGRESS -> Shake
                StartCoroutine(ShakeTicketRoutine(ticket, kanbanColTodo));
            }
        }
        else if (ticketRect.Overlaps(progressRect))
        {
            // Move to IN PROGRESS
            ticket.style.position = Position.Relative;
            ticket.style.left = 0;
            ticket.style.top = 0;
            if (ticket.parent != null) ticket.parent.Remove(ticket);

            if (!ticket.ClassListContains("in-progress")) 
            {
                ticket.AddToClassList("in-progress");
                kanbanColProgress.Add(ticket); // Moving to new column, just append
            }
            else
            {
                // Snapping back to same column
                if (draggedTicketOriginalIndex >= 0 && draggedTicketOriginalIndex <= kanbanColProgress.childCount)
                    kanbanColProgress.Insert(draggedTicketOriginalIndex, ticket);
                else
                    kanbanColProgress.Add(ticket);
            }
        }
        else
        {
            // Snap back
            ticket.style.position = Position.Relative;
            ticket.style.left = 0;
            ticket.style.top = 0;
            if (ticket.parent != null) ticket.parent.Remove(ticket);
            
            if (ticket.ClassListContains("in-progress"))
            {
                if (draggedTicketOriginalIndex >= 0 && draggedTicketOriginalIndex <= kanbanColProgress.childCount)
                    kanbanColProgress.Insert(draggedTicketOriginalIndex, ticket);
                else
                    kanbanColProgress.Add(ticket);
            }
            else
            {
                if (draggedTicketOriginalIndex >= 0 && draggedTicketOriginalIndex <= kanbanColTodo.childCount)
                    kanbanColTodo.Insert(draggedTicketOriginalIndex, ticket);
                else
                    kanbanColTodo.Add(ticket);
            }
        }
    }
    
    private IEnumerator ShakeTicketRoutine(VisualElement ticket, VisualElement targetColumn)
    {
        float duration = 0.3f;
        float elapsed = 0f;
        float magnitude = 10f;
        
        // Put it back in the list for the shake
        ticket.style.position = Position.Relative;
        ticket.style.left = 0;
        ticket.style.top = 0;
        ticket.style.width = new Length(90, LengthUnit.Percent);
        if (ticket.parent != null) ticket.parent.Remove(ticket);

        if (targetColumn == kanbanColTodo && draggedTicketOriginalIndex >= 0 && draggedTicketOriginalIndex <= kanbanColTodo.childCount)
            kanbanColTodo.Insert(draggedTicketOriginalIndex, ticket);
        else
            targetColumn.Add(ticket);
        
        while (elapsed < duration && isPlaying)
        {
            elapsed += Time.deltaTime;
            float xOffset = Mathf.Sin(elapsed * 40f) * magnitude;
            ticket.style.left = xOffset;
            yield return null;
        }
        
        ticket.style.left = 0;
    }

    private IEnumerator KanbanBoardRoutine()
    {
        float timer = currentConfig.timeLimit;
        isPlaying = true;
        
        while (isPlaying)
        {
            timer -= Time.deltaTime;
            if (kanbanTimerText != null) kanbanTimerText.text = $"{Mathf.Max(0, timer):F1}s";
            
            if (timer <= 0f)
            {
                if (currentGame.canFail)
                {
                    if (kanbanTimerText != null) kanbanTimerText.text = "TIME UP!";
                    isPlaying = false;
                    yield break;
                }
                else
                {
                    MarkWinReady();
                }
            }
            yield return null;
        }
    }

    // =============================================
    // FILE SORTER (4-WAY SWIPE)
    // =============================================
    
    private void SetupFileSorter()
    {
        UnityEngine.Debug.Log($"[MiniGameManager] SetupFileSorter called. fileSorterContainer is {(fileSorterContainer == null ? "NULL" : "VALID")}. fileSorterActiveFile is {(fileSorterActiveFile == null ? "NULL" : "VALID")}");
        fileSorterSortedCount = 0;
        if (fileSorterActiveFile != null) fileSorterActiveFile.style.display = DisplayStyle.Flex;
        if (fileSorterProgressText != null) fileSorterProgressText.text = $"0 / {currentConfig.fileSorterTarget}";
        if (fileSorterTimerText != null) fileSorterTimerText.text = $"{currentConfig.timeLimit:F1}s";
        
        NextFileSorterItem();
        
        activeGameRoutine = StartCoroutine(FileSorterRoutine());
    }
    
    private void NextFileSorterItem()
    {
        if (fileSorterActiveFile != null)
        {
            fileSorterActiveFile.style.translate = new Translate(0, 0, 0);
        }
        
        string fileName = "";
        if (currentConfig.fileSorterItems != null && currentConfig.fileSorterItems.Length > 0)
        {
            fileName = currentConfig.fileSorterItems[Random.Range(0, currentConfig.fileSorterItems.Length)];
        }
        else
        {
            fileName = $"Doc_{Random.Range(100, 999)}.txt";
        }
        
        string[] dirs = new string[]{"Up", "Down", "Left", "Right"};
        fileSorterTargetDirection = dirs[Random.Range(0, 4)];
        
        string folderName = "";
        switch (fileSorterTargetDirection)
        {
            case "Up": folderName = "FINANCE"; break;
            case "Down": folderName = "HR"; break;
            case "Left": folderName = "MARKETING"; break;
            case "Right": folderName = "IT"; break;
        }
        
        if (fileSorterActiveFileText != null) 
        {
            fileSorterActiveFileText.text = fileName + $"\n\n\n<b>To: {folderName}</b>";
        }
    }
    
    private void OnFileSorterDown(PointerDownEvent evt)
    {
        if (!isPlaying || isDraggingFileSorter) return;
        isDraggingFileSorter = true;
        fileSorterPointerStart = evt.position;
        fileSorterActiveFile.CapturePointer(evt.pointerId);
    }
    
    private void OnFileSorterMove(PointerMoveEvent evt)
    {
        if (!isPlaying || !isDraggingFileSorter || !fileSorterActiveFile.HasPointerCapture(evt.pointerId)) return;
        
        Vector2 delta = (Vector2)evt.position - fileSorterPointerStart;
        fileSorterActiveFile.style.translate = new Translate(delta.x, delta.y, 0);
    }
    
    private void OnFileSorterUp(PointerUpEvent evt)
    {
        if (!isPlaying || !isDraggingFileSorter || !fileSorterActiveFile.HasPointerCapture(evt.pointerId)) return;
        
        fileSorterActiveFile.ReleasePointer(evt.pointerId);
        isDraggingFileSorter = false;
        
        Vector2 delta = (Vector2)evt.position - fileSorterPointerStart;
        
        if (delta.magnitude < 50f)
        {
            fileSorterActiveFile.style.translate = new Translate(0, 0, 0);
            return;
        }
        
        string swipedDir = "";
        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
        {
            swipedDir = delta.x > 0 ? "Right" : "Left";
        }
        else
        {
            swipedDir = delta.y > 0 ? "Down" : "Up";
        }
        
        if (swipedDir == fileSorterTargetDirection)
        {
            StartCoroutine(FileSorterAnimateOut(swipedDir));
        }
        else
        {
            StartCoroutine(FileSorterShakeError());
        }
    }
    
    private IEnumerator FileSorterAnimateOut(string dir)
    {
        Vector2 currentTrans = new Vector2(fileSorterActiveFile.resolvedStyle.translate.x, fileSorterActiveFile.resolvedStyle.translate.y);
        Vector2 targetTrans = currentTrans;
        
        if (dir == "Up") targetTrans.y -= 500f;
        if (dir == "Down") targetTrans.y += 500f;
        if (dir == "Left") targetTrans.x -= 500f;
        if (dir == "Right") targetTrans.x += 500f;
        
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * 10f;
            fileSorterActiveFile.style.translate = new Translate(Mathf.Lerp(currentTrans.x, targetTrans.x, t), Mathf.Lerp(currentTrans.y, targetTrans.y, t), 0);
            yield return null;
        }
        
        fileSorterSortedCount++;
        if (fileSorterProgressText != null) fileSorterProgressText.text = $"{fileSorterSortedCount} / {currentConfig.fileSorterTarget}";
        
        if (fileSorterSortedCount >= currentConfig.fileSorterTarget)
        {
            fileSorterActiveFile.style.display = DisplayStyle.None;
            MarkWinReady();
        }
        else
        {
            NextFileSorterItem();
        }
    }
    
    private IEnumerator FileSorterShakeError()
    {
        fileSorterActiveFile.style.backgroundColor = new StyleColor(new Color(0.9f, 0.2f, 0.2f));
        
        float duration = 0.3f;
        float elapsed = 0f;
        float magnitude = 15f;
        
        Vector2 baseTrans = new Vector2(fileSorterActiveFile.resolvedStyle.translate.x, fileSorterActiveFile.resolvedStyle.translate.y);
        
        while (elapsed < duration && isPlaying)
        {
            elapsed += Time.deltaTime;
            float xOffset = Mathf.Sin(elapsed * 40f) * magnitude;
            fileSorterActiveFile.style.translate = new Translate(baseTrans.x + xOffset, baseTrans.y, 0);
            yield return null;
        }
        
        fileSorterActiveFile.style.translate = new Translate(0, 0, 0);
        fileSorterActiveFile.style.backgroundColor = new StyleColor(Color.white);
    }
    
    private IEnumerator FileSorterRoutine()
    {
        float timer = currentConfig.timeLimit;
        isPlaying = true;
        
        while (isPlaying)
        {
            timer -= Time.deltaTime;
            if (fileSorterTimerText != null) fileSorterTimerText.text = $"{Mathf.Max(0, timer):F1}s";
            
            if (timer <= 0f)
            {
                if (currentGame.canFail)
                {
                    if (fileSorterTimerText != null) fileSorterTimerText.text = "TIME UP!";
                    isPlaying = false;
                    yield break;
                }
                else
                {
                    MarkWinReady();
                }
            }
            yield return null;
        }
    }
    private void SetupCoffeeWork()
    {
        cwEnergy = 100f;
        cwProgress = 0f;
        UpdateCwUI();
        if (cwTimerText != null) cwTimerText.text = $"{currentConfig.timeLimit:F1}s";
        
        if (cwBtnType != null)
        {
            if (currentGame.name == "Debug CSS at 3 AM")
                cwBtnType.text = "⌨️ DEBUG CSS";
            else
                cwBtnType.text = "⌨️ TYPE SLIDES";
        }
        
        activeGameRoutine = StartCoroutine(CoffeeWorkRoutine());
    }

    private void UpdateCwUI()
    {
        if (cwEnergyFill != null)
        {
            cwEnergyFill.style.height = new Length(cwEnergy, LengthUnit.Percent);
            cwEnergyFill.style.backgroundColor = cwEnergy > 30f ? new StyleColor(new Color(0.96f, 0.62f, 0.04f)) : new StyleColor(Color.red);
        }
        if (cwProgressFill != null)
        {
            cwProgressFill.style.height = new Length(cwProgress, LengthUnit.Percent);
        }
    }

    private void OnCwBtnCoffee()
    {
        if (!isPlaying || hasWon) return;
        cwEnergy = Mathf.Clamp(cwEnergy + currentConfig.coffeeBoost, 0f, 100f);
        UpdateCwUI();
    }

    private void OnCwBtnType()
    {
        if (!isPlaying || hasWon) return;
        cwProgress = Mathf.Clamp(cwProgress + currentConfig.workProgressPerTap, 0f, 100f);
        UpdateCwUI();
        
        if (cwProgress >= 100f)
        {
            MarkWinReady();
        }
    }

    private IEnumerator CoffeeWorkRoutine()
    {
        float timer = currentConfig.timeLimit;
        isPlaying = true;
        
        while (isPlaying)
        {
            timer -= Time.deltaTime;
            if (cwTimerText != null) cwTimerText.text = $"{Mathf.Max(0, timer):F1}s";
            
            cwEnergy -= currentConfig.coffeeDrainRate * Time.deltaTime;
            UpdateCwUI();
            
            if (cwEnergy <= 0f)
            {
                if (currentGame.canFail)
                {
                    if (cwTimerText != null) cwTimerText.text = "FELL ASLEEP!";
                    isPlaying = false;
                    yield break;
                }
                else
                {
                    cwEnergy = 100f;
                }
            }
            
            if (timer <= 0f)
            {
                if (currentGame.canFail)
                {
                    if (cwTimerText != null) cwTimerText.text = "TIME UP!";
                    isPlaying = false;
                    yield break;
                }
                else
                {
                    MarkWinReady();
                }
            }
            yield return null;
        }
    }
    // ==========================================
    // UNIT TEST RUNNER
    // ==========================================
    private void SetupUnitTest()
    {
        utCurrentRound = 0;
        timer = currentConfig.timeLimit;
        UpdateUtUI();
        activeGameRoutine = StartCoroutine(UnitTestRoutine());
    }

    private void UpdateUtUI()
    {
        if (utCurrentRound >= currentConfig.unitTestRounds)
        {
            MarkWinReady();
            return;
        }

        if (utRoundText != null) utRoundText.text = $"Test: {utCurrentRound + 1}/{currentConfig.unitTestRounds}";
        if (utCodeText != null) utCodeText.text = currentConfig.unitTestQuestions[utCurrentRound];
        
        string[] options = currentConfig.unitTestOptions[utCurrentRound];
        if (utBtnOption1 != null) utBtnOption1.text = options.Length > 0 ? options[0] : "";
        if (utBtnOption2 != null) utBtnOption2.text = options.Length > 1 ? options[1] : "";
        if (utBtnOption3 != null) utBtnOption3.text = options.Length > 2 ? options[2] : "";
    }

    private void OnUtBtnOption(int idx)
    {
        if (!isPlaying || hasWon) return;
        if (utCurrentRound >= currentConfig.unitTestRounds) return;

        if (idx == currentConfig.unitTestCorrectIndices[utCurrentRound])
        {
            utCurrentRound++;
            UpdateUtUI();
        }
        else
        {
            // Wrong answer penalty: flash button red and reduce time
            Button clickedBtn = idx == 0 ? utBtnOption1 : (idx == 1 ? utBtnOption2 : utBtnOption3);
            if (clickedBtn != null)
            {
                var originalColor = clickedBtn.style.backgroundColor;
                clickedBtn.style.backgroundColor = new StyleColor(Color.red);
                clickedBtn.schedule.Execute(() => { 
                    if (clickedBtn != null) clickedBtn.style.backgroundColor = originalColor; 
                }).StartingIn(200);
            }
            timer -= 2f;
        }
    }

    private IEnumerator UnitTestRoutine()
    {
        isPlaying = true;
        while (isPlaying)
        {
            timer -= Time.deltaTime;
            if (utTimerText != null) utTimerText.text = $"{Mathf.Max(0, timer):F1}s";

            if (timer <= 0f)
            {
                if (currentGame.canFail)
                {
                    if (utTimerText != null) utTimerText.text = "FAILED!";
                    isPlaying = false;
                    yield break;
                }
                else
                {
                    MarkWinReady();
                }
            }
            yield return null;
        }
    }

    // ==========================================
    // CENTER DIV
    // ==========================================
    private void SetupCenterDiv()
    {
        cdCentersComplete = 0;
        timer = currentConfig.timeLimit;
        
        // Randomize initial target box position horizontally (from 20% to 80%)
        float targetLeft = Random.Range(20f, 80f);
        if (cdTargetBox != null) cdTargetBox.style.left = new Length(targetLeft, LengthUnit.Percent);
        
        cdCurrentSpeed = Random.Range(currentConfig.centerDivSpeedMin, currentConfig.centerDivSpeedMax);
        cdMovingBoxPos = 0f;
        cdMovingBoxDir = 1f;

        if (cdRoundText != null) cdRoundText.text = $"Centered: {cdCentersComplete}/{currentConfig.centerDivTargets}";
        
        activeGameRoutine = StartCoroutine(CenterDivRoutine());
    }

    private void OnCdBtnCenter()
    {
        if (!isPlaying || hasWon) return;

        // Calculate distance between target and moving box
        float targetLeft = cdTargetBox != null ? cdTargetBox.style.left.value.value : 50f; 
        // Note: targetLeft is in percent, movingBoxPos is in percent
        float dist = Mathf.Abs(targetLeft - cdMovingBoxPos);

        // Tolerance is in percent (e.g. 5% = 50px if container is 1000px)
        // Using a slightly larger tolerance (15f) to make it easier for players to hit it when it visually aligns
        float tolerance = 15f; 

        if (dist <= tolerance)
        {
            cdCentersComplete++;
            if (cdRoundText != null) cdRoundText.text = $"Centered: {cdCentersComplete}/{currentConfig.centerDivTargets}";
            
            if (cdCentersComplete >= currentConfig.centerDivTargets)
            {
                if (cdMovingBox != null) cdMovingBox.style.backgroundColor = new StyleColor(new Color(0.13f, 0.77f, 0.36f)); // Green
                MarkWinReady();
            }
            else
            {
                // Next target
                float newTargetLeft = Random.Range(20f, 80f);
                if (cdTargetBox != null) cdTargetBox.style.left = new Length(newTargetLeft, LengthUnit.Percent);
                cdCurrentSpeed = Random.Range(currentConfig.centerDivSpeedMin, currentConfig.centerDivSpeedMax);
                cdMovingBoxPos = Random.value > 0.5f ? 0f : 100f;
            }
        }
        else
        {
            // Missed! Penalty
            timer -= 1.5f;
            if (cdMovingBox != null) cdMovingBox.style.backgroundColor = new StyleColor(Color.red);
        }
    }

    private IEnumerator CenterDivRoutine()
    {
        isPlaying = true;
        while (isPlaying)
        {
            timer -= Time.deltaTime;
            if (cdTimerText != null) cdTimerText.text = $"{Mathf.Max(0, timer):F1}s";

            // Move the box
            // Convert speed to percent/sec. Say speed 300 = 30% per second.
            float percentPerSec = cdCurrentSpeed / 10f; 
            cdMovingBoxPos += percentPerSec * cdMovingBoxDir * Time.deltaTime;

            if (cdMovingBoxPos >= 100f)
            {
                cdMovingBoxPos = 100f;
                cdMovingBoxDir = -1f;
            }
            else if (cdMovingBoxPos <= 0f)
            {
                cdMovingBoxPos = 0f;
                cdMovingBoxDir = 1f;
            }

            if (cdMovingBox != null) 
            {
                cdMovingBox.style.left = new Length(cdMovingBoxPos, LengthUnit.Percent);
                // Reset color back to orange if it was red from a miss
                if (cdMovingBox.style.backgroundColor.value != new Color(0.13f, 0.77f, 0.36f) && cdMovingBox.style.backgroundColor.value != new Color(0.96f, 0.62f, 0.04f))
                {
                    cdMovingBox.style.backgroundColor = new StyleColor(new Color(0.96f, 0.62f, 0.04f));
                }
            }

            if (timer <= 0f)
            {
                if (currentGame.canFail)
                {
                    if (cdTimerText != null) cdTimerText.text = "TIME UP!";
                    isPlaying = false;
                    yield break;
                }
                else
                {
                    MarkWinReady();
                }
            }
            yield return null;
        }
    }
    // ==========================================
    // DRAG CENTER (Center Content Icons)
    // ==========================================
    private void SetupDragCenter()
    {
        timer = currentConfig.timeLimit;
        
        if (dcRoundText != null) dcRoundText.text = $"Centered: {dcPoints}/3";
        if (dcTimerText != null) dcTimerText.text = $"{timer:F1}s";
        
        if (dcBtnConfirm != null) dcBtnConfirm.style.display = DisplayStyle.Flex;
        if (dcScoreArea != null) dcScoreArea.style.display = DisplayStyle.None;
        if (dcTargetCrosshair != null) dcTargetCrosshair.style.display = DisplayStyle.None;
        
        // Randomize initial position of the emoji between 10% and 90%
        if (dcDraggableIcon != null)
        {
            dcDraggableIcon.style.left = new Length(Random.Range(10f, 90f), LengthUnit.Percent);
            dcDraggableIcon.style.top = new Length(Random.Range(10f, 90f), LengthUnit.Percent);
        }

        dcIsScoring = false;
        activeGameRoutine = StartCoroutine(DragCenterRoutine());
    }

    private void OnDcPointerDown(PointerDownEvent ev)
    {
        if (!isPlaying || hasWon || dcIsScoring) return;
        dcIsDragging = true;
        if (dcDraggableIcon != null) dcDraggableIcon.CapturePointer(ev.pointerId);
        ev.StopPropagation();
    }

    private void OnDcPointerMove(PointerMoveEvent ev)
    {
        if (!dcIsDragging || !isPlaying || dcDraggableIcon == null || dcWorkArea == null) return;
        
        // Convert screen position to local coordinates within the work area
        Vector2 localPos = dcWorkArea.WorldToLocal(ev.position);
        
        // Convert to percentage
        float width = dcWorkArea.resolvedStyle.width;
        float height = dcWorkArea.resolvedStyle.height;
        
        if (width > 0 && height > 0)
        {
            float percentX = (localPos.x / width) * 100f;
            float percentY = (localPos.y / height) * 100f;
            
            dcDraggableIcon.style.left = new Length(Mathf.Clamp(percentX, 0f, 100f), LengthUnit.Percent);
            dcDraggableIcon.style.top = new Length(Mathf.Clamp(percentY, 0f, 100f), LengthUnit.Percent);
        }
    }

    private void OnDcPointerUp(PointerUpEvent ev)
    {
        if (dcIsDragging && dcDraggableIcon != null)
        {
            dcIsDragging = false;
            dcDraggableIcon.ReleasePointer(ev.pointerId);
        }
    }

    private void OnDcPointerCaptureOut(PointerCaptureOutEvent ev)
    {
        dcIsDragging = false;
    }

    private void OnDcBtnConfirm()
    {
        if (!isPlaying || hasWon || dcIsScoring) return;
        dcIsScoring = true;
        StartCoroutine(DcScoreAnimation());
    }

    private IEnumerator DcScoreAnimation()
    {
        if (dcBtnConfirm != null) dcBtnConfirm.style.display = DisplayStyle.None;
        if (dcScoreArea != null) dcScoreArea.style.display = DisplayStyle.Flex;
        if (dcScoreBarFill != null) dcScoreBarFill.style.width = new Length(0f, LengthUnit.Percent);
        if (dcScoreText != null) dcScoreText.text = "Accuracy: 0%";

        // Calculate distance from 50%, 50%
        float px = dcDraggableIcon.style.left.value.value;
        float py = dcDraggableIcon.style.top.value.value;
        
        // Maximum possible distance is to a corner (approx 70.71%)
        float dist = Vector2.Distance(new Vector2(px, py), new Vector2(50f, 50f));
        
        // Convert distance to accuracy (0 distance = 100%, 70.71 distance = 0%)
        float accuracy = Mathf.Clamp(100f - (dist / 70.71f * 100f), 0f, 100f);
        
        // Animate progress bar
        float animTime = 1.0f;
        float elapsed = 0f;
        while (elapsed < animTime)
        {
            elapsed += Time.deltaTime;
            float currentAcc = Mathf.Lerp(0f, accuracy, elapsed / animTime);
            
            if (dcScoreBarFill != null) 
            {
                dcScoreBarFill.style.width = new Length(currentAcc, LengthUnit.Percent);
                dcScoreBarFill.style.backgroundColor = new StyleColor(Color.Lerp(new Color(0.93f, 0.26f, 0.22f), new Color(0.13f, 0.77f, 0.36f), currentAcc / 100f));
            }
            if (dcScoreText != null) dcScoreText.text = $"Accuracy: {Mathf.RoundToInt(currentAcc)}%";
            
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        if (accuracy >= 90f)
        {
            dcPoints++;
            if (dcRoundText != null) dcRoundText.text = $"Centered: {dcPoints}/3";
            
            if (dcPoints >= 3)
            {
                MarkWinReady();
                yield break;
            }
        }
        else
        {
            // Failed this round
            if (dcScoreText != null) dcScoreText.text = "FAILED!";
            yield return new WaitForSeconds(0.5f);
        }

        // Setup next round
        SetupDragCenter();
    }

    private IEnumerator DragCenterRoutine()
    {
        isPlaying = true;
        while (isPlaying)
        {
            if (!dcIsScoring)
            {
                timer -= Time.deltaTime;
                if (dcTimerText != null) dcTimerText.text = $"{Mathf.Max(0, timer):F1}s";

                if (timer <= 0f)
                {
                    if (currentGame.canFail)
                    {
                        if (dcTimerText != null) dcTimerText.text = "TIME UP!";
                        isPlaying = false;
                        yield break;
                    }
                    else
                    {
                        MarkWinReady();
                    }
                }
            }
            yield return null;
        }
    }

    // ==========================================
    // SIZE MATCH (Fix CSS layout sizes)
    // ==========================================
    private void SetupSizeMatch()
    {
        timer = currentConfig.timeLimit;
        
        if (smRoundText != null) smRoundText.text = $"Matched: {smPoints}/3";
        if (smTimerText != null) smTimerText.text = $"{timer:F1}s";
        
        smCurrentScale = 0.2f;
        smScaleDir = 1f;

        // Reset color to yellow
        if (smBtnChangingBox != null) smBtnChangingBox.style.backgroundColor = new StyleColor(new Color(0.96f, 0.62f, 0.04f));
        
        activeGameRoutine = StartCoroutine(SizeMatchRoutine());
    }

    private void OnSmBoxClicked()
    {
        if (!isPlaying || hasWon) return;

        // Target box is 200x150. We ping pong the button size from 0.2 to 1.5 of the target box size.
        // Base width = 200, Base height = 150

        
        // current scale goes from 0.2 to 1.5
        // At scale 1.0, the width is 200
        float diff = Mathf.Abs(smCurrentScale - 1.0f);

        // Tolerance: if difference in scale is <= 0.1 (10% difference)
        float tolerance = 0.1f;

        if (diff <= tolerance)
        {
            // Success
            smPoints++;
            if (smRoundText != null) smRoundText.text = $"Matched: {smPoints}/3";
            
            // Flash green
            if (smBtnChangingBox != null)
            {
                smBtnChangingBox.style.backgroundColor = new StyleColor(new Color(0.13f, 0.77f, 0.36f));
                smBtnChangingBox.schedule.Execute(() => { 
                    if (smBtnChangingBox != null) smBtnChangingBox.style.backgroundColor = new StyleColor(new Color(0.96f, 0.62f, 0.04f)); 
                }).StartingIn(200);
            }

            if (smPoints >= 3)
            {
                MarkWinReady();
            }
            else
            {
                // Reset scale
                smCurrentScale = 0.2f;
                smScaleDir = 1f;
            }
        }
        else
        {
            // Missed! Penalty
            timer -= 1.5f;
            
            // Flash red
            if (smBtnChangingBox != null)
            {
                smBtnChangingBox.style.backgroundColor = new StyleColor(new Color(0.93f, 0.26f, 0.22f));
                smBtnChangingBox.schedule.Execute(() => { 
                    if (smBtnChangingBox != null) smBtnChangingBox.style.backgroundColor = new StyleColor(new Color(0.96f, 0.62f, 0.04f)); 
                }).StartingIn(200);
            }
        }
    }

    private IEnumerator SizeMatchRoutine()
    {
        isPlaying = true;
        
        float speed = 1.2f; // Scale per second

        while (isPlaying)
        {
            timer -= Time.deltaTime;
            if (smTimerText != null) smTimerText.text = $"{Mathf.Max(0, timer):F1}s";

            // Ping pong scale between 0.2 and 1.5
            smCurrentScale += smScaleDir * speed * Time.deltaTime;
            
            if (smCurrentScale >= 1.5f)
            {
                smCurrentScale = 1.5f;
                smScaleDir = -1f;
            }
            else if (smCurrentScale <= 0.2f)
            {
                smCurrentScale = 0.2f;
                smScaleDir = 1f;
            }

            // Apply size
            if (smBtnChangingBox != null)
            {
                smBtnChangingBox.style.width = new Length(200f * smCurrentScale, LengthUnit.Pixel);
                smBtnChangingBox.style.height = new Length(150f * smCurrentScale, LengthUnit.Pixel);
                
                // Update text to TAP when in tolerance range
                if (Mathf.Abs(smCurrentScale - 1.0f) <= 0.1f)
                {
                    smBtnChangingBox.text = "TAP";
                }
                else
                {
                    smBtnChangingBox.text = "this";
                }
            }

            if (timer <= 0f)
            {
                if (currentGame.canFail)
                {
                    if (smTimerText != null) smTimerText.text = "TIME UP!";
                    isPlaying = false;
                    yield break;
                }
                else
                {
                    MarkWinReady();
                }
            }
            yield return null;
        }
    }

    // ==========================================
    // STEALTH INJECT (Inject untested code)
    // ==========================================
    private void SetupStealthInject()
    {
        timer = currentConfig.timeLimit;
        siProgress = 0f;
        siIsReviewing = false;
        siIsHolding = false;
        siReviewTimer = Random.Range(1.5f, 3.5f);
        
        UpdateSiUI();
        activeGameRoutine = StartCoroutine(StealthInjectRoutine());
    }

    private void UpdateSiUI()
    {
        if (siTimerText != null) siTimerText.text = $"{Mathf.Max(0, timer):F1}s";
        if (siProgressBarFill != null) siProgressBarFill.style.width = new Length(siProgress, LengthUnit.Percent);
        
        if (siIsReviewing)
        {
            if (siStatusText != null) siStatusText.text = "REVIEWER CHECKING!";
            if (siReviewerIcon != null) siReviewerIcon.text = "🚨";
            if (siReviewerStateText != null) 
            {
                siReviewerStateText.text = "DANGER";
                siReviewerStateText.style.color = new StyleColor(new Color(0.93f, 0.26f, 0.22f)); // red
            }
            if (siWorkArea != null) siWorkArea.style.backgroundColor = new StyleColor(new Color(0.4f, 0.1f, 0.1f));
        }
        else
        {
            if (siStatusText != null) siStatusText.text = "Reviewer distracted...";
            if (siReviewerIcon != null) siReviewerIcon.text = "🙈";
            if (siReviewerStateText != null) 
            {
                siReviewerStateText.text = "SAFE";
                siReviewerStateText.style.color = new StyleColor(new Color(0.13f, 0.77f, 0.36f)); // green
            }
            if (siWorkArea != null) siWorkArea.style.backgroundColor = new StyleColor(new Color(0.06f, 0.09f, 0.16f)); // slate-900
        }
    }

    private void OnSiPointerDown(PointerDownEvent ev)
    {
        if (!isPlaying || hasWon) return;
        siIsHolding = true;
        if (siBtnInject != null) siBtnInject.CapturePointer(ev.pointerId);
    }

    private void OnSiPointerUp(PointerUpEvent ev)
    {
        siIsHolding = false;
        if (siBtnInject != null) siBtnInject.ReleasePointer(ev.pointerId);
    }

    private void OnSiPointerCaptureOut(PointerCaptureOutEvent ev)
    {
        siIsHolding = false;
    }

    private IEnumerator StealthInjectRoutine()
    {
        isPlaying = true;
        
        while (isPlaying)
        {
            timer -= Time.deltaTime;
            siReviewTimer -= Time.deltaTime;

            if (siReviewTimer <= 0f)
            {
                // Toggle state
                siIsReviewing = !siIsReviewing;
                siReviewTimer = siIsReviewing ? Random.Range(1.0f, 2.0f) : Random.Range(2.0f, 4.0f);
                UpdateSiUI();
            }

            if (siIsHolding)
            {
                if (siIsReviewing)
                {
                    // Caught! Apply massive penalty.
                    siIsHolding = false; // force release internally until they lift and press again
                    siProgress = Mathf.Max(0f, siProgress - 50f);
                    
                    // Flash red intensely
                    if (siWorkArea != null)
                    {
                        siWorkArea.style.backgroundColor = new StyleColor(Color.red);
                        siWorkArea.schedule.Execute(() => { 
                            if (siWorkArea != null && siIsReviewing) siWorkArea.style.backgroundColor = new StyleColor(new Color(0.4f, 0.1f, 0.1f)); 
                        }).StartingIn(300);
                    }
                }
                else
                {
                    // Progressing
                    siProgress += 30f * Time.deltaTime; // Takes ~3.3 seconds of holding to hit 100%
                    if (siProgress >= 100f)
                    {
                        siProgress = 100f;
                        UpdateSiUI();
                        MarkWinReady();
                        yield break;
                    }
                }
            }

            UpdateSiUI();

            if (timer <= 0f)
            {
                if (currentGame.canFail)
                {
                    if (siTimerText != null) siTimerText.text = "TIME UP!";
                    isPlaying = false;
                    yield break;
                }
                else
                {
                    MarkWinReady();
                }
            }
            yield return null;
        }
    }

    // ==========================================
    // AUTOMATED TESTS (Flaky tests)
    // ==========================================
    private void SetupAutomatedTests()
    {
        timer = currentConfig.timeLimit;
        atSuiteProgress = 0f;
        atNextFailTimer = Random.Range(1.5f, 3.0f);

        for (int i = 0; i < 5; i++)
        {
            atIsFailing[i] = false;
            atFuses[i] = 0f;
            UpdateAtTestUI(i);
        }

        if (atProgressBarFill != null) atProgressBarFill.style.width = new Length(0f, LengthUnit.Percent);
        if (atTimerText != null) atTimerText.text = $"{timer:F1}s";

        activeGameRoutine = StartCoroutine(AutomatedTestsRoutine());
    }

    private void UpdateAtTestUI(int index)
    {
        if (atBtnTests[index] == null || atStatusTexts[index] == null) return;

        if (atIsFailing[index])
        {
            atStatusTexts[index].text = $"FAILING! ({atFuses[index]:F1}s)";
            atStatusTexts[index].style.color = new StyleColor(new Color(0.93f, 0.26f, 0.22f)); // Red
            atBtnTests[index].style.backgroundColor = new StyleColor(new Color(0.4f, 0.1f, 0.1f)); // Dark red bg
        }
        else
        {
            atStatusTexts[index].text = "PASSING";
            atStatusTexts[index].style.color = new StyleColor(new Color(0.13f, 0.77f, 0.36f)); // Green
            atBtnTests[index].style.backgroundColor = new StyleColor(new Color(0.12f, 0.16f, 0.23f)); // Slate-800
        }
    }

    private void OnAtTestClicked(int index)
    {
        if (!isPlaying || hasWon) return;

        if (atIsFailing[index])
        {
            // Re-run the flaky test!
            atIsFailing[index] = false;
            UpdateAtTestUI(index);
        }
    }

    private IEnumerator AutomatedTestsRoutine()
    {
        isPlaying = true;

        while (isPlaying)
        {
            timer -= Time.deltaTime;
            
            // Suite progress
            atSuiteProgress = 100f * (1f - (timer / currentConfig.timeLimit));
            if (atProgressBarFill != null) atProgressBarFill.style.width = new Length(Mathf.Clamp(atSuiteProgress, 0f, 100f), LengthUnit.Percent);
            if (atTimerText != null) atTimerText.text = $"{Mathf.Max(0, timer):F1}s";

            // Spawn failures
            atNextFailTimer -= Time.deltaTime;
            if (atNextFailTimer <= 0f)
            {
                // Find a random non-failing test
                List<int> available = new List<int>();
                for (int i = 0; i < 5; i++) if (!atIsFailing[i]) available.Add(i);
                
                if (available.Count > 0)
                {
                    int toFail = available[Random.Range(0, available.Count)];
                    atIsFailing[toFail] = true;
                    atFuses[toFail] = 3.0f; // 3 second fuse
                    UpdateAtTestUI(toFail);
                }

                // Schedule next fail
                atNextFailTimer = Random.Range(1.0f, 2.5f);
            }

            // Tick fuses
            for (int i = 0; i < 5; i++)
            {
                if (atIsFailing[i])
                {
                    atFuses[i] -= Time.deltaTime;
                    UpdateAtTestUI(i); // Update the fuse countdown text

                    if (atFuses[i] <= 0f)
                    {
                        // FUSE EXPIRED! Suite crashes.
                        if (atTimerText != null) atTimerText.text = "SUITE CRASHED!";
                        
                        if (currentGame.canFail)
                        {
                            isPlaying = false;
                            yield break;
                        }
                        else
                        {
                            // If it can't fail, we just reset it and give a huge penalty?
                            // Or just let them win if time runs out anyway.
                            // But usually minigames canFail is false if it's a critical path, meaning they MUST win.
                            // We'll reset the time to 15s so they have to start over?
                            // Or just force fail, most Career jobs have canFail=true.
                            timer -= 5f; // Huge penalty
                            atIsFailing[i] = false;
                            UpdateAtTestUI(i);
                        }
                    }
                }
            }

            if (timer <= 0f)
            {
                // Ensure no tests are currently failing
                bool anyFailing = false;
                for (int i = 0; i < 5; i++) if (atIsFailing[i]) anyFailing = true;

                if (anyFailing)
                {
                    if (currentGame.canFail)
                    {
                        if (atTimerText != null) atTimerText.text = "FAILED (Flaky test open)";
                        isPlaying = false;
                        yield break;
                    }
                }
                else
                {
                    if (atTimerText != null) atTimerText.text = "SUITE PASSED!";
                    MarkWinReady();
                    yield break;
                }
            }

            yield return null;
        }
    }

    // ==========================================
    // CODE COMMENTER (Write flaky test workaround)
    // ==========================================
    private void SetupCodeCommenter()
    {
        timer = currentConfig.timeLimit;
        if (ccTimerText != null) ccTimerText.text = $"Time: {timer:F1}s";
        
        for (int i = 0; i < 5; i++)
        {
            ccLineIsFlaky[i] = false;
            ccLineCommented[i] = false;
            ccLineTimers[i] = 0f;
            UpdateCcLineUI(i);
        }

        activeGameRoutine = StartCoroutine(CodeCommenterRoutine());
    }

    private void UpdateCcLineUI(int index)
    {
        if (ccLines[index] == null || ccLineTexts[index] == null) return;
        
        if (ccLineCommented[index])
        {
            ccLines[index].style.backgroundColor = new StyleColor(Color.clear);
            ccLineTexts[index].text = $"// {ccOriginalTexts[index]}";
            ccLineTexts[index].style.color = new StyleColor(ColorUtility.TryParseHtmlString("#8b949e", out Color c1) ? c1 : Color.gray);
            ccLines[index].style.translate = new Translate(0, 0, 0);
        }
        else if (ccLineIsFlaky[index])
        {
            ccLines[index].style.backgroundColor = new StyleColor(ColorUtility.TryParseHtmlString("#e74c3c", out Color c2) ? c2 : Color.red);
            ccLineTexts[index].text = ccOriginalTexts[index];
            ccLineTexts[index].style.color = new StyleColor(Color.white);
        }
        else
        {
            ccLines[index].style.backgroundColor = new StyleColor(Color.clear);
            ccLineTexts[index].text = ccOriginalTexts[index];
            ccLineTexts[index].style.color = new StyleColor(ColorUtility.TryParseHtmlString("#c9d1d9", out Color c3) ? c3 : Color.white);
            ccLines[index].style.translate = new Translate(0, 0, 0);
        }
    }

    private void OnCcLineClicked(int index)
    {
        if (!isPlaying || currentConfig.type != GameType.CodeCommenter) return;
        if (ccLineIsFlaky[index] && !ccLineCommented[index])
        {
            ccLineIsFlaky[index] = false;
            ccLineCommented[index] = true;
            UpdateCcLineUI(index);
        }
        else if (!ccLineIsFlaky[index] && !ccLineCommented[index])
        {
            // Tapped a green/normal line, they failed the build!
            if (currentGame.canFail)
            {
                if (ccTimerText != null) ccTimerText.text = "BUILD FAILED!";
                isPlaying = false;
            }
        }
    }

    private IEnumerator CodeCommenterRoutine()
    {
        isPlaying = true;
        float nextFlakeTimer = Random.Range(1.0f, 2.0f);

        while (isPlaying)
        {
            timer -= Time.deltaTime;
            if (ccTimerText != null) ccTimerText.text = $"Time: {timer:F1}s";

            // Flake triggering
            nextFlakeTimer -= Time.deltaTime;
            if (nextFlakeTimer <= 0f)
            {
                // Find available lines
                List<int> available = new List<int>();
                for(int i=0; i<5; i++) if (!ccLineIsFlaky[i] && !ccLineCommented[i]) available.Add(i);
                
                if (available.Count > 0)
                {
                    int flakeIdx = available[Random.Range(0, available.Count)];
                    ccLineIsFlaky[flakeIdx] = true;
                    ccLineTimers[flakeIdx] = 1.5f; // Time they have to tap it
                    UpdateCcLineUI(flakeIdx);
                }
                
                nextFlakeTimer = Random.Range(0.8f, 1.8f);
            }

            // Update flaking lines
            bool failed = false;
            for (int i = 0; i < 5; i++)
            {
                if (ccLineIsFlaky[i] && !ccLineCommented[i])
                {
                    ccLineTimers[i] -= Time.deltaTime;
                    if (ccLines[i] != null)
                    {
                        // Shake
                        ccLines[i].style.translate = new Translate(Random.Range(-5f, 5f), Random.Range(-5f, 5f), 0);
                    }

                    if (ccLineTimers[i] <= 0f)
                    {
                        failed = true;
                    }
                }
            }

            if (failed && currentGame.canFail)
            {
                if (ccTimerText != null) ccTimerText.text = "BUILD FAILED!";
                isPlaying = false;
                yield break;
            }

            if (timer <= 0f)
            {
                // Survived 10 seconds! Win!
                if (ccTimerText != null) ccTimerText.text = "SUITE PASSED!";
                // Reset transforms
                for(int i=0; i<5; i++) if (ccLines[i] != null) ccLines[i].style.translate = new Translate(0, 0, 0);
                MarkWinReady();
                yield break;
            }

            yield return null;
        }
    }

    // ==========================================
    // DOCUMENT SIGNER (Approve quarterly budget)
    // ==========================================
    private void SetupDocumentSigner()
    {
        timer = currentConfig.timeLimit;
        if (dsTimerText != null) dsTimerText.text = $"{timer:F1}s";
        
        // Reset scroll position to top
        if (dsScrollView != null)
        {
            dsScrollView.scrollOffset = Vector2.zero;
        }

        activeGameRoutine = StartCoroutine(DocumentSignerRoutine());
    }

}

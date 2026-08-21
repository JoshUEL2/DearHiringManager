using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;

public class KanbanBoardMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement container;

    private Label kanbanTimerText;
    private VisualElement kanbanColTodo;
    private VisualElement kanbanColProgress;
    private VisualElement kanbanColDone;
    private List<VisualElement> kanbanTickets = new List<VisualElement>();
    
    private VisualElement draggedTicket = null;
    private Vector2 dragStartPos;
    private Vector2 pointerStartPos;
    private int draggedTicketOriginalIndex;
    private int kanbanTicketsDone = 0;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        
        container = root.Q<VisualElement>("KanbanContainer");
        kanbanTimerText = container.Q<Label>("KanbanTimerText");
        kanbanColTodo = container.Q<VisualElement>("KanbanColTodo");
        kanbanColProgress = container.Q<VisualElement>("KanbanColProgress");
        kanbanColDone = container.Q<VisualElement>("KanbanColDone");
    }

    public void StartGame()
    {
        kanbanColTodo.Clear();
        kanbanColProgress.Clear();
        kanbanColDone.Clear();
        kanbanTickets.Clear();
        kanbanTicketsDone = 0;
        
        container.style.display = DisplayStyle.Flex;
        
        string[] ticketPrefixes = new string[]{"BUG-", "DEV-", "UX-", "OPS-", "QA-"};

        for (int i = 0; i < config.kanbanTicketCount; i++)
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

        manager.StartCoroutine(KanbanBoardRoutine());
    }

    public void AbortGame()
    {
        container.style.display = DisplayStyle.None;
        // Callbacks will be GC'd when tickets are cleared in next StartGame
    }

    private void OnKanbanTicketDown(PointerDownEvent evt, VisualElement ticket)
    {
        if (!manager.isPlaying || draggedTicket != null) return;
        if (kanbanColDone.Contains(ticket)) return; // Already done

        draggedTicket = ticket;
        draggedTicketOriginalIndex = kanbanColTodo.Contains(ticket) ? kanbanColTodo.IndexOf(ticket) : kanbanColProgress.IndexOf(ticket);
        
        Vector2 localPos = new Vector2(
            ticket.worldBound.xMin - container.worldBound.xMin,
            ticket.worldBound.yMin - container.worldBound.yMin
        );
        
        float currentWidth = ticket.layout.width;

        ticket.parent.Remove(ticket);
        container.Add(ticket);
        
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
        if (!manager.isPlaying || draggedTicket != ticket) return;
        if (!ticket.HasPointerCapture(evt.pointerId)) return;
        
        Vector2 delta = (Vector2)evt.position - pointerStartPos;
        ticket.style.left = dragStartPos.x + delta.x;
        ticket.style.top = dragStartPos.y + delta.y;
    }

    private void OnKanbanTicketUp(PointerUpEvent evt, VisualElement ticket)
    {
        if (!manager.isPlaying || draggedTicket != ticket) return;
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
                if (kanbanTicketsDone >= config.kanbanTicketCount) manager.MarkWinReady();
            }
            else
            {
                // Tried to skip IN PROGRESS -> Shake
                manager.StartCoroutine(ShakeTicketRoutine(ticket, kanbanColTodo));
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
        
        while (elapsed < duration && manager.isPlaying)
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
        float timer = config.timeLimit;
        
        while (manager.isPlaying)
        {
            timer -= Time.deltaTime;
            if (kanbanTimerText != null) kanbanTimerText.text = $"{Mathf.Max(0, timer):F1}s";
            
            if (timer <= 0f)
            {
                // Note: Is it safe to call AbortGame when time is up? CenterDiv calls manager.AbortGame() and breaks. 
                // In legacy code it does:
                // if (currentGame.canFail) { manager.AbortGame(); yield break; }
                // else { MarkWinReady(); }
                // BUT we don't have access to `currentGame` from MiniGameConfig directly unless we do something else. Wait, I'll just follow CenterDiv logic which calls manager.AbortGame().
                
                manager.AbortGame();
                yield break;
            }
            yield return null;
        }
    }
}

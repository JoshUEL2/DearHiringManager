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
    private int draggedTicketOriginalIndex = -1;
    private Vector2 pointerStartPos;
    private Vector2 dragStartPos;

    private int kanbanTicketsDone = 0;
    
    // Aesthetic prefixes
    private string[] ticketPrefixes = { "🔴 BUG-", "🔵 TSK-", "🟣 EPC-", "🟠 OPT-" };

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        
        container = root.Q<VisualElement>("KanbanContainer");
        kanbanTimerText = container.Q<Label>("KanbanTimerText");
        kanbanColTodo = container.Q<VisualElement>("KanbanColTodo");
        kanbanColProgress = container.Q<VisualElement>("KanbanColProgress");
        kanbanColDone = container.Q<VisualElement>("KanbanColDone");
        
        CreateJuiceElements();
    }

    private void CreateJuiceElements()
    {
        // Dark Mode Linear/Jira Aesthetic
        container.style.backgroundColor = new StyleColor(new Color(0.04f, 0.06f, 0.1f)); // Deep dark background
        
        StyleColor colBg = new StyleColor(new Color(0.12f, 0.16f, 0.23f)); // Tech slate columns
        StyleColor textColor = new StyleColor(new Color(0.58f, 0.64f, 0.72f)); // Muted text for headers

        var todoBox = kanbanColTodo.parent;
        todoBox.style.backgroundColor = colBg;
        
        var progressBox = kanbanColProgress.parent;
        progressBox.style.backgroundColor = colBg;
        
        var doneBox = kanbanColDone.parent;
        doneBox.style.backgroundColor = colBg;

        // Apply a minimum pixel width to force the popup window open, while letting them flex equally
        todoBox.style.minWidth = 180;
        progressBox.style.minWidth = 180;
        doneBox.style.minWidth = 180;
        
        todoBox.style.flexGrow = 1;
        progressBox.style.flexGrow = 1;
        doneBox.style.flexGrow = 1;

        System.Action<Label, string> StyleHeader = (lbl, txt) => {
            lbl.text = txt;
            lbl.style.color = textColor;
            lbl.style.fontSize = 15;
            lbl.style.unityTextAlign = TextAnchor.MiddleCenter;
            lbl.style.alignSelf = Align.Stretch; // Override UXML center align so it fills width
            
            // Distinct header block styling
            lbl.style.backgroundColor = new StyleColor(new Color(0.08f, 0.11f, 0.16f)); 
            
            lbl.style.marginTop = -10;
            lbl.style.marginLeft = -10;
            lbl.style.marginRight = -10;
            lbl.style.marginBottom = 10;
            
            lbl.style.paddingTop = 10;
            lbl.style.paddingBottom = 10;
            
            lbl.style.borderTopLeftRadius = 8;
            lbl.style.borderTopRightRadius = 8;
            
            lbl.style.borderBottomWidth = 2;
            lbl.style.borderBottomColor = new StyleColor(new Color(0.04f, 0.06f, 0.1f));
        };

        StyleHeader(todoBox.Q<Label>(), "TODO");
        StyleHeader(progressBox.Q<Label>(), "IN\nPROGRESS");
        StyleHeader(doneBox.Q<Label>(), "DONE");
    }

    public void StartGame()
    {
        kanbanTicketsDone = 0;
        draggedTicket = null;
        container.style.display = DisplayStyle.Flex;

        kanbanColTodo.Clear();
        kanbanColProgress.Clear();
        kanbanColDone.Clear();
        kanbanTickets.Clear();

        for (int i = 0; i < config.kanbanTicketCount; i++)
        {
            var ticket = new VisualElement();
            ticket.style.width = new Length(95, LengthUnit.Percent); // Fills almost entire column
            ticket.style.height = 65; // Massive touch target for mobile
            ticket.style.marginBottom = 10;
            ticket.style.flexShrink = 0; // Prevent vertical squishing!
            
            // Ticket Styling
            ticket.style.backgroundColor = new StyleColor(new Color(0.18f, 0.22f, 0.31f)); // Slightly lighter than column
            ticket.style.borderTopLeftRadius = 8;
            ticket.style.borderTopRightRadius = 8;
            ticket.style.borderBottomLeftRadius = 8;
            ticket.style.borderBottomRightRadius = 8;
            
            // Accent left border
            ticket.style.borderLeftWidth = 6;
            ticket.style.borderLeftColor = new StyleColor(new Color(0.22f, 0.74f, 0.97f)); // Tech blue accent
            
            ticket.style.justifyContent = Justify.Center;
            ticket.style.alignItems = Align.Center;
            ticket.style.position = Position.Relative;
            
            // Add subtle shadow (using borders since UI Toolkit shadow is complex)
            ticket.style.borderBottomWidth = 3;
            ticket.style.borderBottomColor = new StyleColor(new Color(0.05f, 0.08f, 0.12f));

            string prefix = ticketPrefixes[Random.Range(0, ticketPrefixes.Length)];
            var label = new Label($"{prefix}{Random.Range(100, 999)}");
            label.style.color = new StyleColor(Color.white);
            label.style.fontSize = 20;
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
        
        // Physics Lift Juice
        ticket.style.scale = new StyleScale(new Vector2(1.05f, 1.05f));
        ticket.style.rotate = new StyleRotate(new Rotate(new Angle(3f, AngleUnit.Degree))); // Slight tilt
        ticket.style.opacity = 0.9f;
        ticket.style.backgroundColor = new StyleColor(new Color(0.24f, 0.28f, 0.38f)); // Brighten when held
        ticket.BringToFront(); // Ensure it renders over everything
        
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
        
        // Reset Lift Physics
        ticket.style.scale = new StyleScale(new Vector2(1f, 1f));
        ticket.style.rotate = new StyleRotate(new Rotate(new Angle(0f, AngleUnit.Degree)));
        ticket.style.opacity = 1f;
        ticket.style.width = new Length(95, LengthUnit.Percent);
        ticket.style.backgroundColor = new StyleColor(new Color(0.18f, 0.22f, 0.31f));

        Rect ticketRect = ticket.worldBound;
        Rect progressRect = kanbanColProgress.worldBound;
        Rect doneRect = kanbanColDone.worldBound;

        if (ticketRect.Overlaps(doneRect))
        {
            if (ticket.ClassListContains("in-progress"))
            {
                // Valid move to DONE
                SnapToColumn(ticket, kanbanColDone);
                
                // Done Juice
                ticket.style.backgroundColor = new StyleColor(new Color(0.13f, 0.77f, 0.36f)); // Green
                ticket.style.borderLeftColor = new StyleColor(new Color(0.05f, 0.4f, 0.15f)); 
                
                var label = ticket.Q<Label>();
                if (!label.text.StartsWith("✅")) 
                {
                    label.text = "✅ " + label.text;
                    label.style.color = new StyleColor(Color.black); // Contrast
                }
                
                manager.StartCoroutine(BounceRoutine(ticket));
                
                kanbanTicketsDone++;
                if (kanbanTicketsDone >= config.kanbanTicketCount) manager.MarkWinReady();
            }
            else
            {
                // Tried to skip IN PROGRESS -> Shake Red
                manager.StartCoroutine(ShakeTicketRoutine(ticket, kanbanColTodo));
            }
        }
        else if (ticketRect.Overlaps(progressRect))
        {
            // Move to IN PROGRESS
            if (!ticket.ClassListContains("in-progress")) 
            {
                ticket.AddToClassList("in-progress");
                SnapToColumn(ticket, kanbanColProgress);
                
                // Progress Juice
                ticket.style.borderLeftColor = new StyleColor(new Color(0.96f, 0.62f, 0.04f)); // Orange accent
                manager.StartCoroutine(BounceRoutine(ticket));
            }
            else
            {
                // Snapping back to same column
                SnapToColumn(ticket, kanbanColProgress, draggedTicketOriginalIndex);
            }
        }
        else
        {
            // Snap back
            if (ticket.ClassListContains("in-progress"))
            {
                SnapToColumn(ticket, kanbanColProgress, draggedTicketOriginalIndex);
            }
            else
            {
                SnapToColumn(ticket, kanbanColTodo, draggedTicketOriginalIndex);
            }
        }
    }
    
    private void SnapToColumn(VisualElement ticket, VisualElement col, int index = -1)
    {
        ticket.style.position = Position.Relative;
        ticket.style.left = 0;
        ticket.style.top = 0;
        if (ticket.parent != null) ticket.parent.Remove(ticket);
        
        if (index >= 0 && index <= col.childCount) col.Insert(index, ticket);
        else col.Add(ticket);
    }
    
    private IEnumerator BounceRoutine(VisualElement ticket)
    {
        float duration = 0.15f;
        float elapsed = 0f;
        
        while (elapsed < duration && manager.isPlaying)
        {
            elapsed += Time.deltaTime;
            float p = elapsed / duration;
            // Pop out then settle
            float scale = 1f + (Mathf.Sin(p * Mathf.PI) * 0.1f);
            ticket.style.scale = new StyleScale(new Vector2(scale, scale));
            yield return null;
        }
        ticket.style.scale = new StyleScale(new Vector2(1f, 1f));
    }

    private IEnumerator ShakeTicketRoutine(VisualElement ticket, VisualElement targetColumn)
    {
        float duration = 0.3f;
        float elapsed = 0f;
        float magnitude = 10f;
        
        // Error Juice
        StyleColor originalBg = ticket.style.backgroundColor;
        ticket.style.backgroundColor = new StyleColor(new Color(0.93f, 0.26f, 0.22f)); // Red flash
        
        SnapToColumn(ticket, targetColumn, draggedTicketOriginalIndex);
        
        while (elapsed < duration && manager.isPlaying)
        {
            elapsed += Time.deltaTime;
            float xOffset = Mathf.Sin(elapsed * 40f) * magnitude;
            ticket.style.left = xOffset;
            yield return null;
        }
        
        ticket.style.left = 0;
        ticket.style.backgroundColor = originalBg; // Restore original color
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
                manager.AbortGame();
                yield break;
            }
            yield return null;
        }
    }
}

using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class CorporateGolfMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement rootElement;
    
    private VisualElement cgContainer;
    private Label cgTimeLabel;
    private VisualElement cgPlayArea;
    private VisualElement cgBall;
    private VisualElement cgHole;
    private VisualElement cgSandTrap;
    private VisualElement cgAimLine;
    private Label cgCeo;
    
    private Vector2 cgBallPos;
    private Vector2 cgBallVel;
    private Vector2 cgDragStartPos;
    private bool cgIsDragging = false;
    private bool cgBallInMotion = false;
    private float timer;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        this.rootElement = root;
        cgContainer = root.Q<VisualElement>("CorporateGolfContainer");
        
        if (cgContainer != null)
        {
            cgTimeLabel = cgContainer.Q<Label>("CgTimeText");
            cgPlayArea = cgContainer.Q<VisualElement>("CgPlayArea");
            cgBall = cgContainer.Q<VisualElement>("CgBall");
            cgHole = cgContainer.Q<VisualElement>("CgHole");
            cgSandTrap = cgContainer.Q<VisualElement>("CgSandTrap");
            cgAimLine = cgContainer.Q<VisualElement>("CgAimLine");
            cgCeo = cgContainer.Q<Label>("CgCeo");
        }
    }

    public void StartGame()
    {
        if (cgContainer != null)
            cgContainer.style.display = DisplayStyle.Flex;
        
        timer = config.timeLimit > 0 ? config.timeLimit : 15f;
        cgIsDragging = false;
        cgBallInMotion = false;
        cgBallVel = Vector2.zero;
        
        if (cgTimeLabel != null) cgTimeLabel.text = $"TIME: {timer:F1}s";
        if (cgCeo != null) cgCeo.text = "👨‍💼";
        
        if (cgPlayArea != null && cgBall != null)
        {
            cgBallPos = new Vector2(178, 380);
            cgBall.style.left = cgBallPos.x;
            cgBall.style.top = cgBallPos.y;
            cgBall.style.bottom = new StyleLength(StyleKeyword.Null);
            cgBall.style.translate = new StyleTranslate(StyleKeyword.Null);
            
            cgPlayArea.RegisterCallback<PointerDownEvent>(OnPointerDown);
            cgPlayArea.RegisterCallback<PointerMoveEvent>(OnPointerMove);
            cgPlayArea.RegisterCallback<PointerUpEvent>(OnPointerUp);
        }

        manager.StartCoroutine(GameRoutine());
    }

    public void AbortGame()
    {
        if (cgContainer != null)
            cgContainer.style.display = DisplayStyle.None;
            
        if (cgPlayArea != null)
        {
            cgPlayArea.UnregisterCallback<PointerDownEvent>(OnPointerDown);
            cgPlayArea.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
            cgPlayArea.UnregisterCallback<PointerUpEvent>(OnPointerUp);
        }
    }

    private void OnPointerDown(PointerDownEvent ev)
    {
        if (cgBallInMotion || !manager.isPlaying) return;
        cgIsDragging = true;
        cgPlayArea.CapturePointer(ev.pointerId);
        cgDragStartPos = cgPlayArea.WorldToLocal(ev.position);
        if (cgAimLine != null) cgAimLine.style.display = DisplayStyle.Flex;
        UpdateAimLine(cgDragStartPos, cgDragStartPos);
    }

    private void OnPointerMove(PointerMoveEvent ev)
    {
        if (cgIsDragging && manager.isPlaying) {
            UpdateAimLine(cgDragStartPos, cgPlayArea.WorldToLocal(ev.position));
        }
    }

    private void OnPointerUp(PointerUpEvent ev)
    {
        if (!cgIsDragging) return;
        cgIsDragging = false;
        cgPlayArea.ReleasePointer(ev.pointerId);
        if (cgAimLine != null) cgAimLine.style.display = DisplayStyle.None;
        
        Vector2 dragEndPos = cgPlayArea.WorldToLocal(ev.position);
        Vector2 dragVector = cgDragStartPos - dragEndPos;
        
        if (dragVector.sqrMagnitude > 400f) {
            cgBallVel = dragVector * 4f;
            cgBallInMotion = true;
        }
    }

    private void UpdateAimLine(Vector2 startPos, Vector2 currentPos)
    {
        if (cgAimLine == null || cgBall == null) return;
        
        Vector2 dragVector = startPos - currentPos; 
        float distance = dragVector.magnitude;
        if (distance > 150f) { dragVector = dragVector.normalized * 150f; distance = 150f; }
        
        Vector2 ballCenter = cgBallPos + new Vector2(12f, 12f);
        
        cgAimLine.style.left = ballCenter.x - 4f;
        cgAimLine.style.top = ballCenter.y;
        cgAimLine.style.height = distance;
        
        float angle = Mathf.Atan2(dragVector.y, dragVector.x) * Mathf.Rad2Deg;
        cgAimLine.style.rotate = new Rotate(new Angle(angle - 90f, AngleUnit.Degree));

        float t = distance / 150f;
        Color color;
        if (t < 0.5f) {
            color = Color.Lerp(Color.yellow, new Color(1f, 0.5f, 0f), t * 2f);
        } else {
            color = Color.Lerp(new Color(1f, 0.5f, 0f), Color.red, (t - 0.5f) * 2f);
        }
        
        cgAimLine.style.backgroundColor = color;
        
        Label arrow = cgAimLine.Q<Label>("CgAimArrow");
        if (arrow != null) {
            arrow.style.color = color;
        }
    }

    private IEnumerator GameRoutine()
    {
        while (manager.isPlaying)
        {
            timer -= Time.deltaTime;
            if (cgTimeLabel != null) cgTimeLabel.text = $"TIME: {Mathf.Max(0, timer):F1}s";
            
            if (timer <= 0f)
            {
                if (cgCeo != null) cgCeo.text = "😡";
                manager.EndGame(false);
                yield break;
            }
            
            if (cgBallInMotion)
            {
                cgBallPos += cgBallVel * Time.deltaTime;
                cgBall.style.left = cgBallPos.x;
                cgBall.style.top = cgBallPos.y;
                
                cgBallVel = Vector2.MoveTowards(cgBallVel, Vector2.zero, 500f * Time.deltaTime);
                
                if (cgBallPos.x < 0) { cgBallPos.x = 0; cgBallVel.x *= -0.7f; }
                if (cgBallPos.x > 356) { cgBallPos.x = 356; cgBallVel.x *= -0.7f; } 
                if (cgBallPos.y < 0) { cgBallPos.y = 0; cgBallVel.y *= -0.7f; }
                if (cgBallPos.y > 416) { cgBallPos.y = 416; cgBallVel.y *= -0.7f; } 
                
                if (cgBallVel.sqrMagnitude < 5f)
                {
                    cgBallInMotion = false;
                    cgBallVel = Vector2.zero;
                    
                    Rect ballRect = new Rect(cgBall.worldBound.x, cgBall.worldBound.y, cgBall.worldBound.width, cgBall.worldBound.height);
                    Rect holeRect = new Rect(cgHole.worldBound.x, cgHole.worldBound.y, cgHole.worldBound.width, cgHole.worldBound.height);
                    Rect sandRect = new Rect(cgSandTrap.worldBound.x, cgSandTrap.worldBound.y, cgSandTrap.worldBound.width, cgSandTrap.worldBound.height);
                    
                    if (ballRect.Overlaps(holeRect))
                    {
                        if (cgCeo != null) cgCeo.text = "😡";
                        AudioManager.Instance?.PlayBuzzer();
                        MiniGameFeedback.PlayError(cgHole, "YOU BEAT THE CEO!", cgPlayArea ?? cgContainer);
                        yield return new WaitForSeconds(1f);
                        manager.EndGame(false);
                        yield break;
                    }
                    else if (ballRect.Overlaps(sandRect))
                    {
                        if (cgCeo != null) cgCeo.text = "🤑";
                        AudioManager.Instance?.PlayScorePop();
                        MiniGameFeedback.PlaySuccess(cgSandTrap, "CEO WON! PROMOTED!", cgPlayArea ?? cgContainer);
                        yield return new WaitForSeconds(1f);
                        manager.EndGame(true);
                        yield break;
                    }
                }
            }
            yield return null;
        }
    }
}
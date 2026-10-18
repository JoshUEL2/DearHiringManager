using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class IncineratorTossMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement container;
    private VisualElement drawArea;
    private Label uiText;
    private Label bagLabel;

    private float doorState = 0f; // 0 = fully closed, 1 = fully open
    private float doorTimer = 0f;
    private float doorCycleSpeed = 1.5f;

    private Vector2 bagPos = new Vector2(200, 320);
    private Vector2 bagVel = Vector2.zero;
    private float bagRot = 0f;
    private float bagRotVel = 0f;
    private bool isBagFlying = false;
    private bool isDead = false;
    
    private int bagsTossed = 0;
    private int targetBags = 5;
    private int health = 3;
    
    private float flashTimer = 0f;
    private Vector2 explosionPos;
    private float explosionTimer = 0f;
    private bool lastTossSuccess = false;

    private Vector2 swipeStart;
    private bool isSwiping = false;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;

        container = new VisualElement();
        container.style.width = Length.Percent(100);
        container.style.height = Length.Percent(100);
        container.style.alignItems = Align.Center;
        container.style.justifyContent = Justify.Center;
        container.style.display = DisplayStyle.None;
        root.Add(container);

        uiText = new Label();
        uiText.style.position = Position.Absolute;
        uiText.style.top = 10;
        uiText.style.left = 10;
        uiText.style.color = Color.white;
        uiText.style.fontSize = 24;
        container.Add(uiText);

        drawArea = new VisualElement();
        drawArea.style.width = 400;
        drawArea.style.height = 400;
        drawArea.style.backgroundColor = new StyleColor(new Color(0.1f, 0.12f, 0.15f));
        drawArea.style.borderTopLeftRadius = 20;
        drawArea.style.borderTopRightRadius = 20;
        drawArea.style.borderBottomLeftRadius = 20;
        drawArea.style.borderBottomRightRadius = 20;
        drawArea.style.overflow = Overflow.Hidden;
        drawArea.generateVisualContent += OnDrawGame;
        container.Add(drawArea);

        bagLabel = new Label("🗑️");
        bagLabel.style.position = Position.Absolute;
        bagLabel.style.fontSize = 50;
        bagLabel.style.color = Color.white;
        bagLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
        drawArea.Add(bagLabel);
    }

    public void StartGame()
    {
        bagsTossed = 0;
        health = 1;
        bagPos = new Vector2(200, 320);
        bagVel = Vector2.zero;
        bagRot = 0f;
        bagRotVel = 0f;
        isBagFlying = false;
        isDead = false;
        doorState = 0f;
        doorTimer = 0f;

        container.style.display = DisplayStyle.Flex;
        
        container.RegisterCallback<PointerDownEvent>(OnPointerDown);
        container.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        container.RegisterCallback<PointerUpEvent>(OnPointerUp);

        manager.StartCoroutine(GameRoutine());
    }

    public void AbortGame()
    {
        container.style.display = DisplayStyle.None;
        container.UnregisterCallback<PointerDownEvent>(OnPointerDown);
        container.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
        container.UnregisterCallback<PointerUpEvent>(OnPointerUp);
    }

    private void OnPointerDown(PointerDownEvent ev)
    {
        if (isBagFlying || !manager.isPlaying) return;
        swipeStart = ev.position;
        isSwiping = true;
    }

    private void OnPointerMove(PointerMoveEvent ev)
    {
        if (!isSwiping || isBagFlying || isDead) return;
        Vector2 delta = (Vector2)ev.position - swipeStart;
        if (delta.y < -40f) // Swiped up
        {
            isBagFlying = true;
            bagVel = new Vector2(0, -900f); // Throw velocity!
            bagRotVel = Random.Range(-100f, 100f);
            isSwiping = false;
        }
    }

    private void OnPointerUp(PointerUpEvent ev)
    {
        isSwiping = false;
    }

    private IEnumerator GameRoutine()
    {
        while (manager != null && manager.isPlaying)
        {
            uiText.text = $"❤️ {health}   Bags: {bagsTossed}/{targetBags}";

            doorTimer += Time.deltaTime * doorCycleSpeed;
            // Sine wave for door state: open, hold, close, hold
            float wave = Mathf.Sin(doorTimer);
            if (wave > 0.5f) doorState = Mathf.Lerp(doorState, 1f, Time.deltaTime * 10f); // snap open
            else if (wave < -0.5f) doorState = Mathf.Lerp(doorState, 0f, Time.deltaTime * 10f); // snap closed

            if (isBagFlying || isDead)
            {
                bagPos += bagVel * Time.deltaTime;
                bagVel.y += 1800f * Time.deltaTime; // Gravity pulling it down!
                bagRot += bagRotVel * Time.deltaTime; // Spin!

                // Hit furnace line check
                if (bagPos.y < 110f && bagVel.y < 0 && !isDead)
                {
                    if (doorState > 0.7f) // Success!
                    {
                        bagsTossed++;
                        TriggerExplosion(true);
                        if (bagsTossed >= targetBags) manager.MarkWinReady();
                        
                        isBagFlying = false;
                        bagPos = new Vector2(200, 320);
                        bagVel = Vector2.zero;
                        bagRot = 0f;
                    }
                    else // Hit closed door - DEATH!
                    {
                        health--;
                        TriggerExplosion(false);
                        flashTimer = 0.2f;
                        isDead = true; // Trigger death animation
                        bagVel = new Vector2(Random.Range(-300f, 300f), 300f); // Bounce off the doors!
                        bagRotVel = Random.Range(-720f, 720f); // Violent spin
                    }
                }
                
                // If it falls off screen while dead
                if (isDead && bagPos.y > 500f) {
                    manager.AbortGame();
                }
                
                // If missed throw entirely (didn't reach) and fell off
                if (!isDead && isBagFlying && bagPos.y > 500f) {
                    bagPos = new Vector2(200, 320);
                    bagVel = Vector2.zero;
                    bagRot = 0f;
                    isBagFlying = false;
                }
            }
            
            if (flashTimer > 0) flashTimer -= Time.deltaTime;
            if (explosionTimer > 0) explosionTimer -= Time.deltaTime;

            if (!isBagFlying || isDead) {
                bagLabel.style.display = DisplayStyle.Flex;
                bagLabel.style.left = bagPos.x - 25f;
                bagLabel.style.top = bagPos.y - 25f;
                bagLabel.style.rotate = new StyleRotate(new Rotate(new Angle(bagRot, AngleUnit.Degree)));
                bagLabel.style.scale = new StyleScale(new Scale(new Vector2(1f, 1f)));
            } else if (isBagFlying && explosionTimer <= 0) {
                bagLabel.style.display = DisplayStyle.Flex;
                bagLabel.style.left = bagPos.x - 25f;
                bagLabel.style.top = bagPos.y - 25f;
                bagLabel.style.rotate = new StyleRotate(new Rotate(new Angle(bagRot, AngleUnit.Degree)));
                float distScale = Mathf.Clamp(bagPos.y / 320f, 0.4f, 1f); // Shrink as it goes up!
                bagLabel.style.scale = new StyleScale(new Scale(new Vector2(distScale, distScale)));
            } else {
                bagLabel.style.display = DisplayStyle.None;
            }

            if (drawArea != null) drawArea.MarkDirtyRepaint();
            yield return null;
        }
    }

    private void TriggerExplosion(bool success)
    {
        lastTossSuccess = success;
        explosionPos = new Vector2(200, 80);
        explosionTimer = 0.5f;
        if (success)
        {
            MiniGameFeedback.PlaySuccess(drawArea, "INCINERATED!", container);
        }
        else
        {
            MiniGameFeedback.PlayError(drawArea, "FURNACE CLOSED!", container);
        }
    }

    private void OnDrawGame(MeshGenerationContext ctx)
    {
        var p = ctx.painter2D;
        float w = ctx.visualElement.layout.width;
        float h = ctx.visualElement.layout.height;
        if (w == 0) return;

        // Factory Ambient Background
        p.fillColor = new Color(0.08f, 0.08f, 0.1f);
        p.BeginPath(); p.MoveTo(new Vector2(0,0)); p.LineTo(new Vector2(w,0)); p.LineTo(new Vector2(w,h)); p.LineTo(new Vector2(0,h)); p.Fill();

        // 1. FURNACE INTERIOR (The roaring fire)
        float furnaceLeft = 50f;
        float furnaceRight = 350f;
        float furnaceTop = 15f;
        float furnaceBottom = 110f;

        p.fillColor = new Color(0.9f, 0.3f, 0f);
        p.BeginPath(); p.MoveTo(new Vector2(furnaceLeft, furnaceTop)); p.LineTo(new Vector2(furnaceRight, furnaceTop)); 
        p.LineTo(new Vector2(furnaceRight, furnaceBottom)); p.LineTo(new Vector2(furnaceLeft, furnaceBottom)); p.Fill();
        
        p.fillColor = new Color(1f, 0.6f, 0f);
        for(int i=0; i<8; i++) {
            float fireX = 70f + i * 35f;
            float pulse = Mathf.Sin(Time.time * 15f + i) * 10f;
            p.BeginPath(); p.Arc(new Vector2(fireX, furnaceBottom + 10f), 30f + pulse, 0, 360); p.Fill();
        }
        p.fillColor = new Color(1f, 0.9f, 0.2f);
        for(int i=0; i<6; i++) {
            float fireX = 90f + i * 40f;
            float pulse = Mathf.Sin(Time.time * 20f + i * 2f) * 15f;
            p.BeginPath(); p.Arc(new Vector2(fireX, furnaceBottom + 20f), 20f + pulse, 0, 360); p.Fill();
        }

        // Floating Embers
        p.fillColor = new Color(1f, 0.8f, 0.2f, 0.8f);
        Random.State oldState = Random.state;
        Random.InitState(Mathf.FloorToInt(Time.time * 5f));
        for(int i=0; i<15; i++) {
            float ex = Random.Range(furnaceLeft + 10f, furnaceRight - 10f);
            float ey = Random.Range(furnaceTop + 10f, furnaceBottom - 10f);
            p.BeginPath(); p.Arc(new Vector2(ex, ey), Random.Range(2f, 5f), 0, 360); p.Fill();
        }
        Random.state = oldState;

        // 2. FURNACE EXTERIOR SHELL (Dark iron)
        p.fillColor = new Color(0.15f, 0.15f, 0.18f);
        p.BeginPath(); p.MoveTo(new Vector2(0, 0)); p.LineTo(new Vector2(furnaceLeft, 0)); p.LineTo(new Vector2(furnaceLeft, 130)); p.LineTo(new Vector2(0, 130)); p.Fill();
        p.BeginPath(); p.MoveTo(new Vector2(furnaceRight, 0)); p.LineTo(new Vector2(w, 0)); p.LineTo(new Vector2(w, 130)); p.LineTo(new Vector2(furnaceRight, 130)); p.Fill();
        p.BeginPath(); p.MoveTo(new Vector2(0, 0)); p.LineTo(new Vector2(w, 0)); p.LineTo(new Vector2(w, furnaceTop)); p.LineTo(new Vector2(0, furnaceTop)); p.Fill();
        p.BeginPath(); p.MoveTo(new Vector2(0, furnaceBottom)); p.LineTo(new Vector2(w, furnaceBottom)); p.LineTo(new Vector2(w, 200)); p.LineTo(new Vector2(0, 200)); p.Fill();
        
        p.fillColor = new Color(0.08f, 0.08f, 0.1f);
        for(float x = 20; x <= w; x += 40) {
            p.BeginPath(); p.Arc(new Vector2(x, 8), 4, 0, 360); p.Fill();
            p.BeginPath(); p.Arc(new Vector2(x, 122), 4, 0, 360); p.Fill();
            p.BeginPath(); p.Arc(new Vector2(x, 182), 4, 0, 360); p.Fill();
        }

        // 3. BLAST DOORS (Heavy steel with hazard stripes)
        float doorWidth = 150f;
        float leftDoorX = 50 - (doorState * doorWidth);
        float rightDoorX = 200 + (doorState * doorWidth);

        p.fillColor = new Color(0.25f, 0.25f, 0.3f);
        p.BeginPath(); p.MoveTo(new Vector2(leftDoorX, furnaceTop)); p.LineTo(new Vector2(leftDoorX + doorWidth, furnaceTop)); 
        p.LineTo(new Vector2(leftDoorX + doorWidth, furnaceBottom)); p.LineTo(new Vector2(leftDoorX, furnaceBottom)); p.Fill();
        
        p.fillColor = new Color(0.8f, 0.7f, 0.1f);
        p.BeginPath(); p.MoveTo(new Vector2(leftDoorX + doorWidth - 20, furnaceTop)); p.LineTo(new Vector2(leftDoorX + doorWidth, furnaceTop)); 
        p.LineTo(new Vector2(leftDoorX + doorWidth, furnaceBottom)); p.LineTo(new Vector2(leftDoorX + doorWidth - 20, furnaceBottom)); p.Fill();
        
        p.strokeColor = new Color(0.1f, 0.1f, 0.1f);
        p.lineWidth = 8f;
        for (float y = furnaceTop + 5; y < furnaceBottom; y += 20) {
            p.BeginPath(); p.MoveTo(new Vector2(leftDoorX + doorWidth - 20, y)); p.LineTo(new Vector2(leftDoorX + doorWidth, y + 15)); p.Stroke();
        }

        p.fillColor = new Color(0.25f, 0.25f, 0.3f);
        p.BeginPath(); p.MoveTo(new Vector2(rightDoorX, furnaceTop)); p.LineTo(new Vector2(rightDoorX + doorWidth, furnaceTop)); 
        p.LineTo(new Vector2(rightDoorX + doorWidth, furnaceBottom)); p.LineTo(new Vector2(rightDoorX, furnaceBottom)); p.Fill();
        
        p.fillColor = new Color(0.8f, 0.7f, 0.1f);
        p.BeginPath(); p.MoveTo(new Vector2(rightDoorX, furnaceTop)); p.LineTo(new Vector2(rightDoorX + 20, furnaceTop)); 
        p.LineTo(new Vector2(rightDoorX + 20, furnaceBottom)); p.LineTo(new Vector2(rightDoorX, furnaceBottom)); p.Fill();
        
        p.strokeColor = new Color(0.1f, 0.1f, 0.1f);
        for (float y = furnaceTop + 5; y < furnaceBottom; y += 20) {
            p.BeginPath(); p.MoveTo(new Vector2(rightDoorX, y)); p.LineTo(new Vector2(rightDoorX + 20, y - 15)); p.Stroke();
        }
        
        p.strokeColor = new Color(0.05f, 0.05f, 0.05f);
        p.lineWidth = 4f;
        p.BeginPath(); p.MoveTo(new Vector2(leftDoorX + doorWidth, furnaceTop)); p.LineTo(new Vector2(leftDoorX + doorWidth, furnaceBottom)); p.Stroke();

        // 4. EXPLOSION / SPLATTER FEEDBACK
        if (explosionTimer > 0)
        {
            if (lastTossSuccess) {
                p.fillColor = new Color(1f, 0.6f, 0f, explosionTimer * 2f);
                p.BeginPath(); p.Arc(explosionPos, (0.5f - explosionTimer) * 300f, 0, 360); p.Fill();
                p.fillColor = new Color(1f, 0.9f, 0.2f, explosionTimer * 2f);
                p.BeginPath(); p.Arc(explosionPos, (0.5f - explosionTimer) * 150f, 0, 360); p.Fill();
            } else {
                p.fillColor = new Color(0.3f, 0.8f, 0.1f, explosionTimer * 2f);
                Random.InitState(42);
                for(int i=0; i<12; i++) {
                    p.BeginPath(); p.Arc(explosionPos + new Vector2(Random.Range(-50f, 50f), Random.Range(-30f, 30f)), Random.Range(10f, 25f), 0, 360); p.Fill();
                }
            }
        }

        // 5. TRASH BAG IS NOW A LABEL
        
        if (flashTimer > 0)
        {
            p.fillColor = new Color(1f, 0f, 0f, 0.4f);
            p.BeginPath(); p.MoveTo(new Vector2(0,0)); p.LineTo(new Vector2(w,0)); p.LineTo(new Vector2(w,h)); p.LineTo(new Vector2(0,h)); p.Fill();
        }
    }
}

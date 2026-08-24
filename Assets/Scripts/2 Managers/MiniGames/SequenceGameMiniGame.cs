using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class SequenceGameMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement rootContainer;
    
    private VisualElement qteContainer;
    private VisualElement dialElement;
    private VisualElement trampoline;
    private Label emojiLabel;
    private Label instructionLabel;
    private Label flipCounterLabel;

    private bool isPlaying = false;
    private int successfulFlips = 0;
    
    private float needleAngle = 0f;
    private float needleSpeed = 300f; 
    
    private float targetStart = 45f;
    private float targetEnd = 135f;
    private bool roundActive = false;
    private bool isFlashingGreen = false;
    private bool isFlashingRed = false;
    
    private float emojiY = 280f;
    private float emojiRotation = 0f;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        this.rootContainer = root;
        
        var old = root.Q<VisualElement>("SequenceGameContainer");
        if (old != null) old.style.display = DisplayStyle.None;
        
        qteContainer = new VisualElement();
        qteContainer.style.position = Position.Relative;
        qteContainer.style.width = 380; // Explicit pixels to prevent shrink-wrap!
        qteContainer.style.height = 400; 
        qteContainer.style.alignSelf = Align.Center; // Center it horizontally
        root.Add(qteContainer); 
        qteContainer.style.backgroundColor = new StyleColor(new Color(0.06f, 0.09f, 0.16f)); 
        qteContainer.style.borderTopLeftRadius = 15; qteContainer.style.borderTopRightRadius = 15;
        qteContainer.style.borderBottomLeftRadius = 15; qteContainer.style.borderBottomRightRadius = 15;
        qteContainer.style.borderTopWidth = 4; qteContainer.style.borderBottomWidth = 4;
        qteContainer.style.borderLeftWidth = 4; qteContainer.style.borderRightWidth = 4;
        qteContainer.style.borderTopColor = new StyleColor(new Color(0.2f, 0.25f, 0.35f));
        qteContainer.style.borderBottomColor = new StyleColor(new Color(0.2f, 0.25f, 0.35f));
        qteContainer.style.borderLeftColor = new StyleColor(new Color(0.2f, 0.25f, 0.35f));
        qteContainer.style.borderRightColor = new StyleColor(new Color(0.2f, 0.25f, 0.35f));
        qteContainer.style.overflow = Overflow.Hidden;

        // Background Speed Lines (Optional, but let's just make it dark)
        
        // Trampoline at the bottom
        trampoline = new VisualElement();
        trampoline.style.position = Position.Absolute;
        trampoline.style.bottom = 10;
        trampoline.style.left = new Length(10, LengthUnit.Percent);
        trampoline.style.width = new Length(80, LengthUnit.Percent);
        trampoline.style.height = 20;
        trampoline.style.backgroundColor = new StyleColor(new Color(0.9f, 0.2f, 0.2f)); // Red safety net
        trampoline.style.borderTopLeftRadius = 10; trampoline.style.borderTopRightRadius = 10;
        trampoline.style.borderBottomLeftRadius = 10; trampoline.style.borderBottomRightRadius = 10;
        qteContainer.Add(trampoline);

        instructionLabel = new Label("HIT THE GREEN ZONES!");
        instructionLabel.style.position = Position.Absolute;
        instructionLabel.style.top = 10;
        instructionLabel.style.left = 0; instructionLabel.style.right = 0;
        instructionLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
        instructionLabel.style.fontSize = 20;
        instructionLabel.style.color = new StyleColor(Color.white);
        instructionLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        qteContainer.Add(instructionLabel);

        flipCounterLabel = new Label("FLIPS: 0 / 3");
        flipCounterLabel.style.position = Position.Absolute;
        flipCounterLabel.style.top = 40;
        flipCounterLabel.style.left = 0; flipCounterLabel.style.right = 0;
        flipCounterLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
        flipCounterLabel.style.fontSize = 24;
        flipCounterLabel.style.color = new StyleColor(new Color(0.18f, 0.8f, 0.44f));
        flipCounterLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        qteContainer.Add(flipCounterLabel);

        // Center Dial moving vertically
        dialElement = new VisualElement();
        dialElement.style.position = Position.Absolute;
        dialElement.style.width = 200;
        dialElement.style.height = 200;
        dialElement.style.left = new Length(50, LengthUnit.Percent);
        dialElement.style.marginLeft = -100; // Perfect horizontal center
        dialElement.generateVisualContent += DrawDial;
        dialElement.style.alignItems = Align.Center;
        dialElement.style.justifyContent = Justify.Center;
        qteContainer.Add(dialElement);

        emojiLabel = new Label("🤸");
        emojiLabel.style.fontSize = 80;
        // Adjusting top margin slightly to perfectly center unicode emojis visually
        emojiLabel.style.marginTop = -10; 
        dialElement.Add(emojiLabel);

        // Invisible full-screen tap area
        var tapArea = new VisualElement();
        tapArea.style.position = Position.Absolute;
        tapArea.style.left = 0; tapArea.style.right = 0; tapArea.style.top = 0; tapArea.style.bottom = 0;
        tapArea.RegisterCallback<PointerDownEvent>(OnPointerDown);
        qteContainer.Add(tapArea);
    }

    public void StartGame()
    {
        if (qteContainer != null) qteContainer.style.display = DisplayStyle.Flex;
        isPlaying = true;
        successfulFlips = 0;
        isFlashingGreen = false;
        isFlashingRed = false;
        roundActive = false;
        emojiY = 260f;
        dialElement.style.top = emojiY;
        
        if (flipCounterLabel != null) flipCounterLabel.text = "FLIPS: 0 / 3";
        if (emojiLabel != null) 
        {
            emojiLabel.text = "🤸";
            emojiLabel.style.rotate = new StyleRotate(new Rotate(new Angle(0)));
        }
        
        manager.StartCoroutine(TrampolineRoutine());
    }

    public void AbortGame()
    {
        isPlaying = false;
        if (qteContainer != null) qteContainer.style.display = DisplayStyle.None;
    }

    private void DrawDial(MeshGenerationContext ctx)
    {
        var p = ctx.painter2D;
        float radius = 90f;
        Vector2 center = new Vector2(100f, 100f);

        // Background Track
        if (isFlashingGreen) p.strokeColor = new Color(0.18f, 0.8f, 0.44f); // Solid green
        else if (isFlashingRed) p.strokeColor = new Color(0.9f, 0.2f, 0.2f); // Solid red
        else p.strokeColor = new Color(0.12f, 0.16f, 0.23f);
        
        p.lineWidth = 16f;
        p.lineCap = LineCap.Round;
        p.BeginPath();
        p.Arc(center, radius, 0, 360);
        p.Stroke();

        if (roundActive)
        {
            // Target Zone (Green)
            p.strokeColor = new Color(0.18f, 0.8f, 0.44f);
            p.lineWidth = 16f;
            p.BeginPath();
            p.Arc(center, radius, targetStart, targetEnd);
            p.Stroke();
            
            // Needle
            p.strokeColor = Color.white;
            p.lineWidth = 5f;
            p.BeginPath();
            p.MoveTo(center);
            float needleRad = needleAngle * Mathf.Deg2Rad;
            Vector2 tip = center + new Vector2(Mathf.Cos(needleRad), Mathf.Sin(needleRad)) * (radius + 10f);
            p.LineTo(tip);
            p.Stroke();
            
            // Center Pin
            p.fillColor = Color.white;
            p.BeginPath();
            p.Arc(center, 6f, 0, 360);
            p.Fill();
        }
    }

    private void OnPointerDown(PointerDownEvent ev)
    {
        if (!isPlaying || !manager.isPlaying || !roundActive) return;

        float normalizedNeedle = needleAngle % 360f;
        if (normalizedNeedle < 0) normalizedNeedle += 360f;
        
        bool hit = false;
        if (targetStart < targetEnd)
        {
            hit = normalizedNeedle >= targetStart && normalizedNeedle <= targetEnd;
        }
        else
        {
            hit = normalizedNeedle >= targetStart || normalizedNeedle <= targetEnd;
        }

        roundActive = false; 

        if (hit)
        {
            successfulFlips++;
            if (flipCounterLabel != null) flipCounterLabel.text = $"FLIPS: {successfulFlips} / 3";
        }
        else
        {
            isPlaying = false; // Trigger fail immediately
            manager.StartCoroutine(FailAnimation());
        }
    }

    private IEnumerator TrampolineRoutine()
    {
        // 1. HUGE JUMP from trampoline (bottom to top)
        float jumpTime = 0.6f;
        float elapsed = 0;
        while(elapsed < jumpTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / jumpTime;
            float easeOut = 1f - Mathf.Pow(1f - t, 3f);
            emojiY = Mathf.Lerp(260f, 20f, easeOut);
            dialElement.style.top = emojiY;
            yield return null;
        }
        
        // 2. The Descent (3 QTE Rounds)
        for (int i = 0; i < 3; i++)
        {
            if (!isPlaying) yield break;

            float zoneWidth = 60f - (i * 12f); // Gets smaller each flip!
            targetStart = Random.Range(0f, 360f);
            targetEnd = (targetStart + zoneWidth) % 360f;
            needleSpeed = 220f + (i * 80f); // Gets faster each flip!
            
            // Bullet time setup
            float roundTime = 2.0f; // 2 seconds to react per slow-fall
            float fallStart = emojiY;
            float fallEnd = fallStart + 20f; // Only fall 20px during the 2s window (slow-mo!)
            
            roundActive = true;
            float roundElapsed = 0f;
            
            while (roundActive && isPlaying && manager.isPlaying)
            {
                roundElapsed += Time.deltaTime;
                float t = roundElapsed / roundTime;
                
                needleAngle += needleSpeed * Time.deltaTime;
                if (needleAngle >= 360f) needleAngle -= 360f;
                dialElement.MarkDirtyRepaint();
                
                // Slow-mo falling
                emojiY = Mathf.Lerp(fallStart, fallEnd, t);
                dialElement.style.top = emojiY;
                
                if (roundElapsed >= roundTime)
                {
                    // Timeout!
                    roundActive = false;
                    isPlaying = false;
                    manager.StartCoroutine(FailAnimation());
                    yield break;
                }
                
                yield return null;
            }
            
            if (!isPlaying) yield break; // Failed via tap
            
            // Success hit! Fast spin and drop
            isFlashingGreen = true;
            dialElement.MarkDirtyRepaint();
            
            float spinStart = emojiY;
            float spinEnd = 20f + ((i + 1) * 75f); // Fast drop to the next station
            
            float spinTime = 0.35f;
            float sElapsed = 0f;
            while(sElapsed < spinTime)
            {
                sElapsed += Time.deltaTime;
                float t = sElapsed / spinTime;
                
                emojiY = Mathf.Lerp(spinStart, spinEnd, t);
                dialElement.style.top = emojiY;
                
                emojiLabel.style.rotate = new StyleRotate(new Rotate(new Angle(t * 360f, AngleUnit.Degree)));
                
                yield return null;
            }
            
            emojiLabel.style.rotate = new StyleRotate(new Rotate(new Angle(0)));
            isFlashingGreen = false;
            dialElement.MarkDirtyRepaint();
        }
        
        // 3. Perfect Landing!
        float landTime = 0.3f;
        float lElapsed = 0f;
        float finalFallStart = emojiY;
        while(lElapsed < landTime)
        {
            lElapsed += Time.deltaTime;
            emojiY = Mathf.Lerp(finalFallStart, 230f, lElapsed / landTime); // Land slightly above the net
            dialElement.style.top = emojiY;
            
            // Bouncy landing squish
            if (lElapsed > landTime * 0.5f) {
                emojiLabel.style.scale = new StyleScale(new Vector2(1.2f, 0.8f));
            }
            yield return null;
        }
        emojiLabel.style.scale = new StyleScale(new Vector2(1f, 1f));
        
        manager.StartCoroutine(StampEffectRoutine("NAILED IT!", new Color(0.18f, 0.8f, 0.44f), true));
    }

    private IEnumerator FailAnimation()
    {
        isFlashingRed = true;
        dialElement.MarkDirtyRepaint();
        
        emojiLabel.text = "😵";
        emojiLabel.style.rotate = new StyleRotate(new Rotate(new Angle(180f, AngleUnit.Degree))); 
        
        // Fast drop to bottom
        float dropTime = 0.3f;
        float elapsed = 0f;
        float currentY = emojiY;
        while(elapsed < dropTime)
        {
            elapsed += Time.deltaTime;
            emojiY = Mathf.Lerp(currentY, 260f, elapsed / dropTime);
            dialElement.style.top = emojiY;
            yield return null;
        }
        
        // Screen shake
        qteContainer.style.translate = new Translate(Random.Range(-10, 10), Random.Range(-10, 10), 0);
        yield return new WaitForSeconds(0.05f);
        qteContainer.style.translate = new Translate(Random.Range(-10, 10), Random.Range(-10, 10), 0);
        yield return new WaitForSeconds(0.05f);
        qteContainer.style.translate = new Translate(0, 0, 0);
        
        manager.StartCoroutine(StampEffectRoutine("FACEPLANT!", new Color(0.93f, 0.26f, 0.22f), false));
    }

    private IEnumerator StampEffectRoutine(string text, Color color, bool isWin)
    {
        Label stampText = new Label(text);
        stampText.style.position = Position.Absolute;
        stampText.style.color = new StyleColor(color);
        stampText.style.fontSize = 90;
        stampText.style.letterSpacing = 5;
        stampText.style.rotate = new StyleRotate(new Rotate(new Angle(-12f, AngleUnit.Degree)));
        stampText.style.textShadow = new StyleTextShadow(new TextShadow { color = new Color(0,0,0,0.8f), offset = new Vector2(12f, 15f), blurRadius = 15f });
        stampText.style.unityFontStyleAndWeight = FontStyle.Bold;
        stampText.style.unityTextAlign = TextAnchor.MiddleCenter;
        
        VisualElement backgroundFade = new VisualElement();
        backgroundFade.style.position = Position.Absolute;
        backgroundFade.style.left = 0; backgroundFade.style.right = 0; backgroundFade.style.top = 0; backgroundFade.style.bottom = 0;
        backgroundFade.style.backgroundColor = new StyleColor(new Color(0f, 0f, 0f, 0.7f));
        backgroundFade.style.opacity = 0f;
        
        stampText.style.top = new Length(50, LengthUnit.Percent);
        stampText.style.left = new Length(50, LengthUnit.Percent);
        stampText.style.translate = new Translate(new Length(-50, LengthUnit.Percent), new Length(-50, LengthUnit.Percent));
        
        manager.miniGameDocument.rootVisualElement.Add(backgroundFade);
        manager.miniGameDocument.rootVisualElement.Add(stampText);
        
        float life = 1.0f;
        float elapsed = 0f;
        while(elapsed < life)
        {
            elapsed += Time.deltaTime;
            float p = elapsed / life;
            
            float scale = Mathf.Lerp(3.5f, 1f, p * 12f);
            if (scale < 1f) scale = 1f;
            stampText.style.scale = new StyleScale(new Vector2(scale, scale));
            
            backgroundFade.style.opacity = Mathf.Lerp(0f, 1f, p * 8f);
            
            if (p > 0.8f) {
                float fadeOut = 1f - ((p - 0.8f) * 5f);
                stampText.style.opacity = fadeOut;
                backgroundFade.style.opacity = fadeOut;
            }
            
            yield return null;
        }
        
        if (manager.miniGameDocument.rootVisualElement.Contains(stampText)) manager.miniGameDocument.rootVisualElement.Remove(stampText);
        if (manager.miniGameDocument.rootVisualElement.Contains(backgroundFade)) manager.miniGameDocument.rootVisualElement.Remove(backgroundFade);
        
        if (isWin) manager.MarkWinReady();
        else {
            if (config.canFail) manager.AbortGame();
            else manager.MarkWinReady(); 
        }
    }
}

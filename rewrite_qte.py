import re

path = 'Assets/Scripts/2 Managers/MiniGames/SequenceGameMiniGame.cs'

code = '''using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class SequenceGameMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement rootContainer;
    
    private VisualElement qteContainer;
    private VisualElement dialElement;
    private Label emojiLabel;
    private Label instructionLabel;
    private Label flipCounterLabel;

    private bool isPlaying = false;
    private int successfulFlips = 0;
    
    private float needleAngle = 0f;
    private float needleSpeed = 300f; // degrees per second
    
    private float targetStart = 45f;
    private float targetEnd = 135f;
    private bool roundActive = false;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        this.rootContainer = root;
        
        // Hide the original UXML container to avoid shrink issues
        var oldContainer = root.Q<VisualElement>("SequenceGameContainer");
        if (oldContainer != null) oldContainer.style.display = DisplayStyle.None;

        // Build a brand new absolute container that forcefully fills the minigame area
        qteContainer = new VisualElement();
        qteContainer.style.position = Position.Absolute;
        qteContainer.style.left = 0; qteContainer.style.right = 0; qteContainer.style.top = 0; qteContainer.style.bottom = 0;
        qteContainer.style.minHeight = 400; // Guarantee minimum height
        qteContainer.style.backgroundColor = new StyleColor(new Color(0.06f, 0.09f, 0.16f)); // Dark background
        qteContainer.style.borderTopLeftRadius = 15; qteContainer.style.borderTopRightRadius = 15;
        qteContainer.style.borderBottomLeftRadius = 15; qteContainer.style.borderBottomRightRadius = 15;
        qteContainer.style.borderTopWidth = 4; qteContainer.style.borderBottomWidth = 4;
        qteContainer.style.borderLeftWidth = 4; qteContainer.style.borderRightWidth = 4;
        qteContainer.style.borderTopColor = new StyleColor(new Color(0.2f, 0.25f, 0.35f));
        qteContainer.style.borderBottomColor = new StyleColor(new Color(0.2f, 0.25f, 0.35f));
        qteContainer.style.borderLeftColor = new StyleColor(new Color(0.2f, 0.25f, 0.35f));
        qteContainer.style.borderRightColor = new StyleColor(new Color(0.2f, 0.25f, 0.35f));
        qteContainer.style.alignItems = Align.Center;
        qteContainer.style.justifyContent = Justify.Center;
        qteContainer.style.overflow = Overflow.Hidden;

        // Header
        instructionLabel = new Label("TAP IN THE GREEN ZONE!");
        instructionLabel.style.position = Position.Absolute;
        instructionLabel.style.top = 20;
        instructionLabel.style.fontSize = 28;
        instructionLabel.style.color = new StyleColor(Color.white);
        instructionLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        qteContainer.Add(instructionLabel);

        // Flip Counter
        flipCounterLabel = new Label("FLIPS: 0 / 3");
        flipCounterLabel.style.position = Position.Absolute;
        flipCounterLabel.style.top = 60;
        flipCounterLabel.style.fontSize = 24;
        flipCounterLabel.style.color = new StyleColor(new Color(0.18f, 0.8f, 0.44f));
        flipCounterLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        qteContainer.Add(flipCounterLabel);

        // Center Dial (Painter2D)
        dialElement = new VisualElement();
        dialElement.style.width = 240;
        dialElement.style.height = 240;
        dialElement.generateVisualContent += DrawDial;
        qteContainer.Add(dialElement);

        // Emoji Avatar inside the dial
        emojiLabel = new Label("🤸");
        emojiLabel.style.position = Position.Absolute;
        emojiLabel.style.fontSize = 90;
        emojiLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
        dialElement.Add(emojiLabel);

        // Invisible full-screen tap area
        var tapArea = new VisualElement();
        tapArea.style.position = Position.Absolute;
        tapArea.style.left = 0; tapArea.style.right = 0; tapArea.style.top = 0; tapArea.style.bottom = 0;
        tapArea.RegisterCallback<PointerDownEvent>(OnPointerDown);
        qteContainer.Add(tapArea);

        root.Add(qteContainer);
    }

    public void StartGame()
    {
        if (qteContainer != null) qteContainer.style.display = DisplayStyle.Flex;
        isPlaying = true;
        successfulFlips = 0;
        if (flipCounterLabel != null) flipCounterLabel.text = "FLIPS: 0 / 3";
        if (emojiLabel != null) emojiLabel.style.rotate = new StyleRotate(new Rotate(new Angle(0)));
        
        manager.StartCoroutine(QTERoutine());
    }

    public void AbortGame()
    {
        isPlaying = false;
        if (qteContainer != null) qteContainer.style.display = DisplayStyle.None;
    }

    private void DrawDial(MeshGenerationContext ctx)
    {
        var p = ctx.painter2D;
        float radius = 110f;
        Vector2 center = new Vector2(120f, 120f);

        // Background Track
        p.strokeColor = new Color(0.12f, 0.16f, 0.23f);
        p.lineWidth = 20f;
        p.lineCap = LineCap.Round;
        p.BeginPath();
        p.Arc(center, radius, 0, 360);
        p.Stroke();

        if (roundActive)
        {
            // Target Zone (Green)
            p.strokeColor = new Color(0.18f, 0.8f, 0.44f);
            p.lineWidth = 20f;
            p.BeginPath();
            p.Arc(center, radius, targetStart, targetEnd);
            p.Stroke();
        }

        // Needle
        p.strokeColor = Color.white;
        p.lineWidth = 6f;
        p.BeginPath();
        p.MoveTo(center);
        float needleRad = needleAngle * Mathf.Deg2Rad;
        Vector2 tip = center + new Vector2(Mathf.Cos(needleRad), Mathf.Sin(needleRad)) * (radius + 15f);
        p.LineTo(tip);
        p.Stroke();
        
        // Center Pin
        p.fillColor = Color.white;
        p.BeginPath();
        p.Arc(center, 8f, 0, 360);
        p.Fill();
    }

    private void OnPointerDown(PointerDownEvent ev)
    {
        if (!isPlaying || !manager.isPlaying || !roundActive) return;

        // Check if needle is inside target
        // targetStart and targetEnd are guaranteed to be ordered and normalized somewhat, but need safe angular check
        float normalizedNeedle = needleAngle % 360f;
        if (normalizedNeedle < 0) normalizedNeedle += 360f;
        
        bool hit = false;
        if (targetStart < targetEnd)
        {
            hit = normalizedNeedle >= targetStart && normalizedNeedle <= targetEnd;
        }
        else // wrapping around 360 (e.g. 300 to 50)
        {
            hit = normalizedNeedle >= targetStart || normalizedNeedle <= targetEnd;
        }

        roundActive = false; // Stop the needle immediately

        if (hit)
        {
            // SUCCESS!
            successfulFlips++;
            if (flipCounterLabel != null) flipCounterLabel.text = $"FLIPS: {successfulFlips} / 3";
            manager.StartCoroutine(SuccessFlipAnimation());
        }
        else
        {
            // FAIL!
            manager.StartCoroutine(FailAnimation());
        }
    }

    private IEnumerator QTERoutine()
    {
        // 3 Rounds
        for (int i = 0; i < 3; i++)
        {
            if (!isPlaying) yield break;

            // Setup new target zone
            float zoneWidth = 70f - (i * 15f); // Gets smaller each flip!
            targetStart = Random.Range(0f, 360f);
            targetEnd = (targetStart + zoneWidth) % 360f;
            
            needleSpeed = 250f + (i * 120f); // Gets faster each flip!
            
            // Give a tiny pause before needle starts
            roundActive = false;
            dialElement.MarkDirtyRepaint();
            yield return new WaitForSeconds(0.4f);
            
            roundActive = true;

            // Needle moves until player taps or needle completes a full 360 past the target?
            // Actually, let's let it spin indefinitely until they tap!
            // This is a mobile game, letting them wait for the right moment is good.
            // BUT maybe a time limit? Let's just give 3 seconds max per round.
            float roundTimer = 4f; 
            
            while (roundActive && isPlaying && manager.isPlaying)
            {
                needleAngle += needleSpeed * Time.deltaTime;
                if (needleAngle >= 360f) needleAngle -= 360f;
                dialElement.MarkDirtyRepaint();
                
                roundTimer -= Time.deltaTime;
                if (roundTimer <= 0)
                {
                    // Timeout!
                    roundActive = false;
                    manager.StartCoroutine(FailAnimation());
                    yield break;
                }
                
                yield return null;
            }
            
            // Wait for animation to finish before starting next round
            while (!roundActive && isPlaying && successfulFlips <= i) 
            {
                // This means they are watching the flip anim
                yield return null;
            }
            
            if (!isPlaying) yield break;
        }
        
        // Won all 3!
        manager.StartCoroutine(StampEffectRoutine("NAILED IT!", new Color(0.18f, 0.8f, 0.44f), true));
    }

    private IEnumerator SuccessFlipAnimation()
    {
        // Flash green
        qteContainer.style.backgroundColor = new StyleColor(new Color(0.1f, 0.3f, 0.15f));
        
        // Rapid emoji spin!
        float spinDuration = 0.5f;
        float elapsed = 0f;
        while (elapsed < spinDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / spinDuration;
            emojiLabel.style.rotate = new StyleRotate(new Rotate(new Angle(t * 360f, AngleUnit.Degree)));
            yield return null;
        }
        emojiLabel.style.rotate = new StyleRotate(new Rotate(new Angle(0)));
        qteContainer.style.backgroundColor = new StyleColor(new Color(0.06f, 0.09f, 0.16f));

        if (successfulFlips >= 3)
        {
            isPlaying = false; // stop routine
        }
        else
        {
            roundActive = true; // resume routine
        }
    }

    private IEnumerator FailAnimation()
    {
        isPlaying = false;
        
        // Flash red
        qteContainer.style.backgroundColor = new StyleColor(new Color(0.3f, 0.1f, 0.1f));
        emojiLabel.text = "😵";
        emojiLabel.style.rotate = new StyleRotate(new Rotate(new Angle(180f, AngleUnit.Degree))); // Faceplant
        
        yield return new WaitForSeconds(0.4f);
        
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
'''

with open(path, 'w') as f:
    f.write(code)

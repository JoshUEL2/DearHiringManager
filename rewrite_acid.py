import re

path = 'Assets/Scripts/2 Managers/MiniGames/AcidRainShieldMiniGame.cs'

code = '''using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;

public class AcidRainShieldMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement rootElement;

    private VisualElement acidRainContainer, acidRainArea, acidSignPivot, acidSignVisual, acidDropContainer, acidBurnBar, acidInputArea;
    private Label acidTimerText, playerAvatar;

    private float acidSignAngle = 0f;
    private float acidWindAngle = 0f;
    private float acidWindTarget = 0f;
    private float acidBurn = 0f;
    private float acidDropTimer = 0f;
    private float acidWindTimer = 0f;
    private float lightningTimer = 0f;
    private bool isPlaying = false;
    private List<AcidDrop> activeAcidDrops = new List<AcidDrop>();

    private class AcidDrop
    {
        public VisualElement element;
        public float radius;
        public float angle;
        public float speed;
    }

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        this.rootElement = root;

        acidRainContainer = root.Q<VisualElement>("AcidRainContainer");
        acidRainArea = root.Q<VisualElement>("AcidRainArea");
        acidSignPivot = root.Q<VisualElement>("AcidSignPivot");
        acidSignVisual = root.Q<VisualElement>("AcidSignVisual");
        acidDropContainer = root.Q<VisualElement>("AcidDropContainer");
        acidBurnBar = root.Q<VisualElement>("AcidBurnBar");
        acidTimerText = root.Q<Label>("AcidTimerText");
        acidInputArea = root.Q<VisualElement>("AcidInputArea");
        
        // Setup styled sign
        if (acidSignVisual != null)
        {
            acidSignVisual.style.backgroundColor = new StyleColor(new Color(0.95f, 0.77f, 0.06f));
            acidSignVisual.style.borderTopColor = new StyleColor(Color.white);
            acidSignVisual.style.borderBottomColor = new StyleColor(Color.white);
            acidSignVisual.style.borderLeftColor = new StyleColor(Color.white);
            acidSignVisual.style.borderRightColor = new StyleColor(Color.white);
            acidSignVisual.style.borderTopWidth = 2; acidSignVisual.style.borderBottomWidth = 2; acidSignVisual.style.borderLeftWidth = 2; acidSignVisual.style.borderRightWidth = 2;
            
            var text = new Label("SALE!");
            text.style.color = Color.black;
            text.style.unityFontStyleAndWeight = FontStyle.Bold;
            text.style.fontSize = 20;
            acidSignVisual.Add(text);
        }
        
        playerAvatar = acidSignPivot?.Q<Label>();

        acidInputArea?.RegisterCallback<PointerDownEvent>(OnPointerDown);
        acidInputArea?.RegisterCallback<PointerUpEvent>(OnPointerUp);
        acidInputArea?.RegisterCallback<PointerMoveEvent>(OnPointerMove);
    }

    public void StartGame()
    {
        isPlaying = true;
        if (acidRainContainer != null) acidRainContainer.style.display = DisplayStyle.Flex;
        acidBurn = 0f;
        acidSignAngle = -90f;
        if (acidSignPivot != null) acidSignPivot.style.rotate = new StyleRotate(new Rotate(new Angle(0f, AngleUnit.Degree)));
        if (acidBurnBar != null) acidBurnBar.style.width = new Length(0, LengthUnit.Percent);
        if (acidDropContainer != null) acidDropContainer.Clear();
        activeAcidDrops.Clear();
        lightningTimer = Random.Range(2f, 5f);
        
        manager.StartCoroutine(AcidRainRoutine());
    }

    public void AbortGame()
    {
        isPlaying = false;
        if (acidRainContainer != null) acidRainContainer.style.display = DisplayStyle.None;
        
        acidInputArea?.UnregisterCallback<PointerDownEvent>(OnPointerDown);
        acidInputArea?.UnregisterCallback<PointerUpEvent>(OnPointerUp);
        acidInputArea?.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
    }

    private void OnPointerDown(PointerDownEvent ev)
    {
        acidInputArea?.CapturePointer(ev.pointerId);
    }

    private void OnPointerUp(PointerUpEvent ev)
    {
        acidInputArea?.ReleasePointer(ev.pointerId);
    }

    private void OnPointerMove(PointerMoveEvent ev)
    {
        if (!manager.isPlaying || !isPlaying) return;
        if (acidInputArea != null && acidInputArea.HasPointerCapture(ev.pointerId))
        {
            acidSignAngle += ev.deltaPosition.x * 0.4f;
            acidSignAngle = Mathf.Clamp(acidSignAngle, -75f, 75f);
            if (acidSignPivot != null) acidSignPivot.style.rotate = new StyleRotate(new Rotate(new Angle(acidSignAngle)));
        }
    }

    private IEnumerator AcidRainRoutine()
    {
        float duration = 15f;
        float elapsed = 0f;

        while (elapsed < duration && acidBurn < 100f && manager.isPlaying && isPlaying)
        {
            elapsed += Time.deltaTime;
            float timeRemaining = duration - elapsed;
            if (timeRemaining < 0f) timeRemaining = 0f;
            if (acidTimerText != null) acidTimerText.text = timeRemaining.ToString("0.0") + "s";

            // Ambient Jiggle on player avatar
            if (playerAvatar != null)
            {
                playerAvatar.style.translate = new Translate(Random.Range(-2f, 2f), Random.Range(-2f, 2f), 0);
            }

            // Lightning Effects
            lightningTimer -= Time.deltaTime;
            if (lightningTimer <= 0f)
            {
                manager.StartCoroutine(LightningFlashRoutine());
                lightningTimer = Random.Range(1f, 4f);
            }

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
            if (acidDropTimer >= 0.20f)
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
                    if (diff <= 35f) // Blocked!
                    {
                        manager.StartCoroutine(SignImpactRoutine());
                        drop.element.RemoveFromHierarchy();
                        activeAcidDrops.RemoveAt(i);
                        continue;
                    }
                }

                if (drop.radius <= 10f)
                {
                    drop.element.RemoveFromHierarchy();
                    activeAcidDrops.RemoveAt(i);
                    
                    manager.StartCoroutine(DamageFlashRoutine());
                    
                    acidBurn += 8f;
                    if (acidBurn > 100f) acidBurn = 100f;
                    
                    if (acidBurnBar != null)
                    {
                        acidBurnBar.style.width = Length.Percent(acidBurn);
                        if (acidBurn > 60f) acidBurnBar.style.backgroundColor = new StyleColor(new Color(0.9f, 0.3f, 0.23f)); // Red
                        else if (acidBurn > 30f) acidBurnBar.style.backgroundColor = new StyleColor(new Color(0.94f, 0.76f, 0.05f)); // Yellow
                    }
                }
            }

            yield return null;
        }

        foreach (var drop in activeAcidDrops)
        {
            drop.element.RemoveFromHierarchy();
        }
        activeAcidDrops.Clear();
        if (playerAvatar != null) playerAvatar.style.translate = new Translate(0, 0, 0);

        if (manager.isPlaying && isPlaying)
        {
            isPlaying = false;
            if (acidBurn < 100f)
            {
                manager.StartCoroutine(StampEffectRoutine("SURVIVED!", new Color(0.18f, 0.8f, 0.44f), true));
            }
            else
            {
                string loseText = (config.canFail) ? "MELTED!" : "BURNT!";
                manager.StartCoroutine(StampEffectRoutine(loseText, new Color(0.93f, 0.26f, 0.22f), false));
            }
        }
    }

    private void SpawnAcidDrop()
    {
        var dropEl = new VisualElement();
        dropEl.style.position = Position.Absolute;
        dropEl.style.width = 6;
        dropEl.style.height = 40; // Stretched to look like rain streaks
        dropEl.style.backgroundColor = new StyleColor(new Color(0.18f, 0.8f, 0.44f, 0.8f));
        dropEl.style.borderTopLeftRadius = 3;
        dropEl.style.borderTopRightRadius = 3;
        dropEl.style.borderBottomLeftRadius = 3;
        dropEl.style.borderBottomRightRadius = 3;
        dropEl.style.bottom = 0; 

        float spawnAngle = acidWindAngle + Random.Range(-15f, 15f);
        
        // Rotate streak to point towards center
        dropEl.style.rotate = new StyleRotate(new Rotate(new Angle(spawnAngle, AngleUnit.Degree)));
        
        if (acidDropContainer != null) acidDropContainer.Add(dropEl);

        activeAcidDrops.Add(new AcidDrop {
            element = dropEl,
            radius = 450f,
            angle = spawnAngle,
            speed = Random.Range(350f, 500f) // Super fast rain
        });
    }

    private IEnumerator LightningFlashRoutine()
    {
        if (acidRainArea != null)
        {
            acidRainArea.style.backgroundColor = new StyleColor(Color.white);
            yield return new WaitForSeconds(0.05f);
            acidRainArea.style.backgroundColor = new StyleColor(new Color(0.06f, 0.09f, 0.16f)); // Dark Slate
            yield return new WaitForSeconds(0.05f);
            acidRainArea.style.backgroundColor = new StyleColor(new Color(0.8f, 0.9f, 1f));
            yield return new WaitForSeconds(0.1f);
            acidRainArea.style.backgroundColor = new StyleColor(new Color(0.06f, 0.09f, 0.16f));
        }
    }

    private IEnumerator DamageFlashRoutine()
    {
        if (acidRainArea != null)
        {
            acidRainArea.style.backgroundColor = new StyleColor(new Color(0.8f, 0.1f, 0.1f)); // Red Damage flash
            acidRainArea.style.translate = new Translate(Random.Range(-15f, 15f), Random.Range(-15f, 15f), 0); // Screen shake
            yield return new WaitForSeconds(0.05f);
            acidRainArea.style.translate = new Translate(Random.Range(-15f, 15f), Random.Range(-15f, 15f), 0);
            yield return new WaitForSeconds(0.05f);
            acidRainArea.style.translate = new Translate(0, 0, 0);
            acidRainArea.style.backgroundColor = new StyleColor(new Color(0.06f, 0.09f, 0.16f));
        }
    }

    private IEnumerator SignImpactRoutine()
    {
        if (acidSignVisual != null)
        {
            // Flash white
            acidSignVisual.style.backgroundColor = new StyleColor(Color.white);
            // Recoil down
            acidSignVisual.style.bottom = 140f; 
            
            yield return new WaitForSeconds(0.05f);
            
            acidSignVisual.style.backgroundColor = new StyleColor(new Color(0.95f, 0.77f, 0.06f));
            acidSignVisual.style.bottom = 150f;
        }
    }

    private IEnumerator StampEffectRoutine(string text, Color color, bool isWin)
    {
        Label stampText = new Label(text);
        stampText.style.position = Position.Absolute;
        stampText.style.color = new StyleColor(color);
        stampText.style.fontSize = 110;
        stampText.style.letterSpacing = 10;
        stampText.style.rotate = new StyleRotate(new Rotate(new Angle(-12f, AngleUnit.Degree)));
        stampText.style.textShadow = new StyleTextShadow(new TextShadow { color = new Color(0,0,0,0.8f), offset = new Vector2(12f, 15f), blurRadius = 15f });
        stampText.style.unityFontStyleAndWeight = FontStyle.Bold;
        stampText.style.unityTextAlign = TextAnchor.MiddleCenter;
        
        VisualElement backgroundFade = new VisualElement();
        backgroundFade.style.position = Position.Absolute;
        backgroundFade.style.width = new Length(100, LengthUnit.Percent);
        backgroundFade.style.height = new Length(100, LengthUnit.Percent);
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
            else manager.MarkWinReady(); // E.g. skip tutorial logic
        }
    }
}
'''

with open(path, 'w') as f:
    f.write(code)

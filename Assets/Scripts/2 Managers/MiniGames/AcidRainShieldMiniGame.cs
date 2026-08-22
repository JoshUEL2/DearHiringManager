using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;

public class AcidRainShieldMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement rootElement;

    private VisualElement acidRainContainer, acidSignPivot, acidDropContainer, acidBurnBar, acidInputArea;
    private Label acidTimerText;

    private float acidSignAngle = 0f;
    private float acidWindAngle = 0f;
    private float acidWindTarget = 0f;
    private float acidBurn = 0f;
    private float acidDropTimer = 0f;
    private float acidWindTimer = 0f;
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
        acidSignPivot = root.Q<VisualElement>("AcidSignPivot");
        acidDropContainer = root.Q<VisualElement>("AcidDropContainer");
        acidBurnBar = root.Q<VisualElement>("AcidBurnBar");
        acidTimerText = root.Q<Label>("AcidTimerText");
        acidInputArea = root.Q<VisualElement>("AcidInputArea");
        
        acidInputArea?.RegisterCallback<PointerDownEvent>(OnPointerDown);
        acidInputArea?.RegisterCallback<PointerUpEvent>(OnPointerUp);
        acidInputArea?.RegisterCallback<PointerMoveEvent>(OnPointerMove);
    }

    public void StartGame()
    {
        if (acidRainContainer != null) acidRainContainer.style.display = DisplayStyle.Flex;
        acidBurn = 0f;
        acidSignAngle = -90f;
        if (acidSignPivot != null) acidSignPivot.style.rotate = new StyleRotate(new Rotate(new Angle(0f, AngleUnit.Degree)));
        if (acidBurnBar != null) acidBurnBar.style.width = new Length(0, LengthUnit.Percent);
        if (acidDropContainer != null) acidDropContainer.Clear();
        activeAcidDrops.Clear();
        
        manager.StartCoroutine(AcidRainRoutine());
    }

    public void AbortGame()
    {
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
        if (!manager.isPlaying) return;
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

        while (elapsed < duration && acidBurn < 100f && manager.isPlaying)
        {
            elapsed += Time.deltaTime;
            float timeRemaining = duration - elapsed;
            if (timeRemaining < 0f) timeRemaining = 0f;
            if (acidTimerText != null) acidTimerText.text = timeRemaining.ToString("0.0") + "s";

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

        if (manager.isPlaying)
        {
            if (acidBurn < 100f)
            {
                manager.MarkWinReady();
            }
            else
            {
                if (config.canFail) {
                    manager.AbortGame(); 
                } else {
                    manager.MarkWinReady(); 
                }
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
        
        if (acidDropContainer != null) acidDropContainer.Add(dropEl);

        float spawnAngle = acidWindAngle + Random.Range(-15f, 15f);
        
        activeAcidDrops.Add(new AcidDrop {
            element = dropEl,
            radius = 450f,
            angle = spawnAngle,
            speed = Random.Range(200f, 300f)
        });
    }
}

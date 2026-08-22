using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;

public class GalacticCouncilMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement rootElement;
    
    private VisualElement container;
    private Label timerText;
    private VisualElement progressFill;
    private VisualElement playArea;
    private VisualElement projectileContainer;
    private VisualElement podium;

    private float timer;
    private float podiumX = 0f;
    private int hits = 0;
    private float progress = 0f;

    private class GcProjectile {
        public VisualElement ve;
        public Vector2 pos;
        public Vector2 vel;
    }
    private List<GcProjectile> projs = new List<GcProjectile>();

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        this.rootElement = root;

        container = root.Q<VisualElement>("GalacticCouncilContainer");
        timerText = root.Q<Label>("GcTimerText");
        progressFill = root.Q<VisualElement>("GcProgressFill");
        playArea = root.Q<VisualElement>("GcPlayArea");
        projectileContainer = root.Q<VisualElement>("GcProjectileContainer");
        podium = root.Q<VisualElement>("GcPodium");

        if (playArea != null) {
            playArea.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        }
        rootElement.RegisterCallback<KeyDownEvent>(OnKeyDown);
    }

    public void StartGame()
    {
        if (container != null) container.style.display = DisplayStyle.Flex;
        
        timer = config.timeLimit;
        podiumX = 0f;
        hits = 0;
        progress = 0f;

        if (podium != null) podium.style.translate = new StyleTranslate(new Translate(0, 0, 0));
        if (progressFill != null) progressFill.style.width = new StyleLength(new Length(0, LengthUnit.Percent));
        if (projectileContainer != null) projectileContainer.Clear();
        projs.Clear();

        manager.StartCoroutine(GameRoutine());
    }

    public void AbortGame()
    {
        if (container != null) container.style.display = DisplayStyle.None;
        if (playArea != null) {
            playArea.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
        }
        rootElement.UnregisterCallback<KeyDownEvent>(OnKeyDown);
    }

    private void OnPointerMove(PointerMoveEvent ev)
    {
        if (!manager.isPlaying) return;
        if (ev.pressedButtons != 0) {
            podiumX += ev.deltaPosition.x;
            podiumX = Mathf.Clamp(podiumX, -150f, 150f);
            if (podium != null) podium.style.translate = new StyleTranslate(new Translate(podiumX, 0, 0));
        }
    }

    private void OnKeyDown(KeyDownEvent ev)
    {
        if (!manager.isPlaying) return;
        
        float dx = 0;
        if (ev.keyCode == KeyCode.LeftArrow || ev.keyCode == KeyCode.A) dx = -20f;
        else if (ev.keyCode == KeyCode.RightArrow || ev.keyCode == KeyCode.D) dx = 20f;
        
        if (dx != 0)
        {
            ev.StopPropagation();
            podiumX += dx;
            podiumX = Mathf.Clamp(podiumX, -150f, 150f);
            if (podium != null) podium.style.translate = new StyleTranslate(new Translate(podiumX, 0, 0));
        }
    }

    private IEnumerator GameRoutine()
    {
        float projTimer = 0f;
        
        while (manager.isPlaying)
        {
            timer -= Time.deltaTime;
            progress += (100f / config.timeLimit) * Time.deltaTime;
            
            if (timerText != null) timerText.text = $"{timer:F1}s";
            
            if (progressFill != null) progressFill.style.width = new StyleLength(new Length(Mathf.Clamp(progress, 0, 100), LengthUnit.Percent));

            projTimer -= Time.deltaTime;
            if (projTimer <= 0f) {
                projTimer = Random.Range(0.4f, 1.0f);
                // Spawn projectile from one of the aliens
                int alien = Random.Range(0, 3);
                float startX = (alien == 0) ? 50f : (alien == 1 ? 150f : 250f);
                var ve = new VisualElement();
                ve.style.position = Position.Absolute;
                ve.style.width = 15; ve.style.height = 15; 
                ve.style.borderTopLeftRadius = 8; ve.style.borderTopRightRadius = 8;
                ve.style.borderBottomLeftRadius = 8; ve.style.borderBottomRightRadius = 8;
                ve.style.backgroundColor = Color.red;
                if (projectileContainer != null) projectileContainer.Add(ve);
                
                // Target player
                Vector2 startPos = new Vector2(startX, 60f);
                Vector2 playerPos = new Vector2(150f + podiumX, 290f);
                Vector2 dir = (playerPos - startPos).normalized;
                
                projs.Add(new GcProjectile { ve = ve, pos = startPos, vel = dir * Random.Range(100f, 250f) });
            }

            // Move projectiles & check collisions
            for (int i=projs.Count-1; i>=0; i--) {
                var p = projs[i];
                p.pos += p.vel * Time.deltaTime;
                p.ve.style.left = p.pos.x - 7.5f;
                p.ve.style.top = p.pos.y - 7.5f;
                
                // Collision with podium
                float playerX = 150f + podiumX;
                if (p.pos.y > 270f && p.pos.y < 310f && Mathf.Abs(p.pos.x - playerX) < 25f) {
                    hits++;
                    p.ve.RemoveFromHierarchy();
                    projs.RemoveAt(i);
                    // Shake podium
                    if (podium != null) podium.style.translate = new StyleTranslate(new Translate(podiumX + (Random.value > 0.5f ? 10 : -10), 0, 0));
                    continue;
                }
                
                if (p.pos.y > 320f || p.pos.x < -20f || p.pos.x > 320f) {
                    p.ve.RemoveFromHierarchy();
                    projs.RemoveAt(i);
                }
            }

            if (hits >= 3) {
                manager.AbortGame(); // Fail
                yield break;
            }

            if (timer <= 0f) {
                manager.MarkWinReady();
                yield break;
            }
            yield return null;
        }
    }
}

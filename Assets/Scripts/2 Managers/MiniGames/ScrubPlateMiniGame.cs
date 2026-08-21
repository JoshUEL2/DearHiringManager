using UnityEngine;
using UnityEngine.UIElements;

public class ScrubPlateMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement container;
    private VisualElement scrubPlateArea;
    private VisualElement scrubDirtOverlay;
    private VisualElement scrubSponge;
    private ProgressBar scrubProgressBar;

    private float scrubProgress = 0f;
    private Vector2 lastScrubPos = Vector2.zero;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        container = root.Q<VisualElement>("ScrubPlateContainer");
        scrubProgressBar = root.Q<ProgressBar>("ScrubProgressBar");
        scrubPlateArea = root.Q<VisualElement>("ScrubPlateArea");
        scrubDirtOverlay = root.Q<VisualElement>("ScrubDirtOverlay");
        scrubSponge = root.Q<VisualElement>("ScrubSponge");

        scrubPlateArea.RegisterCallback<PointerDownEvent>(ev => {
            if (!manager.isPlaying) return;
            lastScrubPos = (Vector2)ev.localPosition;
            scrubPlateArea.CapturePointer(ev.pointerId);
            ev.StopPropagation();
        });

        scrubPlateArea.RegisterCallback<PointerUpEvent>(ev => {
            if (scrubPlateArea.HasPointerCapture(ev.pointerId))
            {
                scrubPlateArea.ReleasePointer(ev.pointerId);
            }
        });

        scrubPlateArea.RegisterCallback<PointerMoveEvent>(ev => {
            if (!manager.isPlaying) return;
            
            // Constrain sponge visually
            Vector2 localPos = (Vector2)ev.localPosition;
            float spongeW = 80f, spongeH = 80f;
            localPos.x = Mathf.Clamp(localPos.x, 0, scrubPlateArea.layout.width - spongeW);
            localPos.y = Mathf.Clamp(localPos.y, 0, scrubPlateArea.layout.height - spongeH);
            
            scrubSponge.style.left = localPos.x;
            scrubSponge.style.top = localPos.y;

            if (scrubPlateArea.HasPointerCapture(ev.pointerId)) // Only scrub if dragging
            {
                float delta = ((Vector2)ev.localPosition - lastScrubPos).magnitude;
                if (delta > 2f)
                {
                    scrubProgress += delta * 0.015f; 
                    scrubProgressBar.value = scrubProgress;
                    
                    float opacity = 1f - (scrubProgress / config.targetScore);
                    scrubDirtOverlay.style.opacity = Mathf.Clamp01(opacity);
                    
                    if (scrubProgress >= config.targetScore)
                    {
                        scrubProgressBar.value = config.targetScore;
                        scrubDirtOverlay.style.opacity = 0f;
                        scrubPlateArea.ReleasePointer(ev.pointerId);
                        manager.MarkWinReady();
                    }
                }
                lastScrubPos = (Vector2)ev.localPosition;
            }
        });
    }

    public void StartGame()
    {
        container.style.display = DisplayStyle.Flex;
        SetupScrubPlate();
    }

    public void AbortGame()
    {
        container.style.display = DisplayStyle.None;
    }

    private void SetupScrubPlate()
    {
        scrubProgress = 0f;
        scrubProgressBar.value = 0f;
        scrubProgressBar.highValue = config.targetScore;
        scrubDirtOverlay.style.opacity = 1f;
        scrubSponge.style.left = 110f;
        scrubSponge.style.top = 120f;
        lastScrubPos = Vector2.zero;

        var plateIcon = scrubPlateArea.Q<Label>("ScrubTargetIcon");
        if (plateIcon != null && manager.currentGame != null)
        {
            string gameName = manager.currentGame.name;
            if (gameName == "Scour microwave grease") plateIcon.text = "📺";
            else if (gameName == "Deep clean grease traps") plateIcon.text = "🛢️";
            else if (gameName == "Wipe smog off the neon arrow") plateIcon.text = "➡️";
            else plateIcon.text = "🍽️";
        }
        
        var spongeLabel = scrubPlateArea.Q<Label>("ScrubSponge");
        if (spongeLabel != null && manager.currentGame != null)
        {
            if (manager.currentGame.name == "Wipe smog off the neon arrow") spongeLabel.text = "🧻";
            else spongeLabel.text = "🧽";
        }
    }
}

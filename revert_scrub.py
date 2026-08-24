import re
import os

# 1. Update GameType enum
path_types = 'Assets/Scripts/2 Managers/MiniGameTypes.cs'
with open(path_types, 'r') as f:
    c_types = f.read()

c_types = c_types.replace('IncineratorToss }', 'IncineratorToss, PhysicalScrub }')
with open(path_types, 'w') as f:
    f.write(c_types)

# 2. Update Factory
path_factory = 'Assets/Scripts/2 Managers/MiniGames/MiniGameFactory.cs'
with open(path_factory, 'r') as f:
    c_factory = f.read()

c_factory = c_factory.replace('case GameType.ScrubPlate: return new ScrubPlateMiniGame();', 'case GameType.ScrubPlate: return new ScrubPlateMiniGame();\n            case GameType.PhysicalScrub: return new PhysicalScrubMiniGame();')
with open(path_factory, 'w') as f:
    f.write(c_factory)

# 3. Update MiniGameManager
path_manager = 'Assets/Scripts/2 Managers/MiniGameManager.cs'
with open(path_manager, 'r') as f:
    c_manager = f.read()

c_manager = c_manager.replace('type = GameType.ScrubPlate, targetScore = 25, timeLimit = 10f, instructions = "Scrape the carbonized grill!"', 'type = GameType.PhysicalScrub, targetScore = 25, timeLimit = 10f, instructions = "Scrape the carbonized grill!"')
c_manager = c_manager.replace('type = GameType.ScrubPlate, targetScore = 20, timeLimit = 5f, instructions = "Power-wash the garbage sludge!"', 'type = GameType.PhysicalScrub, targetScore = 20, timeLimit = 5f, instructions = "Power-wash the garbage sludge!"')

with open(path_manager, 'w') as f:
    f.write(c_manager)

# 4. Rename ScrubPlateMiniGame to PhysicalScrubMiniGame and change class name
os.rename('Assets/Scripts/2 Managers/MiniGames/ScrubPlateMiniGame.cs', 'Assets/Scripts/2 Managers/MiniGames/PhysicalScrubMiniGame.cs')

path_phys = 'Assets/Scripts/2 Managers/MiniGames/PhysicalScrubMiniGame.cs'
with open(path_phys, 'r') as f:
    c_phys = f.read()

c_phys = c_phys.replace('public class ScrubPlateMiniGame : IMiniGame', 'public class PhysicalScrubMiniGame : IMiniGame')
with open(path_phys, 'w') as f:
    f.write(c_phys)


# 5. Restore original ScrubPlateMiniGame.cs
old_code = """using UnityEngine;
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
        if (scrubProgressBar != null) scrubProgressBar.style.display = DisplayStyle.None;
        
        container.style.flexGrow = 1;
        container.style.minHeight = 450;
        container.style.justifyContent = Justify.Center;
        
        scrubPlateArea = root.Q<VisualElement>("ScrubPlateArea");
        scrubPlateArea.style.scale = new StyleScale(new Vector2(1.15f, 1.15f));
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
"""
with open('Assets/Scripts/2 Managers/MiniGames/ScrubPlateMiniGame.cs', 'w') as f:
    f.write(old_code)


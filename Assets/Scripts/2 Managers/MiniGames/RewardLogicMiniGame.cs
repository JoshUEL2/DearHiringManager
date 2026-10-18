using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class RewardLogicMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    
    private VisualElement container;
    private Label rlTimerText;
    private VisualElement rlCore;
    private VisualElement[] rlJunctions = new VisualElement[3];
    private Label[] rlArrows = new Label[3];
    private bool[] rlState = new bool[3]; // false = left, true = right

    private float rlCoreY = 0f;
    private float timer = 0f;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;

        container = root.Q<VisualElement>("RewardLogicContainer");
        rlTimerText = root.Q<Label>("RlTimerText");
        rlCore = root.Q<VisualElement>("RlCore");

        for (int i = 0; i < 3; i++)
        {
            rlJunctions[i] = root.Q<VisualElement>($"RlJunc_{i}");
            rlArrows[i] = root.Q<Label>($"RlArr_{i}");
        }

        if (rlJunctions[0] != null) {
            rlJunctions[0].UnregisterCallback<PointerDownEvent>(OnJunction0);
            rlJunctions[0].RegisterCallback<PointerDownEvent>(OnJunction0);
        }
        if (rlJunctions[1] != null) {
            rlJunctions[1].UnregisterCallback<PointerDownEvent>(OnJunction1);
            rlJunctions[1].RegisterCallback<PointerDownEvent>(OnJunction1);
        }
        if (rlJunctions[2] != null) {
            rlJunctions[2].UnregisterCallback<PointerDownEvent>(OnJunction2);
            rlJunctions[2].RegisterCallback<PointerDownEvent>(OnJunction2);
        }
    }

    private void OnJunction0(PointerDownEvent ev) { ToggleJunction(0); }
    private void OnJunction1(PointerDownEvent ev) { ToggleJunction(1); }
    private void OnJunction2(PointerDownEvent ev) { ToggleJunction(2); }

    private void ToggleJunction(int index)
    {
        if (!manager.isPlaying) return;
        rlState[index] = !rlState[index];
        AudioManager.Instance?.PlayCherrySwitch();
        if (rlArrows[index] != null) rlArrows[index].text = rlState[index] ? "→" : "←";
    }

    public void StartGame()
    {
        container.style.display = DisplayStyle.Flex;
        timer = config.timeLimit;
        rlCoreY = 0f;

        for (int i = 0; i < 3; i++)
        {
            rlState[i] = UnityEngine.Random.value > 0.5f; // Random initial directions
            if (rlArrows[i] != null) rlArrows[i].text = rlState[i] ? "→" : "←";
        }
        
        if (rlCore != null) rlCore.style.translate = new StyleTranslate(new Translate(0, 0, 0));

        manager.StartCoroutine(GameRoutine());
    }

    public void AbortGame()
    {
        if (container != null)
            container.style.display = DisplayStyle.None;

        if (rlJunctions[0] != null) rlJunctions[0].UnregisterCallback<PointerDownEvent>(OnJunction0);
        if (rlJunctions[1] != null) rlJunctions[1].UnregisterCallback<PointerDownEvent>(OnJunction1);
        if (rlJunctions[2] != null) rlJunctions[2].UnregisterCallback<PointerDownEvent>(OnJunction2);
    }

    private IEnumerator GameRoutine()
    {
        float speed = 25f; // pixels per second
        float currentX = 0f;

        while (manager.isPlaying)
        {
            timer -= Time.deltaTime;
            if (rlTimerText != null) rlTimerText.text = $"{timer:F1}s";

            rlCoreY += speed * Time.deltaTime;

            // Core routing logic based on junctions
            // Junction 0 is at Y=80
            // Junction 1 is at Y=180, X=-75
            // Junction 2 is at Y=180, X=+75
            // Endpoints are at Y=300
            
            // From top to Level 1
            if (rlCoreY < 80) {
                currentX = 0f;
            }
            // Passing Level 1 (Junction 0)
            else if (rlCoreY >= 80 && rlCoreY < 180) {
                // Smooth transition left or right
                float t = (rlCoreY - 80) / 100f; // 0 to 1
                float targetX = rlState[0] ? 75f : -75f;
                currentX = Mathf.Lerp(0f, targetX, t);
            }
            // Passing Level 2 (Junctions 1 or 2)
            else if (rlCoreY >= 180 && rlCoreY < 280) {
                float t = (rlCoreY - 180) / 100f; // 0 to 1
                bool rightSide = (rlState[0] == true); // came from right
                float targetX = 0f;
                if (!rightSide) {
                    targetX = rlState[1] ? 0f : -150f;
                } else {
                    targetX = rlState[2] ? 150f : 0f;
                }
                currentX = Mathf.Lerp(rightSide ? 75f : -75f, targetX, t);
            }
            // Result at bottom
            else if (rlCoreY >= 280) {
                // If X > 0 it hits PRESERVE (Green), else DESTROY (Red)
                if (currentX > 0f) {
                    AudioManager.Instance?.PlayVictoryFanfare();
                    manager.MarkWinReady();
                    yield break;
                } else {
                    AudioManager.Instance?.PlayMinigameFail();
                    manager.AbortGame(); // Fail
                    yield break;
                }
            }

            if (rlCore != null) {
                rlCore.style.translate = new StyleTranslate(new Translate(currentX, rlCoreY, 0));
            }

            if (timer <= 0f) {
                AudioManager.Instance?.PlayMinigameFail();
                manager.AbortGame();
                yield break;
            }
            yield return null;
        }
    }
}

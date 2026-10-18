using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class ContainmentProtocolMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement rootElement;

    private VisualElement containmentProtocolContainer;
    private Label cpTimerText;
    private VisualElement[] cpSwitches = new VisualElement[4];
    private bool[] cpSwitchState = new bool[4];
    private VisualElement cpGlassCover;
    private bool cpGlassLifted = false;
    private VisualElement cpRedButton;
    private Label cpButtonText;
    private int cpMashCount = 0;
    
    private float timer;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        this.rootElement = root;

        containmentProtocolContainer = root.Q<VisualElement>("ContainmentProtocolContainer");
        cpTimerText = root.Q<Label>("CpTimerText");
        
        for (int i = 0; i < 4; i++) {
            cpSwitches[i] = root.Q<VisualElement>($"CpSwitch_{i}");
        }
        
        cpGlassCover = root.Q<VisualElement>("CpGlassCover");
        cpRedButton = root.Q<VisualElement>("CpRedButton");
        cpButtonText = root.Q<Label>("CpButtonText");

        for (int i = 0; i < 4; i++) {
            int index = i;
            if (cpSwitches[i] != null) {
                cpSwitches[i].RegisterCallback<PointerMoveEvent>(ev => {
                    if (ev.pressedButtons != 0 && ev.deltaPosition.y > 5f && !cpSwitchState[index]) {
                        cpSwitchState[index] = true;
                        AudioManager.Instance?.PlayCherrySwitch();
                    }
                });
                cpSwitches[i].RegisterCallback<PointerDownEvent>(ev => { 
                    if (!cpSwitchState[index]) {
                        cpSwitchState[index] = true;
                        AudioManager.Instance?.PlayCherrySwitch();
                    }
                }); // fallback tap
            }
        }

        if (cpGlassCover != null) {
            cpGlassCover.RegisterCallback<PointerMoveEvent>(ev => {
                if (ev.pressedButtons != 0 && ev.deltaPosition.y < -5f && !cpGlassLifted) {
                    cpGlassLifted = true;
                    AudioManager.Instance?.PlayWhoosh();
                }
            });
            cpGlassCover.RegisterCallback<PointerDownEvent>(ev => { 
                if (!cpGlassLifted) {
                    cpGlassLifted = true;
                    AudioManager.Instance?.PlayWhoosh();
                }
            });
        }

        if (cpRedButton != null) {
            cpRedButton.RegisterCallback<PointerDownEvent>(ev => {
                if (cpGlassLifted) {
                    cpMashCount++;
                    AudioManager.Instance?.PlayClick();
                }
            });
        }
    }

    public void StartGame()
    {
        if (containmentProtocolContainer != null) 
            containmentProtocolContainer.style.display = DisplayStyle.Flex;

        timer = config.timeLimit;
        cpGlassLifted = false;
        cpMashCount = 0;
        
        for (int i = 0; i < 4; i++) {
            cpSwitchState[i] = false;
            if (cpSwitches[i] != null) 
                cpSwitches[i].style.translate = new StyleTranslate(new Translate(0, 0, 0));
        }
        
        if (cpGlassCover != null) {
            cpGlassCover.style.translate = new StyleTranslate(new Translate(0, 0, 0));
            cpGlassCover.style.backgroundColor = new StyleColor(Color.clear); // Reset glow
        }
            
        if (cpButtonText != null) 
            cpButtonText.text = "LOCKED";
            
        manager.StartCoroutine(ContainmentProtocolRoutine());
    }

    public void AbortGame()
    {
        if (containmentProtocolContainer != null) 
            containmentProtocolContainer.style.display = DisplayStyle.None;
    }

    private IEnumerator ContainmentProtocolRoutine()
    {
        manager.isPlaying = true;
        
        while (manager.isPlaying)
        {
            timer -= Time.deltaTime;
            if (cpTimerText != null) cpTimerText.text = $"{timer:F1}s";

            bool allSwitchesDown = true;
            for (int i = 0; i < 4; i++) {
                if (cpSwitches[i] != null) {
                    cpSwitches[i].style.translate = new StyleTranslate(new Translate(0, cpSwitchState[i] ? 70 : 0, 0));
                }
                if (!cpSwitchState[i]) allSwitchesDown = false;
            }

            if (cpGlassCover != null) {
                if (allSwitchesDown) {
                    cpGlassCover.style.backgroundColor = new StyleColor(new Color(0.2f, 1f, 0.2f, 0.4f)); // Glow green
                    if (cpGlassLifted) cpGlassCover.style.translate = new StyleTranslate(new Translate(0, -200, 0)); // lifted away
                } else {
                    cpGlassLifted = false;
                }
            }

            if (allSwitchesDown && cpGlassLifted) {
                if (cpButtonText != null) cpButtonText.text = $"MASH ({cpMashCount}/10)";
            } else {
                if (cpButtonText != null) cpButtonText.text = "LOCKED";
            }

            if (cpMashCount >= 10) {
                if (cpButtonText != null) cpButtonText.text = "SECURED";
                AudioManager.Instance?.PlayVictoryFanfare();
                manager.MarkWinReady();
                yield break;
            }

            if (timer <= 0f) {
                AudioManager.Instance?.PlayMinigameFail();
                manager.isPlaying = false;
                yield break;
            }
            yield return null;
        }
    }
}

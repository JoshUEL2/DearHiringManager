using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class FranticBackspaceMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement container;

    private Label fbPlayerText;
    private Label fbTimerText;
    private Button fbBackspaceBtn;

    private string fbOriginalAngryMessage = "This is the worst idea I have ever heard! Are you completely out of your mind?! If we ship this, the entire company will tank and it will be 100% your fault! I refuse to merge this garbage!!!";
    private string fbCurrentMessage;
    private float timer;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        
        container = root.Q<VisualElement>("FranticBackspaceContainer");
        fbPlayerText = container.Q<Label>("FbPlayerText");
        fbTimerText = container.Q<Label>("FbTimerText");
        fbBackspaceBtn = container.Q<Button>("FbBackspaceBtn");

        if (fbBackspaceBtn != null) 
        {
            fbBackspaceBtn.clicked += () => {
                if (!manager.isPlaying) return;
                
                if (fbCurrentMessage.Length > 0) {
                    int removeCount = Mathf.Min(5, fbCurrentMessage.Length);
                    fbCurrentMessage = fbCurrentMessage.Substring(0, fbCurrentMessage.Length - removeCount);
                    if (fbPlayerText != null) fbPlayerText.text = fbCurrentMessage + "|";
                    AudioManager.Instance?.PlayFranticKey();
                    MiniGameFeedback.Punch(fbBackspaceBtn, 1.08f);
                }
            };
        }
    }

    public void StartGame()
    {
        timer = config.timeLimit;
        fbCurrentMessage = fbOriginalAngryMessage;
        
        if (fbPlayerText != null) fbPlayerText.text = fbCurrentMessage + "|";
        if (fbTimerText != null) 
        {
            fbTimerText.text = $"AUTO-SEND: {timer:F1}s";
            fbTimerText.style.color = new StyleColor(Color.red);
        }
        
        container.style.display = DisplayStyle.Flex;
        manager.StartCoroutine(FranticBackspaceRoutine());
    }

    public void AbortGame()
    {
        container.style.display = DisplayStyle.None;
    }

    private IEnumerator FranticBackspaceRoutine()
    {
        while (manager.isPlaying)
        {
            timer -= Time.deltaTime;
            
            if (fbTimerText != null) 
            {
                fbTimerText.text = $"AUTO-SEND: {Mathf.Max(0, timer):F1}s";
            }

            if (fbCurrentMessage.Length == 0) 
            {
                if (fbPlayerText != null) fbPlayerText.text = "Sounds great! 👍";
                if (fbTimerText != null) 
                {
                    fbTimerText.text = "SENT!";
                    fbTimerText.style.color = new StyleColor(Color.green);
                }
                AudioManager.Instance?.PlayScorePop();
                MiniGameFeedback.PlaySuccess(fbPlayerText, "CRISIS AVERTED! 😅", container);
                yield return new WaitForSeconds(1.0f);
                manager.MarkWinReady();
                yield break;
            }

            if (timer <= 0f) 
            {
                if (fbTimerText != null) fbTimerText.text = "SENT FIRED!";
                AudioManager.Instance?.PlayBuzzer();
                MiniGameFeedback.PlayError(container, "ACCIDENTALLY SENT!", container);
                yield return new WaitForSeconds(1.0f);
                manager.AbortGame();
                yield break;
            }
            
            yield return null;
        }
    }
}

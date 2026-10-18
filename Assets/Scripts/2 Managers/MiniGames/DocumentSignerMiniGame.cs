using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class DocumentSignerMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement container;

    private ScrollView dsScrollView;
    private Button btnStampApproval;
    private Label dsTimerText;

    private float timer;
    private bool isPlaying = false;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        
        container = root.Q<VisualElement>("DocumentSignerContainer");
        dsScrollView = container.Q<ScrollView>("DsScrollView");
        btnStampApproval = container.Q<Button>("BtnStampApproval");
        dsTimerText = container.Q<Label>("DsTimerText");

        if (btnStampApproval != null)
        {
            btnStampApproval.clicked += () => {
                if (isPlaying)
                {
                    AudioManager.Instance?.PlayStampPass(true);
                    if (dsTimerText != null) dsTimerText.text = "SIGNED!";
                    isPlaying = false;
                    MiniGameFeedback.PlaySuccess(btnStampApproval, "APPROVED! ✍️", container);
                    manager.MarkWinReady();
                }
            };
        }
    }

    public void StartGame()
    {
        isPlaying = true;
        timer = config.timeLimit;
        
        if (dsTimerText != null) dsTimerText.text = $"{timer:F1}s";
        if (dsScrollView != null) dsScrollView.scrollOffset = Vector2.zero;
        
        container.style.display = DisplayStyle.Flex;
        manager.StartCoroutine(DocumentSignerRoutine());
    }

    public void AbortGame()
    {
        isPlaying = false;
        container.style.display = DisplayStyle.None;
    }

    private IEnumerator DocumentSignerRoutine()
    {
        while (isPlaying && manager.isPlaying)
        {
            timer -= Time.deltaTime;
            
            if (dsTimerText != null)
            {
                dsTimerText.text = $"{Mathf.Max(0, timer):F1}s";
            }

            if (timer <= 0f)
            {
                if (manager.currentGame != null && manager.currentGame.canFail)
                {
                    if (dsTimerText != null) dsTimerText.text = "MISSED DEADLINE!";
                    isPlaying = false;
                    manager.AbortGame();
                }
                else
                {
                    isPlaying = false;
                    manager.MarkWinReady();
                }
                yield break;
            }
            yield return null;
        }
    }
}

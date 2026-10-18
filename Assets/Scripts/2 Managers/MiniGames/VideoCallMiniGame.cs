using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class VideoCallMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    
    private VisualElement videoCallContainer;
    private VisualElement vcPhase1;
    private VisualElement vcPhase2;
    private VisualElement vcPhase3;
    private Label vcTimerText;
    private Label vcWigglePromptText;
    private VisualElement vcWiggleProgressBar;
    private ScrollView vcScrollView;
    private VisualElement vcWiggleTouchArea;
    private VisualElement vcMouseGraphic;
    private Button btnVcRaiseHand;
    private Button btnVcAcknowledge;

    private int vcCurrentPhase = 1;
    private float vcWiggleProgress = 0f;
    private bool vcWiggleReady = false;
    private float vcWiggleWaitTimer = 0f;
    private Vector2 vcMousePos;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;

        videoCallContainer = root.Q<VisualElement>("VideoCallContainer");
        if (videoCallContainer != null)
        {
            vcPhase1 = videoCallContainer.Q<VisualElement>("VcPhase1");
            vcPhase2 = videoCallContainer.Q<VisualElement>("VcPhase2");
            vcPhase3 = videoCallContainer.Q<VisualElement>("VcPhase3");
            vcTimerText = videoCallContainer.Q<Label>("VideoCallTimerText");
            vcWigglePromptText = videoCallContainer.Q<Label>("VcWigglePromptText");
            vcWiggleProgressBar = videoCallContainer.Q<VisualElement>("VcWiggleProgressBar");
            vcScrollView = videoCallContainer.Q<ScrollView>("VcScrollView");
            vcWiggleTouchArea = videoCallContainer.Q<VisualElement>("VcWiggleTouchArea");
            vcMouseGraphic = videoCallContainer.Q<VisualElement>("VcMouseGraphic");
            btnVcRaiseHand = videoCallContainer.Q<Button>("BtnVcRaiseHand");
            btnVcAcknowledge = videoCallContainer.Q<Button>("BtnVcAcknowledge");

            if (btnVcRaiseHand != null) btnVcRaiseHand.clicked += OnVcRaiseHandTapped;
            if (btnVcAcknowledge != null) btnVcAcknowledge.clicked += OnVcAcknowledgeTapped;
            
            if (vcWiggleTouchArea != null) 
            {
                vcWiggleTouchArea.RegisterCallback<PointerMoveEvent>(OnVcWiggleMove);
            }
        }
    }

    public void StartGame()
    {
        if (videoCallContainer != null) videoCallContainer.style.display = DisplayStyle.Flex;

        vcCurrentPhase = 1;
        vcWiggleProgress = 0f;
        vcWiggleReady = false;
        vcWiggleWaitTimer = Random.Range(1.0f, 2.5f);
        
        if (vcPhase1 != null) vcPhase1.style.display = DisplayStyle.Flex;
        if (vcPhase2 != null) vcPhase2.style.display = DisplayStyle.None;
        if (vcPhase3 != null) vcPhase3.style.display = DisplayStyle.None;
        
        if (vcScrollView != null) vcScrollView.scrollOffset = Vector2.zero;
        if (vcWiggleProgressBar != null) vcWiggleProgressBar.style.width = new Length(0, LengthUnit.Percent);
        
        if (vcWigglePromptText != null)
        {
            vcWigglePromptText.text = "SCREEN ASLEEP...";
            vcWigglePromptText.style.color = new StyleColor(new Color(0.5f, 0.55f, 0.55f)); // #7F8C8D
        }
        
        vcMousePos = new Vector2(115f, 120f);
        if (vcMouseGraphic != null)
        {
            vcMouseGraphic.style.left = vcMousePos.x;
            vcMouseGraphic.style.top = vcMousePos.y;
        }

        manager.StartCoroutine(GameRoutine());
    }

    public void AbortGame()
    {
        if (videoCallContainer != null) videoCallContainer.style.display = DisplayStyle.None;
        
        // Unregister callbacks
        if (btnVcRaiseHand != null) btnVcRaiseHand.clicked -= OnVcRaiseHandTapped;
        if (btnVcAcknowledge != null) btnVcAcknowledge.clicked -= OnVcAcknowledgeTapped;
        if (vcWiggleTouchArea != null) vcWiggleTouchArea.UnregisterCallback<PointerMoveEvent>(OnVcWiggleMove);
    }

    private void OnVcAcknowledgeTapped()
    {
        if (!manager.isPlaying || vcCurrentPhase != 1) return;
        AudioManager.Instance?.PlayClick();
        
        vcCurrentPhase = 2;
        if (vcPhase1 != null) vcPhase1.style.display = DisplayStyle.None;
        if (vcPhase2 != null) vcPhase2.style.display = DisplayStyle.Flex;
    }

    private void OnVcRaiseHandTapped()
    {
        if (!manager.isPlaying || vcCurrentPhase != 2) return;
        AudioManager.Instance?.PlayClick();
        
        vcCurrentPhase = 3;
        if (vcPhase2 != null) vcPhase2.style.display = DisplayStyle.None;
        if (vcPhase3 != null) vcPhase3.style.display = DisplayStyle.Flex;
    }

    private void OnVcWiggleMove(PointerMoveEvent evt)
    {
        if (!manager.isPlaying || vcCurrentPhase != 3 || !vcWiggleReady) return;
        
        if (evt.pressedButtons != 0)
        {
            vcMousePos.x = Mathf.Clamp(vcMousePos.x + evt.deltaPosition.x, 0f, 230f); // 300 - 70
            vcMousePos.y = Mathf.Clamp(vcMousePos.y + evt.deltaPosition.y, 0f, 240f); // 350 - 110
            
            if (vcMouseGraphic != null)
            {
                vcMouseGraphic.style.left = vcMousePos.x;
                vcMouseGraphic.style.top = vcMousePos.y;
            }

            float moveDist = Mathf.Abs(evt.deltaPosition.x) + Mathf.Abs(evt.deltaPosition.y);
            if (moveDist > 5f)
            {
                vcWiggleProgress += moveDist * 2f;
                float percent = Mathf.Min(100f, vcWiggleProgress / 30f); // 3000px total
                if (vcWiggleProgressBar != null) vcWiggleProgressBar.style.width = new Length(percent, LengthUnit.Percent);
                
                if (percent >= 100f)
                {
                    if (vcWigglePromptText != null) vcWigglePromptText.text = "ONLINE!";
                    AudioManager.Instance?.PlayVictoryFanfare();
                    manager.MarkWinReady();
                }
            }
        }
    }

    private IEnumerator GameRoutine()
    {
        float timer = config.timeLimit;
        
        while (manager.isPlaying)
        {
            timer -= Time.deltaTime;
            if (vcTimerText != null) vcTimerText.text = $"{Mathf.Max(0, timer):F1}s";
            
            if (vcCurrentPhase == 3)
            {
                if (!vcWiggleReady)
                {
                    vcWiggleWaitTimer -= Time.deltaTime;
                    if (vcWiggleWaitTimer <= 0f)
                    {
                        vcWiggleReady = true;
                        if (vcWigglePromptText != null)
                        {
                            vcWigglePromptText.text = "SWIPE TO WAKE!";
                            vcWigglePromptText.style.color = new StyleColor(new Color(0.18f, 0.8f, 0.44f)); // Green
                        }
                    }
                }
                else
                {
                    vcWiggleProgress = Mathf.Max(0f, vcWiggleProgress - (200f * Time.deltaTime));
                    if (vcWiggleProgressBar != null) vcWiggleProgressBar.style.width = new Length(Mathf.Min(100f, vcWiggleProgress / 20f), LengthUnit.Percent);
                }
            }

            if (timer <= 0f)
            {
                if (vcTimerText != null) vcTimerText.text = "TIME UP!";
                AudioManager.Instance?.PlayMinigameFail();
                manager.AbortGame();
                yield break;
            }
            
            yield return null;
        }
    }
}

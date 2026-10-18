using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class BalanceMeterMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement container;

    private VisualElement balanceMeterTrack;
    private VisualElement balanceCenterZone;
    private VisualElement balanceNeedle;
    private VisualElement balanceTimeBar;
    private Label balanceStatusText;
    private Button btnNudgeLeft;
    private Button btnNudgeRight;

    private float balanceValue = 50f;
    private float balanceTimeInZone = 0f;
    private float balanceDrift = 0f;
    private bool isPlaying = false;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        
        container = root.Q<VisualElement>("BalanceMeterContainer");
        balanceMeterTrack = container.Q<VisualElement>("BalanceMeterTrack");
        balanceCenterZone = container.Q<VisualElement>("BalanceCenterZone");
        balanceNeedle = container.Q<VisualElement>("BalanceNeedle");
        balanceTimeBar = container.Q<VisualElement>("BalanceTimeBar");
        balanceStatusText = container.Q<Label>("BalanceStatusText");
        btnNudgeLeft = container.Q<Button>("BtnNudgeLeft");
        btnNudgeRight = container.Q<Button>("BtnNudgeRight");

        if (btnNudgeLeft != null) btnNudgeLeft.clicked += () => OnBalanceNudge(-10f);
        if (btnNudgeRight != null) btnNudgeRight.clicked += () => OnBalanceNudge(10f);
    }

    public void StartGame()
    {
        isPlaying = true;
        balanceValue = 50f;
        balanceTimeInZone = 0f;
        balanceDrift = 0f;

        container.style.display = DisplayStyle.Flex;
        manager.StartCoroutine(BalanceMeterRoutine());
    }

    public void AbortGame()
    {
        isPlaying = false;
        container.style.display = DisplayStyle.None;
    }

    private void OnBalanceNudge(float amount)
    {
        if (!isPlaying || !manager.isPlaying) return;
        balanceValue += amount;
        balanceDrift *= 0.5f; 
        AudioManager.Instance?.PlayClick();
        if (amount > 0 && btnNudgeRight != null) MiniGameFeedback.Punch(btnNudgeRight, 1.12f);
        else if (amount < 0 && btnNudgeLeft != null) MiniGameFeedback.Punch(btnNudgeLeft, 1.12f);
    }

    private IEnumerator BalanceMeterRoutine()
    {
        while (isPlaying && manager.isPlaying)
        {
            balanceDrift += Random.Range(-2f, 2f) * Time.deltaTime * config.balanceDriftSpeed;
            balanceDrift = Mathf.Clamp(balanceDrift, -10f, 10f);
            balanceValue += balanceDrift * Time.deltaTime;
            balanceValue = Mathf.Clamp(balanceValue, 0f, 100f);
            
            if (balanceNeedle != null) balanceNeedle.style.left = new Length(balanceValue, LengthUnit.Percent);
            
            if (balanceValue >= 35f && balanceValue <= 65f)
            {
                balanceTimeInZone += Time.deltaTime;
                float pct = Mathf.Clamp01(balanceTimeInZone / config.balanceHoldTime) * 100f;
                if (balanceTimeBar != null) balanceTimeBar.style.width = new Length(pct, LengthUnit.Percent);
                if (balanceTimeInZone >= config.balanceHoldTime)
                {
                    isPlaying = false;
                    MiniGameFeedback.PlaySuccess(balanceTimeBar ?? container, "BALANCED!", container);
                    AudioManager.Instance?.PlayVictoryFanfare();
                    manager.MarkWinReady();
                }
            }
            yield return null;
        }
    }
}

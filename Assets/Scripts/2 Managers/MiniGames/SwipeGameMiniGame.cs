using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class SwipeGameMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement container;

    private Label swipeObstacleText;
    private Label swipeProgressText;
    private Label swipeTimerText;
    private Button btnDodgeLeft;
    private Button btnDodgeRight;

    private int score = 0;
    private float timer = 15f;
    private int expectedDodgeDir = 0;
    private bool isPlaying = false;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        
        container = root.Q<VisualElement>("SwipeGameContainer");
        swipeObstacleText = container.Q<Label>("SwipeObstacleText");
        swipeProgressText = container.Q<Label>("SwipeProgressText");
        swipeTimerText = container.Q<Label>("SwipeTimerText");
        btnDodgeLeft = container.Q<Button>("BtnDodgeLeft");
        btnDodgeRight = container.Q<Button>("BtnDodgeRight");

        if (btnDodgeLeft != null) btnDodgeLeft.clicked += () => OnDodgeClicked(-1);
        if (btnDodgeRight != null) btnDodgeRight.clicked += () => OnDodgeClicked(1);
    }

    public void StartGame()
    {
        isPlaying = true;
        score = 0;
        timer = config.timeLimit > 0 ? config.timeLimit : 15f;
        container.style.display = DisplayStyle.Flex;
        
        if (swipeProgressText != null) swipeProgressText.text = $"Dodged: {score} / {config.targetScore}";
        manager.StartCoroutine(SwipeGameRoutine());
    }

    public void AbortGame()
    {
        isPlaying = false;
        container.style.display = DisplayStyle.None;
    }

    private void OnDodgeClicked(int dir)
    {
        if (!isPlaying || !manager.isPlaying) return;
        if (expectedDodgeDir == dir)
        {
            expectedDodgeDir = 0;
        }
    }

    private IEnumerator SwipeGameRoutine()
    {
        while (isPlaying && manager.isPlaying && timer > 0 && score < config.targetScore)
        {
            expectedDodgeDir = Random.value > 0.5f ? 1 : -1;
            if (swipeObstacleText != null) swipeObstacleText.text = expectedDodgeDir == -1 ? "◀ THREAT FROM LEFT!" : "THREAT FROM RIGHT! ▶";
            
            float reactTime = 1.5f;
            float elapsed = 0;

            while (elapsed < reactTime)
            {
                if (!isPlaying || !manager.isPlaying) yield break;
                elapsed += Time.deltaTime;
                timer -= Time.deltaTime;
                if (swipeTimerText != null) swipeTimerText.text = $"Time: {Mathf.Ceil(timer)}s";

                if (expectedDodgeDir == 0)
                {
                    score++;
                    if (swipeProgressText != null) swipeProgressText.text = $"Dodged: {score} / {config.targetScore}";
                    break;
                }
                yield return null;
            }

            expectedDodgeDir = 0;
            if (swipeObstacleText != null) swipeObstacleText.text = "Get Ready...";
            yield return new WaitForSeconds(0.5f);
        }

        if (score >= config.targetScore || (manager.currentGame != null && !manager.currentGame.canFail && timer <= 0))
        {
            if (swipeObstacleText != null) swipeObstacleText.text = "SURVIVED!";
            isPlaying = false;
            manager.MarkWinReady();
        }
        else if (timer <= 0 && manager.currentGame != null && manager.currentGame.canFail)
        {
            isPlaying = false;
            manager.AbortGame();
        }
    }
}

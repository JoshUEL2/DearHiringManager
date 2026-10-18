using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class SprintLaneMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement container;

    private Label sprintDistanceText;
    private Label sprintObstacleText;
    private Label sprintTimerText;
    private VisualElement sprintProgressBar;
    private Button btnSprintJump;
    private Button btnSprintDuck;

    private int sprintDistance = 0;
    private bool sprintObstacleIsHigh = false;
    private bool sprintWaitingForInput = false;
    private bool isPlaying = false;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        
        container = root.Q<VisualElement>("SprintLaneContainer");
        sprintDistanceText = container.Q<Label>("SprintDistanceText");
        sprintObstacleText = container.Q<Label>("SprintObstacleText");
        sprintTimerText = container.Q<Label>("SprintTimerText");
        sprintProgressBar = container.Q<VisualElement>("SprintProgressBar");
        btnSprintJump = container.Q<Button>("BtnSprintJump");
        btnSprintDuck = container.Q<Button>("BtnSprintDuck");

        if (btnSprintJump != null) btnSprintJump.clicked += () => OnSprintReact(true);
        if (btnSprintDuck != null) btnSprintDuck.clicked += () => OnSprintReact(false);
    }

    public void StartGame()
    {
        isPlaying = true;
        sprintDistance = 0;
        if (sprintProgressBar != null) sprintProgressBar.style.width = new Length(0, LengthUnit.Percent);
        
        container.style.display = DisplayStyle.Flex;
        manager.StartCoroutine(SprintLaneRoutine());
    }

    public void AbortGame()
    {
        isPlaying = false;
        container.style.display = DisplayStyle.None;
    }

    private void OnSprintReact(bool jump)
    {
        if (!isPlaying || !manager.isPlaying || !sprintWaitingForInput) return;
        
        bool success = (jump && !sprintObstacleIsHigh) || (!jump && sprintObstacleIsHigh);
        if (success)
        {
            sprintDistance++;
            AudioManager.Instance?.PlayWhoosh();
            float pct = Mathf.Clamp01((float)sprintDistance / config.sprintObstacles) * 100f;
            if (sprintProgressBar != null) sprintProgressBar.style.width = new Length(pct, LengthUnit.Percent);
            sprintWaitingForInput = false;
        }
        else if (manager.currentGame != null && manager.currentGame.canFail)
        {
            AudioManager.Instance?.PlayBuzzer();
            AudioManager.Instance?.PlayMinigameFail();
            isPlaying = false;
            manager.AbortGame();
        }
    }

    private IEnumerator SprintLaneRoutine()
    {
        while (isPlaying && manager.isPlaying && sprintDistance < config.sprintObstacles)
        {
            if (sprintDistanceText != null) sprintDistanceText.text = $"Distance: {sprintDistance} / {config.sprintObstacles}";
            sprintObstacleIsHigh = Random.value > 0.5f;
            if (sprintObstacleText != null) sprintObstacleText.text = sprintObstacleIsHigh ? "HIGH OBSTACLE! (Duck)" : "LOW OBSTACLE! (Jump)";
            sprintWaitingForInput = true;
            
            float t = config.sprintSpeed;
            while (t > 0 && sprintWaitingForInput && isPlaying && manager.isPlaying)
            {
                t -= Time.deltaTime;
                if (sprintTimerText != null) sprintTimerText.text = $"React: {t:F1}s";
                yield return null;
            }
            
            if (sprintWaitingForInput && manager.currentGame != null && manager.currentGame.canFail) 
            { 
                AudioManager.Instance?.PlayMinigameFail();
                isPlaying = false; 
                manager.AbortGame(); 
                yield break; 
            }
            sprintWaitingForInput = false;
            yield return new WaitForSeconds(0.2f);
        }
        if (sprintDistance >= config.sprintObstacles)
        {
            AudioManager.Instance?.PlayVictoryFanfare();
            manager.MarkWinReady();
        }
    }
}

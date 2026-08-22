using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class MashRaceMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement container;

    private Label mashTimerText;
    private VisualElement playerProgressBar;
    private VisualElement opponentProgressBar;
    private Button btnMashTap;

    private float playerMashProgress = 0f;
    private float opponentProgress = 0f;
    private int score = 0;
    private bool isPlaying = false;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        
        container = root.Q<VisualElement>("MashRaceContainer");
        mashTimerText = container.Q<Label>("MashTimerText");
        playerProgressBar = container.Q<VisualElement>("PlayerProgressBar");
        opponentProgressBar = container.Q<VisualElement>("OpponentProgressBar");
        btnMashTap = container.Q<Button>("BtnMashTap");

        if (btnMashTap != null)
        {
            btnMashTap.clicked += OnMashTapClicked;
        }
    }

    public void StartGame()
    {
        isPlaying = true;
        playerMashProgress = 0f;
        opponentProgress = 0f;
        score = 0;

        container.style.display = DisplayStyle.Flex;
        
        if (manager.currentGame.name == "Stress test server load")
        {
            if (mashTimerText != null) mashTimerText.style.color = new StyleColor(Color.red);
            if (btnMashTap != null)
            {
                btnMashTap.text = "SEND REQUEST";
                btnMashTap.style.backgroundColor = new StyleColor(Color.red);
            }
        }
        
        manager.StartCoroutine(MashRaceRoutine());
    }

    public void AbortGame()
    {
        isPlaying = false;
        container.style.display = DisplayStyle.None;
    }

    private void OnMashTapClicked()
    {
        if (!isPlaying || !manager.isPlaying) return;

        playerMashProgress += 1f;
        score = Mathf.FloorToInt(playerMashProgress);
        
        if (score >= config.targetScore)
        {
            isPlaying = false;
            float pct = Mathf.Clamp01(playerMashProgress / config.targetScore) * 100f;
            if (playerProgressBar != null) playerProgressBar.style.width = new Length(pct, LengthUnit.Percent);
            manager.MarkWinReady();
        }
    }

    private IEnumerator MashRaceRoutine()
    {
        container.style.translate = new Translate(0, 0, 0);
        if (mashTimerText != null) mashTimerText.text = "MASH!";
        
        float nextAiTapTimer = Random.Range(0.1f, 0.3f);
        float decayRate = (config.targetScore / config.timeLimit) * 0.65f; 

        while (isPlaying && manager.isPlaying)
        {
            playerMashProgress -= decayRate * Time.deltaTime;
            if (playerMashProgress < 0) playerMashProgress = 0;
            
            opponentProgress -= decayRate * Time.deltaTime;
            if (opponentProgress < 0) opponentProgress = 0;

            nextAiTapTimer -= Time.deltaTime;
            if (nextAiTapTimer <= 0f)
            {
                opponentProgress += 1f;
                
                if (Random.value < 0.15f)
                    nextAiTapTimer = Random.Range(0.3f, 0.6f);
                else
                    nextAiTapTimer = Random.Range(0.05f, 0.2f);
            }
            
            score = Mathf.FloorToInt(playerMashProgress);
            
            float pct = Mathf.Clamp01(playerMashProgress / config.targetScore) * 100f;
            if (playerProgressBar != null) playerProgressBar.style.width = new Length(pct, LengthUnit.Percent);
            
            float oppPct = Mathf.Clamp01(opponentProgress / config.targetScore) * 100f;
            if (opponentProgressBar != null) opponentProgressBar.style.width = new Length(oppPct, LengthUnit.Percent);

            if (manager.currentGame != null && manager.currentGame.name == "Stress test server load")
            {
                if (pct > 50f || oppPct > 50f)
                {
                    float intensity = Mathf.Max(pct, oppPct) / 100f * 5f;
                    container.style.translate = new Translate(Random.Range(-intensity, intensity), Random.Range(-intensity, intensity), 0);
                }
                else
                {
                    container.style.translate = new Translate(0, 0, 0);
                }
            }

            if (opponentProgress >= config.targetScore)
            {
                if (manager.currentGame != null && manager.currentGame.canFail)
                {
                    if (mashTimerText != null) mashTimerText.text = "OPPONENT WINS!";
                    isPlaying = false;
                    manager.AbortGame();
                    yield break;
                }
                else
                {
                    opponentProgress = config.targetScore;
                }
            }

            yield return null;
        }
    }
}

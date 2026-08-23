using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class MashRaceMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement container;
    private VisualElement rootContainer;

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
        
        rootContainer = root;
        container = root.Q<VisualElement>("MashRaceContainer");
        mashTimerText = container.Q<Label>("MashTimerText");
        playerProgressBar = container.Q<VisualElement>("PlayerProgressBar");
        opponentProgressBar = container.Q<VisualElement>("OpponentProgressBar");
        btnMashTap = container.Q<Button>("BtnMashTap");

        if (btnMashTap != null)
        {
            btnMashTap.clicked += OnMashTapClicked;
            
            // Stylize the Mash Button
            btnMashTap.style.fontSize = 48;
            btnMashTap.style.borderTopLeftRadius = 20;
            btnMashTap.style.borderTopRightRadius = 20;
            btnMashTap.style.borderBottomLeftRadius = 20;
            btnMashTap.style.borderBottomRightRadius = 20;
            btnMashTap.style.paddingTop = 40;
            btnMashTap.style.paddingBottom = 40;
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
        
        // Physical Button Press
        if (btnMashTap != null)
        {
            btnMashTap.style.scale = new StyleScale(new Vector2(0.9f, 0.9f));
            btnMashTap.schedule.Execute(() => {
                if (btnMashTap != null) btnMashTap.style.scale = new StyleScale(new Vector2(1f, 1f));
            }).StartingIn(50);
        }
        
        // Simulate a "Punch" - Screen shake and flash Player Bar
        manager.StartCoroutine(PunchEffectRoutine(playerProgressBar, new Color(0.18f, 0.8f, 0.44f), true));
        
        if (score >= config.targetScore)
        {
            isPlaying = false;
            float pct = Mathf.Clamp01(playerMashProgress / config.targetScore) * 100f;
            if (playerProgressBar != null) playerProgressBar.style.width = new Length(pct, LengthUnit.Percent);
            
            string winText = (manager.currentGame != null && manager.currentGame.name == "Stress test server load") ? "SERVER OVERLOADED!" : "PULVERISED!";
            manager.StartCoroutine(StampEffectRoutine(winText, new Color(0.18f, 0.8f, 0.44f), true));
        }
    }
    
    private IEnumerator StampEffectRoutine(string text, Color color, bool isWin)
    {
        Label stampText = new Label(text);
        stampText.style.position = Position.Absolute;
        stampText.style.color = new StyleColor(color);
        stampText.style.fontSize = 72;
        stampText.style.unityFontStyleAndWeight = FontStyle.Bold;
        stampText.style.unityTextAlign = TextAnchor.MiddleCenter;
        
        stampText.style.unityTextOutlineColor = Color.black;
        stampText.style.unityTextOutlineWidth = 4f;
        
        stampText.style.top = new Length(50, LengthUnit.Percent);
        stampText.style.left = new Length(50, LengthUnit.Percent);
        stampText.style.translate = new Translate(new Length(-50, LengthUnit.Percent), new Length(-50, LengthUnit.Percent));
        
        manager.miniGameDocument.rootVisualElement.Add(stampText);
        
        float life = 0.8f;
        float elapsed = 0f;
        while(elapsed < life)
        {
            elapsed += Time.deltaTime;
            float p = elapsed / life;
            
            float scale = Mathf.Lerp(2.5f, 1f, p * 10f);
            if (scale < 1f) scale = 1f;
            stampText.style.scale = new StyleScale(new Vector2(scale, scale));
            
            if (p > 0.8f) {
                stampText.style.opacity = 1f - ((p - 0.8f) * 5f);
            }
            
            yield return null;
        }
        
        if (manager.miniGameDocument.rootVisualElement.Contains(stampText)) manager.miniGameDocument.rootVisualElement.Remove(stampText);
        
        if (isWin) manager.MarkWinReady();
        else manager.AbortGame();
    }

    private IEnumerator PunchEffectRoutine(VisualElement bar, Color originalColor, bool isPlayer)
    {
        if (bar != null) bar.style.backgroundColor = new StyleColor(Color.white); // Flash White!
        
        float shakeAmt = isPlayer ? 10f : -10f; // Player punch pushes right, opponent pushes left
        container.style.translate = new Translate(shakeAmt, Random.Range(-5f, 5f), 0);
        
        yield return new WaitForSeconds(0.05f);
        
        container.style.translate = new Translate(0, 0, 0);
        if (bar != null) bar.style.backgroundColor = new StyleColor(originalColor);
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
                
                // Opponent hits back!
                manager.StartCoroutine(PunchEffectRoutine(opponentProgressBar, new Color(0.9f, 0.2f, 0.2f), false));
                
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
                if (mashTimerText != null) mashTimerText.text = "OPPONENT WINS!";
                isPlaying = false;
                
                string loseText = (manager.currentGame != null && manager.currentGame.name == "Stress test server load") ? "TEST FAILED!" : "CRUSHED!";
                manager.StartCoroutine(StampEffectRoutine(loseText, new Color(0.93f, 0.26f, 0.22f), false));
                
                yield break;
            }

            yield return null;
        }
    }
}

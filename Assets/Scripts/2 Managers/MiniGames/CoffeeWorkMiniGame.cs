using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class CoffeeWorkMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement container;

    private VisualElement cwEnergyFill;
    private VisualElement cwProgressFill;
    private Button cwBtnCoffee;
    private Button cwBtnType;
    private Label cwTimerText;

    private float cwEnergy = 100f;
    private float cwProgress = 0f;
    private bool isPlaying = false;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        
        container = root.Q<VisualElement>("CoffeeWorkContainer");
        cwEnergyFill = container.Q<VisualElement>("CwEnergyFill");
        cwProgressFill = container.Q<VisualElement>("CwProgressFill");
        cwBtnCoffee = container.Q<Button>("CwBtnCoffee");
        cwBtnType = container.Q<Button>("CwBtnType");
        cwTimerText = container.Q<Label>("CwTimerText");

        if (cwBtnCoffee != null) cwBtnCoffee.clicked += OnCwBtnCoffee;
        if (cwBtnType != null) cwBtnType.clicked += OnCwBtnType;
    }

    public void StartGame()
    {
        isPlaying = true;
        cwEnergy = 100f;
        cwProgress = 0f;
        container.style.display = DisplayStyle.Flex;

        UpdateCwUI();
        if (cwTimerText != null) cwTimerText.text = $"{config.timeLimit:F1}s";
        
        if (cwBtnType != null)
        {
            if (manager.currentGame != null && manager.currentGame.name == "Debug CSS at 3 AM")
                cwBtnType.text = "⌨️ DEBUG CSS";
            else
                cwBtnType.text = "⌨️ TYPE SLIDES";
        }

        manager.StartCoroutine(CoffeeWorkRoutine());
    }

    public void AbortGame()
    {
        isPlaying = false;
        container.style.display = DisplayStyle.None;
    }

    private void UpdateCwUI()
    {
        if (cwEnergyFill != null)
        {
            cwEnergyFill.style.height = new Length(cwEnergy, LengthUnit.Percent);
            cwEnergyFill.style.backgroundColor = cwEnergy > 30f ? new StyleColor(new Color(0.96f, 0.62f, 0.04f)) : new StyleColor(Color.red);
        }
        if (cwProgressFill != null)
        {
            cwProgressFill.style.height = new Length(cwProgress, LengthUnit.Percent);
        }
    }

    private void OnCwBtnCoffee()
    {
        if (!isPlaying || !manager.isPlaying) return;
        cwEnergy = Mathf.Clamp(cwEnergy + config.coffeeBoost, 0f, 100f);
        MiniGameFeedback.PlayTap(cwBtnCoffee);
        MiniGameFeedback.Punch(cwEnergyFill, 1.12f);
        AudioManager.Instance?.PlayEspressoSteam();
        UpdateCwUI();
    }

    private void OnCwBtnType()
    {
        if (!isPlaying || !manager.isPlaying) return;
        cwProgress = Mathf.Clamp(cwProgress + config.workProgressPerTap, 0f, 100f);
        MiniGameFeedback.PlayTap(cwBtnType);
        MiniGameFeedback.Punch(cwProgressFill, 1.08f);
        AudioManager.Instance?.PlayCherrySwitch();
        UpdateCwUI();
        
        if (cwProgress >= 100f)
        {
            isPlaying = false;
            MiniGameFeedback.PlaySuccess(cwProgressFill, "WORK DONE!", container);
            AudioManager.Instance?.PlayVictoryFanfare();
            manager.MarkWinReady();
        }
    }

    private IEnumerator CoffeeWorkRoutine()
    {
        float timer = config.timeLimit;
        
        while (isPlaying && manager.isPlaying)
        {
            timer -= Time.deltaTime;
            if (cwTimerText != null) cwTimerText.text = $"{Mathf.Max(0, timer):F1}s";
            
            cwEnergy -= config.coffeeDrainRate * Time.deltaTime;
            UpdateCwUI();
            
            if (cwEnergy <= 0f)
            {
                isPlaying = false;
                MiniGameFeedback.PlayError(container, "OUT OF ENERGY!", container);
                AudioManager.Instance?.PlayMinigameFail();
                manager.AbortGame(); // Fails the game
                yield break;
            }
            
            if (timer <= 0)
            {
                isPlaying = false;
                AudioManager.Instance?.PlayMinigameFail();
                manager.AbortGame();
                yield break;
            }
            
            yield return null;
        }
    }
}

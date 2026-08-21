using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class CoffeeWorkMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement container;

    private Label cwTimerText;
    private VisualElement cwEnergyFill;
    private VisualElement cwProgressFill;
    private Button cwBtnCoffee;
    private Button cwBtnType;
    
    private float cwEnergy = 100f;
    private float cwProgress = 0f;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        
        container = root.Q<VisualElement>("CoffeeWorkContainer");
        cwTimerText = container.Q<Label>("CwTimerText");
        cwEnergyFill = container.Q<VisualElement>("CwEnergyFill");
        cwProgressFill = container.Q<VisualElement>("CwProgressFill");
        cwBtnCoffee = container.Q<Button>("CwBtnCoffee");
        cwBtnType = container.Q<Button>("CwBtnType");

        if (cwBtnCoffee != null) cwBtnCoffee.clicked += OnCwBtnCoffee;
        if (cwBtnType != null) cwBtnType.clicked += OnCwBtnType;
    }

    public void StartGame()
    {
        container.style.display = DisplayStyle.Flex;
        
        cwEnergy = 100f;
        cwProgress = 0f;
        UpdateCwUI();
        if (cwTimerText != null) cwTimerText.text = $"{config.timeLimit:F1}s";
        
        // Use instructions or another config variable to decide text. 
        // In legacy code it relied on `currentGame.name == "Debug CSS at 3 AM"`.
        // Let's use config.instructions to deduce what it should be.
        if (cwBtnType != null)
        {
            if (config.instructions != null && config.instructions.Contains("Debug CSS"))
                cwBtnType.text = "⌨️ DEBUG CSS";
            else
                cwBtnType.text = "⌨️ TYPE SLIDES";
        }
        
        manager.StartCoroutine(CoffeeWorkRoutine());
    }

    public void AbortGame()
    {
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
        if (!manager.isPlaying) return;
        cwEnergy = Mathf.Clamp(cwEnergy + config.coffeeBoost, 0f, 100f);
        UpdateCwUI();
    }

    private void OnCwBtnType()
    {
        if (!manager.isPlaying) return;
        cwProgress = Mathf.Clamp(cwProgress + config.workProgressPerTap, 0f, 100f);
        UpdateCwUI();
        
        if (cwProgress >= 100f)
        {
            manager.MarkWinReady();
        }
    }

    private IEnumerator CoffeeWorkRoutine()
    {
        float timer = config.timeLimit;
        
        while (manager.isPlaying)
        {
            timer -= Time.deltaTime;
            if (cwTimerText != null) cwTimerText.text = $"{Mathf.Max(0, timer):F1}s";
            
            cwEnergy -= config.coffeeDrainRate * Time.deltaTime;
            UpdateCwUI();
            
            if (cwEnergy <= 0f)
            {
                if (cwTimerText != null) cwTimerText.text = "FELL ASLEEP!";
                manager.AbortGame();
                yield break;
            }
            
            if (timer <= 0f)
            {
                if (cwTimerText != null) cwTimerText.text = "TIME UP!";
                manager.AbortGame();
                yield break;
            }
            yield return null;
        }
    }
}

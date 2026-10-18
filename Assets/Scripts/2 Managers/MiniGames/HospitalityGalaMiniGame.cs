using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class HospitalityGalaMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement rootElement;
    
    private VisualElement hgContainer;
    private VisualElement hgIndicator;
    private Label hgTimerText;
    
    private float hgBalance = 0.5f;
    private float hgVelocity = 0f;
    private float timer;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        this.rootElement = root;
        
        hgContainer = root.Q<VisualElement>("HospitalityGalaContainer");
        hgIndicator = root.Q<VisualElement>("HgIndicator");
        hgTimerText = root.Q<Label>("HgTimerText");

        if (hgContainer != null) 
        {
            hgContainer.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        }
    }

    private void OnPointerMove(PointerMoveEvent ev)
    {
        if (!manager.isPlaying) return;
        
        float w = hgContainer.resolvedStyle.width;
        if (w > 0) 
        {
            float normalizedX = Mathf.Clamp01(ev.localPosition.x / w);
            hgVelocity = (normalizedX - 0.5f) * 2f; // -1 to 1 based on touch pos
        }
    }

    public void StartGame()
    {
        if (hgContainer != null) hgContainer.style.display = DisplayStyle.Flex;
        
        timer = config.timeLimit;
        hgBalance = 0.5f;
        hgVelocity = 0f;
        
        manager.StartCoroutine(GameRoutine());
    }

    public void AbortGame()
    {
        if (hgContainer != null) hgContainer.style.display = DisplayStyle.None;
    }

    private IEnumerator GameRoutine()
    {
        float randomDrift = 0f;
        while (manager.isPlaying)
        {
            timer -= Time.deltaTime;
            if (hgTimerText != null) hgTimerText.text = $"{Mathf.Max(0, timer):F1}s";
            
            // Apply drift
            randomDrift += Random.Range(-2f, 2f) * Time.deltaTime;
            randomDrift = Mathf.Clamp(randomDrift, -0.5f, 0.5f);
            
            hgBalance += (hgVelocity + randomDrift) * Time.deltaTime * 1.5f;
            
            if (hgIndicator != null) 
            {
                hgIndicator.style.left = new StyleLength(new Length(hgBalance * 100f, LengthUnit.Percent));
            }

            if (hgBalance < 0f || hgBalance > 1f) 
            {
                if (hgTimerText != null) hgTimerText.text = "SPILLED!";
                AudioManager.Instance?.PlayDishClatter();
                AudioManager.Instance?.PlayMinigameFail();
                yield return new WaitForSeconds(1f);
                manager.AbortGame();
                yield break;
            }

            if (timer <= 0f) 
            {
                AudioManager.Instance?.PlayVictoryFanfare();
                manager.MarkWinReady();
                yield break;
            }
            yield return null;
        }
    }
}

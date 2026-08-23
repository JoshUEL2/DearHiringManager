using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class FoodPrepRushMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement rootElement;

    private VisualElement container;
    private Label prepTimerText;
    private Label prepScoreText;
    
    private VisualElement[] prepStations = new VisualElement[4];
    private VisualElement[] prepBars = new VisualElement[4];
    private Label[] prepStatus = new Label[4];
    private Label[] prepEmojis = new Label[4];
    private Button[] prepBtns = new Button[4];

    private float[] prepProgress = new float[4];
    private float[] prepSpeeds = new float[4];
    private float[] prepFreezes = new float[4];
    
    private int score = 0;
    private float timer = 0f;
    
    private System.Action[] clickActions = new System.Action[4];

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        this.rootElement = root;

        container = root.Q<VisualElement>("FoodPrepContainer");
        if (container != null)
        {
            // Stretch the container so it fills nicely
            container.style.alignSelf = Align.Stretch;
            
            prepTimerText = container.Q<Label>("PrepTimerText");
            prepScoreText = container.Q<Label>("PrepScoreText");

            for (int i = 0; i < 4; i++)
            {
                prepStations[i] = container.Q<VisualElement>($"Station{i}");
                prepBars[i] = container.Q<VisualElement>($"PrepBar{i}");
                prepStatus[i] = container.Q<Label>($"StationStatus{i}");
                prepBtns[i] = container.Q<Button>($"BtnStation{i}");
                
                // Find the emoji label (it's the only label other than the status label)
                var labels = prepStations[i].Query<Label>().ToList();
                prepEmojis[i] = labels.FirstOrDefault(l => l != prepStatus[i]);
                
                int index = i;
                clickActions[i] = () => OnFoodPrepStationTapped(index);
            }
        }
    }

    public void StartGame()
    {
        if (container != null)
        {
            container.style.display = DisplayStyle.Flex;
            for (int i = 0; i < 4; i++)
            {
                if (prepBtns[i] != null)
                {
                    prepBtns[i].clicked += clickActions[i];
                }
            }
        }

        timer = config.timeLimit;
        score = 0;

        for (int i = 0; i < 4; i++)
        {
            prepSpeeds[i] = Random.Range(35f, 65f);
            prepProgress[i] = 0f;
            prepFreezes[i] = 0f;
            if (prepStatus[i] != null) prepStatus[i].text = "";
            if (prepBars[i] != null) prepBars[i].style.height = new Length(0, LengthUnit.Percent);
            
            // Reset transforms
            if (prepStations[i] != null)
            {
                prepStations[i].style.translate = new Translate(0, 0, 0);
                prepStations[i].style.scale = new StyleScale(new Vector2(1f, 1f));
                prepStations[i].style.backgroundColor = new StyleColor(new Color(0.12f, 0.16f, 0.23f)); // Slate base
            }
        }

        if (prepScoreText != null) prepScoreText.text = $"Prepped: {score} / {config.targetScore}";
        if (prepTimerText != null) prepTimerText.text = $"{timer:0.0}s";

        manager.StartCoroutine(FoodPrepRoutine());
    }

    public void AbortGame()
    {
        if (container != null)
        {
            container.style.display = DisplayStyle.None;
            for (int i = 0; i < 4; i++)
            {
                if (prepBtns[i] != null)
                {
                    prepBtns[i].clicked -= clickActions[i];
                }
            }
        }
    }

    private void OnFoodPrepStationTapped(int index)
    {
        if (!manager.isPlaying) return;
        if (prepFreezes[index] > 0f) return; // Station is frozen

        float p = prepProgress[index];
        if (p < 70f)
        {
            if (prepStatus[index] != null) 
            {
                prepStatus[index].text = "RAW!";
                prepStatus[index].style.color = new StyleColor(new Color(0.9f, 0.2f, 0.2f)); // Red text
            }
            prepFreezes[index] = 1.0f;
            if (manager.currentGame.canFail) score = Mathf.Max(0, score - 1);
            manager.StartCoroutine(ErrorShakeRoutine(index));
        }
        else if (p >= 70f && p <= 95f)
        {
            if (prepStatus[index] != null) 
            {
                prepStatus[index].text = "PERFECT!";
                prepStatus[index].style.color = new StyleColor(new Color(0.18f, 0.8f, 0.44f)); // Green text
            }
            prepFreezes[index] = 0.5f;
            score++;
            manager.StartCoroutine(SuccessBounceRoutine(index));
            
            if (score >= config.targetScore)
            {
                if (prepTimerText != null) prepTimerText.text = "YOU WIN!";
                manager.MarkWinReady();
            }
        }
        else
        {
            if (prepStatus[index] != null) 
            {
                prepStatus[index].text = (index == 2) ? "SPILLED!" : "BURNED!";
                prepStatus[index].style.color = new StyleColor(new Color(0.9f, 0.2f, 0.2f)); // Red text
            }
            prepFreezes[index] = 1.5f;
            if (manager.currentGame.canFail) score = Mathf.Max(0, score - 1);
            manager.StartCoroutine(ErrorShakeRoutine(index));
        }
        
        manager.StartCoroutine(StatusPopRoutine(prepStatus[index]));
        if (prepScoreText != null) prepScoreText.text = $"Prepped: {score} / {config.targetScore}";
    }

    private IEnumerator StatusPopRoutine(Label statusLabel)
    {
        if (statusLabel == null) yield break;
        
        float t = 0f;
        while (t < 1f && manager.isPlaying)
        {
            t += Time.deltaTime * 10f;
            float scale = Mathf.Lerp(0f, 1f, t * t * (3f - 2f * t)); // Smoothstep
            statusLabel.style.scale = new StyleScale(new Vector2(scale, scale));
            yield return null;
        }
        statusLabel.style.scale = new StyleScale(new Vector2(1f, 1f));
    }

    private IEnumerator SuccessBounceRoutine(int index)
    {
        VisualElement station = prepStations[index];
        StyleColor originalBg = new StyleColor(new Color(0.12f, 0.16f, 0.23f)); // Slate
        
        station.style.backgroundColor = new StyleColor(new Color(0.18f, 0.8f, 0.44f)); // Flash Wordle Green
        
        float duration = 0.2f;
        float elapsed = 0f;
        while (elapsed < duration && manager.isPlaying)
        {
            elapsed += Time.deltaTime;
            float p = elapsed / duration;
            float scale = 1f + (Mathf.Sin(p * Mathf.PI) * 0.15f); // Pop out
            station.style.scale = new StyleScale(new Vector2(scale, scale));
            yield return null;
        }
        
        station.style.scale = new StyleScale(new Vector2(1f, 1f));
        station.style.backgroundColor = originalBg;
    }

    private IEnumerator ErrorShakeRoutine(int index)
    {
        VisualElement station = prepStations[index];
        StyleColor originalBg = new StyleColor(new Color(0.12f, 0.16f, 0.23f)); // Slate
        
        station.style.backgroundColor = new StyleColor(new Color(0.9f, 0.2f, 0.2f)); // Flash Red
        
        float duration = 0.4f;
        float elapsed = 0f;
        while (elapsed < duration && manager.isPlaying)
        {
            elapsed += Time.deltaTime;
            float xOffset = Mathf.Sin(elapsed * 50f) * 8f; // Fast, violent shake
            station.style.translate = new Translate(xOffset, 0, 0);
            yield return null;
        }
        
        station.style.translate = new Translate(0, 0, 0);
        station.style.backgroundColor = originalBg;
    }

    private IEnumerator FoodPrepRoutine()
    {
        while (manager.isPlaying && score < config.targetScore && timer > 0f)
        {
            timer -= Time.deltaTime;
            if (prepTimerText != null) prepTimerText.text = $"{timer:0.0}s";

            for (int i = 0; i < 4; i++)
            {
                if (prepFreezes[i] > 0f)
                {
                    prepFreezes[i] -= Time.deltaTime;
                    if (prepFreezes[i] <= 0f)
                    {
                        prepFreezes[i] = 0f;
                        if (prepStatus[i] != null) prepStatus[i].text = "";
                        prepProgress[i] = 0f;
                        prepSpeeds[i] = Random.Range(35f, 65f); // Slightly faster to compensate for faster clears
                        
                        // Reset Emoji
                        if (prepEmojis[i] != null)
                        {
                            prepEmojis[i].style.scale = new StyleScale(new Vector2(1f, 1f));
                            prepEmojis[i].style.translate = new Translate(0, 0, 0);
                        }
                    }
                }
                else
                {
                    prepProgress[i] += prepSpeeds[i] * Time.deltaTime;
                    float p = prepProgress[i];
                    
                    if (p >= 100f)
                    {
                        p = 100f;
                        prepProgress[i] = 100f;
                        if (prepStatus[i] != null) 
                        {
                            prepStatus[i].text = (i == 2) ? "SPILLED!" : "BURNED!";
                            prepStatus[i].style.color = new StyleColor(new Color(0.9f, 0.2f, 0.2f));
                        }
                        prepFreezes[i] = 1.5f;
                        if (manager.currentGame.canFail) score = Mathf.Max(0, score - 1);
                        if (prepScoreText != null) prepScoreText.text = $"Prepped: {score} / {config.targetScore}";
                        
                        manager.StartCoroutine(StatusPopRoutine(prepStatus[i]));
                        manager.StartCoroutine(ErrorShakeRoutine(i));
                    }
                    
                    // -- DYNAMIC BAR COLORS --
                    if (prepBars[i] != null)
                    {
                        if (p < 70f)
                        {
                            prepBars[i].style.backgroundColor = new StyleColor(new Color(0.94f, 0.76f, 0.05f)); // Warning Yellow
                        }
                        else if (p >= 70f && p <= 95f)
                        {
                            prepBars[i].style.backgroundColor = new StyleColor(new Color(0.18f, 0.8f, 0.44f)); // Perfect Green
                        }
                        else
                        {
                            // Pulsing/Flashing Red when burning
                            float flash = 0.8f + (Mathf.Sin(Time.time * 20f) * 0.2f);
                            prepBars[i].style.backgroundColor = new StyleColor(new Color(flash, 0.2f, 0.2f)); 
                        }
                    }
                    
                    // -- BOILING EMOJI PHYSICS --
                    if (prepEmojis[i] != null)
                    {
                        // Scale up slowly as it cooks, then frantically shake/throb as it nears burning
                        float baseScale = 1f + (p / 100f) * 0.3f; // Grows up to 1.3x
                        
                        if (p >= 70f)
                        {
                            // "Sizzling" effect
                            float pulse = Mathf.Sin(Time.time * (p * 0.3f)) * 0.1f; 
                            prepEmojis[i].style.scale = new StyleScale(new Vector2(baseScale + pulse, baseScale + pulse));
                            
                            // Slight physical jitter when getting dangerously close to burning
                            if (p > 85f) 
                            {
                                float jitter = Mathf.Sin(Time.time * 50f) * 2f;
                                prepEmojis[i].style.translate = new Translate(jitter, 0, 0);
                            }
                        }
                        else
                        {
                            prepEmojis[i].style.scale = new StyleScale(new Vector2(baseScale, baseScale));
                            prepEmojis[i].style.translate = new Translate(0, 0, 0);
                        }
                    }
                }
                
                if (prepBars[i] != null) prepBars[i].style.height = new Length(prepProgress[i], LengthUnit.Percent);
            }

            if (timer <= 0f)
            {
                if (manager.currentGame.canFail)
                {
                    if (prepTimerText != null) prepTimerText.text = "TIME UP!";
                    manager.AbortGame();
                    yield break;
                }
                else
                {
                    manager.MarkWinReady();
                }
            }

            yield return null;
        }
    }
}

using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class SentienceArrayMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement rootElement;

    private VisualElement sentienceArrayContainer;
    private Label saTimerText;
    private VisualElement saProgressFill;
    private VisualElement[] saNodes = new VisualElement[16];
    private float[] saHeat = new float[16];
    private float saSentienceLevel = 0f;
    private float timer = 0f;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        this.rootElement = root;

        sentienceArrayContainer = root.Q<VisualElement>("SentienceArrayContainer");
        saTimerText = root.Q<Label>("SaTimerText");
        saProgressFill = root.Q<VisualElement>("SaProgressFill");

        for (int i = 0; i < 16; i++)
        {
            int index = i;
            saNodes[i] = root.Q<VisualElement>($"SaNode_{i}");
            if (saNodes[i] != null)
            {
                saNodes[i].RegisterCallback<PointerDownEvent>(ev => {
                    if (!manager.isPlaying) return;
                    if (saHeat[index] > 0) saHeat[index] = 0; // suppress
                });
            }
        }
    }

    public void StartGame()
    {
        sentienceArrayContainer.style.display = DisplayStyle.Flex;
        
        timer = config.timeLimit;
        saSentienceLevel = 0f;
        
        for (int i = 0; i < 16; i++)
        {
            saHeat[i] = 0f;
            if (saNodes[i] != null) 
            {
                saNodes[i].style.backgroundColor = new StyleColor(ColorUtility.TryParseHtmlString("#1E293B", out Color c) ? c : Color.blue);
            }
        }
        
        if (saProgressFill != null) 
        {
            saProgressFill.style.width = new StyleLength(new Length(0, LengthUnit.Percent));
        }
        
        manager.StartCoroutine(SentienceArrayRoutine());
    }

    public void AbortGame()
    {
        if (sentienceArrayContainer != null)
        {
            sentienceArrayContainer.style.display = DisplayStyle.None;
        }
    }

    private IEnumerator SentienceArrayRoutine()
    {
        while (manager.isPlaying)
        {
            timer -= Time.deltaTime;
            if (saTimerText != null) saTimerText.text = $"{timer:F1}s";

            // Spawn anomalies randomly (slower spawn rate)
            if (UnityEngine.Random.value < 0.015f) 
            {
                int r = UnityEngine.Random.Range(0, 16);
                if (saHeat[r] == 0f) saHeat[r] = 0.01f;
            }

            for (int i = 0; i < 16; i++)
            {
                if (saHeat[i] > 0f)
                {
                    saHeat[i] += Time.deltaTime;
                    if (saNodes[i] != null) 
                    {
                        saNodes[i].style.backgroundColor = new StyleColor(ColorUtility.TryParseHtmlString("#E74C3C", out Color c) ? c : Color.red);
                    }
                    // If it burns too long, it increases sentience permanently (more reaction time)
                    if (saHeat[i] > 2.5f)
                    {
                        saSentienceLevel += 5f; // Less penalty
                        saHeat[i] = 0f;
                    }
                }
                else
                {
                    if (saNodes[i] != null) 
                    {
                        saNodes[i].style.backgroundColor = new StyleColor(ColorUtility.TryParseHtmlString("#1E293B", out Color c) ? c : Color.blue);
                    }
                }
            }

            if (saProgressFill != null) 
            {
                saProgressFill.style.width = new StyleLength(new Length(Mathf.Clamp(saSentienceLevel, 0, 100), LengthUnit.Percent));
            }

            if (saSentienceLevel >= 100f)
            {
                manager.AbortGame(); // Fail
                yield break;
            }

            if (timer <= 0f)
            {
                manager.MarkWinReady();
                yield break;
            }
            
            yield return null;
        }
    }
}

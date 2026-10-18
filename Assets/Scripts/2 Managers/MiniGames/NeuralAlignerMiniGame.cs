using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class NeuralAlignerMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement rootElement;
    private VisualElement container;

    private Label naStatusText;
    private Slider naMoralSlider;
    private VisualElement naMoralLight;
    private Slider naLogicSlider;
    private VisualElement naLogicLight;
    private Slider naEmpathySlider;
    private VisualElement naEmpathyLight;

    private float naMoralTarget;
    private float naLogicTarget;
    private float naEmpathyTarget;
    private float timer;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        this.rootElement = root;

        container = root.Q<VisualElement>("NeuralAlignerContainer");

        naStatusText = root.Q<Label>("NaStatusText");
        naMoralSlider = root.Q<Slider>("NaMoralSlider");
        naMoralLight = root.Q<VisualElement>("NaMoralLight");
        naLogicSlider = root.Q<Slider>("NaLogicSlider");
        naLogicLight = root.Q<VisualElement>("NaLogicLight");
        naEmpathySlider = root.Q<Slider>("NaEmpathySlider");
        naEmpathyLight = root.Q<VisualElement>("NaEmpathyLight");
        
        if (naMoralSlider != null)
        {
            naMoralSlider.RegisterValueChangedCallback(ev => UpdateNeuralAlignerLights());
            naMoralSlider.RegisterCallback<PointerDownEvent>(ev => { if(manager.isPlaying) ev.StopPropagation(); });
        }
        if (naLogicSlider != null)
        {
            naLogicSlider.RegisterValueChangedCallback(ev => UpdateNeuralAlignerLights());
            naLogicSlider.RegisterCallback<PointerDownEvent>(ev => { if(manager.isPlaying) ev.StopPropagation(); });
        }
        if (naEmpathySlider != null)
        {
            naEmpathySlider.RegisterValueChangedCallback(ev => UpdateNeuralAlignerLights());
            naEmpathySlider.RegisterCallback<PointerDownEvent>(ev => { if(manager.isPlaying) ev.StopPropagation(); });
        }
    }

    public void StartGame()
    {
        container.style.display = DisplayStyle.Flex;
        
        timer = config.timeLimit;
        
        naMoralTarget = UnityEngine.Random.Range(10f, 90f);
        naLogicTarget = UnityEngine.Random.Range(10f, 90f);
        naEmpathyTarget = UnityEngine.Random.Range(10f, 90f);

        if (naMoralSlider != null) naMoralSlider.value = 50f;
        if (naLogicSlider != null) naLogicSlider.value = 50f;
        if (naEmpathySlider != null) naEmpathySlider.value = 50f;

        UpdateNeuralAlignerLights();

        manager.StartCoroutine(NeuralAlignerRoutine());
    }

    public void AbortGame()
    {
        container.style.display = DisplayStyle.None;
    }

    private void UpdateNeuralAlignerLights()
    {
        float totalError = 0f;
        
        if (naMoralSlider != null && naMoralLight != null)
        {
            float err = Mathf.Abs(naMoralSlider.value - naMoralTarget);
            totalError += err;
            if (err <= 5f) naMoralLight.style.backgroundColor = new StyleColor(Color.green);
            else if (err <= 25f) naMoralLight.style.backgroundColor = new StyleColor(Color.yellow);
            else naMoralLight.style.backgroundColor = new StyleColor(ColorUtility.TryParseHtmlString("#EF4444", out Color c) ? c : Color.red);
        }
        if (naLogicSlider != null && naLogicLight != null)
        {
            float err = Mathf.Abs(naLogicSlider.value - naLogicTarget);
            totalError += err;
            if (err <= 5f) naLogicLight.style.backgroundColor = new StyleColor(Color.green);
            else if (err <= 25f) naLogicLight.style.backgroundColor = new StyleColor(Color.yellow);
            else naLogicLight.style.backgroundColor = new StyleColor(ColorUtility.TryParseHtmlString("#EF4444", out Color c) ? c : Color.red);
        }
        if (naEmpathySlider != null && naEmpathyLight != null)
        {
            float err = Mathf.Abs(naEmpathySlider.value - naEmpathyTarget);
            totalError += err;
            if (err <= 5f) naEmpathyLight.style.backgroundColor = new StyleColor(Color.green);
            else if (err <= 25f) naEmpathyLight.style.backgroundColor = new StyleColor(Color.yellow);
            else naEmpathyLight.style.backgroundColor = new StyleColor(ColorUtility.TryParseHtmlString("#EF4444", out Color c) ? c : Color.red);
        }

        if (naStatusText != null)
        {
            if (totalError <= 15f)
            {
                naStatusText.text = "STABILITY: OPTIMAL (ALIGNMENT STABLE)";
                naStatusText.style.color = new StyleColor(Color.green);
                naStatusText.style.translate = new StyleTranslate(new Translate(0, 0, 0));
            }
            else
            {
                naStatusText.text = $"STABILITY: CRITICAL ({totalError:F0}% ERROR)";
                naStatusText.style.color = new StyleColor(ColorUtility.TryParseHtmlString("#EF4444", out Color c) ? c : Color.red);
                // Shake effect
                naStatusText.style.translate = new StyleTranslate(new Translate(UnityEngine.Random.Range(-3f, 3f), UnityEngine.Random.Range(-3f, 3f), 0));
            }
        }
    }

    private IEnumerator NeuralAlignerRoutine()
    {
        while (manager.isPlaying)
        {
            timer -= Time.deltaTime;
            
            UpdateNeuralAlignerLights();

            // Check win condition (all within 5)
            float totalErr = 0f;
            if (naMoralSlider != null) totalErr += Mathf.Abs(naMoralSlider.value - naMoralTarget);
            if (naLogicSlider != null) totalErr += Mathf.Abs(naLogicSlider.value - naLogicTarget);
            if (naEmpathySlider != null) totalErr += Mathf.Abs(naEmpathySlider.value - naEmpathyTarget);

            if (totalErr <= 15f && Mathf.Abs(naMoralSlider.value - naMoralTarget) <= 5f && Mathf.Abs(naLogicSlider.value - naLogicTarget) <= 5f && Mathf.Abs(naEmpathySlider.value - naEmpathyTarget) <= 5f)
            {
                if (naStatusText != null)
                {
                    naStatusText.style.translate = new StyleTranslate(new Translate(0, 0, 0));
                }
                AudioManager.Instance?.PlayVictoryFanfare();
                manager.MarkWinReady();
                yield break;
            }

            if (timer <= 0f)
            {
                if (config.canFail)
                {
                    if (naStatusText != null)
                    {
                        naStatusText.text = "CORE MELTDOWN!";
                        naStatusText.style.translate = new StyleTranslate(new Translate(0, 0, 0));
                    }
                    AudioManager.Instance?.PlayMinigameFail();
                    manager.FailGame();
                }
                else
                {
                    AudioManager.Instance?.PlayVictoryFanfare();
                    manager.MarkWinReady();
                }
                yield break;
            }

            yield return null;
        }
    }
}

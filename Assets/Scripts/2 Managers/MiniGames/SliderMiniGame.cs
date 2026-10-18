using UnityEngine;
using UnityEngine.UIElements;

public class SliderMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;

    private VisualElement container;
    private Label sliderTargetText;
    private Slider gameSlider;
    private Button btnSubmitSlider;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;

        container = root.Q<VisualElement>("SliderGameContainer");
        sliderTargetText = container.Q<Label>("SliderTargetText");
        gameSlider = container.Q<Slider>("GameSlider");
        btnSubmitSlider = container.Q<Button>("BtnSubmitSlider");

        if (gameSlider != null)
        {
            gameSlider.UnregisterValueChangedCallback(OnSliderValueChanged);
            gameSlider.RegisterValueChangedCallback(OnSliderValueChanged);
        }

        if (btnSubmitSlider != null)
        {
            btnSubmitSlider.clicked -= OnSliderSubmitted;
            btnSubmitSlider.clicked += OnSliderSubmitted;
        }
    }

    public void StartGame()
    {
        container.style.display = DisplayStyle.Flex;
        if (gameSlider != null)
        {
            gameSlider.value = 50f;
            UpdateSliderText(50f);
        }
    }

    private void OnSliderValueChanged(ChangeEvent<float> ev)
    {
        UpdateSliderText(ev.newValue);
    }

    private void UpdateSliderText(float val)
    {
        if (sliderTargetText != null)
        {
            sliderTargetText.text = $"Current: {val:F0}% (Target: {config.sliderMin}% - {config.sliderMax}%)";
        }
    }

    private void OnSliderSubmitted()
    {
        if (!manager.isPlaying) return;

        if (gameSlider != null)
        {
            float val = gameSlider.value;
            if (val >= config.sliderMin && val <= config.sliderMax)
            {
                MiniGameFeedback.PlaySuccess(gameSlider, "TARGET MATCH!", container);
                manager.MarkWinReady();
            }
            else
            {
                MiniGameFeedback.PlayError(gameSlider, "OUT OF RANGE!", container);
            }
        }
    }

    public void AbortGame()
    {
        container.style.display = DisplayStyle.None;
        if (btnSubmitSlider != null)
        {
            btnSubmitSlider.clicked -= OnSliderSubmitted;
        }
        if (gameSlider != null)
        {
            gameSlider.UnregisterValueChangedCallback(OnSliderValueChanged);
        }
    }
}

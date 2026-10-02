using UnityEngine;
using UnityEngine.UIElements;

public class TapMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    
    private VisualElement container;
    private Button btnTap;
    private Label tapProgressText;

    private int score;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;

        container = root.Q<VisualElement>("TapGameContainer");
        btnTap = container.Q<Button>("BtnActionTap");
        tapProgressText = container.Q<Label>("TapProgressText");

        if (btnTap != null)
        {
            // Remove previous callbacks to prevent multiple triggers if played again
            btnTap.clicked -= OnTapClicked;
            btnTap.clicked += OnTapClicked;
        }
    }

    public void StartGame()
    {
        score = 0;
        container.style.display = DisplayStyle.Flex;
        tapProgressText.text = $"Progress: 0 / {config.targetScore}";
    }

    private void OnTapClicked()
    {
        if (!manager.isPlaying) return;

        score++;
        tapProgressText.text = $"Progress: {score} / {config.targetScore}";
        
        if (score >= config.targetScore)
        {
            manager.MarkWinReady();
        }
    }

    public void AbortGame()
    {
        container.style.display = DisplayStyle.None;
        if (btnTap != null)
        {
            btnTap.clicked -= OnTapClicked;
        }
    }
}

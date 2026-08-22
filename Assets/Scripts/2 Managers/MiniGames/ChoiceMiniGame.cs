using UnityEngine;
using UnityEngine.UIElements;

public class ChoiceMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement container;

    private Button btnChoice1;
    private Button btnChoice2;
    private Button btnChoice3;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        
        container = root.Q<VisualElement>("ChoiceGameContainer");
        btnChoice1 = container.Q<Button>("BtnChoice1");
        btnChoice2 = container.Q<Button>("BtnChoice2");
        btnChoice3 = container.Q<Button>("BtnChoice3");

        if (btnChoice1 != null) btnChoice1.clicked += () => OnChoiceClicked(btnChoice1.text);
        if (btnChoice2 != null) btnChoice2.clicked += () => OnChoiceClicked(btnChoice2.text);
        if (btnChoice3 != null) btnChoice3.clicked += () => OnChoiceClicked(btnChoice3.text);
    }

    public void StartGame()
    {
        container.style.display = DisplayStyle.Flex;
        if (btnChoice1 != null) btnChoice1.text = config.choices[0];
        if (btnChoice2 != null) btnChoice2.text = config.choices[1];
        if (btnChoice3 != null) btnChoice3.text = config.choices[2];
    }

    public void AbortGame()
    {
        container.style.display = DisplayStyle.None;
    }

    private void OnChoiceClicked(string choice)
    {
        if (!manager.isPlaying) return;
        if (choice == config.correctChoice)
        {
            manager.MarkWinReady();
        }
    }
}

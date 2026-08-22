using UnityEngine.UIElements;

public class LegacyMiniGameWrapper : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
    }

    public void StartGame()
    {
        UnityEngine.Debug.LogError($"Legacy minigame {config.type} has not been implemented in a strategy! Aborting.");
        manager.AbortGame();
    }

    public void AbortGame()
    {
        // Legacy system uses manager.AbortGame() directly, so we do nothing here,
        // or we could call manager.CleanupGame() depending on how Abort is routed.
    }
}

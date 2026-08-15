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
        // Route to the old massive SetupGameType switch statement
        manager.SetupGameType(config.type);
    }

    public void AbortGame()
    {
        // Legacy system uses manager.AbortGame() directly, so we do nothing here,
        // or we could call manager.CleanupGame() depending on how Abort is routed.
    }
}

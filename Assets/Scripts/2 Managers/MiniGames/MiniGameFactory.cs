public static class MiniGameFactory
{
    public static IMiniGame Create(GameType type)
    {
        switch (type)
        {
            case GameType.Tap:
                return new TapMiniGame();
            case GameType.Slider:
                return new SliderMiniGame();
            case GameType.GridDodge:
                return new GridDodgeMiniGame();
            case GameType.TicketSwipe:
                return new TicketSwipeMiniGame();
            case GameType.ConstructionSite:
                return new ConstructionSiteMiniGame();
            // ... future ports ...
            case GameType.QATriage: return new QATriageMiniGame();
            case GameType.GitMerge: return new GitMergeMiniGame();
            default:
                return new LegacyMiniGameWrapper();
        }
    }
}

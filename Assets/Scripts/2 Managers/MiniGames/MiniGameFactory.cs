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
            // ... future ports ...
            default:
                return new LegacyMiniGameWrapper();
        }
    }
}

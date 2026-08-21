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
            case GameType.CenterDiv: return new CenterDivMiniGame();
            case GameType.SizeMatch: return new SizeMatchMiniGame();
            case GameType.DragCenter: return new DragCenterMiniGame();
            case GameType.FallingSort: return new FallingSortMiniGame();
            case GameType.MemoryMatch: return new MemoryMatchMiniGame();
            case GameType.WhackGrid: return new WhackGridMiniGame();
            case GameType.CodeBreaker: return new CodeBreakerMiniGame();
            case GameType.DragMatch: return new DragMatchMiniGame();
            case GameType.ScrubPlate: return new ScrubPlateMiniGame();
            case GameType.SweepFloor: return new SweepFloorMiniGame();
            case GameType.Tetris: return new TetrisMiniGame();
            case GameType.VideoCall: return new VideoCallMiniGame();
            case GameType.KanbanBoard: return new KanbanBoardMiniGame();
            case GameType.FileSorter: return new FileSorterMiniGame();
            case GameType.CoffeeWork: return new CoffeeWorkMiniGame();
            default:
                return new LegacyMiniGameWrapper();
        }
    }
}

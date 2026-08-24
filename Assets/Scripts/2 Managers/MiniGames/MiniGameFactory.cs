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
            case GameType.IncineratorToss: return new IncineratorTossMiniGame();
            case GameType.CenterDiv: return new CenterDivMiniGame();
            case GameType.SizeMatch: return new SizeMatchMiniGame();
            case GameType.DragCenter: return new DragCenterMiniGame();
            case GameType.CoffeeWork:
                return new CoffeeWorkMiniGame();
            case GameType.FranticBackspace:
                return new FranticBackspaceMiniGame();
            case GameType.MashRace:
                return new MashRaceMiniGame();
            case GameType.SprintLane:
                return new SprintLaneMiniGame();
            case GameType.DocumentSigner:
                return new DocumentSignerMiniGame();
            case GameType.TimingRing:
                return new TimingRingMiniGame();
            case GameType.PumpSystem:
                return new PumpSystemMiniGame();
            case GameType.ZenTracing:
                return new ZenTracingMiniGame();
            case GameType.BalanceMeter:
                return new BalanceMeterMiniGame();
            case GameType.MemoryMatch:
                return new MemoryMatchMiniGame();
            case GameType.WhackGrid:
                return new WhackGridMiniGame();
            case GameType.Sequence:
                return new SequenceGameMiniGame();
            case GameType.SweepFloor:
                return new SweepFloorMiniGame();
            case GameType.OrderQueue:
                return new OrderQueueMiniGame();
            case GameType.FallingSort: return new FallingSortMiniGame();
            case GameType.DialogueChain:
                return new DialogueChainMiniGame();
            case GameType.UnitTestRunner:
                return new UnitTestRunnerMiniGame();
            case GameType.CodeCommenter:
                return new CodeCommenterMiniGame();
            case GameType.Swipe:
                return new SwipeGameMiniGame();
            case GameType.AutomatedTests:
                return new AutomatedTestsMiniGame();
            case GameType.ChartAdjuster:
                return new ChartAdjusterMiniGame();
                        case GameType.AcidRainShield:
                return new AcidRainShieldMiniGame();
            case GameType.CloggedVents:
                return new CloggedVentsMiniGame();
            case GameType.ContainmentProtocol:
                return new ContainmentProtocolMiniGame();
            case GameType.CorporateGolf:
                return new CorporateGolfMiniGame();
            case GameType.FoodPrepRush:
                return new FoodPrepRushMiniGame();
            case GameType.ForgePass:
                return new ForgePassMiniGame();
            case GameType.FormatDashboard:
                return new FormatDashboardMiniGame();
            case GameType.GalacticCouncil:
                return new GalacticCouncilMiniGame();
            case GameType.HospitalityGala:
                return new HospitalityGalaMiniGame();
            case GameType.HotStoneSpa:
                return new HotStoneSpaMiniGame();
            case GameType.Choice:
                return new ChoiceMiniGame();
                        case GameType.IceShatter:
                return new IceShatterMiniGame();
            case GameType.IpHack:
                return new IpHackMiniGame();
            case GameType.MowGreens:
                return new MowGreensMiniGame();
            case GameType.NameMemory:
                return new NameMemoryMiniGame();
            case GameType.NeuralAligner:
                return new NeuralAlignerMiniGame();
            case GameType.NightShiftPitch:
                return new NightShiftPitchMiniGame();
            case GameType.RecycleBottles:
                return new RecycleBottlesMiniGame();
            case GameType.RetroRacer:
                return new RetroRacerMiniGame();
            case GameType.RewardLogic:
                return new RewardLogicMiniGame();
            case GameType.SentienceArray:
                return new SentienceArrayMiniGame();
                        case GameType.SkimATM:
                return new SkimATMMiniGame();
            case GameType.SnatchBriefcase:
                return new SnatchBriefcaseMiniGame();
            case GameType.StartupPitch:
                return new StartupPitchMiniGame();
            case GameType.StealthInject:
                return new StealthInjectMiniGame();
            case GameType.TwoPhaseHeist:
                return new TwoPhaseHeistMiniGame();
            case GameType.VrArcadePong:
                return new VrArcadePongMiniGame();
            case GameType.WalkDogs:
                return new WalkDogsMiniGame();
            case GameType.WhackABug:
                return new WhackABugMiniGame();
            case GameType.YesManNod:
                return new YesManNodMiniGame();
            case GameType.CodeBreaker: return new CodeBreakerMiniGame();
            case GameType.DragMatch: return new DragMatchMiniGame();
            case GameType.ScrubPlate: return new ScrubPlateMiniGame();
            case GameType.PhysicalScrub: return new PhysicalScrubMiniGame();
            case GameType.Tetris: return new TetrisMiniGame();
            case GameType.VideoCall: return new VideoCallMiniGame();
            case GameType.KanbanBoard: return new KanbanBoardMiniGame();
            case GameType.FileSorter: return new FileSorterMiniGame();
            default:
                return new LegacyMiniGameWrapper();
        }
    }
}

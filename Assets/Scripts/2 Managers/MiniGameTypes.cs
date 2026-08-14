using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum GameType { Tap, Slider, Choice, Sequence, Swipe, MashRace, MemoryMatch, TwoPhaseHeist, TimingRing, FallingSort, GridDodge, WhackGrid, CodeBreaker, BalanceMeter, DialogueChain, OrderQueue, DragMatch, SprintLane, ScrubPlate, SweepFloor, PumpSystem, Tetris, AcidRainShield, FoodPrepRush, VideoCall, KanbanBoard, FileSorter, CoffeeWork, UnitTestRunner, CenterDiv, DragCenter, SizeMatch, StealthInject, AutomatedTests, CodeCommenter, DocumentSigner, ChartAdjuster, NeuralAligner, SentienceArray, ContainmentProtocol, RewardLogic, GalacticCouncil, FranticBackspace, FormatDashboard, NightShiftPitch, YesManNod, VrArcadePong, HotStoneSpa, HospitalityGala, StartupPitch, CorporateGolf, MowGreens, WalkDogs, ForgePass, SnatchBriefcase, SkimATM, RecycleBottles, CloggedVents, QATriage, WhackABug, ZenTracing, IceShatter, RetroRacer, TicketSwipe, ConstructionSite, NameMemory, IpHack }

public class MiniGameConfig
{
    public GameType type;
    public string instructions;
    public int targetScore;
    public float timeLimit;
    public float sliderMin;
    public float sliderMax;
    public string[] choices;
    public string correctChoice;
    public int sequenceLength;
    public int pairs;
    
    // TimingRing
    public int timingRounds = 3;
    public float timingZoneWidth = 20f; // percentage width of target zone
    // FallingSort
    public string[] fallingItems; // e.g. {"🔩 Iron", "🟤 Copper", "⬜ Aluminium"}
    public string[] binLabels;   // e.g. {"Iron Bin", "Copper Bin", "Alu Bin"}
    public int sortTarget = 10;
    public float sortTimeLimit = 20f;
    // AcidRainShield / Tetris
    public float acidRainTimeLimit = 15f;
    // KanbanBoard
    public int kanbanTicketCount = 8;
    // GridDodge
    public int gridSize = 5;
    public int gridHealth = 3;
    // WhackGrid
    public int whackTarget = 12;
    public float whackTime = 15f;
    // CodeBreaker
    public string[] codeSymbols; // available symbols
    public int codeLength = 4;
    public int maxAttempts = 6;
    // BalanceMeter
    public float balanceHoldTime = 8f; // seconds to hold in zone to win
    public float balanceDriftSpeed = 30f; // drift speed
    // DialogueChain
    public string[] dialogueScenarios;
    public string[][] dialogueOptions; // 3 options per round
    public int[] dialogueCorrectIndices; // correct option index per round (0-2)
    public float rapportThreshold = 0.7f;
    // OrderQueue  
    public string[] orderItems;
    public float orderPreviewTime = 3f;
    // DragMatch
    public string[] matchLeft;
    public string[] matchRight;
    // SprintLane
    public int sprintObstacles = 12;
    public float sprintSpeed = 1.5f; // seconds per obstacle
    // PumpSystem Extended
    public bool pumpIsBalanceMode = false;
    public float pumpBalanceTargetMin = 0.6f;
    public float pumpBalanceTargetMax = 0.8f;
    public float pumpBalanceHoldTime = 3f;
    // TwoPhase extended
    public GameType phase1Type = GameType.Sequence;
    public GameType phase2Type = GameType.MashRace;
    public GameType phase3Type = (GameType)0;

    // FileSorter
    public int fileSorterTarget = 10;
    public string[] fileSorterItems; // filenames to display
    // CoffeeWork
    public float coffeeDrainRate = 20f;
    public float coffeeBoost = 25f;
    public float workProgressPerTap = 5f;
    // UnitTestRunner
    public string[] unitTestQuestions;
    public string[][] unitTestOptions;
    public int[] unitTestCorrectIndices;
    public int unitTestRounds = 10;
    // CenterDiv
    public int centerDivTargets = 3;
    public float centerDivSpeedMin = 300f;
    public float centerDivSpeedMax = 600f;
    public float centerDivTolerance = 30f;

    // CloggedVents
    public int ventCount = 6;
    public float dustBuildRate = 10f;
    // QATriage
    public string[] qaBugLogs;
    public string[] qaCategories;
    public string[] qaSeverities;
    // RetroRacer
    public int racerObstacleCount = 20;
    public float racerSpeed = 2f;
    // IceShatter
    public int iceNodesCount = 15;
    // NameMemory
    public string[] vipNames;
    public string[] vipTraits;
}


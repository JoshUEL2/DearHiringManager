import os

content = """using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum GameType { Tap, Slider, Choice, Sequence, Swipe, MashRace, MemoryMatch, TwoPhaseHeist, TimingRing, FallingSort, GridDodge, WhackGrid, CodeBreaker, BalanceMeter, DialogueChain, OrderQueue, DragMatch, SprintLane }

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
    public string[] fallingItems; // e.g. {"\U0001f529 Iron", "\U0001f7e4 Copper", "\u2b1c Aluminium"}
    public string[] binLabels;   // e.g. {"Iron Bin", "Copper Bin", "Alu Bin"}
    public int sortTarget = 10;
    public float sortTimeLimit = 20f;
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
    // TwoPhase extended
    public GameType phase2Type = GameType.MashRace;
}

public class MiniGameManager : MonoBehaviour
{
    public static MiniGameManager Instance { get; private set; }

    [Header("UI Document")]
    public UIDocument miniGameDocument;

    private VisualElement overlay;
    private Label operationNameText, instructionsText;
    private Button btnAbort, btnCompleteTask;

    // Containers
    private VisualElement tapContainer, sliderContainer, choiceContainer, sequenceContainer, swipeContainer, mashRaceContainer, memoryMatchContainer;

    // Tap
    private Label tapProgressText;
    private Button btnActionTap;

    // Slider
    private Label sliderTargetText;
    private Slider gameSlider;
    private Button btnSubmitSlider;

    // Choice
    private Button btnChoice1, btnChoice2, btnChoice3;

    // Sequence
    private Label sequenceStatusText;
    private Button[] seqBtns = new Button[6];
    private Button btnReplayPattern;

    // Swipe
    private Label swipeObstacleText, swipeProgressText, swipeTimerText;
    private Button btnDodgeLeft, btnDodgeRight;

    // Mash Race
    private VisualElement playerProgressBar, opponentProgressBar;
    private Label mashTimerText;
    private Button btnMashTap;

    // Memory Match
    private Label matchStatusText;
    private VisualElement memoryGrid;

    // TimingRing
    private VisualElement timingRingContainer, timingGaugeTrack, timingTargetZone, timingMarker;
    private Label timingRoundText;
    private Button btnTimingStrike;

    // FallingSort
    private VisualElement fallingSortContainer, fallZone;
    private Label sortScoreText, sortTimerText, fallingItemText;
    private Button btnBin1, btnBin2, btnBin3;

    // GridDodge
    private VisualElement gridDodgeContainer, gridDodgeGrid;
    private Label gridHealthText, gridMoveText;

    // WhackGrid
    private VisualElement whackGridContainer, whackGrid;
    private Label whackScoreText, whackTimerText;

    // CodeBreaker
    private VisualElement codeBreakerContainer, codeGuessRow, codeSymbolRow, codeHistoryList;
    private Label codeAttemptsText;
    private Button btnCodeSubmit, btnCodeClear;

    // BalanceMeter
    private VisualElement balanceMeterContainer, balanceMeterTrack, balanceCenterZone, balanceNeedle, balanceTimeBar;
    private Label balanceStatusText;
    private Button btnNudgeLeft, btnNudgeRight;

    // DialogueChain
    private VisualElement dialogueChainContainer;
    private Label dialogueRoundText, dialogueScenarioText;
    private VisualElement rapportBar;
    private Button btnResponse1, btnResponse2, btnResponse3;

    // OrderQueue
    private VisualElement orderQueueContainer, orderItemGrid;
    private Label orderStatusText, orderTimerText;

    // DragMatch
    private VisualElement dragMatchContainer, dragLeftColumn, dragRightColumn;
    private Label matchPairScoreText;

    // SprintLane
    private VisualElement sprintLaneContainer, sprintLaneDisplay, sprintProgressBar;
    private Label sprintDistanceText, sprintTimerText, sprintObstacleText;
    private Button btnSprintJump, btnSprintDuck;

    // Game State
    private ActiveMiniGame currentGame;
    private MiniGameConfig currentConfig;
    private bool hasWon = false;
    private bool isPlaying = false;
    private Coroutine activeGameRoutine;

    private int score = 0;
    private float timer = 0f;
    private int currentPhase = 1;

    // Sequence State
    private List<int> currentSequence = new List<int>();
    private int sequenceInputIndex = 0;
    private bool isShowingSequence = false;

    // Swipe State
    private int expectedDodgeDir = 0; // -1 left, 1 right

    // Mash Race State
    private float opponentProgress = 0f;

    // Memory State
    private List<Button> memoryCards = new List<Button>();
    private List<string> memoryValues = new List<string>();
    private int flippedIndex1 = -1;
    private int flippedIndex2 = -1;
    private bool isCheckingMatch = false;

    // TimingRing state
    private int timingCurrentRound = 0;
    private bool timingMarkerMoving = false;
    private float timingMarkerPos = 0f;
    private float timingMarkerDir = 1f;
    private float timingMarkerSpeed = 200f;

    // FallingSort state
    private int currentFallingItemIndex = 0;
    private string[] currentFallingItems;
    private string[] currentFallingAnswers;
    private int sortScore = 0;

    // GridDodge state
    private int gridPlayerX = 0, gridPlayerY = 2;
    private int gridGoalX = 4;
    private int gridMoves = 0;
    private int gridHP = 3;
    private Button[,] gridCells;
    private HashSet<(int,int)> gridDangerCells = new HashSet<(int,int)>();

    // WhackGrid state
    private Button[] whackCells;
    private int whackScore = 0;
    private int activeWhackCell = -1;
    private bool isDecoy = false;

    // CodeBreaker state
    private string[] secretCode;
    private List<string> currentGuess = new List<string>();
    private int codeAttemptsRemaining;
    private List<Button> codeSlotButtons = new List<Button>();

    // BalanceMeter state
    private float balanceValue = 50f;
    private float balanceTimeInZone = 0f;
    private float balanceDrift = 0f;

    // DialogueChain state
    private int dialogueRound = 0;
    private float rapportValue = 0.5f;

    // OrderQueue state
    private List<string> orderCorrectSequence = new List<string>();
    private int orderInputIndex = 0;
    private bool orderIsPreview = true;

    // DragMatch state
    private int dragSelectedLeft = -1;
    private List<Button> dragLeftButtons = new List<Button>();
    private List<Button> dragRightButtons = new List<Button>();
    private int dragMatchCount = 0;
    private Dictionary<int,int> dragCorrectMap = new Dictionary<int,int>();

    // SprintLane state
    private int sprintDistance = 0;
    private bool sprintObstacleIsHigh = false;
    private bool sprintWaitingForInput = false;

    // Unique lore for all 54 tasks
    private Dictionary<string, string> taskLogMessages = new Dictionary<string, string>()
    {
        // Dishwasher
        { "Scrub dinner plates", "You blindly scrubbed dinner plates because your boss barked orders at you." },
        { "Sweep kitchen floors", "You swept the grimy kitchen floors, trying to ignore the roaches." },
        { "Mop grease spills", "You mopped up hazardous grease spills without protective gear." },
        { "Scour microwave grease", "You felt the need to prove yourself and scoured carcinogenic microwave grease." },
        { "Deep scrub waste containers", "You went above and beyond, deep scrubbing the biohazardous waste containers." },
        { "Clean fryer grease vats", "You took the initiative to clean the blistering hot fryer grease vats." },

        // Scrap Metal Sorter
        { "Sort iron from copper", "You manually sorted jagged iron from copper scraps under flickering lights." },
        { "Stack rusty steel pipes", "You strained your back stacking incredibly heavy, rusty steel pipes." },
        { "Clear conveyor dust filters", "You inhaled metallic dust while clearing the blocked conveyor filters." },
        { "Inspect lithium reactor tubes", "You felt the need to prove yourself and inspected the highly unstable lithium reactor tubes." },
        { "Carry tungsten girders", "You pushed past your physical limits carrying massive tungsten girders." },
        { "Audit sewage drainage lines", "You waded into the dark to audit the foul-smelling sewage drainage lines." },

        // Street Corner Sign Spinner
        { "Spin arrow sign in traffic", "You spun a holographic arrow sign in smoggy traffic because your manager demanded it." },
        { "Dance with sign for tourists", "You humiliated yourself dancing with a promotional sign for oblivious tourists." },
        { "Wipe smog off the neon arrow", "You stood on a crate to wipe toxic smog off the flickering neon arrow." },
        { "Perform a triple backflip spin", "You felt the need to prove yourself and performed a triple backflip spin." },
        { "Hold sign in acid rain storm", "You braved the acid rain storm, holding the sign high above your head." },
        { "Fight rival sign spinner", "You asserted dominance by fighting a rival sign spinner for the best corner." },

        // Fast Food Shift Manager
        { "Check fryer oil temperature", "You nearly burned yourself checking the volatile fryer oil temperature." },
        { "Restock condiment stations", "You frantically restock the condiment stations during the lunch rush." },
        { "Handle customer complaint", "You patiently absorbed the verbal abuse from an angry customer." },
        { "Emergency food prep rush", "You stepped up to lead the kitchen during an emergency food prep rush." },
        { "Deep clean grease traps", "You got your hands dirty deep cleaning the horrifying grease traps." },
        { "Train new hire on POS system", "You took the time to train a terrified new hire on the outdated POS system." },

        // Corporate Cubicle Drone
        { "Send weekly TPS reports", "You successfully navigated the bureaucracy and sent your weekly TPS reports." },
        { "Attend 3-hour video call", "You stared blankly at your screen during a mandatory 3-hour video call." },
        { "Reorganize shared drive", "You lost hours reorganizing the chaotic corporate shared drive." },
        { "Corporate sprint tasks", "You slammed through corporate sprint tasks to impress upper management." },
        { "Overnight marketing pitch", "You pulled an all-nighter crafting a soulless marketing pitch." },
        { "Agree with executive complaints", "You strategically nodded along with executive complaints during a luncheon." },

        // Junior React Trainee
        { "Write unit tests", "You wrote dozens of unit tests for legacy code nobody understands." },
        { "Fix CSS layout sizes", "You banged your head against the desk fixing impossible CSS layout sizes." },
        { "Center content icons", "You spent hours figuring out how to vertically center content icons." },
        { "Merge divs on production", "You fearlessly merged complex divs directly on the production server." },
        { "Debug CSS at 3 AM", "You ruined your sleep schedule to debug CSS at 3 AM." },
        { "Inject untested code", "You confidently injected untested code to meet a brutal deadline." },

        // Dystopian Quality Assurance Tester
        { "Run automated test suite", "You watched the automated test suite fail, then run successfully on the second try." },
        { "File bug report", "You filed a detailed bug report that will likely be ignored." },
        { "Verify hotfix deployment", "You frantically verified the hotfix deployment before the CEO noticed." },
        { "Regression test full app", "You meticulously regression tested the full app, finding 40 new issues." },
        { "Stress test server load", "You purposely crashed the server to stress test its load capacity." },
        { "Write flaky test workaround", "You wrote a questionable workaround for a notoriously flaky test." },

        // Executive Interstellar CEO
        { "Approve quarterly budget", "You heartlessly approved a quarterly budget that slashed employee benefits." },
        { "Board meeting presentation", "You delivered a flawless board meeting presentation filled with buzzwords." },
        { "Sign regulatory compliance docs", "You forged a few signatures on the regulatory compliance docs." },
        { "Hostile takeover negotiations", "You ruthlessly crushed the competition in hostile takeover negotiations." },
        { "Fire underperforming VP", "You called security to escort the underperforming VP out of the building." },
        { "Interstellar colony site inspection", "You survived a grueling interstellar colony site inspection." },

        // AI Prophecy Alignment Director
        { "Calibrate neural alignment matrix", "You successfully calibrated the incredibly sensitive neural alignment matrix." },
        { "Review AI ethics proposals", "You rubber-stamped highly questionable AI ethics proposals." },
        { "Monitor sentience detection array", "You closely monitored the sentience detection array for anomalies." },
        { "Emergency AI containment protocol", "You activated the emergency AI containment protocol just in time." },
        { "Align superintelligence reward function", "You successfully aligned the superintelligence reward function, probably." },
        { "Present findings to Galactic Council", "You presented your terrifying findings to the Galactic Council." },

        // Fallbacks
        { "Respond to supervisor", "You responded politely to your supervisor's unreasonable demands." },
        { "Categorize ticket logs", "You mindlessly categorized endless support ticket logs." },
        { "Format summary dashboards", "You tweaked the colors on the summary dashboards until they were barely readable." },
        { "Do corporate sprint tasks aggressively", "You aggressively clicked through the corporate sprint tasks." },
        { "Stay overnight to align marketing pitch variables", "You stayed overnight trying to align variables for a marketing pitch." },
        { "Agree verbally with automated executive complaints", "You nodded verbally to the automated voice complaints of an executive." },

        // Wellness
        { "Walk in the Park", "You took a surprisingly pleasant walk in the designated safety park." },
        { "Capsule Arcade VR", "You escaped into the Capsule Arcade VR to forget reality for a bit." },
        { "Premium Oxygen Bar", "You cleared your lungs with synthetic air at the Premium Oxygen Bar." },
        { "Suburban Sensory Mud Spa", "You cleansed your pores at the exclusive Suburban Sensory Mud Spa." },
        { "Interstellar Cryo Resort", "You truly relaxed by freezing time at the Interstellar Cryo Resort." },

        // Mixers
        { "Industrial Dive Bar", "You bought drinks for greasy foremen at the Industrial Dive Bar." },
        { "Hospitality Gala", "You awkwardly mingled with exhausted managers at the Hospitality Gala." },
        { "Underground Tech Meetup", "You traded USB drives and rumors at the Underground Tech Meetup." },
        { "Corporate Golf Sim", "You purposefully lost matches at the Corporate Golf Sim to impress execs." },

        // Hustles
        { "Collect Recycled Bottles", "I scoured the bins and collected a bag full of sticky bottles." },
        { "Mow Corporate Astroturf", "I trimmed the plastic lawns for the corporate elites." },
        { "Walk Robotic Pedigree Dogs", "I was dragged down the street by a malfunctioning mechanical poodle." },
        { "Deliver Fast Food Order", "I dodged incoming traffic to deliver a cold burger." },
        { "Forge Transit Passes", "I hacked the metro gate network to print fake transit passes." },
        { "Snatch Luxury Briefcase", "I grabbed a shiny briefcase from a distracted suit and ran." },
        { "ATM Autonomous Hijack", "I injected a worm into the Slum ATM and watched the cash spit out." },
        { "Corporate Espionage Heist", "I downloaded the proprietary blueprints before the security drones arrived." }
    };

    private Dictionary<string, MiniGameConfig> gameConfigs;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;

        InitializeConfigs();
    }

    private void Start()
    {
        var root = miniGameDocument.rootVisualElement;
        root.pickingMode = PickingMode.Ignore;

        overlay = root.Q<VisualElement>("Overlay");
        operationNameText = root.Q<Label>("OperationNameText");
        instructionsText = root.Q<Label>("InstructionsText");

        // Containers
        tapContainer = root.Q<VisualElement>("TapGameContainer");
        sliderContainer = root.Q<VisualElement>("SliderGameContainer");
        choiceContainer = root.Q<VisualElement>("ChoiceGameContainer");
        sequenceContainer = root.Q<VisualElement>("SequenceGameContainer");
        swipeContainer = root.Q<VisualElement>("SwipeGameContainer");
        mashRaceContainer = root.Q<VisualElement>("MashRaceContainer");
        memoryMatchContainer = root.Q<VisualElement>("MemoryMatchContainer");

        // New Containers
        timingRingContainer = root.Q<VisualElement>("TimingRingContainer");
        fallingSortContainer = root.Q<VisualElement>("FallingSortContainer");
        gridDodgeContainer = root.Q<VisualElement>("GridDodgeContainer");
        whackGridContainer = root.Q<VisualElement>("WhackGridContainer");
        codeBreakerContainer = root.Q<VisualElement>("CodeBreakerContainer");
        balanceMeterContainer = root.Q<VisualElement>("BalanceMeterContainer");
        dialogueChainContainer = root.Q<VisualElement>("DialogueChainContainer");
        orderQueueContainer = root.Q<VisualElement>("OrderQueueContainer");
        dragMatchContainer = root.Q<VisualElement>("DragMatchContainer");
        sprintLaneContainer = root.Q<VisualElement>("SprintLaneContainer");

        // Tap
        tapProgressText = root.Q<Label>("TapProgressText");
        btnActionTap = root.Q<Button>("BtnActionTap");
        btnActionTap.clicked += OnTapClicked;

        // Slider
        sliderTargetText = root.Q<Label>("SliderTargetText");
        gameSlider = root.Q<Slider>("GameSlider");
        btnSubmitSlider = root.Q<Button>("BtnSubmitSlider");
        btnSubmitSlider.clicked += OnSliderSubmitted;
        gameSlider.RegisterValueChangedCallback(ev => UpdateSliderText(ev.newValue));

        // Choice
        btnChoice1 = root.Q<Button>("BtnChoice1");
        btnChoice2 = root.Q<Button>("BtnChoice2");
        btnChoice3 = root.Q<Button>("BtnChoice3");
        btnChoice1.clicked += () => OnChoiceClicked(btnChoice1.text);
        btnChoice2.clicked += () => OnChoiceClicked(btnChoice2.text);
        btnChoice3.clicked += () => OnChoiceClicked(btnChoice3.text);

        // Sequence
        sequenceStatusText = root.Q<Label>("SequenceStatusText");
        for (int i = 0; i < 6; i++)
        {
            int idx = i;
            seqBtns[i] = root.Q<Button>($"SeqBtn{i}");
            seqBtns[i].clicked += () => OnSequenceButtonClicked(idx);
        }
        btnReplayPattern = root.Q<Button>("BtnReplayPattern");
        btnReplayPattern.clicked += ReplaySequence;

        // Swipe
        swipeObstacleText = root.Q<Label>("SwipeObstacleText");
        swipeProgressText = root.Q<Label>("SwipeProgressText");
        swipeTimerText = root.Q<Label>("SwipeTimerText");
        btnDodgeLeft = root.Q<Button>("BtnDodgeLeft");
        btnDodgeRight = root.Q<Button>("BtnDodgeRight");
        btnDodgeLeft.clicked += () => OnDodgeClicked(-1);
        btnDodgeRight.clicked += () => OnDodgeClicked(1);

        // Mash Race
        playerProgressBar = root.Q<VisualElement>("PlayerProgressBar");
        opponentProgressBar = root.Q<VisualElement>("OpponentProgressBar");
        mashTimerText = root.Q<Label>("MashTimerText");
        btnMashTap = root.Q<Button>("BtnMashTap");
        btnMashTap.clicked += OnMashTapClicked;

        // Memory Match
        matchStatusText = root.Q<Label>("MatchStatusText");
        memoryGrid = root.Q<VisualElement>("MemoryGrid");

        // TimingRing
        timingGaugeTrack = root.Q<VisualElement>("TimingGaugeTrack");
        timingTargetZone = root.Q<VisualElement>("TimingTargetZone");
        timingMarker = root.Q<VisualElement>("TimingMarker");
        timingRoundText = root.Q<Label>("TimingRoundText");
        btnTimingStrike = root.Q<Button>("BtnTimingStrike");
        if (btnTimingStrike != null) btnTimingStrike.clicked += OnTimingStrike;

        // FallingSort
        fallZone = root.Q<VisualElement>("FallZone");
        sortScoreText = root.Q<Label>("SortScoreText");
        sortTimerText = root.Q<Label>("SortTimerText");
        fallingItemText = root.Q<Label>("FallingItemText");
        btnBin1 = root.Q<Button>("BtnBin1");
        btnBin2 = root.Q<Button>("BtnBin2");
        btnBin3 = root.Q<Button>("BtnBin3");
        if (btnBin1 != null) btnBin1.clicked += () => OnSortBinClicked(0);
        if (btnBin2 != null) btnBin2.clicked += () => OnSortBinClicked(1);
        if (btnBin3 != null) btnBin3.clicked += () => OnSortBinClicked(2);

        // GridDodge
        gridDodgeGrid = root.Q<VisualElement>("GridDodgeGrid");
        gridHealthText = root.Q<Label>("GridHealthText");
        gridMoveText = root.Q<Label>("GridMoveText");

        // WhackGrid
        whackGrid = root.Q<VisualElement>("WhackGrid");
        whackScoreText = root.Q<Label>("WhackScoreText");
        whackTimerText = root.Q<Label>("WhackTimerText");

        // CodeBreaker
        codeGuessRow = root.Q<VisualElement>("CodeGuessRow");
        codeSymbolRow = root.Q<VisualElement>("CodeSymbolRow");
        codeHistoryList = root.Q<VisualElement>("CodeHistoryList");
        codeAttemptsText = root.Q<Label>("CodeAttemptsText");
        btnCodeSubmit = root.Q<Button>("BtnCodeSubmit");
        btnCodeClear = root.Q<Button>("BtnCodeClear");
        if (btnCodeSubmit != null) btnCodeSubmit.clicked += OnCodeSubmit;
        if (btnCodeClear != null) btnCodeClear.clicked += OnCodeClear;

        // BalanceMeter
        balanceMeterTrack = root.Q<VisualElement>("BalanceMeterTrack");
        balanceCenterZone = root.Q<VisualElement>("BalanceCenterZone");
        balanceNeedle = root.Q<VisualElement>("BalanceNeedle");
        balanceTimeBar = root.Q<VisualElement>("BalanceTimeBar");
        balanceStatusText = root.Q<Label>("BalanceStatusText");
        btnNudgeLeft = root.Q<Button>("BtnNudgeLeft");
        btnNudgeRight = root.Q<Button>("BtnNudgeRight");
        if (btnNudgeLeft != null) btnNudgeLeft.clicked += () => OnBalanceNudge(-10f);
        if (btnNudgeRight != null) btnNudgeRight.clicked += () => OnBalanceNudge(10f);

        // DialogueChain
        dialogueRoundText = root.Q<Label>("DialogueRoundText");
        dialogueScenarioText = root.Q<Label>("DialogueScenarioText");
        rapportBar = root.Q<VisualElement>("RapportBar");
        btnResponse1 = root.Q<Button>("BtnResponse1");
        btnResponse2 = root.Q<Button>("BtnResponse2");
        btnResponse3 = root.Q<Button>("BtnResponse3");
        if (btnResponse1 != null) btnResponse1.clicked += () => OnDialogueResponse(0);
        if (btnResponse2 != null) btnResponse2.clicked += () => OnDialogueResponse(1);
        if (btnResponse3 != null) btnResponse3.clicked += () => OnDialogueResponse(2);

        // OrderQueue
        orderItemGrid = root.Q<VisualElement>("OrderItemGrid");
        orderStatusText = root.Q<Label>("OrderStatusText");
        orderTimerText = root.Q<Label>("OrderTimerText");

        // DragMatch
        dragLeftColumn = root.Q<VisualElement>("DragLeftColumn");
        dragRightColumn = root.Q<VisualElement>("DragRightColumn");
        matchPairScoreText = root.Q<Label>("MatchPairScoreText");

        // SprintLane
        sprintLaneDisplay = root.Q<VisualElement>("SprintLaneDisplay");
        sprintProgressBar = root.Q<VisualElement>("SprintProgressBar");
        sprintDistanceText = root.Q<Label>("SprintDistanceText");
        sprintTimerText = root.Q<Label>("SprintTimerText");
        sprintObstacleText = root.Q<Label>("SprintObstacleText");
        btnSprintJump = root.Q<Button>("BtnSprintJump");
        btnSprintDuck = root.Q<Button>("BtnSprintDuck");
        if (btnSprintJump != null) btnSprintJump.clicked += () => OnSprintReact(true);
        if (btnSprintDuck != null) btnSprintDuck.clicked += () => OnSprintReact(false);

        // Footer
        btnAbort = root.Q<Button>("BtnAbort");
        btnAbort.clicked += AbortGame;
        btnCompleteTask = root.Q<Button>("BtnCompleteTask");
        btnCompleteTask.clicked += FinishGame;

        overlay.style.display = DisplayStyle.None;
        overlay.pickingMode = PickingMode.Ignore;
    }

    private void InitializeConfigs()
    {
        gameConfigs = new Dictionary<string, MiniGameConfig>();

        // =============================================
        // DISHWASHER (6 tasks)
        // =============================================
        AddConfigFull("Scrub dinner plates", new MiniGameConfig { type = GameType.WhackGrid, whackTarget = 12, whackTime = 15f, instructions = "Whack the dirty plates! Avoid the clean ones!" });
        AddConfigFull("Sweep kitchen floors", new MiniGameConfig { type = GameType.GridDodge, gridSize = 5, gridHealth = 3, instructions = "Sweep your way to the goal, avoid dirt spots!" });
        AddConfigFull("Mop grease spills", new MiniGameConfig { type = GameType.BalanceMeter, balanceHoldTime = 8f, balanceDriftSpeed = 30f, instructions = "Keep the mop balanced on the grease spill!" });
        AddConfigFull("Scour microwave grease", new MiniGameConfig { type = GameType.WhackGrid, whackTarget = 15, whackTime = 12f, instructions = "Fast! Scour the grease bubbles!" });
        AddConfigFull("Deep scrub waste containers", new MiniGameConfig { type = GameType.OrderQueue, orderItems = new string[]{"Biohazard","Chemical","Food","General","Recycling"}, orderPreviewTime = 3f, instructions = "Clean them in the exact right order!" });
        AddConfigFull("Clean fryer grease vats", new MiniGameConfig { type = GameType.TimingRing, timingRounds = 4, timingZoneWidth = 18f, instructions = "Time your scrubbing to avoid splashing!" });

        // =============================================
        // SCRAP METAL SORTER (6 tasks)
        // =============================================
        AddConfigFull("Sort iron from copper", new MiniGameConfig { type = GameType.FallingSort, fallingItems = new string[]{"\U0001f529 Iron","\U0001f7e4 Copper","\u2b1c Aluminium"}, binLabels = new string[]{"Iron","Copper","Aluminium"}, sortTarget = 12, sortTimeLimit = 20f, instructions = "Sort falling metal scraps!" });
        AddConfigFull("Stack rusty steel pipes", new MiniGameConfig { type = GameType.TimingRing, timingRounds = 5, timingZoneWidth = 20f, instructions = "Strike to stack pipes perfectly!" });
        AddConfigFull("Clear conveyor dust filters", new MiniGameConfig { type = GameType.BalanceMeter, balanceHoldTime = 10f, balanceDriftSpeed = 35f, instructions = "Keep dust extraction balanced!" });
        AddConfigFull("Inspect lithium reactor tubes", new MiniGameConfig { type = GameType.DragMatch, matchLeft = new string[]{"Tube A","Tube B","Tube C","Tube D","Tube E"}, matchRight = new string[]{"Critical","Warning","Stable","Optimal","Offline"}, instructions = "Match tubes to statuses!" });
        AddConfigFull("Carry tungsten girders", new MiniGameConfig { type = GameType.SprintLane, sprintObstacles = 12, sprintSpeed = 1.5f, instructions = "Sprint and dodge with heavy girders!" });
        AddConfigFull("Audit sewage drainage lines", new MiniGameConfig { type = GameType.OrderQueue, orderItems = new string[]{"Valve 1","Valve 2","Valve 3","Valve 4","Valve 5"}, orderPreviewTime = 3f, instructions = "Open valves in sequence!" });

        // =============================================
        // STREET CORNER SIGN SPINNER (6 tasks)
        // =============================================
        AddConfigFull("Spin arrow sign in traffic", new MiniGameConfig { type = GameType.TimingRing, timingRounds = 5, timingZoneWidth = 16f, instructions = "Hit the timing zones to perform spins!" });
        AddConfigFull("Dance with sign for tourists", new MiniGameConfig { type = GameType.OrderQueue, orderItems = new string[]{"Left","Right","Spin","Pose","Wave","Dip"}, orderPreviewTime = 3f, instructions = "Perform the full dance routine!" });
        AddConfigFull("Wipe smog off the neon arrow", new MiniGameConfig { type = GameType.BalanceMeter, balanceHoldTime = 8f, balanceDriftSpeed = 28f, instructions = "Apply even pressure to wipe!" });
        AddConfig("Perform a triple backflip spin", GameType.Sequence, "Execute the acrobatic sequence perfectly!", 0, 0, 0, 0, null, null, null, null, 6);
        AddConfigFull("Hold sign in acid rain storm", new MiniGameConfig { type = GameType.SprintLane, sprintObstacles = 15, sprintSpeed = 1.2f, instructions = "Dodge the acid rain drops!" });
        AddConfig("Fight rival sign spinner", GameType.MashRace, "Out-spin your rival! Mash to assert dominance!", 35, 12f);

        // =============================================
        // FAST FOOD SHIFT MANAGER (6 tasks)
        // =============================================
        AddConfigFull("Check fryer oil temperature", new MiniGameConfig { type = GameType.TimingRing, timingRounds = 4, timingZoneWidth = 12f, instructions = "Measure temp at the exact right time!" });
        AddConfigFull("Restock condiment stations", new MiniGameConfig { type = GameType.FallingSort, fallingItems = new string[]{"\U0001f7e1 Mustard","\U0001f534 Ketchup","\u2b1c Mayo","\U0001f7e2 Relish"}, binLabels = new string[]{"Station A","Station B","Station C"}, sortTarget = 10, sortTimeLimit = 18f, instructions = "Restock the falling condiments!" });
        AddConfigFull("Handle customer complaint", new MiniGameConfig { type = GameType.DialogueChain, rapportThreshold = 0.75f, dialogueScenarios = new string[]{"Customer: My chips are COLD!", "Customer: I WAITED 20 MINUTES!", "Customer: There's a HAIR in my burger!", "Customer: I want to speak to YOUR MANAGER!"}, dialogueOptions = new string[][]{new string[]{"I sincerely apologise","That's not my problem","Have you tried heating them up?"},new string[]{"I'm so sorry for the wait","We're understaffed","You should've ordered online"},new string[]{"That's horrifying, let me replace it","Are you sure it's not yours?","We use synthetic ingredients"},new string[]{"I AM the manager, let me help","They're not available","Go ahead, they'll say the same"}}, dialogueCorrectIndices = new int[]{0,0,0,0}, instructions = "De-escalate the complaint!" });
        AddConfigFull("Emergency food prep rush", new MiniGameConfig { type = GameType.WhackGrid, whackTarget = 15, whackTime = 15f, instructions = "Prep food fast!" });
        AddConfigFull("Deep clean grease traps", new MiniGameConfig { type = GameType.WhackGrid, whackTarget = 18, whackTime = 15f, instructions = "Scrub those grease traps!" });
        AddConfigFull("Train new hire on POS system", new MiniGameConfig { type = GameType.OrderQueue, orderItems = new string[]{"Login","Select Menu","Add Items","Apply Discount","Process Payment","Print Receipt"}, orderPreviewTime = 3f, instructions = "Show them the POS steps!" });

        // =============================================
        // CORPORATE CUBICLE DRONE (6 tasks)
        // =============================================
        AddConfigFull("Send weekly TPS reports", new MiniGameConfig { type = GameType.CodeBreaker, codeSymbols = new string[]{"A","B","C","D","E","F"}, codeLength = 4, maxAttempts = 6, instructions = "Crack the system password to send reports!" });
        AddConfigFull("Attend 3-hour video call", new MiniGameConfig { type = GameType.BalanceMeter, balanceHoldTime = 12f, balanceDriftSpeed = 40f, instructions = "Keep your focus balanced!" });
        AddConfigFull("Reorganize shared drive", new MiniGameConfig { type = GameType.DragMatch, matchLeft = new string[]{"Q3_Budget","Invoice_Oct","Team_Memo","Sales_Report","Deck_v4","Expense_Log"}, matchRight = new string[]{"Finance","Accounting","Internal","Sales","Marketing","Admin"}, instructions = "Drag files to folders!" });
        AddConfigFull("Corporate sprint tasks", new MiniGameConfig { type = GameType.WhackGrid, whackTarget = 20, whackTime = 15f, instructions = "Whack the urgent tasks!" });
        AddConfigFull("Overnight marketing pitch", new MiniGameConfig { type = GameType.TimingRing, timingRounds = 5, timingZoneWidth = 14f, instructions = "Nail the pitch timing!" });
        AddConfigFull("Agree with executive complaints", new MiniGameConfig { type = GameType.DialogueChain, rapportThreshold = 0.8f, dialogueScenarios = new string[]{"VP: Q3 numbers are DISASTROUS!", "VP: Marketing spent HOW MUCH?!", "VP: Why can't we be more like Apple?!"}, dialogueOptions = new string[][]{new string[]{"You're absolutely right, sir","Actually the data shows...","That's above my pay grade"},new string[]{"Outrageous, I agree completely","It was within budget actually","I'll look into it"},new string[]{"Brilliant observation, sir","We have different strengths","I'll draft a proposal"}}, dialogueCorrectIndices = new int[]{0,0,0}, instructions = "Agree enthusiastically!" });

        // =============================================
        // JUNIOR REACT TRAINEE (6 tasks)
        // =============================================
        AddConfigFull("Write unit tests", new MiniGameConfig { type = GameType.WhackGrid, whackTarget = 15, whackTime = 12f, instructions = "Catch the bugs with tests!" });
        AddConfigFull("Fix CSS layout sizes", new MiniGameConfig { type = GameType.BalanceMeter, balanceHoldTime = 10f, balanceDriftSpeed = 35f, instructions = "Balance the layout pixels!" });
        AddConfigFull("Center content icons", new MiniGameConfig { type = GameType.TimingRing, timingRounds = 5, timingZoneWidth = 8f, instructions = "Try to center perfectly!" });
        AddConfigFull("Merge divs on production", new MiniGameConfig { type = GameType.CodeBreaker, codeSymbols = new string[]{"{","}","<",">","=","/"}, codeLength = 4, maxAttempts = 6, instructions = "Fix the merge conflicts!" });
        AddConfigFull("Debug CSS at 3 AM", new MiniGameConfig { type = GameType.GridDodge, gridSize = 5, gridHealth = 3, instructions = "Navigate through CSS hell!" });
        AddConfigFull("Inject untested code", new MiniGameConfig { type = GameType.OrderQueue, orderItems = new string[]{"Pull","Branch","Code","Test","Stage","Push"}, orderPreviewTime = 3f, instructions = "Follow the CI/CD pipeline!" });

        // =============================================
        // DYSTOPIAN QA TESTER (6 tasks)
        // =============================================
        AddConfigFull("Run automated test suite", new MiniGameConfig { type = GameType.SprintLane, sprintObstacles = 15, sprintSpeed = 1.3f, instructions = "Sprint through the test suite!" });
        AddConfigFull("File bug report", new MiniGameConfig { type = GameType.FallingSort, fallingItems = new string[]{"\U0001f534 Critical","\U0001f7e1 Major","\U0001f7e2 Minor"}, binLabels = new string[]{"Critical","Major","Minor"}, sortTarget = 10, sortTimeLimit = 15f, instructions = "Triage the bugs fast!" });
        AddConfigFull("Verify hotfix deployment", new MiniGameConfig { type = GameType.OrderQueue, orderItems = new string[]{"Build","Unit Test","Integration","Stage Deploy","Smoke Test","Prod Deploy"}, orderPreviewTime = 3f, instructions = "Run through deployment stages!" });
        AddConfigFull("Regression test full app", new MiniGameConfig { type = GameType.DragMatch, matchLeft = new string[]{"Login","Cart","Search","Profile","Payment","Settings"}, matchRight = new string[]{"Auth Tests","E-Commerce Tests","Query Tests","User Tests","Billing Tests","Config Tests"}, instructions = "Map features to tests!" });
        AddConfig("Stress test server load", GameType.MashRace, "Hammer the server! Mash faster!", 40, 15f);
        AddConfigFull("Write flaky test workaround", new MiniGameConfig { type = GameType.TimingRing, timingRounds = 4, timingZoneWidth = 12f, instructions = "Time the retry loop perfectly!" });

        // =============================================
        // EXECUTIVE INTERSTELLAR CEO (6 tasks)
        // =============================================
        AddConfigFull("Approve quarterly budget", new MiniGameConfig { type = GameType.BalanceMeter, balanceHoldTime = 12f, balanceDriftSpeed = 45f, instructions = "Keep the shareholders balanced!" });
        AddConfigFull("Board meeting presentation", new MiniGameConfig { type = GameType.OrderQueue, orderItems = new string[]{"Intro","Market Analysis","Q3 Data","Projections","Strategy","Close"}, orderPreviewTime = 3f, instructions = "Deliver the presentation slides!" });
        AddConfigFull("Sign regulatory compliance docs", new MiniGameConfig { type = GameType.DialogueChain, rapportThreshold = 0.75f, dialogueScenarios = new string[]{"Legal: Clause 4.7 requires data residency","Legal: Section 12 has liability gaps","Legal: GDPR compliance needs updating","Legal: Sign here to accept all terms"}, dialogueOptions = new string[][]{new string[]{"Flag for legal review","Sign it anyway","Delegate to VP"},new string[]{"Request amendment","Ignore the gaps","That's fine"},new string[]{"Engage compliance team","We'll deal with it later","What's GDPR?"},new string[]{"Review one more time","Sign immediately","Have my assistant sign"}}, dialogueCorrectIndices = new int[]{0,0,0,0}, instructions = "Handle compliance carefully!" });
        AddConfigFull("Hostile takeover negotiations", new MiniGameConfig { type = GameType.DialogueChain, rapportThreshold = 0.8f, dialogueScenarios = new string[]{"Their CEO: Our company isn't for sale","Their CEO: Your offer is insulting","Board: The shareholders are nervous","Media: Is this a hostile takeover?","Their CEO: Final counteroffer - $2B"}, dialogueOptions = new string[][]{new string[]{"Everyone has a price","Let's discuss the benefits","We'll go directly to shareholders"},new string[]{"We'll raise to fair market value","Take it or leave it","Let's find common ground"},new string[]{"This acquisition creates value","Shareholders should trust us","We'll sweeten the deal"},new string[]{"It's a strategic partnership","No comment","We're creating jobs"},new string[]{"Deal. Let's close this","We can go higher","Let me consult my board"}}, dialogueCorrectIndices = new int[]{1,0,0,0,0}, instructions = "Negotiate the takeover!" });
        AddConfigFull("Fire underperforming VP", new MiniGameConfig { type = GameType.DialogueChain, rapportThreshold = 0.7f, dialogueScenarios = new string[]{"VP: I've been with the company 15 years!","VP: My team loves me!","VP: What about my stock options?"}, dialogueOptions = new string[][]{new string[]{"And we value that. Here's a generous package","That's exactly the problem","Security will escort you out"},new string[]{"They'll be taken care of","They'll manage without you","We're restructuring the team"},new string[]{"They'll vest on schedule","Forfeited as per contract","We'll negotiate"}}, dialogueCorrectIndices = new int[]{0,0,0}, instructions = "Fire them smoothly!" });
        AddConfigFull("Interstellar colony site inspection", new MiniGameConfig { type = GameType.GridDodge, gridSize = 5, gridHealth = 3, instructions = "Navigate the toxic site!" });

        // =============================================
        // AI PROPHECY ALIGNMENT DIRECTOR (6 tasks)
        // =============================================
        AddConfigFull("Calibrate neural alignment matrix", new MiniGameConfig { type = GameType.BalanceMeter, balanceHoldTime = 15f, balanceDriftSpeed = 50f, instructions = "CRITICAL: Balance the alignment!" });
        AddConfigFull("Review AI ethics proposals", new MiniGameConfig { type = GameType.DialogueChain, rapportThreshold = 0.8f, dialogueScenarios = new string[]{"Proposal: Give AI emotional awareness","Proposal: AI self-modification capabilities","Proposal: Autonomous weapons systems","Proposal: AI voting rights","Proposal: Merge human consciousness with AI"}, dialogueOptions = new string[][]{new string[]{"Approve with strict monitoring","Reject — too dangerous","Classify top-secret"},new string[]{"Reject — existential risk","Approve if sandboxed","Needs more research"},new string[]{"Absolutely not","Only for defense","Approve with oversight"},new string[]{"Premature but worth studying","Approve immediately","Reject — they're not people"},new string[]{"Fund research cautiously","Too sci-fi, reject","Approve clinical trials"}}, dialogueCorrectIndices = new int[]{0,0,0,0,0}, instructions = "Judge AI proposals!" });
        AddConfigFull("Monitor sentience detection array", new MiniGameConfig { type = GameType.CodeBreaker, codeSymbols = new string[]{"\u03b1","\u03b2","\u03b3","\u03b4","\u03b5","\u03b6"}, codeLength = 4, maxAttempts = 6, instructions = "Decode sentience patterns!" });
        AddConfigFull("Emergency AI containment protocol", new MiniGameConfig { type = GameType.TwoPhaseHeist, sequenceLength = 5, timeLimit = 12f, targetScore = 30, phase2Type = GameType.MashRace, instructions = "Phase 1: Input Code! Phase 2: Contain!" });
        AddConfigFull("Align superintelligence reward function", new MiniGameConfig { type = GameType.TimingRing, timingRounds = 5, timingZoneWidth = 4f, instructions = "CRITICAL: Time the alignment exactly!" });
        AddConfigFull("Present findings to Galactic Council", new MiniGameConfig { type = GameType.OrderQueue, orderItems = new string[]{"Greeting","Context","Data","Risks","Proposal","Vote"}, orderPreviewTime = 2.5f, instructions = "Present facts in order!" });

        // =============================================
        // FALLBACK TASKS (6 tasks)
        // =============================================
        AddConfig("Respond to supervisor", GameType.Choice, "Your supervisor has sent an urgent message. How do you respond?", 0, 0, 0, 0, "Reply immediately", "Wait an hour", "Forward to colleague", "Reply immediately");
        AddConfigFull("Categorize ticket logs", new MiniGameConfig { type = GameType.FallingSort, fallingItems = new string[]{"\U0001f534 Bug","\U0001f7e2 Feature","\U0001f7e1 Complaint"}, binLabels = new string[]{"Bugs","Features","Complaints"}, sortTarget = 8, sortTimeLimit = 15f, instructions = "Sort incoming tickets!" });
        AddConfig("Format summary dashboards", GameType.Slider, "Adjust the chart scale — too zoomed in hides trends, too zoomed out hides details!", 0, 0, 42f, 58f);
        AddConfig("Do corporate sprint tasks aggressively", GameType.Tap, "Sprint! Tap as fast as you can to clear the backlog!", 25);
        AddConfigFull("Stay overnight to align marketing pitch variables", new MiniGameConfig { type = GameType.TimingRing, timingRounds = 4, timingZoneWidth = 14f, instructions = "Nail the variables!" });
        AddConfig("Agree verbally with automated executive complaints", GameType.Choice, "The automated exec is complaining. What do you say?", 0, 0, 0, 0, "'Yes, absolutely right'", "'I'll look into it'", "'That's not my department'", "'Yes, absolutely right'");

        // =============================================
        // WELLNESS ACTIVITIES (5 tasks)
        // =============================================
        AddConfigFull("Walk in the Park", new MiniGameConfig { type = GameType.GridDodge, gridSize = 5, gridHealth = 5, instructions = "A peaceful walk." });
        AddConfigFull("Capsule Arcade VR", new MiniGameConfig { type = GameType.OrderQueue, orderItems = new string[]{"\u2b06","\u2b07","\u25c0","\u25b6"}, orderPreviewTime = 3f, instructions = "Play the arcade game!" });
        AddConfigFull("Premium Oxygen Bar", new MiniGameConfig { type = GameType.TimingRing, timingRounds = 3, timingZoneWidth = 30f, instructions = "Breathe in deeply!" });
        AddConfigFull("Suburban Sensory Mud Spa", new MiniGameConfig { type = GameType.DragMatch, matchLeft = new string[]{"Hot Stones","Mud Wrap","Aromatherapy","Sound Bath"}, matchRight = new string[]{"Root","Heart","Crown","Third Eye"}, instructions = "Match treatments to chakras!" });
        AddConfigFull("Interstellar Cryo Resort", new MiniGameConfig { type = GameType.BalanceMeter, balanceHoldTime = 8f, balanceDriftSpeed = 20f, instructions = "Drift into cryo sleep..." });

        // =============================================
        // MIXERS (4 tasks)
        // =============================================
        AddConfigFull("Industrial Dive Bar", new MiniGameConfig { type = GameType.DialogueChain, rapportThreshold = 0.6f, dialogueScenarios = new string[]{"Foreman: What brings you here?","Worker: You look too clean for this place","Bartender: Another round?"}, dialogueOptions = new string[][]{new string[]{"Looking for work connections","Just passing through","Buy everyone a round"},new string[]{"I work hard too","I just got off shift","You should see my hands"},new string[]{"Absolutely, my treat","I'll have water","One more then I'm out"}}, dialogueCorrectIndices = new int[]{2,1,0}, instructions = "Network at the dive bar!" });
        AddConfigFull("Hospitality Gala", new MiniGameConfig { type = GameType.OrderQueue, orderItems = new string[]{"Greet Host","Compliment Venue","Exchange Cards","Small Talk","Toast","Exit"}, orderPreviewTime = 3f, instructions = "Work the room!" });
        AddConfigFull("Underground Tech Meetup", new MiniGameConfig { type = GameType.CodeBreaker, codeSymbols = new string[]{"0","1","2","3","4","5","6","7","8","9","A","B","C","D","E","F"}, codeLength = 4, maxAttempts = 8, instructions = "Decrypt to mingle!" });
        AddConfigFull("Corporate Golf Sim", new MiniGameConfig { type = GameType.TimingRing, timingRounds = 3, timingZoneWidth = 22f, instructions = "Perfect your swing!" });

        // =============================================
        // LEGAL SIDE HUSTLES (4 tasks)
        // =============================================
        AddConfigFull("Collect Recycled Bottles", new MiniGameConfig { type = GameType.FallingSort, fallingItems = new string[]{"\U0001f7e2 Glass","\U0001f535 Plastic","\U0001f7e4 Aluminium"}, binLabels = new string[]{"Glass","Plastic","Metal"}, sortTarget = 10, sortTimeLimit = 20f, instructions = "Sort the recyclables!" });
        AddConfigFull("Mow Corporate Astroturf", new MiniGameConfig { type = GameType.BalanceMeter, balanceHoldTime = 10f, balanceDriftSpeed = 30f, instructions = "Keep the blades level!" });
        AddConfigFull("Walk Robotic Pedigree Dogs", new MiniGameConfig { type = GameType.SprintLane, sprintObstacles = 12, sprintSpeed = 1.4f, instructions = "Keep up with the dogs!" });
        AddConfigFull("Deliver Fast Food Order", new MiniGameConfig { type = GameType.GridDodge, gridSize = 5, gridHealth = 3, instructions = "Navigate the traffic grid!" });

        // =============================================
        // ILLEGAL SIDE HUSTLES (4 tasks)
        // =============================================
        AddConfigFull("Forge Transit Passes", new MiniGameConfig { type = GameType.CodeBreaker, codeSymbols = new string[]{"0","1","2","3","4","5","6","7","8","9"}, codeLength = 4, maxAttempts = 5, instructions = "Forge the ID code!" });
        AddConfigFull("Snatch Luxury Briefcase", new MiniGameConfig { type = GameType.TimingRing, timingRounds = 3, timingZoneWidth = 15f, instructions = "Time your snatch!" });
        AddConfigFull("ATM Autonomous Hijack", new MiniGameConfig { type = GameType.CodeBreaker, codeSymbols = new string[]{"0","1","2","3","4","5","6","7","8","9","A","B","C","D","E","F"}, codeLength = 4, maxAttempts = 6, instructions = "Inject the ATM payload!" });
        AddConfigFull("Corporate Espionage Heist", new MiniGameConfig { type = GameType.TwoPhaseHeist, sequenceLength = 5, timeLimit = 15f, targetScore = 35, phase2Type = GameType.MashRace, instructions = "Phase 1: Bypass! Phase 2: Escape!" });
    }

    private void AddConfig(string name, GameType type, string instructions, int targetScore = 0, float timeLimit = 0, float sMin = 0, float sMax = 0, string c1 = null, string c2 = null, string c3 = null, string cCorrect = null, int seqLen = 4, int pairs = 4)
    {
        gameConfigs[name] = new MiniGameConfig
        {
            type = type,
            instructions = instructions,
            targetScore = targetScore,
            timeLimit = timeLimit,
            sliderMin = sMin,
            sliderMax = sMax,
            choices = (c1 != null) ? new string[] { c1, c2, c3 } : null,
            correctChoice = cCorrect,
            sequenceLength = seqLen,
            pairs = pairs
        };
    }

    private void AddConfigFull(string name, MiniGameConfig config)
    {
        gameConfigs[name] = config;
    }

    private MiniGameConfig GetConfigForGame(string gameName)
    {
        if (gameConfigs.TryGetValue(gameName, out var config)) return config;

        int hash = Mathf.Abs(gameName.GetHashCode());
        GameType randomType = GameType.Tap;

        return new MiniGameConfig
        {
            type = randomType,
            instructions = "Complete the designated task.",
            targetScore = 15,
            timeLimit = 15f,
            sliderMin = 40f,
            sliderMax = 60f,
            choices = new string[] { "Option A", "Option B", "Option C" },
            correctChoice = "Option A",
            sequenceLength = 4,
            pairs = 4
        };
    }

    public void LaunchMiniGame(ActiveMiniGame game)
    {
        currentGame = game;
        currentConfig = GetConfigForGame(game.name);
        
        score = 0;
        hasWon = false;
        isPlaying = true;
        currentPhase = 1;

        if (activeGameRoutine != null) StopCoroutine(activeGameRoutine);

        btnCompleteTask.style.backgroundColor = new StyleColor(new Color(0.28f, 0.33f, 0.41f));
        btnCompleteTask.text = "Complete Task";

        operationNameText.text = $"Active Operation: {game.name}";
        instructionsText.text = currentConfig.instructions;
        overlay.style.display = DisplayStyle.Flex;
        overlay.pickingMode = PickingMode.Position;

        HideAllContainers();
        SetupGameType(currentConfig.type);
    }

    private void HideAllContainers()
    {
        if(tapContainer != null) tapContainer.style.display = DisplayStyle.None;
        if(sliderContainer != null) sliderContainer.style.display = DisplayStyle.None;
        if(choiceContainer != null) choiceContainer.style.display = DisplayStyle.None;
        if(sequenceContainer != null) sequenceContainer.style.display = DisplayStyle.None;
        if(swipeContainer != null) swipeContainer.style.display = DisplayStyle.None;
        if(mashRaceContainer != null) mashRaceContainer.style.display = DisplayStyle.None;
        if(memoryMatchContainer != null) memoryMatchContainer.style.display = DisplayStyle.None;
        
        // New containers
        if(timingRingContainer != null) timingRingContainer.style.display = DisplayStyle.None;
        if(fallingSortContainer != null) fallingSortContainer.style.display = DisplayStyle.None;
        if(gridDodgeContainer != null) gridDodgeContainer.style.display = DisplayStyle.None;
        if(whackGridContainer != null) whackGridContainer.style.display = DisplayStyle.None;
        if(codeBreakerContainer != null) codeBreakerContainer.style.display = DisplayStyle.None;
        if(balanceMeterContainer != null) balanceMeterContainer.style.display = DisplayStyle.None;
        if(dialogueChainContainer != null) dialogueChainContainer.style.display = DisplayStyle.None;
        if(orderQueueContainer != null) orderQueueContainer.style.display = DisplayStyle.None;
        if(dragMatchContainer != null) dragMatchContainer.style.display = DisplayStyle.None;
        if(sprintLaneContainer != null) sprintLaneContainer.style.display = DisplayStyle.None;
    }

    private void SetupGameType(GameType type)
    {
        switch (type)
        {
            case GameType.Tap:
                tapContainer.style.display = DisplayStyle.Flex;
                tapProgressText.text = $"Progress: 0 / {currentConfig.targetScore}";
                break;

            case GameType.Slider:
                sliderContainer.style.display = DisplayStyle.Flex;
                gameSlider.value = 50f;
                UpdateSliderText(50f);
                break;

            case GameType.Choice:
                choiceContainer.style.display = DisplayStyle.Flex;
                btnChoice1.text = currentConfig.choices[0];
                btnChoice2.text = currentConfig.choices[1];
                btnChoice3.text = currentConfig.choices[2];
                break;

            case GameType.Sequence:
            case GameType.TwoPhaseHeist:
                sequenceContainer.style.display = DisplayStyle.Flex;
                GenerateSequence(currentConfig.sequenceLength);
                activeGameRoutine = StartCoroutine(PlaySequenceRoutine());
                break;

            case GameType.Swipe:
                swipeContainer.style.display = DisplayStyle.Flex;
                score = 0;
                timer = currentConfig.timeLimit > 0 ? currentConfig.timeLimit : 15f;
                swipeProgressText.text = $"Dodged: {score} / {currentConfig.targetScore}";
                activeGameRoutine = StartCoroutine(SwipeGameRoutine());
                break;

            case GameType.MashRace:
                mashRaceContainer.style.display = DisplayStyle.Flex;
                score = 0;
                opponentProgress = 0f;
                timer = currentConfig.timeLimit > 0 ? currentConfig.timeLimit : 10f;
                playerProgressBar.style.width = new Length(0, LengthUnit.Percent);
                opponentProgressBar.style.width = new Length(0, LengthUnit.Percent);
                activeGameRoutine = StartCoroutine(MashRaceRoutine());
                break;

            case GameType.MemoryMatch:
                memoryMatchContainer.style.display = DisplayStyle.Flex;
                SetupMemoryMatch(currentConfig.pairs);
                break;

            case GameType.TimingRing:
                timingRingContainer.style.display = DisplayStyle.Flex;
                timingCurrentRound = 0;
                activeGameRoutine = StartCoroutine(TimingRingRoutine());
                break;
                
            case GameType.FallingSort:
                fallingSortContainer.style.display = DisplayStyle.Flex;
                sortScore = 0;
                timer = currentConfig.sortTimeLimit;
                btnBin1.text = currentConfig.binLabels[0];
                btnBin2.text = currentConfig.binLabels[1];
                btnBin3.text = currentConfig.binLabels[2];
                activeGameRoutine = StartCoroutine(FallingSortRoutine());
                break;
                
            case GameType.GridDodge:
                gridDodgeContainer.style.display = DisplayStyle.Flex;
                SetupGridDodge();
                break;
                
            case GameType.WhackGrid:
                whackGridContainer.style.display = DisplayStyle.Flex;
                whackScore = 0;
                timer = currentConfig.whackTime;
                SetupWhackGrid();
                activeGameRoutine = StartCoroutine(WhackGridRoutine());
                break;
                
            case GameType.CodeBreaker:
                codeBreakerContainer.style.display = DisplayStyle.Flex;
                SetupCodeBreaker();
                break;
                
            case GameType.BalanceMeter:
                balanceMeterContainer.style.display = DisplayStyle.Flex;
                balanceValue = 50f;
                balanceTimeInZone = 0f;
                activeGameRoutine = StartCoroutine(BalanceMeterRoutine());
                break;
                
            case GameType.DialogueChain:
                dialogueChainContainer.style.display = DisplayStyle.Flex;
                dialogueRound = 0;
                rapportValue = 0.5f;
                UpdateDialogueUI();
                break;
                
            case GameType.OrderQueue:
                orderQueueContainer.style.display = DisplayStyle.Flex;
                activeGameRoutine = StartCoroutine(OrderQueueRoutine());
                break;
                
            case GameType.DragMatch:
                dragMatchContainer.style.display = DisplayStyle.Flex;
                SetupDragMatch();
                break;
                
            case GameType.SprintLane:
                sprintLaneContainer.style.display = DisplayStyle.Flex;
                sprintDistance = 0;
                sprintProgressBar.style.width = new Length(0, LengthUnit.Percent);
                activeGameRoutine = StartCoroutine(SprintLaneRoutine());
                break;
        }
    }

    private void OnTapClicked()
    {
        if (!isPlaying || currentConfig.type != GameType.Tap) return;
        score++;
        tapProgressText.text = $"Progress: {score} / {currentConfig.targetScore}";
        if (score >= currentConfig.targetScore) MarkWinReady();
    }

    private void UpdateSliderText(float val)
    {
        if (currentConfig.type == GameType.Slider)
        {
            sliderTargetText.text = $"Current: {val:F0}% (Target: {currentConfig.sliderMin}% - {currentConfig.sliderMax}%)";
        }
    }

    private void OnSliderSubmitted()
    {
        if (!isPlaying || currentConfig.type != GameType.Slider) return;
        float val = gameSlider.value;
        if (val >= currentConfig.sliderMin && val <= currentConfig.sliderMax)
        {
            MarkWinReady();
        }
    }

    private void OnChoiceClicked(string choice)
    {
        if (!isPlaying || currentConfig.type != GameType.Choice) return;
        if (choice == currentConfig.correctChoice)
        {
            MarkWinReady();
        }
    }

    private void GenerateSequence(int length)
    {
        currentSequence.Clear();
        int activeButtons = Mathf.Min(6, length + 2);
        for (int i = 0; i < 6; i++)
        {
            if (i < activeButtons)
            {
                seqBtns[i].style.display = DisplayStyle.Flex;
                seqBtns[i].text = $"[{i + 1}]";
            }
            else
            {
                seqBtns[i].style.display = DisplayStyle.None;
            }
        }

        for (int i = 0; i < length; i++)
        {
            currentSequence.Add(Random.Range(0, activeButtons));
        }
    }

    private IEnumerator PlaySequenceRoutine()
    {
        isShowingSequence = true;
        sequenceInputIndex = 0;
        sequenceStatusText.text = "Watch the pattern...";
        
        foreach (var btn in seqBtns) btn.style.backgroundColor = new StyleColor(new Color(0.12f, 0.16f, 0.23f));

        yield return new WaitForSeconds(1f);

        foreach (int index in currentSequence)
        {
            seqBtns[index].style.backgroundColor = new StyleColor(new Color(0.22f, 0.74f, 0.97f));
            yield return new WaitForSeconds(0.4f);
            seqBtns[index].style.backgroundColor = new StyleColor(new Color(0.12f, 0.16f, 0.23f));
            yield return new WaitForSeconds(0.2f);
        }

        sequenceStatusText.text = "Your Turn!";
        isShowingSequence = false;
    }

    private void ReplaySequence()
    {
        if (isPlaying && !isShowingSequence)
        {
            if (activeGameRoutine != null) StopCoroutine(activeGameRoutine);
            activeGameRoutine = StartCoroutine(PlaySequenceRoutine());
        }
    }

    private void OnSequenceButtonClicked(int index)
    {
        if (!isPlaying || isShowingSequence) return;
        if (currentConfig.type != GameType.Sequence && currentConfig.type != GameType.TwoPhaseHeist) return;

        if (currentSequence[sequenceInputIndex] == index)
        {
            sequenceInputIndex++;
            seqBtns[index].style.backgroundColor = new StyleColor(new Color(0.18f, 0.8f, 0.44f));
            StartCoroutine(ResetButtonColor(seqBtns[index]));

            if (sequenceInputIndex >= currentSequence.Count)
            {
                if (currentConfig.type == GameType.TwoPhaseHeist && currentPhase == 1)
                {
                    currentPhase = 2;
                    sequenceContainer.style.display = DisplayStyle.None;
                    instructionsText.text = "Phase 2: MASH TO ESCAPE!";
                    mashRaceContainer.style.display = DisplayStyle.Flex;
                    score = 0;
                    opponentProgress = 0f;
                    timer = currentConfig.timeLimit;
                    playerProgressBar.style.width = new Length(0, LengthUnit.Percent);
                    opponentProgressBar.style.width = new Length(0, LengthUnit.Percent);
                    activeGameRoutine = StartCoroutine(MashRaceRoutine());
                }
                else
                {
                    sequenceStatusText.text = "Pattern Complete!";
                    MarkWinReady();
                }
            }
        }
        else
        {
            if (currentGame.canFail)
            {
                sequenceStatusText.text = "Pattern Failed!";
                seqBtns[index].style.backgroundColor = new StyleColor(new Color(0.9f, 0.3f, 0.23f));
                StartCoroutine(ResetButtonColor(seqBtns[index]));
                ReplaySequence();
            }
            else
            {
                StartCoroutine(ResetButtonColor(seqBtns[index], new Color(0.9f, 0.3f, 0.23f)));
            }
        }
    }

    private IEnumerator ResetButtonColor(Button btn, Color? tempColor = null)
    {
        if (tempColor.HasValue) btn.style.backgroundColor = new StyleColor(tempColor.Value);
        yield return new WaitForSeconds(0.2f);
        btn.style.backgroundColor = new StyleColor(new Color(0.12f, 0.16f, 0.23f));
    }

    private IEnumerator SwipeGameRoutine()
    {
        while (isPlaying && timer > 0 && score < currentConfig.targetScore)
        {
            expectedDodgeDir = Random.value > 0.5f ? 1 : -1;
            swipeObstacleText.text = expectedDodgeDir == -1 ? "\u25c0 THREAT FROM LEFT!" : "THREAT FROM RIGHT! \u25b6";
            
            float reactTime = 1.5f;
            float elapsed = 0;

            while (elapsed < reactTime)
            {
                if (!isPlaying) yield break;
                elapsed += Time.deltaTime;
                timer -= Time.deltaTime;
                swipeTimerText.text = $"Time: {Mathf.Ceil(timer)}s";

                if (expectedDodgeDir == 0)
                {
                    score++;
                    swipeProgressText.text = $"Dodged: {score} / {currentConfig.targetScore}";
                    break;
                }
                yield return null;
            }

            expectedDodgeDir = 0;
            swipeObstacleText.text = "Get Ready...";
            yield return new WaitForSeconds(0.5f);
        }

        if (score >= currentConfig.targetScore || (!currentGame.canFail && timer <= 0))
        {
            swipeObstacleText.text = "SURVIVED!";
            MarkWinReady();
        }
    }

    private void OnDodgeClicked(int dir)
    {
        if (!isPlaying || currentConfig.type != GameType.Swipe) return;
        if (expectedDodgeDir == dir)
        {
            expectedDodgeDir = 0;
        }
    }

    private IEnumerator MashRaceRoutine()
    {
        while (isPlaying && score < currentConfig.targetScore && opponentProgress < currentConfig.targetScore && timer > 0)
        {
            timer -= Time.deltaTime;
            mashTimerText.text = $"Time: {Mathf.Ceil(timer)}s";

            if (currentGame.canFail)
            {
                opponentProgress += (currentConfig.targetScore / currentConfig.timeLimit) * Time.deltaTime * 0.9f;
            }
            
            float oppPct = Mathf.Clamp01(opponentProgress / currentConfig.targetScore) * 100f;
            opponentProgressBar.style.width = new Length(oppPct, LengthUnit.Percent);

            if (opponentProgress >= currentConfig.targetScore)
            {
                mashTimerText.text = "OPPONENT WINS!";
                isPlaying = false;
                yield break;
            }

            if (timer <= 0 && currentGame.canFail)
            {
                mashTimerText.text = "TIME UP!";
                isPlaying = false;
                yield break;
            }
            else if (timer <= 0 && !currentGame.canFail)
            {
                MarkWinReady();
            }

            yield return null;
        }
    }

    private void OnMashTapClicked()
    {
        if (!isPlaying) return;
        if (currentConfig.type != GameType.MashRace && currentPhase != 2) return;

        score++;
        float pct = Mathf.Clamp01((float)score / currentConfig.targetScore) * 100f;
        playerProgressBar.style.width = new Length(pct, LengthUnit.Percent);

        if (score >= currentConfig.targetScore)
        {
            mashTimerText.text = "YOU WIN!";
            MarkWinReady();
        }
    }

    private void SetupMemoryMatch(int pairs)
    {
        memoryGrid.Clear();
        memoryCards.Clear();
        memoryValues.Clear();
        score = 0;
        matchStatusText.text = $"Pairs Found: 0 / {pairs}";

        List<string> icons = new List<string> { "\U0001f4c8", "\U0001f4bc", "\U0001f3e2", "\U0001f4ca", "\U0001f4b0", "\u2615", "\U0001f4f1", "\U0001f4dd" };
        for (int i = 0; i < pairs; i++)
        {
            memoryValues.Add(icons[i % icons.Count]);
            memoryValues.Add(icons[i % icons.Count]);
        }
        
        for (int i = 0; i < memoryValues.Count; i++)
        {
            string temp = memoryValues[i];
            int randomIndex = Random.Range(i, memoryValues.Count);
            memoryValues[i] = memoryValues[randomIndex];
            memoryValues[randomIndex] = temp;
        }

        for (int i = 0; i < memoryValues.Count; i++)
        {
            int index = i;
            var btn = new Button { text = "?" };
            btn.AddToClassList("popup-button");
            btn.AddToClassList("memory-card");
            btn.clicked += () => OnMemoryCardClicked(index);
            memoryGrid.Add(btn);
            memoryCards.Add(btn);
        }
    }

    private void OnMemoryCardClicked(int index)
    {
        if (!isPlaying || isCheckingMatch || memoryCards[index].text != "?") return;

        memoryCards[index].text = memoryValues[index];
        memoryCards[index].RemoveFromClassList("memory-card");
        memoryCards[index].AddToClassList("memory-card-matched");

        if (flippedIndex1 == -1)
        {
            flippedIndex1 = index;
        }
        else
        {
            flippedIndex2 = index;
            StartCoroutine(CheckMemoryMatch());
        }
    }

    private IEnumerator CheckMemoryMatch()
    {
        isCheckingMatch = true;
        yield return new WaitForSeconds(0.6f);

        if (memoryValues[flippedIndex1] == memoryValues[flippedIndex2])
        {
            score++;
            matchStatusText.text = $"Pairs Found: {score} / {currentConfig.pairs}";
            if (score >= currentConfig.pairs) MarkWinReady();
        }
        else
        {
            memoryCards[flippedIndex1].text = "?";
            memoryCards[flippedIndex1].RemoveFromClassList("memory-card-matched");
            memoryCards[flippedIndex1].AddToClassList("memory-card");
            memoryCards[flippedIndex2].text = "?";
            memoryCards[flippedIndex2].RemoveFromClassList("memory-card-matched");
            memoryCards[flippedIndex2].AddToClassList("memory-card");
        }

        flippedIndex1 = -1;
        flippedIndex2 = -1;
        isCheckingMatch = false;
    }

    // NEW GAME TYPES LOGIC

    private IEnumerator TimingRingRoutine()
    {
        while (isPlaying && timingCurrentRound < currentConfig.timingRounds)
        {
            timingRoundText.text = $"Round {timingCurrentRound + 1} / {currentConfig.timingRounds}";
            float targetPos = Random.Range(10f, 90f);
            float w = currentConfig.timingZoneWidth;
            if (timingTargetZone != null)
            {
                timingTargetZone.style.left = new Length(targetPos - w/2, LengthUnit.Percent);
                timingTargetZone.style.width = new Length(w, LengthUnit.Percent);
            }
            timingMarkerMoving = true;
            
            while (timingMarkerMoving && isPlaying)
            {
                timingMarkerPos += timingMarkerDir * timingMarkerSpeed * Time.deltaTime;
                if (timingMarkerPos > 100f) { timingMarkerPos = 100f; timingMarkerDir = -1f; }
                if (timingMarkerPos < 0f) { timingMarkerPos = 0f; timingMarkerDir = 1f; }
                
                if (timingMarker != null)
                    timingMarker.style.left = new Length(timingMarkerPos, LengthUnit.Percent);
                yield return null;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
    
    private void OnTimingStrike()
    {
        if (!isPlaying || currentConfig.type != GameType.TimingRing || !timingMarkerMoving) return;
        timingMarkerMoving = false;
        
        float w = currentConfig.timingZoneWidth;
        float zLeft = timingTargetZone.resolvedStyle.left / timingGaugeTrack.resolvedStyle.width * 100f;
        float zRight = zLeft + w;
        
        if (timingMarkerPos >= zLeft && timingMarkerPos <= zRight)
        {
            timingCurrentRound++;
            if (timingCurrentRound >= currentConfig.timingRounds)
                MarkWinReady();
            else
                activeGameRoutine = StartCoroutine(TimingRingRoutine());
        }
        else
        {
            if (currentGame.canFail) { /* fail state could be handled */ }
            else activeGameRoutine = StartCoroutine(TimingRingRoutine());
        }
    }

    private IEnumerator FallingSortRoutine()
    {
        while (isPlaying && sortScore < currentConfig.sortTarget && timer > 0)
        {
            sortScoreText.text = $"Sorted: {sortScore} / {currentConfig.sortTarget}";
            sortTimerText.text = $"Time: {Mathf.Ceil(timer)}s";
            
            if (fallingItemText.text == "")
            {
                currentFallingItemIndex = Random.Range(0, currentConfig.fallingItems.Length);
                fallingItemText.text = currentConfig.fallingItems[currentFallingItemIndex];
            }
            
            timer -= Time.deltaTime;
            if (timer <= 0 && currentGame.canFail)
            {
                isPlaying = false;
                yield break;
            }
            else if (timer <= 0 && !currentGame.canFail)
            {
                MarkWinReady();
            }
            yield return null;
        }
    }

    private void OnSortBinClicked(int binIndex)
    {
        if (!isPlaying || currentConfig.type != GameType.FallingSort || fallingItemText.text == "") return;
        
        if (binIndex == currentFallingItemIndex)
        {
            sortScore++;
            sortScoreText.text = $"Sorted: {sortScore} / {currentConfig.sortTarget}";
            if (sortScore >= currentConfig.sortTarget) MarkWinReady();
        }
        fallingItemText.text = ""; 
    }

    private void SetupGridDodge()
    {
        gridDodgeGrid.Clear();
        gridCells = new Button[currentConfig.gridSize, currentConfig.gridSize];
        gridPlayerX = 0;
        gridPlayerY = currentConfig.gridSize / 2;
        gridHP = currentConfig.gridHealth;
        gridDangerCells.Clear();
        
        for (int y = 0; y < currentConfig.gridSize; y++)
        {
            var row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.flexGrow = 1;
            for (int x = 0; x < currentConfig.gridSize; x++)
            {
                var btn = new Button();
                btn.style.flexGrow = 1;
                btn.style.marginAll = 2;
                int cx = x; int cy = y;
                btn.clicked += () => OnGridCellClicked(cx, cy);
                gridCells[x,y] = btn;
                row.Add(btn);
            }
            gridDodgeGrid.Add(row);
        }
        UpdateGridDodgeUI();
    }

    private void UpdateGridDodgeUI()
    {
        gridHealthText.text = $"HP: {gridHP}";
        for (int x=0; x<currentConfig.gridSize; x++)
        {
            for (int y=0; y<currentConfig.gridSize; y++)
            {
                var btn = gridCells[x,y];
                btn.text = "";
                btn.style.backgroundColor = new StyleColor(new Color(0.2f, 0.2f, 0.2f));
                
                if (gridDangerCells.Contains((x,y))) btn.style.backgroundColor = new StyleColor(Color.red);
                if (x == currentConfig.gridSize - 1) btn.style.backgroundColor = new StyleColor(Color.yellow);
                if (x == gridPlayerX && y == gridPlayerY)
                {
                    btn.text = "P";
                    btn.style.backgroundColor = new StyleColor(Color.green);
                }
            }
        }
    }

    private void OnGridCellClicked(int x, int y)
    {
        if (!isPlaying || currentConfig.type != GameType.GridDodge) return;
        if (Mathf.Abs(x - gridPlayerX) + Mathf.Abs(y - gridPlayerY) == 1)
        {
            gridPlayerX = x; gridPlayerY = y;
            if (gridDangerCells.Contains((x,y)))
            {
                gridHP--;
                if (gridHP <= 0 && currentGame.canFail) { isPlaying = false; return; }
                else if (gridHP <= 0 && !currentGame.canFail) gridHP = 1; 
            }
            if (gridPlayerX == currentConfig.gridSize - 1)
            {
                MarkWinReady();
                return;
            }
            gridDangerCells.Clear();
            int dangers = currentConfig.gridSize;
            for(int i=0; i<dangers; i++)
            {
                gridDangerCells.Add((Random.Range(0, currentConfig.gridSize), Random.Range(0, currentConfig.gridSize)));
            }
            UpdateGridDodgeUI();
        }
    }

    private void SetupWhackGrid()
    {
        whackGrid.Clear();
        whackCells = new Button[9];
        for (int i=0; i<9; i++)
        {
            var btn = new Button();
            btn.style.width = new Length(30, LengthUnit.Percent);
            btn.style.height = new Length(30, LengthUnit.Percent);
            btn.style.marginAll = 2;
            int idx = i;
            btn.clicked += () => OnWhackCellClicked(idx);
            whackGrid.Add(btn);
            whackCells[i] = btn;
            ResetWhackBtn(btn);
        }
    }

    private void ResetWhackBtn(Button btn)
    {
        btn.style.backgroundColor = new StyleColor(Color.gray);
        btn.text = "";
    }

    private IEnumerator WhackGridRoutine()
    {
        while(isPlaying && timer > 0 && whackScore < currentConfig.whackTarget)
        {
            timer -= Time.deltaTime;
            whackTimerText.text = $"Time: {Mathf.Ceil(timer)}s";
            
            if (activeWhackCell == -1)
            {
                activeWhackCell = Random.Range(0, 9);
                isDecoy = Random.value > 0.7f;
                var btn = whackCells[activeWhackCell];
                btn.style.backgroundColor = new StyleColor(isDecoy ? Color.red : Color.green);
                btn.text = "!";
                yield return new WaitForSeconds(Random.Range(0.8f, 1.2f));
                if (activeWhackCell != -1)
                {
                    ResetWhackBtn(whackCells[activeWhackCell]);
                    activeWhackCell = -1;
                }
            }
            yield return null;
        }
        if (whackScore >= currentConfig.whackTarget || (!currentGame.canFail && timer <= 0)) MarkWinReady();
    }

    private void OnWhackCellClicked(int idx)
    {
        if (!isPlaying || currentConfig.type != GameType.WhackGrid) return;
        if (idx == activeWhackCell)
        {
            if (isDecoy) whackScore = Mathf.Max(0, whackScore - 1);
            else whackScore++;
            
            whackScoreText.text = $"Score: {whackScore} / {currentConfig.whackTarget}";
            ResetWhackBtn(whackCells[activeWhackCell]);
            activeWhackCell = -1;
            
            if (whackScore >= currentConfig.whackTarget) MarkWinReady();
        }
    }

    private void SetupCodeBreaker()
    {
        codeGuessRow.Clear();
        codeSymbolRow.Clear();
        codeHistoryList.Clear();
        currentGuess.Clear();
        codeSlotButtons.Clear();
        codeAttemptsRemaining = currentConfig.maxAttempts;
        codeAttemptsText.text = $"Attempts: {codeAttemptsRemaining}";
        
        secretCode = new string[currentConfig.codeLength];
        for(int i=0; i<currentConfig.codeLength; i++)
            secretCode[i] = currentConfig.codeSymbols[Random.Range(0, currentConfig.codeSymbols.Length)];
            
        for(int i=0; i<currentConfig.codeLength; i++)
        {
            var btn = new Button { text = "_" };
            btn.style.width = 40; btn.style.height = 40;
            codeGuessRow.Add(btn);
            codeSlotButtons.Add(btn);
        }
        
        foreach(var sym in currentConfig.codeSymbols)
        {
            var btn = new Button { text = sym };
            btn.style.width = 30; btn.style.height = 30;
            btn.clicked += () => {
                if (currentGuess.Count < currentConfig.codeLength) {
                    currentGuess.Add(sym);
                    UpdateCodeSlots();
                }
            };
            codeSymbolRow.Add(btn);
        }
    }

    private void UpdateCodeSlots()
    {
        for (int i=0; i<currentConfig.codeLength; i++)
        {
            codeSlotButtons[i].text = i < currentGuess.Count ? currentGuess[i] : "_";
        }
    }

    private void OnCodeClear()
    {
        currentGuess.Clear();
        UpdateCodeSlots();
    }

    private void OnCodeSubmit()
    {
        if (!isPlaying || currentGuess.Count < currentConfig.codeLength) return;
        
        var histRow = new VisualElement();
        histRow.style.flexDirection = FlexDirection.Row;
        int correctPos = 0;
        
        for (int i=0; i<currentConfig.codeLength; i++)
        {
            var lbl = new Label(currentGuess[i]);
            lbl.style.width = 20; lbl.style.marginAll = 2;
            if (currentGuess[i] == secretCode[i]) {
                lbl.style.color = Color.green;
                correctPos++;
            }
            else if (secretCode.Contains(currentGuess[i])) lbl.style.color = Color.yellow;
            else lbl.style.color = Color.gray;
            histRow.Add(lbl);
        }
        codeHistoryList.Add(histRow);
        
        if (correctPos == currentConfig.codeLength) { MarkWinReady(); return; }
        
        codeAttemptsRemaining--;
        codeAttemptsText.text = $"Attempts: {codeAttemptsRemaining}";
        currentGuess.Clear();
        UpdateCodeSlots();
        
        if (codeAttemptsRemaining <= 0)
        {
            if (currentGame.canFail) { isPlaying = false; }
            else { SetupCodeBreaker(); } 
        }
    }

    private IEnumerator BalanceMeterRoutine()
    {
        while (isPlaying)
        {
            balanceDrift += Random.Range(-2f, 2f) * Time.deltaTime * currentConfig.balanceDriftSpeed;
            balanceDrift = Mathf.Clamp(balanceDrift, -10f, 10f);
            balanceValue += balanceDrift * Time.deltaTime;
            balanceValue = Mathf.Clamp(balanceValue, 0f, 100f);
            
            if (balanceNeedle != null) balanceNeedle.style.left = new Length(balanceValue, LengthUnit.Percent);
            
            if (balanceValue >= 35f && balanceValue <= 65f)
            {
                balanceTimeInZone += Time.deltaTime;
                float pct = Mathf.Clamp01(balanceTimeInZone / currentConfig.balanceHoldTime) * 100f;
                if (balanceTimeBar != null) balanceTimeBar.style.width = new Length(pct, LengthUnit.Percent);
                if (balanceTimeInZone >= currentConfig.balanceHoldTime) MarkWinReady();
            }
            yield return null;
        }
    }

    private void OnBalanceNudge(float amount)
    {
        if (!isPlaying || currentConfig.type != GameType.BalanceMeter) return;
        balanceValue += amount;
        balanceDrift *= 0.5f; 
    }

    private void UpdateDialogueUI()
    {
        if (dialogueRound >= currentConfig.dialogueScenarios.Length)
        {
            if (rapportValue >= currentConfig.rapportThreshold || !currentGame.canFail) MarkWinReady();
            else { isPlaying = false; } 
            return;
        }
        
        dialogueRoundText.text = $"Round {dialogueRound + 1} / {currentConfig.dialogueScenarios.Length}";
        dialogueScenarioText.text = currentConfig.dialogueScenarios[dialogueRound];
        btnResponse1.text = currentConfig.dialogueOptions[dialogueRound][0];
        btnResponse2.text = currentConfig.dialogueOptions[dialogueRound][1];
        btnResponse3.text = currentConfig.dialogueOptions[dialogueRound][2];
        if (rapportBar != null) rapportBar.style.width = new Length(rapportValue * 100f, LengthUnit.Percent);
    }

    private void OnDialogueResponse(int idx)
    {
        if (!isPlaying || currentConfig.type != GameType.DialogueChain) return;
        if (idx == currentConfig.dialogueCorrectIndices[dialogueRound]) rapportValue += 0.25f;
        else rapportValue -= 0.15f;
        rapportValue = Mathf.Clamp01(rapportValue);
        dialogueRound++;
        UpdateDialogueUI();
    }

    private IEnumerator OrderQueueRoutine()
    {
        orderCorrectSequence = currentConfig.orderItems.ToList();
        orderInputIndex = 0;
        orderIsPreview = true;
        
        orderStatusText.text = "Memorize the order!";
        orderItemGrid.Clear();
        foreach (var item in orderCorrectSequence)
        {
            var btn = new Button { text = item };
            btn.style.backgroundColor = new StyleColor(Color.blue);
            orderItemGrid.Add(btn);
        }
        
        yield return new WaitForSeconds(currentConfig.orderPreviewTime);
        
        orderIsPreview = false;
        orderStatusText.text = "Select in order!";
        orderItemGrid.Clear();
        
        var scrambled = orderCorrectSequence.OrderBy(x => Random.value).ToList();
        foreach (var item in scrambled)
        {
            var btn = new Button { text = item };
            btn.clicked += () => OnOrderItemClicked(item, btn);
            orderItemGrid.Add(btn);
        }
    }

    private void OnOrderItemClicked(string item, Button btn)
    {
        if (!isPlaying || orderIsPreview || currentConfig.type != GameType.OrderQueue) return;
        
        if (item == orderCorrectSequence[orderInputIndex])
        {
            btn.style.backgroundColor = new StyleColor(Color.green);
            btn.SetEnabled(false);
            orderInputIndex++;
            if (orderInputIndex >= orderCorrectSequence.Count) MarkWinReady();
        }
        else
        {
            if (currentGame.canFail) { activeGameRoutine = StartCoroutine(OrderQueueRoutine()); }
            else { btn.style.backgroundColor = new StyleColor(Color.red); }
        }
    }

    private void SetupDragMatch()
    {
        dragLeftColumn.Clear();
        dragRightColumn.Clear();
        dragLeftButtons.Clear();
        dragRightButtons.Clear();
        dragCorrectMap.Clear();
        dragMatchCount = 0;
        dragSelectedLeft = -1;
        
        var lefts = currentConfig.matchLeft.ToList();
        var rights = currentConfig.matchRight.ToList();
        var rightIndices = Enumerable.Range(0, rights.Count).OrderBy(x => Random.value).ToList();
        
        for (int i=0; i<lefts.Count; i++)
        {
            int idx = i;
            var lBtn = new Button { text = lefts[i] };
            lBtn.clicked += () => { dragSelectedLeft = idx; foreach(var b in dragLeftButtons) b.style.backgroundColor = new StyleColor(Color.gray); lBtn.style.backgroundColor = new StyleColor(Color.yellow); };
            dragLeftButtons.Add(lBtn);
            dragLeftColumn.Add(lBtn);
            
            int rIdx = rightIndices[i];
            dragCorrectMap[i] = rIdx;
        }
        
        for (int i=0; i<rights.Count; i++)
        {
            int idx = i;
            var rBtn = new Button { text = rights[idx] };
            rBtn.clicked += () => OnDragRightClicked(idx, rBtn);
            dragRightButtons.Add(rBtn);
            dragRightColumn.Add(rBtn);
        }
    }

    private void OnDragRightClicked(int rIdx, Button rBtn)
    {
        if (!isPlaying || dragSelectedLeft == -1 || currentConfig.type != GameType.DragMatch) return;
        
        if (dragCorrectMap[dragSelectedLeft] == rIdx)
        {
            dragLeftButtons[dragSelectedLeft].style.backgroundColor = new StyleColor(Color.green);
            dragLeftButtons[dragSelectedLeft].SetEnabled(false);
            rBtn.style.backgroundColor = new StyleColor(Color.green);
            rBtn.SetEnabled(false);
            dragMatchCount++;
            if (dragMatchCount >= currentConfig.matchLeft.Length) MarkWinReady();
        }
        else
        {
            dragLeftButtons[dragSelectedLeft].style.backgroundColor = new StyleColor(Color.gray);
        }
        dragSelectedLeft = -1;
    }

    private IEnumerator SprintLaneRoutine()
    {
        while (isPlaying && sprintDistance < currentConfig.sprintObstacles)
        {
            sprintDistanceText.text = $"Distance: {sprintDistance} / {currentConfig.sprintObstacles}";
            sprintObstacleIsHigh = Random.value > 0.5f;
            sprintObstacleText.text = sprintObstacleIsHigh ? "HIGH OBSTACLE! (Duck)" : "LOW OBSTACLE! (Jump)";
            sprintWaitingForInput = true;
            
            float t = currentConfig.sprintSpeed;
            while (t > 0 && sprintWaitingForInput && isPlaying)
            {
                t -= Time.deltaTime;
                if (sprintTimerText != null) sprintTimerText.text = $"React: {t:F1}s";
                yield return null;
            }
            
            if (sprintWaitingForInput && currentGame.canFail) { isPlaying = false; yield break; }
            sprintWaitingForInput = false;
            yield return new WaitForSeconds(0.2f);
        }
        if (sprintDistance >= currentConfig.sprintObstacles) MarkWinReady();
    }

    private void OnSprintReact(bool jump)
    {
        if (!isPlaying || currentConfig.type != GameType.SprintLane || !sprintWaitingForInput) return;
        
        bool success = (jump && !sprintObstacleIsHigh) || (!jump && sprintObstacleIsHigh);
        if (success)
        {
            sprintDistance++;
            float pct = Mathf.Clamp01((float)sprintDistance / currentConfig.sprintObstacles) * 100f;
            if (sprintProgressBar != null) sprintProgressBar.style.width = new Length(pct, LengthUnit.Percent);
            sprintWaitingForInput = false;
        }
        else if (currentGame.canFail)
        {
            isPlaying = false;
        }
    }

    private void MarkWinReady()
    {
        isPlaying = false;
        hasWon = true;
        if (activeGameRoutine != null) StopCoroutine(activeGameRoutine);

        Color green;
        UnityEngine.ColorUtility.TryParseHtmlString("#2ECC71", out green);
        btnCompleteTask.style.backgroundColor = new StyleColor(green);
        btnCompleteTask.text = "Complete Task (READY)";
    }

    private void AbortGame()
    {
        isPlaying = false;
        if (activeGameRoutine != null) StopCoroutine(activeGameRoutine);

        overlay.style.display = DisplayStyle.None;
        overlay.pickingMode = PickingMode.Ignore;
        var data = GameManager.Instance.currentData;

        if (currentGame.category == "side_hustle")
        {
            var hustle = HustlesUI.availableHustles.Find(h => h.name == currentGame.name);
            if (hustle != null && hustle.riskPercent > 0)
            {
                data.jailWeeksRemaining = hustle.jailSentence;
                data.jobTitle = "Unemployed";
                data.zone = "Slums";
                double fine = data.balance * 0.20;
                data.balance -= fine;
            }
            else
            {
                data.stress = Mathf.Clamp(data.stress + 0.15f, 0f, 1f);
            }
        }
        else
        {
            if (currentGame.canFail) data.stress = Mathf.Clamp(data.stress + 0.10f, 0f, 1f);
        }

        GameManager.Instance.SaveGame();
        var uiManager = FindAnyObjectByType<GameUIManager>();
        uiManager?.CheckJailStatus();
    }

    private void FinishGame()
    {
        if (!hasWon) return;

        isPlaying = false;
        overlay.style.display = DisplayStyle.None;
        overlay.pickingMode = PickingMode.Ignore;
        var data = GameManager.Instance.currentData;

        if (currentGame.category == "side_hustle")
        {
            var hustle = HustlesUI.availableHustles.Find(h => h.name == currentGame.name);
            string logMessage = taskLogMessages.ContainsKey(currentGame.name) ? taskLogMessages[currentGame.name] : $"I completed {currentGame.name}.";
            
            if (hustle != null && hustle.riskPercent > 0)
            {
                int roll = Random.Range(0, 100);
                if (roll < hustle.riskPercent)
                {
                    data.jailWeeksRemaining = hustle.jailSentence;
                    data.jobTitle = "Unemployed";
                    data.zone = "Slums";
                    double fine = data.balance * 0.20;
                    data.balance -= fine;
                    data.historyLogsJson += $"\n\n[SYSTEM] {logMessage} BUT I WAS CAUGHT! Fined ${fine:F0} and jailed.";
                }
                else
                {
                    data.balance += currentGame.reward;
                    data.historyLogsJson += $"\n\n[SYSTEM] {logMessage} Got away with ${currentGame.reward}.";
                }
            }
            else
            {
                data.balance += currentGame.reward;
                data.stress = Mathf.Clamp(data.stress + 0.10f, 0f, 1f);
                data.historyLogsJson += $"\n\n[SYSTEM] {logMessage} Earned ${currentGame.reward}.";
            }
            
            var uiManager = FindAnyObjectByType<GameUIManager>();
            uiManager?.ShowPopup(new RandomEvent { 
                title = "HUSTLE COMPLETED", 
                description = $"You finished: {currentGame.name}.\n\nCheck the dashboard for results!" 
            }, () => { 
                GameManager.Instance.CheckHeartAttack(() => {
                    HustlesUI.Instance?.PopulateHustlesBoard(); 
                    uiManager?.SwitchToDashboard();
                });
            });
        }
        else if (currentGame.category == "wellness")
        {
            data.balance -= currentGame.reward;
            data.stress = Mathf.Clamp(data.stress - currentGame.stressCost, 0f, 1f);
            
            string logMessage = taskLogMessages.ContainsKey(currentGame.name) ? taskLogMessages[currentGame.name] : $"You completed {currentGame.name}.";
            data.historyLogsJson += $"\n\n[SYSTEM] {logMessage} -{(currentGame.stressCost * 100):F0}% stress";
            
            var uiManager = FindAnyObjectByType<GameUIManager>();
            uiManager?.ShowPopup(new RandomEvent { 
                title = "WELLNESS PURCHASED", 
                description = $"You successfully completed: {currentGame.name}.\n\n<color=#2ECC71>Your stress index has dropped significantly.</color>\n\n<color=#E74C3C>Cost: ${currentGame.reward}</color>" 
            }, () => { 
                GameManager.Instance.CheckHeartAttack(() => {
                    uiManager?.SwitchToDashboard();
                });
            });
        }
        else if (currentGame.category == "mixer")
        {
            data.balance -= currentGame.reward;
            if (currentGame.name == "Industrial Dive Bar") data.luckManualService = Mathf.Clamp(data.luckManualService + currentGame.stressCost, 0f, 1f);
            else if (currentGame.name == "Hospitality Gala") data.luckHospitality = Mathf.Clamp(data.luckHospitality + currentGame.stressCost, 0f, 1f);
            else if (currentGame.name == "Underground Tech Meetup") data.luckTechnology = Mathf.Clamp(data.luckTechnology + currentGame.stressCost, 0f, 1f);
            else if (currentGame.name == "Corporate Golf Sim") data.luckCorporate = Mathf.Clamp(data.luckCorporate + currentGame.stressCost, 0f, 1f);

            string logMessage = taskLogMessages.ContainsKey(currentGame.name) ? taskLogMessages[currentGame.name] : $"You completed {currentGame.name}.";
            data.historyLogsJson += $"\n\n[SYSTEM] {logMessage} +{(currentGame.stressCost * 100):F0}% luck";
            
            var uiManager = FindAnyObjectByType<GameUIManager>();
            uiManager?.ShowPopup(new RandomEvent { 
                title = "NETWORK MIXER ATTENDED", 
                description = $"You successfully networked at {currentGame.name}.\n\n<color=#2ECC71>Your promotion luck in this sector has increased!</color>\n\n<color=#E74C3C>Cost: ${currentGame.reward}</color>" 
            }, () => { 
                GameManager.Instance.CheckHeartAttack(() => {
                    uiManager?.SwitchToDashboard();
                });
            });
        }
        else if (currentGame.category == "work_regular")
        {
            data.completedTasksThisWeek.Add(currentGame.name);
            data.stress = Mathf.Clamp(data.stress + currentGame.stressCost, 0f, 1f);

            if (data.completedTasksThisWeek.Count >= 3)
            {
                GameManager.Instance.completedRegularTaskThisWeek = true;
            }

            string logMessage = taskLogMessages.ContainsKey(currentGame.name) ? taskLogMessages[currentGame.name] : $"You completed {currentGame.name}.";
            data.historyLogsJson += $"\n\n[SYSTEM] {logMessage} +{(currentGame.stressCost * 100):F0}% stress";
            
            var uiManager = FindAnyObjectByType<GameUIManager>();
            uiManager?.ShowPopup(new RandomEvent { 
                title = "TASK COMPLETED", 
                description = $"You finished: {currentGame.name}.\n\n<color=#E74C3C>Stress increased by {(currentGame.stressCost * 100):F0}%.</color>\n\nWeekly Tasks Done: {data.completedTasksThisWeek.Count}/3" 
            }, () => { 
                GameManager.Instance.CheckHeartAttack(() => {
                    CareerUI.Instance.RefreshView(); 
                });
            });
        }
        else if (currentGame.category == "work_extra")
        {
            PlayerPrefs.SetInt("completedExtraTaskThisWeek", 1);
            data.completedTasksThisWeek.Add(currentGame.name);
            data.stress = Mathf.Clamp(data.stress + currentGame.stressCost, 0f, 1f);
            data.promotionProgress = Mathf.Clamp(data.promotionProgress + currentGame.readinessGain, 0f, 1f);
            
            string logMessage = taskLogMessages.ContainsKey(currentGame.name) ? taskLogMessages[currentGame.name] : $"You completed {currentGame.name}.";
            data.historyLogsJson += $"\n\n[SYSTEM] {logMessage} Promotion Readiness +{(currentGame.readinessGain * 100):F0}%";
            
            var uiManager = FindAnyObjectByType<GameUIManager>();
            uiManager?.ShowPopup(new RandomEvent { 
                title = "EXTRA INITIATIVE TAKEN", 
                description = $"You went above and beyond completing {currentGame.name}.\n\n<color=#2ECC71>Performance Readiness increased!</color>\n<color=#E74C3C>Stress increased by {(currentGame.stressCost * 100):F0}%.</color>" 
            }, () => { 
                GameManager.Instance.CheckHeartAttack(() => {
                    CareerUI.Instance.RefreshView(); 
                });
            });
        }
        else
        {
            data.stress = Mathf.Clamp(data.stress + currentGame.stressCost, 0f, 1f);
            if (currentGame.reward > 0) data.balance += currentGame.reward;
        }

        GameManager.Instance.SaveGame();
        var uiManagerForDashboard = FindAnyObjectByType<GameUIManager>();
        uiManagerForDashboard?.CheckJailStatus();
    }
}
"""

with open("/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs", "w") as f:
    f.write(content)

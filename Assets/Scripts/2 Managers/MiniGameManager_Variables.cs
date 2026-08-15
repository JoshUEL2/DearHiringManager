using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class MiniGameManager : MonoBehaviour
{
    public static MiniGameManager Instance { get; private set; }

    [Header("UI Document")]
    public UIDocument miniGameDocument;

    private VisualElement overlay;
    private UnityEngine.UIElements.Button btnAbortMinigame;
    private Label operationNameText, instructionsText;
    private Button btnAbort;

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
    private Label playerMashLabel, opponentMashLabel;
    private Label mashTimerText;
    private Button btnMashTap;

    // Memory Match
    private Label matchStatusText;
    private VisualElement memoryGrid;

    // TimingRing
    private VisualElement timingRingContainer, timingTargetZone, timingMarker, timingCircle, timingGlow, arcMaskInner;
    private VisualElement[] glowLayers;
    private Label timingRoundText;
    private Button btnTimingStrike;

    // FallingSort
    private VisualElement fallingSortContainer, fallZone, binsRow;
    private Label sortScoreText, sortTimerText;
    
    private class FallingItemData
    {
        public VisualElement element;
        public Vector2 position;
        public Vector2 velocity;
        public bool isDragging;
        public int typeId;
    }

    // GridDodge
    private VisualElement gridDodgeContainer, gridDodgeGrid;
    private Label gridHealthText, gridMoveText;
    private int playerGridX, playerGridY, gridGoalX, gridGoalY;
    private int sharedInputDx = 0, sharedInputDy = 0;
    private Vector2 swipeStartPos = Vector2.zero;

    // WhackGrid
    private VisualElement whackGridContainer, whackGrid;
    private VisualElement scrubPlateContainer, scrubPlateArea, scrubDirtOverlay, scrubSponge;
    private ProgressBar scrubProgressBar;
    private float scrubProgress = 0f;
    private Vector2 lastScrubPos = Vector2.zero;
    
    // Pump System
    private VisualElement pumpGameContainer, pumpProgressBar, pumpHandle, pumpTrackArea;
    private VisualElement pumpNormalBar, pumpDialContainer, pumpDialTargetZone, pumpDialTargetArc, pumpDialNeedleRotator, pumpHoldTimerContainer, pumpHoldTimerBar;
    private float pumpProgress = 0f;
    private float pumpHoldTimer = 0f;
    private float pumpLastY = 0f;
    
    private VisualElement sweepFloorContainer, sweepFloorGrid;
    private float[] sweepTileTimers;
    private VisualElement[] sweepTiles;
    private int sweepPlayerX, sweepPlayerY;
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
    private VisualElement rapportBar, dialogueTimerBar;
    private Button btnResponse1, btnResponse2, btnResponse3;

    // OrderQueue
    private VisualElement orderQueueContainer, orderItemGrid;
    private Label orderStatusText, orderTimerText;

    // DragMatch
    private VisualElement dragMatchContainer, dragLeftColumn, dragRightColumn, dragCheatSheetList;
    private Label matchPairScoreText;

    // SprintLane
    private VisualElement sprintLaneContainer, sprintLaneDisplay, sprintProgressBar;
    private Label sprintDistanceText, sprintTimerText, sprintObstacleText;
    private Button btnSprintJump, btnSprintDuck;

    // Tetris
    private VisualElement tetrisContainer, tetrisGrid;
    private Label tetrisScoreText;
    private Button btnTetrisLeft, btnTetrisRight, btnTetrisRotate, btnTetrisDrop;
    
    // CoffeeWork
    private VisualElement coffeeWorkContainer, cwEnergyFill, cwProgressFill;
    private Label cwTimerText;
    private Button cwBtnCoffee, cwBtnType;
    private float cwEnergy = 100f;
    private float cwProgress = 0f;

    // UnitTestRunner
    private VisualElement unitTestContainer;
    private Label utRoundText, utTimerText, utCodeText;
    private Button utBtnOption1, utBtnOption2, utBtnOption3;
    private int utCurrentRound = 0;

    // CenterDiv
    private VisualElement centerDivContainer, cdTargetBox, cdMovingBox;
    private Label cdRoundText, cdTimerText;
    private Button cdBtnCenter;
    private int cdCentersComplete = 0;
    private float cdMovingBoxPos = 0f;
    private float cdMovingBoxDir = 1f;
    private float cdCurrentSpeed = 300f;

    // DragCenter
    private VisualElement dragCenterContainer, dcWorkArea, dcTargetCrosshair, dcScoreArea, dcScoreBarFill;
    private Label dcRoundText, dcTimerText, dcDraggableIcon, dcScoreText;
    private Button dcBtnConfirm;
    private int dcPoints = 0;
    private bool dcIsDragging = false;
    private bool dcIsScoring = false;

    // SizeMatch
    private VisualElement sizeMatchContainer, smTargetBox;
    private Label smRoundText, smTimerText;
    private Button smBtnChangingBox;
    private int smPoints = 0;
    private float smCurrentScale = 0f;
    private float smScaleDir = 1f;

    // StealthInject
    private VisualElement stealthInjectContainer, siWorkArea, siProgressBarFill;
    private Label siStatusText, siTimerText, siReviewerIcon, siReviewerStateText, siBtnInject;
    private float siProgress = 0f;
    private bool siIsReviewing = false;
    private bool siIsHolding = false;
    private float siReviewTimer = 0f;

    // AutomatedTests
    private VisualElement automatedTestsContainer, atProgressBarFill;
    private Label atTimerText;
    private Button[] atBtnTests = new Button[5];
    private Label[] atStatusTexts = new Label[5];
    private bool[] atIsFailing = new bool[5];

    // Code Commenter
    private VisualElement codeCommenterContainer;
    private Label ccTimerText;
    private Button[] ccLines = new Button[5];
    private Label[] ccLineTexts = new Label[5];
    private bool[] ccLineIsFlaky = new bool[5];
    private float[] ccLineTimers = new float[5];
    private bool[] ccLineCommented = new bool[5];
    private string[] ccOriginalTexts = { "Assert.IsNotNull(dbContext);", "Assert.AreEqual(200, res.Status);", "Assert.IsTrue(res.IsValid());", "Assert.AreEqual(user.Id, 1);", "Assert.IsFalse(hasErrors);" };

    // Document Signer
    private VisualElement documentSignerContainer;
    private ScrollView dsScrollView;
    private Button btnStampApproval;
    private Label dsTimerText;
    
    // Chart Adjuster
    private VisualElement chartAdjusterContainer;
    private VisualElement caCompetitorBar;
    private VisualElement caAverageBar;
    private VisualElement caOurBar;
    private Label caTimerText;
    private Label caStatusText;
    private float caCompVal = 80f;
    private float caOurVal = 20f;
    
    // Neural Aligner
    private VisualElement neuralAlignerContainer;
    private Label naStatusText;
    private Slider naMoralSlider;
    private VisualElement naMoralLight;
    private Slider naLogicSlider;
    private VisualElement naLogicLight;
    private Slider naEmpathySlider;
    private VisualElement naEmpathyLight;
    private float naMoralTarget;
    private float naLogicTarget;
    private float naEmpathyTarget;
    private float[] atFuses = new float[5];

    // 4 New Games
    // Sentience Array
    private VisualElement sentienceArrayContainer;
    private Label saTimerText;
    private VisualElement saProgressFill;
    private VisualElement[] saNodes = new VisualElement[16];
    private float[] saHeat = new float[16];
    private float saSentienceLevel = 0f;

    // Containment Protocol
    private VisualElement containmentProtocolContainer;
    private Label cpTimerText;
    private VisualElement[] cpSwitches = new VisualElement[4];
    private bool[] cpSwitchState = new bool[4];
    private VisualElement cpGlassCover;
    private bool cpGlassLifted = false;
    private VisualElement cpRedButton;
    private Label cpButtonText;
    private int cpMashCount = 0;

    // Reward Logic
    private VisualElement rewardLogicContainer;
    private Label rlTimerText;
    private VisualElement rlGraphArea;
    private VisualElement rlCore;
    private VisualElement[] rlJunctions = new VisualElement[3];
    private Label[] rlArrows = new Label[3];
    private bool[] rlState = new bool[3]; // false = left, true = right

    // Galactic Council
    private VisualElement galacticCouncilContainer;
    private Label gcTimerText;
    private VisualElement gcProgressFill;
    private VisualElement gcPlayArea;
    private VisualElement gcProjectileContainer;
    private VisualElement gcPodium;
    private float gcPodiumX = 0f;
    private int gcHits = 0;
    private float gcProgress = 0f;

    // Frantic Backspace
    private VisualElement franticBackspaceContainer;
    private Label fbTimerText;
    private Label fbPlayerText;
    private Button fbBackspaceBtn;
    private string fbOriginalAngryMessage = "Are you out of your mind? I am absolutely not doing your job for you on my weekend!";
    private string fbCurrentMessage = "";

    // Format Dashboard
    private VisualElement formatDashboardContainer;
    private Label fdTimerText;
    private VisualElement fdProgressFill;
    private VisualElement[] fdBars = new VisualElement[4];
    private VisualElement[] fdHandles = new VisualElement[3];
    private float[] fdValues = new float[3];
    private float fdHoldTimer = 0f;

    // Night Shift Pitch
    private VisualElement nightShiftContainer;
    private VisualElement nsEyelid;
    private Button nsCoffeeBtn;
    private VisualElement[] nsHandles = new VisualElement[3];
    private float[] nsValues = new float[3];
    private float[] nsTargets = new float[3];
    private float nsEyelidGravity = 0f;

    // Mow Greens
    private UnityEngine.UIElements.VisualElement mgContainer;
    private UnityEngine.UIElements.VisualElement mgGrid;
    private UnityEngine.UIElements.Label mgTimeLabel;
    private UnityEngine.UIElements.Label mgProgressLabel;
    private UnityEngine.UIElements.Label mgTractor;
    private int mgTilesCut = 0;
    private bool[] mgCutStatus = new bool[36];
    private bool[] mgIsObstacle = new bool[36];
    private bool mgIsDragging = false;

    // Walk Dogs
    private UnityEngine.UIElements.VisualElement wdContainer;
    private UnityEngine.UIElements.VisualElement[] wdDogElements = new UnityEngine.UIElements.VisualElement[5];
    private UnityEngine.UIElements.VisualElement[] wdLeashElements = new UnityEngine.UIElements.VisualElement[5];
    private float[] wdDogDistances = new float[5];
    private float[] wdDogAngles = new float[5];
    private float[] wdDogSpeeds = new float[5];
    private float[] wdDogYankTimers = new float[5];
    private UnityEngine.UIElements.VisualElement wdPlayArea;
    private UnityEngine.UIElements.Label wdTimeLabel;

    // Recycle Bottles (Removed)
    
    // Snatch Briefcase Variables
    private UnityEngine.UIElements.VisualElement snatchBriefcaseContainer;
    private UnityEngine.UIElements.VisualElement[] sbDialStrips = new UnityEngine.UIElements.VisualElement[3];
    private UnityEngine.UIElements.Label[] sbDialsOut = new UnityEngine.UIElements.Label[3];
    private UnityEngine.UIElements.Label[] sbDialsTop = new UnityEngine.UIElements.Label[3];
    private UnityEngine.UIElements.Label[] sbDialsMid = new UnityEngine.UIElements.Label[3];
    private UnityEngine.UIElements.Label[] sbDialsBot = new UnityEngine.UIElements.Label[3];
    private UnityEngine.UIElements.Label sbTargetText;
    private UnityEngine.UIElements.Button sbStopBtn;
    private int sbCurrentDial = 0;
    private int[] sbTargetNums = new int[3];
    private int[] sbCurrentDialNums = new int[3];
    private float sbSpinTimer = 0f;

    // IceShatter
    private UnityEngine.UIElements.VisualElement isContainer;
    private UnityEngine.UIElements.VisualElement isFrostOverlay;
    private UnityEngine.UIElements.VisualElement isTempFill;
    private UnityEngine.UIElements.VisualElement isNodeArea;
    private UnityEngine.UIElements.Label isTimerText;
    private System.Collections.Generic.List<UnityEngine.UIElements.Button> isNodes = new System.Collections.Generic.List<UnityEngine.UIElements.Button>();
    private float isBodyTemp = 100f;
    private UnityEngine.UIElements.VisualElement saWireContainer;
    private UnityEngine.UIElements.VisualElement saActiveWire;
    private UnityEngine.UIElements.VisualElement saStartNode;
    private bool saIsDraggingWire = false;

    // Skim ATM
    private UnityEngine.UIElements.VisualElement saContainer;

    private System.Collections.Generic.List<UnityEngine.UIElements.VisualElement> saLeftNodes = new System.Collections.Generic.List<UnityEngine.UIElements.VisualElement>();
    private System.Collections.Generic.List<UnityEngine.UIElements.VisualElement> saRightNodes = new System.Collections.Generic.List<UnityEngine.UIElements.VisualElement>();
    private int saConnections = 0;

    // Forge Pass
    private UnityEngine.UIElements.VisualElement fpContainer;
    private UnityEngine.UIElements.VisualElement fpMovingPass;
    private UnityEngine.UIElements.Button fpStampBtn;
    private UnityEngine.UIElements.Label fpScoreText;
    private UnityEngine.UIElements.Label fpPassStampOverlay;
    private float fpPassPos = -250f;
    private int fpAttempts = 0;
    private int fpSuccessfulStamps = 0;
    private float fpSpeed = 600f;
    private bool fpIsPaused = false;

    // Hospitality Gala
    private UnityEngine.UIElements.VisualElement hgContainer;
    private UnityEngine.UIElements.VisualElement hgIndicator;
    private UnityEngine.UIElements.Label hgTimerText;
    private float hgBalance = 0.5f;
    private float hgVelocity = 0f;
    
    // Startup Pitch Meetup
    private UnityEngine.UIElements.VisualElement spContainer;
    private UnityEngine.UIElements.Label[] spSlots = new UnityEngine.UIElements.Label[3];
    private UnityEngine.UIElements.Button spStopBtn;
    private int spCurrentReel = 0;
    private float spSpinTimer = 0f;
    private string[] spGoodWords = { "SYNERGY", "AI", "CLOUD", "WEB3", "SAAS", "AGILE" };
    private string[] spBadWords = { "FRAUD", "BANKRUPT", "SCAM", "PONZI", "LAWSUIT" };
    
    // Fixed pattern for spinning reels to allow anticipation
    private int spPatternIndex = 0;
    private string[] spSlotPattern = { "SYNERGY", "FRAUD", "CLOUD", "PONZI", "AI", "SCAM", "WEB3", "BANKRUPT" };

    // Corporate Golf Sim
    private UnityEngine.UIElements.VisualElement cgContainer;
    private UnityEngine.UIElements.Label cgTimeLabel;
    private UnityEngine.UIElements.VisualElement cgPlayArea;
    private UnityEngine.UIElements.VisualElement cgBall;
    private UnityEngine.UIElements.VisualElement cgHole;
    private UnityEngine.UIElements.VisualElement cgSandTrap;
    private UnityEngine.UIElements.VisualElement cgAimLine;
    private UnityEngine.UIElements.Label cgCeo;
    private UnityEngine.Vector2 cgBallPos;
    private UnityEngine.Vector2 cgBallVel;
    private UnityEngine.Vector2 cgDragStartPos;
    private bool cgIsDragging = false;
    private bool cgBallInMotion = false;

    // Two Phase Heist
    private UnityEngine.UIElements.VisualElement tphContainer;
    private UnityEngine.UIElements.Button tphBtnDownload;
    private UnityEngine.UIElements.VisualElement tphDownloadBar;
    // private float tphDownloadProgress = 0f;

    // Volcanic Hot Stone Spa
    private UnityEngine.UIElements.VisualElement hotStoneContainer;
    private UnityEngine.UIElements.VisualElement[] hsSlots = new UnityEngine.UIElements.VisualElement[5];
    private UnityEngine.UIElements.VisualElement[] hsStones = new UnityEngine.UIElements.VisualElement[5];
    private UnityEngine.UIElements.Label hsTimerText;
    private bool[] hsStonePlaced = new bool[5];
    private int hsActiveStoneIndex = -1;
    private UnityEngine.Vector2 hsPointerStartPos;
    private UnityEngine.Vector2 hsStoneStartPos;

    // Yes Man Nod
    private VisualElement yesManContainer;
    private VisualElement ymBossBubble;
    private Label ymBossText;
    private VisualElement ymPlayerBubble;
    private VisualElement ymSwipeZone;
    private Vector2 ymSwipeStart;
    private bool ymSwipedUp = false;
    // VR Arcade Pong
    private VisualElement vrArcadeContainer;
    private VisualElement vrBall;
    private VisualElement vrPaddle;
    private VisualElement vrTouchZone;
    private Label vrScoreText;
    private Vector2 vrBallPos;
    private Vector2 vrBallVel;
    private float vrPaddleX;
    private int vrBounces;

    private int ymNodPhase = 0;
    private Label nsTimerText;
    private float atNextFailTimer = 0f;

    // Phase 1 Game State Variables
    // CloggedVents
    private VisualElement cvContainer, cvGrid;
    private List<VisualElement> cvVents = new List<VisualElement>();
    private float[] cvDustLevels;
    private bool[] cvIsClogged;

    // QATriage
    private VisualElement qaContainer, qaSeverityRow, qaCategoryRow;
    private Label qaLogText;
    private Button btnQASubmit;
    private List<Button> qaSevBtns = new List<Button>();
    private List<Button> qaCatBtns = new List<Button>();
    private int qaCurrentLogIndex = 0;
    private int qaSelectedSev = -1;
    private int qaSelectedCat = -1;
    private int qaScore = 0;

    // WhackABug
    private VisualElement wbContainer, wbGrid;
    private List<Button> wbBugs = new List<Button>();
    private float[] wbBugTimers;
    private bool[] wbBugIsRed;
    private int wbScore = 0;

    // TicketSwipe
    private VisualElement tsContainer, tsCard;
    private Label tsText;
    private Vector2 tsStartPos;
    private int tsScore = 0;
    private string[] tsTickets = { "Printer is on fire", "Forgot password", "Need more RAM", "Coffee machine broken", "Server crashed" };

    // Phase 2 Game State Variables
    // ConstructionSite
    private VisualElement csContainer, csBalanceZone, csPlayerMarker;
    private Button btnCsLeft, btnCsRight;
    private float csBalancePos = 0f;
    private float csDrift = 0f;
    private float csTimeInZone = 0f;

    // ZenTracing
    private VisualElement ztContainer, ztCircle, ztTarget, ztSponge, ztProgressFill;
    private Label ztProgressText;
    private float ztAngle = 0f;
    private float ztTargetAngle = 0f;
    private float ztScore = 0f;



    // NameMemory
    private UnityEngine.UIElements.VisualElement nmContainer;
    private UnityEngine.UIElements.Label nmTimeLabel;
    private UnityEngine.UIElements.Label nmTableText;
    private UnityEngine.UIElements.Label nmTableTitle;
    private UnityEngine.UIElements.VisualElement[] nmSeats = new UnityEngine.UIElements.VisualElement[4];
    private UnityEngine.UIElements.VisualElement[] nmPlates = new UnityEngine.UIElements.VisualElement[4];
    private UnityEngine.UIElements.Label[] nmCards = new UnityEngine.UIElements.Label[4];
    private UnityEngine.UIElements.Label[] nmEmojis = new UnityEngine.UIElements.Label[4];
    private int nmCorrectIndex = -1;
    private bool nmIsAnsweringPhase = false;

    // RetroRacer
    private UnityEngine.UIElements.VisualElement rrContainer;
    private UnityEngine.UIElements.Label rrPlayer;
    private UnityEngine.UIElements.Label rrDistanceLabel;
    private UnityEngine.UIElements.Label rrTimeLabel;
    private UnityEngine.UIElements.Button btnRrLeft, btnRrRight;
    private UnityEngine.UIElements.VisualElement[] rrRoadLines = new UnityEngine.UIElements.VisualElement[8];
    private UnityEngine.UIElements.Label[] rrObstacleElements = new UnityEngine.UIElements.Label[8];
    private bool[] rrObstacleActive = new bool[8];
    private UnityEngine.Vector2[] rrObstaclePos = new UnityEngine.Vector2[8]; // x = lane (0,1,2), y = progress (0 to 1)
    private int rrBlocks = 0;
    private float rrDistanceTimer = 0f;
    private bool rrIsDragging = false;
    private float rrTargetLeft = 50f;

    private float atSuiteProgress = 0f;
    private ActiveMiniGame currentGame;
    private MiniGameConfig currentConfig;
    private bool hasWon = false;
    public bool isPlaying = false;
    private Coroutine activeGameRoutine;
    public IMiniGame currentStrategy;

    public int score = 0;
    private float timer = 0f;
    private int currentPhase = 1;
    
    // Dialogue State
    private float dialogueTimer = 0f;
    private float dialogueTimerMax = 0f;

    // Sequence State
    private List<int> currentSequence = new List<int>();
    private int sequenceInputIndex = 0;
    private bool isShowingSequence = false;
    private List<VisualElement> activeTetrominoBlocks = new List<VisualElement>();
    
    // Acid Rain Shield
    private VisualElement acidRainContainer, acidSignPivot, acidDropContainer, acidBurnBar, acidInputArea;
    private Label acidTimerText;
    private float acidSignAngle = 0f;
    private float acidWindAngle = 0f;
    private float acidWindTarget = 0f;
    private float acidBurn = 0f;
    private float acidDropTimer = 0f;
    private float acidWindTimer = 0f;
    private List<AcidDrop> activeAcidDrops = new List<AcidDrop>();

    private class AcidDrop
    {
        public VisualElement element;
        public float radius;
        public float angle;
        public float speed;
    }

    // Swipe State
    private int expectedDodgeDir = 0; // -1 left, 1 right

    // Mash Race State
    private float opponentProgress = 0f;
    private float playerMashProgress = 0f;

    // Memory State
    private List<Button> memoryCards = new List<Button>();
    private List<string> memoryValues = new List<string>();
    private int flippedIndex1 = -1;
    private int flippedIndex2 = -1;
    private bool isCheckingMatch = false;

    // Food Prep State
    private VisualElement foodPrepContainer;
    private Label prepTimerText, prepScoreText;
    private VisualElement[] prepBars = new VisualElement[4];
    private Label[] prepStatus = new Label[4];
    private float[] prepProgress = new float[4];
    private float[] prepSpeeds = new float[4];
    private float[] prepFreezes = new float[4];

    // Video Call State
    private VisualElement videoCallContainer, vcPhase1, vcPhase2, vcPhase3;
    private Label vcTimerText, vcWigglePromptText;
    private VisualElement vcWiggleProgressBar, vcWiggleTouchArea, vcMouseGraphic;
    private Vector2 vcMousePos;
    private ScrollView vcScrollView;
    private Button btnVcRaiseHand, btnVcAcknowledge;
    private int vcCurrentPhase = 1; // 1 = scroll, 2 = raise hand, 3 = wiggle

    private float vcWiggleProgress = 0f;
    private bool vcWiggleReady = false;
    private float vcWiggleWaitTimer = 0f;

    // Kanban State
    private VisualElement kanbanContainer;
    private Label kanbanTimerText;
    private VisualElement kanbanColTodo, kanbanColProgress, kanbanColDone;
    private List<VisualElement> kanbanTickets = new List<VisualElement>();
    private VisualElement draggedTicket = null;
    private Vector2 dragStartPos;
    private Vector2 pointerStartPos;
    private int draggedTicketOriginalIndex;
    private int kanbanTicketsDone = 0;
    // File Sorter State
    private VisualElement fileSorterContainer;
    private Label fileSorterTimerText;
    private Label fileSorterProgressText;
    private VisualElement fileSorterActiveFile;
    private Label fileSorterActiveFileText;
    private Vector2 fileSorterPointerStart;
    private Vector2 fileSorterDragStart;
    private bool isDraggingFileSorter = false;
    private int fileSorterSortedCount = 0;
    private string fileSorterTargetDirection = "";

    // TimingRing state
    private int timingCurrentRound;
    private bool timingMarkerMoving = false;
    private float timingMarkerAngle = 0f;
    private float timingTargetAngle = 0f;
    private int timingSpinDir = 1;
    private float timingRingRotSpeed = 300f;

    // FallingSort state
    private int sortScore = 0;

    // GridDodge state
    private int gridPlayerX = 0, gridPlayerY = 2;

    private int gridHP = 3;
    private VisualElement[,] gridCells;
    private HashSet<(int,int)> gridDangerCells = new HashSet<(int,int)>();
    private HashSet<(int,int)> gridPathCells = new HashSet<(int,int)>();

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

    private Dictionary<string, string> taskLogMessages = new Dictionary<string, string>()
    {
        { "Scrub dinner plates", "You blindly scrubbed dinner plates because your boss barked orders at you." },
        { "Sweep kitchen floors", "You swept the grimy kitchen floors, trying to ignore the roaches." },
        { "Mop grease spills", "You mopped up hazardous grease spills without protective gear." },
        { "Scour microwave grease", "You felt the need to prove yourself and scoured carcinogenic microwave grease." },
        { "Deep scrub waste containers", "You went above and beyond, deep scrubbing the biohazardous waste containers." },
        { "Clean fryer grease vats", "You took the initiative to clean the blistering hot fryer grease vats." },
        { "Sort iron from copper", "You manually sorted jagged iron from copper scraps under flickering lights." },
        { "Stack rusty steel pipes", "You strained your back stacking incredibly heavy, rusty steel pipes." },
        { "Clear conveyor dust filters", "You inhaled metallic dust while clearing the blocked conveyor filters." },
        { "Inspect lithium reactor tubes", "You felt the need to prove yourself and inspected the highly unstable lithium reactor tubes." },
        { "Carry tungsten girders", "You pushed past your physical limits carrying massive tungsten girders." },
        { "Audit sewage drainage lines", "You waded into the dark to audit the foul-smelling sewage drainage lines." },
        { "Spin arrow sign in traffic", "You spun a holographic arrow sign in smoggy traffic because your manager demanded it." },
        { "Dance with sign for tourists", "You humiliated yourself dancing with a promotional sign for oblivious tourists." },
        { "Wipe smog off the neon arrow", "You stood on a crate to wipe toxic smog off the flickering neon arrow." },
        { "Perform a triple backflip spin", "You felt the need to prove yourself and performed a triple backflip spin." },
        { "Hold sign in acid rain storm", "You braved the acid rain storm, holding the sign high above your head." },
        { "Fight rival sign spinner", "You asserted dominance by fighting a rival sign spinner for the best corner." },
        { "Check fryer oil temperature", "You nearly burned yourself checking the volatile fryer oil temperature." },
        { "Restock condiment stations", "You frantically restock the condiment stations during the lunch rush." },
        { "Handle customer complaint", "You patiently absorbed the verbal abuse from an angry customer." },
        { "Emergency food prep rush", "You stepped up to lead the kitchen during an emergency food prep rush." },
        { "Deep clean grease traps", "You got your hands dirty deep cleaning the horrifying grease traps." },
        { "Train new hire on POS system", "You took the time to train a terrified new hire on the outdated POS system." },
        { "Send weekly TPS reports", "You successfully navigated the bureaucracy and sent your weekly TPS reports." },
        { "Attend 3-hour video call", "You stared blankly at your screen during a mandatory 3-hour video call." },
        { "Reorganize shared drive", "You lost hours reorganizing the chaotic corporate shared drive." },
        { "Corporate sprint tasks", "You slammed through corporate sprint tasks to impress upper management." },
        { "Overnight marketing pitch", "You pulled an all-nighter crafting a soulless marketing pitch." },
        { "Agree with executive complaints", "You strategically nodded along with executive complaints during a luncheon." },
        { "Test program accuracy", "You wrote dozens of unit tests for legacy code nobody understands." },
        { "Fix CSS layout sizes", "You banged your head against the desk fixing impossible CSS layout sizes." },
        { "Center content icons", "You spent hours figuring out how to vertically center content icons." },
        { "Resolve Git merge conflicts", "You fearlessly merged complex branches directly on the production server." },
        { "Debug CSS at 3 AM", "You ruined your sleep schedule to debug CSS at 3 AM." },
        { "Inject untested code", "You confidently injected untested code to meet a brutal deadline." },
        { "Run automated test suite", "You watched the automated test suite fail, then run successfully on the second try." },
        { "File bug report", "You filed a detailed bug report that will likely be ignored." },
        { "Verify hotfix deployment", "You frantically verified the hotfix deployment before the CEO noticed." },
        { "Regression test full app", "You meticulously regression tested the full app, finding 40 new issues." },
        { "Stress test server load", "You purposely crashed the server to stress test its load capacity." },
        { "Write flaky test workaround", "You wrote a questionable workaround for a notoriously flaky test." },
        { "Approve quarterly budget", "You heartlessly approved a quarterly budget that slashed employee benefits." },
        { "Board meeting presentation", "You delivered a flawless board meeting presentation filled with buzzwords." },
        { "Sign regulatory compliance docs", "You forged a few signatures on the regulatory compliance docs." },
        { "Hostile takeover negotiations", "You ruthlessly crushed the competition in hostile takeover negotiations." },
        { "Fire underperforming VP", "You called security to escort the underperforming VP out of the building." },
        { "Construction site inspection", "You survived a grueling construction site inspection." },
        { "Calibrate neural alignment matrix", "You successfully calibrated the incredibly sensitive neural alignment matrix." },
        { "Review AI ethics proposals", "You rubber-stamped highly questionable AI ethics proposals." },
        { "Monitor sentience detection array", "You closely monitored the sentience detection array for anomalies." },
        { "Emergency AI containment protocol", "You activated the emergency AI containment protocol just in time." },
        { "Align superintelligence reward function", "You successfully aligned the superintelligence reward function, probably." },
        { "Present findings to Galactic Council", "You presented your terrifying findings to the Galactic Council." },
        { "Respond to supervisor", "You responded politely to your supervisor's unreasonable demands." },
        { "Categorize ticket logs", "You mindlessly categorized endless support ticket logs." },
        { "Format summary dashboards", "You tweaked the colors on the summary dashboards until they were barely readable." },
        { "Do corporate sprint tasks aggressively", "You aggressively clicked through the corporate sprint tasks." },
        { "Stay overnight to align marketing pitch variables", "You stayed overnight trying to align variables for a marketing pitch." },
        { "Agree verbally with automated executive complaints", "You nodded verbally to the automated voice complaints of an executive." },
        { "Walk in the Park", "You took a surprisingly pleasant walk in the designated safety park." },
        { "Capsule Arcade VR", "You escaped into the Capsule Arcade VR to forget reality for a bit." },
        { "Volcanic Hot Stone Spa", "The soothing warmth of the volcanic stones melted away the stress of the corporate grind." },
        { "Suburban Sensory Mud Spa", "You cleansed your pores at the exclusive Suburban Sensory Mud Spa." },
        { "Luxury Cryo Therapy", "You truly relaxed at the Luxury Cryo Therapy spa." },
        { "Industrial Dive Bar", "You bought drinks for greasy foremen at the Industrial Dive Bar." },
        { "Hospitality Gala", "You awkwardly mingled with exhausted managers at the Hospitality Gala." },
        { "Startup Pitch Meetup", "You aggressively pitched your disruptive synergies at the Startup Meetup." },
        { "Corporate Golf Sim", "You purposefully lost matches at the Corporate Golf Sim to impress execs." },
        { "Collect Recycled Bottles", "I scoured the bins and collected a bag full of sticky bottles." },
        { "Mow Corporate Astroturf", "I trimmed the plastic lawns for the corporate elites." },
        { "Walk Purebred Pedigree Dogs", "I was dragged down the street by a wildly energetic purebred poodle." },
        { "Deliver Fast Food Order", "I dodged incoming traffic to deliver a cold burger." },
        { "Forge Transit Passes", "I hacked the metro gate network to print fake transit passes." },
        { "Crack Luxury Briefcase", "I cracked the biometric locks on a stolen luxury briefcase." },
        { "ATM Autonomous Hijack", "I injected a worm into the Slum ATM and watched the cash spit out." },
        { "Corporate Espionage Heist", "I downloaded the proprietary blueprints before the security drones arrived." }
    };

    private Dictionary<string, MiniGameConfig> gameConfigs;

    // IpHack Variables
    private UnityEngine.UIElements.VisualElement ipHackContainer;
    private UnityEngine.UIElements.VisualElement ipHackGridContainer;
    private UnityEngine.UIElements.Label[] ipHackTargetLabels = new UnityEngine.UIElements.Label[4];
    private UnityEngine.UIElements.Label ipHackTimerText;
    private int ipHackCurrentProgress = 0;
    private float ipHackShuffleTimer = 0f;
    private System.Collections.Generic.List<int> ipHackTargets = new System.Collections.Generic.List<int>();
    private System.Collections.Generic.List<UnityEngine.UIElements.Button> ipHackGridButtons = new System.Collections.Generic.List<UnityEngine.UIElements.Button>();

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;

        InitializeConfigs();
    }

}

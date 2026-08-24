using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class MiniGameManager : MonoBehaviour
{
    public static MiniGameManager Instance { get; private set; }

    [Header("UI Document")]
    [FormerlySerializedAs("miniGameDocument")] [SerializeField] private UIDocument _miniGameDocument;
    public UIDocument miniGameDocument { get => _miniGameDocument; set => _miniGameDocument = value; }

    private VisualElement overlay;
    private UnityEngine.UIElements.Button btnAbortMinigame;
    private Label operationNameText, instructionsText;
    private Button btnAbort;

    private Dictionary<string, MiniGameConfig> gameConfigs = new Dictionary<string, MiniGameConfig>();

    [FormerlySerializedAs("currentGame")] [SerializeField] private ActiveMiniGame _currentGame;
    public ActiveMiniGame currentGame { get => _currentGame; set => _currentGame = value; }

    [FormerlySerializedAs("isPlaying")] [SerializeField] private bool _isPlaying = false;
    public bool isPlaying { get => _isPlaying; set => _isPlaying = value; }

    private IMiniGame _currentStrategy;
    public IMiniGame currentStrategy { get => _currentStrategy; set => _currentStrategy = value; }

    [FormerlySerializedAs("score")] [SerializeField] private int _score = 0;
    public int score { get => _score; set => _score = value; }
    private bool hasWon = false;

    private MiniGameConfig currentConfig;

    private Coroutine activeGameRoutine;
    private int currentPhase = 1;
    
    // Dialogue State

    // Sequence State
    
    // Acid Rain Shield
    private VisualElement acidRainContainer, acidSignPivot, acidDropContainer, acidBurnBar, acidInputArea;
    private Label acidTimerText;

    private class AcidDrop
    {
        public VisualElement element;
        public float radius;
        public float angle;
        public float speed;
    }

    // Swipe State

    // Mash Race State

    // Memory State

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


    // Kanban State
    private VisualElement kanbanContainer;
    private Label kanbanTimerText;
    private VisualElement kanbanColTodo, kanbanColProgress, kanbanColDone;
    private List<VisualElement> kanbanTickets = new List<VisualElement>();
    private Vector2 dragStartPos;
    private Vector2 pointerStartPos;
    private int draggedTicketOriginalIndex;
    // File Sorter State
    private VisualElement fileSorterContainer;
    private Label fileSorterTimerText;
    private Label fileSorterProgressText;
    private VisualElement fileSorterActiveFile;
    private Label fileSorterActiveFileText;
    private Vector2 fileSorterPointerStart;
    private Vector2 fileSorterDragStart;

    // TimingRing state
    private int timingCurrentRound;

    // FallingSort state

    // GridDodge state
    private VisualElement[,] gridCells;
    private HashSet<(int,int)> gridDangerCells = new HashSet<(int,int)>();
    private HashSet<(int,int)> gridPathCells = new HashSet<(int,int)>();

    // WhackGrid state
    private Button[] whackCells;

    // CodeBreaker state
    private string[] secretCode;
    private List<string> currentGuess = new List<string>();
    private int codeAttemptsRemaining;
    private List<Button> codeSlotButtons = new List<Button>();

    // BalanceMeter state

    // DialogueChain state

    // OrderQueue state
    private List<string> orderCorrectSequence = new List<string>();

    // DragMatch state
    private List<Button> dragLeftButtons = new List<Button>();
    private List<Button> dragRightButtons = new List<Button>();
    private Dictionary<int,int> dragCorrectMap = new Dictionary<int,int>();

    // SprintLane state

    private Dictionary<string, string> taskLogMessages = new Dictionary<string, string>()
    {
        { "Scrub dinner plates", "You blindly scrubbed dinner plates because your boss barked orders at you." },
        { "Sweep kitchen floors", "You swept the grimy kitchen floors, trying to ignore the roaches." },
        { "Mop grease spills", "You mopped up hazardous grease spills without protective gear." },
        { "Scour microwave grease", "You felt the need to prove yourself and scoured carcinogenic microwave grease." },
        { "Deep scrub waste containers", "You went above and beyond, deep scrubbing the biohazardous waste containers." },
        { "Clean fryer grease vats", "You took the initiative to clean the blistering hot fryer grease vats." },
        { "Sort metal scraps", "You manually sorted jagged iron from copper scraps under flickering lights." },
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


    // IpHack Variables
    private UnityEngine.UIElements.VisualElement ipHackContainer;
    private UnityEngine.UIElements.VisualElement ipHackGridContainer;
    private UnityEngine.UIElements.Label[] ipHackTargetLabels = new UnityEngine.UIElements.Label[4];
    private UnityEngine.UIElements.Label ipHackTimerText;
    private System.Collections.Generic.List<int> ipHackTargets = new System.Collections.Generic.List<int>();
    private System.Collections.Generic.List<UnityEngine.UIElements.Button> ipHackGridButtons = new System.Collections.Generic.List<UnityEngine.UIElements.Button>();

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;

        InitializeConfigs();
    }

}

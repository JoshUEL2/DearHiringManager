using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class MiniGameManager : MonoBehaviour
{
    private void Start()
    {
        var root = miniGameDocument.rootVisualElement;
        root.pickingMode = PickingMode.Ignore;


        overlay = root.Q<VisualElement>("Overlay");
        btnAbortMinigame = root.Q<UnityEngine.UIElements.Button>("BtnAbortMinigame");
        if (btnAbortMinigame != null) btnAbortMinigame.clicked += AbortGame;
        operationNameText = root.Q<Label>("OperationNameText");
        instructionsText = root.Q<Label>("InstructionsText");

        // Footer
        btnAbort = root.Q<Button>("BtnAbortMinigame");
        if (btnAbort != null) btnAbort.clicked += AbortGame;


        overlay.style.display = DisplayStyle.None;
        overlay.pickingMode = PickingMode.Ignore;}
    private void BindLegacyUI(VisualElement activeRoot) { }

    private void InitializeConfigs()
    {
        gameConfigs = new Dictionary<string, MiniGameConfig>();

        // =============================================
        // DISHWASHER (6 tasks)
        // =============================================
        AddConfigFull("Scrub dinner plates", new MiniGameConfig { type = GameType.ScrubPlate, targetScore = 15, instructions = "Scrub the plate clean by moving the sponge!"});
        AddConfigFull("Sweep kitchen floors", new MiniGameConfig { type = GameType.SweepFloor, instructions = "Sweep everywhere! Tiles get dirty again after 15s!"});
        AddConfigFull("Mop grease spills", new MiniGameConfig { type = GameType.SweepFloor, instructions = "Mop everywhere! Tiles get dirty again after 15s!"});
        AddConfigFull("Scour microwave grease", new MiniGameConfig { type = GameType.ScrubPlate, targetScore = 15, instructions = "Fast! Scour the grease bubbles!"});
        AddConfigFull("Deep scrub waste containers", new MiniGameConfig { type = GameType.PumpSystem, instructions = "Pump to scrub the containers!"});
        AddConfigFull("Clean fryer grease vats", new MiniGameConfig { type = GameType.PumpSystem, pumpIsBalanceMode = true, pumpBalanceTargetMin = 0.6f, pumpBalanceTargetMax = 0.8f, pumpBalanceHoldTime = 3f, instructions = "Pump to maintain 170°C - 220°C for 3 seconds!"});
        AddConfigFull("Empty waste bin", new MiniGameConfig { type = GameType.GridDodge, gridSize = 5, gridHealth = 3, instructions = "Carry the leaky bags!"});
        AddConfigFull("Unclog kitchen sinks", new MiniGameConfig { type = GameType.PumpSystem, instructions = "Pump to plunge the sink!"});
        AddConfigFull("Emergency deep freeze sorting", new MiniGameConfig { type = GameType.FallingSort, fallingItems = new string[]{"🥩 Beef","🍗 Chicken","🐟 Fish"}, binLabels = new string[]{"Beef","Chicken","Fish"}, sortTarget = 10, sortTimeLimit = 30f, instructions = "Sort the freezer before it melts!"});
        AddConfigFull("Deep clean grease traps", new MiniGameConfig { type = GameType.PumpSystem, instructions = "Pump to empty the grease traps!"});

        // =============================================
        // SCRAP METAL SORTER (6 tasks)
        // =============================================
        AddConfigFull("Sort iron from copper", new MiniGameConfig { type = GameType.FallingSort, fallingItems = new string[]{"🔩 Iron","🟤 Copper","⬜ Alu"}, binLabels = new string[]{"Iron","Copper","Alu"}, sortTarget = 10, sortTimeLimit = 25f, instructions = "Sort falling metal scraps!"});
        AddConfigFull("Stack rusty steel pipes", new MiniGameConfig { type = GameType.Tetris, instructions = "Stack 10 beams perfectly!"});
        AddConfigFull("Clear conveyor dust filters", new MiniGameConfig { type = GameType.CloggedVents, timeLimit = 20f, ventCount = 6, dustBuildRate = 12f, instructions = "Rapidly tap clogged vents before they overheat!"});
        AddConfigFull("Inspect lithium reactor tubes", new MiniGameConfig { type = GameType.DragMatch, matchLeft = new string[]{"Tube A","Tube B","Tube C","Tube D","Tube E"}, matchRight = new string[]{"Critical","Warning","Stable","Optimal","Offline"}, instructions = "Match tubes to statuses!"});
        AddConfigFull("Carry tungsten girders", new MiniGameConfig { type = GameType.GridDodge, gridSize = 5, gridHealth = 3, instructions = "Carry heavy girders safely!"});
        AddConfigFull("Audit sewage drainage lines", new MiniGameConfig { type = GameType.DragMatch, matchLeft = new string[]{"Valve A","Valve B","Valve C","Valve D"}, matchRight = new string[]{"Clear","Blocked","Leaking","Corroded"}, instructions = "Match valves to their status!"});

        // =============================================
        // STREET CORNER SIGN SPINNER (6 tasks)
        // =============================================
        AddConfigFull("Spin arrow sign in traffic", new MiniGameConfig { type = GameType.TimingRing, timingRounds = 5, timingZoneWidth = 16f, instructions = "Hit the timing zones to perform spins!"});
        AddConfigFull("Dance with sign for tourists", new MiniGameConfig { type = GameType.OrderQueue, orderItems = new string[]{"Left","Right","Spin","Pose","Wave","Dip"}, orderPreviewTime = 3f, instructions = "Perform the full dance routine!"});
        AddConfigFull("Wipe smog off the neon arrow", new MiniGameConfig { type = GameType.ScrubPlate, targetScore = 12, instructions = "Apply even pressure to wipe!"});
        AddConfig("Perform a triple backflip spin", GameType.Sequence, "Execute the acrobatic sequence perfectly!", 0, 0, 0, 0, null, null, null, null, 6);
        AddConfigFull("Hold sign in acid rain storm", new MiniGameConfig { type = GameType.AcidRainShield, instructions = "Block the acid rain!"});
        AddConfig("Fight rival sign spinner", GameType.MashRace, "Out-spin your rival! Mash to assert dominance!", 35, 12f);

        // =============================================
        // FAST FOOD SHIFT MANAGER (6 tasks)
        // =============================================
        AddConfigFull("Check fryer oil temperature", new MiniGameConfig { type = GameType.PumpSystem, pumpIsBalanceMode = true, pumpBalanceTargetMin = 0.6f, pumpBalanceTargetMax = 0.8f, pumpBalanceHoldTime = 3f, instructions = "Pump to maintain 170°C - 220°C for 3 seconds!"});
        AddConfigFull("Restock condiment stations", new MiniGameConfig { type = GameType.FallingSort, fallingItems = new string[]{"🍯 Mustard","🥫 Ketchup","🧂 Relish"}, binLabels = new string[]{"Mustard","Ketchup","Relish"}, sortTarget = 10, sortTimeLimit = 30f, instructions = "Restock the falling condiments!"});
        AddConfigFull("Handle customer complaint", new MiniGameConfig { type = GameType.DialogueChain, rapportThreshold = 0.75f, dialogueScenarios = new string[]{"Customer: My chips are COLD!", "Customer: I WAITED 20 MINUTES!", "Customer: There's a HAIR in my burger!", "Customer: I want to speak to YOUR MANAGER!"}, dialogueOptions = new string[][]{new string[]{"I sincerely apologise","That's not my problem","Have you tried heating them up?"},new string[]{"I'm so sorry for the wait","We're understaffed","You should've ordered online"},new string[]{"That's horrifying, let me replace it","Are you sure it's not yours?","We use synthetic ingredients"},new string[]{"I AM the manager, let me help","They're not available","Go ahead, they'll say the same"}}, dialogueCorrectIndices = new int[]{0,0,0,0}, instructions = "De-escalate the complaint!"});
        AddConfigFull("Emergency food prep rush", new MiniGameConfig { type = GameType.FoodPrepRush, timeLimit = 20f, targetScore = 12, instructions = "Prep food fast! Tap when in the GREEN zone!"});
        AddConfigFull("Train new hire on POS system", new MiniGameConfig { type = GameType.OrderQueue, orderItems = new string[]{"Login","Select Menu","Add Items","Apply Discount","Process Payment","Print Receipt"}, orderPreviewTime = 3f, instructions = "Show them the POS steps!"});

        // =============================================
        // CORPORATE CUBICLE DRONE (6 tasks)
        // =============================================
        AddConfigFull("Send weekly TPS reports", new MiniGameConfig { type = GameType.CodeBreaker, codeSymbols = new string[]{"A","B","C","D","E","F"}, codeLength = 4, maxAttempts = 6, instructions = "Crack the system password to send reports!"});
        AddConfigFull("Attend 3-hour video call", new MiniGameConfig { type = GameType.VideoCall, timeLimit = 15f, targetScore = 1, instructions = "Pay attention!"});
        AddConfigFull("Reorganize shared drive", new MiniGameConfig { type = GameType.FileSorter, timeLimit = 18f, fileSorterTarget = 15, fileSorterItems = new string[]{"Q3_Budget.xlsx", "Q2_Payroll_Final.pdf", "Logo_v4.png", "MeetingNotes.docx", "DO_NOT_DELETE.sys", "ClientList.csv", "Q1_Forecast.xlsx", "Jan_Invoice.pdf", "Deck_v9_Final.pptx"}, instructions = "Swipe files to correct folders!"});
        AddConfigFull("Corporate sprint tasks", new MiniGameConfig { type = GameType.KanbanBoard, timeLimit = 20f, kanbanTicketCount = 8, instructions = "Drag all tickets to IN PROGRESS then DONE!"});
        AddConfigFull("Overnight marketing pitch", new MiniGameConfig { type = GameType.CoffeeWork, timeLimit = 15f, coffeeDrainRate = 22f, coffeeBoost = 20f, workProgressPerTap = 5f, instructions = "Type slides & Drink coffee to stay awake!"});
        AddConfigFull("Agree with executive complaints", new MiniGameConfig { type = GameType.DialogueChain, rapportThreshold = 0.8f, dialogueScenarios = new string[]{"VP: Q3 numbers are DISASTROUS!", "VP: Marketing spent HOW MUCH?!", "VP: Why can't we be more like Apple?!"}, dialogueOptions = new string[][]{new string[]{"You're absolutely right, sir","Actually the data shows...","That's above my pay grade"},new string[]{"Outrageous, I agree completely","It was within budget actually","I'll look into it"},new string[]{"Brilliant observation, sir","We have different strengths","I'll draft a proposal"}}, dialogueCorrectIndices = new int[]{0,0,0}, instructions = "Agree enthusiastically!"});

        // =============================================
        // JUNIOR REACT TRAINEE (6 tasks)
        // =============================================
        AddConfigFull("Test program accuracy", new MiniGameConfig { 
            type = GameType.UnitTestRunner, 
            timeLimit = 15f,
            unitTestRounds = 8,
            unitTestQuestions = new string[] {
                "Assert.AreEqual(?, 2 + 2)",
                "Assert.IsTrue(? > 10)",
                "Assert.IsNull(?)",
                "Assert.IsFalse(? == true)",
                "Assert.AreEqual(\"test\", ?)",
                "Assert.Throws<?>(() => Crash())",
                "Assert.IsNotNull(?)",
                "Assert.AreEqual(?, Math.Abs(-5))"},
            unitTestOptions = new string[][] {
                new string[] { "3", "4", "5"},
                new string[] { "5", "10", "15"},
                new string[] { "\"data\"", "0", "null"},
                new string[] { "true", "false", "null"},
                new string[] { "\"unit\"", "\"test\"", "null"},
                new string[] { "Exception", "String", "Int"},
                new string[] { "null", "new Object()", "0"},
                new string[] { "-5", "0", "5"}},
            unitTestCorrectIndices = new int[] { 1, 2, 2, 1, 1, 0, 1, 2},
            instructions = "Rapid fire unit tests!"});
        AddConfigFull("Fix CSS layout locations", new MiniGameConfig { type = GameType.CenterDiv, timeLimit = 15f, centerDivTargets = 3, centerDivSpeedMin = 300f, centerDivSpeedMax = 700f, instructions = "Center the div exactly!"});
        AddConfigFull("Fix CSS layout sizes", new MiniGameConfig { type = GameType.SizeMatch, timeLimit = 15f, instructions = "Match the exact size!"});
        AddConfigFull("Center content icons", new MiniGameConfig { type = GameType.DragCenter, timeLimit = 15f, instructions = "Drag the icon perfectly to the center!"});
        AddConfigFull("Resolve Git merge conflicts", new MiniGameConfig { type = GameType.GitMerge, timeLimit = 15f, targetScore = 5, instructions = "Tap the correct block to resolve conflicts!"});
        AddConfigFull("Debug CSS at 3 AM", new MiniGameConfig { type = GameType.CoffeeWork, timeLimit = 15f, coffeeDrainRate = 22f, coffeeBoost = 20f, workProgressPerTap = 5f, instructions = "Debug CSS & Drink coffee!"});
        AddConfigFull("Inject untested code", new MiniGameConfig { type = GameType.StealthInject, timeLimit = 15f, instructions = "Hold INJECT while the reviewer is distracted!"});

        // =============================================
        // DYSTOPIAN QA TESTER (6 tasks)
        // =============================================
        AddConfigFull("Run automated test suite", new MiniGameConfig { type = GameType.AutomatedTests, timeLimit = 15f, instructions = "Re-run any flaky tests before they crash the suite!"});
        AddConfigFull("File bug report", new MiniGameConfig { type = GameType.QATriage, timeLimit = 20f, targetScore = 5, qaSeverities = new string[]{"Minor", "Major", "Critical"}, qaCategories = new string[]{"UI", "Network", "Logic"}, qaBugLogs = new string[]{"NullReferenceException in Login()", "Timeout 504 on API", "Button overlaps text", "Infinite loop on checkout", "Database connection lost"}, instructions = "Drag the bug report to the correct department folder!"});
        AddConfigFull("Verify hotfix deployment", new MiniGameConfig { type = GameType.OrderQueue, orderItems = new string[]{"Build","Unit Test","Integration","Stage Deploy","Smoke Test","Prod Deploy"}, orderPreviewTime = 3f, instructions = "Run through deployment stages!"});
        AddConfigFull("Regression test full app", new MiniGameConfig { type = GameType.WhackABug, timeLimit = 20f, targetScore = 15, instructions = "Tap failing (red) modules to pass them!"});
        AddConfig("Stress test server load", GameType.MashRace, "Hammer the server! Mash faster!", 40, 15f);
        AddConfigFull("Write flaky test workaround", new MiniGameConfig { type = GameType.CodeCommenter, timeLimit = 10f, instructions = "Comment out the flaky tests before the build fails!"});

        // =============================================
        // EXECUTIVE INTERSTELLAR CEO (6 tasks)
        // =============================================
        AddConfigFull("Approve quarterly budget", new MiniGameConfig { type = GameType.DocumentSigner, timeLimit = 4f, instructions = "Scroll to the bottom and STAMP APPROVAL!"});
        AddConfigFull("Board meeting presentation", new MiniGameConfig { type = GameType.ChartAdjuster, timeLimit = 10f, instructions = "Fudge the presentation! Shrink them, grow us!"});
        AddConfigFull("Sign regulatory compliance docs", new MiniGameConfig { type = GameType.DialogueChain, rapportThreshold = 0.75f, dialogueScenarios = new string[]{"Legal: Clause 4.7 requires data residency","Legal: Section 12 has liability gaps","Legal: GDPR compliance needs updating","Legal: Sign here to accept all terms"}, dialogueOptions = new string[][]{new string[]{"Flag for legal review","Sign it anyway","Delegate to VP"},new string[]{"Request amendment","Ignore the gaps","That's fine"},new string[]{"Engage compliance team","We'll deal with it later","What's GDPR?"},new string[]{"Review one more time","Sign immediately","Have my assistant sign"}}, dialogueCorrectIndices = new int[]{0,0,0,0}, instructions = "Handle compliance carefully!"});
        AddConfigFull("Hostile takeover negotiations", new MiniGameConfig { type = GameType.DialogueChain, rapportThreshold = 0.8f, dialogueScenarios = new string[]{"Their CEO: Our company isn't for sale","Their CEO: Your offer is insulting","Board: The shareholders are nervous","Media: Is this a hostile takeover?","Their CEO: Final counteroffer - $2B"}, dialogueOptions = new string[][]{new string[]{"Everyone has a price","Let's discuss the benefits","We'll go directly to shareholders"},new string[]{"We'll raise to fair market value","Take it or leave it","Let's find common ground"},new string[]{"This acquisition creates value","Shareholders should trust us","We'll sweeten the deal"},new string[]{"It's a strategic partnership","No comment","We're creating jobs"},new string[]{"Deal. Let's close this","We can go higher","Let me consult my board"}}, dialogueCorrectIndices = new int[]{1,0,0,0,0}, instructions = "Negotiate the takeover!"});
        AddConfigFull("Fire underperforming VP", new MiniGameConfig { type = GameType.DialogueChain, rapportThreshold = 0.7f, dialogueScenarios = new string[]{"VP: I've been with the company 15 years!","VP: My team loves me!","VP: What about my stock options?"}, dialogueOptions = new string[][]{new string[]{"And we value that. Here's a generous package","That's exactly the problem","Security will escort you out"},new string[]{"They'll be taken care of","They'll manage without you","We're restructuring the team"},new string[]{"They'll vest on schedule","Forfeited as per contract","We'll negotiate"}}, dialogueCorrectIndices = new int[]{0,0,0}, instructions = "Fire them smoothly!"});
        AddConfigFull("Construction site inspection", new MiniGameConfig { type = GameType.ConstructionSite, timeLimit = 15f, balanceDriftSpeed = 0f, instructions = "Scan the wall for heat leaks!"});

        // =============================================
        // AI PROPHECY ALIGNMENT DIRECTOR (6 tasks)
        // =============================================
        AddConfigFull("Calibrate neural alignment matrix", new MiniGameConfig { type = GameType.NeuralAligner, timeLimit = 12f, instructions = "Tune the sliders until all lights are GREEN!"});
        AddConfigFull("Review AI ethics proposals", new MiniGameConfig { type = GameType.DialogueChain, rapportThreshold = 0.8f, dialogueScenarios = new string[]{"Proposal: Give AI emotional awareness","Proposal: AI self-modification capabilities","Proposal: Autonomous weapons systems","Proposal: AI voting rights","Proposal: Merge human consciousness with AI"}, dialogueOptions = new string[][]{new string[]{"Approve with strict monitoring","Reject — too dangerous","Classify top-secret"},new string[]{"Reject — existential risk","Approve if sandboxed","Needs more research"},new string[]{"Absolutely not","Only for defense","Approve with oversight"},new string[]{"Premature but worth studying","Approve immediately","Reject — they're not people"},new string[]{"Fund research cautiously","Too sci-fi, reject","Approve clinical trials"}}, dialogueCorrectIndices = new int[]{0,0,0,0,0}, instructions = "Judge AI proposals!"});
        AddConfigFull("Monitor sentience detection array", new MiniGameConfig { type = GameType.SentienceArray, timeLimit = 12f, instructions = "Whack the red sentience spikes!"});
        AddConfigFull("Emergency AI containment protocol", new MiniGameConfig { type = GameType.ContainmentProtocol, timeLimit = 10f, instructions = "Sever hardlines! Lift cover! Mash lockdown!"});
        AddConfigFull("Align superintelligence reward function", new MiniGameConfig { type = GameType.RewardLogic, timeLimit = 12f, instructions = "Route logic to PRESERVE humanity!"});
        AddConfigFull("Present findings to Galactic Council", new MiniGameConfig { type = GameType.GalacticCouncil, timeLimit = 15f, instructions = "Dodge objections and finish speech!"});

        // =============================================
        // FALLBACK TASKS (6 tasks)
        // =============================================
        AddConfigFull("Respond to supervisor", new MiniGameConfig { type = GameType.FranticBackspace, timeLimit = 6f, instructions = "FRANTIC BACKSPACE! DELETE YOUR HONEST THOUGHTS!"});
        AddConfigFull("Categorize ticket logs", new MiniGameConfig { type = GameType.TicketSwipe, timeLimit = 20f, targetScore = 10, instructions = "Correctly categorize 10 tickets before time runs out!"});
        AddConfigFull("Format summary dashboards", new MiniGameConfig { type = GameType.FormatDashboard, timeLimit = 15f, instructions = "Make it POP! Swipe all 3 sliders up and HOLD them in the green!"});
        AddConfigFull("Do corporate sprint tasks aggressively", new MiniGameConfig { type = GameType.KanbanBoard, timeLimit = 15f, kanbanTicketCount = 12, instructions = "Drag ALL tickets to IN PROGRESS then DONE! FAST!"});
        AddConfigFull("Stay overnight to align marketing pitch variables", new MiniGameConfig { type = GameType.NightShiftPitch, timeLimit = 15f, instructions = "Drink coffee to stay awake and align the variables!"});
        AddConfigFull("Agree verbally with automated executive complaints", new MiniGameConfig { type = GameType.YesManNod, timeLimit = 15f, instructions = "Nod (SWIPE UP) aggressively to every complaint!"});

        // =============================================
        // WELLNESS ACTIVITIES (5 tasks)
        // =============================================
        AddConfigFull("Walk in the Park", new MiniGameConfig { type = GameType.GridDodge, gridSize = 5, gridHealth = 5, instructions = "A peaceful walk."});
        AddConfigFull("Capsule Arcade VR", new MiniGameConfig { type = GameType.VrArcadePong, timeLimit = 15f, instructions = "Keep the ball alive! (Bounce 5 times)"});
        AddConfigFull("Volcanic Hot Stone Spa", new MiniGameConfig { type = GameType.HotStoneSpa, timeLimit = 15f, instructions = "Place the hot stones along the spine to relax."});
        AddConfigFull("Suburban Sensory Mud Spa", new MiniGameConfig { type = GameType.ZenTracing, timeLimit = 15f, instructions = "Keep the sponge in the target circle!"});
        AddConfigFull("Luxury Cryo Therapy", new MiniGameConfig { type = GameType.IceShatter, timeLimit = 15f, instructions = "Tap ice nodes before you freeze!"});

        // =============================================
        // MIXERS (4 tasks)
        // =============================================
        AddConfigFull("Industrial Dive Bar", new MiniGameConfig { type = GameType.DialogueChain, rapportThreshold = 0.6f, dialogueScenarios = new string[]{"Foreman: What brings you here?","Worker: You look too clean for this place","Bartender: Another round?"}, dialogueOptions = new string[][]{new string[]{"Looking for work connections","Just passing through","Buy everyone a round"},new string[]{"I work hard too","I just got off shift","You should see my hands"},new string[]{"Absolutely, my treat","I'll have water","One more then I'm out"}}, dialogueCorrectIndices = new int[]{2,1,0}, instructions = "Network at the dive bar!"});
        AddConfigFull("Hospitality Gala", new MiniGameConfig { type = GameType.NameMemory, timeLimit = 15f, vipNames = new string[]{"Sarah from Tech", "John from Sales", "Mike from HR", "Emma from Legal"}, instructions = "Memorize the VIPs!"});
        AddConfigFull("Startup Pitch Meetup", new MiniGameConfig { type = GameType.StartupPitch, timeLimit = 15f, instructions = "Pitch the perfect buzzwords!"});
        AddConfigFull("Corporate Golf Sim", new MiniGameConfig { type = GameType.CorporateGolf, timeLimit = 15f, instructions = "Let the CEO win! Aim for the sand trap!"});

        // =============================================
        // LEGAL SIDE HUSTLES (4 tasks)
        // =============================================
        AddConfigFull("Collect Recycled Bottles", new MiniGameConfig { type = GameType.FallingSort, fallingItems = new string[]{"🟢 Glass","🔵 Plastic","🟤 Aluminium"}, binLabels = new string[]{"Glass","Plastic","Metal"}, sortTarget = 10, sortTimeLimit = 20f, instructions = "Sort the recyclables!"});
        AddConfigFull("Executive Lawn Care", new MiniGameConfig { type = GameType.MowGreens, timeLimit = 15f, instructions = "Mow the grass, avoid obstacles!"});
        AddConfigFull("Walk Purebred Pedigree Dogs", new MiniGameConfig { type = GameType.WalkDogs, timeLimit = 15f, instructions = "Keep all 3 leashes in the green zone!"});
        AddConfigFull("Deliver Fast Food Order", new MiniGameConfig { type = GameType.RetroRacer, timeLimit = 20f, instructions = "Dodge traffic and deliver the food!"});

        // =============================================
        // ILLEGAL SIDE HUSTLES (4 tasks)
        // =============================================
        AddConfigFull("Forge Transit Passes", new MiniGameConfig { type = GameType.ForgePass, timeLimit = 10f, instructions = "Stamp exactly in the center!"});
        AddConfigFull("Crack Luxury Briefcase", new MiniGameConfig { type = GameType.SnatchBriefcase, timeLimit = 15f, instructions = "Crack the lock!"});
        AddConfigFull("Skim ATM Card Data", new MiniGameConfig { type = GameType.SkimATM, timeLimit = 10f, instructions = "Connect the skimmer wires!"});
        AddConfigFull("Corporate Espionage Heist", new MiniGameConfig { type = GameType.IpHack, timeLimit = 30f, instructions = "Hack the IP Address!"});}

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
            choices = (c1 != null) ? new string[] { c1, c2, c3} : null,
            correctChoice = cCorrect,
            sequenceLength = seqLen,
            pairs = pairs};}

    private void AddConfigFull(string name, MiniGameConfig config)
    {
        gameConfigs[name] = config;}

    public Dictionary<string, MiniGameConfig> GameConfigs => gameConfigs;

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
            choices = new string[] { "Option A", "Option B", "Option C"},
            correctChoice = "Option A",
            sequenceLength = 4,
            pairs = 4};}

    private TemplateContainer activeGameUI;

    private string GetUxmlName(GameType type)
    {
        switch(type)
        {
            case GameType.CloggedVents: return "CloggedVents";
            case GameType.WhackABug: return "WhackABug";
            case GameType.ZenTracing: return "ZenTracing";
            case GameType.IceShatter: return "IceShatter";
            case GameType.NameMemory: return "NameMemory";
            case GameType.RetroRacer: return "RetroRacer";
            case GameType.Choice: return "ChoiceGame";
            case GameType.Sequence: return "SequenceGame";
            case GameType.Swipe: return "SwipeGame";
            case GameType.MashRace: return "MashRace";
            case GameType.MemoryMatch: return "MemoryMatch";
            case GameType.TimingRing: return "TimingRing";
            case GameType.FallingSort: return "FallingSort";
            case GameType.WhackGrid: return "WhackGrid";
            case GameType.SweepFloor: return "SweepFloor";
            case GameType.PumpSystem: return "PumpGame";
            case GameType.ScrubPlate: return "ScrubPlate";
            case GameType.CodeBreaker: return "CodeBreaker";
            case GameType.BalanceMeter: return "BalanceMeter";
            case GameType.DialogueChain: return "DialogueChain";
            case GameType.OrderQueue: return "OrderQueue";
            case GameType.DragMatch: return "DragMatch";
            case GameType.SprintLane: return "SprintLane";
            case GameType.Tetris: return "Tetris";
            case GameType.AcidRainShield: return "AcidRain";
            case GameType.VideoCall: return "VideoCall";
            case GameType.KanbanBoard: return "Kanban";
            case GameType.FileSorter: return "FileSorter";
            case GameType.CoffeeWork: return "CoffeeWork";
            case GameType.UnitTestRunner: return "UnitTest";
            case GameType.CenterDiv: return "CenterDiv";
            case GameType.DragCenter: return "DragCenter";
            case GameType.SizeMatch: return "SizeMatch";
            case GameType.StealthInject: return "StealthInject";
            case GameType.AutomatedTests: return "AutomatedTests";
            case GameType.CodeCommenter: return "CodeCommenter";
            case GameType.DocumentSigner: return "DocumentSigner";
            case GameType.ChartAdjuster: return "ChartAdjuster";
            case GameType.VrArcadePong: return "VrArcade";
            case GameType.HotStoneSpa: return "HotStone";
            case GameType.ForgePass: return "ForgePass";
            case GameType.SnatchBriefcase: return "SnatchBriefcase";
            case GameType.SkimATM: return "SkimATM";
            case GameType.IpHack: return "IpHack";
            case GameType.NightShiftPitch: return "NightShift";
            case GameType.YesManNod: return "YesMan";
            case GameType.FormatDashboard: return "FormatDashboard";
            case GameType.FranticBackspace: return "FranticBackspace";
            case GameType.SentienceArray: return "SentienceArray";
            case GameType.ContainmentProtocol: return "ContainmentProtocol";
            case GameType.RewardLogic: return "RewardLogic";
            case GameType.GalacticCouncil: return "GalacticCouncil";
            case GameType.NeuralAligner: return "NeuralAligner";
            case GameType.FoodPrepRush: return "FoodPrep";
            case GameType.Tap: return "TapGame";
            case GameType.Slider: return "SliderGame";
            case GameType.TwoPhaseHeist: return "TwoPhaseHeist";
            case GameType.QATriage: return "QATriage";
            case GameType.TicketSwipe: return "TicketSwipe";
            case GameType.ConstructionSite: return "ConstructionSite";
            case GameType.GitMerge: return "GitMerge";
            case GameType.HospitalityGala: return "HospitalityGala";
            case GameType.CorporateGolf: return "CorporateGolf";
            case GameType.StartupPitch: return "StartupPitch";
            case GameType.MowGreens: return "MowGreens";
            case GameType.WalkDogs: return "WalkDogs";
            case GameType.RecycleBottles: return "RecycleBottles";
            default: return type.ToString();}}

    public void LaunchMiniGame(ActiveMiniGame game)
    {
        currentGame = game;
        currentConfig = GetConfigForGame(game.name);
        
        score = 0;
        hasWon = false;
        isPlaying = true;
        currentPhase = 1;

        if (activeGameRoutine != null) StopCoroutine(activeGameRoutine);

        operationNameText.text = game.name;
        instructionsText.text = currentConfig.instructions;
        overlay.style.display = DisplayStyle.Flex;
        overlay.pickingMode = PickingMode.Position;

        HideAllContainers();
        GameType typeToSetup = currentConfig.type;
        
        // Delegate to new Strategy Pattern
        currentStrategy = MiniGameFactory.Create(typeToSetup);

        // Phase 4 Automation: Automatically load UI templates if they exist
        string uxmlName = GetUxmlName(typeToSetup);
        var template = Resources.Load<VisualTreeAsset>("MiniGames/" + uxmlName);
        
        if (template != null)
        {
            activeGameUI = template.Instantiate();
            activeGameUI.style.flexGrow = 1; // Ensure it expands
            
            var targetContainer = overlay.Q<ScrollView>() as VisualElement ?? overlay;
            targetContainer.Add(activeGameUI);
            
            BindLegacyUI(activeGameUI);
            activeGameUI.Focus();
            
            currentStrategy.Initialize(this, activeGameUI, currentConfig);}
        else
        {
            // Fallback to legacy massive UI document
            currentStrategy.Initialize(this, miniGameDocument.rootVisualElement, currentConfig);}
        
        currentStrategy.StartGame();}

    private void HideAllContainers() { }

    public void SetupGameType(GameType type)
    {
        switch (type)
        {
            case GameType.CloggedVents:
                cvContainer.style.display = DisplayStyle.Flex;
                SetupCloggedVents();
                break;
            case GameType.QATriage:
                // Handled by Strategy
                break;
            case GameType.WhackABug:
                wbContainer.style.display = DisplayStyle.Flex;
                SetupWhackABug();
                break;


            case GameType.ZenTracing:
                ztContainer.style.display = DisplayStyle.Flex;
                SetupZenTracing();
                break;
            case GameType.IceShatter:
                isContainer.style.display = DisplayStyle.Flex;
                SetupIceShatter();
                break;
            case GameType.NameMemory:
                nmContainer.style.display = DisplayStyle.Flex;
                SetupNameMemory();
                break;
            case GameType.RetroRacer:
                rrContainer.style.display = DisplayStyle.Flex;
                SetupRetroRacer();
                break;


            case GameType.Choice:
                choiceContainer.style.display = DisplayStyle.Flex;
                btnChoice1.text = currentConfig.choices[0];
                btnChoice2.text = currentConfig.choices[1];
                btnChoice3.text = currentConfig.choices[2];
                break;

            case GameType.Sequence:
                sequenceContainer.style.display = DisplayStyle.Flex;
                if (btnReplayPattern != null)
                    btnReplayPattern.style.display = (currentGame.name == "Perform a triple backflip spin") ? DisplayStyle.None : DisplayStyle.Flex;
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
                playerMashProgress = 0f;
                opponentProgress = 0f;
                timer = currentConfig.timeLimit > 0 ? currentConfig.timeLimit : 10f;
                playerProgressBar.style.width = new Length(0, LengthUnit.Percent);
                opponentProgressBar.style.width = new Length(0, LengthUnit.Percent);
                
                if (currentGame.name == "Stress test server load")
                {
                    if (playerMashLabel != null) { playerMashLabel.text = "INCOMING TRAFFIC"; playerMashLabel.style.color = new StyleColor(ColorUtility.TryParseHtmlString("#e67e22", out Color c1) ? c1 : Color.yellow);}
                    if (opponentMashLabel != null) { opponentMashLabel.text = "SERVER LOAD (HEAT)"; opponentMashLabel.style.color = new StyleColor(ColorUtility.TryParseHtmlString("#e74c3c", out Color c2) ? c2 : Color.red);}
                    playerProgressBar.style.backgroundColor = new StyleColor(ColorUtility.TryParseHtmlString("#e67e22", out Color c3) ? c3 : Color.yellow);
                    opponentProgressBar.style.backgroundColor = new StyleColor(ColorUtility.TryParseHtmlString("#e74c3c", out Color c4) ? c4 : Color.red);
                    mashRaceContainer.style.backgroundColor = new StyleColor(new Color(0.1f, 0.05f, 0.05f));}
                else
                {
                    if (playerMashLabel != null) { playerMashLabel.text = "YOUR PROGRESS"; playerMashLabel.style.color = new StyleColor(ColorUtility.TryParseHtmlString("#2ECC71", out Color c5) ? c5 : Color.green);}
                    if (opponentMashLabel != null) { opponentMashLabel.text = "OPPONENT"; opponentMashLabel.style.color = new StyleColor(ColorUtility.TryParseHtmlString("#E74C3C", out Color c6) ? c6 : Color.red);}
                    playerProgressBar.style.backgroundColor = new StyleColor(ColorUtility.TryParseHtmlString("#2ECC71", out Color c7) ? c7 : Color.green);
                    opponentProgressBar.style.backgroundColor = new StyleColor(ColorUtility.TryParseHtmlString("#E74C3C", out Color c8) ? c8 : Color.red);
                    mashRaceContainer.style.backgroundColor = new StyleColor(Color.clear);}

                activeGameRoutine = StartCoroutine(MashRaceRoutine());
                break;

            case GameType.MemoryMatch:
                memoryMatchContainer.style.display = DisplayStyle.Flex;
                SetupMemoryMatch(currentConfig.pairs);
                break;

            case GameType.TimingRing:
                timingRingContainer.style.display = DisplayStyle.Flex;
                timingTargetAngle = Random.Range(0f, 360f);
                timingMarkerAngle = 0f;
                timingSpinDir = 1;
                timingCurrentRound = 0;
                activeGameRoutine = StartCoroutine(TimingRingRoutine());
                break;
                
            case GameType.FallingSort:
                fallingSortContainer.style.display = DisplayStyle.Flex;
                SetupFallingSort();
                sortScore = 0;
                timer = currentConfig.sortTimeLimit;
                activeGameRoutine = StartCoroutine(FallingSortRoutine());
                break;
                

            case GameType.WhackGrid:
                whackGridContainer.style.display = DisplayStyle.Flex;
                whackScore = 0;
                timer = currentConfig.whackTime;
                SetupWhackGrid();
                activeGameRoutine = StartCoroutine(WhackGridRoutine());
                break;
                
            case GameType.SweepFloor:
                sweepFloorContainer.style.display = DisplayStyle.Flex;
                SetupSweepFloor();
                miniGameDocument.rootVisualElement.Focus();
                activeGameRoutine = StartCoroutine(SweepFloorRoutine());
                break;
                
            case GameType.PumpSystem:
                pumpGameContainer.style.display = DisplayStyle.Flex;
                pumpProgress = 0f;
                pumpHoldTimer = 0f;
                pumpProgressBar.style.height = Length.Percent(0f);
                pumpHandle.style.top = Length.Percent(0f);
                
                if (currentConfig.pumpIsBalanceMode)
                {
                    pumpNormalBar.style.display = DisplayStyle.None;
                    pumpDialContainer.style.display = DisplayStyle.Flex;
                    
                    pumpHoldTimerContainer.style.display = DisplayStyle.Flex;
                    pumpHoldTimerBar.style.width = Length.Percent(0f);
                    
                    float minAngle = -120f + (currentConfig.pumpBalanceTargetMin * 240f);
                    float maxAngle = -120f + (currentConfig.pumpBalanceTargetMax * 240f);
                    float greenSweep = maxAngle - minAngle;
                    
                    float maskRot = 90f - greenSweep;
                    float zoneRot = minAngle - maskRot;
                    
                    pumpDialTargetZone.style.rotate = new StyleRotate(new Rotate(new Angle(zoneRot)));
                    pumpDialTargetArc.style.rotate = new StyleRotate(new Rotate(new Angle(maskRot)));
                    
                    pumpDialNeedleRotator.style.rotate = new StyleRotate(new Rotate(new Angle(-120f)));}
                else
                {
                    pumpNormalBar.style.display = DisplayStyle.Flex;
                    pumpDialContainer.style.display = DisplayStyle.None;
                    pumpHoldTimerContainer.style.display = DisplayStyle.None;}
                
                activeGameRoutine = StartCoroutine(PumpSystemRoutine());
                break;
                
            case GameType.ScrubPlate:
                scrubPlateContainer.style.display = DisplayStyle.Flex;
                SetupScrubPlate();
                activeGameRoutine = StartCoroutine(ScrubPlateRoutine());
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
                activeGameRoutine = StartCoroutine(DialogueChainRoutine());
                break;
                
            case GameType.OrderQueue:
                orderQueueContainer.style.display = DisplayStyle.Flex;
                if (currentGame.name == "Train new hire on POS system")
                {
                    orderQueueContainer.style.backgroundColor = new StyleColor(new Color(0.05f, 0.12f, 0.05f));
                    orderQueueContainer.style.borderTopWidth = 15;
                    orderQueueContainer.style.borderBottomWidth = 15;
                    orderQueueContainer.style.borderLeftWidth = 15;
                    orderQueueContainer.style.borderRightWidth = 15;
                    orderQueueContainer.style.borderTopColor = new StyleColor(new Color(0.85f, 0.85f, 0.8f));
                    orderQueueContainer.style.borderBottomColor = new StyleColor(new Color(0.85f, 0.85f, 0.8f));
                    orderQueueContainer.style.borderLeftColor = new StyleColor(new Color(0.85f, 0.85f, 0.8f));
                    orderQueueContainer.style.borderRightColor = new StyleColor(new Color(0.85f, 0.85f, 0.8f));
                    orderQueueContainer.style.borderTopLeftRadius = 20;
                    orderQueueContainer.style.borderTopRightRadius = 20;
                    orderQueueContainer.style.borderBottomLeftRadius = 20;
                    orderQueueContainer.style.borderBottomRightRadius = 20;
                    orderQueueContainer.style.paddingTop = 20;
                    orderQueueContainer.style.paddingBottom = 20;
                    
                    orderStatusText.style.color = new StyleColor(new Color(0.2f, 1f, 0.2f));
                    orderStatusText.text = "POS READY\nMEMORIZE INPUT:";}
                else if (currentGame.name == "Verify hotfix deployment")
                {
                    Color ghBg = ColorUtility.TryParseHtmlString("#0d1117", out Color c1) ? c1 : new Color(0.05f, 0.06f, 0.09f);
                    Color ghBorder = ColorUtility.TryParseHtmlString("#30363d", out Color c2) ? c2 : new Color(0.18f, 0.21f, 0.23f);
                    
                    orderQueueContainer.style.backgroundColor = new StyleColor(ghBg);
                    orderQueueContainer.style.borderTopWidth = 2;
                    orderQueueContainer.style.borderBottomWidth = 2;
                    orderQueueContainer.style.borderLeftWidth = 2;
                    orderQueueContainer.style.borderRightWidth = 2;
                    orderQueueContainer.style.borderTopColor = new StyleColor(ghBorder);
                    orderQueueContainer.style.borderBottomColor = new StyleColor(ghBorder);
                    orderQueueContainer.style.borderLeftColor = new StyleColor(ghBorder);
                    orderQueueContainer.style.borderRightColor = new StyleColor(ghBorder);
                    orderQueueContainer.style.borderTopLeftRadius = 10;
                    orderQueueContainer.style.borderTopRightRadius = 10;
                    orderQueueContainer.style.borderBottomLeftRadius = 10;
                    orderQueueContainer.style.borderBottomRightRadius = 10;
                    orderQueueContainer.style.paddingTop = 20;
                    orderQueueContainer.style.paddingBottom = 20;
                    
                    orderStatusText.style.color = new StyleColor(ColorUtility.TryParseHtmlString("#c9d1d9", out Color c3) ? c3 : Color.white);
                    orderStatusText.text = "VERIFY WORKFLOW RUN";}
                else
                {
                    orderQueueContainer.style.backgroundColor = new StyleColor(Color.clear);
                    orderQueueContainer.style.borderTopWidth = 0;
                    orderQueueContainer.style.borderBottomWidth = 0;
                    orderQueueContainer.style.borderLeftWidth = 0;
                    orderQueueContainer.style.borderRightWidth = 0;
                    orderQueueContainer.style.paddingTop = 0;
                    orderQueueContainer.style.paddingBottom = 0;
                    
                    orderStatusText.style.color = new StyleColor(new Color(0.94f, 0.76f, 0.05f));
                    orderStatusText.text = "MEMORIZE THE ORDER!";}
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
                
            case GameType.Tetris:
                tetrisContainer.style.display = DisplayStyle.Flex;
                SetupTetris();
                activeGameRoutine = StartCoroutine(TetrisRoutine());
                break;
                
            case GameType.AcidRainShield:
                acidRainContainer.style.display = DisplayStyle.Flex;
                acidBurn = 0f;
                acidSignAngle = -90f;
                acidSignPivot.style.rotate = new StyleRotate(new Rotate(new Angle(0f, AngleUnit.Degree)));
                acidBurnBar.style.width = new Length(0, LengthUnit.Percent);
                acidDropContainer.Clear();
                activeGameRoutine = StartCoroutine(AcidRainRoutine());
                break;

            case GameType.VideoCall:
                videoCallContainer.style.display = DisplayStyle.Flex;
                SetupVideoCall();
                break;
                
            case GameType.KanbanBoard:
                kanbanContainer.style.display = DisplayStyle.Flex;
                SetupKanbanBoard();
                break;
                
            case GameType.FileSorter:
                if (fileSorterContainer != null) fileSorterContainer.style.display = DisplayStyle.Flex;
                SetupFileSorter();
                break;

            case GameType.CoffeeWork:
                if (coffeeWorkContainer != null) coffeeWorkContainer.style.display = DisplayStyle.Flex;
                SetupCoffeeWork();
                break;
                
            case GameType.UnitTestRunner:
                unitTestContainer.style.display = DisplayStyle.Flex;
                SetupUnitTest();
                break;
                
            case GameType.CenterDiv:
                centerDivContainer.style.display = DisplayStyle.Flex;
                SetupCenterDiv();
                break;
                
            case GameType.DragCenter:
                if (dragCenterContainer != null) dragCenterContainer.style.display = DisplayStyle.Flex;
                dcPoints = 0;
                SetupDragCenter();
                break;
                
            case GameType.SizeMatch:
                if (sizeMatchContainer != null) sizeMatchContainer.style.display = DisplayStyle.Flex;
                smPoints = 0;
                SetupSizeMatch();
                break;

            case GameType.StealthInject:
                if (stealthInjectContainer != null) stealthInjectContainer.style.display = DisplayStyle.Flex;
                SetupStealthInject();
                break;

            case GameType.AutomatedTests:
                if (automatedTestsContainer != null) automatedTestsContainer.style.display = DisplayStyle.Flex;
                SetupAutomatedTests();
                break;
                
            case GameType.CodeCommenter:
                if (codeCommenterContainer != null) codeCommenterContainer.style.display = DisplayStyle.Flex;
                SetupCodeCommenter();
                break;

            case GameType.DocumentSigner:
                if (documentSignerContainer != null) documentSignerContainer.style.display = DisplayStyle.Flex;
                SetupDocumentSigner();
                break;
                
            case GameType.ChartAdjuster:
                if (chartAdjusterContainer != null) chartAdjusterContainer.style.display = DisplayStyle.Flex;
                SetupChartAdjuster();
                break;
                




            case GameType.VrArcadePong:
                if (vrArcadeContainer != null) vrArcadeContainer.style.display = DisplayStyle.Flex;
                SetupVrArcadePong();
                break;
            case GameType.HotStoneSpa:
                if (hotStoneContainer != null) hotStoneContainer.style.display = DisplayStyle.Flex;
                SetupHotStoneSpa();
                break;
            case GameType.MowGreens:
                if (mgContainer != null) mgContainer.style.display = DisplayStyle.Flex;
                SetupMowGreens();
                break;
            case GameType.WalkDogs:
                if (wdContainer != null) wdContainer.style.display = DisplayStyle.Flex;
                SetupWalkDogs();
                break;
            case GameType.TwoPhaseHeist:
                if (tphContainer != null) tphContainer.style.display = DisplayStyle.Flex;
                SetupTwoPhaseHeist();
                break;
            case GameType.ForgePass:
                if (fpContainer != null) fpContainer.style.display = DisplayStyle.Flex;
                SetupForgePass();
                break;
            case GameType.SnatchBriefcase:
                if (snatchBriefcaseContainer != null) snatchBriefcaseContainer.style.display = DisplayStyle.Flex;
                SetupSnatchBriefcase();
                break;
            case GameType.SkimATM:
                if (saContainer != null) saContainer.style.display = DisplayStyle.Flex;
                SetupSkimATM();
                break;
            case GameType.IpHack:
                if (ipHackContainer != null) ipHackContainer.style.display = DisplayStyle.Flex;
                SetupIpHack();
                break;
            case GameType.HospitalityGala:
                if (hgContainer != null) hgContainer.style.display = DisplayStyle.Flex;
                SetupHospitalityGala();
                break;
            case GameType.StartupPitch:
                if (spContainer != null) spContainer.style.display = DisplayStyle.Flex;
                SetupStartupPitch();
                break;
            case GameType.CorporateGolf:
                if (cgContainer != null) cgContainer.style.display = DisplayStyle.Flex;
                SetupCorporateGolf();
                break;
            case GameType.NightShiftPitch:
                if (nightShiftContainer != null) nightShiftContainer.style.display = DisplayStyle.Flex;
                SetupNightShiftPitch();
                break;
            case GameType.YesManNod:
                if (yesManContainer != null) yesManContainer.style.display = DisplayStyle.Flex;
                SetupYesManNod();
                break;
            case GameType.FormatDashboard:
                if (formatDashboardContainer != null) formatDashboardContainer.style.display = DisplayStyle.Flex;
                SetupFormatDashboard();
                break;
            case GameType.FranticBackspace:
                if (franticBackspaceContainer != null) franticBackspaceContainer.style.display = DisplayStyle.Flex;
                SetupFranticBackspace();
                break;
            case GameType.SentienceArray:
                if (sentienceArrayContainer != null) sentienceArrayContainer.style.display = DisplayStyle.Flex;
                SetupSentienceArray();
                break;
            case GameType.ContainmentProtocol:
                if (containmentProtocolContainer != null) containmentProtocolContainer.style.display = DisplayStyle.Flex;
                SetupContainmentProtocol();
                break;
            case GameType.RewardLogic:
                if (rewardLogicContainer != null) rewardLogicContainer.style.display = DisplayStyle.Flex;
                SetupRewardLogic();
                break;
            case GameType.GalacticCouncil:
                if (galacticCouncilContainer != null) galacticCouncilContainer.style.display = DisplayStyle.Flex;
                SetupGalacticCouncil();
                break;
            case GameType.NeuralAligner:
                if (neuralAlignerContainer != null) neuralAlignerContainer.style.display = DisplayStyle.Flex;
                SetupNeuralAligner();
                break;

            case GameType.FoodPrepRush:
                foodPrepContainer.style.display = DisplayStyle.Flex;
                score = 0;
                timer = currentConfig.timeLimit > 0 ? currentConfig.timeLimit : 15f;
                prepTimerText.text = $"{timer:0.0}s";
                prepScoreText.text = $"Prepped: 0 / {currentConfig.targetScore}";
                for(int i=0; i<4; i++)
                {
                    prepProgress[i] = 0f;
                    prepFreezes[i] = 0f;
                    prepBars[i].style.height = new Length(0, LengthUnit.Percent);
                    prepStatus[i].text = "";}
                activeGameRoutine = StartCoroutine(FoodPrepRoutine());
                break;}}

    private void SetupFallingSort()
    {
        fallZone.Clear();
        binsRow.Clear();
        
        Color[] binColors = new Color[] {
            new Color(0.9f, 0.3f, 0.3f), // Red
            new Color(0.3f, 0.8f, 0.3f), // Green
            new Color(0.9f, 0.8f, 0.2f), // Yellow};
        
        for (int i = 0; i < currentConfig.binLabels.Length; i++)
        {
            var bin = new VisualElement();
            bin.style.flexGrow = 1;
            bin.style.backgroundColor = new StyleColor(binColors[i % binColors.Length]);
            bin.style.borderLeftWidth = 4;
            bin.style.borderRightWidth = 4;
            bin.style.borderTopWidth = 0;
            bin.style.borderBottomWidth = 4;
            bin.style.borderLeftColor = new StyleColor(Color.black);
            bin.style.borderRightColor = new StyleColor(Color.black);
            bin.style.borderBottomColor = new StyleColor(Color.black);
            
            // Bin Shape
            bin.style.borderBottomLeftRadius = 20;
            bin.style.borderBottomRightRadius = 20;
            bin.style.borderTopLeftRadius = 5;
            bin.style.borderTopRightRadius = 5;
            
            bin.style.marginRight = 10;
            bin.style.marginLeft = 10;
            bin.style.justifyContent = Justify.Center;
            bin.style.alignItems = Align.Center;
            
            // Extract just the emoji if present, else use full text
            string fullText = currentConfig.fallingItems[i];
            string emojiText = fullText;
            if (fullText.Contains(" ")) emojiText = fullText.Split(' ')[0];
            
            var label = new Label(emojiText); 
            label.style.fontSize = 50;
            label.style.color = Color.white;
            bin.Add(label);
            
            binsRow.Add(bin);}}
    public void MarkWinReady()
    {
        if (currentConfig.type == GameType.TwoPhaseHeist)
        {
            if (currentPhase == 1) {
                currentPhase = 2;
                if (instructionsText != null) instructionsText.text = "Phase 2: HACK!";
                SetupGameType(currentConfig.phase2Type);
                return;
            } else if (currentPhase == 2 && currentConfig.phase3Type != 0) {
                currentPhase = 3;
                if (instructionsText != null) instructionsText.text = "Phase 3: ESCAPE!";
                SetupGameType(currentConfig.phase3Type);
                return;
            }
        }

        isPlaying = false;
        hasWon = true;
        if (activeGameRoutine != null) StopCoroutine(activeGameRoutine);

        FinishGame();
    }

    public void AbortGame()
    {
        isPlaying = false;
        if (activeGameRoutine != null) StopCoroutine(activeGameRoutine);
        
        currentStrategy?.AbortGame();

        overlay.style.display = DisplayStyle.None;
        
        if (activeGameUI != null)
        {
            activeGameUI.RemoveFromHierarchy();
            activeGameUI = null;
        }

        if (currentGame != null && currentGame.category == "side_hustle")
        {
            int jailWeeks = 0;
            if (currentGame.name == "Forge Transit Passes") jailWeeks = 1;
            else if (currentGame.name == "Crack Luxury Briefcase") jailWeeks = 2;
            else if (currentGame.name == "Skim ATM Card Data") jailWeeks = 4;
            else if (currentGame.name == "Corporate Espionage Heist") jailWeeks = 7;
            
            if (jailWeeks > 0)
            {
                UnityEngine.Debug.Log("Failed illegal minigame! Straight to jail.");
                var gData = GameManager.Instance.currentData;
                float fine = (float)(gData.balance * 0.5); 
                gData.balance -= fine;
                gData.jailWeeksRemaining = jailWeeks;
                string logMessage = $"[SYSTEM] Failed {currentGame.name} AND WAS CAUGHT! Fined ${fine:F0} and jailed.";
                GameManager.Instance.activeNewsFeed = logMessage;
                GameManager.Instance.financialHistory.Add(new FinancialSnapshot(gData.currentWeek, gData.balance, 0, -fine));
                
                var gUIManager = GameUIManager.Instance;
                if (gUIManager != null) gUIManager.CheckJailStatus();
                return; 
            }
        }

        overlay.pickingMode = PickingMode.Ignore;
        var data = GameManager.Instance.currentData;

        if (currentGame.category == "side_hustle")
        {
            var hustle = HustlesUI.availableHustles.Find(h => h.name == currentGame.name);
            if (hustle != null && hustle.riskPercent > 0)
            {
                data.jailWeeksRemaining = hustle.jailSentence;
                data.jobTitle = "Unemployed";
                data.zone = District.Slums;
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
        var uiManager = GameUIManager.Instance;
        uiManager?.CheckJailStatus();
    }

    private void FinishGame()
    {
        if (!hasWon) return;

        isPlaying = false;
        currentStrategy?.AbortGame();
        overlay.style.display = DisplayStyle.None;

        if (activeGameUI != null)
        {
            activeGameUI.RemoveFromHierarchy();
            activeGameUI = null;
        }
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
                    data.zone = District.Slums;
                    double fine = data.balance * 0.20;
                    data.balance -= fine;
                    data.historyLogsJson += $@"

[SYSTEM] {logMessage} BUT I WAS CAUGHT! Fined ${fine:F0} and jailed.";
                }
                else
                {
                    data.balance += currentGame.reward;
                    data.historyLogsJson += $@"

[SYSTEM] {logMessage} Got away with ${currentGame.reward}.";
                }
            }
            else
            {
                data.balance += currentGame.reward;
                data.stress = Mathf.Clamp(data.stress + 0.10f, 0f, 1f);
                data.historyLogsJson += $@"

[SYSTEM] {logMessage} Earned ${currentGame.reward}.";
            }
            
            var uiManager = GameUIManager.Instance;
            uiManager?.ShowPopup(new RandomEvent { 
                title = "HUSTLE COMPLETED", 
                description = $@"You finished: {currentGame.name}.

Check the dashboard for results!" 
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
            data.historyLogsJson += $@"

[SYSTEM] {logMessage} -{(currentGame.stressCost * 100):F0}% stress";
            
            var uiManager = GameUIManager.Instance;
            uiManager?.ShowPopup(new RandomEvent { 
                title = "WELLNESS PURCHASED", 
                description = $@"You successfully completed: {currentGame.name}.

<color=#2ECC71>Your stress index has dropped significantly.</color>

<color=#E74C3C>Cost: ${currentGame.reward}</color>" 
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
            data.historyLogsJson += $@"

[SYSTEM] {logMessage} +{(currentGame.stressCost * 100):F0}% luck";
            
            var uiManager = GameUIManager.Instance;
            uiManager?.ShowPopup(new RandomEvent { 
                title = "NETWORK MIXER ATTENDED", 
                description = $@"You successfully networked at {currentGame.name}.

<color=#2ECC71>Your promotion luck in this sector has increased!</color>

<color=#E74C3C>Cost: ${currentGame.reward}</color>" 
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
            data.historyLogsJson += $@"

[SYSTEM] {logMessage} +{(currentGame.stressCost * 100):F0}% stress";
            
            var uiManager = GameUIManager.Instance;
            uiManager?.ShowPopup(new RandomEvent { 
                title = "TASK COMPLETED", 
                description = $@"You finished: {currentGame.name}.

<color=#E74C3C>Stress increased by {(currentGame.stressCost * 100):F0}%.</color>

Weekly Tasks Done: {data.completedTasksThisWeek.Count}/3" 
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
            data.historyLogsJson += $@"

[SYSTEM] {logMessage} Promotion Readiness +{(currentGame.readinessGain * 100):F0}%";
            
            var uiManager = GameUIManager.Instance;
            uiManager?.ShowPopup(new RandomEvent { 
                title = "EXTRA INITIATIVE TAKEN", 
                description = $@"You went above and beyond completing {currentGame.name}.

<color=#2ECC71>Performance Readiness increased!</color>
<color=#E74C3C>Stress increased by {(currentGame.stressCost * 100):F0}%.</color>" 
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
        var uiManagerForDashboard = GameUIManager.Instance;
        uiManagerForDashboard?.CheckJailStatus();
    }


}

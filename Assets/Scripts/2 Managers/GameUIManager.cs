using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance { get; private set; }

    [Header("UI Documents")]
    public UIDocument creationDocument; 
    public UIDocument dashboardDocument; 
    public UIDocument careersDocument;
    public UIDocument hustlesDocument;
    public UIDocument educationDocument;
    public UIDocument activitiesDocument;
    public UIDocument jailDocument; 

    private VisualElement creationRoot, dashboardRoot, careersRoot, hustlesRoot, educationRoot, activitiesRoot, jailRoot;
    
    // Creation Screen Elements
    private VisualElement step1Container, step2Container;
    private TextField nameInput;
    private Button btnRandomizeBackstory, beginButton;
    private Label backstoryTitleText, backstoryDescText, startCashText, startStressText;
    private PlayerBackstory selectedBackstory;

    // Dashboard Elements
    private Label playerNameText, subTitleText, cashText, expenseText, jobText, districtText, logText, jailWeeksText;
    private Label stressVal, luckVal, perfVal;
    private VisualElement stressFill, luckFill, perfFill;
    private Button btnServeWeek;

    // Settings Elements
    private VisualElement settingsOverlay;
    private Button btnOpenSettings, btnCloseSettings, btnWipeHistory;
    private Button btnToggleTheme;
    private Button btnToggleDevConsole;
    private ScrollView terminalScrollView;
    private VisualElement terminalCard;
    private Label historyLabel;

    private VisualElement activeOverlay = null;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        creationRoot = creationDocument.rootVisualElement;
        dashboardRoot = dashboardDocument.rootVisualElement;
        careersRoot = careersDocument.rootVisualElement;
        hustlesRoot = hustlesDocument.rootVisualElement;
        educationRoot = educationDocument.rootVisualElement;
        activitiesRoot = activitiesDocument.rootVisualElement;
        jailRoot = jailDocument.rootVisualElement;

        // Setup Creation
        step1Container = creationRoot.Q<VisualElement>("Step1Container");
        step2Container = creationRoot.Q<VisualElement>("Step2Container");
        nameInput = creationRoot.Q<TextField>("NameInput");
        
        btnRandomizeBackstory = creationRoot.Q<Button>("BtnRandomizeBackstory");
        btnRandomizeBackstory.clicked += OnRandomizeBackstoryClicked;

        beginButton = creationRoot.Q<Button>("BeginButton");
        beginButton.clicked += OnBeginClicked;

        backstoryTitleText = creationRoot.Q<Label>("BackstoryTitleText");
        backstoryDescText = creationRoot.Q<Label>("BackstoryDescText");
        startCashText = creationRoot.Q<Label>("StartCashText");
        startStressText = creationRoot.Q<Label>("StartStressText");
        
        var greetingText = creationRoot.Q<Label>("GreetingText");
        var luckServiceText = creationRoot.Q<Label>("LuckServiceText");
        var luckHospitalityText = creationRoot.Q<Label>("LuckHospitalityText");
        var luckTechText = creationRoot.Q<Label>("LuckTechText");
        var luckCorpText = creationRoot.Q<Label>("LuckCorpText");

        // Setup Dashboard
        playerNameText = dashboardRoot.Q<Label>("PlayerNameText");
        subTitleText = dashboardRoot.Q<Label>("SubTitleText");
        cashText = dashboardRoot.Q<Label>("CashText");
        expenseText = dashboardRoot.Q<Label>("ExpenseText");
        jobText = dashboardRoot.Q<Label>("JobText");
        districtText = dashboardRoot.Q<Label>("DistrictText");
        logText = dashboardRoot.Q<Label>("LogText");
        
        stressVal = dashboardRoot.Q<Label>("StressVal");
        luckVal = dashboardRoot.Q<Label>("LuckVal");
        perfVal = dashboardRoot.Q<Label>("PerfVal");
        
        stressFill = dashboardRoot.Q<VisualElement>("StressFill");
        luckFill = dashboardRoot.Q<VisualElement>("LuckFill");
        perfFill = dashboardRoot.Q<VisualElement>("PerfFill");

        // Setup Settings Overlay
        settingsOverlay = dashboardRoot.Q<VisualElement>("SettingsOverlay");
        btnOpenSettings = dashboardRoot.Q<Button>("BtnOpenSettings");
        btnCloseSettings = dashboardRoot.Q<Button>("BtnCloseSettings");
        btnWipeHistory = dashboardRoot.Q<Button>("BtnWipeHistory");
        
        btnToggleTheme = dashboardRoot.Q<Button>("BtnToggleTheme");
        btnToggleDevConsole = dashboardRoot.Q<Button>("BtnToggleDevConsole");
        
        terminalScrollView = dashboardRoot.Q<ScrollView>("TerminalScrollView");
        terminalCard = dashboardRoot.Q<VisualElement>("TerminalCard");
        
        historyLabel = dashboardRoot.Q<Label>("HistoryLabel");

        if (btnOpenSettings != null) btnOpenSettings.clicked += OpenSettings;
        if (btnCloseSettings != null) btnCloseSettings.clicked += CloseSettings;
        if (btnWipeHistory != null) btnWipeHistory.clicked += PurgeAllData;

        if (btnToggleTheme != null) btnToggleTheme.clicked += ToggleTheme;
        
        if (btnToggleDevConsole != null) 
        {
            btnToggleDevConsole.clicked += () => {
                bool current = PlayerPrefs.GetInt("showDevConsole", 0) == 1;
                PlayerPrefs.SetInt("showDevConsole", current ? 0 : 1);
                RefreshSettingsButtons();
                RefreshLogDisplay();
            };
        }
        
        RefreshSettingsButtons();

        // Hook up the Console interceptor
        Application.logMessageReceived += OnDebugLogReceived;

        // BINDINGS (Updated to fix obsolete warnings)
        dashboardRoot.Q<Button>("BtnOpenMigration")?.RegisterCallback<ClickEvent>(ev => FindAnyObjectByType<MigrationUI>()?.OpenPortal());
        dashboardRoot.Q<Label>("CashText")?.parent.RegisterCallback<ClickEvent>(ev => FindAnyObjectByType<FinancialGraphUI>()?.OpenGraph());
        dashboardRoot.Q<Label>("ExpenseText")?.parent.RegisterCallback<ClickEvent>(ev => FindAnyObjectByType<FinancialGraphUI>()?.OpenGraph());

        // Setup Jail
        jailWeeksText = jailRoot.Q<Label>("JailWeeksText");
        btnServeWeek = jailRoot.Q<Button>("BtnServeWeek");
        btnServeWeek.clicked += OnAdvanceWeekClicked;

        // Wire Up Nav Bars & Top Cards for ALL screens
        SetupNavBar(dashboardRoot);
        SetupNavBar(careersRoot);
        SetupNavBar(hustlesRoot);
        SetupNavBar(educationRoot);
        SetupNavBar(activitiesRoot);

        SetupTopCard(dashboardRoot);
        SetupTopCard(careersRoot);
        SetupTopCard(hustlesRoot);
        SetupTopCard(educationRoot);
        SetupTopCard(activitiesRoot);

        HideAllTabs();

    }

    private void SetupTopCard(VisualElement root)
    {
        if (root == null) return;
        
        var btnViewCV = root.Q<Button>("BtnViewCV");
        if (btnViewCV != null)
        {
            btnViewCV.clicked -= ShowCVPopup;
            btnViewCV.clicked += ShowCVPopup;
        }
        
        root.Q<VisualElement>("ColCash")?.RegisterCallback<ClickEvent>(ev => {
            ShowPopup(new RandomEvent { 
                title = "CASH RESERVE", 
                description = "Your liquid synthetic cash. Used to pay weekly expenses, enroll in courses, or purchase wellness escapes.\n\n<color=#E74C3C>WARNING: If your cash drops below zero and you cannot pay rent, you will be evicted and face game over.</color>" 
            });
        });
        
        root.Q<VisualElement>("ColExpenses")?.RegisterCallback<ClickEvent>(ev => {
            FindAnyObjectByType<EventPopupUI>()?.ShowEvent(new RandomEvent { 
                title = "WEEKLY EXPENSES", 
                description = "The automatic deductions that hit your account every week. This includes housing, food, and active education costs." 
            });
        });
        
        root.Q<VisualElement>("ColJob")?.RegisterCallback<ClickEvent>(ev => {
            FindAnyObjectByType<EventPopupUI>()?.ShowEvent(new RandomEvent { 
                title = "CURRENT JOB", 
                description = "Your primary source of income. Failing to click '+ Week' enough times to fulfill your duties may result in termination." 
            });
        });
        
        root.Q<VisualElement>("ColDistrict")?.RegisterCallback<ClickEvent>(ev => {
            FindAnyObjectByType<MigrationUI>()?.OpenPortal();
        });
        
        // Ensure Settings gear also works globally
        var btnSettings = root.Q<Button>("BtnOpenSettings");
        if (btnSettings != null) {
            btnSettings.clicked -= OpenSettings; // Prevent double firing
            btnSettings.clicked += OpenSettings;
        }
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= OnDebugLogReceived;
    }

    private void Start()
    {
        // Initialize all tab controllers (they are plain C# classes, not MonoBehaviours)
        CareerUI.Instance = new CareerUI();
        CareerUI.Instance.Initialize(careersDocument);

        HustlesUI.Instance = new HustlesUI();
        HustlesUI.Instance.Initialize(hustlesDocument);

        EducationUI.Instance = new EducationUI();
        EducationUI.Instance.Initialize(educationDocument);

        ActivitiesUI.Instance = new ActivitiesUI();
        ActivitiesUI.Instance.Initialize(activitiesDocument);

        Debug.Log("[GameUIManager] All tab controllers initialized.");

        // Strictly check if a save file actually exists on the hard drive
        if (SaveSystem.LoadActiveGame() != null)
        {
            Debug.Log("[GameUIManager] Save file found on disk! Skipping character creation.");
            CheckJailStatus(); 
        }
        else
        {
            Debug.Log("[GameUIManager] No save file found. Starting character creation.");
            HideAllTabs();
            creationRoot.style.display = DisplayStyle.Flex;
            creationRoot.pickingMode = PickingMode.Position;
            step1Container.style.display = DisplayStyle.Flex;
            step2Container.style.display = DisplayStyle.None;
        }

        // Initialize Debug Menu
        SetupDebugMenu();
    }

    private void SetupDebugMenu()
    {
        if (dashboardRoot == null) return;
        var btnDebug = dashboardRoot.Q<Button>("BtnDebugGames");
        var debugOverlay = dashboardRoot.Q<VisualElement>("DebugGamesOverlay");
        var btnCloseDebug = dashboardRoot.Q<Button>("BtnCloseDebugGames");
        var debugList = dashboardRoot.Q<ScrollView>("DebugGamesList");
        
        if (btnDebug != null)
        {
            btnDebug.clicked += () => {
                debugOverlay.style.display = DisplayStyle.Flex;
                debugOverlay.BringToFront();
                debugList.Clear();
                if (MiniGameManager.Instance != null && MiniGameManager.Instance.GameConfigs != null)
                {
                    foreach (var kvp in MiniGameManager.Instance.GameConfigs)
                    {
                        bool isCompleted = kvp.Value.type == GameType.KanbanBoard || kvp.Value.type == GameType.ScrubPlate || kvp.Value.type == GameType.SweepFloor || kvp.Value.type == GameType.PumpSystem || kvp.Value.type == GameType.FallingSort || kvp.Value.type == GameType.DragMatch || kvp.Value.type == GameType.GridDodge || kvp.Value.type == GameType.Tetris || kvp.Value.type == GameType.DialogueChain || kvp.Value.type == GameType.CoffeeWork || kvp.Value.type == GameType.UnitTestRunner || kvp.Value.type == GameType.CenterDiv || kvp.Value.type == GameType.DragCenter || kvp.Value.type == GameType.SizeMatch || kvp.Value.type == GameType.StealthInject || kvp.Value.type == GameType.AutomatedTests || kvp.Value.type == GameType.CodeCommenter || kvp.Key == "Format summary dashboards" || kvp.Key == "Respond to supervisor" || kvp.Key == "Stay overnight to align marketing pitch variables" || kvp.Key == "Agree verbally with automated executive complaints" || kvp.Key == "Capsule Arcade VR" || kvp.Key == "Monitor sentience detection array" || kvp.Key == "Emergency AI containment protocol" || kvp.Key == "Align superintelligence reward function" || kvp.Key == "Present findings to Galactic Council" || kvp.Key == "Calibrate neural alignment matrix" || kvp.Key == "Approve quarterly budget" || kvp.Key == "Board meeting presentation" || kvp.Key == "Verify hotfix deployment" || kvp.Key == "Stress test server load" || kvp.Key == "Write flaky test workaround" || kvp.Key == "Dance with sign for tourists" || kvp.Key == "Perform a triple backflip spin" || kvp.Key == "Spin arrow sign in traffic" || kvp.Key == "Hold sign in acid rain storm" || kvp.Key == "Fight rival sign spinner" || kvp.Key == "Handle customer complaint" || kvp.Key == "Emergency food prep rush" || kvp.Key == "Train new hire on POS system" || kvp.Key == "Send weekly TPS reports" || kvp.Key == "Attend 3-hour video call" || kvp.Key == "Reorganize shared drive" || kvp.Key == "Resolve Git merge conflicts" || kvp.Key == "Fix CSS layout sizes" || kvp.Key == "Fix CSS layout locations" || kvp.Key == "Walk in the Park" || kvp.Key == "Capsule Arcade VR" || kvp.Key == "Volcanic Hot Stone Spa" || kvp.Key == "Suburban Sensory Mud Spa" || kvp.Key == "Luxury Cryo Therapy" || kvp.Key == "Industrial Dive Bar";
                        var b = new Button { text = (isCompleted ? "✅ " : "") + kvp.Key };
                        b.style.paddingTop = 20; b.style.paddingBottom = 20; b.style.marginBottom = 10;
                        b.style.fontSize = 28; b.style.color = Color.white;
                        b.style.backgroundColor = new StyleColor(new Color(0.12f, 0.16f, 0.23f));
                        b.style.borderTopWidth = 2; b.style.borderBottomWidth = 2; b.style.borderLeftWidth = 2; b.style.borderRightWidth = 2;
                        b.style.borderTopColor = new StyleColor(new Color(0.22f, 0.74f, 0.97f));
                        b.style.borderBottomColor = new StyleColor(new Color(0.22f, 0.74f, 0.97f));
                        b.style.borderLeftColor = new StyleColor(new Color(0.22f, 0.74f, 0.97f));
                        b.style.borderRightColor = new StyleColor(new Color(0.22f, 0.74f, 0.97f));
                        b.clicked += () => {
                            debugOverlay.style.display = DisplayStyle.None;
                            var active = new ActiveMiniGame("Debug", kvp.Key, canFail: false);
                            MiniGameManager.Instance.LaunchMiniGame(active);
                        };
                        debugList.Add(b);
                    }
                }
            };
            
            if (btnCloseDebug != null)
            {
                btnCloseDebug.clicked += () => debugOverlay.style.display = DisplayStyle.None;
            }
        }
    }


    private void OnDebugLogReceived(string condition, string stackTrace, LogType type)
    {
        if (GameManager.Instance == null || GameManager.Instance.currentData == null) return;
        
        if (type == LogType.Log || type == LogType.Warning)
        {
            var data = GameManager.Instance.currentData;
            string prefix = type == LogType.Warning ? "<color=#E74C3C>[ALERT]</color>" : "<color=#38BDF8>[SYS]</color>";
            string newEntry = $"{prefix} {condition}\n\n";
            data.historyLogsJson = data.historyLogsJson + newEntry;
            
            if (data.historyLogsJson.Length > 4000) data.historyLogsJson = data.historyLogsJson.Substring(data.historyLogsJson.Length - 4000);
            
            RefreshLogDisplay();
        }
    }

    private void SetupNavBar(VisualElement root)
    {
        root.Q<Button>("BtnCareers")?.RegisterCallback<ClickEvent>(ev => SwitchTab(careersRoot));
        root.Q<Button>("BtnHustles")?.RegisterCallback<ClickEvent>(ev => SwitchTab(hustlesRoot));
        root.Q<Button>("BtnEducation")?.RegisterCallback<ClickEvent>(ev => SwitchTab(educationRoot));
        root.Q<Button>("BtnActivities")?.RegisterCallback<ClickEvent>(ev => SwitchTab(activitiesRoot));
        root.Q<Button>("BtnAdvanceWeek")?.RegisterCallback<ClickEvent>(ev => OnAdvanceWeekClicked());

        var footerStats = root.Query<VisualElement>(className: "footer-stat").ToList();
        if (footerStats.Count >= 3)
        {
            footerStats[0].RegisterCallback<ClickEvent>(ev => SwitchTab(careersRoot));    // Performance -> Careers
            footerStats[1].RegisterCallback<ClickEvent>(ev => ShowLuckStatsPopup(root));  // Job Luck -> Luck Popup
            footerStats[2].RegisterCallback<ClickEvent>(ev => SwitchTab(activitiesRoot)); // Stress -> Activities
        }
    }

    private void ShowLuckStatsPopup(VisualElement activeRoot)
    {
        var data = GameManager.Instance.currentData;
        
        var overlay = new VisualElement();
        overlay.AddToClassList("popup-overlay");
        overlay.style.position = Position.Absolute;
        overlay.style.left = 0; overlay.style.top = 0; overlay.style.right = 0; overlay.style.bottom = 0;
        overlay.style.backgroundColor = new StyleColor(new Color(0, 0, 0, 0.85f));
        overlay.style.alignItems = Align.Center;
        overlay.style.justifyContent = Justify.Center;

        var panel = new VisualElement();
        panel.AddToClassList("popup-panel");
        panel.style.backgroundColor = new StyleColor(Color.white);
        panel.style.borderTopLeftRadius = 40; panel.style.borderTopRightRadius = 40; panel.style.borderBottomLeftRadius = 40; panel.style.borderBottomRightRadius = 40;
        panel.style.width = Length.Percent(90);
        panel.style.maxWidth = 900;
        panel.style.paddingTop = 60; panel.style.paddingBottom = 60; panel.style.paddingLeft = 60; panel.style.paddingRight = 60;
        
        var title = new Label("Career Luck Statistics");
        title.style.fontSize = 60; title.style.unityFontStyleAndWeight = FontStyle.Bold; title.style.color = new StyleColor(new Color(0.01f, 0.53f, 0.82f)); // #0288D1
        title.style.marginBottom = 40; title.style.whiteSpace = WhiteSpace.Normal;
        
        var desc = new Label("Professional networking permanently raises your application acceptance rates. Higher stress levels apply severe interview luck penalties.");
        desc.style.fontSize = 32; desc.style.color = new StyleColor(new Color(0.5f, 0.55f, 0.55f)); // #7F8C8D
        desc.style.marginBottom = 50; desc.style.whiteSpace = WhiteSpace.Normal;

        panel.Add(title);
        panel.Add(desc);

        // Helper func to build a progress row
        System.Action<string, float> CreateRow = (label, luckVal) => {
            var rowContainer = new VisualElement();
            rowContainer.style.marginBottom = 30;

            var textRow = new VisualElement();
            textRow.style.flexDirection = FlexDirection.Row;
            textRow.style.justifyContent = Justify.SpaceBetween;
            textRow.style.marginBottom = 10;

            var nameLbl = new Label(label);
            nameLbl.style.fontSize = 34; nameLbl.style.unityFontStyleAndWeight = FontStyle.Bold; nameLbl.style.color = new StyleColor(new Color(0.17f, 0.24f, 0.31f));

            var valLbl = new Label($"{(luckVal * 100):F0}%");
            valLbl.style.fontSize = 34; valLbl.style.unityFontStyleAndWeight = FontStyle.Bold; valLbl.style.color = new StyleColor(new Color(0.01f, 0.53f, 0.82f));

            textRow.Add(nameLbl);
            textRow.Add(valLbl);

            var track = new VisualElement();
            track.AddToClassList("progress-track");
            track.style.backgroundColor = new StyleColor(new Color(0.9f, 0.91f, 0.91f));
            track.style.height = 16; track.style.borderBottomLeftRadius = 8; track.style.borderBottomRightRadius = 8; track.style.borderTopLeftRadius = 8; track.style.borderTopRightRadius = 8;

            var fill = new VisualElement();
            fill.AddToClassList("progress-fill");
            fill.style.backgroundColor = new StyleColor(new Color(0.01f, 0.53f, 0.82f));
            fill.style.height = 16; fill.style.borderBottomLeftRadius = 8; fill.style.borderBottomRightRadius = 8; fill.style.borderTopLeftRadius = 8; fill.style.borderTopRightRadius = 8;
            fill.style.width = Length.Percent(luckVal * 100f);

            track.Add(fill);
            rowContainer.Add(textRow);
            rowContainer.Add(track);

            panel.Add(rowContainer);
        };

        CreateRow("Manual/Service (Dishwasher)", GameManager.Instance.GetEffectiveLuck("manual"));
        CreateRow("Hospitality/Shift Manager", GameManager.Instance.GetEffectiveLuck("hospitality"));
        CreateRow("Tech/Junior Dev", GameManager.Instance.GetEffectiveLuck("tech"));
        CreateRow("Corporate/Admin", GameManager.Instance.GetEffectiveLuck("corp"));

        // Average
        float avgLuck = (GameManager.Instance.GetEffectiveLuck("manual") + GameManager.Instance.GetEffectiveLuck("hospitality") + GameManager.Instance.GetEffectiveLuck("tech") + GameManager.Instance.GetEffectiveLuck("corp")) / 4f;
        
        var avgBlock = new VisualElement();
        avgBlock.style.backgroundColor = new StyleColor(new Color(0.97f, 0.98f, 0.98f));
        avgBlock.style.borderBottomLeftRadius = 16; avgBlock.style.borderBottomRightRadius = 16; avgBlock.style.borderTopLeftRadius = 16; avgBlock.style.borderTopRightRadius = 16;
        avgBlock.style.paddingTop = 25; avgBlock.style.paddingBottom = 25; avgBlock.style.paddingLeft = 25; avgBlock.style.paddingRight = 25;
        avgBlock.style.flexDirection = FlexDirection.Row;
        avgBlock.style.justifyContent = Justify.SpaceBetween;
        avgBlock.style.marginBottom = 50;

        var avgTitle = new Label("AVERAGE ADMISSION LUCK");
        avgTitle.style.fontSize = 28; avgTitle.style.unityFontStyleAndWeight = FontStyle.Bold; avgTitle.style.color = new StyleColor(new Color(0.07f, 0.13f, 0.2f));

        var avgVal = new Label($"{(avgLuck * 100):F0}%");
        avgVal.style.fontSize = 34; avgVal.style.unityFontStyleAndWeight = FontStyle.Bold; avgVal.style.color = new StyleColor(new Color(0.01f, 0.53f, 0.82f));

        avgBlock.Add(avgTitle);
        avgBlock.Add(avgVal);
        panel.Add(avgBlock);

        var dismissBtn = new Button { text = "Dismiss" };
        dismissBtn.AddToClassList("card-btn");
        dismissBtn.style.backgroundColor = new StyleColor(new Color(0.01f, 0.53f, 0.82f));
        dismissBtn.style.color = new StyleColor(Color.white);
        dismissBtn.style.borderBottomLeftRadius = 35; dismissBtn.style.borderBottomRightRadius = 35; dismissBtn.style.borderTopLeftRadius = 35; dismissBtn.style.borderTopRightRadius = 35;
        dismissBtn.style.paddingTop = 30; dismissBtn.style.paddingBottom = 30;
        dismissBtn.style.fontSize = 40;
        dismissBtn.style.alignSelf = Align.FlexEnd;
        dismissBtn.style.width = 250;
        dismissBtn.style.marginTop = 0; dismissBtn.style.marginBottom = 0;

        dismissBtn.clicked += () => activeRoot.Remove(overlay);
        
        panel.Add(dismissBtn);
        overlay.Add(panel);
        activeRoot.Add(overlay);
    }

    private void OnRandomizeBackstoryClicked()
    {
        if (string.IsNullOrWhiteSpace(nameInput.value)) return;

        int randomIndex = Random.Range(0, DystopianContent.Backstories.Count);
        selectedBackstory = DystopianContent.Backstories[randomIndex];

        backstoryTitleText.text = $"Assigned Biography: {selectedBackstory.title}";
        
        // Use capitalization as provided in selectedBackstory.description, don't lowercase it.
        backstoryDescText.text = selectedBackstory.description;
        
        var greetingText = creationRoot.Q<Label>("GreetingText");
        if (greetingText != null) greetingText.text = $"Greeting, {nameInput.value}!";
        
        startCashText.text = $"${selectedBackstory.startingCash}";
        startStressText.text = $"{(selectedBackstory.startingStress * 100):F0}%";
        
        var luckServiceText = creationRoot.Q<Label>("LuckServiceText");
        if (luckServiceText != null) luckServiceText.text = $"{(selectedBackstory.startingManualLuck * 100):F0}%";
        var luckHospitalityText = creationRoot.Q<Label>("LuckHospitalityText");
        if (luckHospitalityText != null) luckHospitalityText.text = $"{(selectedBackstory.startingHospitalityLuck * 100):F0}%";
        var luckTechText = creationRoot.Q<Label>("LuckTechText");
        if (luckTechText != null) luckTechText.text = $"{(selectedBackstory.startingTechLuck * 100):F0}%";
        var luckCorpText = creationRoot.Q<Label>("LuckCorpText");
        if (luckCorpText != null) luckCorpText.text = $"{(selectedBackstory.startingCorpLuck * 100):F0}%";

        step1Container.style.display = DisplayStyle.None;
        step2Container.style.display = DisplayStyle.Flex;
    }

    private void OnBeginClicked()
    {
        if (selectedBackstory == null) return;
        var data = GameManager.Instance.currentData;

        // Save the typed name to the data model!
        data.playerName = nameInput.value; 

        data.balance = selectedBackstory.startingCash;
        data.stress = selectedBackstory.startingStress;
        data.mentalHealth = 1.0f - data.stress;

        data.luckManualService = selectedBackstory.startingManualLuck;
        data.luckHospitality = selectedBackstory.startingHospitalityLuck;
        data.luckTechnology = selectedBackstory.startingTechLuck;
        data.luckCorporate = selectedBackstory.startingCorpLuck;

        data.jobTitle = "Unemployed";
        data.zone = "Slums";
        data.currentWeek = 1;

        // CRITICAL: cycle jobs so the careers tab has jobs on day 1
        GameManager.Instance.CycleJobs();

        GameManager.Instance.SaveGame();
        CheckJailStatus(); 
    }

    private void OnAdvanceWeekClicked()
    {
        GameManager.Instance.AdvanceWeek();
        CheckJailStatus(); 
    }

    private void HideAllTabs()
    {
        var roots = new[] { creationRoot, careersRoot, hustlesRoot, educationRoot, activitiesRoot, jailRoot };
        foreach (var r in roots)
        {
            if (r != null)
            {
                r.style.display = DisplayStyle.None;
                r.pickingMode = PickingMode.Ignore; 
            }
        }
        
        if (dashboardRoot != null) {
            dashboardRoot.style.display = DisplayStyle.Flex;
            dashboardRoot.pickingMode = PickingMode.Position;
        }
    }

    public void CheckJailStatus()
    {
        if (GameManager.Instance.currentData.jailWeeksRemaining > 0)
        {
            HideAllTabs();
            jailRoot.style.display = DisplayStyle.Flex;
            jailRoot.pickingMode = PickingMode.Position;
            jailWeeksText.text = $"{GameManager.Instance.currentData.jailWeeksRemaining} WEEKS";
        }
        else
        {
            bool anyTabActive = false;
            var roots = new[] { careersRoot, hustlesRoot, educationRoot, activitiesRoot };
            foreach (var r in roots)
            {
                if (r != null && r.style.display == DisplayStyle.Flex)
                {
                    anyTabActive = true;
                    break;
                }
            }
            if (dashboardRoot != null && dashboardRoot.style.display == DisplayStyle.Flex) anyTabActive = true;

            if (jailRoot.style.display == DisplayStyle.Flex || !anyTabActive)
            {
                SwitchTab(dashboardRoot);
            }
        }
    }

    public void SwitchToDashboard()
    {
        SwitchTab(dashboardRoot);
    }

    private void SwitchTab(VisualElement targetRoot)
    {
        if (GameManager.Instance.currentData.jailWeeksRemaining > 0) return;

        // Clicking the same tab that's already open -> go back to dashboard
        if (targetRoot == dashboardRoot)
        {
            if (activeOverlay != null)
            {
                activeOverlay.style.display = DisplayStyle.None;
                activeOverlay.pickingMode = PickingMode.Ignore;
                activeOverlay = null;
            }
            
            HideAllTabs();
            dashboardRoot.style.display = DisplayStyle.Flex;
            dashboardRoot.pickingMode = PickingMode.Position;
            RefreshDashboardUI(GameManager.Instance.currentData.playerName);
            return;
        }

        // Opening a new menu
        AnimateOpenMenu(targetRoot);
    }

    // --- ANIMATION COROUTINES (Simplified to Instant) ---

    private void AnimateOpenMenu(VisualElement targetRoot)
    {
        // Close any currently open overlay instantly
        if (activeOverlay != null && activeOverlay != dashboardRoot)
        {
            activeOverlay.style.display = DisplayStyle.None;
            activeOverlay.pickingMode = PickingMode.Ignore;
        }

        // Hide dashboard so it doesn't render on top
        dashboardRoot.style.display = DisplayStyle.None;

        // Phase 2: Show target 
        targetRoot.style.display = DisplayStyle.Flex;
        targetRoot.pickingMode = PickingMode.Position;
        activeOverlay = targetRoot;

        // Refresh controllers
        if (CareerUI.Instance != null) CareerUI.Instance.RefreshView();
        if (HustlesUI.Instance != null) HustlesUI.Instance.PopulateHustlesBoard();
        if (EducationUI.Instance != null) EducationUI.Instance.RefreshView();
        RefreshDashboardUI(GameManager.Instance.currentData.playerName);
    }

    private void AnimateCloseMenu(VisualElement menuRoot)
    {
        // Hide the menu instantly
        menuRoot.style.display = DisplayStyle.None;
        menuRoot.pickingMode = PickingMode.Ignore;
        activeOverlay = null;

        // Show dashboard again
        dashboardRoot.style.display = DisplayStyle.Flex;
        dashboardRoot.pickingMode = PickingMode.Position;

        RefreshDashboardUI(GameManager.Instance.currentData.playerName);
    }
    private void RefreshDashboardUI(string playerName)
    {
        var data = GameManager.Instance.currentData;
        var roots = new[] { dashboardRoot, careersRoot, hustlesRoot, educationRoot, activitiesRoot };
        
        float stressPct = data.stress * 100f;
        float averageJobLuck = (GameManager.Instance.GetEffectiveLuck("manual") + GameManager.Instance.GetEffectiveLuck("hospitality") + GameManager.Instance.GetEffectiveLuck("tech") + GameManager.Instance.GetEffectiveLuck("corp")) / 4.0f;
        float luckPct = averageJobLuck * 100f; 
        float perfPct = data.promotionProgress * 100f;
        Color stressColor = stressPct >= 80 ? Color.red : (stressPct >= 50 ? Color.yellow : Color.green);

        foreach (var root in roots)
        {
            if (root == null) continue;
            
            var topCard = root.Q<VisualElement>(className: "top-card");
            if (topCard != null)
            {
                if (data.zone == "Slums") 
                    topCard.style.backgroundColor = new StyleColor(new Color(0.20f, 0.11f, 0.08f, 0.88f)); // Brown rust
                else if (data.zone == "Suburbs") 
                    topCard.style.backgroundColor = new StyleColor(new Color(0.08f, 0.16f, 0.09f, 0.88f)); // Green
                else if (data.zone == "Financial District") 
                    topCard.style.backgroundColor = new StyleColor(new Color(0.05f, 0.10f, 0.18f, 0.88f)); // Blue slate
            }

            var pName = root.Q<Label>("PlayerNameText");
            if (pName != null) pName.text = playerName;
            
            var sTitle = root.Q<Label>("SubTitleText");
            if (sTitle != null) sTitle.text = $"Current CV • Week {data.currentWeek}";
            
            var cText = root.Q<Label>("CashText");
            if (cText != null) cText.text = $"${data.balance:F2}";
            
            var eText = root.Q<Label>("ExpenseText");
            if (eText != null) eText.text = $"-${(data.rent + data.food)}";
            
            var jText = root.Q<Label>("JobText");
            if (jText != null) jText.text = data.jobTitle;
            
            var dText = root.Q<Label>("DistrictText");
            if (dText != null) dText.text = data.zone;

            RefreshLogDisplay();
            

            var sVal = root.Q<Label>("StressVal");
            if (sVal != null) sVal.text = $"{stressPct:F0}%";
            
            var sFill = root.Q<VisualElement>("StressFill");
            if (sFill != null) 
            {
                sFill.style.width = new Length(stressPct, LengthUnit.Percent);
                sFill.style.backgroundColor = new StyleColor(stressColor);
            }

            var lVal = root.Q<Label>("LuckVal");
            if (lVal != null) lVal.text = $"{luckPct:F0}%";
            
            var lFill = root.Q<VisualElement>("LuckFill");
            if (lFill != null) lFill.style.width = new Length(luckPct, LengthUnit.Percent);

            var pVal = root.Q<Label>("PerfVal");
            if (pVal != null) pVal.text = $"{perfPct:F0}%";
            
            var pFill = root.Q<VisualElement>("PerfFill");
            if (pFill != null) pFill.style.width = new Length(perfPct, LengthUnit.Percent);

            // Add Interactive Progress Tab Click Handlers
            var stressBtn = root.Q<Button>("BtnInfoStress");
            if (stressBtn != null) stressBtn.clicked += () => SwitchTab(activitiesRoot);
            
            var perfBtn = root.Q<Button>("BtnInfoPerf");
            if (perfBtn != null) perfBtn.clicked += () => SwitchTab(careersRoot);
            
            var luckBtn = root.Q<Button>("BtnInfoLuck");
            if (luckBtn != null) luckBtn.clicked += () => 
            {
                string info = $"Manual Service: {(GameManager.Instance.GetEffectiveLuck("manual")*100):F0}%\nHospitality: {(GameManager.Instance.GetEffectiveLuck("hospitality")*100):F0}%\nTechnology: {(GameManager.Instance.GetEffectiveLuck("tech")*100):F0}%\nCorporate: {(GameManager.Instance.GetEffectiveLuck("corp")*100):F0}%";
                ShowPopup(new RandomEvent("Job Luck Breakdown", info, 0.0, 0.0f, ""));
            };
        }
    }

    // --- SETTINGS LOGIC ---
    private void OpenSettings()
    {
        SwitchTab(dashboardRoot); // Settings only exists on Dashboard root!
        if (settingsOverlay == null) return;
        settingsOverlay.style.display = DisplayStyle.Flex;
        RefreshHistoryLedger();
    }

        private void RefreshHistoryLedger()
    {
        var historyContainer = dashboardRoot.Q<ScrollView>("HistoryContainer");
        if (historyContainer == null) return;
        
        historyContainer.Clear();
        var history = SaveSystem.LoadHistory(); 

        if (history == null || history.Count == 0)
        {
            Label emptyLbl = new Label("No past lives recorded in the mainframe.");
            emptyLbl.style.color = new StyleColor(Color.gray);
            emptyLbl.style.whiteSpace = WhiteSpace.Normal;
            historyContainer.Add(emptyLbl);
            return;
        }

        // Color Setup
        Color cardBg; ColorUtility.TryParseHtmlString("#0F172A", out cardBg);
        Color lightBlue; ColorUtility.TryParseHtmlString("#38BDF8", out lightBlue);
        Color redBadge; ColorUtility.TryParseHtmlString("#E74C3C", out redBadge);
        Color yellowName; ColorUtility.TryParseHtmlString("#F1C40F", out yellowName);
        Color grayText; ColorUtility.TryParseHtmlString("#94A3B8", out grayText);

        foreach (var record in history)
        {
            // Main Card Background
            VisualElement card = new VisualElement();
            card.AddToClassList("card"); 
            card.style.backgroundColor = new StyleColor(cardBg);
            card.style.paddingBottom = 15; card.style.paddingTop = 15;
            card.style.paddingLeft = 15; card.style.paddingRight = 15;
            card.style.marginBottom = 10;

            // Top Row: Player Name & Death Reason Badge
            VisualElement topRow = new VisualElement();
            topRow.AddToClassList("row");
            
            string pName = string.IsNullOrEmpty(record.playerName) ? "Unknown Citizen" : record.playerName;
            Label nameLbl = new Label(pName);
            nameLbl.style.color = new StyleColor(yellowName);
            nameLbl.style.unityFontStyleAndWeight = FontStyle.Bold;
            nameLbl.style.fontSize = 34;
            
            Label badgeLbl = new Label(record.endingAchieved);
            badgeLbl.style.backgroundColor = new StyleColor(redBadge);
            badgeLbl.style.color = Color.white;
            badgeLbl.style.unityFontStyleAndWeight = FontStyle.Bold;
            badgeLbl.style.fontSize = 24;
            badgeLbl.style.paddingLeft = 20; badgeLbl.style.paddingRight = 20;
            badgeLbl.style.paddingTop = 8; badgeLbl.style.paddingBottom = 8;
            badgeLbl.style.borderTopLeftRadius = 12; badgeLbl.style.borderTopRightRadius = 12;
            badgeLbl.style.borderBottomLeftRadius = 12; badgeLbl.style.borderBottomRightRadius = 12;
            
            topRow.Add(nameLbl);
            topRow.Add(badgeLbl);

            // Middle Row: Job Title
            Label jobLbl = new Label(record.highestJob);
            jobLbl.style.color = new StyleColor(lightBlue);
            jobLbl.style.fontSize = 28;
            jobLbl.style.unityFontStyleAndWeight = FontStyle.Bold;
            jobLbl.style.marginTop = 10;
            jobLbl.style.marginBottom = 20;

            // Bottom Row: Stats
            VisualElement bottomRow = new VisualElement();
            bottomRow.AddToClassList("row");
            
            Label survivedLbl = new Label($"Survived: {record.weeksSurvived} weeks");
            survivedLbl.style.color = new StyleColor(grayText);
            survivedLbl.style.fontSize = 24;
            
            Label bankLbl = new Label($"Bank: ${record.finalBalance:F2}");
            bankLbl.style.color = new StyleColor(grayText);
            bankLbl.style.fontSize = 24;

            bottomRow.Add(survivedLbl);
            bottomRow.Add(bankLbl);

            // Assemble everything into the card
            card.Add(topRow);
            card.Add(jobLbl);
            card.Add(bottomRow);

            // Add card to the scroll view container
            historyContainer.Add(card);
        }
    }



    private void CloseSettings()
    {
        if (settingsOverlay != null) settingsOverlay.style.display = DisplayStyle.None;
    }

    private void StartNewLife()
    {
        SaveSystem.DeleteActiveGame(); 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void PurgeAllData()
    {
        PlayerPrefs.DeleteAll(); 
        PlayerPrefs.Save();
        SaveSystem.DeleteActiveGame(); 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void ToggleTheme()
    {
        bool isDark = PlayerPrefs.GetInt("themeIsDark", 1) == 1;
        PlayerPrefs.SetInt("themeIsDark", isDark ? 0 : 1);
        Debug.Log($"[System] Theme toggled. IsDark: {!isDark}");
        RefreshSettingsButtons();
    }

    private void RefreshSettingsButtons()
    {
        if (btnToggleDevConsole != null)
        {
            bool devOn = PlayerPrefs.GetInt("showDevConsole", 0) == 1;
            btnToggleDevConsole.text = devOn ? "Dev Console: ON" : "Dev Console: OFF";
            btnToggleDevConsole.style.backgroundColor = devOn ? new StyleColor(new Color(0.18f, 0.8f, 0.44f)) : new StyleColor(new Color(0.9f, 0.3f, 0.24f));
        }

        if (btnToggleTheme != null)
        {
            bool isDark = PlayerPrefs.GetInt("themeIsDark", 1) == 1;
            btnToggleTheme.text = isDark ? "Render Theme: Dark" : "Render Theme: Light";
            btnToggleTheme.style.backgroundColor = isDark ? new StyleColor(Color.white) : new StyleColor(Color.black);
            btnToggleTheme.style.color = isDark ? new StyleColor(Color.black) : new StyleColor(Color.white);
        }
    }

    private void RefreshLogDisplay()
    {
        if (GameManager.Instance == null || GameManager.Instance.currentData == null) return;
        var data = GameManager.Instance.currentData;
        
        bool showDev = PlayerPrefs.GetInt("showDevConsole", 0) == 1;
        
        if (showDev)
        {
            if (logText != null) logText.text = data.historyLogsJson;
        }
        else
        {
            string[] entries = data.historyLogsJson.Split(new[] { "\n\n" }, System.StringSplitOptions.RemoveEmptyEntries);
            System.Text.StringBuilder filtered = new System.Text.StringBuilder();
            foreach (var entry in entries)
            {
                if (!entry.Contains("</color> ["))
                {
                    string cleanedEntry = entry.Replace("[SYSTEM] ", "").Replace("[SYS] ", "").Replace("[SYSTEM]", "").Replace("[SYS]", "");
                    filtered.Append(cleanedEntry.Trim()).Append("\n\n");
                }
            }
            if (logText != null) logText.text = filtered.ToString();
        }

        if (terminalScrollView != null)
        {
            terminalScrollView.schedule.Execute(() => {
                if (terminalScrollView.verticalScroller != null)
                    terminalScrollView.verticalScroller.value = terminalScrollView.verticalScroller.highValue;
            });
        }
    }

    public void ShowPopup(RandomEvent ev, System.Action onAcknowledge = null)
    {
        VisualElement activeRoot = null;
        if (dashboardRoot.style.display == DisplayStyle.Flex) activeRoot = dashboardRoot;
        else if (hustlesRoot.style.display == DisplayStyle.Flex) activeRoot = hustlesRoot;
        else if (activitiesRoot.style.display == DisplayStyle.Flex) activeRoot = activitiesRoot;
        else if (educationRoot.style.display == DisplayStyle.Flex) activeRoot = educationRoot;
        else if (careersRoot.style.display == DisplayStyle.Flex) activeRoot = careersRoot;
        else activeRoot = dashboardRoot; // fallback

        var overlay = new VisualElement();
        overlay.style.position = Position.Absolute;
        overlay.style.top = 0; overlay.style.bottom = 0; overlay.style.left = 0; overlay.style.right = 0;
        overlay.style.backgroundColor = new StyleColor(new Color(0.06f, 0.09f, 0.16f, 0.95f));
        overlay.style.justifyContent = Justify.Center;
        overlay.style.alignItems = Align.Center;
        
        // This stops clicks from falling through the popup overlay
        overlay.pickingMode = PickingMode.Position;

        var card = new VisualElement();
        card.AddToClassList("popup-card");

        var titleLbl = new Label(ev.title);
        titleLbl.AddToClassList("header-text");
        titleLbl.style.whiteSpace = WhiteSpace.Normal;
        Color titleCol; ColorUtility.TryParseHtmlString("#38BDF8", out titleCol);
        titleLbl.style.color = titleCol;
        titleLbl.style.fontSize = 50;
        titleLbl.style.unityTextAlign = TextAnchor.MiddleCenter;

        var descLbl = new Label(ev.description);
        descLbl.AddToClassList("sub-text");
        descLbl.style.whiteSpace = WhiteSpace.Normal;
        descLbl.style.marginTop = 20;
        descLbl.style.marginBottom = 40;
        descLbl.style.fontSize = 32;
        descLbl.style.color = Color.white;
        descLbl.style.unityTextAlign = TextAnchor.MiddleCenter;

        var btn = new Button { text = "ACKNOWLEDGE" };
        btn.AddToClassList("popup-button");
        btn.clicked += () => { 
            overlay.RemoveFromHierarchy(); 
            onAcknowledge?.Invoke();
        };

        card.Add(titleLbl);
        card.Add(descLbl);
        card.Add(btn);
        if (ev.title.Contains("FIRED"))
        {
            overlay.style.backgroundColor = new StyleColor(new Color(0.3f, 0f, 0f, 0.95f)); // Dark red
            titleLbl.style.color = new StyleColor(new Color(1f, 0.2f, 0.2f)); // Bright red
            btn.style.backgroundColor = new StyleColor(new Color(0.8f, 0.1f, 0.1f));
            StartCoroutine(ShakeRoutine(card));
        }

        overlay.Add(card);
        activeRoot.Add(overlay);
    }

    private System.Collections.IEnumerator ShakeRoutine(VisualElement el)
    {
        float elapsed = 0f;
        float duration = 0.5f;
        float magnitude = 15f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            
            el.style.translate = new StyleTranslate(new Translate(new Length(x, LengthUnit.Pixel), new Length(y, LengthUnit.Pixel), 0));
            
            elapsed += Time.deltaTime;
            yield return null;
        }

        el.style.translate = new StyleTranslate(new Translate(new Length(0, LengthUnit.Pixel), new Length(0, LengthUnit.Pixel), 0));
    }

    public void ShowCVPopup()
    {
        VisualElement activeRoot = null;
        if (dashboardRoot.style.display == DisplayStyle.Flex) activeRoot = dashboardRoot;
        else if (hustlesRoot.style.display == DisplayStyle.Flex) activeRoot = hustlesRoot;
        else if (careersRoot.style.display == DisplayStyle.Flex) activeRoot = careersRoot;
        else if (educationRoot.style.display == DisplayStyle.Flex) activeRoot = educationRoot;
        else if (activitiesRoot.style.display == DisplayStyle.Flex) activeRoot = activitiesRoot;
        else return;

        var data = GameManager.Instance.currentData;

        VisualElement overlay = new VisualElement();
        overlay.style.position = Position.Absolute;
        overlay.style.left = 0; overlay.style.right = 0; overlay.style.top = 0; overlay.style.bottom = 0;
        overlay.style.backgroundColor = new StyleColor(new Color(0, 0, 0, 0.85f));
        overlay.style.alignItems = Align.Center;
        overlay.style.justifyContent = Justify.Center;

        VisualElement paper = new VisualElement();
        paper.style.width = new StyleLength(new Length(90, LengthUnit.Percent));
        paper.style.height = new StyleLength(new Length(85, LengthUnit.Percent));
        paper.style.backgroundColor = new StyleColor(Color.white);
        paper.style.borderTopLeftRadius = 10;
        paper.style.borderTopRightRadius = 10;
        paper.style.borderBottomLeftRadius = 10;
        paper.style.borderBottomRightRadius = 10;
        paper.style.paddingLeft = 40; paper.style.paddingRight = 40; paper.style.paddingTop = 40; paper.style.paddingBottom = 40;
        paper.style.flexDirection = FlexDirection.Column;

        VisualElement header = new VisualElement();
        header.style.flexDirection = FlexDirection.Row;
        header.style.justifyContent = Justify.SpaceBetween;
        header.style.alignItems = Align.FlexStart;
        header.style.marginBottom = 30;
        header.style.borderBottomWidth = 2;
        ColorUtility.TryParseHtmlString("#BDC3C7", out Color cBorder);
        header.style.borderBottomColor = new StyleColor(cBorder);
        header.style.paddingBottom = 20;

        VisualElement leftCol = new VisualElement();
        leftCol.style.flexDirection = FlexDirection.Column;
        
        Label nameLbl = new Label(data.playerName.ToUpper());
        nameLbl.style.fontSize = 60;
        nameLbl.style.unityFontStyleAndWeight = FontStyle.Bold;
        ColorUtility.TryParseHtmlString("#2C3E50", out Color cDark);
        nameLbl.style.color = new StyleColor(cDark);
        nameLbl.style.marginBottom = 0; nameLbl.style.marginTop = 0;

        Label cvLbl = new Label("CURRICULUM VITAE");
        cvLbl.style.fontSize = 24;
        cvLbl.style.letterSpacing = 5;
        ColorUtility.TryParseHtmlString("#7F8C8D", out Color cGray);
        cvLbl.style.color = new StyleColor(cGray);
        cvLbl.style.marginTop = 5; cvLbl.style.marginBottom = 0;
        
        leftCol.Add(nameLbl); leftCol.Add(cvLbl);

        VisualElement rightCol = new VisualElement();
        rightCol.style.flexDirection = FlexDirection.Column;
        rightCol.style.alignItems = Align.FlexEnd;

        Label phoneLbl = new Label("+44 7700 900421");
        phoneLbl.style.fontSize = 24;
        phoneLbl.style.color = new StyleColor(cDark);
        phoneLbl.style.marginBottom = 0; phoneLbl.style.marginTop = 0;

        Label addressLbl = new Label("Flat 42, The Spire\nNew London, E14 9HQ");
        addressLbl.style.fontSize = 24;
        addressLbl.style.color = new StyleColor(cDark);
        addressLbl.style.unityTextAlign = TextAnchor.MiddleRight;
        addressLbl.style.marginBottom = 0; addressLbl.style.marginTop = 0;

        rightCol.Add(phoneLbl); rightCol.Add(addressLbl);

        header.Add(leftCol); header.Add(rightCol);

        ScrollView scroll = new ScrollView();
        scroll.style.flexGrow = 1;
        scroll.style.marginBottom = 20;

        string cvText = data.cvEntries.Count > 0 ? string.Join("\n\n", data.cvEntries) : "No experience recorded.";
        
        Label contentLbl = new Label(cvText);
        contentLbl.style.fontSize = 28;
        contentLbl.style.color = new StyleColor(cDark);
        contentLbl.style.whiteSpace = WhiteSpace.Normal;
        contentLbl.enableRichText = true;

        scroll.Add(contentLbl);

        Button closeBtn = new Button();
        closeBtn.text = "Close CV";
        closeBtn.AddToClassList("apply-button");
        ColorUtility.TryParseHtmlString("#34495E", out Color cBtn);
        closeBtn.style.backgroundColor = new StyleColor(cBtn);
        closeBtn.style.color = new StyleColor(Color.white);
        closeBtn.style.paddingTop = 20; closeBtn.style.paddingBottom = 20;
        closeBtn.style.fontSize = 32;

        closeBtn.clicked += () => {
            activeRoot.Remove(overlay);
        };

        paper.Add(header);
        paper.Add(scroll);
        paper.Add(closeBtn);
        
        overlay.Add(paper);
        activeRoot.Add(overlay);
    }
}

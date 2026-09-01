using System.Collections.Generic;
using System.Linq;
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
    private PlayerBackstorySO selectedBackstory;

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
    private VisualElement dashboardFooter = null;
    private bool _isAnimating = false;

    private void Awake()
    {
        Instance = this;
    }

    private MigrationUI _migrationUI;
    private MigrationUI MigrationUI => _migrationUI != null ? _migrationUI : (_migrationUI = FindAnyObjectByType<MigrationUI>());

    private FinancialGraphUI _financialGraphUI;
    private FinancialGraphUI FinancialGraphUI => _financialGraphUI != null ? _financialGraphUI : (_financialGraphUI = FindAnyObjectByType<FinancialGraphUI>());

    private EventPopupUI _eventPopupUI;
    private EventPopupUI EventPopupUI => _eventPopupUI != null ? _eventPopupUI : (_eventPopupUI = FindAnyObjectByType<EventPopupUI>());

    private void OnEnable()
    {
        GameEvents.OnShowPopupSO += ShowPopup;
        GameEvents.OnShowPopup += ShowPopup;
        GameEvents.OnSwitchToDashboard += SwitchToDashboard;

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
        dashboardRoot.Query<Button>("BtnOpenSettings").ForEach(btn => btn.clicked += OpenSettings);
        btnCloseSettings = dashboardRoot.Q<Button>("BtnCloseSettings");
        btnWipeHistory = dashboardRoot.Q<Button>("BtnWipeHistory");
        
        
        btnToggleTheme = dashboardRoot.Q<Button>("BtnToggleTheme");
        btnToggleDevConsole = dashboardRoot.Q<Button>("BtnToggleDevConsole");
        
        terminalScrollView = dashboardRoot.Q<ScrollView>("TerminalScrollView");
        terminalCard = dashboardRoot.Q<VisualElement>("TerminalCard");
        
        historyLabel = dashboardRoot.Q<Label>("HistoryLabel");

        
        if (btnCloseSettings != null) btnCloseSettings.clicked += CloseSettings;
        if (btnWipeHistory != null) btnWipeHistory.clicked += PurgeAllData;
        
        var btnExpandHistory = dashboardRoot.Q<Button>("BtnExpandHistory");
        var expandedOverlay = dashboardRoot.Q<VisualElement>("ExpandedHistoryOverlay");
        var btnCloseExpanded = dashboardRoot.Q<Button>("BtnCloseExpandedHistory");
        
        if (btnExpandHistory != null && expandedOverlay != null) {
            btnExpandHistory.clicked += () => {
                expandedOverlay.style.display = DisplayStyle.Flex;
                expandedOverlay.style.opacity = 0f;
                var card = expandedOverlay.Q<VisualElement>("ExpandedHistoryCard");
                if (card != null) card.style.scale = new StyleScale(new Scale(new Vector3(1, 0, 1)));
                
                expandedOverlay.schedule.Execute(() => {
                    expandedOverlay.style.opacity = 1f;
                    if (card != null) card.style.scale = new StyleScale(new Scale(new Vector3(1, 1, 1)));
                }).StartingIn(50);
                expandedOverlay.BringToFront();
            };
        }
        if (btnCloseExpanded != null && expandedOverlay != null) {
            btnCloseExpanded.clicked += () => {
                var card = expandedOverlay.Q<VisualElement>("ExpandedHistoryCard");
                if (card != null) card.style.scale = new StyleScale(new Scale(new Vector3(1, 0, 1)));
                expandedOverlay.style.opacity = 0f;
                expandedOverlay.schedule.Execute(() => {
                    expandedOverlay.style.display = DisplayStyle.None;
                }).StartingIn(300);
            };
        }
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
        dashboardRoot.Q<Button>("BtnOpenMigration")?.RegisterCallback<ClickEvent>(ev => MigrationUI?.OpenPortal());
        dashboardRoot.Q<Label>("CashText")?.parent.RegisterCallback<ClickEvent>(ev => FinancialGraphUI?.OpenGraph());
        dashboardRoot.Q<Label>("ExpenseText")?.parent.RegisterCallback<ClickEvent>(ev => FinancialGraphUI?.OpenGraph());

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

        // Cache dashboard footer for animation
        dashboardFooter = dashboardRoot.Q<VisualElement>(className: "bottom-nav-container");

        // For the 4 sub-pages: hide their own bottom-nav (they'll use the dashboard area),
        // and wire up close buttons
        var subPages = new[] { careersRoot, hustlesRoot, educationRoot, activitiesRoot };
        foreach (var page in subPages)
        {
            // Hide the sub-page's own bottom-nav-container (nav + footer stats)
            var pageFooter = page?.Q<VisualElement>(className: "bottom-nav-container");
            if (pageFooter != null) pageFooter.style.display = DisplayStyle.None;

            // Wire the BtnCloseMenu button
            var closeBtn = page?.Q<Button>("BtnCloseMenu");
            if (closeBtn != null)
            {
                closeBtn.clicked += () => AnimateCloseMenu(page);
            }

            // Wire drag to close
            var dragHeader = page?.Q<VisualElement>("DragHeader");
            if (dragHeader != null)
            {
                SetupDragToClose(dragHeader, page);
            }
        }

        HideAllTabs();

    }

    private void SetupTopCard(VisualElement root)
    {
        if (root == null) return;
        
        root.Query<Button>("BtnViewCV").ForEach(btnViewCV => {
            btnViewCV.clicked -= ShowCVPopup;
            btnViewCV.clicked += ShowCVPopup;
        });
        
        root.Q<VisualElement>("ColCash")?.RegisterCallback<ClickEvent>(ev => {
            ShowPopup(new RandomEvent { 
                title = "CASH RESERVE", 
                description = "Your liquid synthetic cash. Used to pay weekly expenses, enroll in courses, or purchase wellness escapes.\n\n<color=#E74C3C>WARNING: If your cash drops below zero and you cannot pay rent, you will be evicted and face game over.</color>" 
            });
        });
        
        root.Q<VisualElement>("ColExpenses")?.RegisterCallback<ClickEvent>(ev => {
            EventPopupUI?.ShowEvent(new RandomEvent { 
                title = "WEEKLY EXPENSES", 
                description = "The automatic deductions that hit your account every week. This includes housing, food, and active education costs." 
            });
        });
        
        root.Q<VisualElement>("ColJob")?.RegisterCallback<ClickEvent>(ev => {
            EventPopupUI?.ShowEvent(new RandomEvent { 
                title = "CURRENT JOB", 
                description = "Your primary source of income. Failing to click '+ Week' enough times to fulfill your duties may result in termination." 
            });
        });
        
        root.Q<VisualElement>("ColDistrict")?.RegisterCallback<ClickEvent>(ev => {
            MigrationUI?.OpenPortal();
        });
        
        // Ensure Settings gear also works globally

        // Ensure CV Button works globally
        root.Query<Button>("BtnViewCV").ForEach(btn => {
            btn.clicked -= ShowCVPopup;
            btn.clicked += ShowCVPopup;
        });
        
        // Ensure Home Icon works globally
        root.Query<Button>("BtnOpenMigration").ForEach(btn => {
            btn.RegisterCallback<ClickEvent>(ev => MigrationUI?.OpenPortal());
        });
        root.Query<Button>("BtnOpenSettings").ForEach(btnSettings => {
            btnSettings.clicked -= OpenSettings;
            btnSettings.clicked += OpenSettings;
        });
    }

    private void OnDisable()
    {
        GameEvents.OnShowPopupSO -= ShowPopup;
        GameEvents.OnShowPopup -= ShowPopup;
        GameEvents.OnSwitchToDashboard -= SwitchToDashboard;
        Application.logMessageReceived -= OnDebugLogReceived;
    }

    private void Start()
    {
        // Ensure the Dashboard is the base layer, and all popup menus render ON TOP of it
        if (dashboardDocument != null) dashboardDocument.sortingOrder = 0;
        if (careersDocument != null) careersDocument.sortingOrder = 10;
        if (hustlesDocument != null) hustlesDocument.sortingOrder = 10;
        if (educationDocument != null) educationDocument.sortingOrder = 10;
        if (activitiesDocument != null) activitiesDocument.sortingOrder = 10;
        if (creationDocument != null) creationDocument.sortingOrder = 20;
        if (jailDocument != null) jailDocument.sortingOrder = 20;
        // Initialize all tab controllers (they are plain C# classes, not MonoBehaviours)
        // Each wrapped in try-catch so one failure doesn't block the rest or the debug menu
        try { CareerUI.Instance = new CareerUI(); CareerUI.Instance.Initialize(careersDocument); }
        catch (System.Exception e) { Debug.LogError($"[GameUIManager] CareerUI init failed: {e}"); }

        try { HustlesUI.Instance = new HustlesUI(); HustlesUI.Instance.Initialize(hustlesDocument); }
        catch (System.Exception e) { Debug.LogError($"[GameUIManager] HustlesUI init failed: {e}"); }

        try { EducationUI.Instance = new EducationUI(); EducationUI.Instance.Initialize(educationDocument); }
        catch (System.Exception e) { Debug.LogError($"[GameUIManager] EducationUI init failed: {e}"); }

        try { ActivitiesUI.Instance = new ActivitiesUI(); ActivitiesUI.Instance.Initialize(activitiesDocument); }
        catch (System.Exception e) { Debug.LogError($"[GameUIManager] ActivitiesUI init failed: {e}"); }

        Debug.Log("[GameUIManager] Tab controller initialization complete.");

        // Strictly check if a save file actually exists on the hard drive
        try
        {
            if (SaveSystem.LoadActiveGame() != null)
            {
                Debug.Log("[GameUIManager] Save file found on disk! Skipping character creation.");
                HideAllTabs();
                CheckJailStatus();
            }
            else
            {
                Debug.Log("[GameUIManager] No save file found. Starting character creation.");
                HideAllTabs();
                if (dashboardRoot != null)
                {
                    dashboardRoot.style.display = DisplayStyle.None;
                    dashboardRoot.pickingMode = PickingMode.Ignore;
                }
                creationRoot.style.display = DisplayStyle.Flex;
                creationRoot.pickingMode = PickingMode.Position;
                step1Container.style.display = DisplayStyle.Flex;
                step2Container.style.display = DisplayStyle.None;
            }
        }
        catch (System.Exception e) { Debug.LogError($"[GameUIManager] Save check failed: {e}"); }

        // Initialize Debug Menu - ALWAYS runs regardless of above errors
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
                debugOverlay.style.opacity = 0f;
                var card = debugOverlay.Q<VisualElement>("DebugGamesCard");
                if (card != null) card.style.scale = new UnityEngine.UIElements.StyleScale(new UnityEngine.UIElements.Scale(new UnityEngine.Vector3(1, 0, 1)));
                debugOverlay.schedule.Execute(() => {
                    debugOverlay.style.opacity = 1f;
                    if (card != null) card.style.scale = new UnityEngine.UIElements.StyleScale(new UnityEngine.UIElements.Scale(new UnityEngine.Vector3(1, 1, 1)));
                }).StartingIn(50);
                debugOverlay.BringToFront();
                debugList.Clear();
                if (MiniGameManager.Instance != null && MiniGameManager.Instance.GameConfigs != null)
                {
                    foreach (var kvp in MiniGameManager.Instance.GameConfigs)
                    {
                        var b = new Button { text = kvp.Key };
                        b.style.paddingTop = 20; b.style.paddingBottom = 20; b.style.marginBottom = 10;
                        b.style.fontSize = 28; b.style.color = Color.white;
                        b.style.backgroundColor = new StyleColor(new Color(0.12f, 0.16f, 0.23f));
                        b.style.borderTopWidth = 2; b.style.borderBottomWidth = 2; b.style.borderLeftWidth = 2; b.style.borderRightWidth = 2;
                        b.style.borderTopColor = new StyleColor(new Color(0.22f, 0.74f, 0.97f));
                        b.style.borderBottomColor = new StyleColor(new Color(0.22f, 0.74f, 0.97f));
                        b.style.borderLeftColor = new StyleColor(new Color(0.22f, 0.74f, 0.97f));
                        b.style.borderRightColor = new StyleColor(new Color(0.22f, 0.74f, 0.97f));
                        b.clicked += () => {
                            var card2 = debugOverlay.Q<VisualElement>("DebugGamesCard");
                            if (card2 != null) card2.style.scale = new UnityEngine.UIElements.StyleScale(new UnityEngine.UIElements.Scale(new UnityEngine.Vector3(1, 0, 1)));
                            debugOverlay.style.opacity = 0f;
                            debugOverlay.schedule.Execute(() => debugOverlay.style.display = DisplayStyle.None).StartingIn(300);
                            var active = new ActiveMiniGame("Debug", kvp.Key, canFail: false);
                            MiniGameManager.Instance.LaunchMiniGame(active);
                        };
                        debugList.Add(b);
                    }
                }
            };
            
            if (btnCloseDebug != null)
            {
                btnCloseDebug.clicked += () => {
                    var card = debugOverlay.Q<VisualElement>("DebugGamesCard");
                    if (card != null) card.style.scale = new UnityEngine.UIElements.StyleScale(new UnityEngine.UIElements.Scale(new UnityEngine.Vector3(1, 0, 1)));
                    debugOverlay.style.opacity = 0f;
                    debugOverlay.schedule.Execute(() => debugOverlay.style.display = DisplayStyle.None).StartingIn(300);
                };
            }
        }
    }


    private System.Text.StringBuilder _logBuilder;
    private System.Text.StringBuilder _devLogBuilder = new System.Text.StringBuilder();

    private void OnDebugLogReceived(string condition, string stackTrace, LogType type)
    {
        if (GameManager.Instance == null || GameManager.Instance.currentData == null) return;
        
        if (type == LogType.Warning || type == LogType.Error || type == LogType.Exception)
        {
            _devLogBuilder.Append($"[{type}] ").Append(condition).Append("\n\n");
            if (dashboardRoot != null)
            {
                var devLogUI = dashboardRoot.Q<Label>("DebugConsoleLog");
                if (devLogUI != null) devLogUI.text = _devLogBuilder.ToString();
            }
        }
        else if (type == LogType.Log)
        {
            if (condition.Contains("Unable to find a font") || condition.Contains("Unable to load font")) return;
            
            var data = GameManager.Instance.currentData;
            if (_logBuilder == null) 
            {
                _logBuilder = new System.Text.StringBuilder(data.historyLogsJson);
            }
            else
            {
                // Sync builder in case historyLogsJson was modified elsewhere
                if (_logBuilder.Length != data.historyLogsJson.Length)
                {
                    _logBuilder.Clear();
                    _logBuilder.Append(data.historyLogsJson);
                }
            }

            // string prefix = "<color=#38BDF8>[SYS]</color>";
            // _logBuilder.Append(prefix).Append(" ").Append(condition).Append("\n\n");
            // The screenshot showed no prefix for some reason, maybe they want it just plain. I'll stick to what was there or what's asked.
            // Wait, the prompt didn't ask to remove [SYS], but the image had [ALERT] because it was a Warning. 
            // So if it's a Log, it will still have [SYS]. I'll keep it.
            _logBuilder.Append("<color=#38BDF8>[SYS]</color> ").Append(condition).Append("\n\n");
            
            if (_logBuilder.Length > 4000) 
            {
                _logBuilder.Remove(0, _logBuilder.Length - 4000);
            }
            
            data.historyLogsJson = _logBuilder.ToString();
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
        panel.style.transitionProperty = new StyleList<StylePropertyName>(new List<StylePropertyName> { new StylePropertyName("scale") });
        panel.style.transitionDuration = new StyleList<TimeValue>(new List<TimeValue> { new TimeValue(0.3f) });
        panel.style.transitionTimingFunction = new StyleList<EasingFunction>(new List<EasingFunction> { new EasingFunction(EasingMode.EaseOutBack) });
        panel.style.scale = new StyleScale(new Scale(new Vector3(1, 0, 1)));
        
        overlay.style.transitionProperty = new StyleList<StylePropertyName>(new List<StylePropertyName> { new StylePropertyName("opacity") });
        overlay.style.transitionDuration = new StyleList<TimeValue>(new List<TimeValue> { new TimeValue(0.2f) });
        overlay.style.opacity = 0f;
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

        CreateRow("Manual/Service (Dishwasher)", GameManager.Instance.GetEffectiveLuck(JobSector.Manual));
        CreateRow("Hospitality/Shift Manager", GameManager.Instance.GetEffectiveLuck(JobSector.Hospitality));
        CreateRow("Tech/Junior Dev", GameManager.Instance.GetEffectiveLuck(JobSector.Tech));
        CreateRow("Corporate/Admin", GameManager.Instance.GetEffectiveLuck(JobSector.Corp));

        // Average
        float avgLuck = (GameManager.Instance.GetEffectiveLuck(JobSector.Manual) + GameManager.Instance.GetEffectiveLuck(JobSector.Hospitality) + GameManager.Instance.GetEffectiveLuck(JobSector.Tech) + GameManager.Instance.GetEffectiveLuck(JobSector.Corp)) / 4f;
        
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

        dismissBtn.clicked += () => {
            panel.style.scale = new StyleScale(new Scale(new Vector3(1, 0, 1)));
            overlay.style.opacity = 0f;
            overlay.schedule.Execute(() => activeRoot.Remove(overlay)).StartingIn(300);
        };
        
        panel.Add(dismissBtn);
        overlay.Add(panel);
        activeRoot.Add(overlay);
        overlay.schedule.Execute(() => {
            overlay.style.opacity = 1f;
            panel.style.scale = new StyleScale(new Scale(new Vector3(1, 1, 1)));
        }).StartingIn(50);
    }

    private void OnRandomizeBackstoryClicked()
    {
        if (string.IsNullOrWhiteSpace(nameInput.value)) return;

        var backstories = GameManager.Instance.gameDatabase.backstories.FindAll(b => b != null);
        if (backstories.Count == 0) return;
        int randomIndex = Random.Range(0, backstories.Count);
        selectedBackstory = backstories[randomIndex];

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
        data.zone = District.Slums;
        data.currentWeek = 1;

        // CRITICAL: cycle jobs so the careers tab has jobs on day 1
        GameManager.Instance.CycleJobs();
        
        if (_logBuilder != null) _logBuilder.Clear();
        data.historyLogsJson = "";
        
        string logMsg = string.IsNullOrEmpty(selectedBackstory.week1Log) 
            ? "Game started on the gritty streets of the Slums!" 
            : selectedBackstory.week1Log;
            
        Debug.Log("<color=#38BDF8><b>=== WEEK 1 ===</b></color>");
        Debug.Log(logMsg);

        GameManager.Instance.SaveGame();

        if (creationRoot != null)
        {
            creationRoot.style.display = DisplayStyle.None;
            creationRoot.pickingMode = PickingMode.Ignore;
        }
        if (dashboardRoot != null)
        {
            dashboardRoot.style.display = DisplayStyle.Flex;
            dashboardRoot.pickingMode = PickingMode.Position;
        }

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
            bool wasInJail = false;
            if (jailRoot != null && jailRoot.style.display == DisplayStyle.Flex)
            {
                wasInJail = true;
                jailRoot.style.display = DisplayStyle.None;
                jailRoot.pickingMode = PickingMode.Ignore;
            }

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

            if (wasInJail || !anyTabActive)
            {
                SwitchTab(dashboardRoot);
            }
            else
            {
                // If a tab is already active (like dashboard on startup), make sure its data is populated!
                RefreshDashboardUI(GameManager.Instance.currentData.playerName);
            }
        }
    }
    public void SwitchToDashboard()
    {
        SwitchTab(dashboardRoot);
    }

    private void SwitchTab(VisualElement targetRoot)
    {
        if (_isAnimating) return;
        if (GameManager.Instance.currentData.jailWeeksRemaining > 0) return;

        // If explicitly requesting dashboard or clicking the currently open tab
        if (targetRoot == dashboardRoot || targetRoot == activeOverlay)
        {
            if (activeOverlay != null && activeOverlay != dashboardRoot)
            {
                AnimateCloseMenu(activeOverlay);
            }
            else if (targetRoot == dashboardRoot && activeOverlay == null)
            {
                // Initializing dashboard on startup
                RefreshDashboardUI(GameManager.Instance.currentData.playerName);
            }
            return;
        }

        // Opening a new menu
        AnimateOpenMenu(targetRoot);
    }



    private void SetupDragToClose(VisualElement dragArea, VisualElement menuRoot)
    {
        bool isDragging = false;
        Vector2 startPos = Vector2.zero;
        
        dragArea.RegisterCallback<PointerDownEvent>(evt => {
            var wrapper = menuRoot.Q<VisualElement>("DynamicMiddleWrapper");
            if (wrapper == null) return;
            isDragging = true;
            startPos = evt.position;
            dragArea.CapturePointer(evt.pointerId);
            
            // Disable USS transitions for instant 1:1 dragging
            wrapper.style.transitionDuration = new List<TimeValue> { new TimeValue(0) };
        });

        dragArea.RegisterCallback<PointerMoveEvent>(evt => {
            if (!isDragging) return;
            var wrapper = menuRoot.Q<VisualElement>("DynamicMiddleWrapper");
            if (wrapper == null) return;
            
            float deltaY = evt.position.y - startPos.y;
            if (deltaY < 0) deltaY = 0; // Only drag down
            
            wrapper.style.translate = new Translate(0, deltaY);
        });

        dragArea.RegisterCallback<PointerUpEvent>(evt => {
            if (!isDragging) return;
            isDragging = false;
            dragArea.ReleasePointer(evt.pointerId);
            
            var wrapper = menuRoot.Q<VisualElement>("DynamicMiddleWrapper");
            if (wrapper == null) return;
            
            float deltaY = evt.position.y - startPos.y;
            
            // Restore USS transition
            wrapper.style.transitionDuration = StyleKeyword.Null;
            wrapper.style.translate = StyleKeyword.Null;
            
            if (deltaY > 250) // Threshold to snap close
            {
                AnimateCloseMenu(menuRoot);
            }
        });
    }

    // --- ANIMATED MENU TRANSITIONS ---

    private void AnimateOpenMenu(VisualElement targetRoot)
    {
        if (activeOverlay != null && activeOverlay != dashboardRoot)
        {
            activeOverlay.style.display = DisplayStyle.None;
            activeOverlay.pickingMode = PickingMode.Ignore;
        }

        targetRoot.style.display = DisplayStyle.Flex;
        targetRoot.pickingMode = PickingMode.Position;
        activeOverlay = targetRoot;

        var dashboardFooter = dashboardRoot.Q<VisualElement>(className: "bottom-nav-container");
        if (dashboardFooter != null) dashboardFooter.AddToClassList("bottom-nav-hidden");

        var targetFooter = targetRoot.Q<VisualElement>(className: "bottom-nav-container");
        if (targetFooter != null) targetFooter.style.display = DisplayStyle.None;

        var screenContainer = targetRoot.Q<VisualElement>(className: "screen-container");
        if (screenContainer != null) 
        {
            screenContainer.style.backgroundColor = new StyleColor(Color.clear);
            
            var targetTopCard = targetRoot.Q<VisualElement>(className: "top-card-root");
            if (targetTopCard != null) targetTopCard.style.visibility = Visibility.Hidden;

            // Animate the UXML-defined wrapper
            var middleWrapper = screenContainer.Q<VisualElement>("DynamicMiddleWrapper");
            if (middleWrapper != null)
            {
                middleWrapper.AddToClassList("section-content-hidden");

                targetRoot.schedule.Execute(() => {
                    middleWrapper.RemoveFromClassList("section-content-hidden");
                }).StartingIn(50);
            }
        }

        // Only refresh the tab we are actually opening to prevent stutter
        if (targetRoot == careersRoot && CareerUI.Instance != null) CareerUI.Instance.RefreshView();
        else if (targetRoot == hustlesRoot && HustlesUI.Instance != null) HustlesUI.Instance.PopulateHustlesBoard();
        else if (targetRoot == educationRoot && EducationUI.Instance != null) EducationUI.Instance.RefreshView();
        else if (targetRoot == dashboardRoot) RefreshDashboardUI(GameManager.Instance.currentData.playerName);
        // ActivitiesUI manages its own Population in Initialize, but we can refresh here if needed later
    }

    private void AnimateCloseMenu(VisualElement menuRoot)
    {
        menuRoot.pickingMode = PickingMode.Ignore;
        var screenContainer = menuRoot.Q<VisualElement>(className: "screen-container");
        if (screenContainer != null) 
        {
            var middleWrapper = screenContainer.Q<VisualElement>("DynamicMiddleWrapper");
            if (middleWrapper != null)
            {
                middleWrapper.AddToClassList("section-content-hidden");
            }
        }

        var dashboardFooter = dashboardRoot.Q<VisualElement>(className: "bottom-nav-container");
        if (dashboardFooter != null) dashboardFooter.RemoveFromClassList("bottom-nav-hidden");

        menuRoot.schedule.Execute(() => {
            menuRoot.style.display = DisplayStyle.None;
            menuRoot.pickingMode = PickingMode.Ignore;
            if (activeOverlay == menuRoot) activeOverlay = null;
        }).StartingIn(400);

        dashboardRoot.pickingMode = PickingMode.Position;
        RefreshDashboardUI(GameManager.Instance.currentData.playerName);
    }
    private void RefreshDashboardUI(string playerName)
    {
        var data = GameManager.Instance.currentData;
        var roots = new[] { dashboardRoot, careersRoot, hustlesRoot, educationRoot, activitiesRoot };
        
        float stressPct = data.stress * 100f;
        float averageJobLuck = (GameManager.Instance.GetEffectiveLuck(JobSector.Manual) + GameManager.Instance.GetEffectiveLuck(JobSector.Hospitality) + GameManager.Instance.GetEffectiveLuck(JobSector.Tech) + GameManager.Instance.GetEffectiveLuck(JobSector.Corp)) / 4.0f;
        float luckPct = averageJobLuck * 100f; 
        float perfPct = data.promotionProgress * 100f;
        Color stressColor = stressPct >= 80 ? Color.red : (stressPct >= 50 ? Color.yellow : Color.green);

        foreach (var root in roots)
        {
            if (root == null) continue;
            
            var topCard = root.Q<VisualElement>(className: "top-card-root");
            var accentBar = root.Q<VisualElement>("DistrictAccentBar");
            Color districtAccent;
            Color districtBg;
            if (data.zone == District.Slums)
            {
                districtAccent = new Color(0.89f, 0.45f, 0.24f); // Warm rust orange
                districtBg = new Color(0.12f, 0.07f, 0.05f, 0.95f);
            }
            else if (data.zone == District.Suburbs)
            {
                districtAccent = new Color(0.18f, 0.80f, 0.44f); // Fresh green
                districtBg = new Color(0.05f, 0.10f, 0.06f, 0.95f);
            }
            else
            {
                districtAccent = new Color(0.22f, 0.74f, 0.97f); // Cyan blue
                districtBg = new Color(0.04f, 0.07f, 0.16f, 0.95f);
            }
            if (topCard != null) topCard.style.backgroundColor = new StyleColor(districtBg);
            if (accentBar != null) accentBar.style.backgroundColor = new StyleColor(districtAccent);

            // Animated diagonal watermark
            var wmContainer = root.Q<VisualElement>("WatermarkContainer");
            if (wmContainer != null)
            {
                string zoneName = data.zone.ToString().ToUpper();
                string wmLine = string.Join("   ", System.Linq.Enumerable.Repeat(zoneName, 20));
                string wmBlock = string.Join("\n\n", System.Linq.Enumerable.Repeat(wmLine, 15));
                
                var existing = wmContainer.Q<Label>("WatermarkText");
                if (existing == null)
                {
                    existing = new Label { name = "WatermarkText" };
                    existing.style.position = Position.Absolute;
                    existing.style.fontSize = 28;
                    existing.style.color = new StyleColor(new Color(1f, 1f, 1f, 0.01f));
                    existing.style.unityFontStyleAndWeight = FontStyle.Bold;
                    existing.style.whiteSpace = WhiteSpace.NoWrap;
                    existing.style.rotate = new StyleRotate(new Rotate(Angle.Degrees(-35f)));
                    existing.style.width = 3000;
                    existing.style.height = 3000;
                    existing.style.left = -400;
                    existing.style.top = -400;
                    wmContainer.Add(existing);
                    
                    // Animate: drift diagonally
                    float offset = 0f;
                    existing.schedule.Execute(() => {
                        offset = (offset + 0.5f) % 120f;
                        existing.style.translate = new StyleTranslate(new Translate(offset, offset));
                    }).Every(33); // ~30fps
                }
                existing.text = wmBlock;
            }

            root.Query<Label>("PlayerNameText").ForEach(l => l.text = playerName);
            root.Query<Button>(className: "id-card-name-btn").ForEach(b => b.text = playerName);
            
            root.Query<Label>("SubTitleText").ForEach(l => l.text = $"WEEK {data.currentWeek}");
            
            root.Query<Label>("CashText").ForEach(l => l.text = $"${data.balance:F2}");
            
            root.Query<Label>("ExpenseText").ForEach(l => l.text = $"-${(data.rent + data.food)}");
            double income = GameManager.Instance.GetBaseSalary(data.jobTitle);
            root.Query<Label>("IncomeText").ForEach(l => l.text = $"+${income:F2}");
            
            root.Query<Label>("JobText").ForEach(l => l.text = data.jobTitle);
            
            root.Query<Label>("DistrictText").ForEach(l => l.text = data.zone.ToString());
            root.Query<Label>("PlayerInitials").ForEach(l => l.text = !string.IsNullOrEmpty(playerName) ? playerName.Substring(0, 1).ToUpper() : "X");
            root.Query<Label>("IDCardWeek").ForEach(l => l.text = $"VALID: WK {data.currentWeek}");
            root.Query<Label>("PhoneTime").ForEach(l => l.text = $"WEEK {data.currentWeek}");
            root.Query<Label>("DistrictSignal").ForEach(l => l.text = $"📶 {data.zone.ToString().ToUpper()} - 3G");
            root.Query<Label>("LedgerDate").ForEach(l => l.text = $"DATE: WEEK {data.currentWeek}");
            root.Query<Label>("TickerWeek").ForEach(l => l.text = $"WK:{data.currentWeek:D2}");




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
                string info = $"Manual Service: {(GameManager.Instance.GetEffectiveLuck(JobSector.Manual)*100):F0}%\nHospitality: {(GameManager.Instance.GetEffectiveLuck(JobSector.Hospitality)*100):F0}%\nTechnology: {(GameManager.Instance.GetEffectiveLuck(JobSector.Tech)*100):F0}%\nCorporate: {(GameManager.Instance.GetEffectiveLuck(JobSector.Corp)*100):F0}%";
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
        settingsOverlay.style.opacity = 0f;
        var card = settingsOverlay.Q<VisualElement>("SettingsCard");
        if (card != null) card.style.scale = new UnityEngine.UIElements.StyleScale(new UnityEngine.UIElements.Scale(new UnityEngine.Vector3(1, 0, 1)));
        settingsOverlay.schedule.Execute(() => {
            settingsOverlay.style.opacity = 1f;
            if (card != null) card.style.scale = new UnityEngine.UIElements.StyleScale(new UnityEngine.UIElements.Scale(new UnityEngine.Vector3(1, 1, 1)));
        }).StartingIn(50);
        RefreshHistoryLedger();
    }

    private void RefreshHistoryLedger()
    {
        var historyContainer = dashboardRoot.Q<ScrollView>("HistoryContainer");
        var expandedHistoryContainer = dashboardRoot.Q<ScrollView>("ExpandedHistoryContainer");
        if (historyContainer == null) return;
        
        historyContainer.Clear();
        if (expandedHistoryContainer != null) expandedHistoryContainer.Clear();
        var history = SaveSystem.LoadHistory(); 

        if (history == null || history.Count == 0)
        {
            Label emptyLbl = new Label("No past lives recorded in the mainframe.");
            emptyLbl.style.color = new StyleColor(Color.gray);
            emptyLbl.style.whiteSpace = WhiteSpace.Normal;
            historyContainer.Add(emptyLbl);
            if (expandedHistoryContainer != null)
            {
                Label emptyLbl2 = new Label("No past lives recorded in the mainframe.");
                emptyLbl2.style.color = new StyleColor(Color.gray);
                emptyLbl2.style.whiteSpace = WhiteSpace.Normal;
                expandedHistoryContainer.Add(emptyLbl2);
            }
            return;
        }

        var template = Resources.Load<VisualTreeAsset>("UI/HistoryRecordTemplate");
        if (template == null)
        {
            Debug.LogError("Could not find UI/HistoryRecordTemplate in Resources.");
            return;
        }

        foreach (var record in history)
        {
            VisualElement card = template.Instantiate().Q<VisualElement>(className: "card");

            string pName = string.IsNullOrEmpty(record.playerName) ? "Unknown Citizen" : record.playerName;
            card.Q<Label>("PlayerName").text = pName;
            card.Q<Label>("DeathBadge").text = record.endingAchieved;
            card.Q<Label>("JobTitle").text = record.highestJob;
            card.Q<Label>("WeeksSurvived").text = $"Survived: {record.weeksSurvived} weeks";
            card.Q<Label>("FinalBank").text = $"Bank: ${record.finalBalance:F2}";

            historyContainer.Add(card);

            if (expandedHistoryContainer != null)
            {
                VisualElement card2 = template.Instantiate().Q<VisualElement>(className: "card");
                card2.Q<Label>("PlayerName").text = pName;
                card2.Q<Label>("DeathBadge").text = record.endingAchieved;
                card2.Q<Label>("JobTitle").text = record.highestJob;
                card2.Q<Label>("WeeksSurvived").text = $"Survived: {record.weeksSurvived} weeks";
                card2.Q<Label>("FinalBank").text = $"Bank: ${record.finalBalance:F2}";
                expandedHistoryContainer.Add(card2);
            }
        }
    }



    private void CloseSettings()
    {
        if (settingsOverlay == null) return;
        var card = settingsOverlay.Q<VisualElement>("SettingsCard");
        if (card != null) card.style.scale = new UnityEngine.UIElements.StyleScale(new UnityEngine.UIElements.Scale(new UnityEngine.Vector3(1, 0, 1)));
        settingsOverlay.style.opacity = 0f;
        settingsOverlay.schedule.Execute(() => settingsOverlay.style.display = DisplayStyle.None).StartingIn(300);
    }

    private void StartNewLife()
    {
        SaveSystem.DeleteActiveGame(); 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void PurgeAllData()
    {
        if (GameManager.Instance != null && GameManager.Instance.currentData != null)
        {
            string finalName = string.IsNullOrEmpty(GameManager.Instance.currentData.playerName) ? "Unknown Citizen" : GameManager.Instance.currentData.playerName;
            SaveSystem.SaveHistoryRecord(
                finalName, 
                GameManager.Instance.currentData.balance, 
                GameManager.Instance.currentData.currentWeek, 
                GameManager.Instance.currentData.jobTitle, 
                "Gave up on life."
            );
        }
        SaveSystem.DeleteActiveGame(); 
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
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
            btnToggleDevConsole.text = devOn ? "Dev Tools: ON" : "Dev Tools: OFF";
            btnToggleDevConsole.style.backgroundColor = devOn ? new StyleColor(new Color(0.18f, 0.8f, 0.44f)) : new StyleColor(new Color(0.9f, 0.3f, 0.24f));
            
            var btnDebug = dashboardRoot?.Q<Button>("BtnDebugGames");
            if (btnDebug != null)
            {
                btnDebug.style.display = devOn ? DisplayStyle.Flex : DisplayStyle.None;
            }
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

    public void ShowPopup(RandomEventSO ev, System.Action onAcknowledge = null)
    {
        ShowPopup(new RandomEvent(ev.title, ev.description, ev.cashOffset, ev.stressOffset, ev.luckType.ToString(), ev.luckOffset), onAcknowledge);
    }

    public void ShowPopup(RandomEvent ev, System.Action onAcknowledge = null)
    {
        VisualElement activeRoot = dashboardRoot;
        if (activeOverlay != null && activeOverlay.style.display == DisplayStyle.Flex) 
        {
            activeRoot = activeOverlay;
        }

        var overlay = new VisualElement();
        overlay.style.position = Position.Absolute;
        overlay.style.top = 0; overlay.style.bottom = 0; overlay.style.left = 0; overlay.style.right = 0;
        overlay.style.backgroundColor = new StyleColor(new Color(0.06f, 0.09f, 0.16f, 0.95f));
        overlay.style.justifyContent = Justify.Center;
        overlay.style.alignItems = Align.Center;
        
        // This stops clicks from falling through the popup overlay
        overlay.pickingMode = PickingMode.Position;

        var card = new VisualElement();
        card.style.transitionProperty = new StyleList<StylePropertyName>(new List<StylePropertyName> { new StylePropertyName("scale") });
        card.style.transitionDuration = new StyleList<TimeValue>(new List<TimeValue> { new TimeValue(0.3f) });
        card.style.transitionTimingFunction = new StyleList<EasingFunction>(new List<EasingFunction> { new EasingFunction(EasingMode.EaseOutBack) });
        card.style.scale = new StyleScale(new Scale(new Vector3(1, 0, 1)));
        
        overlay.style.transitionProperty = new StyleList<StylePropertyName>(new List<StylePropertyName> { new StylePropertyName("opacity") });
        overlay.style.transitionDuration = new StyleList<TimeValue>(new List<TimeValue> { new TimeValue(0.2f) });
        overlay.style.opacity = 0f;
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
            card.style.scale = new StyleScale(new Scale(new Vector3(1, 0, 1)));
            overlay.style.opacity = 0f;
            overlay.schedule.Execute(() => {
                overlay.RemoveFromHierarchy(); 
                onAcknowledge?.Invoke();
            }).StartingIn(300);
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
        overlay.schedule.Execute(() => {
            overlay.style.opacity = 1f;
            card.style.scale = new StyleScale(new Scale(new Vector3(1, 1, 1)));
        }).StartingIn(50);
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
            paper.style.scale = new StyleScale(new Scale(new Vector3(1, 0, 1)));
            overlay.style.opacity = 0f;
            overlay.schedule.Execute(() => activeRoot.Remove(overlay)).StartingIn(300);
        };

        paper.Add(header);
        paper.Add(scroll);
        paper.Add(closeBtn);
        
        paper.style.transitionProperty = new StyleList<StylePropertyName>(new List<StylePropertyName> { new StylePropertyName("scale") });
        paper.style.transitionDuration = new StyleList<TimeValue>(new List<TimeValue> { new TimeValue(0.3f) });
        paper.style.transitionTimingFunction = new StyleList<EasingFunction>(new List<EasingFunction> { new EasingFunction(EasingMode.EaseOutBack) });
        paper.style.scale = new StyleScale(new Scale(new Vector3(1, 0, 1)));
        
        overlay.style.transitionProperty = new StyleList<StylePropertyName>(new List<StylePropertyName> { new StylePropertyName("opacity") });
        overlay.style.transitionDuration = new StyleList<TimeValue>(new List<TimeValue> { new TimeValue(0.2f) });
        overlay.style.opacity = 0f;
        
        overlay.Add(paper);
        activeRoot.Add(overlay);
        overlay.schedule.Execute(() => {
            overlay.style.opacity = 1f;
            paper.style.scale = new StyleScale(new Scale(new Vector3(1, 1, 1)));
        }).StartingIn(50);
    }
}

using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class CareerUI
{
    public static CareerUI Instance;

    public UIDocument careersDocument;

    // UI Containers
    private VisualElement jobBoardContainer;
    private VisualElement activeJobContainer;
    private ScrollView jobScrollView;
    private ScrollView activeJobScrollView;

    // Active Job UI Elements
    private Label jobTitleText;
    private Label baseSalaryText;
    private Label promoBonusText;
    private Label hoursAlignText;
    private Label salaryText;

    private Label hoursText;
    private UnityEngine.UIElements.SliderInt sliderJobHours;
    

    private Label perfProgressText;
    private VisualElement perfProgressBar;
    private Button btnPromotion;

    private VisualElement regularTasksContainer;
    private VisualElement extraTasksContainer;
    private Label regularTasksStatus;
    private Label extraTasksStatus;

    private Button btnQuitJob;

    public void Initialize(UIDocument doc)
    {
        careersDocument = doc;
        if (careersDocument == null) return;
        
        var root = careersDocument.rootVisualElement;

        // Container bindings
        jobBoardContainer = root.Q<VisualElement>("JobBoardContainer");
        activeJobContainer = root.Q<VisualElement>("ActiveJobContainer");
        jobScrollView = root.Q<ScrollView>("JobScrollView");
        activeJobScrollView = root.Q<ScrollView>("ActiveJobScrollView");

        // Active Job bindings
        jobTitleText = root.Q<Label>("JobTitleText");
        baseSalaryText = root.Q<Label>("BaseSalaryText");
        promoBonusText = root.Q<Label>("PromoBonusText");
        hoursAlignText = root.Q<Label>("HoursAlignText");
        salaryText = root.Q<Label>("SalaryText");

        hoursText = root.Q<Label>("HoursText");
        sliderJobHours = root.Q<UnityEngine.UIElements.SliderInt>("SliderJobHours");
        

        perfProgressText = root.Q<Label>("PerfVal");
        perfProgressBar = root.Q<VisualElement>("PerfFill");
        btnPromotion = root.Q<Button>("BtnPromotion");

        regularTasksContainer = root.Q<VisualElement>("regularTasksContainer");
        extraTasksContainer = root.Q<VisualElement>("extraTasksContainer");
        regularTasksStatus = root.Q<Label>("regularTasksStatus");
        extraTasksStatus = root.Q<Label>("extraTasksStatus");

        btnQuitJob = root.Q<Button>("BtnQuitJob");

        // Hook up buttons
        if (sliderJobHours != null) {
            sliderJobHours.RegisterValueChangedCallback(evt => {
                PlayerPrefs.SetInt("currentJobHours", evt.newValue);
                PlayerPrefs.Save();
                UpdateActiveJobUI();
            });
        }
        if (btnPromotion != null) btnPromotion.clicked += OnPromotionClicked;
        if (btnQuitJob != null) btnQuitJob.clicked += OnQuitJobClicked;

        RefreshView();
    }

    public void RefreshView()
    {
        var data = GameManager.Instance.currentData;
        if (data == null) return;

        if (activeJobContainer == null || jobBoardContainer == null)
        {
            Debug.LogError("[CareerUI] Containers are null! Check UXML bindings.");
            return;
        }

        if (data.jobTitle == "Unemployed")
        {
            activeJobContainer.style.display = DisplayStyle.None;
            jobBoardContainer.style.display = DisplayStyle.Flex;
            PopulateJobBoard();
        }
        else
        {
            jobBoardContainer.style.display = DisplayStyle.None;
            activeJobContainer.style.display = DisplayStyle.Flex;
            UpdateActiveJobUI();
        }
    }

    private void UpdateActiveJobUI()
    {
        var data = GameManager.Instance.currentData;
        int currentHours = PlayerPrefs.GetInt("currentJobHours", 40);
        if (sliderJobHours != null) sliderJobHours.SetValueWithoutNotify(currentHours);
        int jobLevel = PlayerPrefs.GetInt("jobLevel", 1);
        
        if (jobTitleText != null) jobTitleText.text = $"Level {jobLevel} {data.jobTitle}";
        
        // Match Kotlin logic
        double baseSalary = 0.0;
        switch (data.jobTitle) {
            case "Dishwasher": baseSalary = 120.0; break;
            case "Scrap Metal Sorter": baseSalary = 150.0; break;
            case "Street Corner Sign Spinner": baseSalary = 180.0; break;
            case "Fast Food Shift Manager": baseSalary = 400.0; break;
            case "Corporate Cubicle Drone": baseSalary = 650.0; break;
            case "Junior React Trainee": baseSalary = 900.0; break;
            case "Dystopian Quality Assurance Tester": baseSalary = 750.0; break;
            case "Executive Interstellar CEO": baseSalary = 12000.0; break;
            case "AI Prophecy Alignment Director": baseSalary = 15000.0; break;
        }

        double levelMultiplier = 1.0 + (jobLevel - 1) * 0.20;
        double hoursMultiplier = currentHours / 40.0;
        double weeklyIncome = baseSalary * levelMultiplier * hoursMultiplier;

        if (baseSalaryText != null) baseSalaryText.text = $"${baseSalary:F0}";
        if (promoBonusText != null) promoBonusText.text = $"+{((levelMultiplier - 1.0) * 100):F0}% income";
        if (hoursAlignText != null) hoursAlignText.text = $"{currentHours} hrs/wk ({hoursMultiplier:F1}x)";
        if (salaryText != null) salaryText.text = $"${weeklyIncome:F2}";

        if (hoursText != null) hoursText.text = $"{currentHours} LABORED HOURS / WEEK";
        
        // Promotion Progress (PlayerPref fallback or Data)
        float perf = PlayerPrefs.GetFloat("jobPerformance", 0f);
        if (perfProgressText != null) perfProgressText.text = $"{(perf * 100):F0}%";
        if (perfProgressBar != null) perfProgressBar.style.width = Length.Percent(perf * 100f);

        PopulateTasks();
    }

    private void PopulateTasks()
    {
        if (regularTasksContainer != null) regularTasksContainer.Clear();
        if (extraTasksContainer != null) extraTasksContainer.Clear();
        var data = GameManager.Instance.currentData;

        bool regDone = GameManager.Instance.completedRegularTaskThisWeek;
        if (regularTasksStatus != null) regularTasksStatus.text = regDone ? "✓ MET" : "⚠️ PENDING";
        if (regularTasksStatus != null) regularTasksStatus.style.color = new StyleColor(regDone ? new Color(0.18f, 0.8f, 0.44f) : new Color(0.9f, 0.3f, 0.24f));

        bool extDone = PlayerPrefs.GetInt("completedExtraTaskThisWeek", 0) == 1;
        if (extraTasksStatus != null) extraTasksStatus.text = extDone ? "✓ DONE THIS WEEK" : "⚠️ PENDING";
        if (extraTasksStatus != null) extraTasksStatus.style.color = new StyleColor(extDone ? new Color(0.22f, 0.74f, 0.97f) : new Color(0.9f, 0.3f, 0.24f));

        // Regular Tasks
        var regularTasks = GetRegularTasks(data.jobTitle);
        foreach (var task in regularTasks)
        {
            if (data.completedTasksThisWeek != null && data.completedTasksThisWeek.Contains(task.name)) continue;
            
            VisualElement card = new VisualElement();
            card.AddToClassList("task-card-stunning");

            VisualElement col = new VisualElement();
            col.style.flexDirection = FlexDirection.Column;
            col.style.flexGrow = 1;

            Label title = new Label(task.name);
            title.style.fontSize = 26; title.style.color = Color.white; title.style.unityFontStyleAndWeight = FontStyle.Bold; title.style.whiteSpace = WhiteSpace.Normal;

            Label stress = new Label($"Stress: +{(task.stress * 100):F0}%");
            stress.style.fontSize = 24; stress.style.color = new Color(0.9f, 0.3f, 0.24f); stress.style.marginTop = 5;

            col.Add(title); col.Add(stress);
            card.Add(col);

            Button btn = new Button();
            btn.text = "do task";
            btn.AddToClassList("btn-task-stunning");
            btn.SetEnabled(!regDone);
            btn.clicked += () => {
                MiniGameManager.Instance.LaunchMiniGame(new ActiveMiniGame("work_regular", task.name, task.stress));
            };
            card.Add(btn);

            regularTasksContainer.Add(card);
        }

        // Extra Tasks
        var extraTasks = GetExtraTasks(data.jobTitle);
        foreach (var task in extraTasks)
        {
            if (data.completedTasksThisWeek != null && data.completedTasksThisWeek.Contains(task.name)) continue;
            
            VisualElement card = new VisualElement();
            card.AddToClassList("task-card-stunning");
            card.AddToClassList("extra");

            VisualElement col = new VisualElement();
            col.style.flexDirection = FlexDirection.Column;
            col.style.flexGrow = 1;

            Label title = new Label(task.name);
            title.style.fontSize = 26; title.style.color = Color.white; title.style.unityFontStyleAndWeight = FontStyle.Bold; title.style.whiteSpace = WhiteSpace.Normal;

            VisualElement row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.marginTop = 5;

            Label stress = new Label($"Stress: +{(task.stress * 100):F0}%");
            stress.style.fontSize = 24; stress.style.color = new Color(0.9f, 0.3f, 0.24f); stress.style.marginRight = 15;
            
            Label readiness = new Label($"Readiness: +{(task.readiness * 100):F0}%");
            readiness.style.fontSize = 24; readiness.style.color = new Color(0.18f, 0.8f, 0.44f);

            row.Add(stress); row.Add(readiness);
            col.Add(title); col.Add(row);
            card.Add(col);

            Button btn = new Button();
            btn.text = "do task";
            btn.AddToClassList("btn-task-stunning");
            btn.AddToClassList("extra");
            btn.SetEnabled(!extDone);
            btn.clicked += () => {
                MiniGameManager.Instance.LaunchMiniGame(new ActiveMiniGame("work_extra", task.name, task.stress));
            };
            card.Add(btn);

            extraTasksContainer.Add(card);
        }
    }

    private void PopulateJobBoard()
    {
        if (jobScrollView == null) return;
        jobScrollView.Clear();

        List<JobListingSO> activeJobs = GameManager.Instance.availableJobOpenings;

        if (activeJobs == null || activeJobs.Count == 0)
        {
            Label noJobsLabel = new Label($"No vacancies posted in {GameManager.Instance.currentData.zone}.\nTick '+ Week' button to fetch new opportunities, or pay commute transit fees to migrate.");
            noJobsLabel.style.color = Color.gray;
            noJobsLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            noJobsLabel.style.marginTop = 20;
            noJobsLabel.style.fontSize = 28;
            noJobsLabel.style.whiteSpace = WhiteSpace.Normal;
            jobScrollView.Add(noJobsLabel);
            return;
        }

        foreach (var job in activeJobs)
        {
            if (job.district != GameManager.Instance.currentData.zone) continue;

            VisualElement card = new VisualElement();
            card.AddToClassList("career-card");

            VisualElement row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.justifyContent = Justify.SpaceBetween;

            VisualElement col = new VisualElement();
            col.style.flexDirection = FlexDirection.Column;
            col.style.flexShrink = 1;

            Label titleLabel = new Label(job.title);
            titleLabel.AddToClassList("career-title");

            VisualElement tagRow = new VisualElement();
            tagRow.style.flexDirection = FlexDirection.Row;
            tagRow.style.marginTop = 5;
            
            Label timeTag = new Label(job.isPartTime ? "Part-Time" : "Full-Time");
            timeTag.AddToClassList("badge");
            timeTag.AddToClassList(job.isPartTime ? "badge-orange" : "badge-blue");
            
            Label areaTag = new Label($"Area: {job.district}");
            areaTag.style.color = new StyleColor(new Color(0.6f, 0.6f, 0.6f));
            areaTag.style.fontSize = 22;
            areaTag.style.unityTextAlign = TextAnchor.MiddleLeft;

            tagRow.Add(timeTag); tagRow.Add(areaTag);
            col.Add(titleLabel); col.Add(tagRow);

            Label salaryLabel = new Label($"${job.weeklySalary}/wk");
            salaryLabel.AddToClassList("job-salary");
            salaryLabel.style.color = new Color(0.18f, 0.8f, 0.44f);
            
            row.Add(col);
            row.Add(salaryLabel);
            
            VisualElement divider1 = new VisualElement();
            divider1.style.height = 1; divider1.style.backgroundColor = new StyleColor(new Color(1f, 1f, 1f, 0.1f));
            divider1.style.marginTop = 20; divider1.style.marginBottom = 15;

            Label descLabel = new Label(job.description);
            descLabel.AddToClassList("job-desc");
            
            VisualElement divider2 = new VisualElement();
            divider2.style.height = 1; divider2.style.backgroundColor = new StyleColor(new Color(1f, 1f, 1f, 0.1f));
            divider2.style.marginTop = 15; divider2.style.marginBottom = 20;

            VisualElement bottomRow = new VisualElement();
            bottomRow.style.flexDirection = FlexDirection.Row;
            bottomRow.style.justifyContent = Justify.SpaceBetween;
            bottomRow.style.alignItems = Align.Center;

            // Verification Check
            bool meets = CheckPreReqs(job, out string _);
            float chance = CalculateHireChance(job) * 100f;

            VisualElement checkRow = new VisualElement();
            checkRow.style.flexDirection = FlexDirection.Row;
            checkRow.style.alignItems = Align.Center;

            Label checkIcon = new Label();
            checkIcon.text = meets ? "[+]" : "[-]";
            checkIcon.style.color = meets ? new Color(0.18f, 0.8f, 0.44f) : new Color(0.9f, 0.3f, 0.24f);
            checkIcon.style.fontSize = 28; checkIcon.style.unityFontStyleAndWeight = FontStyle.Bold;

            Label checkText = new Label(meets ? $"[Y] Eligible to Apply ({chance:F0}% Chance)" : "[X] Credentials block!");
            checkText.style.color = meets ? new Color(0.18f, 0.8f, 0.44f) : new Color(0.9f, 0.3f, 0.24f);
            checkText.style.fontSize = 24; checkText.style.unityFontStyleAndWeight = FontStyle.Bold; checkText.style.marginLeft = 10;

            checkRow.Add(checkIcon); checkRow.Add(checkText);

            Button applyBtn = new Button();
            applyBtn.text = "Apply";
            applyBtn.AddToClassList("btn-do-task-blue");
            if (!meets) applyBtn.style.backgroundColor = new StyleColor(Color.gray);
            
            var localJob = job;
            applyBtn.clicked += () => OnApplyClicked(localJob);

            bottomRow.Add(checkRow);
            bottomRow.Add(applyBtn);

            card.Add(row);
            card.Add(divider1);
            card.Add(descLabel);
            card.Add(divider2);
            card.Add(bottomRow);

            jobScrollView.Add(card);
        }
    }

    private bool CheckPreReqs(JobListingSO job, out string errorMsg)
    {
        var data = GameManager.Instance.currentData;
        errorMsg = "";

        if (job.requiresFinancialDistrict && data.zone != District.FinancialDistrict)
        {
            errorMsg = "Requires high society residential status. You cannot apply unless residing in the Financial District!";
            return false;
        }

        if (job.title == "Dishwasher" || job.title == "Scrap Metal Sorter" || job.title == "Street Corner Sign Spinner") return true;
        
        if (job.title == "Fast Food Shift Manager" && (data.dishwasherWeeks < 3 && !data.hasBa))
        {
            errorMsg = $"Requires BA OR at least 3 weeks of Dishwasher Experience (Current: {data.dishwasherWeeks})";
            return false;
        }

        if ((job.title == "Junior React Trainee" || job.title == "Dystopian Quality Assurance Tester") && !data.hasBootcamp && !data.hasMba)
        {
            errorMsg = "Requires MBA or Bootcamp.";
            return false;
        }
        
        if (job.title == "Corporate Cubicle Drone" && !data.hasBa && !data.hasMa && !data.hasPhd)
        {
            errorMsg = "Requires BA, MA, or PhD.";
            return false;
        }

        if (job.title == "Executive Interstellar CEO" && !data.hasMa && !data.hasPhd)
        {
            errorMsg = "Executive CEO status requires MA or PhD.";
            return false;
        }
        
        if (job.title == "AI Prophecy Alignment Director" && !data.hasPhd)
        {
            errorMsg = "Requires fully certified PhD diploma.";
            return false;
        }

        return true;
    }

    private float CalculateHireChance(JobListingSO job)
    {
        var data = GameManager.Instance.currentData;
        float genreLuck = 0.40f;
        switch (job.title)
        {
            case "Dishwasher": case "Scrap Metal Sorter": case "Street Corner Sign Spinner": genreLuck = GameManager.Instance.GetEffectiveLuck(JobSector.Manual); break;
            case "Fast Food Shift Manager": genreLuck = GameManager.Instance.GetEffectiveLuck(JobSector.Hospitality); break;
            case "Corporate Cubicle Drone": case "Executive Interstellar CEO": case "AI Prophecy Alignment Director": genreLuck = GameManager.Instance.GetEffectiveLuck(JobSector.Corp); break;
            case "Junior React Trainee": case "Corporate Quality Assurance Tester": genreLuck = GameManager.Instance.GetEffectiveLuck(JobSector.Tech); break;
        }
        genreLuck += GameManager.Instance.jobMarketLuckBonus;
        float stressFactorPenalty = data.stress * 0.25f;
        return Mathf.Clamp(genreLuck - stressFactorPenalty, 0.05f, 0.95f);
    }

    private void OnApplyClicked(JobListingSO job)
    {
        var data = GameManager.Instance.currentData;

        if (!CheckPreReqs(job, out string errorMsg))
        {
            data.stress = Mathf.Clamp(data.stress + 0.10f, 0f, 1f);
            GameUIManager.Instance.ShowPopup(new RandomEvent { 
                title = "APPLICATION BLOCK", 
                description = $"Standard compliance algorithms deleted your application. {errorMsg}\n\n<color=#E74C3C>Stress increased +10%!</color>" 
            }, () => {
                
            });
            GameManager.Instance.SaveGame();
            RefreshView();
            GameUIManager.Instance.CheckJailStatus();
            return;
        }

        // GHOST JOB RNG (20%)
        if (Random.value < 0.20f)
        {
            data.stress = Mathf.Clamp(data.stress + 0.12f, 0f, 1f);
            GameUIManager.Instance.ShowPopup(new RandomEvent { 
                title = "GHOST JOB ENCOUNTER", 
                description = $"You applied for '{job.title}' but discovered the role is a statistical facade. Received no response. Lost time and gained <color=#E74C3C>+12% stress!</color>" 
            }, () => {
                
            });
            GameManager.Instance.availableJobOpenings.Remove(job);
            GameManager.Instance.SaveGame();
            RefreshView();
            GameUIManager.Instance.CheckJailStatus();
            return;
        }

        // INTERVIEW PHASE
        if (Random.value <= CalculateHireChance(job) || data.nepotismActive)
        {
            // Hired!
            data.jobTitle = job.title;
            data.nepotismActive = false;
            data.stress = Mathf.Clamp(data.stress - 0.15f, 0f, 1f);
            PlayerPrefs.SetInt("jobLevel", 1);
            PlayerPrefs.SetInt("currentJobHours", 40);
            
            GameManager.Instance.availableJobOpenings.Remove(job);
            
            CVManager.AddJobToCV(job.title);
            
            
            GameUIManager.Instance.ShowPopup(new RandomEvent { 
                title = "YOU GOT THE JOB!", 
                description = $"Congratulations, citizen! You've been accepted for the position of {job.title}.\n\nTo keep this job and advance your career, you'll need to complete at least one weekly task. Use the Career tab to manage your work hours and ask for promotions.\n\n<color=#2ECC71>Stress reduced -15%!</color>" 
            });

            if (job.title == "Executive Interstellar CEO")
            {
                Debug.Log("[VICTORY] You climbed the modern corporate ladder and claimed the CEO desk. Interstellar Overlord Victory achieved!");
            }
        }
        else
        {
            // Rejected
            data.stress = Mathf.Clamp(data.stress + 0.15f, 0f, 1f);
            GameManager.Instance.availableJobOpenings.Remove(job);
            GameManager.Instance.rejectedJobsCooldown[job.title] = 3;
            
            GameUIManager.Instance.ShowPopup(new RandomEvent { 
                title = "RECRUITMENT DECISION", 
                description = "INTERVIEW REJECTION:\n\nAn executive nephew needed an immediate headcount placeholder to bypass the planetary draft, so your application was converted into thermal energy in our server blocks.\n\n<color=#E74C3C>Stress spiked +15%!</color>" 
            }, () => {
                
            });
        }

        GameManager.Instance.SaveGame();
        RefreshView();
        GameUIManager.Instance.CheckJailStatus();
    }

    private void OnQuitJobClicked()
    {
        GameUIManager.Instance.ShowPopup(new RandomEvent { 
            title = "RESIGNED", 
            description = $"You packed your items and walked away from '{GameManager.Instance.currentData.jobTitle}'. UNEMPLOYED!" 
        });
        GameManager.Instance.currentData.jobTitle = "Unemployed";
        PlayerPrefs.SetInt("jobLevel", 1);
        PlayerPrefs.SetInt("currentJobHours", 40);
        GameManager.Instance.SaveGame();
        RefreshView();
        GameUIManager.Instance.CheckJailStatus();
    }
    private void OnPromotionClicked()
    {
        float perf = PlayerPrefs.GetFloat("jobPerformance", 0f);
        if (Random.value < perf)
        {
            int level = PlayerPrefs.GetInt("jobLevel", 1);
            PlayerPrefs.SetInt("jobLevel", level + 1);
            PlayerPrefs.SetFloat("jobPerformance", 0f);
            
            CVManager.AddPromotionToCV(GameManager.Instance.currentData.jobTitle, level + 1);
            
            GameUIManager.Instance.ShowPopup(new RandomEvent { 
                title = "PROMOTION GRANTED!", 
                description = $"Standard HR algorithms signed your advancement! You are now level {level + 1} {GameManager.Instance.currentData.jobTitle} with a 20% wage multiplier!" 
            });
        }
        else
        {
            GameUIManager.Instance.ShowPopup(new RandomEvent { 
                title = "PROMOTION REFUSED", 
                description = "Managers ignored your requests. (Warning: excessive requests carry high firing risks!)" 
            });
        }
        RefreshView();
    }

    // Helper classes for Task generation
    private class TaskDef { public string name; public float stress; public float readiness; }

    private List<TaskDef> GetRegularTasks(string title)
    {
        if (title == "Dishwasher") return new List<TaskDef> {
            new TaskDef { name = "Scrub dinner plates", stress = 0.05f },
            new TaskDef { name = "Sweep kitchen floors", stress = 0.04f },
            new TaskDef { name = "Mop grease spills", stress = 0.05f }
        };
        if (title == "Scrap Metal Sorter") return new List<TaskDef> {
            new TaskDef { name = "Sort iron from copper", stress = 0.05f },
            new TaskDef { name = "Stack rusty steel pipes", stress = 0.06f },
            new TaskDef { name = "Clear conveyor dust filters", stress = 0.05f }
        };
        if (title == "Street Corner Sign Spinner") return new List<TaskDef> {
            new TaskDef { name = "Spin arrow sign in traffic", stress = 0.06f },
            new TaskDef { name = "Dance with sign for tourists", stress = 0.05f },
            new TaskDef { name = "Wipe smog off the neon arrow", stress = 0.04f }
        };
        if (title == "Fast Food Shift Manager") return new List<TaskDef> {
            new TaskDef { name = "Check fryer oil temperature", stress = 0.06f },
            new TaskDef { name = "Restock condiment stations", stress = 0.04f },
            new TaskDef { name = "Handle customer complaint", stress = 0.08f }
        };
        if (title == "Corporate Cubicle Drone") return new List<TaskDef> {
            new TaskDef { name = "Send weekly TPS reports", stress = 0.05f },
            new TaskDef { name = "Attend 3-hour video call", stress = 0.07f },
            new TaskDef { name = "Reorganize shared drive", stress = 0.06f }
        };
        if (title == "Junior React Trainee") return new List<TaskDef> {
            new TaskDef { name = "Test program accuracy", stress = 0.04f },
            new TaskDef { name = Random.value > 0.5f ? "Fix CSS layout locations" : "Fix CSS layout sizes", stress = 0.05f },
            new TaskDef { name = "Center content icons", stress = 0.04f }
        };
        if (title == "Dystopian Quality Assurance Tester") return new List<TaskDef> {
            new TaskDef { name = "Run automated test suite", stress = 0.04f },
            new TaskDef { name = "File bug report", stress = 0.06f },
            new TaskDef { name = "Verify hotfix deployment", stress = 0.05f }
        };
        if (title == "Executive Interstellar CEO") return new List<TaskDef> {
            new TaskDef { name = "Approve quarterly budget", stress = 0.06f },
            new TaskDef { name = "Board meeting presentation", stress = 0.08f },
            new TaskDef { name = "Sign regulatory compliance docs", stress = 0.04f }
        };
        if (title == "AI Prophecy Alignment Director") return new List<TaskDef> {
            new TaskDef { name = "Calibrate neural alignment matrix", stress = 0.08f },
            new TaskDef { name = "Review AI ethics proposals", stress = 0.06f },
            new TaskDef { name = "Monitor sentience detection array", stress = 0.07f }
        };

        // Fallback
        return new List<TaskDef> {
            new TaskDef { name = "Respond to supervisor", stress = 0.04f },
            new TaskDef { name = "Categorize ticket logs", stress = 0.05f },
            new TaskDef { name = "Format summary dashboards", stress = 0.05f }
        };
    }

    private List<TaskDef> GetExtraTasks(string title)
    {
        if (title == "Dishwasher") return new List<TaskDef> {
            new TaskDef { name = "Scour microwave grease", stress = 0.12f, readiness = 0.20f },
            new TaskDef { name = "Deep scrub waste containers", stress = 0.16f, readiness = 0.32f },
            new TaskDef { name = "Clean fryer grease vats", stress = 0.10f, readiness = 0.15f }
        };
        if (title == "Scrap Metal Sorter") return new List<TaskDef> {
            new TaskDef { name = "Inspect lithium reactor tubes", stress = 0.15f, readiness = 0.25f },
            new TaskDef { name = "Carry tungsten girders", stress = 0.18f, readiness = 0.35f },
            new TaskDef { name = "Audit sewage drainage lines", stress = 0.11f, readiness = 0.18f }
        };
        if (title == "Street Corner Sign Spinner") return new List<TaskDef> {
            new TaskDef { name = "Perform a triple backflip spin", stress = 0.15f, readiness = 0.25f },
            new TaskDef { name = "Hold sign in acid rain storm", stress = 0.18f, readiness = 0.30f },
            new TaskDef { name = "Fight rival sign spinner", stress = 0.16f, readiness = 0.28f }
        };
        if (title == "Fast Food Shift Manager") return new List<TaskDef> {
            new TaskDef { name = "Emergency food prep rush", stress = 0.18f, readiness = 0.30f },
            new TaskDef { name = "Deep clean grease traps", stress = 0.14f, readiness = 0.22f },
            new TaskDef { name = "Train new hire on POS system", stress = 0.12f, readiness = 0.20f }
        };
        if (title == "Corporate Cubicle Drone") return new List<TaskDef> {
            new TaskDef { name = "Corporate sprint tasks", stress = 0.12f, readiness = 0.20f },
            new TaskDef { name = "Overnight marketing pitch", stress = 0.16f, readiness = 0.28f },
            new TaskDef { name = "Agree with executive complaints", stress = 0.10f, readiness = 0.18f }
        };
        if (title == "Junior React Trainee") return new List<TaskDef> {
            new TaskDef { name = "Resolve Git merge conflicts", stress = 0.12f, readiness = 0.22f },
            new TaskDef { name = "Debug CSS at 3 AM", stress = 0.14f, readiness = 0.28f },
            new TaskDef { name = "Inject untested code", stress = 0.16f, readiness = 0.35f }
        };
        if (title == "Dystopian Quality Assurance Tester") return new List<TaskDef> {
            new TaskDef { name = "Regression test full app", stress = 0.15f, readiness = 0.25f },
            new TaskDef { name = "Stress test server load", stress = 0.14f, readiness = 0.22f },
            new TaskDef { name = "Write flaky test workaround", stress = 0.12f, readiness = 0.20f }
        };
        if (title == "Executive Interstellar CEO") return new List<TaskDef> {
            new TaskDef { name = "Hostile takeover negotiations", stress = 0.18f, readiness = 0.35f },
            new TaskDef { name = "Fire underperforming VP", stress = 0.12f, readiness = 0.22f },
            new TaskDef { name = "Interstellar colony site inspection", stress = 0.15f, readiness = 0.28f }
        };
        if (title == "AI Prophecy Alignment Director") return new List<TaskDef> {
            new TaskDef { name = "Emergency AI containment protocol", stress = 0.22f, readiness = 0.40f },
            new TaskDef { name = "Align superintelligence reward function", stress = 0.18f, readiness = 0.32f },
            new TaskDef { name = "Present findings to Galactic Council", stress = 0.15f, readiness = 0.25f }
        };

        // Fallback
        return new List<TaskDef> {
            new TaskDef { name = "Do corporate sprint tasks aggressively", stress = 0.12f, readiness = 0.20f },
            new TaskDef { name = "Stay overnight to align marketing pitch variables", stress = 0.14f, readiness = 0.25f },
            new TaskDef { name = "Agree verbally with automated executive complaints", stress = 0.09f, readiness = 0.16f }
        };
    }
}

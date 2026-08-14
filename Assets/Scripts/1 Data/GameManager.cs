using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Active Game State")]
    public SavedGame currentData;
    public bool isGameOver = false;
    public string gameOverReason = "";

    [Header("Dynamic Game State")]
    public RandomEvent activeEventPopup = null;
    public string activeNewsFeed = "OFFICIAL REPORT: Sector surveillance grid 100% online. Real estate and utilities stable.";
    public List<JobListing> availableJobOpenings = new List<JobListing>();
    [System.NonSerialized]
    public Dictionary<string, int> rejectedJobsCooldown = new Dictionary<string, int>();
    public List<FinancialSnapshot> financialHistory = new List<FinancialSnapshot>();
    
    public bool hasHustledThisWeek = false;
    
    // private bool parkFreeThisWeek = false;
    private float rentDiscountThisWeek = 0.0f;
    public float jobMarketLuckBonus = 0.0f;
    public bool completedRegularTaskThisWeek = false;

    // Helper to allow existing EducationUI script to access the list easily
    public List<string> completedCourses => currentData.completedCourses;

    private readonly List<JobListing> masterJobList = new List<JobListing>
    {
        new JobListing("Dishwasher", "Wash grease. Zero requirements. Unpaid trial, bring your own soap.", 120.0, new MapOfReq(0, 0, false, false, false, false, false, false), "Slums", true),
        new JobListing("Scrap Metal Sorter", "Rummage through radioactive electronic scraps.", 150.0, new MapOfReq(0, 0, false, false, false, false, false, false), "Slums", false),
        new JobListing("Street Corner Sign Spinner", "Attract clients to high-society medical experimental trials.", 180.0, new MapOfReq(0, 0, false, false, false, false, false, false), "Slums", true),
        new JobListing("Fast Food Shift Manager", "Complicated shift scheduling. Requires standard PhD (Academic Dr) OR 3 weeks Dishwasher experience.", 400.0, new MapOfReq(3, 0, true, false, false, true, false, false), "Suburbs", false),
        new JobListing("Corporate Cubicle Drone", "Process visual flow forms. Requires PHD, MBA, or Bootcamp.", 650.0, new MapOfReq(0, 0, false, false, false, false, false, false), "Suburbs", false),
        new JobListing("Junior React Trainee", "Type safe centering of divs. Requires PHD, MBA, or Bootcamp + 2 weeks dishwasher experience.", 900.0, new MapOfReq(2, 0, false, false, true, true, false, false), "Suburbs", true),
        new JobListing("Dystopian Quality Assurance Tester", "Hunt visual bugs. Requires Bootcamp.", 750.0, new MapOfReq(0, 0, false, false, true, false, false, false), "Suburbs", false),
        new JobListing("Executive Interstellar CEO", "Sign spreadsheets. Must live in Financial District, have an MBA, and Nepotism Buff active.", 12000.0, new MapOfReq(0, 0, false, true, false, false, true, true), "Financial District", false),
        new JobListing("AI Prophecy Alignment Director", "Align digital high-minds. Requires PhD, MBA, and Financial District Residency.", 15000.0, new MapOfReq(0, 0, true, true, false, false, false, true), "Financial District", false)
    };

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else 
        {
            Instance = this;
            LoadOrInitGame(); // Load data early so GameUIManager.Start doesn't hit NullReferenceException
        }
    }

    public void LoadOrInitGame()
    {
        SavedGame saved = SaveSystem.LoadActiveGame();
        if (saved != null)
        {
            currentData = saved;
            CycleJobs();
            financialHistory.Clear();
            financialHistory.Add(new FinancialSnapshot(currentData.currentWeek, currentData.balance, 0.0, currentData.rent + currentData.food));
        }
        else
        {
            ResetGameValues();
        }
    }

    private void ResetGameValues()
    {
        currentData = new SavedGame();
        isGameOver = false;
        gameOverReason = "";
        financialHistory.Clear();
        financialHistory.Add(new FinancialSnapshot(1, 500.0, 0.0, 200.0));
        rejectedJobsCooldown.Clear();
        CycleJobs();
    }

    public void SaveGame()
    {
        SaveSystem.SaveActiveGame(currentData);
    }

    public void AdvanceWeek()
    {
        if (isGameOver) return;

        // --- 1. JAIL INTERCEPT LOGIC ---
        if (currentData.jailWeeksRemaining > 0)
        {
            currentData.jailWeeksRemaining--;
            
            double rentTallyJail = currentData.rentHikeActive ? currentData.rent * 1.20 : currentData.rent;
            double totalExpenseJail = rentTallyJail + currentData.food;
            currentData.balance -= totalExpenseJail;
            
            currentData.stress = Mathf.Clamp(currentData.stress + 0.12f, 0.0f, 1.0f);
            currentData.mentalHealth = Mathf.Clamp(1.0f - currentData.stress, 0.0f, 1.0f);
            
            financialHistory.Add(new FinancialSnapshot(currentData.currentWeek, currentData.balance, 0.0, totalExpenseJail));
            currentData.hustlesCompletedThisWeek = 0;
            for (int i = currentData.hustleCooldownWeeks.Count - 1; i >= 0; i--)
            {
                currentData.hustleCooldownWeeks[i]--;
                if (currentData.hustleCooldownWeeks[i] <= 0)
                {
                    currentData.hustleCooldownWeeks.RemoveAt(i);
                    currentData.hustleCooldownNames.RemoveAt(i);
                }
            }
            currentData.completedTasksThisWeek.Clear();
            PlayerPrefs.SetInt("completedExtraTaskThisWeek", 0);
            completedRegularTaskThisWeek = false;
            hasHustledThisWeek = false;

            if (currentData.balance < 0)
            {
                TriggerGameOver("PRISON LIQUIDATION: Even inside a jail cell, your bills and housing rent kept rolling. Your bank balance fell below $0. The state seized all your items.");
                return;
            }
            if (currentData.stress >= 1.0f)
            {
                TriggerGameOver("PRISON BURNOUT: The crushing brutality and high stress of incarceration cracked your system. You collapsed inside your cell block.");
                return;
            }

            if (currentData.jailWeeksRemaining == 0)
            {
                currentData.zone = "Slums";
                currentData.rent = 50.0;
                currentData.food = 25.0;
                currentData.jobTitle = "Unemployed";
                activeNewsFeed = $"🚨 RELEASED FROM JAIL! You served your sentence and stepped out onto the Slums streets. Billed weekly rent/food: -${totalExpenseJail:F0}.";
            }
            else
            {
                activeNewsFeed = $"🔒 PRISON JOURNAL: Serving week behind bars. Sentenced remaining: {currentData.jailWeeksRemaining} weeks. Billed home expenses: -${totalExpenseJail:F0}.";
            }

            currentData.currentWeek++;
            CycleJobs();
            SaveGame();
            return; 
        }

        // --- 1.5. EDUCATION PROGRESS CHECKS ---
        if (!string.IsNullOrEmpty(currentData.activeEducationTitle))
        {
            currentData.balance -= currentData.weeklyEducationCost;
            currentData.stress = Mathf.Clamp(currentData.stress + currentData.weeklyEducationStress, 0.0f, 1.0f);
            currentData.mentalHealth = Mathf.Clamp(1.0f - currentData.stress, 0.0f, 1.0f);
            currentData.currentEducationProgressWeeks++;

            Debug.Log($"[EDUCATION STATUS] Studied for {currentData.activeEducationTitle} ({currentData.currentEducationProgressWeeks}/{currentData.totalEducationWeeksRequested} weeks). Cost paid: -${currentData.weeklyEducationCost}. Study Stress: +{(currentData.weeklyEducationStress * 100):F0}%");

            if (currentData.currentEducationProgressWeeks >= currentData.totalEducationWeeksRequested)
            {
                string completedTitle = currentData.activeEducationTitle;
                CompleteCourse(completedTitle);
                
                CVManager.AddEducationToCV(completedTitle);
                
                Debug.Log($"[ACADEMIC MAJESTY] Congratulations! You officially completed your {completedTitle} qualification!");
                
                currentData.activeEducationTitle = "";
                currentData.currentEducationProgressWeeks = 0;
                currentData.totalEducationWeeksRequested = 0;
                currentData.weeklyEducationCost = 0.0;
                currentData.weeklyEducationStress = 0.0f;
            }
        }

        // --- 2. NORMAL JOB SALARY LOGIC ---
        double baseSalary = GetBaseSalary(currentData.jobTitle);
        
        if (currentData.jobTitle == "Dishwasher") currentData.dishwasherWeeks++;
        else if (currentData.jobTitle == "Junior React Trainee") currentData.juniorDevWeeks++;

        int jobLevel = PlayerPrefs.GetInt("jobLevel", 1); 
        int currentJobHours = PlayerPrefs.GetInt("currentJobHours", 40); 
        double levelMultiplier = 1.0 + (jobLevel - 1) * 0.20;
        double hoursMultiplier = currentJobHours / 40.0;
        double weeklySalary = baseSalary * levelMultiplier * hoursMultiplier;

        double rentTally = currentData.rentHikeActive ? currentData.rent * 1.20 : currentData.rent;
        rentTally = System.Math.Max(0.0, rentTally - rentDiscountThisWeek);
        double totalExpense = rentTally + currentData.food;
        
        currentData.balance += (weeklySalary - totalExpense);
        financialHistory.Add(new FinancialSnapshot(currentData.currentWeek, currentData.balance, weeklySalary, totalExpense));

        currentData.hustlesCompletedThisWeek = 0;
        for (int i = currentData.hustleCooldownWeeks.Count - 1; i >= 0; i--)
        {
            currentData.hustleCooldownWeeks[i]--;
            if (currentData.hustleCooldownWeeks[i] <= 0)
            {
                currentData.hustleCooldownWeeks.RemoveAt(i);
                currentData.hustleCooldownNames.RemoveAt(i);
            }
        }
        if (currentData.jobTitle != "Unemployed" && !completedRegularTaskThisWeek)
        {
            string oldJob = currentData.jobTitle;
            currentData.jobTitle = "Unemployed";
            global::GameUIManager.Instance.ShowPopup(new RandomEvent { 
                title = "FIRED!", 
                description = $"You failed to complete your 3 required weekly tasks for {oldJob}. You have been terminated and security has escorted you off the premises." 
            });
        }

        currentData.completedTasksThisWeek.Clear();
        PlayerPrefs.SetInt("completedExtraTaskThisWeek", 0);
        completedRegularTaskThisWeek = false;
        hasHustledThisWeek = false;
        activeNewsFeed = $"Week {currentData.currentWeek}: Earned ${weeklySalary:F0}. Paid ${totalExpense:F0} in living expenses.";

        if (currentData.jobTitle == "Unemployed")
        {
            currentData.stress = Mathf.Clamp(currentData.stress + 0.06f, 0.0f, 1.0f);
        }

        currentData.mentalHealth = Mathf.Clamp(1.0f - currentData.stress, 0.0f, 1.0f);

        if (currentData.balance < 0)
        {
            TriggerGameOver("EVICTED! Your bank account went below $0. Debt collectors seized your bed, and the landlord locked you out.");
            return;
        }

        if (currentData.stress >= 1.0f)
        {
            if (UnityEngine.Random.value < 0.60f)
            {
                TriggerGameOver("HEART ATTACK! The 100% stress crushed your cardiovascular system. You died.");
                return;
            }
        }

        if (currentData.currentWeek % 3 == 0)
        {
            currentData.rent *= 1.12;
            currentData.food *= 1.10;
        }

        activeEventPopup = null;
        if (Random.value < 0.60f)
        {
            DrawRandomEvent();
        }

        TickCooldowns();
        CycleJobs();
        currentData.currentWeek++;
        
        // --- NEWS EVENTS LOGIC ---
        // Tick down existing events
        for (int i = currentData.activeNewsEvents.Count - 1; i >= 0; i--)
        {
            currentData.activeNewsEvents[i].remainingWeeks--;
            if (currentData.activeNewsEvents[i].remainingWeeks <= 0)
            {
                activeNewsFeed += $"\n[NEWS EXPIRED] Impact of '{currentData.activeNewsEvents[i].headline}' has worn off.";
                currentData.activeNewsEvents.RemoveAt(i);
            }
        }

        // Roll for new event
        if (Random.value < 0.70f)
        {
            var newEventTemplate = NewsEventContent.AllEvents[Random.Range(0, NewsEventContent.AllEvents.Count)];
            var activeEvent = new ActiveNewsEvent {
                headline = newEventTemplate.headline,
                luckType = newEventTemplate.luckType,
                luckModifier = newEventTemplate.luckModifier,
                remainingWeeks = newEventTemplate.durationWeeks
            };
            currentData.activeNewsEvents.Add(activeEvent);
            string impactStr = (activeEvent.luckModifier > 0 ? "+" : "") + (activeEvent.luckModifier * 100).ToString("F0") + "%";
            activeNewsFeed += $"\n[BREAKING NEWS] {activeEvent.headline}\n-> {activeEvent.luckType} luck modified by {impactStr} for {activeEvent.remainingWeeks} weeks.";
        }
        
        if (activeEventPopup != null)
        {
            activeNewsFeed += $"\n[EVENT] {activeEventPopup.title}: {activeEventPopup.description}";
        }

        // --- NEW LINE: Output the weekly summary to the UI Terminal! ---
        Debug.Log(activeNewsFeed); 
        
        if (activeEventPopup != null)
        {
            global::GameUIManager.Instance.ShowPopup(activeEventPopup);
        }
        
        SaveGame();
    }

    public float GetEffectiveLuck(string category)
    {
        float baseLuck = 0f;
        switch (category.ToLower())
        {
            case "manual": baseLuck = currentData.luckManualService; break;
            case "hospitality": baseLuck = currentData.luckHospitality; break;
            case "tech": baseLuck = currentData.luckTechnology; break;
            case "corp": baseLuck = currentData.luckCorporate; break;
        }

        float modifier = 0f;
        foreach (var ev in currentData.activeNewsEvents)
        {
            if (ev.luckType.ToLower() == category.ToLower() || ev.luckType.ToLower() == "all")
            {
                modifier += ev.luckModifier;
            }
        }

        return Mathf.Clamp(baseLuck + modifier, 0f, 1f);
    }


    // --- EDUCATION COMPLETION METHODS ---
    public void CompleteCourse(string title)
    {
        if (!currentData.completedCourses.Contains(title))
        {
            currentData.completedCourses.Add(title);
        }

        if (title == "React Bootcamp") currentData.hasBootcamp = true;
        if (title == "BA (Trash Sorting)") currentData.hasBa = true;
        if (title == "MA (Corp Aggression)") currentData.hasMa = true;
        if (title == "PhD (Greasology)") currentData.hasPhd = true;

        var boost = GetCourseBoost(title);
        if (boost.category != "None")
        {
            float amt = boost.amount;
            switch (boost.category)
            {
                case "Manual": currentData.luckManualService = Mathf.Clamp(currentData.luckManualService + amt, 0f, 1f); break;
                case "Hospitality": currentData.luckHospitality = Mathf.Clamp(currentData.luckHospitality + amt, 0f, 1f); break;
                case "Technology": currentData.luckTechnology = Mathf.Clamp(currentData.luckTechnology + amt, 0f, 1f); break;
                case "Corporate": currentData.luckCorporate = Mathf.Clamp(currentData.luckCorporate + amt, 0f, 1f); break;
            }
        }
    }

    private (string category, float amount) GetCourseBoost(string title)
    {
        switch (title)
        {
            case "React Bootcamp": return ("Technology", 0.08f);
            case "MS in Reactive Component Lifecycle": return ("Technology", 0.12f);
            case "PhD in Virtual DOM & Fiber Splicing": return ("Technology", 0.18f);

            case "BA (Trash Sorting)": return ("Manual", 0.05f);
            case "MA (Corp Aggression)": return ("Corporate", 0.10f);
            case "PhD (Greasology)": return ("Corporate", 0.15f);

            case "Solar Energy Certification": return ("Manual", 0.05f);
            case "MS in Photovoltaic Science": return ("Manual", 0.08f);
            case "PhD (Solar Quantum Fusion)": return ("Manual", 0.12f);

            case "Certified Barista Basics": return ("Hospitality", 0.05f);
            case "MS in Artisan Latte Artistry": return ("Hospitality", 0.08f);
            case "PhD (Sensory Siphon Extraction)": return ("Hospitality", 0.12f);

            case "Plumbing Systems 101": return ("Manual", 0.05f);
            case "MS in Liquid Logic Architecture": return ("Manual", 0.08f);
            case "PhD (Hydrodynamic Resonance)": return ("Manual", 0.12f);

            case "Basics of Hologram Projection": return ("Technology", 0.05f);
            case "MS in High-Fidelity Persona Design": return ("Technology", 0.08f);
            case "PhD (Synthetic Neural Theater)": return ("Technology", 0.12f);

            case "Introductory Algae Biology": return ("Manual", 0.05f);
            case "MS in Bioluminescent Agriculture": return ("Technology", 0.08f);
            case "PhD (Chloroplast Optimization)": return ("Technology", 0.12f);

            case "Basic Courier Route Navigation": return ("Manual", 0.05f);
            case "MS in Advanced Logistical Trajectories": return ("Manual", 0.08f);
            case "PhD (Sub-orbital Route Design)": return ("Manual", 0.12f);

            case "Drone Core Mechanics": return ("Technology", 0.05f);
            case "MS in Autonomous Aviatronic Engineering": return ("Technology", 0.08f);
            case "PhD (Robotic Aerodynamics)": return ("Technology", 0.12f);

            case "Introduction to Cybersecurity": return ("Technology", 0.05f);
            case "MS in Network Cryptographic Defense": return ("Technology", 0.08f);
            case "PhD (Quantum Memory Cryptology)": return ("Technology", 0.12f);

            case "Artisanal Beverage Blending": return ("Hospitality", 0.05f);
            case "MS in Gaseous Flavor Infusion": return ("Hospitality", 0.08f);
            case "PhD (Carbonated Chemistry Dynamics)": return ("Hospitality", 0.12f);

            case "Retro Typewriter Restorations": return ("Corporate", 0.05f);
            case "MS in Precision Chronograph Preservation": return ("Corporate", 0.08f);
            case "PhD (Chronological Mechanical Archeology)": return ("Corporate", 0.12f);

            default: return ("None", 0.0f);
        }
    }

    private void DrawRandomEvent()
    {
        // Keep for end-of-week events if needed, but we don't trigger the UI popup directly here since it's used for weekly flow
        float averageJobLuck = (currentData.luckManualService + currentData.luckHospitality + currentData.luckTechnology + currentData.luckCorporate) / 4.0f;
        
        bool isGood = Random.value < (0.50f + (averageJobLuck - 0.40f) * 0.35f);
        activeEventPopup = isGood 
            ? DystopianContent.GoodEvents[Random.Range(0, DystopianContent.GoodEvents.Count)] 
            : DystopianContent.BadEvents[Random.Range(0, DystopianContent.BadEvents.Count)];

        currentData.balance = System.Math.Max(0.0, currentData.balance + activeEventPopup.cashOffset);
        currentData.stress = Mathf.Clamp(currentData.stress + activeEventPopup.stressOffset, 0.0f, 1.0f);
        currentData.mentalHealth = Mathf.Clamp(1.0f - currentData.stress, 0.0f, 1.0f);

        if (!string.IsNullOrEmpty(activeEventPopup.luckType))
        {
            switch (activeEventPopup.luckType)
            {
                case "manual": currentData.luckManualService = Mathf.Clamp(currentData.luckManualService + activeEventPopup.luckOffset, 0f, 1f); break;
                case "hospitality": currentData.luckHospitality = Mathf.Clamp(currentData.luckHospitality + activeEventPopup.luckOffset, 0f, 1f); break;
                case "tech": currentData.luckTechnology = Mathf.Clamp(currentData.luckTechnology + activeEventPopup.luckOffset, 0f, 1f); break;
                case "corp": currentData.luckCorporate = Mathf.Clamp(currentData.luckCorporate + activeEventPopup.luckOffset, 0f, 1f); break;
            }
        }
    }

    private void TickCooldowns()
    {
        var keys = new List<string>(rejectedJobsCooldown.Keys);
        foreach (var key in keys)
        {
            rejectedJobsCooldown[key]--;
            if (rejectedJobsCooldown[key] <= 0)
            {
                rejectedJobsCooldown.Remove(key);
            }
        }
    }

    public void CycleJobs()
    {
        availableJobOpenings.Clear();
        var zoneJobs = masterJobList.FindAll(j => j.district == currentData.zone && (!rejectedJobsCooldown.ContainsKey(j.title) || rejectedJobsCooldown[j.title] <= 0));

        foreach (var job in zoneJobs)
        {
            if (Random.value < 0.85f)
            {
                availableJobOpenings.Add(job);
            }
        }
        
        if (availableJobOpenings.Count == 0 && zoneJobs.Count > 0)
        {
            availableJobOpenings.Add(zoneJobs[Random.Range(0, zoneJobs.Count)]);
        }
    }

    public void TriggerGameOver(string reason)
    {
        Debug.Log($"[GameManager] GAME OVER triggered. Reason: {reason}");

        // 1. Safety check for older save files that might not have a name saved
        string finalName = string.IsNullOrEmpty(currentData.playerName) ? "Unknown Citizen" : currentData.playerName;

        // 2. Save the final stats to the Graveyard (Settings Menu)
        SaveSystem.SaveHistoryRecord(
            finalName, 
            currentData.balance, 
            currentData.currentWeek, 
            currentData.jobTitle, 
            reason
        );

        // 3. Wipe the current active save so the player cannot cheat death by restarting the app
        SaveSystem.DeleteActiveGame();

        // 4. Trigger the Game Over UI Screen
        isGameOver = true;
        gameOverReason = reason;
        
        // Note: The GameOverUI.cs script monitors GameManager in its Update() loop
        // and will automatically display the death screen when isGameOver becomes true.
    }

    public void CheckHeartAttack(System.Action onSafe)
    {
        if (currentData.stress >= 1.0f && UnityEngine.Random.value < 0.60f)
        {
            TriggerGameOver("HEART ATTACK! The 100% stress crushed your cardiovascular system. You died.");
        }
        else
        {
            onSafe?.Invoke();
        }
    }

    private double GetBaseSalary(string title)
    {
        switch (title)
        {
            case "Dishwasher": return 120.0;
            case "Scrap Metal Sorter": return 150.0;
            case "Street Corner Sign Spinner": return 180.0;
            case "Fast Food Shift Manager": return 400.0;
            case "Corporate Cubicle Drone": return 650.0;
            case "Junior React Trainee": return 900.0;
            case "Corporate Quality Assurance Tester": return 750.0;
            case "Executive Interstellar CEO": return 12000.0;
            case "AI Prophecy Alignment Director": return 15000.0;
            default: return 0.0;
        }
    }
}
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
    public RandomEventSO activeEventPopup = null;
    public string activeNewsFeed = "OFFICIAL REPORT: Sector surveillance grid 100% online. Real estate and utilities stable.";
    public List<JobListingSO> availableJobOpenings = new List<JobListingSO>();
    [System.NonSerialized]
    public Dictionary<string, int> rejectedJobsCooldown = new Dictionary<string, int>();
    public List<FinancialSnapshot> financialHistory = new List<FinancialSnapshot>();
    
    public bool hasHustledThisWeek = false;
    
    // private bool parkFreeThisWeek = false;
    public float rentDiscountThisWeek = 0.0f;
    public float jobMarketLuckBonus = 0.0f;
    public bool completedRegularTaskThisWeek = false;

    // Helper to allow existing EducationUI script to access the list easily
    public List<string> completedCourses => currentData.completedCourses;

    public GameDatabaseSO gameDatabase;

    private List<IWeeklySystem> weeklySystems;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else 
        {
            Instance = this;
            if (gameDatabase == null)
            {
                gameDatabase = Resources.Load<GameDatabaseSO>("GameData/GameDatabase");
            }
            weeklySystems = new List<IWeeklySystem>
            {
                new JailSystem(),
                new EducationSystem(),
                new FinancialSystem(),
                new EventSystem()
            };
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

        foreach (var system in weeklySystems)
        {
            if (!system.ProcessWeek(currentData, this))
            {
                break;
            }
        }

        if (!isGameOver)
        {
            currentData.currentWeek++;
            TickCooldowns();
            CycleJobs();
            SaveGame();
        }
    }

    public float GetEffectiveLuck(JobSector category)
    {
        float baseLuck = 0f;
        switch (category)
        {
            case JobSector.Manual: baseLuck = currentData.luckManualService; break;
            case JobSector.Hospitality: baseLuck = currentData.luckHospitality; break;
            case JobSector.Tech: baseLuck = currentData.luckTechnology; break;
            case JobSector.Corp: baseLuck = currentData.luckCorporate; break;
        }

        float modifier = 0f;
        foreach (var ev in currentData.activeNewsEvents)
        {
            if (ev.luckType == category || ev.luckType == JobSector.All)
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

        var course = gameDatabase.courses.Find(c => c != null && c.title == title);
        if (course != null)
        {
            if (course.unlocksBootcamp) currentData.hasBootcamp = true;
            if (course.unlocksBA) currentData.hasBa = true;
            if (course.unlocksMA) currentData.hasMa = true;
            if (course.unlocksPhD) currentData.hasPhd = true;

            float amt = course.luckBoostAmount;
            switch (course.luckCategory)
            {
                case JobSector.Manual: currentData.luckManualService = Mathf.Clamp(currentData.luckManualService + amt, 0f, 1f); break;
                case JobSector.Hospitality: currentData.luckHospitality = Mathf.Clamp(currentData.luckHospitality + amt, 0f, 1f); break;
                case JobSector.Tech: currentData.luckTechnology = Mathf.Clamp(currentData.luckTechnology + amt, 0f, 1f); break;
                case JobSector.Corp: currentData.luckCorporate = Mathf.Clamp(currentData.luckCorporate + amt, 0f, 1f); break;
            }
        }
    }

    public RandomEventSO GetRandomEvent()
    {
        var validGood = gameDatabase.goodEvents.FindAll(e => e != null);
        var validBad = gameDatabase.badEvents.FindAll(e => e != null);
        
        if (validGood.Count == 0 && validBad.Count == 0) return null;

        var pool = (Random.value < 0.30f) 
            ? (validGood.Count > 0 ? validGood : validBad)
            : (validBad.Count > 0 ? validBad : validGood);

        return pool[Random.Range(0, pool.Count)];
    }

    public void DrawRandomEvent()
    {
        // Keep for end-of-week events if needed, but we don't trigger the UI popup directly here since it's used for weekly flow
        float averageJobLuck = (currentData.luckManualService + currentData.luckHospitality + currentData.luckTechnology + currentData.luckCorporate) / 4.0f;
        
        activeEventPopup = GetRandomEvent();
        if (activeEventPopup == null) return;

        currentData.balance = System.Math.Max(0.0, currentData.balance + activeEventPopup.cashOffset);
        currentData.stress = Mathf.Clamp(currentData.stress + activeEventPopup.stressOffset, 0.0f, 1.0f);
        currentData.mentalHealth = Mathf.Clamp(1.0f - currentData.stress, 0.0f, 1.0f);

        if (activeEventPopup.luckType != JobSector.None)
        {
            switch (activeEventPopup.luckType)
            {
                case JobSector.Manual: currentData.luckManualService = Mathf.Clamp(currentData.luckManualService + activeEventPopup.luckOffset, 0f, 1f); break;
                case JobSector.Hospitality: currentData.luckHospitality = Mathf.Clamp(currentData.luckHospitality + activeEventPopup.luckOffset, 0f, 1f); break;
                case JobSector.Tech: currentData.luckTechnology = Mathf.Clamp(currentData.luckTechnology + activeEventPopup.luckOffset, 0f, 1f); break;
                case JobSector.Corp: currentData.luckCorporate = Mathf.Clamp(currentData.luckCorporate + activeEventPopup.luckOffset, 0f, 1f); break;
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
        var zoneJobs = gameDatabase.jobs.FindAll(j => j != null && j.district == currentData.zone && (!rejectedJobsCooldown.ContainsKey(j.title) || rejectedJobsCooldown[j.title] <= 0));

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

    public double GetBaseSalary(string title)
    {
        var job = gameDatabase.jobs.Find(j => j != null && j.title == title);
        if (job != null)
        {
            return job.weeklySalary;
        }
        return 0.0;
    }
}
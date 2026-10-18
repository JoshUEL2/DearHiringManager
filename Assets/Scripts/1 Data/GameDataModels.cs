using System;
using System.Collections.Generic;

// Replaces the Room Entity from GameDatabase.kt
[Serializable]
public class GameRecord
{
    public string id;
    public int weeksSurvived;
    public double finalBalance;
    public string playerName;
    public string highestJob;
    public string endingAchieved;
    public long timestamp;
}

// Replaces the Room Entity from GameDatabase.kt
[Serializable]
public class SavedGame
{
    public string playerName;
    public int currentWeek = 1;
    public double balance = 500.0;
    public float stress = 0.1f;
    public double rent = 75.0;
    public double food = 25.0;
    public District zone = District.Slums; 
    public string jobTitle = "Unemployed";
    public int jobLevel = 1;
    public int currentJobHours = 40;
    public int completedExtraTaskThisWeek = 0;
    public int dishwasherWeeks = 0;
    public int juniorDevWeeks = 0;
    
    // --- EDUCATION & DEGREE TRACKING ---
    public bool hasPhd = false;
    public bool hasMba = false;
    public bool hasBootcamp = false;
    public bool hasBa = false;
    public bool hasMa = false;
    public string activeEducationTitle = "";
    public int currentEducationProgressWeeks = 0;
    public int totalEducationWeeksRequested = 0;
    public double weeklyEducationCost = 0.0;
    public float weeklyEducationStress = 0.0f;
    public List<string> completedCourses = new List<string>();
    // -----------------------------------

    public float luckBoost = 0.0f;
    public bool nepotismActive = false;
    public bool rentHikeActive = false;
    public int industryRedundancyWeeks = 0;
    public string historyLogsJson = "You are officially unemployed and broke.";
    public int jailWeeksRemaining = 0;
    public float luckManualService = 0.70f;
    public float luckHospitality = 0.40f;
    public float luckTechnology = 0.25f;
    public float luckCorporate = 0.10f;
    public float mentalHealth = 0.80f;
    public float promotionProgress = 0.0f;
    
    // Hustle Limits
    public int hustlesCompletedThisWeek = 0;
    public List<string> hustleCooldownNames = new List<string>();
    public List<int> hustleCooldownWeeks = new List<int>();
    
    public List<string> completedTasksThisWeek = new List<string>();
    
    // Active modifiers
    public List<ActiveNewsEvent> activeNewsEvents = new List<ActiveNewsEvent>();
    
    // CV Tracking
    public List<string> cvEntries = new List<string>();
}


[System.Serializable]
public class ActiveNewsEvent
{
    public string headline;
    public JobSector luckType;
    public float luckModifier;
    public int remainingWeeks;
}

[Serializable]
public class FinancialSnapshot
{
    public int week;
    public double balance;
    public double income;
    public double expenses;

    public FinancialSnapshot(int week, double balance, double income, double expenses)
    {
        this.week = week;
        this.balance = balance;
        this.income = income;
        this.expenses = expenses;
    }
}

[Serializable]
public class ActiveMiniGame
{
    public string category; 
    public string name;     
    public float stressCost;
    public float readinessGain;
    public double reward;
    public bool canFail;

    public ActiveMiniGame(string category, string name, float stressCost = 0.0f, float readinessGain = 0.0f, double reward = 0.0, bool canFail = true)
    {
        this.category = category;
        this.name = name;
        this.stressCost = stressCost;
        this.readinessGain = readinessGain;
        this.reward = reward;
        this.canFail = canFail;
    }
}



[Serializable]
public class MapOfReq
{
    public int dishwasherWeeksNeeded;
    public int juniorDevWeeksNeeded;
    public bool phdNeeded;
    public bool mbaNeeded;
    public bool bootcampNeeded;
    public bool isEntryLevelShiftSubstitute;
    public bool nepotismNeeded;
    public bool requiresFinancialDistrict;

    public MapOfReq(int dishwasherWeeksNeeded = 0, int juniorDevWeeksNeeded = 0, bool phdNeeded = false, bool mbaNeeded = false, bool bootcampNeeded = false, bool isEntryLevelShiftSubstitute = false, bool nepotismNeeded = false, bool requiresFinancialDistrict = false)
    {
        this.dishwasherWeeksNeeded = dishwasherWeeksNeeded;
        this.juniorDevWeeksNeeded = juniorDevWeeksNeeded;
        this.phdNeeded = phdNeeded;
        this.mbaNeeded = mbaNeeded;
        this.bootcampNeeded = bootcampNeeded;
        this.isEntryLevelShiftSubstitute = isEntryLevelShiftSubstitute;
        this.nepotismNeeded = nepotismNeeded;
        this.requiresFinancialDistrict = requiresFinancialDistrict;
    }
}

[Serializable]
public class SideHustleItem
{
    public string name;
    public string description;
    public double reward;
    public int riskPercent; 
    public int jailSentence; 

    public SideHustleItem(string name, string description, double reward, int riskPercent, int jailSentence)
    {
        this.name = name;
        this.description = description;
        this.reward = reward;
        this.riskPercent = riskPercent;
        this.jailSentence = jailSentence;
    }
}



// 1. The History Record model for your graveyard
[Serializable]
public class RandomEvent
{
    public string title;
    public string description;
    public double cashOffset;
    public float stressOffset;
    public string luckType; 
    public float luckOffset;

    public RandomEvent() {}

    public RandomEvent(string title, string description, double cashOffset, float stressOffset, string luckType = null, float luckOffset = 0f)
    {
        this.title = title;
        this.description = description;
        this.cashOffset = cashOffset;
        this.stressOffset = stressOffset;
        this.luckType = luckType;
        this.luckOffset = luckOffset;
    }
}

[System.Serializable]
public class HistoryRecord
{
    public string id;
    public string playerName;      
    public double finalBalance;    
    public int weeksSurvived;
    public string highestJob;
    public string endingAchieved;
}

// 2. The JSON Wrapper that tricks Unity into saving Lists
[System.Serializable]
public class Serialization<T>
{
    public List<T> target;
    
    public Serialization(List<T> target) 
    { 
        this.target = target; 
    }
}

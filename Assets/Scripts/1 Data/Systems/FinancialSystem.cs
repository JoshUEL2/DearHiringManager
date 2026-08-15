using UnityEngine;

public class FinancialSystem : IWeeklySystem
{
    public bool ProcessWeek(SavedGame data, GameManager manager)
    {
        double baseSalary = manager.GetBaseSalary(data.jobTitle);
        
        if (data.jobTitle == "Dishwasher") data.dishwasherWeeks++;
        else if (data.jobTitle == "Junior React Trainee") data.juniorDevWeeks++;

        int jobLevel = PlayerPrefs.GetInt("jobLevel", 1); 
        int currentJobHours = PlayerPrefs.GetInt("currentJobHours", 40); 
        double levelMultiplier = 1.0 + (jobLevel - 1) * 0.20;
        double hoursMultiplier = currentJobHours / 40.0;
        double weeklySalary = baseSalary * levelMultiplier * hoursMultiplier;

        double rentTally = data.rentHikeActive ? data.rent * 1.20 : data.rent;
        rentTally = System.Math.Max(0.0, rentTally - manager.rentDiscountThisWeek);
        double totalExpense = rentTally + data.food;
        
        data.balance += (weeklySalary - totalExpense);
        manager.financialHistory.Add(new FinancialSnapshot(data.currentWeek, data.balance, weeklySalary, totalExpense));

        data.hustlesCompletedThisWeek = 0;
        for (int i = data.hustleCooldownWeeks.Count - 1; i >= 0; i--)
        {
            data.hustleCooldownWeeks[i]--;
            if (data.hustleCooldownWeeks[i] <= 0)
            {
                data.hustleCooldownWeeks.RemoveAt(i);
                data.hustleCooldownNames.RemoveAt(i);
            }
        }

        if (data.jobTitle != "Unemployed" && !manager.completedRegularTaskThisWeek)
        {
            string oldJob = data.jobTitle;
            data.jobTitle = "Unemployed";
            global::GameUIManager.Instance.ShowPopup(new RandomEvent { 
                title = "FIRED!", 
                description = $"You failed to complete your 3 required weekly tasks for {oldJob}. You have been terminated and security has escorted you off the premises." 
            });
        }

        data.completedTasksThisWeek.Clear();
        PlayerPrefs.SetInt("completedExtraTaskThisWeek", 0);
        manager.completedRegularTaskThisWeek = false;
        manager.hasHustledThisWeek = false;
        manager.activeNewsFeed = $"Week {data.currentWeek}: Earned ${weeklySalary:F0}. Paid ${totalExpense:F0} in living expenses.";

        if (data.jobTitle == "Unemployed")
        {
            data.stress = Mathf.Clamp(data.stress + 0.06f, 0.0f, 1.0f);
        }

        data.mentalHealth = Mathf.Clamp(1.0f - data.stress, 0.0f, 1.0f);

        if (data.balance < 0)
        {
            manager.TriggerGameOver("EVICTED! Your bank account went below $0. Debt collectors seized your bed, and the landlord locked you out.");
            return false;
        }

        if (data.stress >= 1.0f)
        {
            if (UnityEngine.Random.value < 0.60f)
            {
                manager.TriggerGameOver("HEART ATTACK! The 100% stress crushed your cardiovascular system. You died.");
                return false;
            }
        }

        if (data.currentWeek % 3 == 0)
        {
            data.rent *= 1.12;
            data.food *= 1.10;
        }

        return true;
    }
}

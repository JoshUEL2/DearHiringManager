using UnityEngine;

public class JailSystem : IWeeklySystem
{
    public bool ProcessWeek(SavedGame data, GameManager manager)
    {
        if (data.jailWeeksRemaining > 0)
        {
            data.jailWeeksRemaining--;
            
            double rentTallyJail = data.rentHikeActive ? data.rent * 1.20 : data.rent;
            double totalExpenseJail = rentTallyJail + data.food;
            data.balance -= totalExpenseJail;
            
            data.stress = Mathf.Clamp(data.stress + 0.12f, 0.0f, 1.0f);
            data.mentalHealth = Mathf.Clamp(1.0f - data.stress, 0.0f, 1.0f);
            
            manager.financialHistory.Add(new FinancialSnapshot(data.currentWeek, data.balance, 0.0, totalExpenseJail));
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

            data.completedTasksThisWeek.Clear();
            PlayerPrefs.SetInt("completedExtraTaskThisWeek", 0);
            manager.completedRegularTaskThisWeek = false;
            manager.hasHustledThisWeek = false;

            if (data.balance < 0)
            {
                manager.TriggerGameOver("PRISON LIQUIDATION: Even inside a jail cell, your bills and housing rent kept rolling. Your bank balance fell below $0. The state seized all your items.");
                return false;
            }
            if (data.stress >= 1.0f)
            {
                manager.TriggerGameOver("PRISON BURNOUT: The crushing brutality and high stress of incarceration cracked your system. You collapsed inside your cell block.");
                return false;
            }

            if (data.jailWeeksRemaining == 0)
            {
                data.zone = District.Slums;
                data.rent = 50.0;
                data.food = 25.0;
                data.jobTitle = "Unemployed";
                manager.activeNewsFeed = $"🚨 RELEASED FROM JAIL! You served your sentence and stepped out onto the Slums streets. Billed weekly rent/food: -${totalExpenseJail:F0}.";
            }
            else
            {
                manager.activeNewsFeed = $"🔒 PRISON JOURNAL: Serving week behind bars. Sentenced remaining: {data.jailWeeksRemaining} weeks. Billed home expenses: -${totalExpenseJail:F0}.";
            }

            return false; // Halt normal processing for this week
        }
        
        return true;
    }
}

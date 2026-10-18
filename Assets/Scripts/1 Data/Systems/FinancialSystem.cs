using UnityEngine;

public class FinancialSystem : IWeeklySystem
{
    public bool ProcessWeek(SavedGame data, GameManager manager)
    {
        double weeklySalary = 0.0;

        if (data.jobTitle != "Unemployed")
        {
            if (!manager.completedRegularTaskThisWeek)
            {
                string oldJob = data.jobTitle;
                data.jobTitle = "Unemployed";
                data.jobLevel = 1;
                data.currentJobHours = 40;
                AudioManager.Instance?.PlayExpense();
                HapticManager.HeavyImpact();
                GameEvents.TriggerShowPopup(new RandomEvent { 
                    title = "FIRED!", 
                    description = $"You failed to work your required weekly shift for {oldJob}. You have been terminated and security has escorted you off the premises." 
                });
            }
            else
            {
                double baseSalary = manager.GetBaseSalary(data.jobTitle);
                if (data.jobTitle == "Dishwasher") data.dishwasherWeeks++;
                else if (data.jobTitle == "Junior React Trainee") data.juniorDevWeeks++;

                double levelMultiplier = 1.0 + (data.jobLevel - 1) * 0.20;
                double hoursMultiplier = data.currentJobHours / 40.0;
                weeklySalary = baseSalary * levelMultiplier * hoursMultiplier;

                if (weeklySalary > 0)
                {
                    AudioManager.Instance?.PlayCash();
                    HapticManager.MediumImpact();
                }
            }
        }

        double rentTally = data.rentHikeActive ? data.rent * 1.20 : data.rent;
        rentTally = System.Math.Max(0.0, rentTally - manager.rentDiscountThisWeek);
        double totalExpense = rentTally + data.food;
        if (totalExpense > 0)
        {
            AudioManager.Instance?.PlayExpense();
        }
        
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

        data.completedTasksThisWeek.Clear();
        data.completedExtraTaskThisWeek = 0;
        manager.completedRegularTaskThisWeek = false;
        manager.hasHustledThisWeek = false;
        manager.activeNewsFeed = $"Earned ${weeklySalary:F0}. Paid ${totalExpense:F0} in living expenses.";

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
        else if (data.balance < totalExpense)
        {
            AudioManager.Instance?.PlayDebtWarning();
        }

        if (data.stress >= 1.0f)
        {
            if (UnityEngine.Random.value < 0.60f)
            {
                manager.TriggerGameOver("HEART ATTACK! The 100% stress crushed your cardiovascular system. You died.");
                return false;
            }
        }

        // Controlled inflation: Every 4 weeks rent increases by 4% and food by 3%, capped at 1.5x district base
        if (data.currentWeek % 4 == 0)
        {
            double baseRent = GetDistrictBaseRent(data.zone);
            double baseFood = GetDistrictBaseFood(data.zone);
            data.rent = System.Math.Min(data.rent * 1.04, baseRent * 1.50);
            data.food = System.Math.Min(data.food * 1.03, baseFood * 1.50);
        }

        return true;
    }

    private double GetDistrictBaseRent(District zone)
    {
        switch (zone)
        {
            case District.Slums: return 75.0;
            case District.Suburbs: return 450.0;
            case District.FinancialDistrict: return 2500.0;
            default: return 75.0;
        }
    }

    private double GetDistrictBaseFood(District zone)
    {
        switch (zone)
        {
            case District.Slums: return 25.0;
            case District.Suburbs: return 100.0;
            case District.FinancialDistrict: return 400.0;
            default: return 25.0;
        }
    }
}

using UnityEngine;

public class EducationSystem : IWeeklySystem
{
    public bool ProcessWeek(SavedGame data, GameManager manager)
    {
        if (!string.IsNullOrEmpty(data.activeEducationTitle))
        {
            data.balance -= data.weeklyEducationCost;
            data.stress = Mathf.Clamp(data.stress + data.weeklyEducationStress, 0.0f, 1.0f);
            data.mentalHealth = Mathf.Clamp(1.0f - data.stress, 0.0f, 1.0f);
            data.currentEducationProgressWeeks++;

            Debug.Log($"[EDUCATION STATUS] Studied for {data.activeEducationTitle} ({data.currentEducationProgressWeeks}/{data.totalEducationWeeksRequested} weeks). Cost paid: -${data.weeklyEducationCost}. Study Stress: +{(data.weeklyEducationStress * 100):F0}%");

            if (data.currentEducationProgressWeeks >= data.totalEducationWeeksRequested)
            {
                string completedTitle = data.activeEducationTitle;
                manager.CompleteCourse(completedTitle);
                
                CVManager.AddEducationToCV(completedTitle);
                
                Debug.Log($"[ACADEMIC MAJESTY] Congratulations! You officially completed your {completedTitle} qualification!");
                
                data.activeEducationTitle = "";
                data.currentEducationProgressWeeks = 0;
                data.totalEducationWeeksRequested = 0;
                data.weeklyEducationCost = 0.0;
                data.weeklyEducationStress = 0.0f;
            }
        }
        return true;
    }
}

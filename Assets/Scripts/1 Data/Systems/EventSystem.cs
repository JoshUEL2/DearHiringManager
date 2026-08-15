using UnityEngine;

public class EventSystem : IWeeklySystem
{
    public bool ProcessWeek(SavedGame data, GameManager manager)
    {
        manager.activeEventPopup = null;
        if (Random.value < 0.60f)
        {
            manager.DrawRandomEvent();
        }

        // --- NEWS EVENTS LOGIC ---
        // Tick down existing events
        for (int i = data.activeNewsEvents.Count - 1; i >= 0; i--)
        {
            data.activeNewsEvents[i].remainingWeeks--;
            if (data.activeNewsEvents[i].remainingWeeks <= 0)
            {
                manager.activeNewsFeed += $"\n[NEWS EXPIRED] Impact of '{data.activeNewsEvents[i].headline}' has worn off.";
                data.activeNewsEvents.RemoveAt(i);
            }
        }

        // Roll for new event
        if (Random.value < 0.70f)
        {
            var newEventTemplate = manager.gameDatabase.newsEvents[Random.Range(0, manager.gameDatabase.newsEvents.Count)];
            var activeEvent = new ActiveNewsEvent {
                headline = newEventTemplate.headline,
                luckType = newEventTemplate.luckType,
                luckModifier = newEventTemplate.luckModifier,
                remainingWeeks = newEventTemplate.durationWeeks
            };
            data.activeNewsEvents.Add(activeEvent);
            string impactStr = (activeEvent.luckModifier > 0 ? "+" : "") + (activeEvent.luckModifier * 100).ToString("F0") + "%";
            manager.activeNewsFeed += $"\n[BREAKING NEWS] {activeEvent.headline}\n-> {activeEvent.luckType} luck modified by {impactStr} for {activeEvent.remainingWeeks} weeks.";
        }
        
        if (manager.activeEventPopup != null)
        {
            manager.activeNewsFeed += $"\n[EVENT] {manager.activeEventPopup.title}: {manager.activeEventPopup.description}";
        }

        // Output the weekly summary to the UI Terminal!
        Debug.Log(manager.activeNewsFeed); 
        
        if (manager.activeEventPopup != null)
        {
            global::GameUIManager.Instance.ShowPopup(manager.activeEventPopup);
        }

        return true;
    }
}

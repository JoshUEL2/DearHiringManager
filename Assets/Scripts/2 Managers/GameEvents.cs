using System;

public static class GameEvents
{
    public static event Action<RandomEventSO, Action> OnShowPopupSO;
    public static event Action<RandomEvent, Action> OnShowPopup;
    public static event Action OnSwitchToDashboard;

    public static void TriggerShowPopup(RandomEventSO ev, Action onAcknowledge = null)
    {
        OnShowPopupSO?.Invoke(ev, onAcknowledge);
    }

    public static void TriggerShowPopup(RandomEvent ev, Action onAcknowledge = null)
    {
        OnShowPopup?.Invoke(ev, onAcknowledge);
    }

    public static void TriggerSwitchToDashboard()
    {
        OnSwitchToDashboard?.Invoke();
    }
}

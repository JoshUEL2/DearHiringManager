using UnityEngine;
using UnityEngine.UIElements;

public class EventPopupUI : MonoBehaviour
{
    [Header("UI Document")]
    public UIDocument document;

    private VisualElement eventOverlay;
    private Label titleText;
    private Label descText;
    private Button btnAcknowledge;

    private void OnEnable()
    {
        if (document == null) return;
        var root = document.rootVisualElement;
        
        // CRITICAL: Set root to pass-through so this higher-sort-order panel
        // doesn't block clicks on the tab panels underneath when popup is hidden
        root.pickingMode = PickingMode.Ignore;
        
        eventOverlay = root.Q<VisualElement>("EventOverlay");
        titleText = root.Q<Label>("EventTitleText");
        descText = root.Q<Label>("EventDescText");
        btnAcknowledge = root.Q<Button>("BtnAcknowledge");

        if (btnAcknowledge != null)
            btnAcknowledge.clicked += HideEvent;

        if (eventOverlay != null)
        {
            eventOverlay.style.display = DisplayStyle.None;
            eventOverlay.style.opacity = 0f;
        }
    }

    public void ShowEvent(RandomEvent ev) 
    {
        if (eventOverlay == null) return;

        titleText.text = ev.title; 
        descText.text = ev.description; 

        eventOverlay.style.display = DisplayStyle.Flex;
        eventOverlay.style.opacity = 0f;
        var card = eventOverlay.Q<VisualElement>("EventCard");
        if (card != null) card.style.scale = new UnityEngine.UIElements.StyleScale(new UnityEngine.UIElements.Scale(new UnityEngine.Vector3(1, 0, 1)));
        eventOverlay.schedule.Execute(() => {
            eventOverlay.style.opacity = 1f;
            if (card != null) card.style.scale = new UnityEngine.UIElements.StyleScale(new UnityEngine.UIElements.Scale(new UnityEngine.Vector3(1, 1, 1)));
        }).StartingIn(50);
        eventOverlay.pickingMode = PickingMode.Position;
    }


    private void HideEvent()
    {
        if (eventOverlay != null)
        {
            var card = eventOverlay.Q<VisualElement>("EventCard");
        if (card != null) card.style.scale = new UnityEngine.UIElements.StyleScale(new UnityEngine.UIElements.Scale(new UnityEngine.Vector3(1, 0, 1)));
        eventOverlay.style.opacity = 0f;
        eventOverlay.schedule.Execute(() => {
            eventOverlay.style.display = DisplayStyle.None;
        }).StartingIn(300);
            eventOverlay.pickingMode = PickingMode.Ignore;
        }
    }
}

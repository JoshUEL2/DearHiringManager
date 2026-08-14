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
            eventOverlay.style.display = DisplayStyle.None;
    }

    public void ShowEvent(RandomEvent ev) 
    {
        if (eventOverlay == null) return;

        titleText.text = ev.title; 
        descText.text = ev.description; 

        eventOverlay.style.display = DisplayStyle.Flex;
        eventOverlay.pickingMode = PickingMode.Position;
    }


    private void HideEvent()
    {
        if (eventOverlay != null)
        {
            eventOverlay.style.display = DisplayStyle.None;
            eventOverlay.pickingMode = PickingMode.Ignore;
        }
    }
}

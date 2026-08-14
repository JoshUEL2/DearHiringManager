using UnityEngine;
using UnityEngine.UIElements;

public class JailUI : MonoBehaviour
{
    [Header("UI Document")]
    public UIDocument jailDocument;

    private Button btnWorkout;
    private Button btnTechDocs;
    private Button btnSocial;
    private Button btnBribe;

    private void OnEnable()
    {
        var root = jailDocument.rootVisualElement;

        btnWorkout = root.Q<Button>("BtnWorkout");
        btnTechDocs = root.Q<Button>("BtnTechDocs");
        btnSocial = root.Q<Button>("BtnSocial");
        btnBribe = root.Q<Button>("BtnBribe");

        btnWorkout.clicked += () => LaunchPrisonActivity("Workout");
        btnTechDocs.clicked += () => LaunchPrisonActivity("Read Tech Docs");
        btnSocial.clicked += () => LaunchPrisonActivity("Practice Social Skills");
        btnBribe.clicked += () => LaunchPrisonActivity("Bribe / C-Suite Chat");

        // The GameUIManager handles the BtnServeWeek logic, so we don't bind it here.
    }

    private void LaunchPrisonActivity(string activityName)
    {
        // 1. Check if they already did this specific activity this week
        // Note: In Phase 2, we will add a 'completedPrisonActivitiesThisWeek' list to SavedGame.cs 
        // to grey out the buttons, similar to completedRegularTaskThisWeek.

        Debug.Log($"[JAIL] Launching Prison Yard Activity: {activityName}");

        ActiveMiniGame gameConfig = new ActiveMiniGame(
            category: "jail",
            name: activityName,
            stressCost: 0.0f
        );

        MiniGameManager.Instance.LaunchMiniGame(gameConfig);
    }
}

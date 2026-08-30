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
        Debug.Log($"[JAIL] Launching Prison Yard Activity: {activityName}");

        ActiveMiniGame gameConfig = new ActiveMiniGame(
            category: "jail",
            name: activityName,
            stressCost: 0.0f
        );

        MiniGameManager.Instance.LaunchMiniGame(gameConfig);
    }

    private VisualElement lockIcon;
    private VisualElement container;
    private bool animToggle = false;
    private float animTimer = 0f;

    private void Update()
    {
        if (jailDocument == null || jailDocument.rootVisualElement == null || jailDocument.rootVisualElement.style.display == DisplayStyle.None) return;

        if (lockIcon == null) lockIcon = jailDocument.rootVisualElement.Q<Label>("LockIcon");
        if (container == null) container = jailDocument.rootVisualElement.Q<VisualElement>(className: "screen-container");

        if (lockIcon != null && container != null)
        {
            animTimer += Time.deltaTime;
            if (animTimer > 1.0f)
            {
                animTimer = 0f;
                animToggle = !animToggle;

                lockIcon.style.transitionProperty = new StyleList<StylePropertyName>(new System.Collections.Generic.List<StylePropertyName> { new StylePropertyName("scale") });
                lockIcon.style.transitionDuration = new StyleList<TimeValue>(new System.Collections.Generic.List<TimeValue> { new TimeValue(1f) });
                lockIcon.style.scale = animToggle ? new StyleScale(new Scale(new Vector3(1.1f, 1.1f, 1f))) : new StyleScale(new Scale(new Vector3(1.0f, 1.0f, 1f)));
                
                container.style.transitionProperty = new StyleList<StylePropertyName>(new System.Collections.Generic.List<StylePropertyName> { new StylePropertyName("background-color") });
                container.style.transitionDuration = new StyleList<TimeValue>(new System.Collections.Generic.List<TimeValue> { new TimeValue(1f) });
                container.style.backgroundColor = animToggle ? new StyleColor(new Color(0.1f, 0.05f, 0.05f)) : new StyleColor(new Color(0.04f, 0.07f, 0.1f));
            }
        }
    }
}

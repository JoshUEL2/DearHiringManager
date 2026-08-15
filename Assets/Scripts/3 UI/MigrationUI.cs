using UnityEngine;
using UnityEngine.UIElements;

public class MigrationUI : MonoBehaviour
{
    public static MigrationUI Instance { get; private set; }

    [Header("UI Document")]
    public UIDocument migrationDocument;

    private VisualElement migrationOverlay;
    private Label currentDistrictText;
    private Button btnMoveSlums, btnMoveSuburbs, btnMoveFinancial, btnClosePortal;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    private void OnEnable()
    {
        var root = migrationDocument.rootVisualElement;

        // CRITICAL: Let events pass through this sort-order-100 panel when portal is hidden
        root.pickingMode = PickingMode.Ignore;

        migrationOverlay = root.Q<VisualElement>("MigrationOverlay");
        currentDistrictText = root.Q<Label>("CurrentDistrictText");

        btnMoveSlums = root.Q<Button>("BtnMoveSlums");
        btnMoveSuburbs = root.Q<Button>("BtnMoveSuburbs");
        btnMoveFinancial = root.Q<Button>("BtnMoveFinancial");
        btnClosePortal = root.Q<Button>("BtnClosePortal");

        btnMoveSlums.clicked += () => MigrateTo("Slums");
        btnMoveSuburbs.clicked += () => MigrateTo("Suburbs");
        btnMoveFinancial.clicked += () => MigrateTo("Financial District");
        
        btnClosePortal.clicked += ClosePortal;

        migrationOverlay.style.display = DisplayStyle.None;
        migrationOverlay.pickingMode = PickingMode.Ignore;
    }

    public void OpenPortal()
    {
        var data = GameManager.Instance.currentData;
        currentDistrictText.text = $"Current District: {data.zone}";
        
        // 1. Evaluate Slums Eligibility (Always true)
        btnMoveSlums.SetEnabled(data.zone != District.Slums);
        btnMoveSlums.text = data.zone == District.Slums ? "Current Home" : "Migrate (Free)";

        // 2. Evaluate Suburbs Eligibility ($1200 cash req)
        bool suburbsEligible = data.balance >= 1200.0;
        btnMoveSuburbs.SetEnabled(suburbsEligible && data.zone != District.Suburbs);
        btnMoveSuburbs.text = data.zone == District.Suburbs ? "Current Home" : (suburbsEligible ? "Migrate ($600)" : "Locked");

        // 3. Evaluate Financial District Eligibility ($8000 cash req + Degree)
        bool hasDegree = data.hasPhd || data.hasMba || data.hasBootcamp;
        bool financialEligible = data.balance >= 8000.0 && hasDegree;
        btnMoveFinancial.SetEnabled(financialEligible && data.zone != District.FinancialDistrict);
        btnMoveFinancial.text = data.zone == District.FinancialDistrict ? "Current Home" : (financialEligible ? "Migrate ($3,000)" : "Locked");

        migrationOverlay.style.display = DisplayStyle.Flex;
        migrationOverlay.pickingMode = PickingMode.Position;
    }

    private void ClosePortal()
    {
        migrationOverlay.style.display = DisplayStyle.None;
        migrationOverlay.pickingMode = PickingMode.Ignore;
    }

        private void MigrateTo(string targetZone)
    {
        var data = GameManager.Instance.currentData;
        double transitionCost = 0.0;

        if (targetZone == "Suburbs") transitionCost = 600.0;
        else if (targetZone == "Financial District") transitionCost = 3000.0;

        if (data.balance >= transitionCost)
        {
            data.balance -= transitionCost;
            data.zone = System.Enum.Parse<District>(targetZone.Replace(" ", ""));

            // Apply new district living costs
            if (targetZone == "Slums")
            {
                data.rent = 75.0;
                data.food = 25.0;
                Debug.Log("Moved back to the streets.");
            }
            else if (targetZone == "Suburbs")
            {
                data.rent = 450.0;
                data.food = 100.0;
                Debug.Log("Living the suburban dream.");
            }
            else if (targetZone == "Financial District")
            {
                data.rent = 2500.0;
                data.food = 400.0;
                Debug.Log("Living among corporate high-rises.");
            }

            // Immediately refresh jobs available in the new district!
            GameManager.Instance.CycleJobs(); // Replaces the hallucinated line
            GameManager.Instance.SaveGame();
            
            FindAnyObjectByType<GameUIManager>().CheckJailStatus(); // Refreshes the dashboard text
            ClosePortal();
        }
    }
}

using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    [Header("UI Document")]
    public UIDocument gameOverDocument;

    private VisualElement gameOverOverlay;
    private VisualElement gameOverCard;
    private Label titleText, reasonText, statJobText, statWeeksText, statCashText;
    private Button btnRestart;

    private bool hasTriggered = false;

    private void OnEnable()
    {
        var root = gameOverDocument.rootVisualElement;

        // CRITICAL: Let events pass through this sort-order-100 panel when not active
        root.pickingMode = PickingMode.Ignore;

        gameOverOverlay = root.Q<VisualElement>("GameOverOverlay");
        gameOverCard = root.Q<VisualElement>("GameOverCard");
        
        titleText = root.Q<Label>("TitleText");
        reasonText = root.Q<Label>("ReasonText");
        statJobText = root.Q<Label>("StatJobText");
        statWeeksText = root.Q<Label>("StatWeeksText");
        statCashText = root.Q<Label>("StatCashText");

        btnRestart = root.Q<Button>("BtnRestart");
        btnRestart.clicked += OnRestartClicked;

        gameOverOverlay.style.display = DisplayStyle.None;
        gameOverOverlay.pickingMode = PickingMode.Ignore;
    }

    private void Update()
    {
        // Monitor the GameManager. The moment the player dies or wins, trigger the screen.
        if (!hasTriggered && GameManager.Instance != null && GameManager.Instance.isGameOver)
        {
            ShowGameOver();
        }
    }

    private void ShowGameOver()
    {
        hasTriggered = true;
        var data = GameManager.Instance.currentData;

        bool isVictory = GameManager.Instance.gameOverReason.Contains("VICTORY");

        // Dynamically style for Victory vs Defeat based on the original Kotlin colors
        Color mainColor;
        if (isVictory)
        {
            ColorUtility.TryParseHtmlString("#2ECC71", out mainColor); // BalancedHealth green
            titleText.text = "CONGRATULATIONS OVERLORD!";
            btnRestart.text = "PLAY AGAIN";
        }
        else
        {
            ColorUtility.TryParseHtmlString("#E74C3C", out mainColor); // StressWarning red
            titleText.text = "SIMULATION TERMINATED";
            btnRestart.text = "RETRY LIFE";
        }

        // Apply styles
        titleText.style.color = new StyleColor(mainColor);
        btnRestart.style.backgroundColor = new StyleColor(mainColor);
        
        gameOverCard.style.borderTopColor = new StyleColor(mainColor);
        gameOverCard.style.borderBottomColor = new StyleColor(mainColor);
        gameOverCard.style.borderLeftColor = new StyleColor(mainColor);
        gameOverCard.style.borderRightColor = new StyleColor(mainColor);

        // Apply game statistics
        reasonText.text = GameManager.Instance.gameOverReason;
        statJobText.text = $"• Terminal Job Level: {data.jobTitle}";
        statWeeksText.text = $"• Lifespan Weeks Survived: {data.currentWeek} weeks";
        statCashText.text = $"• Final Wallet Remaining: ${data.balance:F2}";

        gameOverOverlay.style.display = DisplayStyle.Flex;
        gameOverOverlay.pickingMode = PickingMode.Position;
    }

    private void OnRestartClicked()
    {
        // GameManager already wiped the save file. Reloading the Unity scene creates a totally fresh game loop!
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

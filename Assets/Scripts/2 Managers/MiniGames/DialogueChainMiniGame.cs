using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class DialogueChainMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement container;

    private Label dialogueRoundText;
    private Label dialogueScenarioText;
    private VisualElement rapportBar;
    private VisualElement dialogueTimerBar;
    private Button btnResponse1;
    private Button btnResponse2;
    private Button btnResponse3;

    private int dialogueRound = 0;
    private float rapportValue = 0.5f;
    private float dialogueTimer = 0f;
    private float dialogueTimerMax = 3f;
    private bool isPlaying = false;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        
        container = root.Q<VisualElement>("DialogueChainContainer");
        dialogueRoundText = container.Q<Label>("DialogueRoundText");
        dialogueScenarioText = container.Q<Label>("DialogueScenarioText");
        rapportBar = container.Q<VisualElement>("RapportBar");
        dialogueTimerBar = container.Q<VisualElement>("DialogueTimerBar");
        
        btnResponse1 = container.Q<Button>("BtnResponse1");
        btnResponse2 = container.Q<Button>("BtnResponse2");
        btnResponse3 = container.Q<Button>("BtnResponse3");

        if (btnResponse1 != null) btnResponse1.clicked += () => OnDialogueResponse(0);
        if (btnResponse2 != null) btnResponse2.clicked += () => OnDialogueResponse(1);
        if (btnResponse3 != null) btnResponse3.clicked += () => OnDialogueResponse(2);
    }

    public void StartGame()
    {
        isPlaying = true;
        dialogueRound = 0;
        rapportValue = 0.5f;
        container.style.display = DisplayStyle.Flex;
        UpdateDialogueUI();
        manager.StartCoroutine(DialogueChainRoutine());
    }

    public void AbortGame()
    {
        isPlaying = false;
        container.style.display = DisplayStyle.None;
    }

    private void UpdateDialogueUI()
    {
        if (dialogueRound >= config.dialogueScenarios.Length)
        {
            if (rapportValue >= config.rapportThreshold || (manager.currentGame != null && !manager.currentGame.canFail)) 
            {
                isPlaying = false;
                manager.MarkWinReady();
            }
            else 
            { 
                isPlaying = false; 
                manager.AbortGame();
            } 
            return;
        }
        
        dialogueTimerMax = Mathf.Max(1.0f, 3f - (dialogueRound * 0.5f));
        dialogueTimer = dialogueTimerMax;
        
        if (dialogueRoundText != null) dialogueRoundText.text = $"Round {dialogueRound + 1} / {config.dialogueScenarios.Length}";
        if (dialogueScenarioText != null) dialogueScenarioText.text = config.dialogueScenarios[dialogueRound];
        if (btnResponse1 != null) btnResponse1.text = config.dialogueOptions[dialogueRound][0];
        if (btnResponse2 != null) btnResponse2.text = config.dialogueOptions[dialogueRound][1];
        if (btnResponse3 != null) btnResponse3.text = config.dialogueOptions[dialogueRound][2];
        if (rapportBar != null) rapportBar.style.width = new Length(rapportValue * 100f, LengthUnit.Percent);
    }

    private void OnDialogueResponse(int idx)
    {
        if (!isPlaying || !manager.isPlaying) return;
        if (idx == config.dialogueCorrectIndices[dialogueRound]) rapportValue += 0.25f;
        else rapportValue -= 0.15f;
        
        rapportValue = Mathf.Clamp01(rapportValue);
        dialogueRound++;
        UpdateDialogueUI();
    }

    private IEnumerator DialogueChainRoutine()
    {
        while (isPlaying && manager.isPlaying && dialogueRound < config.dialogueScenarios.Length)
        {
            dialogueTimer -= Time.deltaTime;
            
            if (dialogueTimerBar != null)
            {
                float pct = Mathf.Clamp01(dialogueTimer / dialogueTimerMax);
                dialogueTimerBar.style.width = new Length(pct * 100f, LengthUnit.Percent);
                dialogueTimerBar.style.backgroundColor = pct > 0.5f ? new StyleColor(new Color(0.13f, 0.77f, 0.36f)) : (pct > 0.25f ? new StyleColor(new Color(0.96f, 0.62f, 0.04f)) : new StyleColor(Color.red));
            }
            
            if (dialogueTimer <= 0f)
            {
                if (manager.currentGame != null && manager.currentGame.canFail)
                {
                    isPlaying = false;
                    manager.AbortGame();
                    yield break;
                }
                else
                {
                    rapportValue -= 0.15f;
                    rapportValue = Mathf.Clamp01(rapportValue);
                    dialogueRound++;
                    UpdateDialogueUI();
                }
            }
            yield return null;
        }
    }
}

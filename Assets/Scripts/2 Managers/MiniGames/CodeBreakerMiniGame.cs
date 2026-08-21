using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;

public class CodeBreakerMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement container;

    private VisualElement codeGuessRow;
    private VisualElement codeSymbolRow;
    private VisualElement codeHistoryList;
    private Label codeAttemptsText;
    private Button btnCodeSubmit;
    private Button btnCodeClear;

    private string[] secretCode;
    private List<string> currentGuess = new List<string>();
    private int codeAttemptsRemaining;
    private List<Button> codeSlotButtons = new List<Button>();

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;

        container = root.Q<VisualElement>("CodeBreakerContainer");
        codeGuessRow = root.Q<VisualElement>("CodeGuessRow");
        codeSymbolRow = root.Q<VisualElement>("CodeSymbolRow");
        codeHistoryList = root.Q<VisualElement>("CodeHistoryList");
        codeAttemptsText = root.Q<Label>("CodeAttemptsText");
        btnCodeSubmit = root.Q<Button>("BtnCodeSubmit");
        btnCodeClear = root.Q<Button>("BtnCodeClear");

        if (btnCodeSubmit != null) btnCodeSubmit.clicked += OnCodeSubmit;
        if (btnCodeClear != null) btnCodeClear.clicked += OnCodeClear;
    }

    public void StartGame()
    {
        container.style.display = DisplayStyle.Flex;
        SetupCodeBreaker();
    }

    public void AbortGame()
    {
        container.style.display = DisplayStyle.None;
        if (btnCodeSubmit != null) btnCodeSubmit.clicked -= OnCodeSubmit;
        if (btnCodeClear != null) btnCodeClear.clicked -= OnCodeClear;
    }

    private void SetupCodeBreaker()
    {
        codeGuessRow.Clear();
        codeSymbolRow.Clear();
        codeHistoryList.Clear();
        codeHistoryList.style.display = DisplayStyle.None;
        currentGuess.Clear();
        codeSlotButtons.Clear();
        codeAttemptsRemaining = config.maxAttempts;
        codeAttemptsText.text = $"Attempts: {codeAttemptsRemaining}";
        
        secretCode = new string[config.codeLength];
        for(int i=0; i<config.codeLength; i++)
            secretCode[i] = config.codeSymbols[Random.Range(0, config.codeSymbols.Length)];
            
        for(int i=0; i<config.codeLength; i++)
        {
            var btn = new Button { text = "_" };
            btn.style.width = 80; btn.style.height = 80;
            btn.style.fontSize = 36;
            btn.style.marginTop = 5; btn.style.marginBottom = 5; btn.style.marginLeft = 5; btn.style.marginRight = 5;
            codeGuessRow.Add(btn);
            codeSlotButtons.Add(btn);
        }
        
        foreach(var sym in config.codeSymbols)
        {
            var btn = new Button { text = sym };
            btn.style.width = 80; btn.style.height = 80;
            btn.style.fontSize = 36;
            btn.style.marginTop = 5; btn.style.marginBottom = 5; btn.style.marginLeft = 5; btn.style.marginRight = 5;
            btn.clicked += () => {
                if (currentGuess.Count < config.codeLength) {
                    currentGuess.Add(sym);
                    UpdateCodeSlots();
                }
            };
            codeSymbolRow.Add(btn);
        }
    }

    private void UpdateCodeSlots()
    {
        for (int i=0; i<config.codeLength; i++)
        {
            codeSlotButtons[i].text = i < currentGuess.Count ? currentGuess[i] : "_";
        }
    }

    private void OnCodeClear()
    {
        currentGuess.Clear();
        UpdateCodeSlots();
    }

    private void OnCodeSubmit()
    {
        if (!manager.isPlaying || currentGuess.Count < config.codeLength) return;
        
        var histRow = new VisualElement();
        histRow.style.flexDirection = FlexDirection.Row;
        histRow.style.justifyContent = Justify.Center;
        histRow.style.marginBottom = 10;
        int correctPos = 0;
        
        for (int i=0; i<config.codeLength; i++)
        {
            var lbl = new Label(currentGuess[i]);
            lbl.style.width = 50; lbl.style.height = 50;
            lbl.style.marginTop = 5; lbl.style.marginBottom = 5; lbl.style.marginLeft = 5; lbl.style.marginRight = 5;
            lbl.style.fontSize = 32;
            lbl.style.unityTextAlign = TextAnchor.MiddleCenter;
            lbl.style.color = Color.white;
            lbl.style.borderTopLeftRadius = 8;
            lbl.style.borderTopRightRadius = 8;
            lbl.style.borderBottomLeftRadius = 8;
            lbl.style.borderBottomRightRadius = 8;
            lbl.style.unityFontStyleAndWeight = FontStyle.Bold;

            if (currentGuess[i] == secretCode[i]) {
                lbl.style.backgroundColor = new StyleColor(new Color(0.18f, 0.8f, 0.44f)); // Green
                correctPos++;
            }
            else if (secretCode.Contains(currentGuess[i])) {
                lbl.style.backgroundColor = new StyleColor(new Color(0.94f, 0.76f, 0.05f)); // Yellow
            }
            else {
                lbl.style.backgroundColor = new StyleColor(new Color(0.4f, 0.4f, 0.4f)); // Gray
            }
            histRow.Add(lbl);
        }
        
        codeHistoryList.style.display = DisplayStyle.Flex;
        codeHistoryList.Add(histRow);
        
        if (correctPos == config.codeLength) { manager.MarkWinReady(); return; }
        
        codeAttemptsRemaining--;
        codeAttemptsText.text = $"Attempts: {codeAttemptsRemaining}";
        currentGuess.Clear();
        UpdateCodeSlots();
        
        if (codeAttemptsRemaining <= 0)
        {
            if (manager.currentGame.canFail) { manager.EndGame(false); }
            else { SetupCodeBreaker(); } 
        }
    }
}

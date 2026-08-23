using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
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
    
    private bool isAnimating = false;

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
        
        ApplyJuiceLayout();
    }

    private void ApplyJuiceLayout()
    {
        // Clean Modern UI Vibe (Matching Kanban / UI Toolkit popup)
        container.style.backgroundColor = new StyleColor(Color.clear);
        container.style.borderTopWidth = 0;
        
        codeAttemptsText.style.color = new StyleColor(new Color(0.9f, 0.7f, 0.1f)); // Amber/Yellow
        codeAttemptsText.style.unityFontStyleAndWeight = FontStyle.Bold;
        
        // Restore instruction text
        var instructionLabel = container.parent.Q<Label>("MiniGameInstructions");
        if (instructionLabel != null)
        {
            instructionLabel.style.color = new StyleColor(Color.white);
        }
    }

    public void StartGame()
    {
        container.style.display = DisplayStyle.Flex;
        isAnimating = false;
        SetupCodeBreaker();
    }

    public void AbortGame()
    {
        container.style.display = DisplayStyle.None;
        isAnimating = false;
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
        codeAttemptsText.text = $"ATTEMPTS REMAINING: {codeAttemptsRemaining}";
        
        // --- ADD LOGIN SCREEN VISUALS ---
        if (container.Q<VisualElement>("LoginHeader") == null)
        {
            var loginHeader = new VisualElement { name = "LoginHeader" };
            loginHeader.style.flexDirection = FlexDirection.Row;
            loginHeader.style.alignItems = Align.Center;
            loginHeader.style.marginBottom = 15;
            
            var userLabel = new Label("Username");
            userLabel.style.color = new StyleColor(new Color(0.6f, 0.7f, 0.8f));
            userLabel.style.fontSize = 24;
            userLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            userLabel.style.marginRight = 15;
            
            var userBox = new VisualElement();
            userBox.style.backgroundColor = new StyleColor(new Color(0.12f, 0.16f, 0.23f)); // Slate
            userBox.style.borderTopWidth = 2; userBox.style.borderLeftWidth = 2; userBox.style.borderBottomWidth = 2; userBox.style.borderRightWidth = 2;
            StyleColor borderColor = new StyleColor(new Color(0.2f, 0.25f, 0.34f));
            userBox.style.borderTopColor = borderColor; userBox.style.borderBottomColor = borderColor; userBox.style.borderLeftColor = borderColor; userBox.style.borderRightColor = borderColor;
            userBox.style.borderTopLeftRadius = 8; userBox.style.borderTopRightRadius = 8; userBox.style.borderBottomLeftRadius = 8; userBox.style.borderBottomRightRadius = 8;
            userBox.style.paddingTop = 10; userBox.style.paddingBottom = 10; userBox.style.paddingLeft = 85; userBox.style.paddingRight = 85;
            
            var userText = new Label("Admin");
            userText.style.color = Color.white;
            userText.style.fontSize = 24;
            userText.style.unityFontStyleAndWeight = FontStyle.Bold;
            
            userBox.Add(userText);
            loginHeader.Add(userLabel);
            loginHeader.Add(userBox);
            
            // Insert below Attempts (index 1)
            container.Insert(1, loginHeader);
        }
        
        // Add "Password" label to the Guess row
        var passLabel = new Label("Password");
        passLabel.style.color = new StyleColor(new Color(0.6f, 0.7f, 0.8f));
        passLabel.style.fontSize = 24;
        passLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        passLabel.style.marginRight = 15;
        passLabel.style.alignSelf = Align.Center;
        codeGuessRow.Add(passLabel);
        // --------------------------------

        string[] wordList = new string[] {
            "PAST", "PART", "PEST", "PEAR", "PATE", "STAR", "SEAT", 
            "STEP", "SPAT", "SPAR", "SEAR", "TEAR", "TART", "TEST", 
            "TRAP", "TREE", "TAPE", "REST", "REAP", "RARE", "RATE", 
            "RATS", "REAR", "EAST", "EARS", "EATS", "PASS", "PEAS", 
            "ARTS", "APES"
        };
        
        string chosenWord = wordList[Random.Range(0, wordList.Length)];
        secretCode = new string[config.codeLength];
        for(int i=0; i<config.codeLength; i++)
            secretCode[i] = chosenWord[i].ToString();
            
        for(int i=0; i<config.codeLength; i++)
        {
            var btn = new Button { text = "_" };
            StyleTerminalButton(btn, true);
            codeGuessRow.Add(btn);
            codeSlotButtons.Add(btn);
        }
        
        // 6-letter mini keyboard
        string allowedLetters = "AESTRP";
        foreach(char c in allowedLetters)
        {
            string sym = c.ToString();
            var btn = new Button { text = sym };
            StyleTerminalButton(btn, false);
            btn.clicked += () => {
                if (currentGuess.Count < config.codeLength && !isAnimating) {
                    currentGuess.Add(sym);
                    UpdateCodeSlots();
                }
            };
            codeSymbolRow.Add(btn);
        }
        
        StyleActionButtons();
    }
    
    private void StyleTerminalButton(Button btn, bool isGuessSlot)
    {
        btn.style.width = isGuessSlot ? 70 : 60;
        btn.style.height = isGuessSlot ? 70 : 60;
        btn.style.fontSize = isGuessSlot ? 40 : 30;
        
        btn.style.marginTop = 5; btn.style.marginBottom = 5; btn.style.marginLeft = 5; btn.style.marginRight = 5;
        
        // Clean Modern Wordle Style
        btn.style.backgroundColor = new StyleColor(new Color(0.12f, 0.16f, 0.23f)); // Slate Blue
        btn.style.color = new StyleColor(Color.white);
        btn.style.unityFontStyleAndWeight = FontStyle.Bold;
        
        btn.style.borderTopLeftRadius = 8;
        btn.style.borderTopRightRadius = 8;
        btn.style.borderBottomLeftRadius = 8;
        btn.style.borderBottomRightRadius = 8;
        
        btn.style.borderTopWidth = 2; btn.style.borderLeftWidth = 2; btn.style.borderBottomWidth = 2; btn.style.borderRightWidth = 2;
        StyleColor borderColor = new StyleColor(new Color(0.2f, 0.25f, 0.34f)); // Light Slate border
        btn.style.borderTopColor = borderColor;
        btn.style.borderBottomColor = borderColor;
        btn.style.borderLeftColor = borderColor;
        btn.style.borderRightColor = borderColor;
    }
    
    private void StyleActionButtons()
    {
        // Restore to default UXML styling by clearing inline styles
        btnCodeSubmit.style.backgroundColor = new StyleColor(StyleKeyword.Null);
        btnCodeSubmit.style.color = new StyleColor(StyleKeyword.Null);
        
        btnCodeClear.style.backgroundColor = new StyleColor(StyleKeyword.Null);
        btnCodeClear.style.color = new StyleColor(StyleKeyword.Null);
    }

    private void UpdateCodeSlots()
    {
        for (int i=0; i<config.codeLength; i++)
        {
            codeSlotButtons[i].text = i < currentGuess.Count ? currentGuess[i] : "_";
            
            // Pop juice
            if (i == currentGuess.Count - 1 && currentGuess.Count > 0)
            {
                manager.StartCoroutine(ScalePop(codeSlotButtons[i]));
            }
        }
    }
    
    private IEnumerator ScalePop(VisualElement el)
    {
        float t = 0;
        while (t < 1f && manager.isPlaying)
        {
            t += Time.deltaTime * 15f;
            float scale = 1f + Mathf.Sin(t * Mathf.PI) * 0.2f;
            el.style.scale = new StyleScale(new Vector2(scale, scale));
            yield return null;
        }
        el.style.scale = new StyleScale(new Vector2(1f, 1f));
    }

    private void OnCodeClear()
    {
        if (isAnimating) return;
        currentGuess.Clear();
        UpdateCodeSlots();
    }

    private void OnCodeSubmit()
    {
        if (!manager.isPlaying || currentGuess.Count < config.codeLength || isAnimating) return;
        manager.StartCoroutine(SubmitRoutine());
    }
    
    private IEnumerator SubmitRoutine()
    {
        isAnimating = true;
        
        var histRow = new VisualElement();
        histRow.style.flexDirection = FlexDirection.Row;
        histRow.style.justifyContent = Justify.Center;
        histRow.style.marginBottom = 10;
        
        // Setup blocks
        List<Label> histBlocks = new List<Label>();
        for (int i=0; i<config.codeLength; i++)
        {
            var lbl = new Label(currentGuess[i]);
            lbl.style.width = 50; lbl.style.height = 50;
            lbl.style.marginTop = 5; lbl.style.marginBottom = 5; lbl.style.marginLeft = 5; lbl.style.marginRight = 5;
            lbl.style.fontSize = 32;
            lbl.style.unityTextAlign = TextAnchor.MiddleCenter;
            lbl.style.color = Color.white;
            lbl.style.borderTopLeftRadius = 4; lbl.style.borderTopRightRadius = 4; lbl.style.borderBottomLeftRadius = 4; lbl.style.borderBottomRightRadius = 4;
            lbl.style.unityFontStyleAndWeight = FontStyle.Bold;
            
            // Start invisible/scaled down
            lbl.style.scale = new StyleScale(new Vector2(0f, 0f));
            lbl.style.backgroundColor = new StyleColor(new Color(0.2f, 0.2f, 0.2f));
            
            histRow.Add(lbl);
            histBlocks.Add(lbl);
        }
        
        codeHistoryList.style.display = DisplayStyle.Flex;
        codeHistoryList.Add(histRow);
        
        // Proper Wordle Logic
        Dictionary<string, int> secretCounts = new Dictionary<string, int>();
        foreach (var c in secretCode) 
        {
            if (!secretCounts.ContainsKey(c)) secretCounts[c] = 0;
            secretCounts[c]++;
        }
        
        bool[] isGreen = new bool[config.codeLength];
        int correctPos = 0;
        
        // Pass 1: Greens
        for (int i=0; i<config.codeLength; i++) {
            if (currentGuess[i] == secretCode[i]) {
                isGreen[i] = true;
                secretCounts[currentGuess[i]]--;
                correctPos++;
            }
        }
        
        // Pass 2: Yellows & Grays + Animation
        for (int i=0; i<config.codeLength; i++) 
        {
            Color targetColor;
            if (isGreen[i]) 
            {
                targetColor = new Color(0.18f, 0.8f, 0.44f); // Standard Wordle Green
            } 
            else if (secretCounts.ContainsKey(currentGuess[i]) && secretCounts[currentGuess[i]] > 0) 
            {
                targetColor = new Color(0.94f, 0.76f, 0.05f); // Standard Wordle Yellow
                secretCounts[currentGuess[i]]--;
            } 
            else 
            {
                targetColor = new Color(0.2f, 0.25f, 0.34f); // Slate Gray for misses
            }
            
            // Pop animation for reveal
            histBlocks[i].style.backgroundColor = new StyleColor(targetColor);
            float t = 0;
            while (t < 1f && manager.isPlaying)
            {
                t += Time.deltaTime * 8f;
                float s = Mathf.Lerp(0f, 1f, t * t * (3f - 2f * t)); // Smoothstep
                histBlocks[i].style.scale = new StyleScale(new Vector2(s, s));
                yield return null;
            }
            histBlocks[i].style.scale = new StyleScale(new Vector2(1f, 1f));
            yield return new WaitForSeconds(0.05f); // Stagger
        }
        
        if (correctPos == config.codeLength) 
        { 
            yield return WaveBounce(histBlocks);
            manager.MarkWinReady(); 
            yield break;
        }
        
        manager.StartCoroutine(ShakeRoutine(histRow)); // Shake on fail
        
        codeAttemptsRemaining--;
        codeAttemptsText.text = $"ATTEMPTS REMAINING: {codeAttemptsRemaining}";
        currentGuess.Clear();
        UpdateCodeSlots();
        
        isAnimating = false;
        
        if (codeAttemptsRemaining <= 0)
        {
            if (manager.currentGame.canFail) { manager.EndGame(false); }
            else { SetupCodeBreaker(); } 
        }
    }
    
    private IEnumerator WaveBounce(List<Label> blocks)
    {
        for (int i = 0; i < blocks.Count; i++)
        {
            manager.StartCoroutine(ScalePop(blocks[i]));
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(0.3f);
    }
    
    private IEnumerator ShakeRoutine(VisualElement el)
    {
        float duration = 0.4f;
        float elapsed = 0f;
        while (elapsed < duration && manager.isPlaying)
        {
            elapsed += Time.deltaTime;
            float xOffset = Mathf.Sin(elapsed * 40f) * 10f;
            el.style.translate = new Translate(xOffset, 0, 0);
            yield return null;
        }
        el.style.translate = new Translate(0, 0, 0);
    }
}

import re

# 1. Patch MiniGameManager.cs
path_mgr = 'Assets/Scripts/2 Managers/MiniGameManager.cs'
with open(path_mgr, 'r') as f:
    content_mgr = f.read()

target_mgr = r'instructions = "Crack the system password to access database!"'
replacement_mgr = r'instructions = "Guess the password to access database!"'
content_mgr = re.sub(target_mgr, replacement_mgr, content_mgr)

with open(path_mgr, 'w') as f:
    f.write(content_mgr)

# 2. Patch CodeBreakerMiniGame.cs
path_cb = 'Assets/Scripts/2 Managers/MiniGames/CodeBreakerMiniGame.cs'
with open(path_cb, 'r') as f:
    content_cb = f.read()

target_cb = r'''        codeGuessRow\.Clear\(\);
        codeSymbolRow\.Clear\(\);
        codeHistoryList\.Clear\(\);
        codeHistoryList\.style\.display = DisplayStyle\.None;
        currentGuess\.Clear\(\);
        codeSlotButtons\.Clear\(\);
        codeAttemptsRemaining = config\.maxAttempts;
        codeAttemptsText\.text = \$"ATTEMPTS REMAINING: \{codeAttemptsRemaining\}";
        
        string\[\] wordList = new string\[\] \{'''

replacement_cb = r'''        codeGuessRow.Clear();
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

        string[] wordList = new string[] {'''

content_cb = re.sub(target_cb, replacement_cb, content_cb)

with open(path_cb, 'w') as f:
    f.write(content_cb)

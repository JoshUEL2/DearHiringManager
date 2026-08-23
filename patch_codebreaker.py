import re

path = 'Assets/Scripts/2 Managers/MiniGames/CodeBreakerMiniGame.cs'
with open(path, 'r') as f:
    content = f.read()

target = r'''        secretCode = new string\[config\.codeLength\];
        for\(int i=0; i<config\.codeLength; i\+\+\)
            secretCode\[i\] = config\.codeSymbols\[Random\.Range\(0, config\.codeSymbols\.Length\)\];
            
        for\(int i=0; i<config\.codeLength; i\+\+\)
        \{
            var btn = new Button \{ text = "_" \};
            StyleTerminalButton\(btn, true\);
            codeGuessRow\.Add\(btn\);
            codeSlotButtons\.Add\(btn\);
        \}
        
        foreach\(var sym in config\.codeSymbols\)
        \{
            var btn = new Button \{ text = sym \};
            StyleTerminalButton\(btn, false\);
            btn\.clicked \+= \(\) => \{
                if \(currentGuess\.Count < config\.codeLength && !isAnimating\) \{
                    currentGuess\.Add\(sym\);
                    UpdateCodeSlots\(\);
                \}
            \};
            codeSymbolRow\.Add\(btn\);
        \}'''

replacement = r'''        string[] wordList = new string[] {
            "DATA", "CODE", "FILE", "NODE", "BYTE", "TASK", "WORK", 
            "PLAN", "SYNC", "LOAD", "SAVE", "HACK", "CASH", "BOSS",
            "MEET", "DESK", "MEMO", "MAIL", "CALL", "SEND", "READ",
            "EDIT", "TEST", "PASS", "USER", "ROOT", "BOOT", "DISK",
            "WIFI", "TECH", "CORE", "BASE", "PORT", "LINK", "PING",
            "HOST", "MAIN", "PUSH", "PULL", "SORT", "FIND"
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
        
        string qwerty = "QWERTYUIOPASDFGHJKLZXCVBNM";
        foreach(char c in qwerty)
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
        }'''

content = re.sub(target, replacement, content)

with open(path, 'w') as f:
    f.write(content)

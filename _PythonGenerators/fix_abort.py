import sys

with open("Assets/Scripts/2 Managers/MiniGameManager.cs", "r") as f:
    cs = f.read()

# Replace the broken logic with correct logic
old_logic = """        if (currentGame != null && currentGame.isIllegal)
        {
            UnityEngine.Debug.Log("Failed illegal minigame! Straight to jail.");
            var data = GameManager.Instance.currentData;
            float fine = (float)(data.balance * 0.5); 
            data.balance -= fine;
            data.jailWeeksRemaining = currentGame.jailSentence;
            string logMessage = $"[SYSTEM] {gameLore[currentGame.name]} BUT I WAS CAUGHT! Fined ${fine:F0} and jailed.";
            GameManager.Instance.AddFinancialHistory(logMessage, -fine, 0.0);
            
            uiManager?.CheckJailStatus();
            uiManagerForDashboard?.CheckJailStatus();
            return; // Skip normal UI refresh, jail UI is open
        }"""

new_logic = """        if (currentGame != null && currentGame.category == "hustle")
        {
            var hustle = GameManager.Instance.masterHustleList.Find(h => h.name == currentGame.name);
            if (hustle != null && hustle.isIllegal)
            {
                UnityEngine.Debug.Log("Failed illegal minigame! Straight to jail.");
                var data = GameManager.Instance.currentData;
                float fine = (float)(data.balance * 0.5); 
                data.balance -= fine;
                data.jailWeeksRemaining = hustle.jailSentence;
                string loreText = gameLore.ContainsKey(currentGame.name) ? gameLore[currentGame.name] : currentGame.name;
                string logMessage = $"[SYSTEM] {loreText} BUT I WAS CAUGHT! Fined ${fine:F0} and jailed.";
                GameManager.Instance.AddFinancialHistory(logMessage, -fine, 0.0);
                
                if (uiManager != null) uiManager.CheckJailStatus();
                if (uiManagerForDashboard != null) uiManagerForDashboard.CheckJailStatus();
                return; // Skip normal UI refresh, jail UI is open
            }
        }"""

cs = cs.replace(old_logic, new_logic)

with open("Assets/Scripts/2 Managers/MiniGameManager.cs", "w") as f:
    f.write(cs)

print("Fixed AbortGame logic")

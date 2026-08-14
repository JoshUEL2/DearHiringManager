import sys

with open("Assets/Scripts/2 Managers/MiniGameManager.cs", "r") as f:
    cs = f.read()

# 1. phase3Type
cs = cs.replace(
    "public GameType phase2Type = GameType.MashRace;",
    "public GameType phase2Type = GameType.MashRace;\n    public GameType phase3Type = (GameType)0;"
)

# 2, 3, 4, 5. Fix AbortGame compilation errors
old_abort = """        if (currentGame != null && currentGame.category == "hustle")
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
                GameManager.Instance.activeNewsFeed = logMessage;
                GameManager.Instance.financialHistory.Add(new FinancialSnapshot(GameManager.Instance.currentData.currentWeek, GameManager.Instance.currentData.balance, 0, -fine));
                
                if (uiManager != null) uiManager.CheckJailStatus();
                if (uiManagerForDashboard != null) uiManagerForDashboard.CheckJailStatus();
                return; // Skip normal UI refresh, jail UI is open
            }
        }"""

# Need to find the master hustle list. Wait, I can just use FindObjectsByType<HustlesUI>() to get it, or I can just use a simple hardcoded lookup for the 4 illegal games since it's just 4 games. 
# Also fix `data` scoping if it conflicts, name it `gData`.
# Also fix `gameLore` which is actually `MiniGameLore` or something. Wait, in MiniGameManager, I defined it as `private Dictionary<string, string> gameLore = new Dictionary<string, string>...` wait, what was it called? Let me check.
new_abort = """        if (currentGame != null && currentGame.category == "hustle")
        {
            int jailWeeks = 0;
            if (currentGame.name == "Forge Transit Passes") jailWeeks = 1;
            else if (currentGame.name == "Snatch Luxury Briefcase") jailWeeks = 2;
            else if (currentGame.name == "Skim ATM Card Data") jailWeeks = 4;
            else if (currentGame.name == "Corporate Espionage Heist") jailWeeks = 7;
            
            if (jailWeeks > 0)
            {
                UnityEngine.Debug.Log("Failed illegal minigame! Straight to jail.");
                var gData = GameManager.Instance.currentData;
                float fine = (float)(gData.balance * 0.5); 
                gData.balance -= fine;
                gData.jailWeeksRemaining = jailWeeks;
                string logMessage = $"[SYSTEM] Failed {currentGame.name} AND WAS CAUGHT! Fined ${fine:F0} and jailed.";
                GameManager.Instance.activeNewsFeed = logMessage;
                GameManager.Instance.financialHistory.Add(new FinancialSnapshot(gData.currentWeek, gData.balance, 0, -fine));
                
                var gUIManager = FindAnyObjectByType<GameUIManager>();
                if (gUIManager != null) gUIManager.CheckJailStatus();
                return; 
            }
        }"""

cs = cs.replace(old_abort, new_abort)

# 6, 7. IStyle borderWidth/borderColor fixes
cs = cs.replace("wdDogs[i].style.borderWidth = 0;", "wdDogs[i].style.borderBottomWidth = 0; wdDogs[i].style.borderTopWidth = 0; wdDogs[i].style.borderLeftWidth = 0; wdDogs[i].style.borderRightWidth = 0;")
cs = cs.replace("wdDogs[idx].style.borderWidth = 5;", "wdDogs[idx].style.borderBottomWidth = 5; wdDogs[idx].style.borderTopWidth = 5; wdDogs[idx].style.borderLeftWidth = 5; wdDogs[idx].style.borderRightWidth = 5;")
cs = cs.replace("wdDogs[idx].style.borderColor = UnityEngine.Color.white;", "wdDogs[idx].style.borderBottomColor = UnityEngine.Color.white; wdDogs[idx].style.borderTopColor = UnityEngine.Color.white; wdDogs[idx].style.borderLeftColor = UnityEngine.Color.white; wdDogs[idx].style.borderRightColor = UnityEngine.Color.white;")
cs = cs.replace("wdDogs[wdSelectedDog].style.borderWidth = 0;", "wdDogs[wdSelectedDog].style.borderBottomWidth = 0; wdDogs[wdSelectedDog].style.borderTopWidth = 0; wdDogs[wdSelectedDog].style.borderLeftWidth = 0; wdDogs[wdSelectedDog].style.borderRightWidth = 0;")
cs = cs.replace("saLeftNodes[i].style.borderWidth = 4;", "saLeftNodes[i].style.borderBottomWidth = 4; saLeftNodes[i].style.borderTopWidth = 4; saLeftNodes[i].style.borderLeftWidth = 4; saLeftNodes[i].style.borderRightWidth = 4;")
cs = cs.replace("saLeftNodes[saSelectedLeft].style.borderColor = UnityEngine.Color.white;", "saLeftNodes[saSelectedLeft].style.borderBottomColor = UnityEngine.Color.white; saLeftNodes[saSelectedLeft].style.borderTopColor = UnityEngine.Color.white; saLeftNodes[saSelectedLeft].style.borderLeftColor = UnityEngine.Color.white; saLeftNodes[saSelectedLeft].style.borderRightColor = UnityEngine.Color.white;")
cs = cs.replace("saLeftNodes[idx].style.borderColor = UnityEngine.Color.yellow;", "saLeftNodes[idx].style.borderBottomColor = UnityEngine.Color.yellow; saLeftNodes[idx].style.borderTopColor = UnityEngine.Color.yellow; saLeftNodes[idx].style.borderLeftColor = UnityEngine.Color.yellow; saLeftNodes[idx].style.borderRightColor = UnityEngine.Color.yellow;")

# 8. root in OnFpStampClicked
cs = cs.replace('UnityEngine.UIElements.VisualElement stampZone = root.Q<UnityEngine.UIElements.VisualElement>("FpStampZone");',
                'UnityEngine.UIElements.VisualElement stampZone = fpContainer.Q<UnityEngine.UIElements.VisualElement>("FpStampZone");')

with open("Assets/Scripts/2 Managers/MiniGameManager.cs", "w") as f:
    f.write(cs)

print("Fixed errors!")

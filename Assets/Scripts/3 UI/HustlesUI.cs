using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class HustlesUI
{
    public static HustlesUI Instance;

    public UIDocument hustlesDocument;

    private ScrollView hustleScrollView;

    // Made public static so MiniGameManager can access it!
    public static List<SideHustleItem> availableHustles = new List<SideHustleItem>();

    public void Initialize(UIDocument doc)
    {
        hustlesDocument = doc;
        if (hustlesDocument == null) return;
        LoadHustleData();
        var root = hustlesDocument.rootVisualElement;
        hustleScrollView = root.Q<ScrollView>();
        hustleScrollView.contentContainer.style.flexDirection = FlexDirection.Row;
        hustleScrollView.contentContainer.style.flexWrap = Wrap.Wrap;
        hustleScrollView.contentContainer.style.justifyContent = Justify.Center;
        PopulateHustlesBoard();
    }

    private void LoadHustleData()
    {
        availableHustles.Clear();
        availableHustles.Add(new SideHustleItem("Collect Recycled Bottles", "Scour dumpster bins for refundable glass/plastic bottles.", 10.0, 0, 0));
        availableHustles.Add(new SideHustleItem("Executive Lawn Care", "Trim putting greens for elite executives.", 22.0, 0, 0));
        availableHustles.Add(new SideHustleItem("Walk Purebred Pedigree Dogs", "Wrangle incredibly spoiled and aggressive purebreds.", 45.0, 0, 0));
        availableHustles.Add(new SideHustleItem("Deliver Fast Food Order", "Zip around traffic on motorized unicycle deliveries.", 80.0, 0, 0));
        
        availableHustles.Add(new SideHustleItem("Forge Transit Passes", "Forge digital tokens for high-speed transit micro-gates.", 280.0, 15, 1));
        availableHustles.Add(new SideHustleItem("Crack Luxury Briefcase", "Pick the combo locks on stolen executive briefcases.", 650.0, 28, 2));
        availableHustles.Add(new SideHustleItem("Skim ATM Card Data", "Install discreet card skimmers on Slum ATMs.", 1600.0, 48, 4));
        availableHustles.Add(new SideHustleItem("Corporate Espionage Heist", "Infiltrate R&D mainframe to wire out proprietary blueprints.", 4800.0, 68, 7));
    }

    public void PopulateHustlesBoard()
    {
        if (hustleScrollView == null) return;
        hustleScrollView.Clear();

        var data = GameManager.Instance.currentData;
        int hustlesLeft = 3 - data.hustlesCompletedThisWeek;
        
        

        foreach (var hustle in availableHustles)
        {
            bool isIllegal = hustle.riskPercent > 0;
            
            // Context icon & colors
            string iconStr = "💼";
            Color iconBg = new Color(0.2f, 0.2f, 0.3f);
            if (hustle.name.Contains("Bottle")) { iconStr = "♻️"; iconBg = new Color(0.18f, 0.5f, 0.34f); }
            else if (hustle.name.Contains("Lawn")) { iconStr = "⛳"; iconBg = new Color(0.15f, 0.68f, 0.38f); }
            else if (hustle.name.Contains("Dog")) { iconStr = "🐕"; iconBg = new Color(0.9f, 0.49f, 0.13f); }
            else if (hustle.name.Contains("Food")) { iconStr = "🍔"; iconBg = new Color(0.95f, 0.61f, 0.07f); }
            else if (hustle.name.Contains("Transit")) { iconStr = "🚇"; iconBg = new Color(0.61f, 0.35f, 0.71f); }
            else if (hustle.name.Contains("Briefcase")) { iconStr = "💼"; iconBg = new Color(0.75f, 0.22f, 0.17f); }
            else if (hustle.name.Contains("ATM") || hustle.name.Contains("Data")) { iconStr = "💳"; iconBg = new Color(0.9f, 0.3f, 0.24f); }

            VisualElement card = new VisualElement();
            card.AddToClassList("apple-squircle");
            if (isIllegal) 
            {
                card.style.borderLeftColor = new StyleColor(new Color(0.9f, 0.3f, 0.24f));
                card.style.borderLeftWidth = 4;
            }

            // Top Row (Icon & Reward Pill)
            VisualElement topRow = new VisualElement();
            topRow.style.flexDirection = FlexDirection.Row;
            topRow.style.justifyContent = Justify.SpaceBetween;
            topRow.style.alignItems = Align.Center;
            topRow.style.marginBottom = 15;

            VisualElement iconBadge = new VisualElement();
            iconBadge.AddToClassList("apple-icon-badge");
            iconBadge.style.backgroundColor = new StyleColor(iconBg);
            Label iconLbl = new Label(iconStr);
            iconLbl.style.fontSize = 36; iconLbl.style.color = Color.white;
            iconBadge.Add(iconLbl);
            topRow.Add(iconBadge);

            VisualElement rewardPill = new VisualElement();
            rewardPill.AddToClassList("apple-pill-green");
            Label rewardLbl = new Label($"+${hustle.reward}");
            rewardLbl.style.fontSize = 26; rewardLbl.style.color = new Color(0.18f, 0.8f, 0.44f); rewardLbl.style.unityFontStyleAndWeight = FontStyle.Bold;
            rewardPill.Add(rewardLbl);
            topRow.Add(rewardPill);

            card.Add(topRow);

            // Text content
            Label titleLabel = new Label(hustle.name);
            titleLabel.style.fontSize = 32; titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold; titleLabel.style.whiteSpace = WhiteSpace.Normal; 
            titleLabel.style.color = Color.white;
            titleLabel.style.marginBottom = 8;
            card.Add(titleLabel);

            Label descLabel = new Label(hustle.description);
            descLabel.style.fontSize = 22; descLabel.style.color = new Color(0.6f, 0.65f, 0.7f); descLabel.style.whiteSpace = WhiteSpace.Normal; 
            descLabel.style.marginBottom = 20;
            card.Add(descLabel);

            // Risk Pill
            VisualElement riskPill = new VisualElement();
            riskPill.AddToClassList(isIllegal ? "apple-pill-red" : "apple-pill-green");
            riskPill.style.alignSelf = Align.FlexStart;
            riskPill.style.marginBottom = 25;
            
            string riskStr = isIllegal ? $"⚠️ {hustle.riskPercent}% Risk (Jail {hustle.jailSentence}w)" : "🛡️ 0% Risk (Safe)";
            Label riskLbl = new Label(riskStr);
            riskLbl.style.fontSize = 22; riskLbl.style.unityFontStyleAndWeight = FontStyle.Bold;
            riskLbl.style.color = isIllegal ? new Color(0.9f, 0.3f, 0.24f) : new Color(0.18f, 0.8f, 0.44f);
            riskPill.Add(riskLbl);
            card.Add(riskPill);

            // Button
            Button performBtn = new Button();
            performBtn.AddToClassList("apple-btn");
            
            bool isOnCooldown = data.hustleCooldownNames.IndexOf(hustle.name) >= 0;
            if (isOnCooldown)
            {
                int cooldownIndex = data.hustleCooldownNames.IndexOf(hustle.name);
                performBtn.text = $"Wait {data.hustleCooldownWeeks[cooldownIndex]}w";
                performBtn.style.backgroundColor = new StyleColor(new Color(0.4f, 0.4f, 0.4f));
                performBtn.SetEnabled(false);
                performBtn.style.opacity = 0.5f;
            }
            else
            {
                performBtn.text = "Perform Hustle";
                performBtn.style.backgroundColor = isIllegal ? new StyleColor(new Color(0.9f, 0.3f, 0.24f)) : new StyleColor(new Color(0.22f, 0.74f, 0.97f));
                
                var localHustle = hustle;
                performBtn.clicked += () => OnHustleClicked(localHustle);
            }
            
            card.Add(performBtn);

            hustleScrollView.Add(card);
        }
    }

    private void OnHustleClicked(SideHustleItem hustle)
    {
        var data = GameManager.Instance.currentData;
        if (data.hustlesCompletedThisWeek >= 3)
        {
            global::GameUIManager.Instance.ShowPopup(new RandomEvent { 
                title = "EXHAUSTION", 
                description = "You've reached your maximum of 3 side hustles this week. You must advance the week (+ Week) to recover your stamina." 
            });
            return; 
        }
        
        data.hustlesCompletedThisWeek++;
        data.hustleCooldownNames.Add(hustle.name);
        data.hustleCooldownWeeks.Add(2);
        
        GameManager.Instance.SaveGame();
        PopulateHustlesBoard(); // Refresh UI to show updated counts/cooldowns

        // Launch Minigame Instead of Instant Roll
        ActiveMiniGame gameConfig = new ActiveMiniGame(
            category: "side_hustle",
            name: hustle.name,
            reward: (float)hustle.reward
        );

        MiniGameManager.Instance.LaunchMiniGame(gameConfig);
    }
}

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
        hustleScrollView.contentContainer.style.justifyContent = Justify.SpaceBetween;
        hustleScrollView.contentContainer.style.paddingLeft = 20;
        hustleScrollView.contentContainer.style.paddingRight = 20;
        hustleScrollView.contentContainer.style.paddingTop = 40;
        hustleScrollView.contentContainer.style.paddingBottom = 80;
        
        // Gradient from daytime sky to murky depths
                hustleScrollView.style.backgroundColor = new StyleColor(new Color(0.08f, 0.08f, 0.12f));
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
            
            // Context icon
            string iconStr = "📝";
            Color iconBg = new Color(0.4f, 0.4f, 0.4f);
            if (hustle.name.Contains("Bottle")) { iconStr = "♻️"; iconBg = new Color(0.2f, 0.7f, 0.3f); }
            else if (hustle.name.Contains("Lawn")) { iconStr = "⛳"; iconBg = new Color(0.3f, 0.8f, 0.4f); }
            else if (hustle.name.Contains("Dog")) { iconStr = "🐕"; iconBg = new Color(0.9f, 0.5f, 0.2f); }
            else if (hustle.name.Contains("Food")) { iconStr = "🍔"; iconBg = new Color(0.9f, 0.7f, 0.1f); }
            else if (hustle.name.Contains("Transit")) { iconStr = "🚇"; iconBg = new Color(0.6f, 0.2f, 0.8f); }
            else if (hustle.name.Contains("Briefcase")) { iconStr = "💼"; iconBg = new Color(0.8f, 0.1f, 0.1f); }
            else if (hustle.name.Contains("ATM") || hustle.name.Contains("Data")) { iconStr = "💳"; iconBg = new Color(0.9f, 0.2f, 0.2f); }

            VisualElement card = new VisualElement();
            card.AddToClassList("mission-card");
            card.AddToClassList(isIllegal ? "mission-card-illegal" : "mission-card-safe");
            
            // 2 per line width
            card.style.width = Length.Percent(48);

            // Top Row (Icon & Title)
            VisualElement topRow = new VisualElement();
            topRow.style.flexDirection = FlexDirection.Row;
            topRow.style.alignItems = Align.Center;
            topRow.style.marginBottom = 10;

            VisualElement iconBadge = new VisualElement();
            iconBadge.AddToClassList("mission-icon-badge");
            iconBadge.style.backgroundColor = new StyleColor(iconBg);
            Label iconLbl = new Label(iconStr);
            iconLbl.style.fontSize = 24; iconLbl.style.color = Color.white;
            iconBadge.Add(iconLbl);
            topRow.Add(iconBadge);

            Label titleLabel = new Label(hustle.name);
            titleLabel.style.fontSize = 22; 
            titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold; 
            titleLabel.style.whiteSpace = WhiteSpace.Normal; 
            titleLabel.style.flexShrink = 1;
            titleLabel.style.marginLeft = 10;
            titleLabel.style.color = isIllegal ? new Color(1f, 0.4f, 0.4f) : new Color(0.1f, 0.1f, 0.1f);
            topRow.Add(titleLabel);

            card.Add(topRow);

            // Description
            Label descLabel = new Label(hustle.description);
            descLabel.style.fontSize = 16; 
            descLabel.style.color = isIllegal ? new Color(0.7f, 0.7f, 0.7f) : new Color(0.4f, 0.4f, 0.4f); 
            descLabel.style.whiteSpace = WhiteSpace.Normal; 
            descLabel.style.marginBottom = 15;
            card.Add(descLabel);

            // Stats Row
            VisualElement statsRow = new VisualElement();
            statsRow.style.flexDirection = FlexDirection.Row;
            statsRow.style.justifyContent = Justify.SpaceBetween;
            statsRow.style.marginBottom = 15;

            Label rewardLbl = new Label($"+${hustle.reward}");
            rewardLbl.style.fontSize = 20; 
            rewardLbl.style.color = new Color(0.2f, 0.7f, 0.3f); 
            rewardLbl.style.unityFontStyleAndWeight = FontStyle.Bold;
            statsRow.Add(rewardLbl);

            string riskStr = isIllegal ? $"⚠️ {hustle.riskPercent}% (Jail {hustle.jailSentence}w)" : "🛡️ 0% Risk";
            Label riskLbl = new Label(riskStr);
            riskLbl.style.fontSize = 16; 
            riskLbl.style.unityFontStyleAndWeight = FontStyle.Bold;
            riskLbl.style.color = isIllegal ? new Color(0.9f, 0.3f, 0.3f) : new Color(0.5f, 0.5f, 0.5f);
            riskLbl.style.alignSelf = Align.FlexEnd;
            statsRow.Add(riskLbl);
            
            card.Add(statsRow);

            // Button
            Button performBtn = new Button();
            performBtn.AddToClassList("mission-btn");
            
            bool isOnCooldown = data.hustleCooldownNames.IndexOf(hustle.name) >= 0;
            if (isOnCooldown)
            {
                int cooldownIndex = data.hustleCooldownNames.IndexOf(hustle.name);
                performBtn.text = $"Wait {data.hustleCooldownWeeks[cooldownIndex]}w";
                performBtn.style.backgroundColor = new StyleColor(new Color(0.4f, 0.4f, 0.4f));
                performBtn.SetEnabled(false);
            }
            else
            {
                performBtn.text = "Start Mission";
                performBtn.style.backgroundColor = isIllegal ? new StyleColor(new Color(0.8f, 0.1f, 0.1f)) : new StyleColor(new Color(0.2f, 0.6f, 0.9f));
                
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

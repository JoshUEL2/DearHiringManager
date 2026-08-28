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
            
            VisualElement card = new VisualElement();
            card.AddToClassList("task-card-stunning");
            card.style.flexDirection = FlexDirection.Row;
            card.style.alignItems = Align.Center;
            card.style.justifyContent = Justify.SpaceBetween;
            card.style.paddingTop = 15; card.style.paddingBottom = 15;

            if (isIllegal) 
            {
                card.AddToClassList("hustle-illegal");
                card.style.borderLeftColor = new StyleColor(new Color(0.9f, 0.3f, 0.24f));
            }

            // Left Column (Text & Stats)
            VisualElement leftCol = new VisualElement();
            leftCol.style.flexDirection = FlexDirection.Column;
            leftCol.style.flexGrow = 1;
            leftCol.style.flexShrink = 1;
            leftCol.style.marginRight = 20;

            Label titleLabel = new Label(hustle.name);
            titleLabel.style.fontSize = 32; titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold; titleLabel.style.whiteSpace = WhiteSpace.Normal; 
            titleLabel.style.color = isIllegal ? new Color(0.9f, 0.3f, 0.24f) : new Color(0.22f, 0.74f, 0.97f);
            leftCol.Add(titleLabel);

            Label descLabel = new Label(hustle.description);
            descLabel.style.fontSize = 22; descLabel.style.color = new Color(0.8f, 0.8f, 0.8f); descLabel.style.whiteSpace = WhiteSpace.Normal; 
            descLabel.style.marginTop = 5; descLabel.style.marginBottom = 10;
            leftCol.Add(descLabel);

            VisualElement statsRow = new VisualElement();
            statsRow.style.flexDirection = FlexDirection.Row;
            statsRow.style.alignItems = Align.Center;

            Label rewardLabel = new Label($"+${hustle.reward} cash");
            rewardLabel.style.fontSize = 24; rewardLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            rewardLabel.style.color = isIllegal ? Color.white : new Color(0.18f, 0.8f, 0.44f);
            rewardLabel.style.marginRight = 20;
            statsRow.Add(rewardLabel);

            string riskStr = isIllegal ? $"Risk: {hustle.riskPercent}% (Jail {hustle.jailSentence} wks)" : $"Risk: {hustle.riskPercent}% (Safe)";
            Label riskLabel = new Label(riskStr);
            riskLabel.style.fontSize = 24; riskLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            riskLabel.style.color = isIllegal ? new Color(1f, 0.7f, 0.7f) : Color.gray;
            statsRow.Add(riskLabel);

            leftCol.Add(statsRow);
            card.Add(leftCol);

            // Right Column (Button)
            Button performBtn = new Button();
            bool isOnCooldown = data.hustleCooldownNames.IndexOf(hustle.name) >= 0;

            if (isOnCooldown)
            {
                int cooldownIndex = data.hustleCooldownNames.IndexOf(hustle.name);
                performBtn.text = $"Wait {data.hustleCooldownWeeks[cooldownIndex]}w";
                performBtn.AddToClassList("btn-task-stunning"); 
                performBtn.style.backgroundColor = new StyleColor(new Color(0.9f, 0.3f, 0.24f));
                performBtn.SetEnabled(false);
                performBtn.style.opacity = 0.5f;
            }
            else
            {
                performBtn.text = "Perform";
                performBtn.AddToClassList("btn-task-stunning"); 
                if (isIllegal) performBtn.style.backgroundColor = new StyleColor(new Color(0.9f, 0.3f, 0.24f));
                
                var localHustle = hustle;
                performBtn.clicked += () => OnHustleClicked(localHustle);
            }
            
            // Adjust button margins to fit perfectly on the right
            performBtn.style.marginLeft = 0; 
            performBtn.style.marginRight = 0;
            performBtn.style.paddingLeft = 20;
            performBtn.style.paddingRight = 20;

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

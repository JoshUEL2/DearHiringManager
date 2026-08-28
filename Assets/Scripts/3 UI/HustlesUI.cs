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
        
        VisualElement limitCard = new VisualElement();
        limitCard.style.backgroundColor = new StyleColor(new Color(0.12f, 0.16f, 0.23f, 0.95f));
        limitCard.style.paddingTop = 15; limitCard.style.paddingBottom = 15; limitCard.style.paddingLeft = 20; limitCard.style.paddingRight = 20;
        limitCard.style.borderRadius = 12;
        limitCard.style.marginBottom = 25;
        limitCard.style.flexDirection = FlexDirection.Row;
        limitCard.style.justifyContent = Justify.SpaceBetween;
        limitCard.style.alignItems = Align.Center;

        Label limitLbl = new Label("WEEKLY ENERGY LIMIT");
        limitLbl.style.fontSize = 28;
        limitLbl.style.color = new StyleColor(new Color(1f, 1f, 1f));
        limitLbl.style.unityFontStyleAndWeight = FontStyle.Bold;

        Label limitVal = new Label($"{hustlesLeft} REMAINING");
        limitVal.style.fontSize = 28;
        limitVal.style.color = hustlesLeft > 0 ? new Color(0.18f, 0.8f, 0.44f) : Color.red;
        limitVal.style.unityFontStyleAndWeight = FontStyle.Bold;

        limitCard.Add(limitLbl);
        limitCard.Add(limitVal);
        hustleScrollView.Add(limitCard);

        foreach (var hustle in availableHustles)
        {
            bool isIllegal = hustle.riskPercent > 0;
            VisualElement card = new VisualElement();
            card.AddToClassList("task-card-stunning");
            card.style.flexDirection = FlexDirection.Column; card.style.alignItems = Align.Stretch;
            if (isIllegal) card.style.borderLeftColor = new StyleColor(new Color(0.9f, 0.3f, 0.24f));

            VisualElement row = new VisualElement();
            row.AddToClassList("row");

            Label titleLabel = new Label(hustle.name);
            titleLabel.style.fontSize = 32; titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold; titleLabel.style.whiteSpace = WhiteSpace.Normal; titleLabel.style.color = isIllegal ? new Color(0.9f, 0.3f, 0.24f) : new Color(0.22f, 0.74f, 0.97f);
            
            
            row.Add(titleLabel);

            Label descLabel = new Label(hustle.description);
            descLabel.style.fontSize = 22; descLabel.style.color = new Color(0.8f, 0.8f, 0.8f); descLabel.style.whiteSpace = WhiteSpace.Normal; descLabel.style.marginTop = 10; descLabel.style.marginBottom = 15;

            VisualElement bottomRow = new VisualElement();
            bottomRow.AddToClassList("row");
            bottomRow.style.alignItems = Align.Center;

            VisualElement statsCol = new VisualElement();
            statsCol.style.flexDirection = FlexDirection.Column;

            Label rewardLabel = new Label($"+${hustle.reward} cash");
            rewardLabel.style.fontSize = 28;
            rewardLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            rewardLabel.style.color = isIllegal ? Color.white : new Color(0.18f, 0.8f, 0.44f); // Green for safe

            string riskStr = isIllegal ? $"Risk: {hustle.riskPercent}% (Jail {hustle.jailSentence} wks)" : $"Risk: {hustle.riskPercent}% (Safe)";
            Label riskLabel = new Label(riskStr);
            riskLabel.style.fontSize = 28;
            riskLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            riskLabel.style.color = isIllegal ? new Color(1f, 0.7f, 0.7f) : Color.gray;

            statsCol.Add(rewardLabel);
            statsCol.Add(riskLabel);

            Button performBtn = new Button();
            
            bool isOnCooldown = false;
            int cooldownIndex = data.hustleCooldownNames.IndexOf(hustle.name);
            if (cooldownIndex >= 0) isOnCooldown = true;

            if (isOnCooldown)
            {
                performBtn.text = $"Cooldown: {data.hustleCooldownWeeks[cooldownIndex]} wks";
                performBtn.AddToClassList("btn-task-stunning"); performBtn.style.backgroundColor = new StyleColor(new Color(0.9f, 0.3f, 0.24f));
                performBtn.SetEnabled(false);
                performBtn.style.opacity = 0.5f;
            }
            else
            {
                performBtn.text = "Perform";
                performBtn.AddToClassList("btn-task-stunning"); if (isIllegal) performBtn.style.backgroundColor = new StyleColor(new Color(0.9f, 0.3f, 0.24f));
                
                var localHustle = hustle;
                performBtn.clicked += () => OnHustleClicked(localHustle);
            }

            bottomRow.Add(statsCol);
            bottomRow.Add(performBtn);

            card.Add(row);
            card.Add(descLabel);
            card.Add(bottomRow);

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

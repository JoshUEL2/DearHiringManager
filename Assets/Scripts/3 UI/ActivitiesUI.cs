using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class ActivitiesUI
{
    public static ActivitiesUI Instance;

    private UIDocument activitiesDocument;
    private VisualElement wellnessContainer;
    private VisualElement mixersContainer;
    private ScrollView wellnessScrollView;
    private ScrollView mixersScrollView;

    public void Initialize(UIDocument doc)
    {
        activitiesDocument = doc;
        if (activitiesDocument == null) return;
        
        var root = activitiesDocument.rootVisualElement;

        wellnessContainer = root.Q<VisualElement>("WellnessContainer");
        mixersContainer = root.Q<VisualElement>("MixersContainer");
        
        wellnessScrollView = root.Q<ScrollView>("WellnessScrollView");
        mixersScrollView = root.Q<ScrollView>("MixersScrollView");

        root.Q<Button>("BtnTabStress")?.RegisterCallback<ClickEvent>(ev => {
            wellnessContainer.style.display = DisplayStyle.Flex;
            mixersContainer.style.display = DisplayStyle.None;
        });

        root.Q<Button>("BtnTabMixers")?.RegisterCallback<ClickEvent>(ev => {
            wellnessContainer.style.display = DisplayStyle.None;
            mixersContainer.style.display = DisplayStyle.Flex;
        });

        PopulateWellness();
        PopulateMixers();
    }

    private void PopulateWellness()
    {
        if (wellnessScrollView == null) return;
        wellnessScrollView.Clear();

        VisualElement gridWrapper = new VisualElement();
        gridWrapper.style.flexDirection = FlexDirection.Row;
        gridWrapper.style.flexWrap = Wrap.Wrap;
        gridWrapper.style.justifyContent = Justify.SpaceBetween;
        gridWrapper.style.width = Length.Percent(100);
        wellnessScrollView.Add(gridWrapper);

        AddActivityCard(gridWrapper, "Walk in the Park", "A peaceful stroll to clear your mind.", 10.0, 12f, true, () => DoWellness(10.0, 0.12f, "Walk in the Park"));
        AddActivityCard(gridWrapper, "Vintage Retro Arcade", "Immerse yourself in vintage 8-bit classics.", 35.0, 25f, true, () => DoWellness(35.0, 0.25f, "Vintage Retro Arcade"));
        AddActivityCard(gridWrapper, "Volcanic Hot Stone Spa", "Melt away corporate stress with magma stones.", 90.0, 45f, true, () => DoWellness(90.0, 0.45f, "Volcanic Hot Stone Spa"));
        AddActivityCard(gridWrapper, "Suburban Detox Mud Spa", "Total isolation and organic rejuvenation.", 250.0, 70f, true, () => DoWellness(250.0, 0.70f, "Suburban Detox Mud Spa"));
        AddActivityCard(gridWrapper, "Luxury Ice Bath Therapy", "Instantly freeze your worries in freezing cold tubs.", 1200.0, 100f, true, () => DoWellness(1200.0, 1.0f, "Luxury Ice Bath Therapy"));
    }

    private void PopulateMixers()
    {
        if (mixersScrollView == null) return;
        mixersScrollView.Clear();

        VisualElement gridWrapper = new VisualElement();
        gridWrapper.style.flexDirection = FlexDirection.Row;
        gridWrapper.style.flexWrap = Wrap.Wrap;
        gridWrapper.style.justifyContent = Justify.SpaceBetween;
        gridWrapper.style.width = Length.Percent(100);
        mixersScrollView.Add(gridWrapper);

        AddActivityCard(gridWrapper, "Industrial Dive Bar", "Meet fellow working-class citizens and share rumors.", 40.0, 15f, false, () => DoNetworking(40.0, "manual", 0.15f));
        AddActivityCard(gridWrapper, "Hospitality Gala", "Mingle with service managers and event coordinators.", 120.0, 15f, false, () => DoNetworking(120.0, "hospitality", 0.15f));
        AddActivityCard(gridWrapper, "Startup Pitch Meetup", "Network with hungry founders and angel investors.", 300.0, 15f, false, () => DoNetworking(300.0, "tech", 0.15f));
        AddActivityCard(gridWrapper, "Corporate Golf Outing", "Rub elbows with executives in virtual country clubs.", 1000.0, 20f, false, () => DoNetworking(1000.0, "corp", 0.20f));
    }

    private void AddActivityCard(VisualElement container, string name, string desc, double cost, float statChange, bool isWellness, System.Action onClick)
    {
        VisualElement card = new VisualElement();
        card.AddToClassList("squircle-card");
        card.style.width = Length.Percent(100);
        card.style.marginBottom = 20;
        card.style.overflow = Overflow.Hidden; 
        card.style.minHeight = 260; 

        VisualElement animBg = GenerateBackground(name);
        card.Add(animBg);

        VisualElement fadeOverlay = new VisualElement();
        fadeOverlay.style.position = Position.Absolute; fadeOverlay.style.top = 0; fadeOverlay.style.bottom = 0; fadeOverlay.style.left = 0; fadeOverlay.style.right = 0;
        fadeOverlay.pickingMode = PickingMode.Ignore;
        for(int i=0; i<10; i++) {
            VisualElement strip = new VisualElement();
            strip.style.position = Position.Absolute; strip.style.top = 0; strip.style.bottom = 0;
            strip.style.left = Length.Percent(i * 10); strip.style.width = Length.Percent(10.5f);
            float alpha = i < 4 ? 0.95f : 0.95f - ((i - 4) / 6f) * 0.95f;
            strip.style.backgroundColor = new StyleColor(new Color(0.04f, 0.07f, 0.16f, alpha));
            fadeOverlay.Add(strip);
        }
        card.Add(fadeOverlay);

        VisualElement contentWrapper = new VisualElement();
        contentWrapper.style.width = Length.Percent(60);
        contentWrapper.style.paddingLeft = 30; contentWrapper.style.paddingRight = 30;
        contentWrapper.style.paddingTop = 30; contentWrapper.style.paddingBottom = 30;
        contentWrapper.style.flexGrow = 1;
        contentWrapper.style.justifyContent = Justify.SpaceBetween;
        contentWrapper.pickingMode = PickingMode.Ignore;

        VisualElement topRow = new VisualElement();
        topRow.pickingMode = PickingMode.Ignore;
        Label titleLabel = new Label(name);
        titleLabel.style.fontSize = 34;
        titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        titleLabel.style.color = new Color(1f, 1f, 1f);
        titleLabel.style.whiteSpace = WhiteSpace.Normal;
        topRow.Add(titleLabel);

        Label descLabel = new Label(desc);
        descLabel.style.fontSize = 24; 
        descLabel.style.color = new Color(0.9f, 0.9f, 0.9f);
        descLabel.style.whiteSpace = WhiteSpace.Normal;
        descLabel.style.marginTop = 15;
        topRow.Add(descLabel);

        VisualElement bottomRow = new VisualElement();
        bottomRow.style.flexDirection = FlexDirection.Row;
        bottomRow.style.justifyContent = Justify.FlexStart;
        bottomRow.style.alignItems = Align.FlexEnd;
        bottomRow.style.marginTop = 25;
        bottomRow.pickingMode = PickingMode.Ignore;

        VisualElement statsCol = new VisualElement();
        statsCol.style.flexDirection = FlexDirection.Column;
        statsCol.style.marginRight = 30;
        statsCol.pickingMode = PickingMode.Ignore;

        string costStr = $"COST: -${cost}";
        Label costLabel = new Label(costStr);
        costLabel.style.fontSize = 36;
        costLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        costLabel.style.color = new Color(0.93f, 0.26f, 0.26f);
        costLabel.style.marginBottom = 10;
        statsCol.Add(costLabel);

        string statStr = isWellness ? $"Benefit: -{statChange}% Stress" : $"Benefit: +{statChange}% Luck";
        Label statLabel = new Label(statStr);
        statLabel.style.fontSize = 22; 
        statLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        statLabel.style.color = new Color(0.18f, 0.8f, 0.44f);
        statsCol.Add(statLabel);

        bottomRow.Add(statsCol);
        
        Button performBtn = new Button();
        performBtn.text = "ATTEND";
        performBtn.style.backgroundColor = new StyleColor(new Color(0.22f, 0.74f, 0.97f));
        performBtn.style.color = new StyleColor(Color.white);
        performBtn.style.fontSize = 24;
        performBtn.style.unityFontStyleAndWeight = FontStyle.Bold;
        performBtn.style.paddingLeft = 20; performBtn.style.paddingRight = 20;
        performBtn.style.paddingTop = 15; performBtn.style.paddingBottom = 15;
        performBtn.style.borderTopLeftRadius = 12; performBtn.style.borderTopRightRadius = 12;
        performBtn.style.borderBottomLeftRadius = 12; performBtn.style.borderBottomRightRadius = 12;
        performBtn.clicked += onClick;
        
        bottomRow.Add(performBtn);
        contentWrapper.Add(topRow);
        contentWrapper.Add(bottomRow);
        card.Add(contentWrapper);

        container.Add(card);
    }

    private VisualElement GenerateBackground(string name)
    {
        VisualElement bg = new VisualElement();
        bg.style.position = Position.Absolute;
        bg.style.top = 0; bg.style.bottom = 0; bg.style.left = 0; bg.style.right = 0;
        bg.style.overflow = Overflow.Hidden;

        switch (name)
        {
            case "Walk in the Park":
                bg.style.backgroundColor = new StyleColor(new Color(0.1f, 0.3f, 0.1f));
                var trees = new List<VisualElement>();
                for(int i=0; i<5; i++) {
                    VisualElement tree = new VisualElement();
                    tree.userData = (float)(70 + (i*6));
                    tree.style.position = Position.Absolute;
                    tree.style.bottom = -10; tree.style.left = Length.Percent((float)tree.userData);
                    tree.style.width = 15; tree.style.height = Random.Range(60, 100);
                    tree.style.backgroundColor = new StyleColor(new Color(0.15f, 0.4f, 0.15f));
                    tree.style.borderTopLeftRadius = 8; tree.style.borderTopRightRadius = 8;
                    bg.Add(tree);
                    trees.Add(tree);
                }
                bg.schedule.Execute(() => {
                    foreach(var t in trees) {
                        float v = (float)t.userData;
                        v -= 0.1f;
                        if(v < 40) v = 100;
                        t.userData = v;
                        t.style.left = Length.Percent(v);
                    }
                }).Every(16);
                break;
            case "Vintage Retro Arcade":
                bg.style.backgroundColor = new StyleColor(new Color(0.05f, 0.05f, 0.15f));
                var lines = new List<VisualElement>();
                for(int i=0; i<8; i++) {
                    VisualElement line = new VisualElement();
                    line.userData = (float)(60 + (i*5));
                    line.style.position = Position.Absolute;
                    line.style.top = 0; line.style.bottom = 0;
                    line.style.left = Length.Percent((float)line.userData);
                    line.style.width = 2;
                    line.style.backgroundColor = new StyleColor(new Color(0f, 1f, 1f, 0.3f));
                    bg.Add(line);
                    lines.Add(line);
                }
                VisualElement horizon = new VisualElement();
                horizon.style.position = Position.Absolute; horizon.style.top = Length.Percent(50); horizon.style.width = Length.Percent(100); horizon.style.height = 2; horizon.style.backgroundColor = new StyleColor(new Color(1f, 0f, 1f, 0.5f));
                bg.Add(horizon);
                bg.schedule.Execute(() => {
                    foreach(var l in lines) {
                        float v = (float)l.userData;
                        v -= 0.5f;
                        if(v < 50) v = 100;
                        l.userData = v;
                        l.style.left = Length.Percent(v);
                    }
                }).Every(16);
                break;
            case "Volcanic Hot Stone Spa":
                bg.style.backgroundColor = new StyleColor(new Color(0.3f, 0.1f, 0.05f));
                var rocks = new List<VisualElement>();
                for(int i=0; i<12; i++) {
                    VisualElement rock = new VisualElement();
                    rock.userData = (float)Random.Range(-10, 40);
                    rock.style.position = Position.Absolute;
                    rock.style.bottom = (float)rock.userData; rock.style.left = Length.Percent(65 + (i*3));
                    float s = Random.Range(20, 50);
                    rock.style.width = s; rock.style.height = s;
                    rock.style.backgroundColor = new StyleColor(new Color(0.15f, 0.05f, 0.02f));
                    rock.style.borderTopLeftRadius = s/2; rock.style.borderTopRightRadius = s/2; rock.style.borderBottomLeftRadius = s/2; rock.style.borderBottomRightRadius = s/2;
                    bg.Add(rock);
                    rocks.Add(rock);
                }
                bg.schedule.Execute(() => {
                    foreach(var r in rocks) {
                        float v = (float)r.userData;
                        v += 0.2f;
                        if(v > 100) v = -20;
                        r.userData = v;
                        r.style.bottom = v;
                    }
                }).Every(16);
                break;
            case "Suburban Detox Mud Spa":
                bg.style.backgroundColor = new StyleColor(new Color(0.2f, 0.15f, 0.1f));
                VisualElement mud = new VisualElement();
                mud.style.position = Position.Absolute; mud.style.bottom = 0; mud.style.width = Length.Percent(100); mud.style.height = Length.Percent(40);
                mud.style.backgroundColor = new StyleColor(new Color(0.15f, 0.1f, 0.05f));
                bg.Add(mud);
                break;
            case "Luxury Ice Bath Therapy":
                bg.style.backgroundColor = new StyleColor(new Color(0.85f, 0.95f, 1f));
                var flakes = new List<VisualElement>();
                for(int i=0; i<20; i++) {
                    VisualElement flake = new VisualElement();
                    flake.userData = (float)Random.Range(0, 200);
                    flake.style.position = Position.Absolute;
                    flake.style.top = (float)flake.userData; flake.style.left = Length.Percent(Random.Range(60, 100));
                    flake.style.width = 4; flake.style.height = 4;
                    flake.style.backgroundColor = new StyleColor(Color.white);
                    bg.Add(flake);
                    flakes.Add(flake);
                }
                bg.schedule.Execute(() => {
                    foreach(var f in flakes) {
                        float v = (float)f.userData;
                        v += 1f;
                        if(v > 300) v = -10;
                        f.userData = v;
                        f.style.top = v;
                    }
                }).Every(16);
                break;
            case "Industrial Dive Bar":
                bg.style.backgroundColor = new StyleColor(new Color(0.1f, 0.1f, 0.15f));
                VisualElement sign = new VisualElement();
                sign.style.position = Position.Absolute; sign.style.top = 20; sign.style.left = Length.Percent(75);
                sign.style.width = 60; sign.style.height = 30; sign.style.backgroundColor = new StyleColor(new Color(0.8f, 0.8f, 0.2f, 0.5f));
                bg.Add(sign);
                float alpha = 0.5f;
                bg.schedule.Execute(() => {
                    alpha = alpha > 0.3f ? 0.2f : 0.8f;
                    sign.style.backgroundColor = new StyleColor(new Color(0.8f, 0.8f, 0.2f, alpha));
                }).Every(500);
                break;
            case "Hospitality Gala":
                bg.style.backgroundColor = new StyleColor(new Color(0.2f, 0.05f, 0.05f));
                VisualElement carpet = new VisualElement();
                carpet.style.position = Position.Absolute; carpet.style.bottom = 0; carpet.style.width = Length.Percent(100); carpet.style.height = 40;
                carpet.style.backgroundColor = new StyleColor(new Color(0.6f, 0.1f, 0.1f));
                bg.Add(carpet);
                break;
            case "Startup Pitch Meetup":
                bg.style.backgroundColor = new StyleColor(new Color(0.05f, 0.15f, 0.25f));
                var bars = new List<VisualElement>();
                for(int i=0; i<4; i++) {
                    VisualElement bar = new VisualElement();
                    bar.style.position = Position.Absolute; bar.style.bottom = 20; bar.style.left = Length.Percent(70 + (i*6));
                    bar.style.width = 20; bar.style.height = 30 + (i*30);
                    bar.style.backgroundColor = new StyleColor(new Color(0.2f, 0.8f, 0.4f));
                    bg.Add(bar);
                    bars.Add(bar);
                }
                bg.schedule.Execute(() => {
                    foreach(var b in bars) {
                        b.style.height = Random.Range(20, 100);
                    }
                }).Every(200);
                break;
            case "Corporate Golf Outing":
                bg.style.backgroundColor = new StyleColor(new Color(0.05f, 0.25f, 0.1f));
                VisualElement hole = new VisualElement();
                hole.style.position = Position.Absolute; hole.style.bottom = 40; hole.style.left = Length.Percent(85);
                hole.style.width = 20; hole.style.height = 10; hole.style.backgroundColor = new StyleColor(Color.black);
                hole.style.borderTopLeftRadius = 10; hole.style.borderTopRightRadius = 10; hole.style.borderBottomLeftRadius = 10; hole.style.borderBottomRightRadius = 10;
                bg.Add(hole);
                VisualElement flag = new VisualElement();
                flag.style.position = Position.Absolute; flag.style.bottom = 45; flag.style.left = Length.Percent(85); flag.style.width = 2; flag.style.height = 50; flag.style.backgroundColor = new StyleColor(Color.white);
                bg.Add(flag);
                VisualElement ball = new VisualElement();
                ball.style.position = Position.Absolute; ball.style.bottom = 40; ball.style.left = Length.Percent(60); ball.style.width = 8; ball.style.height = 8; ball.style.backgroundColor = new StyleColor(Color.white);
                ball.style.borderTopLeftRadius = 4; ball.style.borderTopRightRadius = 4; ball.style.borderBottomLeftRadius = 4; ball.style.borderBottomRightRadius = 4;
                ball.userData = 60f;
                bg.Add(ball);
                bg.schedule.Execute(() => {
                    float v = (float)ball.userData;
                    v += 0.5f;
                    if(v > 86) v = 60;
                    ball.userData = v;
                    ball.style.left = Length.Percent(v);
                }).Every(16);
                break;
        }

        return bg;
    }

    private void DoWellness(double cost, float stressReduction, string actName)
    {
        var data = GameManager.Instance.currentData;
        if (data.balance >= cost)
        {
            if (actName == "Walk in the Park")
            {
                AudioManager.Instance?.PlayParkAmbience();
            }
            ActiveMiniGame game = new ActiveMiniGame("wellness", actName, stressReduction, 0f, cost, false);
            MiniGameManager.Instance.LaunchMiniGame(game);
        }
        else
        {
            global::GameUIManager.Instance.ShowPopup(new RandomEvent { 
                title = "INSUFFICIENT FUNDS", 
                description = $"You need <color=#E74C3C>${cost}</color> to purchase {actName}." 
            });
        }
    }

    private void DoNetworking(double cost, string type, float luckBoost)
    {
        var data = GameManager.Instance.currentData;
        if (data.balance >= cost)
        {
            string actName = "";
            if (type == "manual") actName = "Industrial Dive Bar";
            if (type == "hospitality") actName = "Hospitality Gala";
            if (type == "tech") actName = "Startup Pitch Meetup";
            if (type == "corp") actName = "Corporate Golf Outing";

            ActiveMiniGame game = new ActiveMiniGame("mixer", actName, luckBoost, 0f, cost, false);
            MiniGameManager.Instance.LaunchMiniGame(game);
        }
        else
        {
            global::GameUIManager.Instance?.ShowPopup(new RandomEvent { 
                title = "INSUFFICIENT FUNDS", 
                description = $"You need <color=#E74C3C>${cost}</color> to attend this networking mixer." 
            });
        }
    }
}

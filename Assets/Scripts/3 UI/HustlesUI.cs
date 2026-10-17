using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class HustlesUI
{
    public static HustlesUI Instance;

    public UIDocument hustlesDocument;
    private ScrollView hustleScrollView;

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
        
        Label limitLbl = new Label($"Hustles Available This Week: {hustlesLeft} / 3");
        limitLbl.style.fontSize = 28;
        limitLbl.style.unityFontStyleAndWeight = FontStyle.Bold;
        limitLbl.style.color = hustlesLeft > 0 ? new Color(0.18f, 0.8f, 0.44f) : Color.red;
        limitLbl.style.marginBottom = 25;
        limitLbl.style.alignSelf = Align.Center;
        hustleScrollView.Add(limitLbl);

        VisualElement gridWrapper = new VisualElement();
        gridWrapper.style.flexDirection = FlexDirection.Row;
        gridWrapper.style.flexWrap = Wrap.Wrap;
        gridWrapper.style.justifyContent = Justify.SpaceBetween;
        gridWrapper.style.width = Length.Percent(100);
        gridWrapper.style.paddingBottom = 40; 
        hustleScrollView.Add(gridWrapper);

        foreach (var hustle in availableHustles)
        {
            bool isIllegal = hustle.riskPercent > 0;
            bool isOnCooldown = data.hustleCooldownNames.IndexOf(hustle.name) >= 0;
            int cooldownIndex = data.hustleCooldownNames.IndexOf(hustle.name);
            VisualElement card = new VisualElement();
            card.AddToClassList("squircle-card");
            
            // FULL WIDTH (1 per line)
            card.style.width = Length.Percent(100);
            card.style.marginBottom = 20;
            card.style.overflow = Overflow.Hidden; 
            card.style.minHeight = 260; 

            if (isIllegal)
            {
                card.style.borderTopWidth = 4; card.style.borderBottomWidth = 4;
                card.style.borderLeftWidth = 4; card.style.borderRightWidth = 4;
                card.style.borderTopColor = card.style.borderBottomColor = card.style.borderLeftColor = card.style.borderRightColor = new StyleColor(new Color(0.9f, 0.2f, 0.2f));
            }

            VisualElement animBg = GenerateMinigameBackground(hustle.name, isOnCooldown);
            card.Add(animBg);

            // Layer a gradient fade from left to right for text legibility
            VisualElement fadeOverlay = new VisualElement();
            fadeOverlay.style.position = Position.Absolute; fadeOverlay.style.top = 0; fadeOverlay.style.bottom = 0; fadeOverlay.style.left = 0; fadeOverlay.style.right = 0;
            fadeOverlay.pickingMode = PickingMode.Ignore;
            for(int i=0; i<10; i++) {
                VisualElement strip = new VisualElement();
                strip.style.position = Position.Absolute; strip.style.top = 0; strip.style.bottom = 0;
                strip.style.left = Length.Percent(i * 10); strip.style.width = Length.Percent(10.5f); // slight overlap to prevent seams
                float alpha = i < 4 ? 0.95f : 0.95f - ((i - 4) / 6f) * 0.95f;
                strip.style.backgroundColor = new StyleColor(new Color(0.04f, 0.07f, 0.16f, alpha));
                fadeOverlay.Add(strip);
            }
            card.Add(fadeOverlay);

            VisualElement contentWrapper = new VisualElement();
            // Using relative positioning so the text content pushes the card height naturally
            contentWrapper.style.width = Length.Percent(60); // Span 60% of width for text
            contentWrapper.style.paddingLeft = 30; contentWrapper.style.paddingRight = 30;
            contentWrapper.style.paddingTop = 30; contentWrapper.style.paddingBottom = 30;
            contentWrapper.style.flexGrow = 1;
            contentWrapper.style.justifyContent = Justify.SpaceBetween;
            contentWrapper.pickingMode = PickingMode.Ignore;

            VisualElement topRow = new VisualElement();
            topRow.pickingMode = PickingMode.Ignore;
            Label titleLabel = new Label(hustle.name);
            titleLabel.style.fontSize = 34;
            titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            titleLabel.style.color = isIllegal ? new Color(1f, 0.4f, 0.4f) : new Color(1f, 1f, 1f);
            titleLabel.style.whiteSpace = WhiteSpace.Normal;
            topRow.Add(titleLabel);

            Label descLabel = new Label(hustle.description);
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

            Label rewardLabel = new Label($"PAYOUT: +${hustle.reward}");
            rewardLabel.style.fontSize = 36; // Larger and more obvious
            rewardLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            rewardLabel.style.color = isIllegal ? new Color(0.93f, 0.26f, 0.26f) : new Color(0.18f, 0.8f, 0.44f);
            rewardLabel.style.marginBottom = 10;

            string riskStr = isIllegal ? $"Risk: {hustle.riskPercent}%\n(Jail {hustle.jailSentence}w)" : $"Risk: {hustle.riskPercent}%\n(Safe)";
            Label riskLabel = new Label(riskStr);
            riskLabel.style.fontSize = 22; 
            riskLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            riskLabel.style.color = isIllegal ? new Color(1f, 0.7f, 0.7f) : new Color(0.6f, 0.8f, 0.6f);
            riskLabel.style.whiteSpace = WhiteSpace.Normal;

            statsCol.Add(rewardLabel);
            statsCol.Add(riskLabel);
            bottomRow.Add(statsCol);
            
                        contentWrapper.Add(topRow);
            contentWrapper.Add(bottomRow);
            
            card.Add(contentWrapper);
            
            if (isOnCooldown)
            {
                card.style.opacity = 0.45f;
                
                VisualElement watermarkContainer = new VisualElement();
                watermarkContainer.style.position = Position.Absolute;
                watermarkContainer.style.top = 0; watermarkContainer.style.bottom = 0;
                watermarkContainer.style.left = 0; watermarkContainer.style.right = 0;
                watermarkContainer.style.justifyContent = Justify.Center;
                watermarkContainer.style.alignItems = Align.Center;
                watermarkContainer.style.backgroundColor = new StyleColor(new Color(0.9f, 0.1f, 0.1f, 0.25f));
                watermarkContainer.pickingMode = PickingMode.Ignore;
                
                Label watermarkText = new Label($"WAIT {data.hustleCooldownWeeks[cooldownIndex]} WEEKS");
                watermarkText.style.fontSize = 70;
                watermarkText.style.unityFontStyleAndWeight = FontStyle.Bold;
                watermarkText.style.color = new Color(1f, 1f, 1f, 0.95f);
                watermarkText.style.rotate = new Rotate(Angle.Degrees(-8));
                watermarkText.style.textShadow = new TextShadow { color = new Color(0f, 0f, 0f, 0.8f), blurRadius = 5f, offset = new Vector2(3, 3) };
                
                watermarkContainer.Add(watermarkText);
                card.Add(watermarkContainer);
            }
            else
            {
                var localHustle = hustle;
                card.RegisterCallback<PointerDownEvent>(e => {
                    card.style.scale = new StyleScale(new Vector2(0.96f, 0.96f));
                    card.style.opacity = 0.8f;
                });
                card.RegisterCallback<PointerUpEvent>(e => {
                    card.style.scale = new StyleScale(new Vector2(1f, 1f));
                    card.style.opacity = 1f;
                    OnHustleClicked(localHustle);
                });
                card.RegisterCallback<PointerLeaveEvent>(e => {
                    card.style.scale = new StyleScale(new Vector2(1f, 1f));
                    card.style.opacity = 1f;
                });
            }

            gridWrapper.Add(card);
        }
    }

    private VisualElement GenerateMinigameBackground(string gameName, bool isOnCooldown)
    {
        VisualElement bg = new VisualElement();
        bg.style.position = Position.Absolute;
        bg.style.top = 0; bg.style.bottom = 0;
        bg.style.left = 0; bg.style.right = 0;
        bg.pickingMode = PickingMode.Ignore;

        switch(gameName)
        {
            case "Collect Recycled Bottles":
                bg.style.backgroundColor = new StyleColor(new Color(0.08f, 0.1f, 0.14f));
                string[] emojis = { "🍾", "🥤", "🥫" };
                VisualElement[] fallingItems = new VisualElement[3];
                float[] itemY = new float[3];
                for(int i=0; i<3; i++) {
                    Label item = new Label(emojis[i]);
                    item.style.position = Position.Absolute;
                    item.style.fontSize = 45; 
                    item.style.left = Length.Percent(70 + (i * 10)); 
                    itemY[i] = -50 - (i * 90);
                    bg.Add(item);
                    fallingItems[i] = item;
                }
                if (!isOnCooldown) bg.schedule.Execute(() => {
                    if (bg.panel == null) return;
                    for(int i=0; i<3; i++) {
                        itemY[i] += 4f;
                        if(itemY[i] > 350) itemY[i] = -50;
                        fallingItems[i].style.top = itemY[i];
                        fallingItems[i].style.rotate = new Rotate(Angle.Degrees(itemY[i] * 2f));
                    }
                }).Every(16);
                break;
                
            case "Executive Lawn Care":
                bg.style.backgroundColor = new StyleColor(new Color(0.1f, 0.2f, 0.1f));
                VisualElement grid = new VisualElement();
                grid.style.position = Position.Absolute; grid.style.top = 10; grid.style.left = Length.Percent(60); grid.style.right = 10; grid.style.bottom = 10;
                grid.style.flexWrap = Wrap.Wrap; grid.style.flexDirection = FlexDirection.Row;
                VisualElement[] tiles = new VisualElement[36];
                for(int i=0; i<36; i++) {
                    VisualElement tile = new VisualElement();
                    tile.style.width = Length.Percent(15); tile.style.height = Length.Percent(15);
                    tile.style.marginRight = Length.Percent(1.5f); tile.style.marginBottom = Length.Percent(1.5f);
                    tile.style.backgroundColor = new StyleColor(new Color(0.1f, 0.4f, 0.1f));
                    grid.Add(tile);
                    tiles[i] = tile;
                }
                bg.Add(grid);
                Label tractor = new Label("🚜");
                tractor.style.position = Position.Absolute;
                tractor.style.fontSize = 35;
                bg.Add(tractor);
                int currentTile = 0;
                float tTime = 0;
                if (!isOnCooldown) bg.schedule.Execute(() => {
                    if (bg.panel == null) return;
                    tTime += 1f;
                    if(tTime > 6) {
                        tTime = 0;
                        tiles[currentTile].style.backgroundColor = new StyleColor(new Color(0.4f, 0.8f, 0.4f)); 
                        currentTile++;
                        if(currentTile >= 36) {
                            currentTile = 0;
                            foreach(var t in tiles) t.style.backgroundColor = new StyleColor(new Color(0.1f, 0.4f, 0.1f));
                        }
                        int row = currentTile / 6;
                        int col = currentTile % 6; 
                        tractor.style.top = 10 + (row * 35); 
                        tractor.style.left = Length.Percent(60 + (col * 6));
                    }
                }).Every(16);
                break;
                
            case "Walk Purebred Pedigree Dogs":
                bg.style.backgroundColor = new StyleColor(new Color(0.15f, 0.25f, 0.15f));
                VisualElement sweetSpot = new VisualElement();
                sweetSpot.style.position = Position.Absolute;
                sweetSpot.style.left = Length.Percent(80); sweetSpot.style.translate = new StyleTranslate(new Translate(Length.Percent(-50), 0, 0)); 
                sweetSpot.style.width = Length.Percent(30);
                sweetSpot.style.top = 0; sweetSpot.style.bottom = 0;
                sweetSpot.style.backgroundColor = new StyleColor(new Color(0f, 1f, 0f, 0.15f));
                bg.Add(sweetSpot);
                Label human = new Label("🧍");
                human.style.position = Position.Absolute; human.style.fontSize = 50;
                human.style.left = Length.Percent(80); human.style.top = Length.Percent(40);
                human.style.translate = new StyleTranslate(new Translate(Length.Percent(-50), Length.Percent(-50), 0));
                bg.Add(human);
                string[] dogEmojis = { "🐕", "🐩", "🦮", "🐕‍🦺", "🐕" };
                VisualElement[] leashes = new VisualElement[5];
                Label[] dogs = new Label[5];
                float[] dogDistances = new float[5];
                float[] dogAngles = new float[5];
                for(int i=0; i<5; i++) {
                    VisualElement leash = new VisualElement();
                    leash.style.position = Position.Absolute; leash.style.height = 3; leash.style.backgroundColor = new StyleColor(new Color(0.6f, 0.3f, 0.1f));
                    leash.style.transformOrigin = new TransformOrigin(new Length(0, LengthUnit.Percent), new Length(50, LengthUnit.Percent));
                    leash.style.left = Length.Percent(80); leash.style.top = Length.Percent(45);
                    bg.Add(leash);
                    leashes[i] = leash;
                    Label dog = new Label(dogEmojis[i]);
                    dog.style.position = Position.Absolute; dog.style.fontSize = 35;
                    dog.style.left = Length.Percent(80); dog.style.top = Length.Percent(45);
                    bg.Add(dog);
                    dogs[i] = dog;
                    dogDistances[i] = 40f + (i*10);
                    dogAngles[i] = i * 72f;
                }
                if (!isOnCooldown) bg.schedule.Execute(() => {
                    if (bg.panel == null) return;
                    for(int i=0; i<5; i++) {
                        dogDistances[i] += 0.3f;
                        if(dogDistances[i] > 110f) dogDistances[i] = 40f; 
                        float rad = dogAngles[i] * Mathf.Deg2Rad;
                        float x = Mathf.Cos(rad) * dogDistances[i];
                        float y = Mathf.Sin(rad) * dogDistances[i];
                        dogs[i].style.translate = new StyleTranslate(new Translate(x, y, 0));
                        leashes[i].style.width = dogDistances[i];
                        leashes[i].style.rotate = new StyleRotate(new Rotate(Angle.Degrees(dogAngles[i])));
                    }
                }).Every(16);
                break;
                
            case "Deliver Fast Food Order":
                bg.style.backgroundColor = new StyleColor(new Color(0.2f, 0.2f, 0.2f));
                VisualElement roadLeft = new VisualElement(); roadLeft.style.position = Position.Absolute; roadLeft.style.top = 0; roadLeft.style.bottom = 0; roadLeft.style.left = Length.Percent(65); roadLeft.style.width = 10; roadLeft.style.backgroundColor = new StyleColor(Color.white);
                VisualElement roadRight = new VisualElement(); roadRight.style.position = Position.Absolute; roadRight.style.top = 0; roadRight.style.bottom = 0; roadRight.style.right = Length.Percent(5); roadRight.style.width = 10; roadRight.style.backgroundColor = new StyleColor(Color.white);
                bg.Add(roadLeft); bg.Add(roadRight);
                VisualElement[] stripes = new VisualElement[4];
                for(int i=0; i<4; i++) {
                    VisualElement stripe = new VisualElement();
                    stripe.style.position = Position.Absolute; stripe.style.left = Length.Percent(80); stripe.style.width = 10; stripe.style.height = 40;
                    stripe.style.backgroundColor = new StyleColor(Color.yellow);
                    bg.Add(stripe);
                    stripes[i] = stripe;
                }
                
                Label[] obstacles = new Label[2];
                float[] obsY = new float[2];
                for(int i=0; i<2; i++) {
                    obstacles[i] = new Label("🚘");
                    obstacles[i].style.position = Position.Absolute; obstacles[i].style.fontSize = 40;
                    obsY[i] = -100 - (i*150);
                    bg.Add(obstacles[i]);
                }
                
                Label playerBike = new Label("🛵");
                playerBike.style.position = Position.Absolute; playerBike.style.bottom = 10; playerBike.style.left = Length.Percent(80); playerBike.style.fontSize = 50;
                bg.Add(playerBike);
                
                float roadY = 0;
                if (!isOnCooldown) bg.schedule.Execute(() => {
                    if (bg.panel == null) return;
                    roadY += 4f;
                    for(int i=0; i<4; i++) {
                        float curY = (i*60) + roadY;
                        if(curY > 280) curY -= 240;
                        stripes[i].style.top = curY;
                    }
                    for(int i=0; i<2; i++) {
                        obsY[i] += 2f;
                        if(obsY[i] > 350f) {
                            obsY[i] = -100f;
                            obstacles[i].style.left = Length.Percent(Random.value > 0.5f ? 70 : 88);
                        }
                        obstacles[i].style.top = obsY[i];
                    }
                    playerBike.style.left = Length.Percent(80 + Mathf.Sin(roadY * 0.05f) * 6f);
                }).Every(16);
                break;
                
            case "Forge Transit Passes":
                bg.style.backgroundColor = new StyleColor(new Color(0.2f, 0.2f, 0.25f));
                VisualElement stampZone = new VisualElement();
                stampZone.style.position = Position.Absolute; stampZone.style.left = Length.Percent(70); stampZone.style.width = Length.Percent(25); stampZone.style.top = 40; stampZone.style.bottom = 40;
                stampZone.style.borderTopWidth = 4; stampZone.style.borderBottomWidth = 4; stampZone.style.borderLeftWidth = 4; stampZone.style.borderRightWidth = 4;
                stampZone.style.borderTopColor = stampZone.style.borderBottomColor = stampZone.style.borderLeftColor = stampZone.style.borderRightColor = new StyleColor(new Color(1f, 1f, 1f, 0.2f));
                bg.Add(stampZone);
                VisualElement passCardBg = new VisualElement();
                passCardBg.style.position = Position.Absolute;
                passCardBg.style.top = 90; passCardBg.style.width = 140; passCardBg.style.height = 80;
                passCardBg.style.backgroundColor = new StyleColor(new Color(0.9f, 0.95f, 0.95f));
                passCardBg.style.borderTopLeftRadius = 15; passCardBg.style.borderTopRightRadius = 15; passCardBg.style.borderBottomLeftRadius = 15; passCardBg.style.borderBottomRightRadius = 15;
                
                VisualElement photo = new VisualElement(); photo.style.position = Position.Absolute; photo.style.left = 10; photo.style.top = 20; photo.style.width = 30; photo.style.height = 40; photo.style.backgroundColor = new StyleColor(new Color(0.7f, 0.7f, 0.8f)); passCardBg.Add(photo);
                for(int i=0; i<3; i++) {
                    VisualElement line = new VisualElement(); line.style.position = Position.Absolute; line.style.left = 50; line.style.top = 25 + (i*12); line.style.height = 4; line.style.width = 60; line.style.backgroundColor = new StyleColor(new Color(0.5f, 0.5f, 0.6f)); passCardBg.Add(line);
                }
                
                Label fStamp = new Label("APPROVED");
                fStamp.style.color = new StyleColor(new Color(0.06f, 0.73f, 0.51f, 1f));
                fStamp.style.fontSize = 22; fStamp.style.unityFontStyleAndWeight = FontStyle.Bold;
                fStamp.style.rotate = new Rotate(Angle.Degrees(-15));
                fStamp.style.position = Position.Absolute; fStamp.style.left = 10; fStamp.style.top = 20;
                fStamp.style.opacity = 0f; 
                passCardBg.Add(fStamp);
                bg.Add(passCardBg);
                
                float cardX = 40f; 
                float phase = 0;
                if (!isOnCooldown) bg.schedule.Execute(() => {
                    if (bg.panel == null) return;
                    phase += 0.015f;
                    if(phase < 1f) {
                        cardX += 0.5f;
                        fStamp.style.opacity = 0f;
                    } else if (phase < 1.5f) {
                        fStamp.style.opacity = 1f;
                    } else {
                        cardX = 40f;
                        phase = 0;
                    }
                    passCardBg.style.left = Length.Percent(cardX);
                }).Every(16);
                break;
                
            case "Crack Luxury Briefcase":
                bg.style.backgroundColor = new StyleColor(new Color(0.27f, 0.1f, 0.01f)); 
                
                VisualElement stickyTarget = new VisualElement();
                stickyTarget.style.position = Position.Absolute; stickyTarget.style.top = 10;
                stickyTarget.style.left = Length.Percent(80); stickyTarget.style.translate = new StyleTranslate(new Translate(Length.Percent(-50), 0, 0));
                stickyTarget.style.backgroundColor = new StyleColor(new Color(0.99f, 0.94f, 0.54f)); 
                stickyTarget.style.paddingTop = 5; stickyTarget.style.paddingBottom = 5;
                stickyTarget.style.paddingLeft = 10; stickyTarget.style.paddingRight = 10;
                stickyTarget.style.rotate = new Rotate(Angle.Degrees(-3));
                stickyTarget.style.borderBottomWidth = 2; stickyTarget.style.borderRightWidth = 2;
                stickyTarget.style.borderBottomColor = stickyTarget.style.borderRightColor = new StyleColor(new Color(0f, 0f, 0f, 0.3f));
                Label targetText = new Label("TARGET: 7 - 2 - 9");
                targetText.style.color = new StyleColor(new Color(0.12f, 0.23f, 0.54f)); 
                targetText.style.fontSize = 20; targetText.style.unityFontStyleAndWeight = FontStyle.Bold;
                stickyTarget.Add(targetText);
                bg.Add(stickyTarget);
                
                VisualElement[] dials = new VisualElement[3];
                Label[] dialNums = new Label[3];
                for(int i=0; i<3; i++) {
                    VisualElement dial = new VisualElement();
                    dial.style.position = Position.Absolute; 
                    dial.style.left = Length.Percent(70 + (i*10)); dial.style.translate = new StyleTranslate(new Translate(Length.Percent(-50), Length.Percent(-50), 0));
                    dial.style.top = Length.Percent(50); dial.style.width = 40; dial.style.height = 105;
                    dial.style.backgroundColor = new StyleColor(new Color(0.64f, 0.5f, 0.13f)); 
                    dial.style.overflow = Overflow.Hidden;
                    dial.style.borderLeftWidth = 2; dial.style.borderRightWidth = 2;
                    dial.style.borderLeftColor = dial.style.borderRightColor = new StyleColor(new Color(0.44f, 0.33f, 0.08f)); 
                    
                    Label num = new Label("1\n2\n3\n4\n5\n6\n7\n8\n9\n0");
                    num.style.color = new StyleColor(Color.black); num.style.fontSize = 30; num.style.unityTextAlign = TextAnchor.MiddleCenter;
                    num.style.unityFontStyleAndWeight = FontStyle.Bold;
                    num.style.position = Position.Absolute;
                    num.style.width = Length.Percent(100);
                    dial.Add(num);
                    
                    VisualElement sTop = new VisualElement(); sTop.style.position = Position.Absolute; sTop.style.width = Length.Percent(100); sTop.style.height = 20; sTop.style.top = 0; sTop.style.backgroundColor = new StyleColor(new Color(0,0,0, 0.6f));
                    VisualElement sBot = new VisualElement(); sBot.style.position = Position.Absolute; sBot.style.width = Length.Percent(100); sBot.style.height = 20; sBot.style.bottom = 0; sBot.style.backgroundColor = new StyleColor(new Color(0,0,0, 0.6f));
                    dial.Add(sTop); dial.Add(sBot);
                    
                    bg.Add(dial);
                    dials[i] = dial;
                    dialNums[i] = num;
                }
                
                VisualElement bracket = new VisualElement();
                bracket.style.position = Position.Absolute;
                bracket.style.width = Length.Percent(30); bracket.style.height = 35;
                bracket.style.top = Length.Percent(50); bracket.style.left = Length.Percent(80); bracket.style.translate = new StyleTranslate(new Translate(Length.Percent(-50), Length.Percent(-50), 0));
                bracket.style.borderTopWidth = 2; bracket.style.borderBottomWidth = 2;
                bracket.style.borderTopColor = bracket.style.borderBottomColor = new StyleColor(new Color(0.99f, 0.83f, 0.3f)); 
                bg.Add(bracket);

                float[] dialY = {0, 0, 0};
                if (!isOnCooldown) bg.schedule.Execute(() => {
                    if (bg.panel == null) return;
                    for(int i=0; i<3; i++) {
                        dialY[i] -= (3f + (i * 0.5f)); 
                        if(dialY[i] < -200) dialY[i] = 0;
                        dialNums[i].style.top = dialY[i];
                    }
                }).Every(16);
                break;
                
            case "Skim ATM Card Data":
                bg.style.backgroundColor = new StyleColor(new Color(0.05f, 0.1f, 0.05f));
                Color[] colors = { Color.red, Color.blue, Color.green, Color.yellow };
                VisualElement[] atmWires = new VisualElement[4];
                for(int i=0; i<4; i++) {
                    VisualElement l = new VisualElement(); l.style.position = Position.Absolute; l.style.left = Length.Percent(65); l.style.top = 50 + (i*40); l.style.width = 20; l.style.height = 20; l.style.backgroundColor = new StyleColor(colors[i]); l.style.borderTopLeftRadius = 10; l.style.borderTopRightRadius = 10; l.style.borderBottomLeftRadius = 10; l.style.borderBottomRightRadius = 10;
                    bg.Add(l);
                    VisualElement r = new VisualElement(); r.style.position = Position.Absolute; r.style.right = 20; r.style.top = 50 + (i*40); r.style.width = 20; r.style.height = 20; r.style.backgroundColor = new StyleColor(colors[i]); r.style.borderTopLeftRadius = 10; r.style.borderTopRightRadius = 10; r.style.borderBottomLeftRadius = 10; r.style.borderBottomRightRadius = 10;
                    bg.Add(r);
                    VisualElement w = new VisualElement(); w.style.position = Position.Absolute; w.style.left = Length.Percent(65); w.style.marginLeft = 10; w.style.top = 58 + (i*40); w.style.height = 4; w.style.backgroundColor = new StyleColor(colors[i]); w.style.width = 0;
                    bg.Add(w);
                    atmWires[i] = w;
                }
                int currWire = 0;
                float wirePercent = 0;
                if (!isOnCooldown) bg.schedule.Execute(() => {
                    if (bg.panel == null) return;
                    wirePercent += 3f;
                    if(wirePercent > 100f) {
                        currWire++;
                        wirePercent = 0;
                        if(currWire >= 4) {
                            currWire = 0;
                            foreach(var w in atmWires) w.style.width = 0;
                        }
                    }
                    if (currWire < 4) {
                        atmWires[currWire].style.width = Length.Percent(wirePercent * 0.33f); 
                    }
                }).Every(16);
                break;
                
            case "Corporate Espionage Heist":
                bg.style.backgroundColor = new StyleColor(new Color(0.05f, 0.05f, 0.1f));
                Label targetsLbl = new Label("TARGETS: 24 81 55 12");
                targetsLbl.style.position = Position.Absolute; targetsLbl.style.top = 10; 
                targetsLbl.style.left = Length.Percent(80); targetsLbl.style.translate = new StyleTranslate(new Translate(Length.Percent(-50), 0, 0));
                targetsLbl.style.color = new StyleColor(new Color(0.93f, 0.26f, 0.26f)); targetsLbl.style.fontSize = 18; targetsLbl.style.unityFontStyleAndWeight = FontStyle.Bold;
                bg.Add(targetsLbl);
                VisualElement ipGrid = new VisualElement();
                ipGrid.style.position = Position.Absolute; ipGrid.style.top = 40; ipGrid.style.left = Length.Percent(65); ipGrid.style.right = 10; ipGrid.style.bottom = 10;
                ipGrid.style.flexWrap = Wrap.Wrap; ipGrid.style.flexDirection = FlexDirection.Row;
                Label[] gridNums = new Label[40];
                for(int i=0; i<40; i++) {
                    Label l = new Label(Random.Range(10, 99).ToString());
                    l.style.width = Length.Percent(18); l.style.height = 25; l.style.fontSize = 18; l.style.color = new StyleColor(new Color(0.5f, 0.5f, 0.5f));
                    ipGrid.Add(l);
                    gridNums[i] = l;
                }
                bg.Add(ipGrid);
                int ticks = 0;
                if (!isOnCooldown) bg.schedule.Execute(() => {
                    if (bg.panel == null) return;
                    ticks++;
                    if(ticks % 15 == 0) {
                        int rnd = Random.Range(0, 40);
                        gridNums[rnd].style.color = new StyleColor(new Color(0.2f, 0.8f, 0.2f)); 
                        gridNums[rnd].style.unityFontStyleAndWeight = FontStyle.Bold;
                    }
                    if(ticks > 120) {
                        ticks = 0;
                        foreach(var l in gridNums) { l.style.color = new StyleColor(new Color(0.5f, 0.5f, 0.5f)); l.style.unityFontStyleAndWeight = FontStyle.Normal; }
                    }
                }).Every(16);
                break;
                
            default:
                bg.style.backgroundColor = new StyleColor(new Color(0.2f, 0.2f, 0.2f, 1f));
                break;
        }

        return bg;
    }

    private void OnHustleClicked(SideHustleItem hustle)
    {
        var data = GameManager.Instance.currentData;
        if (data.hustlesCompletedThisWeek >= 3)
        {
            global::GameUIManager.Instance.ShowPopup(new RandomEvent { 
                title = "EXHAUSTION", 
                description = "You've reached your maximum of 3 side hustles this week." 
            });
            return; 
        }
        
        data.hustlesCompletedThisWeek++;
        data.hustleCooldownNames.Add(hustle.name);
        data.hustleCooldownWeeks.Add(2);
        
        GameManager.Instance.SaveGame();
        PopulateHustlesBoard(); 

        ActiveMiniGame gameConfig = new ActiveMiniGame(
            category: "side_hustle",
            name: hustle.name,
            reward: (float)hustle.reward
        );

        MiniGameManager.Instance.LaunchMiniGame(gameConfig);
    }
}

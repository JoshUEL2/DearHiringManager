using UnityEngine;
using UnityEngine.UIElements;

public class ActivitiesUI
{
    public static ActivitiesUI Instance;

    private UIDocument activitiesDocument;
    private VisualElement wellnessContainer;
    private VisualElement mixersContainer;

    public void Initialize(UIDocument doc)
    {
        activitiesDocument = doc;
        if (activitiesDocument == null) return;
        
        var root = activitiesDocument.rootVisualElement;

        // Containers
        wellnessContainer = root.Q<VisualElement>("WellnessContainer");
        mixersContainer = root.Q<VisualElement>("MixersContainer");

        root.Q<Button>("BtnTabStress")?.RegisterCallback<ClickEvent>(ev => {
            wellnessContainer.style.display = DisplayStyle.Flex;
            mixersContainer.style.display = DisplayStyle.None;
        });

        root.Q<Button>("BtnTabMixers")?.RegisterCallback<ClickEvent>(ev => {
            wellnessContainer.style.display = DisplayStyle.None;
            mixersContainer.style.display = DisplayStyle.Flex;
        });

        // Wellness Buttons
        root.Q<Button>("BtnPark")?.RegisterCallback<ClickEvent>(ev => DoWellness(10.0, 0.12f, "Walk in the Park"));
        root.Q<Button>("BtnVR")?.RegisterCallback<ClickEvent>(ev => DoWellness(35.0, 0.25f, "Capsule Arcade VR"));
        root.Q<Button>("BtnOxygen")?.RegisterCallback<ClickEvent>(ev => DoWellness(90.0, 0.45f, "Volcanic Hot Stone Spa"));
        root.Q<Button>("BtnSpa")?.RegisterCallback<ClickEvent>(ev => DoWellness(250.0, 0.70f, "Suburban Sensory Mud Spa"));
        root.Q<Button>("BtnCryo")?.RegisterCallback<ClickEvent>(ev => DoWellness(1200.0, 1.0f, "Luxury Cryo Therapy"));

        // Mixer Buttons
        root.Q<Button>("BtnMixerManual")?.RegisterCallback<ClickEvent>(ev => DoNetworking(40.0, "manual", 0.15f));
        root.Q<Button>("BtnMixerHospitality")?.RegisterCallback<ClickEvent>(ev => DoNetworking(120.0, "hospitality", 0.15f));
        root.Q<Button>("BtnMixerTech")?.RegisterCallback<ClickEvent>(ev => DoNetworking(300.0, "tech", 0.15f));
        root.Q<Button>("BtnMixerCorp")?.RegisterCallback<ClickEvent>(ev => DoNetworking(1000.0, "corp", 0.20f));
    }

    private void DoWellness(double cost, float stressReduction, string actName)
    {
        var data = GameManager.Instance.currentData;
        if (data.balance >= cost)
        {
            // We pass the stress reduction to stressCost, and cost to reward (which gets deducted in MiniGameManager or handled there)
            ActiveMiniGame game = new ActiveMiniGame("wellness", actName, stressReduction, 0f, cost, false);
            MiniGameManager.Instance.LaunchMiniGame(game);
        }
        else
        {
            GameEvents.TriggerShowPopup(new RandomEvent { 
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
            if (type == "corp") actName = "Corporate Golf Sim";

            // encode type in readinessGain (hacky but works if we don't modify the model further)
            // Actually, let's just use stressCost as luckBoost and reward as cost, and we can check the name in MiniGameManager
            ActiveMiniGame game = new ActiveMiniGame("mixer", actName, luckBoost, 0f, cost, false);
            MiniGameManager.Instance.LaunchMiniGame(game);
        }
        else
        {
            GameEvents.TriggerShowPopup(new RandomEvent { 
                title = "INSUFFICIENT FUNDS", 
                description = $"You need <color=#E74C3C>${cost}</color> to attend this networking mixer." 
            });
        }
    }
}
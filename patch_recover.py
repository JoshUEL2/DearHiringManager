import re

with open('Assets/Scripts/3 UI/ActivitiesUI.cs', 'r') as f:
    content = f.read()

recovery = '''
        return bg;
    }

    private void DoWellness(double cost, float stressReduction, string actName)
    {
        var data = GameManager.Instance.currentData;
        if (data.balance >= cost)
        {
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
            if (type == "corp") actName = "Corporate Golf Sim";

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
'''
if "private void DoWellness" not in content:
    content += recovery

with open('Assets/Scripts/3 UI/ActivitiesUI.cs', 'w') as f:
    f.write(content)

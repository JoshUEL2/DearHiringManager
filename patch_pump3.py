import re

path = 'Assets/Scripts/2 Managers/MiniGames/PumpSystemMiniGame.cs'
with open(path, 'r') as f:
    content = f.read()

target1 = r'''                    // User requested pump to only increase when moving DOWN!
                    if \(dy > 0\) 
                    \{
                        pumpProgress \+= dy \* 0\.4f; // Give it a nice kick
                    \}'''

replacement1 = r'''                    // Pumping direction depends on the game mode!
                    if (config.pumpIsBalanceMode)
                    {
                        if (dy > 0) pumpProgress += dy * 0.4f; // Downward stroke
                    }
                    else
                    {
                        if (dy < 0) pumpProgress += -dy * 0.4f; // Upward stroke
                    }'''
content = re.sub(target1, replacement1, content)

target2 = r'''        // Inject vector gauge into Dial Container
        if \(pumpDialContainer != null\)
        \{
            pumpDialContainer\.style\.display = DisplayStyle\.Flex;'''

replacement2 = r'''        // Inject vector gauge into Dial Container
        if (pumpDialContainer != null)
        {
            pumpDialContainer.style.display = DisplayStyle.Flex;
            
            // Scale everything up massively as requested
            pumpDialContainer.style.width = 320;
            pumpDialContainer.style.height = 320;
            pumpDialContainer.style.borderTopLeftRadius = 160;
            pumpDialContainer.style.borderTopRightRadius = 160;
            pumpDialContainer.style.borderBottomLeftRadius = 160;
            pumpDialContainer.style.borderBottomRightRadius = 160;
            pumpDialContainer.style.marginRight = 80;
            
            if (pumpDialNeedleRotator != null && pumpDialNeedleRotator.childCount > 0)
            {
                var needle = pumpDialNeedleRotator.ElementAt(0);
                needle.style.height = 140;
                needle.style.top = 20;
                needle.style.width = 8;
            }
            
            var oilLabel = pumpDialContainer.Q<Label>();
            if (oilLabel != null)
            {
                oilLabel.style.fontSize = 20;
                oilLabel.style.bottom = 45;
            }
            
            if (pumpTrackArea != null)
            {
                pumpTrackArea.style.width = 120;
                pumpTrackArea.style.borderTopLeftRadius = 60;
                pumpTrackArea.style.borderTopRightRadius = 60;
                pumpTrackArea.style.borderBottomLeftRadius = 60;
                pumpTrackArea.style.borderBottomRightRadius = 60;
            }
            
            if (pumpHandle != null)
            {
                pumpHandle.style.width = 160;
                pumpHandle.style.height = 60;
                pumpHandle.style.borderTopLeftRadius = 15;
                pumpHandle.style.borderTopRightRadius = 15;
                pumpHandle.style.borderBottomLeftRadius = 15;
                pumpHandle.style.borderBottomRightRadius = 15;
            }
            
            var shaft = pumpTrackArea.Q<VisualElement>("PumpShaft");
            if (shaft != null)
            {
                shaft.style.width = 30;
            }'''
content = re.sub(target2, replacement2, content)

with open(path, 'w') as f:
    f.write(content)

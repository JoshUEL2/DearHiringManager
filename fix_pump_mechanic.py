import re

path = 'Assets/Scripts/2 Managers/MiniGames/PumpSystemMiniGame.cs'
with open(path, 'r') as f:
    content = f.read()

target_input = r'''                    // Pumping direction depends on the game mode!
                    if \(config\.pumpIsBalanceMode\)
                    \{
                        if \(dy > 0\) pumpProgress \+= dy \* 0\.4f; // Downward stroke
                    \}
                    else
                    \{
                        if \(dy < 0\) pumpProgress \+= -dy \* 0\.4f; // Upward stroke
                    \}'''

replacement_input = r'''                    // Pumping direction depends on the game mode!
                    if (config.pumpIsBalanceMode)
                    {
                        if (dy > 0) pumpProgress += dy * 0.15f; // Downward stroke
                    }
                    else
                    {
                        if (dy > 0) pumpProgress += dy * 0.08f; // Downward stroke for normal too!
                    }'''

content = re.sub(target_input, replacement_input, content)

target_decay = r'''                // Normal Mode \(e\.g\., Pump Up a tyre or something\)
                float decayFactor = 1f \+ \(pumpProgress / 100f\) \* 2f; 
                pumpProgress -= \(decayRate \* 0\.5f\) \* decayFactor \* Time\.deltaTime;'''

replacement_decay = r'''                // Normal Mode (e.g., Pump Up a tyre or something)
                float decayFactor = 1f + (pumpProgress / 100f) * 2f; 
                pumpProgress -= (decayRate * 0.85f) * decayFactor * Time.deltaTime;'''

content = re.sub(target_decay, replacement_decay, content)

with open(path, 'w') as f:
    f.write(content)


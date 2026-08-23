import re

path = 'Assets/Scripts/2 Managers/MiniGames/CodeBreakerMiniGame.cs'
with open(path, 'r') as f:
    content = f.read()

target = r'''        // Pass 2: Yellows & Grays \+ Animation
        for \(int i=0; i<config\.codeLength; i\+\+\) 
        \{
            Color targetColor;
            if \(isGreen\[i\]\) 
            \{
                targetColor = new Color\(0\.1f, 0\.8f, 0\.1f\); // Neon Green
            \} 
            else if \(secretCounts\.ContainsKey\(currentGuess\[i\]\) && secretCounts\[currentGuess\[i\]\] > 0\) 
            \{
                targetColor = new Color\(0\.9f, 0\.7f, 0\.1f\); // Amber
                secretCounts\[currentGuess\[i\]\]--;
            \} 
            else 
            \{
                targetColor = new Color\(0\.2f, 0\.2f, 0\.2f\); // Dark Gray
            \}'''

replacement = r'''        // Pass 2: Yellows & Grays + Animation
        for (int i=0; i<config.codeLength; i++) 
        {
            Color targetColor;
            if (isGreen[i]) 
            {
                targetColor = new Color(0.18f, 0.8f, 0.44f); // Standard Wordle Green
            } 
            else if (secretCounts.ContainsKey(currentGuess[i]) && secretCounts[currentGuess[i]] > 0) 
            {
                targetColor = new Color(0.94f, 0.76f, 0.05f); // Standard Wordle Yellow
                secretCounts[currentGuess[i]]--;
            } 
            else 
            {
                targetColor = new Color(0.2f, 0.25f, 0.34f); // Slate Gray for misses
            }'''

content = re.sub(target, replacement, content)

with open(path, 'w') as f:
    f.write(content)

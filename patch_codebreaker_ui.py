import re

path = 'Assets/Scripts/2 Managers/MiniGames/CodeBreakerMiniGame.cs'
with open(path, 'r') as f:
    content = f.read()

target = r'''    private void ApplyJuiceLayout\(\)
    \{
        // Hacker Terminal Vibe
        container\.style\.backgroundColor = new StyleColor\(new Color\(0\.02f, 0\.05f, 0\.02f\)\); // Very dark green/black
        container\.style\.borderTopWidth = 2;
        container\.style\.borderTopColor = new StyleColor\(new Color\(0\.2f, 0\.8f, 0\.2f\)\);
        
        codeAttemptsText\.style\.color = new StyleColor\(new Color\(0\.2f, 0\.9f, 0\.2f\)\); // Neon green
        codeAttemptsText\.style\.unityFontStyleAndWeight = FontStyle\.Bold;
        
        // Update instruction text if possible
        var instructionLabel = container\.parent\.Q<Label>\("MiniGameInstructions"\);
        if \(instructionLabel != null\)
        \{
            instructionLabel\.style\.color = new StyleColor\(new Color\(0\.2f, 0\.9f, 0\.2f\)\);
        \}
    \}'''

replacement = r'''    private void ApplyJuiceLayout()
    {
        // Clean Modern UI Vibe (Matching Kanban / UI Toolkit popup)
        container.style.backgroundColor = new StyleColor(Color.clear);
        container.style.borderTopWidth = 0;
        
        codeAttemptsText.style.color = new StyleColor(new Color(0.9f, 0.7f, 0.1f)); // Amber/Yellow
        codeAttemptsText.style.unityFontStyleAndWeight = FontStyle.Bold;
        
        // Restore instruction text
        var instructionLabel = container.parent.Q<Label>("MiniGameInstructions");
        if (instructionLabel != null)
        {
            instructionLabel.style.color = new StyleColor(Color.white);
        }
    }'''

content = re.sub(target, replacement, content)

with open(path, 'w') as f:
    f.write(content)

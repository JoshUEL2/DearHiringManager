import re

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGames/DragCenterMiniGame.cs', 'r') as f:
    content = f.read()

# Replace `private int score = 0;` with `private int chances = 3;`
content = content.replace("private int score = 0;", "private int chances = 3;")

# Update `StartGame`
content = content.replace("score = 0;", "chances = 3;")

# Update `UpdateScoreText`
old_update = """    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Centered: {score}/3";
        }
    }"""
new_update = """    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Chances left: {chances}";
        }
    }"""
content = content.replace(old_update, new_update)

# Update `ScoreRoutine`
old_score = """        if (matchPercent >= 90f) // Use legacy 90% threshold
        {
            if (scoreValueText != null) scoreValueText.style.color = new StyleColor(new Color(0.13f, 0.77f, 0.36f)); // Green
            score++;
            UpdateScoreText();
        }
        else
        {
            if (scoreValueText != null) scoreValueText.style.color = new StyleColor(new Color(0.9f, 0.1f, 0.1f)); // Red
            timeRemaining -= 1.5f;
        }
        
        yield return new WaitForSeconds(1f);
        
        if (score >= 3)
        {
            manager.MarkWinReady();
        }
        else
        {
            ResetRound();
        }"""
new_score = """        if (matchPercent >= 90f) // Use legacy 90% threshold
        {
            if (scoreValueText != null) scoreValueText.style.color = new StyleColor(new Color(0.13f, 0.77f, 0.36f)); // Green
            yield return new WaitForSeconds(1f);
            manager.MarkWinReady(); // Win immediately!
        }
        else
        {
            if (scoreValueText != null) scoreValueText.style.color = new StyleColor(new Color(0.9f, 0.1f, 0.1f)); // Red
            chances--;
            UpdateScoreText();
            yield return new WaitForSeconds(1f);
            
            if (chances <= 0)
            {
                manager.AbortGame(); // Out of chances, fail game
            }
            else
            {
                ResetRound();
            }
        }"""
content = content.replace(old_score, new_score)

with open('/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGames/DragCenterMiniGame.cs', 'w') as f:
    f.write(content)
print("Updated DragCenterMiniGame.cs")

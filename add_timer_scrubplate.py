import re

path = 'Assets/Scripts/2 Managers/MiniGames/ScrubPlateMiniGame.cs'
with open(path, 'r') as f:
    content = f.read()

# Add timer variables
vars_target = r'''    private Label scraperLabel;
    
    private List<Vector2> dirtParticles = new List<Vector2>\(\);'''
vars_replacement = r'''    private Label scraperLabel;
    private Label timerLabel;
    private float timeLeft;
    
    private List<Vector2> dirtParticles = new List<Vector2>();'''
content = re.sub(vars_target, vars_replacement, content)

# Initialize timer UI
init_target = r'''            scraperLabel\.style\.color = Color\.white;
            scraperLabel\.style\.unityTextAlign = TextAnchor\.MiddleCenter;
            drawArea\.Add\(scraperLabel\);'''
init_replacement = r'''            scraperLabel.style.color = Color.white;
            scraperLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            drawArea.Add(scraperLabel);
            
            timerLabel = new Label();
            timerLabel.style.position = Position.Absolute;
            timerLabel.style.right = 15;
            timerLabel.style.top = 15;
            timerLabel.style.fontSize = 24;
            timerLabel.style.color = Color.white;
            timerLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            drawArea.Add(timerLabel);'''
content = re.sub(init_target, init_replacement, content)

# StartGame setup timer
setup_target = r'''    private void SetupScrubPlate\(\)
    \{
        dirtParticles\.Clear\(\);'''
setup_replacement = r'''    private void SetupScrubPlate()
    {
        timeLeft = config.timeLimit > 0 ? config.timeLimit : 15f; // Default 15s if none specified
        dirtParticles.Clear();'''
content = re.sub(setup_target, setup_replacement, content)

# Update logic inside GameRoutine
routine_target = r'''    private System\.Collections\.IEnumerator GameRoutine\(\)
    \{
        while \(manager != null && manager\.isPlaying\)
        \{
            bool needsRepaint = false;
            for \(int i = sparks\.Count - 1; i >= 0; i--\) \{'''
routine_replacement = r'''    private System.Collections.IEnumerator GameRoutine()
    {
        while (manager != null && manager.isPlaying)
        {
            if (timeLeft > 0)
            {
                timeLeft -= Time.deltaTime;
                timerLabel.text = $"⏱️ {Mathf.CeilToInt(timeLeft)}s";
                if (timeLeft <= 0)
                {
                    manager.AbortGame();
                }
            }

            bool needsRepaint = false;
            for (int i = sparks.Count - 1; i >= 0; i--) {'''
content = re.sub(routine_target, routine_replacement, content)

with open(path, 'w') as f:
    f.write(content)

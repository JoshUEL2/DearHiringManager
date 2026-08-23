import re

path = 'Assets/Scripts/2 Managers/MiniGames/DialogueChainMiniGame.cs'
with open(path, 'r') as f:
    content = f.read()

target = r'''        if \(btnResponse1 != null\) btnResponse1\.text = config\.dialogueOptions\[dialogueRound\]\[0\];
        if \(btnResponse2 != null\) btnResponse2\.text = config\.dialogueOptions\[dialogueRound\]\[1\];
        if \(btnResponse3 != null\) btnResponse3\.text = config\.dialogueOptions\[dialogueRound\]\[2\];
    \}

    private void OnDialogueResponse\(int idx, Button btn\)
    \{
        if \(!isPlaying || !manager\.isPlaying || isTransitioning\) return;
        
        // Button physically depresses
        btn\.style\.scale = new StyleScale\(new Vector2\(0\.95f, 0\.95f\)\);
        btn\.schedule\.Execute\(\(\) => \{
            if \(btn != null\) btn\.style\.scale = new StyleScale\(new Vector2\(1f, 1f\)\);
        \}\)\.StartingIn\(100\);

        if \(idx == config\.dialogueCorrectIndices\[dialogueRound\]\) 
        \{
            manager\.StartCoroutine\(HandleFeedback\(btn, true\)\);
        \}
        else 
        \{
            manager\.StartCoroutine\(HandleFeedback\(btn, false\)\);
        \}
    \}'''

replacement = r'''        // --- DYNAMIC SHUFFLING ---
        // Grab the correct option
        int correctIdx = config.dialogueCorrectIndices[dialogueRound];
        string correctOption = config.dialogueOptions[dialogueRound][correctIdx];
        
        // Grab all wrong options
        System.Collections.Generic.List<string> wrongOptions = new System.Collections.Generic.List<string>();
        for (int i = 0; i < config.dialogueOptions[dialogueRound].Length; i++)
        {
            if (i != correctIdx) wrongOptions.Add(config.dialogueOptions[dialogueRound][i]);
        }
        
        // Shuffle wrong options to pick 2 random ones (if we have more than 2)
        for (int i = 0; i < wrongOptions.Count; i++) {
            int r = UnityEngine.Random.Range(i, wrongOptions.Count);
            string temp = wrongOptions[i]; wrongOptions[i] = wrongOptions[r]; wrongOptions[r] = temp;
        }
        
        // Final options pool for this round
        System.Collections.Generic.List<string> finalOptions = new System.Collections.Generic.List<string>();
        finalOptions.Add(correctOption);
        if (wrongOptions.Count > 0) finalOptions.Add(wrongOptions[0]);
        if (wrongOptions.Count > 1) finalOptions.Add(wrongOptions[1]);
        
        // Shuffle the final 3 buttons
        for (int i = 0; i < finalOptions.Count; i++) {
            int r = UnityEngine.Random.Range(i, finalOptions.Count);
            string temp = finalOptions[i]; finalOptions[i] = finalOptions[r]; finalOptions[r] = temp;
        }
        
        if (btnResponse1 != null) btnResponse1.text = finalOptions.Count > 0 ? finalOptions[0] : "";
        if (btnResponse2 != null) btnResponse2.text = finalOptions.Count > 1 ? finalOptions[1] : "";
        if (btnResponse3 != null) btnResponse3.text = finalOptions.Count > 2 ? finalOptions[2] : "";
        
        // Store correct answer in a hidden field or check string equality in OnDialogueResponse
        this.currentCorrectOption = correctOption;
    }
    
    private string currentCorrectOption;

    private void OnDialogueResponse(int idx, Button btn)
    {
        if (!isPlaying || !manager.isPlaying || isTransitioning) return;
        
        // Button physically depresses
        btn.style.scale = new StyleScale(new Vector2(0.95f, 0.95f));
        btn.schedule.Execute(() => {
            if (btn != null) btn.style.scale = new StyleScale(new Vector2(1f, 1f));
        }).StartingIn(100);

        if (btn.text == currentCorrectOption) 
        {
            manager.StartCoroutine(HandleFeedback(btn, true));
        }
        else 
        {
            manager.StartCoroutine(HandleFeedback(btn, false));
        }
    }'''

content = re.sub(target, replacement, content)

with open(path, 'w') as f:
    f.write(content)

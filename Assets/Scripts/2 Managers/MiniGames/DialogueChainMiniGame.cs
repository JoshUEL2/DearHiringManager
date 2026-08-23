using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DialogueChainMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement container;
    private VisualElement rootContainer;

    private Label dialogueRoundText;
    private Label dialogueScenarioText;
    private VisualElement rapportBar;
    private VisualElement dialogueTimerBar;
    private Button btnResponse1;
    private Button btnResponse2;
    private Button btnResponse3;

    private int dialogueRound = 0;
    private float targetRapportValue = 0.5f;
    private float currentRapportValue = 0.5f;
    private float dialogueTimer = 0f;
    private float dialogueTimerMax = 3f;
    private bool isPlaying = false;
    private bool isTransitioning = false;
    
    private string currentCorrectOption;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        
        rootContainer = root;
        container = root.Q<VisualElement>("DialogueChainContainer");
        dialogueRoundText = container.Q<Label>("DialogueRoundText");
        dialogueScenarioText = container.Q<Label>("DialogueScenarioText");
        rapportBar = container.Q<VisualElement>("RapportBar");
        dialogueTimerBar = container.Q<VisualElement>("DialogueTimerBar");
        
        btnResponse1 = container.Q<Button>("BtnResponse1");
        btnResponse2 = container.Q<Button>("BtnResponse2");
        btnResponse3 = container.Q<Button>("BtnResponse3");

        if (btnResponse1 != null) btnResponse1.clicked += () => OnDialogueResponse(btnResponse1);
        if (btnResponse2 != null) btnResponse2.clicked += () => OnDialogueResponse(btnResponse2);
        if (btnResponse3 != null) btnResponse3.clicked += () => OnDialogueResponse(btnResponse3);
        
        // Fix gap styling
        if (dialogueScenarioText != null) 
        {
            dialogueScenarioText.style.marginBottom = 10;
        }
        if (btnResponse1 != null) btnResponse1.style.marginBottom = 5;
        if (btnResponse2 != null) btnResponse2.style.marginBottom = 5;
        if (btnResponse3 != null) btnResponse3.style.marginBottom = 5;
    }

    private void ResetButtonStyles()
    {
        StyleColor defaultBg = new StyleColor(new Color(0.12f, 0.16f, 0.23f)); // Slate
        StyleColor defaultBorder = new StyleColor(new Color(0.28f, 0.33f, 0.41f));
        
        if (btnResponse1 != null) { btnResponse1.style.backgroundColor = defaultBg; btnResponse1.style.borderTopColor = defaultBorder; btnResponse1.style.borderBottomColor = defaultBorder; btnResponse1.style.borderLeftColor = defaultBorder; btnResponse1.style.borderRightColor = defaultBorder; }
        if (btnResponse2 != null) { btnResponse2.style.backgroundColor = defaultBg; btnResponse2.style.borderTopColor = defaultBorder; btnResponse2.style.borderBottomColor = defaultBorder; btnResponse2.style.borderLeftColor = defaultBorder; btnResponse2.style.borderRightColor = defaultBorder; }
        if (btnResponse3 != null) { btnResponse3.style.backgroundColor = defaultBg; btnResponse3.style.borderTopColor = defaultBorder; btnResponse3.style.borderBottomColor = defaultBorder; btnResponse3.style.borderLeftColor = defaultBorder; btnResponse3.style.borderRightColor = defaultBorder; }
    }

    public void StartGame()
    {
        isPlaying = true;
        isTransitioning = false;
        dialogueRound = 0;
        
        targetRapportValue = 0.5f;
        currentRapportValue = 0.5f;
        
        container.style.display = DisplayStyle.Flex;
        ResetButtonStyles();
        LoadNextRound();
        manager.StartCoroutine(DialogueChainRoutine());
    }

    public void AbortGame()
    {
        isPlaying = false;
        container.style.display = DisplayStyle.None;
    }

    private void LoadNextRound()
    {
        if (dialogueRound >= config.dialogueScenarios.Length)
        {
            if (targetRapportValue >= config.rapportThreshold || (manager.currentGame != null && !manager.currentGame.canFail)) 
            {
                isPlaying = false;
                manager.MarkWinReady();
            }
            else 
            { 
                isPlaying = false; 
                manager.AbortGame();
            } 
            return;
        }
        
        dialogueTimerMax = Mathf.Max(1.0f, 3f - (dialogueRound * 0.5f));
        dialogueTimer = dialogueTimerMax;
        
        if (dialogueRoundText != null) dialogueRoundText.text = $"Round {dialogueRound + 1} / {config.dialogueScenarios.Length}";
        
        // Smart Name Parsing
        if (dialogueScenarioText != null) 
        {
            // Enable rich text since we are injecting color tags
            dialogueScenarioText.enableRichText = true;
            
            string rawScenario = config.dialogueScenarios[dialogueRound];
            int colonIndex = rawScenario.IndexOf(":");
            if (colonIndex != -1)
            {
                string speaker = rawScenario.Substring(0, colonIndex);
                string dialogue = rawScenario.Substring(colonIndex + 1).Trim();
                dialogueScenarioText.text = $"<color=#38BDF8>{speaker}:</color> \"{dialogue}\"";
            }
            else
            {
                dialogueScenarioText.text = rawScenario;
            }
        }
        
        // --- DYNAMIC SHUFFLING ---
        int correctIdx = config.dialogueCorrectIndices[dialogueRound];
        string correctOption = config.dialogueOptions[dialogueRound][correctIdx];
        
        List<string> wrongOptions = new List<string>();
        for (int i = 0; i < config.dialogueOptions[dialogueRound].Length; i++)
        {
            if (i != correctIdx) wrongOptions.Add(config.dialogueOptions[dialogueRound][i]);
        }
        
        // Shuffle wrong options
        for (int i = 0; i < wrongOptions.Count; i++) {
            int r = Random.Range(i, wrongOptions.Count);
            string temp = wrongOptions[i]; wrongOptions[i] = wrongOptions[r]; wrongOptions[r] = temp;
        }
        
        List<string> finalOptions = new List<string>();
        finalOptions.Add(correctOption);
        if (wrongOptions.Count > 0) finalOptions.Add(wrongOptions[0]);
        if (wrongOptions.Count > 1) finalOptions.Add(wrongOptions[1]);
        
        // Shuffle the final 3 buttons
        for (int i = 0; i < finalOptions.Count; i++) {
            int r = Random.Range(i, finalOptions.Count);
            string temp = finalOptions[i]; finalOptions[i] = finalOptions[r]; finalOptions[r] = temp;
        }
        
        if (btnResponse1 != null) btnResponse1.text = finalOptions.Count > 0 ? finalOptions[0] : "";
        if (btnResponse2 != null) btnResponse2.text = finalOptions.Count > 1 ? finalOptions[1] : "";
        if (btnResponse3 != null) btnResponse3.text = finalOptions.Count > 2 ? finalOptions[2] : "";
        
        currentCorrectOption = correctOption;
    }

    private void OnDialogueResponse(Button btn)
    {
        if (!isPlaying || !manager.isPlaying || isTransitioning) return;

        if (btn.text == currentCorrectOption) 
        {
            manager.StartCoroutine(HandleFeedback(btn, true));
        }
        else 
        {
            manager.StartCoroutine(HandleFeedback(btn, false));
        }
    }

    private IEnumerator HandleFeedback(Button btn, bool isCorrect)
    {
        isTransitioning = true; // Freeze timer and input
        
        if (isCorrect)
        {
            targetRapportValue += 0.25f;
            
            if (btn != null)
            {
                StyleColor greenColor = new StyleColor(new Color(0.13f, 0.77f, 0.36f));
                btn.style.backgroundColor = greenColor;
                btn.style.borderTopColor = greenColor;
                btn.style.borderBottomColor = greenColor;
                btn.style.borderLeftColor = greenColor;
                btn.style.borderRightColor = greenColor;
                
                manager.StartCoroutine(AnimateFloatingText(btn, "+ Rapport", new Color(0.13f, 0.77f, 0.36f)));
            }
        }
        else
        {
            targetRapportValue -= 0.15f;
            
            if (btn != null)
            {
                StyleColor redColor = new StyleColor(new Color(0.93f, 0.26f, 0.22f));
                btn.style.backgroundColor = redColor;
                btn.style.borderTopColor = redColor;
                btn.style.borderBottomColor = redColor;
                btn.style.borderLeftColor = redColor;
                btn.style.borderRightColor = redColor;
                
                manager.StartCoroutine(AnimateFloatingText(btn, "- Rapport", new Color(0.93f, 0.26f, 0.22f)));
            }
            else
            {
                // Timeout / Ignored
                manager.StartCoroutine(AnimateIgnoredEffect());
            }
            
            // Screen Shake on failure
            float shakeAmt = 8f;
            container.style.left = shakeAmt;
            yield return new WaitForSeconds(0.05f);
            container.style.left = -shakeAmt;
            yield return new WaitForSeconds(0.05f);
            container.style.left = 0f;
        }
        
        targetRapportValue = Mathf.Clamp01(targetRapportValue);
        
        // Wait longer if it was an IGNORED popup so they can see it
        yield return new WaitForSeconds(btn == null ? 0.8f : 0.3f);
        
        dialogueRound++;
        ResetButtonStyles();
        isTransitioning = false;
        LoadNextRound();
    }
    
    private IEnumerator AnimateIgnoredEffect()
    {
        Label ignoredText = new Label("IGNORED");
        ignoredText.style.position = Position.Absolute;
        ignoredText.style.color = new StyleColor(new Color(0.93f, 0.26f, 0.22f)); // Red
        ignoredText.style.fontSize = 72;
        ignoredText.style.unityFontStyleAndWeight = FontStyle.Bold;
        ignoredText.style.unityTextAlign = TextAnchor.MiddleCenter;
        
        // Text Outline for legibility
        ignoredText.style.unityTextOutlineColor = Color.black;
        ignoredText.style.unityTextOutlineWidth = 4f;
        
        // Center text in container
        ignoredText.style.top = new Length(50, LengthUnit.Percent);
        ignoredText.style.left = new Length(50, LengthUnit.Percent);
        ignoredText.style.translate = new Translate(new Length(-50, LengthUnit.Percent), new Length(-50, LengthUnit.Percent));
        
        manager.miniGameDocument.rootVisualElement.Add(ignoredText);
        
        // Pop in animation
        float life = 0.8f;
        float elapsed = 0f;
        while(elapsed < life && isPlaying)
        {
            elapsed += Time.deltaTime;
            float p = elapsed / life;
            
            // Starts big, slams down to size 1
            float scale = Mathf.Lerp(2.5f, 1f, p * 10f); // Fast snap
            if (scale < 1f) scale = 1f;
            ignoredText.style.scale = new StyleScale(new Vector2(scale, scale));
            
            // Fade out at the very end
            if (p > 0.8f) {
                ignoredText.style.opacity = 1f - ((p - 0.8f) * 5f);
            }
            
            yield return null;
        }
        
        if (manager.miniGameDocument.rootVisualElement.Contains(ignoredText)) manager.miniGameDocument.rootVisualElement.Remove(ignoredText);
    }
    
    private IEnumerator AnimateFloatingText(Button parentBtn, string text, Color c)
    {
        Label floatingText = new Label(text);
        floatingText.style.position = Position.Absolute;
        floatingText.style.color = new StyleColor(c);
        floatingText.style.fontSize = 24;
        floatingText.style.unityFontStyleAndWeight = FontStyle.Bold;
        
        // Add to the main container instead of the button so we don't break the button's internal flex layout!
        container.Add(floatingText);
        
        // Position it over the button
        float startY = parentBtn.worldBound.y - container.worldBound.y;
        float startX = parentBtn.worldBound.x - container.worldBound.x + (parentBtn.worldBound.width / 2f) - 50f;
        
        floatingText.style.top = startY;
        floatingText.style.left = startX;
        
        float life = 0.5f;
        float elapsed = 0f;
        while(elapsed < life && isPlaying)
        {
            elapsed += Time.deltaTime;
            float p = elapsed / life;
            
            floatingText.style.top = startY - (p * 30f);
            floatingText.style.opacity = 1f - p;
            yield return null;
        }
        
        if (container.Contains(floatingText)) container.Remove(floatingText);
    }

    private IEnumerator DialogueChainRoutine()
    {
        while (isPlaying && manager.isPlaying && dialogueRound < config.dialogueScenarios.Length)
        {
            float dt = Time.deltaTime;
            
            // Smoothly Lerp Rapport Bar
            if (rapportBar != null)
            {
                currentRapportValue = Mathf.Lerp(currentRapportValue, targetRapportValue, dt * 10f);
                rapportBar.style.width = new Length(currentRapportValue * 100f, LengthUnit.Percent);
            }

            // Only tick timer if we are NOT in a freeze frame
            if (!isTransitioning)
            {
                dialogueTimer -= dt;
                
                if (dialogueTimerBar != null)
                {
                    float pct = Mathf.Clamp01(dialogueTimer / dialogueTimerMax);
                    dialogueTimerBar.style.width = new Length(pct * 100f, LengthUnit.Percent);
                    
                    if (pct > 0.5f) {
                        dialogueTimerBar.style.backgroundColor = new StyleColor(new Color(0.13f, 0.77f, 0.36f)); // Green
                    } else if (pct > 0.25f) {
                        dialogueTimerBar.style.backgroundColor = new StyleColor(new Color(0.96f, 0.62f, 0.04f)); // Orange
                    } else {
                        // Pulse Red aggressively in final 25%
                        float pulse = Mathf.PingPong(Time.time * 8f, 1f);
                        dialogueTimerBar.style.backgroundColor = new StyleColor(Color.Lerp(new Color(0.93f, 0.26f, 0.22f), new Color(1f, 0.6f, 0.6f), pulse));
                    }
                }
                
                if (dialogueTimer <= 0f)
                {
                    if (manager.currentGame != null && manager.currentGame.canFail)
                    {
                        isPlaying = false;
                        manager.AbortGame();
                        yield break;
                    }
                    else
                    {
                        // Time out failure
                        manager.StartCoroutine(HandleFeedback(null, false)); // Pass null to trigger IGNORED popup
                    }
                }
            }
            yield return null;
        }
    }
}

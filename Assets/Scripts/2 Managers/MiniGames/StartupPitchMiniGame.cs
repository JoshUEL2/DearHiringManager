using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Linq;

public class StartupPitchMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement rootElement;

    private VisualElement spContainer;
    private Label[] spSlots = new Label[3];
    private Button spStopBtn;
    
    private int spCurrentReel = 0;
    private float spSpinTimer = 0f;
    private string[] spGoodWords = { "SYNERGY", "AI", "CLOUD", "WEB3", "SAAS", "AGILE" };
    private string[] spBadWords = { "FRAUD", "BANKRUPT", "SCAM", "PONZI", "LAWSUIT" };
    
    // Fixed pattern for spinning reels to allow anticipation
    private int spPatternIndex = 0;
    private string[] spSlotPattern = { "SYNERGY", "FRAUD", "CLOUD", "PONZI", "AI", "SCAM", "WEB3", "BANKRUPT" };

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        this.rootElement = root;

        spContainer = rootElement.Q<VisualElement>("StartupPitchContainer");
        for (int i = 0; i < 3; i++) spSlots[i] = rootElement.Q<Label>($"SpSlot{i}");
        spStopBtn = rootElement.Q<Button>("BtnSpStop");

        if (spStopBtn != null)
        {
            spStopBtn.clicked -= OnSpStopClicked;
            spStopBtn.clicked += OnSpStopClicked;
        }
    }

    public void StartGame()
    {
        if (spContainer != null) spContainer.style.display = DisplayStyle.Flex;

        spCurrentReel = 0;
        spSpinTimer = 0f;
        spPatternIndex = 0;
        if (spStopBtn != null) spStopBtn.text = "STOP REEL 1";
        
        UpdateUnstoppedReels();
        
        manager.StartCoroutine(StartupPitchRoutine());
    }

    public void AbortGame()
    {
        if (spContainer != null) spContainer.style.display = DisplayStyle.None;
        if (spStopBtn != null) spStopBtn.clicked -= OnSpStopClicked;
    }

    private void UpdateUnstoppedReels()
    {
        for (int i = spCurrentReel; i < 3; i++) {
            if (spSlots[i] != null) {
                // Offset the pattern for each reel so they look different but are predictable
                int index = (spPatternIndex + (i * 3)) % spSlotPattern.Length;
                spSlots[i].text = spSlotPattern[index];
                spSlots[i].style.color = Color.white;
            }
        }
    }

    private void OnSpStopClicked()
    {
        if (!manager.isPlaying || spCurrentReel >= 3) return;
        
        string word = spSlots[spCurrentReel].text;
        bool isGood = System.Array.Exists(spGoodWords, w => w == word);
        
        if (isGood) {
            spSlots[spCurrentReel].style.color = new Color(0.18f, 0.8f, 0.44f); // Green
            spCurrentReel++;
            if (spCurrentReel >= 3) {
                if (spStopBtn != null) spStopBtn.text = "FUNDED!";
                manager.MarkWinReady();
            } else {
                if (spStopBtn != null) spStopBtn.text = $"STOP REEL {spCurrentReel + 1}";
            }
        } else {
            spSlots[spCurrentReel].style.color = new Color(0.9f, 0.29f, 0.23f); // Red
            if (spStopBtn != null) spStopBtn.text = "FAILED PITCH";
            manager.AbortGame();
        }
    }

    private IEnumerator StartupPitchRoutine()
    {
        float timer = config.timeLimit;

        while (manager.isPlaying)
        {
            timer -= Time.deltaTime;
            spSpinTimer += Time.deltaTime;
            
            // Slower spin (0.35s) gives time to anticipate the fixed pattern
            if (spSpinTimer > 0.35f) {
                spSpinTimer = 0f;
                spPatternIndex = (spPatternIndex + 1) % spSlotPattern.Length;
                UpdateUnstoppedReels();
            }

            if (timer <= 0f) {
                manager.AbortGame();
                yield break;
            }
            yield return null;
        }
    }
}

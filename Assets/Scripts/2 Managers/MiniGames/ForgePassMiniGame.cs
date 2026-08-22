using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class ForgePassMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement rootElement;

    private VisualElement fpContainer;
    private VisualElement fpMovingPass;
    private Button fpStampBtn;
    private Label fpScoreText;
    private Label fpPassStampOverlay;

    private float timer;
    private float fpPassPos = -250f;
    private int fpAttempts = 0;
    private int fpSuccessfulStamps = 0;
    private float fpSpeed = 700f;
    private bool fpIsPaused = false;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        this.rootElement = root;

        fpContainer = root.Q<VisualElement>("ForgePassContainer");
        fpMovingPass = root.Q<VisualElement>("FpMovingPass");
        fpStampBtn = root.Q<Button>("BtnFpStamp");
        fpScoreText = root.Q<Label>("FpScoreText");
        fpPassStampOverlay = root.Q<Label>("FpPassStampOverlay");

        if (fpStampBtn != null)
        {
            fpStampBtn.clicked += OnFpStampClicked;
        }
    }

    public void StartGame()
    {
        if (fpContainer != null)
        {
            fpContainer.style.display = DisplayStyle.Flex;
        }

        timer = config.timeLimit;
        fpAttempts = 0;
        fpSuccessfulStamps = 0;
        fpSpeed = 700f;
        fpPassPos = -250f;
        fpIsPaused = false;
        
        if (fpScoreText != null) fpScoreText.text = $\"FORGED: {fpSuccessfulStamps} / 1\";
        if (fpStampBtn != null) fpStampBtn.text = \"STAMP NOW! (3 LIVES)\";
        if (fpMovingPass != null) {
            fpMovingPass.style.left = fpPassPos;
            fpMovingPass.style.backgroundColor = new StyleColor(new Color(0.97f, 0.98f, 0.98f));
        }
        if (fpPassStampOverlay != null) {
            fpPassStampOverlay.text = \"\";
            fpPassStampOverlay.style.color = Color.clear;
        }

        manager.StartCoroutine(ForgePassRoutine());
    }

    public void AbortGame()
    {
        if (fpContainer != null)
        {
            fpContainer.style.display = DisplayStyle.None;
        }
        
        if (fpStampBtn != null)
        {
            fpStampBtn.clicked -= OnFpStampClicked;
        }
    }

    private void OnFpStampClicked()
    {
        if (!manager.isPlaying || fpIsPaused) return;
        
        Vector2 passCenter = fpMovingPass.worldBound.center;
        VisualElement stampZone = fpContainer.Q<VisualElement>(\"FpStampZone\");
        Vector2 stampCenter = stampZone.worldBound.center;
        
        float dist = Vector2.Distance(passCenter, stampCenter);
        bool success = (dist < 40f);
        
        manager.StartCoroutine(ShowFpStampResult(success));
    }
    
    private IEnumerator ShowFpStampResult(bool success)
    {
        fpIsPaused = true;
        
        if (fpPassStampOverlay != null) {
            fpPassStampOverlay.text = success ? \"APPROVED\" : \"REJECTED\";
            fpPassStampOverlay.style.color = success ? new Color(0.06f, 0.73f, 0.51f, 0.9f) : new Color(0.94f, 0.27f, 0.27f, 0.9f);
            
            fpPassStampOverlay.style.scale = new StyleScale(new Scale(new Vector3(3f, 3f, 1f)));
            yield return null;
            
            float slamTime = 0.15f;
            float elapsed = 0f;
            while (elapsed < slamTime) {
                elapsed += Time.deltaTime;
                float t = elapsed / slamTime;
                float scale = Mathf.Lerp(3f, 1f, t);
                fpPassStampOverlay.style.scale = new StyleScale(new Scale(new Vector3(scale, scale, 1f)));
                yield return null;
            }
            fpPassStampOverlay.style.scale = new StyleScale(new Scale(new Vector3(1f, 1f, 1f)));
        }
        
        yield return new WaitForSeconds(0.4f);
        
        if (success) {
            fpSuccessfulStamps++;
            if (fpScoreText != null) fpScoreText.text = $\"FORGED: {fpSuccessfulStamps} / 1\";
            if (fpSuccessfulStamps >= 1) {
                manager.MarkWinReady();
                yield break;
            }
        } else {
            fpAttempts++;
            if (fpAttempts >= 3) {
                manager.AbortGame();
                yield break;
            }
            if (fpStampBtn != null) fpStampBtn.text = $\"STAMP NOW! ({3 - fpAttempts} LIVES)\";
        }
        
        fpSpeed += 250f;
        fpPassPos = -250f;
        if (fpPassStampOverlay != null) {
            fpPassStampOverlay.text = \"\";
            fpPassStampOverlay.style.color = Color.clear;
        }
        
        fpIsPaused = false;
    }

    private IEnumerator ForgePassRoutine()
    {
        while (manager.isPlaying)
        {
            if (!fpIsPaused) {
                timer -= Time.deltaTime;
                
                fpPassPos += fpSpeed * Time.deltaTime;
                if (fpPassPos > 700f) {
                    fpAttempts++;
                    if (fpAttempts >= 3) {
                        manager.AbortGame();
                        yield break;
                    } else {
                        fpSpeed += 250f;
                        fpPassPos = -250f;
                        if (fpStampBtn != null) fpStampBtn.text = $\"STAMP NOW! ({3 - fpAttempts} LIVES)\";
                    }
                }
                
                if (fpMovingPass != null) {
                    fpMovingPass.style.left = fpPassPos;
                }
            }

            if (timer <= 0f) {
                manager.AbortGame();
                yield break;
            }
            yield return null;
        }
    }
}

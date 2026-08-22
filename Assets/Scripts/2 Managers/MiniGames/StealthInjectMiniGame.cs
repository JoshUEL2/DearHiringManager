using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class StealthInjectMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement rootElement;

    private VisualElement stealthInjectContainer, siWorkArea, siProgressBarFill;
    private Label siStatusText, siTimerText, siReviewerIcon, siReviewerStateText, siBtnInject;
    
    private float siProgress = 0f;
    private bool siIsReviewing = false;
    private bool siIsHolding = false;
    private float siReviewTimer = 0f;
    private float timer = 0f;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        this.rootElement = root;

        stealthInjectContainer = root.Q<VisualElement>("StealthInjectContainer");
        siWorkArea = root.Q<VisualElement>("SiWorkArea");
        siProgressBarFill = root.Q<VisualElement>("SiProgressBarFill");
        siStatusText = root.Q<Label>("SiStatusText");
        siTimerText = root.Q<Label>("SiTimerText");
        siReviewerIcon = root.Q<Label>("SiReviewerIcon");
        siReviewerStateText = root.Q<Label>("SiReviewerStateText");
        siBtnInject = root.Q<Label>("SiBtnInject");

        if (siBtnInject != null)
        {
            siBtnInject.RegisterCallback<PointerDownEvent>(OnSiPointerDown);
            siBtnInject.RegisterCallback<PointerUpEvent>(OnSiPointerUp);
            siBtnInject.RegisterCallback<PointerCaptureOutEvent>(OnSiPointerCaptureOut);
        }
    }

    public void StartGame()
    {
        if (stealthInjectContainer != null) stealthInjectContainer.style.display = DisplayStyle.Flex;

        timer = config.timeLimit;
        siProgress = 0f;
        siIsReviewing = false;
        siIsHolding = false;
        siReviewTimer = Random.Range(1.5f, 3.5f);
        
        UpdateSiUI();
        manager.StartCoroutine(StealthInjectRoutine());
    }

    public void AbortGame()
    {
        if (stealthInjectContainer != null) stealthInjectContainer.style.display = DisplayStyle.None;

        if (siBtnInject != null)
        {
            siBtnInject.UnregisterCallback<PointerDownEvent>(OnSiPointerDown);
            siBtnInject.UnregisterCallback<PointerUpEvent>(OnSiPointerUp);
            siBtnInject.UnregisterCallback<PointerCaptureOutEvent>(OnSiPointerCaptureOut);
        }
    }

    private void UpdateSiUI()
    {
        if (siTimerText != null) siTimerText.text = $"{Mathf.Max(0, timer):F1}s";
        if (siProgressBarFill != null) siProgressBarFill.style.width = new Length(siProgress, LengthUnit.Percent);
        
        if (siIsReviewing)
        {
            if (siStatusText != null) siStatusText.text = "REVIEWER CHECKING!";
            if (siReviewerIcon != null) siReviewerIcon.text = "🚨";
            if (siReviewerStateText != null) 
            {
                siReviewerStateText.text = "DANGER";
                siReviewerStateText.style.color = new StyleColor(new Color(0.93f, 0.26f, 0.22f)); // red
            }
            if (siWorkArea != null) siWorkArea.style.backgroundColor = new StyleColor(new Color(0.4f, 0.1f, 0.1f));
        }
        else
        {
            if (siStatusText != null) siStatusText.text = "Reviewer distracted...";
            if (siReviewerIcon != null) siReviewerIcon.text = "🙈";
            if (siReviewerStateText != null) 
            {
                siReviewerStateText.text = "SAFE";
                siReviewerStateText.style.color = new StyleColor(new Color(0.13f, 0.77f, 0.36f)); // green
            }
            if (siWorkArea != null) siWorkArea.style.backgroundColor = new StyleColor(new Color(0.06f, 0.09f, 0.16f)); // slate-900
        }
    }

    private void OnSiPointerDown(PointerDownEvent ev)
    {
        if (!manager.isPlaying) return;
        siIsHolding = true;
        if (siBtnInject != null) siBtnInject.CapturePointer(ev.pointerId);
    }

    private void OnSiPointerUp(PointerUpEvent ev)
    {
        siIsHolding = false;
        if (siBtnInject != null) siBtnInject.ReleasePointer(ev.pointerId);
    }

    private void OnSiPointerCaptureOut(PointerCaptureOutEvent ev)
    {
        siIsHolding = false;
    }

    private IEnumerator StealthInjectRoutine()
    {
        while (manager.isPlaying)
        {
            timer -= Time.deltaTime;
            siReviewTimer -= Time.deltaTime;

            if (siReviewTimer <= 0f)
            {
                // Toggle state
                siIsReviewing = !siIsReviewing;
                siReviewTimer = siIsReviewing ? Random.Range(1.0f, 2.0f) : Random.Range(2.0f, 4.0f);
                UpdateSiUI();
            }

            if (siIsHolding)
            {
                if (siIsReviewing)
                {
                    // Caught! Apply massive penalty.
                    siIsHolding = false; // force release internally until they lift and press again
                    siProgress = Mathf.Max(0f, siProgress - 50f);
                    
                    // Flash red intensely
                    if (siWorkArea != null)
                    {
                        siWorkArea.style.backgroundColor = new StyleColor(Color.red);
                        siWorkArea.schedule.Execute(() => { 
                            if (siWorkArea != null && siIsReviewing) siWorkArea.style.backgroundColor = new StyleColor(new Color(0.4f, 0.1f, 0.1f)); 
                        }).StartingIn(300);
                    }
                }
                else
                {
                    // Progressing
                    siProgress += 30f * Time.deltaTime; // Takes ~3.3 seconds of holding to hit 100%
                    if (siProgress >= 100f)
                    {
                        siProgress = 100f;
                        UpdateSiUI();
                        manager.MarkWinReady();
                        yield break;
                    }
                }
            }

            UpdateSiUI();

            if (timer <= 0f)
            {
                if (config.canFail)
                {
                    if (siTimerText != null) siTimerText.text = "TIME UP!";
                    manager.isPlaying = false;
                    yield break;
                }
                else
                {
                    manager.MarkWinReady();
                }
            }
            yield return null;
        }
    }
}

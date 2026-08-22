using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class NightShiftPitchMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement rootElement;

    private VisualElement nightShiftContainer;
    private VisualElement nsEyelid;
    private Label nsTimerText;
    private Button nsCoffeeBtn;

    private VisualElement[] nsHandles = new VisualElement[3];
    private VisualElement[] nsTracks = new VisualElement[3];

    private float[] nsValues = new float[3];
    private float[] nsTargets = new float[3];
    private float nsEyelidGravity = 0f;
    private float timer = 0f;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        this.rootElement = root;

        nightShiftContainer = root.Q<VisualElement>("NightShiftContainer");
        nsEyelid = root.Q<VisualElement>("NsEyelid");
        nsTimerText = root.Q<Label>("NsTimerText");
        nsCoffeeBtn = root.Q<Button>("NsCoffeeBtn");

        for (int i = 0; i < 3; i++)
        {
            nsHandles[i] = root.Q<VisualElement>($"NsHandle{i + 1}");
            nsTracks[i] = root.Q<VisualElement>($"NsTrack{i + 1}");
            
            int idx = i;
            if (nsTracks[i] != null)
            {
                nsTracks[i].RegisterCallback<PointerMoveEvent>(ev => {
                    if (!manager.isPlaying) return;
                    if (ev.pressedButtons != 0) {
                        float relX = ev.localPosition.x;
                        float trackW = nsTracks[idx].resolvedStyle.width;
                        if (trackW <= 0) trackW = 200f; // fallback
                        float val = Mathf.Clamp01(relX / trackW);
                        nsValues[idx] = val;
                    }
                });
                nsTracks[i].RegisterCallback<PointerDownEvent>(ev => {
                    if (!manager.isPlaying) return;
                    float relX = ev.localPosition.x;
                    float trackW = nsTracks[idx].resolvedStyle.width;
                    if (trackW <= 0) trackW = 200f; // fallback
                    float val = Mathf.Clamp01(relX / trackW);
                    nsValues[idx] = val;
                });
            }
        }

        if (nsCoffeeBtn != null)
        {
            nsCoffeeBtn.clicked += () => {
                if (!manager.isPlaying) return;
                nsEyelidGravity = Mathf.Max(0f, nsEyelidGravity - 0.25f);
            };
        }
    }

    public void StartGame()
    {
        nightShiftContainer.style.display = DisplayStyle.Flex;

        timer = config.timeLimit;
        nsEyelidGravity = 0f;
        nsTargets[0] = 0.3f;
        nsTargets[1] = 0.6f;
        nsTargets[2] = 0.8f;

        for(int i=0; i<3; i++) {
            nsValues[i] = 0f;
            if (nsHandles[i] != null) nsHandles[i].style.left = new StyleLength(new Length(0, LengthUnit.Percent));
        }
        
        if (nsEyelid != null) nsEyelid.style.top = new StyleLength(new Length(-100, LengthUnit.Percent));

        manager.StartCoroutine(GameRoutine());
    }

    public void AbortGame()
    {
        if (nightShiftContainer != null)
        {
            nightShiftContainer.style.display = DisplayStyle.None;
        }
    }

    private IEnumerator GameRoutine()
    {
        while (manager.isPlaying)
        {
            timer -= Time.deltaTime;
            
            if (nsTimerText != null) {
                nsTimerText.text = $"{timer:F1}s";
                nsTimerText.style.color = timer <= 3f ? Color.red : Color.white;
            }
            nsEyelidGravity += 0.20f * Time.deltaTime;
            nsEyelidGravity = Mathf.Clamp01(nsEyelidGravity);
            
            if (nsEyelid != null) {
                float topPercent = -100f + (nsEyelidGravity * 100f);
                nsEyelid.style.top = new StyleLength(new Length(topPercent, LengthUnit.Percent));
            }

            if (nsEyelidGravity >= 1f) {
                manager.AbortGame();
                yield break;
            }

            bool allAligned = true;
            for(int i=0; i<3; i++)
            {
                if (nsHandles[i] != null) {
                    float pct = Mathf.Clamp(nsValues[i] * 100f, 0f, 85f);
                    nsHandles[i].style.left = new StyleLength(new Length(pct, LengthUnit.Percent));
                }
                
                if (Mathf.Abs(nsValues[i] - nsTargets[i]) > 0.08f) allAligned = false;
            }

            if (allAligned) {
                manager.MarkWinReady();
                yield break;
            }

            if (timer <= 0f) {
                manager.AbortGame();
                yield break;
            }
            
            yield return null;
        }
    }
}

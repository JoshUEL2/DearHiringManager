using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Video;
using System.Collections;
using System.Linq;

/// <summary>
/// Plays the unified master cinematic intro & loading screen sequence (5.0s = 150 frames @ 30fps):
///   1. 0.00s – 0.35s (0% – 7%):   "DEAR HIRING MANAGER" title lockup, "DEVELOPED BY" header,
///                                 24 Hour Studios emblem, and loading progress bar appear.
///                                 - Musical intro jingle (sfx_intro_jingle) starts while gameplay BGM is ducked.
///   2. 0.40s – 0.80s (8% – 16%):  Cartoon CV swoops into center (sfx_intro_whoosh at 0.40s / Frame 12).
///   3. 0.80s – 1.15s (16% – 23%): 5 CV dossier lines draw with rising marimba notes (sfx_pencil_sketch at 0.80s / Frame 24).
///   4. 1.20s, 1.70s, 2.20s, 2.70s (24%, 34%, 44%, 54% | Frames 36, 51, 66, 81):
///                                 4 exact 5° mechanical clock ticks (5° -> 10° -> 15° -> 20° lock)
///                                 with sfx_clock_tick and haptic ticks.
///   5. 2.70s (54% | Frame 81):    Final 20° clock lock coincides with the red "HIRED!" stamp slam
///                                 (sfx_stamp_slam + HeavyImpact haptic + triumphant jingle chord).
///   6. 3.00s – 4.50s (60% – 90%): Gleam shimmer sweeps across the stamp while loading reaches 100%.
///   7. 4.50s – 5.00s (90% – 100%): Smooth fade out into the main dashboard as gameplay BGM fades in.
/// </summary>
public class LoadingScreenUI : MonoBehaviour
{
    private UIDocument uiDocument;
    private VisualElement rootContainer;
    private VisualElement videoElement;
    private VisualElement fallbackLogo;
    private VisualElement fallbackBadge;

    private VideoPlayer videoPlayer;
    private RenderTexture renderTexture;
    private bool isDismissing = false;

    // Dark Slate Blue (#07090E) - Consistent background throughout the entire intro (no red)
    private static readonly Color LogoDark = new Color(0.027f, 0.035f, 0.055f, 1f);

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void Initialize()
    {
        GameObject obj = new GameObject("LoadingScreen");
        DontDestroyOnLoad(obj);
        obj.AddComponent<LoadingScreenUI>();
    }

    private void Awake()
    {
        SetupUI();
    }

    private void Start()
    {
        StartCoroutine(PlayIntroSequence());
    }

    private void SetupUI()
    {
        uiDocument = gameObject.AddComponent<UIDocument>();
        uiDocument.sortingOrder = 9999; // Topmost UI layer

        uiDocument.visualTreeAsset = ScriptableObject.CreateInstance<VisualTreeAsset>();
        UIDocument existingDoc = FindObjectsByType<UIDocument>(FindObjectsInactive.Exclude)
                                     .FirstOrDefault(d => d.panelSettings != null);
        if (existingDoc != null)
        {
            uiDocument.panelSettings = existingDoc.panelSettings;
        }

        uiDocument.enabled = false;
        uiDocument.enabled = true;

        if (uiDocument.rootVisualElement == null)
        {
            Debug.LogError("LoadingScreenUI: rootVisualElement is null!");
            Destroy(gameObject);
            return;
        }

        rootContainer = new VisualElement();
        rootContainer.style.flexGrow = 1;
        rootContainer.style.position = Position.Absolute;
        rootContainer.style.left = 0;
        rootContainer.style.right = 0;
        rootContainer.style.top = 0;
        rootContainer.style.bottom = 0;
        rootContainer.style.backgroundColor = LogoDark;
        rootContainer.style.justifyContent = Justify.Center;
        rootContainer.style.alignItems = Align.Center;
        rootContainer.style.opacity = 1f;

        // Interactive Tap / Key to Skip
        rootContainer.RegisterCallback<PointerDownEvent>(_ => Dismiss());
        rootContainer.focusable = true;
        rootContainer.Focus();
        rootContainer.RegisterCallback<KeyDownEvent>(_ => Dismiss());

        // High-resolution portrait render texture (1080x1920)
        renderTexture = new RenderTexture(1080, 1920, 0, RenderTextureFormat.ARGB32);
        renderTexture.filterMode = FilterMode.Bilinear;
        renderTexture.Create();

        videoElement = new VisualElement();
        videoElement.style.width = Length.Percent(100);
        videoElement.style.height = Length.Percent(100);
        videoElement.style.backgroundImage = Background.FromRenderTexture(renderTexture);
        videoElement.style.backgroundSize = new BackgroundSize(BackgroundSizeType.Cover);
        videoElement.style.backgroundPositionX = new BackgroundPosition(BackgroundPositionKeyword.Center);
        videoElement.style.backgroundPositionY = new BackgroundPosition(BackgroundPositionKeyword.Center);
        videoElement.style.opacity = 0f;

        rootContainer.Add(videoElement);
        uiDocument.rootVisualElement.Add(rootContainer);

        // VideoPlayer component
        videoPlayer = gameObject.AddComponent<VideoPlayer>();
        videoPlayer.playOnAwake = false;
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.targetTexture = renderTexture;
        videoPlayer.isLooping = false;
        videoPlayer.audioOutputMode = VideoAudioOutputMode.None;
    }

    private IEnumerator PlayIntroSequence()
    {
        VideoClip clip = Resources.Load<VideoClip>("StudioLogo");

        if (clip != null)
        {
            videoPlayer.clip = clip;
            videoPlayer.Prepare();
            while (!videoPlayer.isPrepared)
            {
                if (isDismissing) yield break;
                yield return null;
            }

            videoPlayer.Play();
            videoElement.style.opacity = 1f;

            // Start the musical intro jingle and duck gameplay BGM until the loading screen finishes
            AudioManager.Instance?.BeginIntroSequence();

            // ── Exact SVG-Locked Audio & Haptic Timeline (5.0s = 150 frames @ 30fps) ──
            // Driven by videoPlayer.time (with unscaledDeltaTime fallback) for zero frame drift:
            //   0.40s (8%  | Frame 12): Cartoon CV swoops in (sfx_intro_whoosh)
            //   0.80s (16% | Frame 24): 5 dossier lines draw (sfx_pencil_sketch)
            //   1.20s (24% | Frame 36): SVG Clock Tick 1 -> 5°  (sfx_clock_tick + LightImpact)
            //   1.70s (34% | Frame 51): SVG Clock Tick 2 -> 10° (sfx_clock_tick + LightImpact)
            //   2.20s (44% | Frame 66): SVG Clock Tick 3 -> 15° (sfx_clock_tick + LightImpact)
            //   2.70s (54% | Frame 81): SVG Clock Tick 4 -> 20° Lock + "HIRED!" Stamp Slam (sfx_stamp_slam + HeavyImpact)
            bool playedWhoosh = false;
            bool playedSketch = false;
            bool[] playedTick = new bool[4];
            float[] svgTickTimes = { 1.20f, 1.70f, 2.20f, 2.70f };
            bool playedStampSlam = false;

            float elapsed = 0f;
            float totalLength = (float)clip.length;
            if (totalLength <= 0f) totalLength = 5.0f;

            while (elapsed < totalLength)
            {
                if (isDismissing) yield break;
                elapsed += Time.unscaledDeltaTime;

                float currentVideoTime = (videoPlayer != null && videoPlayer.isPlaying && videoPlayer.time > 0.001)
                    ? (float)videoPlayer.time
                    : elapsed;

                if (!playedWhoosh && currentVideoTime >= 0.40f)
                {
                    playedWhoosh = true;
                    AudioManager.Instance?.PlaySFX("sfx_intro_whoosh", 0.90f);
                }

                if (!playedSketch && currentVideoTime >= 0.80f)
                {
                    playedSketch = true;
                    AudioManager.Instance?.PlaySFX("sfx_pencil_sketch", 0.82f);
                }

                for (int i = 0; i < svgTickTimes.Length; i++)
                {
                    if (!playedTick[i] && currentVideoTime >= svgTickTimes[i])
                    {
                        playedTick[i] = true;
                        AudioManager.Instance?.PlayClockTick(i == 3 ? 0.96f : 0.90f);
                        if (i < 3)
                        {
                            HapticManager.LightImpact();
                        }
                    }
                }

                if (!playedStampSlam && currentVideoTime >= 2.70f)
                {
                    playedStampSlam = true;
                    AudioManager.Instance?.PlaySFX("sfx_stamp_slam", 1.0f);
                    HapticManager.HeavyImpact();
                }

                yield return null;
            }

            // Smooth fade out at the end
            yield return StartCoroutine(FadeOutAndDestroy(0.35f));
        }
        else
        {
            AudioManager.Instance?.BeginIntroSequence();
            yield return StartCoroutine(FallbackStaticSequence());
        }
    }

    private IEnumerator FallbackStaticSequence()
    {
        Texture2D logoTex = Resources.Load<Texture2D>("StudioLogo");
        Texture2D badgeTex = Resources.Load<Texture2D>("MadeWithUnity");

        if (logoTex != null)
        {
            fallbackLogo = new VisualElement();
            fallbackLogo.style.width = Length.Percent(74);
            fallbackLogo.style.maxWidth = 440;
            fallbackLogo.style.aspectRatio = 934f / 1080f;
            fallbackLogo.style.backgroundImage = Background.FromTexture2D(logoTex);
            fallbackLogo.style.backgroundSize = new BackgroundSize(BackgroundSizeType.Contain);
            fallbackLogo.style.backgroundRepeat = new BackgroundRepeat(Repeat.NoRepeat, Repeat.NoRepeat);
            fallbackLogo.style.opacity = 0f;
            rootContainer.Add(fallbackLogo);
        }

        if (badgeTex != null)
        {
            fallbackBadge = new VisualElement();
            fallbackBadge.style.width = 184;
            fallbackBadge.style.height = 56;
            fallbackBadge.style.backgroundImage = Background.FromTexture2D(badgeTex);
            fallbackBadge.style.backgroundSize = new BackgroundSize(BackgroundSizeType.Contain);
            fallbackBadge.style.backgroundRepeat = new BackgroundRepeat(Repeat.NoRepeat, Repeat.NoRepeat);
            fallbackBadge.style.opacity = 0f;
            fallbackBadge.style.position = Position.Absolute;
            fallbackBadge.style.bottom = 60;
            rootContainer.Add(fallbackBadge);
        }

        float elapsed = 0f;
        float fadeIn = 0.45f;
        while (elapsed < fadeIn)
        {
            if (isDismissing) yield break;
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / fadeIn);
            if (fallbackLogo != null) fallbackLogo.style.opacity = t;
            if (fallbackBadge != null) fallbackBadge.style.opacity = t;
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < 2.0f)
        {
            if (isDismissing) yield break;
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        yield return StartCoroutine(FadeOutAndDestroy(0.35f));
    }

    public void Dismiss()
    {
        if (isDismissing) return;
        isDismissing = true;
        StartCoroutine(FadeOutAndDestroy(0.18f));
    }

    private IEnumerator FadeOutAndDestroy(float duration)
    {
        isDismissing = true;
        AudioManager.Instance?.EndIntroSequence(0.6f);

        float elapsed = 0f;
        float startOpacity = rootContainer != null ? rootContainer.style.opacity.value : 1f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            if (rootContainer != null)
                rootContainer.style.opacity = Mathf.Lerp(startOpacity, 0f, t);
            yield return null;
        }

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (videoPlayer != null)
        {
            videoPlayer.Stop();
            Destroy(videoPlayer);
            videoPlayer = null;
        }
        if (renderTexture != null)
        {
            renderTexture.Release();
            Destroy(renderTexture);
            renderTexture = null;
        }
        if (videoElement != null)
        {
            videoElement.style.backgroundImage = default;
            videoElement.RemoveFromHierarchy();
            videoElement = null;
        }
        if (fallbackLogo != null)
        {
            fallbackLogo.style.backgroundImage = default;
            fallbackLogo.RemoveFromHierarchy();
            fallbackLogo = null;
        }
        if (fallbackBadge != null)
        {
            fallbackBadge.style.backgroundImage = default;
            fallbackBadge.RemoveFromHierarchy();
            fallbackBadge = null;
        }
        if (rootContainer != null)
        {
            rootContainer.RemoveFromHierarchy();
            rootContainer = null;
        }
    }
}

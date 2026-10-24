using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Video;
using System.Collections;
using System.Linq;

/// <summary>
/// Plays the two-stage intro sequence:
///   Phase 1 — 24 Hour Studios Logo (StudioLogo.mp4):
///              - Sketching SFX (sfx_pencil_sketch) plays during the initial drawing phase (0 - 1.35s)
///              - Crisp mechanical clock ticks play at 1.35s, 1.80s, 2.25s, 2.70s
///   Transition — Smooth cut to dark slate background (#07090E) matching marketing logo
///   Phase 2 — Dear Hiring Manager Cartoon CV (CartoonCV.mp4):
///              - Intro whoosh SFX (sfx_intro_whoosh) at start
///              - HIRED! stamp slam SFX (sfx_stamp_slam) + heavy haptic impact timed to stamp impact (~2.05s)
///              - Holds on the complete marketing icon visual
///   Phase 3 — Smooth fade to game dashboard → GameObject self-destructs
///
/// Tap or any keypress skips immediately.
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

    // Solid studio crimson red background (#E11D48) for Phase 1
    private static readonly Color StudioRed = new Color(0.882f, 0.114f, 0.282f, 1f);
    // Dark Slate Blue (#07090E) for Phase 2 (matches CartoonCV & marketing logo background)
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
        rootContainer.style.backgroundColor = StudioRed;
        rootContainer.style.justifyContent = Justify.Center;
        rootContainer.style.alignItems = Align.Center;
        rootContainer.style.opacity = 1f;

        // Interactive Tap / Key to Skip
        rootContainer.RegisterCallback<PointerDownEvent>(_ => Dismiss());
        rootContainer.focusable = true;
        rootContainer.Focus();
        rootContainer.RegisterCallback<KeyDownEvent>(_ => Dismiss());

        // Prepare video element inside rootContainer from the start
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
        VideoClip studioClip = Resources.Load<VideoClip>("StudioLogo");
        VideoClip cvClip = Resources.Load<VideoClip>("CartoonCV");

        // ── Phase 1: Studio Logo ──────────────────────────────────────────
        if (studioClip != null)
        {
            yield return StartCoroutine(PlayVideoClipRoutine(studioClip, onStart: () =>
            {
                // Play sketching SFX during the drawing phase (0 - 1.35s)
                StartCoroutine(PlaySketchSFX(1.35f));
                // Play clock ticks starting at 1.35s
                StartCoroutine(SynchronizedTicksSFX());
            }));
        }
        else
        {
            yield return StartCoroutine(FallbackStaticSequence());
        }

        if (isDismissing) yield break;

        // ── Phase 2: Cartoon CV Animation ──────────────────────────────────
        if (cvClip != null)
        {
            // Transition background to dark slate blue matching marketing icon
            rootContainer.style.backgroundColor = LogoDark;
            videoElement.style.opacity = 0f;

            // Brief clean breathing room between clips
            yield return new WaitForSecondsRealtime(0.1f);
            if (isDismissing) yield break;

            yield return StartCoroutine(PlayVideoClipRoutine(cvClip, onStart: () =>
            {
                // Rising whoosh sound when CV swoops in
                AudioManager.Instance?.PlaySFX("sfx_intro_whoosh", 0.85f);
                // Heavy stamp slam THWACK when HIRED! stamp lands at ~2.05s
                StartCoroutine(DelayedSFX("sfx_stamp_slam", 2.05f, true));
            }));
        }

        if (isDismissing) yield break;

        // ── Phase 3: Fade out to game ─────────────────────────────────────
        yield return StartCoroutine(FadeOutAndDestroy(0.35f));
    }

    private IEnumerator PlayVideoClipRoutine(VideoClip clip, System.Action onStart = null)
    {
        if (clip == null || videoPlayer == null) yield break;

        videoElement.style.opacity = 0f;
        videoPlayer.Stop();
        videoPlayer.clip = clip;

        videoPlayer.Prepare();
        while (!videoPlayer.isPrepared)
        {
            if (isDismissing) yield break;
            yield return null;
        }

        videoPlayer.Play();
        videoElement.style.opacity = 1f;

        onStart?.Invoke();

        float elapsed = 0f;
        float totalLength = (float)clip.length;
        if (totalLength <= 0f) totalLength = 4.0f;

        while (elapsed < totalLength)
        {
            if (isDismissing) yield break;
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
    }

    private IEnumerator PlaySketchSFX(float duration)
    {
        AudioManager.Instance?.PlaySFX("sfx_pencil_sketch", 0.75f);
        yield return new WaitForSecondsRealtime(duration);
    }

    private IEnumerator DelayedSFX(string clipName, float delay, bool triggerHaptic = false)
    {
        yield return new WaitForSecondsRealtime(delay);
        if (!isDismissing)
        {
            AudioManager.Instance?.PlaySFX(clipName, 0.95f);
            if (triggerHaptic)
            {
                HapticManager.HeavyImpact();
            }
        }
    }

    private IEnumerator SynchronizedTicksSFX()
    {
        float[] tickTimes = { 1.35f, 1.80f, 2.25f, 2.70f };
        float elapsed = 0f;
        int tickIdx = 0;

        while (tickIdx < tickTimes.Length)
        {
            if (isDismissing) yield break;
            elapsed += Time.unscaledDeltaTime;
            if (elapsed >= tickTimes[tickIdx])
            {
                AudioManager.Instance?.PlayClick();
                tickIdx++;
            }
            yield return null;
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

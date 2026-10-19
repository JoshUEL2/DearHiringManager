using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Video;
using System.Collections;
using System.Linq;

/// <summary>
/// Plays the full game intro sequence then destroys itself.
/// 
/// Sequence:
///   Phase 1 — Studio Logo (StudioLogo.mp4)
///              + sfx_pencil_sketch.wav looping during playback
///              + 4 synchronized clock-tick clicks from SynchronizedTicksSFX
///   Phase 2 — CartoonCV animation (CartoonCV.mp4)
///              + sfx_intro_whoosh.wav at start
///              + sfx_stamp_slam.wav timed to stamp impact (~42% through anim)
///   Phase 3 — Fade to black → Destroy
///
/// Tap / any key skips at any point.
/// </summary>
public class LoadingScreenUI : MonoBehaviour
{
    // ── UI references ──────────────────────────────────────────────────────
    private UIDocument     uiDocument;
    private VisualElement  rootContainer;
    private VisualElement  videoElement;
    private VisualElement  fallbackLogo;
    private VisualElement  fallbackBadge;

    // ── Video ──────────────────────────────────────────────────────────────
    private VideoPlayer    videoPlayer;
    private RenderTexture  renderTexture;

    // ── State ──────────────────────────────────────────────────────────────
    private bool isDismissing = false;

    // Solid studio crimson red background (#E11D48) — kept as instant cover
    private static readonly Color StudioRed = new Color(0.882f, 0.114f, 0.282f, 1f);

    // ── Initialization ─────────────────────────────────────────────────────
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

    // ── UI Setup ───────────────────────────────────────────────────────────
    private void SetupUI()
    {
        uiDocument = gameObject.AddComponent<UIDocument>();
        uiDocument.sortingOrder = 9999;

        uiDocument.visualTreeAsset = ScriptableObject.CreateInstance<VisualTreeAsset>();
        UIDocument existingDoc = FindObjectsByType<UIDocument>(FindObjectsInactive.Exclude)
                                     .FirstOrDefault(d => d.panelSettings != null);
        if (existingDoc != null)
            uiDocument.panelSettings = existingDoc.panelSettings;

        uiDocument.enabled = false;
        uiDocument.enabled = true;

        if (uiDocument.rootVisualElement == null)
        {
            Debug.LogError("LoadingScreenUI: rootVisualElement is null!");
            Destroy(gameObject);
            return;
        }

        // Root container — covers full screen, crimson red immediately
        rootContainer = new VisualElement();
        rootContainer.style.flexGrow            = 1;
        rootContainer.style.position            = Position.Absolute;
        rootContainer.style.left                = 0;
        rootContainer.style.right               = 0;
        rootContainer.style.top                 = 0;
        rootContainer.style.bottom              = 0;
        rootContainer.style.backgroundColor     = StudioRed;
        rootContainer.style.justifyContent      = Justify.Center;
        rootContainer.style.alignItems          = Align.Center;
        rootContainer.style.opacity             = 1f;

        // Tap / keyboard to skip
        rootContainer.RegisterCallback<PointerDownEvent>(_ => Dismiss());
        rootContainer.focusable = true;
        rootContainer.Focus();
        rootContainer.RegisterCallback<KeyDownEvent>(_ => Dismiss());

        uiDocument.rootVisualElement.Add(rootContainer);
    }

    // ── Main Intro Coroutine ───────────────────────────────────────────────
    private IEnumerator PlayIntroSequence()
    {
        // ── Phase 1: Studio Logo ──────────────────────────────────────────
        VideoClip studioClip = Resources.Load<VideoClip>("StudioLogo");
        if (studioClip != null)
        {
            yield return StartCoroutine(PlayVideoPhase(studioClip, onStart: () =>
            {
                // Pencil sketch SFX loops during studio logo
                StartCoroutine(LoopSFXDuring("sfx_pencil_sketch", (float)studioClip.length));
                // Synchronized mechanical clock ticks
                StartCoroutine(SynchronizedTicksSFX());
            }));
        }
        else
        {
            yield return StartCoroutine(FallbackStaticSequence());
        }

        if (isDismissing) yield break;

        // Brief black frame between clips
        rootContainer.style.backgroundColor = Color.black;
        videoElement?.RemoveFromHierarchy();
        yield return new WaitForSecondsRealtime(0.12f);

        // ── Phase 2: CartoonCV Animation ──────────────────────────────────
        VideoClip cvClip = Resources.Load<VideoClip>("CartoonCV");
        if (cvClip != null)
        {
            yield return StartCoroutine(PlayVideoPhase(cvClip, onStart: () =>
            {
                // Rising whoosh at start of CV anim
                AudioManager.Instance?.PlaySFX("sfx_intro_whoosh");
                // Stamp slam timed to ~42% through the animation (≈ 2.1s in 5s clip)
                StartCoroutine(DelayedSFX("sfx_stamp_slam", (float)cvClip.length * 0.42f));
            }));
        }
        // If CartoonCV.mp4 not found, skip silently

        if (isDismissing) yield break;

        // ── Phase 3: Fade out ─────────────────────────────────────────────
        yield return StartCoroutine(FadeOutAndDestroy(0.35f));
    }

    // ── Utility: Play a VideoClip full-screen ─────────────────────────────
    private IEnumerator PlayVideoPhase(VideoClip clip, System.Action onStart = null)
    {
        // Reuse or create render texture
        if (renderTexture == null)
        {
            renderTexture = new RenderTexture(1080, 1920, 0, RenderTextureFormat.ARGB32);
            renderTexture.filterMode = FilterMode.Bilinear;
            renderTexture.Create();
        }

        // Reuse or create video player
        if (videoPlayer == null)
        {
            videoPlayer = gameObject.AddComponent<VideoPlayer>();
            videoPlayer.playOnAwake    = false;
            videoPlayer.renderMode     = VideoRenderMode.RenderTexture;
            videoPlayer.targetTexture  = renderTexture;
            videoPlayer.isLooping      = false;
            videoPlayer.audioOutputMode = VideoAudioOutputMode.None;
        }

        videoPlayer.clip = clip;
        videoPlayer.Stop();

        // Reuse or create video display element
        if (videoElement == null)
        {
            videoElement = new VisualElement();
            videoElement.style.width              = Length.Percent(100);
            videoElement.style.height             = Length.Percent(100);
            videoElement.style.backgroundImage    = Background.FromRenderTexture(renderTexture);
            videoElement.style.backgroundSize     = new BackgroundSize(BackgroundSizeType.Cover);
            videoElement.style.backgroundPositionX = new BackgroundPosition(BackgroundPositionKeyword.Center);
            videoElement.style.backgroundPositionY = new BackgroundPosition(BackgroundPositionKeyword.Center);
            rootContainer.Add(videoElement);
        }
        else
        {
            videoElement.style.backgroundImage = Background.FromRenderTexture(renderTexture);
        }

        videoElement.style.opacity = 0f;

        videoPlayer.Prepare();
        while (!videoPlayer.isPrepared)
        {
            if (isDismissing) yield break;
            yield return null;
        }

        videoPlayer.Play();
        videoElement.style.opacity = 1f;

        onStart?.Invoke();

        float elapsed     = 0f;
        float totalLength = (float)clip.length;
        if (totalLength <= 0f) totalLength = 4f;

        while (elapsed < totalLength)
        {
            if (isDismissing) yield break;
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
    }

    // ── Utility: Loop an SFX AudioClip for a given duration ──────────────
    private IEnumerator LoopSFXDuring(string clipName, float duration)
    {
        AudioClip clip = Resources.Load<AudioClip>($"Audio/SFX/{clipName}");
        if (clip == null) yield break;

        float elapsed = 0f;
        while (elapsed < duration && !isDismissing)
        {
            AudioManager.Instance?.PlaySFX(clipName);
            yield return new WaitForSecondsRealtime(clip.length);
            elapsed += clip.length;
        }
    }

    // ── Utility: Play SFX after a delay ──────────────────────────────────
    private IEnumerator DelayedSFX(string clipName, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        if (!isDismissing)
            AudioManager.Instance?.PlaySFX(clipName);
    }

    // ── Synchronized clock ticks (matching StudioLogo.mp4 keyframes) ──────
    private IEnumerator SynchronizedTicksSFX()
    {
        float[] tickTimes = { 1.35f, 1.80f, 2.25f, 2.70f };
        float elapsed     = 0f;
        int   tickIdx     = 0;

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

    // ── Fallback static logo (if StudioLogo.mp4 missing) ─────────────────
    private IEnumerator FallbackStaticSequence()
    {
        Texture2D logoTex  = Resources.Load<Texture2D>("StudioLogo");
        Texture2D badgeTex = Resources.Load<Texture2D>("MadeWithUnity");

        if (logoTex != null)
        {
            fallbackLogo = new VisualElement();
            fallbackLogo.style.width            = Length.Percent(74);
            fallbackLogo.style.maxWidth         = 440;
            fallbackLogo.style.aspectRatio      = 934f / 1080f;
            fallbackLogo.style.backgroundImage  = Background.FromTexture2D(logoTex);
            fallbackLogo.style.backgroundSize   = new BackgroundSize(BackgroundSizeType.Contain);
            fallbackLogo.style.backgroundRepeat = new BackgroundRepeat(Repeat.NoRepeat, Repeat.NoRepeat);
            fallbackLogo.style.opacity          = 0f;
            rootContainer.Add(fallbackLogo);
        }

        if (badgeTex != null)
        {
            fallbackBadge = new VisualElement();
            fallbackBadge.style.width            = 184;
            fallbackBadge.style.height           = 56;
            fallbackBadge.style.backgroundImage  = Background.FromTexture2D(badgeTex);
            fallbackBadge.style.backgroundSize   = new BackgroundSize(BackgroundSizeType.Contain);
            fallbackBadge.style.backgroundRepeat = new BackgroundRepeat(Repeat.NoRepeat, Repeat.NoRepeat);
            fallbackBadge.style.opacity          = 0f;
            fallbackBadge.style.position         = Position.Absolute;
            fallbackBadge.style.bottom           = 60;
            rootContainer.Add(fallbackBadge);
        }

        float elapsed = 0f;
        float fadeIn  = 0.45f;
        while (elapsed < fadeIn)
        {
            if (isDismissing) yield break;
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / fadeIn);
            if (fallbackLogo  != null) fallbackLogo.style.opacity  = t;
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

    // ── Dismiss (tap/key skip) ────────────────────────────────────────────
    public void Dismiss()
    {
        if (isDismissing) return;
        isDismissing = true;
        StartCoroutine(FadeOutAndDestroy(0.18f));
    }

    // ── Fade + Destroy ────────────────────────────────────────────────────
    private IEnumerator FadeOutAndDestroy(float duration)
    {
        isDismissing = true;
        float elapsed     = 0f;
        float startOpacity = rootContainer?.style.opacity.value ?? 1f;

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

    // ── Cleanup ───────────────────────────────────────────────────────────
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
            videoElement.style.backgroundImage = Background.none;
            videoElement.RemoveFromHierarchy();
            videoElement = null;
        }
        if (fallbackLogo != null)
        {
            fallbackLogo.style.backgroundImage = Background.none;
            fallbackLogo.RemoveFromHierarchy();
            fallbackLogo = null;
        }
        if (fallbackBadge != null)
        {
            fallbackBadge.style.backgroundImage = Background.none;
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


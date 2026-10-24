using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Video;
using System.Collections;
using System.Linq;

/// <summary>
/// Plays the unified master cinematic intro sequence:
///   1. 0.0s – 4.8s: Simultaneous composition featuring:
///                   - Cartoon CV Hero Logo (top/center) swoops in, details draw, 5-star rating pops,
///                     and bold red "HIRED!" stamp slams on top with impact burst lines.
///                   - 24 Hour Studios emblem (smaller below) ticking mechanically every 0.6s.
///                   - Hero lock holds both logos together in golden-ratio balance.
///   2. Synchronized SFX & haptics:
///                   - Continuous clock ticks (0.0s, 0.6s, 1.2s, 1.8s, 2.4s, 3.0s, 3.6s, 4.2s).
///                   - Swoop whoosh at 0.35s.
///                   - Rubber stamp slam THWACK + heavy haptic impact at 1.85s.
///   3. Tap or any keypress skips immediately.
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

            // ── Synchronized Audio Timeline ──
            // 0.0s – 4.2s: Continuous mechanical clock ticks every 0.6s
            StartCoroutine(SynchronizedTicksSFX());

            // 0.35s: Swoop whoosh when CV flies into place
            StartCoroutine(DelayedSFX("sfx_intro_whoosh", 0.35f));

            // 1.85s: Rubber stamp slam THWACK + heavy haptic impact
            StartCoroutine(DelayedSFX("sfx_stamp_slam", 1.85f, true));

            float elapsed = 0f;
            float totalLength = (float)clip.length;
            if (totalLength <= 0f) totalLength = 6.0f;

            while (elapsed < totalLength)
            {
                if (isDismissing) yield break;
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            // Smooth fade out at the end
            yield return StartCoroutine(FadeOutAndDestroy(0.35f));
        }
        else
        {
            yield return StartCoroutine(FallbackStaticSequence());
        }
    }

    private IEnumerator PlaySketchSFX(float duration)
    {
        AudioManager.Instance?.PlaySFX("sfx_pencil_sketch", 0.80f);
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
        float[] tickTimes = { 0.0f, 0.6f, 1.2f, 1.8f, 2.4f, 3.0f, 3.6f, 4.2f };
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

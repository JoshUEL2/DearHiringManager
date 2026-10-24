using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Video;
using System.Collections;
using System.Linq;

/// <summary>
/// Plays the unified master cinematic intro sequence:
///   1. 0.0s – 1.4s: 24 Hour Studios logo original pencil sketch in crisp white on dark slate.
///                   - sfx_pencil_sketch plays during drawing.
///   2. 1.5s – 2.5s: Mechanical clock hand rotation.
///                   - 4 crisp synchronized clock ticks click.
///   3. 2.7s – 3.1s: Dear Hiring Manager Cartoon CV swoops & slams down directly on top of the logo.
///                   - sfx_intro_whoosh plays on swoop.
///                   - Candidate details draw out and 5 stars pop into place.
///   4. 3.6s – 3.95s: Red "HIRED!" stamp drops and slams onto the CV.
///                   - sfx_stamp_slam THWACK + Heavy Haptic impact.
///                   - Impact burst lines radiate outward.
///   5. 3.95s – 5.6s: Complete marketing logo document sits proud in center.
///   6. 5.6s – 6.0s: Smooth fade to black → Game Dashboard loads!
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
            // 0.0s – 1.35s: Sketching SFX as studio logo is drawn
            StartCoroutine(PlaySketchSFX(1.35f));

            // 1.5s – 2.5s: 4 mechanical clock ticks
            StartCoroutine(SynchronizedTicksSFX());

            // 2.7s: Swoop whoosh when CV slams on top
            StartCoroutine(DelayedSFX("sfx_intro_whoosh", 2.70f));

            // 3.9s: Rubber stamp slam THWACK + haptic impact
            StartCoroutine(DelayedSFX("sfx_stamp_slam", 3.90f, true));

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
        float[] tickTimes = { 1.55f, 1.85f, 2.15f, 2.45f };
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

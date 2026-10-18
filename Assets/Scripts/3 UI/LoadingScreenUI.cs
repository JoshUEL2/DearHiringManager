using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Video;
using System.Collections;
using System.Linq;

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

    // Solid studio crimson red background (#E11D48)
    private static readonly Color StudioRed = new Color(0.882f, 0.114f, 0.282f, 1f);

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
        UIDocument existingDoc = FindObjectsByType<UIDocument>(FindObjectsInactive.Exclude).FirstOrDefault(d => d.panelSettings != null);
        if (existingDoc != null)
        {
            uiDocument.panelSettings = existingDoc.panelSettings;
        }

        uiDocument.enabled = false;
        uiDocument.enabled = true;

        if (uiDocument.rootVisualElement != null)
        {
            // Root Container - starts solid red immediately on Awake to eliminate any scene-loading flicker
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

            // Interactive Tap to Skip
            rootContainer.RegisterCallback<PointerDownEvent>(evt => Dismiss());
            rootContainer.focusable = true;
            rootContainer.Focus();
            rootContainer.RegisterCallback<KeyDownEvent>(evt => Dismiss());

            uiDocument.rootVisualElement.Add(rootContainer);
        }
        else
        {
            Debug.LogError("LoadingScreenUI: rootVisualElement is null!");
            Destroy(gameObject);
        }
    }

    private IEnumerator PlayIntroSequence()
    {
        VideoClip clip = Resources.Load<VideoClip>("StudioLogo");

        if (clip != null)
        {
            // Create portrait high-resolution RenderTexture matching video aspect ratio
            renderTexture = new RenderTexture(1080, 1920, 0, RenderTextureFormat.ARGB32);
            renderTexture.filterMode = FilterMode.Bilinear;
            renderTexture.Create();

            videoPlayer = gameObject.AddComponent<VideoPlayer>();
            videoPlayer.playOnAwake = false;
            videoPlayer.clip = clip;
            videoPlayer.renderMode = VideoRenderMode.RenderTexture;
            videoPlayer.targetTexture = renderTexture;
            videoPlayer.isLooping = false;
            videoPlayer.audioOutputMode = VideoAudioOutputMode.None;

            videoElement = new VisualElement();
            videoElement.style.width = Length.Percent(100);
            videoElement.style.height = Length.Percent(100);
            videoElement.style.backgroundImage = Background.FromRenderTexture(renderTexture);
            videoElement.style.backgroundSize = new BackgroundSize(BackgroundSizeType.Cover);
            videoElement.style.backgroundPositionX = new BackgroundPosition(BackgroundPositionKeyword.Center);
            videoElement.style.backgroundPositionY = new BackgroundPosition(BackgroundPositionKeyword.Center);
            videoElement.style.opacity = 0f; // Hidden until video frame 1 is decoded

            rootContainer.Add(videoElement);

            // Wait for video player preparation
            videoPlayer.Prepare();
            while (!videoPlayer.isPrepared)
            {
                if (isDismissing) yield break;
                yield return null;
            }

            videoPlayer.Play();
            videoElement.style.opacity = 1f;

            // Start synchronized mechanical audio ticks for clock hand rotation
            StartCoroutine(SynchronizedTicksSFX());

            // Listen for video finish
            float elapsed = 0f;
            float totalDuration = (float)clip.length;
            if (totalDuration <= 0f) totalDuration = 3.95f;

            while (elapsed < totalDuration)
            {
                if (isDismissing) yield break;
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            // Smooth fade out at end of video
            yield return StartCoroutine(FadeOutAndDestroy(0.3f));
        }
        else
        {
            // Fallback to static StudioLogo.png
            yield return StartCoroutine(FallbackStaticSequence());
        }
    }

    private IEnumerator SynchronizedTicksSFX()
    {
        // 4 crisp mechanical clock ticks matching the clock hand rotation keyframes
        float[] tickTimes = new float[] { 1.35f, 1.80f, 2.25f, 2.70f };
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
        Texture2D logoTexture = Resources.Load<Texture2D>("StudioLogo");
        Texture2D badgeTexture = Resources.Load<Texture2D>("MadeWithUnity");

        if (logoTexture != null)
        {
            fallbackLogo = new VisualElement();
            fallbackLogo.style.width = Length.Percent(74);
            fallbackLogo.style.maxWidth = 440;
            fallbackLogo.style.aspectRatio = 934f / 1080f;
            fallbackLogo.style.backgroundImage = Background.FromTexture2D(logoTexture);
            fallbackLogo.style.backgroundSize = new BackgroundSize(BackgroundSizeType.Contain);
            fallbackLogo.style.backgroundRepeat = new BackgroundRepeat(Repeat.NoRepeat, Repeat.NoRepeat);
            fallbackLogo.style.opacity = 0f;
            rootContainer.Add(fallbackLogo);
        }

        if (badgeTexture != null)
        {
            fallbackBadge = new VisualElement();
            fallbackBadge.style.width = 184;
            fallbackBadge.style.height = 56;
            fallbackBadge.style.backgroundImage = Background.FromTexture2D(badgeTexture);
            fallbackBadge.style.backgroundSize = new BackgroundSize(BackgroundSizeType.Contain);
            fallbackBadge.style.backgroundRepeat = new BackgroundRepeat(Repeat.NoRepeat, Repeat.NoRepeat);
            fallbackBadge.style.opacity = 0f;
            fallbackBadge.style.position = Position.Absolute;
            fallbackBadge.style.bottom = 60;
            rootContainer.Add(fallbackBadge);
        }

        float elapsed = 0f;
        float fadeInDuration = 0.45f;
        while (elapsed < fadeInDuration)
        {
            if (isDismissing) yield break;
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / fadeInDuration);
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
            {
                rootContainer.style.opacity = Mathf.Lerp(startOpacity, 0f, t);
            }
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
            videoElement.style.backgroundImage = null;
            videoElement.RemoveFromHierarchy();
            videoElement = null;
        }
        if (fallbackLogo != null)
        {
            fallbackLogo.style.backgroundImage = null;
            fallbackLogo.RemoveFromHierarchy();
            fallbackLogo = null;
        }
        if (fallbackBadge != null)
        {
            fallbackBadge.style.backgroundImage = null;
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

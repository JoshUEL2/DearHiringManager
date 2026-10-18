using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Linq;

public class LoadingScreenUI : MonoBehaviour
{
    private UIDocument uiDocument;
    private VisualElement rootContainer;
    private VisualElement logoContainer;
    private VisualElement logoElement;
    private VisualElement badgeElement;
    private bool isDismissing = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void Initialize()
    {
        GameObject obj = new GameObject("LoadingScreen");
        DontDestroyOnLoad(obj);
        obj.AddComponent<LoadingScreenUI>();
    }

    private void Awake()
    {
        // Setup immediately on Awake so it covers the screen before the first render frame
        SetupUI();
    }

    private void Start()
    {
        // Start animation sequence on first active frame
        StartCoroutine(IntroSequence());
    }

    private void SetupUI()
    {
        Texture2D bgTexture = Resources.Load<Texture2D>("IntroBackground");
        Texture2D logoTexture = Resources.Load<Texture2D>("StudioLogo");
        Texture2D badgeTexture = Resources.Load<Texture2D>("MadeWithUnity");

        if (logoTexture == null)
        {
            Debug.LogWarning("LoadingScreenUI: Could not load StudioLogo from Resources.");
            Destroy(gameObject);
            return;
        }

        uiDocument = gameObject.AddComponent<UIDocument>();
        uiDocument.sortingOrder = 9999; // Always on top of all other UI

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
            // Root Container - starts 100% opaque to completely hide underlying scene load
            rootContainer = new VisualElement();
            rootContainer.style.flexGrow = 1;
            rootContainer.style.position = Position.Absolute;
            rootContainer.style.left = 0;
            rootContainer.style.right = 0;
            rootContainer.style.top = 0;
            rootContainer.style.bottom = 0;
            rootContainer.style.backgroundColor = new Color(0.04f, 0.01f, 0.02f, 1f);

            if (bgTexture != null)
            {
                rootContainer.style.backgroundImage = Background.FromTexture2D(bgTexture);
                rootContainer.style.backgroundSize = new BackgroundSize(BackgroundSizeType.Cover);
                rootContainer.style.backgroundPositionX = new BackgroundPosition(BackgroundPositionKeyword.Center);
                rootContainer.style.backgroundPositionY = new BackgroundPosition(BackgroundPositionKeyword.Center);
            }

            rootContainer.style.justifyContent = Justify.SpaceBetween;
            rootContainer.style.alignItems = Align.Center;
            rootContainer.style.paddingTop = Length.Percent(14);
            rootContainer.style.paddingBottom = Length.Percent(6);
            rootContainer.style.opacity = 1f;

            // Top spacer
            VisualElement topSpacer = new VisualElement();
            topSpacer.style.height = 10;
            rootContainer.Add(topSpacer);

            // Centered Logo Container
            logoContainer = new VisualElement();
            logoContainer.style.alignItems = Align.Center;
            logoContainer.style.justifyContent = Justify.Center;
            logoContainer.style.width = Length.Percent(74);
            logoContainer.style.maxWidth = 440;
            logoContainer.style.aspectRatio = 934f / 1080f;
            logoContainer.style.opacity = 0f;

            logoElement = new VisualElement();
            logoElement.style.width = Length.Percent(100);
            logoElement.style.height = Length.Percent(100);
            logoElement.style.backgroundImage = Background.FromTexture2D(logoTexture);
            logoElement.style.backgroundSize = new BackgroundSize(BackgroundSizeType.Contain);
            logoElement.style.backgroundRepeat = new BackgroundRepeat(Repeat.NoRepeat, Repeat.NoRepeat);
            logoElement.style.backgroundPositionX = new BackgroundPosition(BackgroundPositionKeyword.Center);
            logoElement.style.backgroundPositionY = new BackgroundPosition(BackgroundPositionKeyword.Center);
            logoContainer.Add(logoElement);
            rootContainer.Add(logoContainer);

            // Bottom Section: Made With Unity Badge
            VisualElement bottomContainer = new VisualElement();
            bottomContainer.style.alignItems = Align.Center;
            bottomContainer.style.justifyContent = Justify.Center;
            bottomContainer.style.width = Length.Percent(100);

            if (badgeTexture != null)
            {
                badgeElement = new VisualElement();
                badgeElement.style.width = 184;
                badgeElement.style.height = 56;
                badgeElement.style.backgroundImage = Background.FromTexture2D(badgeTexture);
                badgeElement.style.backgroundSize = new BackgroundSize(BackgroundSizeType.Contain);
                badgeElement.style.backgroundRepeat = new BackgroundRepeat(Repeat.NoRepeat, Repeat.NoRepeat);
                badgeElement.style.backgroundPositionX = new BackgroundPosition(BackgroundPositionKeyword.Center);
                badgeElement.style.backgroundPositionY = new BackgroundPosition(BackgroundPositionKeyword.Center);
                badgeElement.style.opacity = 0f;
                bottomContainer.Add(badgeElement);
            }

            rootContainer.Add(bottomContainer);

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

    private IEnumerator IntroSequence()
    {
        // Fade in logo and badge (0.45s) with subtle scale
        float elapsed = 0f;
        float fadeInDuration = 0.45f;
        while (elapsed < fadeInDuration)
        {
            if (isDismissing) yield break;
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / fadeInDuration);
            float ease = 1f - (1f - t) * (1f - t); // Ease-out quad

            if (logoContainer != null)
            {
                logoContainer.style.opacity = ease;
                float scale = Mathf.Lerp(0.93f, 1f, ease);
                logoContainer.style.scale = new Scale(new Vector2(scale, scale));
            }
            if (badgeElement != null)
            {
                badgeElement.style.opacity = ease * 0.92f;
            }
            yield return null;
        }

        if (logoContainer != null)
        {
            logoContainer.style.opacity = 1f;
            logoContainer.style.scale = new Scale(Vector2.one);
        }
        if (badgeElement != null)
        {
            badgeElement.style.opacity = 0.92f;
        }

        // Hold display (1.4s)
        elapsed = 0f;
        float holdDuration = 1.4f;
        while (elapsed < holdDuration)
        {
            if (isDismissing) yield break;
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        // Auto dismiss with smooth fade out
        yield return StartCoroutine(FadeOutAndDestroy(0.35f));
    }

    public void Dismiss()
    {
        if (isDismissing) return;
        isDismissing = true;
        StartCoroutine(FadeOutAndDestroy(0.2f));
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
        if (logoElement != null)
        {
            logoElement.style.backgroundImage = null;
            logoElement.RemoveFromHierarchy();
            logoElement = null;
        }
        if (badgeElement != null)
        {
            badgeElement.style.backgroundImage = null;
            badgeElement.RemoveFromHierarchy();
            badgeElement = null;
        }
        if (logoContainer != null)
        {
            logoContainer.RemoveFromHierarchy();
            logoContainer = null;
        }
        if (rootContainer != null)
        {
            rootContainer.style.backgroundImage = null;
            rootContainer.RemoveFromHierarchy();
            rootContainer = null;
        }
    }
}

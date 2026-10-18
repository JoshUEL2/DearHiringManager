using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SafeAreaHelper : MonoBehaviour
{
    public static SafeAreaHelper Instance { get; private set; }

    [Header("Settings")]
    [Tooltip("Extra manual padding to add to top beyond safe area (in UI points).")]
    [SerializeField] private float extraTopPadding = 0f;

    [Tooltip("Extra manual padding to add to bottom beyond safe area (in UI points).")]
    [SerializeField] private float extraBottomPadding = 0f;

    [Header("Registered UIDocuments")]
    [SerializeField] private List<UIDocument> targetDocuments = new List<UIDocument>();

    private class ContainerState
    {
        public bool hasBaseValues;
        public float basePaddingTop;
        public float basePaddingBottom;
        public float basePaddingLeft;
        public float basePaddingRight;
    }

    private class NavState
    {
        public bool hasBaseValue;
        public float baseBottom = 40f;
    }

    private readonly Dictionary<VisualElement, ContainerState> containerStates = new Dictionary<VisualElement, ContainerState>();
    private readonly Dictionary<VisualElement, NavState> navStates = new Dictionary<VisualElement, NavState>();

    private Rect lastSafeArea = Rect.zero;
    private int lastScreenWidth = 0;
    private int lastScreenHeight = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this);
            return;
        }
    }

    private void Start()
    {
        CollectSceneDocuments();
        ApplySafeArea();
    }

    private void Update()
    {
        // Continuously check for changes in orientation, resolution, or safe area (e.g. mobile rotation or editor simulator)
        if (Screen.safeArea != lastSafeArea ||
            Screen.width != lastScreenWidth ||
            Screen.height != lastScreenHeight)
        {
            ApplySafeArea();
        }
    }

    public void RegisterDocument(UIDocument doc)
    {
        if (doc != null && !targetDocuments.Contains(doc))
        {
            targetDocuments.Add(doc);
            ApplySafeAreaToDocument(doc);
        }
    }

    public void CollectSceneDocuments()
    {
        var foundDocs = FindObjectsByType<UIDocument>(FindObjectsInactive.Include);
        foreach (var doc in foundDocs)
        {
            if (doc != null && !targetDocuments.Contains(doc))
            {
                targetDocuments.Add(doc);
            }
        }
    }

    public void ApplySafeArea()
    {
        lastSafeArea = Screen.safeArea;
        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;

        targetDocuments.RemoveAll(d => d == null);

        foreach (var doc in targetDocuments)
        {
            ApplySafeAreaToDocument(doc);
        }
    }

    public void ApplySafeAreaToDocument(UIDocument doc)
    {
        if (doc == null || doc.rootVisualElement == null) return;

        float scale = GetScaleFactor(doc);

        Rect safeArea = Screen.safeArea;
        float topInsetPx = Screen.height - (safeArea.y + safeArea.height);
        float bottomInsetPx = safeArea.y;
        float leftInsetPx = safeArea.x;
        float rightInsetPx = Screen.width - (safeArea.x + safeArea.width);

        // Convert physical screen pixels to UI Toolkit panel units using panel scale factor
        float topPad = Mathf.Max(0f, (topInsetPx / scale) + extraTopPadding);
        float bottomPad = Mathf.Max(0f, (bottomInsetPx / scale) + extraBottomPadding);
        float leftPad = Mathf.Max(0f, leftInsetPx / scale);
        float rightPad = Mathf.Max(0f, rightInsetPx / scale);

        var root = doc.rootVisualElement;

        // Visual containers: screen-container, or the rootVisualElement itself if no container class is used
        var screenContainer = root.Q<VisualElement>(className: "screen-container");
        var bottomNav = root.Q<VisualElement>(className: "bottom-nav-container");

        if (screenContainer != null)
        {
            ApplyToContainer(screenContainer, topPad, bottomPad, leftPad, rightPad);
        }
        else
        {
            ApplyToContainer(root, topPad, bottomPad, leftPad, rightPad);
        }

        if (bottomNav != null)
        {
            ApplyToBottomNav(bottomNav, bottomPad);
        }
    }

    private void ApplyToContainer(VisualElement container, float topPad, float bottomPad, float leftPad, float rightPad)
    {
        if (container == null) return;

        if (!containerStates.TryGetValue(container, out var state))
        {
            state = new ContainerState();
            containerStates[container] = state;
        }

        if (!state.hasBaseValues)
        {
            state.basePaddingTop = container.style.paddingTop.keyword == StyleKeyword.Null
                ? (container.resolvedStyle.paddingTop > 0 ? container.resolvedStyle.paddingTop : 0f)
                : container.style.paddingTop.value.value;

            state.basePaddingBottom = container.style.paddingBottom.keyword == StyleKeyword.Null
                ? (container.resolvedStyle.paddingBottom > 0 ? container.resolvedStyle.paddingBottom : 0f)
                : container.style.paddingBottom.value.value;

            state.basePaddingLeft = container.style.paddingLeft.keyword == StyleKeyword.Null
                ? (container.resolvedStyle.paddingLeft > 0 ? container.resolvedStyle.paddingLeft : 0f)
                : container.style.paddingLeft.value.value;

            state.basePaddingRight = container.style.paddingRight.keyword == StyleKeyword.Null
                ? (container.resolvedStyle.paddingRight > 0 ? container.resolvedStyle.paddingRight : 0f)
                : container.style.paddingRight.value.value;

            if (container.resolvedStyle.height > 0 || state.basePaddingTop > 0 || state.basePaddingBottom > 0)
            {
                state.hasBaseValues = true;
            }
        }

        container.style.paddingTop = state.basePaddingTop + topPad;
        container.style.paddingBottom = state.basePaddingBottom + bottomPad;

        if (leftPad > 0f)
            container.style.paddingLeft = state.basePaddingLeft + leftPad;
        if (rightPad > 0f)
            container.style.paddingRight = state.basePaddingRight + rightPad;
    }

    private void ApplyToBottomNav(VisualElement nav, float bottomPad)
    {
        if (nav == null) return;

        if (!navStates.TryGetValue(nav, out var state))
        {
            state = new NavState();
            navStates[nav] = state;
        }

        if (!state.hasBaseValue)
        {
            state.baseBottom = nav.style.bottom.keyword == StyleKeyword.Null
                ? (nav.resolvedStyle.bottom > 0 ? nav.resolvedStyle.bottom : 40f)
                : nav.style.bottom.value.value;

            if (nav.resolvedStyle.height > 0 || state.baseBottom > 0)
            {
                state.hasBaseValue = true;
            }
        }

        nav.style.bottom = state.baseBottom + bottomPad;
    }

    public static float GetScaleFactor(UIDocument doc)
    {
        if (doc != null && doc.rootVisualElement != null && doc.rootVisualElement.panel != null)
        {
            float ppp = doc.rootVisualElement.panel.scaledPixelsPerPoint;
            if (ppp > 0.001f) return ppp;
        }

        if (doc != null && doc.panelSettings != null && doc.panelSettings.scale > 0.001f)
        {
            return doc.panelSettings.scale;
        }

        return 1.0f;
    }
}

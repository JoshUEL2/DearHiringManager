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
        public bool registeredGeometryCallback;
        public float basePaddingTop;
        public float basePaddingBottom;
        public float basePaddingLeft;
        public float basePaddingRight;
    }

    private class NavState
    {
        public bool hasBaseValue;
        public bool registeredGeometryCallback;
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
            // Do not read resolvedStyle or overwrite inline styles before UI Toolkit layout has resolved.
            // Overwriting container.style.paddingBottom on Frame 0 clobbers UXML inline styles (e.g. padding-bottom: 360px).
            if (float.IsNaN(container.resolvedStyle.height) || container.resolvedStyle.height <= 0f)
            {
                if (!state.registeredGeometryCallback)
                {
                    state.registeredGeometryCallback = true;
                    container.RegisterCallback<GeometryChangedEvent>(_ => ApplySafeArea());
                }
                return;
            }

            state.basePaddingTop = container.resolvedStyle.paddingTop >= 0f ? container.resolvedStyle.paddingTop : 0f;
            state.basePaddingBottom = container.resolvedStyle.paddingBottom >= 0f ? container.resolvedStyle.paddingBottom : 0f;
            if (container.Q<VisualElement>("TerminalCard") != null && state.basePaddingBottom < 310f)
            {
                state.basePaddingBottom = 310f;
            }
            state.basePaddingLeft = container.resolvedStyle.paddingLeft >= 0f ? container.resolvedStyle.paddingLeft : 0f;
            state.basePaddingRight = container.resolvedStyle.paddingRight >= 0f ? container.resolvedStyle.paddingRight : 0f;
            state.hasBaseValues = true;
        }

        if (topPad > 0f)
            container.style.paddingTop = state.basePaddingTop + topPad;
        if (bottomPad > 0f)
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
            if (float.IsNaN(nav.resolvedStyle.height) || nav.resolvedStyle.height <= 0f)
            {
                if (!state.registeredGeometryCallback)
                {
                    state.registeredGeometryCallback = true;
                    nav.RegisterCallback<GeometryChangedEvent>(_ => ApplySafeArea());
                }
                return;
            }

            state.baseBottom = nav.resolvedStyle.bottom > 0f ? nav.resolvedStyle.bottom : 40f;
            state.hasBaseValue = true;
        }

        if (bottomPad > 0f)
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

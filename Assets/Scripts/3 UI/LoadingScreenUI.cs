using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Linq;

/// <summary>
/// High-frame-rate (60–144+ FPS) native UI Toolkit Intro & Loading Screen.
/// Uses a fixed 1080x1920 virtual stage with explicit pixel coordinates matching the master SVG
/// viewBox="0 0 1080 1920", uniformly fitted to any viewport aspect ratio with zero layout distortion.
/// </summary>
public class LoadingScreenUI : MonoBehaviour
{
    /// <summary>
    /// True from SubsystemRegistration (before AudioManager.Awake) until the intro finishes or is skipped.
    /// Prevents gameplay BGM from playing before or during the intro sequence.
    /// </summary>
    public static bool IsIntroPlaying { get; private set; } = true;

    private UIDocument uiDocument;
    private VisualElement rootContainer;
    private VisualElement sceneMaster;

    // Animated Vector Layers (Positioned in exact 1080x1920 SVG pixel space)
    private VisualElement titleElement;
    private VisualElement cvElement;
    private readonly VisualElement[] cvLines = new VisualElement[5];
    private VisualElement impactLinesContainer;
    private VisualElement stampWrapper;
    private VisualElement stampElement;
    private VisualElement gleamClipBox;
    private VisualElement gleamBar;
    private VisualElement attribElement;
    private VisualElement studioBox;
    private VisualElement studioStaticElement;
    private VisualElement studioArmElement;
    private Label loadingStatusLabel;
    private VisualElement loadingBarFill;

    private bool isDismissing = false;

    private static readonly Color LogoDark = new Color(0.020f, 0.031f, 0.067f, 1f);      // #050811
    private static readonly Color LineColor = new Color(0.580f, 0.639f, 0.722f, 1f);     // #94A3B8
    private static readonly Color CyanAccent = new Color(0.220f, 0.741f, 0.973f, 1f);    // #38BDF8
    private static readonly Color EmeraldAccent = new Color(0.204f, 0.827f, 0.600f, 1f); // #34D399
    private static readonly Color TrackDark = new Color(0.059f, 0.090f, 0.165f, 1f);     // #0F172A
    private static readonly Color TrackBorder = new Color(0.200f, 0.255f, 0.333f, 1f);   // #334155
    private static readonly Color SkipTextColor = new Color(0.392f, 0.455f, 0.545f, 1f); // #64748B

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void OnSubsystemRegistration()
    {
        IsIntroPlaying = true;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void Initialize()
    {
        IsIntroPlaying = true;
        GameObject obj = new GameObject("LoadingScreen");
        DontDestroyOnLoad(obj);
        obj.AddComponent<LoadingScreenUI>();
    }

    private void Awake()
    {
        IsIntroPlaying = true;
        SetupUI();
    }

    private void Start()
    {
        StartCoroutine(PlayIntroSequence());
    }

    private void SetupUI()
    {
        uiDocument = gameObject.AddComponent<UIDocument>();
        uiDocument.sortingOrder = 9999;

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
            IsIntroPlaying = false;
            Destroy(gameObject);
            return;
        }

        // Load high-DPI vector textures from Resources/Intro/
        Texture2D texBackdrop = Resources.Load<Texture2D>("Intro/Intro_Backdrop");
        Texture2D texTitle = Resources.Load<Texture2D>("Intro/Intro_Title");
        Texture2D texCVBase = Resources.Load<Texture2D>("Intro/Intro_CV_Base");
        Texture2D texStamp = Resources.Load<Texture2D>("Intro/Intro_Stamp");
        Texture2D texAttrib = Resources.Load<Texture2D>("Intro/Intro_Attribution");
        Texture2D texStudioStatic = Resources.Load<Texture2D>("Intro/Intro_Studio_Static");
        Texture2D texStudioArm = Resources.Load<Texture2D>("Intro/Intro_Studio_Arm");

        rootContainer = new VisualElement();
        rootContainer.style.flexGrow = 1;
        rootContainer.style.position = Position.Absolute;
        rootContainer.style.left = 0;
        rootContainer.style.right = 0;
        rootContainer.style.top = 0;
        rootContainer.style.bottom = 0;
        rootContainer.style.justifyContent = Justify.Center;
        rootContainer.style.alignItems = Align.Center;
        rootContainer.style.overflow = Overflow.Hidden;
        rootContainer.style.backgroundColor = LogoDark;
        if (texBackdrop != null)
        {
            rootContainer.style.backgroundImage = Background.FromTexture2D(texBackdrop);
            rootContainer.style.backgroundSize = new BackgroundSize(BackgroundSizeType.Cover);
            rootContainer.style.backgroundPositionX = new BackgroundPosition(BackgroundPositionKeyword.Center);
            rootContainer.style.backgroundPositionY = new BackgroundPosition(BackgroundPositionKeyword.Center);
        }
        rootContainer.style.opacity = 1f;

        // Interactive Tap / Key to Skip
        rootContainer.RegisterCallback<PointerDownEvent>(_ => Dismiss());
        rootContainer.RegisterCallback<GeometryChangedEvent>(_ => UpdateStageFitScale());
        rootContainer.focusable = true;
        rootContainer.Focus();
        rootContainer.RegisterCallback<KeyDownEvent>(_ => Dismiss());

        // ── Fixed 1080x1920 Virtual Stage (Exact 1:1 SVG Coordinate System) ──
        sceneMaster = new VisualElement();
        sceneMaster.style.width = 1080;
        sceneMaster.style.height = 1920;
        sceneMaster.style.flexShrink = 0;
        sceneMaster.style.transformOrigin = new TransformOrigin(Length.Percent(50), Length.Percent(50), 0);
        sceneMaster.style.opacity = 0f;
        rootContainer.Add(sceneMaster);

        // ── 1. TOP TITLE LOCKUP ("DEAR HIRING MANAGER": left=140, top=50, w=800, h=300) ──
        titleElement = CreateSpriteLayer(texTitle, 140f, 50f, 800f, 300f);
        titleElement.style.transformOrigin = new TransformOrigin(400f, 150f, 0f);
        sceneMaster.Add(titleElement);

        // ── 2. CARTOON CV BASE (left=240, top=329, w=600, h=768; center at 300, 366) ──
        cvElement = CreateSpriteLayer(texCVBase, 240f, 329f, 600f, 768f);
        cvElement.style.transformOrigin = new TransformOrigin(300f, 366f, 0f);
        cvElement.style.opacity = 0f;
        sceneMaster.Add(cvElement);

        // 5 Staggered CV Experience Lines inside cvElement (exact pixel coordinates)
        float[] lineWidthsPx = { 370f, 312f, 348f, 264f, 324f };
        for (int i = 0; i < 5; i++)
        {
            VisualElement line = new VisualElement();
            line.style.position = Position.Absolute;
            line.style.left = 115.2f;
            line.style.top = 252.0f + i * 43.2f;
            line.style.width = lineWidthsPx[i];
            line.style.height = 21.6f;
            line.style.backgroundColor = LineColor;
            line.style.borderTopLeftRadius = 11f;
            line.style.borderTopRightRadius = 11f;
            line.style.borderBottomLeftRadius = 11f;
            line.style.borderBottomRightRadius = 11f;
            line.style.transformOrigin = new TransformOrigin(0f, 10.8f, 0f);
            line.style.scale = new Scale(new Vector3(0f, 1f, 1f));
            line.style.opacity = 0f;
            cvElement.Add(line);
            cvLines[i] = line;
        }

        // Exterior Radial Impact Burst Lines around Stamp (centered at 540, 833)
        impactLinesContainer = new VisualElement();
        impactLinesContainer.style.position = Position.Absolute;
        impactLinesContainer.style.left = 180f;
        impactLinesContainer.style.top = 693f;
        impactLinesContainer.style.width = 720f;
        impactLinesContainer.style.height = 280f;
        impactLinesContainer.style.transformOrigin = new TransformOrigin(360f, 140f, 0f);
        impactLinesContainer.style.opacity = 0f;
        sceneMaster.Add(impactLinesContainer);

        CreateImpactRay(impactLinesContainer, 35f, 45f, -24f);
        CreateImpactRay(impactLinesContainer, 615f, 45f, 24f);
        CreateImpactRay(impactLinesContainer, 25f, 205f, 22f);
        CreateImpactRay(impactLinesContainer, 625f, 205f, -22f);

        // Red "HIRED!" Stamp Wrapper (left=252, top=689, w=576, h=288; centered at 540, 833)
        stampWrapper = new VisualElement();
        stampWrapper.style.position = Position.Absolute;
        stampWrapper.style.left = 252f;
        stampWrapper.style.top = 689f;
        stampWrapper.style.width = 576f;
        stampWrapper.style.height = 288f;
        stampWrapper.style.transformOrigin = new TransformOrigin(288f, 144f, 0f);
        stampWrapper.style.opacity = 0f;
        sceneMaster.Add(stampWrapper);

        stampElement = CreateSpriteLayer(texStamp, 0f, 0f, 576f, 288f);
        stampWrapper.Add(stampElement);

        // Clipped Specular Gleam Box matching inner stamp plate (left=78, top=78, w=420, h=132)
        gleamClipBox = new VisualElement();
        gleamClipBox.style.position = Position.Absolute;
        gleamClipBox.style.left = 78f;
        gleamClipBox.style.top = 78f;
        gleamClipBox.style.width = 420f;
        gleamClipBox.style.height = 132f;
        gleamClipBox.style.borderTopLeftRadius = 21f;
        gleamClipBox.style.borderTopRightRadius = 21f;
        gleamClipBox.style.borderBottomLeftRadius = 21f;
        gleamClipBox.style.borderBottomRightRadius = 21f;
        gleamClipBox.style.overflow = Overflow.Hidden;
        stampWrapper.Add(gleamClipBox);

        gleamBar = new VisualElement();
        gleamBar.style.position = Position.Absolute;
        gleamBar.style.left = -140f;
        gleamBar.style.top = -40f;
        gleamBar.style.width = 110f;
        gleamBar.style.height = 220f;
        gleamBar.style.backgroundColor = new Color(1f, 1f, 1f, 0.50f);
        gleamBar.style.rotate = new Rotate(new Angle(-25f, AngleUnit.Degree));
        gleamBar.style.opacity = 0f;
        gleamClipBox.Add(gleamBar);

        // ── 3. LOWER STUDIO CREDIT: "DEVELOPED BY" + 24 HOUR STUDIOS ──
        attribElement = CreateSpriteLayer(texAttrib, 240f, 1085f, 600f, 70f);
        sceneMaster.Add(attribElement);

        studioBox = new VisualElement();
        studioBox.style.position = Position.Absolute;
        studioBox.style.left = 351.4f;
        studioBox.style.top = 1169.7f;
        studioBox.style.width = 441f;
        studioBox.style.height = 441f;
        sceneMaster.Add(studioBox);

        studioStaticElement = CreateSpriteLayer(texStudioStatic, 0f, 0f, 441f, 441f);
        studioBox.Add(studioStaticElement);

        studioArmElement = CreateSpriteLayer(texStudioArm, 0f, 0f, 441f, 441f);
        // Exact SVG pivot (1277.97, 1454.18) inside viewBox(-100, -100, 2100, 2100) scaled to 441x441 -> (289.37px, 326.38px)
        studioArmElement.style.transformOrigin = new TransformOrigin(289.37f, 326.38f, 0f);
        studioBox.Add(studioArmElement);

        // ── 4. BOTTOM LOADING BAR & STATUS (Y = 1690 .. 1805) ──
        loadingStatusLabel = new Label("PREPARING WORKPLACE SIMULATION...");
        loadingStatusLabel.style.position = Position.Absolute;
        loadingStatusLabel.style.left = 140f;
        loadingStatusLabel.style.top = 1690f;
        loadingStatusLabel.style.width = 800f;
        loadingStatusLabel.style.height = 32f;
        loadingStatusLabel.style.fontSize = 20f;
        loadingStatusLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        loadingStatusLabel.style.color = CyanAccent;
        loadingStatusLabel.style.letterSpacing = 5f;
        loadingStatusLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
        sceneMaster.Add(loadingStatusLabel);

        VisualElement barTrack = new VisualElement();
        barTrack.style.position = Position.Absolute;
        barTrack.style.left = 280f;
        barTrack.style.top = 1732f;
        barTrack.style.width = 520f;
        barTrack.style.height = 22f;
        barTrack.style.backgroundColor = TrackDark;
        barTrack.style.borderTopWidth = 3f;
        barTrack.style.borderBottomWidth = 3f;
        barTrack.style.borderLeftWidth = 3f;
        barTrack.style.borderRightWidth = 3f;
        barTrack.style.borderTopColor = TrackBorder;
        barTrack.style.borderBottomColor = TrackBorder;
        barTrack.style.borderLeftColor = TrackBorder;
        barTrack.style.borderRightColor = TrackBorder;
        barTrack.style.borderTopLeftRadius = 11f;
        barTrack.style.borderTopRightRadius = 11f;
        barTrack.style.borderBottomLeftRadius = 11f;
        barTrack.style.borderBottomRightRadius = 11f;
        barTrack.style.paddingLeft = 3f;
        barTrack.style.paddingRight = 3f;
        barTrack.style.paddingTop = 2f;
        barTrack.style.paddingBottom = 2f;
        sceneMaster.Add(barTrack);

        loadingBarFill = new VisualElement();
        loadingBarFill.style.width = 20f;
        loadingBarFill.style.height = 12f;
        loadingBarFill.style.backgroundColor = CyanAccent;
        loadingBarFill.style.borderTopLeftRadius = 6f;
        loadingBarFill.style.borderTopRightRadius = 6f;
        loadingBarFill.style.borderBottomLeftRadius = 6f;
        loadingBarFill.style.borderBottomRightRadius = 6f;
        barTrack.Add(loadingBarFill);

        Label skipHintLabel = new Label("TAP ANYWHERE TO SKIP");
        skipHintLabel.style.position = Position.Absolute;
        skipHintLabel.style.left = 140f;
        skipHintLabel.style.top = 1768f;
        skipHintLabel.style.width = 800f;
        skipHintLabel.style.height = 28f;
        skipHintLabel.style.fontSize = 16f;
        skipHintLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        skipHintLabel.style.color = SkipTextColor;
        skipHintLabel.style.letterSpacing = 3.5f;
        skipHintLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
        sceneMaster.Add(skipHintLabel);

        uiDocument.rootVisualElement.Add(rootContainer);
    }

    private void UpdateStageFitScale()
    {
        if (rootContainer == null || sceneMaster == null) return;
        float rw = rootContainer.resolvedStyle.width;
        float rh = rootContainer.resolvedStyle.height;
        if (rw > 10f && rh > 10f && !float.IsNaN(rw) && !float.IsNaN(rh))
        {
            float fitScale = Mathf.Min(rw / 1080f, rh / 1920f);
            sceneMaster.style.scale = new Scale(new Vector3(fitScale, fitScale, 1f));
        }
    }

    private static VisualElement CreateSpriteLayer(Texture2D tex, float left, float top, float width, float height)
    {
        VisualElement el = new VisualElement();
        el.style.position = Position.Absolute;
        el.style.left = left;
        el.style.top = top;
        el.style.width = width;
        el.style.height = height;
        if (tex != null)
        {
            el.style.backgroundImage = Background.FromTexture2D(tex);
        }
        return el;
    }

    private static void CreateImpactRay(VisualElement parent, float left, float top, float angleDeg)
    {
        VisualElement ray = new VisualElement();
        ray.style.position = Position.Absolute;
        ray.style.left = left;
        ray.style.top = top;
        ray.style.width = 68f;
        ray.style.height = 9f;
        ray.style.backgroundColor = CyanAccent;
        ray.style.borderTopLeftRadius = 4.5f;
        ray.style.borderTopRightRadius = 4.5f;
        ray.style.borderBottomLeftRadius = 4.5f;
        ray.style.borderBottomRightRadius = 4.5f;
        ray.style.rotate = new Rotate(new Angle(angleDeg, AngleUnit.Degree));
        parent.Add(ray);
    }

    private IEnumerator PlayIntroSequence()
    {
        // Let 1 frame settle after scene load so layout & deltaTime are clean
        yield return null;
        UpdateStageFitScale();

        // Start the musical intro jingle (sfx_intro_jingle) immediately; BGM is already held at 0 volume
        AudioManager.Instance?.BeginIntroSequence();

        bool playedWhoosh = false;
        bool playedSketch = false;
        bool[] playedTick = new bool[4];
        float[] svgTickTimes = { 1.20f, 1.70f, 2.20f, 2.70f };
        bool playedStampSlam = false;

        float elapsed = 0f;
        const float totalDuration = 5.0f;

        while (elapsed < totalDuration)
        {
            if (isDismissing) yield break;

            float dt = Mathf.Min(Time.unscaledDeltaTime, 0.05f);
            elapsed += dt;

            UpdateStageFitScale();

            // ── 1. AUDIO & HAPTIC MILESTONES (Locked to the exact same elapsed clock) ──
            if (!playedWhoosh && elapsed >= 0.40f)
            {
                playedWhoosh = true;
                AudioManager.Instance?.PlaySFX("sfx_intro_whoosh", 0.90f);
            }

            if (!playedSketch && elapsed >= 0.80f)
            {
                playedSketch = true;
                AudioManager.Instance?.PlaySFX("sfx_pencil_sketch", 0.82f);
            }

            for (int i = 0; i < 4; i++)
            {
                if (!playedTick[i] && elapsed >= svgTickTimes[i])
                {
                    playedTick[i] = true;
                    AudioManager.Instance?.PlayClockTick(i == 3 ? 0.96f : 0.90f);
                    if (i < 3)
                    {
                        HapticManager.LightImpact();
                    }
                }
            }

            if (!playedStampSlam && elapsed >= 2.70f)
            {
                playedStampSlam = true;
                AudioManager.Instance?.PlaySFX("sfx_stamp_slam", 1.0f);
                HapticManager.HeavyImpact();
                if (loadingStatusLabel != null)
                {
                    loadingStatusLabel.text = "CANDIDATE HIRED! LAUNCHING...";
                    loadingStatusLabel.style.color = EmeraldAccent;
                }
            }

            // ── 2. NATIVE 60–144 FPS VISUAL KEYFRAME EVALUATION ──
            UpdateVisualKeyframes(elapsed);

            yield return null;
        }

        yield return StartCoroutine(FadeOutAndDestroy(0.35f));
    }

    private void UpdateVisualKeyframes(float t)
    {
        // A. Master Scene Fade (0.00s -> 0.30s fade in, 4.50s -> 5.00s fade out)
        float masterAlpha = 1f;
        if (t < 0.30f) masterAlpha = Mathf.SmoothStep(0f, 1f, t / 0.30f);
        else if (t > 4.50f) masterAlpha = Mathf.SmoothStep(1f, 0f, (t - 4.50f) / 0.50f);
        sceneMaster.style.opacity = masterAlpha;

        // B. Top Title Lockup Drop-In (0.00s -> 0.40s)
        float titleScale = 1f;
        float titleY = 0f;
        if (t < 0.275f)
        {
            float p = EaseOutCubic(t / 0.275f);
            titleScale = Mathf.Lerp(0.82f, 1.04f, p);
            titleY = Mathf.Lerp(-50f, 5f, p);
        }
        else if (t < 0.40f)
        {
            float p = (t - 0.275f) / 0.125f;
            titleScale = Mathf.Lerp(1.04f, 1.0f, p);
            titleY = Mathf.Lerp(5f, 0f, p);
        }
        titleElement.style.scale = new Scale(new Vector3(titleScale, titleScale, 1f));
        titleElement.style.translate = new Translate(0f, titleY, 0f);

        // C. Cartoon CV Swoop-In (0.40s -> 0.80s)
        if (t < 0.40f)
        {
            cvElement.style.opacity = 0f;
        }
        else
        {
            float cvAlpha = Mathf.Clamp01((t - 0.40f) / 0.12f);
            float cvScale = 1f;
            float cvRot = 0f;
            if (t < 0.675f)
            {
                float p = EaseOutCubic((t - 0.40f) / 0.275f);
                cvScale = Mathf.Lerp(2.35f, 1.04f, p);
                cvRot = Mathf.Lerp(-14f, 2f, p);
            }
            else if (t < 0.75f)
            {
                float p = (t - 0.675f) / 0.075f;
                cvScale = Mathf.Lerp(1.04f, 0.98f, p);
                cvRot = Mathf.Lerp(2f, -1f, p);
            }
            else if (t < 0.80f)
            {
                float p = (t - 0.75f) / 0.05f;
                cvScale = Mathf.Lerp(0.98f, 1.0f, p);
                cvRot = Mathf.Lerp(-1f, 0f, p);
            }
            cvElement.style.opacity = cvAlpha;
            cvElement.style.scale = new Scale(new Vector3(cvScale, cvScale, 1f));
            cvElement.style.rotate = new Rotate(new Angle(cvRot, AngleUnit.Degree));
        }

        // D. 5 Staggered CV Dossier Lines (0.80s -> 1.15s)
        for (int i = 0; i < 5; i++)
        {
            float lineStart = 0.80f + i * 0.06f;
            float lineDur = 0.16f;
            if (t < lineStart)
            {
                cvLines[i].style.opacity = 0f;
                cvLines[i].style.scale = new Scale(new Vector3(0f, 1f, 1f));
            }
            else
            {
                float p = EaseOutCubic(Mathf.Clamp01((t - lineStart) / lineDur));
                cvLines[i].style.opacity = Mathf.Lerp(0.25f, 1f, p);
                cvLines[i].style.scale = new Scale(new Vector3(p, 1f, 1f));
            }
        }

        // E. 24 Hour Studios Exact 4-Tick SVG Rotation (1.20s, 1.70s, 2.20s, 2.70s -> 5°, 10°, 15°, 20°)
        float armAngle = EvaluateClockArmAngle(t);
        studioArmElement.style.rotate = new Rotate(new Angle(armAngle, AngleUnit.Degree));

        // F. Red "HIRED!" Stamp Slam (Windup 2.55s, Impact at 2.70s)
        if (t < 2.55f)
        {
            stampWrapper.style.opacity = 0f;
            impactLinesContainer.style.opacity = 0f;
        }
        else
        {
            float stampAlpha = Mathf.Clamp01((t - 2.55f) / 0.08f);
            float stampScale = 1f;
            float stampRot = -10f;

            if (t < 2.70f)
            {
                float p = EaseOutCubic((t - 2.55f) / 0.15f);
                stampScale = Mathf.Lerp(3.1f, 0.92f, p);
                stampRot = Mathf.Lerp(-28f, -10f, p);
            }
            else if (t < 2.80f)
            {
                float p = (t - 2.70f) / 0.10f;
                stampScale = Mathf.Lerp(0.92f, 1.04f, p);
            }
            else if (t < 2.90f)
            {
                float p = (t - 2.80f) / 0.10f;
                stampScale = Mathf.Lerp(1.04f, 1.0f, p);
            }

            stampWrapper.style.opacity = stampAlpha;
            stampWrapper.style.scale = new Scale(new Vector3(stampScale, stampScale, 1f));
            stampWrapper.style.rotate = new Rotate(new Angle(stampRot, AngleUnit.Degree));

            // Impact Burst Lines (2.68s -> 2.98s)
            if (t >= 2.68f && t <= 2.98f)
            {
                float ip = (t - 2.68f) / 0.30f;
                float iScale = Mathf.Lerp(0.75f, 1.35f, EaseOutCubic(ip));
                float iAlpha = ip < 0.25f ? (ip / 0.25f) : Mathf.Lerp(1f, 0f, (ip - 0.25f) / 0.75f);
                impactLinesContainer.style.opacity = iAlpha;
                impactLinesContainer.style.scale = new Scale(new Vector3(iScale, iScale, 1f));
            }
            else
            {
                impactLinesContainer.style.opacity = 0f;
            }
        }

        // G. Specular Gleam Sweep across Stamp (3.00s -> 3.65s)
        if (t >= 3.00f && t <= 3.65f)
        {
            float gp = (t - 3.00f) / 0.65f;
            gleamBar.style.opacity = gp < 0.15f ? (gp / 0.15f) : (gp > 0.85f ? ((1f - gp) / 0.15f) : 1f);
            gleamBar.style.left = Mathf.Lerp(-140f, 460f, gp);
        }
        else
        {
            gleamBar.style.opacity = 0f;
        }

        // H. Bottom Loading Progress Bar Fill (0.00s -> 2.70s reaches 508px = 100%)
        float loadFrac = Mathf.Clamp01(t / 2.70f);
        loadingBarFill.style.width = Mathf.Lerp(20f, 508f, loadFrac);
        if (t >= 2.70f)
        {
            loadingBarFill.style.backgroundColor = EmeraldAccent;
        }
    }

    private static float EvaluateClockArmAngle(float t)
    {
        if (t < 1.20f) return 0f;
        if (t < 1.70f) return EvaluateSingleTick(t - 1.20f, 0f, 5.0f, 5.7f);
        if (t < 2.20f) return EvaluateSingleTick(t - 1.70f, 5.0f, 10.0f, 10.7f);
        if (t < 2.70f) return EvaluateSingleTick(t - 2.20f, 10.0f, 15.0f, 15.7f);
        return EvaluateSingleTick(t - 2.70f, 15.0f, 20.0f, 20.8f);
    }

    private static float EvaluateSingleTick(float localTime, float fromAngle, float targetAngle, float overshootAngle)
    {
        if (localTime < 0.025f)
            return Mathf.Lerp(fromAngle, overshootAngle, localTime / 0.025f);
        if (localTime < 0.070f)
            return Mathf.Lerp(overshootAngle, targetAngle, (localTime - 0.025f) / 0.045f);
        return targetAngle;
    }

    private static float EaseOutCubic(float x)
    {
        float f = 1f - Mathf.Clamp01(x);
        return 1f - f * f * f;
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
        IsIntroPlaying = false;
        AudioManager.Instance?.EndIntroSequence(0.65f);

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
        IsIntroPlaying = false;
        if (rootContainer != null)
        {
            rootContainer.RemoveFromHierarchy();
            rootContainer = null;
        }
    }
}

using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Linq;

/// <summary>
/// High-frame-rate (60–144+ FPS) native UI Toolkit Intro & Loading Screen.
/// Eliminates VideoPlayer/RenderTexture Editor stutter and startup delay by animating
/// high-DPI vector layers directly on the GPU in locked sync with the audio timeline:
///   - 0.00s – 0.35s (0% – 7%):   "DEAR HIRING MANAGER" title lockup, "DEVELOPED BY" header,
///                                 24 Hour Studios emblem, and loading progress bar appear.
///                                 - Gameplay BGM is held at 0 volume from SubsystemRegistration;
///                                   sfx_intro_jingle starts immediately on frame 0.
///   - 0.40s – 0.80s (8% – 16%):  Cartoon CV swoops into center (sfx_intro_whoosh at 0.40s).
///   - 0.80s – 1.15s (16% – 23%): 5 CV dossier lines draw staggered (sfx_pencil_sketch at 0.80s).
///   - 1.20s, 1.70s, 2.20s, 2.70s (24%, 34%, 44%, 54%):
///                                 4 exact 5° mechanical clock ticks (5° -> 10° -> 15° -> 20° lock)
///                                 around the SVG dial pivot (76.55%, 74.01%) with sfx_clock_tick.
///   - 2.70s (54%):               Final 20° clock lock coincides with the red "HIRED!" stamp slam
///                                 (sfx_stamp_slam + HeavyImpact haptic + triumphant jingle chord).
///   - 3.00s – 3.65s (60% – 73%): Specular gleam sweeps across the "HIRED!" stamp.
///   - 4.50s – 5.00s (90% – 100%): Smooth fade out to dashboard while gameplay BGM fades in.
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

    // Animated Vector Layers
    private VisualElement titleElement;
    private VisualElement heroContainer;
    private VisualElement cvElement;
    private readonly VisualElement[] cvLines = new VisualElement[5];
    private VisualElement impactLinesContainer;
    private VisualElement stampWrapper;
    private VisualElement stampElement;
    private VisualElement gleamBar;
    private VisualElement attribElement;
    private VisualElement studioBox;
    private VisualElement studioStaticElement;
    private VisualElement studioArmElement;
    private Label loadingStatusLabel;
    private VisualElement loadingBarFill;

    private bool isDismissing = false;

    private static readonly Color LogoDark = new Color(0.020f, 0.031f, 0.067f, 1f); // #050811
    private static readonly Color LineColor = new Color(0.580f, 0.639f, 0.722f, 1f); // #94A3B8
    private static readonly Color CyanAccent = new Color(0.220f, 0.741f, 0.973f, 1f); // #38BDF8
    private static readonly Color EmeraldAccent = new Color(0.204f, 0.827f, 0.600f, 1f); // #34D399
    private static readonly Color TrackDark = new Color(0.059f, 0.090f, 0.165f, 1f); // #0F172A
    private static readonly Color TrackBorder = new Color(0.200f, 0.255f, 0.333f, 1f); // #334155
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
        rootContainer.focusable = true;
        rootContainer.Focus();
        rootContainer.RegisterCallback<KeyDownEvent>(_ => Dismiss());

        // Master animated scene wrapper
        sceneMaster = new VisualElement();
        sceneMaster.style.flexGrow = 1;
        sceneMaster.style.width = Length.Percent(100);
        sceneMaster.style.height = Length.Percent(100);
        sceneMaster.style.justifyContent = Justify.SpaceBetween;
        sceneMaster.style.alignItems = Align.Center;
        sceneMaster.style.paddingTop = Length.Percent(3.5f);
        sceneMaster.style.paddingBottom = Length.Percent(3.5f);
        sceneMaster.style.opacity = 0f;
        rootContainer.Add(sceneMaster);

        // ── 1. TOP TITLE LOCKUP ("DEAR HIRING MANAGER") ──
        titleElement = new VisualElement();
        titleElement.style.width = Length.Percent(78);
        titleElement.style.maxWidth = 380;
        titleElement.style.aspectRatio = 800f / 300f;
        if (texTitle != null)
        {
            titleElement.style.backgroundImage = Background.FromTexture2D(texTitle);
            titleElement.style.backgroundSize = new BackgroundSize(BackgroundSizeType.Contain);
            titleElement.style.backgroundRepeat = new BackgroundRepeat(Repeat.NoRepeat, Repeat.NoRepeat);
            titleElement.style.backgroundPositionX = new BackgroundPosition(BackgroundPositionKeyword.Center);
            titleElement.style.backgroundPositionY = new BackgroundPosition(BackgroundPositionKeyword.Center);
        }
        titleElement.style.transformOrigin = new TransformOrigin(Length.Percent(50), Length.Percent(50), 0);
        sceneMaster.Add(titleElement);

        // ── 2. CENTER HERO: CARTOON CV + 5 LINES + "HIRED!" STAMP ──
        heroContainer = new VisualElement();
        heroContainer.style.width = Length.Percent(58);
        heroContainer.style.maxWidth = 265;
        heroContainer.style.aspectRatio = 500f / 640f;
        heroContainer.style.justifyContent = Justify.Center;
        heroContainer.style.alignItems = Align.Center;
        sceneMaster.Add(heroContainer);

        cvElement = new VisualElement();
        cvElement.style.position = Position.Absolute;
        cvElement.style.left = 0;
        cvElement.style.top = 0;
        cvElement.style.right = 0;
        cvElement.style.bottom = 0;
        if (texCVBase != null)
        {
            cvElement.style.backgroundImage = Background.FromTexture2D(texCVBase);
            cvElement.style.backgroundSize = new BackgroundSize(BackgroundSizeType.Contain);
            cvElement.style.backgroundRepeat = new BackgroundRepeat(Repeat.NoRepeat, Repeat.NoRepeat);
        }
        cvElement.style.transformOrigin = new TransformOrigin(Length.Percent(50), Length.Percent(50), 0);
        cvElement.style.opacity = 0f;
        heroContainer.Add(cvElement);

        // 5 Staggered CV Experience Lines inside cvElement (viewBox="-60 -50 500 640")
        float[] lineWidthsPct = { 61.6f, 52.0f, 58.0f, 44.0f, 54.0f };
        for (int i = 0; i < 5; i++)
        {
            VisualElement line = new VisualElement();
            line.style.position = Position.Absolute;
            line.style.left = Length.Percent(19.2f);
            line.style.top = Length.Percent((210f + i * 36f) / 640f * 100f);
            line.style.width = Length.Percent(lineWidthsPct[i]);
            line.style.height = Length.Percent(2.81f);
            line.style.backgroundColor = LineColor;
            line.style.borderTopLeftRadius = 5;
            line.style.borderTopRightRadius = 5;
            line.style.borderBottomLeftRadius = 5;
            line.style.borderBottomRightRadius = 5;
            line.style.transformOrigin = new TransformOrigin(Length.Percent(0), Length.Percent(50), 0);
            line.style.scale = new Scale(new Vector3(0f, 1f, 1f));
            line.style.opacity = 0f;
            cvElement.Add(line);
            cvLines[i] = line;
        }

        // Exterior Radial Impact Burst Lines around Stamp
        impactLinesContainer = new VisualElement();
        impactLinesContainer.style.position = Position.Absolute;
        impactLinesContainer.style.left = Length.Percent(-12);
        impactLinesContainer.style.right = Length.Percent(-12);
        impactLinesContainer.style.top = Length.Percent(50);
        impactLinesContainer.style.height = Length.Percent(32);
        impactLinesContainer.style.transformOrigin = new TransformOrigin(Length.Percent(50), Length.Percent(50), 0);
        impactLinesContainer.style.opacity = 0f;
        heroContainer.Add(impactLinesContainer);

        CreateImpactRay(impactLinesContainer, -2f, 12f, -24f);
        CreateImpactRay(impactLinesContainer, 90f, 12f, 24f);
        CreateImpactRay(impactLinesContainer, -4f, 78f, 22f);
        CreateImpactRay(impactLinesContainer, 92f, 78f, -22f);

        // Red "HIRED!" Stamp Wrapper
        stampWrapper = new VisualElement();
        stampWrapper.style.position = Position.Absolute;
        stampWrapper.style.width = Length.Percent(92);
        stampWrapper.style.aspectRatio = 440f / 200f;
        stampWrapper.style.top = Length.Percent(51);
        stampWrapper.style.transformOrigin = new TransformOrigin(Length.Percent(50), Length.Percent(50), 0);
        stampWrapper.style.opacity = 0f;
        heroContainer.Add(stampWrapper);

        stampElement = new VisualElement();
        stampElement.style.position = Position.Absolute;
        stampElement.style.left = 0;
        stampElement.style.right = 0;
        stampElement.style.top = 0;
        stampElement.style.bottom = 0;
        if (texStamp != null)
        {
            stampElement.style.backgroundImage = Background.FromTexture2D(texStamp);
            stampElement.style.backgroundSize = new BackgroundSize(BackgroundSizeType.Contain);
            stampElement.style.backgroundRepeat = new BackgroundRepeat(Repeat.NoRepeat, Repeat.NoRepeat);
        }
        stampElement.style.overflow = Overflow.Hidden;
        stampWrapper.Add(stampElement);

        // Specular Gleam Bar inside Stamp
        gleamBar = new VisualElement();
        gleamBar.style.position = Position.Absolute;
        gleamBar.style.width = Length.Percent(22);
        gleamBar.style.height = Length.Percent(160);
        gleamBar.style.top = Length.Percent(-30);
        gleamBar.style.backgroundColor = new Color(1f, 1f, 1f, 0.45f);
        gleamBar.style.rotate = new Rotate(new Angle(-22f, AngleUnit.Degree));
        gleamBar.style.opacity = 0f;
        stampElement.Add(gleamBar);

        // ── 3. LOWER STUDIO CREDIT: "DEVELOPED BY" + 24 HOUR STUDIOS ──
        VisualElement studioGroup = new VisualElement();
        studioGroup.style.width = Length.Percent(100);
        studioGroup.style.alignItems = Align.Center;
        studioGroup.style.justifyContent = Justify.Center;
        sceneMaster.Add(studioGroup);

        attribElement = new VisualElement();
        attribElement.style.width = Length.Percent(58);
        attribElement.style.maxWidth = 270;
        attribElement.style.aspectRatio = 600f / 70f;
        attribElement.style.marginBottom = 4;
        if (texAttrib != null)
        {
            attribElement.style.backgroundImage = Background.FromTexture2D(texAttrib);
            attribElement.style.backgroundSize = new BackgroundSize(BackgroundSizeType.Contain);
            attribElement.style.backgroundRepeat = new BackgroundRepeat(Repeat.NoRepeat, Repeat.NoRepeat);
        }
        studioGroup.Add(attribElement);

        studioBox = new VisualElement();
        studioBox.style.width = Length.Percent(40);
        studioBox.style.maxWidth = 185;
        studioBox.style.aspectRatio = 540f / 630f;
        studioBox.style.transformOrigin = new TransformOrigin(Length.Percent(50), Length.Percent(50), 0);
        studioGroup.Add(studioBox);

        studioStaticElement = new VisualElement();
        studioStaticElement.style.position = Position.Absolute;
        studioStaticElement.style.left = 0;
        studioStaticElement.style.top = 0;
        studioStaticElement.style.right = 0;
        studioStaticElement.style.bottom = 0;
        if (texStudioStatic != null)
        {
            studioStaticElement.style.backgroundImage = Background.FromTexture2D(texStudioStatic);
            studioStaticElement.style.backgroundSize = new BackgroundSize(BackgroundSizeType.Contain);
            studioStaticElement.style.backgroundRepeat = new BackgroundRepeat(Repeat.NoRepeat, Repeat.NoRepeat);
        }
        studioBox.Add(studioStaticElement);

        studioArmElement = new VisualElement();
        studioArmElement.style.position = Position.Absolute;
        studioArmElement.style.left = 0;
        studioArmElement.style.top = 0;
        studioArmElement.style.right = 0;
        studioArmElement.style.bottom = 0;
        if (texStudioArm != null)
        {
            studioArmElement.style.backgroundImage = Background.FromTexture2D(texStudioArm);
            studioArmElement.style.backgroundSize = new BackgroundSize(BackgroundSizeType.Contain);
            studioArmElement.style.backgroundRepeat = new BackgroundRepeat(Repeat.NoRepeat, Repeat.NoRepeat);
        }
        // Exact SVG pivot (1277.97, 1454.18) inside viewBox(-100, -100, 1800, 2100) -> (76.5539%, 74.0086%)
        studioArmElement.style.transformOrigin = new TransformOrigin(Length.Percent(76.5539f), Length.Percent(74.0086f), 0f);
        studioBox.Add(studioArmElement);

        // ── 4. BOTTOM LOADING BAR & STATUS ──
        VisualElement loadingGroup = new VisualElement();
        loadingGroup.style.width = Length.Percent(80);
        loadingGroup.style.maxWidth = 360;
        loadingGroup.style.alignItems = Align.Center;
        sceneMaster.Add(loadingGroup);

        loadingStatusLabel = new Label("PREPARING WORKPLACE SIMULATION...");
        loadingStatusLabel.style.fontSize = 11;
        loadingStatusLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        loadingStatusLabel.style.color = CyanAccent;
        loadingStatusLabel.style.letterSpacing = 3;
        loadingStatusLabel.style.marginBottom = 6;
        loadingStatusLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
        loadingGroup.Add(loadingStatusLabel);

        VisualElement barTrack = new VisualElement();
        barTrack.style.width = Length.Percent(100);
        barTrack.style.height = 12;
        barTrack.style.backgroundColor = TrackDark;
        barTrack.style.borderTopWidth = 1.5f;
        barTrack.style.borderBottomWidth = 1.5f;
        barTrack.style.borderLeftWidth = 1.5f;
        barTrack.style.borderRightWidth = 1.5f;
        barTrack.style.borderTopColor = TrackBorder;
        barTrack.style.borderBottomColor = TrackBorder;
        barTrack.style.borderLeftColor = TrackBorder;
        barTrack.style.borderRightColor = TrackBorder;
        barTrack.style.borderTopLeftRadius = 6;
        barTrack.style.borderTopRightRadius = 6;
        barTrack.style.borderBottomLeftRadius = 6;
        barTrack.style.borderBottomRightRadius = 6;
        barTrack.style.paddingLeft = 2;
        barTrack.style.paddingRight = 2;
        barTrack.style.paddingTop = 2;
        barTrack.style.paddingBottom = 2;
        loadingGroup.Add(barTrack);

        loadingBarFill = new VisualElement();
        loadingBarFill.style.width = Length.Percent(4);
        loadingBarFill.style.height = Length.Percent(100);
        loadingBarFill.style.backgroundColor = CyanAccent;
        loadingBarFill.style.borderTopLeftRadius = 4;
        loadingBarFill.style.borderTopRightRadius = 4;
        loadingBarFill.style.borderBottomLeftRadius = 4;
        loadingBarFill.style.borderBottomRightRadius = 4;
        barTrack.Add(loadingBarFill);

        Label skipHintLabel = new Label("TAP ANYWHERE TO SKIP");
        skipHintLabel.style.fontSize = 9.5f;
        skipHintLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        skipHintLabel.style.color = SkipTextColor;
        skipHintLabel.style.letterSpacing = 2.5f;
        skipHintLabel.style.marginTop = 8;
        skipHintLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
        loadingGroup.Add(skipHintLabel);

        uiDocument.rootVisualElement.Add(rootContainer);
    }

    private void CreateImpactRay(VisualElement parent, float leftPct, float topPct, float angleDeg)
    {
        VisualElement ray = new VisualElement();
        ray.style.position = Position.Absolute;
        ray.style.left = Length.Percent(leftPct);
        ray.style.top = Length.Percent(topPct);
        ray.style.width = 24;
        ray.style.height = 4;
        ray.style.backgroundColor = CyanAccent;
        ray.style.borderTopLeftRadius = 2;
        ray.style.borderTopRightRadius = 2;
        ray.style.borderBottomLeftRadius = 2;
        ray.style.borderBottomRightRadius = 2;
        ray.style.rotate = new Rotate(new Angle(angleDeg, AngleUnit.Degree));
        parent.Add(ray);
    }

    private IEnumerator PlayIntroSequence()
    {
        // Let 1 frame settle after scene load so deltaTime is clean
        yield return null;

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

            // Clamp single-frame spikes in the Editor so animation never jumps
            float dt = Mathf.Min(Time.unscaledDeltaTime, 0.05f);
            elapsed += dt;

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
            titleY = Mathf.Lerp(-24f, 2f, p);
        }
        else if (t < 0.40f)
        {
            float p = (t - 0.275f) / 0.125f;
            titleScale = Mathf.Lerp(1.04f, 1.0f, p);
            titleY = Mathf.Lerp(2f, 0f, p);
        }
        titleElement.style.scale = new Scale(new Vector3(titleScale, titleScale, 1f));
        titleElement.style.translate = new Translate(0, titleY, 0);

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
                cvScale = Mathf.Lerp(2.25f, 1.04f, p);
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
                stampScale = Mathf.Lerp(3.0f, 0.92f, p);
                stampRot = Mathf.Lerp(-26f, -10f, p);
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
            gleamBar.style.left = Length.Percent(Mathf.Lerp(-30f, 115f, gp));
        }
        else
        {
            gleamBar.style.opacity = 0f;
        }

        // H. Bottom Loading Progress Bar Fill (0.00s -> 2.70s reaches 100%)
        float loadPct = Mathf.Clamp01(t / 2.70f) * 100f;
        loadingBarFill.style.width = Length.Percent(Mathf.Max(4f, loadPct));
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
        // Crisp 25ms mechanical snap to overshoot, 45ms recoil settle to target
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

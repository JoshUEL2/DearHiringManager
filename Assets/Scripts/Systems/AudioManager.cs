using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;
    public static AudioManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<AudioManager>();
                if (_instance == null)
                {
                    var go = new GameObject("AudioManager");
                    _instance = go.AddComponent<AudioManager>();
                }
            }
            return _instance;
        }
    }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource ambienceSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource heartbeatSource;

    [Header("Volume Controls")]
    [Range(0f, 1f)] [SerializeField] private float masterVolume = 1f;
    [Range(0f, 1f)] [SerializeField] private float musicVolume = 0.55f;
    [Range(0f, 1f)] [SerializeField] private float ambienceVolume = 0.65f;
    [Range(0f, 1f)] [SerializeField] private float sfxVolume = 0.85f;
    [SerializeField] private bool isMuted = false;

    // Loaded Audio Clips
    private AudioClip bgmNeon;
    private AudioClip bgmTerminal;
    private AudioClip bgmDebug;
    private List<AudioClip> bgmPlaylist = new List<AudioClip>();
    private int currentBgmIndex = 0;

    private AudioClip ambParkBirds;
    private AudioClip ambEducationMurmurs;

    private AudioClip sfxScrubPlate1;
    private AudioClip sfxScrubPlate2;
    private AudioClip sfxWaterSplash;
    private AudioClip sfxDishClatter;
    private AudioClip sfxTabSwitch;
    private AudioClip sfxWarningShift;
    private AudioClip sfxTypewriter;

    private AudioClip clickClip;
    private AudioClip cashClip;
    private AudioClip expenseClip;
    private AudioClip turnAdvanceClip;
    private AudioClip minigameSuccessClip;
    private AudioClip minigameFailClip;
    private AudioClip heartbeatClip;
    private AudioClip scorePopClip;
    private AudioClip buzzerClip;
    private AudioClip whooshClip;
    private AudioClip dogBarkClip;
    private readonly Dictionary<string, AudioClip> _clipCache = new Dictionary<string, AudioClip>();

    private AudioClip GetOrCreateClip(string key, System.Func<AudioClip> generator)
    {
        if (!_clipCache.TryGetValue(key, out var clip) || clip == null)
        {
            clip = generator();
            _clipCache[key] = clip;
        }
        return clip;
    }

    private float lastClickTime = 0f;

    private const string MUTE_PREF_KEY = "dear_audio_muted";
    private const string MASTER_VOL_KEY = "dear_audio_master_vol";
    private const string MUSIC_VOL_KEY = "dear_audio_music_vol";
    private const string SFX_VOL_KEY = "dear_audio_sfx_vol";
    private const int SAMPLE_RATE = 44100;

    // Heartbeat pulse tracking
    private float nextHeartbeatTime = 0f;

    // Scrub throttling
    private float lastScrubTime = 0f;
    private int scrubCounter = 0;

    // Coroutine tracking for fading
    private Coroutine musicFadeCoroutine;
    private Coroutine ambienceFadeCoroutine;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioSources();
            LoadAudioClips();
            LoadSettings();
            StartBaseBGM();
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void InitializeAudioSources()
    {
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.playOnAwake = false;
            musicSource.loop = true;
            musicSource.spatialBlend = 0f;
        }

        if (ambienceSource == null)
        {
            ambienceSource = gameObject.AddComponent<AudioSource>();
            ambienceSource.playOnAwake = false;
            ambienceSource.loop = true;
            ambienceSource.spatialBlend = 0f;
        }

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
            sfxSource.loop = false;
            sfxSource.spatialBlend = 0f;
        }

        if (heartbeatSource == null)
        {
            heartbeatSource = gameObject.AddComponent<AudioSource>();
            heartbeatSource.playOnAwake = false;
            heartbeatSource.loop = false;
            heartbeatSource.spatialBlend = 0f;
        }
    }

    private void LoadAudioClips()
    {
        // Music
        bgmNeon = Resources.Load<AudioClip>("Audio/Music/bgm_lofi_neon");
        bgmTerminal = Resources.Load<AudioClip>("Audio/Music/bgm_lofi_terminal");
        bgmDebug = Resources.Load<AudioClip>("Audio/Music/bgm_lofi_debug");

        if (bgmNeon != null) bgmPlaylist.Add(bgmNeon);
        if (bgmTerminal != null) bgmPlaylist.Add(bgmTerminal);
        if (bgmDebug != null) bgmPlaylist.Add(bgmDebug);

        // Ambience
        ambParkBirds = Resources.Load<AudioClip>("Audio/Ambience/amb_park_birds");
        ambEducationMurmurs = Resources.Load<AudioClip>("Audio/Ambience/amb_education_murmurs");

        // Physical SFX
        sfxScrubPlate1 = Resources.Load<AudioClip>("Audio/SFX/sfx_scrub_plate_01");
        sfxScrubPlate2 = Resources.Load<AudioClip>("Audio/SFX/sfx_scrub_plate_02");
        sfxWaterSplash = Resources.Load<AudioClip>("Audio/SFX/sfx_water_splash");
        sfxDishClatter = Resources.Load<AudioClip>("Audio/SFX/sfx_dish_clatter");
        sfxTabSwitch = Resources.Load<AudioClip>("Audio/SFX/sfx_tab_switch");
        sfxWarningShift = Resources.Load<AudioClip>("Audio/SFX/sfx_warning_shift");
        sfxTypewriter = Resources.Load<AudioClip>("Audio/SFX/sfx_typewriter");

        // Primary UI and Gameplay SFX
        clickClip = Resources.Load<AudioClip>("Audio/SFX/sfx_click") ?? GenerateClickClip();
        cashClip = Resources.Load<AudioClip>("Audio/SFX/sfx_cash") ?? GenerateCashClip();
        expenseClip = Resources.Load<AudioClip>("Audio/SFX/sfx_bill_deduction") ?? GenerateExpenseClip();
        turnAdvanceClip = Resources.Load<AudioClip>("Audio/SFX/sfx_turn_advance") ?? GenerateTurnAdvanceClip();
        minigameSuccessClip = Resources.Load<AudioClip>("Audio/SFX/sfx_minigame_success") ?? GenerateSuccessClip();
        minigameFailClip = Resources.Load<AudioClip>("Audio/SFX/sfx_minigame_fail") ?? GenerateFailClip();
        heartbeatClip = Resources.Load<AudioClip>("Audio/SFX/sfx_heartbeat") ?? GenerateHeartbeatClip();
        scorePopClip = Resources.Load<AudioClip>("Audio/SFX/sfx_score_pop") ?? GenerateScorePopClip();
        buzzerClip = Resources.Load<AudioClip>("Audio/SFX/sfx_buzzer") ?? GenerateBuzzerClip();
        whooshClip = Resources.Load<AudioClip>("Audio/SFX/sfx_whoosh") ?? GenerateWhooshClip();

        // Fallbacks for scrub and tab switch if not found
        if (sfxTabSwitch == null) sfxTabSwitch = clickClip;
        if (sfxWarningShift == null) sfxWarningShift = minigameFailClip;
        if (sfxScrubPlate1 == null) sfxScrubPlate1 = GenerateClickClip();
        if (sfxScrubPlate2 == null) sfxScrubPlate2 = GenerateClickClip();

        // Pre-cache Intro Jingle & SFX for zero-latency playback during LoadingScreenUI
        string[] introClips = { "sfx_intro_jingle", "sfx_clock_tick", "sfx_intro_whoosh", "sfx_pencil_sketch", "sfx_stamp_slam" };
        foreach (string cName in introClips)
        {
            AudioClip loaded = Resources.Load<AudioClip>($"Audio/SFX/{cName}");
            if (loaded != null)
            {
                _clipCache[cName] = loaded;
                _clipCache[$"Audio/SFX/{cName}"] = loaded;
            }
        }
    }

    private void StartBaseBGM()
    {
        if (bgmPlaylist.Count > 0 && musicSource != null)
        {
            currentBgmIndex = 0;
            musicSource.clip = bgmPlaylist[currentBgmIndex];
            musicSource.volume = (isMuted || LoadingScreenUI.IsIntroPlaying) ? 0f : (musicVolume * masterVolume);
            musicSource.Play();
        }
    }

    private void LoadSettings()
    {
        isMuted = PlayerPrefs.GetInt(MUTE_PREF_KEY, 0) == 1;
        masterVolume = PlayerPrefs.GetFloat(MASTER_VOL_KEY, 1f);
        musicVolume = PlayerPrefs.GetFloat(MUSIC_VOL_KEY, 0.55f);
        sfxVolume = PlayerPrefs.GetFloat(SFX_VOL_KEY, 0.85f);
        ApplyVolumeSettings();
    }

    private void ApplyVolumeSettings()
    {
        if (musicSource != null)
        {
            musicSource.mute = isMuted;
            musicSource.volume = LoadingScreenUI.IsIntroPlaying ? 0f : (musicVolume * masterVolume);
        }
        if (ambienceSource != null)
        {
            ambienceSource.mute = isMuted;
            ambienceSource.volume = ambienceVolume * masterVolume;
        }
        if (sfxSource != null)
        {
            sfxSource.mute = isMuted;
            sfxSource.volume = sfxVolume * masterVolume;
        }
        if (heartbeatSource != null)
        {
            heartbeatSource.mute = isMuted;
            heartbeatSource.volume = sfxVolume * masterVolume;
        }
    }

    private void Update()
    {
        // Loop through playlist when BGM ends
        if (musicSource != null && !musicSource.isPlaying && !isMuted && bgmPlaylist.Count > 0)
        {
            currentBgmIndex = (currentBgmIndex + 1) % bgmPlaylist.Count;
            musicSource.clip = bgmPlaylist[currentBgmIndex];
            musicSource.Play();
        }

        // Monitor critical stress heartbeat when stress > 80%
        if (GameManager.Instance != null && GameManager.Instance.currentData != null && !GameManager.Instance.isGameOver)
        {
            float stress = GameManager.Instance.currentData.stress;
            if (stress > 0.8f)
            {
                if (Time.unscaledTime >= nextHeartbeatTime)
                {
                    float t = Mathf.Clamp01((stress - 0.8f) / 0.2f);
                    float interval = Mathf.Lerp(1.15f, 0.65f, t);
                    nextHeartbeatTime = Time.unscaledTime + interval;

                    PlayHeartbeat(stress);
                }
            }
        }
    }

    #region Contextual Ambience & BGM Crossfading

    /// <summary>
    /// Smoothly transitions in outdoor park ambience and ducks background music.
    /// </summary>
    public void PlayParkAmbience(float fadeDuration = 0.8f)
    {
        if (ambParkBirds == null) return;
        CrossfadeAmbience(ambParkBirds, 0.60f * ambienceVolume * masterVolume, fadeDuration);
        DuckMusic(0.20f * musicVolume * masterVolume, fadeDuration);
    }

    /// <summary>
    /// Smoothly transitions in educational murmur ambience and ducks background music.
    /// </summary>
    public void PlayEducationAmbience(float fadeDuration = 0.8f)
    {
        if (ambEducationMurmurs == null) return;
        CrossfadeAmbience(ambEducationMurmurs, 0.55f * ambienceVolume * masterVolume, fadeDuration);
        DuckMusic(0.25f * musicVolume * masterVolume, fadeDuration);
    }

    /// <summary>
    /// Returns to standard base lo-fi background music, fading out any active ambience.
    /// </summary>
    public void ReturnToBaseMusic(float fadeDuration = 0.8f)
    {
        FadeOutAmbience(fadeDuration);
        RestoreMusic(fadeDuration);
    }

    private void CrossfadeAmbience(AudioClip clip, float targetVol, float duration)
    {
        if (ambienceSource == null || clip == null) return;
        if (ambienceFadeCoroutine != null) StopCoroutine(ambienceFadeCoroutine);
        ambienceFadeCoroutine = StartCoroutine(CrossfadeAmbienceRoutine(clip, targetVol, duration));
    }

    private IEnumerator CrossfadeAmbienceRoutine(AudioClip clip, float targetVol, float duration)
    {
        if (ambienceSource.isPlaying && ambienceSource.clip == clip)
        {
            // Already playing this clip, just adjust volume
            float startVol = ambienceSource.volume;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                ambienceSource.volume = Mathf.Lerp(startVol, isMuted ? 0f : targetVol, elapsed / duration);
                yield return null;
            }
            ambienceSource.volume = isMuted ? 0f : targetVol;
            yield break;
        }

        // Fade out current clip
        if (ambienceSource.isPlaying)
        {
            float startVol = ambienceSource.volume;
            float elapsed = 0f;
            while (elapsed < duration * 0.5f)
            {
                elapsed += Time.unscaledDeltaTime;
                ambienceSource.volume = Mathf.Lerp(startVol, 0f, elapsed / (duration * 0.5f));
                yield return null;
            }
            ambienceSource.Stop();
        }

        // Start new clip and fade in
        ambienceSource.clip = clip;
        ambienceSource.volume = 0f;
        ambienceSource.Play();

        float fadeInElapsed = 0f;
        while (fadeInElapsed < duration * 0.5f)
        {
            fadeInElapsed += Time.unscaledDeltaTime;
            ambienceSource.volume = Mathf.Lerp(0f, isMuted ? 0f : targetVol, fadeInElapsed / (duration * 0.5f));
            yield return null;
        }
        ambienceSource.volume = isMuted ? 0f : targetVol;
    }

    private void FadeOutAmbience(float duration)
    {
        if (ambienceSource == null || !ambienceSource.isPlaying) return;
        if (ambienceFadeCoroutine != null) StopCoroutine(ambienceFadeCoroutine);
        ambienceFadeCoroutine = StartCoroutine(FadeOutAmbienceRoutine(duration));
    }

    private IEnumerator FadeOutAmbienceRoutine(float duration)
    {
        float startVol = ambienceSource.volume;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            ambienceSource.volume = Mathf.Lerp(startVol, 0f, elapsed / duration);
            yield return null;
        }
        ambienceSource.volume = 0f;
        ambienceSource.Stop();
        ambienceSource.clip = null;
    }

    private void DuckMusic(float targetVol, float duration)
    {
        if (musicSource == null) return;
        if (musicFadeCoroutine != null) StopCoroutine(musicFadeCoroutine);
        musicFadeCoroutine = StartCoroutine(FadeMusicRoutine(isMuted ? 0f : targetVol, duration));
    }

    private void RestoreMusic(float duration)
    {
        if (musicSource == null) return;
        if (musicFadeCoroutine != null) StopCoroutine(musicFadeCoroutine);
        float normalVol = isMuted ? 0f : (musicVolume * masterVolume);
        musicFadeCoroutine = StartCoroutine(FadeMusicRoutine(normalVol, duration));
    }

    private IEnumerator FadeMusicRoutine(float targetVol, float duration)
    {
        float startVol = musicSource.volume;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            musicSource.volume = Mathf.Lerp(startVol, targetVol, elapsed / duration);
            yield return null;
        }
        musicSource.volume = targetVol;
    }

    #endregion

    #region Public Playback API

    /// <summary>
    /// Scrubbing sponge / scraper physical audio with alternating texture and pitch jitter.
    /// Rate-limited to prevent buzzing during high frequency touch movement.
    /// </summary>
    public void PlayScrubPlate()
    {
        if (isMuted || sfxSource == null) return;
        if (Time.unscaledTime - lastScrubTime < 0.085f) return; // 85ms minimum interval
        lastScrubTime = Time.unscaledTime;

        scrubCounter++;
        AudioClip clipToPlay = (scrubCounter % 2 == 0) ? sfxScrubPlate1 : sfxScrubPlate2;
        if (clipToPlay == null) clipToPlay = clickClip;

        // Subtle pitch randomization (0.92x to 1.08x) for realistic tactile texture
        float originalPitch = sfxSource.pitch;
        sfxSource.pitch = Random.Range(0.92f, 1.08f);
        sfxSource.PlayOneShot(clipToPlay, sfxVolume * masterVolume * 0.75f);
        sfxSource.pitch = originalPitch;
    }

    public void PlayWaterSplash()
    {
        if (sfxWaterSplash != null) PlayClip(sfxWaterSplash, 0.70f);
    }

    public void PlayDishClatter()
    {
        if (sfxDishClatter != null) PlayClip(sfxDishClatter, 0.80f);
    }

    public void PlayTabSwitch()
    {
        if (sfxTabSwitch != null) PlayClip(sfxTabSwitch, 0.75f);
        else PlayClick();
    }

    public void PlayShiftWarning()
    {
        if (sfxWarningShift != null) PlayClip(sfxWarningShift, 0.85f);
        else PlayMinigameFail();
    }

    public void PlayTypewriter()
    {
        if (sfxTypewriter != null) PlayClip(sfxTypewriter, 0.40f);
    }

    public void PlayClick()
    {
        if (isMuted) return;
        if (Time.unscaledTime - lastClickTime < 0.04f) return;
        lastClickTime = Time.unscaledTime;

        if (clickClip == null) clickClip = GenerateClickClip();
        PlayClip(clickClip, 0.70f);
    }

    public void PlayCash()
    {
        PlayClip(cashClip, 0.85f);
    }

    public void PlayLedgerChaChing()
    {
        PlayCash();
    }

    public void PlayDogBark()
    {
        if (isMuted) return;
        if (dogBarkClip == null) dogBarkClip = GenerateDogBarkClip();
        if (sfxSource != null)
        {
            float prevPitch = sfxSource.pitch;
            sfxSource.pitch = Random.Range(0.92f, 1.08f);
            PlayClip(dogBarkClip, 0.85f);
            sfxSource.pitch = prevPitch;
        }
        else
        {
            PlayClip(dogBarkClip, 0.85f);
        }
    }

    public void PlayExpense()
    {
        PlayClip(expenseClip, 0.80f);
    }

    public void PlayTurnAdvance()
    {
        PlayClip(turnAdvanceClip, 0.85f);
    }

    public void PlayMinigameSuccess()
    {
        PlayClip(minigameSuccessClip, 0.90f);
    }

    public void PlayMinigameFail()
    {
        PlayClip(minigameFailClip, 0.85f);
    }

    public void PlayScorePop()
    {
        if (scorePopClip == null) scorePopClip = GenerateScorePopClip();
        PlayClip(scorePopClip, 0.85f);
    }

    public void PlayBuzzer()
    {
        if (buzzerClip == null) buzzerClip = GenerateBuzzerClip();
        PlayClip(buzzerClip, 0.85f);
    }

    public void PlayWhoosh()
    {
        if (whooshClip == null) whooshClip = GenerateWhooshClip();
        PlayClip(whooshClip, 0.65f);
    }

    public void PlayHeartbeat(float stressIntensity = 0.85f)
    {
        if (isMuted || heartbeatSource == null) return;
        if (heartbeatClip == null) heartbeatClip = GenerateHeartbeatClip();

        float vol = Mathf.Lerp(0.4f, 1.0f, Mathf.Clamp01((stressIntensity - 0.8f) / 0.2f)) * sfxVolume * masterVolume;
        heartbeatSource.PlayOneShot(heartbeatClip, vol);

        if (stressIntensity > 0.92f)
        {
            HapticManager.HeavyImpact();
        }
        else
        {
            HapticManager.LightImpact();
        }
    }

    private int _sweepCounter = 0;
    public void PlaySweepFloor()
    {
        _sweepCounter++;
        string key = $"Sweep_{_sweepCounter % 4}";
        var clip = GetOrCreateClip(key, () => GenerateSweepClip(_sweepCounter % 4));
        PlayPitchedClip(clip, 0.75f, 0.93f, 1.07f);
    }

    private int _mopCounter = 0;
    public void PlayMopFloor()
    {
        _mopCounter++;
        string key = $"Mop_{_mopCounter % 4}";
        var clip = GetOrCreateClip(key, () => GenerateMopClip(_mopCounter % 4));
        PlayPitchedClip(clip, 0.85f, 0.91f, 1.09f);
    }

    public void PlayStampPass(bool approved)
    {
        string key = approved ? "Stamp_Approved" : "Stamp_Rejected";
        var clip = GetOrCreateClip(key, () => GenerateStampPassClip(approved));
        PlayPitchedClip(clip, 0.95f, 0.97f, 1.03f);
        if (approved) HapticManager.MediumImpact();
        else HapticManager.HeavyImpact();
    }

    public void PlayHeartAttack()
    {
        var clip = GetOrCreateClip("HeartAttack", GenerateHeartAttackClip);
        PlayClip(clip, 1.0f);
        HapticManager.HeavyImpact();
    }

    public void PlayPromotion()
    {
        var clip = GetOrCreateClip("Promotion", GeneratePromotionClip);
        PlayClip(clip, 0.95f);
        HapticManager.MediumImpact();
    }

    public void PlayPromotionRefused()
    {
        var clip = GetOrCreateClip("PromotionRefused", GeneratePromotionRefusedClip);
        PlayClip(clip, 0.85f);
        HapticManager.LightImpact();
    }

    public void PlayEviction()
    {
        var clip = GetOrCreateClip("Eviction", GenerateEvictionClip);
        PlayClip(clip, 0.95f);
        HapticManager.HeavyImpact();
    }

    public void PlayJailArrest()
    {
        var clip = GetOrCreateClip("JailArrest", GenerateJailArrestClip);
        PlayClip(clip, 0.95f);
        HapticManager.HeavyImpact();
    }

    public void PlayInterviewHired()
    {
        var clip = GetOrCreateClip("InterviewHired", GenerateJobHiredClip);
        PlayClip(clip, 0.90f);
        HapticManager.LightImpact();
    }

    public void PlayInterviewRejected()
    {
        var clip = GetOrCreateClip("InterviewRejected", GenerateJobInterviewRejectionClip);
        PlayClip(clip, 0.85f);
        HapticManager.MediumImpact();
    }

    public void PlayVictoryFanfare()
    {
        var clip = GetOrCreateClip("VictoryFanfare", GenerateVictoryFanfareClip);
        PlayClip(clip, 1.0f);
        HapticManager.HeavyImpact();
    }

    /// <summary>
    /// Ducks gameplay BGM during the intro loading screen and starts the musical intro jingle.
    /// </summary>
    public void BeginIntroSequence()
    {
        if (musicSource != null)
        {
            if (musicFadeCoroutine != null) StopCoroutine(musicFadeCoroutine);
            musicSource.volume = 0f;
        }
        PlaySFX("sfx_intro_jingle", 0.88f);
    }

    /// <summary>
    /// Smoothly restores the base lo-fi gameplay BGM when the intro loading screen finishes or is skipped.
    /// </summary>
    public void EndIntroSequence(float fadeDuration = 0.6f)
    {
        RestoreMusic(fadeDuration);
    }

    /// <summary>
    /// Plays the high-precision mechanical clock escapement tick for the 24 Hour Studios emblem.
    /// </summary>
    public void PlayClockTick(float volumeScale = 0.90f)
    {
        if (isMuted || sfxSource == null) return;
        if (_clipCache.TryGetValue("sfx_clock_tick", out AudioClip tickClip) && tickClip != null)
        {
            sfxSource.PlayOneShot(tickClip, Mathf.Clamp01(volumeScale * sfxVolume * masterVolume));
        }
        else
        {
            PlaySFX("sfx_clock_tick", volumeScale);
        }
    }

    /// <summary>
    /// Generic SFX player: loads any clip by resource name from Audio/SFX/
    /// and plays it once at the standard SFX volume.
    /// e.g. PlaySFX("sfx_pencil_sketch")  loads "Audio/SFX/sfx_pencil_sketch"
    /// </summary>
    public void PlaySFX(string clipName, float volumeScale = 0.85f)
    {
        if (isMuted || sfxSource == null) return;
        string resourcePath = clipName.Contains("/") ? clipName : $"Audio/SFX/{clipName}";
        if (!_clipCache.TryGetValue(clipName, out AudioClip clip) || clip == null)
        {
            if (!_clipCache.TryGetValue(resourcePath, out clip) || clip == null)
            {
                clip = Resources.Load<AudioClip>(resourcePath);
                if (clip != null)
                {
                    _clipCache[clipName] = clip;
                    _clipCache[resourcePath] = clip;
                }
            }
        }
        if (clip == null)
        {
            Debug.LogWarning($"AudioManager.PlaySFX: clip not found at '{resourcePath}'");
            return;
        }
        sfxSource.PlayOneShot(clip, Mathf.Clamp01(volumeScale * sfxVolume * masterVolume));
    }

    public void PlayActivityWellness(string activityName)
    {
        string lower = (activityName ?? "").ToLowerInvariant();
        string type = "park";
        if (lower.Contains("arcade")) type = "arcade";
        else if (lower.Contains("stone") || lower.Contains("volcanic")) type = "stone";
        else if (lower.Contains("mud") || lower.Contains("detox")) type = "mud";
        else if (lower.Contains("ice") || lower.Contains("bath")) type = "ice";

        var clip = GetOrCreateClip($"Wellness_{type}", () => GenerateWellnessClip(type));
        PlayClip(clip, 0.85f);
        HapticManager.LightImpact();
    }

    public void PlayActivityNetworking(string mixerName)
    {
        string lower = (mixerName ?? "").ToLowerInvariant();
        string type = "bar";
        if (lower.Contains("gala") || lower.Contains("hospitality")) type = "gala";
        else if (lower.Contains("pitch") || lower.Contains("startup") || lower.Contains("tech")) type = "startup";
        else if (lower.Contains("golf") || lower.Contains("corp")) type = "golf";

        var clip = GetOrCreateClip($"Networking_{type}", () => GenerateNetworkingClip(type));
        PlayClip(clip, 0.85f);
        HapticManager.LightImpact();
    }

    public void PlayThemeShift(int themeIndex)
    {
        var clip = GetOrCreateClip($"ThemeShift_{themeIndex}", () => GenerateThemeShiftClip(themeIndex));
        PlayClip(clip, 0.75f);
    }

    public void PlayPopupOpen()
    {
        var clip = GetOrCreateClip("PopupOpen", GeneratePopupOpenClip);
        PlayClip(clip, 0.70f);
    }

    public void PlayPopupClose()
    {
        var clip = GetOrCreateClip("PopupClose", GeneratePopupCloseClip);
        PlayClip(clip, 0.65f);
    }

    public void PlaySettingsToggle()
    {
        var clip = GetOrCreateClip("SettingsToggle", GenerateSettingsToggleClip);
        PlayPitchedClip(clip, 0.75f, 0.95f, 1.05f);
    }

    public void PlayCVOpen()
    {
        var clip = GetOrCreateClip("CVOpen", GenerateCVOpenClip);
        PlayClip(clip, 0.70f);
    }

    public void PlayDebtWarning()
    {
        var clip = GetOrCreateClip("DebtWarning", GenerateDebtWarningClip);
        PlayClip(clip, 0.90f);
        HapticManager.HeavyImpact();
    }

    public void PlayIceShatter()
    {
        var clip = GetOrCreateClip("IceShatter", GenerateIceShatterClip);
        PlayPitchedClip(clip, 0.85f, 0.90f, 1.10f);
    }

    public void PlaySnatchBriefcase()
    {
        var clip = GetOrCreateClip("SnatchBriefcase", GenerateBriefcaseSnapClip);
        PlayPitchedClip(clip, 0.85f, 0.95f, 1.05f);
    }

    public void PlayLockpickPin()
    {
        var clip = GetOrCreateClip("LockpickPin", GenerateLockpickPinClip);
        PlayPitchedClip(clip, 0.75f, 0.95f, 1.05f);
    }

    public void PlayCenterDivSnap()
    {
        var clip = GetOrCreateClip("CenterDivSnap", GenerateCenterDivSnapClip);
        PlayClip(clip, 0.80f);
    }

    public void PlayCherrySwitch()
    {
        var clip = GetOrCreateClip("CherrySwitch", GenerateCherrySwitchClip);
        PlayPitchedClip(clip, 0.70f, 0.92f, 1.08f);
    }

    public void PlayFranticKey()
    {
        var clip = GetOrCreateClip("FranticKey", GenerateFranticKeyClip);
        PlayPitchedClip(clip, 0.80f, 0.90f, 1.10f);
    }

    public void PlayGitMerge(bool conflict)
    {
        string key = conflict ? "GitMerge_Conflict" : "GitMerge_Clean";
        var clip = GetOrCreateClip(key, () => GenerateGitMergeClip(conflict));
        PlayClip(clip, conflict ? 0.90f : 0.80f);
    }

    public void PlayTetrisRotate()
    {
        var clip = GetOrCreateClip("TetrisRotate", GenerateTetrisRotateClip);
        PlayPitchedClip(clip, 0.70f, 0.96f, 1.04f);
    }

    public void PlayTetrisDrop()
    {
        var clip = GetOrCreateClip("TetrisDrop", GenerateTetrisDropClip);
        PlayClip(clip, 0.75f);
    }

    public void PlayTetrisLineClear()
    {
        var clip = GetOrCreateClip("TetrisLineClear", GenerateTetrisLineClearClip);
        PlayClip(clip, 0.85f);
    }

    public void PlayLawnMower()
    {
        var clip = GetOrCreateClip("LawnMower", GenerateLawnMowerClip);
        PlayPitchedClip(clip, 0.75f, 0.95f, 1.05f);
    }

    public void PlayAcidRainDeflect()
    {
        var clip = GetOrCreateClip("AcidRainDeflect", GenerateAcidRainShieldClip);
        PlayPitchedClip(clip, 0.75f, 0.90f, 1.10f);
    }

    public void PlayCloggedVentPop()
    {
        var clip = GetOrCreateClip("CloggedVentPop", GenerateCloggedVentClip);
        PlayPitchedClip(clip, 0.80f, 0.95f, 1.05f);
    }

    public void PlayIPHackFlurry()
    {
        var clip = GetOrCreateClip("IPHackFlurry", GenerateIPHackClip);
        PlayClip(clip, 0.75f);
    }

    public void PlayTurnstileSwipe()
    {
        var clip = GetOrCreateClip("TurnstileSwipe", GenerateTurnstileSwipeClip);
        PlayClip(clip, 0.80f);
    }

    public void PlayConstructionRivet()
    {
        var clip = GetOrCreateClip("ConstructionRivet", GenerateConstructionRivetClip);
        PlayPitchedClip(clip, 0.90f, 0.95f, 1.05f);
    }

    public void PlayRecycleBottle()
    {
        var clip = GetOrCreateClip("RecycleBottle", GenerateRecycleBottleClip);
        PlayPitchedClip(clip, 0.80f, 0.90f, 1.10f);
    }

    public void PlayFoodChop()
    {
        var clip = GetOrCreateClip("FoodChop", GenerateFoodChopClip);
        PlayPitchedClip(clip, 0.80f, 0.92f, 1.08f);
    }

    public void PlayEspressoSteam()
    {
        var clip = GetOrCreateClip("EspressoSteam", GenerateEspressoSteamClip);
        PlayClip(clip, 0.75f);
    }

    public void PlayATMDispense()
    {
        var clip = GetOrCreateClip("ATMDispense", GenerateATMDispenseClip);
        PlayClip(clip, 0.85f);
    }

    private void PlayPitchedClip(AudioClip clip, float volumeScale, float minPitch, float maxPitch)
    {
        if (isMuted || clip == null || sfxSource == null) return;
        float prevPitch = sfxSource.pitch;
        sfxSource.pitch = Random.Range(minPitch, maxPitch);
        sfxSource.PlayOneShot(clip, sfxVolume * masterVolume * volumeScale);
        sfxSource.pitch = prevPitch;
    }

    private void PlayClip(AudioClip clip, float volumeScale = 1f)
    {
        if (isMuted || clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip, sfxVolume * masterVolume * volumeScale);
    }

    public void SetMuted(bool muted)
    {
        isMuted = muted;
        PlayerPrefs.SetInt(MUTE_PREF_KEY, muted ? 1 : 0);
        PlayerPrefs.Save();
        ApplyVolumeSettings();
    }

    public bool IsMuted()
    {
        return isMuted;
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat(MASTER_VOL_KEY, masterVolume);
        PlayerPrefs.Save();
        ApplyVolumeSettings();
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat(MUSIC_VOL_KEY, musicVolume);
        PlayerPrefs.Save();
        ApplyVolumeSettings();
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat(SFX_VOL_KEY, sfxVolume);
        PlayerPrefs.Save();
        ApplyVolumeSettings();
    }

    public float GetMasterVolume() => masterVolume;
    public float GetMusicVolume() => musicVolume;
    public float GetSFXVolume() => sfxVolume;

    #endregion

    #region Procedural Audio Generators (Fallbacks)

    private AudioClip GenerateClickClip()
    {
        float duration = 0.038f;
        int sampleCount = (int)(SAMPLE_RATE * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / SAMPLE_RATE;
            float freq = Mathf.Lerp(2100f, 950f, t / duration);
            float env = Mathf.Exp(-t * 90f);
            samples[i] = Mathf.Sin(2f * Mathf.PI * freq * t) * env * 0.32f;
        }

        var clip = AudioClip.Create("Procedural_Click", sampleCount, 1, SAMPLE_RATE, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip GenerateCashClip()
    {
        float duration = 0.38f;
        int sampleCount = (int)(SAMPLE_RATE * duration);
        float[] samples = new float[sampleCount];

        float[] freqs = new float[] { 1318.5f, 1661.2f, 1975.5f };
        float[] delays = new float[] { 0.0f, 0.07f, 0.14f };

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / SAMPLE_RATE;
            float sum = 0f;
            for (int n = 0; n < 3; n++)
            {
                if (t >= delays[n])
                {
                    float dt = t - delays[n];
                    float f = freqs[n];
                    float fundamental = Mathf.Sin(2f * Mathf.PI * f * dt);
                    float harmonic = 0.35f * Mathf.Sin(4f * Mathf.PI * f * dt);
                    float env = Mathf.Exp(-dt * 13f);
                    sum += (fundamental + harmonic) * env;
                }
            }
            samples[i] = Mathf.Clamp(sum * 0.36f, -1f, 1f);
        }

        var clip = AudioClip.Create("Procedural_Cash", sampleCount, 1, SAMPLE_RATE, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip GenerateExpenseClip()
    {
        float duration = 0.28f;
        int sampleCount = (int)(SAMPLE_RATE * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / SAMPLE_RATE;
            float progress = t / duration;
            float freq = Mathf.Lerp(190f, 50f, Mathf.Pow(progress, 0.6f));
            float tone = Mathf.Sin(2f * Mathf.PI * freq * t) + 0.3f * Mathf.Sin(4f * Mathf.PI * freq * t);
            float env = Mathf.Exp(-t * 11f);

            float hiss = 0f;
            if (t < 0.05f)
            {
                hiss = (Random.value * 2f - 1f) * Mathf.Exp(-t * 60f) * 0.25f;
            }

            samples[i] = Mathf.Clamp((tone * env * 0.45f) + hiss, -1f, 1f);
        }

        var clip = AudioClip.Create("Procedural_Expense", sampleCount, 1, SAMPLE_RATE, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip GenerateTurnAdvanceClip()
    {
        float duration = 0.25f;
        int sampleCount = (int)(SAMPLE_RATE * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / SAMPLE_RATE;
            float sum = 0f;

            if (t < 0.035f)
            {
                float clickFreq = Mathf.Lerp(2400f, 1200f, t / 0.035f);
                sum += Mathf.Sin(2f * Mathf.PI * clickFreq * t) * Mathf.Exp(-t * 90f) * 0.45f;
            }

            if (t >= 0.045f)
            {
                float dt = t - 0.045f;
                float shutterFreq = Mathf.Lerp(850f, 220f, dt / 0.205f);
                float tone = Mathf.Sin(2f * Mathf.PI * shutterFreq * dt);
                float noise = (Random.value * 2f - 1f) * 0.25f;
                float env = Mathf.Exp(-dt * 15f);
                sum += (tone + noise) * env * 0.40f;
            }

            samples[i] = Mathf.Clamp(sum, -1f, 1f);
        }

        var clip = AudioClip.Create("Procedural_TurnAdvance", sampleCount, 1, SAMPLE_RATE, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip GenerateSuccessClip()
    {
        float duration = 0.42f;
        int sampleCount = (int)(SAMPLE_RATE * duration);
        float[] samples = new float[sampleCount];

        float[] notes = new float[] { 523.25f, 659.25f, 783.99f, 1046.50f };
        float[] times = new float[] { 0.0f, 0.07f, 0.14f, 0.21f };

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / SAMPLE_RATE;
            float sum = 0f;
            for (int n = 0; n < notes.Length; n++)
            {
                if (t >= times[n])
                {
                    float dt = t - times[n];
                    float f = notes[n];
                    float fundamental = Mathf.Sin(2f * Mathf.PI * f * dt);
                    float shimmer = 0.25f * Mathf.Sin(4f * Mathf.PI * f * dt);
                    float env = Mathf.Exp(-dt * 10f);
                    sum += (fundamental + shimmer) * env;
                }
            }
            samples[i] = Mathf.Clamp(sum * 0.32f, -1f, 1f);
        }

        var clip = AudioClip.Create("Procedural_MinigameSuccess", sampleCount, 1, SAMPLE_RATE, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip GenerateFailClip()
    {
        float duration = 0.34f;
        int sampleCount = (int)(SAMPLE_RATE * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / SAMPLE_RATE;
            float progress = t / duration;

            float f1 = Mathf.Lerp(340f, 140f, progress);
            float f2 = Mathf.Lerp(365f, 152f, progress);

            float s1 = Mathf.Sin(2f * Mathf.PI * f1 * t);
            float s2 = Mathf.Sin(2f * Mathf.PI * f2 * t);
            float env = Mathf.Exp(-t * 7.5f);

            samples[i] = Mathf.Clamp((s1 + s2) * env * 0.35f, -1f, 1f);
        }

        var clip = AudioClip.Create("Procedural_MinigameFail", sampleCount, 1, SAMPLE_RATE, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip GenerateHeartbeatClip()
    {
        float duration = 0.36f;
        int sampleCount = (int)(SAMPLE_RATE * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / SAMPLE_RATE;
            float sum = 0f;

            if (t < 0.12f)
            {
                float freq = Mathf.Lerp(68f, 44f, t / 0.12f);
                float env = Mathf.Sin(Mathf.PI * (t / 0.12f)) * Mathf.Exp(-t * 12f);
                sum += Mathf.Sin(2f * Mathf.PI * freq * t) * env * 0.65f;
            }

            if (t >= 0.14f && t < 0.28f)
            {
                float dt = t - 0.14f;
                float freq = Mathf.Lerp(82f, 52f, dt / 0.14f);
                float env = Mathf.Sin(Mathf.PI * (dt / 0.14f)) * Mathf.Exp(-dt * 15f);
                sum += Mathf.Sin(2f * Mathf.PI * freq * dt) * env * 0.55f;
            }

            samples[i] = Mathf.Clamp(sum, -1f, 1f);
        }

        var clip = AudioClip.Create("Procedural_Heartbeat", sampleCount, 1, SAMPLE_RATE, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip GenerateScorePopClip()
    {
        float duration = 0.14f;
        int sampleCount = (int)(SAMPLE_RATE * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / SAMPLE_RATE;
            float freq = Mathf.Lerp(880f, 1320f, t / duration);
            float tone = Mathf.Sin(2f * Mathf.PI * freq * t);
            float harmonic = Mathf.Sin(4f * Mathf.PI * freq * t) * 0.35f;
            float env = Mathf.Exp(-t * 22f);
            samples[i] = Mathf.Clamp((tone + harmonic) * env * 0.5f, -1f, 1f);
        }

        var clip = AudioClip.Create("Procedural_ScorePop", sampleCount, 1, SAMPLE_RATE, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip GenerateBuzzerClip()
    {
        float duration = 0.20f;
        int sampleCount = (int)(SAMPLE_RATE * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / SAMPLE_RATE;
            float square = Mathf.Sin(2f * Mathf.PI * 130f * t) > 0f ? 0.35f : -0.35f;
            float buzz = Mathf.Sin(2f * Mathf.PI * 260f * t) * 0.2f;
            float gate = (t < 0.08f || t > 0.11f) ? 1f : 0.05f; // Double buzz stutter
            float env = Mathf.Exp(-t * 8f);
            samples[i] = Mathf.Clamp((square + buzz) * gate * env * 0.45f, -1f, 1f);
        }

        var clip = AudioClip.Create("Procedural_Buzzer", sampleCount, 1, SAMPLE_RATE, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip GenerateWhooshClip()
    {
        float duration = 0.16f;
        int sampleCount = (int)(SAMPLE_RATE * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / SAMPLE_RATE;
            float progress = t / duration;
            float noise = (Random.value * 2f - 1f);
            float env = Mathf.Sin(progress * Mathf.PI);
            samples[i] = Mathf.Clamp(noise * env * 0.35f, -1f, 1f);
        }

        var clip = AudioClip.Create("Procedural_Whoosh", sampleCount, 1, SAMPLE_RATE, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip GenerateDogBarkClip()
    {
        float duration = 0.22f;
        int sampleCount = (int)(SAMPLE_RATE * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / SAMPLE_RATE;
            float progress = t / duration;

            // Pitch drop curve: 480Hz -> 160Hz
            float freq = Mathf.Lerp(480f, 160f, Mathf.Pow(progress, 0.7f));
            float fundamental = Mathf.Sin(2f * Mathf.PI * freq * t);
            float formant1 = Mathf.Sin(2f * Mathf.PI * (freq * 1.85f) * t) * 0.45f;
            float formant2 = Mathf.Sin(2f * Mathf.PI * (freq * 2.7f) * t) * 0.25f;
            float noise = (Random.value * 2f - 1f) * 0.15f * Mathf.Exp(-t * 20f);

            float env;
            if (t < 0.015f) env = t / 0.015f;
            else env = Mathf.Exp(-(t - 0.015f) * 16f);

            samples[i] = Mathf.Clamp((fundamental + formant1 + formant2 + noise) * env * 0.75f, -1f, 1f);
        }

        var clip = AudioClip.Create("Procedural_DogBark", sampleCount, 1, SAMPLE_RATE, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip GenerateSweepClip(int variant)
    {
        float duration = 0.16f + variant * 0.02f;
        int sampleCount = (int)(SAMPLE_RATE * duration);
        float[] samples = new float[sampleCount];
        float baseFreq = 1800f + variant * 180f;

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / SAMPLE_RATE;
            float progress = t / duration;
            float noise = (Random.value * 2f - 1f);
            float friction = Mathf.Sin(2f * Mathf.PI * baseFreq * t) * 0.25f;
            float scrapeTone = Mathf.Sin(2f * Mathf.PI * 340f * t) * 0.15f;
            float env = Mathf.Sin(progress * Mathf.PI);
            samples[i] = Mathf.Clamp((noise * 0.55f + friction + scrapeTone) * env * 0.55f, -1f, 1f);
        }

        var clip = AudioClip.Create($"Procedural_Sweep_{variant}", sampleCount, 1, SAMPLE_RATE, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip GenerateMopClip(int variant)
    {
        float duration = 0.24f + variant * 0.03f;
        int sampleCount = (int)(SAMPLE_RATE * duration);
        float[] samples = new float[sampleCount];
        float squelchFreq = 380f - variant * 40f;

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / SAMPLE_RATE;
            float progress = t / duration;
            float f = Mathf.Lerp(squelchFreq, 140f, progress);
            float squelch = Mathf.Sin(2f * Mathf.PI * f * t) * 0.45f;
            float wetNoise = (Random.value * 2f - 1f) * 0.25f;
            float squeak = Mathf.Sin(2f * Mathf.PI * Mathf.Lerp(820f, 640f, progress) * t) * 0.2f;
            float env = Mathf.Sin(Mathf.Pow(progress, 0.7f) * Mathf.PI);
            samples[i] = Mathf.Clamp((squelch + wetNoise + squeak) * env * 0.65f, -1f, 1f);
        }

        var clip = AudioClip.Create($"Procedural_Mop_{variant}", sampleCount, 1, SAMPLE_RATE, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip GenerateStampPassClip(bool approved)
    {
        float duration = approved ? 0.26f : 0.32f;
        int sampleCount = (int)(SAMPLE_RATE * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / SAMPLE_RATE;
            float sum = 0f;

            // Component 1: Heavy wooden handle thud
            if (t < 0.12f)
            {
                float thudFreq = Mathf.Lerp(110f, 45f, t / 0.12f);
                float thud = Mathf.Sin(2f * Mathf.PI * thudFreq * t) * Mathf.Exp(-t * 28f);
                sum += thud * 0.85f;
            }

            // Component 2: Crisp paper slap
            if (t < 0.04f)
            {
                float slap = (Random.value * 2f - 1f) * Mathf.Exp(-t * 80f) * 0.55f;
                float ping = Mathf.Sin(2f * Mathf.PI * 2200f * t) * Mathf.Exp(-t * 110f) * 0.4f;
                sum += slap + ping;
            }

            // Component 3: Ink suction pop / rejection buzz
            if (approved)
            {
                if (t >= 0.02f && t < 0.15f)
                {
                    float dt = t - 0.02f;
                    float squish = Mathf.Sin(2f * Mathf.PI * 620f * dt) * Mathf.Exp(-dt * 30f) * 0.35f;
                    sum += squish;
                }
            }
            else
            {
                // Double hit + rejection buzz
                if (t >= 0.08f)
                {
                    float dt = t - 0.08f;
                    float buzz1 = Mathf.Sin(2f * Mathf.PI * 330f * dt);
                    float buzz2 = Mathf.Sin(2f * Mathf.PI * 466f * dt) * 0.5f;
                    sum += (buzz1 + buzz2) * Mathf.Exp(-dt * 10f) * 0.45f;
                }
            }

            samples[i] = Mathf.Clamp(sum, -1f, 1f);
        }

        var clip = AudioClip.Create(approved ? "Procedural_Stamp_Approved" : "Procedural_Stamp_Rejected", sampleCount, 1, SAMPLE_RATE, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip GenerateHeartAttackClip()
    {
        float duration = 1.9f;
        int sampleCount = (int)(SAMPLE_RATE * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / SAMPLE_RATE;
            float sum = 0f;

            // Panicked Lub-Dub heart thumps
            if (t < 0.15f)
            {
                float dt = t;
                float freq = Mathf.Lerp(65f, 40f, dt / 0.15f);
                sum += Mathf.Sin(2f * Mathf.PI * freq * dt) * Mathf.Exp(-dt * 14f) * 0.9f;
            }
            if (t >= 0.18f && t < 0.35f)
            {
                float dt = t - 0.18f;
                float freq = Mathf.Lerp(85f, 45f, dt / 0.17f);
                sum += Mathf.Sin(2f * Mathf.PI * freq * dt) * Mathf.Exp(-dt * 16f) * 0.85f;
            }

            // Cardiac strain rupture rumble
            if (t >= 0.35f && t < 0.65f)
            {
                float dt = t - 0.35f;
                float rumble = (Random.value * 2f - 1f) * 0.3f + Mathf.Sin(2f * Mathf.PI * 38f * dt) * 0.6f;
                sum += rumble * Mathf.Exp(-dt * 6f) * 0.7f;
            }

            // Piercing EKG Monitor Flatline Tone (3920Hz)
            if (t >= 0.55f)
            {
                float dt = t - 0.55f;
                float flatlineTone = Mathf.Sin(2f * Mathf.PI * 3920f * dt);
                float subHum = Mathf.Sin(2f * Mathf.PI * 120f * dt) * 0.15f;
                float flatlineEnv = Mathf.Clamp01(dt / 0.05f) * Mathf.Clamp01((duration - t) / 0.2f);
                sum += (flatlineTone * 0.55f + subHum) * flatlineEnv;
            }

            samples[i] = Mathf.Clamp(sum, -1f, 1f);
        }

        var clip = AudioClip.Create("Procedural_HeartAttack", sampleCount, 1, SAMPLE_RATE, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip GeneratePromotionClip()
    {
        float duration = 0.90f;
        int sampleCount = (int)(SAMPLE_RATE * duration);
        float[] samples = new float[sampleCount];

        // Ascending 5-chord brass triumph: C4, E4, G4, C5, E5
        float[] notes = new float[] { 261.63f, 329.63f, 392.00f, 523.25f, 659.25f };
        float[] times = new float[] { 0.0f, 0.12f, 0.24f, 0.36f, 0.52f };
        float[] durs  = new float[] { 0.14f, 0.14f, 0.14f, 0.18f, 0.38f };

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / SAMPLE_RATE;
            float sum = 0f;

            for (int n = 0; n < notes.Length; n++)
            {
                if (t >= times[n])
                {
                    float dt = t - times[n];
                    float f = notes[n];
                    float brass = Mathf.Sin(2f * Mathf.PI * f * dt) + 
                                  0.45f * Mathf.Sin(4f * Mathf.PI * f * dt) + 
                                  0.20f * Mathf.Sin(6f * Mathf.PI * f * dt) +
                                  0.10f * Mathf.Sin(8f * Mathf.PI * f * dt);
                    float env = Mathf.Exp(-dt * (n == 4 ? 4f : 8f));
                    sum += brass * env * 0.35f;

                    // Sparkle chime on top
                    float sparkle = Mathf.Sin(2f * Mathf.PI * (f * 4f) * dt) * Mathf.Exp(-dt * 16f) * 0.15f;
                    sum += sparkle;
                }
            }

            samples[i] = Mathf.Clamp(sum, -1f, 1f);
        }

        var clip = AudioClip.Create("Procedural_Promotion", sampleCount, 1, SAMPLE_RATE, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip GeneratePromotionRefusedClip()
    {
        float duration = 0.70f;
        int sampleCount = (int)(SAMPLE_RATE * duration);
        float[] samples = new float[sampleCount];

        // Descending wah-wah trombone: Eb3, D3, Db3, C3
        float[] notes = new float[] { 155.56f, 146.83f, 138.59f, 130.81f };
        float[] times = new float[] { 0.0f, 0.14f, 0.28f, 0.44f };

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / SAMPLE_RATE;
            float sum = 0f;

            for (int n = 0; n < notes.Length; n++)
            {
                if (t >= times[n])
                {
                    float dt = t - times[n];
                    float f = notes[n] * Mathf.Lerp(1.02f, 0.96f, dt / 0.2f);
                    float wah = (1f + 0.35f * Mathf.Sin(2f * Mathf.PI * 7f * dt));
                    float tone = Mathf.Sin(2f * Mathf.PI * f * dt) + 0.35f * Mathf.Sin(4f * Mathf.PI * f * dt);
                    float env = Mathf.Exp(-dt * (n == 3 ? 5f : 9f));
                    sum += tone * wah * env * 0.38f;
                }
            }

            samples[i] = Mathf.Clamp(sum, -1f, 1f);
        }

        var clip = AudioClip.Create("Procedural_PromotionRefused", sampleCount, 1, SAMPLE_RATE, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip GenerateEvictionClip()
    {
        float duration = 0.75f;
        int sampleCount = (int)(SAMPLE_RATE * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / SAMPLE_RATE;
            float sum = 0f;

            // Padlock snap
            if (t < 0.08f)
            {
                float snapFreq = Mathf.Lerp(1400f, 400f, t / 0.08f);
                sum += Mathf.Sin(2f * Mathf.PI * snapFreq * t) * Mathf.Exp(-t * 40f) * 0.75f;
            }

            // Heavy door slam
            if (t >= 0.06f && t < 0.4f)
            {
                float dt = t - 0.06f;
                float thud = Mathf.Sin(2f * Mathf.PI * Mathf.Lerp(120f, 45f, dt / 0.34f) * dt);
                sum += thud * Mathf.Exp(-dt * 10f) * 0.8f;
            }

            // Low warning gong
            if (t >= 0.10f)
            {
                float dt = t - 0.10f;
                float gong = Mathf.Sin(2f * Mathf.PI * 65.4f * dt) + 0.4f * Mathf.Sin(2f * Mathf.PI * 92.5f * dt);
                sum += gong * Mathf.Exp(-dt * 4f) * 0.55f;
            }

            samples[i] = Mathf.Clamp(sum, -1f, 1f);
        }

        var clip = AudioClip.Create("Procedural_Eviction", sampleCount, 1, SAMPLE_RATE, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip GenerateJailArrestClip()
    {
        float duration = 0.90f;
        int sampleCount = (int)(SAMPLE_RATE * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / SAMPLE_RATE;
            float sum = 0f;

            // Metallic sliding gate
            if (t < 0.25f)
            {
                float scrape = (Random.value * 2f - 1f) * 0.2f + Mathf.Sin(2f * Mathf.PI * 850f * t) * 0.15f;
                sum += scrape * (t / 0.25f);
            }

            // Cell bars heavy slam
            if (t >= 0.22f)
            {
                float dt = t - 0.22f;
                float clang = Mathf.Sin(2f * Mathf.PI * 220f * dt) + 
                              0.5f * Mathf.Sin(2f * Mathf.PI * 440f * dt) + 
                              0.3f * Mathf.Sin(2f * Mathf.PI * 880f * dt);
                float rattle = (Random.value * 2f - 1f) * 0.25f * Mathf.Exp(-dt * 20f);
                sum += (clang * Mathf.Exp(-dt * 6f) * 0.7f) + rattle;
            }

            samples[i] = Mathf.Clamp(sum, -1f, 1f);
        }

        var clip = AudioClip.Create("Procedural_JailArrest", sampleCount, 1, SAMPLE_RATE, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip GenerateJobHiredClip()
    {
        float duration = 0.55f;
        int sampleCount = (int)(SAMPLE_RATE * duration);
        float[] samples = new float[sampleCount];

        // Major 6th corporate welcome chime (E4, G#4, B4, E5)
        float[] notes = new float[] { 329.63f, 415.30f, 493.88f, 659.25f };
        float[] times = new float[] { 0.0f, 0.08f, 0.16f, 0.24f };

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / SAMPLE_RATE;
            float sum = 0f;

            for (int n = 0; n < notes.Length; n++)
            {
                if (t >= times[n])
                {
                    float dt = t - times[n];
                    float tone = Mathf.Sin(2f * Mathf.PI * notes[n] * dt) + 0.3f * Mathf.Sin(4f * Mathf.PI * notes[n] * dt);
                    sum += tone * Mathf.Exp(-dt * 8f) * 0.38f;
                }
            }

            samples[i] = Mathf.Clamp(sum, -1f, 1f);
        }

        var clip = AudioClip.Create("Procedural_JobHired", sampleCount, 1, SAMPLE_RATE, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip GenerateJobInterviewRejectionClip()
    {
        float duration = 0.45f;
        int sampleCount = (int)(SAMPLE_RATE * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / SAMPLE_RATE;
            float thud = Mathf.Sin(2f * Mathf.PI * 90f * t) * Mathf.Exp(-t * 20f) * 0.6f;
            float tear = (Random.value * 2f - 1f) * Mathf.Sin(Mathf.PI * (t / duration)) * 0.25f;
            float lowMuffled = Mathf.Sin(2f * Mathf.PI * 130f * t) * Mathf.Exp(-t * 8f) * 0.3f;
            samples[i] = Mathf.Clamp(thud + tear + lowMuffled, -1f, 1f);
        }

        var clip = AudioClip.Create("Procedural_InterviewRejected", sampleCount, 1, SAMPLE_RATE, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip GenerateVictoryFanfareClip()
    {
        float duration = 1.4f;
        int sampleCount = (int)(SAMPLE_RATE * duration);
        float[] samples = new float[sampleCount];

        // Grand C-major triumphant CEO fanfare progression: C4, G4, C5, E5, G5, C6
        float[] notes = new float[] { 261.63f, 392.00f, 523.25f, 659.25f, 783.99f, 1046.50f };
        float[] times = new float[] { 0.0f, 0.15f, 0.30f, 0.48f, 0.65f, 0.85f };

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / SAMPLE_RATE;
            float sum = 0f;

            for (int n = 0; n < notes.Length; n++)
            {
                if (t >= times[n])
                {
                    float dt = t - times[n];
                    float f = notes[n];
                    float brass = Mathf.Sin(2f * Mathf.PI * f * dt) + 
                                  0.4f * Mathf.Sin(4f * Mathf.PI * f * dt) + 
                                  0.2f * Mathf.Sin(6f * Mathf.PI * f * dt);
                    float env = Mathf.Exp(-dt * (n == notes.Length - 1 ? 3.5f : 6f));
                    sum += brass * env * 0.32f;
                }
            }

            samples[i] = Mathf.Clamp(sum, -1f, 1f);
        }

        var clip = AudioClip.Create("Procedural_VictoryFanfare", sampleCount, 1, SAMPLE_RATE, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip GenerateWellnessClip(string type)
    {
        float duration = 0.40f;
        int sampleCount = (int)(SAMPLE_RATE * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / SAMPLE_RATE;
            float sum = 0f;

            if (type == "arcade")
            {
                // 8-bit coin collect arpeggio
                float[] arp = new float[] { 987.77f, 1318.51f };
                float f = t < 0.12f ? arp[0] : arp[1];
                float sq = Mathf.Sin(2f * Mathf.PI * f * t) > 0f ? 0.35f : -0.35f;
                sum = sq * Mathf.Exp(-t * 7f);
            }
            else if (type == "stone")
            {
                // Basalt stone clack + steam hiss
                float clack = Mathf.Sin(2f * Mathf.PI * 380f * t) * Mathf.Exp(-t * 35f) * 0.6f;
                float hiss = (Random.value * 2f - 1f) * Mathf.Exp(-t * 9f) * 0.25f;
                sum = clack + hiss;
            }
            else if (type == "mud")
            {
                // Thermal mud bubble pop
                float pop = Mathf.Sin(2f * Mathf.PI * Mathf.Lerp(160f, 440f, t / 0.15f) * t) * Mathf.Exp(-t * 20f);
                sum = pop * 0.5f;
            }
            else if (type == "ice")
            {
                // Freezing crystal crackle
                float crack = (Random.value * 2f - 1f) * Mathf.Exp(-t * 40f) * 0.4f;
                float chime = Mathf.Sin(2f * Mathf.PI * 2637f * t) * Mathf.Exp(-t * 12f) * 0.35f;
                sum = crack + chime;
            }
            else
            {
                // Park stroll breeze & footstep
                float breeze = (Random.value * 2f - 1f) * Mathf.Sin(Mathf.PI * (t / duration)) * 0.3f;
                float crunch = (Random.value * 2f - 1f) * Mathf.Exp(-t * 25f) * 0.3f;
                sum = breeze + crunch;
            }

            samples[i] = Mathf.Clamp(sum, -1f, 1f);
        }

        var clip = AudioClip.Create($"Procedural_Wellness_{type}", sampleCount, 1, SAMPLE_RATE, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip GenerateNetworkingClip(string type)
    {
        float duration = 0.45f;
        int sampleCount = (int)(SAMPLE_RATE * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / SAMPLE_RATE;
            float sum = 0f;

            if (type == "bar")
            {
                // Beer pint clink on wood
                float glass = Mathf.Sin(2f * Mathf.PI * 1174f * t) * Mathf.Exp(-t * 14f) * 0.45f;
                float wood = Mathf.Sin(2f * Mathf.PI * 220f * t) * Mathf.Exp(-t * 30f) * 0.5f;
                sum = glass + wood;
            }
            else if (type == "gala")
            {
                // Champagne flute crystalline ping
                float ping = Mathf.Sin(2f * Mathf.PI * 3520f * t) * Mathf.Exp(-t * 8f) * 0.5f;
                float ring = Mathf.Sin(2f * Mathf.PI * 7040f * t) * Mathf.Exp(-t * 18f) * 0.2f;
                sum = ping + ring;
            }
            else if (type == "golf")
            {
                // Titanium golf driver impact (CRACK-WHACK)
                float whack = Mathf.Sin(2f * Mathf.PI * 2800f * t) * Mathf.Exp(-t * 60f) * 0.6f;
                float club = Mathf.Sin(2f * Mathf.PI * 180f * t) * Mathf.Exp(-t * 22f) * 0.7f;
                float ballWhoosh = (Random.value * 2f - 1f) * Mathf.Sin(Mathf.PI * (t / duration)) * 0.2f;
                sum = whack + club + ballWhoosh;
            }
            else
            {
                // Startup pitch synth powerup
                float freq = Mathf.Lerp(440f, 880f, t / duration);
                float synth = Mathf.Sin(2f * Mathf.PI * freq * t) * Mathf.Exp(-t * 8f) * 0.45f;
                sum = synth;
            }

            samples[i] = Mathf.Clamp(sum, -1f, 1f);
        }

        var clip = AudioClip.Create($"Procedural_Networking_{type}", sampleCount, 1, SAMPLE_RATE, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip GenerateThemeShiftClip(int themeIndex)
    {
        float duration = 0.28f;
        int sampleCount = (int)(SAMPLE_RATE * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / SAMPLE_RATE;
            float sum = 0f;

            if (themeIndex == 0)
            {
                // Obsidian: Deep sub-bass singularity boom (55Hz -> 28Hz)
                float f = Mathf.Lerp(55f, 28f, t / duration);
                sum = Mathf.Sin(2f * Mathf.PI * f * t) * Mathf.Exp(-t * 9f) * 0.85f;
            }
            else if (themeIndex == 1)
            {
                // Slate: Futuristic cyber power-grid surge
                float f = Mathf.Lerp(300f, 650f, t / duration);
                sum = Mathf.Sin(2f * Mathf.PI * f * t) * Mathf.Exp(-t * 11f) * 0.6f;
            }
            else
            {
                // Paper: Crisp paper dossier slide
                float paper = (Random.value * 2f - 1f) * Mathf.Sin(Mathf.PI * (t / duration)) * 0.45f;
                float tap = Mathf.Sin(2f * Mathf.PI * 1800f * t) * Mathf.Exp(-t * 50f) * 0.35f;
                sum = paper + tap;
            }

            samples[i] = Mathf.Clamp(sum, -1f, 1f);
        }

        var clip = AudioClip.Create($"Procedural_ThemeShift_{themeIndex}", sampleCount, 1, SAMPLE_RATE, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip GeneratePopupOpenClip()
    {
        float duration = 0.22f;
        int sampleCount = (int)(SAMPLE_RATE * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / SAMPLE_RATE;
            float freq1 = Mathf.Lerp(523.25f, 783.99f, t / duration); // C5 -> G5
            float tone = Mathf.Sin(2f * Mathf.PI * freq1 * t) * Mathf.Exp(-t * 12f) * 0.4f;
            samples[i] = Mathf.Clamp(tone, -1f, 1f);
        }

        var clip = AudioClip.Create("Procedural_PopupOpen", sampleCount, 1, SAMPLE_RATE, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip GeneratePopupCloseClip()
    {
        float duration = 0.16f;
        int sampleCount = (int)(SAMPLE_RATE * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / SAMPLE_RATE;
            float freq = Mathf.Lerp(659.25f, 329.63f, t / duration); // E5 -> E4
            float tone = Mathf.Sin(2f * Mathf.PI * freq * t) * Mathf.Exp(-t * 18f) * 0.4f;
            samples[i] = Mathf.Clamp(tone, -1f, 1f);
        }

        var clip = AudioClip.Create("Procedural_PopupClose", sampleCount, 1, SAMPLE_RATE, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip GenerateSettingsToggleClip()
    {
        float duration = 0.05f;
        int sampleCount = (int)(SAMPLE_RATE * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / SAMPLE_RATE;
            float freq = Mathf.Lerp(1600f, 900f, t / duration);
            float tone = Mathf.Sin(2f * Mathf.PI * freq * t) * Mathf.Exp(-t * 80f) * 0.45f;
            samples[i] = Mathf.Clamp(tone, -1f, 1f);
        }

        var clip = AudioClip.Create("Procedural_SettingsToggle", sampleCount, 1, SAMPLE_RATE, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip GenerateCVOpenClip()
    {
        float duration = 0.32f;
        int sampleCount = (int)(SAMPLE_RATE * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / SAMPLE_RATE;
            float flap = Mathf.Sin(2f * Mathf.PI * 180f * t) * Mathf.Exp(-t * 22f) * 0.4f;
            float rustle = (Random.value * 2f - 1f) * Mathf.Sin(Mathf.PI * (t / duration)) * 0.35f;
            samples[i] = Mathf.Clamp(flap + rustle, -1f, 1f);
        }

        var clip = AudioClip.Create("Procedural_CVOpen", sampleCount, 1, SAMPLE_RATE, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip GenerateDebtWarningClip()
    {
        float duration = 0.65f;
        int sampleCount = (int)(SAMPLE_RATE * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / SAMPLE_RATE;
            float gong1 = Mathf.Sin(2f * Mathf.PI * 65.4f * t);
            float gong2 = Mathf.Sin(2f * Mathf.PI * 92.5f * t) * 0.5f;
            float env = Mathf.Exp(-t * 5.5f);
            samples[i] = Mathf.Clamp((gong1 + gong2) * env * 0.6f, -1f, 1f);
        }

        var clip = AudioClip.Create("Procedural_DebtWarning", sampleCount, 1, SAMPLE_RATE, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip GenerateIceShatterClip()
    {
        float duration = 0.28f;
        int sampleCount = (int)(SAMPLE_RATE * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / SAMPLE_RATE;
            float crack = (Random.value * 2f - 1f) * Mathf.Exp(-t * 30f) * 0.6f;
            float glass = Mathf.Sin(2f * Mathf.PI * 3135f * t) * Mathf.Exp(-t * 14f) * 0.4f;
            samples[i] = Mathf.Clamp(crack + glass, -1f, 1f);
        }

        var clip = AudioClip.Create("Procedural_IceShatter", sampleCount, 1, SAMPLE_RATE, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip GenerateBriefcaseSnapClip()
    {
        float duration = 0.14f;
        int sampleCount = (int)(SAMPLE_RATE * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / SAMPLE_RATE;
            float snap = Mathf.Sin(2f * Mathf.PI * 1850f * t) * Mathf.Exp(-t * 60f) * 0.7f;
            float latch = Mathf.Sin(2f * Mathf.PI * 420f * t) * Mathf.Exp(-t * 25f) * 0.4f;
            samples[i] = Mathf.Clamp(snap + latch, -1f, 1f);
        }

        var clip = AudioClip.Create("Procedural_BriefcaseSnap", sampleCount, 1, SAMPLE_RATE, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip GenerateLockpickPinClip()
    {
        float duration = 0.08f;
        int sampleCount = (int)(SAMPLE_RATE * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / SAMPLE_RATE;
            float pin = Mathf.Sin(2f * Mathf.PI * 2850f * t) * Mathf.Exp(-t * 90f) * 0.5f;
            samples[i] = Mathf.Clamp(pin, -1f, 1f);
        }

        var clip = AudioClip.Create("Procedural_LockpickPin", sampleCount, 1, SAMPLE_RATE, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip GenerateCenterDivSnapClip()
    {
        float duration = 0.15f;
        int sampleCount = (int)(SAMPLE_RATE * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / SAMPLE_RATE;
            float f = Mathf.Lerp(1200f, 2400f, t / duration);
            float ping = Mathf.Sin(2f * Mathf.PI * f * t) * Mathf.Exp(-t * 24f) * 0.45f;
            samples[i] = Mathf.Clamp(ping, -1f, 1f);
        }

        var clip = AudioClip.Create("Procedural_CenterDivSnap", sampleCount, 1, SAMPLE_RATE, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip GenerateCherrySwitchClip()
    {
        float duration = 0.07f;
        int sampleCount = (int)(SAMPLE_RATE * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / SAMPLE_RATE;
            float click = Mathf.Sin(2f * Mathf.PI * 2450f * t) * Mathf.Exp(-t * 110f) * 0.55f;
            float bottomOut = Mathf.Sin(2f * Mathf.PI * 340f * t) * Mathf.Exp(-t * 40f) * 0.35f;
            samples[i] = Mathf.Clamp(click + bottomOut, -1f, 1f);
        }

        var clip = AudioClip.Create("Procedural_CherrySwitch", sampleCount, 1, SAMPLE_RATE, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip GenerateFranticKeyClip()
    {
        float duration = 0.08f;
        int sampleCount = (int)(SAMPLE_RATE * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / SAMPLE_RATE;
            float slam = (Random.value * 2f - 1f) * Mathf.Exp(-t * 70f) * 0.5f;
            float spring = Mathf.Sin(2f * Mathf.PI * 1450f * t) * Mathf.Exp(-t * 30f) * 0.35f;
            samples[i] = Mathf.Clamp(slam + spring, -1f, 1f);
        }

        var clip = AudioClip.Create("Procedural_FranticKey", sampleCount, 1, SAMPLE_RATE, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip GenerateGitMergeClip(bool conflict)
    {
        float duration = conflict ? 0.38f : 0.28f;
        int sampleCount = (int)(SAMPLE_RATE * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / SAMPLE_RATE;
            if (conflict)
            {
                // Double klaxon
                float square = Mathf.Sin(2f * Mathf.PI * 220f * t) > 0f ? 0.4f : -0.4f;
                float env = Mathf.Exp(-t * 8f);
                samples[i] = square * env;
            }
            else
            {
                // Clean merge ping
                float ping = Mathf.Sin(2f * Mathf.PI * 1568f * t) * Mathf.Exp(-t * 14f) * 0.45f;
                samples[i] = ping;
            }
        }

        var clip = AudioClip.Create(conflict ? "Procedural_GitMerge_Conflict" : "Procedural_GitMerge_Clean", sampleCount, 1, SAMPLE_RATE, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip GenerateTetrisRotateClip()
    {
        float duration = 0.08f;
        int sampleCount = (int)(SAMPLE_RATE * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / SAMPLE_RATE;
            float f = Mathf.Lerp(800f, 1600f, t / duration);
            samples[i] = (Mathf.Sin(2f * Mathf.PI * f * t) > 0f ? 0.25f : -0.25f) * Mathf.Exp(-t * 40f);
        }

        var clip = AudioClip.Create("Procedural_TetrisRotate", sampleCount, 1, SAMPLE_RATE, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip GenerateTetrisDropClip()
    {
        float duration = 0.10f;
        int sampleCount = (int)(SAMPLE_RATE * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / SAMPLE_RATE;
            float f = Mathf.Lerp(400f, 120f, t / duration);
            samples[i] = Mathf.Sin(2f * Mathf.PI * f * t) * Mathf.Exp(-t * 30f) * 0.5f;
        }

        var clip = AudioClip.Create("Procedural_TetrisDrop", sampleCount, 1, SAMPLE_RATE, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip GenerateTetrisLineClearClip()
    {
        float duration = 0.35f;
        int sampleCount = (int)(SAMPLE_RATE * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / SAMPLE_RATE;
            float f = Mathf.Lerp(523f, 1046f, t / duration);
            float chord = Mathf.Sin(2f * Mathf.PI * f * t) + 0.3f * Mathf.Sin(3f * Mathf.PI * f * t);
            samples[i] = chord * Mathf.Exp(-t * 7f) * 0.45f;
        }

        var clip = AudioClip.Create("Procedural_TetrisLineClear", sampleCount, 1, SAMPLE_RATE, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip GenerateLawnMowerClip()
    {
        float duration = 0.20f;
        int sampleCount = (int)(SAMPLE_RATE * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / SAMPLE_RATE;
            float engine = Mathf.Sin(2f * Mathf.PI * 115f * t) * 0.4f + Mathf.Sin(2f * Mathf.PI * 230f * t) * 0.2f;
            float snip = (Random.value * 2f - 1f) * 0.25f;
            samples[i] = (engine + snip) * Mathf.Sin(Mathf.PI * (t / duration)) * 0.6f;
        }

        var clip = AudioClip.Create("Procedural_LawnMower", sampleCount, 1, SAMPLE_RATE, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip GenerateAcidRainShieldClip()
    {
        float duration = 0.12f;
        int sampleCount = (int)(SAMPLE_RATE * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / SAMPLE_RATE;
            float sizzle = (Random.value * 2f - 1f) * Mathf.Exp(-t * 45f) * 0.45f;
            float ping = Mathf.Sin(2f * Mathf.PI * 2400f * t) * Mathf.Exp(-t * 50f) * 0.35f;
            samples[i] = sizzle + ping;
        }

        var clip = AudioClip.Create("Procedural_AcidRainDeflect", sampleCount, 1, SAMPLE_RATE, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip GenerateCloggedVentClip()
    {
        float duration = 0.18f;
        int sampleCount = (int)(SAMPLE_RATE * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / SAMPLE_RATE;
            float pop = Mathf.Sin(2f * Mathf.PI * Mathf.Lerp(140f, 480f, t / duration) * t);
            float rattle = (Random.value * 2f - 1f) * 0.2f;
            samples[i] = (pop + rattle) * Mathf.Exp(-t * 20f) * 0.55f;
        }

        var clip = AudioClip.Create("Procedural_CloggedVent", sampleCount, 1, SAMPLE_RATE, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip GenerateIPHackClip()
    {
        float duration = 0.22f;
        int sampleCount = (int)(SAMPLE_RATE * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / SAMPLE_RATE;
            float step = Mathf.Floor(t * 30f);
            float f = 1200f + (step % 5f) * 350f;
            samples[i] = (Mathf.Sin(2f * Mathf.PI * f * t) > 0f ? 0.25f : -0.25f) * Mathf.Exp(-t * 9f);
        }

        var clip = AudioClip.Create("Procedural_IPHack", sampleCount, 1, SAMPLE_RATE, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip GenerateTurnstileSwipeClip()
    {
        float duration = 0.20f;
        int sampleCount = (int)(SAMPLE_RATE * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / SAMPLE_RATE;
            float beep = Mathf.Sin(2f * Mathf.PI * 2093f * t) * Mathf.Exp(-t * 25f) * 0.35f;
            float latch = Mathf.Sin(2f * Mathf.PI * 320f * t) * Mathf.Exp(-t * 30f) * 0.45f;
            samples[i] = beep + latch;
        }

        var clip = AudioClip.Create("Procedural_TurnstileSwipe", sampleCount, 1, SAMPLE_RATE, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip GenerateConstructionRivetClip()
    {
        float duration = 0.22f;
        int sampleCount = (int)(SAMPLE_RATE * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / SAMPLE_RATE;
            float bang = Mathf.Sin(2f * Mathf.PI * 140f * t) * Mathf.Exp(-t * 25f) * 0.8f;
            float metal = (Random.value * 2f - 1f) * Mathf.Exp(-t * 50f) * 0.5f;
            samples[i] = bang + metal;
        }

        var clip = AudioClip.Create("Procedural_ConstructionRivet", sampleCount, 1, SAMPLE_RATE, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip GenerateRecycleBottleClip()
    {
        float duration = 0.25f;
        int sampleCount = (int)(SAMPLE_RATE * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / SAMPLE_RATE;
            float glass1 = Mathf.Sin(2f * Mathf.PI * 2200f * t) * Mathf.Exp(-t * 22f) * 0.4f;
            float glass2 = Mathf.Sin(2f * Mathf.PI * 3400f * t) * Mathf.Exp(-t * 35f) * 0.3f;
            float clatter = (Random.value * 2f - 1f) * Mathf.Exp(-t * 18f) * 0.25f;
            samples[i] = glass1 + glass2 + clatter;
        }

        var clip = AudioClip.Create("Procedural_RecycleBottle", sampleCount, 1, SAMPLE_RATE, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip GenerateFoodChopClip()
    {
        float duration = 0.12f;
        int sampleCount = (int)(SAMPLE_RATE * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / SAMPLE_RATE;
            float chop = Mathf.Sin(2f * Mathf.PI * 260f * t) * Mathf.Exp(-t * 45f) * 0.65f;
            float knife = (Random.value * 2f - 1f) * Mathf.Exp(-t * 80f) * 0.35f;
            samples[i] = chop + knife;
        }

        var clip = AudioClip.Create("Procedural_FoodChop", sampleCount, 1, SAMPLE_RATE, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip GenerateEspressoSteamClip()
    {
        float duration = 0.30f;
        int sampleCount = (int)(SAMPLE_RATE * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / SAMPLE_RATE;
            float steam = (Random.value * 2f - 1f) * Mathf.Sin(Mathf.PI * (t / duration)) * 0.45f;
            float pump = Mathf.Sin(2f * Mathf.PI * 80f * t) * 0.2f;
            samples[i] = steam + pump;
        }

        var clip = AudioClip.Create("Procedural_EspressoSteam", sampleCount, 1, SAMPLE_RATE, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip GenerateATMDispenseClip()
    {
        float duration = 0.35f;
        int sampleCount = (int)(SAMPLE_RATE * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / SAMPLE_RATE;
            float roller = Mathf.Sin(2f * Mathf.PI * 340f * t) * (1f + 0.3f * Mathf.Sin(2f * Mathf.PI * 25f * t));
            float paper = (Random.value * 2f - 1f) * 0.2f;
            samples[i] = (roller * 0.4f + paper) * Mathf.Sin(Mathf.PI * (t / duration));
        }

        var clip = AudioClip.Create("Procedural_ATMDispense", sampleCount, 1, SAMPLE_RATE, false);
        clip.SetData(samples, 0);
        return clip;
    }

    #endregion
}

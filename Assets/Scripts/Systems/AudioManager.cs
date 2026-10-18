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
    }

    private void StartBaseBGM()
    {
        if (bgmPlaylist.Count > 0 && musicSource != null)
        {
            currentBgmIndex = 0;
            musicSource.clip = bgmPlaylist[currentBgmIndex];
            musicSource.volume = isMuted ? 0f : (musicVolume * masterVolume);
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
            musicSource.volume = musicVolume * masterVolume;
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

    #endregion
}

using UnityEngine;

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

    [Header("Audio Clip Overrides (Optional)")]
    [SerializeField] private AudioClip clickClip;
    [SerializeField] private AudioClip cashClip;
    [SerializeField] private AudioClip expenseClip;
    [SerializeField] private AudioClip turnAdvanceClip;
    [SerializeField] private AudioClip minigameSuccessClip;
    [SerializeField] private AudioClip minigameFailClip;
    [SerializeField] private AudioClip heartbeatClip;

    [Header("Volume Settings")]
    [Range(0f, 1f)] [SerializeField] private float sfxVolume = 1f;
    [SerializeField] private bool isMuted = false;

    private AudioSource sfxSource;
    private AudioSource heartbeatSource;

    private const string MUTE_PREF_KEY = "dear_audio_muted";
    private const string VOL_PREF_KEY = "dear_audio_volume";
    private const int SAMPLE_RATE = 44100;

    // Heartbeat pulse tracking
    private float nextHeartbeatTime = 0f;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioSources();
            LoadSettings();
            GenerateProceduralClips();
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void InitializeAudioSources()
    {
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
            sfxSource.spatialBlend = 0f; // 2D UI audio
        }

        if (heartbeatSource == null)
        {
            heartbeatSource = gameObject.AddComponent<AudioSource>();
            heartbeatSource.playOnAwake = false;
            heartbeatSource.spatialBlend = 0f;
        }
    }

    private void LoadSettings()
    {
        isMuted = PlayerPrefs.GetInt(MUTE_PREF_KEY, 0) == 1;
        sfxVolume = PlayerPrefs.GetFloat(VOL_PREF_KEY, 1f);
        ApplyVolumeSettings();
    }

    private void ApplyVolumeSettings()
    {
        if (sfxSource != null)
        {
            sfxSource.mute = isMuted;
            sfxSource.volume = sfxVolume;
        }
        if (heartbeatSource != null)
        {
            heartbeatSource.mute = isMuted;
            heartbeatSource.volume = sfxVolume;
        }
    }

    private void Update()
    {
        // Monitor critical stress heartbeat when stress > 80%
        if (GameManager.Instance != null && GameManager.Instance.currentData != null && !GameManager.Instance.isGameOver)
        {
            float stress = GameManager.Instance.currentData.stress;
            if (stress > 0.8f)
            {
                if (Time.unscaledTime >= nextHeartbeatTime)
                {
                    // Rate increases as stress rises towards 100%
                    float t = Mathf.Clamp01((stress - 0.8f) / 0.2f);
                    float interval = Mathf.Lerp(1.15f, 0.65f, t);
                    nextHeartbeatTime = Time.unscaledTime + interval;

                    PlayHeartbeat(stress);
                }
            }
        }
    }

    #region Procedural Audio Generators

    private void GenerateProceduralClips()
    {
        if (clickClip == null) clickClip = GenerateClickClip();
        if (cashClip == null) cashClip = GenerateCashClip();
        if (expenseClip == null) expenseClip = GenerateExpenseClip();
        if (turnAdvanceClip == null) turnAdvanceClip = GenerateTurnAdvanceClip();
        if (minigameSuccessClip == null) minigameSuccessClip = GenerateSuccessClip();
        if (minigameFailClip == null) minigameFailClip = GenerateFailClip();
        if (heartbeatClip == null) heartbeatClip = GenerateHeartbeatClip();
    }

    /// <summary>
    /// Subtle high-tech pip (short snappy blip)
    /// </summary>
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

    /// <summary>
    /// Crisp pleasant register / synth chime (multi-harmonic arpeggiated chime)
    /// </summary>
    private AudioClip GenerateCashClip()
    {
        float duration = 0.38f;
        int sampleCount = (int)(SAMPLE_RATE * duration);
        float[] samples = new float[sampleCount];

        float[] freqs = new float[] { 1318.5f, 1661.2f, 1975.5f }; // E6, G#6, B6
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

    /// <summary>
    /// Low pneumatic mechanical stamp
    /// </summary>
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
            
            // Sub-bass sine stamp
            float tone = Mathf.Sin(2f * Mathf.PI * freq * t) + 0.3f * Mathf.Sin(4f * Mathf.PI * freq * t);
            float env = Mathf.Exp(-t * 11f);

            // Subtle pneumatic hiss during the first 0.05s
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

    /// <summary>
    /// Digital shutter (turn advance)
    /// </summary>
    private AudioClip GenerateTurnAdvanceClip()
    {
        float duration = 0.25f;
        int sampleCount = (int)(SAMPLE_RATE * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / SAMPLE_RATE;
            float sum = 0f;

            // Phase 1: Leading shutter click (0 to 0.03s)
            if (t < 0.035f)
            {
                float clickFreq = Mathf.Lerp(2400f, 1200f, t / 0.035f);
                sum += Mathf.Sin(2f * Mathf.PI * clickFreq * t) * Mathf.Exp(-t * 90f) * 0.45f;
            }

            // Phase 2: Main digital shutter cycle (0.05s to 0.25s)
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

    /// <summary>
    /// Minigame success triumph chime
    /// </summary>
    private AudioClip GenerateSuccessClip()
    {
        float duration = 0.42f;
        int sampleCount = (int)(SAMPLE_RATE * duration);
        float[] samples = new float[sampleCount];

        // C5, E5, G5, C6 triumphant ascending fanfare
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

    /// <summary>
    /// Minigame fail dissonant tone
    /// </summary>
    private AudioClip GenerateFailClip()
    {
        float duration = 0.34f;
        int sampleCount = (int)(SAMPLE_RATE * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / SAMPLE_RATE;
            float progress = t / duration;

            // Two dissonant frequencies sweeping down
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

    /// <summary>
    /// Rhythmic dual pulse heartbeat ("lub-dub")
    /// </summary>
    private AudioClip GenerateHeartbeatClip()
    {
        float duration = 0.36f;
        int sampleCount = (int)(SAMPLE_RATE * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / SAMPLE_RATE;
            float sum = 0f;

            // Pulse 1: "Lub" (0.0s to 0.12s)
            if (t < 0.12f)
            {
                float freq = Mathf.Lerp(68f, 44f, t / 0.12f);
                float env = Mathf.Sin(Mathf.PI * (t / 0.12f)) * Mathf.Exp(-t * 12f);
                sum += Mathf.Sin(2f * Mathf.PI * freq * t) * env * 0.65f;
            }

            // Pulse 2: "Dub" (0.14s to 0.26s)
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

    #endregion

    #region Public Playback API

    public void PlayClick()
    {
        PlayClip(clickClip);
    }

    public void PlayCash()
    {
        PlayClip(cashClip);
    }

    public void PlayExpense()
    {
        PlayClip(expenseClip);
    }

    public void PlayTurnAdvance()
    {
        PlayClip(turnAdvanceClip);
    }

    public void PlayMinigameSuccess()
    {
        PlayClip(minigameSuccessClip);
    }

    public void PlayMinigameFail()
    {
        PlayClip(minigameFailClip);
    }

    public void PlayHeartbeat(float stressIntensity = 0.85f)
    {
        if (isMuted || heartbeatSource == null) return;
        if (heartbeatClip == null) heartbeatClip = GenerateHeartbeatClip();

        float vol = Mathf.Lerp(0.4f, 1.0f, Mathf.Clamp01((stressIntensity - 0.8f) / 0.2f)) * sfxVolume;
        heartbeatSource.PlayOneShot(heartbeatClip, vol);

        // Tactile heartbeat pulse
        if (stressIntensity > 0.92f)
        {
            HapticManager.HeavyImpact();
        }
        else
        {
            HapticManager.LightImpact();
        }
    }

    private void PlayClip(AudioClip clip)
    {
        if (isMuted || clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip, sfxVolume);
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

    public void SetVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat(VOL_PREF_KEY, sfxVolume);
        PlayerPrefs.Save();
        ApplyVolumeSettings();
    }

    public float GetVolume()
    {
        return sfxVolume;
    }

    #endregion
}

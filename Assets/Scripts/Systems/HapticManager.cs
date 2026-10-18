using UnityEngine;

public class HapticManager : MonoBehaviour
{
    public static HapticManager Instance { get; private set; }

    private const string HAPTICS_PREF_KEY = "dear_haptics_enabled";
    private static bool _hapticsEnabled = true;

    public static bool HapticsEnabled
    {
        get => _hapticsEnabled;
        set
        {
            _hapticsEnabled = value;
            PlayerPrefs.SetInt(HAPTICS_PREF_KEY, value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    private static AndroidJavaObject vibrator;
    private static AndroidJavaClass vibrationEffectClass;
    private static bool androidInitialized = false;
#endif

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitHaptics()
    {
        _hapticsEnabled = PlayerPrefs.GetInt(HAPTICS_PREF_KEY, 1) == 1;

#if UNITY_ANDROID && !UNITY_EDITOR
        InitAndroidVibrator();
#endif
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _hapticsEnabled = PlayerPrefs.GetInt(HAPTICS_PREF_KEY, 1) == 1;
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    private static void InitAndroidVibrator()
    {
        if (androidInitialized) return;
        try
        {
            using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
                vibrationEffectClass = new AndroidJavaClass("android.os.VibrationEffect");
            }
            androidInitialized = true;
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"[HapticManager] Android vibrator init warning: {ex.Message}");
            androidInitialized = false;
        }
    }
#endif

    /// <summary>
    /// Light impact: subtle vibration for button taps, tab switches, and interactions.
    /// </summary>
    public static void LightImpact()
    {
        if (!_hapticsEnabled) return;

#if UNITY_ANDROID && !UNITY_EDITOR
        TryAndroidVibrate(15, 60);
#elif UNITY_IOS && !UNITY_EDITOR
        Handheld.Vibrate();
#elif UNITY_STANDALONE || UNITY_EDITOR
        // Desktop / Editor: haptic no-op
#else
        Handheld.Vibrate();
#endif
    }

    /// <summary>
    /// Medium impact: rewarding vibration for minigame wins, paycheck arrivals, or achievements.
    /// </summary>
    public static void MediumImpact()
    {
        if (!_hapticsEnabled) return;

#if UNITY_ANDROID && !UNITY_EDITOR
        TryAndroidVibrate(40, 150);
#elif UNITY_IOS && !UNITY_EDITOR
        Handheld.Vibrate();
#elif UNITY_STANDALONE || UNITY_EDITOR
        // Desktop / Editor: haptic no-op
#else
        Handheld.Vibrate();
#endif
    }

    /// <summary>
    /// Heavy impact: intense vibration for arrests, job termination, heart attacks, or stress critical warnings.
    /// </summary>
    public static void HeavyImpact()
    {
        if (!_hapticsEnabled) return;

#if UNITY_ANDROID && !UNITY_EDITOR
        TryAndroidVibrate(100, 255);
#elif UNITY_IOS && !UNITY_EDITOR
        Handheld.Vibrate();
#elif UNITY_STANDALONE || UNITY_EDITOR
        // Desktop / Editor: haptic no-op
#else
        Handheld.Vibrate();
#endif
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    private static void TryAndroidVibrate(long milliseconds, int amplitude)
    {
        try
        {
            if (vibrator != null)
            {
                if (vibrationEffectClass != null)
                {
                    // API 26+ support with amplitude
                    var effect = vibrationEffectClass.CallStatic<AndroidJavaObject>("createOneShot", milliseconds, Mathf.Clamp(amplitude, 1, 255));
                    vibrator.Call("vibrate", effect);
                    return;
                }

                // Fallback for older API levels
                vibrator.Call("vibrate", milliseconds);
                return;
            }
        }
        catch
        {
            // Silent fallback to Unity standard
        }

        Handheld.Vibrate();
    }
#endif

    public static void SetHapticsEnabled(bool enabled)
    {
        HapticsEnabled = enabled;
    }

    public static bool IsHapticsEnabled()
    {
        return HapticsEnabled;
    }
}

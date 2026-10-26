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
#elif UNITY_IOS && !UNITY_EDITOR
    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void _iOS_TriggerImpact(int style);

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void _iOS_TriggerSelection();

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void _iOS_TriggerNotification(int type);
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
    /// Light impact: subtle Taptic vibration for button taps, tab switches, and interactions.
    /// </summary>
    public static void LightImpact()
    {
        if (!_hapticsEnabled) return;

#if UNITY_ANDROID && !UNITY_EDITOR
        TryAndroidVibrate(15, 60);
#elif UNITY_IOS && !UNITY_EDITOR
        _iOS_TriggerImpact(0); // UIImpactFeedbackStyleLight
#elif UNITY_STANDALONE || UNITY_EDITOR
        // Desktop / Editor: haptic no-op
#else
        Handheld.Vibrate();
#endif
    }

    /// <summary>
    /// Selection click: extremely subtle, crisp haptic for dial ticks, item drags, or list scrolling.
    /// </summary>
    public static void SelectionClick()
    {
        if (!_hapticsEnabled) return;

#if UNITY_ANDROID && !UNITY_EDITOR
        TryAndroidVibrate(8, 30);
#elif UNITY_IOS && !UNITY_EDITOR
        _iOS_TriggerSelection();
#elif UNITY_STANDALONE || UNITY_EDITOR
        // Desktop / Editor: haptic no-op
#endif
    }

    /// <summary>
    /// Subtle pulse specifically for high-stress heartbeat - maintains rapid rhythm without jarring motor shake.
    /// </summary>
    public static void HeartbeatPulse()
    {
        if (!_hapticsEnabled) return;

#if UNITY_ANDROID && !UNITY_EDITOR
        TryAndroidVibrate(12, 45);
#elif UNITY_IOS && !UNITY_EDITOR
        _iOS_TriggerSelection();
#elif UNITY_STANDALONE || UNITY_EDITOR
        // Desktop / Editor: haptic no-op
#endif
    }

    /// <summary>
    /// Medium impact: rewarding Taptic vibration for minigame wins, paycheck arrivals, or achievements.
    /// </summary>
    public static void MediumImpact()
    {
        if (!_hapticsEnabled) return;

#if UNITY_ANDROID && !UNITY_EDITOR
        TryAndroidVibrate(40, 150);
#elif UNITY_IOS && !UNITY_EDITOR
        _iOS_TriggerImpact(1); // UIImpactFeedbackStyleMedium
#elif UNITY_STANDALONE || UNITY_EDITOR
        // Desktop / Editor: haptic no-op
#else
        Handheld.Vibrate();
#endif
    }

    /// <summary>
    /// Heavy impact: intense Taptic vibration for arrests, job termination, or critical failures.
    /// </summary>
    public static void HeavyImpact()
    {
        if (!_hapticsEnabled) return;

#if UNITY_ANDROID && !UNITY_EDITOR
        TryAndroidVibrate(100, 255);
#elif UNITY_IOS && !UNITY_EDITOR
        _iOS_TriggerImpact(2); // UIImpactFeedbackStyleHeavy
#elif UNITY_STANDALONE || UNITY_EDITOR
        // Desktop / Editor: haptic no-op
#else
        Handheld.Vibrate();
#endif
    }

    /// <summary>
    /// Haptic notification (0 = Success, 1 = Warning, 2 = Error).
    /// </summary>
    public static void NotificationFeedback(int type)
    {
        if (!_hapticsEnabled) return;

#if UNITY_ANDROID && !UNITY_EDITOR
        if (type == 0) TryAndroidVibrate(35, 120);
        else if (type == 1) TryAndroidVibrate(60, 180);
        else TryAndroidVibrate(100, 255);
#elif UNITY_IOS && !UNITY_EDITOR
        _iOS_TriggerNotification(type);
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

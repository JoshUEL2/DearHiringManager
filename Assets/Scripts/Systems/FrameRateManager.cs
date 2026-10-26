using UnityEngine;

/// <summary>
/// Configures high refresh rate rendering (120Hz ProMotion for iPhone 17 Pro and 120Hz Android displays).
/// Runs automatically before any scene loads.
/// </summary>
public static class FrameRateManager
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void InitializePerformance()
    {
        // Disable VSync so targetFrameRate is respected
        QualitySettings.vSyncCount = 0;

        // Query device display refresh rate
        int deviceRefresh = 120;
        try
        {
            var ratio = Screen.currentResolution.refreshRateRatio;
            if (ratio.value > 0)
            {
                deviceRefresh = Mathf.RoundToInt((float)ratio.value);
            }
        }
        catch
        {
            deviceRefresh = 120;
        }

        // Clamp to at least 120 FPS for buttery smooth 120Hz ProMotion
        int targetFPS = Mathf.Max(120, deviceRefresh);
        Application.targetFrameRate = targetFPS;

        // Prevent display from dimming while playing
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        Debug.Log($"[FrameRateManager] Target frame rate configured to {targetFPS} FPS (Device reported refresh: {deviceRefresh}Hz).");
    }
}

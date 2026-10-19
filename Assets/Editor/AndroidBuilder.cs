using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System.IO;

public static class AndroidBuilder
{
    [MenuItem("Build/Build Android APK")]
    public static void BuildAPK()
    {
        Debug.Log("[AndroidBuilder] Starting Android APK Build pipeline...");

        // Ensure directories exist
        string outputDir = Path.Combine(Directory.GetCurrentDirectory(), "Builds", "Android");
        if (!Directory.Exists(outputDir))
        {
            Directory.CreateDirectory(outputDir);
        }
        string apkPath = Path.Combine(outputDir, "DearHiringManager.apk");

        // Player & Android Configurations
        PlayerSettings.companyName = "24-Hour Studios";
        PlayerSettings.productName = "Dear Hiring Manager";
        PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, "com.TwentyFourHourStudios.DearHiringManager");
        PlayerSettings.bundleVersion = "1.0.0";
        PlayerSettings.Android.bundleVersionCode = 1;
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel26;
        PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
        
        // Ensure standalone APK (not Google Play App Bundle) for direct sideloading and itch.io
        EditorUserBuildSettings.buildAppBundle = false;
        EditorUserBuildSettings.androidBuildSubtarget = MobileTextureSubtarget.Generic;

        // Scenes to include
        string[] scenes = new string[] { "Assets/Scenes/Game.unity" };

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = apkPath,
            target = BuildTarget.Android,
            options = BuildOptions.None
        };

        Debug.Log($"[AndroidBuilder] Building APK to: {apkPath}");
        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"[AndroidBuilder] BUILD SUCCESS! Size: {summary.totalSize / (1024f * 1024f):F2} MB in {summary.totalTime.TotalSeconds:F1}s");
            if (Application.isBatchMode)
            {
                EditorApplication.Exit(0);
            }
        }
        else
        {
            Debug.LogError($"[AndroidBuilder] BUILD FAILED! Result: {summary.result}, Errors: {summary.totalErrors}");
            if (Application.isBatchMode)
            {
                EditorApplication.Exit(1);
            }
        }
    }
}

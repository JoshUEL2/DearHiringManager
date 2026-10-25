using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System.IO;

public static class IOSBuilder
{
    [MenuItem("Build/Build iOS Xcode Project")]
    public static void BuildIOS()
    {
        Debug.Log("[IOSBuilder] Starting iOS Xcode Project export pipeline...");

        string outputDir = Path.Combine(Directory.GetCurrentDirectory(), "Builds", "iOS");
        if (!Directory.Exists(outputDir))
        {
            Directory.CreateDirectory(outputDir);
        }

        // Player & iOS Configurations
        PlayerSettings.companyName = "24-Hour Studios";
        PlayerSettings.productName = "Dear Hiring Manager";
        PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.iOS, "com.TwentyFourHourStudios.DearHiringManager");
        PlayerSettings.bundleVersion = "1.0.1";
        PlayerSettings.iOS.buildNumber = "2";
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
        PlayerSettings.allowedAutorotateToPortrait = true;
        PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
        PlayerSettings.allowedAutorotateToLandscapeLeft = false;
        PlayerSettings.allowedAutorotateToLandscapeRight = false;

        // Scenes to include
        string[] scenes = new string[] { "Assets/Scenes/Game.unity" };

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = outputDir,
            target = BuildTarget.iOS,
            options = BuildOptions.None
        };

        Debug.Log($"[IOSBuilder] Exporting Xcode project to: {outputDir}");
        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"[IOSBuilder] EXPORT SUCCESS! Total time: {summary.totalTime.TotalSeconds:F1}s");
            if (Application.isBatchMode)
            {
                EditorApplication.Exit(0);
            }
        }
        else
        {
            Debug.LogError($"[IOSBuilder] EXPORT FAILED! Result: {summary.result}, Errors: {summary.totalErrors}");
            if (Application.isBatchMode)
            {
                EditorApplication.Exit(1);
            }
        }
    }
}

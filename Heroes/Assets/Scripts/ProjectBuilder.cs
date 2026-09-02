#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

internal static class ProjectBuilder
{
    private static readonly string Name = PlayerSettings.productName.Replace(" ", "_");

    [MenuItem("Build/Android/Build Develop")]
    private static void BuildDevelopAndroid()
    {
        SetPLayerSettings();
        Build(false, BuildTarget.Android);
    }

    [MenuItem("Build/Build Release")]
    private static void BuildReleaseAndroid()
    {
        SetPLayerSettings();
        Build(true, BuildTarget.Android);
    }

    [MenuItem("Build/iOS/Build Develop")]
    private static void BuildDevelopIos()
    {
        SetPLayerSettings();
        EditorUserBuildSettings.iOSBuildConfigType = iOSBuildType.Release;
        Build(false, BuildTarget.iOS);
    }

    [MenuItem("Build/iOS/Build Release")]
    private static void BuildReleaseIos()
    {
        SetPLayerSettings();
        EditorUserBuildSettings.iOSBuildConfigType = iOSBuildType.Release;
        Build(true, BuildTarget.iOS);
    }

    private static void Build(bool isReleaseBuild, BuildTarget buildTarget)
    {
        BuildOptions isRelease = isReleaseBuild ? BuildOptions.None : BuildOptions.Development;
        string endOfString = buildTarget == BuildTarget.Android ? ".apk" : String.Empty;

        BuildReport report = BuildPipeline.BuildPlayer(EditorBuildSettings.scenes,
            $"./{Name}_{Enum.GetName(typeof(BuildTarget), buildTarget)}/{Name}{endOfString}", buildTarget, isRelease);

        LogBuildResult(report);
    }

    private static void SetPLayerSettings()
    {
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
    }

    private static void LogBuildResult(BuildReport report)
    {
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
            Debug.Log($"Build succeeded: {summary.totalSize} bytes");

        if (summary.result == BuildResult.Failed)
            Debug.Log("Build failed");
    }
}
#endif
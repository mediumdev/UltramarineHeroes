#if UNITY_ANDROID
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DefaultNamespace;
using UnityEditor;
using UnityEngine;

public class BuildPreProcessor : MonoBehaviour
{
    public static void ChangeAndroidBundleVersionCode()
    {
        var allTags = GitCommandRunner.RunGitCommand("tag", "",
            Directory.GetCurrentDirectory());

        var androidBundleVersionCodeTags = new List<int>();
        foreach (Match match in Regex.Matches(allTags, @"(android_bundle_version_code-\w*?([^,]*))"))
        {
            androidBundleVersionCodeTags.Add(Int32.Parse(match.Groups[2].Value));
        }

        var currentBundleCodeVersion = androidBundleVersionCodeTags.Max();
        PlayerSettings.Android.bundleVersionCode = Convert.ToInt32(currentBundleCodeVersion);
        Debug.Log(currentBundleCodeVersion);
    }
}

#endif
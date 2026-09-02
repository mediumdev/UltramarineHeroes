namespace UnityEngine.CloudBuild.API
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
#if UNITY_CLOUD_BUILD

    using UnityEditor;
    using UnityEngine.CloudBuild;
	using UnityEngine.CloudBuild.API;

#endif

    public class CloudBuildAPI
    {
        public static CloudBuildManifest GetBuildManifest()
        {
#if  UNITY_CLOUD_BUILD
            var manifest = (BuildManifestObject)AssetDatabase.LoadAssetAtPath("Assets/__UnityCloud__/Resources/UnityCloudBuildManifest.scriptable.asset", typeof(BuildManifestObject));
            return new CloudBuildManifest(manifest);
#else
            var result = new CloudBuildManifest();
            result.ProjectId = "com.UltramarineGames.Heroes";
            result.ScmBranch = "master";
            result.ScmCommitId = "9f0070c638b84e281155a41dcf04ce55173355d0";
            result.UnityVersion = Application.unityVersion;
            result.XCodeVersion = "12.4";
            result.BuildNumber = "1";
            result.BuildStartTime = (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds.ToString();
            result.BundleId = "com.UltramarineGames.Heroes";
            return result;
#endif
        }
    }
}
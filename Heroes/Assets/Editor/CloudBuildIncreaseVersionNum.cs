using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections;
using System.Collections.Generic;

#if UNITY_IOS && UNITY_CLOUD_BUILD
using UnityEditor.iOS.Xcode;
using System.IO;
#endif

public class CloudBuildIncreaseVersionNum
{
    public class ChangeIOSBuildNumber
    {
#if UNITY_IOS && UNITY_CLOUD_BUILD
        [PostProcessBuild]
        public static void ChangeXcodePlist(BuildTarget buildTarget, string pathToBuiltProject) {

            if (buildTarget == BuildTarget.iOS) {
                // Get plist
                string plistPath = pathToBuiltProject + "/Info.plist";
                PlistDocument plist = new PlistDocument();
                plist.ReadFromString(File.ReadAllText(plistPath));
       
                // Get root
                PlistElementDict rootDict = plist.root;
       

                // browse manifest
                var cloudbuildVersionValue = UnityEngine.CloudBuild.API.CloudBuildAPI.GetBuildManifest().BuildNumber;
		

                // Change value of CFBundleVersion in Xcode plist
                var buildKey = "CFBundleVersion";
                rootDict.SetString(buildKey,cloudbuildVersionValue);
       
                // Write to file
                File.WriteAllText(plistPath, plist.WriteToString());
            }
        
        }
#endif
    }
}
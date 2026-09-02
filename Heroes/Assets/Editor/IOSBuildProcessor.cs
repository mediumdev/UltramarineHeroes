#if UNITY_IOS
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEngine;

public class BuildPostProcess : MonoBehaviour
{
    [PostProcessBuild(1)]
    public static void ChangeXcodePlist(BuildTarget buildTarget, string pathToBuiltProject)
    {
        var plistPath = $"{pathToBuiltProject}/Info.plist";
        const string TrackingDescription = "Your data will be used to provide you a better and personalized ad experience.";
        Debug.Log($"Plist path is {plistPath}");
        PlistDocument plist = new PlistDocument();
        plist.ReadFromString(File.ReadAllText(plistPath));

        Debug.Log("Get root dict");
        PlistElementDict rootDict = plist.root;
        // For AdMob
        // rootDict.SetString("GADApplicationIdentifier", "ca-app-pub-1681008080188496~5913934402");
        rootDict.SetString("SKAdNetworkIdentifier", "4dzt52r2t5.skadnetwork");
        
        // Set the description key-value in the plist:
        rootDict.SetString("NSUserTrackingUsageDescription", TrackingDescription);

        Debug.Log("Write all stuff");
        File.WriteAllText(plistPath, plist.WriteToString());
    }

    public static void AddCapabilities(BuildTarget buildTarget, string pathToBuiltProject)
    {
        if (buildTarget == BuildTarget.iOS)
        {
            string projPath = pathToBuiltProject + "/Unity-iPhone.xcodeproj/project.pbxproj";

            PBXProject proj = new PBXProject();
            proj.ReadFromString(File.ReadAllText(projPath));

            string target = proj.TargetGuidByName("Unity-iPhone");

            var entitlementsFileName = proj.GetBuildPropertyForAnyConfig(target, "CODE_SIGN_ENTITLEMENTS") ??
                                       Application.identifier + ".entitlements";
            var capManager = new ProjectCapabilityManager(projPath, entitlementsFileName, "Unity-iPhone");
            capManager.AddGameCenter();
            capManager.AddInAppPurchase();
            capManager.AddPushNotifications(false);
            capManager.AddBackgroundModes(BackgroundModesOptions.RemoteNotifications);
            capManager.WriteToFile();
            File.WriteAllText(projPath, proj.WriteToString());
        }
    }
}
#endif
#if UNITY_IOS

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
using UnityEditor.iOS.Xcode;

public class AegisPostProcess : MonoBehaviour
{
    [PostProcessBuildAttribute(1)]
    public static void OnPostprocessBuild(BuildTarget buildTarget, string buildPath)
    {
        if (buildTarget == BuildTarget.iOS)
        {
            UpdateProject(buildTarget, buildPath + "/Unity-iPhone.xcodeproj/project.pbxproj");
            UpdateProjectPlist(buildTarget, buildPath + "/Info.plist");

            UpdateCapabilities(buildTarget,
                                 buildPath + "/Unity-iPhone.xcodeproj/project.pbxproj",
                                 buildPath + "/Unity-iPhone/project.entitlements");
        }
    }

    private static void UpdateProject(BuildTarget buildTarget, string projectPath)
    {
        PBXProject project = new PBXProject();
        project.ReadFromString(File.ReadAllText(projectPath));

        string targetId = project.TargetGuidByName(PBXProject.GetUnityTargetName());
        //Add your code here - BEGIN
		project.AddBuildProperty(targetId, "OTHER_LDFLAGS", "-ObjC");
        project.SetBuildProperty(targetId, "CLANG_ENABLE_MODULES", "YES");
        project.SetBuildProperty(targetId, "GCC_ENABLE_OBJC_EXCEPTIONS", "YES");
		project.SetBuildProperty(targetId, "ENABLE_BITCODE", "YES");
		//Add your code here - END

        File.WriteAllText(projectPath, project.WriteToString());
    }

    private static void UpdateProjectPlist(BuildTarget buildTarget, string plistPath)
    {
        PlistDocument plist = new PlistDocument();
        plist.ReadFromString(File.ReadAllText(plistPath));

        // Get root
        PlistElementDict rootDict = plist.root;

		//Add your code here - BEGIN

        rootDict.SetBoolean("ITSAppUsesNonExemptEncryption", false);

        // remove exit on suspend if it exists
        // Ref: https://forum.unity.com/threads/the-info-plist-contains-a-key-uiapplicationexitsonsuspend.689200/
        string exitsOnSuspendKey = "UIApplicationExitsOnSuspend";
        if(rootDict.values.ContainsKey(exitsOnSuspendKey))
        {
            rootDict.values.Remove(exitsOnSuspendKey);
        }

        //Add your code here - END

        File.WriteAllText(plistPath, plist.WriteToString());
    }

    private static void UpdateCapabilities(BuildTarget buildTarget, string projectPath, string entitlementPath)
    {
		var scheme = "Unity-iPhone";
		var pcm = new ProjectCapabilityManager(projectPath, entitlementPath, scheme);

		//Add your code here - BEGIN
		pcm.AddInAppPurchase();
		pcm.AddPushNotifications(development: true);
		pcm.AddGameCenter();
		//Add your code here - END

		pcm.WriteToFile();
    }
}

#endif

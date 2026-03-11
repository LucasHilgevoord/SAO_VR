#if USING_XR_MANAGEMENT && USING_OPENXR_1_16

#if UNITY_EDITOR
using HurricaneVR.Editor;
using HurricaneVR.Framework.Core.Utils;
using UnityEditor;
using UnityEditor.XR.OpenXR.Features;
using UnityEngine;
using UnityEngine.XR.OpenXR.Features;

[InitializeOnLoad]
public static class WorkaroundForOXRB656Editor
{
    // The key used to store the user's preference in the Editor
    private const string WarningDisabledKey = "MyCustomWarningDisabled";

    // The specific problem this warning is addressing
    private const string ProblemMessage = "Warning: Your OpenXR version is affected by issue OXRB-656 which will lead to Quest 3 controller rotations flipping. Press Apply Fix to enable the workaround OpenXR Feature which can be found in the Project Settings -> OpenXR Settings.";

    // Static constructor runs when the editor loads
    static WorkaroundForOXRB656Editor()
    {
        // Subscribe to the event that fires immediately after assemblies finish reloading
        AssemblyReloadEvents.afterAssemblyReload += ShowWarningPopup;
    }

    private static void ShowWarningPopup()
    {
        var buildTargetGroup = UnityEditor.BuildPipeline.GetBuildTargetGroup(UnityEditor.EditorUserBuildSettings.activeBuildTarget);

        var workaroundFeature = FeatureHelpers.GetFeatureWithIdForBuildTarget(buildTargetGroup, WorkaroundForOXRB656.featureId);

        if (!workaroundFeature || (workaroundFeature && workaroundFeature.enabled))
            return;

        if(HVREditorPreferences.DoNotShowOpenXRQuest3Warning)
            return;

        bool applyFix = EditorUtility.DisplayDialog(
            "HVR OpenXR Quest 3 Warning!", // Title
            ProblemMessage, // Message
            "Apply Fix", // OK Button (returns true)
            "Do not show again" // Cancel Button (returns false)
        );

        if (applyFix)
        {
            workaroundFeature.enabled = true;
            Debug.Log("Fix applied! The OpenXR Feature has been enabled.");
        }
        else
        {
            HVREditorPreferences.DoNotShowOpenXRQuest3Warning = true;
            Debug.Log("Assembly Reload Warning disabled. You can re-enable it via the 'Reset Warning' menu.");
        }
    }
}
#endif
#endif
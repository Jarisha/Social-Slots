using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Linq;

public class BuildScript {
	
	public static void BuildAndroid() {
		var args = System.Environment.GetCommandLineArgs();
		var outputLocation = args[args.Length - 1];
		var currentSetting = EditorUserBuildSettings.activeBuildTarget;
		var scenes = (from scene in EditorBuildSettings.scenes where scene.enabled select scene.path).ToArray();
		EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.Android);
		BuildPipeline.BuildPlayer(scenes, outputLocation, BuildTarget.Android, BuildOptions.None);
		EditorUserBuildSettings.SwitchActiveBuildTarget(currentSetting);
	}
	
	public static void BuildIphone() {
		var currentSetting = EditorUserBuildSettings.activeBuildTarget;
		var scenes = (from scene in EditorBuildSettings.scenes where scene.enabled select scene.path).ToArray();
		EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.iPhone);
		EditorUserBuildSettings.appendProject = true;
		var outputLocation = EditorUserBuildSettings.GetBuildLocation(BuildTarget.iPhone);
		BuildPipeline.BuildPlayer(scenes, outputLocation, BuildTarget.iPhone, 
			BuildOptions.SymlinkLibraries | BuildOptions.AcceptExternalModificationsToPlayer);
		EditorUserBuildSettings.SwitchActiveBuildTarget(currentSetting);
	}
}

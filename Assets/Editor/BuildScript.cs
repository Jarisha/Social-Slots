using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public enum BuildTypes {
	CasinoSlots,
	SocialSlots
}

public class BuildScript {
	
	public static Dictionary<BuildTypes, string> BUNDLE_IDENTIFIERS = new Dictionary<BuildTypes, string>() {
		{BuildTypes.CasinoSlots, "com.slimstown.slots"},
		{BuildTypes.SocialSlots, "com.slimstown.socialslots"}
	};
	
	public static Dictionary<BuildTypes, string> XCODE_LOCATIONS = new Dictionary<BuildTypes, string>() {
		{BuildTypes.CasinoSlots, "xcode_casino"},
		{BuildTypes.SocialSlots, "xcode_social"}
	};
	
	public static void SetRotationOptions(bool portrait, bool landscape) {
		PlayerSettings.allowedAutorotateToLandscapeLeft = landscape;
		PlayerSettings.allowedAutorotateToLandscapeRight = landscape;
		PlayerSettings.allowedAutorotateToPortrait = portrait;
		PlayerSettings.allowedAutorotateToPortraitUpsideDown = portrait;
	}
	
	public static void BuildAndroidCasino() {
		var args = System.Environment.GetCommandLineArgs();
		var outputLocation = args[args.Length - 1];
		SetRotationOptions (false, true);
		PlayerSettings.bundleIdentifier = BUNDLE_IDENTIFIERS[BuildTypes.CasinoSlots];
		var currentSetting = EditorUserBuildSettings.activeBuildTarget;
		var scenes = (from scene in EditorBuildSettings.scenes where scene.enabled select scene.path).ToArray();
		EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.Android);
		BuildPipeline.BuildPlayer(scenes, outputLocation, BuildTarget.Android, BuildOptions.None);
		EditorUserBuildSettings.SwitchActiveBuildTarget(currentSetting);
	}
		
	public static void BuildAndroidSocial() {
		var args = System.Environment.GetCommandLineArgs();
		var outputLocation = args[args.Length - 1];
		SetRotationOptions (true, false);
		PlayerSettings.bundleIdentifier = BUNDLE_IDENTIFIERS[BuildTypes.SocialSlots];
		var currentSetting = EditorUserBuildSettings.activeBuildTarget;
		var scenes = (from scene in EditorBuildSettings.scenes where scene.enabled select scene.path).ToArray();
		EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.Android);
		BuildPipeline.BuildPlayer(scenes, outputLocation, BuildTarget.Android, BuildOptions.None);
		EditorUserBuildSettings.SwitchActiveBuildTarget(currentSetting);
	}
	
	public static void BuildIphoneCasino() {
		var currentSetting = EditorUserBuildSettings.activeBuildTarget;
		var scenes = (from scene in EditorBuildSettings.scenes where scene.enabled select scene.path).ToArray();
		EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.iPhone);
		EditorUserBuildSettings.appendProject = true;
		var outputLocation = EditorUserBuildSettings.GetBuildLocation(BuildTarget.iPhone);
		outputLocation = outputLocation.Replace("xcode", XCODE_LOCATIONS[BuildTypes.CasinoSlots]);
		SetRotationOptions (false, true);
		PlayerSettings.bundleIdentifier = BUNDLE_IDENTIFIERS[BuildTypes.CasinoSlots];
		BuildPipeline.BuildPlayer(scenes, outputLocation, BuildTarget.iPhone, 
			BuildOptions.SymlinkLibraries | BuildOptions.AcceptExternalModificationsToPlayer);
		EditorUserBuildSettings.SwitchActiveBuildTarget(currentSetting);
	}
	
	public static void BuildIphoneSocial() {
		var currentSetting = EditorUserBuildSettings.activeBuildTarget;
		var scenes = (from scene in EditorBuildSettings.scenes where scene.enabled select scene.path).ToArray();
		EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.iPhone);
		EditorUserBuildSettings.appendProject = true;
		var outputLocation = EditorUserBuildSettings.GetBuildLocation(BuildTarget.iPhone);
		outputLocation = outputLocation.Replace("xcode", XCODE_LOCATIONS[BuildTypes.SocialSlots]);
		SetRotationOptions (true, false);
		PlayerSettings.bundleIdentifier = BUNDLE_IDENTIFIERS[BuildTypes.SocialSlots];
		BuildPipeline.BuildPlayer(scenes, outputLocation, BuildTarget.iPhone, 
			BuildOptions.SymlinkLibraries | BuildOptions.AcceptExternalModificationsToPlayer);
		EditorUserBuildSettings.SwitchActiveBuildTarget(currentSetting);
	}
}

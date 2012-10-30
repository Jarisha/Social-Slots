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
	
	public static Dictionary<BuildTypes, string> PRODUCT_NAMES = new Dictionary<BuildTypes, string>() {
		{BuildTypes.CasinoSlots, "Slots"},
		{BuildTypes.SocialSlots, "Social Slots"}
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
		PlayerSettings.defaultInterfaceOrientation = portrait ? UIOrientation.Portrait : UIOrientation.LandscapeLeft;
	}
	
	public static void BuildAndroidCasino() {
		BuildAndroid (BuildTypes.CasinoSlots);
	}
		
	public static void BuildAndroidSocial() {
		BuildAndroid (BuildTypes.SocialSlots);
	}
	
	public static void BuildIphoneCasino() {
		BuildIphone (BuildTypes.CasinoSlots);
	}
	
	public static void BuildIphoneSocial() {
		BuildIphone (BuildTypes.SocialSlots);
	}
	
	static void BuildAndroid(BuildTypes type) {
		bool isCasino = (type == BuildTypes.CasinoSlots);
		var currentSetting = EditorUserBuildSettings.activeBuildTarget;
		var scenes = (from scene in EditorBuildSettings.scenes where scene.enabled select scene.path).ToArray();
		EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.Android);
		
		var args = System.Environment.GetCommandLineArgs();
		var outputLocation = args[args.Length - 1];
		
		SetRotationOptions (!isCasino, isCasino);
		PlayerSettings.productName = PRODUCT_NAMES[type];
		PlayerSettings.bundleIdentifier = BUNDLE_IDENTIFIERS[type];
		GameObject.Find ("Init").GetComponent<InitScript>().IS_CASINO_BUILD = isCasino;
		
		BuildPipeline.BuildPlayer(scenes, outputLocation, BuildTarget.Android, BuildOptions.None);
		EditorUserBuildSettings.SwitchActiveBuildTarget(currentSetting);
	}
	
	static void BuildIphone(BuildTypes type) {
		bool isCasino = (type == BuildTypes.CasinoSlots);
		var currentSetting = EditorUserBuildSettings.activeBuildTarget;
		var scenes = (from scene in EditorBuildSettings.scenes where scene.enabled select scene.path).ToArray();
		EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.iPhone);
		
		EditorUserBuildSettings.appendProject = true;
		var outputLocation = EditorUserBuildSettings.GetBuildLocation(BuildTarget.iPhone);
		outputLocation = ReplaceXcodeWith(outputLocation, XCODE_LOCATIONS[type]);
		
		SetRotationOptions (!isCasino, isCasino);
		PlayerSettings.productName = PRODUCT_NAMES[type];
		PlayerSettings.bundleIdentifier = BUNDLE_IDENTIFIERS[type];
		GameObject.Find ("Init").GetComponent<InitScript>().IS_CASINO_BUILD = isCasino;
		
		BuildPipeline.BuildPlayer(scenes, outputLocation, BuildTarget.iPhone, BuildOptions.SymlinkLibraries);
		EditorUserBuildSettings.SwitchActiveBuildTarget(currentSetting);
	}
	
	static string ReplaceXcodeWith(string baseString, string replacementValue) {
		if(baseString.Contains (XCODE_LOCATIONS[BuildTypes.CasinoSlots])) {
			baseString = baseString.Replace(XCODE_LOCATIONS[BuildTypes.CasinoSlots], replacementValue);
		}
		else if(baseString.Contains (XCODE_LOCATIONS[BuildTypes.SocialSlots])) {
			baseString = baseString.Replace(XCODE_LOCATIONS[BuildTypes.SocialSlots], replacementValue);
		}
		else if(baseString.Contains ("xcode")) {
			baseString = baseString.Replace ("xcode", replacementValue);
		}
		
		return baseString;
	}
}

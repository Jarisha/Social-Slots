using UnityEngine;
using System;
using System.Collections.Generic;

[System.Serializable]
public class ResolutionAdjustObjects {
	public Camera mainCamera;
	public Camera fxCamera;
	public GameObject topBar;
	public GameObject topShade;
	public GameObject sky;
	public GameObject creditsBG;
	public GameObject creditsLabel;
	public GameObject decorations;
	public GameObject col1;
	public GameObject col2;
	public GameObject col3;
	public GameObject col4;
	
	public GameObject leftBar;
	public GameObject rightBar;
	
	public GameObject gemInfo;
}

public class ResolutionHandler : MonoBehaviour {
	
	public List<ScreenConfig> configs;
	public ResolutionAdjustObjects socialObjects;
	public ResolutionAdjustObjects casinoObjects;
	
	ScreenResolution GetCorrectDefault() {
		var width = Screen.width;
		var height = Screen.height;
		if((width > height && Screen.autorotateToPortrait) || (width < height && Screen.autorotateToLandscapeLeft)) {
			Debug.Log ("Swapping aspect");
			var tmp = width;
			width = height;
			height = tmp;
		}
		var aspect = width / (float)height;
		Debug.Log ("Screen aspect: " + aspect);
		
		ScreenResolution toReturn;
		float diff = float.MaxValue;
		
		if(width > height) {
			toReturn = ScreenResolution.RES_DEFAULT_LANDSCAPE;
		}
		else {
			toReturn = ScreenResolution.RES_DEFAULT_PORTRAIT;
		}
		var config = GetConfigForResolution(toReturn);
		Debug.Log ("Default aspect: " + config.GetAspectRatio());
		diff = Mathf.Abs(config.GetAspectRatio() - aspect);
		Debug.Log ("Base diff: " + diff);
		
		foreach(var toCheck in configs) {
			var newDiff = Mathf.Abs (toCheck.GetAspectRatio() - aspect);
			if(newDiff < diff) {
				Debug.Log ("Smaller diff: " + newDiff + " for resolution: " + toCheck.resolution);
				toReturn = toCheck.resolution;
				diff = newDiff;
			}
		}
		
		return toReturn;
	}
	
	public ScreenResolution DetermineResolution() {
		var width = Screen.width;
		var height = Screen.height;
		Debug.Log ("Resolution from system: " + width + "," + height);
		Debug.Log ("Can portrait autorotate: " + Screen.autorotateToPortrait);
		Debug.Log ("Can landscape autorotate: " + Screen.autorotateToLandscapeLeft);
		if((width > height && Screen.autorotateToPortrait) || (width < height && Screen.autorotateToLandscapeLeft)) {
			Debug.Log ("Swapping aspect");
			var tmp = width;
			width = height;
			height = tmp;
		}
		
		var resString = string.Format ("RES_{0}_BY_{1}", width, height);
		Debug.Log ("Looking for: " + resString);
		
		var toReturn = GetCorrectDefault();
		
		try {
			toReturn = (ScreenResolution)Enum.Parse(typeof(ScreenResolution), resString);
		}
		catch(Exception e) {
			e.ToString();
		}
		return toReturn;
	}
	
	public void ConfigureForResolution(bool isCasinoBuild) {
		var objs = isCasinoBuild ? casinoObjects : socialObjects;
		var config = GetConfigForResolution(DetermineResolution());
		Debug.Log ("USING SCREEN RES: " + config.resolution);
		ConfigureObjectsForResolution(objs, config);
	}
	
	void ConfigureObjectsForResolution(ResolutionAdjustObjects objs, ScreenConfig config) {
		objs.mainCamera.orthographicSize = config.mainCameraSize;
		objs.mainCamera.transform.localPosition = config.mainCameraLocation;
		var fxCamLoc = new Vector3(config.mainCameraLocation.x, config.mainCameraLocation.y, config.mainCameraLocation.z);
		fxCamLoc.z -= 400;
		objs.fxCamera.orthographicSize = config.mainCameraSize;
		objs.fxCamera.transform.localPosition = fxCamLoc;
		
		UpdateY(objs.topBar.transform, config.topBarY);
		UpdateY(objs.topShade.transform, config.topShadeY);
		UpdateY(objs.sky.transform, config.skyBgY);
		UpdateY(objs.creditsBG.transform, config.xpBGY);
		UpdateY(objs.creditsLabel.transform, config.xpLabelY);
		UpdateY(objs.decorations.transform, config.decorationsY);
		
		UpdateY(objs.col1.transform, config.columnY);
		UpdateY(objs.col2.transform, config.columnY);
		UpdateY(objs.col3.transform, config.columnY);
		UpdateY(objs.col4.transform, config.columnY);
		
		UpdateX(objs.col1.transform, config.col0x);
		UpdateX(objs.col2.transform, config.col1x);
		UpdateX(objs.col3.transform, config.col2x);
		UpdateX(objs.col4.transform, config.col3x);
		
		UpdateHeight(objs.col1.transform, config.columnHeight);
		UpdateHeight(objs.col2.transform, config.columnHeight);
		UpdateHeight(objs.col3.transform, config.columnHeight);
		UpdateHeight(objs.col4.transform, config.columnHeight);
		
		UpdateY(objs.gemInfo.transform, config.xpBGY);
		
		if(objs.leftBar != null) {
			UpdateX(objs.leftBar.transform, config.leftBarX);
			UpdateHeight(objs.leftBar.transform, config.leftBarHeight);
			UpdateWidth(objs.leftBar.transform, config.leftBarWidth);
		}
		if(objs.rightBar != null) {
			UpdateX(objs.rightBar.transform, config.rightBarX);
			UpdateHeight(objs.rightBar.transform, config.rightBarHeight);
			UpdateWidth(objs.rightBar.transform, config.rightBarWidth);
		}
	}
	
	void UpdateWidth(Transform toUpdate, float newWidth) {
		var scale = toUpdate.localScale;
		scale.x = newWidth;
		toUpdate.localScale = scale;
	}
	
	void UpdateHeight(Transform toUpdate, float newHeight) {
		var scale = toUpdate.localScale;
		scale.y = newHeight;
		toUpdate.localScale = scale;
	}
	
	void UpdateX(Transform toUpdate, float newX) {
		var pos = toUpdate.localPosition;
		pos.x = newX;
		toUpdate.localPosition = pos;
	}
	
	void UpdateY(Transform toUpdate, float newY) {
		var pos = toUpdate.localPosition;
		pos.y = newY;
		toUpdate.localPosition = pos;
	}
	
	ScreenConfig GetConfigForResolution(ScreenResolution res) {
		foreach(var config in configs) {
			if(config.resolution == res) {
				return config;
			}
		}
		return GetConfigForResolution(GetCorrectDefault());
	}
}

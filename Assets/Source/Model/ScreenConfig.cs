using UnityEngine;
using System.Collections;

public enum ScreenResolution {
	
	// portrait resolutions
	RES_DEFAULT_PORTRAIT,
	RES_320_BY_480, // iphone 3gs
	RES_640_BY_960, // iphone 4 & 4s
	RES_640_BY_1136, // iphone 5
	RES_768_BY_1024, // ipad 1 & 2, ipad mini
	RES_1536_BY_2048, // ipad 3 & 4
	RES_480_BY_800, // galaxy s2
	RES_480_BY_854, // motorola droid
	RES_600_BY_1024, // low end android tablets
	RES_720_BY_1280, // galaxy nexus, nexus 7, etc
	
	// landscape resolutions
	RES_DEFAULT_LANDSCAPE,
	RES_480_BY_320,
	RES_960_BY_640,
	RES_1136_BY_640,
	RES_1024_BY_768,
	RES_2048_BY_1536,
	RES_800_BY_480,
	RES_854_BY_480,
	RES_1024_BY_600,
	RES_1280_BY_720
}

[System.Serializable]
public class ScreenConfig {
	public ScreenResolution resolution;
	
	public float width;
	public float height;
	
	public int mainCameraSize;
	public Vector3 mainCameraLocation;
	
	public float topBarY;
	public float topShadeY;
	public float skyBgY;
	
	public float columnHeight;
	public float columnY;
	
	public float col0x;
	public float col1x;
	public float col2x;
	public float col3x;
	
	public float xpBGY;
	public float xpLabelY;
	
	public float decorationsY;
	
	public float leftBarX;
	public float rightBarX;
	
	public float leftBarWidth;
	public float rightBarWidth;
	
	public float leftBarHeight;
	public float rightBarHeight;
	
	public float GetAspectRatio() {
		return width / height;
	}
}

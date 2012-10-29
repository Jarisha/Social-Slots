using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IconGenerator : MonoBehaviour {
	public Mesh baseMesh;
	public List<IconInfo> iconInfo;
	public List<GemInfo> gemInfo;
	
	private static IconGenerator sm_instance = null;
	
	public static IconGenerator Instance() {
		if(IconGenerator.sm_instance == null) {
			IconGenerator.sm_instance = GameObject.Find ("AssetGenerator").GetComponent<IconGenerator>();
		}
		return IconGenerator.sm_instance;
	}
	
	public void Start() {
		IconGenerator.sm_instance = this;
	}
	
	public GameObject CreateIcon(string iconName) {
		var info = GetIconInfo(iconName);
		var toReturn = new GameObject(info.name);
		var mf = toReturn.AddComponent<MeshFilter>();
		mf.mesh = baseMesh;
		var mr = toReturn.AddComponent<MeshRenderer>();
		mr.material = info.material;
		toReturn.transform.localScale = info.scale;
		if(info.animations.Length > 0) {
			var ai = toReturn.AddComponent<AnimatedIcon>();
			ai.animations = info.animations;
			if(ai.HasAnimation("standard")) {
				ai.FireAnimation("standard");
			}
		}
		else {
			toReturn.AddComponent<Icon>();
		}
		
		return toReturn;
	}
	
	public GameObject CreateGem(GemType type) {
		var info = GetGemInfo(type);
		var toReturn = new GameObject(info.name);
		var mf = toReturn.AddComponent<MeshFilter>();
		mf.mesh = baseMesh;
		var mr = toReturn.AddComponent<MeshRenderer>();
		mr.material = info.material;
		toReturn.transform.localScale = info.scale;
		
		return toReturn;
	}
	
	public GemInfo GetGemInfo(GemType type) {
		foreach(var info in gemInfo) {
			if(info.type == type) {
				return info;
			}
		}
		return null;
	}
	
	public IconInfo GetIconInfo(string iconName) {
		foreach(var info in iconInfo) {
			if(info.name.Equals(iconName)) {
				return info;
			}
		}
		return null;
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class IconInfo {
	public string name;
	public Material material;
	public Vector3 scale = new Vector3(90, 0, 0);
}

public class IconGenerator : MonoBehaviour {
	public Mesh baseMesh;
	public List<IconInfo> iconInfo;
	
	private static IconGenerator sm_instance = null;
	
	public static IconGenerator Instance() {
		return sm_instance;
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
		
		return toReturn;
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

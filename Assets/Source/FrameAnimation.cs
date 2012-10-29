using UnityEngine;
using System.Collections;

public enum GemType {
	Red,
	Green,
	Blue
}

[System.Serializable]
public class Frame {
	public Rect uvs;
	public float duration;
	public Material material;
}

[System.Serializable]
public class FrameAnimation {
	public string name;
	public Frame[] frames;
	public bool isLooping;
	
	public float GetTotalDuration() {
		var toReturn = 0.0f;
		for(var i = 0; i < frames.Length; i++) {
			toReturn += frames[i].duration;
		}
		
		return toReturn;
	}
}

[System.Serializable]
public class IconInfo {
	public string name;
	public FrameAnimation[] animations;
	public Material material;
	public Vector3 scale;
}

[System.Serializable]
public class GemInfo {
	public string name;
	public GemType type;
	public Material material;
	public Vector3 scale;
}
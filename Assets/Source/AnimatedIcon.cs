using UnityEngine;
using System.Collections;

public class AnimatedIcon : Icon {
	
	public FrameAnimation[] animations;
	public FrameAnimation activeAnim;
	public int currentFrameIdx;
	public float frameTime;
	
	void Start() {
	}
	
	public void FireAnimation(string toFire) {
		activeAnim = FindAnimation(toFire);
		frameTime = 0.0f;
		currentFrameIdx = 0;
	}
	
	public bool HasAnimation(string toFind) {
		return FindAnimation (toFind) != null;
	}
	
	FrameAnimation FindAnimation(string toFind) {
		for(var i = 0; i < animations.Length; i++) {
			if(animations[i].name.Equals(toFind)) {
				return animations[i];
			}
		}
		return null;
	}
	
	void StopAnimation() {
		activeAnim = null;
		frameTime = 0.0f;
		currentFrameIdx = 0;
	}
	
	void Update() {
		if(activeAnim == null) {
			return;
		}
		
		var currentFrame = GetCurrentFrame();
		if(frameTime >= currentFrame.duration) {
			var newFrame = currentFrameIdx + 1;
			if(newFrame >= activeAnim.frames.Length) {
				if(activeAnim.isLooping) {
					newFrame = 0;
				}
				else {
					StopAnimation();
					return;
				}
			}
			frameTime -= currentFrame.duration;
			SetCurrentFrame(newFrame);
		}
		
		frameTime += Time.deltaTime;
	}
	
	Frame GetCurrentFrame() {
		return activeAnim.frames[currentFrameIdx];
	}
	
	void SetCurrentFrame(int idx) {
		currentFrameIdx = idx;
		UpdateMeshUVs();
	}
	
	void UpdateMeshUVs() {
		var mf = GetComponent<MeshFilter>();
		var mr = GetComponent<MeshRenderer>();
		var frame = GetCurrentFrame();
		mr.material = frame.material;
		
		var uvs = mf.mesh.uv;
		uvs[0].x = frame.uvs.xMin; uvs[0].y = frame.uvs.yMax;
		uvs[1].x = frame.uvs.xMax; uvs[1].y = frame.uvs.yMax;
		uvs[2].x = frame.uvs.xMin; uvs[2].y = frame.uvs.yMin;
		uvs[3].x = frame.uvs.xMax; uvs[3].y = frame.uvs.yMin;
		
		mf.mesh.uv = uvs;
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum SpinState {
	Delay,
	Spinning,
	Stopping,
	Stopped
}

public class Reel : MonoBehaviour {
	
	public float spinTime;
	public float acceleration;
	public float deceleration;
	public float maxVelocity;
	public float minVelocity;
	public List<int> stops;
	public List<GameObject> icons;
	public AudioClip startSound;
	public AudioClip stopSound;
	
	bool isSpinning;
	float velocity;
	int target;
	float spinningFor;
	SpinState state;
	float delay;
	float delaySpent;
	float angle;
	bool haveTarget;
	
	public GameUI owner;
	public int id;

	// Use this for initialization
	void Start () {
		isSpinning = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(isSpinning) {
			UpdateSpin();
		}
		//Debug.Log (isSpinning);
	}
	
	void UpdateSpin() {
		switch(state) {
		case SpinState.Delay:
			UpdateDelay();
			break;
		case SpinState.Spinning:
			UpdateSpinning();
			break;
		case SpinState.Stopping:
			UpdateStopping ();
			break;
		case SpinState.Stopped:
			FinishSpin ();
			break;
		}
	}
	
	void UpdateDelay() {
		delaySpent += Time.deltaTime;
		if(delaySpent > delay) {
			audio.clip = startSound;
			audio.Play ();
			state = SpinState.Spinning;
		}
	}
	
	int IndexOfTarget() {
		return target;
	}
	
	int GetNumIcons() {
		return icons.Count;
	}
	
	void FinishSpin() {
		isSpinning = false;
		
		var bottom = (int)(IndexOfTarget() - 1);
		if(bottom < 0) {
			bottom += GetNumIcons();
		}
		for(int counter = 0; counter < GetNumIcons(); counter++) {
			var idx = (bottom + counter) % GetNumIcons();
			var yLoc = counter * 100.0f;
			var xform = icons[idx].transform;
			var pos = xform.localPosition;
			pos.y = yLoc;
			xform.localPosition = pos;
		}
		owner.ReelFinishedSpinning(id);
		audio.clip = stopSound;
		audio.Play ();
	}
	
	void UpdateStopping() {
		velocity -= deceleration * Time.deltaTime;
		if(velocity < minVelocity) {
			velocity = minVelocity;
		}
		
		foreach(var go in icons) {
			var localPos = go.transform.localPosition;
			localPos.y -= velocity;
			go.transform.localPosition = localPos;
		}
		for(var i = 0; i < icons.Count; i++) {
			var toUpdate = icons[i];
			var pos = toUpdate.transform.localPosition;
			if(pos.y < -50) {
				var idx = i - 1;
				if(idx < 0) {
					idx += icons.Count;
				}
				var next = icons[idx];
				var nextPos = next.transform.localPosition;
				pos.y = nextPos.y + 100.0f;
				toUpdate.transform.localPosition = pos;
			}
		}
		
		var stopPos = icons[(int)target].transform.localPosition.y;
		if(Mathf.Abs(100.0f - stopPos) < minVelocity * 1.5f) {
			state = SpinState.Stopped;
		}
	}
	
	void UpdateSpinning() {
		velocity += acceleration * Time.deltaTime;
		if(velocity > maxVelocity) {
			velocity = maxVelocity;
		}
		
		foreach(var go in icons) {
			var localPos = go.transform.localPosition;
			localPos.y -= velocity;
			go.transform.localPosition = localPos;
		}
		for(var i = 0; i < icons.Count; i++) {
			var toUpdate = icons[i];
			var pos = toUpdate.transform.localPosition;
			if(pos.y < -100) {
				var idx = i - 1;
				if(idx < 0) {
					idx += icons.Count;
				}
				var next = icons[idx];
				var nextPos = next.transform.localPosition;
				pos.y = nextPos.y + 100.0f;
				toUpdate.transform.localPosition = pos;
			}
		}
		
		spinningFor += Time.deltaTime;
		if(haveTarget && spinningFor >= spinTime) {
			state = SpinState.Stopping;
		}
	}
	
	public void StartSpin(float spinDelay) {
		if(isSpinning) {
			return;
		}
		
		this.delay = spinDelay;
		delaySpent = 0.0f;
		velocity = 0;
		spinningFor = 0;
		haveTarget = false;
		state = SpinState.Delay;
		isSpinning = true;
	}
	
	public void SetTarget(int target) {
		this.target = target;
		haveTarget = true;
	}
}

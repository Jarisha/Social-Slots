using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ReelStop {
	One,
	Blank1,
	Two,
	Blank2,
	Three,
	Blank3,
	Four,
	Blank4,
	Five,
	Blank5,
	Six,
	Blank6
}

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
	public List<float> stops;
	
	bool isSpinning;
	Transform xform;
	float velocity;
	ReelStop target;
	float spinningFor;
	SpinState state;
	float delay;
	float delaySpent;
	float angle;

	// Use this for initialization
	void Start () {
		xform = transform;
		isSpinning = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(isSpinning) {
			UpdateSpin();
		}
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
			state = SpinState.Spinning;
		}
	}
	
	void FinishSpin() {
		var targetAngle = stops[(int)target];
		xform.rotation = Quaternion.Euler(targetAngle, 0, 0);
		isSpinning = false;
	}
	
	void UpdateStopping() {
		velocity -= deceleration * Time.deltaTime;
		if(velocity < minVelocity) {
			velocity = minVelocity;
		}
		angle += velocity * Time.deltaTime;
		while(angle > 360.0f) {
			angle -= 360.0f;
		}
		xform.rotation = Quaternion.Euler(angle, 0, 0);
		var targetAngle = stops[(int)target];
		var diff = Mathf.Abs(targetAngle - angle);
		if(diff <= velocity) {
			state = SpinState.Stopped;
		}
	}
	
	void UpdateSpinning() {
		velocity += acceleration * Time.deltaTime;
		if(velocity > maxVelocity) {
			velocity = maxVelocity;
		}
		angle += velocity * Time.deltaTime;
		while(angle > 360.0f) {
			angle -= 360.0f;
		}
		xform.rotation = Quaternion.Euler(angle, 0, 0);
		spinningFor += Time.deltaTime;
		if(spinningFor >= spinTime) {
			state = SpinState.Stopping;
		}
	}
	
	public void Spin(float spinDelay, ReelStop stop) {
		if(isSpinning) {
			return;
		}
		
		this.delay = spinDelay;
		velocity = 0;
		target = stop;
		spinningFor = 0;
		state = SpinState.Delay;
		isSpinning = true;
	}
}

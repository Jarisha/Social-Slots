using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum WheelStop : int {
	NONE = -1,
	BLUE_3,
	STAR,
	RED_2,
	PURPLE_1,
	YELLOW_3,
	BLUE_2,
	RED_1,
	PURPLE_3,
	YELLOW_2,
	BLUE_1,
	RED_3,
	PURPLE_2,
	YELLOW_1
};

public enum WheelSpinState {
	Accelerating,
	Spinning,
	Decelerating,
	Stopped
}

public class Spinner : MonoBehaviour {
	
	public List<float> stops;
	public GameObject wheel;
	public float topSpeed;
	public float minSpeed;
	public float accel;
	public float decel;
	public float spinTime;
	
	public WheelBar redBar;
	public WheelBar purpleBar;
	public WheelBar yellowBar;
	public WheelBar blueBar;
	
	WheelStop target;
	WheelSpinState state;
	float runTime;
	public float angle;
	public float speed;
	Action toCall;
	
	void Start() {
		Reset();
		var rotation = wheel.transform.localRotation;
		rotation = Quaternion.Euler(0, 0, stops[(int)WheelStop.STAR]);
		angle = stops[(int)WheelStop.STAR];
		wheel.transform.localRotation = rotation;
	}
	
	// Update is called once per frame
	void Update () {
		if(state != WheelSpinState.Stopped) {
			UpdateSpin();
		}
	}
	
	void UpdateSpin() {
		switch(state) {
		case WheelSpinState.Accelerating:
			speed += accel * Time.deltaTime;
			if(speed >= topSpeed) {
				speed = topSpeed;
				MoveWheel();
				state = WheelSpinState.Spinning;
			}
			break;
		case WheelSpinState.Spinning:
			runTime += Time.deltaTime;
			MoveWheel();
			if(runTime >= spinTime) {
				state = WheelSpinState.Decelerating;
			}
			break;
		case WheelSpinState.Decelerating:
			speed -= decel * Time.deltaTime;
			if(speed < minSpeed) {
				speed = minSpeed;
			}
			MoveWheel();
			if(CloseToTarget()) {
				SnapToTarget();
				state = WheelSpinState.Stopped;
			}
			break;
		}
	}
	
	void MoveWheel() {
		var xform = wheel.transform;
		angle = angle - speed * Time.deltaTime;
		if(angle < -360.0f) {
			angle += 360.0f;
		}
		else if(angle > 0.0f) {
			angle -= 360.0f;
		}
		xform.localRotation = Quaternion.Euler(0, 0, angle);
	}
	
	bool CloseToTarget() {
		return Mathf.Abs (angle - stops[(int)target]) < 5.0f;
	}
	
	void SnapToTarget() {
		angle = stops[(int)target];
		var finalRotation = Quaternion.Euler(0, 0, angle);
		wheel.transform.localRotation = finalRotation;
		SetMarker(target);
		toCall();
	}
	
	public void Spin(WheelStop stop, Action callback) {
		if(state != WheelSpinState.Stopped) {
			return;
		}
		target = stop;
		runTime = 0.0f;
		state = WheelSpinState.Accelerating;
		toCall = callback;
	}
	
	void Reset() {
		target = WheelStop.NONE;
		state = WheelSpinState.Stopped;
		runTime = 0.0f;
		
	}
	
	void SetMarker(WheelStop mark) {
		switch(mark) {
		case WheelStop.RED_1: redBar.ShowMark(0); break;
		case WheelStop.RED_2: redBar.ShowMark(1); break;
		case WheelStop.RED_3: redBar.ShowMark(2); break;
			
		case WheelStop.PURPLE_1: purpleBar.ShowMark(0); break;
		case WheelStop.PURPLE_2: purpleBar.ShowMark(1); break;
		case WheelStop.PURPLE_3: purpleBar.ShowMark(2); break;
			
		case WheelStop.YELLOW_1: yellowBar.ShowMark(0); break;
		case WheelStop.YELLOW_2: yellowBar.ShowMark(1); break;
		case WheelStop.YELLOW_3: yellowBar.ShowMark(2); break;
			
		case WheelStop.BLUE_1: blueBar.ShowMark(0); break;
		case WheelStop.BLUE_2: blueBar.ShowMark(1); break;
		case WheelStop.BLUE_3: blueBar.ShowMark(2); break;
		}
	}
	
	public void ClearForMark(WheelStop mark) {
		switch(mark) {
		case WheelStop.RED_1:
		case WheelStop.RED_2:
		case WheelStop.RED_3:
			redBar.ClearMarks();
			break;
			
		case WheelStop.PURPLE_1:
		case WheelStop.PURPLE_2:
		case WheelStop.PURPLE_3:
			purpleBar.ClearMarks();
			break;
			
		case WheelStop.BLUE_1:
		case WheelStop.BLUE_2:
		case WheelStop.BLUE_3:
			blueBar.ClearMarks();
			break;
			
		case WheelStop.YELLOW_1:
		case WheelStop.YELLOW_2:
		case WheelStop.YELLOW_3:
			yellowBar.ClearMarks();
			break;
		}
	}
}

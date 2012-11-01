using UnityEngine;
using System.Collections;

public class WheelBar : MonoBehaviour {
	
	public Color tintColor;
	
	public GameObject[] bases;
	public GameObject[] marks;

	// Use this for initialization
	void Start () {
		renderer.material.SetColor ("_Color", tintColor);
		foreach(var go in bases) {
			go.renderer.material.SetColor ("_Color", tintColor);
		}
		ClearMarks();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void ShowMark(int idx) {
		marks[idx].SetActive(true);
	}
	
	public void ClearMarks() {
		foreach(var go in marks) {
			go.SetActive (false);
		}
	}
}

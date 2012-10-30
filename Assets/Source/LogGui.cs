using UnityEngine;
using System.Collections;

public class LogGui : MonoBehaviour {
	
	string output = "";
	Vector2 scroll = Vector2.zero;
	
	// Use this for initialization
	void Start () {
		Application.RegisterLogCallback(HandleLog);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void HandleLog(string logString, string stackTrace, LogType type) {
        output += logString + "\n";
        output += stackTrace + "\n";
    }
	
	void OnGUI() {
		scroll = GUI.BeginScrollView(new Rect(0, 0, 800, 600), scroll, new Rect(0, 0, 800, 100000));
		GUI.TextArea(new Rect(0, 0, 800, 100000), output);
		GUI.EndScrollView();
	}
}

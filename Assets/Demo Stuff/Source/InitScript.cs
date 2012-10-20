//#define DEMO_BUILD

using UnityEngine;
using System.Collections;

public class InitScript : MonoBehaviour {
	
	public MachineParser parser;
	public TextAsset jsonText;
	public MachineInfo machineInfo;
	public GameUI casinoGameDemo;
	public GameUI socialGameDemo;
	
	public bool IS_DEMO_BUILD;

	// Use this for initialization
	void Start () {
		if(IS_DEMO_BUILD) {
			var game = casinoGameDemo.gameObject.active ? casinoGameDemo : socialGameDemo;
			Debug.Log ("Parsing demo stuff on " + game.name);
			Application.targetFrameRate = 60;
			parser = new MachineParser(jsonText.text);
			machineInfo = parser.machineInfo;
			machineInfo.SortPayouts();
			DemoSpinner.sm_info = machineInfo;
			ContentManager.Instance.Player = new Player();
			game.ResetWithResponse(new SelectGameResponse(machineInfo));
			game.UpdateLabels();
		}
	}
}

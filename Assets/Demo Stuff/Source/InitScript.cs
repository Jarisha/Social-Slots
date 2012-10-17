//#define DEMO_BUILD

using UnityEngine;
using System.Collections;

public class InitScript : MonoBehaviour {
	
	public MachineParser parser;
	public TextAsset jsonText;
	public MachineInfo machineInfo;
	public GameUI game;

	// Use this for initialization
	void Start () {
#if DEMO_BUILD
		Debug.Log ("Parsing demo stuff");
		Application.targetFrameRate = 60;
		parser = new MachineParser(jsonText.text);
		machineInfo = parser.machineInfo;
		machineInfo.SortPayouts();
		DemoSpinner.sm_info = machineInfo;
		ContentManager.Instance.Player = new Player();
		game.ResetWithResponse(new SelectGameResponse(machineInfo));
		game.UpdateLabels();
#endif
	}
}

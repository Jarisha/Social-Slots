
using UnityEngine;
using System.Collections;

public class InitScript : MonoBehaviour {
	
	public MachineParser parser;
	public TextAsset jsonText;
	public MachineInfo machineInfo;
	public GameUI casinoGameDemo;
	public GameUI socialGameDemo;
	public GameObject[] casinoGameObjects;
	public GameObject[] socialGameObjects;
	
	public ResolutionHandler resSwitcher;
	
	public GameObject loginScreen;
	
	public bool IS_CASINO_BUILD;

	// Use this for initialization
	void Start () {
		SetBuildObjectsActive();
		GameUI activeGame = IS_CASINO_BUILD ? casinoGameDemo : socialGameDemo;
		loginScreen.SetActive(!activeGame.USE_BAKED_LOGIC);
		resSwitcher.ConfigureForResolution(IS_CASINO_BUILD);
		if(activeGame.USE_BAKED_LOGIC) {
			Debug.Log ("Parsing demo stuff on " + activeGame.name);
			Application.targetFrameRate = 60;
			parser = new MachineParser(jsonText.text);
			machineInfo = parser.machineInfo;
			machineInfo.SortPayouts();
			DemoSpinner.sm_info = machineInfo;
			ContentManager.Instance.Player = new Player();
			activeGame.ResetWithResponse(new SelectGameResponse(machineInfo));
			activeGame.UpdateLabels();
		}
	}
	
	void SetBuildObjectsActive() {
		foreach(var go in casinoGameObjects) {
			go.SetActive(IS_CASINO_BUILD);
		}
		foreach(var go in socialGameObjects) {
			go.SetActive(!IS_CASINO_BUILD);
		}
	}
}

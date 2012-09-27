using UnityEngine;
using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class GameSelectUI : MonoBehaviour {

	public GameObject uiRoot;
	
	private bool m_shouldTransition;
	
	public GameUI simpleGame;
	
	void Update() {
		if(m_shouldTransition) {
			Transition();
		}
	}
	
	public void Show() {
		uiRoot.SetActiveRecursively(true);
	}
	
	public void Hide() {
		uiRoot.SetActiveRecursively(false);
	}
	
	void SimplePressed() {
		var sg = new SelectGame();
		sg.gameid = "simple";
		ConnectionProxy.Connection.SelectGame(sg, (jo) => {
			m_shouldTransition = true;
		});
	}
	
	void Transition() {
		simpleGame.UpdateLabels();
		Hide ();
	}
}

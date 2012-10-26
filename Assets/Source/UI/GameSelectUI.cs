using UnityEngine;
using System.Collections;

public class GameSelectUI : MonoBehaviour {

	public GameObject uiRoot;
	
	private bool m_shouldTransition;
	private SelectGameResponse m_resp;
	public GameUI simpleGame;
	
	void Update() {
		if(m_shouldTransition) {
			Transition();
		}
	}
	
	public void Show() {
		uiRoot.SetActive(true);
	}
	
	public void Hide() {
		uiRoot.SetActive(false);
	}
	
	void SimplePressed() {
		var sg = new SelectGame();
		sg.gameid = "simple";
		ConnectionProxy.Connection.SendMessage(sg, (jdata) => {
			Debug.Log ("Parsed select game response");
			m_resp = new SelectGameResponse(jdata);
			m_shouldTransition = true;
		});
	}
	
	void Transition() {
		simpleGame.ResetWithResponse(m_resp);
		simpleGame.UpdateLabels();
		Hide ();
		m_shouldTransition = false;
	}
}

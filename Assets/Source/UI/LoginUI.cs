using UnityEngine;
using System.Collections;
using System.Net;

public class LoginUI : MonoBehaviour {
	
	public GameObject uiRoot;
	public UILabel nameInput;
	public UILabel serverLabel;
	
	public GameSelectUI selectionScreen;
	
	bool m_wasPaused = false;
	
	private bool m_shouldTransition = false;
	
	public static string[] HOSTS = {
		"socialslots.rjevans.net",
		"127.0.0.1"
	};
	
	public static int[] PORTS = {
		9999,
		8080
	};
	
	int selectedHostIdx = 0;
	
	void OnApplicationPause(bool val) {
		if(val) {
			m_wasPaused = true;
			ConnectionProxy.Quit();
		}
		else {
			if(m_wasPaused) {
				Show();
			}
			m_wasPaused = false;
		}
	}
	
	void Start() {
		SetScreenInfo();
	}
	
	void Update() {
		if(m_shouldTransition) {
			Transition ();
		}
	}
	
	void SetScreenInfo() {
		serverLabel.text = string.Format ("server - tcp://{0}:{1}", HOSTS[selectedHostIdx], PORTS[selectedHostIdx]);
	}
	
	public void Show() {
		uiRoot.SetActiveRecursively(true);
		SetScreenInfo();
	}
	
	public void Hide() {
		uiRoot.SetActiveRecursively(false);
	}
	
	void LoginPressed() {
		ConnectionProxy.CreateConnection(HOSTS[selectedHostIdx], PORTS[selectedHostIdx], () => {
			Debug.Log ("Callback from connect");
			var auth = new Auth(nameInput.text.Trim());
			Debug.Log ("Calling auth");
			ConnectionProxy.Connection.SendMessage(auth, (jdata) => {
				ContentManager.Instance.ReadPlayerInfo(jdata);
				// We use a boolean to mark the transition instead of transitioning directly
				// since the connection is not necessarily working on the main thread.
				m_shouldTransition = true;
			});
		});
	}
	
	void ServerPressed() {
		selectedHostIdx++;
		selectedHostIdx = selectedHostIdx % HOSTS.Length;
		SetScreenInfo ();
	}
	
	void Transition() {
		selectionScreen.Show ();
		Hide ();
		m_shouldTransition = false;
	}
}

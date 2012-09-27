using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

[System.Serializable]
public class Payline {
	public GameObject left;
	public GameObject right;
	public GameObject line;
	
	public void SetVisible(bool isVisible) {
		left.active = isVisible;
		right.active = isVisible;
		line.active = isVisible;
	}
}

public class GameUI : MonoBehaviour {

	public Reel reel0;
	public Reel reel1;
	public Reel reel2;
	public Reel reel3;
	public Reel reel4;
	
	public UILabel betButtonLabel;
	public UILabel lineButtonLabel;
	public UILabel statsLine;
	
	public UIButton spinButton;
	
	public List<Payline> m_visibleLines;
	
	bool[] m_reelDone = {false, false, false, false, false};
	int m_lastWin = 0;
	
	int[] m_betAmounts = {
		1,
		2,
		3
	};
	
	int[][] m_lines = {
		new int[] {0, 1, 0},
		new int[] {1, 1, 1}
	};
	
	int[] m_lineCounts = {
		1,
		3
	};
	
	int m_betIdx = 0;
	int m_lineIdx = 1;
	
	public UILabel creditsLabel;
	
	JObject spinData = null;
	bool spinning = false;
	
	void Start() {
	}
	
	void Update() {
		if(spinning) {
			if(spinData != null) {
				SetTargets();
			}
		}
		if(AllReelsFinished()) {
			HandleSpinResults();
		}
	}
	
	void ShowSpinResults() {
	}
	
	bool AllReelsFinished() {
		for(int i = 0; i < 5; i++) {
			if(!m_reelDone[i]) {
				return false;
			}
		}
		return true;
	}
	
	void HandleSpinResults() {
		var winAmount = (int)spinData["results"]["results"]["total_credits"];
		ContentManager.Instance.Player.IncrementCredits(winAmount);
		if(winAmount > 0) {
			m_lastWin = winAmount;
		}
		ResetReelChecks();
		UpdateLabels();
		spinButton.isEnabled = true;
	}
	
	public void UpdateLabels() {
		creditsLabel.text = string.Format("Credits: {0}", ContentManager.Instance.Player.m_credits);
		var lines = m_lineCounts[m_lineIdx];
		var bet = m_betAmounts[m_betIdx];
		statsLine.text = string.Format("Lines: {0} | Bet: {1} | Total Bet: {2} | Last Win: {3}",
			lines, bet, lines * bet, m_lastWin);
		UpdateButtonInfo();
	}
	
	void ResetReelChecks() {
		for(int i = 0; i < 5; i++) {
			m_reelDone[i] = false;
		}
	}
	
	public void ReelFinishedSpinning(int id) {
		m_reelDone[id] = true;
	}
	
	void SetTargets() {
		reel0.SetTarget((ReelStop)(int)spinData["results"]["reels"][0]);
		reel1.SetTarget((ReelStop)(int)spinData["results"]["reels"][1]);
		reel2.SetTarget((ReelStop)(int)spinData["results"]["reels"][2]);
		reel3.SetTarget((ReelStop)(int)spinData["results"]["reels"][3]);
		reel4.SetTarget((ReelStop)(int)spinData["results"]["reels"][4]);
		spinning = false;
	}
	
	void OnApplicationQuit() {
		ConnectionProxy.Quit();
	}
	
	void SpinPressed() {
		Spin();
		reel0.StartSpin(0);
		reel1.StartSpin(0.1f);
		reel2.StartSpin(0.2f);
		reel3.StartSpin(0.3f);
		reel4.StartSpin(0.4f);
	}
	
	void Spin() {
		spinButton.isEnabled = false;
		spinData = null;
		spinning = true;
		var bet = m_betAmounts[m_betIdx];
		var spin = new Spin(m_lines[m_lineIdx], bet);
		ContentManager.Instance.Player.IncrementCredits(-bet * m_lineCounts[m_lineIdx]);
		UpdateLabels();
		ConnectionProxy.Connection.Spin(spin, (jo) => {
			Debug.Log ("Spin done!");
			spinData = jo;
		});
	}
	
	void BetPressed() {
		m_betIdx = (m_betIdx + 1) % m_betAmounts.Length;
		UpdateButtonInfo();
	}
	
	void LinePressed() {
		m_lineIdx = (m_lineIdx + 1) % m_lines.Length;
		UpdateButtonInfo();
		UpdateLines();
	}
	
	void UpdateLines() {
		var lines = m_lines[m_lineIdx];
		for(var i = 0; i < lines.Length; i++) {
			m_visibleLines[i].SetVisible(lines[i] == 1);
		}
	}
	
	void UpdateButtonInfo() {
		betButtonLabel.text = string.Format ("Bet:\n{0}", m_betAmounts[m_betIdx]);
		lineButtonLabel.text = string.Format ("Lines:\n{0}", m_lineCounts[m_lineIdx]);
	}
}

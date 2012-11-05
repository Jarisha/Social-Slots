

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameUI : MonoBehaviour {
	
	
	public bool USE_BAKED_LOGIC = false;
	
	public Material m_lineMaterial;
	
	public MachineInfo info;
	
	public Reel[] reels;
	
	public UILabel betButtonLabel;
	public UILabel lineButtonLabel;
	public UILabel statsLine;
	
	public UIImageButton spinButton;
	
	public List<Payline> m_visibleLines;
	
	public AudioClip spinningSound;
	
	public Spinner spinner;
	
	public bool[] m_reelDone = {true, true, true, true, true};
	
	int[] m_betAmounts = {
		1,
		2,
		3,
		5
	};
	
	int[][] m_lines = {
		new int[] {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
		new int[] {1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0},
		new int[] {1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0},
		new int[] {1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0},
		new int[] {1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0},
		new int[] {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
	};
	
	int[] m_lineCounts = {
		1,
		3,
		5,
		7,
		9,
		11
	};
	
	int m_betIdx = 3;
	int m_lineIdx = 5;
	
	public UILabel creditsLabel;
	public UILabel payoutsLabel;
	public WinEffects effectPlayer;
	
	SpinResponse spinData = null;
	bool spinning = false;
	
	void Start() {
	}
	
	void Update() {
		if(spinning) {
			if(spinData != null) {
				SetTargets();
				StartCoroutine(UpdateGems());
			}
			if(AllReelsFinished()) {
				Debug.Log ("Handle Spin Results!");
				HandleSpinResults();
			}
		}
	}
	
	IEnumerator UpdateGems() {
		yield return new WaitForSeconds(0.33f);
		foreach(var reel in reels) {
			reel.ClearGems();
		}
		var ig = IconGenerator.Instance();
		foreach(var gem in spinData.gems) {
			var icon = reels[gem.reel].icons[gem.idx].GetComponent<Icon>();
			icon.AddGem(ig.CreateGem (gem.type));
		}
	}
	
	IEnumerator ShowSpinResults() {
		for(var i = 0; i < spinData.lines.Count; i++) {
			var line = spinData.lines[i];
			if(line.credits > 0) {
				StartCoroutine(effectPlayer.PlayWinForLine(info.m_lines[i], line.credits));
				m_visibleLines[i].SetVisible(true);
				yield return new WaitForSeconds(0.33f);
				m_visibleLines[i].StartFadeOut(0.66f);
				yield return new WaitForSeconds(0.66f);
			}
		}
		if(spinner != null && spinData.wheelStop != -1) {
			var stop = (WheelStop)spinData.wheelStop;
			spinner.Spin(stop, () => {
				if(spinData.wheelReward > 0) {
					spinner.ClearForMark(stop);
					ContentManager.Instance.Player.IncrementCredits(spinData.wheelReward);
					UpdateLabels();
				}
			});
			yield return new WaitForSeconds(2.2f);
		}
		spinButton.enabled = true;
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
		spinning = false;
		var winAmount = spinData.totalCredits;
		ContentManager.Instance.Player.IncrementCredits(winAmount);
		ContentManager.Instance.Player.IncrementRedGems(spinData.totalRedGems);
		ContentManager.Instance.Player.IncrementGreenGems(spinData.totalGreenGems);
		ContentManager.Instance.Player.IncrementBlueGems(spinData.totalBlueGems);
		StartCoroutine(ShowSpinResults());
		ResetReelChecks();
		UpdateLabels();
	}
	
	public void UpdateLabels() {
		creditsLabel.text = string.Format("{0}c", ContentManager.Instance.Player.m_credits);
		UpdateButtonInfo();
	}
	
	void ResetReelChecks() {
		for(int i = 0; i < 5; i++) {
			m_reelDone[i] = false;
		}
	}
	
	public void ReelFinishedSpinning(int id) {
		if(audio.isPlaying) {
			audio.Stop();
		}
		m_reelDone[id] = true;
	}
	
	void SetTargets() {
		reels[0].SetTarget(spinData.reels[0]);
		reels[1].SetTarget(spinData.reels[1]);
		reels[2].SetTarget(spinData.reels[2]);
		reels[3].SetTarget(spinData.reels[3]);
		reels[4].SetTarget(spinData.reels[4]);
	}
	
	void OnApplicationQuit() {
		ConnectionProxy.Quit();
	}
	
	void SpinPressed() {
		if(spinning) {
			return;
		}
		Spin();
		reels[0].StartSpin(0);
		reels[1].StartSpin(0.1f);
		reels[2].StartSpin(0.2f);
		reels[3].StartSpin(0.3f);
		reels[4].StartSpin(0.4f);
		audio.loop = true;
		audio.clip = spinningSound;
		audio.Play (17640);
	}
	
	void Spin() {
		spinButton.enabled = false;
		spinData = null;
		spinning = true;
		var bet = m_betAmounts[m_betIdx];
		var spin = new Spin(m_lines[m_lineIdx], bet);
		ContentManager.Instance.Player.IncrementCredits(-bet * m_lineCounts[m_lineIdx]);
		UpdateLabels();
		ResetReelChecks ();
		if(this.USE_BAKED_LOGIC) {
			DemoSpinner.Spin(spin, (text) => {
				Debug.Log ("Spin done!");
				Debug.Log ("Got spin: " + text);
				LitJson.JsonData jdata = LitJson.JsonMapper.ToObject<LitJson.JsonData>(text);			
				spinData = new SpinResponse(jdata);
			});
		}
		else {
			ConnectionProxy.Connection.SendMessage(spin, (jdata) => {
				Debug.Log ("Spin done!");
				Debug.Log (jdata["results"]);
				spinData = new SpinResponse(jdata);
			});
		}
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
		StartCoroutine(FadeLinesAfterDelay(0.66f, 0.33f));
	}
	
	IEnumerator FadeLinesAfterDelay(float delay, float fadeTime) {
		yield return new WaitForSeconds(delay);
		for(var i = 0; i < m_visibleLines.Count; i++) {
			if(m_visibleLines[i].IsVisible()) {
				m_visibleLines[i].StartFadeOut(fadeTime);
			}
		}
	}
	
	void UpdateButtonInfo() {
		betButtonLabel.text = m_betAmounts[m_betIdx].ToString();
		lineButtonLabel.text = m_lineCounts[m_lineIdx].ToString();
	}
	
	public void ResetWithResponse(SelectGameResponse resp) {
		info = resp.machine;
		ResetReelOwner();
		CreateReelIcons();
		UpdateLineColors();
		UpdateLines ();
		UpdatePayoutInfo();
	}
	
	void ResetReelOwner() {
		for(var i = 0; i < reels.Length; i++) {
			reels[i].owner = this;
		}
	}
	
	void UpdatePayoutInfo() {
		var toDisplay = "Payouts:\n";
		for(var i = 0; i < info.m_payouts.Count; i+= 2) {
			var payout = info.m_payouts[i];
			toDisplay += payout.ToString();
			if(i + 1 < info.m_payouts.Count) {
				payout = info.m_payouts[i + 1];
				toDisplay += "        " + payout.ToString ();
			}
			toDisplay += "\n";
		}
		//payoutsLabel.text = toDisplay;
	}
	
	void UpdateLineColors() {
		foreach(var line in m_visibleLines) {
			line.UpdateColor();
		}
	}
	
	void CreateReelIcons() {
		var iconMaker = IconGenerator.Instance();
		for(var i = 0; i < 5; i++) {
			var reelInfo = info.m_reels[i];
			var reel = reels[i];
			foreach(var icon in reel.icons) {
				Destroy (icon);
			}
			reel.stops.Clear();
			var newIcons = new List<GameObject>();
			for(var j = 0; j < reelInfo.Count; j++) {
				var slotIdx = reelInfo[j];
				var slotName = info.NameForId(slotIdx);
				var icon = iconMaker.CreateIcon(slotName);
				reel.stops.Add (slotIdx);
				newIcons.Add (icon);
				icon.transform.parent = reel.transform;
				icon.transform.localPosition = new Vector3(0, j * 100, 0);
			}
			Debug.Log ("Setting reel " + reel.name + " to have " + newIcons.Count + " icons");
			reel.icons = newIcons;
		}
	}
}

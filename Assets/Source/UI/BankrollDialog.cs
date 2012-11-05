using UnityEngine;
using System.Collections;

public class BankrollDialog : MonoBehaviour {
	
	public UIPanel root;
	public UILabel creditsLabel;
	public UILabel goldLabel;
	public UILabel redGemLabel;
	public UILabel greenGemLabel;
	public UILabel blueGemLabel;
	
	public void Show() {
		Refresh();
		root.gameObject.SetActive(true);
	}
	
	public void Hide() {
		root.gameObject.SetActive(false);
	}
	
	public void Refresh() {
		var player = ContentManager.Instance.Player;
		creditsLabel.text = player.m_credits.ToString();
		goldLabel.text = player.m_gold.ToString();
		redGemLabel.text = player.m_redGems.ToString();
		greenGemLabel.text = player.m_greenGems.ToString();
		blueGemLabel.text = player.m_blueGems.ToString();
	}
	
	public void ClosePressed() {
		Hide ();
	}
}

using UnityEngine;
using System.Collections;

public class DialogManager : MonoBehaviour {
	
	public BankrollDialog bankroll;
	
	public void ShowBankroll() {
		bankroll.Show();
	}
}

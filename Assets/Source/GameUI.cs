using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameUI : MonoBehaviour {

	public Reel reel0;
	public Reel reel1;
	public Reel reel2;
	
	void SpinPressed() {
		ReelStop[] spins = GetSpin();
		Debug.Log ("Spin: " + spins[0] + " " + spins[1] + " " + spins[2]);
		reel0.Spin(0, spins[0]);
		reel1.Spin(0.2f, spins[1]);
		reel2.Spin(0.4f, spins[2]);
	}
	
	ReelStop[] GetSpin() {
		ReelStop[] toReturn = new ReelStop[3];
		
		for(int i = 0; i < 3; i++) {
			toReturn[i] = (ReelStop)Random.Range(0, 12);	
		}
		
		return toReturn;
	}
}

using UnityEngine;
using System.Collections;

public class Icon : MonoBehaviour {
	
	public GameObject gem;
	
	public void AddGem(GameObject toAdd) {
		RemoveGem ();
		gem = toAdd;
		gem.transform.localPosition = new Vector3(30, 20, -0.1f);
		gem.transform.parent = transform;
	}
	
	public void RemoveGem() {
		if(gem != null) {
			Destroy (gem);
			gem = null;
		}
	}
}

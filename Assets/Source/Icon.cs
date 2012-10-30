using UnityEngine;
using System.Collections;

public class Icon : MonoBehaviour {
	
	public GameObject gem;
	
	public void AddGem(GameObject toAdd) {
		RemoveGem ();
		gem = toAdd;
		gem.transform.parent = transform;
		gem.transform.localPosition = new Vector3(0.375f, 0.375f, -0.1f);
	}
	
	public void RemoveGem() {
		if(gem != null) {
			Destroy (gem);
			gem = null;
		}
	}
}

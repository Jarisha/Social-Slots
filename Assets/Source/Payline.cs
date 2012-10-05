using UnityEngine;
using System.Collections;

[System.Serializable]
public class Payline : MonoBehaviour {
	public GameObject left;
	public GameObject line;
	public GameObject right;
	
	public Color color;
	
	public void UpdateColor() {
		var mat = left.GetComponent<MeshRenderer>().material;
		mat.color = color;
		mat = right.GetComponent<MeshRenderer>().material;
		mat.color = color;
		line.GetComponent<LineRenderer>().SetColors (color, color);
	}
	
	public void SetVisible(bool isVisible) {
		left.active = isVisible;
		right.active = isVisible;
		line.active = isVisible;
	}
}
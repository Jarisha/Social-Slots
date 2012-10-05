using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LineMarker : MonoBehaviour {

	public Mesh m_defaultMesh;
	public Material m_leftMat;
	public Material m_rightMat;
	
	static List<Color> m_colors = new List<Color>() {
		Color.red,
		Color.green,
		Color.cyan
	};
	
	static LineMarker GetInstance() {
		return GameObject.Find ("AssetGenerator").GetComponent<LineMarker>();
	}
	
	public static GameObject CreateMarker(bool isLeftMarker, float yOffset, int lineIdx) {
		var creator = GetInstance();
		var markerName = string.Format ("{0} Marker {1}", isLeftMarker ? "Left" : "Right", lineIdx);
		var go = new GameObject(markerName);
		var mf = go.AddComponent<MeshFilter>();
		mf.mesh = creator.m_defaultMesh;
		var mr = go.AddComponent<MeshRenderer>();
		mr.material = isLeftMarker ? creator.m_leftMat : creator.m_rightMat;
		mr.material.SetColor("_Color", m_colors[lineIdx]);
		
		var pos = go.transform.localPosition;
		pos.x = isLeftMarker ? -232 : 232;
		pos.y = yOffset;
		pos.z = -1;
		go.transform.localPosition = pos;
		go.transform.localScale = new Vector3(16, 16, 1);
		return go;
	}
}

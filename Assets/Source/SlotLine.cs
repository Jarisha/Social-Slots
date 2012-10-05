using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SlotLine : MonoBehaviour {
	
	public Vector2[] m_points;
	public bool m_regen = false;
	public int m_width = 8;
	
	//TODO: Make this pick up the actual size of the play area
	float[] m_xoffsets = { -240, -190, -95, 0, 95, 190, 240};
	float[] m_yoffsets = { -100, 0, 100 }; // note: must offset by one since the passed offsets are -1 -> 1
	
	float leftOff;
	float rightOff;
	
	static List<Color> m_colors = new List<Color>() {
		Color.red,
		Color.green,
		Color.cyan
	};
	
	public LineRenderer m_renderer;

	public void SetOffsets(int[] newOffsets) {
		if(m_renderer == null) {
			m_renderer = GetComponent<LineRenderer>();
			m_renderer.SetVertexCount(7);
			for(var i = 0; i < 7; i++) {
				var x = m_xoffsets[i];
				var yIdx = i - 1;
				if(yIdx < 0) {
					yIdx = 0;
				}
				else if(yIdx > 4) {
					yIdx = 4;
				}
				var y = m_yoffsets[newOffsets[yIdx] + 1];
				m_renderer.SetPosition(i, new Vector3(x, y, 0));
			}
			leftOff = m_yoffsets[newOffsets[0] + 1] - 162.0f;
			rightOff = m_yoffsets[newOffsets[0] + 1] - 162.0f;
		}
	}
	
	public static GameObject CreateLineWithOffsets(int lineIdx, int[] offsets, Material mat) {
		var go = new GameObject("Payline " + lineIdx);
		var pos = go.transform.localPosition;
		pos.y = -162;
		go.transform.localPosition = pos;
		var lr = go.AddComponent<LineRenderer>();
		lr.SetWidth(4, 4);
		lr.useWorldSpace = false;
		var sl = go.AddComponent<SlotLine>();
		lr.SetColors(m_colors[lineIdx], m_colors[lineIdx]);
		lr.material = mat;
		sl.SetOffsets(offsets);
		
		return go;
	}
	
	public float GetLeftOffset() {
		return leftOff;
	}
	
	public float GetRightOffset() {
		return rightOff;
	}
}

using UnityEngine;
using System.Collections;

[System.Serializable]
public class Payline : MonoBehaviour {
	public GameObject left;
	public GameObject line;
	public GameObject right;
	
	Material m_leftMat;
	Material m_rightMat;
	LineRenderer m_lineRenderer;
	
	public Color color;
	public float m_destAlpha;
	public bool m_isVisible;
	public float m_fadeRate;
	
	void Update() {
		if(color.a != m_destAlpha) {
			var amt = (m_destAlpha - color.a) * Time.deltaTime * m_fadeRate;
			color.a += amt;
			UpdateColor ();
		}
	}
	
	void UpdateReferences() {
		if(m_leftMat == null) {
			m_leftMat = left.GetComponent<MeshRenderer>().material;
		}
		if(m_rightMat == null) {
			m_rightMat = right.GetComponent<MeshRenderer>().material;
		}
		if(m_lineRenderer == null) {
			m_lineRenderer = line.GetComponent<LineRenderer>();
		}
	}
	
	public bool IsVisible() {
		return m_isVisible;
	}
	
	public void UpdateColor() {
		UpdateReferences();
		m_leftMat.color = color;
		m_rightMat.color = color;
		m_lineRenderer.SetColors(color, color);
	}
	
	public void SetAlpha(float alpha) {
		color.a = alpha;
		UpdateColor();
	}
	
	public void SetVisible(bool isVisible) {
		m_isVisible = isVisible;
		left.SetActive (isVisible);
		right.SetActive (isVisible);
		line.SetActive (isVisible);
		SetAlpha (isVisible ? 1.0f : 0.0f);
		m_destAlpha = isVisible ? 1.0f : 0.0f;
	}
	
	public void StartFadeOut(float delay) {
		m_destAlpha = 0.0f;
		m_fadeRate = 1.0f / delay;
	}
	
	public void StartFadeIn(float delay) {
		m_destAlpha = 1.0f;
		m_fadeRate = 1.0f / delay;
	}
}
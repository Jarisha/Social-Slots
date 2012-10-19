using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class EffectReel {
	public List<ParticleSystem> winParticles;
}

public class WinEffects : MonoBehaviour {
	
	public EffectReel[] effects;
	public ParticleSystem coinEffect;
	public AudioClip smallWinSound;
	public AudioClip bigWinSound;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public IEnumerator PlayWinForLine(List<int> lineOffsets, int amount) {
		audio.clip = (amount <= 30) ? smallWinSound : bigWinSound;
		audio.Play ();
		for(var i = 0; i < lineOffsets.Count; i++) {
			effects[i].winParticles[lineOffsets[i] + 1].Emit(100);
		}
		coinEffect.Emit(amount <= 50 ? amount : 50);
		yield return null;
	} 
}

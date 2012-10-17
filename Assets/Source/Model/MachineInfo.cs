using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;

[System.Serializable]
public class MachineInfo {
	
	public List<List<int>> m_reels;
	public List<Payout> m_payouts;
	public Dictionary<string, int> m_slots;
	public List<List<int>> m_lines;
	
	// Only needed for demo info
	public List<List<int>> m_vreels;
	
	public MachineInfo() {
		m_reels = new List<List<int>>();
		m_payouts = new List<Payout>();
		m_slots = new Dictionary<string, int>();
		m_lines = new List<List<int>>();
		m_vreels = new List<List<int>>();
	}
	
	public void AddReel(List<int> toAdd) {
		m_reels.Add (toAdd);
	}
	
	public void AddVReel(List<int> toAdd) {
		m_vreels.Add (toAdd);
	}
	
	public int GetVslotCount(int reelIdx) {
		var vreel = m_vreels[reelIdx];
		var total = 0;
		foreach(var toAdd in vreel) {
			total += toAdd;
		}
		return total;
	}
	
	public int MapVReelIndex(int reel, int vreelIdx) {
		var total = 0;
		var vreel = m_vreels[reel];
		for(var i = 0; i < vreel.Count; i++) {
			total += vreel[i];
			if(vreelIdx < total) {
				return i;
			}
		}
		return -1;
	}
	
	public void AddSlot(string name, int idx) {
		m_slots[name] = idx;
	}
	
	public void AddPayout(Payout toAdd) {
		m_payouts.Add (toAdd);
	}
	
	public MachineInfo(JsonData data) {
		ParseSlots(data["slots"]);
		ParseReels((IList)data["reels"]);
		ParseLines ((IList)data["lines"]);
		ParsePayouts((IList)data["payouts"]);
	}
	
	public void ParsePayouts(IList payoutArray) {
		m_payouts = new List<Payout>();
		for(var i = 0; i < payoutArray.Count; i++) {
			var jd = (JsonData)payoutArray[i];
			var payout = new Payout(jd);
			//Debug.Log ("Adding payout: " + payout.ToString());
			m_payouts.Add (payout);
		}
	}
	
	public void ParseLines(IList lineArray) {
		m_lines = new List<List<int>>();
		for(var i = 0; i < lineArray.Count; i++) {
			var lineInfo = (IList)lineArray[i];
			var toAdd = new List<int>();
			for(var j = 0; j < lineInfo.Count; j++) {
				var jd = (JsonData)lineInfo[j];
				toAdd.Add ((int)jd);
			}
			Debug.Log ("Adding new line" + JsonMapper.ToJson (toAdd));
			m_lines.Add (toAdd);
		}
	}
	
	void ParseReels(IList reelArray) {
		m_reels = new List<List<int>>();
		for(var i = 0; i < 5; i++) {
			var toProcess = (IList)reelArray[i];
			var toAdd = new List<int>();
			for(var j = 0; j < toProcess.Count; j++) {
				var jd = (JsonData)toProcess[j];
				var name = (string)jd;
				toAdd.Add (IndexForName (name));
			}
			m_reels.Add (toAdd);
		}
	}
	
	public void ParseSlots(JsonData jd) {
		m_slots = new Dictionary<string, int>();
		m_slots["Any"] = -1;
		IDictionary dict = (IDictionary)jd;
		foreach(string key in dict.Keys) {
			//Debug.Log (key);
			m_slots[key] = (int)jd[key];
		}
	}
	
	public int IndexForName(string key) {
		if(m_slots.ContainsKey (key)) {
			return m_slots[key];
		}
		else {
			return -1;
		}
	}
	
	public string NameForIndex(int idx) {
		foreach(var key in m_slots.Keys) {
			if(m_slots[key] == idx) {
				return key;
			}
		}
		return null;
	}
	
	public int CheckLineWithSpin(int[] spin, int lineIdx) {
		int[] adjSpin = GetAdjustedSpin(spin, lineIdx);
		var payout = GetTopPayout(adjSpin);
		if(payout == null) {
			return 0;
		}
		else {
			return payout.credits;
		}
	}
	
	int[] GetAdjustedSpin(int[] baseSpin, int lineIdx) {
		var toReturn = new int[5];
		var offsets = m_lines[lineIdx];
		for(var i = 0; i < 5; i++) {
			var baseIdx = FindReelIdxForVal(i, baseSpin[i]);
			var idx = baseIdx + offsets[i];
			if(idx < 0) {
				idx += m_reels[i].Count;
			}
			if(idx >= m_reels[i].Count) {
				idx -= m_reels[i].Count;
			}
			toReturn[i] = m_reels[i][idx];
		}
		return toReturn;
	}
	
	int FindReelIdxForVal(int reel, int val) {
		return m_reels[reel].IndexOf(val);
	}
	
	Payout GetTopPayout(int[] spin) {
		foreach(var toCheck in m_payouts) {
			if(toCheck.DoesMatch(spin, this)) {
				return toCheck;
			}
		}
		return null;
	}
	
	int SortByCredits(Payout a, Payout b) {
		if(a.credits > b.credits) {
	      return -1;
	    }
	    else if(a.credits < b.credits) {
	      return 1;
	    }
	    else {
	      return 0;
    	}
	}
	
	public void SortPayouts() {
		m_payouts.Sort (SortByCredits);
	}
}

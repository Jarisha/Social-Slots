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
	
	public MachineInfo() {
		m_reels = new List<List<int>>();
		m_payouts = new List<Payout>();
		m_slots = new Dictionary<string, int>();
		m_lines = new List<List<int>>();
	}
	
	public void AddReel(List<int> toAdd) {
		m_reels.Add (toAdd);
	}
	
	public int GetSlotCount(int reelIdx) {
		return m_reels[reelIdx].Count;
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
		ParseLines((IList)data["lines"]);
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
				toAdd.Add (IdForName(name));
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
	
	public int IdForName(string key) {
		if(m_slots.ContainsKey (key)) {
			return m_slots[key];
		}
		else {
			return -1;
		}
	}
	
	public string NameForId(int id) {
		foreach(var key in m_slots.Keys) {
			if(m_slots[key] == id) {
				return key;
			}
		}
		return null;
	}
	
	public int CheckLineWithSpin(int[] spin, int lineIdx) {
		int[] adjSpin = GetAdjustedSpin(spin, lineIdx);
	/*	Debug.Log (string.Format ("Adusted spin: {0}:{1}:{2}, {3}:{4}:{5}, {6}:{7}:{8}, {9}:{10}:{11}, {12}:{13}:{14}", 
			spin[0], m_lines[lineIdx][0], adjSpin[0],
			spin[1], m_lines[lineIdx][1], adjSpin[1],
			spin[2], m_lines[lineIdx][2], adjSpin[2],
			spin[3], m_lines[lineIdx][3], adjSpin[3],
			spin[4], m_lines[lineIdx][4], adjSpin[4]
			));*/
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
			var baseIdx = baseSpin[i];
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

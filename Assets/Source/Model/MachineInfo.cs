using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;

[System.Serializable]
public class MachineInfo {
	
	public Dictionary<string, int> m_slots;
	public List<List<int>> m_reels;
	public List<List<int>> m_lines;
	
	public MachineInfo(JsonData data) {
		ParseSlots(data["slots"]);
		ParseReels((IList)data["reels"]);
		ParseLines((IList)data["lines"]);
	}
	
	void ParseLines(IList lineArray) {
		m_lines = new List<List<int>>();
		for(var i = 0; i < lineArray.Count; i++) {
			var line = (IList)lineArray[i];
			var toAdd = new List<int>();
			for(var j = 0; j < line.Count; j++) {
				var jd = (JsonData)line[j];
				toAdd.Add ((int)jd);
			}
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
	
	void ParseSlots(JsonData jd) {
		m_slots = new Dictionary<string, int>();
		IDictionary dict = (IDictionary)jd;
		foreach(string key in dict.Keys) {
			Debug.Log (key);
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
}

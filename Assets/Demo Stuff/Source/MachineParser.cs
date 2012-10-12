using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;

public class MachineParser {
	
	public MachineInfo machineInfo;
	
	public MachineParser(string jsonText) {
		var jd = JsonMapper.ToObject(jsonText);
		ReadMachineInfo(jd);
	}
	
	void ReadMachineInfo(JsonData jd) {
		machineInfo = new MachineInfo();
		machineInfo.ParseSlots(jd["slots"]);
		machineInfo.ParsePayouts(jd["payouts"]);
		machineInfo.ParseLines (jd["lines"]);
		ParseReels(jd["reels"]);
	}
	
	void ParseReels(JsonData jd) {
		IList fullReels = (IList)jd;
		for(var i = 0; i < fullReels.Count; i++) {
			JsonData fullReelObj = (JsonData)fullReels[i];
			IList layout = (IList)fullReelObj["order"];
			IList odds = (IList)fullReelObj["odds"];
			List<int> reel = new List<int>();
			List<int> vreel = new List<int>();
			for(var j = 0; j < layout.Count; j++) {
				var reelJD = (JsonData)layout[j];
				var vreelJD = (JsonData)odds[j];
				reel.Add (machineInfo.IndexForName((string)reelJD));
				vreel.Add ((int)vreelJD);
			}
			machineInfo.AddReel(reel);
			machineInfo.AddVReel(vreel);
		}
	}
}

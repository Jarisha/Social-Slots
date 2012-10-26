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
			List<int> reel = new List<int>();
			for(var j = 0; j < layout.Count; j++) {
				var reelJD = (JsonData)layout[j];
				reel.Add (machineInfo.IdForName((string)reelJD));
			}
			machineInfo.AddReel(reel);
		}
	}
}

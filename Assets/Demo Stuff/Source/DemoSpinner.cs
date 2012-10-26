using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using LitJson;

public class DemoSpinner {
	
	public static MachineInfo sm_info;
	
	public static void Spin(Spin spin, Action<JsonData> callback) {
		var response = new Dictionary<string, object>();
		response["status"] = "OK";
		response["results"] = GetSpinResults(spin);
		var text = JsonMapper.ToJson(response);
		Debug.Log (text);
		callback(JsonMapper.ToObject (text));
	}
	
	static Dictionary<string, object> GetSpinResults(Spin spin) {
		var response = new Dictionary<string, object>();
		var reels = SpinReels();
		response["reels"] = reels;
		
		var results = new Dictionary<string, object>();
		var totalCredits = 0;
		var lines = new List<object>();
		Debug.Log ("Bet is: " + spin.bet);
		for(var i = 0; i < spin.lines.Length; i++) {
			var line = CheckLine(reels, i, spin.lines[i]);
			line["credits"] = (int)line["credits"] * spin.bet;
			totalCredits += (int)line["credits"];
			lines.Add (line);
		}
		results["total_credits"] = totalCredits;
		results["xp"] = 0;
		results["paylines"] = lines;
		response["results"] = results;
		return response;
	}
	
	static int[] SpinReels() {
		var toReturn = new int[5];
		for(var i = 0; i < 5; i++) {
			toReturn[i] = UnityEngine.Random.Range (0, sm_info.GetSlotCount(i));
		}
		return toReturn;
	}
	
	static Dictionary<string, object> CheckLine(int[] reels, int lineIdx, int didSpin) {
		var toReturn = new Dictionary<string, object>();
		if(didSpin == 0) {
			toReturn["credits"] = 0;
		}
		else {
			toReturn["credits"] = sm_info.CheckLineWithSpin(reels, lineIdx);
		}
		
		return toReturn;
	}
}

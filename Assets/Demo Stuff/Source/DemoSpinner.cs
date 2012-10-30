using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using LitJson;

public class GemLocation {
	public GemType type;
	public int reel;
	public int idx;
	
	public GemLocation() {}
	
	public GemLocation(int reel, int idx, GemType type) {
		this.reel = reel;
		this.idx = idx;
		this.type = type;
	}
	
	public GemLocation(JsonData jd) {
		reel = (int)jd["reel"];
		idx = (int)jd["idx"];
		type = (GemType)(int)jd["type"];
	}
}

public class DemoSpinner {
	
	public static MachineInfo sm_info;
	public static int NUM_GEMS_PER_REEL = 5;
	
	public static void Spin(Spin spin, Action<string> callback) {
		var response = new Dictionary<string, object>();
		response["status"] = "OK";
		response["results"] = GetSpinResults(spin);
		var text = JsonMapper.ToJson(response);
		Debug.Log (text);
		callback(text);
	}
	
	static Dictionary<string, object> GetSpinResults(Spin spin) {
		var response = new Dictionary<string, object>();
		var reels = SpinReels();
		var gems = GetGemLocations();
		response["reels"] = reels;
		response["gems"] = gems;
		
		var results = new Dictionary<string, object>();
		var totalCredits = 0;
		var totalRed = 0;
		var totalGreen = 0;
		var totalBlue = 0;
		var lines = new List<object>();
		Debug.Log ("Bet is: " + spin.bet);
		for(var i = 0; i < spin.lines.Length; i++) {
			var line = CheckLine(reels, gems, i, spin.lines[i]);
			line["credits"] = (int)line["credits"] * spin.bet;
			var credits = (int)line["credits"];
			totalCredits += credits;
			if(credits > 0) {
				totalRed += (int)line["red_gems"];
				totalGreen += (int)line["green_gems"];
				totalBlue += (int)line["blue_gems"];
			}
			lines.Add (line);
		}
		results["total_credits"] = totalCredits;
		results["total_red_gems"] = totalRed;
		results["total_green_gems"] = totalGreen;
		results["total_blue_gems"] = totalBlue;
		results["xp"] = 0;
		results["paylines"] = lines;
		response["results"] = results;
		return response;
	}
	
	static List<GemLocation> GetGemLocations() {
		var toReturn = new List<GemLocation>();
		for(var reel = 0; reel < 5; reel++) {
			var usedIndexes = new List<int>();
			for(var i = 0; i < NUM_GEMS_PER_REEL; i++) {
				var idx = UnityEngine.Random.Range (0, sm_info.m_reels[reel].Count);
				while(usedIndexes.Contains (idx)) {
					idx = UnityEngine.Random.Range (0, sm_info.m_reels[reel].Count);
				}
				var type = (GemType)UnityEngine.Random.Range (0, 2);
				toReturn.Add (new GemLocation(reel, idx, type));
			}
		}
		return toReturn;
	}
	
	static int[] SpinReels() {
		var toReturn = new int[5];
		for(var i = 0; i < 5; i++) {
			toReturn[i] = UnityEngine.Random.Range (0, sm_info.GetSlotCount(i));
		}
		return toReturn;
	}
	
	static Dictionary<string, object> CheckLine(int[] reels, List<GemLocation> gems, int lineIdx, int didSpin) {
		var toReturn = new Dictionary<string, object>();
		if(didSpin == 0) {
			toReturn["credits"] = 0;
		}
		else {
			var credits = sm_info.CheckLineWithSpin(reels, lineIdx);
			toReturn["credits"] = credits;
			if(credits > 0) {
				int[] adjustedSpin = sm_info.AdjustSpinIndexesWithLine(reels, lineIdx);
				Debug.Log (string.Format ("Checking for gems: {0} {1} {2} {3} {4}", 
					adjustedSpin[0], 
					adjustedSpin[1],
					adjustedSpin[2],
					adjustedSpin[3],
					adjustedSpin[4]));
				toReturn["red_gems"] = FindGemsOfType(GemType.Red, adjustedSpin, gems);
				toReturn["green_gems"] = FindGemsOfType(GemType.Green, adjustedSpin, gems);
				toReturn["blue_gems"] = FindGemsOfType(GemType.Blue, adjustedSpin, gems);
			}
		}
		
		return toReturn;
	}
	
	static int FindGemsOfType(GemType type, int[] adjustedSpin, List<GemLocation> gems) {
		var counter = 0;
		foreach(var gl in gems) {
			if(gl.type == type && gl.idx == adjustedSpin[gl.reel]) {
				counter++;
			}
		}
		return counter;
	}
}

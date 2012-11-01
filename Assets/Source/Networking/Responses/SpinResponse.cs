using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;

public class LinePayout {
	public int credits;
	
	public LinePayout(JsonData data) {
		credits = (int)data["credits"];
	}
	
	public LinePayout() {
		credits = 0;
	}
}

public class SpinResponse {
	
	public int xp;
	public int totalCredits;
	public int totalRedGems;
	public int totalGreenGems;
	public int totalBlueGems;
	public List<LinePayout> lines;
	public List<GemLocation> gems;
	public int wheelStop;
	public int wheelReward;
	public int[] reels;
	
	public SpinResponse(JsonData data) {
		Debug.Log ("Parsing spin response");
		//Debug.Log (JsonMapper.ToJson (data));
		var results = (JsonData)data["results"];
		
		Debug.Log ("Parsing reel data");
		var reelData = (IList)results["reels"];
		Debug.Log ("Reel data: " + reelData);
		reels = new int[5];
		for(int i = 0; i < 5; i++) {
			var jd = (JsonData)reelData[i];
			Debug.Log (jd.GetJsonType());
			var id = (int)jd;
			Debug.Log ("Reel " + i + ": " + id);
			reels[i] = id;
		}
		
		Debug.Log ("Parsing gems data");
		var gemResults = (IList)results["gems"];
		Debug.Log ("gems data: " + gemResults);
		gems = new List<GemLocation>();
		for(var i = 0; i < gemResults.Count; i++) {
			var jd = (JsonData)gemResults[i];
			gems.Add (new GemLocation(jd));
		}
		
		Debug.Log ("Parsing results");
		var spinResults = (JsonData)results["results"];
		totalCredits = (int)spinResults["total_credits"];
		totalRedGems = (int)spinResults["total_red_gems"];
		totalGreenGems = (int)spinResults["total_green_gems"];
		totalBlueGems = (int)spinResults["total_blue_gems"];
		if(totalCredits > 0) {
			wheelStop = (int)spinResults["wheel"];
		}
		else {
			wheelStop = -1;
		}
		var tmpDict = (IDictionary)spinResults;
		foreach(var key in tmpDict.Keys) {
			if(key.Equals("wheel_reward")) {
				wheelReward = (int)spinResults["wheel_reward"];
			}
		}
		xp = (int)spinResults["xp"];
		
		//Debug.Log ("Parsing line payouts");
		var lineInfo = (IList)spinResults["paylines"];
		lines = new List<LinePayout>();
		for(var i = 0; i < lineInfo.Count; i++) {
			var jd = (JsonData)lineInfo[i];
			lines.Add (new LinePayout(jd));
		}
		
		Debug.Log ("Done parsing spin response");
	}
	
	public SpinResponse() {
		xp = 0;
		totalCredits = 0;
		lines = new List<LinePayout>();
		reels = new int[5];
	}
}

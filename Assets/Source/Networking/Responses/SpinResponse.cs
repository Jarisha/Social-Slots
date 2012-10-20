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
	public List<LinePayout> lines;
	public int[] reels;
	
	public SpinResponse(JsonData data) {
		var results = (JsonData)data["results"];
		
		//Debug.Log ("Parsing reel data");
		var reelData = (IList)results["reels"];
		//Debug.Log ("Reel data: " + reelData);
		reels = new int[5];
		for(int i = 0; i < 5; i++) {
			var jd = (JsonData)reelData[i];
			//Debug.Log (jd.GetJsonType());
			var id = (int)jd;
			//Debug.Log ("Reel " + i + ": " + id);
			reels[i] = id;
		}
		
		//Debug.Log ("Parsing results");
		var spinResults = (JsonData)results["results"];
		totalCredits = (int)spinResults["total_credits"];
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

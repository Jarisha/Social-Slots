using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;

[System.Serializable]
public class Payout {
	public int credits;
	public string[] line;
	
	public Payout(JsonData jd) {
		credits = (int)jd["credits"];
		IList lineData = (IList)jd["line"];
		line = new string[5];
		for(var i = 0; i < line.Length; i++) {
			var lineItem = (JsonData)lineData[i];
			line[i] = (string)lineItem;
		}
	}
	
	public bool DoesMatch(int[] spin, MachineInfo info) {
		for(var i = 0; i < 5; i++) {
			var lineIdx = info.IndexForName (line[i]);
			var spinIdx = spin[i];
			var isWild = (spinIdx == info.IndexForName("Wild"));
			if(lineIdx != info.IndexForName("Any") && !isWild && lineIdx != spinIdx) {
				return false;
			}
		}
		return true;
	}
	
	public override string ToString() {
		return string.Format ("{0}/{1}/{2}/{3}/{4} : {5}c",
			GetShortCode(line[0]),
			GetShortCode(line[1]),
			GetShortCode(line[2]),
			GetShortCode(line[3]),
			GetShortCode(line[4]),
			credits);
	}
	
	public string GetShortCode(string fullName) {
		switch(fullName) {
		case "Cherry":
			return "Ch";
		case "Bell":
			return "Bl";
		case "Bar":
			return "B";
		case "DoubleBar":
			return "2B";
		case "TripleBar":
			return "3B";
		case "Seven":
			return "Sv";
		case "Special":
			return "Sp";
		case "Orange":
			return "Or";
		case "Lemon":
			return "Le";
		case "Plum":
			return "Pl";
		case "Wild":
			return "W";
		case "Any":
			return "*";
		default:
			return "Unk";
		}
	}
}

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using LitJson;

public class SelectGameResponse {
	
	public MachineInfo machine;
	
	public SelectGameResponse(JsonData data) {
		try {
			machine = new MachineInfo(data["game_info"]);
		}
		catch(Exception e) {
			Debug.Log(e);
		}
	}
	
	public SelectGameResponse(MachineInfo mi) {
		machine = mi;
	}
}
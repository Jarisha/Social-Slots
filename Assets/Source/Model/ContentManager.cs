using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;

public class ContentManager {
	
	private static ContentManager sm_instance;
	
	public Player Player { get; set; }
	
	public static ContentManager Instance {
		get {
			if(sm_instance == null) {
				sm_instance = new ContentManager();
			}
			return sm_instance;
		}
	}
	
	private ContentManager() {
	}
	
	public void ReadPlayerInfo(JsonData jdata) {
		Player = new Player((JsonData)jdata["player"]);
	}
}

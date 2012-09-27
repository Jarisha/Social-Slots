using UnityEngine;
using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
	
	public void ReadPlayerInfo(JObject jo) {
		Player = new Player(jo["player"].ToObject<JObject>());
	}
}

using UnityEngine;
using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class Player {
	
	public string m_user;
	public int m_xp;
	public int m_level;
	public int m_credits;
	public bool m_isAuthenticated;
	
	//{"type":"auth_response","player":{"_username":"ryan","_xp":0,"_credits":500,"_level":1}}
	public Player(JObject jo) {
		m_user = (string)jo["_username"];
		m_xp = (int)jo["_xp"];
		m_credits = (int)jo["_credits"];
		m_level = (int)jo["_level"];
	}
	
	public void IncrementCredits(int val) {
		m_credits += val;
	}
}

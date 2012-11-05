using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;

public class Player {
	
	public string m_user;
	public int m_xp;
	public int m_level;
	public int m_credits;
	public int m_gold;
	public int m_redGems;
	public int m_greenGems;
	public int m_blueGems;
	public bool m_isAuthenticated;
	
	//{"type":"auth_response","player":{"_username":"ryan","_xp":0,"_credits":500,"_level":1}}
	public Player(JsonData jo) {
		m_user = (string)jo["_username"];
		m_xp = (int)jo["_xp"];
		m_credits = (int)jo["_credits"];
		m_level = (int)jo["_level"];
	}
	
	public Player() {
		m_user = "Carrie";
		m_xp = 0;
		m_credits = 100000;
		m_gold = 0;
		m_redGems = 0;
		m_greenGems = 0;
		m_blueGems = 0;
		m_level = 1;
	}
	
	public void IncrementCredits(int val) {
		m_credits += val;
	}
	
	public void IncrementRedGems(int val) {
		m_redGems += val;
	}
	
	public void IncrementGreenGems(int val) {
		m_greenGems += val;
	}
	
	public void IncrementBlueGems(int val) {
		m_blueGems += val;
	}
}

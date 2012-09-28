using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using LitJson;

public class Auth : Message {
	
	public string user;
	
	public Auth(string user) {
		this.user = user;
	}
	
	public static new string GetMessageType() {
		return "auth";
	}
	
	public override byte[] ToByteArray() {
		var data = new Dictionary<string, string>() {
			{"type", GetMessageType()},
			{"user", user}
		};
		Debug.Log ("Writing json string");
		var dataString = JsonMapper.ToJson(data);
		Debug.Log ("Getting bytes");
		var bytes = System.Text.Encoding.UTF8.GetBytes(dataString);
		var count = bytes.Length;
		Debug.Log ("Getting count bytes");
		var countBytes = System.BitConverter.GetBytes(count);
		Debug.Log ("Reversing count bytes");
		countBytes = ReverseIfNecessary(countBytes);
		var toReturn = new byte[count + 4];
		Debug.Log ("Copying bytes");
		Array.Copy(countBytes, toReturn, 4);
		Array.Copy(bytes, 0, toReturn, 4, count);
		
		return toReturn;
	}
}
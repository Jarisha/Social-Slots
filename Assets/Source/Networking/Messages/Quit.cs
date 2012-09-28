using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using LitJson;

public class Quit : Message {
	
	public Quit() {
	}
	
	public static new string GetMessageType() {
		return "quit";
	}
	
	public override byte[] ToByteArray() {
		var data = new Dictionary<string, string>() {
			{"type", GetMessageType()}
		};
		var dataString = JsonMapper.ToJson(data);
		var bytes = System.Text.Encoding.UTF8.GetBytes(dataString);
		var count = bytes.Length;
		var countBytes = System.BitConverter.GetBytes(count);
		countBytes = ReverseIfNecessary(countBytes);
		var toReturn = new byte[count + 4];
		Array.Copy(countBytes, toReturn, 4);
		Array.Copy(bytes, 0, toReturn, 4, count);
		
		return toReturn;
	}
}
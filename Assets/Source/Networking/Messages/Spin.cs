using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class Spin : Message {
	
	public int[] lines;
	public int bet;
	
	public Spin(int[] lines, int bet) {
		this.lines = lines;
		this.bet = bet;
	}
	
	public static new string GetMessageType() {
		return "spin";
	}
	
	public override byte[] ToByteArray() {
		var data = new Dictionary<string, object>() {
			{"type", GetMessageType()},
			{"lines", lines},
			{"bet", bet}
		};
		var dataString = JsonConvert.SerializeObject(data);
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
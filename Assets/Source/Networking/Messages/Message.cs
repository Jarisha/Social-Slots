using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;

public abstract class Message {
	
	public static string GetMessageType() {
		return "_INVALID_TYPE_";
	}
	
	public abstract byte[] ToByteArray();
	
	public byte[] ReverseIfNecessary(byte[] inArray) {
		byte temp;
		int highCtr = inArray.Length - 1;
		
		for (int ctr = 0; ctr < inArray.Length / 2; ctr++) {
		 temp = inArray[ctr];
		 inArray[ctr] = inArray[highCtr];
		 inArray[highCtr] = temp;
		 highCtr -= 1;
		}
		return inArray;
	}
}
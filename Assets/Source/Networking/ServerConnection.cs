using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class RecvStateObject {
	public const int MAX_BUF_SIZE = 65536;
	public byte[] buffer = new byte[MAX_BUF_SIZE];
	public int readAmount;
	public bool hasDeterminedLength;
	public int length;
	
	public RecvStateObject() {
		length = -1;
		hasDeterminedLength = false;
		readAmount = 0;
	}
}

public class ConnectionProxy {
	private static ServerConnection sm_con;
	
	public static ServerConnection Connection { get { return sm_con; } }
	
	public static void CreateConnection(string host, int port, Action callback) {
		sm_con = new ServerConnection(host, port, callback);
	}
	
	public static void Quit() {
		if(sm_con != null) {
			if(sm_con.IsConnected()) {
				sm_con.SendMessage(new Quit());
				sm_con.Disconnect();
				sm_con = null;
			}
			sm_con = null;
		}
	}
	
	public static void PrintDebugInfo() {
		Debug.Log ("Connection: " + sm_con);
		if(sm_con != null) {
			Debug.Log ("Connected: " + sm_con.IsConnected());
		}
	}
	
	public static bool HasConnection() {
		return sm_con != null && sm_con.IsConnected();
	}
}

public class ServerConnection {
	
	public static string DEV_HOST = "localhost";
	public static int DEV_PORT = 8080;
	public bool quit = false;
	
	protected Socket m_socket;
	protected Thread m_recvThread;
	
	private Action<JsonData> m_pendingCallback;
	private bool m_hasCallback = true;
	
	public ManualResetEvent m_recvDone = new ManualResetEvent(false);
	
	public ServerConnection (string host, int port, Action callback) {
		m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		m_socket.NoDelay = true;
		m_socket.BeginConnect (host, port, ConnectionComplete, callback);
	}
	
	public void Disconnect() {
		m_recvThread.Abort();
		m_socket.Disconnect(false);
		m_socket.Close();
		quit = true;
		m_recvDone.Set ();
	}
	
	public bool IsConnected() {
		return m_socket != null && m_socket.Connected;
	}
	
	void ConnectionComplete(IAsyncResult results) {
		Debug.Log("Connected!");
		StartRecvThread();
		var callback = results.AsyncState as Action;
		Debug.Log ("Done kicking off recv thread");
		callback();
		
	}
	
	void StartRecvThread() {
		m_recvThread = new Thread(RecvLoop);
		m_recvThread.Start ();
	}
	
	public void SendMessage(Message m, Action<JsonData> callback = null) {
		Debug.Log ("In send message");
		var toSend = m.ToByteArray();
		Debug.Log ("Got byte array");
		Debug.Log(string.Format("{0}: sending {1} bytes", m.GetType(), toSend.Length));
		m_socket.BeginSend (toSend, 0, toSend.Length, SocketFlags.None, SendMessageFinished, m);
		if(callback != null) {
			m_pendingCallback = callback;
			m_hasCallback = true;
		}
		else {
			m_pendingCallback = null;
			m_hasCallback = false;
		}
	}
	
	public void SendMessageFinished(IAsyncResult results) {
		var amt = m_socket.EndSend(results);
		Message m = (Message)results.AsyncState;
		Debug.Log (string.Format ("{0}: {1} bytes sent", m.GetType(), amt));
	}
	
	public void RecvLoop() {
		try {
			while(m_socket.Connected && !quit) {
				Debug.Log ("Starting recv");
				m_recvDone.Reset ();
				RecvStateObject state = new RecvStateObject();
				m_socket.BeginReceive(state.buffer, state.readAmount, RecvStateObject.MAX_BUF_SIZE, SocketFlags.None, DataReceived, state);
				m_recvDone.WaitOne();
			}
		}
		catch(ThreadAbortException tae) {
			Debug.Log ("Caught " + tae.GetType ().Name + " -- aborting recv thread");
		}
	}
	
	public void DataReceived(IAsyncResult results) {
		Debug.Log ("Data received");
		var state = results.AsyncState as RecvStateObject;
		var amt = m_socket.EndReceive(results);
		Debug.Log ("Recv " + amt + " bytes");
		if(amt == 0) {
			FinishRecv(state);
		}
		else {
			state.readAmount += amt;
			if(amt > 4) {
				var lenBytes = new byte[4];
				Array.Copy (state.buffer, lenBytes, 4);
				if(BitConverter.IsLittleEndian) {
					Array.Reverse (lenBytes);
				}
				state.length = BitConverter.ToInt32 (lenBytes, 0);
				state.hasDeterminedLength = true;
			}
			if(!state.hasDeterminedLength || (state.hasDeterminedLength && state.readAmount < state.length)) {
				m_socket.BeginReceive(state.buffer, state.readAmount, RecvStateObject.MAX_BUF_SIZE, SocketFlags.None, DataReceived, state);
			}
			else {
				FinishRecv(state);
			}
		}
	}
	
	void FinishRecv(RecvStateObject state) {
		Debug.Log("Done getting packet");
		var jsonText = System.Text.Encoding.UTF8.GetString(state.buffer, 4, state.length);
		Debug.Log (jsonText);
		JsonData obj = null;
		try {
			obj = JsonMapper.ToObject(jsonText);
		}
		catch(Exception e) {
			Debug.LogWarning (e.ToString());
		}
		DoCallback(obj);
		m_recvDone.Set();
	}
	
	void DoCallback(JsonData jo) {
		if(m_hasCallback) {
			m_pendingCallback(jo);
			m_pendingCallback = null;
			m_hasCallback = false;
		}
	}
}


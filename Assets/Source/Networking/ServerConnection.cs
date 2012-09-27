using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
				sm_con.SendMessage(new Quit(), (jo) => {
					sm_con.Disconnect();
					sm_con = null;
				});
			}
			sm_con = null;
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
	
	private Action<JObject> m_pendingCallback;
	private bool m_hasCallback = true;
	
	public ManualResetEvent m_recvDone = new ManualResetEvent(false);
	
	public ServerConnection (string host, int port, Action callback) {
		m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		m_socket.NoDelay = true;
		m_socket.BeginConnect (host, port, ConnectionComplete, callback);
	}
	
	public void Disconnect() {
		m_socket.Close();
	}
	
	public bool IsConnected() {
		return m_socket.Connected;
	}
	
	void ConnectionComplete(IAsyncResult results) {
		Debug.Log("Connected!");
		StartRecvThread();
		var callback = results.AsyncState as Action;
		callback();
		
	}
	
	void StartRecvThread() {
		var thread = new Thread(RecvLoop);
		thread.Start ();
	}
	
	public void Login(Auth auth, Action<JObject> callback) {
		Debug.Log ("Sending auth");
		var toSend = auth.ToByteArray();
		m_socket.BeginSend (toSend, 0, toSend.Length, SocketFlags.None, LoginFinished, null);
		m_pendingCallback = callback;
		m_hasCallback = true;
	}
	
	public void LoginFinished(IAsyncResult results) {
		Debug.Log ("Done sending auth");
		var amt = m_socket.EndSend (results);
		Debug.Log ("Amount sent for auth: " + amt);
		SocketError errorCode;
		m_socket.EndSend(results, out errorCode);
		Debug.Log (errorCode);
	}
	
	public void SelectGame(SelectGame sg, Action<JObject> callback) {
		Debug.Log ("Sending select game");
		var toSend = sg.ToByteArray();
		m_socket.BeginSend (toSend, 0, toSend.Length, SocketFlags.None, LoginFinished, null);
		m_pendingCallback = callback;
		m_hasCallback = true;
	}
	
	public void SelectGameFinished(IAsyncResult results) {
		Debug.Log ("Done sending select game");
		var amt = m_socket.EndSend (results);
		Debug.Log ("Amount sent for select game: " + amt);
		SocketError errorCode;
		m_socket.EndSend(results, out errorCode);
		Debug.Log (errorCode);
	}
	
	public void Spin(Spin spin, Action<JObject> callback) {
		Debug.Log ("Sending spin");
		var toSend = spin.ToByteArray();
		m_socket.BeginSend (toSend, 0, toSend.Length, SocketFlags.None, LoginFinished, null);
		m_pendingCallback = callback;
		m_hasCallback = true;
	}
	
	public void SpinFinished(IAsyncResult results) {
		Debug.Log ("Done sending spin");
		var amt = m_socket.EndSend (results);
		Debug.Log ("Amount sent for spin: " + amt);
		SocketError errorCode;
		m_socket.EndSend(results, out errorCode);
		Debug.Log (errorCode);
	}
	
	public void SendMessage(Message m, Action<JObject> callback = null) {
		var toSend = m.ToByteArray();
		Debug.Log(string.Format("{0}: sending {1} bytes", m.GetType(), toSend.Length));
		m_socket.BeginSend (toSend, 0, toSend.Length, SocketFlags.None, SendMessageFinished, m);
		if(callback != null) {
			m_pendingCallback = DoCallback;
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
		while(m_socket.Connected && !quit) {
			Debug.Log ("Starting recv");
			m_recvDone.Reset ();
			RecvStateObject state = new RecvStateObject();
			m_socket.BeginReceive(state.buffer, state.readAmount, RecvStateObject.MAX_BUF_SIZE, SocketFlags.None, DataReceived, state);
			m_recvDone.WaitOne();
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
		var jobj = JObject.Parse (jsonText);
		DoCallback(jobj);
		m_recvDone.Set();
	}
	
	void DoCallback(JObject jo) {
		if(m_hasCallback) {
			m_pendingCallback(jo);
			m_pendingCallback = null;
			m_hasCallback = false;
		}
	}
}


using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

// State object for receiving data from remote device.  
public class StateObject {
	// Client socket.  
	public Socket workSocket = null;
	// Size of receive buffer.  
	public const int BufferSize = 1024;
	// Receive buffer.  
	public byte[] buffer = new byte[BufferSize];
	// Received data string.  
	public StringBuilder sb = new StringBuilder();
}

// Singleton instance
public class ClientSocket : MonoBehaviour {
	public static ClientSocket Instance { get; private set; }

	const string IPADDRESS = "72.220.5.176";
	const int PORT = 11000;
	public static bool connected;

	public static ManualResetEvent connectDone;
	public static ManualResetEvent receiveDone;
	public static ManualResetEvent sendDone;
	public static string response;

	private static Socket clientSocket;

	private void Awake() {
		if (!Instance) {
			Instance = this;
			DontDestroyOnLoad(gameObject);
		} else Destroy(gameObject);
	}

	// Start is called before the first frame update
	private void Start() {
		ConnectToServer();
	}

	public static void ConnectToServer() {
		connected = false;
		connectDone = new ManualResetEvent(false);
		response = string.Empty;

		try {
			IPHostEntry host = Dns.GetHostEntry(IPADDRESS);
			IPAddress ipAddress = host.AddressList[0];
			IPEndPoint remoteEP = new IPEndPoint(ipAddress, PORT);

			clientSocket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			Debug.Log("Attempting to connect...");
			clientSocket.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), clientSocket);
		} catch (Exception e) {
			Debug.Log(e.ToString());
		}
	}

	public static void Send(string data) {
		sendDone = new ManualResetEvent(false);

		if (!connected) {
			Debug.LogError("Could not send data; not connected");
			return;
		}

		// Convert the string data to byte data using ASCII encoding.  
		byte[] byteData = Encoding.ASCII.GetBytes(data);

		// Begin sending the data to the remote device.  
		clientSocket.BeginSend(byteData, 0, byteData.Length, SocketFlags.None,
			new AsyncCallback(SendCallback), null);

		sendDone.WaitOne();


	}

	private static void SendCallback(IAsyncResult ar) {
		try {
			// Complete sending the data to the remote device.  
			int bytesSent = clientSocket.EndSend(ar);
			Debug.Log(string.Format("Sent {0} bytes to server.", bytesSent));
			// Signal that all bytes have been sent.  
			sendDone.Set();
		} catch (SocketException socketException) {
			Debug.Log(socketException);
			connected = false;
			sendDone.Set();
		} catch (Exception e) {
			Debug.Log(e.ToString());
			sendDone.Set();
		} 
	}

	public static void Receive() {
		receiveDone = new ManualResetEvent(false);

		if (!connected) {
			Debug.LogError("Could not receive data; not connected");
			return;
		}

		try {
			// Create the state object.  
			StateObject state = new StateObject {
				workSocket = clientSocket
			};

			// Begin receiving the data from the remote device.  
			Debug.Log("Begin receive");
			clientSocket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
			receiveDone.WaitOne();
		} catch (Exception e) {
			Debug.Log(e.ToString());
		}
	}

	private static void ReceiveCallback(IAsyncResult AR) {
		try {
			StateObject state = (StateObject) AR.AsyncState;
			Socket client = state.workSocket;
			int received = client.EndReceive(AR);

			string text = Encoding.ASCII.GetString(state.buffer, 0, received);
			Debug.Log(string.Format("Received {0} bytes from server.", text.Length));

			state.sb.Append(text);

			if (text.IndexOf("<EOF>") > -1) {
				text = state.sb.ToString();
				text = text.Substring(0, text.Length - 5);
				response = text;
				receiveDone.Set();

			} else {
				client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
			}


		} catch (SocketException socketException) {
			Debug.Log(socketException);
			connected = false;
			receiveDone.Set();
		} catch (Exception e) {
			Debug.Log(e.ToString());
			receiveDone.Set();
		}
	}

	private static void ConnectCallback(IAsyncResult AR) {
		Socket client = (Socket) AR.AsyncState;

		try {
			client.EndConnect(AR);
			Debug.Log("Successfully connected");
			connected = true;
		} catch (Exception e) {
			Debug.Log(e.ToString());
			client.Close();
		}
	}

	private static void DisconnectFromServer() {
		Debug.Log("Disconnecting from server");
		clientSocket.Shutdown(SocketShutdown.Both);
		clientSocket.Close();
	}

	// Update is called once per frame
	void OnApplicationQuit() {
		if (connected)
			DisconnectFromServer();
	}
}

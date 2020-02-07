using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Web;
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

public struct AnnouncementEntry {
	public string Title { get; set; }
	public string Date { get; set; }
	public string Body { get; set; }

	public AnnouncementEntry(string _title, string _date, string _body) {
		Title = _title;
		Date = _date;
		Body = _body;
	}

	public override string ToString() {
		return string.Format("Title: {0} | Date: {1} | Body: {2}", Title, Date, Body);
	}
}

public class DownloadAnnouncements : MonoBehaviour {
	const string IPADDRESS = "72.220.5.176";
	const int PORT = 11000;
	private static bool connected;

	static ManualResetEvent connectDone;
	static ManualResetEvent receiveDone;

	//static Socket clientSocket;
	static byte[] buffer;
	static List<AnnouncementEntry> entries;

	static string response = string.Empty;

	static Socket clientSocket;


	public static List<AnnouncementEntry> ConnectAndRetrieveAnnouncements() {
		if (entries != null) entries.Clear();
		connected = false;

		connectDone = new ManualResetEvent(false);
		receiveDone = new ManualResetEvent(false);
		response = string.Empty;

		try {
			IPHostEntry host = Dns.GetHostEntry(IPADDRESS);
			IPAddress ipAddress = host.AddressList[0];
			IPEndPoint remoteEP = new IPEndPoint(ipAddress, PORT);

			clientSocket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			Debug.Log("Attempting to connect...");
			clientSocket.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), clientSocket);
			connectDone.WaitOne();

			if (connected) {
				Receive(clientSocket);
				receiveDone.WaitOne();

				Debug.Log("passed");
				entries = JsonConvert.DeserializeObject<List<AnnouncementEntry>>(response);

				// Convert back into readable
				for (int i = 0; i < entries.Count; i++) {
					var entry = entries[i];
					entry.Title = HttpUtility.HtmlDecode(entry.Title);
					entry.Body = HttpUtility.HtmlDecode(entry.Body);

					entries[i] = entry;
				}

				Debug.Log(string.Format("Received {0} bytes from server.", response.Length));

				foreach (var entry in entries) {
					Debug.Log(entry.ToString());
				}

				Debug.Log("Disconnecting from server");
				clientSocket.Shutdown(SocketShutdown.Both);
				clientSocket.Close();
			} else {
				Debug.Log("Could not establish a connection to the server.");
			}

			return entries;
			
		} catch (Exception e) {
			Debug.Log(e.ToString());
			return null;
		}
	}

	private static void Receive (Socket client) {
		try {
			// Create the state object.  
			StateObject state = new StateObject();
			state.workSocket = client;

			// Begin receiving the data from the remote device.  
			Debug.Log("Begin receive");
			client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
		} catch (Exception e) {
			Console.WriteLine(e.ToString());
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
				Debug.Log(text);
				response = text;
				receiveDone.Set();

			} else {
				client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
			}

			//buffer = new byte[clientSocket.ReceiveBufferSize];


			//string text = Encoding.ASCII.GetString(buffer);

			//Debug.Log(text);

			//Array.Resize(ref buffer, clientSocket.ReceiveBufferSize);


		} catch (Exception e) {
			Debug.Log(e.ToString());
		}
	}

	private static void ConnectCallback(IAsyncResult AR) {
		Socket client = (Socket) AR.AsyncState;

		try {
			client.EndConnect(AR);
			Debug.Log("Successfully connected");
			connected = true;
			connectDone.Set();
		} catch (Exception e) {
			Debug.Log(e.ToString());
			client.Close();
			connectDone.Set();
		}
	}
}

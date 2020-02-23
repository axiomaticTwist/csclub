using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web;
using UnityEngine;
using System.Web.Util;

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

// TODO: Give calendars unique colors
public static class SendAndReceive {
	static List<AnnouncementEntry> entries = new List<AnnouncementEntry>();
	static List<StaffInfo> directory = new List<StaffInfo>();
	static Dictionary<long, string> userTopics;

	public static string SendDeviceToken(string token) {
		try {
			ClientSocket.Send(string.Format("1 {0}<EOF>", token));
			ClientSocket.Receive();
			return ClientSocket.response;
		} catch (Exception e) {
			Debug.Log(e.ToString());
			return null;
		}
	}

	public static Dictionary<long, string> RetrieveTopics() {
		try {
			ClientSocket.Send("3 <EOF>");
			ClientSocket.Receive();

			userTopics = JsonConvert.DeserializeObject<Dictionary<long, string>>(ClientSocket.response);

			return userTopics;

		} catch (Exception e) {
			Debug.Log(e.ToString());
			return null;
		}
	}

	public static List<AnnouncementEntry> RetrieveAnnouncements() {
		try {
			ClientSocket.Send("0 <EOF>");
			ClientSocket.Receive();

			entries = JsonConvert.DeserializeObject<List<AnnouncementEntry>>(ClientSocket.response);

			// Convert back into readable
			for (int i = 0; i < entries.Count; i++) {
				var entry = entries[i];
				HttpEncoder.Current = HttpEncoder.Default;
				entry.Title = HttpUtility.HtmlDecode(entry.Title);
				entry.Body = HttpUtility.HtmlDecode(entry.Body);

				entries[i] = entry;
			}

			return entries;

		} catch (Exception e) {
			Debug.Log(e.ToString());
			return null;
		}
	}

	public static List<StaffInfo> RetrieveDirectory() {
		try {
			ClientSocket.Send("4 <EOF>");
			ClientSocket.Receive();

			directory = JsonConvert.DeserializeObject<List<StaffInfo>>(ClientSocket.response);

			return directory;
		} catch (Exception e) {
			Debug.Log(e.ToString());
			return null;
		}
	}
}

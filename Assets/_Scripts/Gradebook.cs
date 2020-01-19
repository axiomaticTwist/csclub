using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;

public static class Gradebook {
	public static void Main(string[] args) {
		string username = "";
		string password = "";

		string postString = String.Format("username={0}&amp;password={1}", username, password);

		HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create("https://sis.powayusd.com/PXP2_Login_Student.aspx?regenerateSessionId=True");

		httpRequest.Method = "POST";
		httpRequest.ContentType = "application/x-www-form-urlencoded";

		byte[] byteData = System.Text.Encoding.UTF8.GetBytes(postString);
		httpRequest.ContentLength = byteData.Length;

		Stream requestStream = httpRequest.GetRequestStream();
		requestStream.Write(byteData, 0, byteData.Length);
		requestStream.Close();

		HttpWebResponse httpResponse = (HttpWebResponse) httpRequest.GetResponse();
		Stream responseStream = httpResponse.GetResponseStream();

		System.Text.StringBuilder sb = new System.Text.StringBuilder();
		using (StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.UTF8)) {
			string line;
			while ((line = reader.ReadLine()) != null) {
				sb.Append(line);
			}
		}

		string serverResponse = sb.ToString();
	}

}

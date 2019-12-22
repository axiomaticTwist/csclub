using System;
using UnityEngine;

public class DateObject : MonoBehaviour {
	private DateTime date;
	internal Calendar calendar;

	// Find the GetInfo script on the Info Content Panel object
	private void Start() {
		// getInfo = GameObject.Find("Info Content Panel").GetComponent<GetInfo>();
	}
	
	// Show the information on a given date when the day has been clicked on
	public void ShowInfo() {
		calendar.DisplayEvents(date);
	}

	public void SetDate(DateTime _date) {
		date = _date;
	}

	public DateTime GetDate() {
		return date;
	}
}

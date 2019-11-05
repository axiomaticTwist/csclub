using System;
using UnityEngine;

public class DateObject : MonoBehaviour {
	private DateTime date;
	private GetInfo getInfo;

	private void Start() {
		getInfo = GameObject.Find("Info Content Panel").GetComponent<GetInfo>();
	}

	public void ShowInfo() {
		getInfo.GetDayInfo(date);
	}

	public void SetDate(DateTime _date) {
		date = _date;
	}

	public DateTime GetDate() {
		return date;
	}
}

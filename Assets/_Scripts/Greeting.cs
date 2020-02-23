using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Greeting : MonoBehaviour {
	public Text greeting;
	public Text date;
	public Text hour;
	public Text second;
	public Text minute;

	public TimeSpan morning = new TimeSpan(0, 0, 0);
	public TimeSpan afternoon = new TimeSpan(12, 0, 0);
	public TimeSpan evening = new TimeSpan(18, 0, 0);

	DateTime now;

	// Update is called once per frame
	void Update() {
		now = DateTime.Now;

		if (now.TimeOfDay >= morning && now.TimeOfDay < afternoon) {
			greeting.text = "Good Morning!";
		} else if (now.TimeOfDay >= afternoon && now.TimeOfDay < evening) {
			greeting.text = "Good Afternoon!";
		} else {
			greeting.text = "Good Evening!";
		}

		date.text = string.Format("Today is {0:dddd, MMMM dd}\n{1:MM/dd/yyyy}", now, now);
		hour.text = now.ToString("%h");
		minute.text = now.ToString("mm tt");

		if (now.Second % 2 == 0) {
			second.enabled = true;
		} else {
			second.enabled = false;
		}
	}
}

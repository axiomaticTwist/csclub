using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsPanel : MonoBehaviour {
	public Calendar calendar;
	public GetInfo getInfo;

	public void Show() {
		gameObject.SetActive(true);
	}

	// Called on button pressed
	public void Exit() {
		// Hides the interface
		gameObject.SetActive(false);

		// Refreshes all the events
		calendar.RefreshCalendar(true);
		calendar.PopulateCalendar();
		calendar.DisplayCalendar();
		getInfo.Refresh();
		
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsPanel : MonoBehaviour {
	public GoogleCalendar googleCal;
	public PopulateCalendar popCal;
	public GetInfo getInfo;

	// Called on button pressed
	public void Exit() {
		// Hides the interface
		gameObject.SetActive(false);

		// Refreshes all the events
		googleCal.Repopulate();
		popCal.RefreshCalendar();
		getInfo.Refresh();
		
	}
}

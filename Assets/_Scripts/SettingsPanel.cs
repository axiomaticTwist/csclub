using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsPanel : MonoBehaviour {
	public GoogleCalendar googleCal;
	public PopulateCalendar popCal;
	public GetInfo getInfo;

	// Start is called before the first frame update
	void Start() {

	}

	// Update is called once per frame
	void Update() {

	}

	public void Exit() {
		gameObject.SetActive(false);

		googleCal.Repopulate();
		popCal.RefreshCalendar();
		getInfo.Refresh();
		
	}
}

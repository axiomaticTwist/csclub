using UnityEngine;

public class SettingsPanel : MonoBehaviour {
	public Calendar calendar;
	public GetInfo getInfo;
	public GameObject loadingPanel;

	public void Show() {
		gameObject.SetActive(true);
	}

	// Called on button pressed
	public void Exit() {
		// Hides the interface
		gameObject.SetActive(false);

		// Refreshes all the events
		calendar.StartCoroutine(calendar.LoadCalendar());

		loadingPanel.SetActive(true);

		getInfo.Refresh();
		
	}
}

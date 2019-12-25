using UnityEngine;

public class SettingsPanel : MonoBehaviour {
	public Calendar calendar;
	public GameObject loadingPanel;

	public void Show() {
		gameObject.SetActive(true);
	}

	// Called on button pressed
	public void Exit() {
		// Hides the interface
		gameObject.SetActive(false);

		calendar.RefreshEventList();
		// Refreshes all the events
		calendar.RefreshCalendar(true);
		calendar.DisplayCalendar();
		calendar.StartCoroutine(calendar.LoadCalendar(true));

		loadingPanel.SetActive(true);
		
	}
}

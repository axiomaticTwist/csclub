using UnityEngine;
using System;
using System.Linq;

public class SettingsPanel : MonoBehaviour {
	public VisualizeCalendar calendar;
	public GameObject loadingPanel;
	private CalendarURL[] calendars;

	public void Open() {
		calendars = new CalendarURL[GenerateCalendarData.Instance.calendars.Length];
		Array.Copy(GenerateCalendarData.Instance.calendars, calendars, GenerateCalendarData.Instance.calendars.Length);

	}

	// Called on button pressed
	public void Exit() {
		if (!Enumerable.SequenceEqual(GenerateCalendarData.Instance.calendars, calendars)) {
			Debug.Log("Refresh");
			GenerateCalendarData.Instance.ClearAllEvents();
			calendar.RefreshCalendar();
			GenerateCalendarData.Instance.PopulateCalendar();
		}

		PlayerPrefs.Save();
	}
}

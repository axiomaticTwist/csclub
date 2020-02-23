using System;
using System.Collections;
using System.Globalization;
using System.Threading;
using static SceneLoader;
using UnityEngine;
using UnityEngine.UI;
using Event = Google.Apis.Calendar.v3.Data.Event;
using System.Collections.Generic;

public class WeekAtAGlance : MonoBehaviour {
	private Dictionary<DateTime, Transform> days = new Dictionary<DateTime, Transform>();
	public Transform[] dayObjects;

	public Font boldFont;
	public Transform infoItem;
	public Transform infoContentPanel;
	public EventPanel eventPanel;
	public GameObject loadPanel;

	public int maxResults = 20;

	// Start is called before the first frame update
	void Start() {
		GenerateWeek();

		if (GenerateCalendarData.populateWeek == true) {
			PopulateWeek();
			GenerateCalendarData.populateWeek = false;
		}

		CheckForEvents(GenerateCalendarData.Instance.allEvents);
	}

	private void GenerateWeek() {
		DateTime tempDay = FirstDayOfWeekUtility.GetFirstDateOfWeek(DateTime.Today);

		for (int i = 0; i < dayObjects.Length; i++) {
			dayObjects[i].Find("Date").GetComponent<Text>().text = tempDay.ToString("%d");
			
			dayObjects[i].GetComponent<DateObject>().date = tempDay;
			
			days[tempDay] = dayObjects[i];

			if (tempDay == DateTime.Today) {
				foreach (Text t in dayObjects[i].GetComponentsInChildren<Text>()) {
					t.color = Color.white;
				}
				dayObjects[i].GetComponent<Image>().color = new Color32(80, 200, 120, 255);
			}

			tempDay = tempDay.AddDays(1);
		}
	}

	public async void PopulateWeek() {
		GenerateCalendarData.Instance.allEvents.Clear();

		foreach (CalendarURL calendar in GenerateCalendarData.Instance.calendars) {
			Dictionary<DateTime, List<EventInfo>> x = await GenerateCalendarData.Instance.GetCalendarEventsAsync(calendar, GenerateCalendarData.Instance.maxResults);

			CheckForEvents(x);
		}
	}

	private void CheckForEvents(Dictionary<DateTime, List<EventInfo>> eventsOnDays) {
		foreach (DateTime key in eventsOnDays.Keys) {
			if (days.ContainsKey(key)) {
				days[key].Find("Date").GetComponent<Text>().font = boldFont;
			}
		}
	}
}

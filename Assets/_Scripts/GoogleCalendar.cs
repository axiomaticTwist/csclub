using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Event = Google.Apis.Calendar.v3.Data.Event;

[Serializable]
public struct CheckedString {
	public bool check;
	public string name;
}

public class GoogleCalendar : MonoBehaviour {
	public GameObject calendarItemPrefab;
	public Transform calendarsPanel;
	public CheckedString[] calendars;
	private List<IList<Event>> calendarEventList = new List<IList<Event>>();

	// If modifying these scopes, delete your previously saved credentials
	// at ~/.credentials/calendar-dotnet-quickstart.json
	static string[] Scopes = { CalendarService.Scope.CalendarReadonly };
	static string ApplicationName = "Google Calendar API .NET Quickstart";

	// Create Google Calendar API service.
	CalendarService service = new CalendarService(new BaseClientService.Initializer() {
		ApiKey = "AIzaSyCAkCz0rjx4gw1omV7CuWR8wq_0dhKg2ww",
		ApplicationName = ApplicationName,
	});


	// TODO: store each calendar data in a jagged array

	private void Start() {
		foreach(CheckedString s in calendars) {
			EventsResource.ListRequest requestNames = service.Events.List(s.name);
			Events e = requestNames.Execute();

			GameObject item = Instantiate(calendarItemPrefab, calendarsPanel);
			item.GetComponentInChildren<Text>().text = e.Summary;
			item.GetComponentInChildren<Toggle>().onValueChanged.AddListener(delegate { UpdateCheckedString(item.GetComponentInChildren<Toggle>().isOn, item.transform.GetSiblingIndex()); });

		}

		Repopulate();
	}

	public void UpdateCheckedString(bool active, int index) {
		calendars[index].check = active;

	}

	public void Repopulate() {
		for (int i = 0; i < calendars.Length;i++) {
			if (calendars[i].check) {

				// Define parameters of request.
				EventsResource.ListRequest request = service.Events.List(calendars[i].name);
				request.TimeMin = DateTime.Today;
				request.ShowDeleted = false;
				request.SingleEvents = true;
				request.MaxResults = 100;
				request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

				// List events.
				Events events = request.Execute();

				if (events.Items != null && events.Items.Count > 0) {
					calendarEventList.Add(events.Items);
					Debug.Log("Event List Added");

					foreach (Event e in events.Items) {
						Debug.Log(e.Summary);

					}
					
				} else {
					Debug.Log("No upcoming events found.");
				}
			}
		}
	}

	public IList<Event> GetEvents(int index) {
		return calendarEventList[index];
	}
}

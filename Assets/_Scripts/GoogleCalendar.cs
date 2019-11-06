using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Event = Google.Apis.Calendar.v3.Data.Event;

// Calendar URLs and booleans to state whether or not we want to view this calendar
[Serializable]
public struct CheckedString {
	public bool check;
	public string name;
}

public class GoogleCalendar : MonoBehaviour {
	public GameObject calendarItemPrefab;
	public Transform calendarsPanel;

	// Which calendars to use
	public CheckedString[] calendars;

	public List<string> calendarNames = new List<string>();

	// Multiple lists of events stored in a single list
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
	
	private void Start() {
		// Get the names of every single calendar in the array
		foreach(CheckedString s in calendars) {
			// Call the Google API service
			EventsResource.ListRequest requestNames = service.Events.List(s.name);
			requestNames.MaxResults = 1;
			Events e = requestNames.Execute();

			// Add the name of the calendar to the calendar name list
			calendarNames.Add(e.Summary);

			// Create a checkbox for every calendar
			GameObject item = Instantiate(calendarItemPrefab, calendarsPanel);
			// Set the text of the checkbox to the name of the calendar
			item.GetComponentInChildren<Text>().text = e.Summary;

			// Add an event listener when the checkbox is ticked
			// Update the calendar to either activate or deactivate the calendar at the given index
			item.GetComponentInChildren<Toggle>().onValueChanged.AddListener(delegate { UpdateCheckedString(item.GetComponentInChildren<Toggle>().isOn, item.transform.GetSiblingIndex()); });

		}

		// Refresh the events
		Repopulate();
	}

	// Set the calendar to active at a given index, which will display the calendar's events on the next refresh
	public void UpdateCheckedString(bool active, int index) {
		calendars[index].check = active;

	}

	// Repopulates the calendar events list
	public void Repopulate() {
		// Loop through however many calendars we have
		for (int i = 0; i < calendars.Length;i++) {
			// If the user states they want to use the calendar (the checkbox is ticked)
			if (calendars[i].check) {

				// Define parameters of request.
				EventsResource.ListRequest request = service.Events.List(calendars[i].name);
				request.TimeMin = DateTime.Today;
				request.ShowDeleted = false;
				request.SingleEvents = true;
				request.MaxResults = 50;
				request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

				// List events.
				Events events = request.Execute();

				// If we have more than one event, add the event list to an array
				if (events.Items != null && events.Items.Count > 0) {
					calendarEventList.Add(events.Items);
				} else {
					Debug.Log("No upcoming events found.");
				}
			}
		}
	}

	// Gets the event list at a given calendar index
	public IList<Event> GetEvents(int index) {
		return calendarEventList[index];
	}
}

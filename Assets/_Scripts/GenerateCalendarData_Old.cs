using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using static Google.Apis.Calendar.v3.EventsResource;
using Event = Google.Apis.Calendar.v3.Data.Event;



// TODO: Make async
// Singleton instance
public class GenerateCalendarData_Old : MonoBehaviour {
	public event Action CalendarFinishEvent;

	public static GenerateCalendarData_Old Instance { get; private set; }

	public int maxResults = 100;

	bool done = false;

	#region Backend
	[Header("Calendars")]
	public CalendarURL[] calendars;

	private Dictionary<DateTime, List<EventInfo>> eventsOnDay = new Dictionary<DateTime, List<EventInfo>>();

	// If modifying these scopes, delete your previously saved credentials
	// at ~/.credentials/calendar-dotnet-quickstart.json
	private static string[] Scopes = { CalendarService.Scope.CalendarReadonly };
	private static string ApplicationName = "Insight";
	#endregion

	private void Awake() {
		if (!Instance) {
			Instance = this;
			DontDestroyOnLoad(gameObject);
		} else {
			Destroy(gameObject);
		}
	}


	public void GenerateCalendarCheckboxes() {
		for (int i = 0; i < calendars.Length; i++) {
			calendars[i].useCalendar = PlayerPrefsX.GetBool(calendars[i].title);
		}
	}

	public IEnumerator GenerateCalendar(DateTime startTime, int maxResults) {
		done = false;

		new Thread(() => {
			PopulateCalendar(startTime, maxResults);
			done = true;
		}).Start();
		
		while (!done) {
			CalendarFinishEvent?.Invoke();
			yield return null;
		}

		
	}

	public void PopulateCalendar(DateTime startTime, int maxResults) {
		RefreshCalendar();
		// Create Google Calendar API service.
		CalendarService service = new CalendarService(new BaseClientService.Initializer() {
			ApiKey = "AIzaSyCAkCz0rjx4gw1omV7CuWR8wq_0dhKg2ww",
			ApplicationName = ApplicationName,
		});

		foreach (CalendarURL c in calendars) {
			if (c.useCalendar) {
				ListRequest request = service.Events.List(c.url);

				// Create a request for events starting from previous month onward, until 200 events have been added
				request.TimeMin = startTime;
				request.ShowDeleted = false;
				request.SingleEvents = true;
				request.MaxResults = maxResults;
				request.OrderBy = ListRequest.OrderByEnum.StartTime;

				Events events = request.Execute(); // Execute the request

				if (events.Items != null && events.Items.Count > 0) {
					foreach (Event e in events.Items) {
						// Get start and end date
						var tempStart = e.Start.DateTime.ToString();
						var tempEnd = e.End.DateTime.ToString();

						// If the date doesn't is in a bad form, fix it
						if (string.IsNullOrEmpty(tempStart)) {
							tempStart = e.Start.Date;
						}
						if (string.IsNullOrEmpty(tempEnd)) {
							tempEnd = e.End.Date;
						}

						// Parse the event start and end times
						/**
						 * ~~~IMPORTANT~~~
						 * When an event is created in Google calendar, the event defaults to "All Day." When an event
						 * is all day, it runs from 12 AM of the current day to 12 AM of the next day. This means the
						 * difference in days between the start of the event and the end of the event is always 1 for
						 * all-day events. These next two lines ensure that if an event is all-day, the difference
						 * between the start and end dates is 0. If an event runs from 12 AM of the current day to
						 * 12:01 of the next day, then the event is counted as a multi-day event.
						**/
						DateTime start = DateTime.Parse(tempStart).Date;
						DateTime end = DateTime.Parse(tempEnd).AddSeconds(-1).Date;

						int differenceBetweenDays = Mathf.Abs((start - end).Days); // Get difference in days between start and end

						// Create at least one EventInfo every time
						for (int i = 0; i <= differenceBetweenDays; i++) {
							if (!eventsOnDay.ContainsKey(start.AddDays(i))) {
								// Create a new list of EventInfos
								eventsOnDay.Add(start.AddDays(i), new List<EventInfo>());
							}

							// Add the event info to the list of events on that day
							eventsOnDay[start.AddDays(i)].Add(new EventInfo(events.Summary, e, c.color));
						}
					}
				} else {
					Debug.Log("No upcoming events found.");
				}

			}

		}
	}

	public void RefreshCalendar() {
		eventsOnDay.Clear();
	}

	public bool HasEventOnDay(DateTime key) {
		if (eventsOnDay.ContainsKey(key))
			return true;
		else return false;
	}

	public List<EventInfo> GetListOfEventInfos(DateTime date) {
		return eventsOnDay[date];
	}

	public EventInfo GetEventInfo(DateTime date, int index) {
		return eventsOnDay[date][index];
	}
}

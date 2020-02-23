using Google.Apis.Calendar.v3;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Event = Google.Apis.Calendar.v3.Data.Event;
using static Google.Apis.Calendar.v3.EventsResource;
using Google.Apis.Services;
using System;
using Google.Apis.Calendar.v3.Data;



public class GenerateCalendarData : MonoBehaviour {
	public static GenerateCalendarData Instance { get; private set; }
	public int maxResults = 100;
	[Header("Calendars")]
	public CalendarURL[] calendars;
	public Dictionary<DateTime, List<EventInfo>> allEvents = new Dictionary<DateTime, List<EventInfo>>();

	// If modifying these scopes, delete your previously saved credentials
	// at ~/.credentials/calendar-dotnet-quickstart.json
	private static string[] Scopes = { CalendarService.Scope.CalendarReadonly };
	private static string ApplicationName = "Insight";

	public static bool populateCalendar = true;
	public static bool populateWeek = true;

	private void Awake() {
		if (!Instance) {
			Instance = this;
			DontDestroyOnLoad(gameObject);
		} else {
			Destroy(gameObject);
		}

		GenerateCalendarCheckboxes();
	}

	public void ClearAllEvents() {
		allEvents.Clear();
	}

	public void GenerateCalendarCheckboxes() {
		for (int i = 0; i < calendars.Length; i++) {
			calendars[i].useCalendar = PlayerPrefsX.GetBool(calendars[i].title);
		}
	}

	public bool HasEventOnDay(DateTime key) {
		if (allEvents == null) return false;

		return allEvents.ContainsKey(key);
	}

	public Task<Dictionary<DateTime, List<EventInfo>>> GetCalendarEventsAsync(CalendarURL c, DateTime timeMin, int maxResults) {
		return Task.Run(() => {
			Dictionary<DateTime, List<EventInfo>> calendarEvents = new Dictionary<DateTime, List<EventInfo>>();

			CalendarService service = new CalendarService(new BaseClientService.Initializer() {
				ApiKey = "AIzaSyCAkCz0rjx4gw1omV7CuWR8wq_0dhKg2ww",
				ApplicationName = ApplicationName,
			});

			if (c.useCalendar) {
				ListRequest request = service.Events.List(c.url);

				// Create a request for events starting from previous month onward, until 200 events have been added
				request.TimeMin = timeMin;
				request.ShowDeleted = false;
				request.SingleEvents = true;
				request.MaxResults = maxResults;
				request.OrderBy = ListRequest.OrderByEnum.StartTime;

				Events events = request.Execute();
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
							if (!calendarEvents.ContainsKey(start.AddDays(i))) {
								// Create a new list of EventInfos
								calendarEvents.Add(start.AddDays(i), new List<EventInfo>());
							}

							// Add the event info to the list of events on that day
							calendarEvents[start.AddDays(i)].Add(new EventInfo(events.Summary, e, c.color));
						}
					}
				} else {
					Debug.Log("No upcoming events found.");
				}
			}

			foreach (DateTime key in calendarEvents.Keys) {
				if (!allEvents.ContainsKey(key)) {
					allEvents.Add(key, calendarEvents[key]);
				} else {
					allEvents[key].AddRange(calendarEvents[key]);
				}
			}

			return calendarEvents;
		});

		
	}
}

[Serializable]
public struct CalendarURL {
	public string url;
	public string title;
	public bool useCalendar;
	public Color32 color;

	public CalendarURL(string url, string title, bool useCalendar, Color32 color) {
		this.url = url;
		this.title = title;
		this.useCalendar = useCalendar;
		this.color = color;
	}

	public string GetURL() {
		return url;
	}

	public bool UseCalendar() {
		return useCalendar;
	}

	public void UseCalendar(bool use) {
		useCalendar = use;
	}
}

[Serializable]
public struct EventInfo {
	public string calendarName;
	public Event e;
	public Color32 color;

	public EventInfo(string calendarName, Event e, Color32 color) {
		this.calendarName = calendarName;
		this.e = e;
		this.color = color;
	}
}
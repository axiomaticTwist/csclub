using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using System;
using System.Globalization;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Event = Google.Apis.Calendar.v3.Data.Event;
using System.Threading;
using System.Collections;

// Gets the first date in the week, given a particular day
public static class FirstDayOfWeekUtility {
	public static DateTime GetFirstDayOfWeek(DateTime dayInWeek) {
		return GetFirstDateOfWeek(dayInWeek);
	}

	public static DateTime GetFirstDateOfWeek(DateTime dayInWeek) {
		DayOfWeek firstDay = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
		DateTime firstDayInWeek = dayInWeek.Date;
		while (firstDayInWeek.DayOfWeek != firstDay)
			firstDayInWeek = firstDayInWeek.AddDays(-1);

		return firstDayInWeek;
	}
}

[Serializable]
public struct CalendarURL {
	public string url;
	public bool useCalendar;

	public CalendarURL(string url, bool useCalendar) {
		this.url = url;
		this.useCalendar = useCalendar;
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
public struct DayObj {
	private Transform dayTransform;
	private DateTime day;

	public DayObj(DateTime day, Transform dayTransform) {
		this.day = day;
		this.dayTransform = dayTransform;
	}

	public DateTime GetDay() {
		return day;
	}

	public Transform GetDayTransform() {
		return dayTransform;
	}
}

[Serializable]
public struct Month {
	public Transform[] weeks;
}

[Serializable]
public struct EventInfo {
	public string calendarName;
	public Event e;

	public EventInfo(string calendarName, Event e) {
		this.calendarName = calendarName;
		this.e = e;
	}
}

public class Calendar : MonoBehaviour {

	#region Backend
	[Header("Calendars")]
	public CalendarURL[] calendars;

	private Dictionary<DateTime, List<EventInfo>> eventsOnDay = new Dictionary<DateTime, List<EventInfo>>();

	// If modifying these scopes, delete your previously saved credentials
	// at ~/.credentials/calendar-dotnet-quickstart.json
	static string[] Scopes = { CalendarService.Scope.CalendarReadonly };
	static string ApplicationName = "CSClub Project";

	// Create Google Calendar API service.
	CalendarService service = new CalendarService(new BaseClientService.Initializer() {
		ApiKey = "AIzaSyCAkCz0rjx4gw1omV7CuWR8wq_0dhKg2ww",
		ApplicationName = ApplicationName,
	});
	#endregion

	#region Visual
	DateTime desiredDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
	
	[Header("Day Prefab")]
	public Transform dayPrefab;

	[Header("Months")]
	public Month[] months;

	[Header("Month Text")]
	public Text[] monthText;

	[Space(10)]
	public Font boldFont;

	public Transform infoPrefab;

	public Transform infoContentPanel;

	public EventPanel eventPanel;

	public GameObject calendarItemPrefab;
	public Transform calendarsPanel;

	public GameObject loadingPanel;

	private List<DayObj> days = new List<DayObj>();

	private bool generated = false;
	
	#endregion

	private void Start() {
		PopulateCalendar();
		StartCoroutine(LoadCalendar());
	}

	public void DisplayCalendar() {
		DateTime month0 = desiredDate.AddMonths(-1);
		DateTime month1 = desiredDate.AddMonths(1);


		// Set the title of the calendar to the current month and year
		monthText[0].text = DateTimeFormatInfo.CurrentInfo.GetMonthName(month0.Month) + " " + month0.Year;
		monthText[1].text = DateTimeFormatInfo.CurrentInfo.GetMonthName(desiredDate.Month) + " " + desiredDate.Year;
		monthText[2].text = DateTimeFormatInfo.CurrentInfo.GetMonthName(month1.Month) + " " + month1.Year;

		DateTime prevMonth;
		DateTime curMonth;
		DateTime nextMonth;

		DateTime curDay;
		
		// TODO: Bugfix: the utility gets the first day of the week from the previous year when updating to a new year
		prevMonth = FirstDayOfWeekUtility.GetFirstDateOfWeek(desiredDate.AddMonths(-1));
		curMonth = FirstDayOfWeekUtility.GetFirstDateOfWeek(desiredDate);
		nextMonth = FirstDayOfWeekUtility.GetFirstDateOfWeek(desiredDate.AddMonths(1));

		DateTime[] firstDayOfMonth = {prevMonth, curMonth, nextMonth};

		// Loop through each month
		for (int month = 0; month < 3; month++) {
			// Set the current day to the first day of each month
			curDay = firstDayOfMonth[month];

			DateTime beginDate = desiredDate.AddMonths(month - 1);
			DateTime endDate = desiredDate.AddMonths(month);

			// Loop through each week
			for (int week = 0; week < 6; week++) {
				// Loop through each day
				for (int day = 0; day < 7; day++) {
					Transform dayItem = Instantiate(dayPrefab, months[month].weeks[week]);
					dayItem.GetComponent<DateObject>().SetDate(curDay);
					dayItem.GetComponent<DateObject>().calendar = this;

					// If the day is not today
					if (curDay != DateTime.Today) {
						// Set the tile to white
						dayItem.GetComponent<Image>().color = Color.white;

						if (beginDate <= curDay && curDay < endDate)
							dayItem.GetComponentInChildren<Text>().color = Color.black;
						else {
							// Otherwise the day is not in the current month, so set its color to gray
							dayItem.GetComponentInChildren<Text>().color = new Color32(164, 164, 164, 100);
						}
					}

					if (eventsOnDay.ContainsKey(curDay.Date)) {
						dayItem.GetComponentInChildren<Text>().font = boldFont;
					}

					// Set the text of the day tile
					dayItem.GetComponentInChildren<Text>().text = curDay.Day.ToString();

					days.Add(new DayObj(curDay, dayItem));

					// Increment the current day
					curDay = curDay.AddDays(1);
				}
			}

			beginDate = beginDate.AddMonths(1);
			endDate = endDate.AddMonths(1);
		}
	}

	public void AddMonths(int months) {
		desiredDate = desiredDate.AddMonths(months);

		RefreshCalendar(false);

		DisplayCalendar();
	}

	public void UpdateCalendarURL(bool active, int index) {
		calendars[index].useCalendar = active;
	}

	public void PopulateCalendar() {
		// Loop through every calendar URL given
		foreach (CalendarURL c in calendars) {
			// Define parameters of request.
			EventsResource.ListRequest request = service.Events.List(c.url);

			// If the checkbox is ticked
			if (c.useCalendar) {
				request.TimeMin = FirstDayOfWeekUtility.GetFirstDateOfWeek(DateTime.Today.AddMonths(-1));
				request.ShowDeleted = false;
				request.SingleEvents = true;
				request.MaxResults = 500;
				request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;
			} else {
				request.MaxResults = 1;
			}
			
			// Execute the request
			Events events = request.Execute();

			if (!generated) {
				// Create a checkbox for every calendar
				GameObject item = Instantiate(calendarItemPrefab, calendarsPanel);
				// Set the text of the checkbox to the name of the calendar
				item.GetComponentInChildren<Text>().text = events.Summary;

				// Add an event listener when the checkbox is ticked
				// Update the calendar to either activate or deactivate the calendar at the given index
				item.GetComponentInChildren<Toggle>().onValueChanged.AddListener(delegate { UpdateCalendarURL(item.GetComponentInChildren<Toggle>().isOn, item.transform.GetSiblingIndex()); });

			}

			if (c.useCalendar && events.Items != null && events.Items.Count > 0) {
				foreach(Event e in events.Items) {

					string start = e.Start.DateTime.ToString();
					string end = e.End.DateTime.ToString();




					// If the date doesn't is in a bad form, fix it
					if (String.IsNullOrEmpty(start)) {
						start = e.Start.Date;
					}

					if (String.IsNullOrEmpty(end)) {
						end = e.End.Date;
					}

					DateTime adjustedStart = DateTime.Parse(start);
					DateTime adjustedEnd = DateTime.Parse(end);

					//Debug.Log("Event: " + e.Summary + " Start: " + adjustedStart + " End: " + adjustedEnd + " Total Days: " + Mathf.Abs((adjustedStart - adjustedEnd).Days));
					//Debug.Log();


					//Debug.Log("Title: " + e.Summary + " | Start: " + adjustedStart + " | End: " + end);



					/*
					if (!eventsOnDay.ContainsKey(adjustedEnd)) {
						eventsOnDay.Add(adjustedEnd, new List<EventInfo>());

					}*/
					int difference = Mathf.Abs((adjustedStart - adjustedEnd).Days);

					// If difference between event start and end date is greater than 1, do stuff
					if (difference > 1) {
						for (int i = 0; i < difference; i++) {
							// If the day doesn't already contain events
							if (!eventsOnDay.ContainsKey(adjustedStart.AddDays(i))) {
								// Create a new list of EventInfos
								eventsOnDay.Add(adjustedStart.AddDays(i), new List<EventInfo>());
							}

							eventsOnDay[adjustedStart.AddDays(i)].Add(new EventInfo(events.Summary, e));
						}
					} else {
						// If the day doesn't already contain events
						if (!eventsOnDay.ContainsKey(adjustedStart)) {
							// Create a new list of EventInfos
							eventsOnDay.Add(adjustedStart, new List<EventInfo>());
						}

						// Insert into the event list given a date
						eventsOnDay[adjustedStart].Add(new EventInfo(events.Summary, e));
					}

					
					//eventsOnDay[adjustedEnd].Add(new EventInfo(events.Summary, e));
				}

			} else {
				Debug.Log("No upcoming events found.");
			}

			
		}

		generated = true;
	}

	public void JumpToday() {
		desiredDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

		RefreshCalendar(false);
		
		DisplayCalendar();
	}

	// Clears the dictionary and then repopulates it
	public void RefreshCalendar(bool clearAll) {
		if (clearAll)
			// Clear the calendar dictionary
			eventsOnDay.Clear();

		foreach (DayObj d in days) {
			Destroy(d.GetDayTransform().gameObject);
		}

		days.Clear();
	}

	internal void DisplayEvents(DateTime date) {
		RefreshEventList();

		if (eventsOnDay.ContainsKey(date)) {
			// Set the starting hue to 20 degrees
			float h = 20;

			foreach (EventInfo e in eventsOnDay[date]) {
				// Create a new information item
				Transform item = Instantiate(infoPrefab, infoContentPanel);
				// Set its title to the title of the event
				item.GetComponentInChildren<Text>().text = e.e.Summary.ToString();
				// Add an event listener when the information item is clicked, passing in its current index
				item.GetComponent<Button>().onClick.AddListener(() => DisplayEventDetails(date, item.transform.GetSiblingIndex()));

				// TODO: Fix the hue after the 8th element
				Debug.Log(((date.Month - 1) * 30f + h) / 360f);
				// Set its color based off of the current month
				item.GetComponent<Image>().color = Color.HSVToRGB(((date.Month - 1) * 30f + h) / 360f, 0.6f, 0.78f);
				h += 15;
			}
		}
	}

	internal void DisplayEventDetails(DateTime date, int index) {
		// Play a cool animation
		eventPanel.GetComponent<Animator>().Play("slide in");

		Event e = eventsOnDay[date][index].e;

		eventPanel.eventTitle.text = e.Summary.ToString();

		eventPanel.eventCalendar.text = eventsOnDay[date][index].calendarName;

		// Get the start time
		string when = e.Start.DateTime.ToString();
		if (String.IsNullOrEmpty(when)) {
			when = e.Start.Date;
		}

		// Set the location to the event's location, default none
		try {
			eventPanel.eventLoc.text = e.Location.ToString();
		} catch (NullReferenceException exception) {
			eventPanel.eventLoc.text = "No location specified";
		}

		// Set the description to the event's description, default is the time of the event
		try {
			// If the date of the event does not equal 12 AM, then add the time afterwards
			if (!DateTime.Parse(when).ToShortTimeString().Equals(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0).ToShortTimeString())) {
				eventPanel.eventDesc.text = e.Description.ToString() + " " + DateTime.Parse(when).ToShortTimeString();
			} else {
				eventPanel.eventDesc.text = e.Description.ToString();
			}

		} catch (Exception exception) {
			eventPanel.eventDesc.text = DateTime.Parse(when).ToShortTimeString();
		}


		DateTime day = DateTime.Parse(date.ToString());

		// Set the top date to the event date
		eventPanel.eventDay.text = day.Day.ToString();
		eventPanel.eventMonth.text = DateTimeFormatInfo.CurrentInfo.GetMonthName(day.Month).ToUpper();
		eventPanel.eventYear.text = day.Year.ToString();

		eventPanel.eventDateBG.color = Color.HSVToRGB((day.Month - 1) * 30f / 360f, 0.6f, 0.78f);
	}

	internal IEnumerator LoadCalendar() {
		bool done = false;

		RefreshCalendar(true);
		new Thread(() => {
			PopulateCalendar();
			done = true;
		}).Start();
		
		while (!done)
			yield return null;

		loadingPanel.SetActive(false);
		DisplayCalendar();
	}

	// Deletes every child
	public void RefreshEventList() {
		foreach (Transform go in infoContentPanel) {
			Destroy(go.gameObject);
		}
	}
}

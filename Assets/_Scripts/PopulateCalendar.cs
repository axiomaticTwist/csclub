using System.Collections;
using System.Collections.Generic;
using System;
using System.Globalization;
using UnityEngine;
using Event = Google.Apis.Calendar.v3.Data.Event;
using UnityEngine.UI;

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

public class PopulateCalendar : MonoBehaviour {
	public GameObject dayPrefab;
	public Transform[] weeks;
	public Font boldFont;
	public Text monthText;
	public GoogleCalendar calendar;
	public Image title;

	DateTime firstDayOfWeek;
	DateTime dayInWeek;

	[SerializeField]
	DateTime desiredDay = DateTime.Today;


	// Store date of event and string summary
	internal Dictionary<DateTime, List<Event>> events = new Dictionary<DateTime, List<Event>>();
	

	// Start is called before the first frame update
	private void Start() {
		RefreshCalendar();
	}

	// This is the behind-the-scenes stuff for the calendar, storing the events
	public void SetEvents() {
		// Loop through however many calendars we have
		for (int i = 0; i < calendar.calendars.Length; i++) {
			// Only select the calendars that we actually want to see
			if (calendar.calendars[i].check) {
				// Loop through every event in the event list at a given index
				foreach (Event e in calendar.GetEvents(i)) {
					// Get the start date
					string when = e.Start.DateTime.ToString();
					if (String.IsNullOrEmpty(when)) {
						when = e.Start.Date;
					}

					// If the start date is not already in this dictionary
					if (!events.ContainsKey(DateTime.Parse(when))) {
						// Start a new list of events at a given date
						events.Add(DateTime.Parse(when), new List<Event>());
					}

					// Insert into the event list given a date
					events[DateTime.Parse(when)].Add(e);
				}
			}

		}
	}
	
	// This is the visual stuff for the calendar, actually displaying the events
	public void RefreshCalendar() {
		// Loop through every single day and delete them
		foreach (Transform week in weeks) {
			foreach (Transform child in week) {
				Destroy(child.gameObject);
			}
		}

		// Clear every single date, just to be safe
		events.Clear();

		// Populate the events list
		SetEvents();

		// Set the title of the calendar to the current month and year
		monthText.text = DateTimeFormatInfo.CurrentInfo.GetMonthName(desiredDay.Month) + " " + desiredDay.Year;
		// Make it pretty (evenly divides the color wheel into each month of the year
		title.color = Color.HSVToRGB((desiredDay.Month - 1) * 30f / 360f, 0.6f, 0.78f);

		// Set the first day of the month
		firstDayOfWeek = FirstDayOfWeekUtility.GetFirstDateOfWeek(new DateTime(desiredDay.Year, desiredDay.Month, 1));
		// Gets the first day that will be on the calendar
		dayInWeek = firstDayOfWeek;

		// Loop through each week
		for (int j = 0; j < weeks.Length; j++) {
			// And loop through each day in each week
			for (int i = 0; i < 7; i++) {			
				// Create a day visual block and set its information to the current day we're on
				GameObject dayItem = Instantiate(dayPrefab, weeks[j]);
				dayItem.GetComponent<DateObject>().SetDate(dayInWeek);

				// If the day is not today
				if (DateTime.Today != dayInWeek) {
					// If the day is in the current month
					if (dayInWeek.Month == desiredDay.Month)
						// Set the text color to black
						dayItem.GetComponentInChildren<Text>().color = Color.black;
					else
						// Otherwise the day is not in the current month, so set its color to gray
						dayItem.GetComponentInChildren<Text>().color = new Color32(164,164,164,100);

					// If the day is today, set its color to white
					dayItem.GetComponent<Image>().color = Color.white;

					
				}

				// If the dictionary has an event on that day
				if (events.ContainsKey(dayInWeek)) {
					// Make it bold
					dayItem.GetComponentInChildren<Text>().font = boldFont;

				}

				// Set the day block's text to the current day
				dayItem.GetComponentInChildren<Text>().text = dayInWeek.Day.ToString();
				// Increment the current day
				dayInWeek = dayInWeek.AddDays(1);
			}

			// Get the first day of the week of the next week
			firstDayOfWeek = firstDayOfWeek.AddDays(7);
		}
		
	}

	// Move the calendar view forward
	public void Forward() {
		desiredDay = desiredDay.AddMonths(1);
		RefreshCalendar();
	}

	// Move the calendar view backward
	public void Backward() {
		desiredDay = desiredDay.AddMonths(-1);
		RefreshCalendar();
	}
}

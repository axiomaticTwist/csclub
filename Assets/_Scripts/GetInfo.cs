using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using Event = Google.Apis.Calendar.v3.Data.Event;
using System.Globalization;

public class GetInfo : MonoBehaviour {
	public GameObject infoPrefab;
	public PopulateCalendar calendar;
	public EventPanel eventPanel;
	DateTime date;
	
	// Called when a day is clicked
	public void GetDayInfo(DateTime _date) {
		Refresh(); // Delete every transform
		date = _date;

		// If the date that's been clicked on has events
		if (calendar.events.ContainsKey(_date)) {
			// Set the starting hue to 20 degrees
			int h = 20;

			// Loop through the events
			foreach (Event e in calendar.events[_date]) {
				// Create a new information item
				GameObject item = Instantiate(infoPrefab, transform);
				// Set its title to the title of the event
				item.GetComponentInChildren<Text>().text = e.Summary.ToString();
				// Add an event listener when the information item is clicked, passing in its current index
				item.GetComponent<Button>().onClick.AddListener(() => DisplayEventDetails(item.transform.GetSiblingIndex()));
				// Set its color based off of the current month
				item.GetComponent<Image>().color = Color.HSVToRGB(((_date.Month - 1) * 30f + h) / 360f, 0.6f, 0.78f);
				h += 15;
			}
		}
	}
	
	// Show the event details at a given index
	public void DisplayEventDetails(int index) {
		// Play a cool animation
		eventPanel.GetComponent<Animator>().Play("slide in");
		// Get the given event
		Event _event = calendar.events[date][index];

		// Set its title to the event's title
		eventPanel.eventTitle.text = _event.Summary.ToString();

		// Set the location to the event's location, default none
		try {
			eventPanel.eventLoc.text = _event.Location.ToString();
		} catch (NullReferenceException e) {
			eventPanel.eventLoc.text = "No location specified";
		}

		// Set the description to the event's description, default none
		try {
			eventPanel.eventDesc.text = _event.Description.ToString();
		} catch (Exception e) {
			eventPanel.eventDesc.text = "No description given";
		}

		// Get the start time
		string when = _event.Start.DateTime.ToString();
		if (String.IsNullOrEmpty(when)) {
			when = _event.Start.Date;
		}
		DateTime day = DateTime.Parse(when);

		// Set the top date to the event date
		eventPanel.eventDay.text = day.Day.ToString();
		eventPanel.eventMonth.text = DateTimeFormatInfo.CurrentInfo.GetMonthName(day.Month).ToUpper();
		eventPanel.eventYear.text = day.Year.ToString();

		eventPanel.eventDateBG.color = Color.HSVToRGB((day.Month - 1) * 30f / 360f, 0.6f, 0.78f);
	}

	// Deletes every child
	public void Refresh() {
		foreach (Transform go in transform) {
			Destroy(go.gameObject);
		}
	}
}

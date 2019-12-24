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
			float h = 20;

			// Loop through the events
			foreach (EventAndCalendar e in calendar.events[_date]) {
				// Create a new information item
				GameObject item = Instantiate(infoPrefab, transform);
				// Set its title to the title of the event
				item.GetComponentInChildren<Text>().text = e.e.Summary.ToString();
				// Add an event listener when the information item is clicked, passing in its current index
				item.GetComponent<Button>().onClick.AddListener(() => DisplayEventDetails(item.transform.GetSiblingIndex()));

				// TODO: Fix the hue after the 8th element
				// Debug.Log(((_date.Month - 1) * 30f + h) / 360f);
				// Set its color based off of the current month
				item.GetComponent<Image>().color = Color.HSVToRGB(((_date.Month - 1) * 30f + h) / 360f, 0.6f, 0.78f);
				h += 15;
			}
		}
	}
	
	// Show the event details at a given index
	public void DisplayEventDetails(int index) {
		Debug.Log("a" + date);

		// Play a cool animation
		eventPanel.GetComponent<Animator>().Play("slide in");
		// Get the given event
		Event _event = calendar.events[date][index].e;

		// Set its title to the event's title
		eventPanel.eventTitle.text = _event.Summary.ToString();

		// Get the calendar name given an event index
		eventPanel.eventCalendar.text = calendar.events[date][index].calendarName;

		// Get the start time
		string when = _event.Start.DateTime.ToString();
		if (String.IsNullOrEmpty(when)) {
			when = _event.Start.Date;
		}

		// Set the location to the event's location, default none
		try {
			eventPanel.eventLoc.text = _event.Location.ToString();
		} catch (NullReferenceException e) {
			eventPanel.eventLoc.text = "No location specified";
		}

		// Set the description to the event's description, default is the time of the event
		try {
			// If the date of the event does not equal 12 AM, then add the time afterwards
			if (!DateTime.Parse(when).ToShortTimeString().Equals(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0).ToShortTimeString())) {
				eventPanel.eventDesc.text = _event.Description.ToString() + " " + DateTime.Parse(when).ToShortTimeString();
			} else {
				eventPanel.eventDesc.text = _event.Description.ToString();
			}

		} catch (Exception e) {
			eventPanel.eventDesc.text = DateTime.Parse(when).ToShortTimeString();
		}

		
		DateTime day = DateTime.Parse(date.ToString());

		Debug.Log(date);
		// Set the top date to the event date
		eventPanel.eventDay.text = date.Day.ToString();
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

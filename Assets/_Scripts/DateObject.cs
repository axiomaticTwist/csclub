using System;
using UnityEngine;
using UnityEngine.UI;
using Event = Google.Apis.Calendar.v3.Data.Event;

public class DateObject : MonoBehaviour {
	public DateTime date;
	public Transform infoContentPanel;
	public Transform infoPrefab;
	public EventPanel eventPanel;

	// Show the information on a given date when the day has been clicked on
	public void ShowEventsOnDay() {
		foreach (Transform child in infoContentPanel) {
			Destroy(child.gameObject);
		}

		if (GenerateCalendarData.Instance.HasEventOnDay(date)) {
			foreach (EventInfo e in GenerateCalendarData.Instance.allEvents[date]) {
				// Create a new information item
				Transform item = Instantiate(infoPrefab, infoContentPanel);
				// Set its title to the title of the event
				item.GetComponentInChildren<Text>().text = e.e.Summary.ToString();
				// Add an event listener when the information item is clicked, passing in its current index
				item.GetComponent<Button>().onClick.AddListener(() => DisplayEventDetails(item.transform.GetSiblingIndex()));

				// Set its color based off of the current month
				item.Find("Selection").GetComponent<Image>().color = e.color;
			}
		}
		
	}

	public void DisplayEventDetails(int index) {
		eventPanel.GetComponent<Animator>().Play("slide in");
		EventInfo eventInfo = GenerateCalendarData.Instance.allEvents[date][index];
		Event @event = eventInfo.e;


		eventPanel.eventYear.text = date.ToString("yyyy");
		eventPanel.eventMonth.text = date.ToString("MMMM").ToUpper();
		eventPanel.eventDay.text = date.ToString("%d");

		// Get the start time
		string start = @event.Start.DateTime.ToString();
		string end = @event.End.DateTime.ToString();
		if (string.IsNullOrEmpty(start)) {
			start = @event.Start.Date;
		}
		if (string.IsNullOrEmpty(end)) {
			end = @event.End.Date;
		}

		string startTime = DateTime.Parse(start).ToShortTimeString();
		string endTime = DateTime.Parse(end).ToShortTimeString();

		// Set the location to the event's location, default none
		try {
			eventPanel.eventLoc.text = @event.Location.ToString();

		} catch (NullReferenceException exception) {
			eventPanel.eventLoc.text = "No location specified";
		}

		try {
			var midnight = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0).ToShortTimeString();

			// If the date of the event does not equal 12 AM, then add the time 
			if (!startTime.Equals(midnight)) {
				eventPanel.eventDesc.text = @event.Description.ToString() + " " + startTime + " to " + endTime;
			} else {
				eventPanel.eventDesc.text = @event.Description.ToString();
			}

		} catch (Exception exception) {
			eventPanel.eventDesc.text = startTime;
		}
		
		eventPanel.eventTitle.text = eventInfo.e.Summary;
		eventPanel.eventCalendar.text = eventInfo.calendarName;
		eventPanel.eventDateBG.color = eventInfo.color;
		//eventPanel.eventDateBG.color = Color.HSVToRGB((day.Month - 1) * 30f / 360f, 0.6f, 0.78f);
	}
}

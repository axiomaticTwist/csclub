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
	

	public void GetDayInfo(DateTime _date) {
		//Debug.Log(_date);
		Refresh();
		date = _date;

		if (calendar.events.ContainsKey(_date)) {
			// Populate info panel
			int h = 20;

			for (int i = 0; i < calendar.events[_date].Count; i++) {
				GameObject item = Instantiate(infoPrefab, transform);
				item.GetComponentInChildren<Text>().text = calendar.events[_date][i].Summary.ToString();
				item.GetComponent<Button>().onClick.AddListener(() => DisplayEventDetails(item.transform.GetSiblingIndex()));
				//item.GetComponent<Image>().color = Color.HSVToRGB((float) h / 360, 0.6f, 0.78f);
				item.GetComponent<Image>().color = Color.HSVToRGB(((_date.Month - 1) * 30f + h) / 360f, 0.6f, 0.78f);
				h += 15;
			}
		}
	}

	// TODO: Retrieve specific information on a given day
	public void DisplayEventDetails(int index) {
		eventPanel.GetComponent<Animator>().Play("slide in");
		Event _event = calendar.events[date][index];

		eventPanel.eventTitle.text = _event.Summary.ToString();

		try {
			eventPanel.eventLoc.text = _event.Location.ToString();

		} catch (NullReferenceException e) {
			eventPanel.eventLoc.text = "No location specified";

		}

		try {
			eventPanel.eventDesc.text = _event.Description.ToString();
		} catch (Exception e) {
			eventPanel.eventDesc.text = "No description given";
		}

		string when = _event.Start.DateTime.ToString();
		if (String.IsNullOrEmpty(when)) {
			when = _event.Start.Date;
		}
		DateTime day = DateTime.Parse(when);

		eventPanel.eventDay.text = day.Day.ToString();
		eventPanel.eventMonth.text = DateTimeFormatInfo.CurrentInfo.GetMonthName(day.Month).ToUpper();
		eventPanel.eventYear.text = day.Year.ToString();

		eventPanel.eventDateBG.color = Color.HSVToRGB((day.Month - 1) * 30f / 360f, 0.6f, 0.78f);
	}

	public void Refresh() {
		foreach (Transform go in transform) {
			Destroy(go.gameObject);
		}
	}
}

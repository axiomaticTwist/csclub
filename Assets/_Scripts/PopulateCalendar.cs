using System.Collections;
using System.Collections.Generic;
using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

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

	DateTime firstDayOfWeek;
	DateTime dayInWeek;

	[SerializeField]
	DateTime desiredDay = DateTime.Today;


	// Store date of event and string summary
	internal Dictionary<DateTime, ArrayList> events = new Dictionary<DateTime, ArrayList>();
	

	// Start is called before the first frame update
	private void Start() {
		

		foreach(Google.Apis.Calendar.v3.Data.Event e in calendar.GetEvents()) {
			string when = e.Start.DateTime.ToString();
			if (String.IsNullOrEmpty(when)) {
				when = e.Start.Date;
			}

			if (!events.ContainsKey(DateTime.Parse(when))) {
				events.Add(DateTime.Parse(when), new ArrayList());
			}

			events[DateTime.Parse(when)].Add(e.Summary);
		}

		foreach (string s in events[new DateTime(2019, 12, 7)])
			Debug.Log(s);

		RefreshCalendar();
	}
	

	void RefreshCalendar() {
		foreach (Transform week in weeks) {
			foreach (Transform child in week) {
				Destroy(child.gameObject);
			}
		}

		monthText.text = DateTimeFormatInfo.CurrentInfo.GetMonthName(desiredDay.Month) + " " + desiredDay.Year;

		// Set the first day of the month
		firstDayOfWeek = FirstDayOfWeekUtility.GetFirstDateOfWeek(new DateTime(desiredDay.Year, desiredDay.Month, 1));
		dayInWeek = firstDayOfWeek;

		Debug.Log(firstDayOfWeek);

		for (int j = 0; j < weeks.Length; j++) {
			for (int i = 0; i < 7; i++) {				
				//Debug.Log(dayInWeek);

				GameObject dayItem = Instantiate(dayPrefab, weeks[j]);
				dayItem.GetComponent<DateObject>().SetDate(dayInWeek);

				if (DateTime.Today != dayInWeek) {
					// Set each day's text
					if (dayInWeek.Month == desiredDay.Month)
						dayItem.GetComponentInChildren<Text>().color = Color.black;
					else
						dayItem.GetComponentInChildren<Text>().color = new Color32(164,164,164,100);


					dayItem.GetComponent<Image>().color = Color.white;

					
				}

				// if the dictionary has an event on that day
				if (events.ContainsKey(dayInWeek)) {
					dayItem.GetComponentInChildren<Text>().font = boldFont;

				}

				dayItem.GetComponentInChildren<Text>().text = dayInWeek.Day.ToString();
				dayInWeek = dayInWeek.AddDays(1);
			}
			firstDayOfWeek = firstDayOfWeek.AddDays(7);
		}
		
	}

	public void Forward() {
		desiredDay = desiredDay.AddMonths(1);
		RefreshCalendar();
	}

	public void Backward() {
		desiredDay = desiredDay.AddMonths(-1);
		RefreshCalendar();
	}
}

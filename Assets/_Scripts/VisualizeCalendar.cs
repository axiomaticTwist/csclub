using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
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

public class VisualizeCalendar : MonoBehaviour {

	#region Variables

	[Header("Day Prefab")]
	public Transform dayPrefab;

	[Header("Months")]
	public Month[] months;

	[Header("Month Text")]
	public Text[] monthText;

	[Space(10)]
	public Font boldFont;

	public Transform infoContentPanel;
	public Transform infoPrefab;

	public EventPanel eventPanel;

	public Transform calendarsPanel;
	public GameObject calendarItemPrefab;

	public GameObject loadingPanel;
	
	private Dictionary<DateTime, List<Transform>> days = new Dictionary<DateTime, List<Transform>>();
	private DateTime desiredDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
	private string[] calendarNames;
	#endregion


	private void Start() {
		ClearCalendar();
		GenerateCalendar();

		if (GenerateCalendarData.populateCalendar == true) {
			GenerateCalendarData.Instance.PopulateCalendar();
			GenerateCalendarData.populateCalendar = false;
		}

		CheckForEvents(GenerateCalendarData.Instance.allEvents);

		GenerateCalendarCheckboxVisuals();
	}

	private void Update() {
		if (GenerateCalendarData.Instance.eventListQueue.Count == 0) return;

		lock (GenerateCalendarData.Instance.eventListQueue) {
			foreach (var element in GenerateCalendarData.Instance.eventListQueue) {
				CheckForEvents(element);
			}

			GenerateCalendarData.Instance.eventListQueue.Clear();
		}
	}

	public void AddMonths(int monthsToAdd) {
		desiredDate = desiredDate.AddMonths(monthsToAdd);

		RefreshCalendar();
	}

	public void RefreshCalendar() {
		foreach (Transform child in infoContentPanel) {
			Destroy(child.gameObject);
		}

		ClearCalendar();
		GenerateCalendar();

		CheckForEvents(GenerateCalendarData.Instance.allEvents);
	}

	private void CheckForEvents(Dictionary<DateTime, List<EventInfo>> eventsOnDays) {
		foreach (DateTime key in eventsOnDays.Keys) {
			if (days.ContainsKey(key)) {

				foreach (Transform gameObject in days[key])
					gameObject.GetComponentInChildren<Text>().font = boldFont;
			}
		}
	}

	private void ClearCalendar() {
		days.Clear();

		foreach (Month month in months) {
			foreach (Transform week in month.weeks) {
				foreach (Transform day in week) {
					Destroy(day.gameObject);
				}
			}
		}
	}

	private void GenerateCalendar() {
		DateTime prevMonth = desiredDate.AddMonths(-1);
		DateTime nextMonth = desiredDate.AddMonths(1);

		// Set the title of the calendar to the current month and year
		monthText[0].text = DateTimeFormatInfo.CurrentInfo.GetMonthName(prevMonth.Month) + " " + prevMonth.Year;
		monthText[1].text = DateTimeFormatInfo.CurrentInfo.GetMonthName(desiredDate.Month) + " " + desiredDate.Year;
		monthText[2].text = DateTimeFormatInfo.CurrentInfo.GetMonthName(nextMonth.Month) + " " + nextMonth.Year;

		DateTime firstDayOfPrevMonth = FirstDayOfWeekUtility.GetFirstDateOfWeek(desiredDate.AddMonths(-1));
		DateTime firstDayOfCurMonth = FirstDayOfWeekUtility.GetFirstDateOfWeek(desiredDate);
		DateTime firstDayOfNextMonth = FirstDayOfWeekUtility.GetFirstDateOfWeek(desiredDate.AddMonths(1));

		DateTime[] firstDaysOfMonths = { firstDayOfPrevMonth, firstDayOfCurMonth, firstDayOfNextMonth };

		DateTime tempDay;

		try {
			// Loop through each month
			for (int month = 0; month < 3; month++) {
				// Set the current day to the first day of each month
				tempDay = firstDaysOfMonths[month];

				// Get start and end day of each month
				DateTime tempStartOfMonth = desiredDate.AddMonths(month - 1);
				DateTime tempEndOfMonth = desiredDate.AddMonths(month);

				// Look at 6 weeks at a time
				for (int week = 0; week < 6; week++) {
					// Look at each day
					for (int day = 0; day < 7; day++) {
						Transform dayItem = Instantiate(dayPrefab, months[month].weeks[week]); // Create a day within the week Transform within the month Transform

						if (!days.ContainsKey(tempDay)) {
							days.Add(tempDay, new List<Transform>());
							days[tempDay].Add(dayItem);
						} else {
							days[tempDay].Add(dayItem);
						}

						DateObject dateObject = dayItem.GetComponent<DateObject>();

						dateObject.date = tempDay;
						dateObject.infoContentPanel = infoContentPanel;
						dateObject.infoPrefab = infoPrefab;
						dateObject.eventPanel = eventPanel;

						// If the day we instantiated is not today
						if (tempDay != DateTime.Today) {
							// Set the title to white
							dayItem.GetComponent<Image>().color = Color.white;

							// If the day we instantiated is within the current month, set its color to gray
							if (tempStartOfMonth <= tempDay && tempDay < tempEndOfMonth) {
								dayItem.GetComponentInChildren<Text>().color = Color.black;
							} else {
								// Otherwise the day is outside the current month, so gray it out
								dayItem.GetComponentInChildren<Text>().color = new Color32(164, 164, 164, 100);
							}
						}
						// Set the text of the day tile
						dayItem.GetComponentInChildren<Text>().text = tempDay.Day.ToString();

						// Increment the current day
						tempDay = tempDay.AddDays(1);
					}
				}

				tempStartOfMonth = tempStartOfMonth.AddMonths(1);
				tempEndOfMonth = tempEndOfMonth.AddMonths(1);
			}
		} catch (Exception e) {
			Debug.Log(e.ToString());
		}

	}

	public void ListEventsOnDay(DateTime day) {
		if (GenerateCalendarData.Instance.HasEventOnDay(day)) {
			foreach (EventInfo e in GenerateCalendarData.Instance.allEvents[day]) {
				Transform infoItem = Instantiate(infoPrefab, infoContentPanel);
				infoItem.Find("Selection").GetComponent<Image>().color = e.color;
				infoItem.GetComponentInChildren<Text>().text = e.e.Summary.ToString();
			}
		}
	}

	private void GenerateCalendarCheckboxVisuals() {
		foreach (CalendarURL calendar in GenerateCalendarData.Instance.calendars) {
			// Create a checkbox for every calendar
			GameObject item = Instantiate(calendarItemPrefab, calendarsPanel);
			// Set the text of the checkbox to the name of the calendar
			item.GetComponentInChildren<Text>().text = calendar.title;

			ToggleCalendar toggleCalendar = item.GetComponent<ToggleCalendar>();

			toggleCalendar.use = calendar.useCalendar;

			if (toggleCalendar.use) {
				toggleCalendar.selection.SetActive(true);
				toggleCalendar.checkmark.color = Color.black;
				toggleCalendar.title.color = Color.black;
			} else {
				toggleCalendar.selection.SetActive(false);
				toggleCalendar.checkmark.color = new Color32(238, 238, 238, 255);
				toggleCalendar.title.color = new Color32(164, 164, 164, 255);
			}

			toggleCalendar.selection.GetComponent<Image>().color = calendar.color;
		}

	}

	public void JumpToday() {
		desiredDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
		RefreshCalendar();
	}
}

[Serializable]
public struct Month {
	public Transform[] weeks;
}

[Serializable]
public struct Day {
	private Transform dayTransform;
	private DateTime day;

	public Day(DateTime day, Transform dayTransform) {
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

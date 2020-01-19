using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using System;
using System.Globalization;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Event = Google.Apis.Calendar.v3.Data.Event;

public class Scrollbar : MonoBehaviour
{
    private Dictionary<DateTime, List<EventInfo>> eventsOnDay = new Dictionary<DateTime, List<EventInfo>>();

    public Transform infoPrefab;

    public Transform infoContentPanel;

    DateTime date = DateTime.Today;

    public EventPanel eventPanel;

    int h = 0;
    // Start is called before the first frame update
    void Start()
    {
        foreach (EventInfo e in eventsOnDay[date])
        {
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

    internal void DisplayEventDetails(DateTime date, int index)
    {
        // Play a cool animation
        eventPanel.GetComponent<Animator>().Play("slide in");

        Event e = eventsOnDay[date][index].e;

        eventPanel.eventTitle.text = e.Summary.ToString();

        eventPanel.eventCalendar.text = eventsOnDay[date][index].calendarName;

        // Get the start time
        string when = e.Start.DateTime.ToString();
        if (String.IsNullOrEmpty(when))
        {
            when = e.Start.Date;
        }

        // Set the location to the event's location, default none
        try
        {
            eventPanel.eventLoc.text = e.Location.ToString();
        }
        catch (NullReferenceException exception)
        {
            eventPanel.eventLoc.text = "No location specified";
        }

        // Set the description to the event's description, default is the time of the event
        try
        {
            // If the date of the event does not equal 12 AM, then add the time afterwards
            if (!DateTime.Parse(when).ToShortTimeString().Equals(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0).ToShortTimeString()))
            {
                eventPanel.eventDesc.text = e.Description.ToString() + " " + DateTime.Parse(when).ToShortTimeString();
            }
            else
            {
                eventPanel.eventDesc.text = e.Description.ToString();
            }

        }
        catch (Exception exception)
        {
            eventPanel.eventDesc.text = DateTime.Parse(when).ToShortTimeString();
        }


        DateTime day = DateTime.Parse(when);

        // Set the top date to the event date
        eventPanel.eventDay.text = day.Day.ToString();
        eventPanel.eventMonth.text = DateTimeFormatInfo.CurrentInfo.GetMonthName(day.Month).ToUpper();
        eventPanel.eventYear.text = day.Year.ToString();

        eventPanel.eventDateBG.color = Color.HSVToRGB((day.Month - 1) * 30f / 360f, 0.6f, 0.78f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

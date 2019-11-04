using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class GetInfo : MonoBehaviour {
	public GameObject infoPrefab;
	public PopulateCalendar calendar;

	public void GetDayInfo(DateTime _date) {
		Debug.Log(_date);
		Refresh();

		if (calendar.events.ContainsKey(_date)) {
			// Populate info panel
			int h = UnityEngine.Random.Range(0,360);

			foreach (string s in calendar.events[_date]) {
				GameObject item = Instantiate(infoPrefab, transform);
				item.GetComponentInChildren<Text>().text = s;
				item.GetComponent<Image>().color = Color.HSVToRGB((float) h / 360, 0.6f, 0.78f);
				h += 20;
			}
		}
	}

	public void Refresh() {
		foreach (Transform go in transform) {
			Destroy(go.gameObject);
		}
	}
}

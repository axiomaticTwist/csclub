using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

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
			int h = UnityEngine.Random.Range(0,360);

			for (int i = 0; i < calendar.events[_date].Count; i++) {
				GameObject item = Instantiate(infoPrefab, transform);
				item.GetComponentInChildren<Text>().text = calendar.events[_date][i].ToString();
				item.GetComponent<Button>().onClick.AddListener(() => DisplayEventDetails(item.transform.GetSiblingIndex()));
				item.GetComponent<Image>().color = Color.HSVToRGB((float) h / 360, 0.6f, 0.78f);
				h += 20;
			}
		}
	}

	// TODO: Retrieve specific information on a given day
	public void DisplayEventDetails(int index) {
		eventPanel.gameObject.SetActive(true);
		eventPanel.eventTitle.text = calendar.events[date][index].ToString();

		//Debug.Log(index);
	}

	public void Refresh() {
		foreach (Transform go in transform) {
			Destroy(go.gameObject);
		}
	}
}

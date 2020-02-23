using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleCalendar : MonoBehaviour {
	public GameObject selection;
	public Image checkmark;
	public Text title;

	public bool use;

	public void Toggle() {
		use = !use;

		GenerateCalendarData.Instance.calendars[transform.GetSiblingIndex()].useCalendar = use;

		PlayerPrefsX.SetBool(title.text.ToString(), use);

		if (use) {
			selection.SetActive(true);
			checkmark.color = Color.black;
			title.color = Color.black;
		} else {
			selection.SetActive(false);
			checkmark.color = new Color32(238, 238, 238, 255);
			title.color = new Color32(164, 164, 164, 255);
		}
	}
}

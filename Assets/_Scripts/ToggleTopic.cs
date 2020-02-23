using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleTopic : MonoBehaviour {
	public GameObject selection;
	public Image checkmark;
	public Text title;

	// Set by CreateSubscriptions.cs
	public long userId;
	public string topic;

	public bool active;

	public void Toggle() {
		active = !active;

		PlayerPrefsX.SetBool(userId.ToString(), active);

		if (active) {
			selection.SetActive(true);
			checkmark.color = Color.black;
			title.color = Color.black;
			Firebase.Messaging.FirebaseMessaging.SubscribeAsync("/topics/" + topic.Replace(' ', '-'));
		} else {
			selection.SetActive(false);
			checkmark.color = new Color32(238, 238, 238, 255);
			title.color = new Color32(164, 164, 164, 255);
			Firebase.Messaging.FirebaseMessaging.UnsubscribeAsync("/topics/" + topic.Replace(' ', '-'));

		}
	}
}

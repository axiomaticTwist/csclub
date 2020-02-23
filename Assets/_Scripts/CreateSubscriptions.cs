using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CreateSubscriptions : MonoBehaviour {
	Dictionary<long, string> userTopics = new Dictionary<long, string>();
	public Transform contentPanel;
	public GameObject errorPrefab;
	public GameObject topicItemPrefab;

	public void GetTopics() {
		foreach (Transform child in contentPanel) {
			Destroy(child.gameObject);
		}

		if (ClientSocket.connected)
			StartCoroutine(RetrieveTopics());
		else Instantiate(errorPrefab, contentPanel);
	}

	public IEnumerator RetrieveTopics() {
		bool done = false;

		new Thread(() => {
			userTopics = SendAndReceive.RetrieveTopics();
			done = true;
		}).Start();

		while (!done) {
			yield return null;
		}

		if (userTopics != null) {
			foreach (var userTopic in userTopics) {
				GameObject item = Instantiate(topicItemPrefab, contentPanel);

				ToggleTopic toggleTopic = item.GetComponent<ToggleTopic>();
				toggleTopic.userId = userTopic.Key;
				toggleTopic.topic = userTopic.Value;
				toggleTopic.active = PlayerPrefsX.GetBool(userTopic.Key.ToString());

				if (toggleTopic.active) {
					toggleTopic.selection.SetActive(true);
					toggleTopic.checkmark.color = Color.black;
					toggleTopic.title.color = Color.black;
				} else {
					toggleTopic.selection.SetActive(false);
					toggleTopic.checkmark.color = new Color32(238, 238, 238, 255);
					toggleTopic.title.color = new Color32(164, 164, 164, 255);
				}

				toggleTopic.title.text = userTopic.Value;
			}
		} else {
			Instantiate(errorPrefab, contentPanel);
		}
	}
}

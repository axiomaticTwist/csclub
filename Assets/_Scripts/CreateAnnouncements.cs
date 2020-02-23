using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// TODO: Subscribe to announcements
public class CreateAnnouncements : MonoBehaviour {
	List<AnnouncementEntry> entries = new List<AnnouncementEntry>();
	public Transform contentPanel;
	public GameObject newsItemPrefab;
	public GameObject errorPrefab;
	public GameObject announcementPanel;
	public Text announcementsTitleText;

	public void GetAnnouncements() {
		foreach (Transform child in contentPanel) {
			Destroy(child.gameObject);
		}

		if (ClientSocket.connected) 
			StartCoroutine(RetrieveAnnouncements());
		else Instantiate(errorPrefab, contentPanel);
	}

	public IEnumerator RetrieveAnnouncements() {
		bool done = false;

		new Thread(() => {
			entries = SendAndReceive.RetrieveAnnouncements();
			done = true;
		}).Start();

		while (!done) {
			yield return null;
		}

		if (entries != null) {
			announcementsTitleText.text = string.Format("Announcements ({0})", entries.Count);

			foreach (var entry in entries) {
				GameObject item = Instantiate(newsItemPrefab, contentPanel);

				var textComponents = item.GetComponentsInChildren<Text>();

				textComponents[0].text = entry.Title;
				textComponents[1].text = entry.Date;

				if (entry.Date != string.Empty) {
					item.GetComponent<Button>().onClick.AddListener(() => DisplayAnnouncement(entry));
				} else {
					item.GetComponent<Button>().onClick.AddListener(() => OpenLink(entry.Body));
				}
			}
		} else {
			Instantiate(errorPrefab, contentPanel);
		}
	}

	void DisplayAnnouncement(AnnouncementEntry entry) {
		var textComponents = announcementPanel.GetComponentsInChildren<Text>();

		textComponents[0].text = entry.Title;
		textComponents[1].text = entry.Date;

		announcementPanel.GetComponentInChildren<TextMeshProUGUI>().text = entry.Body;
		announcementPanel.GetComponent<BigPanelSlide>().SlideLeft();
	}

	void OpenLink(string uri) {
		Application.OpenURL(uri);
	}
}

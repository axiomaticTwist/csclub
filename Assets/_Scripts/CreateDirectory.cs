using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StaffInfo {
	public string Name { get; }
	public string Title { get; }
	public string Extension { get; }
	public string Email { get; }

	public StaffInfo(string name, string title, string extension, string email) {
		Name = name;
		Title = title;
		Extension = extension;
		Email = email;
	}

	public override string ToString() {
		return string.Format("{0}\n{1}\n{2}\n{3}", Name, Title, Extension, Email);
	}
}

public class CreateDirectory : MonoBehaviour {
	List<StaffInfo> directory = new List<StaffInfo>();
	private List<StaffInfo> searchedDirectory;
	public Transform contentPanel;
	public GameObject staffItem;
	public GameObject staffInfoPanel;
	public GameObject errorPrefab;

	public void Search(string searchString) {
		searchedDirectory = new List<StaffInfo>();

		if (searchString != string.Empty) {
			searchedDirectory.AddRange(directory.Where(staffInfo => staffInfo.Name.Contains(searchString)).ToList());
			searchedDirectory.AddRange(directory.Where(staffInfo => staffInfo.Title.Contains(searchString)).ToList());
		} else searchedDirectory = directory;

		ClearDirectory();
		PopulateDirectory(searchedDirectory);
	}

	private void PopulateDirectory(List<StaffInfo> directory) {
		if (directory != null) {
			foreach (var staffInfo in directory) {
				GameObject item = Instantiate(staffItem, contentPanel);

				var textComponents = item.GetComponentsInChildren<Text>();

				textComponents[0].text = staffInfo.Name;
				textComponents[1].text = staffInfo.Title;

				item.GetComponent<Button>().onClick.AddListener(() => DisplayAnnouncement(staffInfo));
			}
		}
	}

	public void GetDirectory() {
		ClearDirectory();

		if (ClientSocket.connected)
			StartCoroutine(RetrieveDirectory());
		else Instantiate(errorPrefab, contentPanel);
	}

	public void ClearDirectory() {
		for (int i = 1; i < contentPanel.childCount; i++) {
			Destroy(contentPanel.GetChild(i).gameObject);
		}
	}

	public IEnumerator RetrieveDirectory() {
		bool done = false;

		new Thread(() => {
			directory = SendAndReceive.RetrieveDirectory();
			done = true;
		}).Start();

		while (!done) {
			yield return null;
		}

		PopulateDirectory(directory);
	}

	void DisplayAnnouncement(StaffInfo staffInfo) {
		var textComponents = staffInfoPanel.GetComponentsInChildren<Text>();

		textComponents[0].text = staffInfo.Name;
		textComponents[1].text = staffInfo.Title;

		var tmpComponents = staffInfoPanel.GetComponentsInChildren<TextMeshProUGUI>();
		tmpComponents[0].text = string.Format("Extension: <#2F74FF><link=\"tel:+18587480245,{0}\">{0}</link>", staffInfo.Extension);
		tmpComponents[1].text = string.Format("Email: <#2F74FF><link=\"mailto:{0}\">{0}</link>", staffInfo.Email);

		staffInfoPanel.GetComponent<BigPanelSlide>().SlideLeft();
	}
}

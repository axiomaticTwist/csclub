using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {
	AsyncOperation async;
	private void Start() {
		if (!SceneManager.GetActiveScene().Equals(SceneManager.GetSceneByName("Calendar"))) {
			async = SceneManager.LoadSceneAsync("Calendar",LoadSceneMode.Additive);

			async.allowSceneActivation = false;
		}
	}

	private void Update() {
		if (!async.isDone) {
			Debug.Log("Loading progress: " + async.progress * 100);
		}
	}

	public void LoadCalendar() {
		async.allowSceneActivation = true;
	}

	public void LoadHome() {

	}

	public void LoadTiles() {
		SceneManager.LoadScene("Panels");
	}
}

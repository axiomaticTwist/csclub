using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {
	public delegate void SceneChange();
	public static event SceneChange OnSceneChange;

	AsyncOperation async;

	private void Start() {
		if (!SceneManager.GetActiveScene().Equals(SceneManager.GetSceneByName("Calendar"))) {
			async = SceneManager.LoadSceneAsync("Calendar", LoadSceneMode.Additive);

			async.allowSceneActivation = false;
		}
	}

	public void LoadCalendar() {
		async.allowSceneActivation = true;

		if (!ClientSocket.connected) {
			ClientSocket.ConnectToServer();
		}
	}

	public void LoadHome() {
		async = SceneManager.LoadSceneAsync("Calendar", LoadSceneMode.Additive);

		async.allowSceneActivation = false;

		SceneManager.LoadScene("Home");

		if (!ClientSocket.connected) {
			ClientSocket.ConnectToServer();
		}

	}

	public void LoadTiles() {
		async = SceneManager.LoadSceneAsync("Calendar", LoadSceneMode.Additive);

		async.allowSceneActivation = false;

		SceneManager.LoadScene("Panels");

		if (!ClientSocket.connected) {
			ClientSocket.ConnectToServer();
		}
	}
}

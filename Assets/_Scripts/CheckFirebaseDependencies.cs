using Firebase.Analytics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckFirebaseDependencies : MonoBehaviour {
	// Start is called before the first frame update
	void Start () {
		DontDestroyOnLoad(this);
		/*
		Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
			var dependencyStatus = task.Result;
			if (dependencyStatus == Firebase.DependencyStatus.Available) {

				// Create and hold a reference to your FirebaseApp,
				// where app is a Firebase.FirebaseApp property of your application class.
				//   app = Firebase.FirebaseApp.DefaultInstance;
				FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);

				// GetComponent<InitFirebaseMessaging>().SubscribeEvents();

				Debug.Log("Firebase initalized");

				// Set a flag here to indicate whether Firebase is ready to use by your app.
			} else {
				Debug.LogError(string.Format(
				  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
				// Firebase Unity SDK is not safe to use here.
			}
		});*/

		//DownloadAnnouncements.ConnectAndRetrieveTopics();

		SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
	}
}

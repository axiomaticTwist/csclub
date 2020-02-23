using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckFirebaseDependencies : MonoBehaviour {
	// Start is called before the first frame update
	void Start () {
		DontDestroyOnLoad(this);
		
		// If user hasn't seen the introduction
		if (!PlayerPrefsX.GetBool("Introduction"))
			SceneManager.LoadScene(1, LoadSceneMode.Additive);
		else
			SceneManager.LoadScene(2, LoadSceneMode.Additive);
	}
}

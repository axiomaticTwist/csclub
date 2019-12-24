using UnityEngine;
using UnityEngine.UI;

public class LoadingRotate : MonoBehaviour {
	private void Update() {
		GetComponent<Image>().color = Color.Lerp(Color.white, Color.gray, Mathf.PingPong(Time.time, 1));
	}
}

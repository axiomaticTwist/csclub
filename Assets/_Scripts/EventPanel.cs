using UnityEngine.UI;
using UnityEngine;

public class EventPanel : MonoBehaviour {
	public Text eventTitle;
	public Text eventDesc;
	public Text eventLoc;

	public void Exit() {
		transform.gameObject.SetActive(false);
	}
}

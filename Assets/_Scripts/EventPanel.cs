using UnityEngine.UI;
using UnityEngine;

public class EventPanel : MonoBehaviour {
	public Text eventTitle;
	public Text eventDesc;
	public Text eventLoc;

	public Text eventDay;
	public Text eventMonth;
	public Text eventYear;

	public Image eventDateBG;

	public void Exit() {
		GetComponent<Animator>().Play("slide out");
	}
}

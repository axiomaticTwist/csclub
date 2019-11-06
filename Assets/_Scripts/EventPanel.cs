using UnityEngine.UI;
using UnityEngine;

public class EventPanel : MonoBehaviour {
	public Text eventTitle;
	public Text eventDesc;
	public Text eventLoc;

	public Text eventDay;
	public Text eventMonth;
	public Text eventYear;

	public Text eventCalendar;

	public Image eventDateBG;

	// Called on button pressed
	// Plays the slide out animation
	public void Exit() {
		GetComponent<Animator>().Play("slide out");
	}
}

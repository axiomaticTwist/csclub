using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dropdown : MonoBehaviour {

	public GameObject viewport;
	public GameObject arrow;

	public Text label;

	private bool dropped = false;
	private bool isAnimating = false;

	public void PlayAnimations() {
		if (!dropped) {
			viewport.GetComponent<Animator>().SetBool("dropped", dropped);
			arrow.GetComponent<Animator>().SetBool("dropped", dropped);
		} else {
			viewport.GetComponent<Animator>().SetBool("dropped", dropped);
			arrow.GetComponent<Animator>().SetBool("dropped", dropped);
		}

		dropped = !dropped;
	}


}

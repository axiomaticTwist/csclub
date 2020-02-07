using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class BigPanelSlide : MonoBehaviour {
	public void SlideIn() {
		gameObject.SetActive(true);
		GetComponent<Animator>().Play("big panel slide in");
	}

	public void SlideOut() {
		StartCoroutine(SlideOutDisable());
	}

	IEnumerator SlideOutDisable() {
		GetComponent<Animator>().Play("big panel slide out");
		yield return new WaitForSeconds(GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
		gameObject.SetActive(false);
	}

	public void SlideLeft() {
		gameObject.SetActive(true);
		GetComponent<Animator>().Play("big panel slide left");
	}

	public void SlideRight() {
		StartCoroutine(SlideRightDisable());
	}

	IEnumerator SlideRightDisable() {
		GetComponent<Animator>().Play("big panel slide right");
		yield return new WaitForSeconds(GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
		gameObject.SetActive(false);
	}
}

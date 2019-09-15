using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollAutomated : MonoBehaviour {

	private Vector2 panelLocation; // Position of current panel

	[Range(0, 1)]
	public float easing = 0.5f; // How much time you want to take to ease into next panel
								// Start is called before the first frame update
	void Start() {
		panelLocation = transform.position; // Set the position to the current position of the panel

	}

	// Update is called once per frame
	void Update() {

	}

	public void Scroll() {
		// Get the updated position of the panel
		Vector2 newLocation = panelLocation;
		// Subtract the screen width to display the next panel
		newLocation += new Vector2(-Screen.width, 0);

		// Call the SmoothScroll method and pass in the current position, the location of the new panel, and the easing value
		StartCoroutine(SmoothScroll(transform.position, newLocation, easing));
		// Update the initial panel location to the new location
		panelLocation = newLocation;
	}

	IEnumerator SmoothScroll(Vector2 startPos, Vector2 endPos, float seconds) {
		// Parameter t goes from 0 to 1
		float t = 0f;

		while (t <= 1f) {
			// Add the time since last frame divided by the amount of seconds we want it to take
			t += Time.deltaTime / seconds;
			// Update the position to smoothly go between two points
			transform.position = Vector2.Lerp(startPos, endPos, Mathf.SmoothStep(0, 1, t));
			yield return null;
		}
	}
}

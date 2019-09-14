using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// Derives from MonoBehaviour, implements IDragHandler and IEndDragHandler
public class Scroll : MonoBehaviour, IDragHandler, IEndDragHandler {

	[SerializeField]
	private int currentPage = 0; // Current page

	public int pageCount = 3; // Max pages

	private Vector2 panelLocation; // Position of current panel

	[Range(0, 1)]
	public float percentThreshold = 0.5f; // How much you want to scroll before going to next panel
	[Range(0, 1)]
	public float easing = 0.5f; // How much time you want to take to ease into next panel
	private float dragTime = 0f; // How long the user drags the panels

	// Start is called before the first frame update
	void Start() {
		panelLocation = transform.position; // Set the position to the current position of the panel
	}

	// Called whenever the user presses down on the screen and drags
	public void OnDrag(PointerEventData data) {
		dragTime += Time.deltaTime; // Increment drag time by the time since the last frame update

		// The amount the position changed, calculated by the initial position of the press - where the finger is now
		float deltaPosition = data.pressPosition.x - data.position.x; 

		// If the user is not scrolling up or down
		if (Mathf.Abs(data.delta.y) < 10)
			transform.position = panelLocation - new Vector2(deltaPosition, 0); // Update the panels' position to the position of the finger
	}

	// Called whenever the user lifts his finger
	public void OnEndDrag(PointerEventData data) {
		// What percentage of the screen was scrolled (expressed as a decimal)
		float percentageOfScroll = (data.pressPosition.x - data.position.x) / Screen.width;

		// If the screen was scrolled more than the threshold for the next panel OR we swipe really fast
		if (Mathf.Abs(percentageOfScroll) >= percentThreshold || Mathf.Abs(data.delta.x / dragTime) > 100) {
			// Get the updated position of the panel
			Vector2 newLocation = panelLocation;

			// If we swiped to the left
			if (percentageOfScroll > 0) {
				// If we haven't scrolled past the maximum number of panels
				if (currentPage < pageCount - 1) {
					// Subtract the screen width to display the next panel
					newLocation += new Vector2(-Screen.width, 0);
					// Increment the number of pages
					currentPage++;
				}
			// If we swiped to the right
			} else if (percentageOfScroll < 0) {
				// If we haven't scrolled past the minimum number of panels
				if (currentPage > 0) {
					// Add the screen width to display the previous panel
					newLocation += new Vector2(Screen.width, 0);
					// Increment the number of pages
					currentPage--;
				}
			}

			// Call the SmoothScroll method and pass in the current position, the location of the new panel, and the easing value
			StartCoroutine(SmoothScroll(transform.position, newLocation, easing));
			// Update the initial panel location to the new location
			panelLocation = newLocation;
		// If the screen was not scrolled far enough
		} else {
			// Move the screen back to its original position
			StartCoroutine(SmoothScroll(transform.position, panelLocation, easing));
		}
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

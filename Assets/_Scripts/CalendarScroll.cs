using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CalendarScroll : MonoBehaviour, IDragHandler, IEndDragHandler {
	public int currentPage = 1;
	public int pageCount = 3;

	private Vector2 panelLocation;
	private Vector2 panelStartLocation;
	public float percentThreshold = 0.5f;
	public float easing = 0.5f;
	private float dragTime = 0f;

	public VisualizeCalendar calendarScript;

	private void Start() {
		panelLocation = transform.position;
		panelStartLocation = panelLocation;
	}

	public void OnDrag(PointerEventData data) {
		dragTime += Time.deltaTime;

		float difference = data.pressPosition.x - data.position.x;
		if (Mathf.Abs(data.delta.y) < 10)
			transform.position = panelLocation - new Vector2(difference, 0);
	}

	public void OnEndDrag(PointerEventData data) {
		float percentage = (data.pressPosition.x - data.position.x) / Screen.width;

		int monthsToAdd = 0;

		if (Mathf.Abs(percentage) >= percentThreshold || Mathf.Abs(data.delta.x / dragTime) > 100) {
			Vector2 newLocation = panelLocation;

			if (percentage > 0) {
				newLocation += new Vector2(-Screen.width, 0);
				monthsToAdd = 1;
			} else if (percentage < 0) {
				newLocation += new Vector2(Screen.width, 0);
				monthsToAdd = -1;
			}
			
			StartCoroutine(SmoothScroll(transform.position, newLocation, easing, monthsToAdd));
			panelLocation = newLocation;

		} else {
			StartCoroutine(SmoothScroll(transform.position, panelLocation, easing, monthsToAdd));
		}

		dragTime = 0;
	}

	IEnumerator SmoothScroll(Vector2 startPos, Vector2 endPos, float seconds, int _monthsToAdd) {
		float t = 0f;

		while (t <= 1.0f) {
			t += Time.deltaTime / seconds;
			transform.position = Vector2.Lerp(startPos, endPos, Mathf.SmoothStep(0, 1, t));
			yield return null;
		}

		calendarScript.AddMonths(_monthsToAdd);
		

		transform.position = panelStartLocation;
		panelLocation = panelStartLocation;
	}
}

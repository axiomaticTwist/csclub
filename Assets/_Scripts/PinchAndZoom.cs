using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PinchAndZoom : MonoBehaviour {
	private Vector3 initialScale;

	[SerializeField]
	private float zoomSpeed = 0.1f;
	[SerializeField]
	private float maxZoom = 10f;

	public ScrollRect scrollRect;

	// Start is called before the first frame update
	void Start() {
		initialScale = transform.localScale;
	}

	void OnEnable() {
		transform.localScale = Vector3.one;
		initialScale = transform.localScale;
	}

	// Update is called once per frame
	void Update() {
		if (Input.touchCount > 1) {
			scrollRect.enabled = false;

			Touch touch0 = Input.GetTouch(0);
			Touch touch1 = Input.GetTouch(1);

			Vector2 touch0Prev = touch0.position - touch0.deltaPosition;
			Vector2 touch1Prev = touch1.position - touch1.deltaPosition;

			float prevMagnitude = (touch0Prev - touch1Prev).magnitude;
			float currentMagntitude = (touch0.position - touch1.position).magnitude;

			float diff = prevMagnitude - currentMagntitude;
			Zoom(diff);
		} else {
			scrollRect.enabled = true;
		}
	}

	void Zoom (float increment) {
		var delta = Vector3.one * (increment * -zoomSpeed);
		var desiredScale = transform.localScale + delta;

		desiredScale = ClampDesiredScale(desiredScale);

		transform.localScale = desiredScale;
	}

	private Vector3 ClampDesiredScale(Vector3 desiredScale) {
		desiredScale = Vector3.Max(initialScale, desiredScale);
		desiredScale = Vector3.Min(initialScale * maxZoom, desiredScale);
		return desiredScale;
	}
}

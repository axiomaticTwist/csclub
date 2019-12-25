using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalendarScale : MonoBehaviour {
	public RectTransform month0;
	public RectTransform month2;

	public RectTransform titleBar;

	public RectTransform monthYear0, monthYear1, monthYear2;
	RectTransform rectTransform;


	// Start is called before the first frame update
	void Start() {
		rectTransform = GetComponent<RectTransform>();

		float width = Mathf.Abs(rectTransform.rect.width);

		Vector2 offset = new Vector2(width, 0);

		month0.offsetMin = -offset;
		month0.offsetMax = -offset;

		month2.offsetMin = offset;
		month2.offsetMax = offset;

		titleBar.offsetMin = new Vector2(-offset.x, titleBar.offsetMin.y);
		titleBar.offsetMax = new Vector2(offset.x, titleBar.offsetMax.y);

		monthYear0.offsetMax = new Vector2(-offset.x * 2, 0);

		monthYear1.offsetMin = offset;
		monthYear1.offsetMax = -offset;

		monthYear2.offsetMin = new Vector2(offset.x * 2, 0);
	}

	// Update is called once per frame
	void Update() {

	}
}

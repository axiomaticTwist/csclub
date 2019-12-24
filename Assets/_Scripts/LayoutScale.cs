using UnityEngine;
using UnityEngine.UI;

public class LayoutScale : MonoBehaviour {
	private GridLayoutGroup layout;
	private RectTransform rectTransform;
	public CanvasScaler canvas;

	// Start is called before the first frame update
	void Start() {
		layout = GetComponent<GridLayoutGroup>();
		rectTransform = GetComponent<RectTransform>();
		
		layout.cellSize = new Vector2(rectTransform.rect.width / 2, rectTransform.rect.height  / 3f);
	}

	// Update is called once per frame
	void Update() {

	}
}

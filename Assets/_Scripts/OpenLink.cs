using UnityEngine;
using UnityEngine.EventSystems;

namespace TMPro {
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class OpenLink : MonoBehaviour, IPointerClickHandler {
		private TextMeshProUGUI tmp;

		private void Start() {
			tmp = GetComponent<TextMeshProUGUI>();
		}

		public void OnPointerClick(PointerEventData eventData) {
			int linkIndex = TMP_TextUtilities.FindIntersectingLink(tmp, Input.mousePosition, null);

			if (linkIndex != -1) { // was a link clicked?
				TMP_LinkInfo linkInfo = tmp.textInfo.linkInfo[linkIndex];

				// open the link id as a url, which is the metadata we added in the text field
				Application.OpenURL(linkInfo.GetLinkID());
			}
		}
	}
}
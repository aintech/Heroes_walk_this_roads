using UnityEngine;
using System.Collections;

public class StrokeText : MonoBehaviour {

	private TextMesh text, top, bottom, left, right;

	public Color32 textColor { get { return text.color; } set {text.color = value; }}

	public StrokeText init (string layerName, int sortingOrder) {
		text = GetComponent<TextMesh>();
		MeshRenderer rend = text.GetComponent<MeshRenderer>();
		rend.sortingLayerName = layerName;
		rend.sortingOrder = sortingOrder + 1;
		for (int i = 0; i < transform.childCount; i++) {
			rend = transform.GetChild(i).GetComponent<MeshRenderer>();
			rend.sortingLayerName = layerName;
			rend.sortingOrder = sortingOrder;
		}
		top = transform.Find("Top").GetComponent<TextMesh>();
		bottom = transform.Find("Bottom").GetComponent<TextMesh>();
		left = transform.Find("Left").GetComponent<TextMesh>();
		right = transform.Find("Right").GetComponent<TextMesh>();
        return this;
	}

	public void setText (string text) {
		this.text.text = text;
		top.text = text;
		bottom.text = text;
		left.text = text;
		right.text = text;
	}
}
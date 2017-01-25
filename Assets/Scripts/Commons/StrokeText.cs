using UnityEngine;
using System.Collections;

public class StrokeText : MonoBehaviour {
    
    private TextMesh textMesh, topMesh, bottomMesh, leftMesh, rightMesh;

    public string text {
        get { return textMesh.text; }
        set { textMesh.text = value;
              topMesh.text = value;
              bottomMesh.text = value;
              leftMesh.text = value;
              rightMesh.text = value;
        }
    }

	public Color32 color { get { return textMesh.color; } set {textMesh.color = value; }}

	public StrokeText init (string layerName, int sortingOrder) {
		textMesh = GetComponent<TextMesh>();
		MeshRenderer rend = textMesh.GetComponent<MeshRenderer>();
		rend.sortingLayerName = layerName;
		rend.sortingOrder = sortingOrder + 1;
		for (int i = 0; i < transform.childCount; i++) {
			rend = transform.GetChild(i).GetComponent<MeshRenderer>();
			rend.sortingLayerName = layerName;
			rend.sortingOrder = sortingOrder;
		}
        topMesh = transform.Find("Top").GetComponent<TextMesh>();
		bottomMesh = transform.Find("Bottom").GetComponent<TextMesh>();
		leftMesh = transform.Find("Left").GetComponent<TextMesh>();
		rightMesh = transform.Find("Right").GetComponent<TextMesh>();
        return this;
	}
}
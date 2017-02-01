using UnityEngine;
using System.Collections;

public class DescriptionLine : MonoBehaviour {
    
    private TextMesh valueText;

    private MeshRenderer mesh;

    private Transform background;

    private const float fontLengthMulty = 4.9f;

    private Vector3 scale = Vector3.one;

    private float yOffset = -.38f;

    public DescriptionLine init (Transform holder, float pos) {
        valueText = transform.Find("Value").GetComponent<TextMesh>();
        mesh = valueText.GetComponent<MeshRenderer>();
        mesh.sortingLayerName = "User Interface";
        mesh.sortingOrder = 9;

        background = transform.Find("Background");

        transform.SetParent(holder);
        transform.localPosition = new Vector3(0, pos * yOffset, 0);

        return this;
    }

    public float setText (string text) {
        valueText.text = text;
        scale.x = Utils.calcMeshLength(mesh, fontLengthMulty);
        background.localScale = scale;

        gameObject.SetActive(true);

        return mesh.bounds.size.x;
    }

    public void hide () {
        gameObject.SetActive(false);
    }
}
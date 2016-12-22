using UnityEngine;
using System.Collections;

public class ElementsPool : MonoBehaviour {

	private StrokeText fireCountText, airCountText, earthCountText, waterCountText;

	public int fireCount { get; private set; }
	public int airCount { get; private set; }
	public int earthCount { get; private set; }
	public int waterCount { get; private set; }

    private Vector3 originalPosition, smallPosition = new Vector3(-6.8f, 3.2f, 0);

    private Vector3 smallScale = new Vector3(.7f, .7f, 1);

    public ElementsPool init () {
		SpriteRenderer rend = transform.Find("Background").GetComponent<SpriteRenderer>();
		fireCountText = transform.Find("Fire Count").GetComponent<StrokeText>().init(rend.sortingLayerName, rend.sortingOrder + 1);
        airCountText = transform.Find("Air Count").GetComponent<StrokeText>().init(rend.sortingLayerName, rend.sortingOrder + 1);
        earthCountText = transform.Find("Earth Count").GetComponent<StrokeText>().init(rend.sortingLayerName, rend.sortingOrder + 1);
        waterCountText = transform.Find("Water Count").GetComponent<StrokeText>().init(rend.sortingLayerName, rend.sortingOrder + 1);

        originalPosition = transform.localPosition;

		updateCounters();

        return this;
    }

    public void changeSize (bool smaller) {
        transform.localScale = smaller? smallScale: Vector3.one;
        transform.localPosition = smaller? smallPosition: originalPosition;
    }

    public void addElements (ElementType type, int count) {
        switch (type) {
            case ElementType.FIRE: fireCount += count; break;
            case ElementType.AIR: airCount += count; break;
            case ElementType.EARTH: earthCount += count; break;
            case ElementType.WATER: waterCount += count; break;
            default: Debug.Log("Unsupported type: " + type); break;
        }
        updateCounters();
    }

	private void updateCounters () {
		fireCountText.setText(fireCount == 0? "": fireCount.ToString());
		airCountText.setText(airCount == 0? "": airCount.ToString());
		earthCountText.setText(earthCount == 0? "": earthCount.ToString());
		waterCountText.setText(waterCount == 0? "": waterCount.ToString());
	}
}
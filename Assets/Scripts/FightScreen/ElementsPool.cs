using UnityEngine;
using System.Collections;

public class ElementsPool : MonoBehaviour {

	private StrokeText fireCountText, airCountText, earthCountText, waterCountText;

	public int fireCount { get; private set; }
	public int airCount { get; private set; }
	public int earthCount { get; private set; }
	public int waterCount { get; private set; }

    public ElementsPool init () {
		SpriteRenderer rend = transform.Find("Background").GetComponent<SpriteRenderer>();
		fireCountText = transform.Find("Fire Count").GetComponent<StrokeText>().init(rend.sortingLayerName, rend.sortingOrder);
		airCountText = transform.Find("Air Count").GetComponent<StrokeText>().init(rend.sortingLayerName, rend.sortingOrder);
		earthCountText = transform.Find("Earth Count").GetComponent<StrokeText>().init(rend.sortingLayerName, rend.sortingOrder);
		waterCountText = transform.Find("Water Count").GetComponent<StrokeText>().init(rend.sortingLayerName, rend.sortingOrder);

		updateCounters();

        return this;
    }

    public void addElements () {
        Debug.Log("Add elements to Elements Pool");
    }

	private void updateCounters () {
		fireCountText.setText(fireCount == 0? "": fireCount.ToString());
		airCountText.setText(airCount == 0? "": airCount.ToString());
		earthCountText.setText(earthCount == 0? "": earthCount.ToString());
		waterCountText.setText(waterCount == 0? "": waterCount.ToString());
	}
}
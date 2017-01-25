using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ElementsPool : MonoBehaviour {

	public static ElementsPool instance { get; private set;}

	public Dictionary<ElementType, int> elements { get; private set; }

	private StrokeText fireCountText, airCountText, earthCountText, waterCountText;

//	public int fireCount { get; private set; }
//	public int airCount { get; private set; }
//	public int earthCount { get; private set; }
//	public int waterCount { get; private set; }

    private Vector3 originalPosition, smallPosition = new Vector3(-6.8f, 3.2f, 0);

    private Vector3 smallScale = new Vector3(.7f, .7f, 1);

    public ElementsPool init () {
		instance = this;
		elements = new Dictionary<ElementType, int>();
		foreach (ElementType type in Enum.GetValues(typeof (ElementType))) {
			elements.Add(type, 0);
		}

		SpriteRenderer rend = transform.Find("Background").GetComponent<SpriteRenderer>();
		fireCountText = transform.Find("Fire Count").GetComponent<StrokeText>().init(rend.sortingLayerName, rend.sortingOrder + 1);
        airCountText = transform.Find("Air Count").GetComponent<StrokeText>().init(rend.sortingLayerName, rend.sortingOrder + 1);
        earthCountText = transform.Find("Earth Count").GetComponent<StrokeText>().init(rend.sortingLayerName, rend.sortingOrder + 1);
        waterCountText = transform.Find("Water Count").GetComponent<StrokeText>().init(rend.sortingLayerName, rend.sortingOrder + 1);

        originalPosition = transform.localPosition;

		updateCounters();

        gameObject.SetActive(true);

        return this;
    }

    public void changeSize (bool smaller) {
        transform.localScale = smaller? smallScale: Vector3.one;
        transform.localPosition = smaller? smallPosition: originalPosition;
    }

    public void addElements (ElementType type, int count) {
		elements[type] += count;
        updateCounters();
    }

	public void updateCounters () {
        fireCountText.text = elements[ElementType.FIRE] == 0? "": elements[ElementType.FIRE].ToString();
		airCountText.text = elements[ElementType.AIR] == 0? "": elements[ElementType.AIR].ToString();
		earthCountText.text = elements[ElementType.EARTH] == 0? "": elements[ElementType.EARTH].ToString();
		waterCountText.text = elements[ElementType.WATER] == 0? "": elements[ElementType.WATER].ToString();
	}

    public void clear () {
		foreach (ElementType type in Enum.GetValues(typeof (ElementType))) {
			elements[type] = 0;
		}
//        fireCount = airCount = waterCount = earthCount = 0;
        updateCounters();
    }
}
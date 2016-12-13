using UnityEngine;
using System.Collections;

public abstract class Slot : ItemHolder {

	public ItemType itemType;

	public Sprite slotBG, activeSlotBG;

	private SpriteRenderer bgRender, iconRender;

	public void init () {
		bgRender = GetComponent<SpriteRenderer>();
		iconRender = transform.Find("Icon Image").GetComponent<SpriteRenderer>();
		setActive(false);
	}

	public void setActive (bool asActive) {
        bgRender.sprite = asActive? activeSlotBG: slotBG;
        if (asActive) {
            bgRender.enabled = true;
        } else {
            bgRender.enabled = item == null;
        }
	}

	virtual public void setItem (Item newItem) {
		item = newItem;
		item.slot = this;
		item.cell = null;
		item.transform.parent = transform;
		item.transform.localPosition = Vector3.zero;
		iconRender.enabled = false;
        bgRender.enabled = false;
	}

	override public Item takeItem () {
		Item itemRef = item;
		item = null;
		iconRender.enabled = true;
        bgRender.enabled = true;
		return itemRef;
	}
}
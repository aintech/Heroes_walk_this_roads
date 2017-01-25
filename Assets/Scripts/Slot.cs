using UnityEngine;
using System.Collections;

public abstract class Slot : ItemHolder {

    public int index;

	public ItemType itemType;

	public Sprite slotBG, activeSlotBG;

    protected SpriteRenderer iconRender;

	private SpriteRenderer bgRender;

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

	virtual public void setItem (Item item) {
		this.item = item;
		this.item.slot = this;
		this.item.cell = null;
		this.item.transform.parent = transform;
		this.item.transform.localPosition = Vector3.zero;
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

    public virtual void hideItem() {}
}
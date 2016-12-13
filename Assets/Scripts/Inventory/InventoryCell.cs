using UnityEngine;
using System.Collections;

public class InventoryCell : ItemHolder {

	public int index;

	public Inventory inventory { get; private set; }

	private Transform itemsContainer;

	public void init(Inventory inventory, Transform itemsContainer) {
        this.inventory = inventory;
		this.itemsContainer = itemsContainer;
    }

	public void setItem (Item newItem) {
		item = newItem;
		if (item != null) {
			item.slot = null;
			item.cell = this;
			item.transform.parent = itemsContainer;
			item.transform.localPosition = transform.localPosition;
		}
	}

	override public Item takeItem () {
		if (item == null) { return null; }

		inventory.getItems ().Remove (index + inventory.getOffset ());
		Item returnItem = item;
		item = null;
		return returnItem;
	}
}
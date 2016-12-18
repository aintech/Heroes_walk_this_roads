using UnityEngine;
using System.Collections;

public abstract class InventoryContainedScreen : MonoBehaviour, ButtonHolder {

	public Inventory inventory { get; protected set; }
	
	protected Item draggedItem, chosenItem;
	
	private Transform chosenItemBorder;
	
	private Vector3 draggedItemPosition = Vector3.zero;

	private Vector2 dragOffset;

	private int dragOrder = 5;

	protected void innerInit(Inventory inventory, string layerName) {
		this.inventory = inventory;
		chosenItemBorder = transform.Find ("Chosen Item Border");
		if (chosenItemBorder != null) { chosenItemBorder.gameObject.SetActive(false); }
	}

	void Update () {
        if (inventory.inventoryType == Inventory.InventoryType.MARKET_BUY || inventory.inventoryType == Inventory.InventoryType.MARKET_SELL) { return; }
		if (Input.GetMouseButtonDown(0) && Utils.hit != null) {
			if (Utils.hit.name.Equals("Cell")) {
				Item item = Utils.hit.GetComponent<InventoryCell>().item;
				if (item != null) {
					draggedItem = Utils.hit.GetComponent<InventoryCell>().takeItem();
					draggedItem.changeSortOrder(dragOrder);
					choseItem(item);
					chosenItemBorder.transform.position = item.transform.position;
					chosenItemBorder.gameObject.SetActive(true);
				}
			} else if (Utils.hit.GetComponent<Slot>() != null) {
				Slot slot = Utils.hit.GetComponent<Slot>();
				Item item = slot.item;
				if (slot.item != null) {
					chooseDraggedItemFromSlot(slot);
					chosenItemBorder.transform.position = item.transform.position;
					choseItem(item);
					chosenItemBorder.gameObject.SetActive(true);
				}
			}
		}
		if (draggedItem != null) {
			draggedItemPosition.Set(Utils.mousePos.x - dragOffset.x, Utils.mousePos.y - dragOffset.y, 0);
			draggedItem.transform.position = draggedItemPosition;
			chosenItemBorder.position = chosenItem.transform.position;
			if (Input.GetMouseButtonUp(0)) dropItem ();
		}
	}

	public void fireClickButton (Button btn) {
		checkBtnPress(btn);
	}

	abstract protected void checkBtnPress (Button btn);
	
	virtual protected void chooseDraggedItemFromSlot (Slot slot) {
		draggedItem = slot.takeItem();
		draggedItem.changeSortOrder(dragOrder);
	}

	virtual protected void choseItem (Item item) {
		chosenItem = item;
		dragOffset.Set(Utils.mousePos.x - item.transform.position.x, Utils.mousePos.y - item.transform.position.y);
	}
	
	private void dropItem () {
		checkItemDrop ();
		draggedItem.changeSortOrder(dragOrder - 1);
		if(chosenItem != null) chosenItemBorder.position = chosenItem.transform.position;
		draggedItem = null;
		afterItemDrop ();
	}

	virtual protected void checkItemDrop () {}
	virtual protected void afterItemDrop () {}

	protected void hideItemInfo () {
        if (chosenItemBorder != null) {
            chosenItemBorder.gameObject.SetActive(false);
        }
		chosenItem = null;
	}
	
	public Item getChosenItem () {
		return chosenItem;
	}

	public void updateChosenItemBorder (Item newItem) {
		choseItem(newItem);
		updateChosenItemBorder();
	}

	public void updateChosenItemBorder () {
		if (chosenItem != null) {
			if (chosenItem.cell == null && chosenItem.slot == null) {
				chosenItemBorder.gameObject.SetActive(false);
			} else {
				chosenItemBorder.transform.position = chosenItem.transform.position;
				chosenItemBorder.gameObject.SetActive (true);
			}
		} else {
			chosenItemBorder.gameObject.SetActive (false);
		}
	}

	public void updateChosenItemBorder (bool hideBorder) {
		if (hideBorder) chosenItemBorder.gameObject.SetActive (false);
		else chosenItemBorder.gameObject.SetActive (true);
		if (chosenItem != null) chosenItemBorder.position = chosenItem.transform.position;
	}
}
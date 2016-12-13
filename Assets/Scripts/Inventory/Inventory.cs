using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Inventory : MonoBehaviour, ButtonHolder {

	private Transform itemsContainer;

	public InventoryType inventoryType { get; private set; }

	public InventoryContainedScreen containerScreen { get; private set; }

	private float capacity = 100, freeVolume;

	private InventoryCell[] cells;
	
	private Dictionary<int, Item> items = new Dictionary<int, Item>();

	private Button upBtn, downBtn, sortBtn;

	private int offset = 0;

	private int offsetStep;

	private bool scrollableUp, scrollableDown;

	private TextMesh volumeMesh;

	public Inventory init (InventoryType inventoryType) {
		this.inventoryType = inventoryType;

		cells = transform.GetComponentsInChildren<InventoryCell> ();

		itemsContainer = transform.Find("Items Container");

        foreach (InventoryCell cell in cells) {
			cell.init(this, itemsContainer);
        }

		upBtn = transform.FindChild ("Up Button").GetComponent<Button> ().init();
		downBtn = transform.FindChild ("Down Button").GetComponent<Button> ().init();
        if (inventoryType == InventoryType.INVENTORY) {
            sortBtn = transform.FindChild("Sort Button").GetComponent<Button>().init();
            volumeMesh = transform.Find ("VolumeTxt").GetComponent<TextMesh> ();
            MeshRenderer meshRend = volumeMesh.GetComponent<MeshRenderer> ();
            meshRend.sortingLayerName = "Inventory";
            meshRend.sortingOrder = 3;
            sortBtn.gameObject.SetActive(inventoryType == InventoryType.INVENTORY);
            volumeMesh.gameObject.SetActive(inventoryType == InventoryType.INVENTORY);
        }

		checkButtons ();

        gameObject.SetActive(true);

		return this;
	}

	public void setContainerScreen (InventoryContainedScreen containerScreen, int columnsCount) {
		this.containerScreen = containerScreen;
		this.offsetStep = columnsCount;
	}

	void Update () {
		if (Input.GetAxis("Mouse ScrollWheel") > 0 && Utils.hit != null) {
			if (Utils.hit.name.Equals("Cell") && Utils.hit.GetComponent<InventoryCell>().inventory == this) {
				scroll(true);
			}
		} else if (Input.GetAxis("Mouse ScrollWheel") < 0 && Utils.hit != null) {
			if (Utils.hit.name.Equals("Cell") && Utils.hit.GetComponent<InventoryCell>().inventory == this) {
				scroll(false);
			}
		}
	}

	public void fireClickButton (Button btn) {
		if (btn == upBtn) { scroll(true); }
		else if (btn == downBtn) { scroll(false); }
		else if (btn == sortBtn) {
			sortInventory();
			containerScreen.updateChosenItemBorder();
		} else { Debug.Log("Unknown button: " + btn.name); }
	}

	private void scroll (bool up) {
		offset += (up && scrollableUp)? -offsetStep: (!up && scrollableDown)? offsetStep: 0;
		refreshInventory ();
		Item chosenItem = containerScreen.getChosenItem ();
		if (chosenItem != null && chosenItem.cell != null && chosenItem.cell.inventory == this) {
			foreach (InventoryCell cell in cells) {
				if (cell.item == chosenItem) {
					containerScreen.updateChosenItemBorder (false);
					return;
				}
			}
			containerScreen.updateChosenItemBorder (true);
		}
	}

	public void setInventoryToBegin () {
		offset = 0;
		refreshInventory ();
	}

	public void refreshInventory () {
		foreach (InventoryCell cell in cells) {
			cell.setItem (null);
		}

		foreach (KeyValuePair<int, Item> pair in items) {
			Item item = pair.Value;
			if (pair.Key >= offset && pair.Key < (cells.Length + offset)) {
				getCell (pair.Key - offset).setItem (item);
				item.gameObject.SetActive (true);
			} else {
				item.gameObject.SetActive (false);
			}
		}

		calculateFreeVolume();
		checkButtons ();
	}

	public void loadItems (Dictionary<int, ItemData> newItems) {
		clearInventory();
		foreach (KeyValuePair<int, ItemData> pair in newItems) {
			items.Add(pair.Key, Instantiate<Transform>(ItemFactory.itemPrefab).GetComponent<Item>().init(pair.Value, pair.Key));
		}
		refreshInventory ();
	}

	public void addItemToCell (Item item, InventoryCell cell) {
		if (cell == null) {
			addItemToFirstFreePosition (item, true);
			return;
		}

		Inventory source = item.cell == null? null: item.cell.inventory;

		if (source != this) {
			if (inventoryType == InventoryType.INVENTORY && item.slot == null) {
				if (getFreeVolume() < (item.volume * item.quantity)) {
					item.returnToParent ();
					Messenger.inventoryCapacityLow(item.name, item.quantity);
					return;
				}
			}
		}

		if (source != null && source.inventoryType == InventoryType.INVENTORY) { source.calculateFreeVolume(); }

		InventoryCell prevCell = item.cell;
		Item prevItem = null;
		bool stackableItem = false;

		if (cell.item != null && item.type == ItemType.MATERIAL) {
			if (cell.item.type == ItemType.MATERIAL) {
				if (((MaterialData)cell.item.itemData).type == ((MaterialData)item.itemData).type) {
					cell.item.quantity += item.quantity;
					containerScreen.updateChosenItemBorder(cell.item);
					item.dispose();
					stackableItem = true;
				}
			}
		}

		if (!stackableItem) {
			prevItem = cell.takeItem ();

			item.index = cell.index + offset;
			items.Add (item.index, item);

			if (prevItem != null) {
				if (source == this) { addItemToCell (prevItem, prevCell); }
				else { addItemToFirstFreePosition (prevItem, false); }
			}
		}

		refreshInventory();
	}

	private void addItemToFirstFreePosition (Item item, bool refresh) {
		if (item.type == ItemType.MATERIAL) {
			if (addStackableItem(item)) { return; }
		}
		int newIndex = getMinFreeItemIndex ();
		item.index = newIndex;
		items.Add (newIndex, item);
		if (refresh) refreshInventory ();
	}

	private bool addStackableItem (Item item) {
		MaterialType type = ((MaterialData)item.itemData).type;
		foreach (KeyValuePair<int, Item> pair in items) {
			if (pair.Value.type == ItemType.MATERIAL && type == ((MaterialData)pair.Value.itemData).type) {
				pair.Value.quantity += item.quantity;
				item.dispose();
				return true;
			}
		}
		return false;
	}

	private int getMinFreeItemIndex () {
		int maxIndex = getMaximumItemIndex();
		for (int i = 0; i <= maxIndex; i++) {
			if (!items.ContainsKey(i)) { return i; }
		}
		return ++maxIndex;
	}
	
	private int getMaximumItemIndex () {
		int index = 0;
		foreach (KeyValuePair<int, Item> pair in items) {
			if (pair.Key > index) {
				index = pair.Key;
			}
		}
		return index;
	}

	private void checkButtons () {
		scrollableUp = offset != 0;
		upBtn.setActive(scrollableUp);

		scrollableDown = getMaximumItemIndex () >= (cells.Length + offset);
		downBtn.setActive(scrollableDown);
	}

	public void calculateFreeVolume () {
		if (inventoryType != InventoryType.INVENTORY) { return; }
		freeVolume = getCapacity ();
		foreach (KeyValuePair<int, Item> pair in items) {
			freeVolume -= pair.Value.volume;	
		}
		updateVolumeTxt();
	}

	public float getFreeVolume () {
		return freeVolume;
	}

	private InventoryCell getCell (int index) {
		foreach (InventoryCell cell in cells) {
			if (cell.index == index) {
				return cell;
			}
		}
		return null;
	}

	public void fillWithRandomItems () {
		fillWithRandomItems (UnityEngine.Random.Range(20, 50), null);
	}

	public void fillWithRandomItems (int count, string label) {
		clearInventory();
		for (int i = 0; i < count; i++) {
			ItemData data = null;
			switch (Mathf.FloorToInt (UnityEngine.Random.value * 17)) {
				case 0: data = ItemFactory.createItemData(ItemType.SHIELD); break;
				case 1: data = ItemFactory.createItemData(ItemType.HELMET); break;
				case 2: data = ItemFactory.createItemData(ItemType.GLOVE); break;
                case 3: data = ItemFactory.createItemData(ItemType.AMULET); break;
                case 4: data = ItemFactory.createItemData(ItemType.RING); break;
				case 5: case 6: data = ItemFactory.createItemData(ItemType.WEAPON); break;
				case 7: case 8: data = ItemFactory.createItemData(ItemType.ARMOR); break;
				case 9: case 10: case 11: data = ItemFactory.createItemData(ItemType.MATERIAL); break;
				case 12: case 13: case 14: case 15: case 16: data = ItemFactory.createItemData(ItemType.SUPPLY); break;
			}
			Item item = Instantiate<Transform>(ItemFactory.itemPrefab).GetComponent<Item>().init(data);
			item.transform.SetParent(itemsContainer);
			if (label != null) { item.name = label; }
			addItemToFirstFreePosition(item, false);
		}

		refreshInventory ();
		sortInventory();
	}

	private void updateVolumeTxt () {
		if (inventoryType != InventoryType.INVENTORY) { return; }
		volumeMesh.text = "Объём: " + (freeVolume < 0? "<color=red>": "<color=orange>") + freeVolume.ToString("0.0") + "</color>";
	}

	public Dictionary<int, Item> getItems () {
		//По странной причине иногда после боя ячейки теряют свои предметы...
		if (items.Count == 0 && itemsContainer.childCount > 0) {
			Debug.Log("Strange shit happens: " + inventoryType + ": " + transform.name);
			recheckItems();
		}
		return items;
	}

	private void recheckItems () {
		Item item;
		for (int i = 0; i < itemsContainer.childCount; i++) {
			item = itemsContainer.GetChild(i).GetComponent<Item>();
			items.Add(item.index, item);
		}
		refreshInventory();
	}

	public void setItemsFromOtherInventory (Inventory inventory) {
		items = inventory.getItems();
		refreshInventory();
	}

	public Item takeLastItem () {
		int index = getMaximumItemIndex();
		if (getCell(index) != null && getCell(index).item != null) {
			return getCell(index).takeItem();
		}
		Item item = items[index];
		items.Remove(index);
		return item;
	}

	public void sortInventory () {
		Dictionary<ItemType,  List<Item>> itemsList = new Dictionary<ItemType, List<Item>>();

		foreach (ItemType type in Enum.GetValues(typeof(ItemType))) {
			itemsList.Add(type, new List<Item>());
		}

		foreach (KeyValuePair<int, Item> pair in items) {
			itemsList[pair.Value.type].Add(pair.Value);
		}

		items.Clear();

		Dictionary<ItemType, List<Item>> tempDictionary = new Dictionary<ItemType, List<Item>>();
		foreach (KeyValuePair<ItemType, List<Item>> pair in itemsList) {
			tempDictionary.Add(pair.Key, sortList(pair.Value, pair.Key));
		}
		itemsList.Clear();

		int counter = 0;
		counter = addSortToItems(tempDictionary[ItemType.SUPPLY], counter);
        counter = addSortToItems(tempDictionary[ItemType.WEAPON], counter);
        counter = addSortToItems(tempDictionary[ItemType.AMULET], counter);
        counter = addSortToItems(tempDictionary[ItemType.RING], counter);
		counter = addSortToItems(tempDictionary[ItemType.ARMOR], counter);
		counter = addSortToItems(tempDictionary[ItemType.SHIELD], counter);
		counter = addSortToItems(tempDictionary[ItemType.HELMET], counter);
		counter = addSortToItems(tempDictionary[ItemType.GLOVE], counter);
		counter = addSortToItems(tempDictionary[ItemType.MATERIAL], counter);

		refreshInventory ();
	}

	private List<Item> sortList (List<Item> list, ItemType type) {
		SortedDictionary<long, Item> weights = new SortedDictionary<long, Item>();
		long weight = 0;
		foreach (Item item in list) {
			weight = item.itemData.sortWeight + item.cost;
			while(weights.ContainsKey(weight)) {
				weight++;
			}
			weights.Add(weight, item);
		}

		list.Clear();

		foreach (KeyValuePair<long, Item> pair in weights) {
			list.Add(pair.Value);
		}
		list.Reverse();

		return list;
	}

	private int addSortToItems (List<Item> list, int count) {
		for (int i = 0; i < list.Count; i++) {
			items.Add(i + count, list[i]);
		}
		return count + list.Count;
	}

	public void setCapacity (float capacity) {
		this.capacity = capacity;
		calculateFreeVolume();
	}
	
	public float getCapacity () {
		return capacity;
	}

	public int getOffset () {
		return offset;
	}

	private void clearInventory () {
		Dictionary<int, Item> spare = new Dictionary<int, Item>(items);
		foreach (KeyValuePair<int, Item> pair in spare) {
			if (pair.Value.cell != null) {
				pair.Value.cell.takeItem();
			}
			Destroy(pair.Value.gameObject);
		}

		items.Clear();
	}

//	public void sendToVars () {
//		Dictionary<int, ItemData> inventoryToSend = inventoryType == InventoryType.INVENTORY? Vars.inventory: Vars.markets[Vars.planetType];
//		if (inventoryToSend == null) {
//			Debug.Log("Unmapped inventory to send: " + inventoryType);
//		} else {
//			inventoryToSend.Clear();
//			foreach (KeyValuePair<int, Item> pair in items) {
//				inventoryToSend.Add(pair.Key, pair.Value.itemData);
//			}
//			clearInventory();
//		}
//	}
//
//	public void initFromVars () {
//		loadItems(inventoryType == InventoryType.INVENTORY? Vars.inventory: Vars.markets[Vars.planetType]);
//	}

    public enum InventoryType {
        INVENTORY, MARKET_SELL, MARKET_BUY
    }
}
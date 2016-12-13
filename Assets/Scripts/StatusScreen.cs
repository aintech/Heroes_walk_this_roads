using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StatusScreen : InventoryContainedScreen, Closeable {

    public Sprite nude, underwear, armor;

	private ItemDescriptor itemDescriptor;

    private SpriteRenderer playerRender;

    public EquipmentSlot[] ringSlots { get; private set; }

    public List<Slot> allSlots { get; private set; }

    public List<EquipmentSlot> equipmentSlots { get; private set; }

    public List<SupplySlot> supplySlots { get; private set; }

    private StrokeText healthValue, experienceValue, damageValue, armorValue, strengthValue, enduranceValue, agilityValue;

    private bool onTownMainScreen;

	public StatusScreen init (ItemDescriptor itemDescriptor) {
		this.itemDescriptor = itemDescriptor;
        itemDescriptor.statusScreen = this;

		innerInit(transform.Find("Inventory").GetComponent<Inventory>().init(Inventory.InventoryType.INVENTORY), "Inventory");

        allSlots = new List<Slot>();
        equipmentSlots = new List<EquipmentSlot>();
        supplySlots = new List<SupplySlot>();

        Slot slot;
        Transform slotsContainer = transform.Find("Slots Container");
        slotsContainer.gameObject.SetActive(true);
        ringSlots = new EquipmentSlot[2];
        for (int i = 0; i < slotsContainer.childCount; i++) {
            slot = slotsContainer.GetChild(i).GetComponent<Slot>();
            if (slot != null) {
                slot.init();
                if (slot.itemType == ItemType.SUPPLY) {
                    supplySlots.Add((SupplySlot)slot);
                } else {
                    if (slot.itemType == ItemType.RING) {
                        if (slot.transform.position.x < 5) {
                            ringSlots[0] = (EquipmentSlot)slot;
                        } else {
                            ringSlots[1] = (EquipmentSlot)slot;
                        }
                    }
                    equipmentSlots.Add((EquipmentSlot)slot);
                }
                allSlots.Add(slot);
            }
        }

        foreach (EquipmentSlot slt in equipmentSlots) { slt.statusScreen = this; }

        Transform attributes = transform.Find ("Attributes");
        attributes.gameObject.SetActive(true);

        string layerName = "Inventory";
        int layerOrder = 3;
        healthValue = attributes.Find ("Health").GetComponent<StrokeText> ().init(layerName, layerOrder);
        experienceValue = attributes.Find("Experience").GetComponent<StrokeText>().init(layerName, layerOrder);
        damageValue = attributes.Find ("Damage").GetComponent<StrokeText> ().init(layerName, layerOrder);
        armorValue = attributes.Find ("Armor").GetComponent<StrokeText> ().init(layerName, layerOrder);
        strengthValue = attributes.Find("Strength").GetComponent<StrokeText>().init(layerName, layerOrder);
        enduranceValue = attributes.Find("Endurance").GetComponent<StrokeText>().init(layerName, layerOrder);
        agilityValue = attributes.Find("Agility").GetComponent<StrokeText>().init(layerName, layerOrder);

        playerRender =  transform.Find("Player Image").GetComponent<SpriteRenderer>();
        playerRender.gameObject.SetActive(true);

        updateAttributes();

        Player.statusScreen = this;

        gameObject.SetActive(false);

		return this;
	}

    public void updateAttributes () {
        healthValue.setText(Player.health.ToString());// + (Player.health < Player.maxHealth? "/" + Player.maxHealth.ToString(): "");
        experienceValue.setText(((int)Player.experience).ToString() + " %");
        damageValue.setText(Player.damage.ToString());
        armorValue.setText(Player.armorClass.ToString());
        strengthValue.setText(Player.strength.ToString());
        enduranceValue.setText(Player.endurance.ToString());
        agilityValue.setText(Player.agility.ToString());
    }

    public EquipmentSlot getEquipmentSlot (ItemType type) {
        foreach (EquipmentSlot slot in equipmentSlots) {
            if (slot.itemType == type) { return slot; }
        }
        Debug.Log("No slot for item type: " + type);
        return null;
    }

    public SupplySlot getSupplySlot (int index) {
        foreach (SupplySlot slot in supplySlots) {
            if (slot.index == index) { return slot; }
        }
        Debug.Log("Unknown slot index: " + index);
        return null;
    }

    public void updatePlayerImage () {
        playerRender.sprite = Player.armor == null? (Vars.EROTIC? nude: underwear): armor;
    }

	public void showScreen () {
		if (gameObject.activeInHierarchy) { return; }

//		UserInterface.showInterface = false;
        itemDescriptor.setEnabled();

		inventory.setContainerScreen(this, 6);
		inventory.setInventoryToBegin ();

        updateAttributes();

		transform.position = Vector3.zero;

        Gameplay.topHideable.setVisible(false);

		InputProcessor.add(this);

        UserInterface.updateStatusBtnText(true);

		gameObject.SetActive(true);
	}

	public void close (bool byInputProcessor) {
		hideItemInfo();
		gameObject.SetActive(false);

        Gameplay.topHideable.setVisible(true);
//		UserInterface.showInterface = true;
        UserInterface.updateStatusBtnText(false);
		itemDescriptor.setDisabled();

		if (!byInputProcessor) { InputProcessor.removeLast(); }
	}

	protected override void checkBtnPress (Button btn) {}

	override protected void checkItemDrop () {
		if (Utils.hit != null && Utils.hit.name.Equals("Cell")) {
			InventoryCell cell = Utils.hit.GetComponent<InventoryCell>();
			Inventory targetInv = cell.inventory;
			if (draggedItem.cell == null) {
                updateAttributes();
			}
			targetInv.addItemToCell(draggedItem, cell);
        } else if (Utils.hit != null && Utils.hit.name.Equals("Slot")) {
			EquipmentSlot slot = Utils.hit.GetComponent<EquipmentSlot> ();
			if (slot.itemType != draggedItem.type) {
				if (draggedItem.cell == null && draggedItem.slot == null) {
					if (inventory.gameObject.activeInHierarchy) {
						inventory.addItemToCell (draggedItem, draggedItem.cell);
					}
                    updateAttributes();
				} else {
					draggedItem.returnToParent ();
				}
			} else if (slot.item == null) {
				setItemToSlot (slot);
			} else if (slot.item != null) {
				Item currItem = slot.takeItem ();
                if (draggedItem.slot != null && draggedItem.type == slot.itemType) {
					draggedItem.slot.setItem(currItem);
				} else if (draggedItem.slot != null) {
					draggedItem.returnToParent();
				} else if (inventory.gameObject.activeInHierarchy) {
					inventory.addItemToCell (currItem, draggedItem.cell);
				}
				setItemToSlot (slot);
                updateAttributes ();
			}
        } else if (Utils.hit != null && Utils.hit.name.Equals("Supply")) {
			SupplySlot slot = Utils.hit.GetComponent<SupplySlot>();
			if (slot.item == null) {
				setItemToSlot(slot);
			} else {
				Item currItem = slot.takeItem();
				if (draggedItem.slot != null) {
					draggedItem.slot.setItem(currItem);
				} else if (inventory.gameObject.activeInHierarchy) {
					inventory.addItemToCell (currItem, draggedItem.cell);
				}
				setItemToSlot (slot);
			}
		} else if (draggedItem.cell == null && draggedItem.slot == null) {
			if (inventory.gameObject.activeInHierarchy) {
				inventory.addItemToCell (draggedItem, null);
			}
            updateAttributes();
		} else {
			draggedItem.returnToParent();
		}
	}

	private void setItemToSlot (Slot slot) {
		if (draggedItem.cell != null) {
			draggedItem.cell.inventory.calculateFreeVolume();
		}
		slot.setItem (draggedItem);
	}

	override protected void afterItemDrop () {
		if (draggedItem == null) {
			highlightSlot (false, ItemType.MATERIAL);//Material здесь вместо null, т.к. enum неможет в null
		}
	}

	override protected void choseItem (Item item) {
		base.choseItem(item);
		if (draggedItem != null) {
			highlightSlot (true, item.type);
		}
//		perksView.hideInfo();
	}

	private void highlightSlot (bool hightlight, ItemType itemType) {
		if (!hightlight) {
			foreach (Slot slot in allSlots) { slot.setActive(false); }
		} else {
			if (itemType == ItemType.SUPPLY) {
				foreach (Slot slot in supplySlots) { slot.setActive(true); }
			} else {
				foreach (EquipmentSlot slot in equipmentSlots) {
					if (slot.itemType == itemType) { slot.setActive (true); }
				}
			}
		}
	}

	public void sendToVars () {
//		playerData.sendToVars();
//		inventory.sendToVars();
	}

	public void initFromVars () {
//		playerData.initFromVars();
//		inventory.initFromVars();
//		inventory.setCapacity(shipData.hullType.getStorageCapacity());
	}
}
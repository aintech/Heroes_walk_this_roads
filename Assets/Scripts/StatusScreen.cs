using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StatusScreen : InventoryContainedScreen, Closeable {

	private ItemDescriptor itemDescriptor;

    private SpriteRenderer playerRender;

    public EquipmentSlot[] ringSlots { get; private set; }

    public List<Slot> allSlots { get; private set; }

    public List<EquipmentSlot> equipmentSlots { get; private set; }

    public List<SupplySlot> supplySlots { get; private set; }

	private List<HeroPortrait> portraits = new List<HeroPortrait>();

    private StrokeText healthValue, experienceValue, damageValue, armorValue, strengthValue, enduranceValue, agilityValue;

    private bool onTownMainScreen;

	public Hero chosenHero { get; private set; }

    private HeroType lastSelected = HeroType.ALIKA;

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

		Transform portraitsHolder = transform.Find("Portraits");
		portraitsHolder.gameObject.SetActive(true);
		for (int i = 0; i < portraitsHolder.childCount; i++) {
			portraits.Add(portraitsHolder.GetChild(i).GetComponent<HeroPortrait>().init(this));
		}

        gameObject.SetActive(false);

		return this;
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

	public void showScreen () {
		if (gameObject.activeInHierarchy) { return; }

        foreach (HeroPortrait port in portraits) {
            port.updateRepresentative();
        }

        itemDescriptor.setEnabled();

		inventory.setContainerScreen(this, 6);
		inventory.setInventoryToBegin ();

        chooseHero(lastSelected);
        updateAttributes();

		transform.position = Vector3.zero;

        Gameplay.topHideable.setVisible(false);

		InputProcessor.add(this);

        UserInterface.updateStatusBtnText(true);

		gameObject.SetActive(true);
	}

    public void updateAttributes () {
        healthValue.setText(chosenHero.health.ToString());// + (Player.health < Player.maxHealth? "/" + Player.maxHealth.ToString(): "");
        experienceValue.setText(((int)chosenHero.experience).ToString() + " %");
        damageValue.setText(chosenHero.damage().ToString());
        armorValue.setText(chosenHero.armorClass.ToString());
        strengthValue.setText(chosenHero.strength.ToString());
        enduranceValue.setText(chosenHero.endurance.ToString());
        agilityValue.setText(chosenHero.agility.ToString());
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

	public void chooseHero (HeroType type) {
		foreach (EquipmentSlot slot in equipmentSlots) {
			slot.hideItem();
		}
		foreach (HeroPortrait portrait in portraits) {
            portrait.setChosen(portrait.type == type);
		}
		chosenHero = Vars.heroes[type];

		if (chosenHero.weapon != null) { getEquipmentSlot(ItemType.WEAPON).setItem(chosenHero.weapon.item); }
		if (chosenHero.armor != null) { getEquipmentSlot(ItemType.ARMOR).setItem(chosenHero.armor.item); }
		if (chosenHero.helmet != null) { getEquipmentSlot(ItemType.HELMET).setItem(chosenHero.helmet.item); }
		if (chosenHero.shield != null) { getEquipmentSlot(ItemType.SHIELD).setItem(chosenHero.shield.item); }
		if (chosenHero.glove != null) { getEquipmentSlot(ItemType.GLOVE).setItem(chosenHero.glove.item); }
		if (chosenHero.amulet != null) { getEquipmentSlot(ItemType.AMULET).setItem(chosenHero.amulet.item); }
		if (chosenHero.ring_1 != null) { ringSlots[0].setItem(chosenHero.ring_1.item); }
		if (chosenHero.ring_1 != null) { ringSlots[1].setItem(chosenHero.ring_2.item); }

		playerRender.sprite = ImagesProvider.getHero(chosenHero.type);

        lastSelected = type;
		updateAttributes();
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
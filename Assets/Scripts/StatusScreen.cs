using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StatusScreen : InventoryContainedScreen, Closeable {

	public static StatusScreen instance { get; private set; }

    private SpriteRenderer playerRender;

    public EquipmentSlot[] ringSlots { get; private set; }

    public List<Slot> allSlots { get; private set; }

    public List<EquipmentSlot> equipmentSlots { get; private set; }

    public SupplySlot[] supplySlots { get; private set; }

	private List<HeroRepresentative> portraits = new List<HeroRepresentative>();

    private StrokeText healthValue, damageValue, armorValue, strengthValue, enduranceValue, agilityValue;

    private bool onTownMainScreen;

	public Hero chosenHero { get; private set; }

    private HeroType lastSelected = HeroType.ALIKA;

//    private Button inventoryBtn, perksBtn;
//
//    private PerksDisplay perksDisplay;

	public StatusScreen init () {
		instance = this;
		ItemDescriptor.instance.statusScreen = this;

		innerInit(transform.Find("Inventory").GetComponent<Inventory>().init(Inventory.InventoryType.INVENTORY), "Inventory");

//        perksDisplay = transform.Find("Perks Display").GetComponent<PerksDisplay>().init();

        allSlots = new List<Slot>();
        equipmentSlots = new List<EquipmentSlot>();
        supplySlots = new SupplySlot[6];

        Slot slot;
        Transform slotsContainer = transform.Find("Equipment Slots");
        slotsContainer.gameObject.SetActive(true);
        ringSlots = new EquipmentSlot[2];
        for (int i = 0; i < slotsContainer.childCount; i++) {
            slot = slotsContainer.GetChild(i).GetComponent<Slot>();
            slot.init();
            if (slot.itemType == ItemType.RING) {
                if (slot.transform.position.y > -1) {
                    ringSlots[0] = (EquipmentSlot)slot;
                } else {
                    ringSlots[1] = (EquipmentSlot)slot;
                }
            }
            equipmentSlots.Add((EquipmentSlot)slot);
            allSlots.Add(slot);
        }

        slotsContainer = transform.Find("Supply Slots");
        slotsContainer.gameObject.SetActive(true);
        for (int i = 0; i < slotsContainer.childCount; i++) {
            slot = slotsContainer.GetChild(i).GetComponent<Slot>();
            slot.init();
            supplySlots[slot.index] = (SupplySlot)slot;
            allSlots.Add(slot);
        }

        foreach (EquipmentSlot slt in equipmentSlots) { slt.statusScreen = this; }

        Transform attributes = transform.Find ("Attributes");
        attributes.gameObject.SetActive(true);

        string layerName = "Inventory";
        int layerOrder = 3;
        healthValue = attributes.Find ("Health").GetComponent<StrokeText> ().init(layerName, layerOrder);
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
			portraits.Add(portraitsHolder.GetChild(i).GetComponent<HeroRepresentative>().init("Inventory"));
		}

//        inventoryBtn = transform.Find("Inventory Button").GetComponent<Button>().init();
//        perksBtn = transform.Find("Perks Button").GetComponent<Button>().init();

        gameObject.SetActive(false);

		return this;
	}

    public EquipmentSlot getEquipmentSlot (ItemType type) {
        return getEquipmentSlot(type, 0);
    }

    public EquipmentSlot getEquipmentSlot (ItemType type, int index) {
        foreach (EquipmentSlot slot in equipmentSlots) {
            if (slot.itemType == type && slot.index == index) { return slot; }
        }
        Debug.Log("No slot for item type: " + type);
        return null;
    }

	public void showScreen () {
		if (gameObject.activeInHierarchy) { return; }

        foreach (HeroRepresentative port in portraits) {
            port.updateRepresentative();
            port.onHealModified();
        }

		ItemDescriptor.instance.setEnabled();

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
        healthValue.text = chosenHero.health.ToString();// + (Player.health < Player.maxHealth? "/" + Player.maxHealth.ToString(): "");
        damageValue.text = chosenHero.damage().ToString();
        armorValue.text = chosenHero.armorClass.ToString();
        strengthValue.text = chosenHero.strength.ToString();
        enduranceValue.text = chosenHero.endurance.ToString();
        agilityValue.text = chosenHero.agility.ToString();
    }

	public void close (bool byInputProcessor) {
		hideItemInfo();
		gameObject.SetActive(false);

        Gameplay.topHideable.setVisible(true);
//		UserInterface.showInterface = true;
        UserInterface.updateStatusBtnText(false);
		ItemDescriptor.instance.setDisabled();

		if (!byInputProcessor) { InputProcessor.removeLast(); }
	}

	protected override void checkBtnPress (Button btn) {
//        if (btn == inventoryBtn) {
//            inventory.gameObject.SetActive(true);
//            perksDisplay.gameObject.SetActive(false);
//            hideItemInfo();
//        } else if (btn == perksBtn) {
//            inventory.gameObject.SetActive(false);
//            perksDisplay.gameObject.SetActive(true);
//            hideItemInfo();
//        } else {
//            Debug.Log("Unknown button: " + btn.name);
//        }
    }

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
            bool appropriateType = false;
            if (slot.itemType == draggedItem.type) {
                if (draggedItem.type == ItemType.WEAPON) {
                    if (((WeaponData)draggedItem.itemData).type.user() == chosenHero.type) {
                        appropriateType = true;
                    }
                } else {appropriateType = true;}
            }

            if (!appropriateType) {
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
            if (draggedItem.type == ItemType.SUPPLY) {
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
            } else {
                draggedItem.returnToParent();
            }
        } else if (Utils.hit != null && Utils.hit.GetComponent<HeroRepresentative>() != null) {
            HeroRepresentative hero = Utils.hit.GetComponent<HeroRepresentative>();
            if (draggedItem.type == ItemType.SUPPLY) {
                SupplyData supply = (SupplyData)draggedItem.itemData;
                if (supply.type == SupplyType.HEALTH_POTION && hero.character.health < hero.character.maxHealth) {
                    hero.character.heal(supply.value);
                    supply.item.dispose();
                    draggedItem = null;
                } else {
                    draggedItem.returnToParent();
                }
            } else {
                draggedItem.returnToParent();
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
            highlightSlot (false, null);
		}
	}

	override protected void choseItem (Item item) {
		base.choseItem(item);
		if (draggedItem != null) {
			highlightSlot (true, item);
		}
//		perksView.hideInfo();
	}

	private void highlightSlot (bool hightlight, Item item) {
		if (!hightlight) {
			foreach (Slot slot in allSlots) { slot.setActive(false); }
		} else {
            if (item.type == ItemType.SUPPLY) {
				foreach (Slot slot in supplySlots) { slot.setActive(true); }
			} else {
				foreach (EquipmentSlot slot in equipmentSlots) {
                    if (slot.itemType == item.type) {
                        if (slot.itemType == ItemType.WEAPON) {
                            if (((WeaponData)item.itemData).type.user() == chosenHero.type) {
                                slot.setActive(true);
                            }
                        } else { slot.setActive(true); }
                    }
				}
			}
		}
	}

	public void chooseHero (HeroType type) {
        foreach (Slot slot in allSlots) {
			slot.hideItem();
		}
		foreach (HeroRepresentative portrait in portraits) {
            portrait.setChosen(portrait.type == type);
		}
		chosenHero = Vars.heroes[type];

        getEquipmentSlot(ItemType.WEAPON).checkVisible(chosenHero.type);

        if (chosenHero.weapon != null) { getEquipmentSlot(ItemType.WEAPON).setItemWithoutEquip(chosenHero.weapon.item); }
        if (chosenHero.armor != null) { getEquipmentSlot(ItemType.ARMOR).setItemWithoutEquip(chosenHero.armor.item); }
        //		if (chosenHero.helmet != null) { getEquipmentSlot(ItemType.HELMET).setItemWithoutEquip(chosenHero.helmet.item); }
        //		if (chosenHero.shield != null) { getEquipmentSlot(ItemType.SHIELD).setItemWithoutEquip(chosenHero.shield.item); }
        //		if (chosenHero.glove != null) { getEquipmentSlot(ItemType.GLOVE).setItemsetItemWithoutEquipchosenHero.glove.item); }
        if (chosenHero.amulet != null) { getEquipmentSlot(ItemType.AMULET).setItemWithoutEquip(chosenHero.amulet.item); }
        if (chosenHero.ring_1 != null) { ringSlots[0].setItemWithoutEquip(chosenHero.ring_1.item); }
        if (chosenHero.ring_2 != null) { ringSlots[1].setItemWithoutEquip(chosenHero.ring_2.item); }

        for (int i = 0; i < supplySlots.Length; i++) {
            if (chosenHero.supplies[i] != null) {
                supplySlots[i].setItem(chosenHero.supplies[i].item);
            }
        }

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
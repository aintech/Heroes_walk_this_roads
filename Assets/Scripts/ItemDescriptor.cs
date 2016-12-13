using UnityEngine;
using System.Collections;

public class ItemDescriptor : MonoBehaviour {

	private const float fontLengthMulty = 4.9f;

	private Transform 	trans,
						namePre, nameBG,
						qualityPre, qualityBG,
						pre1, bg1,
						pre2, bg2,
						pre3, bg3,
						pre4, bg4,
						pre5, bg5;

	public bool onScreen { get; private set; }

	private ItemHolder holder, tempHolder;

	private Item item;

	private TextMesh qualityValue, nameValue, value1, value2, value3, value4, value5;

	private MeshRenderer qualityRender, nameRender, value1Render, value2Render,value3Render, value4Render, value5Render;

	private Vector3 pos = Vector3.zero, spaceOffset = Vector3.zero;

	private Color32 goodColor = new Color32(176, 195, 217, 255),
					superiorColor = new Color32(94, 152, 217, 255),
					rareColor = new Color32(136, 71, 255, 255),
					uniqueColor = new Color32(173, 229, 92, 255),
					artefactColor = new Color32(235, 75, 75, 255);

	private Vector3 scale = Vector3.one;

	private float minY = -10, maxX = 10, screenWidth;

    private Type inventoryType = Type.NONE;

    private Market buyMarket, sellMarket;

	public FightScreen fightScreen;

    public StatusScreen statusScreen;

	public ItemDescriptor init () {
		trans = transform.Find ("Descriptor");
		qualityPre = trans.Find ("Quality Pre");
		qualityBG = trans.Find("Quality Background");
		namePre = trans.Find ("Name Pre");
		nameBG = trans.Find("Name Background");
		pre1 = trans.Find ("Pre 1");
		bg1 = trans.Find ("Background 1");
		pre2 = trans.Find ("Pre 2");
		bg2 = trans.Find ("Background 2");
		pre3 = trans.Find ("Pre 3");
		bg3 = trans.Find ("Background 3");
		pre4 = trans.Find ("Pre 4");
		bg4 = trans.Find ("Background 4");
		pre5 = trans.Find ("Pre 5");
		bg5 = trans.Find ("Background 5");

		qualityValue = trans.Find("Quality Value").GetComponent<TextMesh>();
		nameValue = trans.Find("Name Value").GetComponent<TextMesh>();
		value1 = trans.Find ("Value 1").GetComponent<TextMesh> ();
		value2 = trans.Find ("Value 2").GetComponent<TextMesh> ();
		value3 = trans.Find ("Value 3").GetComponent<TextMesh> ();
		value4 = trans.Find ("Value 4").GetComponent<TextMesh> ();
		value5 = trans.Find ("Value 5").GetComponent<TextMesh> ();

		qualityRender = qualityValue.GetComponent<MeshRenderer>();
		nameRender = nameValue.GetComponent<MeshRenderer>();
		value1Render = value1.GetComponent<MeshRenderer>();
		value2Render = value2.GetComponent<MeshRenderer>();
		value3Render = value3.GetComponent<MeshRenderer>();
		value4Render = value4.GetComponent<MeshRenderer>();
		value5Render = value5.GetComponent<MeshRenderer>();

		MeshRenderer mesh;
		for (int i = 0; i < trans.childCount; i++) {
			mesh = trans.GetChild (i).GetComponent<MeshRenderer> ();
			if (mesh != null) {
				mesh.sortingLayerName = "User Interface";
				mesh.sortingOrder = 9;
			}
		}
		screenWidth = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height)).x;

        Transform market = GameObject.Find("Town").transform.Find("Market");
        buyMarket = market.Find("Buy").GetComponent<Market>();
        sellMarket = market.Find("Sell").GetComponent<Market>();

		hide ();

		return this;
	}

	public void setSpaceOffset (Vector3 spaceOffset) {
		this.spaceOffset = spaceOffset;
	}

    public void setEnabled () {
        enabled = true;
    }

	public void setDisabled () {
		enabled = false;
		hide();
	}

	void Update () {
		if (Input.GetMouseButtonDown(1)) {
			passRightClick();
		}
		if (onScreen) {
			if (Utils.hit == null) {
				hide ();
			} else if (Utils.hit != null) {
				tempHolder = Utils.hit.GetComponent<ItemHolder> ();
				if (tempHolder == null || tempHolder.item == null) {
					hide ();
				} else if (tempHolder != holder || tempHolder.item != item) {
					showDescription (tempHolder);
				}
			}
			pos = Utils.mousePos;
			if (pos.y < minY + spaceOffset.y) { pos.y = minY + spaceOffset.y; }
			if (pos.x > maxX + spaceOffset.x) { pos.x = maxX + spaceOffset.x; }
			trans.localPosition = pos;
		} else {
			if (Utils.hit != null) {
				holder = Utils.hit.GetComponent<ItemHolder>();
				if (holder != null && holder.item != null) {
					showDescription(holder);
				}
			}
		}
	}

	private float calcMeshLength (MeshRenderer mesh) {
		return mesh.bounds.size.x * fontLengthMulty;
	}

	private void showDescription (ItemHolder holder) {
		this.holder = holder;
		this.item = holder.item;

        if (item.cell != null) {
//        if (holder is InventoryCell) {
            switch (item.cell.inventory.inventoryType) {
                case Inventory.InventoryType.INVENTORY:
                    inventoryType = Type.INVENTORY;
                    break;
                case Inventory.InventoryType.MARKET_BUY:
                    inventoryType = Type.BUY;
                    break;
                case Inventory.InventoryType.MARKET_SELL:
                    inventoryType = Type.SELL;
                    break;
            }
        } else {
            inventoryType = fightScreen.gameObject.activeInHierarchy? Type.FIGHT: Type.NONE;
        }

		for (int i = 0; i < trans.childCount; i++) {
			trans.GetChild (i).gameObject.SetActive (false);
		}

		showTexts (item.itemData);

		onScreen = true;
		Update();
		trans.gameObject.SetActive(true);
	}

	private void passRightClick () {
        switch (inventoryType) {
            case Type.BUY:
                buyMarket.askToBuy(holder);
                break;
            case Type.SELL:
                sellMarket.askToSell(holder);
                break;
            case Type.FIGHT:
                if (holder != null && holder is SupplySlot) {
                    fightScreen.useSupply((SupplySlot)holder);
                }
                break;
            case Type.INVENTORY:
                if (holder != null && holder is InventoryCell && holder.item != null) {
                    if (holder.item.type == ItemType.SUPPLY) {
                        for (int i = 0; i < statusScreen.supplySlots.Count; i++) {
                            if (statusScreen.getSupplySlot(i).item == null) {
                                statusScreen.getSupplySlot(i).setItem(holder.takeItem());
                                break;
                            }
                        }
                    } else if (holder.item.type == ItemType.RING) {
                        if (statusScreen.ringSlots[0].item == null) {
                            statusScreen.ringSlots[0].setItem(holder.takeItem());
                        } else if (statusScreen.ringSlots[1].item == null) {
                            statusScreen.ringSlots[1].setItem(holder.takeItem());
                        } else {
                            Item item = statusScreen.ringSlots[0].takeItem();
                            statusScreen.ringSlots[0].setItem(holder.takeItem());
                            statusScreen.inventory.addItemToCell(item, null);
                        }
                    } else {
                        EquipmentSlot slot = statusScreen.getEquipmentSlot(holder.item.type);
                        if (slot.item == null) {
                            slot.setItem(holder.takeItem());
                        } else {
                            Item item = slot.takeItem();
                            slot.setItem(holder.takeItem());
                            statusScreen.inventory.addItemToCell(item, null);
                        }
                    }
                }
                break;
        }
	}

	private float setCost(int index, ItemData data) {
		string text = (inventoryType == Type.BUY? "Купить за": inventoryType == Type.SELL? "Продать за": "Стоимость:")  + " <color=yellow>" + data.cost +
					  (data.quantity == 1? "$</color>": (" (" + (data.quantity * data.cost) + ")$</color>"));
		minY = - 4.7f + (.4f * index);
		switch (index) {
			case 1: value1.text = text; scale.x = calcMeshLength(value1Render); bg1.localScale = scale; return value1Render.bounds.size.x;
			case 2: value2.text = text; scale.x = calcMeshLength(value2Render); bg2.localScale = scale; return value2Render.bounds.size.x;
			case 3: value3.text = text; scale.x = calcMeshLength(value3Render); bg3.localScale = scale; return value3Render.bounds.size.x;
			case 4: value4.text = text; scale.x = calcMeshLength(value4Render); bg4.localScale = scale; return value4Render.bounds.size.x;
			case 5: value5.text = text; scale.x = calcMeshLength(value5Render); bg5.localScale = scale; return value5Render.bounds.size.x;
			default: Debug.Log("Unknown index: " + index); return 0;
		}
	}

	private void showTexts (ItemData data) {
		float maxLength = 0;

		if (data.quality != ItemQuality.COMMON) {
			qualityPre.gameObject.SetActive (true);
			qualityBG.gameObject.SetActive (true);
			qualityValue.gameObject.SetActive (true);

			qualityValue.text = item.quality.getName();
			scale.x = calcMeshLength(qualityRender);// qualityValue.text.Length + 1;
			qualityBG.localScale = scale;
			qualityValue.color = (data.quality == ItemQuality.ARTEFACT? artefactColor:
								  data.quality == ItemQuality.UNIQUE? uniqueColor: 
								  data.quality == ItemQuality.RARE? rareColor:
								  data.quality == ItemQuality.SUPERIOR? superiorColor:
								  goodColor);
			maxLength = qualityRender.bounds.size.x;
		}

		namePre.gameObject.SetActive (true);
		nameBG.gameObject.SetActive (true);
		nameValue.gameObject.SetActive (true);

		nameValue.text = data.name;
		scale.x = calcMeshLength(nameRender);// nameValue.text.Length + 1;
		nameBG.localScale = scale;
		maxLength = Mathf.Max(maxLength, nameRender.bounds.size.x);

		switch (data.itemType) {
			case ItemType.SUPPLY:
				pre1.gameObject.SetActive (true);
				bg1.gameObject.SetActive (true);
				value1.gameObject.SetActive (true);
				pre2.gameObject.SetActive (true);
				bg2.gameObject.SetActive (true);
				value2.gameObject.SetActive (true);

				SupplyData sud = (SupplyData)data;
				string val = "";
				switch (sud.type) {
                    case SupplyType.HEALTH_POTION:
						val = "Восстанавливает <color=white>" + sud.value + "</color> HP";
						break;
                    case SupplyType.BLINDING_POWDER: val = StatusEffectType.BLINDED.name() + " на <color=white>" + sud.duration + "</color> ходов"; break;
                    case SupplyType.PARALIZING_DUST: val = StatusEffectType.PARALIZED.name() + " на <color=white>" + sud.duration + "</color> ходов"; break;
                    case SupplyType.SPEED_POTION: val = "Дополнительно <color=white>" + sud.value + "</color> действий на <color=white>" + sud.duration + "</color> ходов"; break;
                    case SupplyType.REGENERATION_POTION: val = "Восстановление по <color=white>" + sud.value + "</color> HP в течении <color=white>" + sud.duration + "</color> ходов"; break;
                    case SupplyType.ARMOR_POTION: val = "Повышение защиты на <color=white>" + sud.value + "</color> в течении <color=white>" + sud.duration + "</color> ходов"; break;
					default: Debug.Log("Unknown supply type: " + sud.type); val = ""; break;
				}
				value1.text = "Эффект: <color=orange>" + val + "</color>";
				scale.x = calcMeshLength(value1Render);// value1Render.bounds.size.x * 4.8f;// value1.text.Length - 23;// - (количество спецсимволов)
				bg1.localScale = scale;
				maxLength = Mathf.Max(maxLength, value1Render.bounds.size.x);

				maxLength = Mathf.Max(maxLength, setCost(2, data));
				break;

            case ItemType.MATERIAL:
				pre1.gameObject.SetActive (true);
				bg1.gameObject.SetActive (true);
				value1.gameObject.SetActive (true);

				maxLength = Mathf.Max(maxLength, setCost(1, data));
				break;

            case ItemType.AMULET:
            case ItemType.RING:
                pre1.gameObject.SetActive (true);
                bg1.gameObject.SetActive (true);
                value1.gameObject.SetActive (true);

                maxLength = Mathf.Max(maxLength, setCost(1, data));
                break;
                
            case ItemType.WEAPON:
				pre1.gameObject.SetActive (true);
				bg1.gameObject.SetActive (true);
				value1.gameObject.SetActive (true);
				pre2.gameObject.SetActive (true);
				bg2.gameObject.SetActive (true);
				value2.gameObject.SetActive (true);

				WeaponData wd = (WeaponData)data;
                value1.text = "Урон: <color=orange>" + wd.damage + "</color>";
				scale.x = calcMeshLength(value1Render);//value1.text.Length - 24;// - (количество спецсимволов)
				bg1.localScale = scale;
				maxLength = Mathf.Max(maxLength, value1Render.bounds.size.x);

				maxLength = Mathf.Max(maxLength, setCost(2, data));
				break;

			case ItemType.ARMOR:
            case ItemType.SHIELD:
            case ItemType.GLOVE:
            case ItemType.HELMET:
				pre1.gameObject.SetActive (true);
				bg1.gameObject.SetActive (true);
				value1.gameObject.SetActive (true);
				pre2.gameObject.SetActive (true);
				bg2.gameObject.SetActive (true);
				value2.gameObject.SetActive (true);

                ArmorModifier am = (ArmorModifier)data;
                value1.text = "Защита: <color=orange>" + am.armorClass() + "</color>";
				scale.x = calcMeshLength(value1Render);//value1.text.Length - 22;
				bg1.localScale = scale;
				maxLength = Mathf.Max(maxLength, value1Render.bounds.size.x);

				maxLength = Mathf.Max(maxLength, setCost(2, data));
				break;
		}

		maxX = screenWidth - maxLength - .5f;
	}

	private void hide () {
		onScreen = false;
		item = null;
		trans.gameObject.SetActive (false);
	}

	public enum Type {
		NONE, INVENTORY, SELL, BUY, FIGHT
	}
}
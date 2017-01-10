using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemDescriptor : MonoBehaviour {
    
    public static ItemDescriptor instance { get; private set; }

    public Transform descriptionLinePrefab;

    private List<DescriptionLine> descriptionLines = new List<DescriptionLine>();

    private const float fontLengthMulty = 4.9f;

    private Transform holder;

    public bool onScreen { get; private set; }

    private Describeable describeable, tempDescribe;

    private Vector3 pos = Vector3.zero;

    private Vector3 scale = Vector3.one;

    private float minY = -10, maxX = 10, screenWidth;

    private Type inventoryType = Type.NONE;

    private Market buyMarket, sellMarket;

    [HideInInspector]
    public StatusScreen statusScreen;

    private long descriptionId = Vars.describeableId - 1;

    public ItemDescriptor init () {
        instance = this;

        holder = transform.Find ("Holder");

        screenWidth = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height)).x;

        Transform market = GameObject.Find("Town").transform.Find("Market");
        buyMarket = market.Find("Buy").GetComponent<Market>();
        sellMarket = market.Find("Sell").GetComponent<Market>();

        hide ();

        return this;
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
        } else {
            if (onScreen) {
                if (Utils.hit == null) {
                    hide ();
                } else {
                    tempDescribe = Utils.hit.GetComponent<Describeable> ();
                    if (tempDescribe == null || tempDescribe.descriptionId() == -1) {
                        hide ();
                    } else if (tempDescribe.descriptionId() != descriptionId) {
                        showDescription (tempDescribe);
                    }
                }
                pos = Utils.mousePos;
                if (pos.y < minY) { pos.y = minY; }
                if (pos.x > maxX) { pos.x = maxX; }
                holder.localPosition = pos;
            } else {
                if (Utils.hit != null) {
                    describeable = Utils.hit.GetComponent<Describeable>();
                    if (describeable != null) {
                        showDescription(describeable);
                    }
                }
            }
        }
    }

    private void showDescription (Describeable describeable) {
        if (describeable.descriptionId() == -1) { return; }
        this.describeable = describeable;
        descriptionId = describeable.descriptionId();

        while (descriptionLines.Count < describeable.description().Count + 1) {
            descriptionLines.Add(Instantiate<Transform>(descriptionLinePrefab).GetComponent<DescriptionLine>().init(holder, descriptionLines.Count));
        }

        foreach (DescriptionLine line in descriptionLines) {
            line.hide();
        }

        float maxLength = 0;
        float textLength = 0;
        for (int i = 0; i < describeable.description().Count; i++) {
            textLength = descriptionLines[i].setText(describeable.description()[i]);
            maxLength = Mathf.Max(maxLength, textLength);
        }
        if (describeable is ItemHolder) {
            Item item = ((ItemHolder) describeable).item;
            if (item.cell != null) {
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
                inventoryType = FightScreen.instance.gameObject.activeInHierarchy? Type.FIGHT: Type.NONE;
            }
            textLength = setCost(item.itemData);
            maxLength = Mathf.Max(maxLength, textLength);
        }

        maxX = screenWidth - maxLength - .5f;
        minY = (.4f * (describeable.description().Count - (describeable is ItemHolder? 0: 1))) - 4.7f;

        onScreen = true;
        Update();
        holder.gameObject.SetActive(true);
    }

    private float setCost(ItemData data) {
        string text = (inventoryType == Type.BUY? "Купить за": inventoryType == Type.SELL? "Продать за": "Стоимость:")
            + " <color=yellow>" + data.cost + (data.quantity == 1? "$</color>": (" (" + (data.quantity * data.cost) + ")$</color>"));
        return descriptionLines[describeable.description().Count].setText(text);
    }

    private void passRightClick () {
        ItemHolder holder = (ItemHolder)describeable;
        switch (inventoryType) {
            case Type.BUY:
                buyMarket.askToBuy(holder);
                break;
            case Type.SELL:
                sellMarket.askToSell(holder);
                break;
            case Type.FIGHT:
                if (holder != null && holder is SupplySlot) {
                    FightScreen.instance.useSupply((SupplySlot)holder);
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

    private void hide () {
        onScreen = false;
        describeable = null;
        holder.gameObject.SetActive (false);
    }

    public void recheckValues (Describeable describeable) {
        if (this.describeable != null && this.describeable.descriptionId() == describeable.descriptionId()) {
            showDescription(describeable);
        }
    }

    public enum Type {
        NONE, INVENTORY, SELL, BUY, FIGHT
    }
}
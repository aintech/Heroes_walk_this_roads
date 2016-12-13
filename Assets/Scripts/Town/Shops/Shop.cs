using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Shop : MonoBehaviour, ButtonHolder, PopupListener, Closeable {

    public ShopType shopType;

    public ItemType[] miscItemsTypes;

    public static bool reloadItems = true;

    private List<ShopItemHolder> itemHolders = new List<ShopItemHolder>(), miscHolders;

    private Button closeBtn;

    private ShopScreen parentScreen;

    private ShopItemHolder currHolder;

    private QuantityPopup popup;

    private Inventory inverntory;

    public Shop init(ShopScreen parentScreen) {
        this.parentScreen = parentScreen;
        closeBtn = transform.Find("Close Button").GetComponent<Button>().init();
        ShopItemHolder holder;
        for (int i = 0; i < transform.childCount; i++) {
            holder = transform.GetChild(i).GetComponent<ShopItemHolder>();
            if (holder != null) {
                holder.init();
                itemHolders.Add(holder);
            }
        }
        Transform miscItemsTrans = transform.Find("MiscItems");
        if (miscItemsTrans != null) {
            miscHolders = new List<ShopItemHolder>();
            for (int i = 0; i < miscItemsTrans.childCount; i++) {
                holder = miscItemsTrans.GetChild(i).GetComponent<ShopItemHolder>();
                holder.init();
                holder.gameObject.SetActive(false);
                miscHolders.Add(holder);
            }
        }
        inverntory = Vars.gameplay.statusScreen.inventory;
        return this;
    }

    public void askToBuy (ShopItemHolder holder) {
        if (holder.item.quantity == 1) { buyItem(holder, 1); }
        else { popup.show(this, holder); }
    }

    public void checkPopupResult (ItemHolder holder, int value) {
        buyItem((ShopItemHolder)holder, value);
    }

    public void buyItem (ShopItemHolder holder, int quantity) {
        if (Vars.gold < (holder.item.cost * quantity)) { Messenger.notEnoughtCash(holder.item.itemName, quantity); return; }
        if (holder.item.volume > .001f && (inverntory.getFreeVolume() - (holder.item.volume * quantity)) < 0) { Messenger.showMessage("Недостаточно места в инвентаре."); return; }
        Vars.gold -= (holder.item.cost * quantity);
        UserInterface.updateGold();

        if (holder.item.quantity == quantity) {
            inverntory.addItemToCell(Instantiate<Transform>(ItemFactory.itemPrefab).GetComponent<Item>().init(holder.item.itemData), null);
            holder.hided = true;
        } else {
            Item buyed = Instantiate<Transform>(ItemFactory.itemPrefab).GetComponent<Item>();
            buyed.init(DataCopier.copy(holder.item.itemData));
            buyed.quantity = quantity;
            inverntory.addItemToCell(buyed, null);
            holder.item.quantity -= quantity;
        }
    }

    public void fireClickButton (Button btn) {
        if (btn == closeBtn) { close(false); }
    }

//    void Update() {
//        if (Utils.hit != null && Utils.hit.GetComponent<ShopItem>() != null) {
//            if (currItem == null) {
//                currItem = Utils.hit.GetComponent<ShopItem>();
//				itemDescriptor.showDescription(currItem.getItem());
//            } else if (currItem != Util.hit.GetComponent<ShopItem>()) {
//                currItem = Util.hit.GetComponent<ShopItem>();
//                itemDescriptor.showDescription(currItem.getItem());
//            } else if (!itemDescriptor.isOnScreen()) {
//                currItem = Util.hit.GetComponent<ShopItem>();
//				itemDescriptor.showDescription(currItem.getItem());
//            }
//        } else if (currItem != null) {
//            itemDescriptor.hideDescription();
//            currItem = null;
//        }
//        if (currItem != null && Input.GetMouseButtonDown(1)) {
//            currItem.buyItem();
//            itemDescriptor.hideDescription();
//            currItem = null;
//        }
//    }

    public void showShop() {
        Vars.gameplay.itemDescriptor.setEnabled();
        if (reloadItems) {
            foreach (ShopItemHolder item in itemHolders) {
                item.initNewItem();
            }
            if (miscHolders != null) {
                ItemType type;
                Vector2 pos = Vector2.zero;
                foreach (ShopItemHolder holder in miscHolders) {
                    holder.hided = true;
                }
                for (int i = 0; i < 5; i++) {
                    type = miscItemsTypes[Random.Range(0, miscItemsTypes.Length)];
                    foreach (ShopItemHolder holder in miscHolders) {
                        if (holder.hided && (holder.itemType == type)) {
                            holder.initNewItem();
                            pos.x = -6 + (i * 3);
                            holder.transform.localPosition = pos;
                            break;
                        }
                    }
                }
            }
        }
        InputProcessor.add(this);
        UserInterface.setEquipmentBtnActive(false);
        gameObject.SetActive(true);
    }

	public void setItemHoldersActive (bool active) {
        foreach (ShopItemHolder holder in itemHolders) {
            holder.setActive(active);
        }
        if (miscHolders != null) {
            foreach (ShopItemHolder holder in miscHolders) {
                holder.setActive(active);
            }
        }
		closeBtn.setActive(active);
    }

    public void close (bool byInputProcessor) {
        currHolder = null;
        parentScreen.closeShop();
        gameObject.SetActive(false);
        UserInterface.setEquipmentBtnActive(true);
        if (!byInputProcessor) { InputProcessor.removeLast(); }
    }
}

public enum ShopType {
    WEAPON, ARMOR, POTION
}
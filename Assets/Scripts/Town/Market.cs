using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Market : InventoryContainedScreen, PopupListener {

    public Inventory.InventoryType type;

    private Inventory sellInventory;

	private QuantityPopup popup;

    public Market init (QuantityPopup popup) {
        this.popup = popup;

        if (type == Inventory.InventoryType.MARKET_SELL) {
            sellInventory = inventory;
        } else {
            sellInventory = transform.parent.Find("Sell").GetComponent<Market>().inventory;
        }

        innerInit(transform.Find("Inventory").GetComponent<Inventory>().init(type), "Inventory");
        inventory.setContainerScreen(this, 4);

		for (int i = 0; i < transform.childCount; i++) {
			transform.GetChild(i).gameObject.SetActive(true);
		}

		return this;
	}

	public void askToBuy (ItemHolder holder) {
		if (holder.item == null) { return; }
		if (holder.item.quantity == 1) { buyItem(holder, 1); }
        else { popup.show(this, holder); }
	}

	public void buyItem (ItemHolder holder, int quantity) {
        if (Vars.gold < (holder.item.cost * quantity)) { Messenger.notEnoughtCash(holder.item.itemName, quantity); return; }
        if (holder.item.volume > .001f && (sellInventory.getFreeVolume() - (holder.item.volume * quantity)) < 0) { Messenger.showMessage("Недостаточно места в инвентаре."); return; }

        Vars.gold -= (holder.item.cost * quantity);
        UserInterface.updateGold();

		if (holder.item.quantity == quantity) {
            sellInventory.addItemToCell(holder.item.cell.takeItem(), null);
		} else {
			Item buyed = Instantiate<Transform>(ItemFactory.itemPrefab).GetComponent<Item>();
			buyed.init(DataCopier.copy(holder.item.itemData));
			buyed.quantity = quantity;
            sellInventory.addItemToCell(buyed, null);
			holder.item.quantity -= quantity;
		}
	}

    public void askToSell (ItemHolder holder) {
        if (holder.item == null) { return; }
        if (holder.item.quantity == 1) { sellItem(holder, 1); }
        else { popup.show(this, holder); }
    }

    public void sellItem (ItemHolder holder, int quantity) {
        Vars.gold += (holder.item.cost * quantity);
        UserInterface.updateGold();

        if (holder.item.quantity == quantity) {
            holder.item.cell.takeItem().dispose();
        } else {
            holder.item.quantity -= quantity;
        }
    }

    public void checkPopupResult (ItemHolder holder, int value) {
        InventoryCell cell = (InventoryCell)holder;
        switch (cell.inventory.inventoryType) {
            case Inventory.InventoryType.MARKET_BUY:
                buyItem(holder, value);
                break;
            case Inventory.InventoryType.MARKET_SELL:
                sellItem(holder, value);
                break;
            default:
                Debug.Log("Wrong inventory type: " + cell.inventory.inventoryType);
                break;
        }
    }

	override protected void checkBtnPress (Button btn) {
	}

    public void hide () {
        hideItemInfo();
    }

//	override protected void checkItemDrop () {
//		if (Utils.hit != null && Utils.hit.name.Equals("Cell")) {
//			InventoryCell cell = Utils.hit.transform.GetComponent<InventoryCell>();
//			Inventory source = draggedItem.cell.getInventory();
//			Inventory target = cell.getInventory();
//			
//			if (source != target && (source == inventory || source == storage) && target == market) {
//				target.sellItemToTrader(draggedItem, buyback);
//				hideItemInfo(null);
//			} else {
//				target.addItemToCell(draggedItem, cell);
//			}
//		} else {
//			draggedItem.returnToParent();
//		}
//	}

	private void setBuyActive () {
//		bgRender.sprite = buyBG;
//		innerInit(buyMarket, "default");
//		actionMsg.text = "<color=orange>Покупка</color> - правая кнопка мыши.";
//		itemDescriptor.setEnabled(ItemDescriptor.Type.MARKET_BUY, this);
//		buyBtn.setActive(false);
//		sellBtn.setActive(true);
//		buyMarket.refreshInventory();
//		sellMarket.gameObject.SetActive(false);
//		buyMarket.gameObject.SetActive(true);
	}
	
	private void setSellActive () {
//		bgRender.sprite = sellBG;
//		innerInit(sellMarket, "default");
//		actionMsg.text = "<color=orange>Продажа</color> - правая кнопка мыши.";
//		itemDescriptor.setEnabled(ItemDescriptor.Type.MARKET_SELL, this);
//		buyBtn.setActive(true);
//		sellBtn.setActive(false);
//		sellMarket.refreshInventory();
//		buyMarket.gameObject.SetActive(false);
//		sellMarket.gameObject.SetActive(true);
	}
}
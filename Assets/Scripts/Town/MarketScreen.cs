using UnityEngine;
using System.Collections;

public class MarketScreen : TownScreen {

    private Market buy, sell;

    public override TownScreen init (Town town) {
        this.town = town;
        innerInit(Town.ScreenType.MARKET);

        QuantityPopup popup = GameObject.Find("Commons").transform.Find("Quantity Popup").GetComponent<QuantityPopup>();

        sell = transform.Find("Sell").GetComponent<Market>().init(popup);
        buy = transform.Find("Buy").GetComponent<Market>().init(popup);

        return this;
    }

    public override void beforeShow () {
        UserInterface.setEquipmentBtnActive(false);

        if (buy.inventory.getItems().Count == 0) {
            buy.inventory.fillWithRandomItems();
        }

        buy.inventory.setInventoryToBegin ();
        sell.inventory.setInventoryToBegin ();

        buy.inventory.sortInventory();
		sell.inventory.setItemsFromOtherInventory(StatusScreen.instance.inventory);

        InputProcessor.add(this);

        gameObject.SetActive (true);

		ItemDescriptor2.instance.setEnabled();

        Messenger.showMessage("Правая кнопка мыши - купить или продать предмет.");
    }

    public override void beforeClose () {
        buy.hide();
        sell.hide();
		StatusScreen.instance.inventory.setItemsFromOtherInventory(sell.inventory);
		ItemDescriptor2.instance.setDisabled();
        UserInterface.setEquipmentBtnActive(true);
    }

    public override void fireButton (Button btn) {}
}
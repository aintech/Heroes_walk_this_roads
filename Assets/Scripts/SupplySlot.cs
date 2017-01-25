using UnityEngine;
using System.Collections;

public class SupplySlot : Slot {
    
    public override Item takeItem () {
        StatusScreen.instance.chosenHero.supplies[index] = null;
        return base.takeItem();
    }

    public override void setItem (Item item) {
        if (!item.gameObject.activeInHierarchy) { item.gameObject.SetActive(true); }
        StatusScreen.instance.chosenHero.supplies[index] = (SupplyData)item.itemData;
        base.setItem(item);
    }

    public override void hideItem () {
        if (item != null) {
            Item temp = base.takeItem();
            temp.gameObject.SetActive(false);
        }
    }
}
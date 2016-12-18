using UnityEngine;
using System.Collections;

public class EquipmentSlot : Slot {
	
    [HideInInspector]
    public StatusScreen statusScreen;

	public override void setItem (Item item) {
		if (!item.gameObject.activeInHierarchy) { item.gameObject.SetActive(true); }
		if (item.type == ItemType.WEAPON) {
			statusScreen.chosenHero.equipWeapon((WeaponData)item.itemData);
		} else if (item.itemData is ArmorModifier) {
			statusScreen.chosenHero.equipArmor((ArmorModifier)item.itemData);
		}
		base.setItem (item);
        statusScreen.updateAttributes();
	}

	public override Item takeItem () {
		if (item.type == ItemType.WEAPON) {
			statusScreen.chosenHero.equipWeapon(null);
		} else if (item.itemData is ArmorModifier) {
			statusScreen.chosenHero.unEquipArmor((ArmorModifier)item.itemData);
		}
        Item tempItem = base.takeItem();
        statusScreen.updateAttributes();
        return tempItem;
	}

	public void hideItem () {
		if (item != null) {
			Item temp = base.takeItem();
			temp.gameObject.SetActive(false);
		}
	}
}
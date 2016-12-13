using UnityEngine;
using System.Collections;

public class EquipmentSlot : Slot {
	
    [HideInInspector]
    public StatusScreen statusScreen;

	public override void setItem (Item item) {
		if (item.type == ItemType.WEAPON) {
			Player.equipWeapon((WeaponData)item.itemData);
		} else if (item.itemData is ArmorModifier) {
			Player.equipArmor((ArmorModifier)item.itemData);
		}
		base.setItem (item);
        statusScreen.updateAttributes();
	}

	public override Item takeItem () {
		if (item.type == ItemType.WEAPON) {
			Player.equipWeapon(null);
		} else if (item.itemData is ArmorModifier) {
			Player.unEquipArmor((ArmorModifier)item.itemData);
		}
        Item tempItem = base.takeItem();
        statusScreen.updateAttributes();
        return tempItem;
	}
}
using UnityEngine;
using System.Collections;

public class EquipmentSlot : Slot {
	
    [HideInInspector]
    public StatusScreen statusScreen;

    public int index;

    public Sprite swordIcon, daggersIcon, staffIcon, wandIcon;

    public void checkVisible (HeroType heroType) {
        iconRender.sprite = heroType == HeroType.ALIKA? swordIcon:
                            heroType == HeroType.LIARA? wandIcon:
                            heroType == HeroType.KATE? daggersIcon:
                            heroType == HeroType.VICTORIA? staffIcon:
                            null;
    }

    public void setItemWithoutEquip (Item item) {
        if (!item.gameObject.activeInHierarchy) { item.gameObject.SetActive(true); }
        base.setItem(item);
        statusScreen.updateAttributes();
    }

	public override void setItem (Item item) {
		if (!item.gameObject.activeInHierarchy) { item.gameObject.SetActive(true); }
        switch (item.type) {
            case ItemType.WEAPON: statusScreen.chosenHero.equipWeapon((WeaponData)item.itemData); break;
            case ItemType.ARMOR: statusScreen.chosenHero.equipArmor((ArmorData)item.itemData); break;
            case ItemType.AMULET: statusScreen.chosenHero.equipAmulet((AmuletData)item.itemData); break;
            case ItemType.RING: statusScreen.chosenHero.equipRing((RingData)item.itemData, index); break;
        }
//		if (item.type == ItemType.WEAPON) {
//			statusScreen.chosenHero.equipWeapon((WeaponData)item.itemData);
//        } else if (item.itemData is ArmorModifier) {
//			statusScreen.chosenHero.equipArmor((ArmorModifier)item.itemData);
//		}
		base.setItem (item);
        statusScreen.updateAttributes();
	}

	public override Item takeItem () {
        switch (item.type) {
            case ItemType.WEAPON: statusScreen.chosenHero.equipWeapon(null); break;
            case ItemType.ARMOR: statusScreen.chosenHero.unEquipArmor((ArmorData)item.itemData); break;
            case ItemType.AMULET: statusScreen.chosenHero.equipAmulet(null); break;
            case ItemType.RING: statusScreen.chosenHero.equipRing(null, index); break;
        }
//		if (item.type == ItemType.WEAPON) {
//			statusScreen.chosenHero.equipWeapon(null);
//		} else if (item.itemData is ArmorModifier) {
//			statusScreen.chosenHero.unEquipArmor((ArmorModifier)item.itemData);
//		}
        Item tempItem = base.takeItem();
        statusScreen.updateAttributes();
        return tempItem;
	}

    public override void hideItem () {
        if (item != null) {
            Item temp = base.takeItem();
            temp.gameObject.SetActive(false);
        }
    }
}
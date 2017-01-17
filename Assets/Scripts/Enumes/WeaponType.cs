using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum WeaponType {
	IRON_SWORD, SQUIRE_SWORD, NOBLE_SWORD,//Fighter
    MAGIC_WAND,//Mage
    IRON_DAGGERS,//Thief
    WOODEN_STAFF//Healer
}

public static class WeaponDescriptor {

	private static Dictionary<WeaponType, string> descript;

	public static string name (this WeaponType type) {
		switch (type) {
			case WeaponType.IRON_SWORD: return "Железный меч";
			case WeaponType.SQUIRE_SWORD: return "Меч оруженосца";
			case WeaponType.NOBLE_SWORD: return "Благородный меч";
            case WeaponType.MAGIC_WAND: return "Магическая палочка";
            case WeaponType.IRON_DAGGERS: return "Железные кинжалы";
            case WeaponType.WOODEN_STAFF: return "Деревяный посох";
            default: Debug.Log("Unknown weapon type"); return "Unknown";
		}
	}

	public static int damage (this WeaponType type) {
		switch (type) {
			case WeaponType.IRON_SWORD: return 20;
			case WeaponType.SQUIRE_SWORD: return 30;
            case WeaponType.NOBLE_SWORD: return 40;
            case WeaponType.MAGIC_WAND: return 20;
            case WeaponType.IRON_DAGGERS: return 20;
            case WeaponType.WOODEN_STAFF: return 20;
			default: Debug.Log("Unknown weapon type"); return 0;
		}
	}
	
	public static int cost (this WeaponType type) {
		switch (type) {
			case WeaponType.IRON_SWORD: return 200;
			case WeaponType.SQUIRE_SWORD: return 300;
            case WeaponType.NOBLE_SWORD: return 400;
            case WeaponType.MAGIC_WAND: return 200;
            case WeaponType.IRON_DAGGERS: return 200;
            case WeaponType.WOODEN_STAFF: return 200;
			default: Debug.Log("Unknown weapon type"); return 0;
		}
	}

    public static HeroType user (this WeaponType type) {
        switch (type) {
            case WeaponType.IRON_SWORD:
            case WeaponType.SQUIRE_SWORD:
            case WeaponType.NOBLE_SWORD:
                return HeroType.ALIKA;
            case WeaponType.MAGIC_WAND:
                return HeroType.LIARA;
            case WeaponType.IRON_DAGGERS:
                return HeroType.KATE;
            case WeaponType.WOODEN_STAFF:
                return HeroType.VICTORIA;
            default:
                Debug.Log("Unknown weapon type: " + type);
                return HeroType.ALIKA;
        }
    }

    public static float volume (this WeaponType type) {
        return 0;
    }

    public static bool isArtefact (this WeaponType type) {
        return false;
    }

	public static string description (this WeaponType type) {
		if (descript == null) {
			descript = new Dictionary<WeaponType, string>();
			descript.Add(WeaponType.IRON_SWORD, Utils.breakLines("Неплохо справляется с вырубанием кустарников за домом, в бою же не столь хорош.", Vars.itemTypeCharsInLine));
			descript.Add(WeaponType.SQUIRE_SWORD, Utils.breakLines("Такой меч хорош для боя с небронированным противником, на тролля лезть с ним не стоит.", Vars.itemTypeCharsInLine));
			descript.Add(WeaponType.NOBLE_SWORD, Utils.breakLines("Меч любимый вельможами и другим знатным людом. Хороший урон и выглядит красиво.", Vars.itemTypeCharsInLine));
            descript.Add(WeaponType.IRON_DAGGERS, "");
            descript.Add(WeaponType.MAGIC_WAND, "");
            descript.Add(WeaponType.WOODEN_STAFF, "");
        }
		return descript[type];
	}
}
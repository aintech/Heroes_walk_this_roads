using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum WeaponType {
	IRON_SWORD, SQUIRE_SWORD, NOBLE_SWORD
}

public static class WeaponDescriptor {

	private static Dictionary<WeaponType, string> descript;

	public static string name (this WeaponType type) {
		switch (type) {
			case WeaponType.IRON_SWORD: return "Железный меч";
			case WeaponType.SQUIRE_SWORD: return "Меч оруженосца";
			case WeaponType.NOBLE_SWORD: return "Благородный меч";
            default: Debug.Log("Unknown weapon type"); return "Unknown";
		}
	}

	public static int damage (this WeaponType type) {
		switch (type) {
			case WeaponType.IRON_SWORD: return 20;
			case WeaponType.SQUIRE_SWORD: return 30;
			case WeaponType.NOBLE_SWORD: return 40;
			default: Debug.Log("Unknown weapon type"); return 0;
		}
	}
	
	public static int cost (this WeaponType type) {
		switch (type) {
			case WeaponType.IRON_SWORD: return 200;
			case WeaponType.SQUIRE_SWORD: return 300;
			case WeaponType.NOBLE_SWORD: return 400;
			default: Debug.Log("Unknown weapon type"); return 0;
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
        }
		return descript[type];
	}
}
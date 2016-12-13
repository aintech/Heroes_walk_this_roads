using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ArmorType {
	LEATHER, IRON, STEEL
}

public static class ArmorDescriptor {

	private static Dictionary<ArmorType, string> descript;

	public static string name (this ArmorType type) {
		switch (type) {
			case ArmorType.LEATHER: return "Кожаный доспех";
			case ArmorType.IRON: return "Железная броня";
			case ArmorType.STEEL: return "Стальная кираса";
			default: Debug.Log("Неизвестный тип доспеха"); return "";
		}
	}

	public static string description (this ArmorType type) {
		if (descript == null) {
			descript = new Dictionary<ArmorType, string>();
			descript.Add(ArmorType.LEATHER, Utils.breakLines("Простая куртка из толстой кожи, защищает разве, что от острой палки, да брошеного камня.", Vars.itemTypeCharsInLine));
			descript.Add(ArmorType.IRON, Utils.breakLines("Доспех из железа - тяжелый и неудобный, но и защищает неплохо.", Vars.itemTypeCharsInLine));
			descript.Add(ArmorType.STEEL, Utils.breakLines("Кираса из стали прекрасно защищает от урона, и на теле неплохо сидит.", Vars.itemTypeCharsInLine));
		}
		return descript[type];
	}

	public static int armorClass (this ArmorType type) {
		switch (type) {
			case ArmorType.LEATHER: return 5;
			case ArmorType.IRON: return 10;
			case ArmorType.STEEL: return 20;
			default: Debug.Log("Неизвестный тип доспеха"); return 0;
		}
	}

	public static int cost (this ArmorType type) {
		switch (type) {
			case ArmorType.LEATHER: return 100;
			case ArmorType.IRON: return 200;
			case ArmorType.STEEL: return 300;
			default: Debug.Log("Неизвестный тип доспеха"); return 0;
		}
	}

    public static float volume (this ArmorType type) {
        return 0;
    }

	public static bool isArtefact (this ArmorType type) {
		return false;
	}
}
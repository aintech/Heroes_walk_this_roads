using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum HelmetType {
	LEATHER, IRON, STEEL
}

public static class HelmetDescriptor {

	private static Dictionary<HelmetType, string> descript;

	public static string name (this HelmetType type) {
		switch (type) {
			case HelmetType.LEATHER: return "Кожаный шлем";
			case HelmetType.IRON: return "Железный шлем";
			case HelmetType.STEEL: return "Стальной шлем";
			default: Debug.Log("Неизвестный тип шлема"); return "";
		}
	}
	
	public static string description (this HelmetType type) {
		if (descript == null) {
			descript = new Dictionary<HelmetType, string>();
			descript.Add(HelmetType.LEATHER, Utils.breakLines("Поможет защитить голову от прилетевшего... ну яблока, например.", Vars.itemTypeCharsInLine));
			descript.Add(HelmetType.IRON, Utils.breakLines("Таскать железный шлем на голове не особо удобно, но остаться без головы ещё неудобнее.", Vars.itemTypeCharsInLine));
			descript.Add(HelmetType.STEEL, Utils.breakLines("Поможет сохранить голову в бою, осталось не потерять ее в переносном смысле.", Vars.itemTypeCharsInLine));
		}
		return descript[type];
	}

	public static int armorClass (this HelmetType type) {
		switch (type) {
			case HelmetType.LEATHER: return 5;
			case HelmetType.IRON: return 10;
			case HelmetType.STEEL: return 20;
			default: Debug.Log("Неизвестный тип шлема"); return 0;
		}
	}
	
	public static int cost (this HelmetType type) {
		switch (type) {
			case HelmetType.LEATHER: return 100;
			case HelmetType.IRON: return 200;
			case HelmetType.STEEL: return 300;
			default: Debug.Log("Неизвестный тип шлема"); return 0;
		}
	}
	
    public static float volume (this HelmetType type) {
        return 0;
    }

	public static bool isArtefact (this HelmetType type) {
		return false;
	}
}
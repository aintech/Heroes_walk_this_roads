using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ShieldType {
	WOOD, IRON, STEEL
}

public static class ShieldDescriptor {

	private static Dictionary<ShieldType, string> descript;

	public static string name (this ShieldType type) {
		switch (type) {
			case ShieldType.WOOD: return "Деревянный щит";
			case ShieldType.IRON: return "Железный щит";
			case ShieldType.STEEL: return "Стальной щит";
			default: Debug.Log("Unknown shield type: " + type); return "";
		}
	}

	public static string description (this ShieldType type) {
		if (descript == null) {
			descript = new Dictionary<ShieldType, string>();
			descript.Add(ShieldType.WOOD, Utils.breakLines("Останавливает дротики, камни... кинжалом его проткнуть тоже проблематично.", Vars.itemTypeCharsInLine));
			descript.Add(ShieldType.IRON, Utils.breakLines("Железные полоски способны защитить от стрел, да и топор врядли разрубит его пополам.", Vars.itemTypeCharsInLine));
			descript.Add(ShieldType.STEEL, Utils.breakLines("Щит из дерева, с большой стальной пластиной, им можно закрыться от шквала стрел, топора минотавра и иных вредных для здоровья вещей.", Vars.itemTypeCharsInLine));
		}
		return descript[type];
	}

	public static int armorClass (this ShieldType type) {
		switch (type) {
			case ShieldType.WOOD: return 5;
			case ShieldType.IRON: return 10;
			case ShieldType.STEEL: return 20;
			default: Debug.Log("Unknown shield type: " + type); return 0;
		}
	}

	public static int cost (this ShieldType type) {
		switch (type) {
			case ShieldType.WOOD: return 100;
			case ShieldType.IRON: return 200;
			case ShieldType.STEEL: return 300;
		default: Debug.Log("Unknown shield type: " + type); return 0;
		}
	}
	
    public static float volume (this ShieldType type) {
        return 0;
    }

	public static bool isArtefact (this ShieldType type) {
		return false;
	}
}
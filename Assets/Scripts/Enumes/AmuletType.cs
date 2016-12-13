using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum AmuletType {
	RABBIT_LEG
}

public static class AmuletDescriptor {
	
	private static Dictionary<AmuletType, string> descript;

	public static string name (this AmuletType type) {
		switch (type) {
			case AmuletType.RABBIT_LEG: return "Кроличья лапка";
			default: Debug.Log("Unknown amulet type: " + type); return "";
		}
	}
	
	public static string description (this AmuletType type) {
		if (descript == null) {
			descript = new Dictionary<AmuletType, string>();
			descript.Add(AmuletType.RABBIT_LEG, Utils.breakLines("Довольно милая и пушистая лапка кролика. Говорят приносит удачу.", Vars.itemTypeCharsInLine));
		}
		return descript[type];
	}

	public static int cost (this AmuletType type) {
		switch (type) {
			case AmuletType.RABBIT_LEG: return 100;
			default: Debug.Log("Неизвестный тип ботинок"); return 0;
		}
	}

    public static float volume (this AmuletType type) {
        return 0;
    }

	public static bool isArtefact (this AmuletType type) {
		return false;
	}
}
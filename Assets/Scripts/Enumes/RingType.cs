using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum RingType {
	COPPER
}

public static class RingDescriptor {
	
	private static Dictionary<RingType, string> descript;

	public static string name (this RingType type) {
		switch (type) {
			case RingType.COPPER: return "Медное кольцо";
			default: Debug.Log("Unknown ring type: " + type); return "";
		}
	}
	
	public static string description (this RingType type) {
		if (descript == null) {
			descript = new Dictionary<RingType, string>();
			descript.Add(RingType.COPPER, Utils.breakLines("Медное колечко с гравировкой", Vars.itemTypeCharsInLine));
		}
		return descript[type];
	}

	public static int cost (this RingType type) {
		switch (type) {
			case RingType.COPPER: return 100;
			default: Debug.Log("Unknown ring type: " + type); return 0;
		}
	}

    public static float volume (this RingType type) {
        return 0;
    }

	public static bool isArtefact (this RingType type) {
		return false;
	}
}
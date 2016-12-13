using UnityEngine;
using System.Collections;

public enum MaterialType {
	STEEL_BAR, WOOD_STICK
}

public static class MaterialDescriptor {
	
	public static string name (this MaterialType type) {
		switch (type) {
			case MaterialType.STEEL_BAR: return "Стальной брусок";
			case MaterialType.WOOD_STICK: return "Кусок дерева";
			default: Debug.Log("Unknown material type: " + type); return "";
		}
	}

	public static string description (this MaterialType type) {
		switch (type) {
			case MaterialType.STEEL_BAR: return "Стальной слиток";
			case MaterialType.WOOD_STICK: return "Деревяный брусок";
			default: Debug.Log("Unknown material type: " + type); return "";
		}
	}

	public static int cost (this MaterialType type) {
		return 40;
	}

    public static float volume (this MaterialType type) {
        return 0;
    }
}
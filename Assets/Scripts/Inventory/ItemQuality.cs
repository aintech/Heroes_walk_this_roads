using UnityEngine;
using System.Collections;

public enum ItemQuality {
	COMMON, GOOD, SUPERIOR, RARE, UNIQUE, ARTEFACT
}

public static class ItemQualityDescriptor {
	public static string getName (this ItemQuality quality) {
		switch (quality) {
			case ItemQuality.COMMON: return "Обычный";
			case ItemQuality.GOOD: return "Качественный";
			case ItemQuality.SUPERIOR: return "Отличный";
			case ItemQuality.RARE: return "Редкий";
			case ItemQuality.UNIQUE: return "Уникальный";
			case ItemQuality.ARTEFACT: return "Артефакт";
			default: Debug.Log ("Unknown quality: " + quality); return "";
		}
	}
}
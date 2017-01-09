using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ItemQuality {
	COMMON, GOOD, SUPERIOR, RARE, UNIQUE, ARTEFACT
}

public static class ItemQualityDescriptor {

    private static Dictionary<ItemQuality, string> richNames;

	public static string name (this ItemQuality quality) {
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

    public static string richName (this ItemQuality quality) {
        if (richNames == null) {
            richNames = new Dictionary<ItemQuality, string>();
            richNames.Add(ItemQuality.GOOD, "<color=#B0C3D9FF>" + ItemQuality.GOOD.name() + "</color>");
            richNames.Add(ItemQuality.SUPERIOR, "<color=#5E98D9>" + ItemQuality.SUPERIOR.name() + "</color>");
            richNames.Add(ItemQuality.RARE, "<color=#8847FFFF>" + ItemQuality.RARE.name() + "</color>");
            richNames.Add(ItemQuality.UNIQUE, "<color=#ADE55CFF>" + ItemQuality.UNIQUE.name() + "</color>");
            richNames.Add(ItemQuality.ARTEFACT, "<color=#EB4B4BFF>" + ItemQuality.ARTEFACT.name() + "</color>");
        }
        return richNames[quality];
    }
}
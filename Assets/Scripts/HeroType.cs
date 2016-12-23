﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum HeroType {
	ALIKA, VICTORIA, LIARA, KATE//, ROKSANA, MARIKA
}

public static class HeroDescriptor {

    private static Dictionary<HeroType, HeroActionType[]> heroActionsMap;

	public static string name (this HeroType type) {
		switch (type) {
			case HeroType.ALIKA: return "Алика";
			case HeroType.VICTORIA: return "Викотрия";
			case HeroType.LIARA: return "Лиара";
			case HeroType.KATE: return "Кейт";
			default: Debug.Log("Unknown charakter type"); return "";
		}
	}

	public static HeroType nameToType (string name) {
		switch (name) {
			case "alika": return HeroType.ALIKA;
			case "victoria": return HeroType.VICTORIA;
			case "liara": return HeroType.LIARA;
			case "kate": return HeroType.KATE;
			default: Debug.Log("Unknown name: " + name); return HeroType.ALIKA;
		}
	}

	public static int strenght (this HeroType type) {
		return 10;
	}

	public static int endurance (this HeroType type) {
		return 10;
	}

	public static int agility (this HeroType type) {
        switch (type) {
            case HeroType.KATE: return 10;
            case HeroType.LIARA: return 11;
            case HeroType.VICTORIA: return 12;
            case HeroType.ALIKA: return 13;
            default: return 1;
        }
	}

    public static HeroActionType[] heroActions (this HeroType type) {
        if (heroActionsMap == null) {
            heroActionsMap = new Dictionary<HeroType, HeroActionType[]>();
            heroActionsMap.Add(HeroType.ALIKA, new HeroActionType[]{HeroActionType.ATTACK, HeroActionType.GUARD});
            heroActionsMap.Add(HeroType.KATE, new HeroActionType[]{HeroActionType.ATTACK, HeroActionType.GUARD});
            heroActionsMap.Add(HeroType.LIARA, new HeroActionType[]{HeroActionType.ATTACK, HeroActionType.GUARD});
            heroActionsMap.Add(HeroType.VICTORIA, new HeroActionType[]{HeroActionType.ATTACK, HeroActionType.GUARD});
        }
        return heroActionsMap[type];
    }
}
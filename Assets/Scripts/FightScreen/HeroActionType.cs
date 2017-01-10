using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public enum HeroActionType {
    ATTACK, GUARD, HEAL
}

public static class HeroActionDescription {
    
	private static Dictionary<HeroActionType, Dictionary<ElementType, int>> elementsCostMap;

    public static string name (this HeroActionType type) {
        switch (type) {
            case HeroActionType.ATTACK: return "Атака оружием";
            case HeroActionType.GUARD: return "Защитная стойка";
            case HeroActionType.HEAL: return "Исцеление";
            default: Debug.Log("Unknown action type: " + type); return "";
        }
    }

	public static TargetType targetType (this HeroActionType type) {
		switch (type) {
			case HeroActionType.ATTACK: return TargetType.ENEMY;
			case HeroActionType.GUARD: return TargetType.SELF;
			case HeroActionType.HEAL: return TargetType.ALLY;
			default: Debug.Log("Unknown target type: " + type); return TargetType.SELF;
		}
	}

	public static Dictionary<ElementType, int> elementsCost (this HeroActionType type) {
		if (elementsCostMap == null) {
			elementsCostMap = new Dictionary<HeroActionType, Dictionary<ElementType, int>>();
			foreach (HeroActionType aType in Enum.GetValues(typeof (HeroActionType))) {
				elementsCostMap.Add(aType, new Dictionary<ElementType, int>());
			}
			elementsCostMap[HeroActionType.HEAL].Add(ElementType.WATER, 2);
		}
		return elementsCostMap[type];
	}

    public static int value (this HeroActionType type) {
        switch (type) {
            case HeroActionType.GUARD: return 25;
            case HeroActionType.HEAL: return 50;
            default: return 0;
        }
    }
}

public enum TargetType {
	ENEMY, ALLY, SELF
}

public static class TargetTypeDescription {
    public static string name (this TargetType type) {
        switch (type) {
            case TargetType.ENEMY: return "Противник";
            case TargetType.ALLY: return "Союзник";
            case TargetType.SELF: return "На себя";
            default: Debug.Log("Unknown target type: " + type); return "";
        }
    }
}
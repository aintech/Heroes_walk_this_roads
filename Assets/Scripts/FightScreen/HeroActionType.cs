using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public enum HeroActionType {
    ATTACK, GUARD, HEAL
}

public static class HeroActionDescription {

	private static Dictionary<HeroActionType, Dictionary<ElementType, int>> elementsCostMap;

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
}

public enum TargetType {
	ENEMY, ALLY, SELF
}
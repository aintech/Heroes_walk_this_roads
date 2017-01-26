using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public enum HeroActionType {
    SWORD_SWING, HEAVY_GUARD, CRUSHING,//Fighter
    MAGIC_ARROW, INVULNERABILITY_SPHERE, FIRE_WALL,//Mage
    STAFF_ATTACK, SACRIFICE, HEAL,//Healer
    DAGGERS_CUT, DODGE, DUST_IN_EYES//Thief
}

public static class HeroActionDescription {
    
	private static Dictionary<HeroActionType, Dictionary<ElementType, int>> elementsCostMap;

    public static string name (this HeroActionType type) {
        switch (type) {
            case HeroActionType.SWORD_SWING: return "Взмах мечом";
            case HeroActionType.HEAVY_GUARD: return "Глухая оборона";
            case HeroActionType.CRUSHING: return "Рубилово";
                
            case HeroActionType.MAGIC_ARROW: return "Магическая стрела";
            case HeroActionType.INVULNERABILITY_SPHERE: return "Сфера неуязвимости";
            case HeroActionType.FIRE_WALL: return "Стена огня";
                
            case HeroActionType.STAFF_ATTACK: return "Удар посохом";
            case HeroActionType.SACRIFICE: return "Жертва";
            case HeroActionType.HEAL: return "Исцеление";

            case HeroActionType.DAGGERS_CUT: return "Порез кинжалом";
            case HeroActionType.DODGE: return "Увертливость";
            case HeroActionType.DUST_IN_EYES: return "Пыль в глаза";
            default: Debug.Log("Unknown action type: " + type); return "";
        }
    }

	public static TargetType targetType (this HeroActionType type) {
        switch (type) {
            case HeroActionType.HEAVY_GUARD:
            case HeroActionType.INVULNERABILITY_SPHERE:
            case HeroActionType.SACRIFICE:
            case HeroActionType.DODGE:
                return TargetType.SELF;

            case HeroActionType.HEAL:
                return TargetType.ALLY;

            case HeroActionType.SWORD_SWING:
            case HeroActionType.CRUSHING:
            case HeroActionType.MAGIC_ARROW:
            case HeroActionType.STAFF_ATTACK:
            case HeroActionType.DAGGERS_CUT:
            case HeroActionType.DUST_IN_EYES:
                return TargetType.ENEMY;

            case HeroActionType.FIRE_WALL:
                return TargetType.ENEMIES;

			default: Debug.Log("Unknown target type: " + type); return TargetType.SELF;
		}
	}

	public static Dictionary<ElementType, int> elementsCost (this HeroActionType type) {
		if (elementsCostMap == null) {
			elementsCostMap = new Dictionary<HeroActionType, Dictionary<ElementType, int>>();
			foreach (HeroActionType aType in Enum.GetValues(typeof (HeroActionType))) {
				elementsCostMap.Add(aType, new Dictionary<ElementType, int>());
			}
//            elementsCostMap[HeroActionType.CRUSHING].Add(ElementType.FIRE, 2);
//            elementsCostMap[HeroActionType.FIRE_WALL].Add(ElementType.FIRE, 2);
//            elementsCostMap[HeroActionType.FIRE_WALL].Add(ElementType.EARTH, 1);
//			elementsCostMap[HeroActionType.HEAL].Add(ElementType.WATER, 2);
//            elementsCostMap[HeroActionType.DUST_IN_EYES].Add(ElementType.EARTH, 1);
//            elementsCostMap[HeroActionType.DUST_IN_EYES].Add(ElementType.AIR, 1);

		}
		return elementsCostMap[type];
	}

//    public static int value (this HeroActionType type) {
//        switch (type) {
//            case HeroActionType.GUARD: return 25;
//            case HeroActionType.HEAL: return 50;
//            default: return 0;
//        }
//    }
}

public enum TargetType {
    SELF, ALLY, ALLIES, ENEMY, ENEMIES
}

public static class TargetTypeDescription {
    public static string name (this TargetType type) {
        switch (type) {
            case TargetType.SELF: return "На себя";
            case TargetType.ALLY: return "Союзник";
            case TargetType.ALLIES: return "Все союзники";
            case TargetType.ENEMY: return "Противник";
            case TargetType.ENEMIES: return "Все противники";
            default: Debug.Log("Unknown target type: " + type); return "";
        }
    }
}